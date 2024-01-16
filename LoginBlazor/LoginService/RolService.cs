using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoginService
{
    public class RoleService(AppDbContext context)
    {

        public async Task<GetRoleDto> CreateRoleAsync(CreateRoleDto newRoleDto)
        {
            var newRole = RoleEntity.CreateNewRole(
                    newRoleDto.Code,
                    newRoleDto.Description,
                    newRoleDto.State);
            context.Roles.Add(newRole);
            await context.SaveChangesAsync();
            var getRoleDto = new GetRoleDto
            (
                    newRole.Id,
                    newRole.Code,
                    newRole.Description,
                    newRole.State
            );
            return getRoleDto;
        }

        public async Task<List<GetRoleDto>> GetAllRolesAsync()
        {
            var roles = await context.Roles.ToListAsync();
            return roles.Select(r => new GetRoleDto
            (
                    r.Id,
                    r.Code,
                    r.Description,
                    r.State
            )).ToList();
        }
        
        public async Task<GetRoleDto> GetRoleByIdAsync(int id)
        {
            var role = await context.Roles.SingleAsync(r => r.Id == id);
            return new GetRoleDto
            (
                    role.Id,
                    role.Code,
                    role.Description,
                    role.State
            );
        }
        
        public async Task<GetRoleDto> GetRoleByCodeAsync(int code)
        {
            var role = await context.Roles.SingleAsync(r => r.Code == code);
            return new GetRoleDto
            (
                    role.Id,
                    role.Code,
                    role.Description,
                    role.State
            );
        }

        public async Task<RoleEntity> GetRoleIfExistAsync(int roleId)
        {
            return await context.Roles.SingleAsync(r => r.Id == roleId);
        }

        public async Task<GetRoleDto> UpdateRoleAsync(int roleId, UpdateRoleDto updatedRole)
        {
            var role = await GetRoleIfExistAsync(roleId);
            if (role.Id != updatedRole.Id)
            {
                throw new ArgumentException($"Role ID {roleId} does not match role ID {updatedRole.Id}.");
            }
            role.Code = updatedRole.Code;
            role.Description = updatedRole.Description;
            role.State = updatedRole.State;
            
            
            context.Roles.Update(role);
            await context.SaveChangesAsync();
            var getRoleDto = new GetRoleDto
            (
                    role.Id,
                    role.Code,
                    role.Description,
                    role.State
            );
            return getRoleDto;
        }
    }
}
