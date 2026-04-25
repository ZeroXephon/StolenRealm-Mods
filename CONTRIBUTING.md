# Contributing & first-time GitHub setup

## Pushing this repo to GitHub for the first time

If you haven't created the GitHub repo yet:

1. Create a new repo on GitHub: https://github.com/new
   - Name: `StolenRealm-Mods` (or whatever you want)
   - Description: "BepInEx mods for Stolen Realm: difficulty extension, custom classes, meta progression"
   - Public
   - **Do NOT** initialize with README, .gitignore, or LICENSE — we have all three already
2. From the repo root locally:
   ```bash
   git init
   git add .
   git commit -m "Initial commit: v1.0 stable — DifficultyExtender 1.0.15, EMP 6.7.0, StartingClasses 1.0.5"
   git branch -M main
   git remote add origin https://github.com/<YOUR_USERNAME>/StolenRealm-Mods.git
   git push -u origin main
   ```
3. Tag the v1.0 stable release:
   ```bash
   git tag -a v1.0 -m "v1.0 stable — known-good fallback before Prestige Coin overhaul"
   git push origin v1.0
   ```
4. On GitHub, go to the **Releases** tab → "Draft a new release" → pick the v1.0 tag → attach the three DLLs from `mods/<ModName>/<ModName>.dll` → publish. This gives users a one-click download.

## Find-and-replace placeholders

Before pushing, find-and-replace these tokens throughout the repo:

- `<YOUR_USERNAME>` → your GitHub username (used in URLs)
- `<YOUR_NAME>` → your display name / preferred attribution

Files that contain placeholders:
- `README.md`
- `LICENSE`
- `mods/*/README.md` (none currently, but if you add author lines)

Quick command to find them all:
```bash
grep -rn "<YOUR_" .
```

## Adding a new mod

1. Create `mods/<NewMod>/src/` with your `.cs` files
2. Add a `mods/<NewMod>/README.md` (copy the format from existing mods)
3. Add it to the table in the top-level `README.md`
4. Build: `./build/build.sh <NewMod>`
5. Commit the source AND the built DLL

## Versioning

Each mod has its own version, set inside the source as `PluginVersion` constant. Bump the version when shipping a behavior change. Tag releases as `<modname>-v<version>` if releasing one mod independently, or `v<X.Y>` for whole-suite releases.

## Issue reports

Ask reporters to attach `BepInEx/LogOutput.log` from a session that reproduces the issue. Without it, almost no bug is debuggable.
