# Stolen Realm Mods

A collection of BepInEx mods for [Stolen Realm](https://store.steampowered.com/app/1330000/Stolen_Realm/) that overhaul the meta-progression and starting class systems.

## Mods in this repo

| Mod | Version | Description |
|---|---|---|
| [Extra Meta Progression](mods/ExtraMetaProgression/) | 6.7.0 | Adds new powerups to the roguelike meta-progression screen, expanding what gems can buy. |
| [Starting Classes](mods/StartingClasses/) | 1.0.5 | Adds 13 custom starting classes to roguelike mode (Pyromaniac, Shadow Dragon, Druid, Plague Bringer, and more). |
| [Difficulty Extender](mods/DifficultyExtender/) | 1.0.15 | Extends roguelike difficulties from 6 up to 10, with tuned stat scaling for each new tier. |

These mods are designed to work together but each can be used independently.

## Quick install

1. Install [BepInEx 5.4.x](https://github.com/BepInEx/BepInEx/releases) (the **5.x** line, not 6) for your Stolen Realm install
2. Launch the game once so BepInEx generates its folders
3. Download the latest release DLLs (or build from source — see below)
4. Drop the DLL files into `Stolen Realm/BepInEx/plugins/`
5. **Windows users:** right-click each DLL → Properties → check "Unblock" → Apply. Windows blocks DLLs downloaded from the internet by default; if you skip this BepInEx silently won't load the mod.

## Multiplayer

These mods work in multiplayer. **All players must have the same DLL versions installed.** Mismatched versions will desync the game in confusing ways.

## Compatibility

Built and tested against:
- Stolen Realm (current Steam release as of April 2026)
- BepInEx 5.4.23.5
- Unity 2022.3.10
- HarmonyX (bundled with BepInEx)

If a Stolen Realm patch breaks these mods, file an issue with your `BepInEx/LogOutput.log`.

## Building from source

See [docs/BUILDING.md](docs/BUILDING.md). Requires either Mono `mcs` or .NET SDK; you'll need to copy `Assembly-CSharp.dll` and `Assembly-CSharp-firstpass.dll` from your Stolen Realm install (these are not redistributed here).

## Project structure

```
StolenRealm-Mods/
├── mods/
│   ├── DifficultyExtender/    # Source + built DLL + per-mod README
│   ├── ExtraMetaProgression/
│   └── StartingClasses/
├── build/
│   └── stubs/                 # Stub assemblies used to compile against
├── docs/                      # Building, architecture, design notes
└── README.md                  # this file
```

## Development status

Active. See [docs/ROADMAP.md](docs/ROADMAP.md) for what's planned.

## License

[MIT](LICENSE) — do whatever you want, just keep attribution.

## Credits

Built by <YOUR_NAME>, with engineering help from Claude (Anthropic).

## Disclaimer

Not affiliated with or endorsed by Burst2Flame Entertainment, the developers of Stolen Realm. Modding is at your own risk.
