using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LastDayToPlant
{
    public class ModEntry : Mod
    {
        private List<Crop> SpringCrops;
        private List<Crop> SummerCrops;
        private List<Crop> FallCrops;
        private List<Crop> WinterCrops;

        private const int DaysInAMonth = 28;
        private IModHelper MyHelper;
        private ModConfig MyConfig;
        private ModCompat ModCompat;

        public override void Entry(IModHelper helper)
        {
            // Initialize objects we'll need later
            MyHelper = helper;
            MyConfig = MyHelper.ReadConfig<ModConfig>();
            ModCompat = new ModCompat();
            ModCompat.ConfigureMods(MyConfig);

            // Enable the crops
            SpringCrops = SetSpringCrops();
            SummerCrops = SetSummerCrops();
            FallCrops = SetFallCrops();
            WinterCrops = new List<Crop>(); // No default crops in winter, but potentially some from mods

            // All flags are checked from inside this method
            MyHelper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
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

            var currentDay = SDate.From(Game1.Date).Day;
            var currentSeason = SDate.From(Game1.Date).Season;

            switch (currentSeason)
            {
                case "spring":
                    ShowCrops(SpringCrops, currentDay);
                    break;
                case "summer":
                    ShowCrops(SummerCrops, currentDay);
                    break;
                case "fall":
                    ShowCrops(FallCrops, currentDay);
                    break;
                case "winter":
                    ModCompat.LoadCrops(WinterCrops, "winter", MyHelper);
                    ShowCrops(WinterCrops, currentDay);
                    break;
                default:
                    return;
            }
        }

        private void ShowCrops(List<Crop> cropList, int day)
        {
            foreach (Crop crop in cropList)
            {
                var daysLeft = crop.DaysToMature + day;

                if (daysLeft == DaysInAMonth && MyConfig.ShowBaseCrops)
                {
                    Game1.addHUDMessage(new HUDMessage(crop.Message, HUDMessage.newQuest_type));
                }

                double boostedDaysLeft = crop.DaysToMature;

                if (FarmingSkills.IsAgriculturist)
                {
                    boostedDaysLeft -= crop.DaysToMature * FarmingSkills.AgriculturistGrowthRate;
                }

                if (MyConfig.ShowSpeedGro)
                {
                    boostedDaysLeft -= crop.DaysToMature * Fertilizer.SpeedGro;
                    if(boostedDaysLeft == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageSpeedGro, HUDMessage.newQuest_type));
                    }
                    // Reset the value so we can correctly show the next one
                    boostedDaysLeft += crop.DaysToMature * Fertilizer.SpeedGro;
                }

                if (MyConfig.ShowDeluxeSpeedGro)
                {
                    boostedDaysLeft -= crop.DaysToMature * Fertilizer.DeluxeSpeedGro;
                    if (boostedDaysLeft == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageDelxueSpeedGro, HUDMessage.newQuest_type));
                    }
                    // Reset the value so we can correctly show the next one
                    boostedDaysLeft += crop.DaysToMature * Fertilizer.DeluxeSpeedGro;
                }

                if (MyConfig.ShowHyperSpeedGro)
                {
                    boostedDaysLeft -= crop.DaysToMature * Fertilizer.HyperSpeedGro;
                    if (boostedDaysLeft == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageHyperSpeedGro, HUDMessage.newQuest_type));
                    }
                    // Reset the value so we can correctly show the next one
                    boostedDaysLeft += crop.DaysToMature * Fertilizer.HyperSpeedGro;
                }
            }
        }

        private List<Crop> SetSpringCrops()
        {
            var retval = new List<Crop>()
            {
                Crop.GetLocalizedCrop("spring","Blue Jazz",4, MyHelper),
                Crop.GetLocalizedCrop("spring","Cauliflower",12,MyHelper),
                Crop.GetLocalizedCrop("spring","Garlic",4,MyHelper),
                Crop.GetLocalizedCrop("spring","Green Bean",10,MyHelper),
                Crop.GetLocalizedCrop("spring","Kale",6,MyHelper),
                Crop.GetLocalizedCrop("spring","Parsnip",4,MyHelper),
                Crop.GetLocalizedCrop("spring","Potato",6,MyHelper),
                Crop.GetLocalizedCrop("spring","Rhubarb",13,MyHelper),
                Crop.GetLocalizedCrop("spring","Strawberry",8,MyHelper),
                Crop.GetLocalizedCrop("spring","Tulip",6,MyHelper),
                Crop.GetLocalizedCrop("spring","Rice",8,MyHelper)
            };


            ModCompat.LoadCrops(retval, "spring", MyHelper);

            return retval;
        }

        private List<Crop> SetSummerCrops()
        {
            var retval = new List<Crop>()
            {
                Crop.GetLocalizedCrop("summer","Blueberry",13, MyHelper),
                Crop.GetLocalizedCrop("summer","Hops",11,MyHelper),
                Crop.GetLocalizedCrop("summer","Hot Pepper",5,MyHelper),
                Crop.GetLocalizedCrop("summer","Coffee Bean",10,MyHelper),
                Crop.GetLocalizedCrop("summer","Melon",12,MyHelper),
                Crop.GetLocalizedCrop("summer","Poppy",7,MyHelper),
                Crop.GetLocalizedCrop("summer","Radish",6,MyHelper),
                Crop.GetLocalizedCrop("summer","Red Cabbage",9,MyHelper),
                Crop.GetLocalizedCrop("summer","Starfruit",13,MyHelper),
                Crop.GetLocalizedCrop("summer","Summer Spangle",8,MyHelper),
                Crop.GetLocalizedCrop("summer","Tomato",11,MyHelper)
            };

            return retval;
        }

        private List<Crop> SetFallCrops()
        {
            var retval = new List<Crop>()
            {
                Crop.GetLocalizedCrop("fall","Wheat",4, MyHelper),
                Crop.GetLocalizedCrop("fall","Corn",14,MyHelper),
                Crop.GetLocalizedCrop("fall","Amaranth",7,MyHelper),
                Crop.GetLocalizedCrop("fall","Artichoke",8,MyHelper),
                Crop.GetLocalizedCrop("fall","Beet",6,MyHelper),
                Crop.GetLocalizedCrop("fall","Bok Choy",4,MyHelper),
                Crop.GetLocalizedCrop("fall","Cranberries",7,MyHelper),
                Crop.GetLocalizedCrop("fall","Eggplant",5,MyHelper),
                Crop.GetLocalizedCrop("fall","Sunflower",8,MyHelper),
                Crop.GetLocalizedCrop("fall","Fairy Rose",12,MyHelper),
                Crop.GetLocalizedCrop("fall","Grape",10,MyHelper),
                Crop.GetLocalizedCrop("fall","Pumpkin",13,MyHelper),
                Crop.GetLocalizedCrop("fall","Yam",10,MyHelper),
                Crop.GetLocalizedCrop("fall","Ancient Fruit",28,MyHelper)
            };

            return retval;
        }
    }
}
