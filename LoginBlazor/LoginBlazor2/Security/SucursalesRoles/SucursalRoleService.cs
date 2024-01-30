using System.Net.Http.Json;
using LoginShared.Security.DTOs;

namespace LoginBlazor2.Security.Services;

public class SucursalRoleService
{
    private readonly HttpClient httpClient;

    public SucursalRoleService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("http://localhost:5001/api");
    }

    public async Task<List<GetSucursalDto>> GetSucursales()
    {
        var sucursales = await httpClient.GetFromJsonAsync<List<GetSucursalDto>>("/sucursales");
        Console.WriteLine(sucursales);
        return sucursales!;
    }

    //asignar sucursal a rol
    public async Task AddSucursalRole(int sucursalId, int roleId)
    {
        var assignSucursalToRoleDto = new AssignSucursalToRoleDto(sucursalId, roleId);
        var response = await httpClient.PostAsJsonAsync($"/roles/{roleId}/sucursales", assignSucursalToRoleDto);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to add role {roleId} to sucursal {sucursalId}");
        }
    }
    
    //GetSucursalesByRole
    public async Task<List<GetSucursalDto>> GetSucursalesByRole(int roleId)
    {
        var sucursales = await httpClient.GetFromJsonAsync<List<GetSucursalDto>>($"/roles/{roleId}/sucursales");
        Console.WriteLine(sucursales);
        return sucursales!;
    }
}