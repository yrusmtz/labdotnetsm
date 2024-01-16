namespace LoginShared.Security.DTOs;

public record UpdateUserDto(
    int Id,
    string Name,
    string LastName,
    string Department,
    string Email,
    // string Password,
    string Puesto);