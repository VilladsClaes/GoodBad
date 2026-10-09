namespace GoodBad.Web.Data;

/// <summary>
/// A category definition used to build the seeded taxonomy tree.
/// </summary>
public record CategoryDef(
    string Name,
    string Icon,
    string Description,
    string[] Children,
    string[] Problems,
    string[] Praises);

/// <summary>
/// The GoodBad product taxonomy. Broad shopper-facing top-level categories with
/// deeper sub-categories, plus the defects and strengths that make sense in each
/// category (mirrors the research behind the platform: function is the primary
/// category, attributes are facets).
/// </summary>
public static class TaxonomySeed
{
    public static readonly CategoryDef[] Categories = new[]
    {
        new CategoryDef("Elektronik & computere", "\U0001F4BB",
            "Alt med strøm i: computere, telefoner, lyd, billede og tilbehør.",
            new[] { "Computere & tablets", "Skærme & projektorer", "Mobiltelefoner & tilbehør", "Lyd & høretelefoner", "TV & hjemmebiograf", "Foto & video", "Netværk & routere", "Kabler, opladere & batterier", "Smart home & sikkerhed", "Wearables & smartwatches", "Spilkonsoller & gaming-tilbehør", "Printere & scannere" },
            new[] { "Batteriet holder ikke", "Opladeren holder op med at virke", "Overopheder", "Softwaren bliver ikke opdateret", "Knapper/skærme fejler", "Ruster i stikket", "Plastik af lav kvalitet", "Støjer unødigt" },
            new[] { "Holder i årevis", "Nem at reparere", "Får løbende opdateringer", "Solidt materiale", "God pris/kvalitet", "Lang garanti" }),

        new CategoryDef("Hvidevarer", "\U0001F9FA",
            "Store og små maskiner til hjemmet: vask, madlavning og rengøring.",
            new[] { "Vaskemaskiner & tørretumblere", "Opvaskemaskiner", "Køleskabe & frysere", "Komfurer & ovne", "Emhætter", "Støvsugere & gulvpleje", "Kaffemaskiner & kedler", "Blendere & køkkenmaskiner", "Airfryers & mikrobølgeovne", "Luftrensere & ventilatorer" },
            new[] { "Utæt ved pakningerne", "Støjer meget", "Ruster", "Knapper/håndtag går i stykker", "Fejl efter kort tid", "Dyr at reparere", "Dårlig reservedelsadgang", "Elektronikken fejler" },
            new[] { "Holder i over 10 år", "Nem at få reservedele til", "Energibesparende", "Støjsvag", "Solidt materiale", "God kundeservice" }),

        new CategoryDef("Køkken & madlavning", "\U0001F373",
            "Redskaber og service til at lave og spise mad.",
            new[] { "Gryder & pander", "Knive & skærebrætter", "Køkkenredskaber & utensilier", "Service & glas", "Opbevaring & madkasser", "Bageudstyr", "Termo & drikkeflasker", "Vandfiltre & tilbehør" },
            new[] { "Belægningen slipper", "Ruster", "Skæret bliver sløvt med det samme", "Håndtaget knækker", "Tåler ikke opvaskemaskine", "Plastik af lav kvalitet", "Utæt", "Lugter kemisk" },
            new[] { "Holder skarpt i årevis", "Tåler opvaskemaskine", "Solidt materiale", "Nem at rengøre", "God vægt/balance" }),

        new CategoryDef("Møbler & boligindretning", "\U0001F6CB\uFE0F",
            "Møbler, tekstiler og indretning til hjemmet.",
            new[] { "Sofaer & stole", "Borde & skabe", "Senge & madrasser", "Opbevaring & reoler", "Belysning", "Tæpper & gardiner", "Spejle & dekoration", "Sengetøj & håndklæder", "Badeværelsesmøbler & udstyr" },
            new[] { "Går op i sømmene", "Falder fra hinanden", "Vakler/ustabil", "Falmer", "Lugter kemisk", "Krymper i vask", "Overfladen skaller af", "Spidse kanter" },
            new[] { "Holder i årevis", "Solidt træ/metal", "Nem at samle", "Tåler daglig brug", "Klassisk design der holder" }),

        new CategoryDef("Værktøj & gør-det-selv", "\U0001F527",
            "Håndværktøj, maskinværktøj og materialer til værkstedet.",
            new[] { "Elværktøj", "Håndværktøj", "Værktøjssæt & tasker", "Måleværktøj & laser", "Skruer, bolte & beslag", "Låse & sikkerhed", "Maling & malerudstyr", "VVS & el-tilbehør", "Værkstedsmaskiner & bukke" },
            new[] { "Motor brænder sammen", "Batteri/akku holder ikke", "Knækker ved belastning", "Ruster hurtigt", "Borepatronen slipper", "Plastik af lav kvalitet", "Unøjagtig måling", "Dårlig garanti" },
            new[] { "Professionel kvalitet", "Holder til daglig brug i årevis", "Nem at reparere", "Reservedele let tilgængelige", "Præcis og pålidelig" }),

        new CategoryDef("Have & udendørs", "\U0001F33F",
            "Haverage, udendørsmøbler og alt til haven.",
            new[] { "Havemaskiner & plæneklippere", "Haveredskaber", "Vanding & sprinklere", "Udendørsmøbler & grill", "Planter, frø & jord", "Drivhus & skure", "Hegn & terrasse", "Udendørs belysning" },
            new[] { "Ruster efter én sæson", "Motoren starter ikke", "Knækker ved frost", "Falmer i solen", "Lækker", "Plastik af lav kvalitet", "Utæt sprinkler" },
            new[] { "Tåler vinter og vejr", "Holder i mange sæsoner", "Solidt materiale", "Let at vedligeholde" }),

        new CategoryDef("Tøj, sko & accessories", "\U0001F455",
            "Beklædning, fodtøj, tasker og personlige accessories.",
            new[] { "Overdele & bluser", "Bukser & shorts", "Kjoler & nederdele", "Jakker & overtøj", "Regntøj", "Undertøj & sokker", "Sko & støvler", "Tasker & rygsække", "Bælter, hatte & handsker", "Ure & smykker", "Arbejdstøj & beskyttelse" },
            new[] { "Går op i sømmene", "Farven falmer", "Krymper i vask", "Dårlig pasform", "Lynlåsen går i stykker", "Huller efter kort tid", "Materiel føles tynd/billig", "Sålen går løs" },
            new[] { "Holder formen i årevis", "Sømme og materiale er solidt", "Falmer ikke", "Tåler mange vask", "Kan repareres" }),

        new CategoryDef("Skønhed & personlig pleje", "\U0001F485",
            "Hudpleje, kosmetik, hårpleje og hygiejneprodukter.",
            new[] { "Hudpleje", "Kosmetik & makeup", "Hårpleje", "Hårstyling-værktøj", "Barbering & trimning", "Mundpleje", "Parfume & duft", "Neglepleje", "Hygiejneprodukter" },
            new[] { "Irriterer huden", "Holder ikke hvad den lover", "Holder op med at virke", "Dårlig emballage der lækker", "Kemisk lugt", "Kort holdbarhed efter åbning" },
            new[] { "Skånsom mod huden", "Gør hvad den lover", "Holder længe", "God pris/kvalitet", "Dermatologisk testet" }),

        new CategoryDef("Sundhed & velvære", "\U0001FA7A",
            "Produkter til sundhed, træning og daglig velvære i hjemmet.",
            new[] { "Termometre & helsemålere", "Førstehjælp", "Mobilitetshjælpemidler", "Massage & varme", "Kosttilskud & vitaminer", "Hjemmetest & medicinudstyr" },
            new[] { "Upræcis måling", "Går hurtigt i stykker", "Dårlig hygiejne/pleje", "Virker ikke som beskrevet", "Svær at bruge" },
            new[] { "Pålidelig og præcis", "Nem at bruge", "Holder i årevis", "Solidt materiale" }),

        new CategoryDef("Baby & børn", "\U0001F37C",
            "Alt til de mindste: barnevogne, autostole, bleer og babypasning.",
            new[] { "Barnevogne & klapvogne", "Autostole & bæreseler", "Bleprodukter & pusleudstyr", "Flasker & barnemad-udstyr", "Børneværelsesmøbler", "Babymonitorer", "Børnetøj & sko", "Sikkerhedsudstyr" },
            new[] { "Mekanisme går i stykker", "Går op i sømmene", "Skadelige stoffer", "Ustabil/sikkerhedsrisiko", "Dårlig pasform til barnet", "Plastik af lav kvalitet" },
            new[] { "Sikker og solid", "Kan bruges til flere børn", "Nem at rengøre", "Gode sikkerhedstest", "Holder i årevis" }),

        new CategoryDef("Legetøj, spil & hobby", "\U0001F9F8",
            "Legetøj, brætspil og hobbyudstyr i god kvalitet.",
            new[] { "Byggeklodser & konstruktion", "Dukker & figurer", "Puslespil & brætspil", "Kreative sæt & hobby", "Udelegetøj", "Modelbyggeri & samlerobjekter", "Spil til voksne" },
            new[] { "Knækker ved første leg", "Mangler dele", "Giftige/skadelige materialer", "Delene passer ikke sammen", "Billigt plastik", "Dårlige/misvisende regler" },
            new[] { "Holder til generationer", "Solidt træ/plastik", "Tåler hård leg", "God genbrugsværdi", "Nem at reparere" }),

        new CategoryDef("Sport, fitness & outdoor", "\U0001F3C3",
            "Træningsudstyr, sport og udstyr til friluftsliv.",
            new[] { "Fitnessudstyr & vægte", "Cykler & cykeltilbehør", "Camping & vandring", "Fiskeri & jagt", "Vandsport", "Vintersport", "Boldspil & racketsport", "Beskyttelsesudstyr" },
            new[] { "Går op i sømmene", "Knækker ved belastning", "Ruster", "Punkterer let", "Dårlig pasform", "Mekanisme fejler" },
            new[] { "Holder i mange sæsoner", "Solidt materiale", "Kan repareres", "Professionel kvalitet" }),

        new CategoryDef("Bil, motorcykel & mobility", "\U0001F697",
            "Tilbehør, reservedele og udstyr til køretøjer.",
            new[] { "Bildæk & hjul", "Batterier & ladeudstyr", "Bilpleje & tilbehør", "Værktøj & reservedele", "Motorcykel & scooter", "Elcykel & løbehjul", "Autostole & interiør", "Elbil-ladere" },
            new[] { "Passer ikke som angivet", "Ruster hurtigt", "Holder ikke spændingen", "Lækker", "Dårlig kvalitet i materialet", "Fejler efter kort tid" },
            new[] { "Holder mange år", "Præcis pasform", "Solidt materiale", "God godkendelse/certificering" }),

        new CategoryDef("Bøger, musik & medier", "\U0001F3B5",
            "Fysiske bøger, instrumenter og medier.",
            new[] { "Bøger & e-bøger", "Musikinstrumenter", "Lydudstyr & tilbehør", "Plader, CD & DVD", "Noter & undervisningsmateriale" },
            new[] { "Limningen slipper", "Tryk af dårlig kvalitet", "Knækker/fejler", "Stemmer ikke", "Transportskader" },
            new[] { "Holder i årevis", "God trykkvalitet", "Solidt bundet", "Professionel lyd" }),

        new CategoryDef("Kontor, skole & papirvarer", "\u270F\uFE0F",
            "Kontorartikler, skoleudstyr og papirvarer.",
            new[] { "Papir & notesbøger", "Skriveredskaber", "Kontormaskiner", "Opbevaring & arkivering", "Skoleudstyr & tasker", "Kreativt materiale" },
            new[] { "Bleeder igennem", "Går i stykker", "Tørrer ud", "Dårlig byggekvalitet", "Sider falder ud" },
            new[] { "Holder længe", "God papirkvalitet", "Solidt materiale", "God pris/kvalitet" }),

        new CategoryDef("Mad, drikkevarer & husholdning", "\U0001F6D2",
            "Fødevarer, drikkevarer og forbrugsvarer til hjemmet.",
            new[] { "Kolonial & snacks", "Kaffe & te", "Drikkevarer & vin", "Frost & friskvarer", "Rengøring & vask", "Papir & husholdning", "Dyrefoder & -pleje-varer" },
            new[] { "Dårlig smag", "Kort holdbarhed", "Mange tilsætningsstoffer", "Emballagen er svær at åbne", "Lugter/løber ud", "Holder ikke tør" },
            new[] { "Fremragende smag", "Rene råvarer", "God holdbarhed", "Miljøvenlig emballage" }),

        new CategoryDef("Kæledyr", "\U0001F43E",
            "Udstyr, foder og pleje til kæledyr.",
            new[] { "Foder & godbidder", "Senge & transport", "Legetøj", "Halsbånd & seler", "Pleje & hygiejne", "Bur & akvarier", "Træning & tilbehør" },
            new[] { "Går i stykker", "Kemisk lugt", "Irriterer dyret", "Dårlig pasform", "Kort holdbarhed på foder" },
            new[] { "Dyrlægeanbefalet", "Solidt materiale", "Holder længe", "Skånsom mod dyret" }),

        new CategoryDef("Kunst, håndværk & gaver", "\U0001F3A8",
            "Kreative materialer, kunst og gaveartikler.",
            new[] { "Hobby & kreative materialer", "Malergrej & lærreder", "Gavepapir & kort", "Fest & dekoration", "Samlerobjekter & kunst" },
            new[] { "Falmer", "Knækker/skaller", "Tørrer ud", "Dårlig trykkvalitet", "Billigt plastik" },
            new[] { "Høj kvalitet", "Holder i årevis", "Ægte materialer", "Smukt håndværk" })
    };

    /// <summary>Cross-cutting aspects that make sense in every category.</summary>
    public static readonly string[] CommonProblems =
    {
        "Virker ikke som annonceret",
        "Går hurtigt i stykker",
        "Svær eller umulig at reparere",
        "Dårlig kundeservice fra producenten",
        "Misvisende markedsføring",
        "Kort levetid",
        "Dårlig emballage"
    };

    public static readonly string[] CommonPraises =
    {
        "Holder i mange år",
        "Solidt materiale",
        "Nem at reparere",
        "God pris/kvalitet",
        "Gør præcis hvad den lover",
        "God kundeservice",
        "Lang producentgaranti",
        "Bæredygtig produktion",
        "Nem at vedligeholde",
        "Reservedele let tilgængelige"
    };
}
