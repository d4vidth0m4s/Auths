namespace Auths.Application.DTOs.Login.Response
{
    public class LoginResponseDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string? ComercioId { get; set; }
        public ComercioLoginDto? Comercio { get; set; }
        public string pictureUrl { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenDto
    {
        public bool ValToken { get; set; }
        public string Mensaje { get; set; } = null!;
    }

    public class ComercioLoginDto
    {
        public string ComercioId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Abierto { get; set; }
        public double Calificacion { get; set; }
        public List<string> Categorias { get; set; } = new();
        public string ImgBannerUrl { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}
