using Database;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AlertaService
{
    private readonly GestaoDbContext _context;

    public AlertaService(GestaoDbContext context)
    {
        _context = context;
    }

    public async Task<List<AlertaEstoqueDTO>> ObterAlertasEstoqueAsync()
    {
        var alertas = new List<AlertaEstoqueDTO>();

        // Alertas de Ingredientes (Estoque Mínimo)
        var ingredientesEmFalta = await _context.Ingredientes
            .Where(i => i.QuantidadeEstoque < i.EstoqueMinimo)
            .Select(i => new AlertaEstoqueDTO
            {
                Nome = i.Nome,
                Tipo = "Ingrediente",
                // CORREÇÃO: Conversão explícita de decimal para double
                EstoqueAtual = (double)i.QuantidadeEstoque,
                EstoqueMinimo = (double)i.EstoqueMinimo
            })
            .ToListAsync();

        // Alertas de Produtos (Estoque Mínimo)
        var produtosEmFalta = await _context.Produtos
            .Where(p => p.EstoqueAtual < (decimal)p.EstoqueMinimo) // Converta p.EstoqueMinimo para decimal para a comparação
            .Select(p => new AlertaEstoqueDTO
            {
                Nome = p.Nome,
                Tipo = "Produto",
                // CORREÇÃO: Conversão explícita de decimal para double
                EstoqueAtual = (double)p.EstoqueAtual,
                EstoqueMinimo = (double)p.EstoqueMinimo
            })
            .ToListAsync();

        alertas.AddRange(ingredientesEmFalta);
        alertas.AddRange(produtosEmFalta);

        return alertas;
    }
}