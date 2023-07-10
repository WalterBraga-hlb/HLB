using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcAppHyLinedoBrasil.Models
{
    public class LayoutPedidoPlanilha
    {
        public int ID { get; set; }

        public string EmailVendedor { get; set; }
        public string Empresa { get; set; }
        public string Operacao { get; set; }
        public string CodigoCliente { get; set; }
        public string DescricaoCliente { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Vacina { get; set; }
        public int Bouba { get; set; }
        public int Gombouro { get; set; }
        public int Coccidiose { get; set; }
        public int Laringo { get; set; }
        public int Salmonela { get; set; }
        public int TratamentoInfravermelho { get; set; }
        public int QtdePintinhosTratInfraVerm { get; set; }
        public decimal PercPintinhosTratInfraVerm { get; set; }
        public int OvosBrasil { get; set; }
        public string Embalagem { get; set; }
        public string CondicaoPagamento { get; set; }
        public string Observacao { get; set; }
        public string Vendedor { get; set; }
        public string NumeroPedidoRepresentante { get; set; }

        public int NumeroPedidoCHIC { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Linhagem { get; set; }
        public int QtdeLiquida { get; set; }
        public decimal PercBonificacao { get; set; }
        public int QtdeBonificacao { get; set; }
        public int QtdeReposicao { get; set; }
        public int QtdeTotal { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}