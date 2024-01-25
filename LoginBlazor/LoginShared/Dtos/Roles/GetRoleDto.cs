namespace LoginShared.Dtos.Roles;

public record GetRoleDto(
        int Id,
        int Code,
        string Description,
        bool State
);
