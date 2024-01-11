using LoginShared;
using Microsoft.EntityFrameworkCore;

namespace LoginService
{
    public class UserService(AppDbContext context)
    {
        public async Task<User> CreateUserAsync(User newUser)
        {
            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            return newUser;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await context.Users.SingleAsync(u => u.Email == email);
        }

        public async Task<User> UpdateUserPasswordAsync(int userId, User updatedUser)
        {
            await GetUserByIdIfExistAsync(userId);
            context.Users.Update(updatedUser);
            await context.SaveChangesAsync();
            return updatedUser;
        }

        public async Task<Boolean> DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdIfExistAsync(userId);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<User> UpdateUserAsync(int userId, User updatedUser)
        {
            if (userId != updatedUser.Id)
            {
                throw new ArgumentException($"User ID {userId} does not match user ID {updatedUser.Id}.");
            }


            await GetUserByIdIfExistAsync(userId);

            context.Users.Update(updatedUser);
            await context.SaveChangesAsync();
            return updatedUser;
        }

        public async Task<User> GetUserByIdIfExistAsync(int userId)
        {
            try
            {
                return await context.Users.SingleAsync(u => u.Id == userId);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }
        }
    }
}