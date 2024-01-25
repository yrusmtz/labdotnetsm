namespace LoginShared.Dtos.Roles;

public record RoleDto(
        int Id,
        int Code,
        string Description,
        bool State
        );
