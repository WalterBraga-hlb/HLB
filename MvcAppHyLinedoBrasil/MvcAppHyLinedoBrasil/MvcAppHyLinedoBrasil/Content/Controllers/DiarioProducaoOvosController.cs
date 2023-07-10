using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Models.Apolo;
using ImportaIncubacao.Data.Apolo;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class DiarioProducaoOvosController : Controller
    {
        #region Importação

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Verifica Fechamento de Estoque

            DATA_FECH_LANCTableAdapter dfTA = new DATA_FECH_LANCTableAdapter();
            FLIPDataSet.DATA_FECH_LANCDataTable dfDT = new FLIPDataSet.DATA_FECH_LANCDataTable();
            dfTA.Fill(dfDT);

            FLIPDataSet.DATA_FECH_LANCRow dfR = dfDT
                .Where(w => w.LOCATION == "Granja")
                .FirstOrDefault();

            if (dfR != null)
                Session["DataFechamentoMsg"] = "**** OS LANÇAMENTOS ANTERIORES A " + dfR.DATA_FECH_LANC.ToShortDateString()
                    + " NÃO SERÃO ATUALIZADOS / INSERIDOS, POIS ESTE PERÍODO ESTÁ FECHADO!"
                    + " CASO NECESSITE ALTERAR, SOLICTAR PARA JONATAN REALIZAR A ABERTURA DO PERÍODO!";

            #endregion

            return View();
        }

        [HttpPost]
        public ActionResult ImportaDadosDiarioProducaoOvos()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Salva Arquivo no Disco

            string caminho = @"C:\inetpub\wwwroot\Relatorios\DiarioProducaoOvos_" 
                + Session["login"].ToString() + ".xlsx";

            Request.Files[0].SaveAs(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            int linhaErro = 0;

            #endregion

            try
            {
                #region Abre arquivo Excel e carrega lista de Planilhas

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                int existe = 0;

                #endregion

                // Navega entre cada Planilha
                foreach (var planilha in lista)
                {
                    if (planilha.Name != "Dados Nucleos" && planilha.Name != "Dados Lotes"
                        && planilha.Name != "Dados Diário de Produção")
                    {
                        #region Carrega Linhas da Planilha

                        // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                        string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                            .Where(s => s.Name == planilha.Name)
                            .First().Id;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                        .GetPartById(relationshipId);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        var listaLinhas = sheetData.Descendants<Row>().ToList();

                        #endregion

                        #region Carrega campo Lote para verificar se existe cadastrado no FLIP

                        Row linhaLote = sheetData.Elements<Row>().Where(r => r.RowIndex == 5).First();
                        Cell celulaLote = linhaLote.Elements<Cell>().Where(c => c.CellReference == "C5").First();

                        string lote = DiarioProducaoRacaoController
                            .FromExcelTextBollean(celulaLote, spreadsheetDocument.WorkbookPart);

                        FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
                        FLIPDataSet.FLOCKSDataTable fDT = new FLIPDataSet.FLOCKSDataTable();
                        fTA.FillByFlockID(fDT, lote, 1);

                        #endregion

                        if (fDT.Count > 0)
                        {
                            // Navega nas linhas da Planilha
                            foreach (var linha in listaLinhas)
                            {
                                #region Verifica se na coluna D existe Data de Produção para ler a linha

                                existe = 0;
                                existe = linha.Elements<Cell>()
                                    .Where(c => c.CellReference == "D" + linha.RowIndex).Count();

                                if (existe > 0)
                                    existe = linha.Elements<Cell>()
                                        .Where(c => c.CellReference == "D" + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count();

                                int sequencia = 0;

                                linhaErro = Convert.ToInt32(linha.RowIndex.Value);

                                #endregion

                                if ((existe > 0) && (linha.RowIndex >= 8))
                                {
                                    #region Data de Produção

                                    DateTime dataProducao =
                                        DiarioProducaoRacaoController.FromExcelSerialDate(
                                            Convert.ToInt32(linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "D" + linha.RowIndex)
                                            .First().InnerText));

                                    #endregion

                                    #region Verifica Fechamento de Estoque

                                    DATA_FECH_LANCTableAdapter dfTA = new DATA_FECH_LANCTableAdapter();
                                    FLIPDataSet.DATA_FECH_LANCDataTable dfDT = new FLIPDataSet.DATA_FECH_LANCDataTable();
                                    dfTA.Fill(dfDT);

                                    existe = 0;
                                    existe = dfDT.Where(w => dataProducao >= w.DATA_FECH_LANC
                                        && w.LOCATION == "Granja")
                                        .Count();

                                    #endregion

                                    if (existe == 1)
                                    {
                                        #region Carrega Dados da Linhas da Planilha

                                        #region Carrega Variáveis para pegar valores das Linhas

                                        int mortalidadeFemea = 0;
                                        int mortalidadeMacho = 0;
                                        int ovosTotais = 0;
                                        int ovosIncubaveis = 0;
                                        int foraPadrao = 0;
                                        decimal consumoAguaM3 = 0;
                                        decimal consumoRacaoKG = 0;
                                        decimal? pesoOvosG = null;
                                        decimal uniformidade = 0;
                                        decimal CV = 0;
                                        decimal? pesoFemeaG = null;
                                        decimal uniformidadeFemea = 0;
                                        decimal? pesoMacho = null;
                                        decimal uniformidadeMacho = 0;
                                        int ovosSangue = 0;
                                        decimal temperaturaMinima = 0;
                                        decimal temperaturaMaxima = 0;
                                        string observacao = "";
                                        int ovosSujos = 0;
                                        int ovosChao = 0;
                                        int ovosQuebrados = 0;
                                        int ovosX = 0;

                                        #endregion

                                        #region Preenche Variáveis com os Valores das Colunas

                                        #region Mortalidade Femea

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            mortalidadeFemea = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "I" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Mortalidade Macho

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "K" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            mortalidadeMacho = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "K" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Ovos Totais

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            ovosTotais = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "M" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Ovos Incubáveis

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "O" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            ovosIncubaveis = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "O" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Fora de Padrão

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "R" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            foraPadrao = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "R" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Consumo de Agua (M³)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "T" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            consumoAguaM3 = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "T" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Consumo de Ração (KG)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "U" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            consumoRacaoKG = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "U" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Peso Ovos (g)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "V" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            pesoOvosG = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "V" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Uniformidade (%)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "W" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            uniformidade = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "W" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region CV (%)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "X" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            CV = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "X" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Peso Fêmea (g)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "Y" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            pesoFemeaG = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "Y" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Uniformidade Fêmea (%)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "Z" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            uniformidadeFemea = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "Z" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Peso Macho (g)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AA" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            pesoMacho = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AA" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Uniformidade Macho (%)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AB" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            uniformidadeMacho = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AB" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Ovos Sangue

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AC" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            ovosSangue = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AC" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Temperatura Mínima (ºC)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AD" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            temperaturaMinima = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AD" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Temperatura Máxima (ºC)

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AE" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            temperaturaMaxima = Decimal.Round(Convert.ToDecimal(double.Parse(
                                                linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AE" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Observação

                                        Row linhaObservacao = sheetData.Elements<Row>()
                                            .Where(r => r.RowIndex == linha.RowIndex).First();
                                        Cell celulaObservacao = linhaObservacao.Elements<Cell>()
                                            .Where(c => c.CellReference == "AF" + linha.RowIndex).First();

                                        observacao = DiarioProducaoRacaoController
                                            .FromExcelTextBollean(celulaObservacao, spreadsheetDocument.WorkbookPart);

                                        #endregion

                                        #region Ovos Sujos

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AG" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            ovosSujos = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AG" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Ovos Chão

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AH" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            ovosChao = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AH" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Ovos Quebrados

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AI" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            ovosQuebrados = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AI" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #region Ovos X

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AJ" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            ovosX = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AJ" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault()
                                                .Text.Replace(".", ",")));
                                        }

                                        #endregion

                                        #endregion

                                        #endregion

                                        #region Insere / Atualiza no FLIP

                                        #region Carrega Dados do Diário de Produção do FLIP se existir

                                        FLOCK_DATATableAdapter fdTA = new FLOCK_DATATableAdapter();
                                        FLIPDataSet.FLOCK_DATADataTable fdDT = new FLIPDataSet.FLOCK_DATADataTable();
                                        fdTA.FillByFlockIDAndTrxDate(fdDT, lote, dataProducao);

                                        #endregion

                                        if (fdDT.Count > 0)
                                        {
                                            #region Se já existe digitado, atualiza

                                            FLIPDataSet.FLOCK_DATARow rFD = fdDT.FirstOrDefault();

                                            rFD.HEN_MORT = mortalidadeFemea;
                                            rFD.MALE_MORT = mortalidadeMacho;
                                            rFD.TOTAL_EGGS_PROD = ovosTotais;
                                            rFD.NUM_1 = ovosIncubaveis;
                                            rFD.NUM_9 = foraPadrao;
                                            rFD.NUM_2 = consumoAguaM3;
                                            rFD.HEN_FEED_DEL = consumoRacaoKG;
                                            if (pesoOvosG != null) rFD.EGG_WT = Convert.ToDecimal(pesoOvosG);
                                            rFD.NUM_15 = uniformidade;
                                            rFD.NUM_16 = CV;
                                            if (pesoFemeaG != null) rFD.HEN_WT = Convert.ToDecimal(pesoFemeaG);
                                            rFD.NUM_5 = uniformidadeFemea;
                                            if (pesoMacho != null) rFD.NUM_4 = Convert.ToDecimal(pesoMacho);
                                            rFD.NUM_6 = uniformidadeMacho;
                                            rFD.NUM_13 = ovosSangue;
                                            rFD.TEXT_1 = observacao;
                                            rFD.NUM_10 = ovosSujos;
                                            rFD.NUM_11 = ovosChao;
                                            rFD.NUM_12 = ovosQuebrados;
                                            rFD.NUM_17 = ovosX;

                                            fdTA.Update(rFD);

                                            #endregion
                                        }
                                        else
                                        {
                                            #region Se não existe, insere um novo

                                            FLIPDataSet.FLOCKSRow rF = fDT.FirstOrDefault();

                                            fdTA.Insert(rF.COMPANY, rF.REGION, rF.LOCATION, rF.FARM_ID, rF.FLOCK_ID,
                                                1, dataProducao, null, mortalidadeFemea, pesoFemeaG, mortalidadeMacho,
                                                consumoRacaoKG, ovosTotais, pesoOvosG, null, null, null, observacao,
                                                null, ovosIncubaveis, consumoAguaM3, null, pesoMacho, uniformidadeFemea,
                                                uniformidadeMacho, null, null, foraPadrao, ovosSujos, ovosChao,
                                                ovosQuebrados, ovosSangue, null, null, uniformidade, CV, ovosX);

                                            #endregion
                                        }

                                        #endregion
                                    }
                                }
                            }
                        }
                        else
                        {
                            ViewBag.fileName = "";
                            ViewBag.erro = "**** LOTE " + lote + " NÃO CADASTRADO NO FLIP! "
                                + "POR FAVOR, SOLICITAR O CADASTRO ANTES DE IMPORTAR! ****";
                        }
                    }
                }

                arquivo.Close();

                return View("Index");
            }
            catch (Exception e)
            {
                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao realizar a importação: " + e.Message 
                    + " | Linha da Planilha: " + linhaErro.ToString()
                    + " | Linha de Erro no Código: " + linenum.ToString();
                arquivo.Close();
                return View("Index");
            }
        }

        #endregion

        #region Planilha de Preenchimento

        public string GeraPlanilhaDiarioProducaoOvos(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino)
        {
            #region Deleta Arquivos Antigos e faz uma cópia do mais atual

            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Diario_Producao_Ovos"
                + ".xlsx", destino);

            #endregion

            #region Abre o EXCEL e grava o ID do Processo

            object oMissing = System.Reflection.Missing.Value;

            Process[] P0, P1;
            P0 = Process.GetProcessesByName("Excel");

            Excel.Application oExcel = new Excel.Application();

            int I, J;
            P1 = Process.GetProcessesByName("Excel");
            I = 0;
            if (P1.Length > 1)
            {
                for (I = 0; I < P1.Length; I++)
                {
                    for (J = 0; J < P0.Length; J++)
                        if (P0[J].Id == P1[I].Id) break;
                    if (J == P0.Length) break;
                }
            }
            Process P = P1[I];

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            #endregion

            #region Carrega as Consultas

            #region BRFLOCKS

            string commandTextCHICCabecalho =
                "select " +
                    @"V.""Núcleo""," +
                    @"V.""Lote / Galpão / Linha"", " +
                    @"V.""Linhagem"", " +
                    @"V.""Data de Produção"", " +
                    @"V.""Qtde. Fêmeas"", " +
                    @"V.""Qtde. Machos"" ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_DIARIO_COMPLETO V  ";

            string commandTextCHICCondicaoJoins = 
                "where ";

            string commandTextCHICCondicaoFiltros =
                    @"V.""Status Lote"" = 1 and ";

            string filtroGranjasBRFLOCKS = RetornaFiltroGranjas(@"V.""Núcleo""");

            string commandTextCHICCondicaoParametros =
                    filtroGranjasBRFLOCKS;

            string commandTextCHICAgrupamento = "";

            #endregion

            #region FLOCKS

            string commandTextCHICCabecalhoFLOCKS =
                "select * ";

            string commandTextCHICTabelasFLOCKS =
                "from " +
                    "FLOCKS ";

            string commandTextCHICCondicaoJoinsFLOCKS =
                "where ";

            string commandTextCHICCondicaoFiltrosFLOCKS = 
                    "Active = 1 and ";

            string filtroGranjasFLOCKS = RetornaFiltroGranjas("Farm_ID");

            string commandTextCHICCondicaoParametrosFLOCKS = 
                    filtroGranjasFLOCKS;

            string commandTextCHICAgrupamentoFLOCKS = "";

            string commandTextCHICOrdenacaoFLOCKS =
                " order by Farm_ID, Flock_ID";

            #endregion

            #region Nucleos

            string commandTextCHICCabecalhoNucleos =
                "select " +
                    "distinct Farm_ID ";

            string commandTextCHICTabelasNucleos =
                "from " +
                    "FLOCK_DATA ";

            string commandTextCHICCondicaoJoinsNucleos =
                "where ";

            string commandTextCHICCondicaoFiltrosNucleos =
                    "Active = 1 and ";

            string commandTextCHICCondicaoParametrosNucleos =
                    filtroGranjasFLOCKS;

            string commandTextCHICAgrupamentoNucleos = "";

            string commandTextCHICOrdenacaoNucleos =
                " order by Farm_ID";

            #endregion

            #endregion

            #region Atualiza as Consultas no EXCEL

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                if (item.Name.Equals("BRFLOCKS"))
                {
                    item.ODBCConnection.BackgroundQuery = false;
                    item.ODBCConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros +
                        commandTextCHICAgrupamento;
                }
                else if (item.Name.Equals("FLOCKS"))
                {
                    item.ODBCConnection.BackgroundQuery = false;
                    item.ODBCConnection.CommandText =
                        commandTextCHICCabecalhoFLOCKS + commandTextCHICTabelasFLOCKS +
                        commandTextCHICCondicaoJoinsFLOCKS +
                        commandTextCHICCondicaoFiltrosFLOCKS + commandTextCHICCondicaoParametrosFLOCKS +
                        commandTextCHICAgrupamentoFLOCKS +
                        commandTextCHICOrdenacaoFLOCKS;
                }
                else if (item.Name.Equals("Nucleos"))
                {
                    item.OLEDBConnection.BackgroundQuery = false;
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoNucleos + commandTextCHICTabelasNucleos +
                        commandTextCHICCondicaoJoinsNucleos +
                        commandTextCHICCondicaoFiltrosNucleos + commandTextCHICCondicaoParametrosNucleos +
                        commandTextCHICAgrupamentoNucleos +
                        commandTextCHICOrdenacaoNucleos;
                }
            }

            #endregion

            #region Atualiza a Planilha e Fecha o EXCEL

            oBook.RefreshAll();

            // Quit Excel and clean up.
            oBook.Close(true, oMissing, oMissing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
            oBook = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
            oBooks = null;
            oExcel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
            oExcel = null;

            P.Kill();

            GC.Collect();

            #endregion

            return destino;
        }

        public ActionResult DownloadPlanilha()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadPlanilhaDiarioProducaoOvos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Diario_Producao_Ovos_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Diario_Producao_Ovos_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            destino = GeraPlanilhaDiarioProducaoOvos(pesquisa, true, pasta, destino);

            return File(destino, "Download", "Diario_Producao_Ovos_"
                + DateTime.Today.ToString("yyyy-MM-dd") + ".xlsx");
        }

        #endregion

        #region Outro Métodos

        public string RetornaFiltroGranjas(string campo)
        {
            string retorno = "(";

            #region Carrega Usuário Logado

            string login = Session["login"].ToString().ToUpper();

            if (login.Equals("PALVES"))
                login = "RIOSOFT";

            #endregion

            #region Carrega Empresas Internas do APOLO

            ApoloEntities apolo = new ApoloEntities();

            var listaFiliais = apolo.EMPRESA_FILIAL
                .Where(e => e.USERFLIPCod != null
                    && apolo.EMP_FIL_USUARIO.Any(u => u.UsuCod == login && u.EmpCod == e.EmpCod)
                    && (e.USERTipoUnidadeFLIP == "Granja" || e.USERTipoUnidadeFLIP == "Incubatório"))
                .SelectMany(
                    x => x.EMP_FILIAL_CERTIFICACAO.DefaultIfEmpty(),
                    (x, y) => new { EMPRESA_FILIAL = x, EMP_FILIAL_CERTIFICACAO = y })
                .OrderBy(f => f.EMPRESA_FILIAL.EmpNome)
                .ToList();

            foreach (var item in listaFiliais)
            {
                string codFLIP = "";
                if (item.EMP_FILIAL_CERTIFICACAO == null)
                    codFLIP = item.EMPRESA_FILIAL.USERFLIPCod;
                else
                    codFLIP = item.EMP_FILIAL_CERTIFICACAO.EmpFilCertificNum;

                retorno = retorno + campo + " like '" + codFLIP + "%'";

                if (listaFiliais.IndexOf(item) != (listaFiliais.Count - 1))
                    retorno = retorno + " or ";
            }

            #endregion

            #region Carrega Empresas Integradas

            Apolo10EntitiesService apoloService = new Apolo10EntitiesService();

            var listaEntidadesTerceiros = apoloService.ENTIDADE
                .Where(e => apoloService.ENTIDADE1.Any(e1 => e1.EntCod == e.EntCod && e1.USERFLIPCodigo != null
                    && apoloService.ENT_CATEG.Any(c => c.EntCod == e1.EntCod && c.CategCodEstr == "07.01"
                        && apoloService.CATEG_USUARIO.Any(u => u.CategCodEstr == c.CategCodEstr 
                            && u.UsuCod == login))))
                .OrderBy(e => e.EntNomeFant)
                .ToList();

            if (retorno != "" && listaEntidadesTerceiros.Count > 0)
                retorno = retorno + " or ";

            foreach (var item in listaEntidadesTerceiros)
            {
                ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = 
                    apoloService.ENTIDADE1.Where(e1 => e1.EntCod == item.EntCod).FirstOrDefault();

                retorno = retorno + campo + " like '" + entidade1.USERFLIPCodigo + "%'";

                if (listaEntidadesTerceiros.IndexOf(item) != (listaEntidadesTerceiros.Count - 1))
                    retorno = retorno + " or ";
            }

            #endregion

            return retorno = retorno + ")";
        }

        public bool VerificaSessao()
        {
            if (Session["usuario"] == null)
            {
                return true;
            }
            else
            {
                if (Session["usuario"].ToString() == "0")
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
