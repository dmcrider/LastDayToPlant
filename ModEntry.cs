using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
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

        ReloadCrops(localize: true); // initial localization

        MyHelper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        MyHelper.Events.Content.LocaleChanged += Content_LocaleChanged;
        MyHelper.Events.Content.AssetsInvalidated += Content_AssetsInvalidated;
    }

    private void ReloadCrops(bool localize)
    {
        AllCrops.Clear();
        var loaded = Crop.LoadAllCrops(MyHelper, Monitor);
        foreach (var crop in loaded)
        {
            if (localize)
            {
                crop.Localize(MyHelper);
            }
            AllCrops.Add(crop);
        }
        Monitor.Log($"Reloaded {AllCrops.Count} crops.", LogLevel.Trace);
    }

    private void Content_AssetsInvalidated(object sender, AssetsInvalidatedEventArgs e)
    {
        foreach (var asset in e.Names)
        {
            var name = asset.Name;
            if (name == "GameData/Crops" || name == "Data/Crops" || name == "Data/ObjectInformation")
            {
                ReloadCrops(localize: false);
                break;
            }
        }
    }

    private void Content_LocaleChanged(object sender, LocaleChangedEventArgs e)
    {
        foreach (var crop in AllCrops)
        {
            crop.Localize(MyHelper);
        }
    }

    private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
    {
        FarmingSkills.IsAgriculturist = false;
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (farmer.IsMainPlayer && farmer.getProfessionForSkill(Farmer.farmingSkill, 10) == Farmer.agriculturist)
            {
                FarmingSkills.IsAgriculturist = true;
                break;
            }
        }

        int currentDay = Game1.dayOfMonth;
        int currentYear = Game1.year;
        string currentSeasonStr = Game1.currentSeason;
        Season? seasonEnum = currentSeasonStr switch
        {
            "spring" => Season.spring,
            "summer" => Season.summer,
            "fall" => Season.fall,
            "winter" => Season.winter,
            _ => null
        };
        if (seasonEnum.HasValue)
        {
            ShowCrops(seasonEnum.Value, currentDay, currentYear);
        }
    }

    private void ShowCrops(Season season, int day, int year)
    {
        int tomorrowDay = day == DaysInAMonth ? 1 : day + 1;
        bool tomorrowIsWednesday = ((tomorrowDay - 1) % 7) == 2; // approximation

        foreach (var crop in AllCrops)
        {
            if (crop.Seasons == null || crop.Seasons.Count == 0) continue;
            if (!crop.IsLastGrowSeason(season) || year < crop.AvailableYear) continue;
            if (!MyConfig.ShowGingerIslandCrops && crop.GingerIsland) continue;

            // Always show base
            ProcessCropForFertilizer(crop, Fertilizer.None, crop.Message, day, tomorrowDay, tomorrowIsWednesday);
            if (MyConfig.ShowSpeedGro) ProcessCropForFertilizer(crop, Fertilizer.SpeedGro, crop.MessageSpeedGro, day, tomorrowDay, tomorrowIsWednesday);
            if (MyConfig.ShowDeluxeSpeedGro) ProcessCropForFertilizer(crop, Fertilizer.DeluxeSpeedGro, crop.MessageDelxueSpeedGro, day, tomorrowDay, tomorrowIsWednesday);
            if (MyConfig.ShowHyperSpeedGro) ProcessCropForFertilizer(crop, Fertilizer.HyperSpeedGro, crop.MessageHyperSpeedGro, day, tomorrowDay, tomorrowIsWednesday);
        }
    }

    private static void ProcessCropForFertilizer(Crop crop, double fertilizerFactor, string baseMessage, int currentDay, int tomorrowDay, bool tomorrowIsWednesday)
    {
        if (string.IsNullOrEmpty(baseMessage)) return;

        int daysToMatureBoosted = CalculateActualGrowRate(crop, fertilizerFactor);
        if (daysToMatureBoosted + currentDay == DaysInAMonth)
        {
            Game1.addHUDMessage(new HUDMessage(baseMessage, HUDMessage.newQuest_type));
        }
        if (tomorrowIsWednesday && daysToMatureBoosted + tomorrowDay == DaysInAMonth)
        {
            string msg = baseMessage;
            if (fertilizerFactor == Fertilizer.None) msg = I18n.Notification_Crop_NoFertilizerWednesday(crop.Name);
            else if (fertilizerFactor == Fertilizer.SpeedGro) msg = I18n.Notification_Crop_SpeedGroWednesday(crop.Name);
            else if (fertilizerFactor == Fertilizer.DeluxeSpeedGro) msg = I18n.Notification_Crop_DeluxeSpeedGroWednesday(crop.Name);
            else if (fertilizerFactor == Fertilizer.HyperSpeedGro) msg = I18n.Notification_Crop_HyperSpeedGroWednesday(crop.Name);
            Game1.addHUDMessage(new HUDMessage(msg, HUDMessage.newQuest_type));
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

public enum Season { spring, summer, fall, winter }