# Starting Classes

**Version:** 1.0.5
**Type:** BepInEx 5 plugin
**Game:** Stolen Realm

Adds **13 custom starting classes** to roguelike mode, each with curated starting skills, attribute spreads, and starting equipment.

## Classes added

| Class | Concept | Stats (M/D/V/I/R) | Starting Skills |
|---|---|---|---|
| Pyromaniac | Fire-spec damage dealer | 12/10/10/12/6 | Burning Ground, Fuel for the Flames II, Avatar of Flame, Flash Fire |
| Shadow Dragon | Stealth + dragon shapeshift | 10/10/12/12/6 | Endless Night, Skin Walker, Vengeful Shadows |
| Dragon Kin | Dragonkin shapeshifter | 15/6/12/12/5 | Shapeshift Dragonkin, Skin Walker, Hot Head I |
| Florist | Plant/bloom summoner | 10/10/10/14/6 | The Good Bloom, The Bad Bloom, Titan Bloom, Mind Blossom |
| Bubbler | Shield/light support | 10/10/5/15/10 | Mana Shield, Shield of Light, Perfected Soul, Light's Brilliance |
| Dire Lycan | Werewolf melee | 12/8/10/8/12 | Shapeshift Dire Werewolf, Skin Walker, Quick Hands |
| Druid | Nature summoner | 12/8/10/14/6 | Circle of Life, Nature Summoning III, Mass Entangle, Sleep Spores |
| Plague Bringer | Poison/contagion | 14/14/5/10/7 | Contagion, Poison Cloud, Fight Dirty, Contaminated Wound |
| Mitosis | Cloning chaos build | 10/10/6/14/10 | Replicate |
| Holy Dragon | Light/healer dragon | 10/8/13/13/6 | Wrath of the Righteous, Empowered Light I, Empowered Light II, Mass Cure, Skin Walker |
| Harmful Blapper | Pure utility/support | 10/10/10/10/10 | Harm |
| One Punch Man | Single-target obliteration | 14/12/10/4/10 | One Punch Monk, Strength, Focused Strike, Hot Head I, Hot Head II, Warmonger I, Warmonger II, Light's Strength |
| The Oracle | Mystic mark/bind | 6/10/10/14/12 | Soul Link, Stalker's Mark, Chain Lightning |

Stat key: M=Might, D=Dexterity, V=Vitality, I=Intelligence, R=Reflexes

## Bundled debug dumpers

When you create a character, this mod also dumps the game's data tables to your `BepInEx/plugins/` folder for inspection:
- `stolenrealm_skills.txt` — all 427 skills with their full data
- `stolenrealm_items.txt` and `stolenrealm_items.csv` — all 905 items
- `stolenrealm_fortunes.txt` — all 83 fortunes

Useful when authoring new classes and you need to find the right skill/item GUID.

## Compatibility

- Works alongside Extra Meta Progression (StartingClasses dynamically registers EMP powerups when they're available)
- **Multiplayer:** works in MP, but loading-order is sensitive. The mod uses an "early build / late register" pattern to handle the case where a non-host client loads into a session where classes are already needed
- Save data: registered classes get auto-assigned indices in the preset array, GUID-based references are stable across loads

## Authoring new classes

Classes are defined in the source as data tuples — name, attributes, starting skills (by name), starting items (by name), preset GUID, etc. The dumper tools listed above will help you find the names you need.

## Source

[`src/`](src/) — multi-file mod.

## Building

See [BUILDING.md](../../docs/BUILDING.md).
