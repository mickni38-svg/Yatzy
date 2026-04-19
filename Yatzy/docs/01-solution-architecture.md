# Solution architecture – browser-baseret Yatzy

## Mål
Denne løsning skal være en **online multiplayer Yatzy-webapp**, der kører i Chrome.

Arkitekturen skal være enkel at udvikle videre på og opdelt i tydelige lag.

---

## Overordnet arkitektur

```text
Angular frontend (Chrome)
        |
        | HTTP + SignalR
        v
ASP.NET core 9 Web API + SignalR Hub
        |
        | Application services
        v
Domain + Logic libraries
        |
        | EF Core
        v
SQL database
```

---

## Ansvarsfordeling

## Frontend – Angular
Frontend skal:
- vise lobby
- vise hvilke spillere der er med
- vise scorekort
- vise terninger
- lade bruger holde terninger
- kalde API endpoints
- forbinde til SignalR hub
- opdatere UI i realtime

Frontend må **ikke** være autoritet for regler eller scoring.

---

## API – ASP.NET Core 9
API’et skal:
- oprette spil
- joine spil
- starte spil
- eksponere game state
- modtage brugerens intentioner
- validere alle requests
- udstille SignalR hub

---

## Domain-lag
Domain-laget skal være rent C# uden afhængighed til UI og database.

Det skal indeholde:
- entiteter
- value objects
- enums
- regler
- scoreberegning
- turn-flow regler

---

## Application-lag
Application-laget skal orkestrere:
- use cases
- validering
- samarbejde mellem domain og persistence
- mapping mellem DTO’er og domæne

---

## Persistence-lag
Persistence skal indeholde:
- EF Core DbContext
- entity configurations
- repository implementations
- migrations
- eventuel seed data

---

## Realtime lag
SignalR skal bruges til:
- player joined
- player left
- game started
- turn started
- dice rolled
- hold changed
- score selected
- scoreboard updated
- game ended

---

## Designprincipper
- backend ejer sandheden
- regler beregnes server-side
- score beregnes server-side
- terningekast afgøres server-side
- frontend sender kun intentioner
- løsningen opdeles i små testbare enheder

---

## Foreslået solution layout

```text
src/
  Yatzy.sln
  Yatzy.Domain/
  Yatzy.Application/
  Yatzy.Infrastructure/
  Yatzy.Persistence/
  Yatzy.Api/
  yatzy-web/
```

---

## Projektreferencer

```text
Yatzy.Application -> Yatzy.Domain
Yatzy.Infrastructure -> Yatzy.Application, Yatzy.Domain
Yatzy.Persistence -> Yatzy.Application, Yatzy.Domain
Yatzy.Api -> Yatzy.Application, Yatzy.Infrastructure, Yatzy.Persistence
```

---

## Teknologistak
- C#
- .NET 9
- ASP.NET Core 9
- SignalR
- Entity Framework Core
- SQL Server
- Angular
- TypeScript
- RxJS
- Angular SignalR client

---

## Copilot-opgave
Brug denne fil som arkitektur-reference.

Bed Copilot om at:
1. oprette projekterne
2. oprette references
3. oprette basis-foldere
4. holde domain rent for framework-afhængigheder
5. holde API som tyndt orkestreringslag
