#!/bin/bash
set -e

REPO_ROOT="$(cd "$(dirname "$0")" && pwd)"
PROJECT="$REPO_ROOT/UnityProject/BrainAcademy"
LOG_DIR="$PROJECT/Logs"
mkdir -p "$LOG_DIR"

# ── Auto-detect Unity 6 ─────────────────────────────────────────────────
UNITY_HUB="/c/Program Files/Unity/Hub/Editor"
UNITY_EXE=""

for d in "$UNITY_HUB"/6000.*/; do
    [ -f "$d/Editor/Unity.exe" ] && UNITY_EXE="$d/Editor/Unity.exe"
done

if [ -z "$UNITY_EXE" ]; then
    echo "ERROR: Unity 6 not found in $UNITY_HUB"
    echo "If installed elsewhere, set UNITY_EXE manually:"
    echo "  export UNITY_EXE=\"/path/to/Unity.exe\""
    exit 1
fi

echo "Unity:   $UNITY_EXE"
echo "Project: $PROJECT"
echo ""

# ── Step 1/4: Initialize project structure ───────────────────────────────
if [ ! -d "$PROJECT/ProjectSettings" ]; then
    echo "==> Step 1/4: Initializing Unity project structure..."
    "$UNITY_EXE" \
        -createProject "$PROJECT" \
        -batchmode -quit -nographics \
        -logFile "$LOG_DIR/01_create.log" 2>&1 || true

    echo "    Done. Log: $LOG_DIR/01_create.log"
else
    echo "==> Step 1/4: ProjectSettings/ exists, skipping init."
fi

# ── Step 2/4: Ensure Newtonsoft JSON in manifest ─────────────────────────
MANIFEST="$PROJECT/Packages/manifest.json"
echo "==> Step 2/4: Checking Newtonsoft JSON package..."

if [ -f "$MANIFEST" ] && ! grep -q "newtonsoft" "$MANIFEST"; then
    sed -i 's/"dependencies": {/"dependencies": {\n    "com.unity.nuget.newtonsoft-json": "3.2.1",/' "$MANIFEST"
    echo "    Added com.unity.nuget.newtonsoft-json to manifest.json"
elif grep -q "newtonsoft" "$MANIFEST" 2>/dev/null; then
    echo "    Newtonsoft already present."
else
    echo "    WARNING: manifest.json not found at $MANIFEST"
fi

# ── Step 3/4: Extract TMP Essential Resources ────────────────────────────
echo "==> Step 3/4: Extracting TMP Essential Resources..."

TMP_DEST="$PROJECT/Assets/TextMesh Pro"
if [ -d "$TMP_DEST/Resources" ]; then
    echo "    TMP already extracted, skipping."
else
    # Find the .unitypackage in the package cache
    UGUI_PKG=""
    for d in "$PROJECT/Library/PackageCache"/com.unity.ugui*/; do
        candidate="$d/Package Resources/TMP Essential Resources.unitypackage"
        [ -f "$candidate" ] && UGUI_PKG="$candidate"
    done

    if [ -n "$UGUI_PKG" ]; then
        TMPDIR_EXTRACT=$(mktemp -d)
        tar xzf "$UGUI_PKG" -C "$TMPDIR_EXTRACT" 2>/dev/null

        # Each GUID folder contains: pathname, asset, asset.meta
        for guid_dir in "$TMPDIR_EXTRACT"/*/; do
            [ -f "$guid_dir/pathname" ] || continue
            rel_path=$(cat "$guid_dir/pathname" | tr -d '\r\n')

            # Only process files under Assets/TextMesh Pro/
            case "$rel_path" in Assets/TextMesh\ Pro/*) ;; *) continue ;; esac

            dest_path="$PROJECT/$rel_path"
            dest_dir=$(dirname "$dest_path")
            mkdir -p "$dest_dir"

            if [ -f "$guid_dir/asset" ]; then
                cp "$guid_dir/asset" "$dest_path"
            fi
            if [ -f "$guid_dir/asset.meta" ]; then
                cp "$guid_dir/asset.meta" "${dest_path}.meta"
            fi
        done

        rm -rf "$TMPDIR_EXTRACT"
        echo "    Extracted TMP fonts and shaders to Assets/TextMesh Pro/"
    else
        echo "    WARNING: TMP Essential Resources.unitypackage not found."
        echo "    Run Unity once to populate Library/PackageCache, then re-run."
    fi
fi

# ── Step 4/4: Run ProjectSetup.Run ───────────────────────────────────────
echo "==> Step 4/4: Running ProjectSetup.Run (scenes, prefabs, wiring)..."
echo "    This may take a few minutes on first open (package resolution + compilation)."

"$UNITY_EXE" \
    -projectPath "$PROJECT" \
    -executeMethod ProjectSetup.Run \
    -batchmode -quit \
    -logFile "$LOG_DIR/02_setup.log"

EXIT_CODE=$?

echo ""
if [ $EXIT_CODE -eq 0 ]; then
    echo "=== Setup complete! ==="
    echo ""
    echo "Next steps:"
    echo "  1. Open the project in Unity Hub"
    echo "  2. Open Assets/Scenes/MenuScene"
    echo "  3. Hit Play"
else
    echo "=== Setup failed (exit code $EXIT_CODE) ==="
    echo "Check log: $LOG_DIR/02_setup.log"
    tail -20 "$LOG_DIR/02_setup.log" 2>/dev/null
fi
