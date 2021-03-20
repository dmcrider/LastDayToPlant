using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDayToPlant
{
    class ModConfig
    {
        public bool IncludeBaseGameCrops { get; set; } = true;
        public bool ShowBaseCrops { get; set; } = true;
        public bool ShowSpeedGro { get; set; } = false;
        public bool ShowDeluxeSpeedGro { get; set; } = false;
        public bool ShowHyperSpeedGro { get; set; } = false;
        public string PPJAFruitsAndVeggiesPath { get; set; } = "";
        public string PPJAFantasyCropsPath { get; set; } = "";
        public string PPJAAncientCropsPath { get; set; } = "";
        public string PPJACannabisKitPath { get; set; } = "";
        public string BonstersFruitAndVeggiesPath { get; set; } = "";
    }
}
