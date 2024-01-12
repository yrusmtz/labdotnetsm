namespace LoginShared;

public record User(
        int Id,
        string Name,
        string LastName,
        string Department,
        string Email,
        string Password,
        string Puesto
  
)
{
   
    // public object Username { get; set; }
    
    public int Id { get; init; }
    public string Name { get; init; }
    public string LastName { get; init; }
    public string Department { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public List<Role>   Roles { get; init; }
    
    public string Code => Email;
    public string FullName => $"{Name} {LastName}";
    

};
