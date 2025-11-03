using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DTO;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly GestaoDbContext _context;

        public PedidosController(GestaoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] CriaPedidoDTO dto)
        {
            if (dto.Itens.Count == 0)
                return BadRequest("O pedido deve conter ao menos um item.");

            var pedido = new CriaPedido
            {
                ValorEntrega = dto.ValorEntrega,
                EnderecoEntrega = dto.EnderecoEntrega,
                TelefoneContato = dto.TelefoneContato,
                NomeCliente = dto.NomeCliente,
                CuponDesconto = dto.CuponDesconto,
                Observacoes = dto.Observacoes,
                Status = "Pendente"
            };

            decimal total = 0m; // CORREÇÃO: Inicialize com 'm' para garantir que é decimal

            foreach (var item in dto.Itens)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                if (produto == null) return BadRequest($"Produto ID {item.ProdutoId} não encontrado.");

                var itemPedido = new ItemPedido
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    // CORREÇÃO: Se 'produto.Preco' for 'double', converta para 'decimal'
                    PrecoUnitario = (decimal)produto.Preco
                };

                // CORREÇÃO: Faça o cálculo usando decimal
                total += itemPedido.PrecoUnitario * (decimal)itemPedido.Quantidade;
                pedido.Itens.Add(itemPedido);
            }

            pedido.ValorTotal = total + pedido.ValorEntrega;

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CriaPedido>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                                        .Include(p => p.Itens)
                                        .ThenInclude(item => item.Produto)
                                        .OrderByDescending(p => p.DataPedido)
                                        .ToListAsync();

            if (pedidos == null || !pedidos.Any())
            {
                return NotFound("Nenhum pedido encontrado.");
            }

            return Ok(pedidos);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        [HttpPost("{id}/finalizar")]
        public async Task<IActionResult> FinalizarPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(item => item.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == "Entregue")
                return BadRequest("Este pedido já foi finalizado.");

            var alertasCriticos = new List<string>();

            // Verifica se o estoque é suficiente
            foreach (var itemPedido in pedido.Itens)
            {
                var receita = await _context.Receitas
                    .Include(r => r.Itens)
                    .ThenInclude(ir => ir.Ingrediente)
                    .FirstOrDefaultAsync(r => r.ProdutoId == itemPedido.ProdutoId);

                if (receita == null)
                    return BadRequest($"Produto '{itemPedido.Produto.Nome}' não tem uma receita associada. O pedido não pode ser finalizado.");

                foreach (var ingredienteReceita in receita.Itens)
                {
                    var ingrediente = ingredienteReceita.Ingrediente;
                    var quantidadeNecessaria = (decimal)ingredienteReceita.Quantidade * (decimal)itemPedido.Quantidade;

                    if (ingrediente.QuantidadeEstoque < quantidadeNecessaria)
                    {
                        return BadRequest($"Estoque insuficiente de '{ingrediente.Nome}'. Necessário: {quantidadeNecessaria}, Disponível: {ingrediente.QuantidadeEstoque}");
                    }
                }
            }

            // Se o estoque é suficiente, inicia a dedução
            foreach (var itemPedido in pedido.Itens)
            {
                var receita = await _context.Receitas
                    .Include(r => r.Itens)
                    .ThenInclude(ir => ir.Ingrediente)
                    .FirstOrDefaultAsync(r => r.ProdutoId == itemPedido.ProdutoId);

                foreach (var ingredienteReceita in receita.Itens)
                {
                    var ingrediente = ingredienteReceita.Ingrediente;
                    var quantidadeNecessaria = (decimal)ingredienteReceita.Quantidade * (decimal)itemPedido.Quantidade;

                    ingrediente.QuantidadeEstoque -= quantidadeNecessaria;

                    if (ingrediente.QuantidadeEstoque <= ingrediente.EstoqueMinimo)
                    {
                        alertasCriticos.Add($"Estoque do ingrediente '{ingrediente.Nome}' está abaixo ou no nível mínimo ({ingrediente.EstoqueMinimo}). Quantidade atual: {ingrediente.QuantidadeEstoque}");
                    }

                    var novoLancamento = new LancamentoIngrediente
                    {
                        IngredienteId = ingrediente.Id,
                        Quantidade = quantidadeNecessaria,
                        CustoUnitario = ingrediente.CustoMedio,
                        Tipo = "saida",
                        DataEntrada = DateTime.UtcNow
                    };
                    _context.LancamentosIngredientes.Add(novoLancamento);
                }
            }

            pedido.Status = "Entregue";
            pedido.DataEntrega = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Pedido finalizado e estoque atualizado com sucesso.", Alertas = alertasCriticos });
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPedidosByStatus(string status)
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.Status.ToLower() == status.ToLower())
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            if (!pedidos.Any())
            {
                return NotFound($"Nenhum pedido encontrado com o status '{status}'.");
            }

            return Ok(pedidos);
        }

        [HttpPost("{id}/em-preparacao")]
        public async Task<IActionResult> MarcarEmPreparacao(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            pedido.Status = "EmPreparacao";
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = $"Pedido {id} marcado como 'Em Preparação'." });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizaStatusPedidoDTO dto)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            if (dto.NovoStatus.ToLower() != "pendente" &&
                dto.NovoStatus.ToLower() != "em-preparacao" &&
                dto.NovoStatus.ToLower() != "concluido" &&
                dto.NovoStatus.ToLower() != "entregue" &&
                dto.NovoStatus.ToLower() != "cancelado")
            {
                return BadRequest("Status inválido.");
            }

            pedido.Status = dto.NovoStatus;
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = $"Status do pedido {id} atualizado para '{dto.NovoStatus}'." });
        }
    }
}