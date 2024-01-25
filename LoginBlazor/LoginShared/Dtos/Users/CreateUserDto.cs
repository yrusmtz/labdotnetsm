using LoginShared.Models;

namespace LoginShared.Dtos.Users;

public record CreateUserDto(
        string Name,
        string LastName,
        string Department,
        string Email,
        string Puesto,
        string Password,
        List<Role> Roles);
