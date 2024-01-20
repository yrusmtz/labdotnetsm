using LoginShared;
using LoginShared.Security.DTOs;
using Microsoft.EntityFrameworkCore;



using LoginShared.Security.DTOs;

namespace LoginService;

public class PantallaService(AppDbContext context)

{
    public async Task<List<GetPantallaDto>> GetAllPantallasAsync()
    {
        var pantallas = await context.Pantallas.ToListAsync();
        return pantallas.Select(p => new GetPantallaDto
        (
            p.Id,
            p.Codigo,
            p.Descripcion,
            p.Estado
        )).ToList();
    }
}