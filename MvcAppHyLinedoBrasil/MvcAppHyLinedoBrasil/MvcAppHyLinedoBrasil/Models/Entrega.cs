using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHyLinedoBrasil.Models
{
    public class Entrega
    {
        public int ID { get; set; }

        public DateTime DataNascimento { get; set; }
        public string Caminhao { get; set; }
        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Bau { get; set; }
    }
}