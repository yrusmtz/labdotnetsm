namespace LoginShared;

public class UserRole
{
    public int UserId { get; set; }
    public string RoleId { get; set; }

    public UserRole(int userId, string roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}