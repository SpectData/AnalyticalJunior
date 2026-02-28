# Development Rules

## Environment

- **Unity:** Unity 6000.3.10f1 (Unity 6). Open the project via Unity Hub pointing at `UnityProject/BrainAcademy/`.
- **Build:** Use Unity batch mode or the Unity Editor for builds. CLI: `Unity.exe -batchmode -quit -projectPath ... -executeMethod BuildAndroid.Build`
- **Android deploy:** `adb install -r Builds/BrainAcademy.apk` (adb is at `%LOCALAPPDATA%/Android/Sdk/platform-tools/adb.exe`)
- **No dotnet CLI:** This is a Unity-only project. Do not run `dotnet` commands.
- **No pip:** This repo has a Python `pyproject.toml` for sprite tooling only. Use `uv` commands if Python is needed.

## Code Style

- Follow existing patterns in the codebase.
- Do not refactor unrelated code.
- Use **PascalCase** for public methods, classes, and properties (C# convention).
- Use **camelCase** for private fields and local variables.
- Use **4-space indentation** (configured in `.editorconfig`).

## Pre-commit Checks

The pre-commit hook (`scripts/pre-commit`) runs automatically and checks:
1. No secret/credential files staged
2. No trailing whitespace in C# files
3. No accidentally large files staged
4. Unity compilation (optional, set `UNITY_EXE` env var to enable)

Install the hook if not already present:
```bash
cp scripts/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
```

## Testing

Run EditMode tests via Unity batch mode:
```bash
Unity.exe -projectPath UnityProject/BrainAcademy -batchmode -runTests -testPlatform EditMode -testResults Logs/test-results.xml
```

Or in Unity Editor: **Window > General > Test Runner > EditMode > Run All**.

- Tests live in `Assets/Tests/EditMode/` with the `EditModeTests.asmdef` assembly.
- Test pure logic classes (models, wave generator, difficulty config) in EditMode tests.
- Do not test MonoBehaviour rendering/UI in EditMode; those belong in PlayMode tests (future).

## Git Practices

- Write clear, concise commit messages prefixed with: `Feature/`, `Bugfix/`, `Refactor/`, `Chore/`.
- Only commit when explicitly asked by the user.
- Prefer adding specific files over `git add -A`.
- Never work directly on `main` — always create a branch aligned to the GitHub issue.
- Never merge PRs. Raise the PR, and the user manually merges after CI passes.
- Use `gh` CLI for GitHub operations.

## Safety Rules

- Never commit `.env*`, `**/secrets/**`, `*credentials*`, `*.pem`, `*.key` files.
- Never run destructive git commands (`push --force`, `clean -f`) without explicit user approval.
- Never make network requests to unknown/external endpoints.
- When doing web search, only use reputable corporate-published sources. Avoid personal blogs, reddit, medium.com.

## Question Bank

- Questions are aligned to the **NSW Opportunity Class Placement Test** curriculum (Year 4, Stage 2).
- The source of truth is `docs/question_bank.json`. After editing, copy to `UnityProject/BrainAcademy/Assets/Resources/question_bank.json`.
- Each question has: id, topic, subtopic, difficulty, question text, options, correct_index, correct_answer, explanation.
- Difficulty levels: `easy`, `medium`, `hard`, `super_hard`.
- When adding questions, follow the existing JSON structure and assign appropriate difficulty and topic tags.
