# 🎲 Yatzy – Multiplayer Online Yatzy

Et fullstack multiplayer Yatzy-spil bygget med **ASP.NET Core 9** (backend) og **Angular** (frontend), med realtidskommunikation via **SignalR**.

---

## 📐 Arkitektur

Løsningen følger **Clean Architecture** med følgende lag:

```
Yatzy/
├── src/
│   ├── Yatzy.Api              # ASP.NET Core Web API + SignalR Hubs
│   ├── Yatzy.Application      # Use cases, services, DTOs, interfaces
│   ├── Yatzy.Domain           # Domæneentiteter, regler, exceptions
│   ├── Yatzy.Infrastructure   # Eksterne services (f.eks. room code generator)
│   ├── Yatzy.Persistence      # EF Core, migrationer, repository
│   ├── Yatzy.Api.Tests        # Integrationstests for API
│   ├── Yatzy.Domain.Tests     # Unit tests for domænelogik
│   └── yatzy-web/             # Angular frontend
```

### Lagenes ansvar

| Lag | Ansvar |
|---|---|
| **Domain** | Kernelogik: `Game`, `Player`, `Dice`, `ScoreSheet`, `ScoreCalculator` – ingen afhængigheder |
| **Application** | Orkestrerer use cases via services og interfaces. Definerer DTOs |
| **Infrastructure** | Implementerer interfaces fra Application (f.eks. room code generering) |
| **Persistence** | EF Core + SQL Server. Repository-pattern. Kører migrationer ved opstart |
| **API** | REST-endpoints + SignalR hubs. Middleware til fejlhåndtering |
| **Frontend** | Angular SPA med SignalR-klient og WebRTC til video |

---

## 🚀 Teknologier

| Teknologi | Formål |
|---|---|
| ASP.NET Core 9 | Backend REST API |
| SignalR | Realtidskommunikation (spil events) |
| Entity Framework Core 9 | ORM til SQL Server |
| Angular | Frontend SPA |
| WebRTC | Peer-to-peer video/lyd i lobbyen |
| Swagger | API dokumentation (Development) |

---

## 🎮 Spilflow

```
1. Spiller A opretter et spil  →  POST /api/games
2. Spiller B joiner via roomcode  →  POST /api/games/{roomCode}/join
3. Begge forbinder til SignalR  →  /hubs/game (JoinRoom)
4. Host starter spillet  →  POST /api/games/{gameId}/start
5. Aktiv spiller kaster terninger  →  SignalR: RollDice
6. Spiller vælger at holde terninger  →  SignalR: ToggleHold
7. Spiller registrerer score  →  SignalR: RegisterScore
8. Tur skifter automatisk – gentages til alle kategorier er udfyldt
9. Spil slut  →  GameOver event udsendes med endelig score
```

---

## 🌐 API Endpoints

### Games – `POST /api/games`
Opretter et nyt spil og returnerer `GameStateResponse` med `roomCode`.

### Join – `POST /api/games/{roomCode}/join`
Tilføjer en spiller til spillet. Broadcaster `PlayerJoined` til alle i rummet.

### Start – `POST /api/games/{gameId}/start`
Starter spillet (kræver minimum 2 spillere).

### Get – `GET /api/games/{gameId}`
Henter aktuel spiltilstand.

### Get by room – `GET /api/games/by-room/{roomCode}`
Henter spiltilstand via roomcode.

---

## 📡 SignalR Hub – `/hubs/game`

| Client → Server | Beskrivelse |
|---|---|
| `JoinRoom(roomCode, playerId)` | Forbinder spiller til SignalR-gruppe |
| `RollDice(roomCode, playerId)` | Kaster ikke-holdte terninger (max 3 kast pr. tur) |
| `ToggleHold(roomCode, playerId, diceIndex)` | Holder/frigiver en terning |
| `RegisterScore(roomCode, playerId, category)` | Registrerer score for en kategori |
| `LeaveGame(roomCode, playerId)` | Spiller forlader spillet |

| Server → Client | Beskrivelse |
|---|---|
| `GameStateUpdated` | Fuld spiltilstand efter hver handling |
| `PlayerJoined` | En ny spiller har joined |
| `GameOver` | Spillet er afsluttet med final score |
| `Error` | Fejlbesked til klienten |

---

## 🏗️ Domænemodel

### `Game`
- Maks **6 spillere**, minimum **2** for at starte
- **5 terninger**, maks **3 kast** pr. tur
- Holder styr på `CurrentPlayerIndex`, `RoundNumber`, `RollNumber`
- Status: `WaitingForPlayers` → `ReadyToStart` → `InProgress` → `Finished`

### `Player`
- Har en `ScoreSheet` med alle Yatzy-kategorier
- Tracker `IsConnected` og `HasLeft`

### `ScoreSheet` / `ScoreCalculator`
- Beregner point for alle 15 standard Yatzy-kategorier
- Inkl. bonus (øvre sektion ≥ 63 point)

---

## 🖥️ Frontend (Angular)

```
yatzy-web/src/app/
├── core/
│   ├── models/          # DTOs (game-state.dto.ts m.fl.)
│   └── services/        # game-realtime.service, webrtc.service, dice-sound.service
├── features/
│   ├── lobby/           # Lobby-side (opret/join spil, video)
│   └── game/            # Spil-side (terninger, scoresheet, spillerliste)
└── shared/
    └── components/      # dice-tile, score-sheet, player-list, turn-banner, jitsi
```

- Kommunikerer med backend via **REST** (opret/join) og **SignalR** (live events)
- **WebRTC / Jitsi** integration til video i lobbyen
- **Dice sound service** spiller lyde ved kast

---

## ⚙️ Kørsel lokalt

### Forudsætninger
- .NET 9 SDK
- Node.js + npm
- SQL Server (LocalDB er nok til development)

### Backend
```bash
cd src
dotnet run --project Yatzy.Api
```
> API starter på `https://localhost:7xxx` – migrationer køres automatisk ved opstart.
> Swagger UI tilgængelig på `/swagger` i Development.

### Frontend
```bash
cd src/yatzy-web
npm install
ng serve
```
> Angular starter på `http://localhost:4200`

---

## 🧪 Tests

```bash
# Unit tests (domænelogik)
dotnet test src/Yatzy.Domain.Tests

# Integrationstests (API)
dotnet test src/Yatzy.Api.Tests
```

---

## 🚢 Deployment

Løsningen er konfigureret til deployment på **Azure App Service** (`paybysharepay.dk`).

- Angular bygges og kopieres til `Yatzy.Api/wwwroot/` (serves som statiske filer)
- API og frontend serveres fra samme origin i produktion
- Database migrationer køres automatisk ved opstart
- Deploy-script: `src/deploy-azure.ps1`
