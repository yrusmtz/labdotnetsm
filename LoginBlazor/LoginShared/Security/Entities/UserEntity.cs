using LoginShared.Security.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LoginShared.Security.Entities;

public class UserEntity
{
    [Key]
    public int Id { get; init; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Department { get; set; }
    public string Puesto { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [PasswordPropertyText]
    public string Password { get; set; }
    
    public UserEntity(int id, string name, string lastName, string department, string puesto, string email, string password)
    {
        Id = id;
        Name = name;
        LastName = lastName;
        Department = department;
        Puesto = puesto;
        Email = email;
        Password = password;
    }
    public static UserEntity CreateNewUser(string name, string lastName, string department, string puesto, string email, string password)
    {
        return new UserEntity(0, name, lastName, department, puesto, email, password);
    }

}
