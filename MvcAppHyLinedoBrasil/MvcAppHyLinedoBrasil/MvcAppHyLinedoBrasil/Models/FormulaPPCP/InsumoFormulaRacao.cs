using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHyLinedoBrasil
{
    public class InsumoFormulaRacao
    {
        public string ProdCodEstr { get; set; }
        public Decimal QtdeKg { get; set; }
        public Decimal PercAjusteQuebra { get; set; }
        public DateTime DataIni { get; set; }
        public DateTime? DataFim { get; set; }
    }
}