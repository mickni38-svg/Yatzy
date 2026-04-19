# Yatzy browser edition – Copilot package

Dette dokument samler retningen for at omskrive den tidligere MAUI Android-idé til en browser-baseret løsning.

## Ny målstak
- Angular + TypeScript frontend
- ASP.NET Core 9 Web API
- C# class libraries til domæne og logik
- Entity Framework Core til database
- SignalR til realtime multiplayer
- Chrome som målplatform

## Hvorfor denne omskrivning?
Den tidligere version var tænkt som MAUI Android med delt domænelogik, backend og realtime events fileciteturn0file0L5-L17
Nu flyttes klienten til browseren, men de gode principper bevares:
- ren domænelogik
- backend som autoritet
- realtime multiplayer
- tydeligt scorekort i centrum

## Dokumenter i pakken
Start med:
1. `00-step-by-step-guide.md`
2. `10-copilot-master-prompt.md`
3. `11-copilot-prompt-phase-1.md`

Fortsæt derefter fase for fase.
