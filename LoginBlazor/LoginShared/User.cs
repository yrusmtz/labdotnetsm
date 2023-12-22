namespace LoginShared;

public record User(
        string Name,
        string Email,
        string Title,
        List<Role> Roles
);
