using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.DTOs;
using Vendas.Domain.Entities;
using Vendas.Domain.Events;
using Vendas.Domain.Interfaces;
using Vendas.Domain.Repositories;

namespace Vendas.Data.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VendaService> _logger;
        private readonly IServiceBusPublisher _serviceBusPublisher;
        private const string _topico = "sbq-quee-vendas";

        public VendaService(IVendaRepository vendaRepository, IMapper mapper, ILogger<VendaService> logger, IServiceBusPublisher serviceBusPublisher)
        {
            _vendaRepository = vendaRepository;
            _mapper = mapper;
            _logger = logger;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task<VendaDTO> ObterPorIdAsync(Guid id)
        {
            var venda = await _vendaRepository.ObterPorIdAsync(id);
            return _mapper.Map<VendaDTO>(venda);
        }

        public async Task<IEnumerable<VendaDTO>> ObterTodasAsync()
        {
            var vendas = await _vendaRepository.ObterTodasAsync();
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }

        public async Task AdicionarAsync(VendaDTO vendaDTO)
        {
            ArgumentNullException.ThrowIfNull(vendaDTO);

            var venda = _mapper.Map<Venda>(vendaDTO);

            await _vendaRepository.AdicionarAsync(venda);

            _logger.LogInformation("Publicando evento de venda criada");
            var evento = new CompraCriadaEvent
            {
                CompraId = venda.Id,
                DataCriacao = DateTime.UtcNow,
                Cliente = venda.Cliente,
                ValorTotal = venda.ValorTotal,
                Itens = _mapper.Map<List<ItemVendaDTO>>(venda.Itens)
            };

            await _serviceBusPublisher.PublicarAsync(evento, _topico);
        }

        public async Task AtualizarAsync(VendaDTO vendaDTO)
        {
            var venda = _mapper.Map<Venda>(vendaDTO);
            await _vendaRepository.AtualizarAsync(venda);

            _logger.LogInformation("Publicando evento de venda atualizada");
            var evento = new CompraAlteradaEvent
            {
                CompraId = venda.Id,
                DataAlteracao = DateTime.UtcNow,
                Cliente = venda.Cliente,
                ValorTotal = venda.ValorTotal
            };

            await _serviceBusPublisher.PublicarAsync(evento, _topico);
        }

        public async Task RemoverAsync(Guid id)
        {
            var venda = await _vendaRepository.ObterPorIdAsync(id);
            if (venda != null)
            {
                await _vendaRepository.RemoverAsync(id);

                _logger.LogInformation("Publicando evento de venda removida");
                var evento = new CompraCanceladaEvent
                {
                    CompraId = id,
                    DataCancelamento = DateTime.UtcNow
                };

                await _serviceBusPublisher.PublicarAsync(evento, _topico);
            }
        }
    }
}