using System.Text.Json.Serialization;

namespace LoginShared.Security.DTOs;

public class ApiErrorResponseDto
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errors")]
    public string[]? Errors { get; set; }
}
