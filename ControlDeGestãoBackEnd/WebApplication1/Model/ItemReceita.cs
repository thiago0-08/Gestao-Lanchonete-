namespace WebApplication1.Model
{
    public class ItemReceita
    {
        public int Id { get; set; }
        public int receitaId { get; set; }
        public Receita Receita { get; set; }
        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; }
        public decimal Quantidade { get; set; }
    }
}
