# GoodBad

**Køb bedre, køb sjældnere.** GoodBad er en anmeldelsesplatform for *produkter* (i modsætning til
Trustpilot, der anmelder *firmaer*). Man tager et billede af produktet, siger god/dårlig, fortæller
**hvad** der er galt (eller hvorfor det er så godt), og det kobles til den rigtige
**produktkategori**, så andre kan finde det, bekræfte det og finde et fix.

Bygget i ASP.NET Core 8 MVC med EF Core + ASP.NET Core Identity. Kører på Simply.com
(self-contained `win-x86` + Web Deploy) med en **MySQL-database** (fx en af dine 3 ekstra MySQL-databaser)
og billedupload til dit eget webhotel.

---

## 1. Funktioner

| Krav | Løsning i koden |
|---|---|
| Tag et billede af produktet, sig god/dårlig | `Products/Details` → 👎/👍 (`Verdict`) + billedvælger (kamera, kamerarulle, Google Photos eller link) |
| Uddyb *hvad* der er galt / hvorfor det er godt | `Review.Body` (obligatorisk) + valg af konkrete **aspekter** |
| Træk problemet til den rigtige produktkategori | `Aspect` ↔ `CategoryAspect`. Aspekter er kategorispecifikke, fælles aspekter gælder alle steder |
| Kategorisering af alle menneskeskabte produkter | `Data/TaxonomySeed.cs` – 18 topkategorier med 150+ underkategorier (research-baseret) |
| Andre brugere kan komme med fixes | `Fixes/Create` på hver anmeldelse og på produktsiden |
| Systemet finder **rigtige** YouTube-videoer og Reddit-tråde | `FixSuggestionService` + `YouTubeSearchClient` (Data API v3) + `RedditSearchClient` (OAuth eller offentligt JSON-endpoint) – kører automatisk ved ny anmeldelse |
| "Det problem har jeg også" | `Reviews/VoteAspect` (`ReviewAspectVote`) |
| "Jeg er enig i at produktet er godt/dårligt" | `Reviews/Vote` (`ReviewVote`) |
| Produkt knyttet til brand/virksomhed med logo/link | `Brand` (logo, hjemmeside, land) + `Brands/Details` |
| Forsiden fremhæver virkelig gode, holdbare produkter | `Product.IsRecommended` → "GoodBad anbefaler" |
| Lister med "must have" pr. kategori | `ProductList` – fx *Værktøj til hobbyværkstedet*, *Regntøj til børn*, *Legetøj af høj kvalitet* |
| "Du skulle have købt dette i stedet" / "Godt at du ikke købte dette" | `ProductRecommendation` – vises som callout på produktsiden |
| Alt skal kunne styres | **Admin-område** på `/Admin`: kategorier, aspekter, brands, produkter, fix-regler, fixes, anmeldelser, lister, anbefalinger, brugere, indstillinger |
| Egen database | EF Core-migrationer + seed af taksonomi, brands, produkter, anmeldelser og keyword-regler |

**Bonus:** bayesiansk kvalitetsscore (0–100) pr. produkt, så et produkt med mange anmeldelser ikke
automatisk slår et med få, samt en advarselsliste over produkter fællesskabet fraråder.

---

## 2. Teknisk overblik

- **.NET 8** (LTS), ASP.NET Core **MVC** + Razor, Bootstrap 5, dansk UI.
- **EF Core 8** med **Pomelo MySQL**-provideren (fungerer med både MySQL 8 og MariaDB – Simply bruger
  typisk MySQL 8). Versionen auto-detekteres ved opstart, men kan pinnes med `Database:ServerVersion`.
- **ASP.NET Core Identity** (brugere, login, roller: `Admin`).
- **Billedupload** til `wwwroot/uploads` (med automatisk fallback til `App_Data/uploads`, hvis webhotellet
  ikke tillader skrivning i web-roden). Ingen ekstern billedtjeneste nødvendig.
- **Ingen API-nøgler er påkrævet** for at appen virker: mangler YouTube-nøglen eller
  Reddit-legitimationsoplysninger, falder fix-motoren tilbage til søgelinks, så featuren aldrig ser
  i stykker ud. Med nøgler (gratis) kommer der rigtige videoer og tråde ind.

### Projektstruktur

```
src/GoodBad.Web/
├─ Areas/Admin/      Admin-område (controllers + views for alt indhold)
├─ Controllers/      Home, Products, Categories, Reviews, Fixes, Lists, Brands, Account
├─ Data/             AppDbContext, DbSeeder, TaxonomySeed, Migrations
├─ Models/           Category, Brand, Product, Review, Aspect, Fix, ProductList, SiteSetting, ...
├─ Services/         FixSuggestionService, YouTubeSearchClient, RedditSearchClient,
│                    ImageStorageService, UploadLocation, SiteSettingsService, AdminBootstrapper
├─ ViewModels/       Projektioner til visning
├─ Views/            Razor-visninger (dansk)
└─ wwwroot/          CSS, JS (image-picker.js) og uploads/
database/
└─ goodbad-mysql.sql Færdigt MySQL-skema + seed-data til import i phpMyAdmin
tests/
└─ GoodBad.Web.Tests xUnit-tests for YouTube-/Reddit-klienterne, upload og fix-nøgleord
```

### Datamodel (kort)

```
Category ──< Product >── Brand
                │
                ├──< Review >── ApplicationUser        (Review.ImageUrl = foto)
                │       ├──< ReviewAspect >── Aspect ──< CategoryAspect >── Category
                │       │        └──< ReviewAspectVote  ("det problem har jeg også")
                │       └──< ReviewVote               ("jeg er enig")
                ├──< Fix >── Review, ApplicationUser, Aspect
                │      └──< FixVote
                ├──< ProductListItem >── ProductList
                └──< ProductRecommendation  ("køb dette i stedet")
FixSuggestionRule    (nøgleord -> fix-forslag, redigeres i /Admin/FixRegler)
SiteSetting          (alt på /Admin/Indstillinger)
```

---

## 3. Kør lokalt

Du skal bruge en lokal MySQL/MariaDB (appen bruger ikke længere SQLite).

```bash
# 1) Opret en lokal database
mysql -u root -e "CREATE DATABASE goodbad CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"

# 2) Giv appen connection string'en (uden at gemme den i en fil)
export ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=goodbad;User=root;Password=;Character Set=utf8mb4"

# 3) Kør
dotnet run --project src/GoodBad.Web/GoodBad.Web.csproj
```

Alternativt: skriv strengen ind i `appsettings.Development.json` (der ligger et udkommenteret
eksempel klar). Databasen oprettes og seedes automatisk ved første start – du skal ikke køre SQL selv.

Første konto, der registrerer sig, bliver **administrator** (så `/Admin` kan nås med det samme).
Sæt gerne din egen e-mail i `Admin:Emails` i `appsettings.Production.json`, så er du sikker på at have
adgang.

### Tests og migrationer

```bash
dotnet test                                              # 19 tests
dotnet tool restore
dotnet ef migrations add <Navn> --project src/GoodBad.Web --output-dir Data/Migrations
dotnet ef migrations script --project src/GoodBad.Web -o migration.sql
```

---

## 4. Deploy til Simply.com

Simply.com kører delt IIS-hosting. Til .NET-versioner der ikke er installeret på serveren kræver de
**self-contained deployment (SCD) med `win-x86`** og **Web Deploy**.

### 4.1 Database: brug MySQL

Du har kun **én MS SQL-database** og den er i brug, men **3 ekstra MySQL-databaser** – derfor bruger
GoodBad MySQL. Det kræver ingen ændringer ud over connection string'en.

1. Log ind i Simplys kontrolpanel → **MySQL** → opret en database (fx `goodbad`).
2. Find oplysningerne under **Administration → Loginoplysninger → MySQL**:
   server (fx `mysql12.simply.com`), databasenavn, bruger og adgangskode.

**Valgfrit – vil du se/redigere databasen i et værktøj først?**
Importér `database/goodbad-mysql.sql` i phpMyAdmin (vælg databasen → *Importér* → upload filen), eller
åbn den i **MySQL Workbench** / **Visual Studio Server Explorer** (View → Server Explorer → *Add
Connection* → MySQL). Scriptet indeholder hele skemaet (23 tabeller) **og** det seed-indhold appen selv
opretter, så du kan se kategorier, produkter og anmeldelser med det samme.

Du *behøver* ikke importere noget: appen kører selv EF Core-migrationerne og seeder indholdet ved
første start.

### 4.2 Udfyld connection string

`src/GoodBad.Web/appsettings.Production.json` indeholder færdig struktur og vejledning – erstat
pladsholderne:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=mysql12.simply.com;Port=3306;Database=goodbad;User=ditbrugernavn_goodbad;Password=DIN-ADGANGSKODE;Character Set=utf8mb4;SslMode=Preferred;AllowPublicKeyRetrieval=True;"
}
```

| Felt | Hvor finder du det |
|---|---|
| `Server` | MySQL-serveren i kontrolpanelet, fx `mysql12.simply.com` |
| `Database` | Databasenavnet (ofte `<brugernavn>_<navn>`) |
| `User` | Databasebrugeren – den samme bruger kan bruge alle dine MySQL-databaser |
| `Password` | Adgangskoden fra kontrolpanelet |

- Har serveren et selvsigneret certifikat, så brug `SslMode=Preferred` (eller `SslMode=None` hvis
  forbindelsen bliver afvist – data ligger alligevel kun på Simplys eget net).
- Undgå `;` og `"` i adgangskoden, da de ødelægger connection string-syntaksen.
- Vil du ikke have koden liggende i filen, så sæt den som miljøvariabel i stedet
  (`ConnectionStrings__DefaultConnection`) – den vinder over JSON-filen.

### 4.3 Publicer

Udfyld pladsholderne i `src/GoodBad.Web/Properties/PublishProfiles/SimplyWebDeploy.pubxml`
(server/login står i Simplys kontrolpanel → **Administration → Loginoplysninger**).

**Visual Studio:** Build → Publish → profilen `SimplyWebDeploy`. Kontrollér at:

- **Deployment Mode** = *Self-contained*
- **Target Runtime** = **`win-x86`** (vigtigt hos Simply)
- **Hosting model** = *Out of process* (et webhotel har kun én application pool)

**CLI:**

```bash
dotnet publish src/GoodBad.Web/GoodBad.Web.csproj -c Release -p:PublishProfile=SimplyWebDeploy
```

`web.config` genereres med `AspNetCoreModuleV2`, `processPath=".\GoodBad.Web.exe"` og
`hostingModel="outofprocess"`.

### 4.4 Billeder på webhotellet

Fotos gemmes i `wwwroot/uploads/reviews` (og `…/products`, `…/brands`) og serveres direkte af IIS.
Det gør billedupload til "dit eget webhotel" uden eksterne tjenester.

Kan app-poolen ikke skrive i `wwwroot`, falder appen automatisk tilbage til `App_Data/uploads` og
serverer den mappe via et separat static-file-provider på samme URL (`/uploads/...`). Du kan også pege
den et bestemt sted hen i `appsettings.json`:

```json
"Storage": {
  "UploadWebPath": "/uploads",
  "UploadFolder": "wwwroot/uploads",
  "MaxImageBytes": 8388608
}
```

`MaxImageBytes` og de tilladte filtyper kan også ændres under **Admin → Indstillinger**.

### 4.5 Slå rigtige YouTube- og Reddit-opslag til

Begge dele er gratis, og begge kan sættes under **Admin → Indstillinger** (dér vinder værdierne over
`appsettings`). På `/Admin` (Overblik) viser appen en advarsel, indtil de er sat.

**YouTube Data API v3**
1. Opret et projekt i [Google Cloud Console](https://console.cloud.google.com/apis/library/youtube.googleapis.com).
2. Aktivér *YouTube Data API v3* → **Opret legitimationsoplysninger → API-nøgle**.
3. Indsæt nøglen i feltet *YouTube Data API-nøgle*.
   (Gratis kvote: 100 søgninger/dag – appen cacher resultater i 24 timer.)

**Reddit**
Reddit afviser anonyme JSON-kald fra webhoteller (HTTP 403). Opret en app, og appen søger i stedet
via Reddits godkendte API:
1. Gå til [reddit.com/prefs/apps](https://www.reddit.com/prefs/apps) → *create another app…*
2. Vælg typen **script** (eller *installed app*) og gem.
3. Kopiér **client id** (under appnavnet) og **secret** ind i *Reddit Client ID* og *Reddit Client Secret*.

Uden nøgler virker fix-motoren stadig – den viser så bare links til YouTube-/Reddit-søgninger i stedet
for konkrete videoer og tråde.

### 4.6 Fejlfinding på Simply

| Symptom | Løsning |
|---|---|
| **500.30 / 500.31** | App-pool og arkitektur matcher ikke. Sørg for `win-x86` (SCD) og out-of-process hosting |
| **503 Service Unavailable** | App-pool er crashet/throttlet. Sæt `stdoutLogEnabled="true"` i `web.config` og læs `logs\stdout_*.log` |
| **Access denied for user … to database** | Forkert brugernavn/database i connection string, eller databasen er ikke givet til brugeren i kontrolpanelet |
| **Ingen billeder efter upload** | Kontrollér skriverettigheder til `wwwroot/uploads` (appen falder ellers tilbage til `App_Data/uploads`) |
| **CSS mangler efter deploy** | Sørg for at hele `wwwroot` er uploadet |
| **Ingen rigtige Reddit-tråde** | Client ID/secret mangler (se 4.5). Tjek at klokken på serveren er rigtig |

---

## 5. Billedupload (kamera, kamerarulle, Google Photos)

Billedvælgeren ligger i `Views/Shared/_ImagePicker.cshtml` + `wwwroot/js/image-picker.js` og bruges på
produktformularen, på anmeldelsesformularen og i admin:

- **📷 Tag billede** – åbner kameraet direkte på mobilen (`capture="environment"` → bagkameraet).
- **🖼 Vælg billede** – åbner telefonens normale billedvælger, hvor både kamerarullen og **Google Photos**
  ligger (Google Photos dukker op som en del af OS'ets filvælger).
- **Linkfelt** – man kan indsætte en URL til et billede i stedet (fx fra en webshop).
- Upload valideres på størrelse, filtype og MIME-type, får et unikt filnavn og slettes fra disk igen,
  når billedet udskiftes eller indholdet slettes i admin (`ImageStorageService.Delete`).

---

## 6. Hvordan fix-forslag virker

1. Brugeren skriver sin anmeldelse (fx "plastikken i spændet flækkede" eller "håndtaget ruster").
2. **Kuraterede regler** fra `FixSuggestionRules` matches på kommaseparerede nøgleord
   (redigeres i **Admin → Fix-regler**).
3. **Live-søgning**: de fundne nøgleord (plus produktets kategori som kontekst, fx "rust værktøj")
   sendes til YouTube Data API og Reddit. Rigtige resultater gemmes som system-fixes på anmeldelsen.
4. **Fallback**: giver søgningerne intet (manglende nøgle, kvote, netværk), oprettes et link til
   YouTube-/Reddit-søgningen, så featuren altid virker.
5. Brugeren kan trykke **"Find systemforslag"** igen, eller skrive sit eget fix.

Resultater caches i hukommelsen (YouTube 24 t, Reddit 6 t) – både for at holde kvoten nede og for at
holde svartiden lav.

---

## 7. Admin-området (`/Admin`)

| Side | Kan man |
|---|---|
| **Overblik** | Nøgletal, seneste anmeldelser/brugere, advarsler hvis YouTube-/Reddit-nøgler mangler |
| **Kategorier** | Oprette/redigere/slette kategorier og underkategorier, sortering, ikon |
| **Aspekter** | Typiske problemer/styrker og hvilke kategorier de gælder for |
| **Brands** | Navn, hjemmeside, land, logo (upload eller URL) |
| **Produkter** | Redigere alt, skjule, fremhæve på forsiden, skifte billede, slette |
| **Fix-regler** | Nøgleord → fix-forslag (titel, tekst, link, YouTube/Reddit) |
| **Fixes** | Skjule/slette brugeres og systemets fixes |
| **Anmeldelser** | Søge, filtrere, se billede, skjule, slette |
| **Lister** | Redigere og fremhæve indkøbslister |
| **Anbefalinger** | "Køb dette i stedet"-parringer mellem produkter |
| **Brugere** | Se brugere, give/fjerne Admin-rollen |
| **Indstillinger** | Sidetitel, advarselstekst på forsiden, YouTube-nøgle, Reddit-nøgle/User-Agent, maks. billedstørrelse, moderationsgrænse |

---

## 8. Produktkategorier (taksonomi)

Taksonomien i `Data/TaxonomySeed.cs` er bygget efter samme princip som Amazon/Google Shopping:
**funktion er den primære kategori**, og attributter (brand, materiale, alder) er filtre. De 18
topkategorier dækker alle menneskeskabte fysiske produkter:

Elektronik & computere · Hvidevarer · Køkken & madlavning · Møbler & boligindretning ·
Værktøj & gør-det-selv · Have & udendørs · Tøj, sko & accessories · Skønhed & personlig pleje ·
Sundhed & velvære · Baby & børn · Legetøj, spil & hobby · Sport, fitness & outdoor ·
Bil, motorcykel & mobility · Bøger, musik & medier · Kontor, skole & papirvarer ·
Mad, drikkevarer & husholdning · Kæledyr · Kunst, håndværk & gaver

Hver kategori har egne typiske **problemer** og **styrker** – det er dem, brugeren vælger imellem, og
det er det, der gør anmeldelsen søgbar i kategorien. Alt kan redigeres i **Admin → Kategorier** og
**Admin → Aspekter** (eller i `TaxonomySeed.cs` for nye standarddata).

---

## 9. Næste skridt (ikke implementeret endnu)

- **Moderator-rolle og godkendelseskø** – alle brugere kan i dag oprette produkter og lister.
- **Producent-svar** – mulighed for at en brandejer kommenterer på en anmeldelse.
- **E-mailbekræftelse og password reset** (Identity er sat op, kræver en SMTP-udbyder).
- **Søgning** er `LIKE`-baseret; ved større datamængder brug MySQL full-text search.
- **Billedoptimering** – billeder gemmes i original størrelse; en resize/WebP-konvertering ville sprede mindre.

---

## 10. Licens / bemærkning

Seed-data bruger **opdigtede brands og produkter** (Nordvik, Stålform, BilligTex, VoltMax, …), så der
ikke fremsættes påstande om rigtige virksomheder i demoen. Erstat dem med rigtige data, når platformen
tages i brug. Demo-brugerne i seed-data har ingen adgangskode og kan ikke logges ind med.
