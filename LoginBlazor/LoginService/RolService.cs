using LoginShared;
using Microsoft.EntityFrameworkCore;

namespace LoginService
{
    public class RoleService(AppDbContext context)
    {
        public async Task<Role> CreateRoleAsync(Role newRole)
        {
            await context.Roles.AddAsync(newRole);
            await context.SaveChangesAsync();
            return newRole;
            
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await context.Roles.ToListAsync();
        }

        public async Task<Role?> GetRoleIfExistAsync(int roleId)
        {
            return await context.Roles.SingleAsync(r => r.Id == roleId);
        }

        public async Task<Role> UpdateRoleAsync(int roleId, Role updatedRole)
        {
            if (roleId != updatedRole.Id)
            {
                throw new AggregateException($"Role ID {roleId} does not match role ID {updatedRole.Id}");
            }

            await GetRoleIfExistAsync(roleId);
            context.Roles.Update(updatedRole);
            await context.SaveChangesAsync();
            return updatedRole;
        }

        public async Task<Role> DeleteRoleAsync(int roleId)
        {
            var role = await GetRoleIfExistAsync(roleId);
            context.Roles.Remove(role);
            await context.SaveChangesAsync();
            return role;
        }
    }
}