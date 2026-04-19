# Opret Visual Studio 2026 solution

## Formål
Denne fil bruges til at få GitHub Copilot til at hjælpe med at oprette den korrekte solution-struktur til et browser-baseret Yatzy-spil.

---

## Lokal placering
Solution skal oprettes her:

```text
C:\Users\Michael\Documents\MINEPROJEKTER\Yatzy
```

---

## Mappestruktur

```text
Yatzy/
  docs/
  src/
```

---

## Trin 1 – Opret solution
Opret solution i `src` med navn:

```text
Yatzy.sln
```

---

## Trin 2 – Opret .NET projekter
Opret følgende projekter:

### 1. Yatzy.Domain
- Type: Class Library
- Framework: .NET 9

### 2. Yatzy.Application
- Type: Class Library
- Framework: .NET 9

### 3. Yatzy.Infrastructure
- Type: Class Library
- Framework: .NET 9

### 4. Yatzy.Persistence
- Type: Class Library
- Framework: .NET 9

### 5. Yatzy.Api
- Type: ASP.NET Core Web API
- Framework: .NET 9

---

## Trin 3 – Opret Angular frontend
Opret Angular app i `src` med navn:

```text
yatzy-web
```

Anbefalet:
- standalone components
- routing
- strict TypeScript
- SCSS
- Angular services
- SignalR client

---

## Trin 4 – Projektreferencer
Tilføj disse referencer:

- `Yatzy.Application` refererer `Yatzy.Domain`
- `Yatzy.Infrastructure` refererer `Yatzy.Application` og `Yatzy.Domain`
- `Yatzy.Persistence` refererer `Yatzy.Application` og `Yatzy.Domain`
- `Yatzy.Api` refererer `Yatzy.Application`, `Yatzy.Infrastructure`, `Yatzy.Persistence`

---

## Trin 5 – Basis-foldere i projekterne

### Yatzy.Domain
```text
Entities/
Enums/
ValueObjects/
Rules/
Interfaces/
Exceptions/
```

### Yatzy.Application
```text
Abstractions/
DTOs/
Features/
Commands/
Queries/
Services/
Mappings/
Validators/
```

### Yatzy.Infrastructure
```text
Services/
Random/
Time/
Configuration/
```

### Yatzy.Persistence
```text
Context/
Entities/
Configurations/
Repositories/
Migrations/
```

### Yatzy.Api
```text
Controllers/
Hubs/
Contracts/
Extensions/
Middleware/
```

### yatzy-web
```text
src/app/core/
src/app/core/services/
src/app/core/models/
src/app/features/lobby/
src/app/features/game/
src/app/features/results/
src/app/shared/
src/app/shared/components/
src/app/shared/models/
src/app/shared/enums/
```

---

## Trin 6 – Nødvendige NuGet pakker

### Yatzy.Api
- Microsoft.AspNetCore.SignalR
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore

### Yatzy.Persistence
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design

### Testprojekter senere
- xUnit
- FluentAssertions
- Moq

---

## Trin 7 – Angular pakker
Installer:
- `@microsoft/signalr`

Senere eventuelt:
- state management hvis nødvendigt
- component library kun hvis du virkelig har brug for det

---

## Copilot-prompt
Brug denne prompt til Copilot:

> Create a Visual Studio 2026 solution for an online multiplayer Yatzy browser game. Use .Net 9 for backend and class libraries, Angular + TypeScript for frontend, SignalR for realtime gameplay, and Entity Framework Core for persistence. Create the projects, folder structure, project references, and initial boilerplate files exactly as described in this markdown file.

---

## Done-kriterier
Denne fase er færdig når:
- solution findes
- alle projekter findes
- Angular app findes
- referencer er sat korrekt
- projekter kan buildes uden domænelogik endnu
