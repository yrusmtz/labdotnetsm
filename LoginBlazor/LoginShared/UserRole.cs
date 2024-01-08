
namespace LoginShared
{
    public record UserRole(int UserId, int RoleId)
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
