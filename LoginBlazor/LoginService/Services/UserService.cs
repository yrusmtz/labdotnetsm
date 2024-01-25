using LoginShared;
using LoginShared.Security.DTOs;
using LoginShared.Security.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoginService
{
    public class UserService(AppDbContext context)
    {
        public async Task<GetUserDto> CreateUserAsync(CreateUserDto newUserDto)
        {
            string defaultPassword = "12345678";
            var newUser = UserEntity.CreateNewUser(
                    newUserDto.Name,
                    newUserDto.LastName,
                    newUserDto.Department,
                    newUserDto.Puesto,
                    newUserDto.Email,
                    defaultPassword);
            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            var getUserDto = new GetUserDto
            (
                    newUser.Id,
                    newUser.Name,
                    newUser.LastName,
                    newUser.Department,
                    newUser.Email,
                    newUser.Puesto
            );
            return getUserDto;
        }

        public async Task<List<GetUserDto>> GetAllUsersAsync()
        {
            var users = await context.Users.ToListAsync();
            return users.Select(u => new GetUserDto
            (
                    u.Id,
                    u.Name,
                    u.LastName,
                    u.Department,
                    u.Email,
                    u.Puesto
            )).ToList();
        }

        public async Task<GetUserDto> GetUserByEmailAsync(string email)
        {
            var user = await context.Users.SingleAsync(u => u.Email == email);
            return new GetUserDto
            (
                    user.Id,
                    user.Name,
                    user.LastName,
                    user.Department,
                    user.Email,
                    user.Puesto
            );
            
        }

        public async Task<GetUserDto> UpdateUserPasswordAsync(int userId, User updatedUser)
        {
            var user = await GetUserByIdIfExistAsync(userId);
            user.Password = updatedUser.Password;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            var getUserDto = new GetUserDto
            (
                    user.Id,
                    user.Name,
                    user.LastName,
                    user.Department,
                    user.Email,
                    user.Puesto
            );
            return getUserDto;
        }

        public async Task<Boolean> DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdIfExistAsync(userId);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<GetUserDto> UpdateUserAsync(int userId, UpdateUserDto updatedUser)
        {
            if (userId != updatedUser.Id)
            {
                throw new ArgumentException($"User ID {userId} does not match user ID {updatedUser.Id}.");
            }


            var user = await GetUserByIdIfExistAsync(userId);
            
            // TODO: Update user properties in a more elegant way.
            user.Name = updatedUser.Name;
            user.LastName = updatedUser.LastName;
            user.Department = updatedUser.Department;
            user.Email = updatedUser.Email;
            // user.Password = updatedUser.Password;
            user.Puesto = updatedUser.Puesto;

            context.Users.Update(user);
            await context.SaveChangesAsync();
            var getUserDto = new GetUserDto
            (
                    user.Id,
                    user.Name,
                    user.LastName,
                    user.Department,
                    user.Email,
                    user.Puesto
            );
            return getUserDto;
        }
        
        public async Task<GetUserDto> GetUserByIdAsync(int userId)
        {
            var user = await GetUserByIdIfExistAsync(userId);
            var getUserDto = new GetUserDto
            (
                    user.Id,
                    user.Name,
                    user.LastName,
                    user.Department,
                    user.Email,
                    user.Puesto
            );
            return getUserDto;
        }

        public async Task<UserEntity> GetUserByIdIfExistAsync(int userId)
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
        
        public async Task<UserEntity> TestUserPasswordAsync(string email)
        {
            var user = await context.Users.SingleAsync(u => u.Email == email);
            return user;
        }
    }
}
