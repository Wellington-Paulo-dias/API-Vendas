using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vendas.Domain.DTOs;
using Vendas.Domain.Interfaces;

namespace Vendas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaService _vendaService;
        private readonly ILogger<VendasController> _logger;

        public VendasController(IVendaService vendaService, ILogger<VendasController> logger)
        {
            _vendaService = vendaService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            _logger.LogInformation("Buscando venda por Id: {VendaId}", id);
            var venda = await _vendaService.ObterPorIdAsync(id);
            if (venda == null)
            {
                _logger.LogWarning("Venda não encontrada: {VendaId}", id);
                return NotFound();
            }

            _logger.LogInformation("Venda encontrada: {VendaId}", id);
            return Ok(venda);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodas()
        {
            _logger.LogInformation("Buscando todas as vendas");
            var vendas = await _vendaService.ObterTodasAsync();
            return Ok(vendas);
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar([FromBody] VendaDTO vendaDTO)
        {
            _logger.LogInformation("Adicionando nova venda");
            await _vendaService.AdicionarAsync(vendaDTO);
            _logger.LogInformation("Venda adicionada com sucesso, Id: {VendaId}", vendaDTO.Id);

            return CreatedAtAction(nameof(ObterPorId), new { id = vendaDTO.Id }, vendaDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] VendaDTO vendaDTO)
        {
            _logger.LogInformation("Atualizando venda, Id: {VendaId}", id);
            vendaDTO.Id = id;
            await _vendaService.AtualizarAsync(vendaDTO);
            _logger.LogInformation("Venda atualizada com sucesso, Id: {VendaId}", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            _logger.LogInformation("Deletando venda, Id: {VendaId}", id);
            await _vendaService.RemoverAsync(id);
            _logger.LogInformation("Venda deletada com sucesso, Id: {VendaId}", id);

            return NoContent();
        }
    }

}
