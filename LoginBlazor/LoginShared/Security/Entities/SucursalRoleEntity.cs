using System.ComponentModel.DataAnnotations;


namespace LoginShared.Security.Entities;

public class SucursalRoleEntity
{
    [Key]
    public int Id { get; set; }
    
    public int SucursalId { get; set; }
    public SucursalEntity? Sucursal { get; set; }

    public int RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    
    public SucursalRoleEntity(int id, int sucursalId, int roleId)
    {
        Id = id;
        SucursalId = sucursalId;
        RoleId = roleId;
    }
    
    public static SucursalRoleEntity CreateNewSucursalRole(int sucursalId, int roleId)
    {
        return new SucursalRoleEntity(0, sucursalId, roleId);
    }
}