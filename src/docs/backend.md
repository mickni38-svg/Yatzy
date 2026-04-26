# Backend

Teknisk beskrivelse af alle backend-lag: Domain, Application, API og Persistence.

---

## Domain

Projektets kerne — ingen afhængigheder til andre lag.

### `Game`

`Yatzy.Domain/Entities/Game.cs`

Den centrale entitet der ejer spillets tilstand.

**Konstanter:**
| Konstant | Værdi | Beskrivelse |
|---|---|---|
| `MaxPlayers` | 6 | Maks antal spillere |
| `MinPlayersToStart` | 2 | Minimum for at starte |
| `DiceCount` | 5 | Antal terninger |
| `MaxRollsPerTurn` | 3 | Maks slag per runde |

**Vigtige metoder:**

```csharp
// Fabriksmetode — opretter nyt spil med 5 terninger
Game.Create(string roomCode)

// Tilføjer spiller — validerer kapacitet og status
Player AddPlayer(Guid playerId, string displayName)

// Starter spillet — sætter status til InProgress
void StartGame()

// Ruller ikke-holdte terninger (kræver IRandomProvider)
void RollDice(Guid playerId, IRandomProvider random)

// Holder/frigiver terning
void ToggleHold(Guid playerId, int diceIndex)

// Registrerer kategori-score for aktiv spiller, avancerer tur
void SelectScore(Guid playerId, ScoreCategory category, IScoreCalculator calculator)

// Markerer spiller som forladt
void LeaveGame(Guid playerId)

// Spil slutter når alle aktive spillere har udfyldt alle kategorier (se AdvanceTurn)
GameStatus.Completed
```

> Klik for at se implementeringen direkte på GitHub:
> | Metode | Linje |
> |---|---|
> | [`Game.Create`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L33) | L33 |
> | [`AddPlayer`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L54) | L54 |
> | [`StartGame`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L95) | L95 |
> | [`RollDice`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L108) | L108 |
> | [`ToggleHold`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L122) | L122 |
> | [`SelectScore`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L136) | L136 |
> | [`LeaveGame`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L80) | L80 |
> | [`AdvanceTurn` (spil slut-logik)](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Game.cs#L176) | L176 |

---

### `Player`

[`Yatzy.Domain/Entities/Player.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/Player.cs)

Repræsenterer én spiller i et spil.

**Egenskaber:** `Id`, `DisplayName`, `JoinOrder`, `IsConnected`, `HasLeft`, `ScoreSheet`

---

### `ScoreSheet`

[`Yatzy.Domain/Entities/ScoreSheet.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Entities/ScoreSheet.cs)

Holder alle 15 score-entries for en spiller. Beregner øvre sektion, bonus og total score.

**Bonusregel:** Øvre sektion ≥ 63 point → +50 bonuspoint.

---

### `ScoreCalculator`

[`Yatzy.Domain/Rules/ScoreCalculator.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Rules/ScoreCalculator.cs)

Implementerer `IScoreCalculator`. Beregner point for alle 15 kategorier:

**Øvre sektion** (sum af matchende øjne):
| Kategori | Regel |
|---|---|
| `Ones` – `Sixes` | Sum af alle terninger med den pågældende værdi |

**Nedre sektion:**
| Kategori | Regel |
|---|---|
| `OnePair` | 2× højeste par |
| `TwoPairs` | Sum af to par |
| `ThreeOfAKind` | 3× matchende terning |
| `FourOfAKind` | 4× matchende terning |
| `SmallStraight` | 1-2-3-4-5 → 15 point |
| `LargeStraight` | 2-3-4-5-6 → 20 point |
| `FullHouse` | Par + tre ens → sum |
| `Chance` | Sum af alle terninger |
| `Yatzy` | Fem ens → 50 point |

**Kode-eksempel:**
```csharp
var points = calculator.Calculate(ScoreCategory.Yatzy, new[] { 4, 4, 4, 4, 4 });
// → 50
```

> Klik for at se implementeringen direkte på GitHub:
> | Metode | Linje |
> |---|---|
> | [`Calculate` (switch over alle kategorier)](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Rules/ScoreCalculator.cs#L8) | L8 |
> | [`SumOfFace` (øvre sektion)](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Rules/ScoreCalculator.cs#L41) | L41 |
> | [`BestPair`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Rules/ScoreCalculator.cs#L48) | L48 |

---

### Enums

**[`GameStatus`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Enums/GameStatus.cs)**:
```csharp
WaitingForPlayers  // Lobby, under minimumgrænse
ReadyToStart       // Nok spillere, afventer host
InProgress         // Spillet er i gang
Completed          // Alle kategorier udfyldt
```

**[`ScoreCategory`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Domain/Enums/ScoreCategory.cs)**:
15 kategorier:

---

### Exceptions

| Exception | Hvornår |
|---|---|
| `DomainException` | Ugyldig domænehandling (spil fuldt, forkert status m.m.) |
| `ValidationException` | Input-validering fejler |
| `NotFoundException` | Spil eller spiller ikke fundet |

---

## Application

Use case-lag — orkestrerer domain og infrastruktur.

### `GameAppService`

`Yatzy.Application/Services/GameAppService.cs`

Håndterer spilopsætning og lobby.

```csharp
Task<GameStateResponse> CreateGameAsync(CreateGameRequest request)
Task<GameStateResponse> JoinGameAsync(string roomCode, JoinGameRequest request)
Task<GameStateResponse> StartGameAsync(Guid gameId)
Task<GameStateResponse> GetByIdAsync(Guid gameId)
Task<GameStateResponse> GetByRoomCodeAsync(string roomCode)
```

> | Metode | Linje |
> |---|---|
> | [`CreateGameAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameAppService.cs#L25) | L25 |
> | [`JoinGameAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameAppService.cs#L40) | L40 |
> | [`StartGameAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameAppService.cs#L55) | L55 |

---

### `GameplayAppService`

[`Yatzy.Application/Services/GameplayAppService.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameplayAppService.cs)

Håndterer selve gameplay-handlinger.

```csharp
Task<GameStateResponse> RollDiceAsync(RollDiceRequest request)
Task<(GameStateResponse State, Task PersistTask)> RollDiceFastAsync(RollDiceRequest request)
Task<GameStateResponse> ToggleHoldAsync(ToggleHoldRequest request)
Task<GameStateResponse> SelectScoreAsync(SelectScoreRequest request)
Task<GameStateResponse> LeaveGameAsync(LeaveGameRequest request)
Task<GameStateResponse?> PlayerReconnectedAsync(Guid gameId, Guid playerId)
```

> | Metode | Linje | Note |
> |---|---|---|
> | [`RollDiceAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameplayAppService.cs#L28) | L28 | Standard — beregn + gem sekventielt |
> | [`RollDiceFastAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameplayAppService.cs#L40) | L40 | ⚡ Optimeret — returnerer state + igangværende DB-task parallelt |
> | [`ToggleHoldAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameplayAppService.cs#L55) | L55 | |
> | [`SelectScoreAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameplayAppService.cs#L67) | L67 | |
> | [`LeaveGameAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/GameplayAppService.cs#L91) | L91 | |

#### ⚡ Performance-optimering — `RollDiceFastAsync`

For at reducere forsinkelse hos tilskuere bruges `RollDiceFastAsync` i stedet for `RollDiceAsync` når der rulles terninger. Metoden starter DB-skrivningen og returnerer spilstaten **uden at afvente** at skrivningen er færdig:

```csharp
// GameplayAppService.cs — RollDiceFastAsync
var state = MapToResponse(game);          // Beregnet resultat klar
_gameRepository.Update(game);
var persistTask = _unitOfWork.SaveChangesAsync(CancellationToken.None);  // Startet, ikke afventet
return (state, persistTask);              // Returnér straks
```

I [`GameHub.RollDice`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L105) køres broadcast og DB-gem **parallelt**:

```csharp
// GameHub.cs — RollDice
var (state, persistTask) = await _gameplayAppService.RollDiceFastAsync(request);
await Task.WhenAll(
    _hubService.BroadcastDiceRolledAsync(state.RoomCode, state),  // Til alle spillere straks
    persistTask                                                     // DB gem samtidig
);
```

**Effekt:** Tilskuere modtager terningresultatet ~300-500ms hurtigere på shared hosting.

---

### `ConnectionService`

`Yatzy.Application/Services/ConnectionService.cs`

In-memory `ConcurrentDictionary` der mapper `ConnectionId → (GameId, PlayerId, RoomCode)`.
Bruges af `GameHub` til at identificere hvilken spiller der sender en besked.

```csharp
void Register(string connectionId, Guid gameId, Guid playerId, string roomCode)
void Unregister(string connectionId)
(Guid GameId, Guid PlayerId, string RoomCode)? Get(string connectionId)
```

> | Metode | Linje |
> |---|---|
> | [`Register`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/ConnectionService.cs#L10) | L10 |
> | [`Unregister`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/ConnectionService.cs#L13) | L13 |
> | [`Get`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/Services/ConnectionService.cs#L16) | L16 |

---

### DTOs

**[`GameStateResponse`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/DTOs/GameStateResponse.cs)** — sendes til alle klienter ved enhver ændring:
```csharp
GameId, RoomCode, Status, RoundNumber, RollNumber, CurrentPlayerId
Players[]  → PlayerId, DisplayName, IsHost, IsConnected, HasLeft, TotalScore, ScoreEntries[]
Dice[]     → Position, Value, IsHeld
```

**Hub-request DTOs** ([`HubRequests.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Application/DTOs/HubRequests.cs)):
```csharp
StartGameRequest   { GameId }
RollDiceRequest    { GameId, PlayerId }
ToggleHoldRequest  { GameId, PlayerId, Position }
SelectScoreRequest { GameId, PlayerId, Category }
LeaveGameRequest   { GameId, PlayerId }
```

---

## REST API

[`Yatzy.Api/Controllers/GamesController.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Controllers/GamesController.cs)

| Method | Route | Beskrivelse |
|---|---|---|
| `POST` | `/api/games` | Opret nyt spil (host) |
| `POST` | `/api/games/{roomCode}/join` | Join eksisterende spil |

Disse endpoints bruges kun ved oprettelse/join — alt gameplay sker via SignalR.

---

## Middleware

[`Yatzy.Api/Middleware/ExceptionHandlingMiddleware.cs`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Middleware/ExceptionHandlingMiddleware.cs)

Fanger alle ubehandlede exceptions og returnerer passende HTTP-statuskoder:
- `NotFoundException` → 404
- `DomainException` / `ValidationException` → 400
- Øvrige → 500

---

## Se også

- [Use Cases](use-cases.md)
- [Real-time & SignalR](realtime.md)
- [Arkitektur](architecture.md)
