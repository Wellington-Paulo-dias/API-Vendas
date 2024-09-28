using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Vendas.API.Mapping;
using Vendas.Data.Services;
using Vendas.Domain.DTOs;
using Vendas.Domain.Events;
using Vendas.Domain.Repositories;

public class ServiceBusTests : IAsyncLifetime
{
    private const string ServiceBusConnectionString = "serive-bus-connection-string";
    private const string TopicName = "sbq-quee-vendas";
    private ServiceBusClient _serviceBusClient;
    private ServiceBusSender _sender;
    private ServiceBusReceiver _receiver;

    private VendaService _vendaService;

    public async Task InitializeAsync()
    {
        // Inicializa o cliente do Azure Service Bus
        _serviceBusClient = new ServiceBusClient(ServiceBusConnectionString);
        _sender = _serviceBusClient.CreateSender(TopicName);
        _receiver = _serviceBusClient.CreateReceiver(TopicName, new ServiceBusReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        });

        // Configura o serviço de vendas com os mocks necessários
        var vendaRepositoryMock = new Mock<IVendaRepository>();
        // Configurar o mock do IMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VendaProfile>();
        });
        var mapper = mapperConfig.CreateMapper();
        var loggerMock = new Mock<ILogger<VendaService>>();
        var serviceBusPublisher = new ServiceBusPublisher(ServiceBusConnectionString);
        // Configurar IConfiguration para os testes
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ServiceBus:TopicoVendas", TopicName }
            })
            .Build();
        _vendaService = new VendaService(
            vendaRepositoryMock.Object,
            mapper,
            loggerMock.Object,
            serviceBusPublisher,
            configuration
        );
    }

    public async Task DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _receiver.DisposeAsync();
        await _serviceBusClient.DisposeAsync();
    }

    [Fact]
    public async Task DevePublicarEventoCompraCriadaNoServiceBus()
    {
        // Limpar mensagens residuais
        while (true)
        {
            var receivedMessages = await _receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));
            if (receivedMessages.Count == 0)
            {
                break;
            }
        }

        // Arrange
        var vendaDTO = new VendaDTO
        {
            Id = Guid.NewGuid(),
            Cliente = "Teste Cliente Service Bus 1",
            Filial = "Filial Centro",
            Itens = new List<ItemVendaDTO>
            {
                new ItemVendaDTO { Produto = "Produto A", Quantidade = 2, ValorUnitario = 100.00m, Desconto = 10.00m }
            }
        };

        // Act
        await _vendaService.AdicionarAsync(vendaDTO);

        // Assert
        var allReceivedMessages = new List<ServiceBusReceivedMessage>();
        while (true)
        {
            var receivedMessages = await _receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(10));
            if (receivedMessages.Count == 0)
            {
                break;  
            }
            allReceivedMessages.AddRange(receivedMessages);
        }

        Assert.NotEmpty(allReceivedMessages);

        // Verificar se apenas uma mensagem existe
        Assert.Single(allReceivedMessages);

        var receivedMessage = allReceivedMessages.First();
        var compraCriadaEvent = JsonSerializer.Deserialize<CompraCriadaEvent>(receivedMessage.Body.ToString());
        Assert.NotNull(compraCriadaEvent);
        Assert.Equal(vendaDTO.Cliente, compraCriadaEvent.Cliente);
        Assert.Equal(180.00m, compraCriadaEvent.ValorTotal); // (2 * (100 - 10)) = 180
    }
}
