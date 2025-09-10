using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DTO;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelatorioController : ControllerBase
    {
        private readonly RelatorioService _relatorioService;

        public RelatorioController(RelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet("itens-em-falta")]
        public async Task<ActionResult<List<AlertaEstoqueDTO>>> GetItensEmFalta()
        {
            var relatorio = await _relatorioService.GerarRelatorioItensEmFaltaAsync();

            if (relatorio == null || relatorio.Count == 0)
            {
                return NotFound("Nenhum item em falta encontrado.");
            }

            return Ok(relatorio);
        }

        [HttpGet("historico-precos")]
        public async Task<ActionResult<List<HistoricoPrecoDTO>>> GetHistoricoPrecos()
        {
            var historico = await _relatorioService.GerarHistoricoPrecosAsync();

            if (historico == null || historico.Count == 0)
            {
                return NotFound("Nenhum histórico de preços encontrado.");
            }

            return Ok(historico);
        }
    }
}