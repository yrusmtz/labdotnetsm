using System.Net;
using System.Net.Http.Json;
using LoginShared;

namespace LoginBlazor2.Security.Services

{
public class UserRole
{
    public string UserId { get; set; }
    public string RoleId { get; set; }
    public User User { get; set; }
    public Role Role { get; set; }
}
}

public class UserRoleService
{
    private readonly List<UserRole> userRoles;
    private readonly UserService userService;
    private readonly RoleService roleService;

    public UserRoleService(UserService userService, RoleService roleService)
    {
        this.userService = userService;
        this.roleService = roleService;
        userRoles = new List<UserRole>();
    }

    public async Task AssignRoleToUser(string userId, string roleId)
    {
        var user = await userService.GetUserById(userId);
        var role = await roleService.GetRoleById(roleId);

        if (user != null && role != null)
        {
            userRoles.Add(new UserRole
            {
                UserId = user.Id,
                User = user,
                RoleId = role.Id,
                Role = role
            });
        }
    }

    public List<Role> GetRolesByUserId(string userId)
    {
        return userRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role).ToList();
    }

    public List<User> GetUsersByRoleId(string roleId)
    {
        return userRoles.Where(ur => ur.RoleId == roleId).Select(ur => ur.User).ToList();
    }
}