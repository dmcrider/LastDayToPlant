using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDayToPlant
{
    public class Crop
    {
        public string Name { get; set; }
        public int DaysToMature { get; set; }
        public string Message { get; private set; }
        public string MessageSpeedGro { get; private set; }
        public string MessageDelxueSpeedGro { get; private set; }
        public string MessageHyperSpeedGro { get; private set; }


        public Crop(string name, int daysToMature, IModHelper helper)
        {
            Name = name;
            DaysToMature = daysToMature;
            Message = helper.Translation.Get("notification.crop.no-fertilizer", new { cropName = Name });
            MessageSpeedGro = helper.Translation.Get("notification.crop.speed-gro", new { cropName = Name });
            MessageDelxueSpeedGro = helper.Translation.Get("notification.crop.deluxe-speed-gro", new { cropName = Name });
            MessageHyperSpeedGro = helper.Translation.Get("notification.crop.hyper-speed-gro", new { cropName = Name });
        }
    }
}
