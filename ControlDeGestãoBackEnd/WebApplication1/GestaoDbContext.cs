﻿using Microsoft.EntityFrameworkCore;
using WebApplication1.Model;

namespace Database;

   
   
  public partial class GestaoDbContext : DbContext
  {
   
    public DbSet<EntradaSaida> Lancamentos { get; set; }
    public DbSet<CriaProduto> Produtos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Receita> Receitas { get; set; }
    public DbSet<Ingrediente> Ingredientes { get; set; }
    public DbSet<ItemReceita> ItemReceitas { get; set; }
    public DbSet<CriaPedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }
    public DbSet<LancamentoIngrediente> LancamentosIngredientes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }







    public GestaoDbContext(DbContextOptions<GestaoDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=ControleGestao;Username=postgres;Password=1234");
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


}




