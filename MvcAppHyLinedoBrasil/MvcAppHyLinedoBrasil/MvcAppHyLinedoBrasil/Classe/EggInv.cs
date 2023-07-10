using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHyLinedoBrasil.Classe
{
    public class EggInv
    {
        public string Company { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string Hatch_Loc { get; set; }
        public string Farm_ID { get; set; }
        public string Variety { get; set; }
        public string Flock_ID { get; set; }
        public string Track_NO { get; set; }
        public DateTime Lay_Date { get; set; }
        public decimal Egg_Units { get; set; }
        public string Status { get; set; }
        public string Flock_Key { get; set; }        
    }
}