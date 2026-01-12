# SETUP.md
## Kom igång med databasen i Visual Studio

Denna guide visar hur du som utvecklare får databasen att stämma med de senaste ändringarna via EF Core migrationer, 
utan att behöva använda kommandoprompten.

---

### 1️⃣ Kontrollera att projektet kompilerar

1. Öppna solutionen i Visual Studio.  
2. Bygg solutionen (`Build → Build Solution` eller `Ctrl+Shift+B`).  
3. Säkerställ att inga fel visas.

---

### 2️⃣ Öppna Package Manager Console (PMC)

1. Gå till **Tools → NuGet Package Manager → Package Manager Console**.  
2. Välj **MyZombieProjecr.App** som Default project (där `DbContext` finns).  

---

### 3️⃣ Uppdatera databasen med migrationer

I **Package Manager Console**, kör:

Update-Database


Detta applicerar alla senaste migrationer på din lokala databasanvändare (localdb)\MSSQLLocalDB.
Om databasen inte finns skapas den automatiskt.


### 4️⃣ Verifiera databasen

1. Öppna **SQL Server Object Explorer** (`View → SQL Server Object Explorer`).  
2. Anslut till `(localdb)\MSSQLLocalDB`.  
3. Expandera **Databases → ZombieDb**.  
4. Kontrollera att tabellerna och eventuella seed-data finns.


💡 **Sammanfattning**

- Bygg solution → öppna PMC (**Tools → NuGet Package Manager → Package Manager Console**) → `Update-Database` → kontrollera med SQL Server Object Explorer.  
- Detta säkerställer att alla utvecklare har samma schema och senaste ändringar lokalt.