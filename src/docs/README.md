# 🎲 Yatzy — Multiplayer Web Game

Et real-time multiplayer Yatzy-spil bygget med **.NET 9** (backend) og **Angular 19** (frontend), med live videochat og GIF-fejringer.

---

## Funktioner

- 🎮 Op til 6 spillere i samme rum
- 🎲 Fuld Yatzy-regelimplementering (15 kategorier, bonuspoint)
- 📡 Real-time gameplay via **SignalR**
- 📹 Peer-to-peer videochat via **WebRTC**
- 🎉 Automatisk og manuel Yatzy-fejring med GIFs
- 🏠 Host-styret lobby og spilstart
- 📱 Responsivt design til mobil og desktop

---

## Dokumentation

| Dokument | Beskrivelse |
|---|---|
| [Use Cases](use-cases.md) | Alle brugsscenarier beskrevet funktionelt |
| [Arkitektur](architecture.md) | Systemarkitektur og lagdeling |
| [Backend](backend.md) | Domain, Application, API og database |
| [Frontend](frontend.md) | Angular-komponenter, services og routing |
| [Real-time & SignalR](realtime.md) | Hub-events og WebRTC-videoflow |
| [GIF-systemet](gif-system.md) | Tilføj og konfigurér fejringsgifs |
| [Deployment](deployment.md) | Byg og kør lokalt samt produktionsnotes |

---

## Hurtig start

```bash
# Backend
cd src
dotnet run --project Yatzy.Api

# Frontend
cd yatzy-web
npm install
ng serve
```

Åbn `http://localhost:4200` i to browser-vinduer og opret/join et rum.

---

## Teknologistak

| Lag | Teknologi |
|---|---|
| Backend runtime | .NET 9 / ASP.NET Core |
| Real-time | ASP.NET Core SignalR |
| Database | Entity Framework Core (SQL Server / SQLite) |
| Frontend | Angular 19 (standalone components) |
| Videochat | WebRTC (peer-to-peer) |
| Styling | SCSS |
