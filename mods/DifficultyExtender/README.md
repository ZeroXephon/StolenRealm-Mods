# Difficulty Extender

**Version:** 1.0.15
**Type:** BepInEx 5 plugin
**Game:** Stolen Realm

Extends roguelike difficulties from the vanilla maximum of 6 up to **10**, with tuned stat scaling for each new tier. All ten tiles fit on the existing difficulty-select screen at scaled-down size, with simplified locked-tile labels ("Clear VI", "Clear VII", etc.).

## What it changes

The vanilla `DifficultiesRoguelike` list (a `List<DifficultySetting>` on `DifficultySettings`) is extended at runtime with four new entries: Difficulty VII, VIII, IX, and X. Each tier is cloned from Difficulty VI as a template and its multipliers are scaled.

### Scaling values (relative to D6 = HP 350% / Dmg 350% / BossDmg +45 / BossHP +100)

| Tier  | HP Modifier | Dmg Modifier | Boss Dmg | Boss HP |
|-------|-------------|--------------|----------|---------|
| D6 (vanilla) | 350% | 350% | +45 | +100 |
| **D7** | 525% | 437.5% | +75 | +150 |
| **D8** | 700% | 542.5% | +105 | +200 |
| **D9** | 962.5% | 647.5% | +140 | +275 |
| **D10** | 1400% | 787.5% | +180 | +375 |

These are tunable via the `NewDifficulties[][]` table at the top of the source file.

### UI changes

When more than 6 difficulty tiles exist, the postfix:
- Scales each tile's `localScale` to `6/N` (capped at 0.55 minimum) so all fit horizontally
- Hides the redundant "Difficulty I/II/..." caption under each character (the big Roman numeral on the sprite already conveys the level)
- Replaces the lock-tile message (which was breaking "Difficulty" mid-word at the narrow width) with a compact `"Clear VI"` / `"Clear VII"` / etc.

## How it works (technical)

- Hooks `DifficultySettingManager.PopulateDifficulties` (via two patches: prefix that ensures the array is extended, postfix that handles UI cleanup)
- Hooks `GameLogic.Start` / `GameLogic.Awake` as a fallback to extend the array if the difficulty UI isn't the first thing to populate
- Hooks `PresetManager.UpdatePresetButtons` to support class-unlock-by-difficulty gating (used by other mods via `DifficultyExtender.RegisterRequirement(Guid presetGuid, int minDifficultyIndex)`)
- Resolves the `GameLogic` singleton via lowercase `instance` property (this game uses `get_instance` not `get_Instance`)
- Reads/writes `DifficultiesRoguelike` as `IList`, not `Array` — the field is a `List<DifficultySetting>`
- Clones difficulty entries via `Activator.CreateInstance` (the type is a plain POCO extending `System.Object`, not a Unity `ScriptableObject`)

## Public API for other mods

```csharp
// Register a class preset that should require a minimum difficulty completion
// to unlock. Call this from another mod's plugin Awake or initialization.
//
//   minHighestUnlockedIndex: the value HighestRoguelikeDifficultyIndexUnlocked
//                            must equal or exceed for the preset to be selectable.
//
// Example: require players to have unlocked Difficulty 6 (index 5)
//   typeof(DifficultyExtenderPlugin)
//       .GetMethod("RegisterRequirement")
//       .Invoke(null, new object[] { presetGuid, 5 });
```

When a registered preset is selected and the player's `HighestRoguelikeDifficultyIndexUnlocked` is below the requirement, the gate disables the Roguelike Accept button and activates the tile's existing `LockedByDifficultyObj` overlay.

## Compatibility

- Works alongside other mods that touch the difficulty system as long as those mods don't also rebuild the `DifficultiesRoguelike` list
- Multiplayer: all players must run the same version
- Save data: no schema changes — uses vanilla `HighestRoguelikeDifficultyIndexUnlocked`

## Known limitations

- **Steam achievements are not granted for D7-D10 completions.** The vanilla `ProcessRoguelikeDifficultyAchievements` builds achievement IDs by index ("RoguelikeDifficulty1", "RoguelikeDifficulty2", ...), so it'll attempt to fire IDs that don't exist on Steam's side. No crash, just a silent no-op. (Steam will not retroactively grant the achievement once you reach back down to D6.)
- Lock-tile cleanup assumes lock text is short. If you change the lock messages, longer strings may wrap.
- Tile-scaling floor is 0.55. With `>10` total difficulty tiles you'd start to lose readability.

## Source

[`src/DifficultyExtender.cs`](src/DifficultyExtender.cs) — single-file mod.

## Building

See the top-level [BUILDING.md](../../docs/BUILDING.md). Once set up:

```bash
./build/build.sh DifficultyExtender
```

Output: `mods/DifficultyExtender/DifficultyExtender.dll`
