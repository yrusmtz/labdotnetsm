namespace LoginShared;

public record User(
        int Id,
        string Name,
        string LastName,
        string Department,
        string Email,
        string Password,
        string Title,
        List<Role> Roles
)
{
    public string Code => Email;
    public string FullName => $"{Name} {LastName}";
};
