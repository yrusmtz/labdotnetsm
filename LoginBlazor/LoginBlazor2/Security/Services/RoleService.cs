using System.Net;
using System.Net.Http.Json;
using LoginShared;

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

        public async Task<Role> CreateRole(Role Role)
        {
            var response = await httpClient.PostAsJsonAsync("http://localhost:5001/roles", Role);
            var role = await response.Content.ReadFromJsonAsync<Role>();
            return role!;
        }

        public async Task<List<Role>> GetRoles()
        {
            var roles = await httpClient.GetFromJsonAsync<List<Role>>("http://localhost:5001/roles");
            Console.WriteLine(roles);
            return roles;
        }

        public async Task<Role?> GetRoleById(String roleId)
        {
            var role = await httpClient.GetFromJsonAsync<Role>($"http://localhost:5001/roles/{roleId}");
            return role;
        }

        public async Task<Role> UpdateRole(Role role)
        {
            var response = await httpClient.PutAsJsonAsync($"http://localhost:5001/roles/{role.Id}", role);
            var Role = await response.Content.ReadFromJsonAsync<Role>();
            return Role;
        }
    }
}