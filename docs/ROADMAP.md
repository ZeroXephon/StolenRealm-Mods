# Roadmap

## Shipped (v1.0)

- ✅ **Extra Meta Progression** v6.7.0 — 10 additional gem-bought powerups
- ✅ **Starting Classes** v1.0.5 — 13 custom roguelike starting classes
- ✅ **Difficulty Extender** v1.0.15 — Difficulties VII–X with stat scaling and UI cleanup

## Planned (next major iteration)

A meta-progression overhaul introducing **Prestige Coins**, a second currency earned at higher difficulties:

1. **MP recon** — confirm what's safe to add to multiplayer-shared data
2. **Currency plumbing** — `PrestigeCurrency` save field, earn hooks on boss kill / run completion, payouts scale by difficulty
3. **UI: counter** — Prestige Coin counter alongside the existing gem counter
4. **UI: Prestige powerup tab** — endless levels of "Advanced X" stats (Advanced Crit Chance, Advanced Mana on Hit, etc.)
5. **EMP rebalance** — cap existing gem-bought powerups at sensible levels with steeper cost ramps
6. **Class unlocks page** — separate UI page where classes can be unlocked using either Prestige Coins or gems
7. **Nightmare mode** — toggle on D6+ that replaces every battle with a boss fight, with extra bosses on world-end encounters at higher difficulties. Pays out coins per boss.

### Design constraints

- All systems must work in multiplayer (everyone needs the same DLL)
- Existing v1.0 data shape must remain compatible (no breaking changes to current save schema)
- Stable v1.0 build remains usable as a fallback if the overhaul breaks something during development

## Long-term ideas

- Gems → Prestige Coins conversion (e.g. 10k gems = 1 coin) as a safety valve for late-game gem hoarders
- Cosmetic / utility unlocks (not just stat upgrades)
- Per-class progression (class mastery levels, similar to roguelike difficulty completion tracking)

## Open questions

- Steam achievement support for D7-D10 — Burst2Flame would have to add them on the Steam side; modding can't add new achievement IDs
- Save migration on game updates — if Burst2Flame patches the save format, modded fields may need migration logic
