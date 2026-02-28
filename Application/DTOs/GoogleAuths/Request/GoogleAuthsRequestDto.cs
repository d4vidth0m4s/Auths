using System.ComponentModel.DataAnnotations;

namespace Auths.Application.DTOs.AuthsGoogle.Request
{
  public class GoogleAuthsRequestDto
  {
    [Required]
    public string Token { get; set; }
  }
}