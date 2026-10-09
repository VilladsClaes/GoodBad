using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GoodBad.Web.Data;

/// <summary>
/// Used by the EF Core CLI tools (dotnet ef) to create migrations without
/// starting the web application. The connection string only has to be
/// syntactically valid – migrations are generated offline against the MySQL
/// provider, which is the database used on Simply.com.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(
                "Server=localhost;Port=3306;Database=goodbad;User=root;Password=;Character Set=utf8mb4",
                new MySqlServerVersion(new Version(8, 0, 42)))
            .Options;

        return new AppDbContext(options);
    }
}
