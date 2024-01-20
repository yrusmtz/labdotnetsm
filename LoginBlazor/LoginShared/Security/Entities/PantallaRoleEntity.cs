using System.ComponentModel.DataAnnotations;
namespace LoginShared.Security.Entities;

public class PantallaRoleEntity
{
    [Key]
    public int Id { get; set; }
    
    public int PantallaId { get; set; }
    public PantallaEntity? Pantalla { get; set; }

    public int RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    
    public PantallaRoleEntity(int id, int pantallaId, int roleId)
    {
        Id = id;
        PantallaId = pantallaId;
        RoleId = roleId;
    }
    
    public static PantallaRoleEntity CreateNewPantallaRole(int pantallaId, int roleId)
    {
        return new PantallaRoleEntity(0, pantallaId, roleId);
    }
}
