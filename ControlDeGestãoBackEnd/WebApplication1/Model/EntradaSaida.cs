﻿using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Enums;


namespace WebApplication1.Model
{
    
    public class EntradaSaida
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        [ForeignKey("ProdutoId")]
        public CriaProduto? Produto { get; set; }
        public double Quantidade { get; set; }
        public string Tipo { get; set; }

        public UnidadeMedida UnidadeMedida { get; set; }
        public DateTime Data { get; set; }
    }
}
