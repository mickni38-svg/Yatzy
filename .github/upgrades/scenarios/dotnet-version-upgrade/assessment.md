# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v9.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [Yatzy.Api\Yatzy.Api.csproj](#yatzyapiyatzyapicsproj)
  - [Yatzy.Application\Yatzy.Application.csproj](#yatzyapplicationyatzyapplicationcsproj)
  - [Yatzy.Domain\Yatzy.Domain.csproj](#yatzydomainyatzydomaincsproj)
  - [Yatzy.Infrastructure\Yatzy.Infrastructure.csproj](#yatzyinfrastructureyatzyinfrastructurecsproj)
  - [Yatzy.Persistence\Yatzy.Persistence.csproj](#yatzypersistenceyatzypersistencecsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 5 | All require upgrade |
| Total NuGet Packages | 2 | 1 need upgrade |
| Total Code Files | 2 |  |
| Total Code Files with Incidents | 5 |  |
| Total Lines of Code | 54 |  |
| Total Number of Issues | 6 |  |
| Estimated LOC to modify | 0+ | at least 0,0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| [Yatzy.Api\Yatzy.Api.csproj](#yatzyapiyatzyapicsproj) | net8.0 | 🟢 Low | 1 | 0 |  | AspNetCore, Sdk Style = True |
| [Yatzy.Application\Yatzy.Application.csproj](#yatzyapplicationyatzyapplicationcsproj) | net8.0 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [Yatzy.Domain\Yatzy.Domain.csproj](#yatzydomainyatzydomaincsproj) | net8.0 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [Yatzy.Infrastructure\Yatzy.Infrastructure.csproj](#yatzyinfrastructureyatzyinfrastructurecsproj) | net8.0 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [Yatzy.Persistence\Yatzy.Persistence.csproj](#yatzypersistenceyatzypersistencecsproj) | net8.0 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 1 | 50,0% |
| ⚠️ Incompatible | 0 | 0,0% |
| 🔄 Upgrade Recommended | 1 | 50,0% |
| ***Total NuGet Packages*** | ***2*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 105 |  |
| ***Total APIs Analyzed*** | ***105*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| Microsoft.AspNetCore.OpenApi | 8.0.26 | 9.0.15 | [Yatzy.Api.csproj](#yatzyapiyatzyapicsproj) | NuGet package upgrade is recommended |
| Swashbuckle.AspNetCore | 6.6.2 |  | [Yatzy.Api.csproj](#yatzyapiyatzyapicsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;Yatzy.Api.csproj</b><br/><small>net8.0</small>"]
    P2["<b>📦&nbsp;Yatzy.Application.csproj</b><br/><small>net8.0</small>"]
    P3["<b>📦&nbsp;Yatzy.Domain.csproj</b><br/><small>net8.0</small>"]
    P4["<b>📦&nbsp;Yatzy.Infrastructure.csproj</b><br/><small>net8.0</small>"]
    P5["<b>📦&nbsp;Yatzy.Persistence.csproj</b><br/><small>net8.0</small>"]
    P1 --> P5
    P1 --> P2
    P1 --> P4
    P2 --> P3
    P4 --> P2
    P4 --> P3
    P5 --> P2
    P5 --> P3
    click P1 "#yatzyapiyatzyapicsproj"
    click P2 "#yatzyapplicationyatzyapplicationcsproj"
    click P3 "#yatzydomainyatzydomaincsproj"
    click P4 "#yatzyinfrastructureyatzyinfrastructurecsproj"
    click P5 "#yatzypersistenceyatzypersistencecsproj"

```

## Project Details

<a id="yatzyapiyatzyapicsproj"></a>
### Yatzy.Api\Yatzy.Api.csproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net9.0
- **SDK-style**: True
- **Project Kind:** AspNetCore
- **Dependencies**: 3
- **Dependants**: 0
- **Number of Files**: 3
- **Number of Files with Incidents**: 1
- **Lines of Code**: 44
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["Yatzy.Api.csproj"]
        MAIN["<b>📦&nbsp;Yatzy.Api.csproj</b><br/><small>net8.0</small>"]
        click MAIN "#yatzyapiyatzyapicsproj"
    end
    subgraph downstream["Dependencies (3"]
        P5["<b>📦&nbsp;Yatzy.Persistence.csproj</b><br/><small>net8.0</small>"]
        P2["<b>📦&nbsp;Yatzy.Application.csproj</b><br/><small>net8.0</small>"]
        P4["<b>📦&nbsp;Yatzy.Infrastructure.csproj</b><br/><small>net8.0</small>"]
        click P5 "#yatzypersistenceyatzypersistencecsproj"
        click P2 "#yatzyapplicationyatzyapplicationcsproj"
        click P4 "#yatzyinfrastructureyatzyinfrastructurecsproj"
    end
    MAIN --> P5
    MAIN --> P2
    MAIN --> P4

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 105 |  |
| ***Total APIs Analyzed*** | ***105*** |  |

<a id="yatzyapplicationyatzyapplicationcsproj"></a>
### Yatzy.Application\Yatzy.Application.csproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net9.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 3
- **Number of Files**: 0
- **Number of Files with Incidents**: 1
- **Lines of Code**: 0
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (3)"]
        P1["<b>📦&nbsp;Yatzy.Api.csproj</b><br/><small>net8.0</small>"]
        P4["<b>📦&nbsp;Yatzy.Infrastructure.csproj</b><br/><small>net8.0</small>"]
        P5["<b>📦&nbsp;Yatzy.Persistence.csproj</b><br/><small>net8.0</small>"]
        click P1 "#yatzyapiyatzyapicsproj"
        click P4 "#yatzyinfrastructureyatzyinfrastructurecsproj"
        click P5 "#yatzypersistenceyatzypersistencecsproj"
    end
    subgraph current["Yatzy.Application.csproj"]
        MAIN["<b>📦&nbsp;Yatzy.Application.csproj</b><br/><small>net8.0</small>"]
        click MAIN "#yatzyapplicationyatzyapplicationcsproj"
    end
    subgraph downstream["Dependencies (1"]
        P3["<b>📦&nbsp;Yatzy.Domain.csproj</b><br/><small>net8.0</small>"]
        click P3 "#yatzydomainyatzydomaincsproj"
    end
    P1 --> MAIN
    P4 --> MAIN
    P5 --> MAIN
    MAIN --> P3

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="yatzydomainyatzydomaincsproj"></a>
### Yatzy.Domain\Yatzy.Domain.csproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net9.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 3
- **Number of Files**: 1
- **Number of Files with Incidents**: 1
- **Lines of Code**: 10
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (3)"]
        P2["<b>📦&nbsp;Yatzy.Application.csproj</b><br/><small>net8.0</small>"]
        P4["<b>📦&nbsp;Yatzy.Infrastructure.csproj</b><br/><small>net8.0</small>"]
        P5["<b>📦&nbsp;Yatzy.Persistence.csproj</b><br/><small>net8.0</small>"]
        click P2 "#yatzyapplicationyatzyapplicationcsproj"
        click P4 "#yatzyinfrastructureyatzyinfrastructurecsproj"
        click P5 "#yatzypersistenceyatzypersistencecsproj"
    end
    subgraph current["Yatzy.Domain.csproj"]
        MAIN["<b>📦&nbsp;Yatzy.Domain.csproj</b><br/><small>net8.0</small>"]
        click MAIN "#yatzydomainyatzydomaincsproj"
    end
    P2 --> MAIN
    P4 --> MAIN
    P5 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="yatzyinfrastructureyatzyinfrastructurecsproj"></a>
### Yatzy.Infrastructure\Yatzy.Infrastructure.csproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net9.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 1
- **Number of Files**: 0
- **Number of Files with Incidents**: 1
- **Lines of Code**: 0
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P1["<b>📦&nbsp;Yatzy.Api.csproj</b><br/><small>net8.0</small>"]
        click P1 "#yatzyapiyatzyapicsproj"
    end
    subgraph current["Yatzy.Infrastructure.csproj"]
        MAIN["<b>📦&nbsp;Yatzy.Infrastructure.csproj</b><br/><small>net8.0</small>"]
        click MAIN "#yatzyinfrastructureyatzyinfrastructurecsproj"
    end
    subgraph downstream["Dependencies (2"]
        P2["<b>📦&nbsp;Yatzy.Application.csproj</b><br/><small>net8.0</small>"]
        P3["<b>📦&nbsp;Yatzy.Domain.csproj</b><br/><small>net8.0</small>"]
        click P2 "#yatzyapplicationyatzyapplicationcsproj"
        click P3 "#yatzydomainyatzydomaincsproj"
    end
    P1 --> MAIN
    MAIN --> P2
    MAIN --> P3

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="yatzypersistenceyatzypersistencecsproj"></a>
### Yatzy.Persistence\Yatzy.Persistence.csproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net9.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 1
- **Number of Files**: 0
- **Number of Files with Incidents**: 1
- **Lines of Code**: 0
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P1["<b>📦&nbsp;Yatzy.Api.csproj</b><br/><small>net8.0</small>"]
        click P1 "#yatzyapiyatzyapicsproj"
    end
    subgraph current["Yatzy.Persistence.csproj"]
        MAIN["<b>📦&nbsp;Yatzy.Persistence.csproj</b><br/><small>net8.0</small>"]
        click MAIN "#yatzypersistenceyatzypersistencecsproj"
    end
    subgraph downstream["Dependencies (2"]
        P2["<b>📦&nbsp;Yatzy.Application.csproj</b><br/><small>net8.0</small>"]
        P3["<b>📦&nbsp;Yatzy.Domain.csproj</b><br/><small>net8.0</small>"]
        click P2 "#yatzyapplicationyatzyapplicationcsproj"
        click P3 "#yatzydomainyatzydomaincsproj"
    end
    P1 --> MAIN
    MAIN --> P2
    MAIN --> P3

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

