using Auths.Extensions;
using Auths.Application.IoC;

using Auths.Infrastructure.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddServices();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();


app.UseHeaderInjection(internalSecret);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
