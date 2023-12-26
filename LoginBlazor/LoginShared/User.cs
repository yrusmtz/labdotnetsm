namespace LoginShared;

public record User(
        string Name,
        string Email,
        string Password,
        string Title,
        List<Role> Roles
);
