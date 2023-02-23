using Infrastructure;
using WebApi.Configurations;
using WebApi.Interfaces;
using WebApi.Middlewares;
using WebApi.Services;

namespace WebApi;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
        services.AddLogging(logging => { logging.AddConsole(); });
        services.AddTransient<ILogger<Startup>, Logger<Startup>>();

        InfrastructureConfiguration.SetDefaultLanguage(Configuration.GetSection("DefaultLanguage").Value);

        services.AddInfracstruture(Configuration);
        services.AddTransient<IJwtService, JwtService>();

        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = Configuration.GetConnectionString("RedisConnection");
        //     options.InstanceName = Configuration["EnvironmentName"].Replace(" ", "");
        // });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfiguration();
        services.AddHttpContextAccessor();

        services.Configure<IISOptions>(options => { options.ForwardClientCertificate = false; });
        services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("X-SourceFiles");
                return Task.CompletedTask;
            });

            return next();
        });

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.AddSwaggerService();
        }

        app.UseAuthentication();
        app.UseMiddleware<JwtRenewalMiddleware>();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        logger.LogWarning("Application started");
    }
}