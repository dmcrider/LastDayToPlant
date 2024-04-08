using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingCalendar
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if(e.NameWithoutLocale.IsEquivalentTo("Objects/Planting Calendar"))
            {
                e.LoadFromModFile<PlantingCalendar>("assets/planting-calendar.png", AssetLoadPriority.Low);
            }
        }
    }
}
