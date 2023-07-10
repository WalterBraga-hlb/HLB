using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHyLinedoBrasil.Classe
{
    public class HatcheryEgg
    {
        public string Company { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string Hatch_Loc { get; set; }
        public string Flock_ID { get; set; }
        public string Track_NO { get; set; }
        public DateTime Lay_Date { get; set; }
        public DateTime Set_Date { get; set; }
        public float Eggs_Rcvd { get; set; }
        public float Egg_Key { get; set; }
        public string Machine { get; set; }
        public int Posicao { get; set; }
        public string Horario { get; set; }
        public int bandejas { get; set; }
        public float PesoOvo { get; set; }
        public float PesoEntInc { get; set; }
        public float PesoTransf { get; set; }
        public float PesoRent { get; set; }
    }
}