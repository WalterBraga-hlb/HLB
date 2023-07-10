using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MvcAppHylinedoBrasilMobile.Models
{
    public class LayoutDiarioExpedicaoPai
    {
        [DisplayName("Cód. Transp.")]
        public int ID { get; set; }

        public string Granja { get; set; }
        [DisplayName("Carreg.")]
        public DateTime DataHoraCarreg { get; set; }
        [DisplayName("Receb. Inc.")]
	    public DateTime? DataHoraRecebInc { get; set; }
        [DisplayName("Resp. Carreg.")]
	    public string ResponsavelCarreg { get; set; }
        [DisplayName("Resp. Receb.")]
	    public string ResponsavelReceb { get; set; }
        [DisplayName("Data Digitação")]
	    public DateTime? DataHoraDig { get; set; }
        [DisplayName("Núm. N.F.")]
        public string NFNum { get; set; }
        [DisplayName("GTA")]
        public string GTANum { get; set; }
        [DisplayName("Lacre")]
        public string Lacre { get; set; }
        [DisplayName("Tipo DEO")]
        public string TipoDEO { get; set; }
        [DisplayName("Nº Apolo")]
        public string NumIdentificacao { get; set; }
        [DisplayName("Incubatório de Destino")]
        public string IncubatorioDestino { get; set; }
    }
}