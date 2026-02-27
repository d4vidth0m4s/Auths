using Auths.Domain.Entities;

namespace Auths.Application.Interfaz
{
    public interface IJwtService
    {
        string CrearToken(Usuario usuario, int? comercio_id=null);
    }
}
