using System.Net;
using System.Net.Http.Json;
using LoginShared;
using LoginShared.Security.DTOs;

namespace LoginBlazor2.Security.Services;

public class UserService
{
    private readonly HttpClient httpClient;

    // It is recommended to inject HttpClient via dependency injection
    public UserService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<GetUserDto> CreateUser(CreateUserDto newUserDto)
    {
        var response = await httpClient.PostAsJsonAsync("http://localhost:5001/users", newUserDto);
        var userReponse = await response.Content.ReadFromJsonAsync<GetUserDto>();
        return userReponse!;
    }

    public async Task<List<GetUserDto>> GetUsers()
    {
        var users = await httpClient.GetFromJsonAsync<List<GetUserDto>>("http://localhost:5001/users");
        Console.WriteLine(users);
        return users!;
    }

    public async Task<GetUserDto?> GetUserById(string userId)
    {
        var userResponse = await httpClient.GetFromJsonAsync<GetUserDto>($"http://localhost:5001/users/{userId}");
        return userResponse!;
    }

    public async Task<GetUserDto> UpdateUser(UpdateUserDto user)
    {
        var response = await httpClient.PutAsJsonAsync($"http://localhost:5001/users/{user.Id}", user);
        var updateUser = await response.Content.ReadFromJsonAsync<GetUserDto>();
        return updateUser!;
    }
    
    public async Task<List<GetUserDto>> GetUsersByIds(List<int> userIds)
    {
        var users = new List<GetUserDto>();
        foreach(var id in userIds)
        {
            var user = await GetUserById(id.ToString());
            if(user != null)
            {
                users.Add(user);
            }
        }
        return users;
    }
}

