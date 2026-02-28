using Auths.Extensions;
using Auths.Application.IoC;

using Auths.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);
var internalSecret = builder.Configuration["InternalSecret"]
    ?? throw new InvalidOperationException("InternalSecret no configurado en appsettings");

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
