# User-Level Instructions

## Working Environment

Claude Code sessions use **git worktrees** for parallel work. Each agent operates in its own worktree under `.claude/worktrees/<name>/`, created via `claude --worktree <name>`. All worktrees share the same repository history and remote connections. When claiming a ticket, start a worktree named after the ticket (e.g., `claude --worktree radial-battlefield`).

This project uses `uv` as the Python package manager — use `uv run` to execute Python commands (e.g., `uv run scripts/build_question_bank.py`).

## Ticket Claiming

When claiming a GitHub Issue / ticket, always indicate:
1. **Computer name** (hostname) you are running on
2. **Worktree name** you are operating in (e.g., `.claude/worktrees/radial-battlefield`)

This helps the human user identify which parallel Claude Code session is claiming the ticket.

## Progress Updates

After claiming a ticket, post frequent comment updates on the GitHub Issue at each significant step (e.g., planning complete, implementation done, tests passing, precommit passing, PR raised). If working on a sub-ticket, use your judgement to also update the parent ticket when the progress is relevant there.

## GitHub Comments

When posting any comment on GitHub (issues, PRs, or otherwise), always identify yourself with your **hostname** and **worktree name**. This applies to every comment, not just the initial claim.

## GitHub Issues & PRs

When creating GitHub Issues or Pull Requests, always set the assignee to `joshkyh` and identify yourself (hostname + worktree name) in the body or first comment.

## Ticket Creation Process

When planning a main ticket that will be broken into sub-tickets, consider that **multiple parallel agents** may work on sub-tickets simultaneously in separate worktrees. This requires careful dependency analysis.

### Issue Labels — Ticket Hierarchy

Every repo should have two labels for issue hierarchy:

| Label | Color | Description |
|-------|-------|-------------|
| `main-ticket` | `#0075ca` (blue) | Parent ticket with sub-tickets |
| `sub-ticket` | `#e4e669` (yellow) | Child ticket of a main-ticket |

**Rules:**
- When creating a **main ticket**, apply the `main-ticket` label via `gh issue create --label "main-ticket"`.
- When creating a **sub-ticket**, apply the `sub-ticket` label via `gh issue create --label "sub-ticket"`.
- **Sub-ticket titles** must be suffixed with the parent reference: `Title [Main: repo#NNN]`.
  - Same-repo example: `Implement radial battlefield model [Main: AnalyticalJunior#10]`
  - Cross-repo example: `Wire layer checks into orchestrator [Main: sdDbt#511]`
- If these labels do not yet exist in the target repo, create them first:
  ```
  gh label create "main-ticket" --repo OWNER/REPO --description "Parent ticket with sub-tickets" --color "0075ca"
  gh label create "sub-ticket" --repo OWNER/REPO --description "Child ticket of a main-ticket" --color "e4e669"
  ```
- Standalone issues (no parent/child relationship) should **not** receive either label.

### Workflow

1. **Research** — Explore the codebase to understand the scope of the main ticket and identify natural boundaries for sub-tickets.
2. **Dependency analysis** — Determine which sub-tickets can be worked on in parallel without agents stepping on each other's work (e.g., touching the same files, conflicting schema changes, import ordering). Sub-tickets that modify overlapping files or shared interfaces should be sequenced.
3. **Phase grouping** — Organise sub-tickets into sequential **phases**. Sub-tickets within the same phase are parallel-safe; a new phase begins only after all sub-tickets in the prior phase are merged.
4. **Create sub-tickets** — Use `gh issue create --label "sub-ticket"` for each sub-ticket. In the body of each sub-ticket:
   - Clearly describe the scope and acceptance criteria.
   - Explicitly list dependencies (e.g., "Depends on #123, #124").
   - State which phase it belongs to and whether it is parallel-safe within that phase.
   - Include a "Parent: #NNN" line at the top of the body.
5. **Update main ticket** — Post a task list on the main ticket that groups sub-tickets by phase, e.g.:

   ```
   **Phase 1** (parallel)
   - [ ] #201 — Add data model
   - [ ] #202 — Add utility helpers

   **Phase 2** (parallel, after Phase 1 merges)
   - [ ] #203 — Implement service layer (depends on #201)
   - [ ] #204 — Add CLI command (depends on #202)

   **Phase 3**
   - [ ] #205 — Integration tests (depends on #203, #204)
   ```

6. **Human dispatches** — The human user assigns sub-tickets to agents. Do not self-assign sub-tickets from a main ticket you created unless the user tells you to.

## Ticket Development Cycle

Follow this end-to-end workflow when working on a ticket:

0. **Pull latest** — Do not assume the local repo is up to date. Always `git fetch origin && git checkout main && git pull origin main` (or the relevant base branch) before starting work.
1. **Worktree** — Create a worktree for this ticket: `claude --worktree <ticket-name>`. If already in a worktree, verify you are on the correct branch.
2. **Branch** — Create a new branch from `remote/main` (unless the issue specifies a different base; if so, confirm with the user).
3. **Plan** — Enter plan mode. Explore the codebase, design the approach, and finalise the plan.
4. **Comment: Plan** — Post the finalised plan as a comment on the GitHub Issue.
5. **Implement** — Execute the plan step-by-step, posting progress comments on the issue along the way.
6. **Precommit** — Run precommit checks and fix **all** issues. Do not proceed until precommit passes cleanly — any remaining failures will cause CI to fail and block auto-merge.
7. **Test** — Run the test suite and fix **all** failures. Do not proceed until the full suite passes — any remaining failures will cause CI to fail and block auto-merge.
8. **Commit & Push** — Commit with a clear message and push the branch.
9. **Confirm outstanding steps** — Ask the human user whether any remaining steps (e.g., manual verification, config changes) should be handled by them or by you.
10. **Raise PR** — Open a pull request via `gh` using the PR body format below.
11. **Auto-merge** — Set the PR to auto-merge with branch deletion (e.g., `gh pr merge --auto --squash --delete-branch`).
12. **Monitor** — Watch for the PR to be auto-merged (CI passing). Report the outcome.
13. **Summary comment** — Post a summary comment on the issue (and parent issue if applicable) describing what was done.
14. **Close ticket** — Close the GitHub Issue.
15. **Confirm closure** — Do not move on until the PR is confirmed merged and the Issue is confirmed closed. Verify both before proceeding.
16. **Next steps** — Tell the user what's next (e.g., next ticket, pending items, or nothing remaining). Recommend checking the ticket's dependency chain on GitHub to identify which sub-tickets are now unblocked. Before suggesting a specific ticket, verify it hasn't already been claimed by another agent.

### PR Body Format

Use this structure when creating pull requests. Include all sections; omit `## Results` only if the change is purely cosmetic (docs, comments, config) with nothing measurable.

```
## Summary
<1-3 bullet points describing what changed and why>

Closes #<issue-number>

## Results

### Tests
| Suite | Passed | Failed | Skipped | Total |
|-------|--------|--------|---------|-------|
| pytest | N | 0 | M | N+M |

### Other measurable outcomes (include when relevant)
- Pipeline run time: X min → Y min
- Query performance: X s → Y s
- Precommit: all checks pass

## Test plan
- [ ] <checklist of manual or automated verification steps>
```

**Guidelines for the Results section:**
- Always include the test pass/fail table — this is the minimum.
- For **gameplay** changes: confirm the game runs without errors in Unity Editor.
- For **question bank** changes: include question counts from the build script output.
- For **performance** changes: include before/after metrics with methodology.
- Quantitative evidence in PRs helps the reviewer merge with confidence and creates an audit trail.
