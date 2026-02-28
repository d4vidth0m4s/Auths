using Auths.Domain.Entities;
using Auths.Domain.IRepository;
using Auths.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auths.Infrastructure.Repositories
{
    public class LoginRepository : IloginRepository
    {
        private readonly UsersDbContext _context;

        public LoginRepository(UsersDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorUsernameAsync(string username)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username && u.Activo);
        }

        public async Task<bool> ValidarCredencialesAsync(string username, string password)
        {
            var user = await ObtenerPorUsernameAsync(username);

            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            // ⚠ Aquí deberías validar hash real (BCrypt por ejemplo)
            if (user.PasswordHash != password)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            return true;
        }
    }
}
