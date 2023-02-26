using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Services;
using Infrastructure.Data.Contexts;
using Infrastructure.Data.Repositories;
using Infrastructure.EventHandlers;
using Npgsql;

namespace Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfracstruture(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SqlContext>(
        options => options.UseNpgsql(
            configuration.GetConnectionString("ConnSql"), b => b.MigrationsAssembly(typeof(SqlContext).Assembly.FullName))
        );

        services.AddScoped<SqlContext>();

        services.Scan(scan => scan
            .FromAssemblyOf<SqlContext>()
            .AddClasses(classes => classes.AssignableTo(typeof(Repository<>))
                .Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>))))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.Scan(scan => scan
            .FromAssemblyOf<Entity>()
            .AddClasses(classes => classes.AssignableTo(typeof(Service<>))
                .Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IService<>))))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(SqlContext).Assembly)
        );

        services.AddScoped<IDomainEventHandler, DomainEventHandler>();

        return services;
    }
    
    public static void SetDefaultLanguage(string language)
    {
        CultureInfo cultureInfo = new(language);

        switch (language)
        {
            case "pt-BR":
            default:
                cultureInfo.NumberFormat.CurrencySymbol = "R$";
                cultureInfo.NumberFormat.CurrencyGroupSeparator = ",";

                break;
            case "en-US":
                cultureInfo.NumberFormat.CurrencySymbol = "$";
                cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";

                break;
        }

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}
