# Compose & Android Rules

## Jetpack Compose

- Use `@Composable` functions with PascalCase names (e.g., `GameScreen`, `QuestionPanel`).
- Keep composables focused — extract sub-composables when a function exceeds ~100 lines.
- Use `Modifier` as the last parameter with default `Modifier`.
- Prefer `mutableStateOf` for ViewModel-level state, observed by Compose recomposition.
- Use `LaunchedEffect` for side effects tied to composition lifecycle.

## ViewModel

- Use `ViewModel` for simple state, `AndroidViewModel` when Android `Context` is needed (e.g., asset access).
- The `viewModel()` compose function handles both types automatically.
- Game loops use `viewModelScope.launch` with `delay(33L)` for ~30fps.

## Navigation

- All routes are defined in `navigation/NavGraph.kt` as `Routes` object constants.
- Use `popUpTo(Routes.MENU)` to prevent back-stack buildup during game flows.

## Canvas Rendering

- The Snake Spellcaster battlefield uses Compose `Canvas` for real-time rendering.
- Coordinate system: X 0 (left/wizard) to 1000 (right/spawn), Y split into 3 lanes.
- Spell/snake positions are in game coordinates, scaled to canvas size at draw time.

## Screen Orientation

- App is locked to portrait via `android:screenOrientation="portrait"` in AndroidManifest.xml.
