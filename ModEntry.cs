using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LastDayToPlant;

public class ModEntry : Mod
{
    private const int DaysInAMonth = 28;
    private IModHelper MyHelper;
    private ModConfig MyConfig;
    private readonly List<Crop> AllCrops = new();

    public override void Entry(IModHelper helper)
    {
        MyHelper = helper;
        I18n.Init(helper.Translation);
        MyConfig = MyHelper.ReadConfig<ModConfig>();

        LoadBaseGameCrops();
        LoadModCrops();

        MyHelper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        MyHelper.Events.Content.LocaleChanged += Content_LocaleChanged;
    }

    private void Content_LocaleChanged(object sender, LocaleChangedEventArgs e)
    {
        foreach (var crop in AllCrops)
        {
            var baseName = crop.Name;
            crop.Localize(MyHelper, baseName);
        }
    }

    private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
    {
        // Check if the farmer has the agriculturist profession
        var farmers = Game1.getAllFarmers();

        foreach(var farmer in farmers)
        {
            if(farmer.IsMainPlayer && farmer.getProfessionForSkill(Farmer.farmingSkill,10) == Farmer.agriculturist)
            {
                FarmingSkills.IsAgriculturist = true;
                break;
            }
        }

        // Show any crops that can't be planted after today and still be harvested
        var currentDay = SDate.From(Game1.Date).Day;
        var currentYear = SDate.From(Game1.Date).Year;
        var currentSeason = SDate.From(Game1.Date).Season;
        switch (currentSeason)
        {
            case StardewValley.Season.Spring:
                ShowCrops(Season.spring, currentDay, currentYear);
                break;
            case StardewValley.Season.Summer:
                ShowCrops(Season.summer, currentDay, currentYear);
                break;
            case StardewValley.Season.Fall:
                ShowCrops(Season.fall, currentDay, currentYear);
                break;
            case StardewValley.Season.Winter:
                ShowCrops(Season.winter, currentDay, currentYear);
                break;
            default:
                return;
        }
    }

    private void LoadBaseGameCrops()
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LastDayToPlant.BaseGameCrops.Crops.json"))
        using (var reader = new StreamReader(stream))
        {
            var json = reader.ReadToEnd();
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            var list = JsonSerializer.Deserialize<List<Crop>>(json, options);
            AllCrops.AddRange(list);
        }
    }

    private void LoadModCrops()
    {
        var modsPath = Path.Combine(Constants.GamePath, "Mods");
        var modsPathExists = Directory.Exists(modsPath);

        if (modsPathExists)
        {
            FindAndLoadCrops(modsPath);
        }
    }

    private void FindAndLoadCrops(string path)
    {
        var files = Directory.GetFiles(path, "crop.json", SearchOption.AllDirectories);
        var modName = Functions.StringFunctions.ExtractModNameFromPath(path);
        foreach (var file in files)
        {
            var info = new FileInfo(file);
            if(info.Name == "crop.json")
            {
                AllCrops.Add(Crop.FromModFile(file, modName, Monitor));
            }
        }
    }

    private void ShowCrops(Season season, int day, int year)
    {
        // Some crops are only available in Year 2. We don't want to show them in Year 1
        var filtered = AllCrops.Where(x => x.IsLastGrowSeason(season) && year >= x.AvailableYear);
        if(MyConfig.ShowGingerIslandCrops == false)
        {
            filtered = filtered.Where(x => x.GingerIsland == false);
        }

        // Determine tomorrow's day within the month and whether it's Wednesday
        var tomorrowDay = day == DaysInAMonth ? 1 : day + 1;
        var tomorrowDate = new SDate(Game1.dayOfMonth, Game1.currentSeason, Game1.year).AddDays(1);
        var tomorrowIsWednesday = tomorrowDate.DayOfWeek == DayOfWeek.Wednesday;

        int daysToMatureBoosted;
        foreach (var crop in filtered)
        {
            var fertilizers = new Dictionary<string, Dictionary<double, string>>
            {
                { "None", new Dictionary<double, string> { { Fertilizer.None, crop.Message } } },
                { "SpeedGro", new Dictionary<double, string> { { Fertilizer.SpeedGro, crop.MessageSpeedGro } } },
                { "DeluxeSpeedGro", new Dictionary<double, string> { { Fertilizer.DeluxeSpeedGro, crop.MessageDelxueSpeedGro } } },
                { "HyperSpeedGro", new Dictionary<double, string> { { Fertilizer.HyperSpeedGro, crop.MessageHyperSpeedGro } } }
            };
            foreach (var fertilizerObj in fertilizers)
            {
                var fertilizer = fertilizerObj.Value.Keys.First();
                var message = fertilizerObj.Value[fertilizer];

                daysToMatureBoosted = CalculateActualGrowRate(crop, fertilizer);
                if (daysToMatureBoosted + day == DaysInAMonth)
                {
                    Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type));
                }

                if (tomorrowIsWednesday && daysToMatureBoosted + tomorrowDay == DaysInAMonth)
                {
                    var wednesdayMessage = fertilizerObj.Key switch
                    {
                        "None" => I18n.Notification_Crop_NoFertilizerWednesday(crop.Name),
                        "SpeedGro" => I18n.Notification_Crop_SpeedGroWednesday(crop.Name),
                        "DeluxeSpeedGro" => I18n.Notification_Crop_DeluxeSpeedGroWednesday(crop.Name),
                        "HyperSpeedGro" => I18n.Notification_Crop_HyperSpeedGroWednesday(crop.Name),
                        _ => message
                    };
                    Game1.addHUDMessage(new HUDMessage(wednesdayMessage, HUDMessage.newQuest_type));
                }
            }
        }
    }

    private static int CalculateActualGrowRate(Crop crop, double factor)
    {
        int daysToGrow = crop.DaysToGrowIrrigated > 0 ? crop.DaysToGrowIrrigated : crop.DaysToGrow;
        if (FarmingSkills.IsAgriculturist)
        {
            return (int)(daysToGrow - (daysToGrow * (factor + FarmingSkills.AgriculturistGrowthRate)));
        }

        return (int)(daysToGrow - (daysToGrow * factor));
    }
}

public enum Season
{
    spring,
    summer,
    fall,
    winter
}