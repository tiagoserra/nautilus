using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Infrastructure.Data.Contexts;
using Infrastructure.EventHandlers;
using Npgsql;
using SqlKata.Execution;

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

        services.AddSingleton(provider => new QueryFactory
        {
            Connection = new NpgsqlConnection(configuration.GetConnectionString("ConnSql"))
            //Logger = compiled => Console.WriteLine(compiled)
        });

        services.Scan(scan => scan
            .FromAssemblyOf<Entity>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.Scan(scan => scan
            .FromAssemblyOf<Entity>()
            .AddClasses(classes => classes.AssignableTo(typeof(IService<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(SqlContext).Assembly)
        );

        services.AddScoped<IDomainEventHandler, DomainEventHandler>();

        return services;
    }
}