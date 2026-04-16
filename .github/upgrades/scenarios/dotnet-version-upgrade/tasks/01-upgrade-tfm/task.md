# 01-upgrade-tfm: Update target frameworks to net9.0

Update `TargetFramework` from `net8.0` to `net9.0` in all 5 project files. This includes all projects:
- Yatzy.Domain
- Yatzy.Application
- Yatzy.Infrastructure
- Yatzy.Persistence
- Yatzy.Api

**Done when**: All 5 .csproj files reference `net9.0` and the solution restores without errors.
