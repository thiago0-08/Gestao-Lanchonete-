﻿namespace WebApplication1.Model
{
    public class CriaPedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = "Pendente"; // Pendente, EmPreparacao, Entregue, Cancelado

        public decimal ValorEntrega { get; set; } = 0.0m;
        public string? EnderecoEntrega { get; set; }
        public string? TelefoneContato { get; set; }
        public string? NomeCliente { get; set; }
        public string? CuponDesconto { get; set; }
        public string? Observacoes { get; set; }
        public DateTime? DataEntrega { get; set; }

        public List<ItemPedido> Itens { get; set; } = new();
    }
}
