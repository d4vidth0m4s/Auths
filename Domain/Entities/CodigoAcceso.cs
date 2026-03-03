namespace Auths.Domain.Entities
{
    public class CodigoAcceso
    {
        public long UsuarioId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public DateTime FechaExpiracionUtc { get; set; }
    }
}
