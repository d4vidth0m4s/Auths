using Auths.Application.Interfaz;
using Auths.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Auths.Application.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginServices, LoginServices>();
            services.AddScoped <IJwtService, JwtService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthsService>();
            services.AddScoped<IUsuarioServices, UsuarioServices>();
            return services;
        }
    }
}
