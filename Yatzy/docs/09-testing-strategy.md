# Testing strategy

## Formål
Denne fil beskriver teststrategien for løsningen.

---

## Backend tests

### Unit tests
Skriv unit tests for:
- score calculator
- bonus logic
- full house
- straights
- yatzy
- turn progression
- max 3 rolls
- used category validation

### Integration tests
Skriv integration tests for:
- create game endpoint
- join game endpoint
- start game endpoint
- get game state endpoint

### Realtime tests
Test:
- player joins room
- dice roll broadcast
- score selection broadcast
- reconnect

---

## Frontend tests

### Component tests
Test:
- score-sheet rendering
- dice rendering
- active player banner
- category selector behavior

### Service tests
Test:
- GameApiService
- GameRealtimeService
- mapping af DTO’er
- reconnect logic

---

## Manuel testliste
- to browsere kan deltage i samme spil
- turn order virker
- kun aktiv spiller kan kaste
- kun aktiv spiller kan vælge kategori
- score opdateres live
- spil afsluttes korrekt
- refresh og reconnect fungerer acceptabelt

---

## Copilot-prompt
> Add a practical test strategy for the Yatzy solution with domain unit tests, API integration tests, SignalR realtime tests, Angular component tests, and service tests. Focus on the most important gameplay risks first.

---

## Done-kriterier
Fasen er færdig når:
- de vigtigste domæneregler er testet
- API-flow er testet
- kritiske realtime flows er testet
