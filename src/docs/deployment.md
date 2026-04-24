# Deployment

Vejledning til at bygge og køre Yatzy lokalt samt noter til produktionsdrift.

---

## Krav

| Værktøj | Minimum version |
|---|---|
| .NET SDK | 9.0 |
| Node.js | 20+ |
| Angular CLI | 19+ (`npm install -g @angular/cli`) |
| SQL Server / SQLite | Valgfrit — se konfiguration |

---

## Kør lokalt (development)

### 1. Backend

```powershell
cd src
dotnet restore
dotnet run --project Yatzy.Api
```

API starter på `https://localhost:5001` (eller porten defineret i `launchSettings.json`).

**Database:** EF Core kører automatisk migrationer ved opstart. Standard er SQL Server — se `appsettings.json` for connection string.

### 2. Frontend

```powershell
cd src/yatzy-web
npm install
ng serve
```

Frontend starter på `http://localhost:4200`.

CORS er konfigureret til at tillade `localhost:4200` i development-mode.

### 3. Test i browser

Åbn to browser-vinduer på `http://localhost:4200`:
- Vindue 1: Opret spil → noter rum-koden
- Vindue 2: Join med rum-koden
- Klik Start i vindue 1

---

## Miljøkonfiguration

### Backend (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=YatzyDb;..."
  }
}
```

Til lokal test kan SQLite bruges ved at ændre `DependencyInjection.cs` i Persistence-laget.

### Frontend (`environment.ts`)

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001'
};
```

---

## Produktion

### Backend — publiser

```powershell
dotnet publish Yatzy.Api -c Release -o ./publish
```

### Frontend — byg til produktion

```powershell
cd src/yatzy-web
ng build --configuration production
```

Output lægges i `dist/yatzy-web/browser/`.

### Serve Angular fra API (samme origin)

I `Program.cs` er Angular konfigureret til at blive servet fra `wwwroot`:

```csharp
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
```

Kopier indholdet af `dist/yatzy-web/browser/` til `Yatzy.Api/wwwroot/` inden publish — så serves frontend og API fra samme URL.

### CORS i produktion

`Program.cs` tillader kun:
```
https://paybysharepay.dk
https://www.paybysharepay.dk
```

Opdatér disse domæner ved deployment til andet miljø.

---

## SignalR i produktion

Visse hosting-miljøer kræver konfiguration for at SignalR WebSockets virker:

- **IIS:** Aktivér WebSocket Protocol via Windows Features
- **Azure App Service:** Aktivér WebSockets under Configuration → General Settings
- **Nginx:** Tilføj proxy-headers:
  ```nginx
  proxy_http_version 1.1;
  proxy_set_header Upgrade $http_upgrade;
  proxy_set_header Connection "upgrade";
  ```

---

## WebRTC i produktion

WebRTC kræver **HTTPS** for at få adgang til kamera/mikrofon i produktionsmiljøer.

Peer-to-peer videoforbindelser kræver typisk en STUN-server til NAT-traversal. Tilføj STUN/TURN-server i `webrtc.service.ts`:

```typescript
const configuration: RTCConfiguration = {
  iceServers: [
    { urls: 'stun:stun.l.google.com:19302' }
  ]
};
```

---

## Se også

- [Arkitektur](architecture.md)
- [Backend](backend.md)
- [Frontend](frontend.md)
