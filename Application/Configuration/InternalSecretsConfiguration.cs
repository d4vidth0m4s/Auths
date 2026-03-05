using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auths.Application.Configuration
{
    public static class InternalSecretsConfiguration
    {
        public static IServiceCollection AddInternalSecurityOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var defaultInternalSecret = ResolveFirstNonEmpty(
                configuration["InternalSecret"],
                configuration["INTERNAL_SECRET"],
                configuration["expectedSecret"]);

            var grpcComerciosInternalSecret = ResolveFirstNonEmpty(
                configuration["Grpc:ComerciosInternalSecret"],
                configuration["GRPC_COMERCIOS_INTERNAL_SECRET"],
                defaultInternalSecret);

            services.AddOptions<InternalSecurityOptions>()
                .Configure(options =>
                {
                    options.InternalSecret = defaultInternalSecret ?? string.Empty;
                })
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.InternalSecret),
                    "InternalSecret no configurado. Define 'InternalSecret' en appsettings o la variable de entorno 'INTERNAL_SECRET'.")
                .ValidateOnStart();

            services.AddOptions<ComerciosGrpcSecurityOptions>()
                .Configure(options =>
                {
                    options.InternalSecret = grpcComerciosInternalSecret ?? string.Empty;
                })
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.InternalSecret),
                    "InternalSecret no configurado para llamadas internas gRPC.")
                .ValidateOnStart();

            return services;
        }

        private static string? ResolveFirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return null;
        }
    }
}
