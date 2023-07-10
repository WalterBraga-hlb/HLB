using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHyLinedoBrasil.Models
{
    public class ArquivosLidos
    {
        public int ID { get; set; }

        public string Arquivo { get; set; }
        public DateTime DataLeitura { get; set; }
    }
}