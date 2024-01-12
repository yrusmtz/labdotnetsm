namespace LoginShared.Security.DTOs;

public record AssignRoleToUserDto(
        int UserId,
        int RoleId
);
