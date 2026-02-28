using Auths.Application.DTOs.CrearUsuario;
using Auths.Application.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Auths.Controllers
{
    [ApiController]
    [Route("Auths")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioServices _usuarioServices;

        public UsuariosController(IUsuarioServices usuarioServices)
        {
            _usuarioServices = usuarioServices;
        }

        [HttpPost("CrearUsuario")]
        public async Task<ActionResult<long>> Create([FromBody] CrearUsuarioRequestDto dto)
        {
            try
            {
                var id = await _usuarioServices.CreateAsync(dto);
                return StatusCode(201, id);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("ActualizarUsuario/{id:long}")]
        public async Task<ActionResult> Update([FromRoute] long id, [FromBody] CrearUsuarioRequestDto dto)
        {
            try
            {
                var updated = await _usuarioServices.UpdateAsync(id, dto);
                return updated ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
