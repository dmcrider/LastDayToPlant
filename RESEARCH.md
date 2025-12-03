# Research: Supporting Mod Translations Automatically

## Overview

This document outlines approaches to automatically support translations from other mods for crop names in LastDayToPlant. The goal is to display localized crop names for mod-added crops without requiring manual translation entries in our own i18n files.

## Current Implementation

Currently, the mod:
1. Loads crop data from `GameData/Crops`, `Data/Crops`, and JSON Assets API
2. Stores the original (English) crop name in `Crop.OriginalName`
3. Attempts localization via our own i18n files using `helper.Translation.Get($"crop.{OriginalName.Replace(" ", "")}")`
4. Falls back to the original name if no translation exists

**Problem**: Mod-added crops from other mods won't have translations in our i18n files, so they display with their original (usually English) names.

## Research Findings

### Approach 1: Use DisplayName from Game Data Assets (Recommended)

**How it works**: Stardew Valley 1.6+ stores localized display names directly in game data assets. The `DisplayName` field in `GameData/Crops` and `Data/Objects` contains either:
- A literal localized string, OR
- A tokenized string like `[LocalizedText Strings\Objects:Cauliflower_Name]` that SMAPI/game resolves automatically

**Implementation**:
```csharp
// When loading from GameData/Crops, the DisplayName is already localized
var displayName = GetPropRef<string>(entry, "DisplayName");
// This returns the localized name based on the current game locale
```

**Pros**:
- Works automatically for any mod that properly defines translations
- No additional API dependencies
- Uses the same localization system as the base game
- Mod authors handle their own translations

**Cons**:
- Only works for Stardew Valley 1.6+ data format
- Requires content packs to properly define their translations

### Approach 2: Query Other Mod Translation APIs

**How it works**: Some content mod frameworks expose translation APIs that can be queried.

**JSON Assets API**:
JSON Assets does not expose a direct translation API, but crop names returned by `GetCropNames()` are typically the internal names, not localized display names.

**Content Patcher**:
Content Patcher mods define translations in their content packs. The localized strings become part of game assets that we can read.

**Implementation Strategy**:
```csharp
// For crops loaded via JSON Assets, try to find localized name from game objects
var objectData = helper.GameContent.Load<Dictionary<string, ObjectData>>("Data/Objects");
// Find the crop's harvest item and use its DisplayName
```

**Pros**:
- Can work with older mod formats
- Leverages existing localization work by mod authors

**Cons**:
- Complex to implement reliably
- Different mods use different approaches
- Not all mods expose translations

### Approach 3: Token Strings with ITranslationHelper

**How it works**: SMAPI's translation system supports reading translations from other mods via `IModHelper.Translation` with the mod's unique ID.

**Implementation**:
```csharp
// This requires knowing the source mod's UniqueID
var otherModTranslation = helper.ModRegistry.GetApi<ITranslationHelper>("other.mod.uniqueid");
// However, this API is not directly exposed by SMAPI
```

**Status**: SMAPI does not currently provide a public API to read another mod's translation files directly. This approach is **not feasible** with current SMAPI capabilities.

### Approach 4: Load Crop Names from Data/Objects

**How it works**: Every crop produces a harvest item, and those items have localized names in `Data/Objects`. We can look up the crop's harvest item to get a localized name.

**Implementation**:
```csharp
public static string GetLocalizedCropName(IModHelper helper, string cropId)
{
    try
    {
        // Load the crops data
        var crops = helper.GameContent.Load<Dictionary<string, CropData>>("Data/Crops");
        if (crops.TryGetValue(cropId, out var cropData))
        {
            // Get the harvest item ID
            string harvestItemId = cropData.HarvestItemId;
            
            // Load objects data (this contains localized names)
            var objects = helper.GameContent.Load<Dictionary<string, ObjectData>>("Data/Objects");
            if (objects.TryGetValue(harvestItemId, out var objectData))
            {
                return objectData.DisplayName; // Already localized!
            }
        }
    }
    catch { }
    return null;
}
```

**Pros**:
- Works with the standard game data format
- Automatically picks up translations from any mod that properly localizes their items
- No special API required

**Cons**:
- Crop name might differ from harvest item name (e.g., "Coffee Bean Seeds" vs "Coffee Bean")
- Requires mapping between crop IDs and harvest items

### Approach 5: Rely on TokenParser for Localized Strings

**How it works**: Stardew Valley 1.6 introduced `TokenParser` which can resolve tokenized strings like `[LocalizedText Strings\Objects:Cauliflower_Name]`.

**Implementation**:
```csharp
// If DisplayName contains a token string, use TokenParser to resolve it
string displayName = GetPropRef<string>(entry, "DisplayName");
if (!string.IsNullOrEmpty(displayName))
{
    // TokenParser.ParseText resolves localization tokens
    string resolved = StardewValley.TokenizableStrings.TokenParser.ParseText(displayName);
    crop.Name = resolved;
}
```

**Pros**:
- Handles all standard Stardew Valley localization formats
- Works with vanilla and modded content
- Officially supported method

**Cons**:
- Requires understanding of how mods define their tokens
- Only works with properly formatted token strings

## Recommended Solution

**Primary Approach**: Use the `DisplayName` field from `GameData/Crops` directly, as it's already localized by the game/SMAPI when the asset is loaded. This requires minimal changes:

1. When loading crops from `GameData/Crops`, use `DisplayName` as the crop name directly (it's already localized)
2. Store the crop ID separately for deduplication purposes
3. For legacy `Data/Crops` format, look up the harvest item's localized name from `Data/Objects`
4. For JSON Assets crops, check if the game's object data has a localized version

**Implementation Changes Needed**:

```csharp
// In LoadAllCrops for GameData/Crops:
var displayName = GetPropRef<string>(entry, "DisplayName");
// displayName is already localized when loaded via GameContent

// Create crop with localized name directly
var crop = new Crop
{
    OriginalName = cropId,  // Use ID for deduplication
    Name = displayName,     // Already localized
    // ... other properties
};

// Skip the separate Localize() call for these crops since they're pre-localized
```

**Fallback Chain**:
1. `DisplayName` from game data (pre-localized)
2. Harvest item name from `Data/Objects` (pre-localized)
3. Our own i18n translations (for manual overrides)
4. Original name (unlocalized fallback)

## Additional Considerations

### Re-localization on Locale Change
The `Content.LocaleChanged` event already triggers crop reloading. When using pre-localized data from game assets, reloading the assets after a locale change will automatically provide the new locale's translations.

### Asset Invalidation
When `Content.AssetsInvalidated` fires for crop-related assets, reloading should pick up any updated translations.

### Testing Strategy
1. Test with base game crops in different locales
2. Test with popular crop mods (e.g., Stardew Valley Expanded, PPJA crops)
3. Verify fallback behavior for mods without translations

## Conclusion

The best approach is to leverage Stardew Valley 1.6's native localization system through `DisplayName` fields in game data assets. This requires minimal code changes, automatically supports any mod that follows standard localization practices, and maintains compatibility with the existing system.

The current `Localize()` method can be simplified to only apply our own translations as overrides, rather than as the primary localization mechanism.

## References

- [SMAPI Translation Guide](https://stardewvalleywiki.com/Modding:Translations)
- [Stardew Valley 1.6 Modding Migration](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6)
- [Content Patcher Localization](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/translations.md)
- [Game Data Assets](https://stardewvalleywiki.com/Modding:Items)
