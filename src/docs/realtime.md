# Real-time kommunikation

Beskrivelse af SignalR-hubs, alle events og WebRTC-videoflow.

---

## GameHub

[`Yatzy.Api/Hubs/GameHub.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs) — monteret på `/hubs/game`

Alle gameplay-handlinger går igennem denne hub. Klienten kalder server-metoder, og serveren broadcaster til hele gruppen (rum-koden bruges som SignalR-gruppe).

---

### Klient → Server metoder

#### `JoinRoom`
```
JoinRoom(roomCode: string, playerId: Guid)
```
Tilmelder klientens connection til SignalR-gruppen for rummet og registrerer forbindelsen i `ConnectionService`. Hvis spilleren genforbinder, sendes fuldt spilstate.

> [Se implementering → L39](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L39)

#### `StartGame`
```
StartGame({ GameId: Guid })
```
Kun host (første spiller). Starter spillet og broadcaster `GameStarted` til alle.

> [Se implementering → L72](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L72)

#### `RollDice`
```
RollDice({ GameId: Guid, PlayerId: Guid })
```
Ruller ikke-holdte terninger. Broadcaster `DiceRolled` til alle spillere **parallelt** med DB-skrivning for minimal forsinkelse.

> [Se implementering → L105](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L105) · [Optimeringsdetaljer](../docs/backend.md#-performance-optimering--rolldicefastasync)

#### `ToggleHold`
```
ToggleHold({ GameId: Guid, PlayerId: Guid, Position: number })
```
Holder eller frigiver én terning. Broadcaster `HoldChanged` til alle.

> [Se implementering → L120](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L120)

#### `SelectScore`
```
SelectScore({ GameId: Guid, PlayerId: Guid, Category: string })
```
Registrerer valgt kategori. Broadcaster `ScoreSelected` (eller `GameEnded` hvis spillet er slut) til alle.

> [Se implementering → L131](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L131)

#### `TriggerYatzy`
```
TriggerYatzy(targetPlayerId: Guid, gifName: string)
```
Kun host. Broadcaster `TriggerYatzy`-eventet med GIF-navn til alle spillere i rummet.

> [Se implementering → L148](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L148)

#### `LeaveGame`
```
LeaveGame({ GameId: Guid, PlayerId: Guid })
```
Markerer spilleren som forladt. Broadcaster `PlayerLeft` (eller `GameEnded` hvis host forlader) til alle.

> [Se implementering → L172](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L172)

---

### Server → Klient events

Defineret i [`Yatzy.Api/Hubs/HubEvents.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/HubEvents.cs):

| Event navn | Payload | Hvornår |
|---|---|---|
| `GameStateUpdated` | `GameStateResponse` | Genforbind — sender state kun til caller |
| `PlayerJoined` | `GameStateResponse` | Ny spiller joiner lobby |
| `PlayerLeft` | `GameStateResponse` | Spiller forlader |
| `GameStarted` | `GameStateResponse` | Host starter spillet |
| `DiceRolled` | `GameStateResponse` | Terninger rullet |
| `HoldChanged` | `GameStateResponse` | Terning holdt/frigivet |
| `ScoreSelected` | `GameStateResponse` | Kategori valgt, tur skiftet |
| `GameEnded` | `GameStateResponse` | Alle kategorier udfyldt |
| `TriggerYatzy` | `{ playerId, gifName }` | Yatzy-fejring (auto eller host) |
| `Error` | `string` | Fejlbesked til klient |

Alle events undtagen `GameStateUpdated` og `TriggerYatzy` sender hele `GameStateResponse` som payload.

---

### Disconnect-håndtering

Når en SignalR-forbindelse afbrydes (`OnDisconnectedAsync`), sættes spilleren som ikke-forbundet (`IsConnected = false`) i spilstaten. Ved genforbind (nyt `JoinRoom`-kald) sættes spilleren som forbundet igen og modtager det fulde spilstate.

---

## Angular SignalR-klient

[`game-realtime.service.ts`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts)

Wrapper om SignalR JS-klienten. Eksponerer RxJS-observables som komponenter kan subscribere på.

**Subjects/Observables:**
```typescript
gameState$: BehaviorSubject<GameStateDto | null>   // Alt spilstate
yatzy$:     Subject<{ playerId: string, gifName: string }>
```

**Metoder:**

| Metode | Linje |
|---|---|
| [`start()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L46) | L46 |
| [`stop()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L53) | L53 |
| [`joinRoom()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L61) | L61 |
| [`rollDice()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L71) | L71 |
| [`toggleHold()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L75) | L75 |
| [`selectScore()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L79) | L79 |
| [`triggerYatzy()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L87) | L87 |
| [`leaveGame()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L83) | L83 |
| [`registerHandlers()` (alle events)](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/game-realtime.service.ts#L95) | L95 |

---

## VideoHub

[`Yatzy.Api/Hubs/VideoHub.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/VideoHub.cs) — monteret på `/hubs/video`

Bruges udelukkende til **WebRTC signalering** — selve video/audio strømmer går peer-to-peer og passerer ikke serveren.

### Signalering-flow

```
Spiller A joiner rum
        │
        ▼
VideoHub: JoinVideoRoom(roomCode, playerId)
        │  Server sender "UserJoined" til alle andre
        ▼
Spiller B modtager "UserJoined"
        │  Opretter RTCPeerConnection
        ▼
Spiller B: SendOffer(roomCode, targetId, offer)
        │  Server videresender til Spiller A
        ▼
Spiller A: SendAnswer(roomCode, targetId, answer)
        │  Server videresender til Spiller B
        ▼
Begge: SendIceCandidate(roomCode, targetId, candidate)
        │  Udveksles via server indtil P2P er etableret
        ▼
[Direkte WebRTC-forbindelse — video/lyd peer-to-peer]
```

### VideoHub metoder

| Metode | Beskrivelse |
|---|---|
| `JoinVideoRoom(roomCode, playerId)` | Tilmelder klienten og notificerer andre |
| `SendOffer(roomCode, targetId, offer)` | Videresender WebRTC offer |
| `SendAnswer(roomCode, targetId, answer)` | Videresender WebRTC answer |
| `SendIceCandidate(roomCode, targetId, candidate)` | Videresender ICE candidate |
| `LeaveVideoRoom(roomCode)` | Afmelder og notificerer andre |

---

## WebRTC-service (Angular)

[`webrtc.service.ts`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/webrtc.service.ts)

Håndterer kamera/mikrofon-adgang og alle peer-to-peer forbindelser.

**Nøglemetoder:**

| Metode | Linje |
|---|---|
| [`start(roomCode, playerId)`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/webrtc.service.ts#L44) | L44 — `getUserMedia` + forbind til VideoHub |
| [`stop()`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/core/services/webrtc.service.ts#L80) | L80 — lukker stream + alle peers |

**Streams:**
- `localStream` — kamerafeed fra den lokale spiller
- `remoteStreams$` — `BehaviorSubject<Map<string, MediaStream>>` keyed på playerId

> **Se:** [Frontend → VideoGrid](frontend.md#video-grid)

---

## Se også

- [Use Cases](use-cases.md)
- [Frontend](frontend.md)
- [GIF-systemet](gif-system.md)
