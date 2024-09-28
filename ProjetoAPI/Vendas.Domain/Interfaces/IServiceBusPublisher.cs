using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Interfaces
{
    public interface IServiceBusPublisher
    {
        Task PublicarAsync<T>(T evento, string topico) where T : class;

    }
}
