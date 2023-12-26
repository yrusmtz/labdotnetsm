using System.Net;
using System.Net.Http.Json;
using LoginShared;

namespace LoginBlazor2.Services;

public class UserService
{
    private readonly HttpClient httpClient;

    // It is recommended to inject HttpClient via dependency injection
    public UserService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<User> CreateUser(User newUser)
    {
        var response = await httpClient.PostAsJsonAsync("http://localhost:5001/users", newUser);
        var user = await response.Content.ReadFromJsonAsync<User>();
        return user!;
    }

    public async Task<List<User>> GetUsers()
    {
        var users = await httpClient.GetFromJsonAsync<List<User>>("http://localhost:5001/users");
        Console.WriteLine(users);
        return users;
    }
}