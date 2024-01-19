using LoginShared.Security.DTOs;

namespace LoginBlazor2.Security.Services;

using System.Net.Http.Json;
using LoginShared;

public class UserRoleService
{
    private readonly HttpClient httpClient;

    public UserRoleService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("http://localhost:5001/api");
    }

    public async Task AddUserRoleAsync(int userId, int roleId)
    {
        var assignRoleToUserDto = new AssignRoleToUserDto(userId, roleId);
        var response = await httpClient.PostAsJsonAsync($"/users/{userId}/roles", assignRoleToUserDto);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to add role {roleId} to user {userId}");
        }
    }
    
    public async Task DeleteUserRoleAsync(int userId, int roleId)
    {
        var response = await httpClient.DeleteAsync($"/users/{userId}/roles/{roleId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to delete role {roleId} from user {userId}");
        }
    }
    
    public async Task<IEnumerable<GetRoleDto>> GetRolesByUserIdAsync(int userId)
    {
        var response = await httpClient.GetAsync($"/users/{userId}/roles");
        if (response.IsSuccessStatusCode)
        {
            var roles = await response.Content.ReadFromJsonAsync<IEnumerable<GetRoleDto>>();
            return roles!;
        }
        else
        {
            throw new Exception($"Failed to retrieve roles for user {userId}");
        }
    }
}
