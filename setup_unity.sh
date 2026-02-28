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

# ── Step 1: Initialize project structure ─────────────────────────────────
# -createProject generates ProjectSettings/, .meta files, etc.
# Existing Assets/ content (scripts, Editor/) is preserved.
if [ ! -d "$PROJECT/ProjectSettings" ]; then
    echo "==> Step 1/3: Initializing Unity project structure..."
    "$UNITY_EXE" \
        -createProject "$PROJECT" \
        -batchmode -quit -nographics \
        -logFile "$LOG_DIR/01_create.log" 2>&1 || true

    echo "    Done. Log: $LOG_DIR/01_create.log"
else
    echo "==> Step 1/3: ProjectSettings/ exists, skipping init."
fi

# ── Step 2: Ensure Newtonsoft JSON in manifest ───────────────────────────
MANIFEST="$PROJECT/Packages/manifest.json"
echo "==> Step 2/3: Checking Newtonsoft JSON package..."

if [ -f "$MANIFEST" ] && ! grep -q "newtonsoft" "$MANIFEST"; then
    # Unity's -createProject may have overwritten our manifest.
    # Inject Newtonsoft into the dependencies block.
    sed -i 's/"dependencies": {/"dependencies": {\n    "com.unity.nuget.newtonsoft-json": "3.2.1",/' "$MANIFEST"
    echo "    Added com.unity.nuget.newtonsoft-json to manifest.json"
elif grep -q "newtonsoft" "$MANIFEST" 2>/dev/null; then
    echo "    Newtonsoft already present."
else
    echo "    WARNING: manifest.json not found at $MANIFEST"
fi

# ── Step 3: Run ProjectSetup.Run ─────────────────────────────────────────
echo "==> Step 3/3: Running ProjectSetup.Run (scenes, prefabs, wiring)..."
echo "    This may take a few minutes on first open (package resolution + compilation)."

"$UNITY_EXE" \
    -projectPath "$PROJECT" \
    -executeMethod ProjectSetup.Run \
    -batchmode -quit -nographics \
    -logFile "$LOG_DIR/02_setup.log"

EXIT_CODE=$?

echo ""
if [ $EXIT_CODE -eq 0 ]; then
    echo "=== Setup complete! ==="
    echo ""
    echo "Next steps:"
    echo "  1. Open the project in Unity Hub"
    echo "  2. Window > TextMeshPro > Import TMP Essential Resources"
    echo "  3. Hit Play in MenuScene"
else
    echo "=== Setup failed (exit code $EXIT_CODE) ==="
    echo "Check log: $LOG_DIR/02_setup.log"
    echo ""
    echo "Common fixes:"
    echo "  - Ensure Unity 6 has Android Build Support installed"
    echo "  - Check for compile errors in the log"
    tail -20 "$LOG_DIR/02_setup.log" 2>/dev/null
fi
