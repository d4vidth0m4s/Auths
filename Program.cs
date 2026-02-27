using Auths.Extensions;
using Auths.Application.IoC;

using Auths.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddServices();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();


app.UseAuthorization();
app.UseHeaderInjection();
app.MapControllers();


app.Run();
