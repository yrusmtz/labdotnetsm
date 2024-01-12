namespace LoginShared.Security.DTOs;

public record UpdateRoleDto(
        int Id,
        int Code,
        string Description,
        bool State
);
