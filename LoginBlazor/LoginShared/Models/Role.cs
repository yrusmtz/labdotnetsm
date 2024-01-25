namespace LoginShared.Models;

public record Role(
    int Id,
    int Code,
    string Description,
    bool State
);
