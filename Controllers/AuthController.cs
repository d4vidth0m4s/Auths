using Auths.Application.DTOs.AuthsGoogle.Request;
using Auths.Application.DTOs.CodigoAcceso.Request;
using Auths.Application.DTOs.Login.Resquest;
using Auths.Application.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Auths.Controllers
{
    [ApiController]
    [Route("Auths")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginServices _loginService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ICodigoAccesoServices _codigoAccesoServices;

        public AuthController(
            ILoginServices loginService,
            IGoogleAuthService googleAuthService,
            ICodigoAccesoServices codigoAccesoServices)
        {
            _loginService = loginService;
            _googleAuthService = googleAuthService;
            _codigoAccesoServices = codigoAccesoServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _loginService.LoginAsync(
                    request.Username,
                    request.Password
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthsRequestDto request)
        {
            try
            {
                var result = await _googleAuthService.AuthenticateWithGoogleAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("generar-codigo-acceso")]
        public async Task<IActionResult> GenerarCodigoAcceso([FromBody] GenerarCodigoAccesoRequestDto request)
        {
            try
            {
                var result = await _codigoAccesoServices.GenerarCodigoAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("canjear-codigo-acceso")]
        public async Task<IActionResult> CanjearCodigoAcceso([FromBody] CanjearCodigoAccesoRequestDto request)
        {
            try
            {
                var result = await _codigoAccesoServices.CanjearCodigoAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("ok")]
        public IActionResult Health()
        {
            return Ok(new
            {
                service = "Auths",
                status = "OK"
            });
        }
    }
}
