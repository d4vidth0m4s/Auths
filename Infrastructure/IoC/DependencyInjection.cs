using Auths.Domain.IRepository;
using Auths.Infrastructure.Data;
using Auths.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Auths.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")));

            var redisConnection = configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisConnection))
                redisConnection = "localhost:6379";

            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var redisOptions = ConfigurationOptions.Parse(redisConnection);
                redisOptions.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(redisOptions);
            });

            services.AddScoped<IloginRepository, LoginRepository>();
            services.AddScoped<IUsuariosRepository, UsuariosRepository>();
            services.AddScoped<ICodigoAccesoRepository, CodigoAccesoRepository>();

            return services;
        }
    }
}
