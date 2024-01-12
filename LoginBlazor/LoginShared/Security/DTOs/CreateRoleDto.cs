namespace LoginShared.Security.DTOs;

public record CreateRoleDto(
        int Code,
        string Description,
        bool State
);
