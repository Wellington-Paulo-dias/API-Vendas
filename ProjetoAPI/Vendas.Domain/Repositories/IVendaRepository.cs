using Vendas.Domain.Entities;

namespace Vendas.Domain.Repositories
{
    public interface IVendaRepository
    {
        Task<Venda> ObterPorIdAsync(Guid id);
        Task<List<Venda>> ObterTodasAsync();
        Task AdicionarAsync(Venda venda);
        Task AtualizarAsync(Venda venda);
        Task RemoverAsync(Guid id);
    }
}
