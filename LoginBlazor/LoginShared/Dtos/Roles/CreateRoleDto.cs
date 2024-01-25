namespace LoginShared.Dtos.Roles;

public record CreateRoleDto(
        int Code,
        string Description,
        bool State
);
