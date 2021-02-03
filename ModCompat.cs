using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDayToPlant
{
    public class ModCompat
    {
        public List<Mod> Mods;

        public ModCompat()
        {
            Mods = new List<Mod>();
        }

        public void ConfigureMods(ModConfig config)
        {
            Mods.Add(new PPJAFruitsAndVeggies(config.PPJAFruitsAndVeggies));
        }

        public void LoadCrops(List<Crop> cropList, string season, IModHelper helper)
        {
            // For each enabled mod
            foreach(var mod in Mods)
            {
                if (mod.IsEnabled)
                {
                    switch (season)
                    {
                        case "spring":
                            mod.LoadSpringCrops(cropList, helper);
                            break;
                        case "summer":
                            mod.LoadSummerCrops(cropList, helper);
                            break;
                        case "fall":
                            mod.LoadFallCrops(cropList, helper);
                            break;
                        case "winter":
                            mod.LoadWinterCrops(cropList, helper);
                            break;
                        default:
                            return;
                    }
                }
            }
        }
    }
}
