using System.Linq;
using LoginShared;
using Microsoft.EntityFrameworkCore;

namespace LoginService
{
    public class UserRoleService
    {
        private readonly AppDbContext _context;

        public UserRoleService(AppDbContext context) => _context = context;

        public async Task<bool> AddUserRoleAsync(int userId, int roleId)
        {
            if (await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId))
                throw new ArgumentException($"User with ID {userId} already has a role with ID {roleId}");

            _context.UserRoles.Add(new UserRole(userId, roleId));
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserRoleAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles.SingleOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole == null)
                throw new ArgumentException($"No UserRole found for User ID {userId} and Role ID {roleId}");

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Role> GetUserRoleAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles.Include(ur => ur.Role).SingleOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole == null)
                throw new ArgumentException($"No UserRole found for User ID {userId} and Role ID {roleId}");

            return userRole.Role;
        }

        public async Task<List<Role>> GetUserRolesByUserIdAsync(int userId)
        {
            var userRoles = await _context.UserRoles.Include(ur => ur.Role).Where(ur => ur.UserId == userId).ToListAsync();
            return userRoles.Select(ur => ur.Role).ToList();
        }

       
    }
}