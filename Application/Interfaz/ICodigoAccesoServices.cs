using Auths.Application.DTOs.CodigoAcceso.Request;
using Auths.Application.DTOs.CodigoAcceso.Response;
using Auths.Application.DTOs.Login.Response;

namespace Auths.Application.Interfaz
{
    public interface ICodigoAccesoServices
    {
        Task<GenerarCodigoAccesoResponseDto> GenerarCodigoAsync(GenerarCodigoAccesoRequestDto dto);
        Task<LoginResponseDto> CanjearCodigoAsync(CanjearCodigoAccesoRequestDto dto);
    }
}
