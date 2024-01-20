namespace LoginShared.Security.DTOs;

public record GetPantallaDto(
    int Id,
    int Codigo,
    string Descripcion,
    bool Estado
);
