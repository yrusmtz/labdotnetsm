using LoginShared;

namespace LoginService;

// TODO: Posteriormente pasara a usar repositorio y aceptar injeccion de dependencias
public class UserService(List<UserDto> users)
{
    private readonly List<UserDto> _users = users;

    public UserDto CreateUser(UserDto newUser)
    {
        _users.Add(newUser);
        return newUser;
    }

    public List<UserDto> GetAllUsers()
    {
        return _users;
    }

    public UserDto? GetUser(string userId)
    {
        return _users.Find(u => u.Email == userId);
    }

    public UserDto? UpdateUserPassword(string userId, UserDto updatedUser)
    {
        var user = _users.Find(u => u.Email == userId);
        if (user != null)
        {
            user = updatedUser;
            return user;
        }
        return null;

    }
}
