using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MvcAppHylinedoBrasilMobile.Models
{
    public class LayoutDiarioExpedicao
    {
        public int ID { get; set; }

        [DisplayName("Carreg.")]
        public DateTime DataHoraCarreg { get; set; }
        [DisplayName("Receb. Inc.")]
        public DateTime DataHoraRecebInc { get; set; }
        [DisplayName("Resp. Carreg.")]
        public string ResponsavelCarreg { get; set; }
        [DisplayName("Resp. Receb.")]
        public string ResponsavelReceb { get; set; }
        [DisplayName("Núm. N.F.")]
        public string NFNum { get; set; }
        [DisplayName("Selecione a Granja:")]
        public string Granja { get; set; }
        [DisplayName("GTA")]
        public string GTANum { get; set; }
        [DisplayName("Lacre")]
        public string Lacre { get; set; }
        [DisplayName("Observação")]
        public string Observacao { get; set; }

        public string TipoDEO { get; set; }
        public string Incubatorio { get; set; }
        public string Importado { get; set; }
        public string Nucleo { get; set; }
        public string Galpao { get; set; }
        public string Lote { get; set; }
        public int Idade { get; set; }
        public string Linhagem { get; set; }
        public string LoteCompleto { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Data de Produção")]
        public DateTime DataProducao { get; set; }
        public string NumeroReferencia { get; set; }
        public decimal QtdeOvos { get; set; }
        public decimal QtdeBandejas { get; set; }
        public string Usuario { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataHora { get; set; }
        public string NumIdentificacao { get; set; }
        public int CodItemDEO { get; set; }

        public string TipoOvo { get; set; }
    }
}