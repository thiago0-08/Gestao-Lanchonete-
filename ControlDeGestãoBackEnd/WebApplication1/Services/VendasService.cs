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
            // Converte para UTC para o Postgres nao da erro
            var inicioDia = DateTime.SpecifyKind(data.Date, DateTimeKind.Utc);
            var fimDia = inicioDia.AddDays(1);

            return await _context.Pedidos
                .Where(p => p.DataEntrega.HasValue &&
                            p.DataEntrega >= inicioDia &&
                            p.DataEntrega < fimDia &&
                            p.Status == "Entregue")
                .SumAsync(p => p.ValorTotal);
        }

        public async Task<int> GetTotalPedidosDiarioAsync(DateTime data)
        {
            var inicioDia = DateTime.SpecifyKind(data.Date, DateTimeKind.Utc);
            var fimDia = inicioDia.AddDays(1);

            return await _context.Pedidos
                .CountAsync(p => p.DataEntrega.HasValue &&
                                 p.DataEntrega >= inicioDia &&
                                 p.DataEntrega < fimDia &&
                                 p.Status == "Entregue");
        }

        public async Task<decimal> GetTicketMedioDiarioAsync(DateTime data)
        {
            var inicioDia = DateTime.SpecifyKind(data.Date, DateTimeKind.Utc);
            var fimDia = inicioDia.AddDays(1);

            var pedidosDoDia = await _context.Pedidos
                .Where(p => p.DataEntrega.HasValue &&
                            p.DataEntrega >= inicioDia &&
                            p.DataEntrega < fimDia &&
                            p.Status == "Entregue")
                .ToListAsync();

            if (!pedidosDoDia.Any()) return 0m;

            return pedidosDoDia.Average(p => p.ValorTotal);
        }
        // -----------------------------------------------------------

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

        public async Task<List<RelatorioGraficoDTO>> GetVendasUltimos7DiasAsync()
        {
            var hoje = DateTime.UtcNow.Date;
            var dataInicio = hoje.AddDays(-6);

            var vendasBrutas = await _context.Pedidos
                .Where(p => p.Status == "Entregue" &&
                            p.DataEntrega.HasValue &&
                            p.DataEntrega >= dataInicio)
                .Select(p => new { p.DataEntrega, p.ValorTotal }) 
                .ToListAsync();

            var vendasAgrupadas = vendasBrutas
                .GroupBy(p => p.DataEntrega.Value.Date)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.ValorTotal));

            var resultado = new List<RelatorioGraficoDTO>();
            for (int i = 0; i < 7; i++)
            {
                var data = dataInicio.AddDays(i);
                vendasAgrupadas.TryGetValue(data, out decimal total);
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
            
            var dataInicioAno = new DateTime(ano, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dataFimAno = dataInicioAno.AddYears(1);

            var vendasBrutas = await _context.Pedidos
                .Where(p => p.Status == "Entregue" &&
                            p.DataEntrega.HasValue &&
                            p.DataEntrega >= dataInicioAno &&
                            p.DataEntrega < dataFimAno)
                .Select(p => new { p.DataEntrega, p.ValorTotal })
                .ToListAsync();

            var vendasAgrupadas = vendasBrutas
                .GroupBy(p => p.DataEntrega.Value.Month)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.ValorTotal));

            var resultado = new List<RelatorioGraficoDTO>();
            var culture = new CultureInfo("pt-BR");

            for (int i = 1; i <= 12; i++)
            {
                vendasAgrupadas.TryGetValue(i, out decimal total);
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