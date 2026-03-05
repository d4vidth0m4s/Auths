using System.Globalization;
using System.Security.Cryptography;
using Auths.Application.Configuration;
using Auths.Application.DTOs.CodigoAcceso.Request;
using Auths.Application.DTOs.CodigoAcceso.Response;
using Auths.Application.DTOs.Login.Response;
using Auths.Application.Interfaz;
using Auths.Domain.Entities;
using Auths.Domain.IRepository;
using Comercios.Grpc;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Auths.Application.Services
{
    public class CodigoAccesoServices : ICodigoAccesoServices
    {
        private readonly ICodigoAccesoRepository _codigoAccesoRepository;
        private readonly IJwtService _jwtService;
        private readonly ComerciosService.ComerciosServiceClient _comerciosClient;
        private readonly string _comerciosInternalSecret;

        public CodigoAccesoServices(
            ICodigoAccesoRepository codigoAccesoRepository,
            IJwtService jwtService,
            ComerciosService.ComerciosServiceClient comerciosClient,
            IOptions<ComerciosGrpcSecurityOptions> comerciosGrpcSecurityOptions)
        {
            _codigoAccesoRepository = codigoAccesoRepository;
            _jwtService = jwtService;
            _comerciosClient = comerciosClient;
            _comerciosInternalSecret = comerciosGrpcSecurityOptions.Value.InternalSecret;
        }

        public async Task<GenerarCodigoAccesoResponseDto> GenerarCodigoAsync(GenerarCodigoAccesoRequestDto dto, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new UnauthorizedAccessException("El identificador del usuario es requerido.");

            var usuario = await _codigoAccesoRepository.ObtenerUsuarioPorIdentificadorAsync(username);
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

            var usuario = new Usuario
            {
                Id = payload.UsuarioId,
                Username = payload.Username,
                Email = payload.Email,
                Nombre = payload.Nombre,
                FamilyName = payload.FamilyName,
                pictureUrl = payload.PictureUrl
            };

            var usuarioIdentificador = string.IsNullOrWhiteSpace(payload.Username)
                ? (!string.IsNullOrWhiteSpace(payload.Email)
                    ? payload.Email
                    : payload.UsuarioId.ToString(CultureInfo.InvariantCulture))
                : payload.Username;

            var headers = new Metadata
            {
                { "x-internal-secret", _comerciosInternalSecret },
                { "x-user-id", usuarioIdentificador }
            };

            var comercioResponse = await _comerciosClient.ObtenerComercioPorUsuarioAsync(
                new ObtenerComercioPorUsuarioRequest { UsuarioId = usuarioIdentificador },
                headers: headers);

            ComercioLoginDto? comercio = null;
            if (comercioResponse.Encontrado && !string.IsNullOrWhiteSpace(comercioResponse.ComercioId))
            {
                comercio = new ComercioLoginDto
                {
                    ComercioId = comercioResponse.ComercioId,
                    Nombre = comercioResponse.Nombre,
                    Descripcion = comercioResponse.Descripcion,
                    Abierto = comercioResponse.Abierto,
                    Calificacion = comercioResponse.Calificacion,
                    Categorias = comercioResponse.Categorias.ToList(),
                    ImgBannerUrl = comercioResponse.ImgBannerUrl,
                    Direccion = comercioResponse.Direccion,
                    Ciudad = comercioResponse.Ciudad,
                    Telefono = comercioResponse.Telefono
                };
            }

            return new LoginResponseDto
            {
                Id = usuario.Id,
                Username = usuario.Username,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                FamilyName = usuario.FamilyName,
                pictureUrl = usuario.pictureUrl,
                ComercioId = comercio?.ComercioId,
                Comercio = comercio,
                Token = _jwtService.CrearToken(usuario, comercio is not null)
            };
        }

        private static string GenerarCodigoAleatorio()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000)
                .ToString(CultureInfo.InvariantCulture);
        }
    }
}
