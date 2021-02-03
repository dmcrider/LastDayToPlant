using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDayToPlant
{
    public class ModCompat
    {
        public List<Tuple<string, bool>> Mods;

        public ModCompat()
        {
            Mods = new List<Tuple<string, bool>>();
        }

        public void ConfigureMods(ModConfig config)
        {
            Mods.Add(Tuple.Create("BetterCropsAndForaging", config.PPJAFruitsAndVeggies));
        }
    }
}
