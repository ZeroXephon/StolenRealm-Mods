#!/usr/bin/env bash
#
# Build a mod from this repo.
#
# Usage:
#   ./build/build.sh <ModName>
#   ./build/build.sh DifficultyExtender
#   ./build/build.sh all
#
# Requires:
#   - mcs (Mono C# compiler) on PATH
#   - Game assemblies copied to build/game-assemblies/
#

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
GAME_DLLS="$REPO_ROOT/build/game-assemblies"
STUBS_DIR="$REPO_ROOT/build/stubs"

if ! command -v mcs >/dev/null 2>&1; then
    echo "ERROR: mcs (Mono C# compiler) not found on PATH"
    echo "  Debian/Ubuntu:  sudo apt install mono-mcs"
    echo "  macOS:          brew install mono"
    exit 1
fi

if [ ! -f "$GAME_DLLS/Assembly-CSharp.dll" ] || [ ! -f "$GAME_DLLS/Assembly-CSharp-firstpass.dll" ]; then
    echo "ERROR: game assemblies not found at $GAME_DLLS/"
    echo "  Copy these from your Stolen Realm install:"
    echo "    <Steam>/steamapps/common/Stolen Realm/StolenRealm_Data/Managed/Assembly-CSharp.dll"
    echo "    <Steam>/steamapps/common/Stolen Realm/StolenRealm_Data/Managed/Assembly-CSharp-firstpass.dll"
    exit 1
fi

# Ensure stubs are built
if [ ! -f "$STUBS_DIR/BepInEx.dll" ] || [ ! -f "$STUBS_DIR/0Harmony.dll" ] \
   || [ ! -f "$STUBS_DIR/UnityEngine.CoreModule.dll" ] || [ ! -f "$STUBS_DIR/UnityEngine.UI.dll" ]; then
    echo "Building stubs first..."
    "$REPO_ROOT/build/build-stubs.sh"
fi

build_mod() {
    local mod="$1"
    local mod_dir="$REPO_ROOT/mods/$mod"
    local src_dir="$mod_dir/src"
    local out_dll="$mod_dir/$mod.dll"

    if [ ! -d "$src_dir" ]; then
        echo "ERROR: $src_dir doesn't exist (is '$mod' a real mod name?)"
        return 1
    fi

    # Some mods need extra game-shipped DLLs (Sirenix, TextMeshPro, etc.).
    # If mods/<Mod>/extra-refs.txt exists, each line is the basename of a DLL
    # that the build will look for in build/game-assemblies/.
    local extra_refs=()
    local extra_refs_file="$mod_dir/extra-refs.txt"
    if [ -f "$extra_refs_file" ]; then
        while IFS= read -r line; do
            line="${line%%#*}"   # strip comments
            line="${line## }"; line="${line%% }"
            [ -z "$line" ] && continue
            local p="$GAME_DLLS/$line"
            if [ ! -f "$p" ]; then
                echo "ERROR: $mod requires $line (listed in extra-refs.txt) but it's not in $GAME_DLLS/"
                echo "  Copy it from your Stolen Realm install:"
                echo "    <Steam>/steamapps/common/Stolen Realm/StolenRealm_Data/Managed/$line"
                return 1
            fi
            extra_refs+=("-r:$p")
        done < "$extra_refs_file"
    fi

    echo "Building $mod..."

    mcs -target:library \
        -r:"$STUBS_DIR/UnityEngine.CoreModule.dll" \
        -r:"$STUBS_DIR/UnityEngine.UI.dll" \
        -r:"$STUBS_DIR/BepInEx.dll" \
        -r:"$STUBS_DIR/0Harmony.dll" \
        -r:"$GAME_DLLS/Assembly-CSharp.dll" \
        -r:"$GAME_DLLS/Assembly-CSharp-firstpass.dll" \
        "${extra_refs[@]}" \
        -r:System.Core.dll \
        -out:"$out_dll" \
        "$src_dir"/*.cs

    echo "  -> $out_dll"
}

if [ $# -lt 1 ]; then
    echo "Usage: $0 <ModName>|all"
    exit 1
fi

if [ "$1" = "all" ]; then
    for mod in DifficultyExtender ExtraMetaProgression StartingClasses; do
        build_mod "$mod"
    done
else
    build_mod "$1"
fi

echo "Done."
