using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vendas.Domain.Interfaces;

namespace Vendas.Data.Services
{
    public class ServiceBusPublisher : IServiceBusPublisher
    {
        private readonly ServiceBusClient _client;

        public ServiceBusPublisher(string connectionString)
        {
            _client = new ServiceBusClient(connectionString);
        }

        public async Task PublicarAsync<T>(T evento, string topico) where T : class
        {
            if (string.IsNullOrEmpty(topico))
            {
                throw new ArgumentException("O nome do tópico não pode ser nulo ou vazio.", nameof(topico));
            }

            ServiceBusSender sender = _client.CreateSender(topico);

            try
            {
                string messageBody = JsonSerializer.Serialize(evento);
                ServiceBusMessage message = new ServiceBusMessage(messageBody);

                await sender.SendMessageAsync(message);
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }
    }
}
