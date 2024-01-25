namespace LoginShared.Security.DTOs;

public record UpdatePatrocinadorDto(
    int Id,
    int Codigo,
    string Descripcion
    );