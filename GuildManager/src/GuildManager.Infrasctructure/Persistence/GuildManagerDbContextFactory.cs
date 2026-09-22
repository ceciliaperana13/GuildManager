using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace GuildManager.Infrastructure.Persistence;


public class GuildManagerDbContextFactory : IDesignTimeDbContextFactory<GuildManagerDbContext>
{
    public GuildManagerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("GuildManagerDb")
            ?? throw new InvalidOperationException("Connection string 'GuildManagerDb' introuvable.");

        var optionsBuilder = new DbContextOptionsBuilder<GuildManagerDbContext>();
        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new GuildManagerDbContext(optionsBuilder.Options);
    }
}