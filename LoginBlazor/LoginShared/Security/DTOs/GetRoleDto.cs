namespace LoginShared.Security.DTOs;

public record GetRoleDto(
    int Id,
    int Code,
    string Description,
    bool State
);