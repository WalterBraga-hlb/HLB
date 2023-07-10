using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcAppHyLinedoBrasil.Models
{
    public class LayoutDDASegmentoG
    {
        public int ID { get; set; }

        // Dados de Controle
        public string BancoCompensacao { get; set; }
        public int Lote { get; set; }
        public string Registro { get; set;}
        public int NumeroRegistro { get; set; }
        public string Segmento { get; set; }
        public string CNAB { get; set; }
        public string Movimento { get; set; }

        // Dados de Título
        public string Empresa { get; set; }
        public string BancoCedente { get; set; }
        public int CodigoMoeda { get; set; }
        public int DigitoVerificadorCodigoBarras { get; set; }
        public string ValorImpressoCodigoBarras { get; set; }
        public string CampoLivre { get; set; }
        public int TipoInscricao { get; set; }
        public string Inscricao { get; set; }
        public string NomeCedente { get; set; }
        public DateTime DataVencimento { get; set; }
        public Decimal ValorTitulo { get; set; }
        public Decimal QuantidadeMoeda { get; set; }
        public string NumeroDocumento { get; set; }
        public string Filler { get; set; }
        public Decimal ValorAbatimento { get; set; }
        public string CodigoCarteira { get; set; }
        public int EspecieTitulo { get; set; }
        public DateTime DataEmissaoTitulo { get; set; }
        public Decimal JurosMora { get; set; }
        public int CodigoDesconto1 { get; set; }
        public DateTime DataDesconto1 { get; set; }
        public Decimal Desconto1 { get; set; }
        public int CodigoProtesto { get; set; }
        public int PrazoProtesto { get; set; }
        public DateTime DataLimite { get; set; }
        public string LinhaDigitavel { get; set; }
        public string Sacador { get; set; }

        // Outros Dados
        public string TituloNoApolo { get; set; }
        public string ImportaNoApolo { get; set; }
        public string EnviaEmailFiscal { get; set; }
        public string Retorno { get; set; }
        public string UserSession { get; set; }
        public string EmpresaApolo { get; set; }
        public int ChaveDocApolo { get; set; }
        public int? SeqDocApolo { get; set; }
        public int? SeqDesmPagDocApolo { get; set; }
        public string DupNumDocApolo { get; set; }
    }
}