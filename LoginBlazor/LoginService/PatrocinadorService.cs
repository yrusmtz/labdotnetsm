using LoginShared;
using LoginShared.Security.DTOs;
using Microsoft.EntityFrameworkCore;
namespace LoginService;

public class PatrocinadorService(AppDbContext context)
{
    public async Task<List<GetPatrocinadorDto>> GetAllPatrocinadoresAsync()
    {
        var patrocinadores = await context.Patrocinadores.ToListAsync();
        return patrocinadores.Select(p => new GetPatrocinadorDto
        (
            p.Id,
            p.Codigo,
            p.Descripcion
        )).ToList();
    }
    
    //GetPatrocinadorByIdAsync
    public async Task<GetPatrocinadorDto> GetPatrocinadorByIdAsync(int id)
    {
        var patrocinador = await context.Patrocinadores.FindAsync(id);
        return new GetPatrocinadorDto
        (
            patrocinador.Id,
            patrocinador.Codigo,
            patrocinador.Descripcion
        );
    }
    
    
    
    
    
}