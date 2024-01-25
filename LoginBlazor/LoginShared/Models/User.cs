using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LoginShared.Models;

public class User
{
    public User( string name,
        string lastName,
        string department,
        string email,
        string password,
        string puesto
        // List<Role>? roles = null
        )
    {
        Name = name;
        LastName = lastName;
        Department = department;
        Email = email;
        Password = password;
        Puesto = puesto;
        // Roles = roles;
    }

    [Key]
    public int Id { get; init; }
    public string Name { get; init; }
    public string LastName { get; init; }
    public string Department { get; init; }
    public string Email { get; init; }
    public string? Password { get; init; }
    public string Puesto { get; init; }
    // public List<Role>? Roles { get; init; }

    [JsonIgnore]
    public string Code => Email;
    [JsonIgnore]
    public string FullName => $"{Name} {LastName}";
};
