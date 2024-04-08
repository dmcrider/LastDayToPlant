using StardewValley;
using StardewModdingAPI;

namespace PlantingCalendar{
    public class CalendarMenu : StardewValley.Menus.IClickableMenu
    {
        private static int windowWidth = 1050;
        private static int windowHeight = 600;

        public CalendarMenu() : base(Game1.viewport.Width / 2 - 1050 / 2, Game1.viewport.Height / 2 - 600 / 2, windowWidth, windowHeight, true)
        {
            // This is the constructor for the CalendarMenu class
        }
    }
}