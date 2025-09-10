using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
    }
}