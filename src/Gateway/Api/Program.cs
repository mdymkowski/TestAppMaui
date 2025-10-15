using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TestAppMaui.Gateway.Application.Common.Interfaces;
using TestAppMaui.Gateway.Api.Endpoints;
using TestAppMaui.Gateway.Application;
using TestAppMaui.Gateway.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TestAppMaui Gateway",
        Version = "v1"
    });
});

builder.Services.AddGatewayApplication();
var connectionString = builder.Configuration.GetConnectionString("GatewayDatabase")
    ?? throw new InvalidOperationException("Connection string 'GatewayDatabase' is not configured.");

var crmSection = builder.Configuration.GetSection("Crm");
var crmConfigured = !string.IsNullOrWhiteSpace(crmSection.GetValue<string>("BaseUrl"));

builder.Services.AddGatewayInfrastructure(options =>
{
    options.UseSqlServer(connectionString);

    if (crmConfigured)
    {
        options.ConfigureCrm(crm => crmSection.Bind(crm));
    }
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTaskEndpoints();

if (crmConfigured)
{
    app.MapCrmTaskEndpoints();
}

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IAppDatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.Run();
