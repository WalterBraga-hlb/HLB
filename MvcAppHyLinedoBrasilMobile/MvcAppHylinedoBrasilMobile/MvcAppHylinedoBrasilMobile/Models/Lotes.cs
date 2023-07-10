using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHylinedoBrasilMobile.Models
{
    public class Lotes
    {
        public string Granja { get; set; }
        public string LoteCompleto { get; set; }
        public string NumeroLote { get; set; }
        public string Linhagem { get; set; }
        public string Galpao { get; set; }
        public string Location { get; set; }
        public DateTime DataNascimento { get; set; }
        public string TipoOvo { get; set; }
        public string DescricaoTipoOvo { get; set; }
        public int Saldo { get; set; }
        public string SaldoString { get; set; }
        public int Idade { get; set; }
    }
}