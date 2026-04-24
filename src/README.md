# 🎲 Yatzy — Multiplayer Web Game

Et real-time multiplayer Yatzy-spil med live videochat og GIF-fejringer.

Bygget med **.NET 9** (Clean Architecture) og **Angular 19** (standalone components).

---

## Funktioner

- 🎮 Op til 6 spillere i samme rum via rum-kode
- 🎲 Fuld Yatzy-regelimplementering — alle 15 kategorier + bonus
- 📡 Real-time gameplay via **SignalR**
- 📹 Peer-to-peer videochat via **WebRTC**
- 🎉 Automatisk Yatzy-GIF-fejring + host kan vælge manuel GIF
- 🏠 Host-styret lobby
- 📱 Responsivt design

---

## Kom i gang

```bash
# Backend (.NET 9)
cd src
dotnet run --project Yatzy.Api

# Frontend (Angular 19)
cd src/yatzy-web
npm install && ng serve
```

Åbn `http://localhost:4200` — opret et rum i ét vindue og join fra et andet.

---

## Dokumentation

| Dokument | Indhold |
|---|---|
| [Use Cases](docs/use-cases.md) | Alle brugsscenarier (opret spil, slå terninger, videochat, mm.) |
| [Arkitektur](docs/architecture.md) | Clean Architecture-lagdeling, dataflow og databasemodel |
| [Backend](docs/backend.md) | Domain-entiteter, scoring-regler, services og REST API |
| [Frontend](docs/frontend.md) | Angular-komponenter, services og routing |
| [Real-time & SignalR](docs/realtime.md) | Alle hub-events, WebRTC-videoflow |
| [GIF-systemet](docs/gif-system.md) | Tilføj GIFs uden kodeændringer |
| [Deployment](docs/deployment.md) | Byg og kør lokalt samt produktionsnotes |

---

## Teknologistak

| Lag | Teknologi |
|---|---|
| Backend | .NET 9 / ASP.NET Core |
| Real-time | ASP.NET Core SignalR |
| Database | Entity Framework Core |
| Frontend | Angular 19 |
| Videochat | WebRTC |
