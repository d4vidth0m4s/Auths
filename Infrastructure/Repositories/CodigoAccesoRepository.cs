using Auths.Domain.Entities;
using Auths.Domain.IRepository;
using Auths.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace Auths.Infrastructure.Repositories
{
    public class CodigoAccesoRepository : ICodigoAccesoRepository
    {
        private readonly UsersDbContext _context;
        private readonly IConnectionMultiplexer _redis;

        public CodigoAccesoRepository(
            UsersDbContext context,
            IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdentificadorAsync(string identificador)
        {
            var normalized = identificador.Trim().ToLower();

            return await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.Activo &&
                (u.Username.ToLower() == normalized || u.Email.ToLower() == normalized));
        }

        public async Task<bool> GuardarCodigoAccesoAsync(CodigoAcceso entity, TimeSpan ttl)
        {
            var db = _redis.GetDatabase();
            var userKey = GetUserKey(entity.UsuarioId);
            var previousCode = await db.StringGetAsync(userKey);

            if (previousCode.HasValue)
            {
                await db.KeyDeleteAsync(GetCodeKey(previousCode.ToString()));
            }

            var payload = JsonSerializer.Serialize(entity);

            var created = await db.StringSetAsync(
                GetCodeKey(entity.Codigo),
                payload,
                ttl,
                when: When.NotExists);

            if (!created)
                return false;

            await db.StringSetAsync(userKey, entity.Codigo, ttl);
            return true;
        }

        public async Task<CodigoAcceso?> CanjearCodigoAccesoAsync(string codigo)
        {
            var db = _redis.GetDatabase();
            var script = LuaScript.Prepare(
                "local v=redis.call('GET', @k); if not v then return nil end; redis.call('DEL', @k); return v;");

            var result = await db.ScriptEvaluateAsync(script, new { k = (RedisKey)GetCodeKey(codigo) });
            if (result.IsNull)
                return null;

            var payload = JsonSerializer.Deserialize<CodigoAcceso>(result.ToString());
            if (payload == null)
                return null;

            var userKey = GetUserKey(payload.UsuarioId);
            var storedCode = await db.StringGetAsync(userKey);

            if (storedCode.HasValue && storedCode.ToString() == codigo)
                await db.KeyDeleteAsync(userKey);

            return payload;
        }

        private static string GetCodeKey(string code) => $"auths:access-code:{code}";
        private static string GetUserKey(long userId) => $"auths:access-user:{userId}";
    }
}
