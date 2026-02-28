using System.ComponentModel.DataAnnotations;

namespace Auths.Application.DTOs.CrearUsuario
{
    public class CrearUsuarioRequestDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string FamilyName { get; set; } = string.Empty;

    }
}
