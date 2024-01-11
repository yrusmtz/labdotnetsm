using LoginShared;
using Microsoft.EntityFrameworkCore;

namespace LoginService
{
    public class RoleService(AppDbContext context)
    {

        public async Task<Role> CreateRoleAsync(Role newRole)
        {
            context.Roles.Add(newRole);
            await context.SaveChangesAsync();
            return newRole;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await context.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleIfExistAsync(int roleId)
        {
            return await context.Roles.SingleAsync(r => r.Id == roleId);
        }

        public async Task<Role> UpdateRoleAsync(int roleId, Role updatedRole)
        {
            Role role = await GetRoleIfExistAsync(roleId);
            if (role.Id != updatedRole.Id)
            {
                throw new ArgumentException($"Role ID {roleId} does not match role ID {updatedRole.Id}.");
            }
            context.Roles.Update(updatedRole);
            await context.SaveChangesAsync();
            return updatedRole;
        }
    }
}