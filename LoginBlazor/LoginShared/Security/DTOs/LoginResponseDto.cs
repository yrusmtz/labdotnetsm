using System.Text.Json.Serialization;

namespace LoginShared.Security.DTOs;

public class LoginResponseDto
{
    [JsonPropertyName("token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
}