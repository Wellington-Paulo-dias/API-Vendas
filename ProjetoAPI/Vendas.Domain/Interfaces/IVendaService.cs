using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.DTOs;

namespace Vendas.Domain.Interfaces
{
    public interface IVendaService
    {
        Task<VendaDTO> ObterPorIdAsync(Guid id);
        Task<IEnumerable<VendaDTO>> ObterTodasAsync();
        Task AdicionarAsync(VendaDTO vendaDTO);
        Task AtualizarAsync(VendaDTO vendaDTO);
        Task RemoverAsync(Guid id);
    }
}
