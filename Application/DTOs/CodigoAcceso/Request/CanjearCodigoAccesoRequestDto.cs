using System.ComponentModel.DataAnnotations;

namespace Auths.Application.DTOs.CodigoAcceso.Request
{
    public class CanjearCodigoAccesoRequestDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;
    }
}
