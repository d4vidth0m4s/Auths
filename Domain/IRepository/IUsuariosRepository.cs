using Auths.Domain.Entities;

namespace Auths.Domain.IRepository
{
    public interface IUsuariosRepository
    {
        Task<long> CreateAsync(Usuario entity);
        Task<bool> UpdateAsync(long id, Usuario entity);
    }
}
