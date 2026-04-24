# Use Cases

Denne side beskriver alle brugsscenarier i Yatzy-applikationen som funktionelle krav.

---

## Oversigt

| # | Use case | Primær aktør |
|---|---|---|
| UC-01 | [Opret spil](#uc-01-opret-spil) | Spiller (host) |
| UC-02 | [Join spil](#uc-02-join-spil) | Spiller |
| UC-03 | [Start spil](#uc-03-start-spil) | Host |
| UC-04 | [Slå med terninger](#uc-04-slå-med-terninger) | Aktiv spiller |
| UC-05 | [Hold terning](#uc-05-hold-terning) | Aktiv spiller |
| UC-06 | [Vælg kategori](#uc-06-vælg-kategori) | Aktiv spiller |
| UC-07 | [Yatzy-fejring](#uc-07-yatzy-fejring) | System / Host |
| UC-08 | [Videochat](#uc-08-videochat) | Alle spillere |
| UC-09 | [Forlad spil](#uc-09-forlad-spil) | Spiller |
| UC-10 | [Genforbind](#uc-10-genforbind) | Spiller |
| UC-11 | [Spil slutter](#uc-11-spil-slutter) | System |

---

## UC-01: Opret spil

**Aktør:** Spiller (bliver host)

**Forudsætning:** Spilleren er på lobby-siden.

**Beskrivelse:**
Spilleren indtaster sit navn og klikker "Opret spil". Systemet genererer et unikt rum-kode (fx `ABCD`) og opretter et nyt spil i databasen. Spilleren viderestilles til lobbyen og er nu host.

**Successkriterium:** Rum-koden vises og kan deles med andre spillere.

**Alternativt forløb:** Hvis et rum med samme kode allerede eksisterer, returnerer API'et en fejl.

> **Teknisk reference:** [`GameAppService.CreateAsync`](backend.md#gameappservice) · [`GamesController.POST /api/games`](backend.md#rest-api)

---

## UC-02: Join spil

**Aktør:** Spiller

**Forudsætning:** Spilleren kender rum-koden. Spillet er i status `WaitingForPlayers` eller `ReadyToStart`.

**Beskrivelse:**
Spilleren indtaster rum-koden og sit navn. API'et tilføjer spilleren til spillet. SignalR-forbindelsen oprettes, og alle spillere i rummet modtager en opdateret spilliste.

**Successkriterium:** Spilleren ser lobbyen med alle tilmeldte spillere.

**Fejlscenarier:**
- Spillet er allerede startet → fejlbesked
- Rummet er fuldt (max 6 spillere) → fejlbesked
- Spiller-ID allerede i spillet → fejlbesked

> **Teknisk reference:** [`Game.AddPlayer`](backend.md#game) · [`GameHub.JoinRoom`](realtime.md#joinroom)

---

## UC-03: Start spil

**Aktør:** Host (første spiller der oprettede rummet)

**Forudsætning:** Mindst 2 spillere er joinede.

**Beskrivelse:**
Host klikker "Start spil". Systemet ændrer spillets status til `InProgress`, sætter første spiller som aktiv og sender `GameStarted`-eventet til alle spillere. Alle navigerer til spilsiden.

**Successkriterium:** Alle spillere ser spilsiden med terninger og scoreark.

**Fejlscenarier:**
- Ikke nok spillere → fejlbesked
- Ikke host der prøver at starte → fejlbesked

> **Teknisk reference:** [`GameAppService.StartGameAsync`](backend.md#gameappservice) · [`GameHub.StartGame`](realtime.md#startgame)

---

## UC-04: Slå med terninger

**Aktør:** Aktiv spiller (spillerens tur)

**Forudsætning:** Det er spillerens tur. Der er færre end 3 slag i runden.

**Beskrivelse:**
Spilleren klikker "Slå". De terninger der ikke er holdt, får nye tilfældige værdier (1–6). Alle spillere ser animationen og de nye terningsværdier via `DiceRolled`-eventet. Slagtælleren øges.

**Successkriterium:** Terningerne viser nye værdier. Scoreforslag opdateres.

**Fejlscenarier:**
- Ikke spillerens tur → fejlbesked
- Allerede 3 slag brugt → fejlbesked (knap er deaktiveret)

> **Teknisk reference:** [`GameplayAppService.RollDiceAsync`](backend.md#gameplayappservice) · [`GameHub.RollDice`](realtime.md#rolldice)

---

## UC-05: Hold terning

**Aktør:** Aktiv spiller

**Forudsætning:** Mindst 1 slag er foretaget i runden.

**Beskrivelse:**
Spilleren klikker på en terning for at holde eller frigøre den. En holdt terning rulles ikke ved næste slag. Alle spillere ser opdateringen via `HoldChanged`-eventet.

**Successkriterium:** Terningen markeres visuelt som holdt.

> **Teknisk reference:** [`GameplayAppService.ToggleHoldAsync`](backend.md#gameplayappservice) · [`GameHub.ToggleHold`](realtime.md#togglehold)

---

## UC-06: Vælg kategori

**Aktør:** Aktiv spiller

**Forudsætning:** Mindst 1 slag er foretaget (kan vælge allerede efter første slag).

**Beskrivelse:**
Spilleren klikker på en ikke-brugt kategori i scorearket. Pointene beregnes baseret på de aktuelle terningsværdier. Kategorien markeres som brugt. Turen overgår til næste spiller. Alle spillere modtager opdateret spilstatus.

**Successkriterium:** Kategorien vises med points. Næste spiller aktiveres.

**Fejlscenarier:**
- Kategorien er allerede brugt → fejlbesked
- Ikke spillerens tur → fejlbesked

**Bonusregel:** Hvis øvre sektion summerer til ≥ 63 points, tildeles 50 bonuspoint.

> **Teknisk reference:** [`ScoreCalculator`](backend.md#scorecalculator) · [`GameplayAppService.SelectScoreAsync`](backend.md#gameplayappservice) · [`GameHub.SelectScore`](realtime.md#selectscore)

---

## UC-07: Yatzy-fejring

**Aktør:** System (automatisk) / Host (manuelt)

### Automatisk fejring
Når en spiller stopper terningerne og alle 5 viser samme værdi (Yatzy), og kategorien "Yatzy" ikke allerede er brugt, sender klienten automatisk en fejring til alle via SignalR. En GIF vises for alle spillere.

### Manuel fejring (Host Fun)
Host kan trykke på 🎉-knappen og vælge en GIF fra konfigurationslisten. GIF'en sendes til alle spillere med `TriggerYatzy`-eventet.

**Successkriterium:** Alle spillere ser den samme GIF på skærmen.

> **Teknisk reference:** [`GameHub.TriggerYatzy`](realtime.md#triggeryatzy) · [GIF-systemet](gif-system.md)

---

## UC-08: Videochat

**Aktør:** Alle spillere

**Forudsætning:** Browser har adgang til kamera og mikrofon.

**Beskrivelse:**
Når spilleren joiner, etableres automatisk WebRTC peer-to-peer forbindelser til alle andre spillere i rummet. Video-streams vises i et grid øverst på spilsiden.

**Successkriterium:** Alle spillere kan se og høre hinanden.

**Alternativt forløb:** Hvis kamera/mikrofon afvises, spilles der videre uden video.

> **Teknisk reference:** [`VideoHub`](realtime.md#videohub) · [`WebRtcService`](frontend.md#webrtcservice)

---

## UC-09: Forlad spil

**Aktør:** Spiller

**Beskrivelse:**
Spilleren klikker "Forlad". Kamera og mikrofon stoppes, SignalR-forbindelsen lukkes, og spilleren navigeres tilbage til forsiden. Øvrige spillere modtager `PlayerLeft`-eventet og den pågældende spiller vises ikke længere i videogriddet.

**Hvis host forlader:** Spillet afsluttes for alle spillere.

> **Teknisk reference:** [`GameHub.LeaveGame`](realtime.md#leavegame) · [`game.component.ts leaveGame()`](frontend.md#game-component)

---

## UC-10: Genforbind

**Aktør:** Spiller (der har mistet forbindelsen midlertidigt)

**Beskrivelse:**
Hvis en spiller mister SignalR-forbindelsen (fx netværksudfald), markeres vedkommende som ikke-forbundet. Når forbindelsen genetableres, kalder klienten `JoinRoom` igen og modtager fuldt spilstate.

> **Teknisk reference:** [`GameplayAppService.PlayerReconnectedAsync`](backend.md#gameplayappservice) · [`GameHub.JoinRoom`](realtime.md#joinroom)

---

## UC-11: Spil slutter

**Aktør:** System

**Forudsætning:** Alle spillere har udfyldt alle 15 kategorier.

**Beskrivelse:**
Når den sidste kategori vælges, sætter systemet spillets status til `Completed` og sender `GameEnded`-eventet. Alle spillere ser en resultatliste med endelig score sorteret fra højeste til laveste.

**Successkriterium:** Vinderen fremhæves. Spillerne kan vende tilbage til lobby.

> **Teknisk reference:** [`Game.IsGameOver`](backend.md#game) · [`GameHub.SelectScore`](realtime.md#selectscore)
