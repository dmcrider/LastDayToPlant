using StardewModdingAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LastDayToPlant;

/// <summary>
/// API interface for Json Assets mod integration.
/// Must be a top-level public interface so SMAPI can map the mod-provided API.
/// </summary>
public interface IJsonAssetsApi
{
    List<string> GetCropNames();
    int[] GetCropGrowthStageDays(string name);
    IList<string> GetCropSeasons(string name);
}

public class Crop
{
    public string OriginalName { get; set; } // base name for localization
    public string Name { get; set; }
    public int DaysToGrow { get; set; }
    public List<Season> Seasons { get; set; }
    public int DaysToGrowIrrigated { get; set; } = 0;
    public int AvailableYear { get; set; } = 1;
    public bool GingerIsland { get; set; } = false;

    public string Message { get; private set; }
    public string MessageSpeedGro { get; private set; }
    public string MessageDelxueSpeedGro { get; private set; }
    public string MessageHyperSpeedGro { get; private set; }

    public Crop(string name, int daysToGrow)
    {
        OriginalName = name;
        Name = name;
        DaysToGrow = daysToGrow;
        Seasons = new List<Season>();
    }

    public Crop() { }

    public bool IsLastGrowSeason(Season season)
    {
        if (Seasons == null || Seasons.Count == 0) return false;

        var seasons = Seasons.OrderByDescending(x => x);
        return seasons.FirstOrDefault() == season;
    }

    public void Localize(IModHelper helper)
    {
        // Attempt translation; fall back to original
        var localized = helper.Translation.Get($"crop.{OriginalName.Replace(" ", "")}");
        if (!string.IsNullOrEmpty(localized))
        {
            Name = localized;
        }
        else
        {
            Name = OriginalName;
        }
        PrecomputeMessages();
    }

    private void PrecomputeMessages()
    {
        Message = I18n.Notification_Crop_NoFertilizer(Name);
        MessageSpeedGro = I18n.Notification_Crop_SpeedGro(Name);
        MessageDelxueSpeedGro = I18n.Notification_Crop_DeluxeSpeedGro(Name);
        MessageHyperSpeedGro = I18n.Notification_Crop_HyperSpeedGro(Name);
    }

    private static IJsonAssetsApi _jaApiCache;

    public static List<Crop> LoadAllCrops(IModHelper helper, IMonitor monitor)
    {
        var results = new List<Crop>(256); // initial capacity for optimization

        // 1. GameData/Crops via reflection (typed access would be faster; reflection keeps compatibility)
        try
        {
            var raw = helper.GameContent.Load<object>("GameData/Crops");
            if (raw is IEnumerable enumerable)
            {
                foreach (var entry in enumerable)
                {
                    if (entry == null) continue;
                    var displayName = GetPropRef<string>(entry, "DisplayName") ?? GetPropRef<string>(entry, "Id") ?? "?";
                    var seasonList = GetPropRef<IList>(entry, "Seasons") ?? new List<string>();
                    var phaseIntList = GetPropRef<IList>(entry, "PhaseDays")
                                        ?? GetPropRef<IList>(entry, "Phases")
                                        ?? GetPropRef<IList>(entry, "GrowthStageDays")
                                        ?? new List<int>();
                    var minHarvestYear = GetPropInt(entry, "MinHarvestYear") ?? 1;
                    var canGrowIsland = GetPropBool(entry, "CanGrowInIslandLocations") ?? false;

                    var seasonsParsed = new List<Season>(4);
                    foreach (var s in seasonList)
                    {
                        var str = s?.ToString();
                        if (string.IsNullOrEmpty(str)) continue;

                        if (Enum.TryParse<Season>(str, true, out var seasonEnum) && !seasonsParsed.Contains(seasonEnum))
                        {
                            seasonsParsed.Add(seasonEnum);
                        }
                    }

                    int totalDays = 0;
                    foreach (var p in phaseIntList)
                    {
                        if (p == null) continue;

                        if (int.TryParse(p.ToString(), out var d))
                        {
                            totalDays += d;
                        }
                    }

                    if (totalDays <= 0 || seasonsParsed.Count == 0) continue;

                    results.Add(new Crop(displayName, totalDays)
                    {
                        Seasons = seasonsParsed,
                        AvailableYear = minHarvestYear,
                        GingerIsland = canGrowIsland
                    });
                }
            }
        }
        catch (Exception ex)
        {
            monitor.Log($"GameData/Crops reflection load failed: {ex.Message}", LogLevel.Trace);
        }

        // 2. Legacy Data/Crops fallback for mods that still patch that asset
        try
        {
            var legacy = helper.GameContent.Load<Dictionary<int, string>>("Data/Crops");
            foreach (var kv in legacy)
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) continue;

                var parts = kv.Value.Split('/');
                if (parts.Length < 2) continue;

                var seasonsRaw = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var seasonsParsed = new List<Season>(seasonsRaw.Length);
                foreach (var s in seasonsRaw)
                {
                    if (Enum.TryParse<Season>(s, true, out var seasonEnum) && !seasonsParsed.Contains(seasonEnum))
                    {
                        seasonsParsed.Add(seasonEnum);
                    }
                }

                var phaseDaysRaw = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                int totalPhaseDays = 0;
                foreach (var p in phaseDaysRaw)
                {
                    if (int.TryParse(p, out var d)) totalPhaseDays += d;
                }

                if (totalPhaseDays <= 0 || seasonsParsed.Count == 0) continue;

                string seedName = ResolveSeedName(helper, kv.Key) ?? $"Seed {kv.Key}";
                if (results.Exists(r => string.Equals(r.OriginalName, seedName, StringComparison.OrdinalIgnoreCase))) continue;

                results.Add(new Crop(seedName, totalPhaseDays) { Seasons = seasonsParsed });
            }
        }
        catch (Exception ex)
        {
            monitor.Log($"Data/Crops load failed: {ex.Message}", LogLevel.Trace);
        }

        // 3. Json Assets merge (cache API)
        try
        {
            // Guard: only try to get JA API if the mod is loaded
            if (helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                _jaApiCache ??= helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
                var ja = _jaApiCache;
                if (ja != null)
                {
                    foreach (var name in ja.GetCropNames())
                    {
                        int[] phases = ja.GetCropGrowthStageDays(name) ?? Array.Empty<int>();
                        int phaseSum = 0; foreach (var v in phases) phaseSum += v;
                        var seasonStrings = ja.GetCropSeasons(name) ?? Array.Empty<string>();
                        var seasonsParsed = new List<Season>(seasonStrings.Count);
                        foreach (var s in seasonStrings)
                        {
                            if (Enum.TryParse<Season>(s, true, out var seasonEnum) && !seasonsParsed.Contains(seasonEnum))
                            {
                                seasonsParsed.Add(seasonEnum);
                            }
                        }

                        if (phaseSum <= 0 || seasonsParsed.Count == 0) continue;

                        var existing = results.Find(c => string.Equals(c.OriginalName, name, StringComparison.OrdinalIgnoreCase));
                        if (existing != null)
                        {
                            existing.DaysToGrow = phaseSum;
                            existing.Seasons = seasonsParsed;
                        }
                        else
                        {
                            results.Add(new Crop(name, phaseSum) { Seasons = seasonsParsed });
                        }
                    }
                }
                else
                {
                    // API failed to map; log and continue without JA data
                    monitor.Log("Json Assets API not available despite mod being loaded.", LogLevel.Trace);
                }
            }
            else
            {
                // Mod not installed; log at trace and skip JA
                monitor.Log("Json Assets not loaded; skipping JA crop merge.", LogLevel.Trace);
            }
        }
        catch (Exception ex)
        {
            monitor.Log($"Json Assets merge failed: {ex.Message}", LogLevel.Warn);
        }

        // Final filter & sort
        results.RemoveAll(c => c.DaysToGrow <= 0 || c.Seasons == null || c.Seasons.Count == 0);
        results.Sort((a, b) => string.Compare(a.OriginalName, b.OriginalName, StringComparison.OrdinalIgnoreCase));
        return results;
    }

    private static T GetPropRef<T>(object obj, string name) where T : class
    {
        if (obj == null) return null;

        var type = obj.GetType();
        var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null) return null;

        try
        {
            return prop.GetValue(obj) as T;
        }
        catch
        {
            return null;
        }
    }
    private static int? GetPropInt(object obj, string name)
    {
        if (obj == null) return null;

        var type = obj.GetType();
        var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null) return null;

        try
        {
            var val = prop.GetValue(obj);
            if (val == null) return null;

            if (val is int i) return i;

            if (int.TryParse(val.ToString(), out var parsed)) return parsed;

            return null;
        }
        catch
        {
            return null;
        }
    }
    private static bool? GetPropBool(object obj, string name)
    {
        if (obj == null) return null;

        var type = obj.GetType();
        var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null) return null;

        try
        {
            var val = prop.GetValue(obj);
            if (val == null) return null;

            if (val is bool b) return b;

            if (bool.TryParse(val.ToString(), out var parsed)) return parsed;

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static string ResolveSeedName(IModHelper helper, int seedObjectId)
    {
        try
        {
            var objects = helper.GameContent.Load<Dictionary<int, string>>("Data/ObjectInformation");
            if (objects.TryGetValue(seedObjectId, out var raw))
            {
                var firstSlash = raw.IndexOf('/');
                if (firstSlash > 0)
                {
                    return raw[..firstSlash];
                }
            }
        }
        catch { }
        return null;
    }

    public override string ToString() => Name;
}
