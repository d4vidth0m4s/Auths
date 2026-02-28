using Auths.Application.DTOs.CrearUsuario;

namespace Auths.Application.Interfaz
{
    public interface IUsuarioServices
    {
        Task<long> CreateAsync(CrearUsuarioRequestDto dto);
        Task<bool> UpdateAsync(long id, CrearUsuarioRequestDto dto);
    }
}
