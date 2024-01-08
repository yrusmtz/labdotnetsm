namespace LoginShared;

public record User(
        int Id,
        string Name,
        string LastName,
        string Department,
        string Email,
        string Password,
        string Puesto,
        List<Role> Roles
)
{
    public string Code => Email;
    public string FullName => $"{Name} {LastName}";
    public object Username { get; set; }
};
