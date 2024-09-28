namespace Vendas.Domain.Events
{
    public class CompraCanceladaEvent
    {
        public Guid CompraId { get; set; }
        public DateTime DataCancelamento { get; set; }
    }
}
