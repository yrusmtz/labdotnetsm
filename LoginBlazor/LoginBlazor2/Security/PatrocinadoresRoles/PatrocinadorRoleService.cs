using System.Net.Http.Json;
using LoginShared.Security.DTOs;


namespace LoginBlazor2.Security.Services;

public class PatrocinadorRoleService
{
    private readonly HttpClient httpClient;
    
    public PatrocinadorRoleService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("http://localhost:5001/api");
    }
    
    public async Task<List<GetPatrocinadorDto>> GetPatrocinadores()
    {
        var patrocinadores = await httpClient.GetFromJsonAsync<List<GetPatrocinadorDto>>("/patrocinadores");
        Console.WriteLine(patrocinadores);
        return patrocinadores!;
    }
    
    //asignar patrocinador a rol
    public async Task AddPatrocinadorRole(int patrocinadorId, int roleId)
    {
        var assignPatrocinadorToRoleDto = new AssignPatrocinadorToRoleDto(patrocinadorId, roleId);
        var response = await httpClient.PostAsJsonAsync($"/roles/{roleId}/patrocinadores", assignPatrocinadorToRoleDto);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to add role {roleId} to patrocinador {patrocinadorId}");
        }
    }
}