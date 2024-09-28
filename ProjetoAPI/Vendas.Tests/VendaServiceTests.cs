using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vendas.API.Mapping;
using Vendas.Data.Services;
using Vendas.Domain.DTOs;
using Vendas.Domain.Entities;
using Vendas.Domain.Events;
using Vendas.Domain.Interfaces;
using Vendas.Domain.Repositories;

public class VendaServiceTests
{
    private readonly Mock<IVendaRepository> _vendaRepositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<VendaService>> _loggerMock;
    private readonly Mock<IServiceBusPublisher> _serviceBusPublisherMock;
    private readonly VendaService _vendaService;
    private const string _topico = "sbq-quee-vendas";


    public VendaServiceTests()
    {
        _vendaRepositoryMock = new Mock<IVendaRepository>();
        _loggerMock = new Mock<ILogger<VendaService>>();
        _serviceBusPublisherMock = new Mock<IServiceBusPublisher>();

        // Configuração do AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VendaProfile>();
        });
        _mapper = config.CreateMapper();

        _vendaService = new VendaService(
            _vendaRepositoryMock.Object,
            _mapper,
            _loggerMock.Object,
            _serviceBusPublisherMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarVendaDTO_QuandoVendaExistir()
    {
        // Arrange
        var vendaId = Guid.NewGuid();
        var venda = new Venda(vendaId, DateTime.UtcNow, "Teste Cliente", "Filial 1");
        venda.AdicionarItem(new ItemVenda(Guid.NewGuid(), "Produto 1", 2, 100m, 10m));
        _vendaRepositoryMock.Setup(repo => repo.ObterPorIdAsync(vendaId)).ReturnsAsync(venda);

        // Act
        var result = await _vendaService.ObterPorIdAsync(vendaId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(vendaId);
        result.Cliente.Should().Be("Teste Cliente");
        result.ValorTotal.Should().Be(180m);  // (2 * 90) = 180
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoVendaNaoExistir()
    {
        // Arrange
        var vendaId = Guid.NewGuid();
        _vendaRepositoryMock.Setup(repo => repo.ObterPorIdAsync(vendaId)).ReturnsAsync((Venda?)null);

        // Act
        var result = await _vendaService.ObterPorIdAsync(vendaId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodasAsync_DeveRetornarListaDeVendaDTOs()
    {
        // Arrange
        var vendas = new List<Venda>
        {
            new Venda(Guid.NewGuid(), DateTime.UtcNow, "Teste Cliente 1", "Filial 1"),
            new Venda(Guid.NewGuid(), DateTime.UtcNow, "Teste Cliente 2", "Filial 2")
        };
        vendas[0].AdicionarItem(new ItemVenda(Guid.NewGuid(), "Produto 1", 2, 100m, 10m));
        vendas[1].AdicionarItem(new ItemVenda(Guid.NewGuid(), "Produto 2", 1, 200m, 20m));

        _vendaRepositoryMock.Setup(repo => repo.ObterTodasAsync()).ReturnsAsync(vendas);

        // Act
        var result = await _vendaService.ObterTodasAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);
        result.ElementAt(0).Cliente.Should().Be("Teste Cliente 1");
        result.ElementAt(0).ValorTotal.Should().Be(180m);  // (2 * 90) = 180
        result.ElementAt(1).Cliente.Should().Be("Teste Cliente 2");
        result.ElementAt(1).ValorTotal.Should().Be(180m);  // (1 * 180) = 180
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarVendaEPublicarEvento()
    {
        // Arrange
        var vendaDTO = new VendaDTO
        {
            Id = Guid.NewGuid(),
            Cliente = "Teste Cliente",
            Filial = "Filial 1",
            Itens = new List<ItemVendaDTO>
            {
                new ItemVendaDTO { Produto = "Produto 10", Quantidade = 2, ValorUnitario = 200m, Desconto = 10m }
            }
        };

        // Act
        await _vendaService.AdicionarAsync(vendaDTO);

        // Assert
        _vendaRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Venda>()), Times.Once);
        _serviceBusPublisherMock.Verify(pub => pub.PublicarAsync(It.IsAny<CompraCriadaEvent>(), _topico), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarVendaEPublicarEvento()
    {
        // Arrange
        var vendaDTO = new VendaDTO
        {
            Id = Guid.NewGuid(),
            Cliente = "Teste Cliente Atualizado",
            Filial = "Filial 1",
            Itens = new List<ItemVendaDTO>
            {
                new ItemVendaDTO { Produto = "Produto 1", Quantidade = 2, ValorUnitario = 100m, Desconto = 10m }
            }
        };

        // Act
        await _vendaService.AtualizarAsync(vendaDTO);

        // Assert
        _vendaRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Venda>()), Times.Once);
        _serviceBusPublisherMock.Verify(pub => pub.PublicarAsync(It.IsAny<CompraAlteradaEvent>(), _topico), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverVendaEPublicarEvento_QuandoVendaExistir()
    {
        // Arrange
        var vendaId = Guid.NewGuid();
        var venda = new Venda(vendaId, DateTime.UtcNow, "Teste Cliente", "Filial 1");
        venda.AdicionarItem(new ItemVenda(Guid.NewGuid(), "Produto 1", 2, 100m, 10m));

        _vendaRepositoryMock.Setup(repo => repo.ObterPorIdAsync(vendaId)).ReturnsAsync(venda);

        // Act
        await _vendaService.RemoverAsync(vendaId);

        // Assert
        _vendaRepositoryMock.Verify(repo => repo.RemoverAsync(vendaId), Times.Once);
        _serviceBusPublisherMock.Verify(pub => pub.PublicarAsync(It.IsAny<CompraCanceladaEvent>(), _topico), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_NaoDevePublicarEvento_QuandoVendaNaoExistir()
    {
        // Arrange
        var vendaId = Guid.NewGuid();
        _vendaRepositoryMock.Setup(repo => repo.ObterPorIdAsync(vendaId)).ReturnsAsync((Venda?)null);

        // Act
        await _vendaService.RemoverAsync(vendaId);

        // Assert
        _vendaRepositoryMock.Verify(repo => repo.RemoverAsync(vendaId), Times.Never);
        _serviceBusPublisherMock.Verify(pub => pub.PublicarAsync(It.IsAny<CompraCanceladaEvent>(), _topico), Times.Never);
    }
}
