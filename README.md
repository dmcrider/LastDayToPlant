# Release History
v3.0.0 - Mod overhaul for 1.6

- Added a new item - Planting Calendar. This item is purchasable from Pierre's shop starting Year 2 for 1,000. The Planting calendar has the image of the crop on the last day it can be planted for that year (so crops that grow in both spring and summer, for example, will only show on the calendar in summer).

- If you have the Mod Config UI mod installed, you can set some settings there. Otherwise you'll need to manually edit the config file (see below).

- Crops can optionally be shown with fertilizer enhancements (indicated on the crop by the same icon for the fertilizer)

# Examples

~Add images here~

# Install
- Requires SMAPI
- Download and extract the ZIP to `Mods/`
- Start the game to create the `config.json` file. Once created, it can be edited

# Configuration
The `config.json` file looks like this:

    {
      "HideCustomCrops": true,
      "ShowFertilizerBoosts": true,
      "ShowGingerIslandCrops": false
    }

The Mod Config UI looks like this:

~Add image here~

If `HideCustomCrops` is `true` (the deafult) only vanilla crops will be shown on the calendar. If it's `false`, then any crops added by other mods should be shown as well. **Disclaimer**: I have not tested this will all possible mods. If there are issues, please let me know and I will try to make this mod compatible.

`ShowFertilizerBoosts` will add additional crop icons to the calendar for all vanilla Speed-Gro levels. When you hover over a crop on the calendar, it will tell you which fertilizer you need to use, if any, on that day.

`ShowGingerIslandCrops` by default hides crops only made availabe after Ginger Island has been unlocked. Once Ginger Island has been unlocked, this mod automatically updates this config option.

# Supported Languages
Crops are not directly translated by this Mod. This mod supports all officially supported SDV languages.

# Source Code
The source code is available on [my GitHub](https://github.com/dmcrider/LastDayToPlant).
