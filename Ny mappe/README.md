# Yatzy .NET MAUI Online Multiplayer Game

Dette projekt implementerer et online Yatzy-spil baseret på arkitekturen beskrevet i `yatzy_maui_android_guide.md`.

## Projekt Struktur

Løsningen består af 3 projekter:

### 1. Yatzy.Shared (.NET 9.0 Class Library)
Fælles kode delt mellem frontend og backend:
- **Domain**: Domæne modeller (GameRoom, Player, ScoreSheet, DiceState, etc.)
- **Enums**: Fælles enumerations (ScoreCategory, GameStatus)
- **DTOs**: Data Transfer Objects for API kommunikation
- **Rules**: Spilregler inkl. ScoreCalculator
- **Contracts**: SignalR hub events

### 2. Yatzy.Api (.NET 9.0 ASP.NET Core Web API)
Backend server med SignalR hub:
- **Hubs/GameHub.cs**: SignalR hub til realtime kommunikation
- **Services/GameService.cs**: Game state management og validering
- **Program.cs**: API konfiguration med CORS og SignalR support

### 3. Yatzy (.NET 9.0 MAUI)
Android MAUI frontend app:
- **Views**: LobbyPage, GamePage (XAML)
- **ViewModels**: LobbyViewModel, GameViewModel (MVVM pattern)
- **Services/GameHubService.cs**: SignalR client til server kommunikation
- **Converters**: XAML value converters

## Hvordan du kører projektet

### 1. Start Backend (Yatzy.Api)
```bash
cd Yatzy.Api
dotnet run
```
Backend kører på https://localhost:7000 (eller en anden port vist i console)

### 2. Opdater SignalR URL
I `ViewModels/LobbyViewModel.cs`, opdater hub URL hvis nødvendigt:
```csharp
await _gameHubService.ConnectAsync("https://localhost:7000/gamehub");
```

### 3. Start MAUI App
I Visual Studio:
- Sæt Yatzy som startup project
- Vælg Android emulator eller device
- Tryk F5 for at bygge og køre

Eller fra command line:
```bash
dotnet build Yatzy.csproj -f net9.0-android
```

## Features Implementeret

✅ Domæne modeller og spilregler  
✅ Score calculator med alle Yatzy regler  
✅ ASP.NET Core backend med SignalR  
✅ SignalR hub til multiplayer  
✅ MAUI frontend med MVVM  
✅ Lobby til at oprette/joine rum  
✅ Game state management  
✅ Terning kast og hold funktionalitet  

## Næste Skridt

For at færdiggøre spillet, skal du implementere:

1. **Score Selection UI**: Brugergrænseflade til at vælge scorekategori efter terningekast
2. **Video Integration**: Tilføj WebRTC eller en video SDK (Agora, Azure Communication Services)
3. **Forbedret UI**: Animationer, terning grafik, scorekort visning
4. **Error Handling**: Bedre fejlhåndtering og bruger feedback
5. **Reconnect Logic**: Håndter disconnects og reconnects
6. **Persistence**: Gem game state i database (SQL Server/PostgreSQL)
7. **Authentication**: Tilføj brugerlogind

## Teknologi Stack

- .NET 9.0
- .NET MAUI for Android
- ASP.NET Core Web API
- SignalR for realtime kommunikation
- Community Toolkit MVVM
- Entity Framework Core (fremtidig database integration)

## Design Principper

Projektet følger:
- **MVVM pattern** i MAUI app
- **Clean Architecture** med separation mellem domain, infrastructure og presentation
- **SignalR** til realtime multiplayer
- **Dependency Injection** for loose coupling
- **Repository pattern** klar til database integration

## Yderligere Ressourcer

Se `yatzy_maui_android_guide.md` for detaljeret arkitektur og implementeringsguide.
