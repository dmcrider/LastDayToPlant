# GitHub Copilot Instructions for LastDayToPlant

## Project Overview

LastDayToPlant is a Stardew Valley mod built using SMAPI (Stardew Modding API). It notifies players when today is the last day they can plant a crop and still harvest it before the season ends. The mod supports various fertilizers, the Agriculturist profession, Ginger Island crops, and crops from other mods.

## Technology Stack

- **Language**: C# (.NET 6.0)
- **Framework**: SMAPI 3.8.0+
- **Build System**: MSBuild via `Pathoschild.Stardew.ModBuildConfig`
- **Localization**: SMAPI translation system with `Pathoschild.Stardew.ModTranslationClassBuilder`

## Project Structure

- `ModEntry.cs` - Main mod entry point, handles SMAPI events
- `Crop.cs` - Crop data model and loading logic (supports GameData, Data/Crops, and JSON Assets)
- `ModConfig.cs` - Configuration options (ShowGingerIslandCrops, ShowSpeedGro, etc.)
- `Fertilizer.cs` - Fertilizer growth rate constants
- `FarmingSkills.cs` - Agriculturist profession detection
- `i18n/` - Translation files (default.json, es.i18n.json, fr.i18n.json, pt.i18n.json)
- `manifest.json` - SMAPI mod manifest

## Coding Conventions

### C# Style
- Use file-scoped namespaces (e.g., `namespace LastDayToPlant;`)
- Use `var` for implicit typing when the type is obvious
- Use expression-bodied members where appropriate
- Prefix private fields with underscore when they are static (e.g., `_jaApiCache`)
- Use PascalCase for public members, camelCase for local variables

### SMAPI Patterns
- Subscribe to events in `Entry()` method
- Use `IModHelper` for accessing game content, translations, and config
- Use `IMonitor` for logging (LogLevel.Trace for debug, LogLevel.Warn for issues)
- Handle `GameLoop.DayStarted` for daily checks
- Handle `Content.LocaleChanged` and `Content.AssetsInvalidated` for dynamic content updates

### Localization
- All user-facing strings should be in `i18n/default.json`
- Use `I18n.YourKey()` pattern via ModTranslationClassBuilder
- Translation keys use dot notation (e.g., `notification.crop.no-fertilizer`)
- Support placeholders with `{{variableName}}` syntax

## Building the Mod

The mod uses SMAPI's mod build system. To build:

1. Install SMAPI and Stardew Valley
2. Build with `dotnet build` or via Visual Studio
3. The mod will automatically deploy to the Stardew Valley Mods folder

## Testing

This is a Stardew Valley mod and requires manual testing within the game:
- Test notifications appear at the correct day for each crop
- Test fertilizer notifications (Speed Gro, Deluxe Speed Gro, Hyper Speed Gro)
- Test Agriculturist profession detection
- Test with different locales
- Test compatibility with JSON Assets mods

## Key Behaviors

- Notifications appear on the last day a crop can be planted and still mature before season end
- Wednesday notifications show a day early since Pierre's shop is closed on Wednesdays
- Supports the Agriculturist profession's 10% growth speed bonus
- Dynamically loads crops from GameData/Crops, Data/Crops, and JSON Assets API

## When Making Changes

1. Keep SMAPI compatibility (minimum version 3.8.0)
2. Maintain support for mod integrations (JSON Assets, etc.)
3. Add translations for any new user-facing strings
4. Test with and without various config options enabled
5. Consider edge cases around season transitions and year requirements
