using Auths.Application.DTOs.AuthsGoogle.Request;
using Auths.Application.DTOs.Login.Response;

namespace Auths.Application.Interfaz
{
    public interface IGoogleAuthService
    {
        Task<LoginResponseDto> AuthenticateWithGoogleAsync(GoogleAuthsRequestDto token);
    }
}
