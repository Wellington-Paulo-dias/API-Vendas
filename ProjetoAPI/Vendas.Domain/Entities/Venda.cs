namespace Vendas.Domain.Entities
{
    public class Venda
    {
        public Guid Id { get; private set; }
        public DateTime DataVenda { get; private set; }
        public string Cliente { get; private set; }
        public string Filial { get; private set; }
        public bool Cancelado { get; private set; }

        private readonly List<ItemVenda> _itens = new();

        public IEnumerable<ItemVenda> Itens => _itens;

        public decimal ValorTotal => CalcularValorTotal();
        public decimal DescontoTotal => CalcularDescontoTotal();

        public Venda(Guid id, DateTime dataVenda, string cliente, string filial)
        {
            Id = id;
            DataVenda = dataVenda;
            Cliente = cliente;
            Filial = filial;
            Cancelado = false;
        }

        public void AdicionarItem(ItemVenda item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _itens.Add(item);
        }

        public void RemoverItem(ItemVenda item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _itens.Remove(item);
        }

        public void CancelarVenda()
        {
            Cancelado = true;
        }

        private decimal CalcularValorTotal()
        {
            return _itens.Sum(item => item.ValorTotal);
        }

        private decimal CalcularDescontoTotal()
        {
            return _itens.Sum(item => item.Desconto);
        }
    }

}
