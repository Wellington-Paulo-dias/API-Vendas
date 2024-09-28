namespace Vendas.Domain.Events
{
    public class CompraAlteradaEvent
    {
        public Guid CompraId { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Cliente { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
