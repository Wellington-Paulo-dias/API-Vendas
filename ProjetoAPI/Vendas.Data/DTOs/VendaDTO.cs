namespace Vendas.Domain.DTOs
{
    public class VendaDTO
    {
        public Guid Id { get; set; }
        public DateTime DataVenda { get; set; }
        public string Cliente { get; set; }
        public string Filial { get; set; }
        public bool Cancelado { get; set; }
        public List<ItemVendaDTO> Itens { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal DescontoTotal { get; set; }
    }
   

}
