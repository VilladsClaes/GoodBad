-- =====================================================================
--  GoodBad - komplet MySQL-database til Simply.com (MySQL 8 / MariaDB 10.11+)
-- =====================================================================
--  Hvad er dette?
--    Et færdigt SQL-script med hele skemaet (23 tabeller) OG det seed-indhold
--    appen selv opretter (kategorier, produkter, anmeldelser, fix-regler ...).
--
--  Sådan bruger du det
--    1) Simply-kontrolpanelet -> MySQL -> opret databasen (fx goodbad).
--    2) phpMyAdmin -> vælg databasen -> Importér -> upload denne fil.
--       (eller i en klient:  mysql -u BRUGER -p DATABASE < goodbad-mysql.sql)
--    3) Sæt connection string i appsettings.Production.json (se README.md).
--
--  Du behøver IKKE importere filen for at få gang i appen:
--    ASP.NET Core-appen kører selv EF Core-migrationerne og seeder indholdet
--    første gang den starter. Filen er til dig der vil kunne se/redigere
--    skemaet og data i Visual Studio Server Explorer, MySQL Workbench eller
--    phpMyAdmin på forhånd - eller tage en backup.
--
--  Bemærk: scriptet dropper og genskaber tabellerne, så kør det på en tom
--  database (eller tag backup først).
-- =====================================================================

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
DROP TABLE IF EXISTS `AspNetRoleClaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AspNetRoleClaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `AspNetRoleClaims` WRITE;
/*!40000 ALTER TABLE `AspNetRoleClaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `AspNetRoleClaims` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `AspNetRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AspNetRoles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `AspNetRoles` WRITE;
/*!40000 ALTER TABLE `AspNetRoles` DISABLE KEYS */;
INSERT INTO `AspNetRoles` VALUES
('f9e289af-99d2-439d-947f-759daf04bb1d','Admin','ADMIN',NULL);
/*!40000 ALTER TABLE `AspNetRoles` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `AspNetUserClaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AspNetUserClaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `AspNetUserClaims` WRITE;
/*!40000 ALTER TABLE `AspNetUserClaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `AspNetUserClaims` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `AspNetUserLogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AspNetUserLogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext DEFAULT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `AspNetUserLogins` WRITE;
/*!40000 ALTER TABLE `AspNetUserLogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `AspNetUserLogins` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `AspNetUserRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AspNetUserRoles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `AspNetUserRoles` WRITE;
/*!40000 ALTER TABLE `AspNetUserRoles` DISABLE KEYS */;
/*!40000 ALTER TABLE `AspNetUserRoles` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `AspNetUserTokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AspNetUserTokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext DEFAULT NULL,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `AspNetUserTokens` WRITE;
/*!40000 ALTER TABLE `AspNetUserTokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `AspNetUserTokens` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `AspNetUsers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AspNetUsers` (
  `Id` varchar(255) NOT NULL,
  `DisplayName` varchar(80) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext DEFAULT NULL,
  `SecurityStamp` longtext DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL,
  `PhoneNumber` longtext DEFAULT NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `AspNetUsers` WRITE;
/*!40000 ALTER TABLE `AspNetUsers` DISABLE KEYS */;
INSERT INTO `AspNetUsers` VALUES
('3624421f-13fd-4485-8e2f-2442f5b7397a','Jonas K.','2026-10-09 15:44:13.100449','demo2@goodbad.dk',NULL,'demo2@goodbad.dk',NULL,1,NULL,'57ab2a4b-5d64-45e2-975f-7af35d7c1154','17be3340-631c-4228-8cc6-9ccc3d743d40',NULL,0,0,NULL,0,0),
('770da7dc-764a-4903-979e-b2e06913b970','Mette H.','2026-10-09 15:44:13.039380','demo1@goodbad.dk',NULL,'demo1@goodbad.dk',NULL,1,NULL,'bfaebd5d-89ea-45b1-8a2e-b7ca5e183870','741d3bed-d2ea-4189-94e1-d30378ac4120',NULL,0,0,NULL,0,0),
('bad7b07d-85cc-4343-8223-bf80da21be3d','Anders B.','2026-10-09 15:44:13.100599','demo4@goodbad.dk',NULL,'demo4@goodbad.dk',NULL,1,NULL,'d96597df-c719-4dc6-8514-779e35185dd0','181ffe2f-6395-49db-b77c-30822554abe3',NULL,0,0,NULL,0,0),
('da9f15bb-5bab-4b82-ab49-8c0c9f128dfb','Sofie L.','2026-10-09 15:44:13.100559','demo3@goodbad.dk',NULL,'demo3@goodbad.dk',NULL,1,NULL,'339d8bac-bb3e-4eb9-b004-1862ac6e5f89','cc4c70f9-4f75-491f-afab-e99080578bab',NULL,0,0,NULL,0,0);
/*!40000 ALTER TABLE `AspNetUsers` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `Aspects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Aspects` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Text` varchar(160) NOT NULL,
  `Slug` varchar(180) NOT NULL,
  `Kind` int(11) NOT NULL,
  `Description` varchar(600) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Aspects_Slug` (`Slug`)
) ENGINE=InnoDB AUTO_INCREMENT=167 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `Aspects` WRITE;
/*!40000 ALTER TABLE `Aspects` DISABLE KEYS */;
INSERT INTO `Aspects` VALUES
(1,'Virker ikke som annonceret','virker-ikke-som-annonceret',0,NULL),
(2,'Går hurtigt i stykker','gaar-hurtigt-i-stykker',0,NULL),
(3,'Svær eller umulig at reparere','svaer-eller-umulig-at-reparere',0,NULL),
(4,'Dårlig kundeservice fra producenten','daarlig-kundeservice-fra-producenten',0,NULL),
(5,'Misvisende markedsføring','misvisende-markedsfoering',0,NULL),
(6,'Kort levetid','kort-levetid',0,NULL),
(7,'Dårlig emballage','daarlig-emballage',0,NULL),
(8,'Holder i mange år','holder-i-mange-aar',1,NULL),
(9,'Solidt materiale','solidt-materiale',1,NULL),
(10,'Nem at reparere','nem-at-reparere',1,NULL),
(11,'God pris/kvalitet','god-pris-kvalitet',1,NULL),
(12,'Gør præcis hvad den lover','goer-praecis-hvad-den-lover',1,NULL),
(13,'God kundeservice','god-kundeservice',1,NULL),
(14,'Lang producentgaranti','lang-producentgaranti',1,NULL),
(15,'Bæredygtig produktion','baeredygtig-produktion',1,NULL),
(16,'Nem at vedligeholde','nem-at-vedligeholde',1,NULL),
(17,'Reservedele let tilgængelige','reservedele-let-tilgaengelige',1,NULL),
(18,'Batteriet holder ikke','batteriet-holder-ikke',0,NULL),
(19,'Opladeren holder op med at virke','opladeren-holder-op-med-at-virke',0,NULL),
(20,'Overopheder','overopheder',0,NULL),
(21,'Softwaren bliver ikke opdateret','softwaren-bliver-ikke-opdateret',0,NULL),
(22,'Knapper/skærme fejler','knapper-skaerme-fejler',0,NULL),
(23,'Ruster i stikket','ruster-i-stikket',0,NULL),
(24,'Plastik af lav kvalitet','plastik-af-lav-kvalitet',0,NULL),
(25,'Støjer unødigt','stoejer-unoedigt',0,NULL),
(26,'Holder i årevis','holder-i-aarevis',1,NULL),
(27,'Får løbende opdateringer','faar-loebende-opdateringer',1,NULL),
(28,'Lang garanti','lang-garanti',1,NULL),
(29,'Utæt ved pakningerne','utaet-ved-pakningerne',0,NULL),
(30,'Støjer meget','stoejer-meget',0,NULL),
(31,'Ruster','ruster',0,NULL),
(32,'Knapper/håndtag går i stykker','knapper-haandtag-gaar-i-stykker',0,NULL),
(33,'Fejl efter kort tid','fejl-efter-kort-tid',0,NULL),
(34,'Dyr at reparere','dyr-at-reparere',0,NULL),
(35,'Dårlig reservedelsadgang','daarlig-reservedelsadgang',0,NULL),
(36,'Elektronikken fejler','elektronikken-fejler',0,NULL),
(37,'Holder i over 10 år','holder-i-over-10-aar',1,NULL),
(38,'Nem at få reservedele til','nem-at-faa-reservedele-til',1,NULL),
(39,'Energibesparende','energibesparende',1,NULL),
(40,'Støjsvag','stoejsvag',1,NULL),
(41,'Belægningen slipper','belaegningen-slipper',0,NULL),
(42,'Skæret bliver sløvt med det samme','skaeret-bliver-sloevt-med-det-samme',0,NULL),
(43,'Håndtaget knækker','haandtaget-knaekker',0,NULL),
(44,'Tåler ikke opvaskemaskine','taaler-ikke-opvaskemaskine',0,NULL),
(45,'Utæt','utaet',0,NULL),
(46,'Lugter kemisk','lugter-kemisk',0,NULL),
(47,'Holder skarpt i årevis','holder-skarpt-i-aarevis',1,NULL),
(48,'Tåler opvaskemaskine','taaler-opvaskemaskine',1,NULL),
(49,'Nem at rengøre','nem-at-rengoere',1,NULL),
(50,'God vægt/balance','god-vaegt-balance',1,NULL),
(51,'Går op i sømmene','gaar-op-i-soemmene',0,NULL),
(52,'Falder fra hinanden','falder-fra-hinanden',0,NULL),
(53,'Vakler/ustabil','vakler-ustabil',0,NULL),
(54,'Falmer','falmer',0,NULL),
(55,'Krymper i vask','krymper-i-vask',0,NULL),
(56,'Overfladen skaller af','overfladen-skaller-af',0,NULL),
(57,'Spidse kanter','spidse-kanter',0,NULL),
(58,'Solidt træ/metal','solidt-trae-metal',1,NULL),
(59,'Nem at samle','nem-at-samle',1,NULL),
(60,'Tåler daglig brug','taaler-daglig-brug',1,NULL),
(61,'Klassisk design der holder','klassisk-design-der-holder',1,NULL),
(62,'Motor brænder sammen','motor-braender-sammen',0,NULL),
(63,'Batteri/akku holder ikke','batteri-akku-holder-ikke',0,NULL),
(64,'Knækker ved belastning','knaekker-ved-belastning',0,NULL),
(65,'Ruster hurtigt','ruster-hurtigt',0,NULL),
(66,'Borepatronen slipper','borepatronen-slipper',0,NULL),
(67,'Unøjagtig måling','unoejagtig-maaling',0,NULL),
(68,'Dårlig garanti','daarlig-garanti',0,NULL),
(69,'Professionel kvalitet','professionel-kvalitet',1,NULL),
(70,'Holder til daglig brug i årevis','holder-til-daglig-brug-i-aarevis',1,NULL),
(71,'Præcis og pålidelig','praecis-og-paalidelig',1,NULL),
(72,'Ruster efter én sæson','ruster-efter-en-saeson',0,NULL),
(73,'Motoren starter ikke','motoren-starter-ikke',0,NULL),
(74,'Knækker ved frost','knaekker-ved-frost',0,NULL),
(75,'Falmer i solen','falmer-i-solen',0,NULL),
(76,'Lækker','laekker',0,NULL),
(77,'Utæt sprinkler','utaet-sprinkler',0,NULL),
(78,'Tåler vinter og vejr','taaler-vinter-og-vejr',1,NULL),
(79,'Holder i mange sæsoner','holder-i-mange-saesoner',1,NULL),
(80,'Let at vedligeholde','let-at-vedligeholde',1,NULL),
(81,'Farven falmer','farven-falmer',0,NULL),
(82,'Dårlig pasform','daarlig-pasform',0,NULL),
(83,'Lynlåsen går i stykker','lynlaasen-gaar-i-stykker',0,NULL),
(84,'Huller efter kort tid','huller-efter-kort-tid',0,NULL),
(85,'Materiel føles tynd/billig','materiel-foeles-tynd-billig',0,NULL),
(86,'Sålen går løs','saalen-gaar-loes',0,NULL),
(87,'Holder formen i årevis','holder-formen-i-aarevis',1,NULL),
(88,'Sømme og materiale er solidt','soemme-og-materiale-er-solidt',1,NULL),
(89,'Falmer ikke','falmer-ikke',1,NULL),
(90,'Tåler mange vask','taaler-mange-vask',1,NULL),
(91,'Kan repareres','kan-repareres',1,NULL),
(92,'Irriterer huden','irriterer-huden',0,NULL),
(93,'Holder ikke hvad den lover','holder-ikke-hvad-den-lover',0,NULL),
(94,'Holder op med at virke','holder-op-med-at-virke',0,NULL),
(95,'Dårlig emballage der lækker','daarlig-emballage-der-laekker',0,NULL),
(96,'Kemisk lugt','kemisk-lugt',0,NULL),
(97,'Kort holdbarhed efter åbning','kort-holdbarhed-efter-aabning',0,NULL),
(98,'Skånsom mod huden','skaansom-mod-huden',1,NULL),
(99,'Gør hvad den lover','goer-hvad-den-lover',1,NULL),
(100,'Holder længe','holder-laenge',1,NULL),
(101,'Dermatologisk testet','dermatologisk-testet',1,NULL),
(102,'Upræcis måling','upraecis-maaling',0,NULL),
(103,'Dårlig hygiejne/pleje','daarlig-hygiejne-pleje',0,NULL),
(104,'Virker ikke som beskrevet','virker-ikke-som-beskrevet',0,NULL),
(105,'Svær at bruge','svaer-at-bruge',0,NULL),
(106,'Pålidelig og præcis','paalidelig-og-praecis',1,NULL),
(107,'Nem at bruge','nem-at-bruge',1,NULL),
(108,'Mekanisme går i stykker','mekanisme-gaar-i-stykker',0,NULL),
(109,'Skadelige stoffer','skadelige-stoffer',0,NULL),
(110,'Ustabil/sikkerhedsrisiko','ustabil-sikkerhedsrisiko',0,NULL),
(111,'Dårlig pasform til barnet','daarlig-pasform-til-barnet',0,NULL),
(112,'Sikker og solid','sikker-og-solid',1,NULL),
(113,'Kan bruges til flere børn','kan-bruges-til-flere-boern',1,NULL),
(114,'Gode sikkerhedstest','gode-sikkerhedstest',1,NULL),
(115,'Knækker ved første leg','knaekker-ved-foerste-leg',0,NULL),
(116,'Mangler dele','mangler-dele',0,NULL),
(117,'Giftige/skadelige materialer','giftige-skadelige-materialer',0,NULL),
(118,'Delene passer ikke sammen','delene-passer-ikke-sammen',0,NULL),
(119,'Billigt plastik','billigt-plastik',0,NULL),
(120,'Dårlige/misvisende regler','daarlige-misvisende-regler',0,NULL),
(121,'Holder til generationer','holder-til-generationer',1,NULL),
(122,'Solidt træ/plastik','solidt-trae-plastik',1,NULL),
(123,'Tåler hård leg','taaler-haard-leg',1,NULL),
(124,'God genbrugsværdi','god-genbrugsvaerdi',1,NULL),
(125,'Punkterer let','punkterer-let',0,NULL),
(126,'Mekanisme fejler','mekanisme-fejler',0,NULL),
(127,'Passer ikke som angivet','passer-ikke-som-angivet',0,NULL),
(128,'Holder ikke spændingen','holder-ikke-spaendingen',0,NULL),
(129,'Dårlig kvalitet i materialet','daarlig-kvalitet-i-materialet',0,NULL),
(130,'Fejler efter kort tid','fejler-efter-kort-tid',0,NULL),
(131,'Holder mange år','holder-mange-aar',1,NULL),
(132,'Præcis pasform','praecis-pasform',1,NULL),
(133,'God godkendelse/certificering','god-godkendelse-certificering',1,NULL),
(134,'Limningen slipper','limningen-slipper',0,NULL),
(135,'Tryk af dårlig kvalitet','tryk-af-daarlig-kvalitet',0,NULL),
(136,'Knækker/fejler','knaekker-fejler',0,NULL),
(137,'Stemmer ikke','stemmer-ikke',0,NULL),
(138,'Transportskader','transportskader',0,NULL),
(139,'God trykkvalitet','god-trykkvalitet',1,NULL),
(140,'Solidt bundet','solidt-bundet',1,NULL),
(141,'Professionel lyd','professionel-lyd',1,NULL),
(142,'Bleeder igennem','bleeder-igennem',0,NULL),
(143,'Går i stykker','gaar-i-stykker',0,NULL),
(144,'Tørrer ud','toerrer-ud',0,NULL),
(145,'Dårlig byggekvalitet','daarlig-byggekvalitet',0,NULL),
(146,'Sider falder ud','sider-falder-ud',0,NULL),
(147,'God papirkvalitet','god-papirkvalitet',1,NULL),
(148,'Dårlig smag','daarlig-smag',0,NULL),
(149,'Kort holdbarhed','kort-holdbarhed',0,NULL),
(150,'Mange tilsætningsstoffer','mange-tilsaetningsstoffer',0,NULL),
(151,'Emballagen er svær at åbne','emballagen-er-svaer-at-aabne',0,NULL),
(152,'Lugter/løber ud','lugter-loeber-ud',0,NULL),
(153,'Holder ikke tør','holder-ikke-toer',0,NULL),
(154,'Fremragende smag','fremragende-smag',1,NULL),
(155,'Rene råvarer','rene-raavarer',1,NULL),
(156,'God holdbarhed','god-holdbarhed',1,NULL),
(157,'Miljøvenlig emballage','miljoevenlig-emballage',1,NULL),
(158,'Irriterer dyret','irriterer-dyret',0,NULL),
(159,'Kort holdbarhed på foder','kort-holdbarhed-paa-foder',0,NULL),
(160,'Dyrlægeanbefalet','dyrlaegeanbefalet',1,NULL),
(161,'Skånsom mod dyret','skaansom-mod-dyret',1,NULL),
(162,'Knækker/skaller','knaekker-skaller',0,NULL),
(163,'Dårlig trykkvalitet','daarlig-trykkvalitet',0,NULL),
(164,'Høj kvalitet','hoej-kvalitet',1,NULL),
(165,'Ægte materialer','aegte-materialer',1,NULL),
(166,'Smukt håndværk','smukt-haandvaerk',1,NULL);
/*!40000 ALTER TABLE `Aspects` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `Brands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Brands` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(120) NOT NULL,
  `Slug` varchar(140) NOT NULL,
  `LogoUrl` varchar(600) DEFAULT NULL,
  `WebsiteUrl` varchar(600) DEFAULT NULL,
  `Country` varchar(80) DEFAULT NULL,
  `Description` varchar(2000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Brands_Slug` (`Slug`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `Brands` WRITE;
/*!40000 ALTER TABLE `Brands` DISABLE KEYS */;
INSERT INTO `Brands` VALUES
(1,'Nordvik','nordvik','https://placehold.co/160x80?text=Nordvik','https://example.com/nordvik','Danmark','Dansk outdoor-brand kendt for gedigne regnjakker og uldprodukter med mange års garanti.'),
(2,'Stålform','staalform','https://placehold.co/160x80?text=St%C3%A5lform','https://example.com/staalform','Danmark','Værktøjsmærke der satser på professionel kvalitet og reservedele i mange år.'),
(3,'Birkholm','birkholm','https://placehold.co/160x80?text=Birkholm','https://example.com/birkholm','Danmark','Familieforetagende der laver klassiske træprodukter og legetøj af FSC-træ.'),
(4,'Kobber & Co','kobber-co','https://placehold.co/160x80?text=Kobber','https://example.com/kobber','Danmark','Køkkenudstyr i kobber og stål til langsigtet brug.'),
(5,'Terra Outdoor','terra-outdoor','https://placehold.co/160x80?text=Terra','https://example.com/terra','Sverige','Skandinavisk friluftsmærke med fokus på slidstyrke.'),
(6,'BilligTex','billigtex','https://placehold.co/160x80?text=BilligTex','https://example.com/billigtex','Kina','Lavprismærke. Kortere levetid og svær adgang til reservedele.'),
(7,'VoltMax','voltmax','https://placehold.co/160x80?text=VoltMax','https://example.com/voltmax','Kina','Lavpris elektronik uden service.'),
(8,'PlastoByg','plastobyg','https://placehold.co/160x80?text=PlastoByg','https://example.com/plastobyg','Kina','Plastlegetøj i meget lav prisklasse.');
/*!40000 ALTER TABLE `Brands` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `Categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Categories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(120) NOT NULL,
  `Slug` varchar(140) NOT NULL,
  `Description` varchar(600) DEFAULT NULL,
  `Icon` varchar(24) DEFAULT NULL,
  `ParentId` int(11) DEFAULT NULL,
  `SortOrder` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Categories_Slug` (`Slug`),
  KEY `IX_Categories_ParentId` (`ParentId`),
  CONSTRAINT `FK_Categories_Categories_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `Categories` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=162 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `Categories` WRITE;
/*!40000 ALTER TABLE `Categories` DISABLE KEYS */;
INSERT INTO `Categories` VALUES
(1,'Elektronik & computere','elektronik-computere','Alt med strøm i: computere, telefoner, lyd, billede og tilbehør.','💻',NULL,0),
(2,'Hvidevarer','hvidevarer','Store og små maskiner til hjemmet: vask, madlavning og rengøring.','🧺',NULL,1),
(3,'Køkken & madlavning','koekken-madlavning','Redskaber og service til at lave og spise mad.','🍳',NULL,2),
(4,'Møbler & boligindretning','moebler-boligindretning','Møbler, tekstiler og indretning til hjemmet.','🛋️',NULL,3),
(5,'Værktøj & gør-det-selv','vaerktoej-goer-det-selv','Håndværktøj, maskinværktøj og materialer til værkstedet.','🔧',NULL,4),
(6,'Have & udendørs','have-udendoers','Haverage, udendørsmøbler og alt til haven.','🌿',NULL,5),
(7,'Tøj, sko & accessories','toej-sko-accessories','Beklædning, fodtøj, tasker og personlige accessories.','👕',NULL,6),
(8,'Skønhed & personlig pleje','skoenhed-personlig-pleje','Hudpleje, kosmetik, hårpleje og hygiejneprodukter.','💅',NULL,7),
(9,'Sundhed & velvære','sundhed-velvaere','Produkter til sundhed, træning og daglig velvære i hjemmet.','🩺',NULL,8),
(10,'Baby & børn','baby-boern','Alt til de mindste: barnevogne, autostole, bleer og babypasning.','🍼',NULL,9),
(11,'Legetøj, spil & hobby','legetoej-spil-hobby','Legetøj, brætspil og hobbyudstyr i god kvalitet.','🧸',NULL,10),
(12,'Sport, fitness & outdoor','sport-fitness-outdoor','Træningsudstyr, sport og udstyr til friluftsliv.','🏃',NULL,11),
(13,'Bil, motorcykel & mobility','bil-motorcykel-mobility','Tilbehør, reservedele og udstyr til køretøjer.','🚗',NULL,12),
(14,'Bøger, musik & medier','boeger-musik-medier','Fysiske bøger, instrumenter og medier.','🎵',NULL,13),
(15,'Kontor, skole & papirvarer','kontor-skole-papirvarer','Kontorartikler, skoleudstyr og papirvarer.','✏️',NULL,14),
(16,'Mad, drikkevarer & husholdning','mad-drikkevarer-husholdning','Fødevarer, drikkevarer og forbrugsvarer til hjemmet.','🛒',NULL,15),
(17,'Kæledyr','kaeledyr','Udstyr, foder og pleje til kæledyr.','🐾',NULL,16),
(18,'Kunst, håndværk & gaver','kunst-haandvaerk-gaver','Kreative materialer, kunst og gaveartikler.','🎨',NULL,17),
(19,'Computere & tablets','computere-tablets',NULL,NULL,1,0),
(20,'Skærme & projektorer','skaerme-projektorer',NULL,NULL,1,0),
(21,'Mobiltelefoner & tilbehør','mobiltelefoner-tilbehoer',NULL,NULL,1,0),
(22,'Lyd & høretelefoner','lyd-hoeretelefoner',NULL,NULL,1,0),
(23,'TV & hjemmebiograf','tv-hjemmebiograf',NULL,NULL,1,0),
(24,'Foto & video','foto-video',NULL,NULL,1,0),
(25,'Netværk & routere','netvaerk-routere',NULL,NULL,1,0),
(26,'Kabler, opladere & batterier','kabler-opladere-batterier',NULL,NULL,1,0),
(27,'Smart home & sikkerhed','smart-home-sikkerhed',NULL,NULL,1,0),
(28,'Wearables & smartwatches','wearables-smartwatches',NULL,NULL,1,0),
(29,'Spilkonsoller & gaming-tilbehør','spilkonsoller-gaming-tilbehoer',NULL,NULL,1,0),
(30,'Printere & scannere','printere-scannere',NULL,NULL,1,0),
(31,'Vaskemaskiner & tørretumblere','vaskemaskiner-toerretumblere',NULL,NULL,2,0),
(32,'Opvaskemaskiner','opvaskemaskiner',NULL,NULL,2,0),
(33,'Køleskabe & frysere','koeleskabe-frysere',NULL,NULL,2,0),
(34,'Komfurer & ovne','komfurer-ovne',NULL,NULL,2,0),
(35,'Emhætter','emhaetter',NULL,NULL,2,0),
(36,'Støvsugere & gulvpleje','stoevsugere-gulvpleje',NULL,NULL,2,0),
(37,'Kaffemaskiner & kedler','kaffemaskiner-kedler',NULL,NULL,2,0),
(38,'Blendere & køkkenmaskiner','blendere-koekkenmaskiner',NULL,NULL,2,0),
(39,'Airfryers & mikrobølgeovne','airfryers-mikroboelgeovne',NULL,NULL,2,0),
(40,'Luftrensere & ventilatorer','luftrensere-ventilatorer',NULL,NULL,2,0),
(41,'Gryder & pander','gryder-pander',NULL,NULL,3,0),
(42,'Knive & skærebrætter','knive-skaerebraetter',NULL,NULL,3,0),
(43,'Køkkenredskaber & utensilier','koekkenredskaber-utensilier',NULL,NULL,3,0),
(44,'Service & glas','service-glas',NULL,NULL,3,0),
(45,'Opbevaring & madkasser','opbevaring-madkasser',NULL,NULL,3,0),
(46,'Bageudstyr','bageudstyr',NULL,NULL,3,0),
(47,'Termo & drikkeflasker','termo-drikkeflasker',NULL,NULL,3,0),
(48,'Vandfiltre & tilbehør','vandfiltre-tilbehoer',NULL,NULL,3,0),
(49,'Sofaer & stole','sofaer-stole',NULL,NULL,4,0),
(50,'Borde & skabe','borde-skabe',NULL,NULL,4,0),
(51,'Senge & madrasser','senge-madrasser',NULL,NULL,4,0),
(52,'Opbevaring & reoler','opbevaring-reoler',NULL,NULL,4,0),
(53,'Belysning','belysning',NULL,NULL,4,0),
(54,'Tæpper & gardiner','taepper-gardiner',NULL,NULL,4,0),
(55,'Spejle & dekoration','spejle-dekoration',NULL,NULL,4,0),
(56,'Sengetøj & håndklæder','sengetoej-haandklaeder',NULL,NULL,4,0),
(57,'Badeværelsesmøbler & udstyr','badevaerelsesmoebler-udstyr',NULL,NULL,4,0),
(58,'Elværktøj','elvaerktoej',NULL,NULL,5,0),
(59,'Håndværktøj','haandvaerktoej',NULL,NULL,5,0),
(60,'Værktøjssæt & tasker','vaerktoejssaet-tasker',NULL,NULL,5,0),
(61,'Måleværktøj & laser','maalevaerktoej-laser',NULL,NULL,5,0),
(62,'Skruer, bolte & beslag','skruer-bolte-beslag',NULL,NULL,5,0),
(63,'Låse & sikkerhed','laase-sikkerhed',NULL,NULL,5,0),
(64,'Maling & malerudstyr','maling-malerudstyr',NULL,NULL,5,0),
(65,'VVS & el-tilbehør','vvs-el-tilbehoer',NULL,NULL,5,0),
(66,'Værkstedsmaskiner & bukke','vaerkstedsmaskiner-bukke',NULL,NULL,5,0),
(67,'Havemaskiner & plæneklippere','havemaskiner-plaeneklippere',NULL,NULL,6,0),
(68,'Haveredskaber','haveredskaber',NULL,NULL,6,0),
(69,'Vanding & sprinklere','vanding-sprinklere',NULL,NULL,6,0),
(70,'Udendørsmøbler & grill','udendoersmoebler-grill',NULL,NULL,6,0),
(71,'Planter, frø & jord','planter-froe-jord',NULL,NULL,6,0),
(72,'Drivhus & skure','drivhus-skure',NULL,NULL,6,0),
(73,'Hegn & terrasse','hegn-terrasse',NULL,NULL,6,0),
(74,'Udendørs belysning','udendoers-belysning',NULL,NULL,6,0),
(75,'Overdele & bluser','overdele-bluser',NULL,NULL,7,0),
(76,'Bukser & shorts','bukser-shorts',NULL,NULL,7,0),
(77,'Kjoler & nederdele','kjoler-nederdele',NULL,NULL,7,0),
(78,'Jakker & overtøj','jakker-overtoej',NULL,NULL,7,0),
(79,'Regntøj','regntoej',NULL,NULL,7,0),
(80,'Undertøj & sokker','undertoej-sokker',NULL,NULL,7,0),
(81,'Sko & støvler','sko-stoevler',NULL,NULL,7,0),
(82,'Tasker & rygsække','tasker-rygsaekke',NULL,NULL,7,0),
(83,'Bælter, hatte & handsker','baelter-hatte-handsker',NULL,NULL,7,0),
(84,'Ure & smykker','ure-smykker',NULL,NULL,7,0),
(85,'Arbejdstøj & beskyttelse','arbejdstoej-beskyttelse',NULL,NULL,7,0),
(86,'Hudpleje','hudpleje',NULL,NULL,8,0),
(87,'Kosmetik & makeup','kosmetik-makeup',NULL,NULL,8,0),
(88,'Hårpleje','haarpleje',NULL,NULL,8,0),
(89,'Hårstyling-værktøj','haarstyling-vaerktoej',NULL,NULL,8,0),
(90,'Barbering & trimning','barbering-trimning',NULL,NULL,8,0),
(91,'Mundpleje','mundpleje',NULL,NULL,8,0),
(92,'Parfume & duft','parfume-duft',NULL,NULL,8,0),
(93,'Neglepleje','neglepleje',NULL,NULL,8,0),
(94,'Hygiejneprodukter','hygiejneprodukter',NULL,NULL,8,0),
(95,'Termometre & helsemålere','termometre-helsemaalere',NULL,NULL,9,0),
(96,'Førstehjælp','foerstehjaelp',NULL,NULL,9,0),
(97,'Mobilitetshjælpemidler','mobilitetshjaelpemidler',NULL,NULL,9,0),
(98,'Massage & varme','massage-varme',NULL,NULL,9,0),
(99,'Kosttilskud & vitaminer','kosttilskud-vitaminer',NULL,NULL,9,0),
(100,'Hjemmetest & medicinudstyr','hjemmetest-medicinudstyr',NULL,NULL,9,0),
(101,'Barnevogne & klapvogne','barnevogne-klapvogne',NULL,NULL,10,0),
(102,'Autostole & bæreseler','autostole-baereseler',NULL,NULL,10,0),
(103,'Bleprodukter & pusleudstyr','bleprodukter-pusleudstyr',NULL,NULL,10,0),
(104,'Flasker & barnemad-udstyr','flasker-barnemad-udstyr',NULL,NULL,10,0),
(105,'Børneværelsesmøbler','boernevaerelsesmoebler',NULL,NULL,10,0),
(106,'Babymonitorer','babymonitorer',NULL,NULL,10,0),
(107,'Børnetøj & sko','boernetoej-sko',NULL,NULL,10,0),
(108,'Sikkerhedsudstyr','sikkerhedsudstyr',NULL,NULL,10,0),
(109,'Byggeklodser & konstruktion','byggeklodser-konstruktion',NULL,NULL,11,0),
(110,'Dukker & figurer','dukker-figurer',NULL,NULL,11,0),
(111,'Puslespil & brætspil','puslespil-braetspil',NULL,NULL,11,0),
(112,'Kreative sæt & hobby','kreative-saet-hobby',NULL,NULL,11,0),
(113,'Udelegetøj','udelegetoej',NULL,NULL,11,0),
(114,'Modelbyggeri & samlerobjekter','modelbyggeri-samlerobjekter',NULL,NULL,11,0),
(115,'Spil til voksne','spil-til-voksne',NULL,NULL,11,0),
(116,'Fitnessudstyr & vægte','fitnessudstyr-vaegte',NULL,NULL,12,0),
(117,'Cykler & cykeltilbehør','cykler-cykeltilbehoer',NULL,NULL,12,0),
(118,'Camping & vandring','camping-vandring',NULL,NULL,12,0),
(119,'Fiskeri & jagt','fiskeri-jagt',NULL,NULL,12,0),
(120,'Vandsport','vandsport',NULL,NULL,12,0),
(121,'Vintersport','vintersport',NULL,NULL,12,0),
(122,'Boldspil & racketsport','boldspil-racketsport',NULL,NULL,12,0),
(123,'Beskyttelsesudstyr','beskyttelsesudstyr',NULL,NULL,12,0),
(124,'Bildæk & hjul','bildaek-hjul',NULL,NULL,13,0),
(125,'Batterier & ladeudstyr','batterier-ladeudstyr',NULL,NULL,13,0),
(126,'Bilpleje & tilbehør','bilpleje-tilbehoer',NULL,NULL,13,0),
(127,'Værktøj & reservedele','vaerktoej-reservedele',NULL,NULL,13,0),
(128,'Motorcykel & scooter','motorcykel-scooter',NULL,NULL,13,0),
(129,'Elcykel & løbehjul','elcykel-loebehjul',NULL,NULL,13,0),
(130,'Autostole & interiør','autostole-interioer',NULL,NULL,13,0),
(131,'Elbil-ladere','elbil-ladere',NULL,NULL,13,0),
(132,'Bøger & e-bøger','boeger-e-boeger',NULL,NULL,14,0),
(133,'Musikinstrumenter','musikinstrumenter',NULL,NULL,14,0),
(134,'Lydudstyr & tilbehør','lydudstyr-tilbehoer',NULL,NULL,14,0),
(135,'Plader, CD & DVD','plader-cd-dvd',NULL,NULL,14,0),
(136,'Noter & undervisningsmateriale','noter-undervisningsmateriale',NULL,NULL,14,0),
(137,'Papir & notesbøger','papir-notesboeger',NULL,NULL,15,0),
(138,'Skriveredskaber','skriveredskaber',NULL,NULL,15,0),
(139,'Kontormaskiner','kontormaskiner',NULL,NULL,15,0),
(140,'Opbevaring & arkivering','opbevaring-arkivering',NULL,NULL,15,0),
(141,'Skoleudstyr & tasker','skoleudstyr-tasker',NULL,NULL,15,0),
(142,'Kreativt materiale','kreativt-materiale',NULL,NULL,15,0),
(143,'Kolonial & snacks','kolonial-snacks',NULL,NULL,16,0),
(144,'Kaffe & te','kaffe-te',NULL,NULL,16,0),
(145,'Drikkevarer & vin','drikkevarer-vin',NULL,NULL,16,0),
(146,'Frost & friskvarer','frost-friskvarer',NULL,NULL,16,0),
(147,'Rengøring & vask','rengoering-vask',NULL,NULL,16,0),
(148,'Papir & husholdning','papir-husholdning',NULL,NULL,16,0),
(149,'Dyrefoder & -pleje-varer','dyrefoder-pleje-varer',NULL,NULL,16,0),
(150,'Foder & godbidder','foder-godbidder',NULL,NULL,17,0),
(151,'Senge & transport','senge-transport',NULL,NULL,17,0),
(152,'Legetøj','legetoej',NULL,NULL,17,0),
(153,'Halsbånd & seler','halsbaand-seler',NULL,NULL,17,0),
(154,'Pleje & hygiejne','pleje-hygiejne',NULL,NULL,17,0),
(155,'Bur & akvarier','bur-akvarier',NULL,NULL,17,0),
(156,'Træning & tilbehør','traening-tilbehoer',NULL,NULL,17,0),
(157,'Hobby & kreative materialer','hobby-kreative-materialer',NULL,NULL,18,0),
(158,'Malergrej & lærreder','malergrej-laerreder',NULL,NULL,18,0),
(159,'Gavepapir & kort','gavepapir-kort',NULL,NULL,18,0),
(160,'Fest & dekoration','fest-dekoration',NULL,NULL,18,0),
(161,'Samlerobjekter & kunst','samlerobjekter-kunst',NULL,NULL,18,0);
/*!40000 ALTER TABLE `Categories` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `CategoryAspects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CategoryAspects` (
  `CategoryId` int(11) NOT NULL,
  `AspectId` int(11) NOT NULL,
  PRIMARY KEY (`CategoryId`,`AspectId`),
  KEY `IX_CategoryAspects_AspectId` (`AspectId`),
  CONSTRAINT `FK_CategoryAspects_Aspects_AspectId` FOREIGN KEY (`AspectId`) REFERENCES `Aspects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CategoryAspects_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `CategoryAspects` WRITE;
/*!40000 ALTER TABLE `CategoryAspects` DISABLE KEYS */;
INSERT INTO `CategoryAspects` VALUES
(1,9),
(1,10),
(1,11),
(1,18),
(1,19),
(1,20),
(1,21),
(1,22),
(1,23),
(1,24),
(1,25),
(1,26),
(1,27),
(1,28),
(2,9),
(2,13),
(2,29),
(2,30),
(2,31),
(2,32),
(2,33),
(2,34),
(2,35),
(2,36),
(2,37),
(2,38),
(2,39),
(2,40),
(3,9),
(3,24),
(3,31),
(3,41),
(3,42),
(3,43),
(3,44),
(3,45),
(3,46),
(3,47),
(3,48),
(3,49),
(3,50),
(4,26),
(4,46),
(4,51),
(4,52),
(4,53),
(4,54),
(4,55),
(4,56),
(4,57),
(4,58),
(4,59),
(4,60),
(4,61),
(5,10),
(5,17),
(5,24),
(5,62),
(5,63),
(5,64),
(5,65),
(5,66),
(5,67),
(5,68),
(5,69),
(5,70),
(5,71),
(6,9),
(6,24),
(6,72),
(6,73),
(6,74),
(6,75),
(6,76),
(6,77),
(6,78),
(6,79),
(6,80),
(7,51),
(7,55),
(7,81),
(7,82),
(7,83),
(7,84),
(7,85),
(7,86),
(7,87),
(7,88),
(7,89),
(7,90),
(7,91),
(8,11),
(8,92),
(8,93),
(8,94),
(8,95),
(8,96),
(8,97),
(8,98),
(8,99),
(8,100),
(8,101),
(9,2),
(9,9),
(9,26),
(9,102),
(9,103),
(9,104),
(9,105),
(9,106),
(9,107),
(10,24),
(10,26),
(10,49),
(10,51),
(10,108),
(10,109),
(10,110),
(10,111),
(10,112),
(10,113),
(10,114),
(11,10),
(11,115),
(11,116),
(11,117),
(11,118),
(11,119),
(11,120),
(11,121),
(11,122),
(11,123),
(11,124),
(12,9),
(12,31),
(12,51),
(12,64),
(12,69),
(12,79),
(12,82),
(12,91),
(12,125),
(12,126),
(13,9),
(13,65),
(13,76),
(13,127),
(13,128),
(13,129),
(13,130),
(13,131),
(13,132),
(13,133),
(14,26),
(14,134),
(14,135),
(14,136),
(14,137),
(14,138),
(14,139),
(14,140),
(14,141),
(15,9),
(15,11),
(15,100),
(15,142),
(15,143),
(15,144),
(15,145),
(15,146),
(15,147),
(16,148),
(16,149),
(16,150),
(16,151),
(16,152),
(16,153),
(16,154),
(16,155),
(16,156),
(16,157),
(17,9),
(17,82),
(17,96),
(17,100),
(17,143),
(17,158),
(17,159),
(17,160),
(17,161),
(18,26),
(18,54),
(18,119),
(18,144),
(18,162),
(18,163),
(18,164),
(18,165),
(18,166);
/*!40000 ALTER TABLE `CategoryAspects` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `FixSuggestionRules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `FixSuggestionRules` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Keywords` varchar(300) NOT NULL,
  `Title` varchar(200) NOT NULL,
  `Url` varchar(600) DEFAULT NULL,
  `Kind` int(11) NOT NULL,
  `AspectId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_FixSuggestionRules_AspectId` (`AspectId`),
  CONSTRAINT `FK_FixSuggestionRules_Aspects_AspectId` FOREIGN KEY (`AspectId`) REFERENCES `Aspects` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `FixSuggestionRules` WRITE;
/*!40000 ALTER TABLE `FixSuggestionRules` DISABLE KEYS */;
INSERT INTO `FixSuggestionRules` VALUES
(1,'går op i sømmene,revne,sprækket,sømmen er gået op,tynd i stoffet','Sådan reparerer du en revne i tøj','https://www.youtube.com/results?search_query=reparer+revne+i+t%C3%B8j',0,NULL),
(2,'lynlås,lynlåsen går i stykker','Skift eller reparer en lynlås','https://www.youtube.com/results?search_query=reparer+lynl%C3%A5s',0,NULL),
(3,'ruster,rust','Fjern rust og beskyt overfladen','https://www.reddit.com/search/?q=remove%20rust%20metal',1,NULL),
(4,'belægningen slipper,nonstick,teflon,slip-let','Kan en teflonpande reddes?','https://www.youtube.com/results?search_query=reparer+teflonpande',0,NULL),
(5,'batteriet holder ikke,batteri,akku','Skift eller genopbyg batteriet','https://www.reddit.com/search/?q=replace%20tool%20battery',1,NULL),
(6,'plastik af lav kvalitet,knækket plastik,knækker','Reparation af knækket plastik','https://www.youtube.com/results?search_query=repair+broken+plastic',0,NULL),
(7,'utæt,lækker,vand trænger ind','Find og tæt lækagen','https://www.youtube.com/results?search_query=fix+leak+seal',0,NULL),
(8,'dårlig smag,smager dårligt','Sådan forbedrer du smagen','https://www.reddit.com/search/?q=improve%20taste%20food',1,NULL);
/*!40000 ALTER TABLE `FixSuggestionRules` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `FixVotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `FixVotes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `FixId` int(11) NOT NULL,
  `UserId` varchar(450) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_FixVotes_FixId_UserId` (`FixId`,`UserId`),
  KEY `IX_FixVotes_UserId` (`UserId`),
  CONSTRAINT `FK_FixVotes_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`),
  CONSTRAINT `FK_FixVotes_Fixes_FixId` FOREIGN KEY (`FixId`) REFERENCES `Fixes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `FixVotes` WRITE;
/*!40000 ALTER TABLE `FixVotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `FixVotes` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `Fixes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Fixes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` int(11) NOT NULL,
  `ReviewId` int(11) DEFAULT NULL,
  `UserId` varchar(450) DEFAULT NULL,
  `Title` varchar(200) NOT NULL,
  `Body` varchar(4000) NOT NULL,
  `SourceUrl` varchar(600) DEFAULT NULL,
  `Source` int(11) NOT NULL,
  `SourceKind` int(11) DEFAULT NULL,
  `AspectId` int(11) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `IsHidden` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Fixes_AspectId` (`AspectId`),
  KEY `IX_Fixes_ProductId` (`ProductId`),
  KEY `IX_Fixes_ReviewId` (`ReviewId`),
  KEY `IX_Fixes_UserId` (`UserId`),
  CONSTRAINT `FK_Fixes_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Fixes_Aspects_AspectId` FOREIGN KEY (`AspectId`) REFERENCES `Aspects` (`Id`),
  CONSTRAINT `FK_Fixes_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fixes_Reviews_ReviewId` FOREIGN KEY (`ReviewId`) REFERENCES `Reviews` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `Fixes` WRITE;
/*!40000 ALTER TABLE `Fixes` DISABLE KEYS */;
INSERT INTO `Fixes` VALUES
(1,2,1,'3624421f-13fd-4485-8e2f-2442f5b7397a','Sy sømmen med en kraftigere tråd','Vend jakken og sy den revnede søm med en kraftig nylontråd og dobbelt sting. Det forlænger levetiden betydeligt.',NULL,0,NULL,NULL,'2026-10-09 15:44:13.529189',0),
(2,8,7,'bad7b07d-85cc-4343-8223-bf80da21be3d','Brug panden til lav varme','Slip-let belægning holder meget længere, hvis man aldrig varmer panden op uden indhold og kun bruger plastikredskaber.',NULL,0,NULL,NULL,'2026-10-09 15:44:13.529475',0),
(3,4,3,'770da7dc-764a-4903-979e-b2e06913b970','Køb et uoriginalt adapterbatteri','Der findes adaptere fra andre 18V-systemer. Det er en nødløsning, men holder maskinen kørende lidt endnu.','https://www.reddit.com/r/Tools/search/?q=18v%20battery%20adapter',0,NULL,NULL,'2026-10-09 15:44:13.529477',0),
(4,2,1,NULL,'Sådan reparerer du en revne i tøj (YouTube)','Automatisk forslag fundet ud fra dine ord om \"går op i sømmene\".','https://www.youtube.com/results?search_query=reparer+revne+i+t%C3%B8j',1,0,NULL,'2026-10-09 15:44:13.529498',0),
(5,8,7,NULL,'Kan en teflonpande reddes? (YouTube)','Automatisk forslag fundet ud fra dine ord om \"belægningen slipper\".','https://www.youtube.com/results?search_query=reparer+teflonpande+bel%C3%A6gning',1,0,NULL,'2026-10-09 15:44:13.529554',0),
(6,4,3,NULL,'Reddit: Erfaringer med at skifte celler i værktøjsbatterier','Automatisk forslag fundet ud fra dine ord om \"batteriet holder ikke\".','https://www.reddit.com/search/?q=replace%20tool%20battery%20cells',1,1,NULL,'2026-10-09 15:44:13.529554',0);
/*!40000 ALTER TABLE `Fixes` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `ProductListItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ProductListItems` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProductListId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL,
  `Note` varchar(600) DEFAULT NULL,
  `SortOrder` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ProductListItems_ProductListId_ProductId` (`ProductListId`,`ProductId`),
  KEY `IX_ProductListItems_ProductId` (`ProductId`),
  CONSTRAINT `FK_ProductListItems_ProductLists_ProductListId` FOREIGN KEY (`ProductListId`) REFERENCES `ProductLists` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProductListItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `ProductListItems` WRITE;
/*!40000 ALTER TABLE `ProductListItems` DISABLE KEYS */;
INSERT INTO `ProductListItems` VALUES
(1,1,1,'Trelags med tapede sømme og reparerbar lynlås.',0),
(2,2,3,'Akku-borehammer med 8 års reservedelsgaranti.',0),
(3,3,7,'Stål og kobber – ingen belægning der slipper.',0),
(4,3,11,'Holder kaffen varm i 12 timer.',1),
(5,3,12,'Kan stoppes og holder formen.',2),
(6,4,5,'Massivt bøgetræ, går i arv.',0);
/*!40000 ALTER TABLE `ProductListItems` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `ProductLists`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ProductLists` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(160) NOT NULL,
  `Slug` varchar(180) NOT NULL,
  `Description` varchar(2000) DEFAULT NULL,
  `CategoryId` int(11) DEFAULT NULL,
  `OwnerUserId` varchar(450) DEFAULT NULL,
  `IsEditorial` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `IsHidden` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ProductLists_Slug` (`Slug`),
  KEY `IX_ProductLists_CategoryId` (`CategoryId`),
  KEY `IX_ProductLists_OwnerUserId` (`OwnerUserId`),
  CONSTRAINT `FK_ProductLists_AspNetUsers_OwnerUserId` FOREIGN KEY (`OwnerUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ProductLists_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `ProductLists` WRITE;
/*!40000 ALTER TABLE `ProductLists` DISABLE KEYS */;
INSERT INTO `ProductLists` VALUES
(1,'Regntøj til børn der holder','regntoej-til-boern','Peer-reviewed regntøj, der stadig er tæt efter flere sæsoner.',79,NULL,1,'2026-10-09 15:44:13.584898',0),
(2,'Værktøj til hobbyværkstedet','vaerktoej-hobbyvaerkstedet','Det grundlæggende værktøj du kun køber én gang.',58,NULL,1,'2026-10-09 15:44:13.585635',0),
(3,'Must-have i køkkenet','must-have-koekkenet','Redskaber der holder i årevis og kan repareres eller vedligeholdes.',3,NULL,1,'2026-10-09 15:44:13.585642',0),
(4,'Legetøj af høj kvalitet','legetoej-af-hoej-kvalitet','Legetøj der kan tåle leg og gå i arv.',11,NULL,1,'2026-10-09 15:44:13.585646',0);
/*!40000 ALTER TABLE `ProductLists` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `ProductRecommendations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ProductRecommendations` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `SourceProductId` int(11) NOT NULL,
  `AlternativeProductId` int(11) NOT NULL,
  `Reason` varchar(600) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ProductRecommendations_SourceProductId_AlternativeProductId` (`SourceProductId`,`AlternativeProductId`),
  KEY `IX_ProductRecommendations_AlternativeProductId` (`AlternativeProductId`),
  CONSTRAINT `FK_ProductRecommendations_Products_AlternativeProductId` FOREIGN KEY (`AlternativeProductId`) REFERENCES `Products` (`Id`),
  CONSTRAINT `FK_ProductRecommendations_Products_SourceProductId` FOREIGN KEY (`SourceProductId`) REFERENCES `Products` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `ProductRecommendations` WRITE;
/*!40000 ALTER TABLE `ProductRecommendations` DISABLE KEYS */;
INSERT INTO `ProductRecommendations` VALUES
(1,2,1,'BilligTex-jakken går op i sømmene efter få uger. Storm-jakken er tapet og kan repareres.'),
(2,4,3,'PowerMax har ingen reservedele og et batteri der dør hurtigt. SH18 har 8 års reservedelsgaranti.'),
(3,6,5,'Plastklodserne mangler dele og knækker. Træklodserne holder i generationer.'),
(4,8,7,'Belægningen slipper efter få måneder. Kobberpanden har ingen belægning der kan slippe.'),
(5,10,11,'Eksempel på parring på tværs af kategorier – redigeres i admin.');
/*!40000 ALTER TABLE `ProductRecommendations` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `Products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Products` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(160) NOT NULL,
  `Slug` varchar(180) NOT NULL,
  `Description` varchar(4000) DEFAULT NULL,
  `ImageUrl` varchar(600) DEFAULT NULL,
  `ModelNumber` varchar(80) DEFAULT NULL,
  `CategoryId` int(11) NOT NULL,
  `BrandId` int(11) DEFAULT NULL,
  `CreatedByUserId` varchar(450) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `IsRecommended` tinyint(1) NOT NULL,
  `IsHidden` tinyint(1) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Products_Slug` (`Slug`),
  KEY `IX_Products_BrandId` (`BrandId`),
  KEY `IX_Products_CategoryId` (`CategoryId`),
  KEY `IX_Products_CreatedByUserId` (`CreatedByUserId`),
  CONSTRAINT `FK_Products_AspNetUsers_CreatedByUserId` FOREIGN KEY (`CreatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Products_Brands_BrandId` FOREIGN KEY (`BrandId`) REFERENCES `Brands` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Products_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `Products` WRITE;
/*!40000 ALTER TABLE `Products` DISABLE KEYS */;
INSERT INTO `Products` VALUES
(1,'Nordvik Storm Regnjakke','nordvik-storm-regnjakke','Trelags regnjakke med tapede sømme, til børn og voksne. Kan repareres og får reservedele i 10 år.','https://placehold.co/400x300?text=Storm','NV-900',79,1,NULL,'2026-10-09 15:44:12.691325',1,0,NULL),
(2,'BilligTex Regnjakke Junior','billigtex-regnjakke-junior','Billig børneregnjakke. Let at købe, men sømme og lynlås holder ikke til daglig brug.','https://placehold.co/400x300?text=BilligTex','BT-JR',79,6,NULL,'2026-10-09 15:44:12.729142',0,0,NULL),
(3,'Stålform Akku-borehammer SH18','staalform-akku-borehammer-sh18','18V akku-borehammer med to batterier, metalgear og 8 års reservedelsgaranti.','https://placehold.co/400x300?text=SH18','SH18',58,2,NULL,'2026-10-09 15:44:12.729322',1,0,NULL),
(4,'VoltMax PowerMax 18V Boremaskine','voltmax-powermax-18v','Lavpris boremaskine. Batteriet mister kapacitet hurtigt og der findes ikke reservedele.','https://placehold.co/400x300?text=PowerMax','PM-18',58,7,NULL,'2026-10-09 15:44:12.729424',0,0,NULL),
(5,'Birkholm Træklodser 100 dele','birkholm-traeklodser-100','Massivt bøgetræ, gennemtestet og kan gå i arv i generationer.','https://placehold.co/400x300?text=Tr%C3%A6klodser','BH-100',109,3,NULL,'2026-10-09 15:44:12.729540',1,0,NULL),
(6,'PlastoByg Byggeklodser 250 dele','plastobyg-byggeklodser-250','Meget billige plastklodser. Flere dele mangler og kanterne er skarpe.','https://placehold.co/400x300?text=PlastoByg','PB-250',109,8,NULL,'2026-10-09 15:44:12.729610',0,0,NULL),
(7,'Kobber & Co Stegepande 28 cm','kobber-co-stegepande-28','Kobberkerne med stålbund, tåler opvaskemaskine og holder varmen i årevis.','https://placehold.co/400x300?text=Stegepande','KC-28',41,4,NULL,'2026-10-09 15:44:12.729698',1,0,NULL),
(8,'NonStick Pro Stegepande 28 cm','nonstick-pro-stegepande-28','Billig pande med slip-let belægning der hurtigt slipper.','https://placehold.co/400x300?text=NonStick','NS-28',41,7,NULL,'2026-10-09 15:44:12.729762',0,0,NULL),
(9,'Terra Outdoor Vandrestøvler','terra-outdoor-vandrestoevler','Vandtætte vandrestøvler med udskiftelig sål og solidt læder.','https://placehold.co/400x300?text=Terra','TO-HIKE',81,5,NULL,'2026-10-09 15:44:12.729849',1,0,NULL),
(10,'VoltMax USB-C Oplader 65W','voltmax-usb-c-oplader-65w','Billig oplader. Bliver varm og holder op med at virke efter få måneder.','https://placehold.co/400x300?text=Oplader','VM-65',26,7,NULL,'2026-10-09 15:44:12.729917',0,0,NULL),
(11,'Birkholm Termokande 1,0 L','birkholm-termokande-1l','Rustfri termokande der holder kaffen varm i 12 timer, år efter år.','https://placehold.co/400x300?text=Termokande','BH-TERM',47,3,NULL,'2026-10-09 15:44:12.730003',1,0,NULL),
(12,'Nordvik Uldsokker 3-pak','nordvik-uldsokker-3-pak','Uldsokker der holder formen og kan stoppes, når de bliver slidte.','https://placehold.co/400x300?text=Uldsokker','NV-SOK',80,1,NULL,'2026-10-09 15:44:12.730067',1,0,NULL);
/*!40000 ALTER TABLE `Products` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `ReviewAspectVotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ReviewAspectVotes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ReviewAspectId` int(11) NOT NULL,
  `UserId` varchar(450) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ReviewAspectVotes_ReviewAspectId_UserId` (`ReviewAspectId`,`UserId`),
  KEY `IX_ReviewAspectVotes_UserId` (`UserId`),
  CONSTRAINT `FK_ReviewAspectVotes_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`),
  CONSTRAINT `FK_ReviewAspectVotes_ReviewAspects_ReviewAspectId` FOREIGN KEY (`ReviewAspectId`) REFERENCES `ReviewAspects` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `ReviewAspectVotes` WRITE;
/*!40000 ALTER TABLE `ReviewAspectVotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `ReviewAspectVotes` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `ReviewAspects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ReviewAspects` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ReviewId` int(11) NOT NULL,
  `AspectId` int(11) NOT NULL,
  `Note` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ReviewAspects_AspectId` (`AspectId`),
  KEY `IX_ReviewAspects_ReviewId` (`ReviewId`),
  CONSTRAINT `FK_ReviewAspects_Aspects_AspectId` FOREIGN KEY (`AspectId`) REFERENCES `Aspects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ReviewAspects_Reviews_ReviewId` FOREIGN KEY (`ReviewId`) REFERENCES `Reviews` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `ReviewAspects` WRITE;
/*!40000 ALTER TABLE `ReviewAspects` DISABLE KEYS */;
INSERT INTO `ReviewAspects` VALUES
(1,1,51,'Sprang op under armen'),
(2,1,83,NULL),
(3,1,85,NULL),
(4,2,87,NULL),
(5,2,88,NULL),
(6,3,18,NULL),
(7,3,36,NULL),
(8,4,69,NULL),
(9,4,17,NULL),
(10,5,115,NULL),
(11,5,116,NULL),
(12,5,119,NULL),
(13,6,121,NULL),
(14,6,122,NULL),
(15,7,41,NULL),
(16,7,1,NULL),
(17,8,48,NULL),
(18,8,47,NULL),
(19,9,20,NULL),
(20,10,79,NULL),
(21,10,91,NULL),
(22,11,87,NULL);
/*!40000 ALTER TABLE `ReviewAspects` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `ReviewVotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ReviewVotes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ReviewId` int(11) NOT NULL,
  `UserId` varchar(450) DEFAULT NULL,
  `IsAgree` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ReviewVotes_ReviewId_UserId` (`ReviewId`,`UserId`),
  KEY `IX_ReviewVotes_UserId` (`UserId`),
  CONSTRAINT `FK_ReviewVotes_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`),
  CONSTRAINT `FK_ReviewVotes_Reviews_ReviewId` FOREIGN KEY (`ReviewId`) REFERENCES `Reviews` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `ReviewVotes` WRITE;
/*!40000 ALTER TABLE `ReviewVotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `ReviewVotes` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `Reviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Reviews` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` int(11) NOT NULL,
  `UserId` varchar(450) DEFAULT NULL,
  `Verdict` int(11) NOT NULL,
  `Title` varchar(160) NOT NULL,
  `Body` varchar(6000) NOT NULL,
  `OwnershipDuration` varchar(120) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `IsHidden` tinyint(1) NOT NULL,
  `ImageUrl` varchar(600) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Reviews_ProductId` (`ProductId`),
  KEY `IX_Reviews_UserId` (`UserId`),
  CONSTRAINT `FK_Reviews_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Reviews_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `Reviews` WRITE;
/*!40000 ALTER TABLE `Reviews` DISABLE KEYS */;
INSERT INTO `Reviews` VALUES
(1,2,'770da7dc-764a-4903-979e-b2e06913b970',0,'Gik op i sømmene efter to uger','Lynlåsen gik i stykker i første uge, og efter to uger var sømmen under armen gået op. Man kan reparere den, men stoffet er så tyndt, at det næsten ikke kan syes. Plastikken i lynlåsen er af lav kvalitet.','2 måneder','2026-10-09 15:44:13.396087',NULL,0,NULL),
(2,1,'3624421f-13fd-4485-8e2f-2442f5b7397a',1,'Holder tør i timevis','Brugt i to vintre, tapede sømme er stadig intakte og lynlåsen kører let. Da hætten revnede, sendte Nordvik en ny uden beregning.','3 år','2026-10-09 15:44:13.396581',NULL,0,NULL),
(3,4,'da9f15bb-5bab-4b82-ab49-8c0c9f128dfb',0,'Batteriet holder ikke, og der findes ingen reservedele','Efter tre måneder kunne batteriet kun holde til 10 minutters boring. Der findes ikke reservedele at købe, så hele maskinen er skrald.','4 måneder','2026-10-09 15:44:13.396582',NULL,0,NULL),
(4,3,'3624421f-13fd-4485-8e2f-2442f5b7397a',1,'Værkstedskvalitet til hjemmebrugeren','To år med hård brug og begge batterier er stadig som nye. Reservedele kan bestilles i 8 år.','2 år','2026-10-09 15:44:13.396583',NULL,0,NULL),
(5,6,'770da7dc-764a-4903-979e-b2e06913b970',0,'Skarpe kanter og manglende dele','Kassen lovede 250 dele, men der var kun 231. Flere klodser er knækket ved første leg, og kanterne er skarpe.','1 måned','2026-10-09 15:44:13.396583',NULL,0,NULL),
(6,5,'bad7b07d-85cc-4343-8223-bf80da21be3d',1,'Går i arv','De samme klodser som mine forældre havde. Massivt træ, ingen skarpe kanter, og de passer perfekt sammen.','1 år','2026-10-09 15:44:13.396585',NULL,0,NULL),
(7,8,'da9f15bb-5bab-4b82-ab49-8c0c9f128dfb',0,'Belægningen slipper efter få måneder','Efter fire måneder begyndte belægningen at slippe i bunden. Panden er nu ubrugelig.','5 måneder','2026-10-09 15:44:13.396585',NULL,0,NULL),
(8,7,'bad7b07d-85cc-4343-8223-bf80da21be3d',1,'Holder varmen perfekt','Brugt dagligt i et år. Ingen belægning der slipper, fordi det er stål og kobber. Kan tåle opvaskemaskine.','1 år','2026-10-09 15:44:13.396586',NULL,0,NULL),
(9,10,'770da7dc-764a-4903-979e-b2e06913b970',0,'Bliver gloende varm og dør','Opladeren bliver meget varm og holdt op med at virke efter to måneder.','2 måneder','2026-10-09 15:44:13.396587',NULL,0,NULL),
(10,9,'3624421f-13fd-4485-8e2f-2442f5b7397a',1,'Kan fås med ny sål','Tre års vandring, og jeg har lige fået skiftet sålen. Læderet er stadig fint.','3 år','2026-10-09 15:44:13.396587',NULL,0,NULL),
(11,12,'da9f15bb-5bab-4b82-ab49-8c0c9f128dfb',1,'Varme og kan stoppes','Uldsokkerne holder formen og kan stoppes, når hælen bliver slidt. Anbefales.','2 år','2026-10-09 15:44:13.396590',NULL,0,NULL);
/*!40000 ALTER TABLE `Reviews` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `SiteSettings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `SiteSettings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Key` varchar(120) NOT NULL,
  `Value` varchar(4000) DEFAULT NULL,
  `Description` varchar(400) DEFAULT NULL,
  `Group` varchar(80) DEFAULT NULL,
  `UpdatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_SiteSettings_Key` (`Key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `SiteSettings` WRITE;
/*!40000 ALTER TABLE `SiteSettings` DISABLE KEYS */;
/*!40000 ALTER TABLE `SiteSettings` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES
('20261009131105_InitialMySql','8.0.13'),
('20261009150348_AddReviewPhoto','8.0.13');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

