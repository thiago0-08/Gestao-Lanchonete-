using Database;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;

namespace WebApplication1.Services
{
    public class VendasService
    {
        private readonly GestaoDbContext _context;

        public VendasService(GestaoDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetFaturamentoDiario(DateTime data)
        {
            return await _context.Pedidos
                .Where(p => p.DataEntrega.HasValue && p.DataEntrega.Value.Date == data.Date && p.Status == "Entregue")
                .SumAsync(p => p.ValorTotal);
        }

        public async Task<List<ProdutoVendaDTO>> GetProdutosMaisVendidos(int topN)
        {
            return await _context.ItensPedido
                .Include(ip => ip.Produto)
                .GroupBy(ip => ip.ProdutoId)
                .OrderByDescending(g => g.Sum(ip => ip.Quantidade))
                .Take(topN)
                .Select(g => new ProdutoVendaDTO
                {
                    ProdutoId = g.Key,
                    NomeProduto = g.First().Produto.Nome,
                    QuantidadeVendida = g.Sum(ip => ip.Quantidade)
                })
                .ToListAsync();
        }
    }
}