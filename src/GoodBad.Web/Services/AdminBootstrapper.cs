using GoodBad.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace GoodBad.Web.Services;

/// <summary>
/// Makes sure the Admin role exists and that the e-mail addresses listed under
/// "Admin:Emails" in the configuration are administrators. Runs on every start,
/// which makes it easy to (re)gain access to /Admin without a database client.
/// </summary>
public static class AdminBootstrapper
{
    public static async Task EnsureAdminRoleAsync(IServiceProvider services, IConfiguration configuration, ILogger? logger = null)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            logger?.LogInformation("Oprettede rollen {Role}.", Roles.Admin);
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var emails = configuration.GetSection("Admin:Emails").Get<string[]>() ?? Array.Empty<string>();

        foreach (var email in emails.Where(e => !string.IsNullOrWhiteSpace(e)))
        {
            var user = await userManager.FindByEmailAsync(email.Trim());
            if (user is null)
            {
                logger?.LogWarning("Admin-e-mailen {Email} findes ikke som bruger endnu. Opret kontoen, og genstart appen.", email);
                continue;
            }

            if (!await userManager.IsInRoleAsync(user, Roles.Admin))
            {
                await userManager.AddToRoleAsync(user, Roles.Admin);
                logger?.LogInformation("{Email} er nu administrator.", email);
            }
        }

        if (configuration.GetValue("Admin:AutoPromoteFirstUser", true))
        {
            var admins = await userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count == 0)
            {
                logger?.LogWarning(
                    "Ingen administratorer fundet. Den foerste bruger der registrerer sig, bliver administrator. " +
                    "Tilfoej din e-mail under Admin:Emails i appsettings.Production.json for at styre det.");
            }
        }
    }

    public static bool AutoPromoteFirstUser(IConfiguration configuration)
        => configuration.GetValue("Admin:AutoPromoteFirstUser", true);
}
