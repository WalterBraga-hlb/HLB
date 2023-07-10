using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHyLinedoBrasil.Models.XML
{
    public class det
    {
        // prod
        public string NCM { get; set; }
        public string Descricao { get; set; }
        public string UnidadeMedida { get; set; }
        public decimal Qtde { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotalProduto { get; set; }

        // ICMS
        public string IcmsCST { get; set; }
        public decimal IcmsBC { get; set; }
        public decimal IcmsPerc { get; set; }
        public decimal IcmsValor { get; set; }

        // PIS
        public string PisCST { get; set; }
        public decimal PisBC { get; set; }
        public decimal PisPerc { get; set; }
        public decimal PisValor { get; set; }

        // COFINS
        public string CofinsCST { get; set; }
        public decimal CofinsBC { get; set; }
        public decimal CofinsPerc { get; set; }
        public decimal CofinsValor { get; set; }
    }
}