namespace LoginShared.Security.DTOs;

public record GetUserDto(
        int Id,
        string Name,
        string LastName,
        string Department,
        string Email,
        string Puesto
        );
