using System.ComponentModel.DataAnnotations;

namespace LoginShared.Security.Entities;

public class PatrocinadorRoleEntity
{
    [Key]
    public int Id { get; set; }
    
    public int PatrocinadorId { get; set; }
    public PatrocinadorEntity? Patrocinador { get; set; }

    public int RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    
    public PatrocinadorRoleEntity(int id, int patrocinadorId, int roleId)
    {
        Id = id;
        PatrocinadorId = patrocinadorId;
        RoleId = roleId;
    }
    
    public static PatrocinadorRoleEntity CreateNewPatrocinadorRole(int patrocinadorId, int roleId)
    {
        return new PatrocinadorRoleEntity(0, patrocinadorId, roleId);
    }
}