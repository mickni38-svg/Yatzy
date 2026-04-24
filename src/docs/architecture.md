# Arkitektur

Yatzy er bygget efter **Clean Architecture**-princippet, hvor afhængigheder altid peger indad mod domænet — aldrig udad.

---

## Lagdeling

```
┌─────────────────────────────────────────────┐
│              Yatzy.Api                      │  ← HTTP / SignalR / WebRTC
│   (Controllers, Hubs, Middleware)           │
├─────────────────────────────────────────────┤
│           Yatzy.Application                 │  ← Use case-orkestrering
│   (AppServices, DTOs, Interfaces)           │
├─────────────────────────────────────────────┤
│             Yatzy.Domain                    │  ← Forretningslogik & regler
│   (Entities, Rules, Enums, Exceptions)      │
├──────────────┬──────────────────────────────┤
│Yatzy.Persist.│  Yatzy.Infrastructure        │  ← Ydre afhængigheder
│(EF Core, DB) │  (Tilfældighedsgenerator m.m.)│
└──────────────┴──────────────────────────────┘

         Angular 19 (yatzy-web)
         ↕ SignalR + REST
```

### Afhængighedsretning

- **Domain** kender ingen af de andre lag
- **Application** kender kun Domain (via interfaces)
- **Api** kender Application og registrerer konkrete implementeringer
- **Persistence** og **Infrastructure** implementerer Application-interfaces

---

## Projektstruktur

```
Yatzy/
├── src/
│   ├── Yatzy.Domain/           ← Entiteter og regler
│   ├── Yatzy.Application/      ← Use cases og DTOs
│   ├── Yatzy.Api/              ← ASP.NET Core API + SignalR Hubs
│   ├── Yatzy.Persistence/      ← EF Core + SQL-database
│   ├── Yatzy.Infrastructure/   ← Infrastrukturtjenester
│   └── yatzy-web/              ← Angular frontend
│       └── src/app/
│           ├── core/           ← Services (SignalR, WebRTC)
│           ├── features/       ← Sider (lobby, game)
│           └── shared/         ← Genbrugelige komponenter
└── docs/                       ← Denne dokumentation
```

---

## Dataflow — en spillerrunde

```
[Spiller klikker "Slå"]
        │
        ▼
[game.component.ts: rollDice()]
        │  SignalR
        ▼
[GameHub.RollDice(request)]
        │
        ▼
[GameplayAppService.RollDiceAsync]
        │
        ▼
[Game.Roll() — terningværdier randomiseres i Domain]
        │
        ▼
[GameRepository.SaveChanges()]
        │
        ▼
[GameHubService.BroadcastDiceRolledAsync]
        │  SignalR → alle spillere i gruppen
        ▼
[game-realtime.service.ts: gameState$ emitter]
        │
        ▼
[game.component.ts opdaterer UI + starter animation]
```

---

## Databasemodel

Spildata gemmes i en relationsdatabase via Entity Framework Core:

```
Game              (Id, RoomCode, Status, CurrentPlayerIndex, RoundNumber, RollNumber)
  ├── Player[]    (Id, GameId, DisplayName, JoinOrder, IsConnected, HasLeft)
  │     └── ScoreEntry[]  (PlayerId, Category, Score, IsUsed)
  └── Dice[]      (Id, GameId, Position, Value, IsHeld)
```

Migrationer:
- `InitialCreate` — basisskema
- `AddPlayerHasLeft` — tilføjer `HasLeft`-kolonne

---

## Sikkerhed og validering

- **Host-validering:** Kun første spiller (JoinOrder = 0) kan starte spil og sende Yatzy-fejringer
- **Tur-validering:** `GameplayAppService` validerer at det er den rigtige spillers tur
- **Domain-exceptions:** `DomainException`, `ValidationException`, `NotFoundException` fanges i `ExceptionHandlingMiddleware`
- **CORS:** Kun tilladt fra `localhost:4200` i development; produktionsdomain i production

---

## Se også

- [Backend — detaljeret beskrivelse](backend.md)
- [Frontend — detaljeret beskrivelse](frontend.md)
- [Real-time kommunikation](realtime.md)
