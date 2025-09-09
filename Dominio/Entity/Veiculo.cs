using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entity;


public class Veiculo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;
    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Modelo { get; set; } = default!;

    [Required]
    public int Ano { get; set; } = default!;
}
