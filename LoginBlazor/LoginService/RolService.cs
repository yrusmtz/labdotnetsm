using LoginShared;

namespace LoginService
{
    public class RoleService
    {
        private readonly List<Role> _roles;

        public RoleService(List<Role> roles)
        {
            _roles = roles;
        }

        public Role CreateRole(Role newRole)
        {
            var role = _roles.FirstOrDefault(r => r.Id == newRole.Id);
            if (role == null)
            {
                _roles.Add(newRole);
                return newRole;
            }

            return null;
        }

        public List<Role> GetAllRoles()
        {
            return _roles;
        }

        public Role? GetRoleById(int id)
        {
            return _roles.Find(r => r.Id == id);
        }
        

        public Role? DeleteRole(int id)
        {
            var role = GetRoleById(id);
            if (role != null)
            {
                _roles.Remove(role);
                return role;
            }

            return null;
        }

        public Role UpdateRole(int id, Role updatedRole)
        {
            // Encontrar el índice del rol que quieres actualizar
            var index = _roles.FindIndex(r => r.Id == id);

            // Verificar si el rol existe o no
            if (index == -1)
            {
                throw new KeyNotFoundException($"Role with Id {id} was not found.");
            }

            // Actualizar el rol
            _roles[index] = updatedRole with { Id = id };
            return _roles[index];
        }
        
    }
}