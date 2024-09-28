namespace Vendas.Domain.Events
{
    public class ItemCanceladoEvent
    {
        public Guid ItemId { get; set; }
        public string Produto { get; set; }
        public int Quantidade { get; set; }
    }
}
