namespace Vendas.Domain.DTOs
{
    public class ItemVendaDTO
    {
        public Guid Id { get; set; }
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
