using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LoginShared.Security.DTOs;

public class LoginRequestDto
{

    [Required]
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(16, MinimumLength = 5)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
