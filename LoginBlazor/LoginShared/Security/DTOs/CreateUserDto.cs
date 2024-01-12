namespace LoginShared.Security.DTOs;

public record CreateUserDto(
        string Name,
        string LastName,
        string Department,
        string Email,
        string Password,
        string Puesto
);
