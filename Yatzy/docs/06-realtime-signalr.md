# Realtime multiplayer med SignalR

## Formål
SignalR bruges til live opdatering af gameplay.

Serveren er autoritativ.

Klienten sender intentioner.
Serveren validerer og broadcaster opdateringer.

---

## Hændelser
Definér events som:

- PlayerJoined
- PlayerLeft
- GameStarted
- TurnStarted
- DiceRolled
- HoldChanged
- ScoreSelected
- ScoreboardUpdated
- GameEnded

Den tidligere guide lagde også op til denne type realtime events og server-side validering fileciteturn0file1L255-L272

---

## GameHub
Opret `GameHub` med metoder:

- `JoinRoom(string roomCode)`
- `RollDice(RollDiceRequest request)`
- `ToggleHold(ToggleHoldRequest request)`
- `SelectScore(SelectScoreRequest request)`

---

## Request contracts
```csharp
public record RollDiceRequest(Guid GameId, Guid PlayerId);
public record ToggleHoldRequest(Guid GameId, Guid PlayerId, int DiceIndex, bool IsHeld);
public record SelectScoreRequest(Guid GameId, Guid PlayerId, ScoreCategory Category);
```

---

## Servervalidering
Serveren skal validere:
- det er spillerens tur
- maks 3 kast
- valgt kategori er ledig
- score beregnes server-side
- dice state er konsistent

---

## Room groups
Hver room code skal være en SignalR group.

Når spilleren joiner:
- forbindelsen tilføjes gruppen
- seneste game state kan sendes
- andre spillere informeres

---

## Reconnect strategy
Ved disconnect:
- markér spiller som disconnected
- forsøg automatisk reconnect i frontend
- efter reconnect hentes game state igen
- hub joines igen

---

## Angular integration
Frontend skal have en `GameRealtimeService` der:
- opretter SignalR connection
- joiner room
- lytter til events
- eksponerer RxJS observables
- håndterer reconnect

---

## Copilot-prompt
> Implement SignalR realtime gameplay for the Yatzy solution. Create the hub, event contracts, request models, room group logic, server-side validation, and reconnect support. The server must remain authoritative over turn order, dice rolls, and score calculation.

---

## Done-kriterier
Fasen er færdig når:
- flere browser-vinduer kan joine samme spil
- kast opdateres live
- hold opdateres live
- scorekort opdateres live
- reconnect er håndteret acceptabelt
