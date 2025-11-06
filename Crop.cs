using StardewModdingAPI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace LastDayToPlant;

public class Crop
{
    public string Name { get; set; }
    public int DaysToGrow { get; set; }
    public List<Season> Seasons { get; set; }
    public int DaysToGrowIrrigated { get; set; } = 0;
    public int AvailableYear { get; set; } = 1;
    public bool GingerIsland { get; set; } = false;

    public string Message { get; set; }
    public string MessageSpeedGro { get; set; }
    public string MessageDelxueSpeedGro { get; set; }
    public string MessageHyperSpeedGro { get; set; }


    public Crop(string name, int daysToGrow)
    {
        Name = name;
        DaysToGrow = daysToGrow;
    }

    public Crop() { }

    public bool IsLastGrowSeason(Season season)
    {
        var seasons = Seasons.OrderByDescending(x => x);
        return seasons.First() == season;
    }

    public void Localize(IModHelper helper, string baseName)
    {
        // This one can't be handled by I18n because it's dynamic
        Name = helper.Translation.Get($"crop.{baseName.Replace(" ", "")}");
        // The rest of the messages can though
        Message = I18n.Notification_Crop_NoFertilizer(Name);
        MessageSpeedGro = I18n.Notification_Crop_SpeedGro(Name);
        MessageDelxueSpeedGro = I18n.Notification_Crop_DeluxeSpeedGro(Name);
        MessageHyperSpeedGro = I18n.Notification_Crop_HyperSpeedGro(Name);
    }

    public static Crop FromModFile(string cropFilePath, string modName, IMonitor logger)
    {
        try
        {
            string jsonText;
            try
            {
                jsonText = File.ReadAllText(cropFilePath);
            }
            catch (Exception ex)
            {
                logger.Log($"Failed to read crop file {cropFilePath} for {modName}: {ex.Message}", LogLevel.Error);
                return null;
            }

            JsonObject jsonObject;
            try
            {
                var node = JsonNode.Parse(jsonText);
                jsonObject = node?.AsObject();
                if (jsonObject == null)
                {
                    logger.Log($"Failed to parse JSON in {cropFilePath} for {modName}: document is not an object", LogLevel.Warn);
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Failed to parse JSON in {cropFilePath} for {modName}: {ex.Message}", LogLevel.Warn);
                return null;
            }

            if (!ValidJsonCropData(jsonObject))
            {
                logger.Log($"Failed to load crops for {modName} from file: {cropFilePath} — missing required keys", LogLevel.Warn);
                return null;
            }

            // Name
            var name = jsonObject["Name"]?.GetValue<string>()?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                logger.Log($"Crop `Name` empty or invalid in {cropFilePath}", LogLevel.Warn);
                return null;
            }

            var crop = new Crop
            {
                Name = name,
                Seasons = new List<Season>()
            };

            // Seasons
            var seasonsNode = jsonObject["Seasons"];
            if (seasonsNode is JsonArray seasonsArray)
            {
                foreach (var item in seasonsArray)
                {
                    var s = item?.GetValue<string>()?.Trim();
                    if (string.IsNullOrEmpty(s))
                        continue;

                    if (Enum.TryParse<Season>(s, ignoreCase: true, out var seasonEnum))
                        crop.Seasons.Add(seasonEnum);
                    else
                        logger.Log($"Unknown season '{s}' in {cropFilePath} for {modName}", LogLevel.Trace);
                }
            }
            else
            {
                // Try string form
                var seasonsStr = seasonsNode?.GetValue<string>();
                if (!string.IsNullOrEmpty(seasonsStr))
                {
                    var parts = seasonsStr.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        var s = part.Trim();
                        if (Enum.TryParse<Season>(s, ignoreCase: true, out var seasonEnum))
                            crop.Seasons.Add(seasonEnum);
                        else
                            logger.Log($"Unknown season '{s}' in {cropFilePath} for {modName}", LogLevel.Trace);
                    }
                }
                else
                {
                    logger.Log($"Seasons node is missing or not an array/string in {cropFilePath}", LogLevel.Warn);
                }
            }

            if (crop.Seasons.Count == 0)
            {
                logger.Log($"No valid seasons parsed for {name} in {cropFilePath}", LogLevel.Warn);
                // continue — days might still be useful
            }

            // DaysToGrow — extract first integer from SeedDescription
            var desc = jsonObject["SeedDescription"]?.GetValue<string>() ?? string.Empty;
            if (!string.IsNullOrEmpty(desc))
            {
                var m = System.Text.RegularExpressions.Regex.Match(desc, @"\b(\d+)\b");
                if (m.Success && int.TryParse(m.Groups[1].Value, out var days))
                {
                    crop.DaysToGrow = days;
                }
                else
                {
                    logger.Log($"Could not parse days to grow from SeedDescription for {name} in {cropFilePath}", LogLevel.Trace);
                    crop.DaysToGrow = 0;
                }
            }
            else
            {
                logger.Log($"SeedDescription missing or empty for {name} in {cropFilePath}", LogLevel.Trace);
                crop.DaysToGrow = 0;
            }

            logger.Log($"Loaded crop data for {modName} from file: {cropFilePath}", LogLevel.Info);
            return crop;
        }
        catch (Exception ex)
        {
            // Catch-all should not throw — log and return null to fail gracefully
            try
            {
                logger.Log($"Unexpected error loading crop data from {cropFilePath} for {modName}: {ex.Message}", LogLevel.Error);
            }
            catch { }
            return null;
        }
    }

    private static bool ValidJsonCropData(JsonObject jsonObject)
    {
        if (jsonObject == null)
        {
            return false;
        }
        var requiredKeys = new List<string> { "Name", "Seasons", "SeedDescription" };

        foreach (var key in requiredKeys)
        {
            if (!jsonObject.ContainsKey(key))
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        return Name;
    }
}
