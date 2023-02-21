using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();

var startup = new Startup(builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

var logger = app.Services.GetService<ILogger<Startup>>();

startup.Configure(app, builder.Environment, logger);

app.Run();