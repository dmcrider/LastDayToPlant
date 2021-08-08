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
        public string Message { get; set; }
        public string MessageSpeedGro { get; set; }
        public string MessageDelxueSpeedGro { get; set; }
        public string MessageHyperSpeedGro { get; set; }


        public Crop(string name, int daysToMature)
        {
            Name = name;
            DaysToMature = daysToMature;
        }

        public static Crop GetLocalizedCrop(string season, string name, int daysToMature, IModHelper helper)
        {
            var tagName = name.Replace(" ", "").Trim().ToLower();

            Crop crop = new Crop(helper.Translation.Get($"crop.{season}.{tagName}", new { cropName = name }), daysToMature);

            crop.Message = helper.Translation.Get("notification.crop.no-fertilizer", new { cropName = crop.Name });
            crop.MessageSpeedGro = helper.Translation.Get("notification.crop.speed-gro", new { cropName = crop.Name });
            crop.MessageDelxueSpeedGro = helper.Translation.Get("notification.crop.deluxe-speed-gro", new { cropName = crop.Name });
            crop.MessageHyperSpeedGro = helper.Translation.Get("notification.crop.hyper-speed-gro", new { cropName = crop.Name });

            return crop;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
