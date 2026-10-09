using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodBad.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMySql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aspects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Slug = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aspects", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_unicode_ci"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_unicode_ci"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    DisplayName = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_unicode_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_unicode_ci"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_unicode_ci"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8mb4_unicode_ci"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Slug = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: false, collation: "utf8mb4_unicode_ci"),
                    LogoUrl = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci"),
                    WebsiteUrl = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Country = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Slug = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Icon = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true, collation: "utf8mb4_unicode_ci"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Value = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Group = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_unicode_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "FixSuggestionRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Keywords = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Url = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    AspectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixSuggestionRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixSuggestionRules_Aspects_AspectId",
                        column: x => x.AspectId,
                        principalTable: "Aspects",
                        principalColumn: "Id");
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_unicode_ci"),
                    Value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "CategoryAspects",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    AspectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAspects", x => new { x.CategoryId, x.AspectId });
                    table.ForeignKey(
                        name: "FK_CategoryAspects_Aspects_AspectId",
                        column: x => x.AspectId,
                        principalTable: "Aspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryAspects_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "ProductLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Slug = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true, collation: "utf8mb4_unicode_ci"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    OwnerUserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true, collation: "utf8mb4_unicode_ci"),
                    IsEditorial = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLists_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductLists_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Slug = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true, collation: "utf8mb4_unicode_ci"),
                    ImageUrl = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci"),
                    ModelNumber = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_unicode_ci"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true, collation: "utf8mb4_unicode_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsRecommended = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "ProductListItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductListId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci"),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductListItems_ProductLists_ProductListId",
                        column: x => x.ProductListId,
                        principalTable: "ProductLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductListItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "ProductRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceProductId = table.Column<int>(type: "int", nullable: false),
                    AlternativeProductId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRecommendations_Products_AlternativeProductId",
                        column: x => x.AlternativeProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRecommendations_Products_SourceProductId",
                        column: x => x.SourceProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Verdict = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Body = table.Column<string>(type: "varchar(6000)", maxLength: 6000, nullable: false, collation: "utf8mb4_unicode_ci"),
                    OwnershipDuration = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_unicode_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Fixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ReviewId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_unicode_ci"),
                    Body = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false, collation: "utf8mb4_unicode_ci"),
                    SourceUrl = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true, collation: "utf8mb4_unicode_ci"),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SourceKind = table.Column<int>(type: "int", nullable: true),
                    AspectId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fixes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Fixes_Aspects_AspectId",
                        column: x => x.AspectId,
                        principalTable: "Aspects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fixes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fixes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id");
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "ReviewAspects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    AspectId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, collation: "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewAspects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewAspects_Aspects_AspectId",
                        column: x => x.AspectId,
                        principalTable: "Aspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewAspects_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "ReviewVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true, collation: "utf8mb4_unicode_ci"),
                    IsAgree = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewVotes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "FixVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FixId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true, collation: "utf8mb4_unicode_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixVotes_Fixes_FixId",
                        column: x => x.FixId,
                        principalTable: "Fixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "ReviewAspectVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReviewAspectId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true, collation: "utf8mb4_unicode_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewAspectVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewAspectVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewAspectVotes_ReviewAspects_ReviewAspectId",
                        column: x => x.ReviewAspectId,
                        principalTable: "ReviewAspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_Slug",
                table: "Aspects",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Slug",
                table: "Brands",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAspects_AspectId",
                table: "CategoryAspects",
                column: "AspectId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixes_AspectId",
                table: "Fixes",
                column: "AspectId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixes_ProductId",
                table: "Fixes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixes_ReviewId",
                table: "Fixes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixes_UserId",
                table: "Fixes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixSuggestionRules_AspectId",
                table: "FixSuggestionRules",
                column: "AspectId");

            migrationBuilder.CreateIndex(
                name: "IX_FixVotes_FixId_UserId",
                table: "FixVotes",
                columns: new[] { "FixId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FixVotes_UserId",
                table: "FixVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductListItems_ProductId",
                table: "ProductListItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductListItems_ProductListId_ProductId",
                table: "ProductListItems",
                columns: new[] { "ProductListId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductLists_CategoryId",
                table: "ProductLists",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLists_OwnerUserId",
                table: "ProductLists",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLists_Slug",
                table: "ProductLists",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecommendations_AlternativeProductId",
                table: "ProductRecommendations",
                column: "AlternativeProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecommendations_SourceProductId_AlternativeProductId",
                table: "ProductRecommendations",
                columns: new[] { "SourceProductId", "AlternativeProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedByUserId",
                table: "Products",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAspects_AspectId",
                table: "ReviewAspects",
                column: "AspectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAspects_ReviewId",
                table: "ReviewAspects",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAspectVotes_ReviewAspectId_UserId",
                table: "ReviewAspectVotes",
                columns: new[] { "ReviewAspectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAspectVotes_UserId",
                table: "ReviewAspectVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_ReviewId_UserId",
                table: "ReviewVotes",
                columns: new[] { "ReviewId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_UserId",
                table: "ReviewVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_Key",
                table: "SiteSettings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CategoryAspects");

            migrationBuilder.DropTable(
                name: "FixSuggestionRules");

            migrationBuilder.DropTable(
                name: "FixVotes");

            migrationBuilder.DropTable(
                name: "ProductListItems");

            migrationBuilder.DropTable(
                name: "ProductRecommendations");

            migrationBuilder.DropTable(
                name: "ReviewAspectVotes");

            migrationBuilder.DropTable(
                name: "ReviewVotes");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Fixes");

            migrationBuilder.DropTable(
                name: "ProductLists");

            migrationBuilder.DropTable(
                name: "ReviewAspects");

            migrationBuilder.DropTable(
                name: "Aspects");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
