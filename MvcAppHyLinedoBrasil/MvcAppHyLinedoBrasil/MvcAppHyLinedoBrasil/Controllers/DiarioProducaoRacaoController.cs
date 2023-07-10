using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.ComponentModel;
using System.Data.Objects;
using MvcAppHyLinedoBrasil.Models;
using System.Threading;
using PRODUTO1 = MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO1;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class DiarioProducaoRacaoController : Controller
    {
        #region Objetos

        DiarioProducaoRacaoEntities bdDiarioProducaoRacao = new DiarioProducaoRacaoEntities();
        public static DiarioProducaoRacaoEntities bdDiarioProducaoRacaoStatic = new DiarioProducaoRacaoEntities();
        LayoutDb bd = new LayoutDb();
        public static int linhaErro;
        public static string teste;
        public static Cell celulaDataTeste;
        public static SpreadsheetDocument spreadsheetDocumentTeste;
        public static SheetData sheetDataTeste;
        public static HttpPostedFileBase file;

        public static List<Row> listaLinhasJScript = new List<Row>();
        public static List<SequenciaLinha> listaSequencia = new List<SequenciaLinha>();
        public static Row linhaAdicional = new Row();
        //public static SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(@"C:\inetpub\wwwroot\Relatorios", DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);
        public static string formulaAnterior;
        public static int percAnterior;

        #endregion

        #region Views

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            bdDiarioProducaoRacao.CommandTimeout = 10000;

            //bd.Database.ExecuteSqlCommand("delete from LayoutOrdemProducaos");
            //bd.SaveChanges();
            return View(bd.OrdemProducao.Where(w => w.Importado == "Não trazer").ToList());
        }

        #region Importa Excel e baixa no Apolo direto

        [HttpPost]
        public ActionResult ImportaDadosDiarioProducaoRacao()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            bd.Database.ExecuteSqlCommand("delete from LayoutOrdemProducaos");
            bd.SaveChanges();

            bdDiarioProducaoRacao.CommandTimeout = 10000;
            string caminho = @"C:\inetpub\wwwroot\Relatorios\DiarioProducaoRacao_" + Session["login"].ToString() + ".xlsx";

            Request.Files[0].SaveAs(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            //Thread.Sleep(5000);

            try
            {
                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                int existe = 0;

                //if (existe == -1)
                //{
                // Navega entre cada Planilha
                foreach (var planilha in lista)
                {
                    // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                    if (planilha.Name.ToString() == "Rel.Dia.Produção")
                    {
                        string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == planilha.Name)
                                                    .First().Id;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                        .GetPartById(relationshipId);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        var listaLinhas = sheetData.Descendants<Row>().ToList();

                        Row linhaData = sheetData.Elements<Row>().Where(r => r.RowIndex == 6).First();
                        Cell celulaData = linhaData.Elements<Cell>().Where(c => c.CellReference == "F6").First();

                        Cell celulaImportado = linhaData.Elements<Cell>().Where(c => c.CellReference == "M6").First();

                        // Descrição dos Produtos Adicionais
                        // Coluna M8
                        linhaAdicional = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                        //ViewBag.AdicionalM8 = DescricaoProduto(linhaAdicional, "M8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalN8 = DescricaoProduto(linhaAdicional, "N8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalO8 = DescricaoProduto(linhaAdicional, "O8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalP8 = DescricaoProduto(linhaAdicional, "P8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalQ8 = DescricaoProduto(linhaAdicional, "Q8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalR8 = DescricaoProduto(linhaAdicional, "R8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalS8 = DescricaoProduto(linhaAdicional, "S8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalT8 = DescricaoProduto(linhaAdicional, "T8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalU8 = DescricaoProduto(linhaAdicional, "U8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalV8 = DescricaoProduto(linhaAdicional, "V8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalW8 = DescricaoProduto(linhaAdicional, "W8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalX8 = DescricaoProduto(linhaAdicional, "X8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalY8 = DescricaoProduto(linhaAdicional, "Y8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalZ8 = DescricaoProduto(linhaAdicional, "Z8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAA8 = DescricaoProduto(linhaAdicional, "AA8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAB8 = DescricaoProduto(linhaAdicional, "AB8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAC8 = DescricaoProduto(linhaAdicional, "AC8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAD8 = DescricaoProduto(linhaAdicional, "AD8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAE8 = DescricaoProduto(linhaAdicional, "AE8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAF8 = DescricaoProduto(linhaAdicional, "AF8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAG8 = DescricaoProduto(linhaAdicional, "AG8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAH8 = DescricaoProduto(linhaAdicional, "AH8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAI8 = DescricaoProduto(linhaAdicional, "AI8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAJ8 = DescricaoProduto(linhaAdicional, "AJ8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAK8 = DescricaoProduto(linhaAdicional, "AK8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAL8 = DescricaoProduto(linhaAdicional, "AL8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAM8 = DescricaoProduto(linhaAdicional, "AM8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAN8 = DescricaoProduto(linhaAdicional, "AN8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAO8 = DescricaoProduto(linhaAdicional, "AO8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAP8 = DescricaoProduto(linhaAdicional, "AP8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAQ8 = DescricaoProduto(linhaAdicional, "AQ8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAR8 = DescricaoProduto(linhaAdicional, "AR8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAS8 = DescricaoProduto(linhaAdicional, "AS8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAT8 = DescricaoProduto(linhaAdicional, "AT8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAU8 = DescricaoProduto(linhaAdicional, "AU8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAV8 = DescricaoProduto(linhaAdicional, "AV8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAW8 = DescricaoProduto(linhaAdicional, "AW8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAX8 = DescricaoProduto(linhaAdicional, "AX8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAY8 = DescricaoProduto(linhaAdicional, "AY8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalAZ8 = DescricaoProduto(linhaAdicional, "AZ8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalBA8 = DescricaoProduto(linhaAdicional, "BA8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalBB8 = DescricaoProduto(linhaAdicional, "BB8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalBC8 = DescricaoProduto(linhaAdicional, "BC8", spreadsheetDocument.WorkbookPart);
                        ViewBag.AdicionalBD8 = DescricaoProduto(linhaAdicional, "BD8", spreadsheetDocument.WorkbookPart);

                        string importadoConfiguracao = FromExcelTextBollean(celulaImportado, spreadsheetDocument.WorkbookPart);

                        if (importadoConfiguracao == "NÃO")
                        {
                            // Navega nas linhas da Planilha
                            foreach (var linha in listaLinhas)
                            {
                                existe = 0;
                                existe = linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).Count();

                                if (existe > 0)
                                    existe = linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().Count();

                                int sequencia = 0;

                                //  if (linha.RowIndex == 56)
                                linhaErro = Convert.ToInt32(linha.RowIndex.Value);

                                if ((existe > 0) && (linha.RowIndex >= 10) && (linha.RowIndex <= 55))
                                {
                                    int codigoFormula = 0;

                                    codigoFormula = Convert.ToInt32(linha.Elements<Cell>()
                                                        .Where(c => c.CellReference == "E" + linha.RowIndex)
                                                        .First().InnerText);

                                    existe = 0;
                                    existe = bdDiarioProducaoRacao.PRODUTO1
                                            .Where(p => p.USERNumFormula == codigoFormula)
                                            .Count();

                                    if (existe > 0)
                                    {
                                        PRODUTO1 codigoProdutoPai1 = bdDiarioProducaoRacao.PRODUTO1
                                            .Where(p => p.USERNumFormula == codigoFormula)
                                            .First();

                                        MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO codigoProdutoPai = bdDiarioProducaoRacao.PRODUTO
                                            .Where(p => p.ProdCodEstr == codigoProdutoPai1.ProdCodEstr)
                                            .First();

                                        existe = 0;
                                        existe = bdDiarioProducaoRacao.FIC_TEC_PROD
                                            .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr).Count();

                                        // Caso ele exista, realiza as outras operações
                                        if (existe > 0)
                                        {
                                            string importado = "Nao";

                                            var oMyInt = new ObjectParameter("codigo", typeof(int));
                                            bdDiarioProducaoRacao.GerarCodigo("3", "PLAN_PRODUC", oMyInt);
                                            int codigo = Convert.ToInt32(oMyInt.Value);

                                            int qtdCaracteres = 7 - codigo.ToString().Length;
                                            string codigoCompleto = new String('0', qtdCaracteres) + codigo.ToString();

                                            PLAN_PRODUC planoProducao = new PLAN_PRODUC();

                                            planoProducao.EmpCod = "3";
                                            planoProducao.PlanProducNum = codigoCompleto;

                                            planoProducao.PlanProducData = FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText));

                                            planoProducao.PlanProducNome = "DATA PROD.RAÇÃO " + String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " - Fórmula " + codigoFormula.ToString();
                                            planoProducao.PlanProducDataInic = planoProducao.PlanProducData;
                                            planoProducao.PlanProducDataFim = planoProducao.PlanProducData;
                                            planoProducao.PlanProducCompEstq = "Nenhum";
                                            planoProducao.PlanProducConsidLoteEcon = "Não";
                                            planoProducao.PlanProducConsidEstq = "Não";
                                            planoProducao.PlanProducDesativado = "Não";

                                            bdDiarioProducaoRacao.PLAN_PRODUC.AddObject(planoProducao);

                                            NEC_PLAN_PRODUC necessidadePlanejamentoProducao = new NEC_PLAN_PRODUC();

                                            necessidadePlanejamentoProducao.EmpCod = "3";
                                            necessidadePlanejamentoProducao.PlanProducNum = planoProducao.PlanProducNum;
                                            necessidadePlanejamentoProducao.NecPlanProducDataEmis = planoProducao.PlanProducData;
                                            necessidadePlanejamentoProducao.NecPlanProducDataInic = planoProducao.PlanProducData;
                                            necessidadePlanejamentoProducao.NecPlanProducDataFim = planoProducao.PlanProducData;
                                            necessidadePlanejamentoProducao.NecPlanProducGerouOp = "Sim";
                                            necessidadePlanejamentoProducao.NecPlanProducReqMat = "Não";
                                            necessidadePlanejamentoProducao.NecPlanProducReqComp = "Não";
                                            necessidadePlanejamentoProducao.NecPlanProducVerEstqMat = "Sim";
                                            necessidadePlanejamentoProducao.NecPlanProducVerEstqAcab = "Não";
                                            necessidadePlanejamentoProducao.NecPlanProducVerEstqSemiAcab = "Não";
                                            necessidadePlanejamentoProducao.NecPlanProducVerEstqAlt = "Não";
                                            necessidadePlanejamentoProducao.NecPlanProducVerPedComp = "Não";
                                            necessidadePlanejamentoProducao.NecPlanProducDesmSemiAcab = "Não";
                                            necessidadePlanejamentoProducao.NecPlanProducDesativada = "Não";

                                            bdDiarioProducaoRacao.NEC_PLAN_PRODUC.AddObject(necessidadePlanejamentoProducao);

                                            // Localiza Produto Filho na Ficha Técnica
                                            FIC_TEC_PROD codigoProdutoPaiFicha = bdDiarioProducaoRacao.FIC_TEC_PROD
                                                .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr).First();

                                            ITEM_PLAN_PRODUC itemPlanoProducao = new ITEM_PLAN_PRODUC();

                                            itemPlanoProducao.EmpCod = "3";
                                            itemPlanoProducao.PlanProducNum = planoProducao.PlanProducNum;
                                            itemPlanoProducao.ProdCodEstr = codigoProdutoPaiFicha.ProdCodEstr;

                                            sequencia = sequencia + 1;
                                            itemPlanoProducao.ItPlanProducSeq = Convert.ToInt16(sequencia);

                                            /*
                                             * Ocorrência 3 - 14 - MNOTTI
                                             * 
                                             * Erro ao incluir quando o produto não tem a Unidade de Medida KG.
                                             * Foi definido que quando não existir, será exibida uma mensagem avisando esse problema.
                                             * 
                                             * 
                                                filho.FicTecProdUnidMedCodDig = "KG";
                                                filho.FicTecProdUnidMedPosDig = 1;
                                             */

                                            existe = 0;
                                            existe = bdDiarioProducaoRacao.PROD_UNID_MED
                                                .Where(u => u.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                                .Count();

                                            if (existe > 0)
                                            {
                                                PROD_UNID_MED prodUnidMed = bdDiarioProducaoRacao.PROD_UNID_MED
                                                    .Where(u => u.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                                    .First();

                                                itemPlanoProducao.ItPlanProducUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                                itemPlanoProducao.ItPlanProducUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                            }
                                            else
                                            {
                                                ViewBag.fileName = "";
                                                ViewBag.erro = "Erro ao realizar a importação: O produto " + codigoProdutoPaiFicha.ProdCodEstr
                                                    + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                                    + " novamente!";
                                                arquivo.Close();
                                                return View("Index", "");
                                            }

                                            /****/

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "G" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                itemPlanoProducao.ItPlanProducQtd = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference == "G" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);
                                            }
                                            else
                                            {
                                                itemPlanoProducao.ItPlanProducQtd = 0;
                                            }

                                            existe = 0;
                                            existe = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                                .Where(s => s.EmpCod == "3" &&
                                                    s.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr)
                                                .OrderByDescending(s => s.SaldoEstqData)
                                                .Count();

                                            decimal? saldoQtd = 0;

                                            if (existe > 0)
                                            {
                                                SALDO_ESTQ_DATA saldo = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                                    .Where(s => s.EmpCod == "3" &&
                                                        s.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr)
                                                    .OrderByDescending(s => s.SaldoEstqData)
                                                    .First();

                                                saldoQtd = saldo.SaldoEstqDataQtd;
                                            }

                                            itemPlanoProducao.ItPlanProducQtdEstq = saldoQtd;
                                            itemPlanoProducao.ItPlanProducQtdReserv = 0;
                                            itemPlanoProducao.ItPlanProducQtdEmp = 0;
                                            itemPlanoProducao.ItPlanProducQtdNec = itemPlanoProducao.ItPlanProducQtd;
                                            itemPlanoProducao.ItPlanProducQtdDisp = itemPlanoProducao.ItPlanProducQtdEstq;
                                            itemPlanoProducao.ItPlanProducQtdComp = 0;
                                            itemPlanoProducao.ItPlanProducQtdNecPeso = 0;
                                            itemPlanoProducao.ItPlanProducQtdNecPesoTot = 0;
                                            itemPlanoProducao.ItPlanProducCapHrMaq = 0;
                                            itemPlanoProducao.ItPlanProducConsidEstq = "Padrão";

                                            bdDiarioProducaoRacao.ITEM_PLAN_PRODUC.AddObject(itemPlanoProducao);

                                            ITEM_NEC_PLAN_PRODUC itemNecessidadePlanejamentoProducao = new ITEM_NEC_PLAN_PRODUC();

                                            itemNecessidadePlanejamentoProducao.EmpCod = "3";
                                            itemNecessidadePlanejamentoProducao.PlanProducNum = itemPlanoProducao.PlanProducNum;
                                            itemNecessidadePlanejamentoProducao.ProdCodEstr = itemPlanoProducao.ProdCodEstr;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducSeq = itemPlanoProducao.ItPlanProducSeq;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducUnidMedCod = itemPlanoProducao.ItPlanProducUnidMedCod;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducUnidMedPos = itemPlanoProducao.ItPlanProducUnidMedPos;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdOrig = itemPlanoProducao.ItPlanProducQtd;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdReal = 0;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdEstq = itemPlanoProducao.ItPlanProducQtdEstq;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdReserv = 0;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdEmp = 0;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNec = itemPlanoProducao.ItPlanProducQtdNec;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdDisp = itemPlanoProducao.ItPlanProducQtdDisp;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdDesm = 0;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducUtiliz = "Próprio";
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdComp = itemPlanoProducao.ItPlanProducQtdComp;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNecPeso = itemPlanoProducao.ItPlanProducQtdNecPeso;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducIndRetalho = 0;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNecPesoTot = itemPlanoProducao.ItPlanProducQtdNecPesoTot;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducCapHrMaq = itemPlanoProducao.ItPlanProducCapHrMaq;
                                            itemNecessidadePlanejamentoProducao.ItNecPlanProducSeqLeit = itemNecessidadePlanejamentoProducao.ItNecPlanProducSeq;

                                            bdDiarioProducaoRacao.ITEM_NEC_PLAN_PRODUC.AddObject(itemNecessidadePlanejamentoProducao);

                                            var listaFichaTecnicaFilhos = bdDiarioProducaoRacao.FIC_TEC_PROD
                                                                            .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr
                                                                                && f.FicTecProdDataInic >= codigoProdutoPai.ProdDataValidInic
                                                                                && (f.FicTecProdDataFim <= codigoProdutoPai.ProdDataValidInic || f.FicTecProdDataFim == null))
                                                                            .ToList();

                                            foreach (var itemFichaTecnicaFilhos in listaFichaTecnicaFilhos)
                                            {
                                                PLAN_PRODUC_FIC_TEC planejamentoProducaoFichaTecnica = new PLAN_PRODUC_FIC_TEC();

                                                planejamentoProducaoFichaTecnica.EmpCod = "3";
                                                planejamentoProducaoFichaTecnica.PlanProducNum = planoProducao.PlanProducNum;
                                                planejamentoProducaoFichaTecnica.ProdCodEstr = itemPlanoProducao.ProdCodEstr;
                                                planejamentoProducaoFichaTecnica.ItPlanProducSeq = itemPlanoProducao.ItPlanProducSeq;
                                                planejamentoProducaoFichaTecnica.FTProdCodEstr = itemPlanoProducao.ProdCodEstr;
                                                planejamentoProducaoFichaTecnica.FicTecProdSeq = itemFichaTecnicaFilhos.FicTecProdSeq;
                                                planejamentoProducaoFichaTecnica.PlanProducFicTecProdCodEstr = itemFichaTecnicaFilhos.FicTecProdCodEstr;
                                                planejamentoProducaoFichaTecnica.PlanProducFicTecQtd = itemFichaTecnicaFilhos.FicTecProdQtd * itemPlanoProducao.ItPlanProducQtd;

                                                existe = 0;
                                                existe = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                                    .Where(s => s.EmpCod == "3" &&
                                                        s.ProdCodEstr == itemFichaTecnicaFilhos.FicTecProdCodEstr)
                                                    .OrderByDescending(s => s.SaldoEstqData)
                                                    .Count();

                                                decimal? saldoFilhoQtd = 0;

                                                if (existe > 0)
                                                {
                                                    SALDO_ESTQ_DATA saldoFilho = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                                    .Where(s => s.EmpCod == "3" &&
                                                        s.ProdCodEstr == itemFichaTecnicaFilhos.FicTecProdCodEstr)
                                                    .OrderByDescending(s => s.SaldoEstqData)
                                                    .First();

                                                    saldoFilhoQtd = saldoFilho.SaldoEstqDataQtd;
                                                }

                                                planejamentoProducaoFichaTecnica.PlanProducFicTecQtdEstq = saldoFilhoQtd;
                                                planejamentoProducaoFichaTecnica.PlanProducFicTecQtdReserv = 0;
                                                planejamentoProducaoFichaTecnica.PlanProducFicTecQtdEmp = 0;
                                                planejamentoProducaoFichaTecnica.PlanProducFicTecQtdNec = planejamentoProducaoFichaTecnica.PlanProducFicTecQtd;
                                                planejamentoProducaoFichaTecnica.PlanProducFicTecQtdComp = 0;

                                                bdDiarioProducaoRacao.PLAN_PRODUC_FIC_TEC.AddObject(planejamentoProducaoFichaTecnica);
                                            }

                                            bdDiarioProducaoRacao.SaveChanges();

                                            bdDiarioProducaoRacao.GeraOrdemProducao(planoProducao.PlanProducNum, itemPlanoProducao.ProdCodEstr,
                                                itemPlanoProducao.ItPlanProducSeq, null, "3", planoProducao.PlanProducData, "RIOSOFT");

                                            bdDiarioProducaoRacao.SaveChanges();

                                            ORD_PRODUC ordProducNum = bdDiarioProducaoRacao.ORD_PRODUC
                                                .Where(o => o.EmpCod == "3" && o.PlanProducNum == planoProducao.PlanProducNum)
                                                .First();

                                            Cell localCelula = linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First();
                                            string nucleo = FromExcelTextBollean(localCelula, spreadsheetDocument.WorkbookPart);

                                            LOC_ARMAZ local = bdDiarioProducaoRacao.LOC_ARMAZ
                                                .Where(l => l.LocArmazNome.Contains(nucleo)).FirstOrDefault();

                                            if (local == null)
                                            {
                                                ViewBag.Erro = "Local " + nucleo + " não configurado nos "
                                                    + "locais de armazenagem do APOLO! Verifique a descrição "
                                                    + "do local de armazenagem!";
                                                arquivo.Close();
                                                var listaExibicaoErro = bd.OrdemProducao;
                                                return View("Index", listaExibicaoErro);
                                            }

                                            ordProducNum.LocArmazCodEstr = local.LocArmazCodEstr;

                                            OPER_ORD_PRODUC operOrdProduc = new OPER_ORD_PRODUC();

                                            operOrdProduc.EmpCod = "3";
                                            operOrdProduc.OrdProducNum = ordProducNum.OrdProducNum;
                                            operOrdProduc.ProdCodEstr = ordProducNum.ProdCodEstr;
                                            operOrdProduc.ProdOperSeq = 10;

                                            OPER_ORD_PRODUC ultimoOperOrdProduc = bdDiarioProducaoRacao.OPER_ORD_PRODUC
                                                .Where(o => o.EmpCod == "3").OrderByDescending(o => o.OperOrdProducSeq)
                                                .First();

                                            operOrdProduc.OperOrdProducSeq = ultimoOperOrdProduc.OperOrdProducSeq + 1;
                                            operOrdProduc.CCtrlCodEstr = "1.07.0001";
                                            operOrdProduc.OperOrdProducStat = "Manual";

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "H" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                double d = double.Parse(linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "H" + linha.RowIndex)
                                                    .First().InnerText.Replace(".", ","));

                                                DateTime dt = DateTime.FromOADate(d);

                                                string dataHora = String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " " +
                                                    String.Format("{0:hh:mm}", dt);

                                                operOrdProduc.OperOrdProducDataHoraInic = Convert.ToDateTime(dataHora);
                                            }
                                            else
                                            {
                                                operOrdProduc.OperOrdProducDataHoraInic = null;
                                            }

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "I" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                double d = double.Parse(linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "I" + linha.RowIndex)
                                                    .First().InnerText.Replace(".", ","));

                                                DateTime dt = DateTime.FromOADate(d);

                                                string dataHora = String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " " +
                                                    String.Format("{0:hh:mm}", dt);

                                                operOrdProduc.OperOrdProducDataHoraFim = Convert.ToDateTime(dataHora);
                                            }
                                            else
                                            {
                                                operOrdProduc.OperOrdProducDataHoraFim = null;
                                            }

                                            operOrdProduc.OperOrdProducQtdBoa = itemPlanoProducao.ItPlanProducQtd;
                                            operOrdProduc.OperOrdProducQtdRefug = 0;
                                            operOrdProduc.OperOrdProducQtdReproc = 0;
                                            operOrdProduc.OperCod = "0000001";
                                            operOrdProduc.UsuCod = "MNOTTI";

                                            DateTime dataInicial = Convert.ToDateTime(operOrdProduc.OperOrdProducDataHoraInic);
                                            DateTime dataFinal = Convert.ToDateTime(operOrdProduc.OperOrdProducDataHoraFim);

                                            decimal tempoCent = ((dataInicial - dataFinal).Minutes / 60);

                                            operOrdProduc.OperOrdProducTempoCent = tempoCent;
                                            operOrdProduc.OperOrdProducApont = "Operação";
                                            //operOrdProduc.OperOrdProducApont = "Preparação";
                                            operOrdProduc.OperOrdProducGerReqMat = "Sim";
                                            operOrdProduc.OperOrdProducPesoUnitProd = 0;
                                            operOrdProduc.OperOrdProducQtdRetalho = 0;
                                            operOrdProduc.AtivGrpCodEstr = "01.01";
                                            operOrdProduc.OperOrdProducGeraLoteAutom = "Configuração";

                                            /*
                                             * Ocorrência 3 - 14 - MNOTTI
                                             * 
                                             * Erro ao incluir quando o produto não tem a Unidade de Medida KG.
                                             * Foi definido que quando não existir, será exibida uma mensagem avisando esse problema.
                                             * 
                                             * 
                                                filho.FicTecProdUnidMedCodDig = "KG";
                                                filho.FicTecProdUnidMedPosDig = 1;
                                             */

                                            existe = 0;
                                            existe = bdDiarioProducaoRacao.PROD_UNID_MED
                                                .Where(u => u.ProdCodEstr == operOrdProduc.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                                .Count();

                                            if (existe > 0)
                                            {
                                                PROD_UNID_MED prodUnidMed = bdDiarioProducaoRacao.PROD_UNID_MED
                                                    .Where(u => u.ProdCodEstr == operOrdProduc.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                                    .First();

                                                operOrdProduc.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                                operOrdProduc.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                            }
                                            else
                                            {
                                                ViewBag.fileName = "";
                                                ViewBag.erro = "Erro ao realizar a importação: O produto " + operOrdProduc.ProdCodEstr
                                                    + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                                    + " novamente!";
                                                arquivo.Close();
                                                return View("Index", "");
                                            }

                                            /****/

                                            operOrdProduc.OperOrdProducCompBruto = 0;
                                            operOrdProduc.OperOrdProducCompLiq = 0;
                                            operOrdProduc.OperOrdProducAltBruta = 0;
                                            operOrdProduc.OperOrdProducAltLiq = 0;
                                            operOrdProduc.OperOrdProducLargBruta = 0;
                                            operOrdProduc.OperOrdProducLargLiq = 0;
                                            operOrdProduc.OperOrdProducEspacoBruto = 0;
                                            operOrdProduc.OperOrdProducEspacoLiq = 0;
                                            operOrdProduc.OperOrdProducDataHoraApont = DateTime.Now;
                                            operOrdProduc.OperOrdProducTara = 0;
                                            operOrdProduc.OperOrdProducPesoBruto = 0;
                                            operOrdProduc.OperOrdProducTipo = "Produção";
                                            operOrdProduc.OperOrdProducQtdReal = 0;
                                            operOrdProduc.TipoLancCod = codigoProdutoPai1.USERTipoLancEntradaProd;
                                            operOrdProduc.OperOrdProducIntegraEstq = "Sim";
                                            operOrdProduc.OperOrdProducIntegradoEstq = "Não";
                                            operOrdProduc.OperOrdProducUnidMedPeso = 1;
                                            operOrdProduc.OperOrdProducQtdCalc = operOrdProduc.OperOrdProducQtdBoa;

                                            bdDiarioProducaoRacao.OPER_ORD_PRODUC.AddObject(operOrdProduc);

                                            OPER_ORD_PRODUC_FUNC operOrdProducFunc = new OPER_ORD_PRODUC_FUNC();

                                            operOrdProducFunc.EmpCod = "3";
                                            operOrdProducFunc.OrdProducNum = operOrdProduc.OrdProducNum;
                                            operOrdProducFunc.ProdCodEstr = operOrdProduc.ProdCodEstr;
                                            operOrdProducFunc.ProdOperSeq = operOrdProduc.ProdOperSeq;
                                            operOrdProducFunc.OperOrdProducSeq = operOrdProduc.OperOrdProducSeq;

                                            Cell responsavelCelula = linha.Elements<Cell>().Where(c => c.CellReference == "M" + linha.RowIndex).First();
                                            string responsavel = FromExcelTextBollean(responsavelCelula, spreadsheetDocument.WorkbookPart);

                                            if (responsavel == "Paulo Sérgio")
                                            {
                                                operOrdProducFunc.FuncCod = "0000008";
                                            }
                                            else if (responsavel == "Leandro")
                                            {
                                                operOrdProducFunc.FuncCod = "0000011";
                                            }
                                            else if (responsavel == "Osmanir")
                                            {
                                                operOrdProducFunc.FuncCod = "0000110";
                                            }
                                            else if (responsavel == "Caio Santiago")
                                            {
                                                operOrdProducFunc.FuncCod = "0000118";
                                            }
                                            else if (responsavel == "Pedro Fuentes")
                                            {
                                                operOrdProducFunc.FuncCod = "0000286";
                                            }

                                            operOrdProducFunc.OperOrdProducFuncApont = "Sim";

                                            bdDiarioProducaoRacao.OPER_ORD_PRODUC_FUNC.AddObject(operOrdProducFunc);

                                            bdDiarioProducaoRacao.SaveChanges();

                                            bdDiarioProducaoRacao.InsertOperOrdProducProc("3", ordProducNum.OrdProducNum, operOrdProduc.OperOrdProducSeq, operOrdProduc.ProdCodEstr);

                                            bdDiarioProducaoRacao.SaveChanges();

                                            // Exibição
                                            LayoutOrdemProducao ordemProducaoExibe = new LayoutOrdemProducao();

                                            ordemProducaoExibe.CodigoApolo = codigoProdutoPai.ProdCodEstr;
                                            ordemProducaoExibe.CodFormula = codigoFormula;
                                            ordemProducaoExibe.TipoRacao = codigoProdutoPai.ProdNome;
                                            ordemProducaoExibe.TotalProduzido = operOrdProduc.OperOrdProducQtdBoa;
                                            ordemProducaoExibe.OrdemProducao = operOrdProduc.OrdProducNum;
                                            ordemProducaoExibe.Responsavel = responsavel;
                                            ordemProducaoExibe.NucleoGalpao = nucleo;

                                            //**** Insere os Materiais Adicionais ****
                                            decimal qtde = 0;
                                            Row linhaProdutoAdicional = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();

                                            // Célula N8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "N8", "N", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional01 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula O8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "O8", "O", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional02 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula P8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "P8", "P", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional03 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula Q8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "Q8", "Q", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional04 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula R8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "R8", "R", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional05 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula S8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "S8", "S", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional06 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula T8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "T8", "T", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional07 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula U8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "U8", "U", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional08 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula V8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "V8", "V", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional09 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula W8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "W8", "W", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional10 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula X8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "X8", "X", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional11 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula Y8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "Y8", "Y", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional12 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula Z8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "Z8", "Z", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional13 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AA8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AA8", "AA", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional14 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AB8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AB8", "AB", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional15 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AC8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AC8", "AC", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional16 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AD8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AD8", "AD", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional17 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AE8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AE8", "AE", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional18 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AF8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AF8", "AF", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional19 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AG8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AG8", "AG", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional20 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AH8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AH8", "AH", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional21 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AI8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AI8", "AI", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional22 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AJ8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AJ8", "AJ", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional23 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AK8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AK8", "AK", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional24 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AL8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AL8", "AL", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional25 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AM8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AM8", "AM", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional26 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AN8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AN8", "AN", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional27 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AO8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AO8", "AO", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional28 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AP8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AP8", "AP", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional29 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AQ8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AQ8", "AQ", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional30 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AR8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AR8", "AR", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional31 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AS8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AS8", "AS", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional32 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AT8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AT8", "AT", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional33 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AU8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AU8", "AU", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional34 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AV8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AV8", "AV", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional35 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AW8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AW8", "AW", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional36 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AX8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AX8", "AX", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional37 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AY8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AY8", "AY", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional38 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula AZ8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AZ8", "AZ", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional39 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula BA8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BA8", "BA", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional40 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula BB8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BB8", "BB", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional41 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula BC8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BC8", "BC", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional42 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                            // Célula BD8
                                            qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BD8", "BD", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText)));
                                            ordemProducaoExibe.Adicional43 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);

                                            importado = VerificaOrdemProducaoBaixada(ordemProducaoExibe.OrdemProducao);

                                            ordemProducaoExibe.Importado = importado;

                                            bd.OrdemProducao.Add(ordemProducaoExibe);

                                            bd.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ViewBag.fileName = "";
                            ViewBag.erro = "**** ARQUIVO JÁ IMPORTADO! ****";
                        }

                        //bdDiarioProducaoRacao.SaveChanges();
                    }
                }
                //}

                var listaExibicaoOK = bd.OrdemProducao;

                arquivo.Close();

                return View("Index", listaExibicaoOK);
            }
            catch (Exception e)
            {
                var listaExibicaoErro = bd.OrdemProducao;

                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                ViewBag.fileName = "";
                if (e.InnerException == null)
                    ViewBag.erro = "Erro ao realizar a importação: " + e.Message + " | linha: "
                        + linhaErro.ToString() + " | linha erro código: " + linenum.ToString();
                else
                    ViewBag.erro = "Erro ao realizar a importação: " + e.Message
                        + " | Erro Interno: " + e.InnerException.Message
                        + " | linha: "
                        + linhaErro.ToString() + " | linha erro código: " + linenum.ToString();
                arquivo.Close();
                return View("Index", listaExibicaoErro);
            }
        }

        #endregion

        #region Importação linha a linha via JScript

        [HttpPost]
        public ActionResult ImportaDadosDiarioProducaoRacaoTeste()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            Stream arquivo = null;
            spreadsheetDocumentTeste = null;
            celulaDataTeste = null;
            sheetDataTeste = null;
            listaLinhasJScript = null;
            listaSequencia = null;
            linhaAdicional = null;
            file = null;

            try
            {
                //bd.Database.ExecuteSqlCommand("delete from LayoutOrdemProducaos");
                //bd.SaveChanges();

                string caminho = @"C:\inetpub\wwwroot\Relatorios\DiarioProducaoRacao_" + Session["login"].ToString() + ".xls";

                if (System.IO.File.Exists(caminho))
                {
                    System.IO.File.Delete(caminho);
                }

                //Request.Files[0].SaveAs(caminho);
                if (Request.Files.Count == 1)
                {

                    file = Request.Files[0];

                    file.SaveAs(caminho);
                }

                arquivo = System.IO.File.Open(caminho, FileMode.Open);

                List<Row> listaLinhas = new List<Row>();

                if ((Request.Files.Count == 0) && (arquivo != null))
                {
                    listaLinhasJScript = new List<Row>();

                    //ViewBag.fileName = "Arquivo " + file.FileName + " importado com sucesso!";

                    //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                    // Open a SpreadsheetDocument based on a stream.
                    spreadsheetDocumentTeste = SpreadsheetDocument.Open(arquivo, true);

                    // Lista de Planilhas do Documento Excel
                    var lista = spreadsheetDocumentTeste.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                    // Navega entre cada Planilha
                    foreach (var planilha in lista)
                    {
                        // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                        if (planilha.Name.ToString() == "Rel.Dia.Produção")
                        {
                            string relationshipId = spreadsheetDocumentTeste.WorkbookPart.Workbook.Descendants<Sheet>()
                                                        .Where(s => s.Name == planilha.Name)
                                                        .First().Id;
                            WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocumentTeste.WorkbookPart
                                                            .GetPartById(relationshipId);

                            sheetDataTeste = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                            listaLinhas = sheetDataTeste.Descendants<Row>().ToList();

                            Row linhaData = sheetDataTeste.Elements<Row>().Where(r => r.RowIndex == 6).First();
                            celulaDataTeste = linhaData.Elements<Cell>().Where(c => c.CellReference == "F6").First();

                            Cell celulaImportado = linhaData.Elements<Cell>().Where(c => c.CellReference == "M6").First();

                            string importadoConfiguracao = FromExcelTextBollean(celulaImportado, spreadsheetDocumentTeste.WorkbookPart);

                            linhaAdicional = sheetDataTeste.Elements<Row>().Where(r => r.RowIndex == 8).First();

                            //if (importadoConfiguracao != "NÃO")
                            //{
                            //    listaLinhas = null;
                            //    ViewBag.fileName = "";
                            //    ViewBag.erro = "**** ARQUIVO JÁ IMPORTADO! ****";
                            //}
                        }
                    }

                    file = null;
                }
                arquivo.Close();
                percAnterior = 1;
                formulaAnterior = "Importação iniciada";

                var listaLinhasFiltro = listaLinhas.Where(l => l.RowIndex >= 10 && l.RowIndex <= 55).OrderBy(o => o.RowIndex.Value).ToList();

                foreach (var item in listaLinhasFiltro)
                {
                    int codigoFormula = 0;

                    if (item.Elements<Cell>()
                                        .Where(c => c.CellReference == "E" + item.RowIndex)
                                        .First().InnerText != "")
                        codigoFormula = Convert.ToInt32(item.Elements<Cell>()
                                            .Where(c => c.CellReference == "E" + item.RowIndex)
                                            .First().InnerText);

                    int existe = 0;
                    existe = bdDiarioProducaoRacao.PRODUTO1
                            .Where(p => p.USERNumFormula == codigoFormula)
                            .Count();

                    if (existe > 0)
                    {
                        listaLinhasJScript.Add(item);
                    }
                }

                //listaLinhasJScript = listaLinhas.Where(l => l.RowIndex >= 10 && l.RowIndex <= 55).OrderBy(o => o.RowIndex.Value).ToList();

                listaSequencia = new List<SequenciaLinha>();

                int id = 1;
                foreach (var item in listaLinhasJScript)
                {
                    int seq = (int)item.RowIndex.Value;

                    SequenciaLinha linha = new SequenciaLinha();
                    linha.ID = id;
                    linha.Sequencia = seq.ToString();

                    listaSequencia.Add(linha);

                    id++;
                }

                return Json(listaSequencia);
            }
            catch (Exception e)
            {
                var listaExibicaoErro = bd.OrdemProducao;

                ViewBag.fileName = "";
                string erro = "Erro ao ler o aquivo: " + e.Message;
                ViewBag.erro = erro;
                if (arquivo != null)
                    arquivo.Close();
                return new HttpStatusCodeResult(500, erro);
                //return View("Index", listaExibicaoErro);
                //return Json(erro);
            }
        }

        [HttpPost]
        public ActionResult ImportaLinhaDadosDiarioProducaoRacao(string id)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            int existe = 0;
            // Exibição
            LayoutOrdemProducao ordemProducaoExibe = new LayoutOrdemProducao();
            string linhaPLanilha = "";
            try
            {
                int sequencia = Convert.ToInt32(id);

                if (sequencia == -1)
                {
                    SequenciaLinha sequenciaLinha = listaSequencia.Where(s => s.ID == sequencia).FirstOrDefault();

                    if (sequenciaLinha != null)
                    {
                        linhaPLanilha = sequenciaLinha.Sequencia;

                        Row linha = listaLinhasJScript.Where(l => l.RowIndex == sequenciaLinha.Sequencia).FirstOrDefault();

                        if (linha != null)
                        {
                            int codigoFormula = 0;

                            if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "E" + linha.RowIndex)
                                                .First().InnerText != "")
                                codigoFormula = Convert.ToInt32(linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "E" + linha.RowIndex)
                                                    .First().InnerText);

                            existe = 0;
                            existe = bdDiarioProducaoRacao.PRODUTO1
                                    .Where(p => p.USERNumFormula == codigoFormula)
                                    .Count();

                            if (existe > 0)
                            //if (existe == -50)
                            {
                                PRODUTO1 codigoProdutoPai1 = bdDiarioProducaoRacao.PRODUTO1
                                    .Where(p => p.USERNumFormula == codigoFormula)
                                    .First();

                                MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO codigoProdutoPai = bdDiarioProducaoRacao.PRODUTO
                                    .Where(p => p.ProdCodEstr == codigoProdutoPai1.ProdCodEstr)
                                    .First();

                                existe = 0;
                                existe = bdDiarioProducaoRacao.FIC_TEC_PROD
                                    .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr).Count();

                                // Caso ele exista, realiza as outras operações
                                if (existe > 0)
                                {
                                    string importado = "Nao";

                                    var oMyInt = new ObjectParameter("codigo", typeof(int));
                                    bdDiarioProducaoRacao.GerarCodigo("3", "PLAN_PRODUC", oMyInt);
                                    int codigo = Convert.ToInt32(oMyInt.Value);

                                    int qtdCaracteres = 7 - codigo.ToString().Length;
                                    string codigoCompleto = new String('0', qtdCaracteres) + codigo.ToString();

                                    PLAN_PRODUC planoProducao = new PLAN_PRODUC();

                                    planoProducao.EmpCod = "3";
                                    planoProducao.PlanProducNum = codigoCompleto;

                                    planoProducao.PlanProducData = FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText));

                                    planoProducao.PlanProducNome = "DATA PROD.RAÇÃO " + String.Format("{0:MM/dd/yyyy}", planoProducao.PlanProducData) + " - Fórmula " + codigoFormula.ToString();
                                    planoProducao.PlanProducDataInic = planoProducao.PlanProducData;
                                    planoProducao.PlanProducDataFim = planoProducao.PlanProducData;
                                    planoProducao.PlanProducCompEstq = "Nenhum";
                                    planoProducao.PlanProducConsidLoteEcon = "Não";
                                    planoProducao.PlanProducConsidEstq = "Não";
                                    planoProducao.PlanProducDesativado = "Não";

                                    bdDiarioProducaoRacao.PLAN_PRODUC.AddObject(planoProducao);

                                    NEC_PLAN_PRODUC necessidadePlanejamentoProducao = new NEC_PLAN_PRODUC();

                                    necessidadePlanejamentoProducao.EmpCod = "3";
                                    necessidadePlanejamentoProducao.PlanProducNum = planoProducao.PlanProducNum;
                                    necessidadePlanejamentoProducao.NecPlanProducDataEmis = planoProducao.PlanProducData;
                                    necessidadePlanejamentoProducao.NecPlanProducDataInic = planoProducao.PlanProducData;
                                    necessidadePlanejamentoProducao.NecPlanProducDataFim = planoProducao.PlanProducData;
                                    necessidadePlanejamentoProducao.NecPlanProducGerouOp = "Sim";
                                    necessidadePlanejamentoProducao.NecPlanProducReqMat = "Não";
                                    necessidadePlanejamentoProducao.NecPlanProducReqComp = "Não";
                                    necessidadePlanejamentoProducao.NecPlanProducVerEstqMat = "Sim";
                                    necessidadePlanejamentoProducao.NecPlanProducVerEstqAcab = "Não";
                                    necessidadePlanejamentoProducao.NecPlanProducVerEstqSemiAcab = "Não";
                                    necessidadePlanejamentoProducao.NecPlanProducVerEstqAlt = "Não";
                                    necessidadePlanejamentoProducao.NecPlanProducVerPedComp = "Não";
                                    necessidadePlanejamentoProducao.NecPlanProducDesmSemiAcab = "Não";
                                    necessidadePlanejamentoProducao.NecPlanProducDesativada = "Não";

                                    bdDiarioProducaoRacao.NEC_PLAN_PRODUC.AddObject(necessidadePlanejamentoProducao);

                                    // Localiza Produto Filho na Ficha Técnica
                                    FIC_TEC_PROD codigoProdutoPaiFicha = bdDiarioProducaoRacao.FIC_TEC_PROD
                                        .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr).First();

                                    ITEM_PLAN_PRODUC itemPlanoProducao = new ITEM_PLAN_PRODUC();

                                    itemPlanoProducao.EmpCod = "3";
                                    itemPlanoProducao.PlanProducNum = planoProducao.PlanProducNum;
                                    itemPlanoProducao.ProdCodEstr = codigoProdutoPaiFicha.ProdCodEstr;

                                    sequencia = sequencia + 1;
                                    itemPlanoProducao.ItPlanProducSeq = Convert.ToInt16(sequencia);

                                    /*
                                        * Ocorrência 3 - 14 - MNOTTI
                                        * 
                                        * Erro ao incluir quando o produto não tem a Unidade de Medida KG.
                                        * Foi definido que quando não existir, será exibida uma mensagem avisando esse problema.
                                        * 
                                        * 
                                        filho.FicTecProdUnidMedCodDig = "KG";
                                        filho.FicTecProdUnidMedPosDig = 1;
                                        */

                                    existe = 0;
                                    existe = bdDiarioProducaoRacao.PROD_UNID_MED
                                        .Where(u => u.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                        .Count();

                                    if (existe > 0)
                                    {
                                        PROD_UNID_MED prodUnidMed = bdDiarioProducaoRacao.PROD_UNID_MED
                                            .Where(u => u.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                            .First();

                                        itemPlanoProducao.ItPlanProducUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                        itemPlanoProducao.ItPlanProducUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                    }
                                    else
                                    {
                                        ViewBag.fileName = "";
                                        ViewBag.erro = "Erro ao realizar a importação: O produto " + codigoProdutoPaiFicha.ProdCodEstr
                                            + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                            + " novamente!";
                                        return View("Index", "");
                                    }

                                    /****/

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "G" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        itemPlanoProducao.ItPlanProducQtd = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                        .Where(c => c.CellReference == "G" + linha.RowIndex)
                                                        .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);
                                    }
                                    else
                                    {
                                        itemPlanoProducao.ItPlanProducQtd = 0;
                                    }

                                    existe = 0;
                                    existe = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                        .Where(s => s.EmpCod == "3" &&
                                            s.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr)
                                        .OrderByDescending(s => s.SaldoEstqData)
                                        .Count();

                                    decimal? saldoQtd = 0;

                                    if (existe > 0)
                                    {
                                        SALDO_ESTQ_DATA saldo = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                            .Where(s => s.EmpCod == "3" &&
                                                s.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr)
                                            .OrderByDescending(s => s.SaldoEstqData)
                                            .First();

                                        saldoQtd = saldo.SaldoEstqDataQtd;
                                    }

                                    itemPlanoProducao.ItPlanProducQtdEstq = saldoQtd;
                                    itemPlanoProducao.ItPlanProducQtdReserv = 0;
                                    itemPlanoProducao.ItPlanProducQtdEmp = 0;
                                    itemPlanoProducao.ItPlanProducQtdNec = itemPlanoProducao.ItPlanProducQtd;
                                    itemPlanoProducao.ItPlanProducQtdDisp = itemPlanoProducao.ItPlanProducQtdEstq;
                                    itemPlanoProducao.ItPlanProducQtdComp = 0;
                                    itemPlanoProducao.ItPlanProducQtdNecPeso = 0;
                                    itemPlanoProducao.ItPlanProducQtdNecPesoTot = 0;
                                    itemPlanoProducao.ItPlanProducCapHrMaq = 0;
                                    itemPlanoProducao.ItPlanProducConsidEstq = "Padrão";

                                    bdDiarioProducaoRacao.ITEM_PLAN_PRODUC.AddObject(itemPlanoProducao);

                                    ITEM_NEC_PLAN_PRODUC itemNecessidadePlanejamentoProducao = new ITEM_NEC_PLAN_PRODUC();

                                    itemNecessidadePlanejamentoProducao.EmpCod = "3";
                                    itemNecessidadePlanejamentoProducao.PlanProducNum = itemPlanoProducao.PlanProducNum;
                                    itemNecessidadePlanejamentoProducao.ProdCodEstr = itemPlanoProducao.ProdCodEstr;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducSeq = itemPlanoProducao.ItPlanProducSeq;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducUnidMedCod = itemPlanoProducao.ItPlanProducUnidMedCod;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducUnidMedPos = itemPlanoProducao.ItPlanProducUnidMedPos;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdOrig = itemPlanoProducao.ItPlanProducQtd;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdReal = 0;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdEstq = itemPlanoProducao.ItPlanProducQtdEstq;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdReserv = 0;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdEmp = 0;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNec = itemPlanoProducao.ItPlanProducQtdNec;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdDisp = itemPlanoProducao.ItPlanProducQtdDisp;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdDesm = 0;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducUtiliz = "Próprio";
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdComp = itemPlanoProducao.ItPlanProducQtdComp;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNecPeso = itemPlanoProducao.ItPlanProducQtdNecPeso;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducIndRetalho = 0;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNecPesoTot = itemPlanoProducao.ItPlanProducQtdNecPesoTot;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducCapHrMaq = itemPlanoProducao.ItPlanProducCapHrMaq;
                                    itemNecessidadePlanejamentoProducao.ItNecPlanProducSeqLeit = itemNecessidadePlanejamentoProducao.ItNecPlanProducSeq;

                                    bdDiarioProducaoRacao.ITEM_NEC_PLAN_PRODUC.AddObject(itemNecessidadePlanejamentoProducao);

                                    var listaFichaTecnicaFilhos = bdDiarioProducaoRacao.FIC_TEC_PROD
                                                                    .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr
                                                                        && f.FicTecProdDataInic >= codigoProdutoPai.ProdDataValidInic
                                                                        && (f.FicTecProdDataFim <= codigoProdutoPai.ProdDataValidInic || f.FicTecProdDataFim == null))
                                                                    .ToList();

                                    foreach (var itemFichaTecnicaFilhos in listaFichaTecnicaFilhos)
                                    {
                                        PLAN_PRODUC_FIC_TEC planejamentoProducaoFichaTecnica = new PLAN_PRODUC_FIC_TEC();

                                        planejamentoProducaoFichaTecnica.EmpCod = "3";
                                        planejamentoProducaoFichaTecnica.PlanProducNum = planoProducao.PlanProducNum;
                                        planejamentoProducaoFichaTecnica.ProdCodEstr = itemPlanoProducao.ProdCodEstr;
                                        planejamentoProducaoFichaTecnica.ItPlanProducSeq = itemPlanoProducao.ItPlanProducSeq;
                                        planejamentoProducaoFichaTecnica.FTProdCodEstr = itemPlanoProducao.ProdCodEstr;
                                        planejamentoProducaoFichaTecnica.FicTecProdSeq = itemFichaTecnicaFilhos.FicTecProdSeq;
                                        planejamentoProducaoFichaTecnica.PlanProducFicTecProdCodEstr = itemFichaTecnicaFilhos.FicTecProdCodEstr;
                                        planejamentoProducaoFichaTecnica.PlanProducFicTecQtd = itemFichaTecnicaFilhos.FicTecProdQtd * itemPlanoProducao.ItPlanProducQtd;

                                        existe = 0;
                                        existe = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                            .Where(s => s.EmpCod == "3" &&
                                                s.ProdCodEstr == itemFichaTecnicaFilhos.FicTecProdCodEstr)
                                            .OrderByDescending(s => s.SaldoEstqData)
                                            .Count();

                                        decimal? saldoFilhoQtd = 0;

                                        if (existe > 0)
                                        {
                                            SALDO_ESTQ_DATA saldoFilho = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                            .Where(s => s.EmpCod == "3" &&
                                                s.ProdCodEstr == itemFichaTecnicaFilhos.FicTecProdCodEstr)
                                            .OrderByDescending(s => s.SaldoEstqData)
                                            .First();

                                            saldoFilhoQtd = saldoFilho.SaldoEstqDataQtd;
                                        }

                                        planejamentoProducaoFichaTecnica.PlanProducFicTecQtdEstq = saldoFilhoQtd;
                                        planejamentoProducaoFichaTecnica.PlanProducFicTecQtdReserv = 0;
                                        planejamentoProducaoFichaTecnica.PlanProducFicTecQtdEmp = 0;
                                        planejamentoProducaoFichaTecnica.PlanProducFicTecQtdNec = planejamentoProducaoFichaTecnica.PlanProducFicTecQtd;
                                        planejamentoProducaoFichaTecnica.PlanProducFicTecQtdComp = 0;

                                        bdDiarioProducaoRacao.PLAN_PRODUC_FIC_TEC.AddObject(planejamentoProducaoFichaTecnica);
                                    }

                                    bdDiarioProducaoRacao.SaveChanges();

                                    bdDiarioProducaoRacao.GeraOrdemProducao(planoProducao.PlanProducNum, itemPlanoProducao.ProdCodEstr,
                                        itemPlanoProducao.ItPlanProducSeq, null, "3", planoProducao.PlanProducData, "RIOSOFT");

                                    bdDiarioProducaoRacao.SaveChanges();

                                    ORD_PRODUC ordProducNum = bdDiarioProducaoRacao.ORD_PRODUC
                                        .Where(o => o.EmpCod == "3" && o.PlanProducNum == planoProducao.PlanProducNum)
                                        .First();

                                    Cell localCelula = linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First();
                                    string nucleo = FromExcelTextBollean(localCelula, spreadsheetDocumentTeste.WorkbookPart);

                                    LOC_ARMAZ local = bdDiarioProducaoRacao.LOC_ARMAZ
                                        .Where(l => l.LocArmazNome.Contains(nucleo)).First();

                                    ordProducNum.LocArmazCodEstr = local.LocArmazCodEstr;

                                    OPER_ORD_PRODUC operOrdProduc = new OPER_ORD_PRODUC();

                                    operOrdProduc.EmpCod = "3";
                                    operOrdProduc.OrdProducNum = ordProducNum.OrdProducNum;
                                    operOrdProduc.ProdCodEstr = ordProducNum.ProdCodEstr;
                                    operOrdProduc.ProdOperSeq = 10;

                                    OPER_ORD_PRODUC ultimoOperOrdProduc = bdDiarioProducaoRacao.OPER_ORD_PRODUC
                                        .Where(o => o.EmpCod == "3").OrderByDescending(o => o.OperOrdProducSeq)
                                        .First();

                                    operOrdProduc.OperOrdProducSeq = ultimoOperOrdProduc.OperOrdProducSeq + 1;
                                    operOrdProduc.CCtrlCodEstr = "1.07.0001";
                                    operOrdProduc.OperOrdProducStat = "Manual";

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "H" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        double d = double.Parse(linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "H" + linha.RowIndex)
                                            .First().InnerText.Replace(".", ","));

                                        DateTime dt = DateTime.FromOADate(d);

                                        string dataHora = String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " " +
                                            String.Format("{0:hh:mm}", dt);

                                        operOrdProduc.OperOrdProducDataHoraInic = Convert.ToDateTime(dataHora);
                                    }
                                    else
                                    {
                                        operOrdProduc.OperOrdProducDataHoraInic = null;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "I" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        double d = double.Parse(linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "I" + linha.RowIndex)
                                            .First().InnerText.Replace(".", ","));

                                        DateTime dt = DateTime.FromOADate(d);

                                        string dataHora = String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " " +
                                            String.Format("{0:hh:mm}", dt);

                                        operOrdProduc.OperOrdProducDataHoraFim = Convert.ToDateTime(dataHora);
                                    }
                                    else
                                    {
                                        operOrdProduc.OperOrdProducDataHoraFim = null;
                                    }

                                    operOrdProduc.OperOrdProducQtdBoa = itemPlanoProducao.ItPlanProducQtd;
                                    operOrdProduc.OperOrdProducQtdRefug = 0;
                                    operOrdProduc.OperOrdProducQtdReproc = 0;
                                    operOrdProduc.OperCod = "0000001";
                                    operOrdProduc.UsuCod = "MNOTTI";

                                    DateTime dataInicial = Convert.ToDateTime(operOrdProduc.OperOrdProducDataHoraInic);
                                    DateTime dataFinal = Convert.ToDateTime(operOrdProduc.OperOrdProducDataHoraFim);

                                    decimal tempoCent = ((dataInicial - dataFinal).Minutes / 60);

                                    operOrdProduc.OperOrdProducTempoCent = tempoCent;
                                    operOrdProduc.OperOrdProducApont = "Operação";
                                    //operOrdProduc.OperOrdProducApont = "Preparação";
                                    operOrdProduc.OperOrdProducGerReqMat = "Sim";
                                    operOrdProduc.OperOrdProducPesoUnitProd = 0;
                                    operOrdProduc.OperOrdProducQtdRetalho = 0;
                                    operOrdProduc.AtivGrpCodEstr = "01.01";
                                    operOrdProduc.OperOrdProducGeraLoteAutom = "Configuração";

                                    /*
                                        * Ocorrência 3 - 14 - MNOTTI
                                        * 
                                        * Erro ao incluir quando o produto não tem a Unidade de Medida KG.
                                        * Foi definido que quando não existir, será exibida uma mensagem avisando esse problema.
                                        * 
                                        * 
                                        filho.FicTecProdUnidMedCodDig = "KG";
                                        filho.FicTecProdUnidMedPosDig = 1;
                                        */

                                    existe = 0;
                                    existe = bdDiarioProducaoRacao.PROD_UNID_MED
                                        .Where(u => u.ProdCodEstr == operOrdProduc.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                        .Count();

                                    if (existe > 0)
                                    {
                                        PROD_UNID_MED prodUnidMed = bdDiarioProducaoRacao.PROD_UNID_MED
                                            .Where(u => u.ProdCodEstr == operOrdProduc.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                            .First();

                                        operOrdProduc.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                        operOrdProduc.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                    }
                                    else
                                    {
                                        ViewBag.fileName = "";
                                        ViewBag.erro = "Erro ao realizar a importação: O produto " + operOrdProduc.ProdCodEstr
                                            + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                            + " novamente!";
                                        return View("Index", "");
                                    }

                                    /****/

                                    operOrdProduc.OperOrdProducCompBruto = 0;
                                    operOrdProduc.OperOrdProducCompLiq = 0;
                                    operOrdProduc.OperOrdProducAltBruta = 0;
                                    operOrdProduc.OperOrdProducAltLiq = 0;
                                    operOrdProduc.OperOrdProducLargBruta = 0;
                                    operOrdProduc.OperOrdProducLargLiq = 0;
                                    operOrdProduc.OperOrdProducEspacoBruto = 0;
                                    operOrdProduc.OperOrdProducEspacoLiq = 0;
                                    operOrdProduc.OperOrdProducDataHoraApont = DateTime.Now;
                                    operOrdProduc.OperOrdProducTara = 0;
                                    operOrdProduc.OperOrdProducPesoBruto = 0;
                                    operOrdProduc.OperOrdProducTipo = "Produção";
                                    operOrdProduc.OperOrdProducQtdReal = 0;
                                    operOrdProduc.TipoLancCod = codigoProdutoPai1.USERTipoLancEntradaProd;
                                    operOrdProduc.OperOrdProducIntegraEstq = "Sim";
                                    operOrdProduc.OperOrdProducIntegradoEstq = "Não";
                                    operOrdProduc.OperOrdProducUnidMedPeso = 1;
                                    operOrdProduc.OperOrdProducQtdCalc = operOrdProduc.OperOrdProducQtdBoa;

                                    bdDiarioProducaoRacao.OPER_ORD_PRODUC.AddObject(operOrdProduc);

                                    OPER_ORD_PRODUC_FUNC operOrdProducFunc = new OPER_ORD_PRODUC_FUNC();

                                    operOrdProducFunc.EmpCod = "3";
                                    operOrdProducFunc.OrdProducNum = operOrdProduc.OrdProducNum;
                                    operOrdProducFunc.ProdCodEstr = operOrdProduc.ProdCodEstr;
                                    operOrdProducFunc.ProdOperSeq = operOrdProduc.ProdOperSeq;
                                    operOrdProducFunc.OperOrdProducSeq = operOrdProduc.OperOrdProducSeq;

                                    Cell responsavelCelula = linha.Elements<Cell>().Where(c => c.CellReference == "M" + linha.RowIndex).First();
                                    string responsavel = FromExcelTextBollean(responsavelCelula, spreadsheetDocumentTeste.WorkbookPart);

                                    if (responsavel == "Paulo Sérgio")
                                    {
                                        operOrdProducFunc.FuncCod = "0000008";
                                    }
                                    else if (responsavel == "Leandro")
                                    {
                                        operOrdProducFunc.FuncCod = "0000011";
                                    }
                                    else if (responsavel == "Osmanir")
                                    {
                                        operOrdProducFunc.FuncCod = "0000110";
                                    }
                                    else if (responsavel == "Caio Santiago")
                                    {
                                        operOrdProducFunc.FuncCod = "0000118";
                                    }
                                    else if (responsavel == "Pedro Fuentes")
                                    {
                                        operOrdProducFunc.FuncCod = "0000286";
                                    }

                                    operOrdProducFunc.OperOrdProducFuncApont = "Sim";

                                    bdDiarioProducaoRacao.OPER_ORD_PRODUC_FUNC.AddObject(operOrdProducFunc);

                                    bdDiarioProducaoRacao.SaveChanges();

                                    bdDiarioProducaoRacao.InsertOperOrdProducProc("3", ordProducNum.OrdProducNum, operOrdProduc.OperOrdProducSeq, operOrdProduc.ProdCodEstr);

                                    bdDiarioProducaoRacao.SaveChanges();

                                    ordemProducaoExibe.CodigoApolo = codigoProdutoPai.ProdCodEstr;
                                    ordemProducaoExibe.CodFormula = codigoFormula;
                                    ordemProducaoExibe.TipoRacao = codigoProdutoPai.ProdNome;
                                    ordemProducaoExibe.TotalProduzido = operOrdProduc.OperOrdProducQtdBoa;
                                    ordemProducaoExibe.OrdemProducao = operOrdProduc.OrdProducNum;
                                    ordemProducaoExibe.Responsavel = responsavel;
                                    ordemProducaoExibe.NucleoGalpao = nucleo;

                                    //**** Insere os Materiais Adicionais ****
                                    decimal qtde = 0;
                                    Row linhaProdutoAdicional = sheetDataTeste.Elements<Row>().Where(r => r.RowIndex == 8).First();

                                    // Célula N8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "N8", "N", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional01 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula O8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "O8", "O", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional02 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula P8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "P8", "P", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional03 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula Q8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "Q8", "Q", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional04 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula R8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "R8", "R", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional05 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula S8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "S8", "S", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional06 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula T8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "T8", "T", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional07 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula U8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "U8", "U", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional08 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula V8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "V8", "V", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional09 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula W8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "W8", "W", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional10 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula X8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "X8", "X", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional11 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula Y8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "Y8", "Y", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional12 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula Z8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "Z8", "Z", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional13 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AA8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AA8", "AA", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional14 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AB8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AB8", "AB", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional15 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AC8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AC8", "AC", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional16 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AD8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AD8", "AD", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional17 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AE8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AE8", "AE", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional18 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AF8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AF8", "AF", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional19 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AG8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AG8", "AG", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional20 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AH8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AH8", "AH", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional21 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AI8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AI8", "AI", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional22 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AJ8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AJ8", "AJ", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional23 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AK8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AK8", "AK", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional24 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AL8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AL8", "AL", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional25 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AM8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AM8", "AM", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional26 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AN8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AN8", "AN", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional27 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AO8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AO8", "AO", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional28 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AP8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AP8", "AP", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional29 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AQ8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AQ8", "AQ", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional30 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AR8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AR8", "AR", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional31 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);
                                    // Célula AS8
                                    qtde = InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocumentTeste.WorkbookPart, "AS8", "AS", ordProducNum.OrdProducNum, FromExcelSerialDate(Convert.ToInt32(celulaDataTeste.InnerText)));
                                    ordemProducaoExibe.Adicional32 = qtde == -1 ? "Não Baixado" : String.Format("{0:F}", qtde) == "0,00" ? "" : String.Format("{0:F}", qtde);

                                    importado = VerificaOrdemProducaoBaixada(ordemProducaoExibe.OrdemProducao);

                                    ordemProducaoExibe.Importado = importado;

                                    bd.OrdemProducao.Add(ordemProducaoExibe);

                                    bd.SaveChanges();
                                }
                            }
                        }

                        sequenciaLinha.Importado = "Sim";

                        int perc = 0;
                        if (sequenciaLinha != null)
                        {
                            decimal parte = Convert.ToDecimal(listaSequencia.Where(s => s.Importado == "Sim").Count());
                            decimal total = Convert.ToDecimal(listaSequencia.Count);
                            perc = Convert.ToInt32(((parte / total) * 100));
                        }

                        if (ordemProducaoExibe.CodFormula != 0)
                        {
                            ordemProducaoExibe.Erro = "Importando Fórmula " + ordemProducaoExibe.CodFormula.ToString() + " - " + ordemProducaoExibe.TipoRacao;
                            formulaAnterior = ordemProducaoExibe.Erro;
                            if (ordemProducaoExibe.PercentagemImportada != null) percAnterior = Convert.ToInt32(ordemProducaoExibe.PercentagemImportada);
                        }
                        else
                        {
                            ordemProducaoExibe.Erro = formulaAnterior;
                            ordemProducaoExibe.PercentagemImportada = percAnterior;
                        }
                        ordemProducaoExibe.PercentagemImportada = perc;
                    }
                    else
                    {
                        ordemProducaoExibe.Erro = formulaAnterior;
                        ordemProducaoExibe.PercentagemImportada = percAnterior;
                    }
                }
                return Json(ordemProducaoExibe);
            }
            catch (Exception e)
            {
                ordemProducaoExibe.Erro = "Erro ao importar linha " + linhaPLanilha + ": " + e.Message;
                ordemProducaoExibe.PercentagemImportada = 0;
                return Json(ordemProducaoExibe);
            }
        }

        #endregion

        #region Visualiza Lista Importada

        [HttpPost]
        public ActionResult RetornaListaImportada()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            if (spreadsheetDocumentTeste != null)
            {
                ViewBag.AdicionalM8 = DescricaoProduto(linhaAdicional, "M8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalN8 = DescricaoProduto(linhaAdicional, "N8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalO8 = DescricaoProduto(linhaAdicional, "O8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalP8 = DescricaoProduto(linhaAdicional, "P8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalQ8 = DescricaoProduto(linhaAdicional, "Q8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalR8 = DescricaoProduto(linhaAdicional, "R8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalS8 = DescricaoProduto(linhaAdicional, "S8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalT8 = DescricaoProduto(linhaAdicional, "T8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalU8 = DescricaoProduto(linhaAdicional, "U8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalV8 = DescricaoProduto(linhaAdicional, "V8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalW8 = DescricaoProduto(linhaAdicional, "W8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalX8 = DescricaoProduto(linhaAdicional, "X8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalY8 = DescricaoProduto(linhaAdicional, "Y8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalZ8 = DescricaoProduto(linhaAdicional, "Z8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAA8 = DescricaoProduto(linhaAdicional, "AA8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAB8 = DescricaoProduto(linhaAdicional, "AB8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAC8 = DescricaoProduto(linhaAdicional, "AC8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAD8 = DescricaoProduto(linhaAdicional, "AD8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAE8 = DescricaoProduto(linhaAdicional, "AE8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAF8 = DescricaoProduto(linhaAdicional, "AF8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAG8 = DescricaoProduto(linhaAdicional, "AG8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAH8 = DescricaoProduto(linhaAdicional, "AH8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAI8 = DescricaoProduto(linhaAdicional, "AI8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAJ8 = DescricaoProduto(linhaAdicional, "AJ8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAK8 = DescricaoProduto(linhaAdicional, "AK8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAL8 = DescricaoProduto(linhaAdicional, "AL8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAM8 = DescricaoProduto(linhaAdicional, "AM8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAN8 = DescricaoProduto(linhaAdicional, "AN8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAO8 = DescricaoProduto(linhaAdicional, "AO8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAP8 = DescricaoProduto(linhaAdicional, "AP8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAQ8 = DescricaoProduto(linhaAdicional, "AQ8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAR8 = DescricaoProduto(linhaAdicional, "AR8", spreadsheetDocumentTeste.WorkbookPart);
                ViewBag.AdicionalAS8 = DescricaoProduto(linhaAdicional, "AS8", spreadsheetDocumentTeste.WorkbookPart);

                var listaExibicaoOK = bd.OrdemProducao;
                return View("Index", listaExibicaoOK);
            }
            else
            {
                var listaExibicaoErro = bd.OrdemProducao;

                ViewBag.fileName = "";
                ViewBag.erro = "Não existem dados a serem visualizados!";
                return View("Index", listaExibicaoErro);
            }
        }

        #endregion

        #region Nova rotina onde são importadas as linhas e depois o usuário realiza a importação manualmente

        public ActionResult ImportaDiarioPR()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            bdDiarioProducaoRacao.CommandTimeout = 100000;

            //if (bd.OrdemProducao.Count() > 0)
            //    ViewBag.erro = "** EXISTE PLANILHA IMPORTADA!!! CASO IMPORTE NOVAMENTE, OS DADOS ABAIXO SERÃO DELETADOS!!! **";

            var anoMes = DateTime.Today.ToString("yyyy-MM");
            DateTime dataInicial = Convert.ToDateTime(anoMes + "-01");
            Session["sDataInicial"] = dataInicial;
            DateTime dataFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            Session["sDataFinal"] = dataFinal;

            return View(bd.OrdemProducao.Where(w => w.DataProducao >= dataInicial && w.DataProducao <= dataFinal).ToList());
        }

        [HttpPost]
        public ActionResult ImportaPlanilhaDadosDiarioProducaoRacao()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Deleta Dados Antigos

            bd.Database.ExecuteSqlCommand("delete from LayoutOrdemProducaos");
            bd.SaveChanges();

            #endregion

            #region Copia o Arquivo para realizar a importação

            bdDiarioProducaoRacao.CommandTimeout = 10000;
            string caminho = @"C:\inetpub\wwwroot\Relatorios\ProducaoRacao\DPR\DiarioProducaoRacao_" 
                + Session["login"].ToString() + ".xlsx";

            Request.Files[0].SaveAs(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            #endregion

            try
            {
                #region Carrega Excel

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                #endregion

                int existe = 0;

                foreach (var planilha in lista)
                {
                    // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                    if (planilha.Name.ToString() == "Rel.Dia.Produção")
                    {
                        #region Carrega Linhas do Cabeçalho

                        string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == planilha.Name)
                                                    .First().Id;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                        .GetPartById(relationshipId);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        var listaLinhas = sheetData.Descendants<Row>().ToList();

                        #endregion

                        #region Descrição dos Produtos Adicionais

                        linhaAdicional = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                        string adicionalO8 = CodigoProduto(linhaAdicional, "O8", spreadsheetDocument.WorkbookPart);
                        string adicionalP8 = CodigoProduto(linhaAdicional, "P8", spreadsheetDocument.WorkbookPart);
                        string adicionalQ8 = CodigoProduto(linhaAdicional, "Q8", spreadsheetDocument.WorkbookPart);
                        string adicionalR8 = CodigoProduto(linhaAdicional, "R8", spreadsheetDocument.WorkbookPart);
                        string adicionalS8 = CodigoProduto(linhaAdicional, "S8", spreadsheetDocument.WorkbookPart);
                        string adicionalT8 = CodigoProduto(linhaAdicional, "T8", spreadsheetDocument.WorkbookPart);
                        string adicionalU8 = CodigoProduto(linhaAdicional, "U8", spreadsheetDocument.WorkbookPart);
                        string adicionalV8 = CodigoProduto(linhaAdicional, "V8", spreadsheetDocument.WorkbookPart);
                        string adicionalW8 = CodigoProduto(linhaAdicional, "W8", spreadsheetDocument.WorkbookPart);
                        string adicionalX8 = CodigoProduto(linhaAdicional, "X8", spreadsheetDocument.WorkbookPart);
                        string adicionalY8 = CodigoProduto(linhaAdicional, "Y8", spreadsheetDocument.WorkbookPart);
                        string adicionalZ8 = CodigoProduto(linhaAdicional, "Z8", spreadsheetDocument.WorkbookPart);
                        string adicionalAA8 = CodigoProduto(linhaAdicional, "AA8", spreadsheetDocument.WorkbookPart);
                        string adicionalAB8 = CodigoProduto(linhaAdicional, "AB8", spreadsheetDocument.WorkbookPart);
                        string adicionalAC8 = CodigoProduto(linhaAdicional, "AC8", spreadsheetDocument.WorkbookPart);
                        string adicionalAD8 = CodigoProduto(linhaAdicional, "AD8", spreadsheetDocument.WorkbookPart);
                        string adicionalAE8 = CodigoProduto(linhaAdicional, "AE8", spreadsheetDocument.WorkbookPart);
                        string adicionalAF8 = CodigoProduto(linhaAdicional, "AF8", spreadsheetDocument.WorkbookPart);
                        string adicionalAG8 = CodigoProduto(linhaAdicional, "AG8", spreadsheetDocument.WorkbookPart);
                        string adicionalAH8 = CodigoProduto(linhaAdicional, "AH8", spreadsheetDocument.WorkbookPart);
                        string adicionalAI8 = CodigoProduto(linhaAdicional, "AI8", spreadsheetDocument.WorkbookPart);
                        string adicionalAJ8 = CodigoProduto(linhaAdicional, "AJ8", spreadsheetDocument.WorkbookPart);
                        string adicionalAK8 = CodigoProduto(linhaAdicional, "AK8", spreadsheetDocument.WorkbookPart);
                        string adicionalAL8 = CodigoProduto(linhaAdicional, "AL8", spreadsheetDocument.WorkbookPart);
                        string adicionalAM8 = CodigoProduto(linhaAdicional, "AM8", spreadsheetDocument.WorkbookPart);
                        string adicionalAN8 = CodigoProduto(linhaAdicional, "AN8", spreadsheetDocument.WorkbookPart);
                        string adicionalAO8 = CodigoProduto(linhaAdicional, "AO8", spreadsheetDocument.WorkbookPart);
                        string adicionalAP8 = CodigoProduto(linhaAdicional, "AP8", spreadsheetDocument.WorkbookPart);
                        string adicionalAQ8 = CodigoProduto(linhaAdicional, "AQ8", spreadsheetDocument.WorkbookPart);
                        string adicionalAR8 = CodigoProduto(linhaAdicional, "AR8", spreadsheetDocument.WorkbookPart);
                        string adicionalAS8 = CodigoProduto(linhaAdicional, "AS8", spreadsheetDocument.WorkbookPart);
                        string adicionalAT8 = CodigoProduto(linhaAdicional, "AT8", spreadsheetDocument.WorkbookPart);
                        string adicionalAU8 = CodigoProduto(linhaAdicional, "AU8", spreadsheetDocument.WorkbookPart);
                        string adicionalAV8 = CodigoProduto(linhaAdicional, "AV8", spreadsheetDocument.WorkbookPart);
                        string adicionalAW8 = CodigoProduto(linhaAdicional, "AW8", spreadsheetDocument.WorkbookPart);
                        string adicionalAX8 = CodigoProduto(linhaAdicional, "AX8", spreadsheetDocument.WorkbookPart);
                        string adicionalAY8 = CodigoProduto(linhaAdicional, "AY8", spreadsheetDocument.WorkbookPart);
                        string adicionalAZ8 = CodigoProduto(linhaAdicional, "AZ8", spreadsheetDocument.WorkbookPart);
                        string adicionalBA8 = CodigoProduto(linhaAdicional, "BA8", spreadsheetDocument.WorkbookPart);
                        string adicionalBB8 = CodigoProduto(linhaAdicional, "BB8", spreadsheetDocument.WorkbookPart);
                        string adicionalBC8 = CodigoProduto(linhaAdicional, "BC8", spreadsheetDocument.WorkbookPart);
                        string adicionalBD8 = CodigoProduto(linhaAdicional, "BD8", spreadsheetDocument.WorkbookPart);
                        string adicionalBE8 = CodigoProduto(linhaAdicional, "BE8", spreadsheetDocument.WorkbookPart);
                        string adicionalBF8 = CodigoProduto(linhaAdicional, "BF8", spreadsheetDocument.WorkbookPart);
                        string adicionalBG8 = CodigoProduto(linhaAdicional, "BG8", spreadsheetDocument.WorkbookPart);
                        string adicionalBH8 = CodigoProduto(linhaAdicional, "BH8", spreadsheetDocument.WorkbookPart);
                        string adicionalBI8 = CodigoProduto(linhaAdicional, "BI8", spreadsheetDocument.WorkbookPart);
                        string adicionalBJ8 = CodigoProduto(linhaAdicional, "BJ8", spreadsheetDocument.WorkbookPart);
                        string adicionalBK8 = CodigoProduto(linhaAdicional, "BK8", spreadsheetDocument.WorkbookPart);
                        string adicionalBL8 = CodigoProduto(linhaAdicional, "BL8", spreadsheetDocument.WorkbookPart);
                        string adicionalBM8 = CodigoProduto(linhaAdicional, "BM8", spreadsheetDocument.WorkbookPart);
                        string adicionalBN8 = CodigoProduto(linhaAdicional, "BN8", spreadsheetDocument.WorkbookPart);

                        #endregion

                        foreach (var linha in listaLinhas)
                        {
                            #region Recupera Código da Fórmula

                            existe = 0;
                            existe = linha.Elements<Cell>()
                                .Where(c => c.CellReference == "F" + linha.RowIndex).Count();

                            if (existe > 0)
                                existe = linha.Elements<Cell>()
                                    .Where(c => c.CellReference == "F" + linha.RowIndex)
                                    .First().Descendants<CellValue>().Count();

                            int sequencia = 0;
                            linhaErro = Convert.ToInt32(linha.RowIndex.Value);

                            #endregion

                            if ((existe > 0) && (linha.RowIndex >= 10))
                            {
                                #region Verifica se Fórmula está no Apolo

                                int codigoFormula = 0;

                                codigoFormula = Convert.ToInt32(linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "F" + linha.RowIndex)
                                                    .First().InnerText);

                                existe = 0;
                                existe = bdDiarioProducaoRacao.PRODUTO1
                                        .Where(p => p.USERNumFormula == codigoFormula)
                                        .Count();

                                #endregion

                                if (existe > 0)
                                {
                                    #region Verifica Estrutura da Fórmula

                                    PRODUTO1 codigoProdutoPai1 = bdDiarioProducaoRacao.PRODUTO1
                                            .Where(p => p.USERNumFormula == codigoFormula)
                                            .First();

                                    MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO codigoProdutoPai = bdDiarioProducaoRacao.PRODUTO
                                        .Where(p => p.ProdCodEstr == codigoProdutoPai1.ProdCodEstr)
                                        .First();

                                    existe = 0;
                                    existe = bdDiarioProducaoRacao.FIC_TEC_PROD
                                        .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr).Count();

                                    #endregion

                                    // Caso ele exista, realiza as outras operações
                                    if (existe > 0)
                                    {
                                        #region Carrega Valores Planilha

                                        #region Data Produção

                                        Cell celulaData = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "D" + linha.RowIndex).First();
                                        DateTime dataProducao = FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText));

                                        #endregion

                                        #region Total Produzido Fórmula

                                        decimal totalProduzido = 0;
                                        if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference == "H" + linha.RowIndex)
                                                    .First().InnerText != "")
                                        {
                                            totalProduzido = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference == "H" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);
                                        }

                                        #endregion

                                        #region Responsável

                                        Cell responsavelCelula = linha.Elements<Cell>().Where(c => c.CellReference == "N" + linha.RowIndex).First();
                                        string responsavel = FromExcelTextBollean(responsavelCelula, spreadsheetDocument.WorkbookPart);

                                        #endregion

                                        #region Núcleo

                                        Cell localCelula = linha.Elements<Cell>().Where(c => c.CellReference == "M" + linha.RowIndex).First();
                                        string nucleo = FromExcelTextBollean(localCelula, spreadsheetDocument.WorkbookPart);

                                        #endregion

                                        #region Lote

                                        Cell localLote = linha.Elements<Cell>().Where(c => c.CellReference == "K" + linha.RowIndex).First();
                                        string lote = FromExcelTextBollean(localLote, spreadsheetDocument.WorkbookPart);

                                        #region Verifica se existe número de lote no sistema, não permitindo ser repetido

                                        if (lote == "")
                                        {
                                            arquivo.Close();
                                            ViewBag.erro = "A linha " + linha.RowIndex + " está sem o número do lote! Por favor, insira o valor e já verifique "
                                                + "as outras linhas!";
                                            return View("ImportaDiarioPR", bd.OrdemProducao);
                                        }

                                        existe = bdDiarioProducaoRacao.ORD_PRODUC.Where(o => o.OrdProducOrigem == lote).Count();
                                        if (existe > 0)
                                        {
                                            arquivo.Close();
                                            ViewBag.erro = "Já existe o número do Lote " + lote + " inserido no sistema!"
                                                + " Esse número não pode ser repetido! Por favor, realize a correção para importar novamente!";
                                            return View("ImportaDiarioPR", bd.OrdemProducao);
                                        }

                                        #endregion

                                        #endregion

                                        #region Hora Início Produção

                                        decimal horaIncioProducao = 0;
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "I" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            double d = double.Parse(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "I" + linha.RowIndex)
                                                .First().InnerText.Replace(".", ","));
                                            
                                            horaIncioProducao = Convert.ToDecimal(d);
                                        }

                                        #endregion
                                        
                                        #region Hora Término Produção

                                        decimal horaTerminoProducao = 0;
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "J" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            double d = double.Parse(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "J" + linha.RowIndex)
                                                .First().InnerText.Replace(".", ","));
                                            
                                            horaTerminoProducao = Convert.ToDecimal(d);
                                        }

                                        #endregion

                                        #endregion

                                        #region Verifica se existe ordem de produção e armazena o número

                                        string ordProducNum = "";
                                        ORD_PRODUC op = bdDiarioProducaoRacao.ORD_PRODUC
                                            .Where(w => w.OrdProducOrigem == lote).FirstOrDefault();
                                        if (op != null) ordProducNum = op.OrdProducNum;

                                        #endregion

                                        #region Carrega Valores para Exibição se maior de 01/04/2018

                                        if (dataProducao >= Convert.ToDateTime("01/04/2018"))
                                        {
                                            #region Valores da Fórmula

                                            // Exibição
                                            LayoutOrdemProducao ordemProducaoExibe = new LayoutOrdemProducao();

                                            ordemProducaoExibe.DataProducao = dataProducao;
                                            ordemProducaoExibe.CodigoApolo = codigoProdutoPai.ProdCodEstr;
                                            ordemProducaoExibe.CodFormula = codigoFormula;
                                            ordemProducaoExibe.TipoRacao = codigoProdutoPai.ProdNome;
                                            ordemProducaoExibe.TotalProduzido = totalProduzido;
                                            ordemProducaoExibe.Responsavel = responsavel;
                                            ordemProducaoExibe.NucleoGalpao = nucleo;
                                            ordemProducaoExibe.OrdemProducao = lote;
                                            ordemProducaoExibe.HoraInicioProducao = horaIncioProducao;
                                            ordemProducaoExibe.HoraTerminoProducao = horaTerminoProducao;

                                            #endregion

                                            #region Adicionais

                                            Row linhaProdutoAdicional = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();

                                            // Célula O8
                                            ordemProducaoExibe.Adicional01 = adicionalO8;
                                            ordemProducaoExibe.Qtde01 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "O8", "O");
                                            // Célula P8
                                            ordemProducaoExibe.Adicional02 = adicionalP8;
                                            ordemProducaoExibe.Qtde02 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "P8", "P");
                                            // Célula Q8
                                            ordemProducaoExibe.Adicional03 = adicionalQ8;
                                            ordemProducaoExibe.Qtde03 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "Q8", "Q");
                                            // Célula R8
                                            ordemProducaoExibe.Adicional04 = adicionalR8;
                                            ordemProducaoExibe.Qtde04 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "R8", "R");
                                            // Célula S8
                                            ordemProducaoExibe.Adicional05 = adicionalS8;
                                            ordemProducaoExibe.Qtde05 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "S8", "S");
                                            // Célula T8
                                            ordemProducaoExibe.Adicional06 = adicionalT8;
                                            ordemProducaoExibe.Qtde06 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "T8", "T");
                                            // Célula U8
                                            ordemProducaoExibe.Adicional07 = adicionalU8;
                                            ordemProducaoExibe.Qtde07 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "U8", "U");
                                            // Célula V8
                                            ordemProducaoExibe.Adicional08 = adicionalV8;
                                            ordemProducaoExibe.Qtde08 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "V8", "V");
                                            // Célula W8
                                            ordemProducaoExibe.Adicional09 = adicionalW8;
                                            ordemProducaoExibe.Qtde09 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "W8", "W");
                                            // Célula X8
                                            ordemProducaoExibe.Adicional10 = adicionalX8;
                                            ordemProducaoExibe.Qtde10 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "X8", "X");
                                            // Célula Y8
                                            ordemProducaoExibe.Adicional11 = adicionalY8;
                                            ordemProducaoExibe.Qtde11 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "Y8", "Y");
                                            // Célula Z8
                                            ordemProducaoExibe.Adicional12 = adicionalZ8;
                                            ordemProducaoExibe.Qtde12 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "Z8", "Z");
                                            // Célula AA8
                                            ordemProducaoExibe.Adicional13 = adicionalAA8;
                                            ordemProducaoExibe.Qtde13 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AA8", "AA");
                                            // Célula AB8
                                            ordemProducaoExibe.Adicional14 = adicionalAB8;
                                            ordemProducaoExibe.Qtde14 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AB8", "AB");
                                            // Célula AC8
                                            ordemProducaoExibe.Adicional15 = adicionalAC8;
                                            ordemProducaoExibe.Qtde15 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AC8", "AC");
                                            // Célula AD8
                                            ordemProducaoExibe.Adicional16 = adicionalAD8;
                                            ordemProducaoExibe.Qtde16 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AD8", "AD");
                                            // Célula AE8
                                            ordemProducaoExibe.Adicional17 = adicionalAE8;
                                            ordemProducaoExibe.Qtde17 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AE8", "AE");
                                            // Célula AF8
                                            ordemProducaoExibe.Adicional18 = adicionalAF8;
                                            ordemProducaoExibe.Qtde18 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AF8", "AF");
                                            // Célula AG8
                                            ordemProducaoExibe.Adicional19 = adicionalAG8;
                                            ordemProducaoExibe.Qtde19 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AG8", "AG");
                                            // Célula AH8
                                            ordemProducaoExibe.Adicional20 = adicionalAH8;
                                            ordemProducaoExibe.Qtde20 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AH8", "AH");
                                            // Célula AI8
                                            ordemProducaoExibe.Adicional21 = adicionalAI8;
                                            ordemProducaoExibe.Qtde21 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AI8", "AI");
                                            // Célula AJ8
                                            ordemProducaoExibe.Adicional22 = adicionalAJ8;
                                            ordemProducaoExibe.Qtde22 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AJ8", "AJ");
                                            // Célula AK8
                                            ordemProducaoExibe.Adicional23 = adicionalAK8;
                                            ordemProducaoExibe.Qtde23 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AK8", "AK");
                                            // Célula AL8
                                            ordemProducaoExibe.Adicional24 = adicionalAL8;
                                            ordemProducaoExibe.Qtde24 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AL8", "AL");
                                            // Célula AM8
                                            ordemProducaoExibe.Adicional25 = adicionalAM8;
                                            ordemProducaoExibe.Qtde25 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AM8", "AM");
                                            // Célula AN8
                                            ordemProducaoExibe.Adicional26 = adicionalAN8;
                                            ordemProducaoExibe.Qtde26 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AN8", "AN");
                                            // Célula AO8
                                            ordemProducaoExibe.Adicional27 = adicionalAO8;
                                            ordemProducaoExibe.Qtde27 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AO8", "AO");
                                            // Célula AP8
                                            ordemProducaoExibe.Adicional28 = adicionalAP8;
                                            ordemProducaoExibe.Qtde28 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AP8", "AP");
                                            // Célula AQ8
                                            ordemProducaoExibe.Adicional29 = adicionalAQ8;
                                            ordemProducaoExibe.Qtde29 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AQ8", "AQ");
                                            // Célula AR8
                                            ordemProducaoExibe.Adicional30 = adicionalAR8;
                                            ordemProducaoExibe.Qtde30 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AR8", "AR");
                                            // Célula AS8
                                            ordemProducaoExibe.Adicional31 = adicionalAS8;
                                            ordemProducaoExibe.Qtde31 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AS8", "AS");
                                            // Célula AT8
                                            ordemProducaoExibe.Adicional32 = adicionalAT8;
                                            ordemProducaoExibe.Qtde32 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AT8", "AT");
                                            // Célula AU8
                                            ordemProducaoExibe.Adicional33 = adicionalAU8;
                                            ordemProducaoExibe.Qtde33 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AU8", "AU");
                                            // Célula AV8
                                            ordemProducaoExibe.Adicional34 = adicionalAV8;
                                            ordemProducaoExibe.Qtde34 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AV8", "AV");
                                            // Célula AW8
                                            ordemProducaoExibe.Adicional35 = adicionalAW8;
                                            ordemProducaoExibe.Qtde35 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AW8", "AW");
                                            // Célula AX8
                                            ordemProducaoExibe.Adicional36 = adicionalAX8;
                                            ordemProducaoExibe.Qtde36 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AX8", "AX");
                                            // Célula AY8
                                            ordemProducaoExibe.Adicional37 = adicionalAY8;
                                            ordemProducaoExibe.Qtde37 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AY8", "AY");
                                            // Célula AZ8
                                            ordemProducaoExibe.Adicional38 = adicionalAZ8;
                                            ordemProducaoExibe.Qtde38 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "AZ8", "AZ");
                                            // Célula BA8
                                            ordemProducaoExibe.Adicional39 = adicionalBA8;
                                            ordemProducaoExibe.Qtde39 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BA8", "BA");
                                            // Célula BB8
                                            ordemProducaoExibe.Adicional40 = adicionalBB8;
                                            ordemProducaoExibe.Qtde40 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BB8", "BB");
                                            // Célula BC8
                                            ordemProducaoExibe.Adicional41 = adicionalBC8;
                                            ordemProducaoExibe.Qtde41 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BC8", "BC");
                                            // Célula BD8
                                            ordemProducaoExibe.Adicional42 = adicionalBD8;
                                            ordemProducaoExibe.Qtde42 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BD8", "BD");
                                            // Célula BE8
                                            ordemProducaoExibe.Adicional43 = adicionalBE8;
                                            ordemProducaoExibe.Qtde43 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BE8", "BE");
                                            // Célula BF8
                                            ordemProducaoExibe.Adicional44 = adicionalBF8;
                                            ordemProducaoExibe.Qtde44 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BF8", "BF");
                                            // Célula BG8
                                            ordemProducaoExibe.Adicional45 = adicionalBG8;
                                            ordemProducaoExibe.Qtde45 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BG8", "BG");
                                            // Célula BH8
                                            ordemProducaoExibe.Adicional46 = adicionalBH8;
                                            ordemProducaoExibe.Qtde46 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BH8", "BH");
                                            // Célula BI8
                                            ordemProducaoExibe.Adicional47 = adicionalBI8;
                                            ordemProducaoExibe.Qtde47 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BI8", "BI");
                                            // Célula BJ8
                                            ordemProducaoExibe.Adicional48 = adicionalBJ8;
                                            ordemProducaoExibe.Qtde48 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BJ8", "BJ");
                                            // Célula BK8
                                            ordemProducaoExibe.Adicional49 = adicionalBK8;
                                            ordemProducaoExibe.Qtde49 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BK8", "BK");
                                            // Célula BL8
                                            ordemProducaoExibe.Adicional50 = adicionalBL8;
                                            ordemProducaoExibe.Qtde50 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BL8", "BL");
                                            // Célula BM8
                                            ordemProducaoExibe.Adicional51 = adicionalBM8;
                                            ordemProducaoExibe.Qtde51 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BM8", "BM");
                                            // Célula BN8
                                            ordemProducaoExibe.Adicional52 = adicionalBN8;
                                            ordemProducaoExibe.Qtde52 = RetornaQtdeProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linhaProdutoAdicional, linha, spreadsheetDocument.WorkbookPart, "BN8", "BN");

                                            #endregion

                                            #region Verifica Ordem Baixada

                                            string importado = VerificaOrdemProducaoBaixada(ordProducNum);

                                            ordemProducaoExibe.Importado = importado;

                                            bd.OrdemProducao.Add(ordemProducaoExibe);

                                            bd.SaveChanges();

                                            #endregion
                                        }

                                        #endregion
                                    }
                                }
                            }
                        }
                    }
                }

                var listaExibicaoOK = bd.OrdemProducao;

                arquivo.Close();

                return View("ImportaDiarioPR", listaExibicaoOK);
            }
            catch (Exception e)
            {
                var listaExibicaoErro = bd.OrdemProducao;

                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                ViewBag.fileName = "";
                if (e.InnerException == null)
                    ViewBag.erro = "Erro ao realizar a importação: " + e.Message + " | linha: "
                        + linhaErro.ToString() + " | linha erro código: " + linenum.ToString();
                else
                    ViewBag.erro = "Erro ao realizar a importação: " + e.Message
                        + " | Erro Interno: " + e.InnerException.Message
                        + " | linha: "
                        + linhaErro.ToString() + " | linha erro código: " + linenum.ToString();
                arquivo.Close();
                return View("ImportaDiarioPR", listaExibicaoErro);
            }
        }

        [HttpPost]
        public ActionResult ImportaWebDadosDiarioProducaoRacao(FormCollection model)
        {
            var anoMes = model["anoMes"].ToString();

            if (Convert.ToDateTime(anoMes+"-01") < Convert.ToDateTime("2020-09-01"))
            {
                ViewBag.erro = "OS DADOS DA WEB SÓ ESTÃO DISPONÍVEIS A PARTIR DE 09/2020!";
                var listaDPRErro = bd.OrdemProducao
                    .Where(w => w.OrdemProducao == "xx")
                    .OrderBy(o => o.DataProducao)
                    .ToList();

                return View("ImportaDiarioPR", listaDPRErro);
            }

            //var listaDPRExisteImportado = bd.OrdemProducao
            //    .Where(w => w.DataProducao.ToString("yyyy-MM") == anoMes && w.Importado == "Sim")
            //    .OrderBy(o => o.DataProducao)
            //    .ToList();

            //if (listaDPRExisteImportado.Count() > 0)
            //{
            //    ViewBag.erro = "NÃO É POSSÍVEL IMPORTAR OS DADOS DO WEB PORQUE EXISTEM DADOS JÁ IMPORTADOS PARA O APOLO! EXCLUA AS BAIXAS DO APOLO PARA IMPORTAR NOVAMENTE!";
            //    var listaDPRErro = bd.OrdemProducao
            //        .Where(w => w.DataProducao.ToString("yyyy-MM") == "")
            //        .OrderBy(o => o.DataProducao)
            //        .ToList();

            //    return View("ImportaDiarioPR", listaDPRErro);
            //}

            DateTime dataInicial = Convert.ToDateTime(anoMes + "-01");
            DateTime dataFinal = new DateTime(dataInicial.Year, dataInicial.Month, DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month));
            
            Models.HLBAPP.HLBAPPEntities1 hlbapp = new Models.HLBAPP.HLBAPPEntities1();
            hlbapp.Integra_Baixa_Ordem_Producao(dataInicial, dataFinal, Session["login"].ToString().ToUpper());

            var listaDPR = bd.OrdemProducao
                .Where(w => w.DataProducao >= dataInicial && w.DataProducao <= dataFinal)
                .OrderBy(o => o.DataProducao)
                .ToList();

            foreach (var item in listaDPR)
            {
                #region Verifica se existe ordem de produção e armazena o número

                string ordProducNum = "";
                ORD_PRODUC op = bdDiarioProducaoRacao.ORD_PRODUC.Where(w => w.OrdProducOrigem == item.OrdemProducao).FirstOrDefault();
                if (op != null) ordProducNum = op.OrdProducNum;

                #endregion

                #region Verifica Ordem Baixada

                string importado = VerificaOrdemProducaoBaixada(ordProducNum);

                item.Importado = importado;
                
                #endregion
            }

            bd.SaveChanges();

            ViewBag.fileName = "Dados do mês " + anoMes + " importado com sucesso!";

            var listaDPRRetorno = bd.OrdemProducao
                .Where(w => w.DataProducao >= dataInicial && w.DataProducao <= dataFinal)
                .OrderBy(o => o.DataProducao)
                .ToList();

            return View("ImportaDiarioPR", listaDPRRetorno);
        }

        [HttpPost]
        public ActionResult ConsultaImportaDiarioPR(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento dos Parâmetros

            #region Datas

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("ImportaDiarioPR", bd.OrdemProducao.ToList());
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("ImportaDiarioPR", bd.OrdemProducao.ToList());
            }

            #endregion

            #endregion

            DateTime dataIni = Convert.ToDateTime(model["dataIni"].ToString());
            DateTime dataFim = Convert.ToDateTime(model["dataFim"].ToString());

            var listaDPR = bd.OrdemProducao
                .Where(w => w.DataProducao >= dataIni && w.DataProducao <= dataFim)
                .OrderBy(o => o.DataProducao)
                .ToList();

            return View("ImportaDiarioPR", listaDPR);
        }

        [HttpPost]
        public ActionResult GerarAcoesSelecionados(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string retorno = "";

            try
            {
                #region Importa p/ Apolo

                if (model["importa"] != null)
                {
                    var fileIds = model["id"].Split(',');
                    var selectedIndices = model["importa"].Replace("true,false", "true")
                                .Split(',')
                                .Select((item, index) => new { item = item, index = index })
                                .Where(row => row.item == "true")
                                .Select(row => row.index).ToArray();

                    foreach (var index in selectedIndices)
                    {
                        int fileId;
                        if (int.TryParse(fileIds[index], out fileId))
                        {
                            retorno = ImportaLinhaDPRApolo(fileId);
                            if (retorno != "")
                            {
                                ViewBag.Erro = "Erro ao importar o ID " + fileId.ToString()
                                    + ": " + retorno;
                                return View("ImportaDiarioPR", bd.OrdemProducao.ToList());
                            }
                        }
                    }
                }

                #endregion

                #region Deleta do Apolo

                if (model["deleta"] != null)
                {
                    var fileIdsEmailFiscal = model["idDeleta"].Split(',');
                    var selectedIndicesEmailFiscal = model["deleta"].Replace("true,false", "true")
                                .Split(',')
                                .Select((item, index) => new { item = item, index = index })
                                .Where(row => row.item == "true")
                                .Select(row => row.index).ToArray();

                    foreach (var index in selectedIndicesEmailFiscal)
                    {
                        int fileId;
                        if (int.TryParse(fileIdsEmailFiscal[index], out fileId))
                        {
                            retorno = DeletaLinhaBaixadaApolo(fileId);
                            if (retorno != "")
                            {
                                ViewBag.Erro = "Erro ao deletar o ID " + fileId.ToString()
                                    + ": " + retorno;
                                return View("ImportaDiarioPR", bd.OrdemProducao.ToList());
                            }
                        }
                    }
                }

                #endregion

                ViewBag.fileName = "As importações / exclusões selecionadas foram realizadas com sucesso!";

                DateTime dataIni = Convert.ToDateTime(Session["sDataInicial"]);
                DateTime dataFim = Convert.ToDateTime(Session["sDataFinal"]);

                return View("ImportaDiarioPR", bd.OrdemProducao.Where(w => w.DataProducao >= dataIni && w.DataProducao <= dataFim).ToList());
            }
            catch (Exception e)
            {
                if (e.InnerException == null)
                    ViewBag.erro = "Erro ao executar importações / exclusões: " + e.Message;
                else
                    ViewBag.erro = "Erro ao executar importações / exclusões: " + e.Message
                        + " / Erro Interno: " + e.InnerException.Message;
                return View("ImportaDiarioPR", bd.OrdemProducao.ToList());
            }
        }

        #endregion

        #endregion

        #region Outros Métodos

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

        #region Métodos do Excel

        public static DateTime FromExcelSerialDate(int SerialDate)
        {
            if (SerialDate > 59) SerialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(SerialDate);
        }

        public static String FromExcelTextBollean(Cell theCell, WorkbookPart wbPart)
        {
            string value = value = theCell.InnerText;

            // If the cell represents an integer number, you are done. 
            // For dates, this code returns the serialized value that 
            // represents the date. The code handles strings and 
            // Booleans individually. For shared strings, the code 
            // looks up the corresponding value in the shared string 
            // table. For Booleans, the code converts the value into 
            // the words TRUE or FALSE.
            if (theCell.DataType != null)
            {
                switch (theCell.DataType.Value)
                {
                    case CellValues.SharedString:

                        // For shared strings, look up the value in the
                        // shared strings table.
                        var stringTable =
                            wbPart.GetPartsOfType<SharedStringTablePart>()
                            .FirstOrDefault();

                        // If the shared string table is missing, something 
                        // is wrong. Return the index that is in
                        // the cell. Otherwise, look up the correct text in 
                        // the table.
                        if (stringTable != null)
                        {
                            value =
                                stringTable.SharedStringTable
                                .ElementAt(int.Parse(value)).InnerText;
                        }
                        break;

                    case CellValues.Boolean:
                        switch (value)
                        {
                            case "0":
                                value = "FALSE";
                                break;
                            default:
                                value = "TRUE";
                                break;
                        }
                        break;
                }
            }

            return value;
        }

        public static String FormataCodigoProduto(string codigo, DocumentFormat.OpenXml.UInt32Value indice)
        {
            if (codigo.Length >= 7 && indice >= 5)
                return codigo = "00" + codigo.Substring(0, 1) + "." + codigo.Substring(1, 3) + "." + codigo.Substring(4, 3);
            else
                return codigo;
        }

        public decimal RetornaQtdeProdutoAdicional(string tipoLanc, Row linhaProdutoAdicional, Row linha, WorkbookPart wbPart, string colunaLinhaCodigoProduto, string coluna)
        {
            string codigoProdutoAdicional = "";
            decimal qtde = 0;
            int existe = 0;

            existe = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).Count();
            if (existe > 0)
            {
                Cell adicionalCelula = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).First();
                codigoProdutoAdicional = FormataCodigoProduto(FromExcelTextBollean(adicionalCelula, wbPart), 8);

                existe = 0;
                existe = bdDiarioProducaoRacao.PRODUTO
                    .Where(p => p.ProdCodEstr == codigoProdutoAdicional).Count();

                if (existe > 0)
                {
                    existe = 0;
                    existe = linha.Descendants<Cell>()
                        .Where(c => c.CellReference == coluna + linha.RowIndex)
                        .Count();
                    if (existe > 0)
                    {
                        existe = 0;
                        existe = linha.Descendants<Cell>()
                                        .Where(c => c.CellReference == coluna + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count();

                        if (existe > 0)
                        {
                            qtde = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Descendants<Cell>()
                                            .Where(c => c.CellReference == coluna + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);
                        }
                    }
                }
            }

            return qtde;
        }

        #endregion

        #region Métodos Apolo

        public string GeraRequisicaoMaterialMaterialAdicional(string tipoLanc, string codigoProduto, DateTime dataProducao, decimal qtde, string ordemProducao)
        {
            REQ_MAT requisicao = new REQ_MAT();

            requisicao.EmpCod = "3";

            var oMyInt = new ObjectParameter("codigo", typeof(int));
            bdDiarioProducaoRacao.GerarCodigo("3", "REQ_MAT", oMyInt);
            int codigo = Convert.ToInt32(oMyInt.Value);

            int qtdCaracteres = 10 - codigo.ToString().Length;
            string codigoCompleto = new String('0', qtdCaracteres) + codigo.ToString();

            requisicao.ReqMatNum = codigoCompleto;
            requisicao.ReqMatData = dataProducao;

            MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO produto = bdDiarioProducaoRacao.PRODUTO
                .Where(p => p.ProdCodEstr == codigoProduto)
                .First();

            int tamanho = produto.ProdNome.Length > 21 ? 21 : produto.ProdNome.Length;

            requisicao.ReqMatDescr = "ADIC. " + String.Format("{0:dd/MM/yyyy}", dataProducao) + " - " + produto.ProdNome.Substring(0, tamanho);
            requisicao.CCtrlCodEstr = "1.01.0001";
            requisicao.FuncCod = "0000052";
            requisicao.ReqMatEspecDoc = "RM";
            requisicao.ReqMatSerieDoc = "0";
            requisicao.ReqMatNumDoc = ordemProducao;
            requisicao.ReqMatStat = "Aberta";
            requisicao.ReqMatBaixouEstq = "Não";
            requisicao.TipoLancCod = tipoLanc;
            requisicao.UsuCod = "MNOTTI";
            requisicao.ReqMatOper = "Retirada";
            requisicao.ReqMatNumDocEmit = "";
            requisicao.ReqMatTipoAtend = "Automático";
            requisicao.ReqMatAlarmeValidade = "Não";
            requisicao.ReqMatEmpen = "Não";
            requisicao.ReqMatControle = "Nenhum";
            requisicao.ReqMatValLimite = 0;
            requisicao.ReqMatSucata = "Não";
            requisicao.ReqMatGerouPed = "Nenhum";
            requisicao.ReqMatTipoConsumo = "Normal";
            requisicao.ReqMatAlarmePrevRetirada = "Não";
            requisicao.ReqMatAlarmePrevSeparac = "Não";
            requisicao.ReqMatOrig = "Manual";
            requisicao.REQMATCONFERESEPARAC = "OK";

            bdDiarioProducaoRacao.REQ_MAT.AddObject(requisicao);

            bdDiarioProducaoRacao.SaveChanges();

            ITEM_REQ_MAT itemReqMat = new ITEM_REQ_MAT();

            itemReqMat.EmpCod = requisicao.EmpCod;
            itemReqMat.ReqMatNum = requisicao.ReqMatNum;
            itemReqMat.ItReqMatSeq = 1;
            itemReqMat.ProdCodEstr = codigoProduto;

            /*
                * Ocorrência 3 - 14 - MNOTTI
                * 
                * Erro ao incluir quando o produto não tem a Unidade de Medida KG.
                * Foi definido que quando não existir, será exibida uma mensagem avisando esse problema.
                * 
                * 
                filho.FicTecProdUnidMedCodDig = "KG";
                filho.FicTecProdUnidMedPosDig = 1;
            */

            int existe = 0;
            existe = bdDiarioProducaoRacao.PROD_UNID_MED
                .Where(u => u.ProdCodEstr == codigoProduto && u.ProdUnidMedCod == "KG")
                .Count();

            if (existe > 0)
            {
                PROD_UNID_MED prodUnidMed = bdDiarioProducaoRacao.PROD_UNID_MED
                    .Where(u => u.ProdCodEstr == codigoProduto && u.ProdUnidMedCod == "KG")
                    .First();

                itemReqMat.ItReqMatUnidMedCod = prodUnidMed.ProdUnidMedCod;
                itemReqMat.ItReqMatUnidMedPos = prodUnidMed.ProdUnidMedPos;
            }
            else
            {
                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao realizar a importação: O produto " + codigoProduto
                    + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                    + " novamente!";
                return "";
            }

            /****/

            ESTQ_LOC_ARMAZ estqLocArmaz = bdDiarioProducaoRacao.ESTQ_LOC_ARMAZ
                .Where(e => e.ProdCodEstr == codigoProduto && e.EmpCod == "3")
                .OrderByDescending(e => e.EstqLocArmazQtd)
                .First();

            itemReqMat.LocArmazCodEstr = estqLocArmaz.LocArmazCodEstr;
            itemReqMat.ItReqMatQtd = qtde;
            itemReqMat.ItReqMatQtdAtend = 0;
            itemReqMat.ItReqMatSaldoQtd = qtde;
            itemReqMat.ItReqMatGeraPend = "Não";
            itemReqMat.ItReqMatGeraEmpen = "Não";
            itemReqMat.ItReqMatQtdEmpen = 0;
            itemReqMat.ItReqMatQtdComprar = 0;
            itemReqMat.ItReqMatQtdComprarEmpen = "Não";
            itemReqMat.ItReqMatBxaEstqQtdAtend = "Sim";
            itemReqMat.ItReqMatCanc = "Não";
            itemReqMat.ItReqMatQtdOrig = qtde;
            itemReqMat.ItReqMatQtdAtendMaior = 0;
            itemReqMat.ItReqMatQtdCalc = qtde;
            itemReqMat.ItReqMatProcSeqIt = 1;
            itemReqMat.ItReqMatQtdEntregaParc = 0;
            itemReqMat.ItReqMatQtdAtendCalc = 0;
            itemReqMat.ItReqMatQtdAtendSim = 0;
            itemReqMat.ItReqMatEstornado = "Não";
            itemReqMat.ItReqMatValLimite = 0;
            itemReqMat.ItReqMatTerc = "Não";
            itemReqMat.ItReqMatQtdSeparada = 0;
            itemReqMat.ItReqMatQtdSeparadaCalc = 0;
            itemReqMat.ItReqMatQtdDevol = 0;
            itemReqMat.ItReqMatQtdPerda = 0;

            bdDiarioProducaoRacao.ITEM_REQ_MAT.AddObject(itemReqMat);

            bdDiarioProducaoRacao.SaveChanges();

            REQ_MAT_CLASSE_REC_DESP classeRecDesp = new REQ_MAT_CLASSE_REC_DESP();

            classeRecDesp.EmpCod = requisicao.EmpCod;
            classeRecDesp.ReqMatNum = requisicao.ReqMatNum;
            classeRecDesp.ClasseRecDespCodEstr = "3.435";
            classeRecDesp.ReqMatClasseRecDespPerc = 100;

            bdDiarioProducaoRacao.REQ_MAT_CLASSE_REC_DESP.AddObject(classeRecDesp);

            bdDiarioProducaoRacao.SaveChanges();

            RATEIO_REQ_MAT rateio = new RATEIO_REQ_MAT();

            rateio.EmpCod = requisicao.EmpCod;
            rateio.ReqMatNum = requisicao.ReqMatNum;
            rateio.ClasseRecDespCodEstr = classeRecDesp.ClasseRecDespCodEstr;
            rateio.CCtrlCodEstr = "1.07.0001";
            rateio.RatReqMatPerc = 100;

            bdDiarioProducaoRacao.RATEIO_REQ_MAT.AddObject(rateio);

            bdDiarioProducaoRacao.SaveChanges();

            bdDiarioProducaoRacao.CommandTimeout = 180;

            bdDiarioProducaoRacao.ReqMaterialAtendeAutomatico(requisicao.EmpCod, requisicao.ReqMatNum, "MNOTTI", "Retirada", "SimST");

            bdDiarioProducaoRacao.SaveChanges();

            return requisicao.ReqMatNum;
        }

        public decimal InsereProdutoAdicional(string tipoLanc, Row linhaProdutoAdicional, Row linha, WorkbookPart wbPart, string colunaLinhaCodigoProduto, string coluna, string ordem, DateTime data)
        {
            string codigoProdutoAdicional = "";
            decimal qtde = 0;
            int existe = 0;
            string numRequisicao = "";

            existe = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).Count();
            if (existe > 0)
            {
                Cell adicionalCelula = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).First();
                codigoProdutoAdicional = FormataCodigoProduto(FromExcelTextBollean(adicionalCelula, wbPart), 8);

                existe = 0;
                existe = bdDiarioProducaoRacao.PRODUTO
                    .Where(p => p.ProdCodEstr == codigoProdutoAdicional).Count();

                if (existe > 0)
                {
                    existe = 0;
                    existe = linha.Descendants<Cell>()
                        .Where(c => c.CellReference == coluna + linha.RowIndex)
                        .Count();
                    if (existe > 0)
                    {
                        existe = 0;
                        existe = linha.Descendants<Cell>()
                                        .Where(c => c.CellReference == coluna + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count();

                        if (existe > 0)
                        {
                            qtde = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Descendants<Cell>()
                                            .Where(c => c.CellReference == coluna + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);

                            numRequisicao = GeraRequisicaoMaterialMaterialAdicional(tipoLanc, codigoProdutoAdicional, data, qtde, ordem);
                        }
                    }
                }
            }

            existe = 0;
            existe = bdDiarioProducaoRacao.REQ_MAT
                .Where(r => r.EmpCod == "3" && r.ReqMatNum == numRequisicao && r.ReqMatStat != "Atendida Total")
                .Count();

            if (existe > 0)
            {
                qtde = -1;
            }

            return qtde;
        }

        public string DescricaoProduto(Row linhaProdutoAdicional, string colunaLinhaCodigoProduto, WorkbookPart wbPart)
        {
            string codigoProdutoAdicional = "";
            int existe = 0;
            string retorno = "";

            existe = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).Count();
            if (existe > 0)
            {
                Cell adicionalCelula = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).First();
                codigoProdutoAdicional = FormataCodigoProduto(FromExcelTextBollean(adicionalCelula, wbPart), 8);

                existe = 0;
                existe = bdDiarioProducaoRacao.PRODUTO
                    .Where(p => p.ProdCodEstr == codigoProdutoAdicional).Count();

                if (existe > 0)
                {
                    MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO produto = bdDiarioProducaoRacao.PRODUTO
                        .Where(p => p.ProdCodEstr == codigoProdutoAdicional).First();

                    int tamanho = produto.ProdNome.Length > 30 ? 30 : produto.ProdNome.Length;

                    retorno = produto.ProdNome.Substring(0, tamanho);
                }
            }

            return retorno;
        }

        public static string DescricaoProdutoStatic(string codigoProduto)
        {
            int existe = 0;
            string retorno = "";

            existe = 0;
            existe = bdDiarioProducaoRacaoStatic.PRODUTO
                .Where(p => p.ProdCodEstr == codigoProduto).Count();

            if (existe > 0)
            {
                MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO produto = bdDiarioProducaoRacaoStatic.PRODUTO
                    .Where(p => p.ProdCodEstr == codigoProduto).First();

                int tamanho = produto.ProdNome.Length > 30 ? 30 : produto.ProdNome.Length;

                retorno = produto.ProdNome.Substring(0, tamanho);
            }

            return retorno;
        }

        public static string VerificaBaixaRequisicaoAdicional(string numLote, string codProdutoAdicional)
        {
            string retorno = "";

            ORD_PRODUC ordemProducao = bdDiarioProducaoRacaoStatic.ORD_PRODUC
                .Where(w => w.OrdProducOrigem == numLote).FirstOrDefault();

            if (ordemProducao != null)
            {
                REQ_MAT requisicao = bdDiarioProducaoRacaoStatic.REQ_MAT
                    .Where(r => r.EmpCod == "3" 
                        && r.ReqMatEspecDoc == "RM"
                        && r.ReqMatSerieDoc == "0"
                        && r.ReqMatNumDoc == ordemProducao.OrdProducNum 
                        //&& r.ReqMatStat == "Atendida Total"
                        && bdDiarioProducaoRacaoStatic.ITEM_REQ_MAT
                            .Any(i => i.EmpCod == r.EmpCod && i.ReqMatNum == r.ReqMatNum
                                && i.ProdCodEstr == codProdutoAdicional))
                    .FirstOrDefault();

                if (requisicao != null)
                    retorno = requisicao.ReqMatNum + " - " + requisicao.ReqMatStat;
            }

            return retorno;
        }

        public string CodigoProduto(Row linhaProdutoAdicional, string colunaLinhaCodigoProduto, WorkbookPart wbPart)
        {
            string codigoProdutoAdicional = "";
            int existe = 0;
            string retorno = "";

            existe = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).Count();
            if (existe > 0)
            {
                Cell adicionalCelula = linhaProdutoAdicional.Elements<Cell>().Where(c => c.CellReference == colunaLinhaCodigoProduto).First();
                codigoProdutoAdicional = FormataCodigoProduto(FromExcelTextBollean(adicionalCelula, wbPart), 8);

                existe = 0;
                existe = bdDiarioProducaoRacao.PRODUTO
                    .Where(p => p.ProdCodEstr == codigoProdutoAdicional).Count();

                if (existe > 0)
                {
                    retorno = codigoProdutoAdicional;
                }
            }

            return retorno;
        }

        public string VerificaOrdemProducaoBaixada(string numeroOrdem)
        {
            int existe = bdDiarioProducaoRacao.REQ_MAT
                .Where(r => r.EmpCod == "3" && r.ReqMatNumDoc == numeroOrdem && r.ReqMatEspecDoc == "AOP" && r.ReqMatSerieDoc == "99")
                .Count();

            string retorno = "";

            if (existe > 0)
            {
                existe = 0;
                REQ_MAT requisicao = bdDiarioProducaoRacao.REQ_MAT
                    .Where(r => r.EmpCod == "3" && r.ReqMatNumDoc == numeroOrdem && r.ReqMatEspecDoc == "AOP" && r.ReqMatSerieDoc == "99")
                    .First();

                if (requisicao.ReqMatStat == "Atendida Total")
                {
                    retorno = "Sim";
                }
                else
                {
                    retorno = "aviso";
                }
            }
            else
            {
                retorno = "Nao";
            }

            return retorno;
        }

        public decimal InsereProdutoAdicional(string tipoLanc, string codigoProdutoAdicional, decimal? qtde, string ordem, DateTime data)
        {
            int existe = 0;
            string numRequisicao = "";
            decimal qtd = 0;
            if (qtde != null) qtd = Convert.ToDecimal(qtde);

            if (qtde > 0)
            {
                numRequisicao = GeraRequisicaoMaterialMaterialAdicional(tipoLanc, codigoProdutoAdicional, data, qtd, ordem);

                existe = 0;
                existe = bdDiarioProducaoRacao.REQ_MAT
                    .Where(r => r.EmpCod == "3" && r.ReqMatNum == numRequisicao && r.ReqMatStat != "Atendida Total")
                    .Count();

                if (existe > 0)
                {
                    qtde = -1;
                }
            }

            return qtd;
        }

        public string ImportaLinhaDPRApolo(int idLinha)
        {
            string retorno = "";
            linhaErro = idLinha;

            bdDiarioProducaoRacao.CommandTimeout = 100000;
            
            try
            {
                #region Carrega Linha

                LayoutOrdemProducao linha = bd.OrdemProducao.Where(w => w.ID == idLinha).FirstOrDefault();

                #endregion

                #region Verifica se existe a fórmula no cadastro de Produtos do Apolo

                int existe = 0;
                existe = bdDiarioProducaoRacao.PRODUTO1
                        .Where(p => p.USERNumFormula == linha.CodFormula)
                        .Count();

                #endregion

                if (existe > 0)
                {
                    #region Verifica se existe Ficha Técnica para a Fórmula

                    PRODUTO1 codigoProdutoPai1 = bdDiarioProducaoRacao.PRODUTO1
                        .Where(p => p.USERNumFormula == linha.CodFormula)
                        .First();

                    MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao.PRODUTO codigoProdutoPai = bdDiarioProducaoRacao.PRODUTO
                        .Where(p => p.ProdCodEstr == codigoProdutoPai1.ProdCodEstr)
                        .First();

                    existe = 0;
                    existe = bdDiarioProducaoRacao.FIC_TEC_PROD
                        .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr).Count();

                    #endregion

                    // Caso ele exista, realiza as outras operações
                    if (existe > 0)
                    {
                        string importado = "Nao";

                        #region Gera Código do Plano de Produção

                        var oMyInt = new ObjectParameter("codigo", typeof(int));
                        bdDiarioProducaoRacao.GerarCodigo("3", "PLAN_PRODUC", oMyInt);
                        int codigo = Convert.ToInt32(oMyInt.Value);

                        int qtdCaracteres = 7 - codigo.ToString().Length;
                        string codigoCompleto = new String('0', qtdCaracteres) + codigo.ToString();

                        #endregion

                        #region Insere Plano de Produção

                        PLAN_PRODUC planoProducao = new PLAN_PRODUC();

                        planoProducao.EmpCod = "3";
                        planoProducao.PlanProducNum = codigoCompleto;

                        planoProducao.PlanProducData = linha.DataProducao;

                        planoProducao.PlanProducNome = "DATA PROD.RAÇÃO " + String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " - Fórmula " 
                            + linha.CodFormula.ToString();
                        planoProducao.PlanProducDataInic = planoProducao.PlanProducData;
                        planoProducao.PlanProducDataFim = planoProducao.PlanProducData;
                        planoProducao.PlanProducCompEstq = "Nenhum";
                        planoProducao.PlanProducConsidLoteEcon = "Não";
                        planoProducao.PlanProducConsidEstq = "Não";
                        planoProducao.PlanProducDesativado = "Não";

                        bdDiarioProducaoRacao.PLAN_PRODUC.AddObject(planoProducao);

                        #endregion

                        #region Insere Necessidade de Planejamento de Produção

                        NEC_PLAN_PRODUC necessidadePlanejamentoProducao = new NEC_PLAN_PRODUC();

                        necessidadePlanejamentoProducao.EmpCod = "3";
                        necessidadePlanejamentoProducao.PlanProducNum = planoProducao.PlanProducNum;
                        necessidadePlanejamentoProducao.NecPlanProducDataEmis = planoProducao.PlanProducData;
                        necessidadePlanejamentoProducao.NecPlanProducDataInic = planoProducao.PlanProducData;
                        necessidadePlanejamentoProducao.NecPlanProducDataFim = planoProducao.PlanProducData;
                        necessidadePlanejamentoProducao.NecPlanProducGerouOp = "Sim";
                        necessidadePlanejamentoProducao.NecPlanProducReqMat = "Não";
                        necessidadePlanejamentoProducao.NecPlanProducReqComp = "Não";
                        necessidadePlanejamentoProducao.NecPlanProducVerEstqMat = "Sim";
                        necessidadePlanejamentoProducao.NecPlanProducVerEstqAcab = "Não";
                        necessidadePlanejamentoProducao.NecPlanProducVerEstqSemiAcab = "Não";
                        necessidadePlanejamentoProducao.NecPlanProducVerEstqAlt = "Não";
                        necessidadePlanejamentoProducao.NecPlanProducVerPedComp = "Não";
                        necessidadePlanejamentoProducao.NecPlanProducDesmSemiAcab = "Não";
                        necessidadePlanejamentoProducao.NecPlanProducDesativada = "Não";

                        bdDiarioProducaoRacao.NEC_PLAN_PRODUC.AddObject(necessidadePlanejamentoProducao);

                        #endregion

                        #region Localiza Produto Filho na Ficha Técnica

                        FIC_TEC_PROD codigoProdutoPaiFicha = bdDiarioProducaoRacao.FIC_TEC_PROD
                            .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr).First();

                        #endregion

                        #region Item do Planejamento da Produção

                        ITEM_PLAN_PRODUC itemPlanoProducao = new ITEM_PLAN_PRODUC();

                        itemPlanoProducao.EmpCod = "3";
                        itemPlanoProducao.PlanProducNum = planoProducao.PlanProducNum;
                        itemPlanoProducao.ProdCodEstr = codigoProdutoPaiFicha.ProdCodEstr;

                        itemPlanoProducao.ItPlanProducSeq = 1;

                        #region Verifica Unidade de Medida

                        /*
                            * Ocorrência 3 - 14 - MNOTTI
                            * 
                            * Erro ao incluir quando o produto não tem a Unidade de Medida KG.
                            * Foi definido que quando não existir, será exibida uma mensagem avisando esse problema.
                            * 
                            * 
                            filho.FicTecProdUnidMedCodDig = "KG";
                            filho.FicTecProdUnidMedPosDig = 1;
                            */

                        existe = 0;
                        existe = bdDiarioProducaoRacao.PROD_UNID_MED
                            .Where(u => u.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr && u.ProdUnidMedCod == "KG")
                            .Count();

                        if (existe > 0)
                        {
                            PROD_UNID_MED prodUnidMed = bdDiarioProducaoRacao.PROD_UNID_MED
                                .Where(u => u.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                .First();

                            itemPlanoProducao.ItPlanProducUnidMedCod = prodUnidMed.ProdUnidMedCod;
                            itemPlanoProducao.ItPlanProducUnidMedPos = prodUnidMed.ProdUnidMedPos;
                        }
                        else
                        {
                            retorno = "Erro ao realizar a importação: O produto " + codigoProdutoPaiFicha.ProdCodEstr
                                + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                + " novamente!";
                            return retorno;
                        }

                        /****/

                        #endregion

                        itemPlanoProducao.ItPlanProducQtd = linha.TotalProduzido;
                                
                        #region Localiza saldo para informar no item

                        existe = 0;
                        existe = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                            .Where(s => s.EmpCod == "3" &&
                                s.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr)
                            .OrderByDescending(s => s.SaldoEstqData)
                            .Count();

                        decimal? saldoQtd = 0;

                        if (existe > 0)
                        {
                            SALDO_ESTQ_DATA saldo = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                .Where(s => s.EmpCod == "3" &&
                                    s.ProdCodEstr == codigoProdutoPaiFicha.ProdCodEstr)
                                .OrderByDescending(s => s.SaldoEstqData)
                                .First();

                            saldoQtd = saldo.SaldoEstqDataQtd;
                        }

                        #endregion

                        itemPlanoProducao.ItPlanProducQtdEstq = saldoQtd;
                        itemPlanoProducao.ItPlanProducQtdReserv = 0;
                        itemPlanoProducao.ItPlanProducQtdEmp = 0;
                        itemPlanoProducao.ItPlanProducQtdNec = itemPlanoProducao.ItPlanProducQtd;
                        itemPlanoProducao.ItPlanProducQtdDisp = itemPlanoProducao.ItPlanProducQtdEstq;
                        itemPlanoProducao.ItPlanProducQtdComp = 0;
                        itemPlanoProducao.ItPlanProducQtdNecPeso = 0;
                        itemPlanoProducao.ItPlanProducQtdNecPesoTot = 0;
                        itemPlanoProducao.ItPlanProducCapHrMaq = 0;
                        itemPlanoProducao.ItPlanProducConsidEstq = "Padrão";

                        bdDiarioProducaoRacao.ITEM_PLAN_PRODUC.AddObject(itemPlanoProducao);

                        #endregion

                        #region Item da Necessida do Planejamento da Produção

                        ITEM_NEC_PLAN_PRODUC itemNecessidadePlanejamentoProducao = new ITEM_NEC_PLAN_PRODUC();

                        itemNecessidadePlanejamentoProducao.EmpCod = "3";
                        itemNecessidadePlanejamentoProducao.PlanProducNum = itemPlanoProducao.PlanProducNum;
                        itemNecessidadePlanejamentoProducao.ProdCodEstr = itemPlanoProducao.ProdCodEstr;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducSeq = itemPlanoProducao.ItPlanProducSeq;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducUnidMedCod = itemPlanoProducao.ItPlanProducUnidMedCod;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducUnidMedPos = itemPlanoProducao.ItPlanProducUnidMedPos;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdOrig = itemPlanoProducao.ItPlanProducQtd;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdReal = 0;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdEstq = itemPlanoProducao.ItPlanProducQtdEstq;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdReserv = 0;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdEmp = 0;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNec = itemPlanoProducao.ItPlanProducQtdNec;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdDisp = itemPlanoProducao.ItPlanProducQtdDisp;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdDesm = 0;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducUtiliz = "Próprio";
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdComp = itemPlanoProducao.ItPlanProducQtdComp;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNecPeso = itemPlanoProducao.ItPlanProducQtdNecPeso;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducIndRetalho = 0;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducQtdNecPesoTot = itemPlanoProducao.ItPlanProducQtdNecPesoTot;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducCapHrMaq = itemPlanoProducao.ItPlanProducCapHrMaq;
                        itemNecessidadePlanejamentoProducao.ItNecPlanProducSeqLeit = itemNecessidadePlanejamentoProducao.ItNecPlanProducSeq;

                        bdDiarioProducaoRacao.ITEM_NEC_PLAN_PRODUC.AddObject(itemNecessidadePlanejamentoProducao);

                        #endregion

                        #region Localiza e insere os itens da Ficha Técnica

                        var listaFichaTecnicaFilhos = bdDiarioProducaoRacao.FIC_TEC_PROD
                                                        .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr
                                                            && f.FicTecProdDataInic >= codigoProdutoPai.ProdDataValidInic
                                                            && (f.FicTecProdDataFim <= codigoProdutoPai.ProdDataValidInic || f.FicTecProdDataFim == null))
                                                        .ToList();

                        foreach (var itemFichaTecnicaFilhos in listaFichaTecnicaFilhos)
                        {
                            #region Insere Item da Ficha Técnica no Plano de Produção

                            PLAN_PRODUC_FIC_TEC planejamentoProducaoFichaTecnica = new PLAN_PRODUC_FIC_TEC();

                            planejamentoProducaoFichaTecnica.EmpCod = "3";
                            planejamentoProducaoFichaTecnica.PlanProducNum = planoProducao.PlanProducNum;
                            planejamentoProducaoFichaTecnica.ProdCodEstr = itemPlanoProducao.ProdCodEstr;
                            planejamentoProducaoFichaTecnica.ItPlanProducSeq = itemPlanoProducao.ItPlanProducSeq;
                            planejamentoProducaoFichaTecnica.FTProdCodEstr = itemPlanoProducao.ProdCodEstr;
                            planejamentoProducaoFichaTecnica.FicTecProdSeq = itemFichaTecnicaFilhos.FicTecProdSeq;
                            planejamentoProducaoFichaTecnica.PlanProducFicTecProdCodEstr = itemFichaTecnicaFilhos.FicTecProdCodEstr;
                            planejamentoProducaoFichaTecnica.PlanProducFicTecQtd = itemFichaTecnicaFilhos.FicTecProdQtd * itemPlanoProducao.ItPlanProducQtd;

                            #region Localiza Saldo do item para inserir no item do Plano de Produção

                            existe = 0;
                            existe = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                .Where(s => s.EmpCod == "3" &&
                                    s.ProdCodEstr == itemFichaTecnicaFilhos.FicTecProdCodEstr)
                                .OrderByDescending(s => s.SaldoEstqData)
                                .Count();

                            decimal? saldoFilhoQtd = 0;

                            if (existe > 0)
                            {
                                SALDO_ESTQ_DATA saldoFilho = bdDiarioProducaoRacao.SALDO_ESTQ_DATA
                                .Where(s => s.EmpCod == "3" &&
                                    s.ProdCodEstr == itemFichaTecnicaFilhos.FicTecProdCodEstr)
                                .OrderByDescending(s => s.SaldoEstqData)
                                .First();

                                saldoFilhoQtd = saldoFilho.SaldoEstqDataQtd;
                            }

                            #endregion

                            planejamentoProducaoFichaTecnica.PlanProducFicTecQtdEstq = saldoFilhoQtd;
                            planejamentoProducaoFichaTecnica.PlanProducFicTecQtdReserv = 0;
                            planejamentoProducaoFichaTecnica.PlanProducFicTecQtdEmp = 0;
                            planejamentoProducaoFichaTecnica.PlanProducFicTecQtdNec = planejamentoProducaoFichaTecnica.PlanProducFicTecQtd;
                            planejamentoProducaoFichaTecnica.PlanProducFicTecQtdComp = 0;

                            bdDiarioProducaoRacao.PLAN_PRODUC_FIC_TEC.AddObject(planejamentoProducaoFichaTecnica);

                            #endregion
                        }

                        bdDiarioProducaoRacao.SaveChanges();

                        #endregion

                        #region Gera Ordem de Produção
                                            
                        bdDiarioProducaoRacao.GeraOrdemProducao(planoProducao.PlanProducNum, itemPlanoProducao.ProdCodEstr,
                            itemPlanoProducao.ItPlanProducSeq, null, "3", planoProducao.PlanProducData, "RIOSOFT");

                        bdDiarioProducaoRacao.SaveChanges();

                        ORD_PRODUC ordProducNum = bdDiarioProducaoRacao.ORD_PRODUC
                            .Where(o => o.EmpCod == "3" && o.PlanProducNum == planoProducao.PlanProducNum)
                            .First();

                        #region Verifica Local de Armazenagem da Planilha com o Apolo

                        LOC_ARMAZ local = bdDiarioProducaoRacao.LOC_ARMAZ
                            .Where(l => l.LocArmazNome.Contains(linha.NucleoGalpao)).FirstOrDefault();

                        if (local == null)
                        {
                            retorno = "Local " + linha.NucleoGalpao + " não configurado nos "
                                + "locais de armazenagem do APOLO! Verifique a descrição "
                                + "do local de armazenagem!";
                            return retorno;
                        }

                        #endregion

                        ordProducNum.LocArmazCodEstr = local.LocArmazCodEstr;
                        ordProducNum.OrdProducOrigem = linha.OrdemProducao;

                        #endregion

                        #region Insere Operações da Ordem de Produção

                        OPER_ORD_PRODUC operOrdProduc = new OPER_ORD_PRODUC();

                        operOrdProduc.EmpCod = "3";
                        operOrdProduc.OrdProducNum = ordProducNum.OrdProducNum;
                        operOrdProduc.ProdCodEstr = ordProducNum.ProdCodEstr;
                        operOrdProduc.ProdOperSeq = 10;

                        OPER_ORD_PRODUC ultimoOperOrdProduc = bdDiarioProducaoRacao.OPER_ORD_PRODUC
                            .Where(o => o.EmpCod == "3").OrderByDescending(o => o.OperOrdProducSeq)
                            .First();

                        operOrdProduc.OperOrdProducSeq = ultimoOperOrdProduc.OperOrdProducSeq + 1;
                        operOrdProduc.CCtrlCodEstr = "1.07.0001";
                        operOrdProduc.OperOrdProducStat = "Manual";
                        if (linha.HoraInicioProducao > 0)
                        {
                            double d = Convert.ToDouble(linha.HoraInicioProducao);
                            DateTime dt = DateTime.FromOADate(d);

                            string dataHora = String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " " +
                                String.Format("{0:hh:mm}", dt);
                            operOrdProduc.OperOrdProducDataHoraInic = Convert.ToDateTime(dataHora);
                        }
                        else
                            operOrdProduc.OperOrdProducDataHoraInic = planoProducao.PlanProducData;

                        if (linha.HoraTerminoProducao > 0)
                        {
                            double d = Convert.ToDouble(linha.HoraTerminoProducao);
                            DateTime dt = DateTime.FromOADate(d);

                            string dataHora = String.Format("{0:dd/MM/yyyy}", planoProducao.PlanProducData) + " " +
                                String.Format("{0:hh:mm}", dt);
                            operOrdProduc.OperOrdProducDataHoraFim = Convert.ToDateTime(dataHora);
                        }
                        else
                            operOrdProduc.OperOrdProducDataHoraFim = planoProducao.PlanProducData;

                        operOrdProduc.OperOrdProducQtdBoa = itemPlanoProducao.ItPlanProducQtd;
                        operOrdProduc.OperOrdProducQtdRefug = 0;
                        operOrdProduc.OperOrdProducQtdReproc = 0;
                        operOrdProduc.OperCod = "0000001";
                        operOrdProduc.UsuCod = "MNOTTI";

                        DateTime dataInicial = Convert.ToDateTime(operOrdProduc.OperOrdProducDataHoraInic);
                        DateTime dataFinal = Convert.ToDateTime(operOrdProduc.OperOrdProducDataHoraFim);

                        decimal tempoCent = ((dataInicial - dataFinal).Minutes / 60);

                        operOrdProduc.OperOrdProducTempoCent = tempoCent;
                        operOrdProduc.OperOrdProducApont = "Operação";
                        //operOrdProduc.OperOrdProducApont = "Preparação";
                        operOrdProduc.OperOrdProducGerReqMat = "Sim";
                        operOrdProduc.OperOrdProducPesoUnitProd = 0;
                        operOrdProduc.OperOrdProducQtdRetalho = 0;
                        operOrdProduc.AtivGrpCodEstr = "01.01";
                        operOrdProduc.OperOrdProducGeraLoteAutom = "Configuração";

                        #region Verifica Unidade de Medida do item

                        /*
                            * Ocorrência 3 - 14 - MNOTTI
                            * 
                            * Erro ao incluir quando o produto não tem a Unidade de Medida KG.
                            * Foi definido que quando não existir, será exibida uma mensagem avisando esse problema.
                            * 
                            * 
                            filho.FicTecProdUnidMedCodDig = "KG";
                            filho.FicTecProdUnidMedPosDig = 1;
                            */

                        existe = 0;
                        existe = bdDiarioProducaoRacao.PROD_UNID_MED
                            .Where(u => u.ProdCodEstr == operOrdProduc.ProdCodEstr && u.ProdUnidMedCod == "KG")
                            .Count();

                        if (existe > 0)
                        {
                            PROD_UNID_MED prodUnidMed = bdDiarioProducaoRacao.PROD_UNID_MED
                                .Where(u => u.ProdCodEstr == operOrdProduc.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                .First();

                            operOrdProduc.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                            operOrdProduc.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                        }
                        else
                        {
                            retorno = "Erro ao realizar a importação: O produto " + operOrdProduc.ProdCodEstr
                                + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                + " novamente!";
                            return retorno;
                        }

                        /****/

                        #endregion

                        operOrdProduc.OperOrdProducCompBruto = 0;
                        operOrdProduc.OperOrdProducCompLiq = 0;
                        operOrdProduc.OperOrdProducAltBruta = 0;
                        operOrdProduc.OperOrdProducAltLiq = 0;
                        operOrdProduc.OperOrdProducLargBruta = 0;
                        operOrdProduc.OperOrdProducLargLiq = 0;
                        operOrdProduc.OperOrdProducEspacoBruto = 0;
                        operOrdProduc.OperOrdProducEspacoLiq = 0;
                        operOrdProduc.OperOrdProducDataHoraApont = DateTime.Now;
                        operOrdProduc.OperOrdProducTara = 0;
                        operOrdProduc.OperOrdProducPesoBruto = 0;
                        operOrdProduc.OperOrdProducTipo = "Produção";
                        operOrdProduc.OperOrdProducQtdReal = 0;
                        operOrdProduc.TipoLancCod = codigoProdutoPai1.USERTipoLancEntradaProd;
                        operOrdProduc.OperOrdProducIntegraEstq = "Sim";
                        operOrdProduc.OperOrdProducIntegradoEstq = "Não";
                        operOrdProduc.OperOrdProducUnidMedPeso = 1;
                        operOrdProduc.OperOrdProducQtdCalc = operOrdProduc.OperOrdProducQtdBoa;

                        bdDiarioProducaoRacao.OPER_ORD_PRODUC.AddObject(operOrdProduc);

                        #endregion

                        #region Inserção dos Funcionários das Operação da Produção

                        OPER_ORD_PRODUC_FUNC operOrdProducFunc = new OPER_ORD_PRODUC_FUNC();

                        operOrdProducFunc.EmpCod = "3";
                        operOrdProducFunc.OrdProducNum = operOrdProduc.OrdProducNum;
                        operOrdProducFunc.ProdCodEstr = operOrdProduc.ProdCodEstr;
                        operOrdProducFunc.ProdOperSeq = operOrdProduc.ProdOperSeq;
                        operOrdProducFunc.OperOrdProducSeq = operOrdProduc.OperOrdProducSeq;

                        if (linha.Responsavel == "Paulo Sérgio")
                        {
                            operOrdProducFunc.FuncCod = "0000008";
                        }
                        else if (linha.Responsavel == "Leandro")
                        {
                            operOrdProducFunc.FuncCod = "0000011";
                        }
                        else if (linha.Responsavel == "Osmanir")
                        {
                            operOrdProducFunc.FuncCod = "0000110";
                        }
                        else if (linha.Responsavel == "Caio Santiago")
                        {
                            operOrdProducFunc.FuncCod = "0000118";
                        }
                        else if (linha.Responsavel == "Pedro Fuentes")
                        {
                            operOrdProducFunc.FuncCod = "0000286";
                        }
                        else
                        {
                            operOrdProducFunc.FuncCod = linha.Responsavel;
                        }
                        operOrdProducFunc.OperOrdProducFuncApont = "Sim";

                        bdDiarioProducaoRacao.OPER_ORD_PRODUC_FUNC.AddObject(operOrdProducFunc);

                        bdDiarioProducaoRacao.SaveChanges();

                        #endregion

                        #region Baixa a Ordem de Produção

                        bdDiarioProducaoRacao.InsertOperOrdProducProc("3", ordProducNum.OrdProducNum, operOrdProduc.OperOrdProducSeq, operOrdProduc.ProdCodEstr);
                        bdDiarioProducaoRacao.SaveChanges();

                        #endregion

                        #region Insere os Materiais Adicionais

                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional01, linha.Qtde01, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional02, linha.Qtde02, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional03, linha.Qtde03, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional04, linha.Qtde04, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional05, linha.Qtde05, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional06, linha.Qtde06, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional07, linha.Qtde07, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional08, linha.Qtde08, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional09, linha.Qtde09, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional10, linha.Qtde10, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional11, linha.Qtde11, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional12, linha.Qtde12, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional13, linha.Qtde13, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional14, linha.Qtde14, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional15, linha.Qtde15, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional16, linha.Qtde16, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional17, linha.Qtde17, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional18, linha.Qtde18, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional19, linha.Qtde19, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional20, linha.Qtde20, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional21, linha.Qtde21, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional22, linha.Qtde22, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional23, linha.Qtde23, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional24, linha.Qtde24, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional25, linha.Qtde25, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional26, linha.Qtde26, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional27, linha.Qtde27, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional28, linha.Qtde28, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional29, linha.Qtde29, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional30, linha.Qtde30, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional31, linha.Qtde31, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional32, linha.Qtde32, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional33, linha.Qtde33, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional34, linha.Qtde34, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional35, linha.Qtde35, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional36, linha.Qtde36, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional37, linha.Qtde37, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional38, linha.Qtde38, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional39, linha.Qtde39, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional40, linha.Qtde40, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional41, linha.Qtde41, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional42, linha.Qtde42, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional43, linha.Qtde43, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional44, linha.Qtde44, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional45, linha.Qtde45, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional46, linha.Qtde46, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional47, linha.Qtde47, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional48, linha.Qtde48, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional49, linha.Qtde49, ordProducNum.OrdProducNum, linha.DataProducao);
                        InsereProdutoAdicional(codigoProdutoPai1.USERTLBaixaAdicProd, linha.Adicional50, linha.Qtde50, ordProducNum.OrdProducNum, linha.DataProducao);

                        #endregion

                        #region Verifica se A Fórmula foi baixada

                        importado = VerificaOrdemProducaoBaixada(ordProducNum.OrdProducNum);

                        linha.Importado = importado;

                        bd.SaveChanges();

                        #endregion
                    }
                }

                return retorno;
            }
            catch (Exception e)
            {
                var listaExibicaoErro = bd.OrdemProducao;

                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                if (e.InnerException == null)
                    retorno = "Erro ao realizar a importação: " + e.Message + " | linha: "
                        + linhaErro.ToString() + " | linha erro código: " + linenum.ToString();
                else
                    retorno = "Erro ao realizar a importação: " + e.Message
                        + " | Erro Interno: " + e.InnerException.Message
                        + " | linha: "
                        + linhaErro.ToString() + " | linha erro código: " + linenum.ToString();
                
                return retorno;
            }
        }

        public string DeletaLinhaBaixadaApolo(int idLinha)
        {
            string retorno = "";
            string usuario = Session["login"].ToString().ToUpper();
            
            bdDiarioProducaoRacao.CommandTimeout = 100000;

            #region Carrega dados da Linha

            LayoutOrdemProducao linha = bd.OrdemProducao.Where(w => w.ID == idLinha).FirstOrDefault();

            #endregion

            #region Navega nas colunas dos Adicionais para estornar e deletar a requisição dos mesmos

            foreach (var col in linha.GetType().GetProperties())
            {
                if (col.Name.Contains("Adicional"))
                {
                    if (col.GetValue(linha, null) != null)
                    {
                        string codigoProdutoAdicional = col.GetValue(linha, null).ToString();
                        string numRequisicao = VerificaBaixaRequisicaoAdicional(linha.OrdemProducao, codigoProdutoAdicional);
                        if (numRequisicao != "")
                        {
                            bdDiarioProducaoRacao.EstornaAtendReqMat("3", numRequisicao, numRequisicao, null, null, usuario);

                            #region Verifica se a requisição foi estornada

                            REQ_MAT verificaRequisicao = bdDiarioProducaoRacao.REQ_MAT
                                .Where(w => w.EmpCod == "3" && w.ReqMatNum == numRequisicao && w.ReqMatStat != "Aberto").FirstOrDefault();
                            if (verificaRequisicao != null)
                            {
                                return "Requisição " + numRequisicao + " não estornada! Por favor, verificar!";
                            }

                            #endregion

                            bdDiarioProducaoRacao.DeleteReqmat("3", numRequisicao);

                            #region Verifica se a requisição foi deletada

                            verificaRequisicao = bdDiarioProducaoRacao.REQ_MAT
                                .Where(w => w.EmpCod == "3" && w.ReqMatNum == numRequisicao).FirstOrDefault();
                            if (verificaRequisicao != null)
                            {
                                return "Requisição " + numRequisicao + " não deletada! Por favor, verificar!";
                            }

                            #endregion
                        }
                    }
                }
            }

            #endregion

            #region Deleta Dados do PPCP

            ORD_PRODUC ordemProducao = bdDiarioProducaoRacao.ORD_PRODUC
                .Where(w => w.OrdProducOrigem == linha.OrdemProducao).FirstOrDefault();

            if (ordemProducao != null)
            {
                OPER_ORD_PRODUC apontamento = bdDiarioProducaoRacao.OPER_ORD_PRODUC
                    .Where(w => w.EmpCod == "3" && w.OrdProducNum == ordemProducao.OrdProducNum).FirstOrDefault();

                if (apontamento != null)
                {
                    bdDiarioProducaoRacao.DeleteOperOrdProduc("3", apontamento.OrdProducNum, apontamento.ProdCodEstr, apontamento.ProdOperSeq,
                        apontamento.OperOrdProducSeq, usuario, "", usuario);
                }

                bdDiarioProducaoRacao.DeleteOrdProduc("3", ordemProducao.OrdProducNum, ordemProducao.ProdCodEstr, usuario);
                bdDiarioProducaoRacao.DeleteNecPlanProduc("3", ordemProducao.PlanProducNum, usuario);
                bdDiarioProducaoRacao.DeletePlanProduc("3", ordemProducao.PlanProducNum);
            }

            #endregion

            #region Atualiza Dados da Linha

            linha.Importado = "Nao";

            bd.SaveChanges();

            #endregion

            return retorno;
        }

        #endregion

        #region Métodos Fluig

        

        #endregion
    }
}
