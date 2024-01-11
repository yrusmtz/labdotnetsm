namespace LoginShared;

public record Role(
    int Id,  // tipo string para el id.
    int Code,  // tipo int para el código.
    string Description,
    bool State  // un tipo bool para el estado.
)
{
    public object rol;
    public List<UserRole> UserRoles { get; set; }
}