# Domain og logic layer

## Formål
Denne fil beskriver den rene spilkerne for Yatzy.

Det er her reglerne skal implementeres.

---

## Regler
Spillet følger klassisk Yatzy-logik fra den tidligere guide:

- 5 terninger
- op til 3 kast pr. tur
- spilleren kan holde terninger mellem kast
- præcis én kategori vælges pr. tur
- hver kategori må kun bruges én gang
- bonus på 50 ved mindst 63 i øvre sektion
- Yatzy giver 50 point fileciteturn0file1L147-L156

---

## Centrale domæneobjekter

### Game
Ansvar:
- spillerliste
- aktiv spiller
- runde
- status
- fælles dice state
- regler for progression

### Player
Ansvar:
- id
- display name
- connection state
- score sheet

### Dice
Ansvar:
- position
- value
- isHeld

### ScoreSheet
Ansvar:
- entries
- upper section sum
- bonus
- lower section sum
- total

---

## Enums

### GameStatus
```csharp
public enum GameStatus
{
    WaitingForPlayers = 0,
    ReadyToStart = 1,
    InProgress = 2,
    Completed = 3,
    Abandoned = 4
}
```

### ScoreCategory
```csharp
public enum ScoreCategory
{
    Ones,
    Twos,
    Threes,
    Fours,
    Fives,
    Sixes,
    OnePair,
    TwoPairs,
    ThreeOfAKind,
    FourOfAKind,
    SmallStraight,
    LargeStraight,
    FullHouse,
    Chance,
    Yatzy
}
```

---

## Foreslåede entities

```csharp
public class Game
{
    public Guid Id { get; private set; }
    public string RoomCode { get; private set; } = string.Empty;
    public GameStatus Status { get; private set; }
    public int CurrentPlayerIndex { get; private set; }
    public int RoundNumber { get; private set; }
    public int RollNumber { get; private set; }
    public List<Player> Players { get; private set; } = new();
    public List<Dice> Dice { get; private set; } = new();
}
```

```csharp
public class Player
{
    public Guid Id { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public bool IsConnected { get; private set; }
    public ScoreSheet ScoreSheet { get; private set; } = new();
}
```

```csharp
public class Dice
{
    public int Position { get; private set; }
    public int Value { get; private set; }
    public bool IsHeld { get; private set; }
}
```

---

## Regelmotor
Lav en separat service:

```csharp
public interface IScoreCalculator
{
    int Calculate(ScoreCategory category, IReadOnlyList<int> dice);
}
```

Den skal implementere:
- ones
- twos
- threes
- fours
- fives
- sixes
- one pair
- two pairs
- three of a kind
- four of a kind
- small straight
- large straight
- full house
- chance
- yatzy

---

## Turn rules
Game skal kunne:
- starte spillet
- starte en tur
- kaste terninger
- toggle hold
- vælge scorekategori
- afslutte tur
- gå til næste spiller
- afslutte spillet

---

## Domænemetoder der bør findes
- `StartGame()`
- `RollDice(IRandomProvider randomProvider)`
- `ToggleHold(int diceIndex)`
- `SelectScore(Guid playerId, ScoreCategory category, IScoreCalculator calculator)`
- `AdvanceTurn()`
- `CanRoll(Guid playerId)`
- `CanSelectCategory(Guid playerId, ScoreCategory category)`

---

## Domæneregler der skal håndhæves
- kun aktiv spiller må kaste
- ingen må kaste mere end 3 gange
- kun aktiv spiller må vælge kategori
- kategori må ikke være brugt før
- spillet skal være i gang før man kan kaste
- man må ikke ændre hold efter scoring
- spillet afsluttes når alle kategorier er udfyldt for alle spillere

---

## Unit tests
Lav unit tests for:
- alle score categories
- bonus calculation
- turn progression
- max 3 rolls
- hold logic
- next player logic
- game completion logic

---

## Copilot-prompt
> Implement the Yatzy.Domain project with entities, enums, rule engine, and domain methods for a multiplayer online Yatzy game. Keep the domain layer pure C# with no EF or ASP.NET dependencies. Add unit tests for scoring and turn progression.

---

## Done-kriterier
Fasen er færdig når:
- domænemodeller findes
- score calculator virker
- regler er dækket af unit tests
- domænelaget er uafhængigt af web, database og Angular
