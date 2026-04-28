# Angular frontend

## Formål
Frontend er implementeret i Angular 19 og TypeScript og kører i browseren.

Den er enkel, modulær og opdelt i et tydeligt service-lag.

---

## Hovedprincip

- Komponenter viser UI
- Services kalder backend
- Services holder SignalR connection
- Komponenter kender ikke HTTP-detaljer

---

## Angular struktur

```text
src/app/
  core/
    models/         – GameStateDto, PlayerDto, DiceDto, ScoreEntryDto
    services/       – GameApiService, GameRealtimeService, PlayerSessionService, DiceSoundService, WebRtcService
  features/
    lobby/          – LobbyComponent (opret/join spil)
    game/           – GameComponent (hovedelspilsskærm)
    results/        – ResultsComponent (slutstilling)
  shared/
    components/
      dice-tile/    – Én terning med pip-SVG og rolling-animation
      dice-tray/    – Container for 5 terninger
      score-sheet/  – Scorekort med kategori-valg
      player-list/  – Spillerliste
      turn-banner/  – Aktiv-spiller-banner
    enums/          – GameStatus, ScoreCategory
```

---

## Services

### GameApiService
- create game
- join game
- get game state (HTTP)

### GameRealtimeService
Ansvar: SignalR connection og event-streams.

Eksponerede observables:

| Observable | Type | Beskrivelse |
|---|---|---|
| `gameState$` | `GameStateDto` | Opdateret game state |
| `diceRolling$` | `number[]` | Terning-positioner der ruller |
| `diceRolled$` | `GameStateDto` | Endelig state efter kast |
| `error$` | `string` | Fejlbeskeder |
| `yatzy$` | `{ playerId, gifName }` | Yatzy-fejring |
| `connected$` | `boolean` | Forbindelsesstatus |

`applyState(state)` bruges til manuelt at opdatere `gameState$` efter animation.

### PlayerSessionService
- gem og restore `playerId`, `gameId`, `roomCode` i sessionStorage

### DiceSoundService
- `startSpin()` / `stopSpin()` – løbende kastelyd
- `playBing()` – lyd når én terning lander

---

## Terning-animationsflow i GameComponent

```
DiceRolling-event modtages (NgZone.run)
    ↓
_startSpin(rollingPositions) kaldes
    ↓
isAnimating = true, diceSpinning = true
    ↓
setInterval: animatedValues opdateres hvert 80ms
    ↓
DiceRolled-event modtages → _pendingRolledState gemmes
    ↓
Efter 3 sekunder: interval stoppes
    ↓
Terningerne reveales én ad gangen (1 sekund interval)
    animatedValues[i] = finalValues[i], rollingDice[i] = false
    ↓
isAnimating = false, applyState(finalState)
```

Template bruger `isAnimating` til at vælge kilde:
```html
[value]="isAnimating ? animatedValues[i] : die.value"
```

`NgZone.run()` er påkrævet så change detection kører for alle klienter (ikke kun kasteren).

---

## Sider

### LobbyPage
- Opret spil / join med room code
- Vis spillerliste
- Host kan starte spil

### GamePage
- Aktiv spiller og runde
- 5 terninger med spin-animation og hold-knapper
- Scorekort med kategori-forslag
- Kamera-grid (WebRTC)
- Yatzy-fejrings-GIF overlay

### ResultPage
- Slutstilling med vinder
- Link til nyt spil

---

## TypeScript interfaces

```typescript
interface GameStateDto { gameId, roomCode, status, roundNumber, rollNumber, currentPlayerId, players, dice }
interface PlayerDto    { playerId, displayName, isHost, isConnected, hasLeft, totalScore, scoreEntries }
interface DiceDto      { position, value, isHeld }
interface ScoreEntryDto { category, score, isUsed }
```

---

## UI-regler
- Grøn Yatzy-identitet
- Tydelig aktiv spiller
- Tydelig hold-markering
- Tydelig remaining rolls
- Scorekort centralt og let læseligt

---

## Done-kriterier

- [x] Man kan oprette og joine spil fra browseren
- [x] Game state vises korrekt
- [x] Kast og hold kan udføres via UI
- [x] Scorevalg fungerer
- [x] Realtime events opdaterer alle klienter synkroniseret
- [x] Terning-spin-animation kører i 3 sekunder på alle klienter
- [x] Terningerne reveales én ad gangen med 1 sekund interval
- [x] Kameravisning via WebRTC
- [x] Yatzy-fejring med GIF
