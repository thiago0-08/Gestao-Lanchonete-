using Database;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Model;

public class RelatorioService
{
    private readonly GestaoDbContext _context;

    public RelatorioService(GestaoDbContext context)
    {
        _context = context;
    }

    public async Task<List<AlertaEstoqueDTO>> GerarRelatorioItensEmFaltaAsync()
    {
        var produtosEmFalta = await _context.Produtos
            .Where(p => p.EstoqueAtual < (decimal)p.EstoqueMinimo)
            .Select(p => new AlertaEstoqueDTO
            {
                Nome = p.Nome,
                Tipo = "Produto",
                EstoqueAtual = (double)p.EstoqueAtual,
                EstoqueMinimo = (double)p.EstoqueMinimo
            })
            .ToListAsync();

        var ingredientesEmFalta = await _context.Ingredientes
            .Where(i => i.QuantidadeEstoque < i.EstoqueMinimo)
            .Select(i => new AlertaEstoqueDTO
            {
                Nome = i.Nome,
                Tipo = "Ingrediente",
                EstoqueAtual = (double)i.QuantidadeEstoque,
                EstoqueMinimo = (double)i.EstoqueMinimo
            })
            .ToListAsync();

        var relatorio = new List<AlertaEstoqueDTO>();
        relatorio.AddRange(produtosEmFalta);
        relatorio.AddRange(ingredientesEmFalta);

        return relatorio;
    }

    public async Task<List<HistoricoPrecoDTO>> GerarHistoricoPrecosAsync()
    {
        var historico = await _context.LancamentosIngredientes
            .Include(l => l.Ingrediente)
            .Where(l => l.Tipo == "entrada")
            .OrderByDescending(l => l.DataEntrada)
            .Select(l => new HistoricoPrecoDTO
            {
                NomeIngrediente = l.Ingrediente.Nome,
                DataEntrada = l.DataEntrada,
                CustoUnitario = l.CustoUnitario
            })
            .ToListAsync();

        return historico;
    }
}