using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHylinedoBrasilMobile.Models
{
    public class RDVMensal
    {
        public string Empresa { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string Usuario { get; set; }
        public string NomeUsuario { get; set; }
        public decimal Valor { get; set; }
        public string NumeroRDV { get; set; }
    }
}