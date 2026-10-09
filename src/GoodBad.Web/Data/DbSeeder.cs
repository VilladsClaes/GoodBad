using GoodBad.Web.Models;
using GoodBad.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Data;

/// <summary>
/// Seeds the taxonomy, some well-known aspects, demo brands, products, reviews,
/// community fixes, curated lists and the keyword rules used by the
/// fix-suggestion engine. Idempotent: does nothing if categories already exist.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Categories.AnyAsync())
        {
            return;
        }

        var aspects = new Dictionary<string, Aspect>(StringComparer.OrdinalIgnoreCase);

        Aspect AspectFor(string text, AspectKind kind)
        {
            if (aspects.TryGetValue(text, out var existing))
            {
                return existing;
            }

            var aspect = new Aspect
            {
                Text = text,
                Slug = SlugHelper.Slugify(text),
                Kind = kind
            };
            aspects[text] = aspect;
            db.Aspects.Add(aspect);
            return aspect;
        }

        // Common aspects have no category link, meaning they are available everywhere.
        foreach (var p in TaxonomySeed.CommonProblems) AspectFor(p, AspectKind.Problem);
        foreach (var p in TaxonomySeed.CommonPraises) AspectFor(p, AspectKind.Praise);

        var categoryLinks = new List<CategoryAspect>();
        var sort = 0;

        foreach (var def in TaxonomySeed.Categories)
        {
            var category = new Category
            {
                Name = def.Name,
                Slug = SlugHelper.Slugify(def.Name),
                Description = def.Description,
                Icon = def.Icon,
                SortOrder = sort++
            };
            db.Categories.Add(category);

            foreach (var childName in def.Children)
            {
                var child = new Category
                {
                    Name = childName,
                    Slug = SlugHelper.Slugify(childName),
                    Parent = category
                };
                db.Categories.Add(child);
            }

            foreach (var problem in def.Problems)
            {
                categoryLinks.Add(new CategoryAspect { Category = category, Aspect = AspectFor(problem, AspectKind.Problem) });
            }

            foreach (var praise in def.Praises)
            {
                categoryLinks.Add(new CategoryAspect { Category = category, Aspect = AspectFor(praise, AspectKind.Praise) });
            }
        }

        db.CategoryAspects.AddRange(categoryLinks);

        // ----- Demo brands -------------------------------------------------
        var brands = new Dictionary<string, Brand>();
        Brand Brand(string name, string country, string website, string logo, string description)
        {
            var b = new Brand
            {
                Name = name,
                Slug = SlugHelper.Slugify(name),
                Country = country,
                WebsiteUrl = website,
                LogoUrl = logo,
                Description = description
            };
            brands[name] = b;
            db.Brands.Add(b);
            return b;
        }

        var nordvik = Brand("Nordvik", "Danmark", "https://example.com/nordvik", "https://placehold.co/160x80?text=Nordvik",
            "Dansk outdoor-brand kendt for gedigne regnjakker og uldprodukter med mange års garanti.");
        var staalform = Brand("Stålform", "Danmark", "https://example.com/staalform", "https://placehold.co/160x80?text=St%C3%A5lform",
            "Værktøjsmærke der satser på professionel kvalitet og reservedele i mange år.");
        var birkholm = Brand("Birkholm", "Danmark", "https://example.com/birkholm", "https://placehold.co/160x80?text=Birkholm",
            "Familieforetagende der laver klassiske træprodukter og legetøj af FSC-træ.");
        var kobber = Brand("Kobber & Co", "Danmark", "https://example.com/kobber", "https://placehold.co/160x80?text=Kobber",
            "Køkkenudstyr i kobber og stål til langsigtet brug.");
        var terra = Brand("Terra Outdoor", "Sverige", "https://example.com/terra", "https://placehold.co/160x80?text=Terra",
            "Skandinavisk friluftsmærke med fokus på slidstyrke.");
        var billigtex = Brand("BilligTex", "Kina", "https://example.com/billigtex", "https://placehold.co/160x80?text=BilligTex",
            "Lavprismærke. Kortere levetid og svær adgang til reservedele.");
        var voltmax = Brand("VoltMax", "Kina", "https://example.com/voltmax", "https://placehold.co/160x80?text=VoltMax",
            "Lavpris elektronik uden service.");
        var plasto = Brand("PlastoByg", "Kina", "https://example.com/plastobyg", "https://placehold.co/160x80?text=PlastoByg",
            "Plastlegetøj i meget lav prisklasse.");

        // ----- Helper to build categories lookup ---------------------------
        // Lookup of the categories created above. It is refreshed after
        // SaveChanges so we always work with the currently tracked instances.
        var categoriesBySlug = db.ChangeTracker.Entries<Category>()
            .Select(e => e.Entity)
            .GroupBy(c => c.Slug)
            .ToDictionary(g => g.Key, g => g.First());
        Category Find(string slug) => categoriesBySlug[slug];

        // ----- Products ----------------------------------------------------
        var products = new List<Product>();
        Product Product(string name, string slug, Brand brand, string categorySlug, bool recommended, string model, string image, string description)
        {
            var p = new Product
            {
                Name = name,
                Slug = slug,
                Brand = brand,
                Category = Find(categorySlug),
                IsRecommended = recommended,
                ModelNumber = model,
                ImageUrl = image,
                Description = description
            };
            products.Add(p);
            db.Products.Add(p);
            return p;
        }

        var stormJakke = Product("Nordvik Storm Regnjakke", "nordvik-storm-regnjakke", nordvik, "regntoej", true,
            "NV-900", "https://placehold.co/400x300?text=Storm",
            "Trelags regnjakke med tapede sømme, til børn og voksne. Kan repareres og får reservedele i 10 år.");
        var billigJakke = Product("BilligTex Regnjakke Junior", "billigtex-regnjakke-junior", billigtex, "regntoej", false,
            "BT-JR", "https://placehold.co/400x300?text=BilligTex",
            "Billig børneregnjakke. Let at købe, men sømme og lynlås holder ikke til daglig brug.");
        var borehammer = Product("Stålform Akku-borehammer SH18", "staalform-akku-borehammer-sh18", staalform, "elvaerktoej", true,
            "SH18", "https://placehold.co/400x300?text=SH18",
            "18V akku-borehammer med to batterier, metalgear og 8 års reservedelsgaranti.");
        var powermax = Product("VoltMax PowerMax 18V Boremaskine", "voltmax-powermax-18v", voltmax, "elvaerktoej", false,
            "PM-18", "https://placehold.co/400x300?text=PowerMax",
            "Lavpris boremaskine. Batteriet mister kapacitet hurtigt og der findes ikke reservedele.");
        var klodser = Product("Birkholm Træklodser 100 dele", "birkholm-traeklodser-100", birkholm, "byggeklodser-konstruktion", true,
            "BH-100", "https://placehold.co/400x300?text=Tr%C3%A6klodser",
            "Massivt bøgetræ, gennemtestet og kan gå i arv i generationer.");
        var plastbyg = Product("PlastoByg Byggeklodser 250 dele", "plastobyg-byggeklodser-250", plasto, "byggeklodser-konstruktion", false,
            "PB-250", "https://placehold.co/400x300?text=PlastoByg",
            "Meget billige plastklodser. Flere dele mangler og kanterne er skarpe.");
        var pande = Product("Kobber & Co Stegepande 28 cm", "kobber-co-stegepande-28", kobber, "gryder-pander", true,
            "KC-28", "https://placehold.co/400x300?text=Stegepande",
            "Kobberkerne med stålbund, tåler opvaskemaskine og holder varmen i årevis.");
        var nonstick = Product("NonStick Pro Stegepande 28 cm", "nonstick-pro-stegepande-28", voltmax, "gryder-pander", false,
            "NS-28", "https://placehold.co/400x300?text=NonStick",
            "Billig pande med slip-let belægning der hurtigt slipper.");
        var stovler = Product("Terra Outdoor Vandrestøvler", "terra-outdoor-vandrestoevler", terra, "sko-stoevler", true,
            "TO-HIKE", "https://placehold.co/400x300?text=Terra",
            "Vandtætte vandrestøvler med udskiftelig sål og solidt læder.");
        var oplader = Product("VoltMax USB-C Oplader 65W", "voltmax-usb-c-oplader-65w", voltmax, "kabler-opladere-batterier", false,
            "VM-65", "https://placehold.co/400x300?text=Oplader",
            "Billig oplader. Bliver varm og holder op med at virke efter få måneder.");
        var termokande = Product("Birkholm Termokande 1,0 L", "birkholm-termokande-1l", birkholm, "termo-drikkeflasker", true,
            "BH-TERM", "https://placehold.co/400x300?text=Termokande",
            "Rustfri termokande der holder kaffen varm i 12 timer, år efter år.");
        var uldsokker = Product("Nordvik Uldsokker 3-pak", "nordvik-uldsokker-3-pak", nordvik, "undertoej-sokker", true,
            "NV-SOK", "https://placehold.co/400x300?text=Uldsokker",
            "Uldsokker der holder formen og kan stoppes, når de bliver slidte.");

        await db.SaveChangesAsync();

        // ----- Demo users ---------------------------------------------------
        var users = new List<ApplicationUser>();
        for (var i = 1; i <= 4; i++)
        {
            var u = new ApplicationUser
            {
                UserName = $"demo{i}@goodbad.dk",
                Email = $"demo{i}@goodbad.dk",
                EmailConfirmed = true,
                DisplayName = i switch
                {
                    1 => "Mette H.",
                    2 => "Jonas K.",
                    3 => "Sofie L.",
                    _ => "Anders B."
                }
            };
            users.Add(u);
            db.Users.Add(u);
        }

        await db.SaveChangesAsync();

        // Re-fetch through the context so we always work with the instances the
        // change tracker already knows about (EF resolves them via its identity
        // map, so no duplicate/detached graphs are created).
        categoriesBySlug = await db.Categories.ToDictionaryAsync(c => c.Slug);
        nordvik = await db.Brands.FirstAsync(b => b.Slug == "nordvik");
        billigtex = await db.Brands.FirstAsync(b => b.Slug == "billigtex");
        voltmax = await db.Brands.FirstAsync(b => b.Slug == "voltmax");
        stormJakke = await db.Products.FirstAsync(p => p.Slug == "nordvik-storm-regnjakke");
        billigJakke = await db.Products.FirstAsync(p => p.Slug == "billigtex-regnjakke-junior");
        borehammer = await db.Products.FirstAsync(p => p.Slug == "staalform-akku-borehammer-sh18");
        powermax = await db.Products.FirstAsync(p => p.Slug == "voltmax-powermax-18v");
        klodser = await db.Products.FirstAsync(p => p.Slug == "birkholm-traeklodser-100");
        plastbyg = await db.Products.FirstAsync(p => p.Slug == "plastobyg-byggeklodser-250");
        pande = await db.Products.FirstAsync(p => p.Slug == "kobber-co-stegepande-28");
        nonstick = await db.Products.FirstAsync(p => p.Slug == "nonstick-pro-stegepande-28");
        stovler = await db.Products.FirstAsync(p => p.Slug == "terra-outdoor-vandrestoevler");
        oplader = await db.Products.FirstAsync(p => p.Slug == "voltmax-usb-c-oplader-65w");
        uldsokker = await db.Products.FirstAsync(p => p.Slug == "nordvik-uldsokker-3-pak");
        users = await db.Users.OrderBy(u => u.UserName).ToListAsync();

        var aspectLookup = await db.Aspects.ToDictionaryAsync(a => a.Text, a => a, StringComparer.OrdinalIgnoreCase);

        Aspect A(string text) => aspectLookup[text];

        // ----- Reviews ------------------------------------------------------
        Review Review(Product product, int userIndex, Verdict verdict, string title, string body, string duration, params (string Aspect, string? Note)[] reviewAspects)
        {
            var r = new Review
            {
                Product = product,
                User = users[userIndex],
                Verdict = verdict,
                Title = title,
                Body = body,
                OwnershipDuration = duration
            };
            foreach (var (a, note) in reviewAspects)
            {
                r.Aspects.Add(new ReviewAspect { Aspect = A(a), Note = note });
            }
            return r;
        }

        var reviews = new List<Review>
        {
            Review(billigJakke, 0, Verdict.Bad,
                "Gik op i sømmene efter to uger",
                "Lynlåsen gik i stykker i første uge, og efter to uger var sømmen under armen gået op. Man kan reparere den, men stoffet er så tyndt, at det næsten ikke kan syes. Plastikken i lynlåsen er af lav kvalitet.",
                "2 måneder", ("Går op i sømmene", "Sprang op under armen"), ("Lynlåsen går i stykker", null), ("Materiel føles tynd/billig", null)),
            Review(stormJakke, 1, Verdict.Good,
                "Holder tør i timevis",
                "Brugt i to vintre, tapede sømme er stadig intakte og lynlåsen kører let. Da hætten revnede, sendte Nordvik en ny uden beregning.",
                "3 år", ("Holder formen i årevis", null), ("Sømme og materiale er solidt", null)),
            Review(powermax, 2, Verdict.Bad,
                "Batteriet holder ikke, og der findes ingen reservedele",
                "Efter tre måneder kunne batteriet kun holde til 10 minutters boring. Der findes ikke reservedele at købe, så hele maskinen er skrald.",
                "4 måneder", ("Batteriet holder ikke", null), ("Elektronikken fejler", null)),
            Review(borehammer, 1, Verdict.Good,
                "Værkstedskvalitet til hjemmebrugeren",
                "To år med hård brug og begge batterier er stadig som nye. Reservedele kan bestilles i 8 år.",
                "2 år", ("Professionel kvalitet", null), ("Reservedele let tilgængelige", null)),
            Review(plastbyg, 0, Verdict.Bad,
                "Skarpe kanter og manglende dele",
                "Kassen lovede 250 dele, men der var kun 231. Flere klodser er knækket ved første leg, og kanterne er skarpe.",
                "1 måned", ("Knækker ved første leg", null), ("Mangler dele", null), ("Billigt plastik", null)),
            Review(klodser, 3, Verdict.Good,
                "Går i arv",
                "De samme klodser som mine forældre havde. Massivt træ, ingen skarpe kanter, og de passer perfekt sammen.",
                "1 år", ("Holder til generationer", null), ("Solidt træ/plastik", null)),
            Review(nonstick, 2, Verdict.Bad,
                "Belægningen slipper efter få måneder",
                "Efter fire måneder begyndte belægningen at slippe i bunden. Panden er nu ubrugelig.",
                "5 måneder", ("Belægningen slipper", null), ("Virker ikke som annonceret", null)),
            Review(pande, 3, Verdict.Good,
                "Holder varmen perfekt",
                "Brugt dagligt i et år. Ingen belægning der slipper, fordi det er stål og kobber. Kan tåle opvaskemaskine.",
                "1 år", ("Tåler opvaskemaskine", null), ("Holder skarpt i årevis", null)),
            Review(oplader, 0, Verdict.Bad,
                "Bliver gloende varm og dør",
                "Opladeren bliver meget varm og holdt op med at virke efter to måneder.",
                "2 måneder", ("Overopheder", null)),
            Review(stovler, 1, Verdict.Good,
                "Kan fås med ny sål",
                "Tre års vandring, og jeg har lige fået skiftet sålen. Læderet er stadig fint.",
                "3 år", ("Holder i mange sæsoner", null), ("Kan repareres", null)),
            Review(uldsokker, 2, Verdict.Good,
                "Varme og kan stoppes",
                "Uldsokkerne holder formen og kan stoppes, når hælen bliver slidt. Anbefales.",
                "2 år", ("Holder formen i årevis", null))
        };

        foreach (var r in reviews) db.Reviews.Add(r);
        await db.SaveChangesAsync();

        var savedReviews = await db.Reviews.Include(r => r.Aspects).ToListAsync();
        var reviewByTitle = savedReviews.ToDictionary(r => r.Title);

        // ----- Community fixes ---------------------------------------------
        var fixes = new List<Fix>
        {
            new()
            {
                Product = billigJakke,
                Review = reviewByTitle["Gik op i sømmene efter to uger"],
                User = users[1],
                Source = FixSource.User,
                Title = "Sy sømmen med en kraftigere tråd",
                Body = "Vend jakken og sy den revnede søm med en kraftig nylontråd og dobbelt sting. Det forlænger levetiden betydeligt."
            },
            new()
            {
                Product = nonstick,
                Review = reviewByTitle["Belægningen slipper efter få måneder"],
                User = users[3],
                Source = FixSource.User,
                Title = "Brug panden til lav varme",
                Body = "Slip-let belægning holder meget længere, hvis man aldrig varmer panden op uden indhold og kun bruger plastikredskaber."
            },
            new()
            {
                Product = powermax,
                Review = reviewByTitle["Batteriet holder ikke, og der findes ingen reservedele"],
                User = users[0],
                Source = FixSource.User,
                Title = "Køb et uoriginalt adapterbatteri",
                Body = "Der findes adaptere fra andre 18V-systemer. Det er en nødløsning, men holder maskinen kørende lidt endnu.",
                SourceUrl = "https://www.reddit.com/r/Tools/search/?q=18v%20battery%20adapter"
            }
        };

        var systemFixes = new List<Fix>
        {
            new()
            {
                Product = billigJakke,
                Review = reviewByTitle["Gik op i sømmene efter to uger"],
                Source = FixSource.System,
                SourceKind = FixSourceKind.YouTube,
                Title = "Sådan reparerer du en revne i tøj (YouTube)",
                Body = "Automatisk forslag fundet ud fra dine ord om \"går op i sømmene\".",
                SourceUrl = "https://www.youtube.com/results?search_query=reparer+revne+i+t%C3%B8j"
            },
            new()
            {
                Product = nonstick,
                Review = reviewByTitle["Belægningen slipper efter få måneder"],
                Source = FixSource.System,
                SourceKind = FixSourceKind.YouTube,
                Title = "Kan en teflonpande reddes? (YouTube)",
                Body = "Automatisk forslag fundet ud fra dine ord om \"belægningen slipper\".",
                SourceUrl = "https://www.youtube.com/results?search_query=reparer+teflonpande+bel%C3%A6gning"
            },
            new()
            {
                Product = powermax,
                Review = reviewByTitle["Batteriet holder ikke, og der findes ingen reservedele"],
                Source = FixSource.System,
                SourceKind = FixSourceKind.Reddit,
                Title = "Reddit: Erfaringer med at skifte celler i værktøjsbatterier",
                Body = "Automatisk forslag fundet ud fra dine ord om \"batteriet holder ikke\".",
                SourceUrl = "https://www.reddit.com/search/?q=replace%20tool%20battery%20cells"
            }
        };

        db.Fixes.AddRange(fixes);
        db.Fixes.AddRange(systemFixes);
        await db.SaveChangesAsync();

        // ----- Curated lists -------------------------------------------------
        var listRegntoej = new ProductList
        {
            Name = "Regntøj til børn der holder",
            Slug = "regntoej-til-boern",
            Description = "Peer-reviewed regntøj, der stadig er tæt efter flere sæsoner.",
            Category = Find("regntoej"),
            IsEditorial = true
        };
        listRegntoej.Items.Add(new ProductListItem { Product = stormJakke, SortOrder = 0, Note = "Trelags med tapede sømme og reparerbar lynlås." });

        var listVaerksted = new ProductList
        {
            Name = "Værktøj til hobbyværkstedet",
            Slug = "vaerktoej-hobbyvaerkstedet",
            Description = "Det grundlæggende værktøj du kun køber én gang.",
            Category = Find("elvaerktoej"),
            IsEditorial = true
        };
        listVaerksted.Items.Add(new ProductListItem { Product = borehammer, SortOrder = 0, Note = "Akku-borehammer med 8 års reservedelsgaranti." });

        var listKoekken = new ProductList
        {
            Name = "Must-have i køkkenet",
            Slug = "must-have-koekkenet",
            Description = "Redskaber der holder i årevis og kan repareres eller vedligeholdes.",
            Category = Find("koekken-madlavning"),
            IsEditorial = true
        };
        listKoekken.Items.Add(new ProductListItem { Product = pande, SortOrder = 0, Note = "Stål og kobber – ingen belægning der slipper." });
        listKoekken.Items.Add(new ProductListItem { Product = termokande, SortOrder = 1, Note = "Holder kaffen varm i 12 timer." });
        listKoekken.Items.Add(new ProductListItem { Product = uldsokker, SortOrder = 2, Note = "Kan stoppes og holder formen." });

        var listLegetoej = new ProductList
        {
            Name = "Legetøj af høj kvalitet",
            Slug = "legetoej-af-hoej-kvalitet",
            Description = "Legetøj der kan tåle leg og gå i arv.",
            Category = Find("legetoej-spil-hobby"),
            IsEditorial = true
        };
        listLegetoej.Items.Add(new ProductListItem { Product = klodser, SortOrder = 0, Note = "Massivt bøgetræ, går i arv." });

        db.ProductLists.AddRange(listRegntoej, listVaerksted, listKoekken, listLegetoej);

        // ----- "Du skulle have købt dette i stedet" --------------------------
        db.ProductRecommendations.AddRange(
            new ProductRecommendation
            {
                SourceProduct = billigJakke,
                AlternativeProduct = stormJakke,
                Reason = "BilligTex-jakken går op i sømmene efter få uger. Storm-jakken er tapet og kan repareres."
            },
            new ProductRecommendation
            {
                SourceProduct = powermax,
                AlternativeProduct = borehammer,
                Reason = "PowerMax har ingen reservedele og et batteri der dør hurtigt. SH18 har 8 års reservedelsgaranti."
            },
            new ProductRecommendation
            {
                SourceProduct = plastbyg,
                AlternativeProduct = klodser,
                Reason = "Plastklodserne mangler dele og knækker. Træklodserne holder i generationer."
            },
            new ProductRecommendation
            {
                SourceProduct = nonstick,
                AlternativeProduct = pande,
                Reason = "Belægningen slipper efter få måneder. Kobberpanden har ingen belægning der kan slippe."
            },
            new ProductRecommendation
            {
                SourceProduct = oplader,
                AlternativeProduct = termokande,
                Reason = "Eksempel på parring på tværs af kategorier – redigeres i admin."
            });

        // ----- Fix suggestion keyword rules ----------------------------------
        db.FixSuggestionRules.AddRange(
            new FixSuggestionRule
            {
                Keywords = "går op i sømmene,revne,sprækket,sømmen er gået op,tynd i stoffet",
                Title = "Sådan reparerer du en revne i tøj",
                Url = "https://www.youtube.com/results?search_query=reparer+revne+i+t%C3%B8j",
                Kind = FixSourceKind.YouTube
            },
            new FixSuggestionRule
            {
                Keywords = "lynlås,lynlåsen går i stykker",
                Title = "Skift eller reparer en lynlås",
                Url = "https://www.youtube.com/results?search_query=reparer+lynl%C3%A5s",
                Kind = FixSourceKind.YouTube
            },
            new FixSuggestionRule
            {
                Keywords = "ruster,rust",
                Title = "Fjern rust og beskyt overfladen",
                Url = "https://www.reddit.com/search/?q=remove%20rust%20metal",
                Kind = FixSourceKind.Reddit
            },
            new FixSuggestionRule
            {
                Keywords = "belægningen slipper,nonstick,teflon,slip-let",
                Title = "Kan en teflonpande reddes?",
                Url = "https://www.youtube.com/results?search_query=reparer+teflonpande",
                Kind = FixSourceKind.YouTube
            },
            new FixSuggestionRule
            {
                Keywords = "batteriet holder ikke,batteri,akku",
                Title = "Skift eller genopbyg batteriet",
                Url = "https://www.reddit.com/search/?q=replace%20tool%20battery",
                Kind = FixSourceKind.Reddit
            },
            new FixSuggestionRule
            {
                Keywords = "plastik af lav kvalitet,knækket plastik,knækker",
                Title = "Reparation af knækket plastik",
                Url = "https://www.youtube.com/results?search_query=repair+broken+plastic",
                Kind = FixSourceKind.YouTube
            },
            new FixSuggestionRule
            {
                Keywords = "utæt,lækker,vand trænger ind",
                Title = "Find og tæt lækagen",
                Url = "https://www.youtube.com/results?search_query=fix+leak+seal",
                Kind = FixSourceKind.YouTube
            },
            new FixSuggestionRule
            {
                Keywords = "dårlig smag,smager dårligt",
                Title = "Sådan forbedrer du smagen",
                Url = "https://www.reddit.com/search/?q=improve%20taste%20food",
                Kind = FixSourceKind.Reddit
            });

        await db.SaveChangesAsync();
    }
}
