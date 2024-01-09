using LoginShared;

namespace LoginService;

public class UserRoleService
{
    //lista de roles
    private readonly List<Role> _roleDb;

    //lista de usuarios
    private readonly List<User> _userDb;

    public UserRoleService(List<User> usersDb, List<Role> rolesDb)
    {
        // Guardando en las listas los roles y usuarios pasados como argumentos
        _roleDb = rolesDb;
        _userDb = usersDb;
    }

    public Task<bool> AddUserRole(int userId, int roleId)
    {
        // Raise no implemented
        throw new NotImplementedException();
    }
    
    public Task<bool> DeleteUserRole(int userId, int roleId)
    {
        // Raise no implemented
        throw new NotImplementedException();
    }
    
    public Task<Role?> GetUserRole(int userId, int roleId)
    {
        // Raise no implemented
        throw new NotImplementedException();
    }
    
    public Task<bool> UpdateUserRole(int userId, int roleId, UserRole updatedUserRole)
    {
        // Raise no implemented
        throw new NotImplementedException();
    }
    
    public Task<List<UserRole>> GetUserRolesByUserId(int userId)
    {
        // Raise no implemented
        throw new NotImplementedException();
    }
}
