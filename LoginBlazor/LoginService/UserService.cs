using LoginShared;
using Microsoft.EntityFrameworkCore;

namespace LoginService
{
    // TODO: Posteriormente pasara a usar repositorio y aceptar injeccion de dependencias
    public class UserService(AppDbContext context)
    {
        public async Task<User> CreateUserAsync(User newUser)
        {
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();
            return newUser;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(int userId, string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> updateUserAsync(int userId, User updatedUser)
        {
            await GetUserByIdfExistAsync(userId);
            context.Users.Update(updatedUser);
            await context.SaveChangesAsync();
            return updatedUser;
        }

        public async Task<User> DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdfExistAsync(userId);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(int userId, User updatedUser)
        {
            if (userId != updatedUser.Id)
            {
                throw new AggregateException($"User ID {userId} does not match user ID {updatedUser.Id}");
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
                throw new ArgumentException($"User with ID {userId} does not exist", e);
            }
        }


        public async Task<User> GetUserByEmailAsync(int userId,string email)
        
        {
             return await context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Email == email);
        }
        
    }
}