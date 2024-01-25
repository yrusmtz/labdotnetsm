using LoginShared.Models;

namespace LoginShared.Dtos.Users;

public record GetUserDto(
        string Name,
        string LastName,
        string Department,
        string Email,
        string Puesto,
        int Id,
        List<Role> Roles);
