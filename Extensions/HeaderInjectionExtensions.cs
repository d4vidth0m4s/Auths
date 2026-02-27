using System.Security.Cryptography;
using System.Text;

namespace Auths.Extensions
{
    public static class HeaderInjectionExtensions
    {
        public static IApplicationBuilder UseHeaderInjection(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var config = context.RequestServices.GetRequiredService<IConfiguration>();
                var expectedSecret = config["expectedSecret"]
                                     ?? throw new InvalidOperationException("InternalSecret no configurado");

                if (!context.Request.Headers.TryGetValue("X-Internal-Secret", out var receivedSecret))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Header de seguridad ausente" });
                    return;
                }

                if (!CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(receivedSecret.ToString()),
                        Encoding.UTF8.GetBytes(expectedSecret)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "Secreto inválido" });
                    return;
                }

                await next();
            });
        }
    }
}
