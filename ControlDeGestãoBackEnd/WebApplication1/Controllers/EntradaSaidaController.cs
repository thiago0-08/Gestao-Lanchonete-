﻿using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Model;

namespace WebApplication1.Controllers.Vitrine
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class EntradaSaidaController : ControllerBase
    {
        private readonly GestaoDbContext _context;

        public EntradaSaidaController(GestaoDbContext context)
        {
            _context = context;
        }

        [HttpGet("produto/{produtoId}")]
        public async Task<IActionResult> GetByProduto(int produtoId)
        {
            var lancamentos = await _context.Lancamentos
                .Where(l => l.ProdutoId == produtoId)
                .OrderBy(l => l.Data)
                .ToListAsync();

            if (!lancamentos.Any())
                return NotFound("Nenhum lançamento encontrado para este produto.");

            return Ok(lancamentos);
        }

        


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var lancamentos = await _context.Lancamentos
                .Include(l => l.Produto)
                .OrderByDescending(l => l.Data)
                .ToListAsync();
            return Ok(lancamentos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EntradaSaidaDTO lancamentoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produto = await _context.Produtos.FindAsync(lancamentoDto.ProdutoId);
            if (produto == null)
                return NotFound("Produto não encontrado.");

            if (lancamentoDto.Tipo == "saida" && produto.EstoqueAtual < (decimal)lancamentoDto.Quantidade)
                return BadRequest("Estoque insuficiente para essa saída.");

            if (lancamentoDto.Tipo == "entrada")
            {
                produto.EstoqueAtual += (decimal)lancamentoDto.Quantidade;
            }
            else if (lancamentoDto.Tipo == "saida")
            {
                produto.EstoqueAtual -= (decimal)lancamentoDto.Quantidade;
            }

            var lancamento = new EntradaSaida
            {
                ProdutoId = lancamentoDto.ProdutoId,
                Quantidade = (double)lancamentoDto.Quantidade,
                Tipo = lancamentoDto.Tipo,
                UnidadeMedida = lancamentoDto.UnidadeMedida,
                Data = DateTime.UtcNow
            };

            _context.Lancamentos.Add(lancamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProduto), new { produtoId = lancamento.ProdutoId }, lancamento);
        }
    }
}