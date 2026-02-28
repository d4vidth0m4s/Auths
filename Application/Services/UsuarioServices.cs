using Auths.Application.DTOs.CrearUsuario;
using Auths.Application.Interfaz;
using Auths.Domain.Entities;
using Auths.Domain.IRepository;

namespace Auths.Application.Services
{
    public class UsuarioServices : IUsuarioServices
    {
        private readonly IUsuariosRepository _repository;

        public UsuarioServices(IUsuariosRepository repository)
        {
            _repository = repository;
        }

        public async Task<long> CreateAsync(CrearUsuarioRequestDto dto)
        {
            var usuario = new Usuario
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password,
                Nombre = dto.Nombre,
                FamilyName = dto.FamilyName,
                pictureUrl = string.Empty,
                locale = string.Empty,
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow
            };

            return await _repository.CreateAsync(usuario);
        }

        public async Task<bool> UpdateAsync(long id, CrearUsuarioRequestDto dto)
        {
            var usuario = new Usuario
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password,
                Nombre = dto.Nombre,
                FamilyName = dto.FamilyName,
                FechaModificacion = DateTime.UtcNow
            };

            return await _repository.UpdateAsync(id, usuario);
        }
    }
}
