using Auths.Application.DTOs.Login.Response;

namespace Auths.Application.Interfaz
{
    public interface ILoginServices
    {
        Task <LoginResponseDto> LoginAsync(string username, string password);
    }
}
