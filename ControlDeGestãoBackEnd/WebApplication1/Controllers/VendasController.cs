using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class VendasController : ControllerBase
    {
        private readonly VendasService _vendasService;

        public VendasController(VendasService vendasService)
        {
            _vendasService = vendasService;
        }

        [HttpGet("faturamento-diario/{data}")]
        public async Task<IActionResult> GetFaturamentoDiario(DateTime data)
        {
            var faturamento = await _vendasService.GetFaturamentoDiario(data);
            return Ok(new { data = data.Date, faturamento = faturamento });
        }

        [HttpGet("mais-vendidos/{topN}")]
        public async Task<IActionResult> GetProdutosMaisVendidos(int topN)
        {
            var produtos = await _vendasService.GetProdutosMaisVendidos(topN);
            return Ok(produtos);
        }

        [HttpGet("vendas-ultimos-7-dias")]
        public async Task<IActionResult> GetVendasUltimos7Dias()
        {
            var vendas = await _vendasService.GetVendasUltimos7DiasAsync();
            return Ok(vendas);
        }

        [HttpGet("faturamento-mensal/{ano}")]
        public async Task<IActionResult> GetFaturamentoMensal(int ano)
        {
            var faturamento = await _vendasService.GetFaturamentoMensalAsync(ano);
            return Ok(faturamento);
        }

        [HttpGet("total-pedidos/{data}")]
        public async Task<IActionResult> GetTotalPedidosDiario(DateTime data)
        {
            var total = await _vendasService.GetTotalPedidosDiarioAsync(data);
            return Ok(new { data = data.Date, totalPedidos = total });
        }

        [HttpGet("ticket-medio/{data}")]
        public async Task<IActionResult> GetTicketMedioDiario(DateTime data)
        {
            var ticket = await _vendasService.GetTicketMedioDiarioAsync(data);
            return Ok(new { data = data.Date, ticketMedio = ticket });
        }
    }
}