﻿using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class NovoProdutoController : ControllerBase
    {
        private readonly GestaoDbContext _context;

        public NovoProdutoController(GestaoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProdutos(
            string? nome,
            int pagina = 1,
            int tamanhoPagina = 12,
            int? categoriaId = null,
            string ordenarPor = "nome",
            bool apenasDisponiveis = false
        )
        {
            IQueryable<CriaProduto> produtosQuery = _context.Produtos.Include(p => p.Categoria);

            if (!string.IsNullOrEmpty(nome))
            {
                produtosQuery = produtosQuery.Where(p => EF.Functions.ILike(p.Nome, $"%{nome}%"));
            }

            if (categoriaId.HasValue)
            {
                produtosQuery = produtosQuery.Where(p => p.Categoria.Id == categoriaId.Value);
            }

            if (apenasDisponiveis)
            {
                produtosQuery = produtosQuery.Where(p => p.EstoqueAtual > 0);
            }

            int totalProdutos = await produtosQuery.CountAsync();

            produtosQuery = ordenarPor.ToLower() switch
            {
                "-preco" => produtosQuery.OrderByDescending(p => p.Preco),
                "preco" => produtosQuery.OrderBy(p => p.Preco),
                "-nome" => produtosQuery.OrderByDescending(p => p.Nome),
                _ => produtosQuery.OrderBy(p => p.Nome)
            };

            var produtosPaginados = await produtosQuery
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            var resultado = produtosPaginados.Select(p =>
            {
                return new
                {
                    p.Id,
                    p.Nome,
                    p.Descricao,
                    p.Preco,
                    p.Imagem,
                    Categoria = new { p.Categoria.Id, p.Categoria.Nome },
                    EstoqueAtual = p.EstoqueAtual,
                    Indisponivel = p.EstoqueAtual <= 0
                };
            });

            var resultadoPaginado = new
            {
                TotalItems = totalProdutos,
                Page = pagina,
                PageSize = tamanhoPagina,
                TotalPages = (int)Math.Ceiling(totalProdutos / (double)tamanhoPagina),
                Products = resultado
            };

            return Ok(resultadoPaginado);
        }

        
        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] CriaProdutoDTO produtoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _context.Categorias.FindAsync(produtoDto.IdCategoria);
            if (categoria == null)
            {
                return BadRequest("Categoria inválida.");
            }

            var produto = new CriaProduto
            {
                Nome = produtoDto.Nome,
                Descricao = produtoDto.Descricao,
                Preco = produtoDto.Preco,
                Imagem = produtoDto.Imagem,
                Categoria = categoria,
                EstoqueAtual = 0 
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return NotFound();

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(int id, [FromBody] CriaProdutoDTO produtoDto)
        {
            var produto = await _context.Produtos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
                return NotFound("Produto não encontrado.");

            var categoria = await _context.Categorias.FindAsync(produtoDto.IdCategoria);
            if (categoria == null)
                return BadRequest("Categoria inválida.");

            produto.Nome = produtoDto.Nome;
            produto.Descricao = produtoDto.Descricao;
            produto.Preco = produtoDto.Preco;
            produto.Imagem = produtoDto.Imagem;
            produto.Categoria = categoria;

            await _context.SaveChangesAsync();

            return Ok(produto);
        }
    }
}