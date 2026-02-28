# Unity Rules

## Scene Setup

- All scenes are built programmatically via `Editor/Setup/ProjectSetup.cs`.
- Run setup: **Brain Academy → Setup → Run Full Setup** in the Unity Editor menu.
- Do not manually edit scenes in the Unity Editor; regenerate via the setup scripts.

## UI Architecture

- UI uses Unity's Canvas system with `Image`, `Button`, and `TextMeshProUGUI` components.
- All UI elements use `RectTransform` with anchor-based layout for responsive scaling.
- Reference resolution: 1080x1920 (portrait), scale mode: match width or height.

## Canvas Rendering

- The Snake Spellcaster battlefield uses a `BattlefieldRenderer` MonoBehaviour with object pooling.
- Coordinate system: X 0 (left/wizard) to 1000 (right/spawn), Y split into 3 lanes.
- Snakes/spells are `Image` components instantiated from prefabs, positioned via `anchoredPosition`.
- Sprites are loaded at setup time and wired to `BattlefieldRenderer` via `SerializedObject`.

## Controllers

- `SnakeSpellController` — game loop via coroutines, manages battlefield state, spawns snakes.
- `SnakeSpellUIController` — wires question panel UI (timer, question text, answer buttons).
- `BattlefieldRenderer` — renders snakes/spells/wizards each frame from `BattlefieldState`.
- `GameManager` — singleton, persists score/state across scenes via `DontDestroyOnLoad`.

## Assembly Definitions

- `BrainAcademy` — runtime game scripts (`Assets/Scripts/`)
- `BrainAcademy.Editor` — editor-only setup scripts (`Assets/Editor/`)
- `EditModeTests` — NUnit tests (`Assets/Tests/EditMode/`)

## Screen Orientation

- App is locked to portrait via Android player settings.

## Android Builds

- CLI build: `Unity.exe -batchmode -quit -projectPath ... -executeMethod BuildAndroid.Build`
- APK output: `Builds/BrainAcademy.apk`
- Install: `adb install -r Builds/BrainAcademy.apk`
