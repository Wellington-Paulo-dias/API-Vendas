using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Entities;

namespace Vendas.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações para a entidade Venda
            modelBuilder.Entity<Venda>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cliente)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Filial)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.HasMany(e => e.Itens)
                      .WithOne()
                      .OnDelete(DeleteBehavior.NoAction);
                entity.Ignore(v => v.ValorTotal);
                entity.Ignore(v => v.DescontoTotal);
            });

            // Configurações para a entidade ItemVenda
            modelBuilder.Entity<ItemVenda>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Produto)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Quantidade)
                      .IsRequired();
                entity.Property(e => e.ValorUnitario)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.Desconto)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");
            });


        }
    }
}
