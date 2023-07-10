using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ConsoleApplication1.Data;
//using ConsoleApplication1.Data.CHICDataSetTableAdapters;
using System.Data.Common;
using System.Data.OleDb;
using System.Configuration;
using ImportaCHICService;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int teste = DateTime.Now.Hour;

            ImportaCHIC servico = new ImportaCHIC();

            //servico.ImportaPedidosCHIC();
            //servico.ImportaClientesAPOLO();
            //servico.AtulizaStatusPedidoCHIC();
            //servico.EnviaEmailsLogin();
            //servico.AtualizaPrecoPedidos();
            //servico.AjustaErroPrecoPedidos();
            //servico.EnviarVerificacaoFinal();
            //servico.EnviaPedidosCHIC();
            //servico.AtualizaWEBxCHIC();
            //servico.AjustaPrecosPedidosCHICparaWEB();
            //servico.AtualizaEnderecoEntregaCHICtoApolo();
            //servico.ImportaPedidosCHIC("89061");
            //servico.GeraRelatorioVerificacaoFinal("*" + "000101" + "*", true, 
            //    "\\\\Srv-app-01\\w\\Relatorios_CHIC\\Verificacao_Final",
            //    "\\\\Srv-app-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Teste_PH.xlsx",
            //    DateTime.Today,
            //    "BR", "BR", "000101", "GLAUCO E. GEROMINI / ES", "Verificacao_Final",
            //    "Vendedor");
            //DateTime data = DateTime.Today;
            //servico.InserePedidosCHICProgDiariaTransp(data);
            //servico.EnviaCurriculosFTPEmail();
            //servico.EnviarVerificacaoFinalPlanalto();
            //servico.EnviarVerificacaoFinalTeste();
            //string emailsCopiaGranja = "cgamboa@hyline.com.br;dmelo@hyline.com.br;"
            //                + "lgasparino@hyline.com.br;cbarros@hyline.com.br;egarcia@hyline.com.br;"
            //                + "snociti@hyline.com.br;mgomes@hyline.com.br";
            //servico.EnviarDiasEstoqueGranjas(emailsCopiaGranja, "Matrizes");
            //string emailsCopiaIncubatorios = "bvieira@hyline.com.br;aneves@planaltopostura.com.br;"
            //                + "sdoimo@hyline.com.br;mgomes@hyline.com.br;lribeiro@planaltopostura.com.br";
            //servico.EnviarDiasEstoqueIncubatorios(emailsCopiaIncubatorios, "Matrizes");
            //string emailsCopiaGranjaAvos = "aprates@hyline.com.br;lalmeida@hyline.com.br";
            //servico.EnviarDiasEstoqueGranjas(emailsCopiaGranjaAvos, "Avós");
            //string emailsCopiaIncubatoriosAvos = "rsilva@hyline.com.br";
            //servico.EnviarDiasEstoqueIncubatorios(emailsCopiaIncubatoriosAvos, "Avós");
            //servico.SendReportLossWeeklyMatriz();
            //DateTime data = Convert.ToDateTime("05/04/2021");
            //servico.AtualizarProgDiariaTranspDiaNascimento(data);
            //servico.AtualizaPedidosVendidosWEBxCHIC();
            //servico.AtualizaPedidosCHICNovoModelo();
            //servico.AtualizaPedidosReposicaoWEBxCHIC();
            //servico.AtualizaQtdeVacinasServicosPedidosCHICNovoModelo();
            //servico.ImportaPedidosEmbarcador();
            //servico.ImportaPedidoEmbarcador("79291", "Automático");

            //DateTime dataInicial = DateTime.Today.AddDays(-30);
            //servico.AtualizarProgDiariaTranspDiaNascimentoPeriodo(dataInicial, DateTime.Today);

            //DateTime dataInicial = DateTime.Today;
            //DateTime dataFinal = dataInicial.AddYears(5);
            //servico.ImportacaoProgDiariaTranspCHICPeriodo(dataInicial, dataFinal);

            //servico.EnviarProgramacaoDiariaTransportesSemanal();

            //servico.ChamadaTeste();

            //servico.TesteAtualizacao();

            //servico.AtualizaWEBxCHIC();

            //servico.AtualizaPedidosLTZParaCH();

            //servico.AtualizaPrecoTratamentoInfravermelhoChamado31610();

            //servico.AtualizaPrecoVacinasChamado35671();

            //servico.EnviarVerificacaoFinalAniPlan();
            servico.EnviarVerificacaoFinalPlanaltoAniPlan();
        }
    }
}
