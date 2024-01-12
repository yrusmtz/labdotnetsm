using System.ComponentModel.DataAnnotations;

namespace LoginShared.Security.Entities;

public class RoleEntity
{
    [Key]
    public int Id { get; init; }
    public int Code { get; set; }
    public string Description { get; set; }
    public bool State { get; set; }
    
    public RoleEntity(int id, int code, string description, bool state)
    {
        Id = id;
        Code = code;
        Description = description;
        State = state;
    }
    
    public static RoleEntity CreateNewRole(int code, string description, bool state)
    {
        return new RoleEntity(0, code, description, state);
    }
}
