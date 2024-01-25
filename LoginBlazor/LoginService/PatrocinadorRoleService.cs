using System.Linq;
using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
using Microsoft.EntityFrameworkCore;
namespace LoginService;

public class PatrocinadorRoleService
{
    private readonly AppDbContext _context;

    public PatrocinadorRoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddPatrocinadorRoleAsync(int patrocinadorId, int roleId)
    {
        if (await _context.PatrocinadorRoles.AnyAsync(pr => pr.PatrocinadorId == patrocinadorId && pr.RoleId == roleId))
            throw new ArgumentException($"Patrocinador with ID {patrocinadorId} already has a role with ID {roleId}");

        var newPatrocinadorRole = PatrocinadorRoleEntity.CreateNewPatrocinadorRole(patrocinadorId, roleId);
        _context.PatrocinadorRoles.Add(newPatrocinadorRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePatrocinadorRoleAsync(int patrocinadorId, int roleId)
    {
        var patrocinadorRole =
            await _context.PatrocinadorRoles.SingleOrDefaultAsync(pr => pr.PatrocinadorId == patrocinadorId && pr.RoleId == roleId);
        if (patrocinadorRole == null)
            throw new ArgumentException($"No PatrocinadorRole found for Patrocinador ID {patrocinadorId} and Role ID {roleId}");

        _context.PatrocinadorRoles.Remove(patrocinadorRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<GetRoleDto> GetPatrocinadorRoleAsync(int patrocinadorId, int roleId)
    {
        var patrocinadorRole = await _context.PatrocinadorRoles.Include(pr => pr.Role)
            .SingleOrDefaultAsync(pr => pr.PatrocinadorId == patrocinadorId && pr.RoleId == roleId);
        if (patrocinadorRole == null)
            throw new ArgumentException($"No PatrocinadorRole found for Patrocinador ID {patrocinadorId} and Role ID {roleId}");

        var getRoleDto = new GetRoleDto
        (
            patrocinadorRole.Role.Id,
            patrocinadorRole.Role.Code,
            patrocinadorRole.Role.Description,
            patrocinadorRole.Role.State
        );
        return getRoleDto;
    }

    public async Task<List<GetRoleDto>> GetPatrocinadorRolesByPatrocinadorIdAsync(int patrocinadorId)
    {
        var patrocinadorRoles = await _context.PatrocinadorRoles.Include(pr => pr.Role)
                                       .Where(pr => pr.PatrocinadorId == patrocinadorId).ToListAsync();
        return patrocinadorRoles.Select(pr => new GetRoleDto
        (
            pr.Role.Id,
            pr.Role.Code,
            pr.Role.Description,
            pr.Role.State
        )).ToList();
    }
}