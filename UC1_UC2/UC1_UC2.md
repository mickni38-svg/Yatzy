# Use Cases – Venner (UC1 + UC2)

## UC1 + UC2: Søg efter bruger og send venneanmodning

### Aktør
App-bruger (A)

### Forudsætninger
- A er logget ind i appen.
- UI er som i [UI/UX] (.\index.html) (mørkt tema, søgefelt, resultater i kort).

---

### Flow
1. A går til fanen **Venner**.  
2. A indtaster navn, email eller telefonnummer i søgefeltet.  
3. Systemet søger i **databasen** efter brugere, der matcher input.  
4. Resultaterne vises som kort med avatar, navn og email.  
5. A klikker på et kort (fx bruger B).  
6. Systemet opretter en **venneanmodning** i databasen med:  
   - Requester = A  
   - Receiver = B  
   - Status = *Pending*.  
7. UI opdateres:  
   - Kortet viser nu badge **“Afventer…”** i stedet for *Tilføj ven*.  
   - Brugeren tilføjes samtidig i A’s venneoversigt med status *Afventer*.  

---

### Resultat
- A kan se, at anmodningen til B er sendt og afventer svar.  
- A kan ikke sende en ny anmodning til samme bruger, så længe status = *Afventer*.  

---

### Bemærkninger
- **Notifikation til B** (modtageren) om, at der er kommet en ny venneanmodning, håndteres i en **separat use case**.  
- Denne use case dækker kun søgning, valg af bruger og oprettelse af anmodning med status *Afventer*.  
- UI/UX eksempel:[search-users.html](https://mickni38-svg.github.io/batchpay/UC1_UC2/search-users.html)
