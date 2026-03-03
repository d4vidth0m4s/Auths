namespace Auths.Application.DTOs.CodigoAcceso.Response
{
    public class GenerarCodigoAccesoResponseDto
    {
        public long UsuarioId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public DateTime FechaExpiracionUtc { get; set; }
    }
}
