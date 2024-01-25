using LoginShared;
using LoginShared.Security.DTOs;
using Microsoft.EntityFrameworkCore;



namespace LoginService;

public class SucursalService(AppDbContext context)
{
    
    
    //GetAllSucursalesAsync
    public async Task<List<GetSucursalDto>> GetAllSucursalesAsync()
    {
        var sucursales = await context.Sucursales.ToListAsync();
        return sucursales.Select(s => new GetSucursalDto
        (
            s.Id,
            s.Codigo,
            s.Descripcion
        )).ToList();
    }
    
    //GetSucursalByIdAsync
    public async Task<GetSucursalDto> GetSucursalByIdAsync(int id)
    {
        var sucursal = await context.Sucursales.FindAsync(id);
        return new GetSucursalDto
        (
            sucursal.Id,
            sucursal.Codigo,
            sucursal.Descripcion
        );
    }
}