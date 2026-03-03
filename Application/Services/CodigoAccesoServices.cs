using System.Globalization;
using System.Security.Cryptography;
using Auths.Application.DTOs.CodigoAcceso.Request;
using Auths.Application.DTOs.CodigoAcceso.Response;
using Auths.Application.Interfaz;
using Auths.Application.DTOs.Login.Response;
using Auths.Domain.Entities;
using Auths.Domain.IRepository;

namespace Auths.Application.Services
{
    public class CodigoAccesoServices : ICodigoAccesoServices
    {
        private readonly ICodigoAccesoRepository _codigoAccesoRepository;
        private readonly IJwtService _jwtService;

        public CodigoAccesoServices(
            ICodigoAccesoRepository codigoAccesoRepository,
            IJwtService jwtService)
        {
            _codigoAccesoRepository = codigoAccesoRepository;
            _jwtService = jwtService;
        }

        public async Task<GenerarCodigoAccesoResponseDto> GenerarCodigoAsync(GenerarCodigoAccesoRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.IdentificadorUsuario))
                throw new UnauthorizedAccessException("El identificador del usuario es requerido.");

            var usuario = await _codigoAccesoRepository.ObtenerUsuarioPorIdentificadorAsync(dto.IdentificadorUsuario);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado.");

            var expiracionMinutos = dto.ExpiracionMinutos <= 0 ? 5 : dto.ExpiracionMinutos;
            var now = DateTime.UtcNow;
            var ttl = TimeSpan.FromMinutes(expiracionMinutos);
            var codigoAcceso = new CodigoAcceso();
            var creado = false;
            const int maxIntentos = 10;

            for (var i = 0; i < maxIntentos && !creado; i++)
            {
                codigoAcceso = new CodigoAcceso
                {
                    UsuarioId = usuario.Id,
                    Codigo = GenerarCodigoAleatorio(),
                    Username = usuario.Username,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    FamilyName = usuario.FamilyName,
                    PictureUrl = usuario.pictureUrl,
                    FechaExpiracionUtc = now.Add(ttl)
                };

                creado = await _codigoAccesoRepository.GuardarCodigoAccesoAsync(codigoAcceso, ttl);
            }

            if (!creado)
                throw new Exception("No fue posible generar un codigo de acceso unico.");

            return new GenerarCodigoAccesoResponseDto
            {
                UsuarioId = usuario.Id,
                Codigo = codigoAcceso.Codigo,
                FechaExpiracionUtc = codigoAcceso.FechaExpiracionUtc
            };
        }

        public async Task<LoginResponseDto> CanjearCodigoAsync(CanjearCodigoAccesoRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                throw new UnauthorizedAccessException("El codigo es requerido.");

            var payload = await _codigoAccesoRepository.CanjearCodigoAccesoAsync(dto.Codigo.Trim());
            if (payload == null)
                throw new UnauthorizedAccessException("Codigo invalido, expirado o ya canjeado.");

            var user = new Usuario
            {
                Id = payload.UsuarioId,
                Username = payload.Username,
                Email = payload.Email,
                Nombre = payload.Nombre,
                FamilyName = payload.FamilyName,
                pictureUrl = payload.PictureUrl
            };

            return new LoginResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Nombre = user.Nombre,
                FamilyName = user.FamilyName,
                pictureUrl = user.pictureUrl,
                Token = _jwtService.CrearToken(user)
            };
        }

        private static string GenerarCodigoAleatorio()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000)
                .ToString(CultureInfo.InvariantCulture);
        }
    }
}
