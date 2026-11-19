# Git workflow for real evidence

Suggested repository: `SE-Lab-A2-B2B-Wholesale-Management`.

Use real tasks and actual team members. Do not invent commits or PRs for grading.

```bash
git init
git add .
git commit -m "chore: initialize shared wholesale solution"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/SE-Lab-A2-B2B-Wholesale-Management.git
git push -u origin main
# Each owner then creates an assigned feature branch, e.g.:
git switch -c feature/agent-portal
# modify relevant files, then:
git add .
git commit -m "feat(web): implement agent catalog and order workflow"
git push -u origin feature/agent-portal
```

Open a GitHub Pull Request `feature/agent-portal` -> `main` and request review from a teammate before merging.
Repeat for genuinely distinct feature work. The repository and PRs must reflect what the team really did.

Suggested branch naming: `feature/database`, `feature/winforms-admin`, `feature/agent-portal`, `test/business-logic`, `docs/report`.

For the report provide screenshots of: branch list, reviewed and merged PR, commit graph, and a division-of-labor table matching actual work.
