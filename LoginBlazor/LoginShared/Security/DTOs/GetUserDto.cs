namespace LoginShared.Security.DTOs;

// Por favor, ten en cuenta que necesitarás agregar una propiedad Roles a tu GetUserDto para almacenar los roles de cada usuario.


public record GetUserDto(
        int Id,
        string Name,
        string LastName,
        string Department,
        string Email,
        string Puesto
        );


