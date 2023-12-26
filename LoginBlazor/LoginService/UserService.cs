using LoginShared;

namespace LoginService;

// TODO: Posteriormente pasara a usar repositorio y aceptar injeccion de dependencias
public class UserService(List<User> users)
{
    private readonly List<User> _users = users;

    public User CreateUser(User newUser)
    {
        _users.Add(newUser);
        return newUser;
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }

    public User? GetUser(string userId)
    {
        return _users.Find(u => u.Email == userId);
    }

    public User? UpdateUserPassword(string userId, User updatedUser)
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
    public User? DeleteUser(string userId)
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