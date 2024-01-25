namespace LoginShared.Dtos.Roles;

public record UpdateRoleDto(
        int Id,
        int Code,
        string Description,
        bool State
);
