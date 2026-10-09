using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Database: MySQL (the database type available on Simply.com). The connection
// string is read from appsettings.Production.json, from an environment
// variable (ConnectionStrings__DefaultConnection) or from the web.config on
// the web hotel. See README.md for the exact Simply.com setup.
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' mangler. Saet den i appsettings.Production.json " +
        "eller som environment variable ConnectionStrings__DefaultConnection.");
}

var serverVersion = ResolveServerVersion(builder.Configuration, connectionString!);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString!, serverVersion, my => my.EnableRetryOnFailure()));

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

// Application services
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddScoped<SiteSettingsService>();
builder.Services.AddScoped<ISiteSettings>(sp => sp.GetRequiredService<SiteSettingsService>());
builder.Services.AddScoped<ImageStorageService>();
builder.Services.AddSingleton(sp => UploadLocation.Resolve(
    sp.GetRequiredService<IConfiguration>(),
    sp.GetRequiredService<IWebHostEnvironment>(),
    sp.GetRequiredService<ILogger<UploadLocation>>()));
builder.Services.AddScoped<FixSuggestionService>();
builder.Services.AddScoped<YouTubeSearchClient>();
builder.Services.AddScoped<RedditSearchClient>();

var app = builder.Build();

// Uploaded photos live in wwwroot/uploads, or in App_Data/uploads when the web
// root is read-only on the web hotel (see UploadLocation). Resolved eagerly so
// a missing/full folder is reported at startup instead of on the first upload.
var uploads = app.Services.GetRequiredService<UploadLocation>();

// Create / migrate the database and seed the taxonomy on startup. This keeps
// deployment to shared hosting simple (no separate migration step needed).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
    await AdminBootstrapper.EnsureAdminRoleAsync(scope.ServiceProvider, builder.Configuration);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

if (uploads.IsOutsideWebRoot)
{
    // The upload folder sits outside wwwroot, so it is served explicitly.
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploads.RootDirectory),
        RequestPath = uploads.WebPath
    });
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin area, e.g. /Admin/Products or /Admin/Settings.
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

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

/// <summary>
/// Works out which MySQL flavour the app is talking to. Simply.com may run
/// either MySQL 8 or MariaDB, and the two need slightly different SQL. The
/// version can be pinned with "Database:ServerVersion" (e.g. "8.0.42" or
/// "10.11.6-MariaDB"); otherwise it is detected from the server at startup.
/// </summary>
static ServerVersion ResolveServerVersion(IConfiguration configuration, string connectionString)
{
    var configured = configuration["Database:ServerVersion"];
    if (!string.IsNullOrWhiteSpace(configured))
    {
        return ServerVersion.Parse(configured!);
    }

    try
    {
        return ServerVersion.AutoDetect(connectionString);
    }
    catch (Exception)
    {
        // If the server cannot be reached we still want the app to boot far
        // enough to give a useful error page instead of crashing.
        return new MySqlServerVersion(new Version(8, 0, 42));
    }
}
