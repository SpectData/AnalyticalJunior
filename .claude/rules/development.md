# Development Rules

## Environment

- **Java:** Use Android Studio's bundled JDK (configured in `gradle.properties` via `org.gradle.java.home`).
- **Build:** Use `./gradlew` for all build tasks. Never use `gradle` directly.
- **No pip:** This repo has a Python `pyproject.toml` for PDF extraction tooling only. Use `uv` commands if Python is needed.

## Code Style

- Follow existing patterns in the codebase.
- Do not refactor unrelated code.
- Composable functions use **PascalCase** (Compose industry standard).
- Non-composable functions use **camelCase**.

## Pre-commit Checks

Before committing, always run:

```bash
./gradlew ktlintCheck detekt
```

- **ktlint** — Code formatting (auto-fix with `./gradlew ktlintFormat`).
- **detekt** — Static analysis.
- Both are enforced by the git pre-commit hook (auto-installed on Gradle sync).
- Fix all issues before committing. Do not skip or suppress warnings without justification.

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
- The source of truth is `docs/question_bank.json`. After editing, copy to `app/src/main/assets/question_bank.json`.
- Each question has: id, topic, subtopic, difficulty, question text, options, correct_index, correct_answer, explanation.
- Difficulty levels: `easy`, `medium`, `hard`, `super_hard`.
- When adding questions, follow the existing JSON structure and assign appropriate difficulty and topic tags.
