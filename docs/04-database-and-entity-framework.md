# Database og Entity Framework Core

## Formål
Denne fil beskriver persistenslaget.

Databasen skal kunne gemme:
- spil
- spillere
- score entries
- aktuel tur
- terningetilstand
- forbindelsesstatus
- historik senere hvis ønsket

---

## Teknologi
- Entity Framework Core
- SQL Server
- Code First migrations

---

## Entiteter i databasen

### GameEntity
Felter:
- Id
- RoomCode
- Status
- CurrentPlayerIndex
- RoundNumber
- RollNumber
- CreatedUtc
- UpdatedUtc

### PlayerEntity
Felter:
- Id
- GameId
- DisplayName
- IsConnected
- JoinOrder

### DiceEntity
Felter:
- Id
- GameId
- Position
- Value
- IsHeld

### ScoreEntryEntity
Felter:
- Id
- PlayerId
- Category
- Points

---

## DbContext
Opret `YatzyDbContext` med:
- DbSet<GameEntity>
- DbSet<PlayerEntity>
- DbSet<DiceEntity>
- DbSet<ScoreEntryEntity>

---

## Mapping
Brug `IEntityTypeConfiguration<T>` til:
- keys
- relationships
- max lengths
- required constraints
- enum conversions hvis nødvendigt

---

## Repository-abstractions
Application-laget må ikke kende EF direkte.

Definér interfaces som:
- `IGameRepository`
- `IUnitOfWork`

---

## Repositories
Persistence-laget implementerer repositories:
- create game
- get by room code
- get by id
- save changes
- update full aggregate

---

## Migrations
Første migration skal etablere alle tabeller.

---

## Persistensstrategi
Den simpleste strategi er:
- gem hele game aggregate efter hver væsentlig handling
- hent hele aggregate ved reconnect eller API-kald

Det er ikke mest avanceret, men det er ofte det mest robuste at starte med.

---

## Copilot-prompt
> Implement the persistence layer using EF Core and SQL Server for the Yatzy game. Create DbContext, entity models, entity configurations, repositories, and initial migration support. Persist the game aggregate so it can be restored after reconnects.

---

## Done-kriterier
Fasen er færdig når:
- DbContext findes
- entity mappings findes
- repositories findes
- første migration kan oprettes
- API senere kan hente og gemme game state
