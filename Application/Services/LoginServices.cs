using Auths.Application.Interfaz;
using Auths.Domain.IRepository;
using Auths.Application.DTOs.Login.Response;

namespace Auths.Application.Services
{
    public class LoginServices : ILoginServices
    {
        private readonly IloginRepository _repositoryLogin;
        private readonly IJwtService _JwtService;

        public LoginServices(IloginRepository repositoryLogin, IJwtService jwtService )
        {
            _repositoryLogin = repositoryLogin;
            _JwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(string username, string password)
        {
            var valid = await _repositoryLogin.ValidarCredencialesAsync(username, password);

            if (!valid)
                throw new Exception("Credenciales inválidas");

            var user = await _repositoryLogin.ObtenerPorUsernameAsync(username);

            return new LoginResponseDto
            {
                Id = user!.Id,
                Username = user.Username,
                Email = user.Email,
                Nombre = user.Nombre,
                FamilyName = user.FamilyName,
                pictureUrl = user.pictureUrl,
                Token = _JwtService.CrearToken(user)
            };
        }
    }
}
