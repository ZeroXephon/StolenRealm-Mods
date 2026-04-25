# Architecture notes

Notes on how these mods interact with the game and with each other. Living document — gets updated as we learn more.

## Game internals we depend on

### Singletons
The game uses **lowercase** singleton naming: `GameLogic.instance`, `DifficultySettingManager.instance`, `GlobalSaveData.Instance` (this last one is uppercase — exception to the pattern). Always check both casings via reflection.

### `DifficultySettings`
- Lives on `GlobalSettingsManager.difficultySettings`
- Accessed conveniently via `GameLogic.DifficultySettings` (property does the indirection)
- `DifficultiesRoguelike` is a `List<DifficultySetting>`, NOT an array
- `DifficultySetting` is a plain POCO (extends `System.Object`), not a Unity `ScriptableObject`

### `DifficultyItem` (the tile component)
- `DifficultyTitle` — text under the sprite ("Difficulty I", "Difficulty II", ...)
- `LockedObj` — the pad-lock GameObject overlay, auto-managed by `Update` based on `IsLocked`
- `LockedText` — the lock-message text
- `_DifficultySetting` — back-reference to the data
- `Icon` — the character sprite

### `CharacterPresetTile` (class-pick tile)
- `LockedByDifficultyObj` — a separate "locked behind difficulty" overlay, vanilla but unused by default
- `LockedByDifficultyText` — its message text
- DifficultyExtender activates these when its registered class-unlock requirements aren't met

### `RoguelikeCurrency` (gems)
- Stored on `GlobalSaveData.RoguelikeCurrency` as a float
- Shared display value across save data
- Earned through normal play, spent on EMP powerups

## Inter-mod communication

Mods talk via reflection (no compile-time deps between mods, so they can ship in any order).

### DifficultyExtender → Other mods
Exposes `RegisterRequirement(Guid presetGuid, int minHighestUnlockedIndex)`. Other mods register class GUIDs that should be locked behind difficulty progression.

```csharp
// From StartingClasses or any other mod:
var t = Type.GetType("DifficultyExtender.DifficultyExtenderPlugin, DifficultyExtender");
var m = t?.GetMethod("RegisterRequirement");
m?.Invoke(null, new object[] { myPresetGuid, 5 });  // require D6 cleared
```

The gate hooks `PresetManager.UpdatePresetButtons` and disables the Roguelike Accept button when an attempted preset has an unmet requirement.

### StartingClasses → ExtraMetaProgression
StartingClasses dynamically reflects EMP's powerup-definition list and registers its own additional powerups through that path. Soft dependency — if EMP isn't loaded, StartingClasses skips the powerup registration.

## Critical gotchas (battle-tested)

### BepInEx Cecil scanner is name-strict
The `[BepInPlugin]` attribute class must be named `BepInPlugin` in metadata, not `BepInPluginAttribute`. C# source treats them identically but the compiled IL preserves the actual class name. Cecil string-matches `FullName == "BepInEx.BepInPlugin"`.

### `Logger` must be a property, not a field
Real BepInEx exposes `Logger` as a property. If your stub defines it as a field, `Log = Logger` compiles to `ldfld Logger` instead of `call get_Logger`, which doesn't match the runtime class layout. Symptom: null-ref on first log call.

### `AccessTools.Method(string)` is version-dependent
The single-string colon-syntax overload (`AccessTools.Method("Type:Method")`) doesn't exist in all HarmonyX versions. The version bundled with BepInEx 5.4.23.5 doesn't have it. Use `AccessTools.TypeByName(name).GetMethod(name)` instead. The `FindMethod` helper in DifficultyExtender wraps this.

### Windows Mark-of-the-Web blocks DLL loading
If users download a DLL via browser/chat, Windows tags it with a `Zone.Identifier` NTFS stream. BepInEx's Cecil scanner silently fails to read MOTW-flagged files. Symptom: file is in `plugins/` but `LogOutput.log` reports the wrong plugin count and never mentions the mod by name. Fix: right-click DLL → Properties → Unblock.

### Layout-driven sizing reads as 0
For UI elements inside a `HorizontalLayoutGroup` or similar, `RectTransform.sizeDelta` is 0 — the layout group computes the actual rendered size dynamically. To read real dimensions use `RectTransform.rect.width/height`. To set tile sizes after disabling a layout group, force `sizeDelta` explicitly or tiles render at 0×0.

### Don't add UI components to shared parents
`DifficultyItemHolder.parent` is shared with the Select Party screen. Adding `ScrollRect`, `Mask`, or `Image` components there causes them to bleed onto subsequent screens. **Mod the tiles themselves, not their parents.** If you need clipping/scrolling, find a holder-local container or create a new one.

### Multiplayer
- Game appears to be host-authoritative for encounters with peer-side simulation for stat calculations
- Save data does not appear to cross-validate between peers
- All players need the same DLL versions to avoid desync — the issue is data shape compatibility, not networking
- StartingClasses uses a "build-without-registering, register-later" pattern to avoid race conditions when a non-host loads in mid-class-definition

## Reflection patterns we use

### Resolve a singleton across casing variations
```csharp
static object GetSingleton(Type t)
{
    foreach (var name in new[] { "instance", "Instance" })
    {
        var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
        if (p != null) return p.GetValue(null, null);
    }
    foreach (var name in new[] { "_instance", "instance", "Instance", "s_instance" })
    {
        var f = t.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
        if (f != null) return f.GetValue(null);
    }
    return null;
}
```

### Find a method without HarmonyX's colon-syntax
```csharp
static MethodBase FindMethod(string typeName, string methodName)
{
    var t = AccessTools.TypeByName(typeName);
    if (t == null) return null;
    return t.GetMethod(methodName,
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.Static);
}
```

### Read Vector2/Vector3 fields without compile-time UnityEngine references
Reflection on `.x` and `.y` fields, instantiate via `Type.GetConstructor(new[] { typeof(float), typeof(float) }).Invoke(...)`. Verbose but stub-friendly.
