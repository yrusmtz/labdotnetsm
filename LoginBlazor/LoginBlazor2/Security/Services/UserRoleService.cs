namespace LoginBlazor2.Security.Services;

using System.Net.Http.Json;
using LoginShared;

public class UserRoleService
{
    private readonly HttpClient httpClient;

    public UserRoleService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
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
        return userRole!;
    }
}

    
    