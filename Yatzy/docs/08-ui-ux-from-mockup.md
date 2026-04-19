# UI/UX ud fra mockup

## Formål
Denne fil omsætter mockup-retningen til Angular browser-UI.

Mockup’en viser tre vigtige skærmtilstande:
1. lobby – klar til start
2. aktiv tur – første kast
3. vælg kategori og se stillingen opdatere

Det samme flow skal bruges i browserløsningen.

---

## Centrale UX-principper
- scorekortet er centrum
- grøn Yatzy-visual identitet
- tydelig aktiv spiller
- tydelig turstatus
- holdte terninger skal være visuelt markeret
- scorevalg skal være hurtigt og forståeligt

Den tidligere MAUI-guide understregede også, at scorekortet skal være centrum og at det skal være klart hvem der har tur, hvor mange kast der er tilbage, og hvilke kategorier der stadig er ledige fileciteturn0file1L25-L37

---

## Browser-layout forslag

### Desktop / Chrome
Brug 3 kolonner:
- venstre: spillerliste / avatars / status
- center: terninger og handlinger
- højre: scorekort

### Smalere bredde
Brug 1 kolonne:
- top: banner
- midt: spillerliste
- under: terninger
- nederst: scorekort

---

## Lobby-skærm
Vis:
- room code
- spillere
- max 4 spillere
- status for hvem der er klar
- start-knap for host

---

## Aktiv tur
Vis:
- rundeinfo
- hvem der har tur
- kast 1 af 3 / 2 af 3 / 3 af 3
- fem terninger
- hold-knapper
- kast igen-knap

---

## Kategorivalg
Vis:
- anbefalede point for ledige kategorier
- tydelige knapper
- bekræft valg
- stilling nederst

---

## Visuel adfærd
- holdte terninger får grøn border eller badge
- deaktivér kast-knap når spilleren ikke må kaste
- deaktivér kategorier der allerede er brugt
- markér aktiv spiller med highlight

---

## Copilot-prompt
> Build the browser UI for the Yatzy game based on the uploaded mockup: lobby, active turn, and score selection. Use a clean Angular component structure, a classic green Yatzy look, a central scoreboard, clear turn information, and strong visual indicators for held dice and active player.

---

## Done-kriterier
Fasen er færdig når UI:
- minder om mockup-retningen
- er overskueligt i Chrome
- gør turn flow tydeligt
- gør scorevalg tydeligt
