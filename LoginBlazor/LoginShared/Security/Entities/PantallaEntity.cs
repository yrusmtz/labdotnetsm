using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginShared.Security.Entities;

public class PantallaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public int Codigo { get; set; }

    [Required] [StringLength(100)] public string Descripcion { get; set; }

    [Required] [DefaultValue(1)] public Boolean Estado { get; set; }

    [StringLength(50)] public string? Icono { get; set; }

    [StringLength(50)] public string? HostName { get; set; }
    

    
}