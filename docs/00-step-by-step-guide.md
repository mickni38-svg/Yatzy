# Yatzy online i browser – step by step guide

## Formål
Denne guide beskriver, hvordan du opbygger en **browser-baseret Yatzy-løsning** i stedet for den tidligere MAUI Android-løsning.

Målet er en løsning med denne stack:

- **Frontend:** Angular + TypeScript
- **Service-lag i frontend:** Angular services til API- og SignalR-kald
- **API-lag:** ASP.NET Core 9 Web API
- **Logic-lag:** C# class libraries
- **Database-lag:** Entity Framework Core
- **Realtime multiplayer:** SignalR
- **Målplatform:** Chrome browser

Denne guide er skrevet, så du kan lægge filerne ind i repoet og derefter arbejde **trin for trin med GitHub Copilot i Visual Studio 2026**.

---

## Vigtigt først

jeg har lavet alle de `.md`-filer, du kan lægge i repoet og bruge som Copilot-grundlag.

---

## Anbefalet løsningsstruktur

```text
Yatzy/
  docs/
    00-step-by-step-guide.md
    01-solution-architecture.md
    02-create-visual-studio-solution.md
    03-domain-and-logic-layer.md
    04-database-and-entity-framework.md
    05-api-layer.md
    06-realtime-signalr.md
    07-angular-frontend.md
    08-ui-ux-from-mockup.md
    09-testing-strategy.md
    10-copilot-master-prompt.md
    11-copilot-prompt-phase-1.md
    12-copilot-prompt-phase-2.md
    13-copilot-prompt-phase-3.md
    14-copilot-prompt-phase-4.md
  src/
    Yatzy.sln
    Yatzy.Api/
    Yatzy.Application/
    Yatzy.Domain/
    Yatzy.Infrastructure/
    Yatzy.Persistence/
    yatzy-web/
```

---

## Foreslået projektopdeling

### 1. Yatzy.Domain
Ren domænelogik og kernemodeller:
- Game
- Player
- Dice
- ScoreSheet
- ScoreCategory
- regler for scoring

### 2. Yatzy.Application
Use cases og forretningslogik:
- CreateGame
- JoinGame
- StartGame
- RollDice
- ToggleHold
- SelectScore
- AdvanceTurn

### 3. Yatzy.Infrastructure
Tværgående services:
- random provider
- tidsprovider
- event publishing abstractions

### 4. Yatzy.Persistence
Entity Framework Core:
- DbContext
- entity mappings
- repositories
- migration setup

### 5. Yatzy.Api
ASP.NET Core 9:
- controllers
- SignalR hub
- dependency injection
- authentication senere hvis nødvendigt

### 6. yatzy-web
Angular frontend:
- pages
- components
- services
- models
- SignalR client
- responsive UI til Chrome

---

## Arbejdsrækkefølge

## Fase 1 – Opret solution og grundstruktur
Læs og brug:
- `01-solution-architecture.md`
- `02-create-visual-studio-solution.md`

Mål:
- opret solution
- opret projekter
- opret references
- opret Angular projekt
- commit basisstruktur

---

## Fase 2 – Byg domæne og regelmotor
Læs og brug:
- `03-domain-and-logic-layer.md`

Mål:
- definer game state
- definer score categories
- implementér score calculator
- implementér turn rules
- skriv unit tests

---

## Fase 3 – Byg database og persistens
Læs og brug:
- `04-database-and-entity-framework.md`

Mål:
- opret DbContext
- entities
- mapping
- migrations
- lagring af game state

---

## Fase 4 – Byg API-lag
Læs og brug:
- `05-api-layer.md`

Mål:
- REST endpoints
- DTO’er
- controller struktur
- validation
- dependency injection

---

## Fase 5 – Byg realtime multiplayer
Læs og brug:
- `06-realtime-signalr.md`

Mål:
- GameHub
- room groups
- broadcast events
- reconnect strategy
- autoritativ server state

---

## Fase 6 – Byg Angular frontend
Læs og brug:
- `07-angular-frontend.md`
- `08-ui-ux-from-mockup.md`

Mål:
- lobby
- game screen
- score selection
- standings
- Angular services for API + SignalR

---

## Fase 7 – Test og stabilisering
Læs og brug:
- `09-testing-strategy.md`

Mål:
- unit tests
- integration tests
- multiplayer flow tests
- reconnect tests

---

## Sådan arbejder du med GitHub Copilot
Brug ikke alle prompts på én gang.

Arbejd i denne rækkefølge:

1. upload eller læg alle `.md`-filer i repoet under `docs/`
2. lad Copilot læse `10-copilot-master-prompt.md`
3. start med `11-copilot-prompt-phase-1.md`
4. når fase 1 er færdig, gå videre til fase 2
5. fortsæt sekventielt frem til fase 4

Det gør arbejdet mere overskueligt og reducerer risikoen for, at Copilot springer for hurtigt frem.

---

## Helt konkret i Visual Studio 2026
1. Opret mappen `C:\Users\Michael\Documents\MINEPROJEKTER\Yatzy`
2. Læg disse `.md`-filer i en `docs` mappe
3. Opret solution og projekter som beskrevet i `02-create-visual-studio-solution.md`
4. Åbn repoet i Visual Studio 2026
5. Brug Copilot Chat i workspace-kontekst
6. Giv Copilot én fase ad gangen
7. Review og commit efter hver fase

---

## Designretning fra de uploadede filer
Den tidligere MAUI-guide beskrev en løsning med:
- scorekort som centrum
- op til 4 spillere
- 3 kast
- hold-funktion
- realtime state
- serverbaseret validering fileciteturn0file1L14-L23

README-filen beskrev også en opdeling mellem delt logik, backend og UI, som nu omsættes til browserarkitektur i stedet for MAUI fileciteturn0file0L5-L17

---

## Anbefaling
Byg først:
- domænelogik
- API
- SignalR
- browser-UI

Vent med videochat, login og avancerede features til senere iterationer.
