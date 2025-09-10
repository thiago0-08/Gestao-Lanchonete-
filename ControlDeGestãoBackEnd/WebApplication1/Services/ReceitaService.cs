using Database;
using WebApplication1.Model;
using WebApplication1.DTO;
using Microsoft.EntityFrameworkCore;

public class ReceitaService : IReceitaService
{
    private readonly GestaoDbContext _context;

    public ReceitaService(GestaoDbContext context)
    {
        _context = context;
    }

    public async Task<ReceitaDetalhadaDTO> CriarReceitaAsync(ReceitaDTO dto)
    {
        var produto = await _context.Produtos.FindAsync(dto.ProdutoId);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado.");

        var receita = new Receita
        {
            Nome = dto.Nome,
            ProdutoId = dto.ProdutoId,
            Itens = new List<ItemReceita>()
        };

        decimal custoTotal = 0;

        foreach (var item in dto.Itens)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(item.IngredienteId);
            if (ingrediente == null)
                throw new ArgumentException($"Ingrediente {item.IngredienteId} não encontrado.");

            receita.Itens.Add(new ItemReceita
            {
                IngredienteId = item.IngredienteId,
                Quantidade = item.Quantidade
            });

            custoTotal += (decimal)item.Quantidade * ingrediente.CustoMedio;
        }

        _context.Receitas.Add(receita);
        await _context.SaveChangesAsync();

        return await ObterReceitaPorIdAsync(receita.Id)
            ?? throw new Exception("Erro ao carregar a receita criada.");
    }

    public async Task<ReceitaDetalhadaDTO> AtualizarReceitaAsync(ReceitaDTO dto)
    {
        var receita = await _context.Receitas
            .Include(r => r.Itens)
            .FirstOrDefaultAsync(r => r.Id == dto.Id);

        if (receita == null)
            throw new KeyNotFoundException("Receita não encontrada.");

        receita.Nome = dto.Nome;
        receita.ProdutoId = dto.ProdutoId;

        _context.ItemReceitas.RemoveRange(receita.Itens);

        receita.Itens = new List<ItemReceita>();
        foreach (var item in dto.Itens)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(item.IngredienteId);
            if (ingrediente == null)
                throw new ArgumentException($"Ingrediente {item.IngredienteId} não encontrado.");

            receita.Itens.Add(new ItemReceita
            {
                IngredienteId = item.IngredienteId,
                Quantidade = item.Quantidade
            });
        }

        await _context.SaveChangesAsync();

        return await ObterReceitaPorIdAsync(dto.Id)
               ?? throw new Exception("Erro ao carregar a receita atualizada.");
    }

    public async Task<ReceitaDetalhadaDTO?> ObterReceitaPorIdAsync(int id)
    {
        var receita = await _context.Receitas
            .Include(r => r.Itens)
            .ThenInclude(i => i.Ingrediente)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receita == null)
        {
            return null;
        }

        decimal custoTotal = receita.Itens.Sum(i => i.Ingrediente.CustoMedio * (decimal)i.Quantidade);

        return new ReceitaDetalhadaDTO
        {
            Nome = receita.Nome,
            ProdutoId = receita.ProdutoId,
            CustoTotal = custoTotal,
            Itens = receita.Itens.Select(i => new ItemDetalhadoDTO
            {
                IngredienteId = i.IngredienteId,
                Nome = i.Ingrediente.Nome,
                Unidade = i.Ingrediente.UnidadeMedida,
                Quantidade = (decimal)i.Quantidade,
                PrecoUnitario = i.Ingrediente.CustoMedio,
                Subtotal = (decimal)i.Quantidade * i.Ingrediente.CustoMedio
            }).ToList()
        };
    }

    public async Task<CustoProdutoDTO> CalcularCustoProdutoAsync(int produtoId)
    {
        var produto = await _context.Produtos.FindAsync(produtoId);
        if (produto == null)
        {
            throw new ArgumentException("Produto não encontrado.");
        }

        var receita = await _context.Receitas
            .Include(r => r.Itens)
            .ThenInclude(ir => ir.Ingrediente)
            .FirstOrDefaultAsync(r => r.ProdutoId == produtoId);

        if (receita == null)
        {
            throw new ArgumentException("Receita não encontrada para este produto.");
        }

        decimal custoProducao = 0m;
        foreach (var itemReceita in receita.Itens)
        {
            custoProducao += itemReceita.Ingrediente.CustoMedio * (decimal)itemReceita.Quantidade;
        }

        decimal precoVendaDecimal = (decimal)produto.Preco;

        decimal margemBruta = precoVendaDecimal - custoProducao;
        decimal porcentagemMargem = 0m;
        if (precoVendaDecimal > 0)
        {
            porcentagemMargem = (margemBruta / precoVendaDecimal) * 100m;
        }

        return new CustoProdutoDTO
        {
            ProdutoId = produto.Id,
            NomeProduto = produto.Nome,
            PrecoVenda = (double)precoVendaDecimal,
            CustoProducao = (double)custoProducao,
            MargemBruta = (double)margemBruta,
            PorcentagemMargem = (double)porcentagemMargem
        };
    }
}