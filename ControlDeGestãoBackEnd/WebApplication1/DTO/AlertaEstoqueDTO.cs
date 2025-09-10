namespace WebApplication1.DTO
{
    public class AlertaEstoqueDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; 
        public double EstoqueAtual { get; set; }
        public double EstoqueMinimo { get; set; }
    }
}