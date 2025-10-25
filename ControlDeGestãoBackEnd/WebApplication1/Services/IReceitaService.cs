using WebApplication1.DTO;
using WebApplication1.Model;

public interface IReceitaService
{
    Task<ReceitaDetalhadaDTO> CriarReceitaAsync(ReceitaDTO dto);
    Task<ReceitaDetalhadaDTO> AtualizarReceitaAsync(ReceitaDTO dto);
    Task<ReceitaDetalhadaDTO?> ObterReceitaPorIdAsync(int id);
    Task<CustoProdutoDTO> CalcularCustoProdutoAsync(int produtoId);
    Task<IEnumerable<Receita>> ObterTodasReceitasAsync();
}

