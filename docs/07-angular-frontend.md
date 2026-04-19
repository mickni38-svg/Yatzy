# Angular frontend

## Formål
Frontend skal implementeres i Angular og TypeScript og køre i Chrome.

Den skal være enkel, modulær og nem at udvide.

---

## Hovedprincip
Frontend skal have et tydeligt service-lag.

Det betyder:
- komponenter viser UI
- services kalder backend
- services holder SignalR connection
- komponenter må ikke selv kende HTTP detaljer

---

## Forslået Angular struktur

```text
src/app/
  core/
    models/
    services/
    guards/
    interceptors/
  features/
    lobby/
    game/
    results/
  shared/
    components/
    enums/
    models/
```

---

## Services der bør findes

### GameApiService
Ansvar:
- create game
- join game
- start game
- get game state

### GameRealtimeService
Ansvar:
- SignalR connect
- join room
- lyt til events
- send roll/toggle/select

### PlayerSessionService
Ansvar:
- gem player id lokalt
- gem game id eller room code lokalt
- restore session ved refresh

---

## Sider
Byg mindst disse pages:

### LobbyPage
Vis:
- room code
- spillere
- ready/start status
- knap til at starte spil

### GamePage
Vis:
- aktiv spiller
- runde
- kast nummer
- 5 terninger
- hold knapper
- scorekort
- kategori-valg

### ResultPage
Vis:
- slutstilling
- vinder
- nyt spil / tilbage til lobby

---

## Komponenter
Byg små genbrugelige komponenter:

- `dice-tray`
- `dice-tile`
- `score-sheet`
- `player-list`
- `turn-banner`
- `category-selector`
- `standings-bar`

---

## State-model i frontend
Hold en central state for game screen:
- game id
- room code
- players
- current player
- dice
- roll number
- selected suggestions
- scoreboard

---

## TypeScript interfaces
Definér interfaces der matcher backend DTO’er.

Eksempler:
- `GameStateDto`
- `PlayerDto`
- `DiceDto`
- `ScoreEntryDto`

---

## UI-regler
- grøn Yatzy-identitet
- tydelig aktiv spiller
- tydelig hold-markering
- tydelig remaining rolls
- scorekort skal være centralt og let læseligt

---

## Angular HTTP
Brug `HttpClient`.

Base URL skal ligge i environment.

---

## SignalR i Angular
Brug `@microsoft/signalr`.

Realtime service skal:
- starte forbindelse
- genforbinde automatisk
- gen-joine room efter reconnect
- pushe events ind i RxJS streams

---

## Copilot-prompt
> Implement the Angular frontend for the Yatzy browser game. Use a clean service layer in TypeScript for HTTP and SignalR communication. Build lobby, game page, result page, reusable shared components, and models matching the ASP.NET Core backend DTOs.

---

## Done-kriterier
Fasen er færdig når:
- man kan oprette/joine spil fra browseren
- game state vises korrekt
- kast og hold kan udføres via UI
- scorevalg fungerer
- realtime events opdaterer browseren
