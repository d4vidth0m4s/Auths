using Auths.Domain.Entities;

namespace Auths.Domain.IRepository
{
    public interface ICodigoAccesoRepository
    {
        Task<Usuario?> ObtenerUsuarioPorIdentificadorAsync(string identificador);
        Task<bool> GuardarCodigoAccesoAsync(CodigoAcceso entity, TimeSpan ttl);
        Task<CodigoAcceso?> CanjearCodigoAccesoAsync(string codigo);
    }
}
