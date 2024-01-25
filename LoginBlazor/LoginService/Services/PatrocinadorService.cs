using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
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
    
    //CreatePatrocinadorAsync
        public async Task<GetPatrocinadorDto> CreatePatrocinadorAsync(CreatePatrocinadorDto newPatrocinadorDto)
        {
            PatrocinadorEntity? newPatrocinador = new()
            {
                Codigo = newPatrocinadorDto.Codigo,
                Descripcion = newPatrocinadorDto.Descripcion,
            };

            context.Patrocinadores.Add(newPatrocinador);
            await context.SaveChangesAsync();

            return new GetPatrocinadorDto (
                newPatrocinador.Id,
                newPatrocinador.Codigo,
                newPatrocinador.Descripcion
            );
        }
        
        //UpdatePatrocinadorAsync
        public async Task<GetPatrocinadorDto> UpdatePatrocinadorAsync(int patrocinadorId,
            UpdatePatrocinadorDto updatePatrocinadorDto)
        {
            var patrocinador = await context.Patrocinadores.FindAsync(updatePatrocinadorDto.Id);
            patrocinador.Codigo = updatePatrocinadorDto.Codigo;
            patrocinador.Descripcion = updatePatrocinadorDto.Descripcion;
            await context.SaveChangesAsync();
            return new GetPatrocinadorDto
            (
                patrocinador.Id,
                patrocinador.Codigo,
                patrocinador.Descripcion
            );
        }
        
        //DeletePatrocinadorAsync
        public async Task<bool> DeletePatrocinadorAsync(int id)
        {
            PatrocinadorEntity? patrocinadorToDelete = await context.Patrocinadores.FindAsync(id);

            if (patrocinadorToDelete == null)
            {
                return false;
            }

            context.Patrocinadores.Remove(patrocinadorToDelete);
            await context.SaveChangesAsync();

            return true;
        }
    
    
}