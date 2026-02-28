# AnalyticalJunior — Project Rules

## Overview

Android app (Kotlin + Jetpack Compose) for NSW Opportunity Class test preparation.
Game modes: Quiz (Math/Logic) and Snake Spellcaster (tower defense with OC questions).

## Tech Stack

- **Language:** Kotlin
- **UI:** Jetpack Compose + Material 3
- **Build:** Gradle 8.11.1, AGP 8.7.3, Kotlin 2.1.0
- **Min SDK:** 26, Target SDK: 35
- **Architecture:** ViewModel + Compose Navigation
- **Package:** `com.braincademy.junior`

## Project Structure

```
app/src/main/java/com/braincademy/junior/
├── game/              # Question generators + question bank loader
├── model/             # Data classes, enums (Difficulty, Category, Snake types)
├── navigation/        # NavGraph with routes
├── screens/           # Composable screens (Menu, Game, Results, SnakeSpell)
├── ui/theme/          # Colors, Theme, Typography
├── viewmodel/         # GameViewModel, SnakeSpellViewModel
└── MainActivity.kt
app/src/main/assets/
└── question_bank.json # OC-aligned questions (70 questions)
docs/
├── oc_curriculum.md   # NSW OC test curriculum reference
└── question_bank.json # Source question bank (copied to assets)
```

## Key Files

- `docs/question_bank.json` — Source of truth for OC questions. Edit here, then copy to `app/src/main/assets/`.
- `game/QuestionBankLoader.kt` — Parses question bank JSON, serves questions by difficulty.
- `viewmodel/SnakeSpellViewModel.kt` — Game loop (30fps coroutine), question integration, collision detection.
- `screens/SnakeSpellScreen.kt` — Canvas battlefield + question panel UI.
- `detekt.yml` — Static analysis config (tuned for Compose).
- `.editorconfig` — ktlint config (allows PascalCase @Composable functions).
