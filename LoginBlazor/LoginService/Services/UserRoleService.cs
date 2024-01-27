using System.Linq;
using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
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

            var newUserRolerel = UserRoleEntity.CreateNewUserRole(userId, roleId);
            _context.UserRoles.Add(newUserRolerel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserRoleAsync(int userId, int roleId)
        {
            var userRole =
                await _context.UserRoles.SingleOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole == null)
                throw new ArgumentException($"No UserRole found for User ID {userId} and Role ID {roleId}");

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<GetRoleDto> GetUserRoleAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles.Include(ur => ur.Role)
                .SingleOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole == null)
                throw new ArgumentException($"No UserRole found for User ID {userId} and Role ID {roleId}");

            var getRoleDto = new GetRoleDto
            (
                userRole.Role.Id,
                userRole.Role.Code,
                userRole.Role.Description,
                userRole.Role.State
            );
            return getRoleDto;
        }

        public async Task<List<GetRoleDto>> GetUserRolesByUserIdAsync(int userId)
        {
            var userRoles = await _context.UserRoles.Include(ur => ur.Role).Where(ur => ur.UserId == userId)
                .ToListAsync();
            return userRoles.Select(ur => new GetRoleDto
            (
                ur.Role.Id,
                ur.Role.Code,
                ur.Role.Description,
                ur.Role.State
            )).ToList();
        }

      
    }
}