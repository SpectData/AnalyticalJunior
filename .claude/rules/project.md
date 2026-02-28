# AnalyticalJunior — Project Rules

## Overview

Unity app for NSW Opportunity Class test preparation, targeting Android.
Game modes: Quiz (Math/Logic) and Snake Spellcaster (tower defense with OC questions).

## Tech Stack

- **Engine:** Unity 6000.3.10f1 (Unity 6 LTS)
- **Language:** C#
- **UI:** Unity UI (Canvas + Image + TextMeshPro)
- **Build:** Unity batch mode or Editor, Gradle for final APK
- **Min SDK:** 26, Target SDK: 35
- **Architecture:** MonoBehaviour controllers + ScriptableObject-style data
- **Package:** `com.braincademy.junior`

## Project Structure

```
UnityProject/BrainAcademy/Assets/
├── Scripts/
│   ├── Core/              # AppColors, SnakeSpellConstants, GameManager, AudioManager
│   ├── Models/            # Enums, SnakeSpellModels, GameModels, SnakeSpellConstants
│   ├── Questions/         # QuestionBankLoader, MathQuestionGenerator, LogicQuestionGenerator
│   ├── Quiz/              # QuizController, QuizUIController
│   ├── SnakeSpell/        # SnakeSpellController, BattlefieldRenderer, WaveGenerator
│   ├── UI/                # AnswerButtonUI, FeedbackOverlay, MenuUIController, etc.
│   └── BrainAcademy.asmdef
├── Editor/
│   ├── Setup/             # ProjectSetup, SceneBuilderSnake, PrefabFactory, UIFactory
│   ├── BuildAndroid.cs
│   └── BrainAcademy.Editor.asmdef
├── Tests/
│   └── EditMode/          # NUnit tests for pure logic classes
├── Scenes/                # MenuScene, QuizGameScene, SnakeSpellScene, etc.
├── Sprites/               # DALL-E generated PvZ-style sprites (8 PNGs)
├── Prefabs/               # AnswerButton, SnakePrefab, SpellPrefab, MemoryCell
└── TextMesh Pro/          # TMP essential resources
```

## Key Files

- `docs/question_bank.json` — Source of truth for OC questions.
- `Scripts/Questions/QuestionBankLoader.cs` — Parses question bank JSON, serves questions by difficulty.
- `Scripts/SnakeSpell/SnakeSpellController.cs` — Game loop, question integration, collision detection.
- `Scripts/SnakeSpell/BattlefieldRenderer.cs` — UI Canvas battlefield rendering with sprites.
- `Editor/Setup/ProjectSetup.cs` — One-click scene/prefab generation (Brain Academy → Setup menu).
- `Editor/BuildAndroid.cs` — CLI-invokable Android APK build.
- `.editorconfig` — C# formatting rules.
