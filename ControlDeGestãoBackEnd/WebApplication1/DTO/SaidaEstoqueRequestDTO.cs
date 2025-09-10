using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class SaidaEstoqueRequestDTO
    {
        [Required]
        public int IdIngrediente { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade de saída deve ser maior que zero.")]
        public decimal QuantidadeSaida { get; set; }
    }
}