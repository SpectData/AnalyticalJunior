# Snake Spellcaster Gameplay

## Overview

The wizard stands at the centre of a circular battlefield. Snakes approach from random angles (360 degrees). The game alternates between two phases: **Wave Phase** (short questions) and **Inter-Wave Phase** (long questions).

## Battlefield Layout

- **Orientation:** Top-down radial view. The wizard is at the centre.
- **Snakes:** Spawn at the edges and move inward toward the wizard from random angles.
- **Lanes:** No fixed lanes. Snakes can approach from any direction.

## Phase 1: Wave Phase (Short Questions)

- Snakes spawn and advance toward the wizard.
- The player answers **short questions** (maths, thinking skills) to cast spells.
- **No per-question timer.** The time pressure comes from snakes advancing toward the wizard — answer faster to kill them before they arrive.
- Correct answer = spell auto-aims at the nearest snake and destroys it.
- Wrong answer = the snake keeps advancing (no penalty beyond lost time).
- If the player earned a **Lightning Bolt** from the previous inter-wave phase, a lightning bolt button is available. The player can tap it at any strategic moment to zap 3 snakes at once.
- Lightning bolt is **use-it-or-lose-it** — it does not carry over to future waves.
- The wave ends when all snakes in the wave are defeated or the wizard takes too much damage.

## Phase 2: Inter-Wave Phase (Long Questions)

- Between waves, the player is presented with a **long question** (reading comprehension with passage).
- **Passage grouping:** Questions are grouped by passage. The passage is shown once and reused across consecutive inter-wave phases until all questions for that passage are exhausted, then a new passage is introduced.
- The player reads the passage and answers the question at their own pace. **No timer** — there is no wave pressure during this phase.
- Correct answer = the wizard earns a **Lightning Bolt** for the next wave.
- Wrong answer = no lightning bolt; the next wave starts without the bonus.
- **Supported question types:** comprehension, poem (MCQ), and in future: cloze, sentence insertion, extract matching.

## Adaptive Difficulty

- **No difficulty selection.** The game adapts to the player automatically.
- The game adjusts **snake spawn rate** (snakes per second) to target an ~80% success rate for the player.
- If the player is answering correctly and quickly, snakes spawn faster. If the player is struggling, snakes slow down.
- This replaces traditional difficulty levels (easy/medium/hard) — the game finds each player's challenge sweet spot dynamically.
