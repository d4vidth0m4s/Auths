using Auths.Application.DTOs.AuthsGoogle.Request;
using Auths.Application.DTOs.Login.Response;
using Auths.Application.Interfaz;
using Auths.Domain.Entities;
using Auths.Infrastructure.Data;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace Auths.Application.Services
{
    public class GoogleAuthsService : IGoogleAuthService
    {
        private readonly UsersDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public GoogleAuthsService(
            UsersDbContext context,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> AuthenticateWithGoogleAsync(GoogleAuthsRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                throw new UnauthorizedAccessException("Token de Google requerido.");

            var googleClientId = _configuration["Google:ClientId"];
            if (string.IsNullOrWhiteSpace(googleClientId))
                throw new InvalidOperationException("Google:ClientId no configurado.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    request.Token,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { googleClientId }
                    });
            }
            catch (InvalidJwtException)
            {
                throw new UnauthorizedAccessException("Token de Google invalido.");
            }

            var email = payload.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedAccessException("El token de Google no incluye email valido.");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email || u.Username == email);

            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Username = email,
                    Email = email,
                    Nombre = payload.GivenName ?? payload.Name ?? "Usuario",
                    FamilyName = payload.FamilyName ?? string.Empty,
                    pictureUrl = payload.Picture ?? string.Empty,
                    locale = payload.Locale ?? string.Empty,
                    PasswordHash = Guid.NewGuid().ToString("N"),
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuario);
            }
            else
            {
                usuario.Email = email;
                usuario.Username = email;
                usuario.Nombre = payload.GivenName ?? payload.Name ?? usuario.Nombre;
                usuario.FamilyName = payload.FamilyName ?? usuario.FamilyName;
                usuario.pictureUrl = payload.Picture ?? usuario.pictureUrl;
                usuario.locale = payload.Locale ?? usuario.locale;
                usuario.Activo = true;
                usuario.FechaModificacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Id = usuario.Id,
                Username = usuario.Username,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                FamilyName = usuario.FamilyName,
                pictureUrl = usuario.pictureUrl,
                Token = _jwtService.CrearToken(usuario)
            };
        }
    }
}
