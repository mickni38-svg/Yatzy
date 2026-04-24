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
Ruller ikke-holdte terninger. Broadcaster `DiceRolled` til alle.

> [Se implementering → L109](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L109)

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

`src/yatzy-web/src/app/core/services/game-realtime.service.ts`

Wrapper om SignalR JS-klienten. Eksponerer RxJS-observables som komponenter kan subscribere på.

**Subjects/Observables:**
```typescript
gameState$: Subject<GameStateResponse>   // Alt spilstate
yatzy$:     Subject<{ playerId: string, gifName: string }>
```

**Metoder:**
```typescript
start(roomCode, playerId): Promise<void>   // Opretter forbindelse + JoinRoom
stop(): Promise<void>                      // Lukker forbindelsen
rollDice(gameId, playerId): void
toggleHold(gameId, playerId, position): void
selectScore(gameId, playerId, category): void
triggerYatzy(targetPlayerId, gifName): void
leaveGame(gameId, playerId): void
startGame(gameId): void
```

---

## VideoHub

`Yatzy.Api/Hubs/VideoHub.cs` — monteret på `/hubs/video`

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

`src/yatzy-web/src/app/core/services/webrtc.service.ts`

Håndterer kamera/mikrofon-adgang og alle peer-to-peer forbindelser.

**Nøglemetoder:**
```typescript
startLocalStream(): Promise<MediaStream>
joinRoom(roomCode, playerId): void      // Starter signalering
stop(): Promise<void>                   // Stopper stream + lukker alle peers
```

**Streams:**
- `localStream$` — kamerafeed fra den lokale spiller
- `remoteStreams$` — map over remote streams, keyed på playerId

> **Se:** [Frontend → VideoGrid](frontend.md#video-grid)

---

## Se også

- [Use Cases](use-cases.md)
- [Frontend](frontend.md)
- [GIF-systemet](gif-system.md)
