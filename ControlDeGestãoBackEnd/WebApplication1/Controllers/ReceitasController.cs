using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ReceitasController : ControllerBase
    {
        private readonly IReceitaService _receitaService;

        public ReceitasController(IReceitaService receitaService)
        {
            _receitaService = receitaService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarReceita([FromBody] ReceitaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resultado = await _receitaService.CriarReceitaAsync(dto);
                return Created(string.Empty, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterReceita(int id)
        {
            var receita = await _receitaService.ObterReceitaPorIdAsync(id);
            if (receita == null)
                return NotFound("Receita não encontrada."); 
            return Ok(receita);
        }

        [HttpGet("custo/{produtoId}")]
        public async Task<IActionResult> CalcularCustoProduto(int produtoId)
        {
            try
            {
                var custoProduto = await _receitaService.CalcularCustoProdutoAsync(produtoId);
                return Ok(custoProduto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarReceita(int id, [FromBody] ReceitaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                dto.Id = id;
                var resultado = await _receitaService.AtualizarReceitaAsync(dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}