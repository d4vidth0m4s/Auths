using Auths.Domain.Entities;
using Auths.Domain.IRepository;
using Auths.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auths.Infrastructure.Repositories
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly UsersDbContext _context;

        public UsuariosRepository(UsersDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateAsync(Usuario entity)
        {
            var existsByUsername = await _context.Usuarios
                .AnyAsync(u => u.Username.ToLower() == entity.Username.ToLower());

            if (existsByUsername)
                throw new UnauthorizedAccessException("El username ya existe");

            var existsByEmail = await _context.Usuarios
                .AnyAsync(u => u.Email.ToLower() == entity.Email.ToLower());

            if (existsByEmail)
                throw new UnauthorizedAccessException("El email ya existe");

            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(long id, Usuario entity)
        {
            var existing = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo);

            if (existing == null)
                return false;

            var usernameTaken = await _context.Usuarios
                .AnyAsync(u => u.Id != id && u.Username.ToLower() == entity.Username.ToLower());

            if (usernameTaken)
                throw new UnauthorizedAccessException("El username ya existe");

            var emailTaken = await _context.Usuarios
                .AnyAsync(u => u.Id != id && u.Email.ToLower() == entity.Email.ToLower());

            if (emailTaken)
                throw new UnauthorizedAccessException("El email ya existe");

            existing.Username = entity.Username;
            existing.Email = entity.Email;
            existing.PasswordHash = entity.PasswordHash;
            existing.Nombre = entity.Nombre;
            existing.FamilyName = entity.FamilyName;
            existing.FechaModificacion = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
