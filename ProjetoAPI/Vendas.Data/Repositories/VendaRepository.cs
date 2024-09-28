using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Data.Context;
using Vendas.Domain.Entities;
using Vendas.Domain.Repositories;

namespace Vendas.Data.Repositories
{
    public class VendaRepository: IVendaRepository
    {
        private readonly AppDbContext _context;
        public VendaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> ObterPorIdAsync(Guid id)
        {
            return await _context.Set<Venda>().Include(v => v.Itens).FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Venda>> ObterTodasAsync()
        {
            return await _context.Set<Venda>().Include(v => v.Itens).ToListAsync();
        }

        public async Task AdicionarAsync(Venda venda)
        {
            ArgumentNullException.ThrowIfNull(venda);
            await _context.AddAsync(venda);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Venda venda)
        {
            ArgumentNullException.ThrowIfNull(venda);

            _context.Set<Venda>().Update(venda);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var venda = await ObterPorIdAsync(id);
            if (venda != null)
            {
                _context.Set<Venda>().Remove(venda);
                await _context.SaveChangesAsync();
            }
        }
    }
}
