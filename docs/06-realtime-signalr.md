# Realtime multiplayer med SignalR

## Formål
SignalR bruges til live opdatering af gameplay.

Serveren er autoritativ.

Klienten sender intentioner.
Serveren validerer og broadcaster opdateringer.

---

## Hændelser

Følgende events er implementeret i `HubEvents.cs`:

| Event | Payload | Beskrivelse |
|---|---|---|
| `GameStateUpdated` | `GameStateResponse` | Generel state-opdatering |
| `PlayerJoined` | `GameStateResponse` | Spiller har joined |
| `PlayerLeft` | `GameStateResponse` | Spiller har forladt spillet |
| `GameStarted` | `GameStateResponse` | Spillet er startet fra lobby |
| `DiceRolling` | `int[]` rollingPositions | Terningerne er ved at rulle – alle klienter starter animation |
| `DiceRolled` | `GameStateResponse` | Endelig state efter kast – bruges til sekventiel reveal |
| `HoldChanged` | `GameStateResponse` | Hold-status ændret |
| `ScoreSelected` | `GameStateResponse` | Kategori valgt og score registreret |
| `GameEnded` | `GameStateResponse` | Spillet er slut |
| `TriggerYatzy` | `Guid playerId, string gifName` | Fejrings-GIF vises for alle klienter |
| `Error` | `string` | Fejlbesked til klienten |

---

## Terning-animationsflow

Terningkastet følger dette flow for at sikre at **alle klienter er synkroniserede**:

```
Klient kalder RollDice(request)
    ↓
Server ruller terningerne og gemmer i DB
    ↓
Server broadcaster DiceRolling(rollingPositions) til alle klienter
    ↓
Server broadcaster DiceRolled(state) til alle klienter (umiddelbart efter)
    ↓
Alle klienter: 3 sekunders spin-animation
    ↓
Alle klienter: terningerne reveales én ad gangen med 1 sekund interval
    ↓
Game state opdateres i UI (kun efter animation er færdig)
```

Nøglepunkter:
- Serveren ruller **først**, broadcaster **derefter** – alle klienter modtager resultatet på samme tidspunkt
- `DiceRolling` starter animationen; `DiceRolled` gemmes som `_pendingRolledState` til brug ved reveal
- `isAnimating`-flag sikrer at alle terninger (inkl. revealed) læses fra `animatedValues` under hele animationen
- `NgZone.run()` bruges i Angular til at sikre change detection for ikke-kastende klienter

---

## GameHub

Implementerede metoder i `GameHub`:

| Metode | Beskrivelse |
|---|---|
| `JoinRoom(roomCode, playerId)` | Tilføj klient til SignalR group |
| `StartGame(StartGameRequest)` | Kun host kan starte |
| `RollDice(RollDiceRequest)` | Rul terninger – server-autoritativt |
| `ToggleHold(ToggleHoldRequest)` | Skift hold-status på én terning |
| `SelectScore(SelectScoreRequest)` | Registrér score for kategori |
| `TriggerYatzy(Guid, string)` | Broadcast fejrings-GIF til alle |
| `LeaveGame(LeaveGameRequest)` | Forlad spillet |

---

## Request contracts

```csharp
public record RollDiceRequest(Guid GameId, Guid PlayerId);
public record ToggleHoldRequest(Guid GameId, Guid PlayerId, int DiceIndex);
public record SelectScoreRequest(Guid GameId, Guid PlayerId, ScoreCategory Category);
public record StartGameRequest(Guid GameId);
public record LeaveGameRequest(Guid GameId, Guid PlayerId);
```

---

## Servervalidering

Serveren validerer:
- det er spillerens tur
- maks 3 kast pr. runde
- valgt kategori er ledig
- score beregnes server-side
- dice state er konsistent

---

## Room groups

Hver room code er en SignalR group.

Når spilleren joiner:
- forbindelsen tilføjes gruppen
- seneste game state sendes til klienten
- andre spillere informeres

---

## Reconnect strategy

Ved disconnect:
- markér spiller som disconnected
- forsøg automatisk reconnect i frontend (`withAutomaticReconnect()`)
- efter reconnect joines hub igen og seneste state hentes

---

## Angular integration

`GameRealtimeService` eksponerer følgende RxJS observables:

| Observable | Type | Beskrivelse |
|---|---|---|
| `gameState$` | `GameStateDto` | Opdateret game state |
| `diceRolling$` | `number[]` | Positioner der ruller – start animation |
| `diceRolled$` | `GameStateDto` | Endelig state – bruges til reveal |
| `error$` | `string` | Fejlbeskeder |
| `yatzy$` | `{ playerId, gifName }` | Yatzy-fejring |
| `connected$` | `boolean` | Forbindelsesstatus |

`applyState(state)` bruges til manuelt at opdatere `gameState$` efter animation er færdig.

---

## Done-kriterier

- [x] Flere browser-vinduer kan joine samme spil
- [x] Kast opdateres live og synkroniseret på alle klienter
- [x] Alle klienter spinner terningerne i 3 sekunder
- [x] Terningerne reveales én ad gangen med 1 sekund interval på alle klienter
- [x] Hold opdateres live
- [x] Scorekort opdateres live
- [x] Reconnect er håndteret
