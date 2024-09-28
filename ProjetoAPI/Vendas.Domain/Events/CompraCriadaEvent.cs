using Vendas.Domain.DTOs;

namespace Vendas.Domain.Events
{
    public class CompraCriadaEvent
    {
        public Guid CompraId { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Cliente { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemVendaDTO> Itens { get; set; }
    }
}
