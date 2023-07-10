using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHylinedoBrasilMobile.Models
{
    public class VacinaSecundaria
    {
        public string CodigoVacinaApolo { get; set; }
        public string NomeComercialVacina { get; set; }
        public bool Bonificacao { get; set; }
        public bool ClienteEnvia { get; set; }
        public string Preco { get; set; }
        public bool ExistePV { get; set; }
    }
}