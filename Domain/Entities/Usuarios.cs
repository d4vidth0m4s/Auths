using System.ComponentModel.DataAnnotations;

namespace Auths.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }


        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;


        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string FamilyName { get; set; } = string.Empty;


        public string pictureUrl { get; set; } = string.Empty;
        public string locale { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }


      
    }

}
