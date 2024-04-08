using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if(e.Button.IsActionButton() && Game1.player.ActiveObject != null && Game1.player.ActiveObject.Name == "Planting Calendar")
            {
                Game1.activeClickableMenu = new CalendarMenu();
            }
        }
    }
}
