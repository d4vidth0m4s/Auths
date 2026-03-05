using System.Security.Cryptography;
using System.Text;
using Auths.Application.Configuration;
using Microsoft.Extensions.Options;

namespace Auths.Extensions
{
    public static class InternalSecretValidationExtensions
    {
        public static IApplicationBuilder UseInternalSecretValidation(this IApplicationBuilder app)
        {
            var internalSecret = app.ApplicationServices
                .GetRequiredService<IOptions<InternalSecurityOptions>>()
                .Value.InternalSecret;

            if (string.IsNullOrWhiteSpace(internalSecret))
                throw new InvalidOperationException("InternalSecret no configurado");

            return app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.TryGetValue("X-Internal-Secret", out var receivedSecret))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Header de seguridad ausente" });
                    return;
                }

                if (!CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(receivedSecret.ToString()),
                        Encoding.UTF8.GetBytes(internalSecret)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "Secreto invalido" });
                    return;
                }

                await next();
            });
        }

        public static IApplicationBuilder UseHeaderInjection(this IApplicationBuilder app)
        {
            return app.UseInternalSecretValidation();
        }
    }
}
