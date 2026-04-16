# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade all 5 Yatzy backend/library projects from .NET 8 to .NET 9.0  
**Scope**: 5 projects — Yatzy.Domain, Yatzy.Application, Yatzy.Infrastructure, Yatzy.Persistence, Yatzy.Api

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.  
**Rationale**: 5 projects, all on .NET 8, straightforward TFM bumps and NuGet package updates.

## Tasks

### 01-upgrade-tfm: Update target frameworks to net9.0

Update `TargetFramework` from `net8.0` to `net9.0` in all 5 project files. This includes all projects:
- Yatzy.Domain
- Yatzy.Application
- Yatzy.Infrastructure
- Yatzy.Persistence
- Yatzy.Api

**Done when**: All 5 .csproj files reference `net9.0` and the solution restores without errors.

---

### 02-update-packages: Update NuGet packages to net9.0-compatible versions

Update NuGet packages identified in the assessment as needing version bumps for .NET 9.0 compatibility. Applies to Yatzy.Api (NuGet.0002 flagged). Restore and verify no dependency conflicts.

**Done when**: All packages resolve to net9.0-compatible versions with no conflicts.

---

### 03-validate: Build solution and run tests

Build the full solution and run all tests to confirm the upgrade is complete and correct. Fix any compilation errors or test failures.

**Done when**: Solution builds with 0 errors, all tests pass.
