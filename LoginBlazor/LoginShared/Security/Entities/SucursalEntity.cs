using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginShared.Security.Entities;

public class SucursalEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    public int Codigo { get; set; }
    [Required] [StringLength(100)] public string Descripcion { get; set; }
}