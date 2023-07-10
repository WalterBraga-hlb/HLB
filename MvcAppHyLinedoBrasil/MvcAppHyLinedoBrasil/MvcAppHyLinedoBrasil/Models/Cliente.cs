using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHyLinedoBrasil.Models
{
    public class Cliente
    {
        public int ID { get; set; }

        public string EntCod { get; set; }
        public string EntNome { get; set; }
        public string EntNomeFant { get; set; }
        public string CidNomeComp { get; set; }
        public string UfSigla { get; set; }
        public string PaisSigla { get; set; }
        public string Origem { get; set; }

        public string ClienteSelecionado { get; set; }
    }
}