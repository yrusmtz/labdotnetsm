using System.Net;
using System.Net.Http.Json;
using LoginShared;
using LoginShared.Security.DTOs;

namespace LoginBlazor2.Security.Services

{
    public class RoleService
    {
        private readonly HttpClient httpClient;

        // It is recommended to inject HttpClient via dependency injection
        public RoleService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<GetRoleDto> CreateRole(CreateRoleDto role )
        {
            var response = await httpClient.PostAsJsonAsync("http://localhost:5001/roles", role);
            var roleResponce = await response.Content.ReadFromJsonAsync<GetRoleDto>();
            return roleResponce!;
        }

        public async Task<List<GetRoleDto>> GetRoles()
        {
            var roles = await httpClient.GetFromJsonAsync<List<GetRoleDto>>("http://localhost:5001/roles");
            Console.WriteLine(roles);
            return roles!;
        }

        public async Task<GetRoleDto?> GetRoleById(String roleId)
        {
            var roleResponse = await httpClient.GetFromJsonAsync<GetRoleDto>($"http://localhost:5001/roles/{roleId}");
            return roleResponse!;
        }

        public async Task<GetRoleDto> UpdateRole(Role role)
        {
            var response = await httpClient.PutAsJsonAsync($"http://localhost:5001/roles/{role.Id}", role);
            var roleResponse = await response.Content.ReadFromJsonAsync<GetRoleDto>();
            return roleResponse!;
        }

        public async Task<List<GetRoleDto>> GetRolesByIds(List<string> roleIds)
        {
            var roles = new List<GetRoleDto>();
            foreach (var id in roleIds)
            {
                var role = await GetRoleById(id);
                if (role != null)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }
    }
}