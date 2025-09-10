using Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DTO;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertaController : ControllerBase
    {
        private readonly AlertaService _alertaService;

        public AlertaController(AlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        [HttpGet("estoque")]
        public async Task<IActionResult> GetAlertasEstoque()
        {
            var alertas = await _alertaService.ObterAlertasEstoqueAsync();

            if (alertas == null || alertas.Count == 0)
            {
                return Ok(new { Mensagem = "Nenhum alerta de estoque encontrado." });
            }

            return Ok(alertas);
        }
    }
}