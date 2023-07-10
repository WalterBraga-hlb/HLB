using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHyLinedoBrasil.Models.FormulaPPCP;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.ComponentModel;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class FormulaPPCPController : Controller
    {
        FormulaPPCPEntities bdFormulaPPCP = new FormulaPPCPEntities();

        //
        // GET: /FormulaPPCP/

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");
            return View();
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

        [HttpPost]
        public ActionResult ImportaDadosFormulaPPCP()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string caminho = @"C:\inetpub\wwwroot\Relatorios\" + Session["login"].ToString() + ".xls";

            Request.Files[0].SaveAs(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            string erroAba = "";
            string erroLinha = "";
            string erroColuna = "";

            try
            {
                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                PRODUTO1 codigoProdutoPai1 = new PRODUTO1();
                PRODUTO codigoProdutoPai = new PRODUTO();

                // Navega entre cada Planilha
                foreach (var planilha in lista)
                {
                    int result;
                    int name = 0;

                    if (int.TryParse(planilha.Name.ToString(),out result))
                        name = Convert.ToInt16(planilha.Name);

                    erroAba = planilha.Name.ToString();

                    int existe = bdFormulaPPCP.PRODUTO1
                        .Where(p => p.USERNumFormula == name)
                        .Count();

                    // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                    //if ((existe > 0) && (planilha.Name == "71"))
                    if (existe > 0)
                    {
                        // Localiza o Produto no Apolo que tem a Fórmula Cadastrada
                        codigoProdutoPai1 = bdFormulaPPCP.PRODUTO1
                            .Where(p => p.USERNumFormula == name)
                            .First();

                        codigoProdutoPai = bdFormulaPPCP.PRODUTO
                            .Where(p => p.ProdCodEstr == codigoProdutoPai1.ProdCodEstr)
                            .First();

                        string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == planilha.Name)
                                                    .First().Id;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                        .GetPartById(relationshipId);
                        
                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        //var listaLinhas = planilha.Elements<Row>().ToList();
                        var listaLinhas = sheetData.Descendants<Row>().ToList();

                        // Pega a Validade da Fórmula e altera na Data de Validade do Produto
                        Row linhaData = sheetData.Elements<Row>().Where(r => r.RowIndex == 2).First();
                        Cell celulaData = linhaData.Elements<Cell>().Where(c => c.CellReference == "C2").First();
                        codigoProdutoPai.ProdDataValidInic = FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText));

                        //DateTime teste = FromExcelSerialDate(Convert.ToInt32(testeCelula.InnerText));

                        /* TESTES */
                        //Cell testeCelulaTexto = testeLinha.Elements<Cell>().Where(c => c.CellReference == "B2").First();

                        //string testeTexto = FromExcelTextBollean(testeCelulaTexto, spreadsheetDocument.WorkbookPart);

                        // Navega nas linhas da Planilha
                        foreach (var linha in listaLinhas)
	                    {
                            erroLinha = linha.RowIndex.ToString();

                            // Recupera o Código do Produto Filho da Planilha caso exista
                            existe = 0;
                            existe = linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex)
                                .Count();

                            if (existe > 0)
                            {
                                string codigoProduto = FormataCodigoProduto(linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "B" + linha.RowIndex)
                                                    .First().InnerText, linha.RowIndex);

                                existe = 0;
                                existe = bdFormulaPPCP.PRODUTO
                                    .Where(p => p.ProdCodEstr == codigoProduto)
                                    .Count();

                                if (existe > 0)
                                {
                                    existe = 0;
                                    existe = bdFormulaPPCP.FIC_TEC_PROD
                                        .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr &&
                                            f.FicTecProdCodEstr == codigoProduto).Count();

                                    // Caso ele exista, realiza as outras operações
                                    if (existe > 0)
                                    {
                                        // Localiza Produto Filho na Ficha Técnica
                                        FIC_TEC_PROD filho = bdFormulaPPCP.FIC_TEC_PROD
                                            .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr &&
                                                f.FicTecProdCodEstr == codigoProduto).OrderByDescending(f => f.FicTecProdSeq).First();

                                        //var teste = linha.Elements<Cell>()
                                        //                    .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                        //                    .First().Descendants<CellValue>().FirstOrDefault();

                                        //double db = double.Parse(teste.Text);

                                        //var teste2 = teste.Text.Replace(".", ",");

                                        //double db2 = double.Parse(teste2);

                                        //decimal d3 = Convert.ToDecimal(db2);

                                        //var teste3 = Convert.ToDecimal(teste.Text);

                                        erroColuna = "E";

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            filho.FicTecProdQtd = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);
                                        }
                                        else
                                        {
                                            filho.FicTecProdQtd = 0;
                                        }

                                        erroColuna = "G";

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            filho.FicTecProdPerc = Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))) * 100;
                                        }
                                        else
                                        {
                                            filho.FicTecProdPerc = 0;
                                        }


                                        filho.FicTecProdQtdCalc = filho.FicTecProdQtd / ((filho.FicTecProdPerc == 0 ? 1 : filho.FicTecProdPerc) / 100);
                                        filho.FicTecProdCustoQtd = filho.FicTecProdQtdCalc;

                                        DateTime? dataAntiga = filho.FicTecProdDataInic;

                                        erroColuna = "H";

                                        filho.FicTecProdDataInic = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));

                                        erroColuna = "I";

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            filho.FicTecProdDataFim = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text));
                                        }
                                        else
                                        {
                                            filho.FicTecProdDataFim = null;
                                        }

                                        // Caso a Data Inicial do Filho seja diferente da Planilha, será inserido no Log de datas
                                        if (dataAntiga != FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text)))
                                        {
                                            FIC_TEC_PROD_DATA filhoData = new FIC_TEC_PROD_DATA();

                                            filhoData.ProdCodEstr = filho.ProdCodEstr;
                                            filhoData.FicTecProdSeq = filho.FicTecProdSeq;

                                            int sequenciaData = bdFormulaPPCP.FIC_TEC_PROD_DATA
                                                .Where(d => d.ProdCodEstr == filho.ProdCodEstr &&
                                                    d.FicTecProdSeq == filho.FicTecProdSeq)
                                                .Count() + 1;

                                            filhoData.FicTecProdDataSeq = sequenciaData;
                                            filhoData.FicTecProdDataInicio = filho.FicTecProdDataInic;
                                            filhoData.FicTecProdDataQtd = filho.FicTecProdQtd;
                                            filhoData.FicTecProdDataPerc = filho.FicTecProdPerc;
                                            filhoData.FicTecProdDataPercTipo = filho.FicTecProdPercTipo;
                                            filhoData.FicTecProdDataQtdCalc = filho.FicTecProdQtdCalc;
                                            filhoData.FicTecProdDataCustoPerc = filho.FicTecProdCustoPerc;
                                            filhoData.FicTecProdDataCustoPercTipo = filho.FicTecProdCustoPercTipo;
                                            filhoData.FicTecProdDataCustoQtd = filho.FicTecProdCustoQtd;
                                            filhoData.FicTecProdDataCompCusto = filho.FicTecProdCompCusto;
                                            filhoData.FicTecProdDataGeraOP = filho.FicTecProdGeraOP;
                                            filhoData.FicTecProdDataPercPartic = filho.FicTecProdPercPartic;
                                            filhoData.FicTecProdDataPartVenda = filho.FicTecProdPartVenda;
                                            filhoData.FicTecProdDataPartVenPcDesc = filho.FicTecProdPartVendaPercDesc;
                                            filhoData.FicTecProdDataPartVenPcAcresc = filho.FicTecProdPartVendaPercAcresc;

                                            bdFormulaPPCP.FIC_TEC_PROD_DATA.AddObject(filhoData);
                                        }
                                        else
                                        {
                                            FIC_TEC_PROD_DATA filhoData = bdFormulaPPCP.FIC_TEC_PROD_DATA
                                                .Where(d => d.ProdCodEstr == codigoProdutoPai.ProdCodEstr
                                                    && d.FicTecProdSeq == filho.FicTecProdSeq)
                                                .OrderByDescending(d => d.FicTecProdDataSeq)
                                                .First();

                                            filhoData.FicTecProdDataInicio = filho.FicTecProdDataInic;
                                            filhoData.FicTecProdDataQtd = filho.FicTecProdQtd;
                                            filhoData.FicTecProdDataPerc = filho.FicTecProdPerc;
                                            filhoData.FicTecProdDataPercTipo = filho.FicTecProdPercTipo;
                                            filhoData.FicTecProdDataQtdCalc = filho.FicTecProdQtdCalc;
                                            filhoData.FicTecProdDataCustoPerc = filho.FicTecProdCustoPerc;
                                            filhoData.FicTecProdDataCustoPercTipo = filho.FicTecProdCustoPercTipo;
                                            filhoData.FicTecProdDataCustoQtd = filho.FicTecProdCustoQtd;
                                            filhoData.FicTecProdDataCompCusto = filho.FicTecProdCompCusto;
                                            filhoData.FicTecProdDataGeraOP = filho.FicTecProdGeraOP;
                                            filhoData.FicTecProdDataPercPartic = filho.FicTecProdPercPartic;
                                            filhoData.FicTecProdDataPartVenda = filho.FicTecProdPartVenda;
                                            filhoData.FicTecProdDataPartVenPcDesc = filho.FicTecProdPartVendaPercDesc;
                                            filhoData.FicTecProdDataPartVenPcAcresc = filho.FicTecProdPartVendaPercAcresc;
                                        }

                                        bdFormulaPPCP.SaveChanges();
                                    }
                                    else
                                    {
                                        // Insere Produto Filho na Ficha Técnica
                                        FIC_TEC_PROD filho = new FIC_TEC_PROD();

                                        filho.ProdCodEstr = codigoProdutoPai.ProdCodEstr;
                                        filho.FicTecProdCodEstr = codigoProduto;
                                        filho.FicTecProdSeq = bdFormulaPPCP.FIC_TEC_PROD.Max(f => f.FicTecProdSeq) + 1;
                                        filho.FicTecProdPercTipo = "Aproveitamento";
                                        filho.FicTecProdCustoPerc = 100;
                                        filho.FicTecProdCustoPercTipo = "Aproveitamento";
                                        filho.FicTecProdCompCusto = "Sim";
                                        filho.FicTecProdGeraOP = "Sim";
                                        filho.FicTecProdPercPartic = 100;
                                        filho.FicTecProdPartVenda = "Não";

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
                                        existe = bdFormulaPPCP.PROD_UNID_MED1
                                            .Where(u => u.ProdCodEstr == codigoProduto && u.ProdUnidMedCod == "KG")
                                            .Count();

                                        if (existe > 0)
                                        {
                                            PROD_UNID_MED1 prodUnidMed = bdFormulaPPCP.PROD_UNID_MED1
                                                .Where(u => u.ProdCodEstr == codigoProduto && u.ProdUnidMedCod == "KG")
                                                .First();

                                            filho.FicTecProdUnidMedCodDig = prodUnidMed.ProdUnidMedCod;
                                            filho.FicTecProdUnidMedPosDig = prodUnidMed.ProdUnidMedPos;
                                        }
                                        else
                                        {
                                            ViewBag.fileName = "";
                                            ViewBag.erro = "Erro ao realizar a importação: O produto " + codigoProduto
                                                + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                                + " novamente!";
                                            arquivo.Close();
                                            return View("Index", "");
                                        }

                                        /****/
                                        filho.FicTecProdAcessorio = "Não";
                                        filho.FicTecProdDiscOpt = "Não";
                                        filho.FicTecProdFormula = "procedure Execute ;begin end;";
                                        filho.FicTecProdTerc = "Não";
                                        filho.FicTecProdGeraNFRetorno = "Nenhum";
                                        filho.FicTecProdOperCalculo = "Nenhum";
                                        filho.FicTecProdCompCustoAnaFinanc = "Não";
                                        filho.FicTecProdBloqueada = "Não";
                                        filho.fictecprodcalccustovalinsumo = "Não";

                                        erroColuna = "E";

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            filho.FicTecProdQtd = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);
                                        }
                                        else
                                        {
                                            filho.FicTecProdQtd = 0;
                                        }

                                        erroColuna = "G";

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            filho.FicTecProdPerc = Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))) * 100;
                                        }
                                        else
                                        {
                                            filho.FicTecProdPerc = 1;
                                        }

                                        filho.FicTecProdQtdCalc = filho.FicTecProdQtd / ((filho.FicTecProdPerc == 0 ? 1 : filho.FicTecProdPerc) / 100);
                                        filho.FicTecProdCustoQtd = filho.FicTecProdQtdCalc;

                                        DateTime? dataAntiga = filho.FicTecProdDataInic;

                                        erroColuna = "H";

                                        filho.FicTecProdDataInic = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));

                                        erroColuna = "I";

                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                                .First().InnerText != "")
                                        {
                                            filho.FicTecProdDataFim = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text));
                                        }
                                        else
                                        {
                                            filho.FicTecProdDataFim = null;
                                        }

                                        // Caso a Data Inicial do Filho seja diferente da Planilha, será inserido no Log de datas
                                        if (dataAntiga != FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text)))
                                        {
                                            FIC_TEC_PROD_DATA filhoData = new FIC_TEC_PROD_DATA();

                                            filhoData.ProdCodEstr = filho.ProdCodEstr;
                                            filhoData.FicTecProdSeq = filho.FicTecProdSeq;

                                            int sequenciaData = bdFormulaPPCP.FIC_TEC_PROD_DATA
                                                .Where(d => d.ProdCodEstr == filho.ProdCodEstr &&
                                                    d.FicTecProdSeq == filho.FicTecProdSeq)
                                                .Count() + 1;

                                            filhoData.FicTecProdDataSeq = sequenciaData;
                                            filhoData.FicTecProdDataInicio = filho.FicTecProdDataInic;
                                            filhoData.FicTecProdDataQtd = filho.FicTecProdQtd;
                                            filhoData.FicTecProdDataPerc = filho.FicTecProdPerc;
                                            filhoData.FicTecProdDataPercTipo = filho.FicTecProdPercTipo;
                                            filhoData.FicTecProdDataQtdCalc = filho.FicTecProdQtdCalc;
                                            filhoData.FicTecProdDataCustoPerc = filho.FicTecProdCustoPerc;
                                            filhoData.FicTecProdDataCustoPercTipo = filho.FicTecProdCustoPercTipo;
                                            filhoData.FicTecProdDataCustoQtd = filho.FicTecProdCustoQtd;
                                            filhoData.FicTecProdDataCompCusto = filho.FicTecProdCompCusto;
                                            filhoData.FicTecProdDataGeraOP = filho.FicTecProdGeraOP;
                                            filhoData.FicTecProdDataPercPartic = filho.FicTecProdPercPartic;
                                            filhoData.FicTecProdDataPartVenda = filho.FicTecProdPartVenda;
                                            filhoData.FicTecProdDataPartVenPcDesc = filho.FicTecProdPartVendaPercDesc;
                                            filhoData.FicTecProdDataPartVenPcAcresc = filho.FicTecProdPartVendaPercAcresc;

                                            bdFormulaPPCP.FIC_TEC_PROD_DATA.AddObject(filhoData);
                                        }

                                        bdFormulaPPCP.FIC_TEC_PROD.AddObject(filho);

                                        bdFormulaPPCP.SaveChanges();
                                    }
                                }

                                if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "B" + linha.RowIndex)
                                                .First().InnerText != "")
                                {
                                    if (FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart) == "Totais")
                                    {
                                        codigoProdutoPai1.USERPpcpKg01Batida = Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));

                                        break;
                                    }
                                }
                            }
	                    }

                        bdFormulaPPCP.SaveChanges();

                        // Atualiza Custo da Ficha Técnica
                        bdFormulaPPCP.CustoFicTec(codigoProdutoPai.ProdCodEstr, "3", 0, 0);

                        // Localiza os Itens dos Processos do Produto para deletar
                        existe = 0;

                        existe = bdFormulaPPCP.PROD_OPER_ITEM
                            .Where(o => o.ProdCodEstr == codigoProdutoPai.ProdCodEstr)
                            .Count();

                        if (existe > 0)
                        {
                            var listaItensProcessosProduto = bdFormulaPPCP.PROD_OPER_ITEM
                                .Where(o => o.ProdCodEstr == codigoProdutoPai.ProdCodEstr)
                                .ToList();

                            // Deleta os Itens do Processos do Produto Pai
                            foreach (var itemProcessoProduto in listaItensProcessosProduto)
                            {
                                bdFormulaPPCP.DeleteObject(itemProcessoProduto);
                            }
                        }

                        // Localiza os filhos da Ficha Técnica do Pai
                        var listaFichaTecnicaFilhos = bdFormulaPPCP.FIC_TEC_PROD
                            .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr
                                && f.FicTecProdDataInic >= codigoProdutoPai.ProdDataValidInic
                                && (f.FicTecProdDataFim <= codigoProdutoPai.ProdDataValidInic || f.FicTecProdDataFim == null))
                            .ToList();

                        // Localiza Operação do Pai
                        PROD_OPER prodOper = bdFormulaPPCP.PROD_OPER
                            .Where(o => o.ProdCodEstr == codigoProdutoPai.ProdCodEstr)
                            .First();

                        // Insere os Itens dos Processos de Acordo com a Ficha Técnica do Pai
                        foreach (var itemFichaTecnicaFilhos in listaFichaTecnicaFilhos)
                        {
                            PROD_OPER_ITEM prodOperItem = new PROD_OPER_ITEM();

                            prodOperItem.ProdCodEstr = itemFichaTecnicaFilhos.ProdCodEstr;
                            prodOperItem.ProdOperSeq = prodOper.ProdOperSeq;
                            prodOperItem.ProdOperItProdCodEstr = itemFichaTecnicaFilhos.FicTecProdCodEstr;
                            prodOperItem.ProdOperItSeq = itemFichaTecnicaFilhos.FicTecProdSeq;
                            prodOperItem.ProdOperItQtd = itemFichaTecnicaFilhos.FicTecProdQtdCalc;
                            prodOperItem.ProdOperItDataValidInic = itemFichaTecnicaFilhos.FicTecProdDataInic;
                            prodOperItem.ProdOperItQtdFicTecAlter = "Não";

                            bdFormulaPPCP.PROD_OPER_ITEM.AddObject(prodOperItem);
                        }
                    }
                }

                arquivo.Close();

                bdFormulaPPCP.SaveChanges();
                
                return View("Index", "");
            }
            catch (Exception e)
            {
                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                string msg = "";
                ViewBag.fileName = "";
                msg = "Erro ao realizar a importação: " + e.Message;
                if (e.InnerException != null)
                {
                    msg = msg + " / " + e.InnerException.Message;
                }
                msg = msg + " / Erro na planilha: Aba - " + erroAba + " | Linha - " + erroLinha
                    + " | Coluna: - " + erroColuna;
                msg = msg + " / Erro linha código: " + linenum.ToString();
                ViewBag.Erro = msg;
                arquivo.Close();
                return View("Index", "");
            }
        }

        [HttpPost]
        public ActionResult ImportaDadosFormulaPPCPNew()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string caminho = @"C:\inetpub\wwwroot\Relatorios\" + Session["login"].ToString() + ".xls";

            Request.Files[0].SaveAs(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            string erroAba = "";
            string erroLinha = "";
            string erroColuna = "";

            try
            {
                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                PRODUTO1 codigoProdutoPai1 = new PRODUTO1();
                PRODUTO codigoProdutoPai = new PRODUTO();

                // Navega entre cada Planilha
                foreach (var planilha in lista)
                {
                    #region Verifica se existe a fórmula

                    int result;
                    int name = 0;

                    if (int.TryParse(planilha.Name.ToString(), out result))
                        name = Convert.ToInt16(planilha.Name);

                    erroAba = planilha.Name.ToString();

                    int existe = bdFormulaPPCP.PRODUTO1
                        .Where(p => p.USERNumFormula == name)
                        .Count();

                    #endregion

                    // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                    if (existe > 0)
                    {
                        #region Carrega dados do cabeçalho da fórmula e as linhas da aba

                        // Localiza o Produto no Apolo que tem a Fórmula Cadastrada
                        codigoProdutoPai1 = bdFormulaPPCP.PRODUTO1
                            .Where(p => p.USERNumFormula == name)
                            .First();

                        codigoProdutoPai = bdFormulaPPCP.PRODUTO
                            .Where(p => p.ProdCodEstr == codigoProdutoPai1.ProdCodEstr)
                            .First();

                        string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == planilha.Name)
                                                    .First().Id;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                        .GetPartById(relationshipId);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        //var listaLinhas = planilha.Elements<Row>().ToList();
                        var listaLinhas = sheetData.Descendants<Row>().ToList();

                        // Pega a Validade da Fórmula e altera na Data de Validade do Produto
                        Row linhaData = sheetData.Elements<Row>().Where(r => r.RowIndex == 2).First();
                        Cell celulaData = linhaData.Elements<Cell>().Where(c => c.CellReference == "C2").First();
                        codigoProdutoPai.ProdDataValidInic = FromExcelSerialDate(Convert.ToInt32(celulaData.InnerText));

                        List<InsumoFormulaRacao> listaInsumos = new List<InsumoFormulaRacao>();

                        #endregion

                        // Navega nas linhas da Planilha
                        foreach (var linha in listaLinhas)
                        {
                            erroLinha = linha.RowIndex.ToString();

                            // Recupera o Código do Produto Filho da Planilha caso exista
                            existe = 0;
                            existe = linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex)
                                .Count();

                            if (existe > 0)
                            {
                                string codigoProduto = FormataCodigoProduto(linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "B" + linha.RowIndex)
                                                    .First().InnerText, linha.RowIndex);

                                existe = 0;
                                existe = bdFormulaPPCP.PRODUTO
                                    .Where(p => p.ProdCodEstr == codigoProduto)
                                    .Count();

                                if (existe > 0)
                                {
                                    #region Carrega dados do Insumo

                                    existe = 0;
                                    existe = bdFormulaPPCP.FIC_TEC_PROD
                                        .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr &&
                                            f.FicTecProdCodEstr == codigoProduto).Count();

                                    InsumoFormulaRacao insumo = new InsumoFormulaRacao();
                                    insumo.ProdCodEstr = codigoProduto;

                                    if (linha.Elements<Cell>().Where(c => c.CellReference.Value == "E" + linha.RowIndex).First().InnerText != "")
                                        insumo.QtdeKg = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                        .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 9);
                                    else
                                        insumo.QtdeKg = 0;

                                    if (linha.Elements<Cell>().Where(c => c.CellReference.Value == "G" + linha.RowIndex).First().InnerText != "")
                                        insumo.PercAjusteQuebra = Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))) * 100;
                                    else
                                        insumo.PercAjusteQuebra = 0;

                                    insumo.DataIni = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));

                                    if (linha.Elements<Cell>().Where(c => c.CellReference.Value == "I" + linha.RowIndex).First().InnerText != "")
                                        insumo.DataFim = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));
                                    else
                                        insumo.DataFim = null;

                                    listaInsumos.Add(insumo);

                                    #endregion
                                }
                            }
                        }

                        var retorno = AtualizaFormulaApolo((int)codigoProdutoPai1.USERNumFormula, listaInsumos);

                        if (retorno != "")
                        {
                            ViewBag.fileName = "";
                            ViewBag.erro = retorno;
                            arquivo.Close();
                            return View("Index", "");
                        }
                    }
                }

                arquivo.Close();

                return View("Index", "");
            }
            catch (Exception e)
            {
                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                string msg = "";
                ViewBag.fileName = "";
                msg = "Erro ao realizar a importação: " + e.Message;
                if (e.InnerException != null)
                {
                    msg = msg + " / " + e.InnerException.Message;
                }
                msg = msg + " / Erro na planilha: Aba - " + erroAba + " | Linha - " + erroLinha
                    + " | Coluna: - " + erroColuna;
                msg = msg + " / Erro linha código: " + linenum.ToString();
                ViewBag.Erro = msg;
                arquivo.Close();
                return View("Index", "");
            }
        }

        public static DateTime FromExcelSerialDate(int SerialDate)
        {
            if (SerialDate > 59) SerialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(SerialDate);
        }

        public static String FromExcelTextBollean(Cell theCell, WorkbookPart wbPart)
        {
            string value = value = theCell.Descendants<CellValue>().First().Text;

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

        #region Integração APOLO

        public static String AtualizaFormulaApolo(int numeroFormula, List<InsumoFormulaRacao> listaInsumos)
        {
            string retorno = "";

            FormulaPPCPEntities bd = new FormulaPPCPEntities();

            PRODUTO1 codigoProdutoPai1 = bd.PRODUTO1.Where(p => p.USERNumFormula == numeroFormula).FirstOrDefault();

            if (codigoProdutoPai1 != null)
            {
                // Localiza o Produto no Apolo que tem a Fórmula Cadastrada
                PRODUTO codigoProdutoPai = bd.PRODUTO.Where(p => p.ProdCodEstr == codigoProdutoPai1.ProdCodEstr).First();

                foreach (var insumo in listaInsumos)
                {
                    FIC_TEC_PROD filho = bd.FIC_TEC_PROD.Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr && f.FicTecProdCodEstr == insumo.ProdCodEstr)
                        .OrderByDescending(f => f.FicTecProdSeq).FirstOrDefault();

                    if (filho != null)
                    {
                        #region Caso ele exista, realiza as outras operações

                        filho.FicTecProdQtd = insumo.QtdeKg;
                        filho.FicTecProdPerc = insumo.PercAjusteQuebra;
                        filho.FicTecProdQtdCalc = filho.FicTecProdQtd / ((filho.FicTecProdPerc == 0 ? 1 : filho.FicTecProdPerc) / 100);
                        filho.FicTecProdCustoQtd = filho.FicTecProdQtdCalc;

                        DateTime? dataAntiga = filho.FicTecProdDataInic;

                        filho.FicTecProdDataInic = insumo.DataIni;
                        filho.FicTecProdDataFim = insumo.DataFim;

                        if (dataAntiga != insumo.DataIni)
                        {
                            #region Caso a Data Inicial do Filho seja diferente da Planilha, será inserido no Log de datas

                            FIC_TEC_PROD_DATA filhoData = new FIC_TEC_PROD_DATA();

                            filhoData.ProdCodEstr = filho.ProdCodEstr;
                            filhoData.FicTecProdSeq = filho.FicTecProdSeq;

                            int sequenciaData = bd.FIC_TEC_PROD_DATA.Where(d => d.ProdCodEstr == filho.ProdCodEstr && d.FicTecProdSeq == filho.FicTecProdSeq).Count() + 1;

                            filhoData.FicTecProdDataSeq = sequenciaData;
                            filhoData.FicTecProdDataInicio = filho.FicTecProdDataInic;
                            filhoData.FicTecProdDataQtd = filho.FicTecProdQtd;
                            filhoData.FicTecProdDataPerc = filho.FicTecProdPerc;
                            filhoData.FicTecProdDataPercTipo = filho.FicTecProdPercTipo;
                            filhoData.FicTecProdDataQtdCalc = filho.FicTecProdQtdCalc;
                            filhoData.FicTecProdDataCustoPerc = filho.FicTecProdCustoPerc;
                            filhoData.FicTecProdDataCustoPercTipo = filho.FicTecProdCustoPercTipo;
                            filhoData.FicTecProdDataCustoQtd = filho.FicTecProdCustoQtd;
                            filhoData.FicTecProdDataCompCusto = filho.FicTecProdCompCusto;
                            filhoData.FicTecProdDataGeraOP = filho.FicTecProdGeraOP;
                            filhoData.FicTecProdDataPercPartic = filho.FicTecProdPercPartic;
                            filhoData.FicTecProdDataPartVenda = filho.FicTecProdPartVenda;
                            filhoData.FicTecProdDataPartVenPcDesc = filho.FicTecProdPartVendaPercDesc;
                            filhoData.FicTecProdDataPartVenPcAcresc = filho.FicTecProdPartVendaPercAcresc;

                            bd.FIC_TEC_PROD_DATA.AddObject(filhoData);

                            #endregion
                        }
                        else
                        {
                            #region Senão será atualizado o Log existente

                            FIC_TEC_PROD_DATA filhoData = bd.FIC_TEC_PROD_DATA.Where(d => d.ProdCodEstr == codigoProdutoPai.ProdCodEstr && d.FicTecProdSeq == filho.FicTecProdSeq)
                                .OrderByDescending(d => d.FicTecProdDataSeq).FirstOrDefault();

                            filhoData.FicTecProdDataInicio = filho.FicTecProdDataInic;
                            filhoData.FicTecProdDataQtd = filho.FicTecProdQtd;
                            filhoData.FicTecProdDataPerc = filho.FicTecProdPerc;
                            filhoData.FicTecProdDataPercTipo = filho.FicTecProdPercTipo;
                            filhoData.FicTecProdDataQtdCalc = filho.FicTecProdQtdCalc;
                            filhoData.FicTecProdDataCustoPerc = filho.FicTecProdCustoPerc;
                            filhoData.FicTecProdDataCustoPercTipo = filho.FicTecProdCustoPercTipo;
                            filhoData.FicTecProdDataCustoQtd = filho.FicTecProdCustoQtd;
                            filhoData.FicTecProdDataCompCusto = filho.FicTecProdCompCusto;
                            filhoData.FicTecProdDataGeraOP = filho.FicTecProdGeraOP;
                            filhoData.FicTecProdDataPercPartic = filho.FicTecProdPercPartic;
                            filhoData.FicTecProdDataPartVenda = filho.FicTecProdPartVenda;
                            filhoData.FicTecProdDataPartVenPcDesc = filho.FicTecProdPartVendaPercDesc;
                            filhoData.FicTecProdDataPartVenPcAcresc = filho.FicTecProdPartVendaPercAcresc;

                            #endregion
                        }

                        bd.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region Insere Produto Filho na Ficha Técnica

                        filho = new FIC_TEC_PROD();

                        filho.ProdCodEstr = codigoProdutoPai.ProdCodEstr;
                        filho.FicTecProdCodEstr = insumo.ProdCodEstr;
                        filho.FicTecProdSeq = bd.FIC_TEC_PROD.Max(f => f.FicTecProdSeq) + 1;
                        filho.FicTecProdPercTipo = "Aproveitamento";
                        filho.FicTecProdCustoPerc = 100;
                        filho.FicTecProdCustoPercTipo = "Aproveitamento";
                        filho.FicTecProdCompCusto = "Sim";
                        filho.FicTecProdGeraOP = "Sim";
                        filho.FicTecProdPercPartic = 100;
                        filho.FicTecProdPartVenda = "Não";

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

                        var existe = 0;
                        existe = bd.PROD_UNID_MED1.Where(u => u.ProdCodEstr == insumo.ProdCodEstr && u.ProdUnidMedCod == "KG").Count();

                        if (existe > 0)
                        {
                            PROD_UNID_MED1 prodUnidMed = bd.PROD_UNID_MED1
                                .Where(u => u.ProdCodEstr == insumo.ProdCodEstr && u.ProdUnidMedCod == "KG")
                                .First();

                            filho.FicTecProdUnidMedCodDig = prodUnidMed.ProdUnidMedCod;
                            filho.FicTecProdUnidMedPosDig = prodUnidMed.ProdUnidMedPos;
                        }
                        else
                        {
                            retorno = "Erro ao realizar a importação: O produto " + insumo.ProdCodEstr
                                + " não tem Unidade de Medida 'KG' cadastrada. Primeiro realize o cadastro e realize a importação"
                                + " novamente!";
                            return retorno;
                        }

                        /****/
                        filho.FicTecProdAcessorio = "Não";
                        filho.FicTecProdDiscOpt = "Não";
                        filho.FicTecProdFormula = "procedure Execute ;begin end;";
                        filho.FicTecProdTerc = "Não";
                        filho.FicTecProdGeraNFRetorno = "Nenhum";
                        filho.FicTecProdOperCalculo = "Nenhum";
                        filho.FicTecProdCompCustoAnaFinanc = "Não";
                        filho.FicTecProdBloqueada = "Não";
                        filho.fictecprodcalccustovalinsumo = "Não";

                        filho.FicTecProdQtd = insumo.QtdeKg;
                        filho.FicTecProdPerc = insumo.PercAjusteQuebra;
                        filho.FicTecProdQtdCalc = filho.FicTecProdQtd / ((filho.FicTecProdPerc == 0 ? 1 : filho.FicTecProdPerc) / 100);
                        filho.FicTecProdCustoQtd = filho.FicTecProdQtdCalc;

                        DateTime? dataAntiga = filho.FicTecProdDataInic;

                        filho.FicTecProdDataInic = insumo.DataIni;

                        filho.FicTecProdDataFim = insumo.DataFim;
                        
                        FIC_TEC_PROD_DATA filhoData = new FIC_TEC_PROD_DATA();

                        filhoData.ProdCodEstr = filho.ProdCodEstr;
                        filhoData.FicTecProdSeq = filho.FicTecProdSeq;

                        int sequenciaData = bd.FIC_TEC_PROD_DATA.Where(d => d.ProdCodEstr == filho.ProdCodEstr && d.FicTecProdSeq == filho.FicTecProdSeq).Count() + 1;

                        filhoData.FicTecProdDataSeq = sequenciaData;
                        filhoData.FicTecProdDataInicio = filho.FicTecProdDataInic;
                        filhoData.FicTecProdDataQtd = filho.FicTecProdQtd;
                        filhoData.FicTecProdDataPerc = filho.FicTecProdPerc;
                        filhoData.FicTecProdDataPercTipo = filho.FicTecProdPercTipo;
                        filhoData.FicTecProdDataQtdCalc = filho.FicTecProdQtdCalc;
                        filhoData.FicTecProdDataCustoPerc = filho.FicTecProdCustoPerc;
                        filhoData.FicTecProdDataCustoPercTipo = filho.FicTecProdCustoPercTipo;
                        filhoData.FicTecProdDataCustoQtd = filho.FicTecProdCustoQtd;
                        filhoData.FicTecProdDataCompCusto = filho.FicTecProdCompCusto;
                        filhoData.FicTecProdDataGeraOP = filho.FicTecProdGeraOP;
                        filhoData.FicTecProdDataPercPartic = filho.FicTecProdPercPartic;
                        filhoData.FicTecProdDataPartVenda = filho.FicTecProdPartVenda;
                        filhoData.FicTecProdDataPartVenPcDesc = filho.FicTecProdPartVendaPercDesc;
                        filhoData.FicTecProdDataPartVenPcAcresc = filho.FicTecProdPartVendaPercAcresc;

                        bd.FIC_TEC_PROD_DATA.AddObject(filhoData);
                        bd.FIC_TEC_PROD.AddObject(filho);

                        bd.SaveChanges();

                        #endregion
                    }

                    codigoProdutoPai1.USERPpcpKg01Batida = 2000;
                }

                bd.SaveChanges();

                // Atualiza Custo da Ficha Técnica
                bd.CustoFicTec(codigoProdutoPai.ProdCodEstr, "3", 0, 0);

                #region  Localiza os Itens dos Processos do Produto para deletar

                var existe2 = 0;

                existe2 = bd.PROD_OPER_ITEM
                    .Where(o => o.ProdCodEstr == codigoProdutoPai.ProdCodEstr)
                    .Count();

                if (existe2 > 0)
                {
                    var listaItensProcessosProduto = bd.PROD_OPER_ITEM
                        .Where(o => o.ProdCodEstr == codigoProdutoPai.ProdCodEstr)
                        .ToList();

                    // Deleta os Itens do Processos do Produto Pai
                    foreach (var itemProcessoProduto in listaItensProcessosProduto)
                    {
                        bd.DeleteObject(itemProcessoProduto);
                    }
                }

                #endregion

                // Localiza os filhos da Ficha Técnica do Pai
                var listaFichaTecnicaFilhos = bd.FIC_TEC_PROD
                    .Where(f => f.ProdCodEstr == codigoProdutoPai.ProdCodEstr
                        && f.FicTecProdDataInic >= codigoProdutoPai.ProdDataValidInic
                        && (f.FicTecProdDataFim <= codigoProdutoPai.ProdDataValidInic || f.FicTecProdDataFim == null))
                    .ToList();

                // Localiza Operação do Pai
                PROD_OPER prodOper = bd.PROD_OPER
                    .Where(o => o.ProdCodEstr == codigoProdutoPai.ProdCodEstr)
                    .First();

                #region Insere os Itens dos Processos de Acordo com a Ficha Técnica do Pai

                foreach (var itemFichaTecnicaFilhos in listaFichaTecnicaFilhos)
                {
                    PROD_OPER_ITEM prodOperItem = new PROD_OPER_ITEM();

                    prodOperItem.ProdCodEstr = itemFichaTecnicaFilhos.ProdCodEstr;
                    prodOperItem.ProdOperSeq = prodOper.ProdOperSeq;
                    prodOperItem.ProdOperItProdCodEstr = itemFichaTecnicaFilhos.FicTecProdCodEstr;
                    prodOperItem.ProdOperItSeq = itemFichaTecnicaFilhos.FicTecProdSeq;
                    prodOperItem.ProdOperItQtd = itemFichaTecnicaFilhos.FicTecProdQtdCalc;
                    prodOperItem.ProdOperItDataValidInic = itemFichaTecnicaFilhos.FicTecProdDataInic;
                    prodOperItem.ProdOperItQtdFicTecAlter = "Não";

                    bd.PROD_OPER_ITEM.AddObject(prodOperItem);
                }

                #endregion

                bd.SaveChanges();
            }

            return retorno;
        }

        #endregion
    }
}
