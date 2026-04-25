#!/usr/bin/env bash
# Build the stub assemblies used to compile the mods against.
# Run this once. Output lands in build/stubs/.
set -euo pipefail
REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
STUBS_DIR="$REPO_ROOT/build/stubs"
SRC="$STUBS_DIR/src"

cd "$STUBS_DIR"

echo "Building UnityEngine.CoreModule.dll..."
mcs -target:library -out:UnityEngine.CoreModule.dll "$SRC/UnityEngine.CoreModule.cs"

echo "Building UnityEngine.UI.dll..."
mcs -target:library -r:UnityEngine.CoreModule.dll -out:UnityEngine.UI.dll "$SRC/UnityEngine.UI.cs"

echo "Building BepInEx.dll..."
mcs -target:library -r:UnityEngine.CoreModule.dll -out:BepInEx.dll "$SRC/BepInEx.cs"

echo "Building 0Harmony.dll..."
mcs -target:library -r:UnityEngine.CoreModule.dll -out:0Harmony.dll "$SRC/Harmony.cs"

echo "Stubs built in $STUBS_DIR"
