using System.Reflection;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Infrastructure.Data.Contexts;
using Infrastructure.Data.Extensions;
using Infrastructure.EventHandlers;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Execution;

namespace Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfracstruture(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SqlServerContext>(
        options => options.UseSqlServer(
            configuration.GetConnectionString("Conn"), b => b.MigrationsAssembly(typeof(SqlServerContext).Assembly.FullName))
        );
        
        services.AddScoped<SqlServerContext>();
        
        services.AddSingleton(provider => new QueryFactory
        {
            Compiler = new NoLockCompilerExtensions(),
            Connection = new SqlConnection(configuration.GetConnectionString("Conn"))
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
        
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddScoped<IDomainEventHandler, DomainEventHandler>();
        
        return services;
    }
}