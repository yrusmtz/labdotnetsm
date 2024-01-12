using System.ComponentModel.DataAnnotations;

namespace LoginShared.Security.Entities;

public class UserRoleEntity
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public UserEntity? User { get; set; }

    public int RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    
    public UserRoleEntity(int id, int userId, int roleId)
    {
        Id = id;
        UserId = userId;
        RoleId = roleId;
    }
    
    public static UserRoleEntity CreateNewUserRole(int userId, int roleId)
    {
        return new UserRoleEntity(0, userId, roleId);
    }
}
