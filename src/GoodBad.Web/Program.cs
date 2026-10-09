using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// The database provider can be switched through configuration. On Simply.com
// (MS SQL Server) the "DefaultConnection" string is used automatically; if no
// connection string is present the app falls back to a local SQLite file, which
// makes local development and testing trivial.
var provider = builder.Configuration["Database:Provider"] ?? "Auto";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.Equals(provider, "Auto", StringComparison.OrdinalIgnoreCase))
{
    provider = string.IsNullOrWhiteSpace(connectionString) ? "Sqlite" : "SqlServer";
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(string.IsNullOrWhiteSpace(connectionString) ? "Data Source=goodbad.db" : connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
    }
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<ApplicationUser>, AppUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FixSuggestionService>();

var app = builder.Build();

// Create / migrate the database and seed the taxonomy on startup. This keeps
// deployment to shared hosting simple (no separate migration step needed).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsSqlite())
    {
        await db.Database.EnsureCreatedAsync();
    }
    else
    {
        await db.Database.MigrateAsync();
    }

    await DbSeeder.SeedAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Slug based detail pages, e.g. /Products/Details/nordvik-storm-regnjakke or
// /Categories/Details/regntoej. Declared before the default route so the third
// segment binds to "slug" instead of the numeric "id".
app.MapControllerRoute(
    name: "slugDetails",
    pattern: "{controller=Home}/{action=Index}/{slug}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
