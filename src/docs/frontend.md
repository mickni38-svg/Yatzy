# Frontend

Teknisk beskrivelse af Angular-applikationen, komponenter, services og routing.

---

## Projektstruktur

```
yatzy-web/src/app/
├── core/
│   └── services/
│       ├── game-realtime.service.ts   ← SignalR-klient
│       └── webrtc.service.ts          ← WebRTC peer-to-peer
├── features/
│   ├── lobby/
│   │   ├── lobby.component.ts
│   │   ├── lobby.component.html
│   │   └── lobby.component.scss
│   └── game/
│       ├── game.component.ts
│       ├── game.component.html
│       └── game.component.scss
└── shared/
    └── components/
        └── score-sheet/
            ├── score-sheet.component.ts
            ├── score-sheet.component.html
            └── score-sheet.component.scss
```

---

## Routing

| Route | Komponent | Beskrivelse |
|---|---|---|
| `/` | `LobbyComponent` | Startside — opret eller join spil |
| `/game` | `GameComponent` | Spilsiden (navigeres til efter GameStarted) |

State videresendes via Angular Router's `state`-parameter (ikke URL-parametre):
```typescript
router.navigate(['/game'], {
  state: { gameId, roomCode, playerId, displayName }
});
```

---

## LobbyComponent

`src/yatzy-web/src/app/features/lobby/lobby.component.ts`

**Ansvar:** Formular til oprettelse og joining af spil. Kalder REST API, navigerer til `/game` ved `GameStarted`.

**Flows:**
1. Bruger udfylder navn + vælger "Opret" → `POST /api/games` → gem `gameId`/`playerId` → opret SignalR-forbindelse → vent på andre spillere
2. Bruger udfylder navn + rum-kode → `POST /api/games/{roomCode}/join` → gem state → forbind SignalR
3. Host klikker Start → `gameRealtime.startGame(gameId)` → server sender `GameStarted` → naviger til `/game`

---

## GameComponent

`src/yatzy-web/src/app/features/game/game.component.ts`

Hovedkomponenten for selve spillet. Håndterer al visuel spilstate.

### Vigtige properties

```typescript
game: GameStateResponse          // Aktuelt spilstate fra server
diceSpinning: boolean            // True mens terningsanimation kører
showGifOverlay: boolean          // Viser GIF-overlayet
activeGif: GifConfig | null      // Den aktive GIF
gifList: GifConfig[]             // Liste fra gif-config.json
```

### Getters

```typescript
get activePlayers()     // Filtrerer p.hasLeft === true
get isMyTurn()          // playerId === game.currentPlayerId
get isIAmHost()         // Første spiller (index 0) er den lokale spiller
get canRoll()           // isMyTurn && rollNumber < 3 && !diceSpinning
```

### Centrale metoder

#### `rollDice()`
Kalder `gameRealtime.rollDice()`. Lokalt startes animationen via `_triggerAnimation()` straks (responsivitet).

#### `_triggerAnimation(dice, isRoller)`
Starter CSS-animationer for alle terninger. Sætter `diceSpinning = true`. Stopper animationerne efter beregnet forsinkelse og kalder `_checkYatzyOnDice()`.

```typescript
// Animationsvarighed baseret på antal slag
const stopInterval = isRoller ? Math.max(800, rollNumber * 400) : 1200;
```

#### `_checkYatzyOnDice()`
Kaldes når animationen stopper. Tjekker om alle 5 terninger viser samme værdi OG `Yatzy`-kategorien ikke er brugt. Hvis ja → `triggerYatzyCelebration(myPlayerId, true)`.

#### `triggerYatzyCelebration(playerId, sendToServer)`
Viser GIF lokalt og sender `TriggerYatzy` til serveren (hvis `sendToServer = true`), så alle spillere ser den samme fejring.

```typescript
triggerYatzyCelebration(playerId: string, sendToServer: boolean) {
  const gif = this._pickRandomGif();
  this._showGif(playerId, gif.file);
  if (sendToServer) {
    this.gameRealtime.triggerYatzy(playerId, gif.file);
  }
}
```

#### `leaveGame()`
Sekventiel oprydning:
1. Afmelder alle subscriptions
2. `await webrtc.stop()` — stopper kamera/mikrofon
3. `gameRealtime.leaveGame(gameId, playerId)` — notificerer server
4. `await gameRealtime.stop()` — lukker SignalR-forbindelsen
5. `router.navigate(['/'])` — tilbage til lobby

#### `showYatzyOverlay(overlayEnabled)`
Viser/skjuler YATZY-tekst-overlay oven på GIF'en (styret af `showOverlay`-feltet i `gif-config.json`).

---

### Subscription setup (ngOnInit)

```typescript
// Spilstate-opdateringer
gameRealtime.gameState$.subscribe(state => {
  this.game = state;
  // Hvis andre spiller ruller: start animation lokalt
});

// Yatzy-fejring fra server
gameRealtime.yatzy$.subscribe(({ playerId, gifName }) => {
  this.triggerYatzyCelebration(playerId, false);
});

// GIF-konfiguration
http.get<GifConfig[]>('/gif-config.json').subscribe(list => {
  this.gifList = list;
});
```

---

## ScoreSheetComponent

`src/yatzy-web/src/app/shared/components/score-sheet/score-sheet.component.ts`

Viser scorearket for alle spillere som tabel.

**Inputs:**
```typescript
@Input() players: PlayerDto[]    // Alle spillere med scores
@Input() myPlayerId: string      // Fremhæver lokal spillers kolonne
@Input() currentPlayerId: string // Fremhæver aktiv spillers tur
@Input() spinning: boolean       // Skjuler scoreforslag mens terninger animeres
```

**`getSuggestion(category, player)`:**
Beregner og viser en forhåndsvisning af hvad spilleren ville score i en kategori — returnerer `null` mens `spinning = true` for at undgå forstyrrende opdateringer under animation.

**Bonus-badge:** Viser øvre sektion-sum og fremhæver med ring når bonus er opnået (≥ 63 point).

---

## WebRtcService

`src/yatzy-web/src/app/core/services/webrtc.service.ts`

Håndterer al WebRTC-logik:

1. `getUserMedia()` — anmoder om kamera/mikrofon
2. Opretter `RTCPeerConnection` per remote spiller
3. Håndterer `offer`/`answer`/`ICE candidate`-udveksling via `VideoHub`
4. Eksponerer `localStream$` og `remoteStreams$` til template-binding

**`stop()`** lukker alle peer-connections, stopper alle MediaStreamTracks og disconnecter fra `VideoHub`.

---

## Video Grid

Vises øverst på spilsiden. Itererer `activePlayers` og viser:
- Lokal spiller: binder `localStream$` til `<video>`-element
- Remote spillere: binder fra `remoteStreams$`-map

Spillere der har forladt (`hasLeft = true`) filtreres fra via `activePlayers`-getteren.

---

## Se også

- [Real-time & SignalR](realtime.md)
- [GIF-systemet](gif-system.md)
- [Use Cases](use-cases.md)
