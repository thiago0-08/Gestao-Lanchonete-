namespace WebApplication1.DTO
{
    public class CustoProdutoDTO
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public double PrecoVenda { get; set; }
        public double CustoProducao { get; set; }
        public double MargemBruta { get; set; }
        public double PorcentagemMargem { get; set; }
    }
}