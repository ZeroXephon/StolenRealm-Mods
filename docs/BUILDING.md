# Building from source

These mods are written in C# and compile against:
- BepInEx 5.4.x runtime types
- HarmonyX (bundled with BepInEx)
- UnityEngine (Unity 2022.3.10)
- The game's `Assembly-CSharp.dll` and `Assembly-CSharp-firstpass.dll`

## Prerequisites

### 1. C# compiler

Either works:

**Mono `mcs`** (Linux/macOS, or Windows via WSL/MSYS2):
```bash
sudo apt install mono-mcs   # Debian/Ubuntu
brew install mono           # macOS
```

**.NET SDK** (Windows native, also works on Linux/macOS):
- Install [.NET 6 or newer](https://dotnet.microsoft.com/download)

### 2. Game assemblies

Copy these files **from your Stolen Realm install** to `build/game-assemblies/`:

```
<Steam>/steamapps/common/Stolen Realm/StolenRealm_Data/Managed/
  ├── Assembly-CSharp.dll
  └── Assembly-CSharp-firstpass.dll
```

These files are not redistributed in this repo — they belong to Burst2Flame.

Some mods need **additional** DLLs from that same `Managed/` folder (e.g. StartingClasses transitively depends on `Sirenix.Serialization.dll`, `Sirenix.OdinInspector.Attributes.dll`, and `Unity.TextMeshPro.dll`). Each mod that needs more lists its requirements in `mods/<ModName>/extra-refs.txt`. The build script will tell you exactly what's missing if you run it.

### 3. Stub assemblies

The repo includes minimal stub assemblies in `build/stubs/` for compilation against:
- BepInEx (just enough surface for `[BepInPlugin]`, `BaseUnityPlugin`, `ManualLogSource`)
- HarmonyX (`AccessTools`, `[HarmonyPatch]` and friends)
- UnityEngine.CoreModule (`MonoBehaviour`, `Component`, `Transform`, etc.)
- UnityEngine.UI (`Image`, `Text`, `Button`, layout groups, etc.)

These are stubs only — they let the C# compiler resolve type references. At runtime, the real BepInEx/Harmony/Unity assemblies provide the implementations.

If you'd rather not use the stubs, you can build against the real DLLs by copying them out of your BepInEx install. Either approach works.

## Building

### Linux/macOS / WSL

```bash
# From the repo root:
./build/build.sh DifficultyExtender
./build/build.sh ExtraMetaProgression
./build/build.sh StartingClasses
```

Output DLLs land in `mods/<ModName>/<ModName>.dll`.

### Windows (native, with .NET SDK)

Use a `.csproj` per mod (not currently committed; see `build/build.sh` for the exact reference list — translates straight to `<Reference>` items in a csproj). Or just install Mono on Windows and use the build script.

## Common issues

### "Plugin loaded but doesn't run"
Check that the BepInEx `LogOutput.log` shows `Loading [<YourMod> <version>]`. If not:
- Windows: right-click DLL → Properties → "Unblock" → Apply. Mark-of-the-Web blocks downloaded DLLs from being loaded by Cecil scanners.
- Verify the DLL is in `BepInEx/plugins/` directly, not a subfolder
- Check the plugin metadata uses `BepInEx.BepInPlugin` (without the "Attribute" suffix) — Cecil string-matches on the type name. Our stubs handle this correctly; if you swap to a different stub set, make sure it doesn't.

### "AccessTools.Method not found at runtime"
HarmonyX's `AccessTools.Method(string)` (single-string colon-syntax) doesn't exist in all versions. Use `AccessTools.TypeByName(...)` then `Type.GetMethod(...)` — see `DifficultyExtender.FindMethod` helper.

### "MonoBehaviour Awake throws null-ref on Logger"
Make sure your stub's `Logger` is a *property* (not a field). C# emits `call get_Logger` for properties and `ldfld Logger` for fields, and the latter doesn't match BepInEx's actual layout. Symptom is `Logger.LogInfo(...)` calls throwing immediately on plugin load.

## Verifying a build

After compiling, you can check the DLL's metadata without running it:

```bash
# Check it has a BepInPlugin attribute and references BepInEx.BepInPlugin (not BepInPluginAttribute)
python3 -c "
import dnfile, sys
dn = dnfile.dnPE(sys.argv[1])
for tr in dn.net.mdtables.TypeRef.rows:
    if 'BepIn' in str(tr.TypeName):
        print(f'{tr.TypeNamespace}.{tr.TypeName}')
" mods/DifficultyExtender/DifficultyExtender.dll
```

Should show `BepInEx.BepInPlugin` (correct), not `BepInEx.BepInPluginAttribute` (broken).

## Architecture notes

See [ARCHITECTURE.md](ARCHITECTURE.md) for how the mods talk to each other and what hooks they use.
