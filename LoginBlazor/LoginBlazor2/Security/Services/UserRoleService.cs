namespace LoginBlazor2.Security.Services;

using System.Net.Http.Json;
using LoginShared;

public class UserRoleService
{
    private readonly HttpClient httpClient;

    // Declarando listas para guardar los roles y usuarios
    private readonly List<Role> roleDb;
    private readonly List<User> userDb;

    public UserRoleService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public UserRoleService(List<Role> roleDb, List<User> userDb)
    {
        // Guardando en las listas los roles y usuarios pasados como argumentos
        this.roleDb = roleDb;
        this.userDb = userDb;
    }


    public async Task<UserRole> CreateUserRole(UserRole newUserRole)
    {
        var response = await httpClient.PostAsJsonAsync("http://localhost:5001/userRoles", newUserRole);
        var userRole = await response.Content.ReadFromJsonAsync<UserRole>();
        return userRole!;
    }

    public async Task<List<UserRole>> GetUserRoles()
    {
        var userRoles = await httpClient.GetFromJsonAsync<List<UserRole>>("http://localhost:5001/userRoles");
        return userRoles;
    }

    public async Task<UserRole?> GetUserRole(int userId, int roleId)
    {
        var userRole =
            await httpClient.GetFromJsonAsync<UserRole>($"http://localhost:5001/userRoles/{userId}/{roleId}");
        return userRole;
    }

    public async Task<UserRole> UpdateUserRole(int userId, int roleId, UserRole updatedUserRole)
    {
        var response =
            await httpClient.PutAsJsonAsync($"http://localhost:5001/userRoles/{userId}/{roleId}", updatedUserRole);
        var updateUserRole = await response.Content.ReadFromJsonAsync<UserRole>();
        return updateUserRole;
    }

    public async Task<List<UserRole>> GetUserRolesByUserId(int userId)
    {
        var userRoles = await httpClient.GetFromJsonAsync<List<UserRole>>($"http://localhost:5001/userRoles/{userId}");
        return userRoles;
    }

    public async Task<UserRole> AddUserRole(int userId, int roleId)
    {
        var newUserRole = new UserRole(userId, roleId);
        var response = await httpClient.PostAsJsonAsync("http://localhost:5001/userRoles", newUserRole);
        var userRole = await response.Content.ReadFromJsonAsync<UserRole>();
        Console.WriteLine($"\nRole with id: {roleId} has been assigned to user with id: {userId}.");
        return userRole!;
    }

    public Task DeleteUserRole(int userId, int roleId)
    {
        // TODO: Implement this method
        throw new NotImplementedException();
    }
}