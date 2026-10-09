# GoodBad

**Køb bedre, køb sjældnere.** GoodBad er en anmeldelsesplatform for *produkter* (i modsætning til
Trustpilot, der anmelder *firmaer*). Man siger god/dårlig om et produkt, fortæller **hvad** der er galt
(eller hvorfor det er så godt), og det kobles til den rigtige **produktkategori**, så andre kan finde
det, bekræfte det, og finde et fix.

Bygget i ASP.NET Core 8 MVC med EF Core + ASP.NET Core Identity. Kan køre på Simply.com
(self-contained `win-x86` + Web Deploy) med MS SQL Server 2022.

---

## 1. Funktioner (mappet til kravene)

| Krav | Løsning i koden |
|---|---|
| Tag et produkt, sig god/dårlig | `Products/Details` → anmeldelsesformular med 👎/👍 (`Verdict`) |
| Uddyb *hvad* der er galt / hvorfor det er godt | `Review.Body` (obligatorisk) + valg af konkrete **aspekter** |
| Træk problemet til den rigtige produktkategori | `Aspect` ↔ `CategoryAspect`. Aspekter er kategorispecifikke, fælles aspekter gælder alle steder |
| Kategorisering af alle menneskeskabte produkter | `Data/TaxonomySeed.cs` – 18 topkategorier med 150+ underkategorier (research-baseret) |
| Andre brugere kan komme med fixes | `Fixes/Create` på hver anmeldelse og på produktsiden |
| Systemet finder YouTube-tutorials/Reddit-opslag ud fra keywords | `Services/FixSuggestionService.cs` + `FixSuggestionRule`-tabellen, kører automatisk ved ny anmeldelse og via "Find systemforslag" |
| "Det problem har jeg også" | `Reviews/VoteAspect` (`ReviewAspectVote`) |
| "Jeg er enig i at produktet er godt/dårligt" | `Reviews/Vote` (`ReviewVote`) |
| Produkt knyttet til brand/virksomhed/producent med logo/link | `Brand` (logo, hjemmeside, land) og `Brands/Details` |
| Forsiden fremhæver virkelig gode, holdbare produkter | `Products/Details` + `Product.IsRecommended` → "GoodBad anbefaler" |
| Lister med "must have" pr. kategori | `ProductList` / `ProductListItem` – fx *Værktøj til hobbyværkstedet*, *Regntøj til børn*, *Legetøj af høj kvalitet* |
| "Du skulle have købt dette i stedet" / "Godt at du ikke købte dette" | `ProductRecommendation` – vises som callout på produktsiden |
| Egen database | EF Core-migrationer + seed af taksonomi, brands, produkter, anmeldelser og keyword-regler |

**Bonus:** bayesiansk kvalitetsscore (0–100) pr. produkt, så et produkt med mange anmeldelser ikke
automatisk slår et med få, samt en advarselsliste over produkter fællesskabet fraråder.

---

## 2. Teknisk overblik

- **.NET 8** (LTS), ASP.NET Core **MVC** + Razor, Bootstrap 5, dansk UI.
- **EF Core 8** med to providers:
  - `SqlServer` (produktion / Simply.com) – **migrationer** ligger i `Data/Migrations`.
  - `Sqlite` (lokal udvikling) – oprettes automatisk med `EnsureCreated()`.
- **ASP.NET Core Identity** (brugere, login, adgangskoder).
- Ingen eksterne API-nøgler nødvendige: fix-forslag bygges af keyword-regler + genererede
  YouTube/Reddit-søgelinks, så det virker på shared hosting.

### Projektstruktur

```
src/GoodBad.Web/
├─ Controllers/      Home, Products, Categories, Reviews, Fixes, Lists, Brands, Account
├─ Data/             AppDbContext, DbSeeder, TaxonomySeed, Migrations
├─ Models/           Category, Brand, Product, Review, Aspect, Fix, ProductList, ...
├─ Services/         FixSuggestionService, CatalogQueries, SlugHelper
├─ ViewModels/       Projektioner til visning
├─ Views/            Razor-visninger (dansk)
└─ wwwroot/css/site.css
```

### Datamodel (kort)

```
Category ──< Product >── Brand
                │
                ├──< Review >── ApplicationUser
                │       ├──< ReviewAspect >── Aspect ──< CategoryAspect >── Category
                │       │        └──< ReviewAspectVote  ("det problem har jeg også")
                │       └──< ReviewVote               ("jeg er enig")
                ├──< Fix >── Review, ApplicationUser, Aspect
                │      └──< FixVote
                ├──< ProductListItem >── ProductList
                └──< ProductRecommendation  ("køb dette i stedet")
```

---

## 3. Kør lokalt

```bash
# .NET 8 SDK kræves
dotnet build
dotnet run --project src/GoodBad.Web/GoodBad.Web.csproj
```

Uden en connection string falder appen automatisk tilbage til en **SQLite-fil** (`goodbad.db`), og
databasen oprettes + seedes ved første start. Åbn derefter den viste URL (fx `http://localhost:5012`).

Vil du tvinge SQLite/SQL Server manuelt, sæt `Database:Provider` til `Sqlite` eller `SqlServer`
i `appsettings.Development.json`.

### EF Core-migrationer

```bash
dotnet tool restore
dotnet ef migrations add <Navn> --project src/GoodBad.Web --output-dir Data/Migrations
dotnet ef migrations script --project src/GoodBad.Web -o migration.sql   # til manuel kørsel
```

---

## 4. Deploy til Simply.com

Simply.com kører delt IIS-hosting. Til .NET-versioner der ikke er installeret på serveren kraever de
**self-contained deployment (SCD) med `win-x86`** og **Web Deploy**. Databasen er **MS SQL Server 2022**
(findes på Standard/Pro/Enterprise – vælg et webhotel med MS SQL).

### 4.1 Opret databasen
1. Log ind i Simplys kontrolpanel → opret et **MS SQL-database** og en databasebruger.
2. Noter server, databasenavn, brugernavn og adgangskode.

### 4.2 Udfyld connection string
Redigér `src/GoodBad.Web/appsettings.Production.json`:

```json
{
  "Database": { "Provider": "SqlServer" },
  "ConnectionStrings": {
    "DefaultConnection": "Server=DIN-SERVER;Database=DIN-DB;User Id=DIN-BRUGER;Password=DIN-KODE;MultipleActiveResultSets=true;TrustServerCertificate=true;Encrypt=true"
  }
}
```

Tabellen oprettes automatisk ved første start (`MigrateAsync()` + seed). Har databasebrugeren ikke
DDL-rettigheder, så kør `dotnet ef migrations script` og indlæs SQL-scriptet manuelt via phpMyAdmin/
MS SQL-værktøjet i kontrolpanelet.

### 4.3 Publicer
Udfyld pladsholderne i `src/GoodBad.Web/Properties/PublishProfiles/SimplyWebDeploy.pubxml`
(server/login står i Simplys kontrolpanel → **Administration → Loginoplysninger**).

**Visual Studio:** Build → Publish → profilen `SimplyWebDeploy`.
Kontrollér i profilen at:

- **Deployment Mode** = *Self-contained*
- **Target Runtime** = **`win-x86`** (vigtigt hos Simply)
- **Hosting model** = *Out of process* (et webhotel har kun én application pool)

**CLI:**

```bash
dotnet publish src/GoodBad.Web/GoodBad.Web.csproj -c Release -p:PublishProfile=SimplyWebDeploy
```

Profilen publicerer via Web Deploy. `web.config` genereres/transformeres automatisk til IIS med
`AspNetCoreModuleV2`, `processPath=".\GoodBad.Web.exe"` og `hostingModel="outofprocess"`.

### 4.4 Fejlfinding på Simply
- **500.30 / 500.31** → app-pool og arkitektur matcher ikke. Sørg for `win-x86` (SCD) og
  out-of-process hosting.
- **503 Service Unavailable** → app-pool er crashet eller throttlet. Sæt
  `stdoutLogEnabled="true"` i `web.config` og læs `logs\stdout_*.log`.
- **CSS mangler efter deploy** → sørg for at hele `wwwroot` er uploadet.

---

## 5. Produktkategorier (taksonomi)

Taksonomien i `Data/TaxonomySeed.cs` er bygget efter samme princip som Amazon/Google Shopping:
**funktion er den primære kategori**, og attributter (brand, materiale, alder) er filtre. De 18
topkategorier dækker alle menneskeskabte fysiske produkter:

Elektronik & computere · Hvidevarer · Køkken & madlavning · Møbler & boligindretning ·
Værktøj & gør-det-selv · Have & udendørs · Tøj, sko & accessories · Skønhed & personlig pleje ·
Sundhed & velvære · Baby & børn · Legetøj, spil & hobby · Sport, fitness & outdoor ·
Bil, motorcykel & mobility · Bøger, musik & medier · Kontor, skole & papirvarer ·
Mad, drikkevarer & husholdning · Kæledyr · Kunst, håndværk & gaver

Hver kategori har egne typiske **problemer** og **styrker** – det er dem, brugeren vælger imellem, og
det er det, der gør anmeldelsen søgbar i kategorien. Nye kategorier/aspekter tilføjes i
`TaxonomySeed.cs` (eller direkte i databasen).

---

## 6. Hvordan fix-forslag virker

1. Brugeren skriver sin anmeldelse (fx "batteriet holder ikke" eller "plastikken føles af lav kvalitet").
2. `FixSuggestionService.SuggestAsync` matcher teksten mod `FixSuggestionRule.Keywords`
   (kommaseparerede, case-insensitive).
3. Ved match oprettes et **system-fix** med titel + link (YouTube/Reddit).
4. Matcher intet, udtrækkes de mest relevante ord (dansk stopordsliste) og der bygges
   YouTube- og Reddit-**søgelinks** automatisk.
5. Brugeren kan altid trykke **"Find systemforslag"** igen, eller skrive sit eget fix.

Nye regler tilføjes i `DbSeeder` (afsnittet *Fix suggestion keyword rules*) eller direkte i
tabellen `FixSuggestionRules`. Vil man senere bruge rigtige YouTube/Reddit-API'er, er det kun
`FixSuggestionService` der skal udskiftes – resten af systemet er uændret.

---

## 7. Næste skridt (ikke implementeret endnu)

- **Billedupload**: i dag bruges billed-URL'er (`Product.ImageUrl`). Næste skridt er upload til
  `wwwroot/uploads` eller til et cloud-bucket.
- **Moderering/roller**: alle brugere kan i dag oprette produkter og lister. Tilføj en `Moderator`-rolle
  og en godkendelseskø.
- **Producent-svar**: mulighed for at en brandejer kommenterer på en anmeldelse.
- **"Du skulle have købt dette i stedet"** vedligeholdes i dag i seed-data; mangler et admin-UI.
- **E-mailbekræftelse og password reset** (Identity er sat op, men kræver en SMTP-udbyder).
- **Søgning** er i dag `LIKE`-baseret; ved større datamængder brug full-text search.

---

## 8. Licens / bemærkning

Seed-data bruger **opdigtede brands og produkter** (Nordvik, Stålform, BilligTex, VoltMax, …), så der
ikke fremsættes påstande om rigtige virksomheder i demoen. Erstat dem med rigtige data, når platformen
tages i brug.
