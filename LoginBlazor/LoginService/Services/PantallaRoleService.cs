using System.Linq;
using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
using Microsoft.EntityFrameworkCore;


namespace LoginService;

public class PantallaRoleService
{
    private readonly AppDbContext _context;

    public PantallaRoleService(AppDbContext context) => _context = context;

    public async Task<bool> AddPantallaRoleAsync(int pantallaId, int roleId)
    {
        if (await _context.PantallaRoles.AnyAsync(ur => ur.PantallaId == pantallaId && ur.RoleId == roleId))
            throw new ArgumentException($"Pantalla with ID {pantallaId} already has a role with ID {roleId}");

        var newPantallaRolerel = PantallaRoleEntity.CreateNewPantallaRole(pantallaId, roleId);
        _context.PantallaRoles.Add(newPantallaRolerel);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePantallaRoleAsync(int pantallaId, int roleId)
    {
        var pantallaRole =
            await _context.PantallaRoles.SingleOrDefaultAsync(ur => ur.PantallaId == pantallaId && ur.RoleId == roleId);
        if (pantallaRole == null)
            throw new ArgumentException($"No PantallaRole found for Pantalla ID {pantallaId} and Role ID {roleId}");

        _context.PantallaRoles.Remove(pantallaRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<GetRoleDto> GetPantallaRoleAsync(int pantallaId, int roleId)
    {
        var pantallaRole = await _context.PantallaRoles.Include(ur => ur.Role)
            .SingleOrDefaultAsync(ur => ur.PantallaId == pantallaId && ur.RoleId == roleId);
        if (pantallaRole == null)
            throw new ArgumentException($"No PantallaRole found for Pantalla ID {pantallaId} and Role ID {roleId}");

        var getRoleDto = new GetRoleDto
        (
            pantallaRole.Role.Id,
            pantallaRole.Role.Code,
            pantallaRole.Role.Description,
            pantallaRole.Role.State
        );
        return getRoleDto;
    }

    public async Task<List<GetRoleDto>> GetPantallaRolesByPantallaIdAsync(int pantallaId)
    {
        var pantallaRoles = await _context.PantallaRoles.Include(ur => ur.Role).Where(ur => ur.PantallaId == pantallaId)
            .ToListAsync();
        return pantallaRoles.Select(ur => new GetRoleDto
        (
            ur.Role.Id,
            ur.Role.Code,
            ur.Role.Description,
            ur.Role.State
        )).ToList();
    }
}