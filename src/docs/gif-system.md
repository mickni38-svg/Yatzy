# GIF-systemet

Yatzy understøtter tilpassede GIF-fejringer — både automatisk ved Yatzy og manuelt via host.

---

## Hvad er GIF-systemet?

Når en spiller slår Yatzy (fem ens terninger), eller når host trykker 🎉-knappen, vises en GIF for alle spillere i rummet på samme tid.

Alle GIFs og indstillinger styres fra én konfigurationsfil — **ingen kodeændringer nødvendige** for at tilføje eller fjerne GIFs.

---

## Konfigurationsfilen

**Sti:** [`yatzy-web/public/gif-config.json`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/public/gif-config.json)

```json
[
  { "name": "🍕 Yatzy",         "file": "danse.gif",      "showOverlay": true  },
  { "name": "🎉 Stirreflip #2", "file": "stirreflip.gif", "showOverlay": false },
  { "name": "🎉 Cat wtf",       "file": "catwtf.gif",     "showOverlay": false }
]
```

### Felter

| Felt | Type | Beskrivelse |
|---|---|---|
| `name` | `string` | Vises i host-menuen. Kan indeholde emojis. |
| `file` | `string` | Filnavn — GIF-filen skal ligge i `yatzy-web/public/` |
| `showOverlay` | `boolean` | Viser "YATZY!"-tekst-overlay oven på GIF'en |

---

## Tilføj en ny GIF

1. **Kopier GIF-filen** til `yatzy-web/public/`  
   Eksempel: `yatzy-web/public/minny-gif.gif`

2. **Tilføj en linje** i `gif-config.json`:
   ```json
   { "name": "🎊 Min GIF", "file": "minny-gif.gif", "showOverlay": true }
   ```

3. **Genstart** frontend (`ng serve`) eller byg på ny — ingen kodeændringer.

---

## Automatisk fejring

```
[Terningsanimation slutter]
        │
        ▼
game.component.ts: _checkYatzyOnDice()
        │  Alle 5 terninger ens?
        │  Yatzy-kategorien ikke brugt?
        ▼
triggerYatzyCelebration(myPlayerId, sendToServer=true)
        │  Vælger tilfældig GIF fra gifList
        ▼
gameRealtime.triggerYatzy(playerId, gif.file)
        │  SignalR → GameHub.TriggerYatzy
        ▼
BroadcastYatzyTriggerAsync → TriggerYatzy-event til alle
        │
        ▼
Alle klienter: _showGif(playerId, gifName)
```

Fordi `gifName` sendes via SignalR, ser **alle spillere den samme GIF**.

---

## Manuel fejring (Host Fun)

Host ser en 🎉-knap under terningerne. Ved klik åbnes en menu med alle GIFs fra `gif-config.json` (vist med `name`-feltet).

Når host vælger én:
1. `game.component.ts: triggerYatzyCelebration(targetPlayerId, true)`
2. `gifName` sendes via `TriggerYatzy` til serveren
3. Serveren validerer at afsender er host (index 0)
4. Broadcaster til alle i gruppen

---

## showOverlay

Når `showOverlay: true`, vises en animeret "YATZY!"-tekst oven på GIF'en.

Bruges typisk på GIFs der er stille/loopende baggrunde — ikke på GIFs der selv har meget action.

---

## Teknisk reference

**Angular-siden:**
- GIF-listen hentes ved komponentinitiering: [`HttpClient.get<GifEntry[]>('/gif-config.json')`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/features/game/game.component.ts#L131)
- `GifEntry` interface: [`{ name, file, showOverlay? }`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/features/game/game.component.ts#L17)
- [`_showGif(playerId, gifFile)`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/features/game/game.component.ts#L73) — sætter fejrings-GIF, auto-skjules efter 6 sekunder
- [`showYatzyOverlay(playerId)`](https://github.com/mickni38-svg/Yatzy/blob/main/src/yatzy-web/src/app/features/game/game.component.ts#L106) — slår YATZY-overlay til/fra

**Backend-siden:**
- [`GameHub.TriggerYatzy`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/GameHub.cs#L148) — kun host
- [`IGameHubService.BroadcastYatzyTriggerAsync`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Services/GameHubService.cs)
- Event-navn: [`TriggerYatzy`](https://github.com/mickni38-svg/Yatzy/blob/main/src/Yatzy.Api/Hubs/HubEvents.cs)

---

## Se også

- [Real-time & SignalR — TriggerYatzy](realtime.md#triggeryatzy)
- [Frontend — triggerYatzyCelebration](frontend.md#triggeryatzycelebrationplayerid-sendtoserver)
- [Use Cases — UC-07](use-cases.md#uc-07-yatzy-fejring)
