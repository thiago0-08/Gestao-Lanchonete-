using WebApplication1.DTO;

namespace WebApplication1.DTO
{
    public class ReceitaDetalhadaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public int ProdutoId { get; set; }
        public decimal CustoTotal { get; set; }
        public List<ItemDetalhadoDTO> Itens { get; set; } = new();
    }

    public class ItemDetalhadoDTO
    {
        public int IngredienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;
        public decimal Quantidade { get; set; } 
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}