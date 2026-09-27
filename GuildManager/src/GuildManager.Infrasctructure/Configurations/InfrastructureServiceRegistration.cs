using GuildManager.Domain.Repositories;
using GuildManager.Infrastructure.Persistence;
using GuildManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuildManager.Infrastructure.Configurations;

public static class InfrastructureServiceRegistration
{
    
    //     builder.Services.AddInfrastructure(builder.Configuration);
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("GuildManagerDb")
            ?? throw new InvalidOperationException("Connection string 'GuildManagerDb' introuvable dans la configuration.");

        services.AddDbContext<GuildManagerDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention()); // tables/colonnes en snake_case pour PostgreSQL

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
