using LoginShared.Security.Entities;

namespace LoginService.Data;

internal class PantallaData
{

// Instancias de PantallaEntity
        public static readonly PantallaEntity  pantalla1 = new PantallaEntity
        {
            Id = 1,
            Codigo = 1,
            Descripcion = "Inicio",
            Estado = true,
            Icono = "fa fa-home",
            HostName = "http://localhost:44389",
        };

        public static readonly PantallaEntity  pantalla2 = new PantallaEntity
        {
            Id = 2,
            Codigo = 2,
            Descripcion = "Seguridad",
            Estado = true,
            Icono = "fa fa-users",
            HostName = "http://localhost:44377",
        };

        public static readonly PantallaEntity  pantalla3 = new PantallaEntity
        {
            Id = 3,
            Codigo = 3,
            Descripcion = "Usuario",
            Estado = true,
        };

        public static readonly PantallaEntity  pantalla4 = new PantallaEntity
        {
            Id = 4,
            Codigo = 4,
            Descripcion = "Roles",
            Estado = true,
        };

        public static readonly PantallaEntity  pantalla5 = new PantallaEntity
        {
            Id = 5,
            Codigo = 5,
            Descripcion = "Roles Por Usuario",
            Estado = true,
        };

        public static readonly PantallaEntity  pantalla6 = new PantallaEntity
        {
            Id = 6,
            Codigo = 6,
            Descripcion = "Roles por Pantalla",
            Estado = true,
        };

// Lista de todas las instancias
        public static readonly List<PantallaEntity> pantallas = new List<PantallaEntity> { pantalla1, pantalla2, pantalla3, pantalla4, pantalla5, pantalla6 };
    
}
