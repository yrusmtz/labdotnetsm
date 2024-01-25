namespace LoginShared.Dtos.Users;

public abstract record UserDto(
        string Name,
        string LastName,
        string Department,
        string Email,
        string Puesto);
