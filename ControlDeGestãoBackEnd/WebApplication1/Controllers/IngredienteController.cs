using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DTO;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Protege este controlador com JWT
    public class IngredienteController : ControllerBase
    {
        private readonly GestaoDbContext _context;

        public IngredienteController(GestaoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredienteResponseDTO>>> GetIngredientes()
        {
            var ingredientes = await _context.Ingredientes.ToListAsync();

            var responseDTOs = ingredientes.Select(i => new IngredienteResponseDTO
            {
                Id = i.Id,
                Nome = i.Nome,
                UnidadeMedida = i.UnidadeMedida,
                QuantidadeEstoque = i.QuantidadeEstoque,
                CustoMedio = i.CustoMedio,
                EstoqueMinimo = i.EstoqueMinimo,
                FornecedorPadrao = i.FornecedorPadrao,
                DataValidadeProxima = i.DataValidadeProxima,
                DataUltimaAtualizacao = i.DataUltimaAtualizacao
            }).ToList();

            return Ok(responseDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IngredienteResponseDTO>> GetIngrediente(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);

            if (ingrediente == null)
            {
                return NotFound("Ingrediente não encontrado");
            }

            var responseDTO = new IngredienteResponseDTO
            {
                Id = ingrediente.Id,
                Nome = ingrediente.Nome,
                UnidadeMedida = ingrediente.UnidadeMedida,
                QuantidadeEstoque = ingrediente.QuantidadeEstoque,
                CustoMedio = ingrediente.CustoMedio,
                EstoqueMinimo = ingrediente.EstoqueMinimo,
                FornecedorPadrao = ingrediente.FornecedorPadrao,
                DataValidadeProxima = ingrediente.DataValidadeProxima,
                DataUltimaAtualizacao = ingrediente.DataUltimaAtualizacao
            };

            return Ok(responseDTO);
        }

        [HttpPost] // NOVO ENDPOINT DE CRIAÇÃO
        public async Task<ActionResult<IngredienteResponseDTO>> PostIngrediente(IngredienteDTO ingredienteDTO)
        {
            // Validação: Verificar se o nome já existe
            var nomeExistente = await _context.Ingredientes.AnyAsync(i => i.Nome == ingredienteDTO.Nome);
            if (nomeExistente)
            {
                return Conflict("Já existe um ingrediente com este nome.");
            }

            var ingrediente = new Ingrediente
            {
                Nome = ingredienteDTO.Nome,
                UnidadeMedida = ingredienteDTO.UnidadeMedida,
                EstoqueMinimo = ingredienteDTO.EstoqueMinimo,
                FornecedorPadrao = ingredienteDTO.FornecedorPadrao,
                QuantidadeEstoque = 0m, // Inicializado como zero
                CustoMedio = 0m,     // Inicializado como zero
                DataUltimaAtualizacao = DateTime.UtcNow
            };

            _context.Ingredientes.Add(ingrediente);
            await _context.SaveChangesAsync();

            var responseDTO = new IngredienteResponseDTO
            {
                Id = ingrediente.Id,
                Nome = ingrediente.Nome,
                UnidadeMedida = ingrediente.UnidadeMedida,
                QuantidadeEstoque = ingrediente.QuantidadeEstoque,
                CustoMedio = ingrediente.CustoMedio,
                EstoqueMinimo = ingrediente.EstoqueMinimo,
                FornecedorPadrao = ingrediente.FornecedorPadrao,
                DataValidadeProxima = ingrediente.DataValidadeProxima,
                DataUltimaAtualizacao = ingrediente.DataUltimaAtualizacao
            };

            // Retorna 201 Created com o recurso recém-criado
            return CreatedAtAction(nameof(GetIngrediente), new { id = ingrediente.Id }, responseDTO);
        }

        [HttpPost("entrada")]
        public async Task<ActionResult<IngredienteResponseDTO>> RegistrarEntradaEstoque(EntradaEstoqueRequestDTO entradaDTO)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(entradaDTO.IdIngrediente);

            if (ingrediente == null)
            {
                return NotFound("Ingrediente não encontrado para registrar a entrada.");
            }

            if (entradaDTO.QuantidadeEntrada <= 0 || entradaDTO.CustoUnitario <= 0)
            {
                return BadRequest("Quantidade e Custo Unitário da entrada devem ser maiores que zero.");
            }

            decimal custoTotalAtual = ingrediente.QuantidadeEstoque * ingrediente.CustoMedio;
            decimal custoTotalEntrada = entradaDTO.QuantidadeEntrada * entradaDTO.CustoUnitario;
            decimal novaQuantidadeTotal = ingrediente.QuantidadeEstoque + entradaDTO.QuantidadeEntrada;

            if (novaQuantidadeTotal > 0)
            {
                // CORREÇÃO (Custo): Arredonda o CustoMedio para 2 casas decimais
                ingrediente.CustoMedio = Math.Round((custoTotalAtual + custoTotalEntrada) / novaQuantidadeTotal, 2);
            }
            else
            {
                ingrediente.CustoMedio = 0m;
            }

            // CORREÇÃO (Estoque): Arredonda a quantidade de estoque para 2 casas decimais
            ingrediente.QuantidadeEstoque = Math.Round(novaQuantidadeTotal, 2);
            ingrediente.DataUltimaAtualizacao = DateTime.UtcNow;

            if (entradaDTO.DataValidade.HasValue)
            {
                if (!ingrediente.DataValidadeProxima.HasValue || entradaDTO.DataValidade.Value < ingrediente.DataValidadeProxima.Value)
                {
                    ingrediente.DataValidadeProxima = entradaDTO.DataValidade.Value;
                }
            }

            // Adiciona o lançamento no histórico
            var novoLancamento = new LancamentoIngrediente
            {
                IngredienteId = ingrediente.Id,
                Quantidade = entradaDTO.QuantidadeEntrada,
                CustoUnitario = entradaDTO.CustoUnitario,
                Tipo = "entrada",
                DataEntrada = DateTime.UtcNow
            };
            _context.LancamentosIngredientes.Add(novoLancamento);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"ERRO DE CONCORRÊNCIA (Entrada): {ex.Message}");
                if (!IngredienteExists(ingrediente.Id))
                {
                    return NotFound("Ingrediente não encontrado durante a atualização da entrada.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex) // CAPTURA DE ERROS GERAIS
            {
                Console.WriteLine($"ERRO GERAL NO SAVE (Entrada): {ex.Message}");
                return StatusCode(500, new { erro = "Erro interno ao salvar as alterações. Verifique o log do servidor." });
            }

            var responseDTO = new IngredienteResponseDTO
            {
                Id = ingrediente.Id,
                Nome = ingrediente.Nome,
                UnidadeMedida = ingrediente.UnidadeMedida,
                QuantidadeEstoque = ingrediente.QuantidadeEstoque,
                CustoMedio = ingrediente.CustoMedio,
                EstoqueMinimo = ingrediente.EstoqueMinimo,
                FornecedorPadrao = ingrediente.FornecedorPadrao,
                DataValidadeProxima = ingrediente.DataValidadeProxima,
                DataUltimaAtualizacao = ingrediente.DataUltimaAtualizacao
            };

            return Ok(responseDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngrediente(int id, IngredienteDTO ingredienteDTO)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);

            if (ingrediente == null)
            {
                return NotFound("Ingrediente não encontrado");
            }

            ingrediente.Nome = ingredienteDTO.Nome;
            ingrediente.UnidadeMedida = ingredienteDTO.UnidadeMedida;
            ingrediente.EstoqueMinimo = ingredienteDTO.EstoqueMinimo;
            ingrediente.FornecedorPadrao = ingredienteDTO.FornecedorPadrao;
            ingrediente.DataUltimaAtualizacao = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredienteExists(id))
                {
                    return NotFound("Ingrediente não encontrado durante a atualização.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost("saida")]
        public async Task<IActionResult> RegistrarSaidaEstoque(SaidaEstoqueRequestDTO saidaDTO)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(saidaDTO.IdIngrediente);

            if (ingrediente == null)
            {
                return NotFound("Ingrediente não encontrado para registrar a saída.");
            }

            if (saidaDTO.QuantidadeSaida > ingrediente.QuantidadeEstoque)
            {
                return BadRequest("Estoque insuficiente para a quantidade de saída solicitada.");
            }

            // CORREÇÃO: Arredonda a quantidade de estoque após a subtração
            ingrediente.QuantidadeEstoque = Math.Round(ingrediente.QuantidadeEstoque - saidaDTO.QuantidadeSaida, 2);
            ingrediente.DataUltimaAtualizacao = DateTime.UtcNow;

            var novoLancamento = new LancamentoIngrediente
            {
                IngredienteId = ingrediente.Id,
                Quantidade = saidaDTO.QuantidadeSaida,
                CustoUnitario = ingrediente.CustoMedio,
                Tipo = "saida",
                DataEntrada = DateTime.UtcNow
            };

            _context.LancamentosIngredientes.Add(novoLancamento);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredienteExists(saidaDTO.IdIngrediente))
                {
                    return NotFound("Ingrediente não encontrado durante a atualização da saída.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex) // CAPTURA DE ERROS GERAIS
            {
                Console.WriteLine($"ERRO GERAL NO SAVE (Saída): {ex.Message}");
                return StatusCode(500, new { erro = "Erro interno ao salvar as alterações. Verifique o log do servidor." });
            }

            return Ok(new { Mensagem = "Saída de estoque registrada com sucesso." });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngrediente(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
            {
                return NotFound("Ingrediente não encontrado para exclusão.");
            }

            _context.Ingredientes.Remove(ingrediente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IngredienteExists(int id)
        {
            return _context.Ingredientes.Any(e => e.Id == id);
        }
    }
}
