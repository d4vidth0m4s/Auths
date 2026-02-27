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

        public AuthController(ILoginServices loginService)
        {
            _loginService = loginService;
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
