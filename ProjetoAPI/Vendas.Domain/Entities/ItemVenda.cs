namespace Vendas.Domain.Entities
{
    public class ItemVenda
    {
        public Guid Id { get; private set; }
        public string Produto { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public decimal Desconto { get; private set; }

        public decimal ValorTotal => Quantidade * (ValorUnitario - Desconto);

        public ItemVenda(Guid id, string produto, int quantidade, decimal valorUnitario, decimal desconto = 0)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            if (valorUnitario < 0)
                throw new ArgumentException("Valor unitário não pode ser negativo", nameof(valorUnitario));

            if (desconto < 0)
                throw new ArgumentException("Desconto não pode ser negativo", nameof(desconto));

            Id = id;
            Produto = produto;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            Desconto = desconto;
        }
    }

}
