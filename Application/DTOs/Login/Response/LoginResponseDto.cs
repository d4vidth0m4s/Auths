
using System.ComponentModel.DataAnnotations;


namespace Auths.Application.DTOs.Login.Response
{
    public class LoginResponseDto

    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;

        public string? ComercioId { get; set; }
        public string pictureUrl { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;


    }


    public class ValidateTokenDto
    {
        public bool ValToken { get; set; }
        public string Mensaje { get; set; } = null!;
    }

}
