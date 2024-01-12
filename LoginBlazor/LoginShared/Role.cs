namespace LoginShared;

public record Role(
    int Id,  // tipo string para el id.
    int Code,  // tipo int para el código.
    string Description,
    bool State  // un tipo bool para el estado.
)
{
    public int Id { get; init; }
    public int Code { get; init; }
    public string Description { get; init; }
    public bool State { get; init; }
   
};