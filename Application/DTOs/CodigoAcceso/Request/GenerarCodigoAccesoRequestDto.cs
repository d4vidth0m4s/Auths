using System.ComponentModel.DataAnnotations;

namespace Auths.Application.DTOs.CodigoAcceso.Request
{
    public class GenerarCodigoAccesoRequestDto
    {
        [Required]
        public string IdentificadorUsuario { get; set; } = string.Empty;
        public int ExpiracionMinutos { get; set; } = 10;
    }
}
