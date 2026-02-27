using Auths.Application.Interfaz;
using Auths.Domain.Entities;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Configuration;

namespace Auths.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Secret Key no configurada");

            _issuer = configuration["Jwt:Issuer"] ?? "ControlGastosAPI";
            _audience = configuration["Jwt:Audience"] ?? "ControlGastosClients";
            _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        public string CrearToken(Usuario usuario, int? comercio_Id = null)
        {
            var expiration = DateTimeOffset.UtcNow
                .AddMinutes(_expirationMinutes)
                .ToUnixTimeSeconds();

            var roles = new List<string> { "user" };

            if (comercio_Id != null)
                roles.Add("userComercio");

            var token = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secretKey)
                .AddClaim("iss", _issuer)
                .AddClaim("aud", _audience)
                .AddClaim("exp", expiration)
                .AddClaim("sub", usuario.Id)
                .AddClaim("jti", Guid.NewGuid().ToString())
                .AddClaim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .AddClaim("user_id", usuario.Id)
                .AddClaim("username", usuario.Username)
                .AddClaim("email", usuario.Email)
                .AddClaim("roles", roles)
                 
                .Encode();

            return token;
        }
    }
}
