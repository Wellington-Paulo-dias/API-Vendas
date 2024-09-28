using Testcontainers.MySql;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Vendas.Data.Repositories;
using Vendas.Data.Services;
using Vendas.Domain.DTOs;
using Vendas.Data.Context;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Vendas.Domain.Interfaces;
using Vendas.API.Mapping;
using Microsoft.Extensions.Configuration;

public class VendaApiIntegrationTests : IAsyncLifetime
{
    private readonly MySqlContainer _dbContainer;

    public VendaApiIntegrationTests()
    {
        _dbContainer = new MySqlBuilder()
            .WithDatabase("VendaApiDb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Configure seu contexto de banco de dados para usar a string de conexão do contêiner MySQL
        var connectionString = _dbContainer.GetConnectionString();

        // Aqui você pode aplicar migrações ao banco de dados
        await ApplyMigrations(connectionString);
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    private async Task ApplyMigrations(string connectionString)
    {
        // Aplique suas migrações de Entity Framework aqui
        // Por exemplo, se você estiver usando um DbContext, você pode fazer:
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();
    }

    [Fact]
    public async Task DeveInserirEVenderProdutoCorretamente()
    {
        // Exemplo de teste que usa o banco de dados
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(_dbContainer.GetConnectionString(), ServerVersion.AutoDetect(_dbContainer.GetConnectionString()))
            .Options;

        using var context = new AppDbContext(options);

        // Configurar o mock do IMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VendaProfile>();
        });
        var mapper = mapperConfig.CreateMapper();


        // Criar mocks para os serviços necessários
        //var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<VendaService>>();
        var serviceBusPublisherMock = new Mock<IServiceBusPublisher>();
        // Configurar IConfiguration para os testes
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ServiceBus:TopicoVendas", "sbq-quee-vendas" }
            })
            .Build();

        var vendaService = new VendaService(
            new VendaRepository(context),
            mapper,
            loggerMock.Object,
            serviceBusPublisherMock.Object,
            configuration
        );

        var vendaDTO = new VendaDTO
        {
            Cliente = "João da Silva",
            Filial = "Filial Centro",
            DataVenda = DateTime.Now,
            Cancelado = false,
            Itens = new List<ItemVendaDTO>
            {
                new ItemVendaDTO {
                    Produto = "Produto A",
                    Quantidade = 2,
                    ValorUnitario = 100.00m,
                    Desconto = 10.00m,
                    ValorTotal = 0
                }
            }
            ,
            ValorTotal = 0,
            DescontoTotal = 0


        };

        await vendaService.AdicionarAsync(vendaDTO);

        // Valide se a venda foi inserida corretamente
        var venda = await context.Vendas.FirstOrDefaultAsync(v => v.Cliente == "João da Silva");
        Assert.NotNull(venda);
        Assert.Equal(180.00m, venda.ValorTotal); // (2 * (100 - 10)) = 180
    }
}
