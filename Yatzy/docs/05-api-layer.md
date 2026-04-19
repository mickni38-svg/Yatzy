# API layer – ASP.NET core 9

## Formål
API-laget skal eksponere REST endpoints til lobby og game management.

Realtime gameplay håndteres i SignalR, men REST bruges stadig til:
- oprettelse af spil
- join af spil
- hent initial game state
- start spil
- helbredstjek

---

## Controllers

### GamesController
Endpoints:
- `POST /api/games`
- `POST /api/games/{roomCode}/join`
- `POST /api/games/{gameId}/start`
- `GET /api/games/{gameId}`
- `GET /api/games/by-room/{roomCode}`

---

## DTO’er

### CreateGameRequest
- hostName

### JoinGameRequest
- playerName

### GameStateResponse
- gameId
- roomCode
- status
- roundNumber
- currentPlayerId
- rollNumber
- players
- dice
- scoreboard

---

## Principper
- controllers skal være tynde
- business logic skal ligge i application-laget
- validation skal være tydelig
- returnér klare HTTP-statuskoder
- alle fejl skal være brugbare for frontend

---

## Program.cs
Konfigurér:
- controllers
- swagger
- CORS til Angular dev server
- SignalR
- DbContext
- DI for application og repositories

---

## CORS
Tillad Angular-klienten i udvikling, fx:
- `http://localhost:4200`

---

## Fejlhåndtering
Tilføj middleware til:
- domain exceptions
- validation exceptions
- generelle fejl

---

## Mulige services i application-laget
- `IGameAppService`
- `CreateGameAsync`
- `JoinGameAsync`
- `StartGameAsync`
- `GetGameStateAsync`

---

## Copilot-prompt
> Implement the ASP.NET Core 9 API layer for the Yatzy solution. Add controllers, DTOs, dependency injection, CORS, Swagger, and clean error handling. Keep controllers thin and delegate business logic to the application layer.

---

## Done-kriterier
Fasen er færdig når:
- API kan starte
- swagger virker
- create game virker
- join game virker
- hent game state virker
