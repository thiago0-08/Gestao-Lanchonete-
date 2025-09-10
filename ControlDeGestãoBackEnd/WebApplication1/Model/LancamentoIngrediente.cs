using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Model
{
    public class LancamentoIngrediente
    {
        [Key]
        public int Id { get; set; }

        public int IngredienteId { get; set; }
        [ForeignKey("IngredienteId")]
        public Ingrediente Ingrediente { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantidade { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal CustoUnitario { get; set; }

        [StringLength(10)]
        public string Tipo { get; set; } // "entrada" ou "saida"

        public DateTime DataEntrada { get; set; } = DateTime.UtcNow;
    }
}