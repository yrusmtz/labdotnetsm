using System.Net.Http.Json;
using LoginShared.Security.DTOs;

namespace LoginBlazor2.Security.Services;

public class RolePantallaService
{
    private readonly HttpClient httpClient;

    public RolePantallaService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("http://localhost:5001/api");
    }
    
    public async Task<List<GetPantallaDto>> GetPantallas()
    {
        var pantallas = await httpClient.GetFromJsonAsync<List<GetPantallaDto>>("/pantallas");
        Console.WriteLine(pantallas);
        return pantallas!;
    }
}