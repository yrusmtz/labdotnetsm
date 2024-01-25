using System.Linq;
using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoginService;

public class SucursalRoleService
{
    private readonly AppDbContext _context;
    
    public SucursalRoleService(AppDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<bool> AddSucursalRoleAsync(int sucursalId, int roleId)
    {
        if (await _context.SucursalRoles.AnyAsync(ur => ur.SucursalId == sucursalId && ur.RoleId == roleId))
            throw new ArgumentException($"Sucursal with ID {sucursalId} already has a role with ID {roleId}");

        var newSucursalRolerel = SucursalRoleEntity.CreateNewSucursalRole(sucursalId, roleId);
        _context.SucursalRoles.Add(newSucursalRolerel);
        await _context.SaveChangesAsync();
        return true;
    }
    
    
    
    public async Task<bool> DeleteSucursalRoleAsync(int sucursalId, int roleId)
    {
        var sucursalRole =
            await _context.SucursalRoles.SingleOrDefaultAsync(ur => ur.SucursalId == sucursalId && ur.RoleId == roleId);
        if (sucursalRole == null)
            throw new ArgumentException($"No SucursalRole found for Sucursal ID {sucursalId} and Role ID {roleId}");

        _context.SucursalRoles.Remove(sucursalRole);
        await _context.SaveChangesAsync();
        return true;
    }
    
   
    public async Task<GetRoleDto> GetSucursalRoleAsync(int sucursalId, int roleId)
    {
        var sucursalRole = await _context.SucursalRoles.Include(ur => ur.Role)
            .SingleOrDefaultAsync(ur => ur.SucursalId == sucursalId && ur.RoleId == roleId);
        if (sucursalRole == null)
            throw new ArgumentException($"No SucursalRole found for Sucursal ID {sucursalId} and Role ID {roleId}");

        var getRoleDto = new GetRoleDto
        (
            sucursalRole.Role.Id,
            sucursalRole.Role.Code,
            sucursalRole.Role.Description,
            sucursalRole.Role.State
        );
        return getRoleDto;
    }
    
    
    public async Task<List<GetRoleDto>> GetSucursalRolesBySucursalIdAsync(int sucursalId)
    {
        var sucursalRoles = await _context.SucursalRoles.Include(ur => ur.Role).Where(ur => ur.SucursalId == sucursalId)
            .ToListAsync();
        return sucursalRoles.Select(ur => new GetRoleDto
        (
            ur.Role.Id,
            ur.Role.Code,
            ur.Role.Description,
            ur.Role.State
        )).ToList();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
}
