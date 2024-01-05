namespace LoginShared;

public record UserDto
{
    public UserDto(int id,
            string name,
            string lastName,
            string department,
            string email,
            string password,
            string puesto,
            List<Role> roles)
    {
        Id = id;
        Name = name;
        LastName = lastName;
        Department = department;
        Email = email;
        Password = password;
        Puesto = puesto;
        Roles = roles;
    }
    public int Id { get; init; }
    public string Name { get; init; }
    public string LastName { get; init; }
    public string Department { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string Puesto { get; init; }
    public List<Role> Roles { get; init; }
};
