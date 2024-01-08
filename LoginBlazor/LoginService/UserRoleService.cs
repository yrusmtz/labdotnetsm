using LoginShared;

namespace LoginService;

public class UserRoleService
{
    private readonly List<UserRole> UserRoles = new List<UserRole>();
    private readonly UserService userService;

    public UserRoleService(UserService userService)
    {
        this.userService = userService;
    }

    public bool AssignRoleToUser(int userId, string roleId)
    {
        var user = userService.GetUserById(userId);
        if (user == null)
        {
            throw new Exception($"The user with ID '{userId}' wasn't found");
        }

        var userRole = new UserRole(userId, roleId);
        UserRoles.Add(userRole);
        return true;
    }

    public IEnumerable<string> GetRolesByUserId(int userId)
    {
        return UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId);
    }

    public IEnumerable<int> GetUserIdsByRoleId(string roleId)
    {
        return UserRoles.Where(ur => ur.RoleId == roleId).Select(ur => ur.UserId);
    }
}