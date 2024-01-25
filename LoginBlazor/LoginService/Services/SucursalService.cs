using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
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
    
    //CreateSucursalAsync
    public async Task<GetSucursalDto> CreateSucursalAsync(CreateSucursalDto newSucursalDto)
    {
        SucursalEntity? newSucursal = new()
        {
            Codigo = newSucursalDto.Codigo,
            Descripcion = newSucursalDto.Descripcion,
        };

        context.Sucursales.Add(newSucursal);
        await context.SaveChangesAsync();

        return new GetSucursalDto (
            newSucursal.Id,
            newSucursal.Codigo,
            newSucursal.Descripcion
        );
    }
    
    
    //UpdateSucursalAsync
    
    public async Task<GetSucursalDto> UpdateSucursalAsync(int sucursalId,
        UpdateSucursalDto updateSucursalDto)
    {
        var sucursal = await context.Sucursales.FindAsync(updateSucursalDto.Id);
        sucursal.Codigo = updateSucursalDto.Codigo;
        sucursal.Descripcion = updateSucursalDto.Descripcion;
        await context.SaveChangesAsync();
        return new GetSucursalDto
        (
            sucursal.Id,
            sucursal.Codigo,
            sucursal.Descripcion
        );
    }
    
    
    //DeleteSucursalAsync
    public async Task<bool> DeleteSucursalAsync(int id)
    {
        var sucursal = await context.Sucursales.FindAsync(id);
        context.Sucursales.Remove(sucursal);
        await context.SaveChangesAsync();
        return true;
    }
}