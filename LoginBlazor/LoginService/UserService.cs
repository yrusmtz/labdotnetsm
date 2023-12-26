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
        for (int i = 0; i < _users.Count; i++)
        {
            if (_users[i].Email == userId)
            {
                _users[i] = updatedUser;
                return _users[i];
            }
        }

        return null;
    }

    //delete user
    public UserDto? DeleteUser(string userId)
    {
        var user = _users.Find(u => u.Email == userId);
        if (user != null)
        {
            _users.Remove(user);
            return user;
        }

        return null;
    }
}