using Auths.Extensions;
using Auths.Application.IoC;

using Auths.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);
var internalSecret = builder.Configuration["InternalSecret"];

if (string.IsNullOrWhiteSpace(internalSecret))
    internalSecret = builder.Configuration["INTERNAL_SECRET"];

if (string.IsNullOrWhiteSpace(internalSecret))
    internalSecret = builder.Configuration["expectedSecret"];

if (string.IsNullOrWhiteSpace(internalSecret))
    throw new InvalidOperationException(
        "InternalSecret no configurado. Define 'InternalSecret' en appsettings o la variable de entorno 'INTERNAL_SECRET'.");

builder.Services.AddEndpointsApiExplorer();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddServices();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();


app.UseHeaderInjection(internalSecret);
app.UseAuthorization();
app.MapControllers();


app.Run();
