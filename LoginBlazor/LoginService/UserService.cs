using LoginShared;

namespace LoginService
{
    // TODO: Posteriormente pasara a usar repositorio y aceptar injeccion de dependencias
    public class UserService
    {
        private readonly List<User> _users;

        public UserService(List<User> users)
        {
            _users = users;
        }

        public User CreateUser(User newUser)
        {
            newUser = newUser with { Id = _users.Count + 1 };
            _users.Add(newUser);
            return newUser;
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public User? GetUserById(int userId)
        {
            return _users.Find(u => u.Id == userId);
        }        
        public User? GetUserByEmail(string email)
        {
            return _users.Find(u => u.Email == email);
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

        //add role to user
        public Role? AddRoleToUser(string userId, Role newRole)
        {
            var user = _users.Find(u => u.Email == userId);
            if (user != null)
            {
                user.Roles.Add(newRole);
                return newRole;
            }

            return null;
        }

        //remove role from user
        public Role? RemoveRoleFromUser(string userId, string roleName)
        {
            var user = _users.Find(u => u.Email == userId);
            if (user != null)
            {
                var role = user.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role != null)
                {
                    user.Roles.Remove(role);
                    return role;
                }
            }

            return null;
        }
    }
}
