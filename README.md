# Release History
v2.3.0 - Added additional logging and error messages when trying to load crops from mods. When a crop's last day to plant is a Wednesday, the notification will show on both Tuesday (indicating that tomorrow is the last day to plant) and Wednesday (indicating that today is the last day) so that you can buy seeds from Pierre before he closes shop. Working on a fix for translations.

v2.2.0 - Added support for v1.6 (thanks to Tenebrosful﻿ for the assistance).

v2.1.0 - Added full support for any mod that uses crop.json files.

v2.0.2 - Fixed crop names not being translated correctly.

v2.0.1 - Added support for other mods.

v1.2.0 - Automatically check for Agriculturist skill (determined by the main player in multiplayer). Added flag for base crops in preparation for mod compatibility (coming soon).

v1.1.0 - Added configuration settings. Will now show messages when it is the last day to use each type of Speed Gro.

v1.0.0 - Initial Release.

# Install
- Download from NexusMods: https://www.nexusmods.com/stardewvalley/mods/7917 (recommended) or from the latest release: https://github.com/dmcrider/LastDayToPlant/releases/latest
- Requires SMAPI
- Extract the ZIP to `\Stardew Valley\Mods\`
- Start the game to create the `config.json` file

# Configuration
The configuration options work like this: In addition to a notification on Day 16 of Spring for Cauliflower, if you enable all options, you will also get a notification on Days 18, 19, and 20 for Speed Gro, Deluxe Speed Gro, and Hyper Speed Gro, respectively, for Cauliflower. As another example, you could set `ShowSpeedGro` to `true` to only recieve notifications on the last day that Speed Gro can be used.

The `config.json` file looks like this:

    {
      "ShowGingerIslandCrops": true,
      "ShowSpeedGro": false,
      "ShowDeluxeSpeedGro": false,
      "ShowHyperSpeedGro": false
    }

If you have any issues getting the path right, let me know via GitHub: https://github.com/dmcrider/LastDayToPlant/issues.

# Running into issues?
If something is going wrong, please let me know by submitting an issue on GitHub: https://github.com/dmcrider/LastDayToPlant/issues/new.

# Supported Languages
- English
- Spanish
- Brazilian Portuguese
- French

# Special Thanks
@Duckexza (https://github.com/Duckexza) - Brazilian translation
@4x4cheesecake (https://github.com/4x4cheesecake) - Fix for this issue: https://github.com/dmcrider/LastDayToPlant/issues/16

[@4x4cheesecake](https://github.com/4x4cheesecake) - Fix for [#16](https://github.com/dmcrider/LastDayToPlant/issues/16)
