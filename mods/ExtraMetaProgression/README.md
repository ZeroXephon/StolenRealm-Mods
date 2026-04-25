# Extra Meta Progression

**Version:** 6.7.0
**Type:** BepInEx 5 plugin
**Game:** Stolen Realm

Adds new powerups to the roguelike meta-progression system, expanding what gems can buy beyond the vanilla 18 baseline powerups. Gives accumulated gems somewhere to go and lets your characters become measurably stronger across runs.

## What it changes

Extends the `RoguelikePowerups` list with 10 additional powerups that integrate cleanly with vanilla code paths. Vanilla's loop iterates by `.Count`, so new entries are picked up automatically.

The 10 additional powerups grant levels in:
- Critical Chance
- Critical Damage
- Max Health
- Dodge Rating
- Block Chance
- Life on Hit
- Max Mana
- Mana On Hit
- Summon Limit
- Extra Action

Each maps to a real `CharacterAttribute` enum value on the game's character stat system.

## How it works

- Hooks the powerup definition list to append entries
- Powerup icons are sourced from existing vanilla items (no new sprite assets shipped)
- Click handler uses Postfix on the vanilla initialization so window setup completes before the mod's logic runs

## Compatibility

- Works alongside Starting Classes (StartingClasses imports EMP powerups dynamically)
- Multiplayer: all players must run the same version
- Save data: powerups are tracked as GUID→level entries in `RoguelikePowerupLevels`, schema-compatible with vanilla

## Source

[`src/`](src/)

## Building

See [BUILDING.md](../../docs/BUILDING.md).
