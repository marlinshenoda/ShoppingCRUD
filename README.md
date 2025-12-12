🛒 ShoppingCRUD – E-handelsprojekt (C# / .NET 8 / EF Core / SQLite + JSON)

Detta projekt är utvecklat som en del av kursen Databasutveckling (SYSS8, HT2025) och syftar till att bygga ett komplett e-handelssystem med stöd för både databaslagring (SQLite) och filbaserad lagring (JSON).

📌 Funktionalitet
✔ Produkt- och kategorihantering

Skapa, läsa, uppdatera och ta bort produkter och kategorier

Datavalidering och felhantering

Lagring i SQLite via Entity Framework Core

✔ Kundhantering

CRUD för kunder

Extra fält: PhoneNumber

Lagring i både JSON och SQLite

Filtrering av kunder per stad

✔ Order & OrderRow

Skapa ordrar med flera orderrader

Beräkning av totalbelopp

Transaktionshantering vid orderläggning

Automatisk uppdatering av lagersaldo

✔ Säkerhet

Kryptering av känsliga fält i JSON med en egen EncryptionHelper

Hashning och saltning av lösenord (om användarkonton används)

✔ Databasfunktioner

Migrationer via EF Core

Triggers och vyer (views) används i databasen

Prestandamätning och optimering

🗄 Tekniker & Bibliotek

C# / .NET 8

Entity Framework Core

SQLite

JSON-lagring

LINQ

Triggers & Views

Transaktioner

Kryptering (XOR + Base64)

🚀 Syfte

Projektet visar hur man bygger ett robust, transaktionssäkert och flexibelt e-handelssystem som använder flera datalager samtidigt och uppfyller kursens krav på databasdesign, prestanda och säkerhet.
