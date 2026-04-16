# Scenario Instructions

## Scenario
- **ID**: dotnet-version-upgrade
- **Goal**: Upgrade Yatzy solution library projects from .NET 8 to .NET 9.0
- **Solution**: C:\Users\Michael\Documents\MINEPROJEKTER\Yatzy\src\Yatzy.slnx
- **Target Framework**: net9.0

## Preferences

### Flow Mode
Automatic — run end-to-end, pause only when blocked.

### Technical Preferences
- **Frontend projects**: Do NOT upgrade — keep existing target framework
- **Library/backend projects**: Upgrade to net9.0

### Source Control
- Source branch: `NytDesignAfApp`
- Working branch: `upgrade-to-NET9`

## Strategy
**Selected**: All-at-Once — all 5 projects upgraded simultaneously.
**Rationale**: 5 projects, all on .NET 8, only TFM bumps and NuGet package updates.

### Execution Constraints
- All project files updated in a single pass
- Validate full solution build after all changes
- Tests run only after successful build
- Commit after each completed task

## Key Decisions Log
- 2025: Frontend projects excluded from upgrade; only library projects upgraded to net9.0
- 2025: All-at-Once strategy selected — 5 backend projects, straightforward upgrade
