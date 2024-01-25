using LoginShared.Models;

namespace LoginShared.Dtos.Users;

public record UpdateUserDto(
        string Name,
        string LastName,
        string Department,
        string Email,
        string Puesto,
        int Id,
        string Password,
        List<Role> Roles);
