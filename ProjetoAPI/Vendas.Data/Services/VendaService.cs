using AutoMapper;
using Microsoft.Extensions.Configuration;
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
        private readonly string _topico;

        public VendaService(IVendaRepository vendaRepository, 
            IMapper mapper,
            ILogger<VendaService> logger, 
            IServiceBusPublisher serviceBusPublisher,
            IConfiguration configuration)
        {
            _vendaRepository = vendaRepository;
            _mapper = mapper;
            _logger = logger;
            _serviceBusPublisher = serviceBusPublisher;
            _topico = configuration["ServiceBus:TopicoVendas"]!;
        }

        public async Task<VendaDTO> ObterPorIdAsync(Guid id)
        {
            try
            {
                var venda = await _vendaRepository.ObterPorIdAsync(id);
                return _mapper.Map<VendaDTO>(venda);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter venda por ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<VendaDTO>> ObterTodasAsync()
        {
            try
            {
                var vendas = await _vendaRepository.ObterTodasAsync();
                return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as vendas");
                throw;
            }
        }

        public async Task AdicionarAsync(VendaDTO vendaDTO)
        {
            ArgumentNullException.ThrowIfNull(vendaDTO);

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar venda");
                throw;
            }
        }

        public async Task AtualizarAsync(VendaDTO vendaDTO)
        {
            ArgumentNullException.ThrowIfNull(vendaDTO);

            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar venda");
                throw;
            }
        }

        public async Task RemoverAsync(Guid id)
        {
            try
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
                else
                {
                    _logger.LogWarning("Venda com ID {Id} não encontrada", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover venda com ID {Id}", id);
                throw;
            }
        }
    }
}