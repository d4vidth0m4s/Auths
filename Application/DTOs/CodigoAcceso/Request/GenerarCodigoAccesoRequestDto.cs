using System.ComponentModel.DataAnnotations;

namespace Auths.Application.DTOs.CodigoAcceso.Request
{
    public class GenerarCodigoAccesoRequestDto
    {
        [Required]
        public int ExpiracionMinutos { get; set; } = 10;
    }
}
