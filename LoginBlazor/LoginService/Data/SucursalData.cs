using LoginShared.Security.Entities;
namespace LoginService.Data;
internal class SucursalData
{
    // Instancias de PatrocinadorEntity
    public static readonly SucursalEntity sucursal1 = new SucursalEntity
    {
        Id = 1,
        Codigo = 1,
        Descripcion = "Sucursal 1",
    };
    
    public static readonly SucursalEntity sucursal2 = new SucursalEntity
    {
        Id = 2,
        Codigo = 2,
        Descripcion = "Sucursal 2",
        
    };

// Lista de todas las instancias
    public static readonly List<SucursalEntity> sucursales = new List<SucursalEntity>
        { sucursal1, sucursal2};
}