namespace WebApplication1.DTO
{
    public class HistoricoPrecoDTO
    {
        public string NomeIngrediente { get; set; } = string.Empty;
        public DateTime DataEntrada { get; set; }
        public decimal CustoUnitario { get; set; }
    }
}