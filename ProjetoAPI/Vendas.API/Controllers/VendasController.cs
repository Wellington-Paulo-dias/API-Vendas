using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        /// <summary>
        /// Obter venda por ID.
        /// </summary>
        /// <param name="id">ID da venda.</param>
        /// <returns>Retorna uma venda específica pelo ID.</returns>
        [HttpGet("{id}")]        
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar venda por Id: {VendaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter todas as vendas.
        /// </summary>
        /// <returns>Retorna uma lista de todas as vendas.</returns>
        [HttpGet]
           public async Task<IActionResult> BuscarTodas()
        {
            try
            {
                _logger.LogInformation("Buscando todas as vendas");
                var vendas = await _vendaService.ObterTodasAsync();
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as vendas");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Adicionar nova venda.
        /// </summary>
        /// <param name="vendaDTO">Dados da venda a ser adicionada.</param>
        /// <returns>Retorna a venda criada.</returns>
        [HttpPost]    
        public async Task<IActionResult> Adicionar([FromBody] VendaDTO vendaDTO)
        {
            try
            {
                _logger.LogInformation("Adicionando nova venda");
                await _vendaService.AdicionarAsync(vendaDTO);
                _logger.LogInformation("Venda adicionada com sucesso, Id: {VendaId}", vendaDTO.Id);

                return CreatedAtAction(nameof(ObterPorId), new { id = vendaDTO.Id }, vendaDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar nova venda");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualizar venda.
        /// </summary>
        /// <param name="id">ID da venda a ser atualizada.</param>
        /// <param name="vendaDTO">Dados atualizados da venda.</param>
        /// <returns>Retorna o status da operação.</returns>
        [HttpPut("{id}")]     
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] VendaDTO vendaDTO)
        {
            try
            {
                _logger.LogInformation("Atualizando venda, Id: {VendaId}", id);
                vendaDTO.Id = id;
                await _vendaService.AtualizarAsync(vendaDTO);
                _logger.LogInformation("Venda atualizada com sucesso, Id: {VendaId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar venda, Id: {VendaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Remover venda.
        /// </summary>
        /// <param name="id">ID da venda a ser removida.</param>
        /// <returns>Retorna o status da operação.</returns>
        [HttpDelete("{id}")]       
        public async Task<IActionResult> Deletar(Guid id)
        {
            try
            {
                _logger.LogInformation("Deletando venda, Id: {VendaId}", id);
                await _vendaService.RemoverAsync(id);
                _logger.LogInformation("Venda deletada com sucesso, Id: {VendaId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar venda, Id: {VendaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor");
            }
        }
    }

}
