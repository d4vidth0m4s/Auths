using Auths.Domain.Entities;
 namespace Auths.Domain.IRepository
{
    public interface IloginRepository
    {
        Task<Usuario?> ObtenerPorUsernameAsync(string username);
        Task<bool> ValidarCredencialesAsync(string username, string password);
    }
}
