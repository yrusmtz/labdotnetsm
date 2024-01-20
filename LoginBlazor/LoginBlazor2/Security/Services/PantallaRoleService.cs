using System.Net.Http.Json;
using LoginShared.Security.DTOs;

namespace LoginBlazor2.Security.Services;

public class PantallaRoleService
{
    private readonly HttpClient httpClient;

    public PantallaRoleService(HttpClient httpClient)
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
    
    //asignar pantalla a rol
    public async Task AddPantallaRole(int pantallaId, int roleId)
    {
        var assignPantallaToRoleDto = new AssignPantallaToRoleDto(pantallaId, roleId);
        var response = await httpClient.PostAsJsonAsync($"/roles/{roleId}/pantallas", assignPantallaToRoleDto);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to add role {roleId} to pantalla {pantallaId}");
        }
    }
}