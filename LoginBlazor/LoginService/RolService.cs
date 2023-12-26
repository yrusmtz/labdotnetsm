using LoginShared;

namespace LoginService;

public class RoleService
{
    private readonly List<Role> _roles;

    public RoleService(List<Role> roles)
    {
        _roles = roles;
    }

    public Role CreateRole(Role newRole)
    {
        _roles.Add(newRole);
        return newRole;
    }

    public List<Role> GetAllRoles()
    {
        return _roles;
    }

    public Role? GetRole(string roleName)
    {
        return _roles.Find(r => r.Name == roleName);
    }

    public Role? UpdateRole(string roleName, Role updatedRole)
    {
        for (int i = 0; i < _roles.Count; i++)
        {
            if (_roles[i].Name == roleName)
            {
                _roles[i] = updatedRole;
                return _roles[i];
            }
        }

        return null;
    }
}