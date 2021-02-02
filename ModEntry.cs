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

        private const int DaysInAMonth = 28;
        private IModHelper MyHelper;
        private ModConfig MyConfig;

        public override void Entry(IModHelper helper)
        {
            MyConfig = MyHelper.ReadConfig<ModConfig>();
            SpringCrops = SetSpringCrops();
            SummerCrops = SetSummerCrops();
            FallCrops = SetFallCrops();
            MyHelper = helper;

            MyHelper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
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
                    return; // No crops to plant in winter
                default:
                    return;
            }
        }

        private void ShowCrops(List<Crop> cropList, int day)
        {
            foreach (Crop crop in cropList)
            {
                var daysLeft = crop.DaysToMature + day;

                if (daysLeft == DaysInAMonth)
                {
                    Game1.addHUDMessage(new HUDMessage(crop.Message, HUDMessage.newQuest_type));
                }

                double boostedDaysLeft = crop.DaysToMature;

                if (MyConfig.IsAgriculturist)
                {
                    boostedDaysLeft -= crop.DaysToMature * FarmingSkills.Agriculturist;
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
                new Crop("Blue Jazz",7, MyHelper),
                new Crop("Cauliflower",12, MyHelper),
                new Crop("Garlic",4, MyHelper),
                new Crop("Green Bean",10, MyHelper),
                new Crop("Kale",6, MyHelper),
                new Crop("Parsnip",4, MyHelper),
                new Crop("Potato",6, MyHelper),
                new Crop("Rhubarb",13, MyHelper),
                new Crop("Strawberry",8, MyHelper),
                new Crop("Tulip",6, MyHelper),
                new Crop("Rice",8, MyHelper)
            };

            return retval;
        }

        private List<Crop> SetSummerCrops()
        {
            var retval = new List<Crop>()
            {
                new Crop("Blueberry",13, MyHelper),
                new Crop("Hops",11, MyHelper),
                new Crop("Hot Pepper",5, MyHelper),
                new Crop("Coffee Bean",10, MyHelper),
                new Crop("Melon",12, MyHelper),
                new Crop("Poppy",7, MyHelper),
                new Crop("Radish",6, MyHelper),
                new Crop("Red Cabbage",9, MyHelper),
                new Crop("Starfruit",13, MyHelper),
                new Crop("Summer Spangle",8, MyHelper),
                new Crop("Tomato",11, MyHelper)
            };

            return retval;
        }

        private List<Crop> SetFallCrops()
        {
            var retval = new List<Crop>()
            {
                new Crop("Wheat",4, MyHelper),
                new Crop("Corn",14, MyHelper),
                new Crop("Amaranth",7, MyHelper),
                new Crop("Artichoke",8, MyHelper),
                new Crop("Beet",6, MyHelper),
                new Crop("Bok Choy",4, MyHelper),
                new Crop("Cranberries",7, MyHelper),
                new Crop("Eggplant",5, MyHelper),
                new Crop("Sunflower",8, MyHelper),
                new Crop("Fairy Rose",12, MyHelper),
                new Crop("Grape",10, MyHelper),
                new Crop("Pumpkin",13, MyHelper),
                new Crop("Yam",10, MyHelper),
                new Crop("Ancient Fruit",28, MyHelper)
            };

            return retval;
        }
    }
}
