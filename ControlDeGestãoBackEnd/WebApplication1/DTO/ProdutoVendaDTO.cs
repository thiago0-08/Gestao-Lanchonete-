namespace WebApplication1.DTO
{
    public class ProdutoVendaDTO
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
    }
}