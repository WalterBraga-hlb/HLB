using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using EnviaRelatorioAutomatico;
using System.Diagnostics;
using ImportaIncubacao;

namespace ConsoleTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            /**** REL CHIC ****/
            //Service1 servico = new Service1();

            ////servico.AtualizaRelatorioVendasCHIC(linhagens, dataInicial, dataFinal);
            //try
            //{
            //    servico.EnviaRelatorioVendasCHIC("dnogueira@hyline.com.br", "BR", "Davi Nogueira");
            //    servico.EnviaRelatorioVendasCHIC("dnogueira@hyline.com.br", "LB", "Davi Nogueira");
            //    servico.EnviaRelatorioVendasCHIC("dnogueira@hyline.com.br", "HN", "Davi Nogueira");
            //}
            //catch (Exception e)
            //{
            //    EventLog.WriteEntry("EnviaRelatorioAutomaticoCHIC", "Erro: " + e.Message);
            //}

            ImportaIncubacaoService servico = new ImportaIncubacaoService();

            try
            {
                //servico.ImportaIncubacaoFLIP();
                //servico.AtualizaIdadeLinhagens();
                //servico.ReintegraEggInvComProblema("PP");
                //servico.ReintegraEggInvComProblema("GP");
                //servico.InsereProducaoEstoqueApolo();
                //servico.InventarioOvos();
                //servico.InsereProducaoEstoqueApoloPorGranja("JRP03", "PP", "1", "NÃO", "E0000483");
                //servico.RetornaNumeroGalpao("P115251HB");
                //servico.AjustaEggInvFLIP();
                //servico.AjustaEggInvFLIPNegativo();
                //servico.AjusteLotesNegativosApolo();
                //servico.InsereProducaoEstoqueApolo();
                //servico.AjustaTabelaImportaDEOs();
                //servico.RefazEstoqueApolo();
                //servico.AtualizaTabelaSaldo();
                //servico.DeletaMovAjuste();
                //servico.AjustaTabelaImportaDEOs();
                //servico.AjustaIncubacoes();
                //servico.ReinciarServico();
                //servico.AjustaDiarioProducaoPlanalto();
                //servico.ImportaDadosNascimentoFLIPparaWEB();
                //servico.AjustaDiarioProducaoJeriquara();
                servico.AtualizaTodasIncubacoesWEBparaFLIP();
                //servico.AtualizaNascimentosWEBparaFLIPNM();

                //string granja = "JD";
                //DateTime dataHoraCarreg = Convert.ToDateTime("2016-06-15 16:55:00.000");
                //servico.ImportaDEOApolo(granja, dataHoraCarreg);

                //servico.AjustaDEOxApolo();
                //servico.AtualizaNumGalpaoFLOCKDATAWEB();
                //servico.AtualizaLoteAndIdadePedidoRacaoItem();

                //servico.InsertProductionCLFLOCKS("HYCL", "CL");
                //servico.InsertProductionHCFLOCKS("HYCO", "EC");
                //servico.InsertProductionHCFLOCKS("HYCO", "CO");

                //DateTime setDate = new DateTime(2020, 6, 30);
                //servico.RefreshSettingEggsFLIP("CH", setDate);
                //servico.CorrigiNascimentosWEBparaFLIPIncAvos();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
