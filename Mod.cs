using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDayToPlant
{
    public abstract class Mod
    {
        public bool IsEnabled { get; private set; }
        public string Name { get; private set; }

        public Mod(string name, bool enabled)
        {
            Name = name;
            IsEnabled = enabled;
        }
        
        public abstract void LoadSpringCrops(List<Crop> list, IModHelper helper);
        public abstract void LoadSummerCrops(List<Crop> list, IModHelper helper);
        public abstract void LoadFallCrops(List<Crop> list, IModHelper helper);
        public abstract void LoadWinterCrops(List<Crop> list, IModHelper helper);
    }
}
