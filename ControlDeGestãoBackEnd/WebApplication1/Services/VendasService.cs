using Database;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using System.Globalization; 

namespace WebApplication1.Services
{
    public class RelatorioGraficoDTO
    {
        public string Label { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

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
                .Select(g => new ProdutoVendaDTO
                {
                    ProdutoId = g.Key,
                    NomeProduto = g.First().Produto.Nome,
                    QuantidadeVendida = g.Sum(ip => ip.Quantidade)
                })
                .ToListAsync();
        }

        
        public async Task<List<RelatorioGraficoDTO>> GetVendasUltimos7DiasAsync()
        {
            var hoje = DateTime.UtcNow.Date;
            var dataInicio = hoje.AddDays(-6); // 6 dias atrás + hoje = 7 dias

            var vendas = await _context.Pedidos
                .Where(p => p.Status == "Entregue" && p.DataEntrega.HasValue && p.DataEntrega.Value.Date >= dataInicio)
                .GroupBy(p => p.DataEntrega.Value.Date)
                .Select(g => new
                {
                    Data = g.Key,
                    Total = g.Sum(p => p.ValorTotal)
                })
                .ToDictionaryAsync(k => k.Data, v => v.Total);

           
            var resultado = new List<RelatorioGraficoDTO>();
            for (int i = 0; i < 7; i++)
            {
                var data = dataInicio.AddDays(i);
                vendas.TryGetValue(data, out decimal total);
                resultado.Add(new RelatorioGraficoDTO
                {
                    Label = data.ToString("dd/MM"), 
                    Valor = total
                });
            }
            return resultado;
        }

        public async Task<List<RelatorioGraficoDTO>> GetFaturamentoMensalAsync(int ano)
        {
            var vendas = await _context.Pedidos
                .Where(p => p.Status == "Entregue" && p.DataEntrega.HasValue && p.DataEntrega.Value.Year == ano)
                .GroupBy(p => p.DataEntrega.Value.Month)
                .Select(g => new
                {
                    Mes = g.Key,
                    Total = g.Sum(p => p.ValorTotal)
                })
                .ToDictionaryAsync(k => k.Mes, v => v.Total);

            var resultado = new List<RelatorioGraficoDTO>();
            var culture = new CultureInfo("pt-BR"); 

            for (int i = 1; i <= 12; i++)
            {
                vendas.TryGetValue(i, out decimal total);
                resultado.Add(new RelatorioGraficoDTO
                {
                    Label = culture.DateTimeFormat.GetMonthName(i), 
                    Valor = total
                });
            }
            return resultado;
        }
    }
}