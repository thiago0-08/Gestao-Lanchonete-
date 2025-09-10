﻿using System.ComponentModel.DataAnnotations;
using WebApplication1.Model;
using WebApplication1.Enums;


namespace WebApplication1.DTO
{
    public class EntradaSaidaDTO
    {
        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public decimal Quantidade { get; set; }

        [Required]
        [RegularExpression("entrada|saida", ErrorMessage = "Tipo deve ser 'entrada' ou 'saida'.")]
        public string Tipo { get; set; }

        [Required]
        public UnidadeMedida UnidadeMedida { get; set; }
    }
}
