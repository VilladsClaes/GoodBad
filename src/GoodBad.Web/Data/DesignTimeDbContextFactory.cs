using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GoodBad.Web.Data;

/// <summary>
/// Used by the EF Core CLI tools (dotnet ef) to create migrations without
/// starting the web application. The connection string only has to be
/// syntactically valid – migrations are generated offline.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=GoodBad;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        return new AppDbContext(options);
    }
}
