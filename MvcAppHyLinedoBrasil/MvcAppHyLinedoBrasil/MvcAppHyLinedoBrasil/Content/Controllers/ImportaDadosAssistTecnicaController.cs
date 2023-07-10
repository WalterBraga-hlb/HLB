using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHyLinedoBrasil.Models.HLBAPP;
using MvcAppHyLinedoBrasil.Models;
using MvcAppHyLinedoBrasil.Models.Apolo;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MvcAppHyLinedoBrasil.Controllers;
using System.Text.RegularExpressions;
using Access = Microsoft.Office.Interop.Access;
using Excel = Microsoft.Office.Interop.Excel;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Net;
using System.Threading;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class ImportaDadosAssistTecnicaController : Controller
    {
        #region Objetos

        HLBAPPEntities1 bdHLBAPP = new HLBAPPEntities1();
        FinanceiroEntities bdApolo = new FinanceiroEntities();
        ApoloEntities apolo = new ApoloEntities();

        #endregion

        #region Carrega Listas

        public void CarregaModelosDeArquivo()
        {
            List<SelectListItem> modelosDeArquivo = new List<SelectListItem>();

            modelosDeArquivo.Add(new SelectListItem { Text = "EggCell - Modelo Antigo", Value = "1", Selected = true });

            Session["ModelosDeArquivo"] = modelosDeArquivo;
        }

        public void CarregaListaEstados()
        {
            List<SelectListItem> listaEstados = new List<SelectListItem>();

            var lista = bdApolo.CIDADE.GroupBy(g => g.UfSigla).OrderBy(o => o.Key).ToList();

            listaEstados.Add(new SelectListItem { Text = "(Todos)", Value = "(Todos)", Selected = true });

            foreach (var item in lista)
            {
                listaEstados.Add(new SelectListItem { Text = item.Key, Value = item.Key, Selected = false });
            }

            Session["ListaEstados"] = listaEstados;
        }

        public void CarregaListaTiposRelatorioDadosAssitTecnica()
        {
            List<SelectListItem> listaTiposRelatorio = new List<SelectListItem>();

            listaTiposRelatorio.Add(new SelectListItem { Text = "Recria", Value = "Recria", Selected = true });
            listaTiposRelatorio.Add(new SelectListItem { Text = "Produção", Value = "Produção", Selected = false });

            Session["ListaTiposRelatorioDadosAssitTecnica"] = listaTiposRelatorio;
        }

        public void AtualizaTipoRelatorioSelecionado(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaTiposRelatorioDadosAssitTecnica"];

            foreach (var item in estados)
            {
                if (item.Text == modelo)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
        }  

        public void AtualizaModelosDeArquivo(string modelo)
        {
            List<SelectListItem> modelos = (List<SelectListItem>)Session["ModelosDeArquivo"];

            foreach (var item in modelos)
            {
                if (item.Text == modelo)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ModelosDeArquivo"] = modelos;
        }

        public void AtualizaEstadoSelecionado(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaEstados"];

            foreach (var item in estados)
            {
                if (item.Text == modelo)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaEstados"] = estados;
        }

        #endregion

        #region Funções Leituras Excel

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private string GetColumnName(string cellReference)
        {
            var regex = new Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);

            return match.Value;
        }

        private int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            var convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                // ASCII 'A' = 65
                int current = i == 0 ? letter - 65 : letter - 64;
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private void RunMacro(object oApp, object[] oRunArgs)
        {
            oApp.GetType().InvokeMember("Run",
                System.Reflection.BindingFlags.Default |
                System.Reflection.BindingFlags.InvokeMethod,
                null, oApp, oRunArgs);
        }

        #endregion

        #region Importa Dados Arquivo Único

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaModelosDeArquivo();
            CarregaListaEstados();
            Session["descricao"] = "";
            Session["estado"] = "";
            Session["marcado"] = "";
            List<Cliente> listaExibeClientes = null;
            Session["ListaClientes"] = listaExibeClientes;
            return View("Index", listaExibeClientes);
        }

        [HttpPost]
        public ActionResult ImportaDados(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            List<Cliente> listaExibeClientes = null;

            string codigoCliente = model["clienteSelecionado"];
            Session["marcado"] = codigoCliente;

            if (codigoCliente == null)
            {
                Session["marcado"] = "";
                ViewBag.erro = "Necessário selecionar um cliente primeiro!";
                listaExibeClientes = (List<Cliente>)Session["ListaClientes"];
                return View("Index", listaExibeClientes);
            }

            string retorno = "";
            retorno = ImportaPlanilha(codigoCliente, DateTime.Now);

            if (retorno.Equals(""))
            {
                ViewBag.erro = "";
                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";
            }
            else
            {
                ViewBag.erro = retorno;
                ViewBag.fileName = "";
            }

            listaExibeClientes = (List<Cliente>)Session["ListaClientes"];
            return View("Index", listaExibeClientes);
        }

        public string ImportaPlanilha(string codigoCliente, DateTime dataImportacao)
        {
            string caminho = @"C:\inetpub\wwwroot\Relatorios\DadosAssistTecnica_" + Session["login"].ToString() + "_" 
                //+ Session.SessionID.ToString() 
                + "_" + DateTime.Now.ToString("dd-MM-yyy")
                + "_" + DateTime.Now.ToString("mm-ss")
                + "_" + DateTime.Now.Millisecond 
                + ".xlsx";

            //List<Cliente> listaExibeClientes = null;

            Request.Files[0].SaveAs(caminho);
            caminho = VerificaFormatoArquivo(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            //string modeloArquivo = model["Text"].ToString();

            Session["dataImportacao"] = dataImportacao;

            //AtualizaModelosDeArquivo(modeloArquivo);

            if (arquivo.Length > 0)
            {
                int retornoModeloArquivo = VerificaModeloArquivo(arquivo);

                if (retornoModeloArquivo.Equals(1))
                {
                    return ModeloEggCellAntigo(arquivo, codigoCliente);
                }
                else if (retornoModeloArquivo.Equals(2))
                {
                    return ModeloIana(arquivo, codigoCliente);
                }
                else if (retornoModeloArquivo.Equals(3))
                {
                    return ModeloErnestoRaigoAsaumi(arquivo, codigoCliente);
                }
                else if (retornoModeloArquivo.Equals(4))
                {
                    return ModeloEggCellCrescimentoAtual(arquivo, codigoCliente);
                }
                else if (retornoModeloArquivo.Equals(5))
                {
                    return ModeloEggCellCrescimentoNovo(arquivo, codigoCliente);
                }
                else if (retornoModeloArquivo.Equals(6))
                {
                    return ModeloEggCellProducaoNovo(arquivo, codigoCliente);
                }
                else if (retornoModeloArquivo.Equals(7))
                {
                    return ModeloEggCellProducaoAtual(arquivo, codigoCliente);
                }
                else
                {
                    return "Modelo não existente para importação!";
                }
            }
            else
            {
                //ViewBag.erro = "Selecione um arquivo para ser importado!";
                //listaExibeClientes = (List<Cliente>)Session["ListaClientes"];
                //return View("Index", listaExibeClientes);
                return "Selecione um arquivo para ser importado!";
            }
        }

        #endregion

        #region Modelos de Formulário

        public string ModeloEggCellAntigo(Stream arquivo, string codigoCliente)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                /**** Caso seja tenha Recria, iremos inserir a Recria. ****/

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Controle Recria")
                                                    .First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var listaLinhasRecria = sheetData.Descendants<Row>().ToList();

                // O Número do Lote está na aba de Produção. Sendo assim, iremos pegar ele também.

                relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Produção" || s.Name == "Produção 01")
                                                    .First().Id;

                WorksheetPart worksheetPartProducao = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetDataProducao = worksheetPartProducao.Worksheet.GetFirstChild<SheetData>();

                // Nº do Lote
                Row linhaLote = sheetDataProducao.Elements<Row>().Where(r => r.RowIndex == 7).First();
                Cell celulaLote = linhaLote.Elements<Cell>().Where(c => c.CellReference == "I7").First();
                //string numLote = FormulaPPCPController.FromExcelTextBollean(celulaLote, spreadsheetDocument.WorkbookPart);
                string numLote = celulaLote.Descendants<CellValue>().First().Text;

                /** Pega os Dados do Cabeçalho **/

                // Cliente
                Row linhaCliente = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                Cell celulaCliente = linhaCliente.Elements<Cell>().Where(c => c.CellReference == "C7").First();
                //string cliente = FormulaPPCPController.FromExcelTextBollean(celulaCliente, spreadsheetDocument.WorkbookPart);
                string cliente = celulaCliente.Descendants<CellValue>().First().Text;

                relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Indice")
                                                    .First().Id;

                WorksheetPart worksheetIndice = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetIndice = worksheetIndice.Worksheet.GetFirstChild<SheetData>();

                // Cidade
                Row linhaCidade = sheetIndice.Elements<Row>().Where(r => r.RowIndex == 10).First();
                Cell celulaCidade = linhaCidade.Elements<Cell>().Where(c => c.CellReference == "M10").First();
                //string cliente = FormulaPPCPController.FromExcelTextBollean(celulaCliente, spreadsheetDocument.WorkbookPart);
                string cidadeUf = FormulaPPCPController.FromExcelTextBollean(celulaCidade, spreadsheetDocument.WorkbookPart);
                int contemUF = cidadeUf.IndexOf("/");
                int tamanho = cidadeUf.IndexOf("/") == -1 ? cidadeUf.Length : cidadeUf.IndexOf("/") - 1;
                string cidade = cidadeUf.Substring(0, tamanho);

                int existe = 0;
                //string codigoCliente;

                //existe = bdApolo.CIDADE
                //    .Where(c => c.CidNome.Contains(cidade))
                //    .Count();

                //if (existe > 0)
                //{
                //    CIDADE cidadeObjeto = bdApolo.CIDADE
                //        .Where(c => c.CidNome.Contains(cidade))
                //        .First();

                //    existe = 0;

                //    existe = bdApolo.ENTIDADE
                //        .Where(e => e.EntNome.Contains(cliente) && e.CidCod == cidadeObjeto.CidCod)
                //        .Count();

                //    if (existe > 0)
                //    {
                //        ENTIDADE entidade = bdApolo.ENTIDADE
                //            .Where(e => e.EntNome.Contains(cliente) && e.CidCod == cidadeObjeto.CidCod)
                //            .First();

                //        codigoCliente = entidade.EntCod;
                //    }
                //    else
                //        codigoCliente = "";
                //}
                //else
                //{
                //    existe = 0;

                //    existe = bdApolo.ENTIDADE
                //        .Where(e => e.EntNome.Contains(cliente))
                //        .Count();

                //    if (existe > 0)
                //    {
                //        ENTIDADE entidade = bdApolo.ENTIDADE
                //            .Where(e => e.EntNome.Contains(cliente))
                //            .First();

                //        codigoCliente = entidade.EntCod;
                //    }
                //    else
                //        codigoCliente = "";
                //}

                // Granja
                Row linhaGranja = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                Cell celulaGranja = linhaGranja.Elements<Cell>().Where(c => c.CellReference == "C8").First();
                //string granja = FormulaPPCPController.FromExcelTextBollean(celulaGranja, spreadsheetDocument.WorkbookPart);
                string granja = celulaGranja.Descendants<CellValue>().First().Text;

                // Nº de Aves
                Row linhaNumAves = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                Cell celulaNumAves = linhaNumAves.Elements<Cell>().Where(c => c.CellReference == "L7").First();
                int numAves = Convert.ToInt32(celulaNumAves.Descendants<CellValue>().First().Text);

                // Data de Alojamento
                Row linhaDataAloj = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                Cell celulaDataAloj = linhaDataAloj.Elements<Cell>().Where(c => c.CellReference == "P7").First();
                DateTime dataAloj = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataAloj.Descendants<CellValue>().First().Text));

                // Linhagem
                Row linhaLinhagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                Cell celulaLinhagem = linhaLinhagem.Elements<Cell>().Where(c => c.CellReference == "N8").First();
                string linhagem = celulaLinhagem.Descendants<CellValue>().First().Text;

                string empresaLayout = Session["empresaLayout"].ToString();

                existe = 0;
                existe = bdHLBAPP.Dados_Assistencia_Tecnica
                    .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                        && d.Linhagem == linhagem
                        && d.Lote == numLote
                        && d.DataAlojamento == dataAloj //&& d.Idade != 1
                        && d.Tipo == "Recria")
                    .Count();

                string operacao = "Inclusão";
                if (existe > 0)
                {
                    var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.DataAlojamento == dataAloj //&& d.Idade != 1
                            && d.Linhagem == linhagem
                            && d.Lote == numLote
                            && d.Tipo == "Recria")
                        .ToList();

                    foreach (var item in listaExiste)
                    {
                        bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                    }

                    bdHLBAPP.SaveChanges();
                    existe = 0;
                    operacao = "Substituição dos Dados";
                }

                if (existe == 0)
                {
                    // Galpão
                    Row linhaGalpao = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                    Cell celulaGalpao = linhaGranja.Elements<Cell>().Where(c => c.CellReference == "K8").First();
                    string galpao = celulaGalpao.Descendants<CellValue>().First().Text;

                    // Observação Geral
                    Row linhaObservacaoGeral = sheetData.Elements<Row>().Where(r => r.RowIndex == 33).First();
                    Cell celulaObservacaoGeral = linhaObservacaoGeral.Elements<Cell>().Where(c => c.CellReference == "A33").First();
                    string observacaoGeral = FormulaPPCPController.FromExcelTextBollean(celulaObservacaoGeral, spreadsheetDocument.WorkbookPart);

                    int inventarioAves = numAves;

                    // Navega nas linhas da Planilha de Recria
                    foreach (var linha in listaLinhasRecria)
                    {
                        if ((linha.RowIndex >= 12) && (linha.RowIndex <= 31))
                        {
                            if (linha.Elements<Cell>()
                                    .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                    .First().InnerText != "")
                            {
                                Dados_Assistencia_Tecnica recria = new Dados_Assistencia_Tecnica();

                                recria.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                recria.Usuario = usuario;

                                recria.Empresa = Session["empresaLayout"].ToString();
                                recria.Tipo = "Recria";
                                recria.Lote = numLote;
                                recria.CodigoCliente = codigoCliente;
                                recria.NomeCliente = cliente;
                                recria.Granja = granja;
                                recria.SaldoInicialAvesAlojadas = numAves;
                                recria.DataAlojamento = dataAloj;
                                recria.Galpao = galpao;
                                recria.Linhagem = linhagem;
                                recria.Semana = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                        .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                                                        .First().Descendants<CellValue>().FirstOrDefault().Text));
                                recria.Idade = Convert.ToInt32(FormulaPPCPController.FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart));
                                recria.InventarioAves = inventarioAves;
                                recria.NumeroAvesMortas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "K" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                inventarioAves = inventarioAves - (int)recria.NumeroAvesMortas;

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "O" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.PercViabilidadeStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "O" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.PercViabilidadeStd = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.PesoCorporalStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "Q" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.PesoCorporalStd = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "R" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.PesoAve = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "R" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.PesoAve = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "T" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.Uniformidade = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "T" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.Uniformidade = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "U" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "U" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.ComsumoSemanal = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "V" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.ConsumoRacaoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "V" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.ConsumoRacaoStd = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "Y" + linha.RowIndex)
                                        .Count() > 0)
                                {
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "Y" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.Observacao = FormulaPPCPController.FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "Y" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart);
                                    }
                                    else
                                    {
                                        recria.Observacao = "";
                                    }
                                }

                                recria.ObservacaoGeral = observacaoGeral;

                                bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(recria);
                            }
                        }
                    }

                    InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Recria");

                    existe = 0;
                    existe = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.Linhagem == linhagem
                            && d.Lote == numLote
                            && d.DataAlojamento == dataAloj
                            && d.Tipo == "Produção")
                        .Count();

                    operacao = "Inclusão";
                    if (existe > 0)
                    {
                        var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                            .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                                && d.DataAlojamento == dataAloj
                                && d.Linhagem == linhagem
                                && d.Lote == numLote
                                && d.Tipo == "Produção")
                            .ToList();

                        foreach (var item in listaExiste)
                        {
                            bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                        }

                        bdHLBAPP.SaveChanges();
                        existe = 0;
                        operacao = "Substituição dos Dados";
                    }

                    // Varre as planilhas de Produção existentes
                    var listaPlanilhas = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().ToList();

                    foreach (var item in listaPlanilhas)
                    {
                        string nome = item.Name;
                        string contem = "Produção";
                        string naoContem = "Gráfico";

                        if ((nome.Contains(contem)) && (!nome.Contains(naoContem)))
                        {
                            WorksheetPart worksheetPartPrd = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(item.Id);

                            SheetData sheetDataPrd = worksheetPartPrd.Worksheet.GetFirstChild<SheetData>();

                            // Linhagem na Produção
                            Row linhaLinhagemProducao = sheetDataPrd.Elements<Row>().Where(r => r.RowIndex == 10).First();
                            Cell celulaLinhagemProducao = linhaLinhagemProducao.Elements<Cell>().Where(c => c.CellReference == "C10").First();
                            string linhagemProducao = FormulaPPCPController.FromExcelTextBollean(celulaLinhagemProducao, spreadsheetDocument.WorkbookPart);

                            // Nº de Aves na Produção
                            Row linhaNumAvesProducao = sheetDataPrd.Elements<Row>().Where(r => r.RowIndex == 10).First();
                            Cell celulaNumAvesProducao = linhaNumAvesProducao.Elements<Cell>().Where(c => c.CellReference == "I10").First();
                            int numAvesProducao = Convert.ToInt32(celulaNumAvesProducao.Descendants<CellValue>().First().Text);

                            var listaLinhasPrd = sheetDataPrd.Descendants<Row>().ToList();

                            inventarioAves = 0;
                            inventarioAves = numAvesProducao;

                            // Navega nas linhas da Planilha de Produção
                            foreach (var linha in listaLinhasPrd)
                            {
                                if (linha.RowIndex >= 15)
                                {
                                    if ((linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                            .Count() > 0)
                                        &&
                                        (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                            .Count() > 0))
                                    {
                                        if ((linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                                .First().InnerText != "")
                                            &&
                                            (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                                .First().InnerText != ""))
                                        {
                                            Dados_Assistencia_Tecnica producao = new Dados_Assistencia_Tecnica();

                                            producao.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                            producao.Usuario = usuario;

                                            producao.Empresa = Session["empresaLayout"].ToString();
                                            producao.Tipo = "Produção";
                                            producao.Lote = numLote;
                                            producao.CodigoCliente = codigoCliente;
                                            producao.NomeCliente = cliente;
                                            producao.Granja = granja;
                                            producao.SaldoInicialAvesAlojadas = numAvesProducao;
                                            producao.DataAlojamento = dataAloj;
                                            producao.Galpao = galpao;
                                            producao.Linhagem = linhagemProducao;
                                            producao.Semana = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                                    .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                                                                    .First().Descendants<CellValue>().FirstOrDefault().Text));
                                            producao.Idade = Convert.ToInt32(FormulaPPCPController.FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart));
                                            producao.InventarioAves = inventarioAves;
                                            producao.NumeroAvesMortas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                            inventarioAves = inventarioAves - (int)producao.NumeroAvesMortas;
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.PercViabilidadeStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.PercViabilidadeStd = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.QtdeOvosProduzidos = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "H" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                            }
                                            else
                                            {
                                                producao.QtdeOvosProduzidos = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.PercOvosProduzidoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.PercOvosProduzidoStd = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.OvosPorAveAlojStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "M" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.OvosPorAveAlojStd = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "P" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.PesoOvoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.PesoOvoStd = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.PesoOvo = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "Q" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.PesoOvo = 0;
                                            }

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "R" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "R" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.ComsumoSemanal = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "U" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.ConsumoRacaoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "U" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.ConsumoRacaoStd = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "W" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.PesoCorporalStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "W" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.PesoCorporalStd = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "X" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.PesoAve = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "X" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.PesoAve = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "Y" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.Uniformidade = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "Y" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.Uniformidade = 0;
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "Z" + linha.RowIndex)
                                                    .First().InnerText != "")
                                            {
                                                producao.Temperatura = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "Z" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                            }
                                            else
                                            {
                                                producao.Temperatura = 0;
                                            }

                                            bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(producao);
                                        }
                                    }
                                }
                            }

                            InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Produção");
                        }
                    }
                    //  

                    arquivo.Close();

                    bdHLBAPP.SaveChanges();
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";

                    return "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                }

                return "";
                //return View("Index", "");
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                return "Erro ao realizar a importação: " + e.Message;
                //return View("Index", "");
            }
        }

        public string ModeloIana(Stream arquivo, string codigoCliente)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";
                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                /**** Caso seja tenha Recria, iremos inserir a Recria. ****/

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Recria")
                                                    .First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var listaLinhasRecria = sheetData.Descendants<Row>().ToList();

                // O Número do Lote está na aba de Diário. Sendo assim, iremos pegar ele também.

                relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Diário")
                                                    .First().Id;

                WorksheetPart worksheetPartProducao = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetDataProducao = worksheetPartProducao.Worksheet.GetFirstChild<SheetData>();

                // Nº do Lote
                Row linhaLote = sheetDataProducao.Elements<Row>().Where(r => r.RowIndex == 4).First();
                Cell celulaLote = linhaLote.Elements<Cell>().Where(c => c.CellReference == "D4").First();
                string numLote = FormulaPPCPController.FromExcelTextBollean(celulaLote, spreadsheetDocument.WorkbookPart);

                /** Pega os Dados do Cabeçalho **/

                // Cliente
                string cliente = "Amauri Pinto Costa";
                //string codigoCliente = "0003803";

                // Granja
                string granja = "Granja Iana";

                // Nº de Aves
                Row linhaNumAves = sheetData.Elements<Row>().Where(r => r.RowIndex == 64).First();
                Cell celulaNumAves = linhaNumAves.Elements<Cell>().Where(c => c.CellReference == "K64").First();
                int numAves = Convert.ToInt32(celulaNumAves.Descendants<CellValue>().First().Text);

                // Data de Alojamento
                Row linhaDataAloj = sheetData.Elements<Row>().Where(r => r.RowIndex == 64).First();
                Cell celulaDataAloj = linhaDataAloj.Elements<Cell>().Where(c => c.CellReference == "O64").First();
                DateTime dataAloj = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataAloj.Descendants<CellValue>().First().Text));

                // Linhagem
                Row linhaLinhagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 65).First();
                Cell celulaLinhagem = linhaLinhagem.Elements<Cell>().Where(c => c.CellReference == "M65").First();
                string linhagem = FormulaPPCPController.FromExcelTextBollean(celulaLinhagem, spreadsheetDocument.WorkbookPart);


                string empresaLayout = Session["empresaLayout"].ToString();

                int existe = 0;
                existe = bdHLBAPP.Dados_Assistencia_Tecnica
                    .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                        && d.DataAlojamento == dataAloj //&& d.Idade != 1
                        && d.Linhagem == linhagem
                        && d.Lote == numLote
                        && d.Tipo == "Recria")
                    .Count();

                string operacao = "Inclusão";
                if (existe > 0)
                {
                    var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.DataAlojamento == dataAloj //&& d.Idade != 1
                            && d.Linhagem == linhagem
                            && d.Lote == numLote
                            && d.Tipo == "Recria")
                        .ToList();

                    foreach (var item in listaExiste)
                    {
                        bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                    }

                    operacao = "Substituição dos Dados";
                    bdHLBAPP.SaveChanges();
                    existe = 0;
                }

                if (existe == 0)
                {
                    // Galpão
                    Row linhaGalpao = sheetData.Elements<Row>().Where(r => r.RowIndex == 65).First();
                    Cell celulaGalpao = linhaGalpao.Elements<Cell>().Where(c => c.CellReference == "J65").First();
                    string galpao = FormulaPPCPController.FromExcelTextBollean(celulaGalpao, spreadsheetDocument.WorkbookPart);

                    // Observação Geral
                    string observacaoGeral = "";

                    foreach (var linha in listaLinhasRecria)
                    {
                        if ((linha.RowIndex >= 91) && (linha.RowIndex <= 97))
                        {
                            var lista2 = linha.Descendants<Cell>().ToList();

                            foreach (var coluna in lista2)
                            {
                                string columnName = GetColumnName(coluna.CellReference);

                                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                                if ((currentColumnIndex >= 3) && (currentColumnIndex <= 24))
                                {
                                    Cell celula = linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == coluna.CellReference.Value)
                                        .First();

                                    string obs = FormulaPPCPController.FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                    if (!obs.Equals(""))
                                        if (!observacaoGeral.Equals(""))
                                            observacaoGeral = observacaoGeral + " / " + obs;
                                        else
                                            observacaoGeral = obs;
                                }
                            }
                        }
                    }

                    int inventarioAves = numAves;

                    // Navega nas linhas da Planilha de Recria
                    foreach (var linha in listaLinhasRecria)
                    {
                        if ((linha.RowIndex >= 72) && (linha.RowIndex <= 89))
                        {
                            if (linha.Elements<Cell>()
                                    .Where(c => c.CellReference.Value == "B" + linha.RowIndex)
                                    .First().InnerText != "")
                            {
                                Dados_Assistencia_Tecnica recria = new Dados_Assistencia_Tecnica();

                                recria.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                recria.Usuario = usuario;

                                recria.Empresa = Session["empresaLayout"].ToString();
                                recria.Tipo = "Recria";
                                recria.Lote = numLote;
                                recria.CodigoCliente = codigoCliente;
                                recria.NomeCliente = cliente;
                                recria.Granja = granja;
                                recria.SaldoInicialAvesAlojadas = numAves;
                                recria.DataAlojamento = dataAloj;
                                recria.Galpao = galpao;
                                recria.Linhagem = linhagem;
                                recria.Idade = Convert.ToInt32(FormulaPPCPController.FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "A" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart));
                                recria.InventarioAves = inventarioAves;
                                recria.NumeroAvesMortas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                inventarioAves = inventarioAves - (int)recria.NumeroAvesMortas;

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "N" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.PercViabilidadeStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "N" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.PercViabilidadeStd = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "P" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.PesoCorporalStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.PesoCorporalStd = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.PesoAve = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "Q" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.PesoAve = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "S" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.Uniformidade = (Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "S" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))) * 100);
                                }
                                else
                                {
                                    recria.Uniformidade = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "U" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "U" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.ComsumoSemanal = 0;
                                }
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "W" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    recria.ConsumoRacaoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "W" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                }
                                else
                                {
                                    recria.ConsumoRacaoStd = 0;
                                }

                                recria.ObservacaoGeral = observacaoGeral;
                                recria.TipoComedouro = FormulaPPCPController.FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "V" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart);

                                bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(recria);
                            }
                        }
                    }

                    InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Recria");

                    existe = 0;
                    existe = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.DataAlojamento == dataAloj
                            && d.Linhagem == linhagem
                            && d.Lote == numLote
                            && d.Tipo == "Produção")
                        .Count();

                    operacao = "Inclusão";
                    if (existe > 0)
                    {
                        var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                            .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                                && d.DataAlojamento == dataAloj
                                && d.Linhagem == linhagem
                                && d.Lote == numLote
                                && d.Tipo == "Produção")
                            .ToList();

                        foreach (var item in listaExiste)
                        {
                            bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                        }

                        operacao = "Substituição dos Dados";
                        bdHLBAPP.SaveChanges();
                        existe = 0;
                    }

                    // Varre as planilhas de Produção existentes
                    var listaPlanilhas = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().ToList();

                    int numAvesProducao = 0;
                    string linhagemProducao = "";
                    DateTime dataInicial;
                    DateTime dataAtual = DateTime.Now;
                    int numeroAvesMortasAnterior = 0;
                    int numeroAvesMortasAtual = 0;

                    // Carregar Observações da Produção
                    relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                        .Where(s => s.Name == "Observação")
                                                        .First().Id;

                    WorksheetPart worksheetPartObs = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                    SheetData sheetDataObs = worksheetPartObs.Worksheet.GetFirstChild<SheetData>();

                    var listaLinhasObs = sheetDataObs.Descendants<Row>().ToList();

                    List<Row> listaLinhasDiario = null;

                    foreach (var item in listaPlanilhas)
                    {
                        if (item.Name.Value.Equals("Diário"))
                        {
                            WorksheetPart worksheetPartPrdDiario = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(item.Id);

                            SheetData sheetDataPrdDiario = worksheetPartPrdDiario.Worksheet.GetFirstChild<SheetData>();

                            listaLinhasDiario = sheetDataPrdDiario.Descendants<Row>().ToList();

                            // Linhagem na Produção
                            Row linhaLinhagemProducao = sheetDataPrdDiario.Elements<Row>().Where(r => r.RowIndex == 2).First();
                            Cell celulaLinhagemProducao = linhaLinhagemProducao.Elements<Cell>().Where(c => c.CellReference == "D2").First();
                            linhagemProducao = FormulaPPCPController.FromExcelTextBollean(celulaLinhagemProducao, spreadsheetDocument.WorkbookPart);

                            // Nº de Aves na Produção
                            Row linhaNumAvesProducao = sheetDataPrdDiario.Elements<Row>().Where(r => r.RowIndex == 4).First();
                            Cell celulaNumAvesProducao = linhaNumAvesProducao.Elements<Cell>().Where(c => c.CellReference == "C4").First();
                            numAvesProducao = Convert.ToInt32(celulaNumAvesProducao.Descendants<CellValue>().First().Text);

                            // Data Inicial
                            Row linhaDataInicial = sheetDataPrdDiario.Elements<Row>().Where(r => r.RowIndex == 2).First();
                            Cell celulaDataInicial = linhaDataInicial.Elements<Cell>().Where(c => c.CellReference == "C2").First();
                            dataInicial = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataInicial.Descendants<CellValue>().First().Text));
                            dataAtual = dataInicial;
                        }

                        if (item.Name.Value.Equals("Semanal"))
                        {
                            WorksheetPart worksheetPartPrd = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(item.Id);

                            SheetData sheetDataPrd = worksheetPartPrd.Worksheet.GetFirstChild<SheetData>();

                            var listaLinhasPrd = sheetDataPrd.Descendants<Row>().ToList();

                            DateTime semanaAnterior = DateTime.Now;

                            inventarioAves = 0;
                            inventarioAves = numAvesProducao;

                            // Navega nas linhas da Planilha de Produção
                            foreach (var linha in listaLinhasPrd)
                            {
                                if (linha.RowIndex >= 3)
                                {
                                    if ((linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                            .Count() > 0))
                                    {
                                        if ((linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                                .First().InnerText != ""))
                                        {
                                            Dados_Assistencia_Tecnica producao = new Dados_Assistencia_Tecnica();

                                            producao.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                            producao.Usuario = usuario;

                                            producao.Empresa = Session["empresaLayout"].ToString();
                                            producao.Tipo = "Produção";
                                            producao.Lote = numLote;
                                            producao.CodigoCliente = codigoCliente;
                                            producao.NomeCliente = cliente;
                                            producao.Granja = granja;
                                            producao.SaldoInicialAvesAlojadas = numAvesProducao;
                                            producao.DataAlojamento = dataAloj;
                                            producao.Galpao = galpao;
                                            producao.Linhagem = linhagemProducao;
                                            producao.Semana = dataAtual;
                                            producao.Idade = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "A" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);

                                            producao.InventarioAves = inventarioAves;

                                            numeroAvesMortasAtual = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                            if (numeroAvesMortasAnterior.Equals(0))
                                                producao.NumeroAvesMortas = 0;
                                            else
                                            {
                                                producao.NumeroAvesMortas = numeroAvesMortasAnterior - numeroAvesMortasAtual;
                                                inventarioAves = inventarioAves - (int)producao.NumeroAvesMortas;
                                            }

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.PercViabilidadeStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "D" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                }
                                                else
                                                {
                                                    producao.PercViabilidadeStd = 0;
                                                }
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.QtdeOvosProduzidos = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                                }
                                                else
                                                {
                                                    producao.QtdeOvosProduzidos = 0;
                                                }
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.PercOvosProduzidoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "H" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                }
                                                else
                                                {
                                                    producao.PercOvosProduzidoStd = 0;
                                                }
                                            }

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    if (linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text != "#DIV/0!")
                                                    {
                                                        decimal percTrincados = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                        producao.OvosTrincados = Convert.ToInt32(producao.QtdeOvosProduzidos * (percTrincados / 100));
                                                    }
                                                    else
                                                        producao.OvosTrincados = 0;
                                                }
                                                else
                                                {
                                                    producao.OvosTrincados = 0;
                                                }
                                            }

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "L" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "L" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.OvosPorAveAlojStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                }
                                                else
                                                {
                                                    producao.OvosPorAveAlojStd = 0;
                                                }
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "T" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "T" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.PesoOvoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "T" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                }
                                                else
                                                {
                                                    producao.PesoOvoStd = 0;
                                                }
                                            }
                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "S" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "S" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.PesoOvo = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "S" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                }
                                                else
                                                {
                                                    producao.PesoOvo = 0;
                                                }
                                            }

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "M" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                }
                                                else
                                                {
                                                    producao.ComsumoSemanal = 0;
                                                }
                                            }

                                            if (linha.Elements<Cell>()
                                                    .Where(c => c.CellReference.Value == "P" + linha.RowIndex)
                                                    .Count() > 0)
                                            {
                                                if (linha.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "P" + linha.RowIndex)
                                                        .First().InnerText != "")
                                                {
                                                    producao.ConsumoRacaoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                                }
                                                else
                                                {
                                                    producao.ConsumoRacaoStd = 0;
                                                }
                                            }

                                            string obsProducao = "";
                                            foreach (var obs in listaLinhasObs)
                                            {
                                                DateTime dataObs;

                                                if (obs.RowIndex >= 2)
                                                {
                                                    if (obs.Elements<Cell>()
                                                        .Where(c => c.CellReference.Value == "A" + obs.RowIndex)
                                                        .Count() > 0)
                                                    {
                                                        if (obs.Elements<Cell>()
                                                            .Where(c => c.CellReference.Value == "A" + obs.RowIndex)
                                                            .First().InnerText != "")
                                                        {
                                                            dataObs = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(obs.Elements<Cell>()
                                                               .Where(c => c.CellReference.Value == "A" + obs.RowIndex)
                                                               .First().Descendants<CellValue>().First().Text));

                                                            if ((dataObs < producao.Semana)
                                                                &&
                                                               (dataObs >= semanaAnterior))
                                                            {
                                                                string diagnostico = "";
                                                                string tratamento = "";

                                                                if (obs.Elements<Cell>()
                                                                        .Where(c => c.CellReference.Value == "B" + obs.RowIndex)
                                                                        .Count() > 0)
                                                                    diagnostico = FormulaPPCPController.FromExcelTextBollean(obs.Elements<Cell>().Where(c => c.CellReference == "B" + obs.RowIndex).First(), spreadsheetDocument.WorkbookPart);
                                                                if (obs.Elements<Cell>()
                                                                        .Where(c => c.CellReference.Value == "C" + obs.RowIndex)
                                                                        .Count() > 0)
                                                                    tratamento = FormulaPPCPController.FromExcelTextBollean(obs.Elements<Cell>().Where(c => c.CellReference == "C" + obs.RowIndex).First(), spreadsheetDocument.WorkbookPart);

                                                                string dataInicioStr = "";
                                                                string dataFimStr = "";

                                                                if (obs.Elements<Cell>()
                                                                    .Where(c => c.CellReference.Value == "D" + obs.RowIndex)
                                                                    .Count() > 0)
                                                                {
                                                                    if (obs.Elements<Cell>()
                                                                        .Where(c => c.CellReference.Value == "D" + obs.RowIndex)
                                                                        .First().InnerText != "")
                                                                    {
                                                                        DateTime dataInicio = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(obs.Elements<Cell>()
                                                                            .Where(c => c.CellReference.Value == "D" + obs.RowIndex)
                                                                            .First().Descendants<CellValue>().First().Text));

                                                                        dataInicioStr = dataInicio.ToShortDateString();
                                                                    }
                                                                }

                                                                if (obs.Elements<Cell>()
                                                                        .Where(c => c.CellReference.Value == "E" + obs.RowIndex)
                                                                        .Count() > 0)
                                                                {
                                                                    if (obs.Elements<Cell>()
                                                                            .Where(c => c.CellReference.Value == "E" + obs.RowIndex)
                                                                            .First().InnerText != "")
                                                                    {
                                                                        DateTime dataFim = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(obs.Elements<Cell>()
                                                                            .Where(c => c.CellReference.Value == "E" + obs.RowIndex)
                                                                            .First().Descendants<CellValue>().First().Text));

                                                                        dataFimStr = dataFim.ToShortDateString();
                                                                    }
                                                                }

                                                                obsProducao = "Data da Observação: " + dataObs.ToShortDateString() + (char)10 + (char)13;
                                                                if (!diagnostico.Equals(""))
                                                                    obsProducao = obsProducao + "Diagnóstico: " + diagnostico + (char)10 + (char)13;
                                                                if (!tratamento.Equals(""))
                                                                    obsProducao = obsProducao + "Tratamento: " + tratamento + (char)10 + (char)13;
                                                                if ((!dataInicioStr.Equals("")) && (!dataFimStr.Equals("")))
                                                                    obsProducao = obsProducao + "Período: " + dataInicioStr + " à " + dataFimStr;
                                                                else if ((!dataInicioStr.Equals("")) && (dataFimStr.Equals("")))
                                                                    obsProducao = obsProducao + "Início: " + dataInicioStr;

                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            producao.Observacao = obsProducao;

                                            semanaAnterior = (DateTime)producao.Semana;

                                            bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(producao);

                                            dataAtual = dataAtual.AddDays(7);
                                            numeroAvesMortasAnterior = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //    

                    InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Produção");

                    arquivo.Close();

                    bdHLBAPP.SaveChanges();
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";

                    return "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                }

                return "";
                //return View("Index", "");
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                return "Erro ao realizar a importação: " + e.Message;
                //return View("Index", "");
            }
        }

        public string ModeloErnestoRaigoAsaumi(Stream arquivo, string codigoCliente)
        {
            int erro = 0;
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";
                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                /**** Planilha de Dados da Produção ****/

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Geral")
                                                    .First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var listaLinhasProducao = sheetData.Descendants<Row>().ToList();

                /** Pega os Dados do Cabeçalho **/

                // Cliente
                Row linhaCliente = sheetData.Elements<Row>().Where(r => r.RowIndex == 2).First();
                Cell celulaCliente = linhaCliente.Elements<Cell>().Where(c => c.CellReference == "E2").First();
                string cliente = FormulaPPCPController.FromExcelTextBollean(celulaCliente, spreadsheetDocument.WorkbookPart);
                //string codigoCliente = "";

                // Granja
                string granja = Request.Files[0].FileName;

                // Nº de Aves
                Row linhaNumAves = sheetData.Elements<Row>().Where(r => r.RowIndex == 2).First();
                Cell celulaNumAves = linhaNumAves.Elements<Cell>().Where(c => c.CellReference == "C2").First();
                int numAves = Convert.ToInt32(celulaNumAves.Descendants<CellValue>().First().Text);

                // Data de Alojamento
                Row linhaDataAloj = sheetData.Elements<Row>().Where(r => r.RowIndex == 6).First();
                Cell celulaDataAloj = linhaDataAloj.Elements<Cell>().Where(c => c.CellReference == "C6").First();
                DateTime dataAloj = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataAloj.Descendants<CellValue>().First().Text));

                // Linhagem
                Row linhaLinhagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 1).First();
                Cell celulaLinhagem = linhaLinhagem.Elements<Cell>().Where(c => c.CellReference == "C1").First();
                string linhagem = FormulaPPCPController.FromExcelTextBollean(celulaLinhagem, spreadsheetDocument.WorkbookPart);

                string empresaLayout = Session["empresaLayout"].ToString();

                int existe = 0;
                existe = bdHLBAPP.Dados_Assistencia_Tecnica
                    .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                        && d.DataAlojamento == dataAloj
                        && d.Linhagem == linhagem
                        && d.Lote == ""
                        && d.Tipo == "Produção")
                    .Count();

                string operacao = "Inclusão";
                if (existe > 0)
                {
                    var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.DataAlojamento == dataAloj
                            && d.Linhagem == linhagem
                            && d.Lote == ""
                            && d.Tipo == "Produção")
                        .ToList();

                    foreach (var item in listaExiste)
                    {
                        bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                    }

                    operacao = "Substituição dos Dados";
                    bdHLBAPP.SaveChanges();
                    existe = 0;
                }

                if (existe == 0)
                {
                    int inventarioAves = numAves;

                    // Navega nas linhas da Planilha de Produção
                    foreach (var linha in listaLinhasProducao)
                    {
                        if (linha.RowIndex >= 12)
                        //if (linha.RowIndex == 461)
                        {
                            erro = (int)linha.RowIndex.Value;
                            if ((linha.Elements<Cell>()
                                    .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                    .First().Descendants<CellValue>().Count() > 0))
                            {
                                if ((linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                        .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != ""))
                                {
                                    Dados_Assistencia_Tecnica producao = new Dados_Assistencia_Tecnica();

                                    producao.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                    producao.Usuario = usuario;

                                    producao.Empresa = Session["empresaLayout"].ToString();
                                    producao.Tipo = "Produção";
                                    producao.Lote = "";
                                    producao.CodigoCliente = codigoCliente;
                                    producao.NomeCliente = cliente;
                                    producao.Granja = granja;
                                    producao.SaldoInicialAvesAlojadas = numAves;
                                    producao.DataAlojamento = dataAloj;
                                    producao.Galpao = "";
                                    producao.Linhagem = linhagem;
                                    producao.Semana = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "A" + linha.RowIndex).First().Descendants<CellValue>().First().Text));
                                    producao.Idade = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);

                                    producao.InventarioAves = inventarioAves;

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            producao.QtdeOvosProduzidos = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "D" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                        }
                                        else
                                        {
                                            producao.QtdeOvosProduzidos = 0;
                                        }
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            producao.NumeroAvesMortas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                            inventarioAves = inventarioAves - (int)producao.NumeroAvesMortas;
                                        }
                                        else
                                        {
                                            producao.NumeroAvesMortas = 0;
                                        }
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            producao.ConsumoAgua = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "G" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                        }
                                        else
                                        {
                                            producao.ConsumoAgua = 0;
                                        }
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            producao.PercOvosProduzidoStd = (Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "H" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")))
                                                / producao.SaldoInicialAvesAlojadas) * 100;
                                        }
                                        else
                                        {
                                            producao.PercOvosProduzidoStd = 0;
                                        }
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            producao.PercViabilidadeStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                        }
                                        else
                                        {
                                            producao.PercViabilidadeStd = 0;
                                        }
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "N" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "N" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            producao.MortalidadeStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "N" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                        }
                                        else
                                        {
                                            producao.MortalidadeStd = 0;
                                        }
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "P" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "P" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            //string teste = linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim();
                                            producao.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                        }
                                        else
                                        {
                                            producao.ComsumoSemanal = 0;
                                        }
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            producao.ConsumoRacaoStd = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "Q" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                        }
                                        else
                                        {
                                            producao.ConsumoRacaoStd = 0;
                                        }
                                    }

                                    bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(producao);
                                }
                            }
                        }
                    }
                    // 

                    InsereLOG(codigoCliente, linhagem, "", dataAloj, usuario, operacao, "Produção");

                    arquivo.Close();

                    bdHLBAPP.SaveChanges();
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Lote do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                    return "Lote do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                }

                //return View("Index", "");
                return "";
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "Linha " + erro.ToString();
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                //return View("Index", "");
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        public string ModeloEggCellCrescimentoAtual(Stream arquivo, string codigoCliente)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                /**** Caso seja tenha Recria, iremos inserir a Recria. ****/

                if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Entrada Semanal")
                                                    .Count() > 0)
                {
                    string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                        .Where(s => s.Name == "Entrada Semanal")
                                                        .First().Id;

                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    var listaLinhasRecria = sheetData.Descendants<Row>().ToList();

                    /** Pega os Dados do Cabeçalho **/

                    // Nº do Lote
                    Row linhaLote = sheetData.Elements<Row>().Where(r => r.RowIndex == 5).First();
                    Cell celulaLote = linhaLote.Elements<Cell>().Where(c => c.CellReference == "E5").First();
                    string numLote = FormulaPPCPController.FromExcelTextBollean(celulaLote, spreadsheetDocument.WorkbookPart);
                    //string numLote = celulaLote.Descendants<CellValue>().First().Text;

                    // Cliente
                    Row linhaCliente = sheetData.Elements<Row>().Where(r => r.RowIndex == 4).First();
                    Cell celulaCliente = linhaCliente.Elements<Cell>().Where(c => c.CellReference == "E4").First();
                    string cliente = FormulaPPCPController.FromExcelTextBollean(celulaCliente, spreadsheetDocument.WorkbookPart);
                    //string cliente = celulaCliente.Descendants<CellValue>().First().Text;

                    int existe = 0;
                    //string codigoCliente = "";

                    /*existe = bdApolo.ENTIDADE
                        .Where(e => e.EntNome.Contains(cliente))
                        .Count();

                    if (existe > 0)
                    {
                        ENTIDADE entidade = bdApolo.ENTIDADE
                            .Where(e => e.EntNome.Contains(cliente))
                            .First();

                        codigoCliente = entidade.EntCod;
                    }
                    else
                        codigoCliente = "";*/

                    // Nº de Aves
                    Row linhaNumAves = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                    Cell celulaNumAves = linhaNumAves.Elements<Cell>().Where(c => c.CellReference == "E7").First();
                    int numAves = Convert.ToInt32(celulaNumAves.Descendants<CellValue>().First().Text);

                    // Data de Alojamento
                    Row linhaDataAloj = sheetData.Elements<Row>().Where(r => r.RowIndex == 6).First();
                    Cell celulaDataAloj = linhaDataAloj.Elements<Cell>().Where(c => c.CellReference == "E6").First();
                    DateTime dataAloj = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataAloj.Descendants<CellValue>().First().Text));

                    // Linhagem
                    Row linhaLinhagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                    Cell celulaLinhagem = linhaLinhagem.Elements<Cell>().Where(c => c.CellReference == "E8").First();
                    string linhagem = FormulaPPCPController.FromExcelTextBollean(celulaLinhagem, spreadsheetDocument.WorkbookPart);
                    //string linhagem = celulaLinhagem.Descendants<CellValue>().First().Text;

                    string empresaLayout = Session["empresaLayout"].ToString();

                    existe = 0;
                    existe = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.Linhagem == linhagem 
                            && d.Lote == numLote
                            && d.DataAlojamento == dataAloj //&& d.Idade != 1 
                            && d.Tipo == "Recria")
                        .Count();

                    string operacao = "Inclusão";
                    if (existe > 0)
                    {
                        var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                            .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                                && d.Linhagem == linhagem 
                                && d.Lote == numLote
                                && d.DataAlojamento == dataAloj //&& d.Idade != 1 
                                && d.Tipo == "Recria")
                            .ToList();

                        foreach (var item in listaExiste)
                        {
                            bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                        }

                        operacao = "Substituição dos Dados";
                        bdHLBAPP.SaveChanges();
                        existe = 0;
                    }

                    if (existe == 0)
                    {
                        string galpao = numLote;

                        int inventarioAves = numAves;

                        // Navega nas linhas da Planilha de Recria
                        foreach (var linha in listaLinhasRecria)
                        {
                            if ((linha.RowIndex >= 12) && (linha.RowIndex <= 28))
                            {
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    Dados_Assistencia_Tecnica recria = new Dados_Assistencia_Tecnica();

                                    recria.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                    recria.Usuario = usuario;

                                    recria.Empresa = Session["empresaLayout"].ToString();
                                    recria.Tipo = "Recria";
                                    recria.Lote = numLote;
                                    recria.CodigoCliente = codigoCliente;
                                    recria.NomeCliente = cliente;
                                    recria.Granja = cliente;
                                    recria.SaldoInicialAvesAlojadas = numAves;
                                    recria.DataAlojamento = dataAloj;
                                    recria.Galpao = galpao;
                                    recria.Linhagem = linhagem;
                                    recria.InventarioAves = inventarioAves;
                                    recria.Semana = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));
                                    recria.Idade = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First().Descendants<CellValue>().First().Text);

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.NumeroAvesMortas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                        inventarioAves = inventarioAves - (int)recria.NumeroAvesMortas;
                                    }
                                    else
                                    {
                                        recria.NumeroAvesMortas = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.PesoAve = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "F" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.PesoAve = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "G" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.ComsumoSemanal = 0;
                                    }

                                    bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(recria);
                                }
                            }
                        }

                        InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Recria");

                        arquivo.Close();

                        bdHLBAPP.SaveChanges();
                    }
                    else
                    {
                        ViewBag.fileName = "";
                        ViewBag.erro = "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                    }
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                    return "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                }

                //return View("Index", "");
                return "";
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                //return View("Index", "");
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        public string ModeloEggCellCrescimentoNovo(Stream arquivo, string codigoCliente)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                /**** Caso seja tenha Recria, iremos inserir a Recria. ****/

                if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Weekly Input" || s.Name == "Semanal"
                                                        || s.Name == "Entrada Semanal")
                                                    .Count() > 0)
                {
                    string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                        .Where(s => s.Name == "Weekly Input" || s.Name == "Semanal"
                                                            || s.Name == "Entrada Semanal")
                                                        .First().Id;

                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    var listaLinhasRecria = sheetData.Descendants<Row>().ToList();

                    /** Pega os Dados do Cabeçalho **/

                    // Nº do Lote
                    Row linhaLote = sheetData.Elements<Row>().Where(r => r.RowIndex == 5).First();
                    Cell celulaLote = linhaLote.Elements<Cell>().Where(c => c.CellReference == "E5").First();
                    string numLote = FormulaPPCPController.FromExcelTextBollean(celulaLote, spreadsheetDocument.WorkbookPart);
                    //string numLote = celulaLote.Descendants<CellValue>().First().Text;

                    // Cliente
                    Row linhaCliente = sheetData.Elements<Row>().Where(r => r.RowIndex == 4).First();
                    Cell celulaCliente = linhaCliente.Elements<Cell>().Where(c => c.CellReference == "E4").First();
                    string cliente = FormulaPPCPController.FromExcelTextBollean(celulaCliente, spreadsheetDocument.WorkbookPart);
                    //string cliente = celulaCliente.Descendants<CellValue>().First().Text;

                    int existe = 0;
                    //string codigoCliente = "";

                    /*existe = bdApolo.ENTIDADE
                        .Where(e => e.EntNome.Contains(cliente))
                        .Count();

                    if (existe > 0)
                    {
                        ENTIDADE entidade = bdApolo.ENTIDADE
                            .Where(e => e.EntNome.Contains(cliente))
                            .First();

                        codigoCliente = entidade.EntCod;
                    }
                    else
                        codigoCliente = "";*/

                    // Nº de Aves
                    Row linhaNumAves = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                    Cell celulaNumAves = linhaNumAves.Elements<Cell>().Where(c => c.CellReference == "E7").First();
                    int numAves = Convert.ToInt32(celulaNumAves.Descendants<CellValue>().First().Text);

                    // Data de Alojamento
                    Row linhaDataAloj = sheetData.Elements<Row>().Where(r => r.RowIndex == 6).First();
                    Cell celulaDataAloj = linhaDataAloj.Elements<Cell>().Where(c => c.CellReference == "E6").First();
                    DateTime dataAloj = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataAloj.Descendants<CellValue>().First().Text));

                    // Linhagem
                    Row linhaLinhagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                    Cell celulaLinhagem = linhaLinhagem.Elements<Cell>().Where(c => c.CellReference == "E8").First();
                    string linhagem = FormulaPPCPController.FromExcelTextBollean(celulaLinhagem, spreadsheetDocument.WorkbookPart);
                    //string linhagem = celulaLinhagem.Descendants<CellValue>().First().Text;

                    string empresaLayout = Session["empresaLayout"].ToString();

                    existe = 0;
                    existe = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.Linhagem == linhagem 
                            && d.Lote == numLote
                            && d.DataAlojamento == dataAloj //&& d.Idade != 1 
                            && d.Tipo == "Recria")
                        .Count();

                    string operacao = "Inclusão";
                    if (existe > 0)
                    {
                        var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                            .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                                && d.Linhagem == linhagem 
                                && d.Lote == numLote
                                && d.DataAlojamento == dataAloj //&& d.Idade != 1 
                                && d.Tipo == "Recria")
                            .ToList();

                        foreach (var item in listaExiste)
                        {
                            bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                        }

                        operacao = "Substituição dos Dados";
                        bdHLBAPP.SaveChanges();
                        existe = 0;
                    }

                    if (existe == 0)
                    {
                        string galpao = numLote;

                        int inventarioAves = numAves;

                        // Navega nas linhas da Planilha de Recria
                        foreach (var linha in listaLinhasRecria)
                        {
                            if ((linha.RowIndex >= 12) && (linha.RowIndex <= 28))
                            {
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    Dados_Assistencia_Tecnica recria = new Dados_Assistencia_Tecnica();

                                    recria.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                    recria.Usuario = usuario;

                                    recria.Empresa = Session["empresaLayout"].ToString();
                                    recria.Tipo = "Recria";
                                    recria.Lote = numLote.Trim();
                                    recria.CodigoCliente = codigoCliente.Trim();
                                    recria.NomeCliente = cliente.Trim();
                                    recria.Granja = cliente.Trim();
                                    recria.SaldoInicialAvesAlojadas = numAves;
                                    recria.DataAlojamento = dataAloj;
                                    recria.Galpao = galpao.Trim();
                                    recria.Linhagem = linhagem.Trim();
                                    recria.InventarioAves = inventarioAves;
                                    recria.Semana = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));
                                    recria.Idade = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First().Descendants<CellValue>().First().Text);

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.NumeroAvesMortas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                        inventarioAves = inventarioAves - (int)recria.NumeroAvesMortas;
                                    }
                                    else
                                    {
                                        recria.NumeroAvesMortas = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.PesoAve = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "F" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.PesoAve = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.Uniformidade = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "G" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.Uniformidade = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "I" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.ComsumoSemanal = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.CoeficienteVariacao = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "H" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.CoeficienteVariacao = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.HorasProgramaLuz = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.HorasProgramaLuz = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "K" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.TemperaturaMinima = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "K" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.TemperaturaMinima = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "L" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.TemperaturaMaxima = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.TemperaturaMaxima = 0;
                                    }

                                    bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(recria);
                                }
                            }
                        }

                        InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Recria");

                        arquivo.Close();

                        bdHLBAPP.SaveChanges();
                    }
                    else
                    {
                        ViewBag.fileName = "";
                        ViewBag.erro = "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                    }
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                    return "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                }

                //return View("Index", "");
                return "";
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                //return View("Index", "");
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        public string ModeloEggCellProducaoNovo(Stream arquivo, string codigoCliente)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Weekly Input" || s.Name == "Semanal"
                                                        || s.Name == "Entrada Semanal")
                                                    .Count() > 0)
                {
                    string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                        .Where(s => s.Name == "Weekly Input" || s.Name == "Semanal"
                                                            || s.Name == "Entrada Semanal")
                                                        .First().Id;

                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    var listaLinhas = sheetData.Descendants<Row>().ToList();

                    /** Pega os Dados do Cabeçalho **/

                    // Nº do Lote
                    Row linhaLote = sheetData.Elements<Row>().Where(r => r.RowIndex == 5).First();
                    Cell celulaLote = linhaLote.Elements<Cell>().Where(c => c.CellReference == "E5").First();
                    string numLote = FormulaPPCPController.FromExcelTextBollean(celulaLote, spreadsheetDocument.WorkbookPart);
                    //string numLote = celulaLote.Descendants<CellValue>().First().Text;

                    // Cliente
                    Row linhaCliente = sheetData.Elements<Row>().Where(r => r.RowIndex == 4).First();
                    Cell celulaCliente = linhaCliente.Elements<Cell>().Where(c => c.CellReference == "E4").First();
                    string cliente = FormulaPPCPController.FromExcelTextBollean(celulaCliente, spreadsheetDocument.WorkbookPart);
                    //string cliente = celulaCliente.Descendants<CellValue>().First().Text;

                    int existe = 0;
                    //string codigoCliente = "";

                    /*existe = bdApolo.ENTIDADE
                        .Where(e => e.EntNome.Contains(cliente))
                        .Count();

                    if (existe > 0)
                    {
                        ENTIDADE entidade = bdApolo.ENTIDADE
                            .Where(e => e.EntNome.Contains(cliente))
                            .First();

                        codigoCliente = entidade.EntCod;
                    }
                    else
                        codigoCliente = "";*/

                    // Nº de Aves
                    Row linhaNumAves = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                    Cell celulaNumAves = linhaNumAves.Elements<Cell>().Where(c => c.CellReference == "E7").First();
                    int numAves = Convert.ToInt32(celulaNumAves.Descendants<CellValue>().First().Text);

                    // Data de Alojamento
                    Row linhaDataAloj = sheetData.Elements<Row>().Where(r => r.RowIndex == 6).First();
                    Cell celulaDataAloj = linhaDataAloj.Elements<Cell>().Where(c => c.CellReference == "E6").First();
                    DateTime dataAloj = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataAloj.Descendants<CellValue>().First().Text));

                    // Linhagem
                    Row linhaLinhagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                    Cell celulaLinhagem = linhaLinhagem.Elements<Cell>().Where(c => c.CellReference == "E8").First();
                    string linhagem = FormulaPPCPController.FromExcelTextBollean(celulaLinhagem, spreadsheetDocument.WorkbookPart);
                    //string linhagem = celulaLinhagem.Descendants<CellValue>().First().Text;

                    string empresaLayout = Session["empresaLayout"].ToString();

                    existe = 0;
                    existe = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.Linhagem == linhagem 
                            && d.Lote == numLote
                            && d.DataAlojamento == dataAloj && d.Idade != 1 && d.Tipo == "Produção")
                        .Count();

                    string operacao = "Inclusão";
                    if (existe > 0)
                    {
                        var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                            .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                                && d.Linhagem == linhagem 
                                && d.Lote == numLote
                                && d.DataAlojamento == dataAloj && d.Idade != 1 && d.Tipo == "Produção")
                            .ToList();

                        foreach (var item in listaExiste)
                        {
                            bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                        }

                        operacao = "Substituição dos Dados";
                        bdHLBAPP.SaveChanges();
                        existe = 0;
                    }

                    if (existe == 0)
                    {
                        string galpao = numLote;

                        int inventarioAves = numAves;

                        string teste = "";

                        // Navega nas linhas da Planilha de Recria
                        foreach (var linha in listaLinhas)
                        {
                            if ((linha.RowIndex >= 12) && (linha.RowIndex <= 74))
                            {
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    Dados_Assistencia_Tecnica recria = new Dados_Assistencia_Tecnica();

                                    recria.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                    recria.Usuario = usuario;

                                    if (linha.RowIndex == 25)
                                        teste = linha.RowIndex.ToString();

                                    recria.Empresa = Session["empresaLayout"].ToString();
                                    recria.Tipo = "Produção";
                                    recria.Lote = numLote.Trim();
                                    recria.CodigoCliente = codigoCliente.Trim();
                                    recria.NomeCliente = cliente.Trim();
                                    recria.Granja = cliente.Trim();
                                    recria.SaldoInicialAvesAlojadas = numAves;
                                    recria.DataAlojamento = dataAloj;
                                    recria.Galpao = galpao.Trim();
                                    recria.Linhagem = linhagem.Trim();
                                    recria.InventarioAves = inventarioAves;
                                    recria.Semana = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));
                                    recria.Idade = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First().Descendants<CellValue>().First().Text);

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.AvesDescartadas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                    }
                                    else
                                    {
                                        recria.AvesDescartadas = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.NumeroAvesMortas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "F" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                        inventarioAves = inventarioAves - (int)recria.NumeroAvesMortas;
                                    }
                                    else
                                    {
                                        recria.NumeroAvesMortas = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.QtdeOvosProduzidos = Convert.ToInt32(
                                            Convert.ToDecimal(linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "G" + linha.RowIndex).First()
                                            .Descendants<CellValue>().FirstOrDefault().Text.Replace(".",",")));
                                    }
                                    else
                                    {
                                        recria.QtdeOvosProduzidos = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.OvosSegunda = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "H" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                    }
                                    else
                                    {
                                        recria.OvosSegunda = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.PesoAve = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.PesoAve = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "K" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.PesoOvo = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "K" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.PesoOvo = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "L" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.ConsumoAgua = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.ConsumoAgua = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "M" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.ComsumoSemanal = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "N" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.HorasProgramaLuz = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "N" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.HorasProgramaLuz = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "O" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.TemperaturaMinima = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "O" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.TemperaturaMinima = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "P" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.TemperaturaMaxima = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.TemperaturaMaxima = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.TipoComedouro = FormulaPPCPController.FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "Q" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart);
                                    }
                                    else
                                    {
                                        recria.TipoComedouro = "";
                                    }

                                    if (linha.Elements<Cell>()
                                           .Where(c => c.CellReference.Value == "R" + linha.RowIndex)
                                           .First().InnerText != "")
                                    {
                                        recria.TeorCalcio = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "R" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.TeorCalcio = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                           .Where(c => c.CellReference.Value == "S" + linha.RowIndex)
                                           .First().InnerText != "")
                                    {
                                        recria.TeorLisina = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "S" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.TeorLisina = 0;
                                    }

                                    bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(recria);
                                }
                            }
                        }

                        InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Produção");

                        arquivo.Close();

                        bdHLBAPP.SaveChanges();
                    }
                    else
                    {
                        ViewBag.fileName = "";
                        ViewBag.erro = "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                    }
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                    return "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                }

                //return View("Index", "");
                return "";
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                //return View("Index", "");
                return "Erro ao realizar a importação: " + ex.Message;
            }
        }

        public string ModeloEggCellProducaoAtual(Stream arquivo, string codigoCliente)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Entrada Semanal")
                                                    .Count() > 0)
                {
                    string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                        .Where(s => s.Name == "Entrada Semanal")
                                                        .First().Id;

                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    var listaLinhas = sheetData.Descendants<Row>().ToList();

                    /** Pega os Dados do Cabeçalho **/

                    // Nº do Lote
                    Row linhaLote = sheetData.Elements<Row>().Where(r => r.RowIndex == 5).First();
                    Cell celulaLote = linhaLote.Elements<Cell>().Where(c => c.CellReference == "E5").First();
                    string numLote = FormulaPPCPController.FromExcelTextBollean(celulaLote, spreadsheetDocument.WorkbookPart);
                    //string numLote = celulaLote.Descendants<CellValue>().First().Text;

                    // Cliente
                    Row linhaCliente = sheetData.Elements<Row>().Where(r => r.RowIndex == 4).First();
                    Cell celulaCliente = linhaCliente.Elements<Cell>().Where(c => c.CellReference == "E4").First();
                    string cliente = FormulaPPCPController.FromExcelTextBollean(celulaCliente, spreadsheetDocument.WorkbookPart);
                    //string cliente = celulaCliente.Descendants<CellValue>().First().Text;

                    int existe = 0;
                    //string codigoCliente = "";

                    /*existe = bdApolo.ENTIDADE
                        .Where(e => e.EntNome.Contains(cliente))
                        .Count();

                    if (existe > 0)
                    {
                        ENTIDADE entidade = bdApolo.ENTIDADE
                            .Where(e => e.EntNome.Contains(cliente))
                            .First();

                        codigoCliente = entidade.EntCod;
                    }
                    else
                        codigoCliente = "";*/

                    // Nº de Aves
                    Row linhaNumAves = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                    Cell celulaNumAves = linhaNumAves.Elements<Cell>().Where(c => c.CellReference == "E7").First();
                    int numAves = Convert.ToInt32(celulaNumAves.Descendants<CellValue>().First().Text);

                    // Data de Alojamento
                    Row linhaDataAloj = sheetData.Elements<Row>().Where(r => r.RowIndex == 6).First();
                    Cell celulaDataAloj = linhaDataAloj.Elements<Cell>().Where(c => c.CellReference == "E6").First();
                    DateTime dataAloj = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(celulaDataAloj.Descendants<CellValue>().First().Text));

                    // Linhagem
                    Row linhaLinhagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                    Cell celulaLinhagem = linhaLinhagem.Elements<Cell>().Where(c => c.CellReference == "E8").First();
                    string linhagem = FormulaPPCPController.FromExcelTextBollean(celulaLinhagem, spreadsheetDocument.WorkbookPart);
                    //string linhagem = celulaLinhagem.Descendants<CellValue>().First().Text;

                    string empresaLayout = Session["empresaLayout"].ToString();

                    existe = 0;
                    existe = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                            && d.Linhagem == linhagem 
                            && d.Lote == numLote
                            && d.DataAlojamento == dataAloj && d.Idade != 1 && d.Tipo == "Produção")
                        .Count();

                    string operacao = "Inclusão";
                    if (existe > 0)
                    {
                        var listaExiste = bdHLBAPP.Dados_Assistencia_Tecnica
                            .Where(d => d.CodigoCliente == codigoCliente && d.Empresa == empresaLayout
                                && d.Linhagem == linhagem 
                                && d.Lote == numLote
                                && d.DataAlojamento == dataAloj && d.Idade != 1 && d.Tipo == "Produção")
                            .ToList();

                        foreach (var item in listaExiste)
                        {
                            bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
                        }

                        operacao = "Substituição dos Dados";
                        bdHLBAPP.SaveChanges();
                        existe = 0;
                    }

                    if (existe == 0)
                    {
                        string galpao = numLote;

                        int inventarioAves = numAves;

                        // Navega nas linhas da Planilha de Recria
                        foreach (var linha in listaLinhas)
                        {
                            if ((linha.RowIndex >= 12) && (linha.RowIndex <= 74))
                            {
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    Dados_Assistencia_Tecnica recria = new Dados_Assistencia_Tecnica();

                                    recria.DataHoraImportacao = Convert.ToDateTime(Session["dataImportacao"]);
                                    recria.Usuario = usuario;

                                    recria.Empresa = Session["empresaLayout"].ToString();
                                    recria.Tipo = "Produção";
                                    recria.Lote = numLote.Trim();
                                    recria.CodigoCliente = codigoCliente.Trim();
                                    recria.NomeCliente = cliente.Trim();
                                    recria.Granja = cliente.Trim();
                                    recria.SaldoInicialAvesAlojadas = numAves;
                                    recria.DataAlojamento = dataAloj;
                                    recria.Galpao = galpao.Trim();
                                    recria.Linhagem = linhagem.Trim();
                                    recria.InventarioAves = inventarioAves;
                                    recria.Semana = FormulaPPCPController.FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                                            .First().Descendants<CellValue>().FirstOrDefault().Text));
                                    recria.Idade = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First().Descendants<CellValue>().First().Text);

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.AvesDescartadas = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                    }
                                    else
                                    {
                                        recria.AvesDescartadas = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.NumeroAvesMortas = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>().Where(c => c.CellReference == "F" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                        inventarioAves = inventarioAves - (int)recria.NumeroAvesMortas;
                                    }
                                    else
                                    {
                                        recria.NumeroAvesMortas = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.QtdeOvosProduzidos = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>().Where(c => c.CellReference == "G" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".",",")));
                                    }
                                    else
                                    {
                                        recria.QtdeOvosProduzidos = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.OvosPrimeira = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>().Where(c => c.CellReference == "H" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".",",")));
                                    }
                                    else
                                    {
                                        recria.OvosPrimeira = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.OvosSegunda = Convert.ToInt32(Convert.ToDecimal(linha.Elements<Cell>().Where(c => c.CellReference == "I" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.OvosSegunda = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.PesoAve = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.PesoAve = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "K" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.PesoOvo = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "K" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.PesoOvo = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "L" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.ConsumoAgua = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.ConsumoAgua = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                            .First().InnerText != "")
                                    {
                                        recria.ComsumoSemanal = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "M" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                    }
                                    else
                                    {
                                        recria.ComsumoSemanal = 0;
                                    }

                                    bdHLBAPP.Dados_Assistencia_Tecnica.AddObject(recria);
                                }
                            }
                        }

                        InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, operacao, "Produção");

                        arquivo.Close();

                        bdHLBAPP.SaveChanges();
                    }
                    else
                    {
                        ViewBag.fileName = "";
                        //ViewBag.erro = "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Verifique!";
                        ViewBag.erro = "Lote " + numLote + " do cliente " + cliente + " alojado em " + dataAloj.ToShortDateString() + " já importado! Deseja a exclusão destes dados?";
                        ViewBag.Substituicao = "Sim";
                        Session["codigoCliente"] = codigoCliente;
                        Session["linhagem"] = linhagem;
                        Session["numLote"] = numLote;
                        Session["dataAloj"] = dataAloj;
                    }
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                    return "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                }

                //return View("Index", "");
                return "";
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                //return View("Index", "");
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        public string VerificaFormatoArquivo(string caminho)
        {
            string formatoArquivo = Request.Files[0].ContentType;

            if (formatoArquivo.Equals("application/vnd.ms-excel"))
            {
                object oMissing = System.Reflection.Missing.Value;
                Excel.Application oExcel = new Excel.Application();

                oExcel.Visible = false;
                oExcel.DisplayAlerts = false;
                Excel.Workbooks oBooks = oExcel.Workbooks;
                Excel._Workbook oBook = null;
                oBook = oBooks.Open(caminho, false, oMissing,
                    oMissing, oMissing, oMissing, true, oMissing, oMissing,
                    //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                    oMissing, false, oMissing, oMissing, oMissing, oMissing);

                caminho = caminho + "x";

                if (System.IO.File.Exists(caminho))
                {
                    System.IO.File.Delete(caminho);
                }

                oBook.SaveAs(caminho, Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                    Excel.XlSaveConflictResolution.xlOtherSessionChanges, false, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                // Quit Excel and clean up.
                oBook.Close(true, oMissing, oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;
                oExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                //P.Kill();

                GC.Collect();
            }

            return caminho;
        }

        public int VerificaModeloArquivo(Stream arquivo)
        {
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

            /**** Modelo EggCell - Antigo ****/
            if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                .Where(s => s.Name == "Controle Recria")
                                                .Count() > 0)
            {
                return 1;
            }
            /**** Modelo Planilha - Granja Iana ****/
            else if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                .Where(s => s.Name == "Diário")
                                                .Count() > 0)
            {
                return 2;
            }
            /**** Modelo Planilha - Ernesto Raigo Asaumi ****/
            else if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                            .Where(s => s.Name == "Geral")
                                            .Count() > 0)
            {
                return 3;
            }
            /**** Modelo EggCell Crescimento - Atual ****/
            else if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                        .Where(s => s.Name == "Ração e Peso")
                                        .Count() > 0)
            {
                return 4;
            }
            /**** Modelo EggCell Crescimento - Novo ****/
            else if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                        .Where(s => s.Name == "Feed & Weight" || s.Name == "PC, Unif. %, CV%"
                                            || s.Name == "Peso Corporal, Uniformidad, CV")
                                        .Count() > 0)
            {
                return 5;
            }
            /**** Modelo EggCell Produção - Novo ****/
            else if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                        .Where(s => s.Name == "Feed" || s.Name == "Ração"
                                            || s.Name == "Nutrición")
                                        .Count() > 0)
            {
                return 6;
            }
            /**** Modelo EggCell Produção - Atual ****/
            else if (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                        .Where(s => s.Name == "Gráfico de Ração")
                                        .Count() > 0)
            {
                return 7;
            }
            else
            {
                return 0;
            }
        }

        #endregion
        
        #region Consulta de Clientes

        public ActionResult ListaClientesImportacao(string descricao, string Text)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View("Index", ListaClientes(descricao, Text));
        }

        public ActionResult ListaClientesRelatorioGeral(string descricao, string Text)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View("RelatorioGeral", ListaClientes(descricao, Text));
        }

        public List<Cliente> ListaClientes(string descricao, string Text)
        {
            string empresa = Session["empresaApolo"].ToString();

            AtualizaEstadoSelecionado(Text);
            Session["descricao"] = descricao;
            Session["estado"] = Text;

            var listaClientes = bdApolo.ENTIDADE
                .Where(e => bdApolo.ENT_CATEG.Any(c => c.EntCod == e.EntCod && (c.CategCodEstr == "01" || c.CategCodEstr == "01.01"))
                    && (e.EntNome.Contains(descricao) || e.EntNomeFant.Contains(descricao)) && e.StatEntCod != "05"
                    && bdApolo.VEND_ENT.Any(ve => ve.EntCod == e.EntCod &&
                        bdApolo.VENDEDOR.Any(v => v.VendCod == ve.VendCod && (v.USEREmpresa == empresa || empresa == "TODAS"))))
                //&& bdApolo.CIDADE.Any(cid => cid.CidCod == e.CidCod && cid.CidNomeComp == cidade && cid.UfSigla == estado))
                .Join(bdApolo.CIDADE.Where(cid => cid.UfSigla == Text || Text == "(Todos)"),
                    ecid => ecid.CidCod,
                    c => c.CidCod,
                    (ecid, c) => new { ENTIDADE = ecid, CIDADE = c })
                .OrderBy(o => o.ENTIDADE.EntNome)
                .Select(e2 => new
                {
                    e2.ENTIDADE.EntCod,
                    e2.ENTIDADE.EntNome,
                    e2.ENTIDADE.EntNomeFant,
                    e2.CIDADE.CidNomeComp,
                    e2.CIDADE.UfSigla
                })
                .ToList();

            List<Cliente> listaExibeClientes = new List<Cliente>();

            foreach (var item in listaClientes)
            {
                Cliente cliente = new Cliente();

                cliente.EntCod = item.EntCod;
                cliente.EntNome = item.EntNome;
                cliente.EntNomeFant = item.EntNomeFant;
                cliente.CidNomeComp = item.CidNomeComp;
                cliente.UfSigla = item.UfSigla;

                listaExibeClientes.Add(cliente);
            }

            if (listaClientes.Count == 0) listaExibeClientes = null;

            if (listaClientes.Count == 1) Session["marcado"] = listaClientes.FirstOrDefault().EntCod;

            Session["ListaClientes"] = listaExibeClientes;

            return listaExibeClientes;
        }

        public ActionResult ListaClientesImportacaoMultiplosArquivos(string descricao, string Text)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View("ImportMultiplesFiles", ListaClientes(descricao, Text));
        }

        #endregion

        #region Relatorio Geral

        public ActionResult RelatorioGeral()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaEmpresasVendedores(true);
            CarregaListaEstados();
            CarregaListaTiposRelatorioDadosAssitTecnica();
            Session["descricao"] = "";
            Session["estado"] = "";
            Session["marcado"] = "";
            Session["sTipoData"] = "";
            Session["sDataInicial"] = DateTime.Today.ToShortDateString();
            Session["sDataFinal"] = DateTime.Today.AddDays(1).ToShortDateString();
            List<Cliente> listaExibeClientes = null;
            Session["ListaClientes"] = listaExibeClientes;
            return View("RelatorioGeral", listaExibeClientes);
        }

        [HttpPost]
        public ActionResult DownloadRelatorioGeral(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            List<Cliente> listaExibeClientes = null;
            string tipoRelatorio = model["Text"];
            AtualizaTipoRelatorioSelecionado(tipoRelatorio);

            string destino = "";
            string pesquisa = "";

            string empresa = "";
            if (model["Empresa"] != null)
            {
                empresa = model["Empresa"].ToString();
            }
            else
            {
                empresa = Session["empresa"].ToString();
            }

            AtualizaDDL(empresa, (List<SelectListItem>)Session["ListaEmpresasRelComercial"]);

            if (tipoRelatorio.Equals("Produção"))
            {
                destino = "C:\\inetpub\\wwwroot\\Relatorios\\Rel_Geral_Assist_Tec_" + empresa + "_" + Session["login"].ToString() + Session.SessionID + ".xlsm";
                pesquisa = "*Rel_Geral_Assist_Tec_" + empresa + "_" + Session["login"].ToString() + "*.xlsm";
            }
            else
            {
                destino = "C:\\inetpub\\wwwroot\\Relatorios\\Rel_Geral_Assist_Tec_Recria_" + empresa + "_" + Session["login"].ToString() + Session.SessionID + ".xlsm";
                pesquisa = "*Rel_Geral_Assist_Tec_Recria_" + empresa + "_" + Session["login"].ToString() + "*.xlsm";
            }

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                listaExibeClientes = (List<Cliente>)Session["ListaClientes"];
                return View("RelatorioGeral", listaExibeClientes);
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                listaExibeClientes = (List<Cliente>)Session["ListaClientes"];
                return View("RelatorioGeral", listaExibeClientes);
            }

            if (model["tipoData"] != null)
                Session["sTipoData"] = model["tipoData"].ToString();
            else
            {
                ViewBag.erro = "Por favor, selecionar o Tipo de Data!";
                listaExibeClientes = (List<Cliente>)Session["ListaClientes"];
                return View("RelatorioGeral", listaExibeClientes);
            }

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            if (tipoRelatorio.Equals("Produção"))
            {
                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Rel_Geral_Assist_Tec_"
                    + empresa + ".xlsm", destino);
            }
            else
            {
                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Rel_Geral_Assist_Tec_Recria_"
                    + empresa + ".xlsm", destino);
            }

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

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Relatorio"];

            if (model["clienteSelecionado"] != null)
                worksheet.Cells[1, 1] = "'" + model["clienteSelecionado"].ToString();
            else
                worksheet.Cells[1, 1] = "";
            worksheet.Cells[1, 2] = Convert.ToDateTime(model["dataIni"].ToString());
            worksheet.Cells[1, 3] = Convert.ToDateTime(model["dataFim"].ToString());
            if (Session["estado"] != null)
            {
                if (Session["estado"].ToString() != "")
                {
                    worksheet.Cells[1, 4] = Session["estado"].ToString();
                }
                else
                {
                    worksheet.Cells[1, 4] = "(Todos)";
                }
            }
            else
            {
                worksheet.Cells[1, 4] = "(Todos)";
            }
            worksheet.Cells[1, 5] = model["tipoData"].ToString();

            RunMacro(oExcel, new Object[] { "AtualizaRelatorio" });
            
            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
            }

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

            return File(destino, "Download", "Relatorio_Geral_" + tipoRelatorio + ".xlsm");
        }

        #endregion
        
        #region Importa Dados Múltiplos Arquivos

        public ActionResult ImportMultiplesFiles()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaModelosDeArquivo();
            CarregaListaEstados();
            Session["descricao"] = "";
            Session["estado"] = "";
            Session["marcado"] = "";
            List<Cliente> listaExibeClientes = null;
            Session["ListaClientes"] = listaExibeClientes;

            string pesquisa = "*DadosAssistTecnica_" + Session["login"].ToString() + "*.xls*";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            return View("ImportMultiplesFiles", listaExibeClientes);
        }

        [HttpPost]
        public ActionResult Upload(string id, string data)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            //List<Cliente> listaExibeClientes = null;

            SequenciaLinha retorno2 = new SequenciaLinha();

            string codigoCliente = id;
            Session["marcado"] = codigoCliente;

            DateTime dataImportacao = DateTime.Now;
            //DateTime.TryParse(data, out dataImportacao);

            //if (dataImportacao == Convert.ToDateTime("01/01/0001"))
            //{
            //    retorno2.Importado = "Data de Importação Inválida!";
            //    retorno2.ID = 0;
            //    return View("Arquivo", retorno2);
            //}

            if (codigoCliente == null)
            {
                Session["marcado"] = "";
                retorno2.Importado = "Cliente não selecionado! É necessário primeiro selecionar o Cliente!";
                retorno2.ID = 0;
            }
            else
            {
                //string retorno = ImportaPlanilha(codigoCliente);
                HttpPostedFileBase filetmp = Request.Files["Filedata"];

                string retorno = ImportaPlanilha(codigoCliente, dataImportacao);

                if (retorno.Equals(""))
                {
                    retorno2.Importado = "Arquivo " + filetmp.FileName + " importado com sucesso!";
                    retorno2.ID = 1;
                }
                else
                {
                    retorno2.Importado = "Erro ao importar o arquivo " + filetmp.FileName + ": " + retorno;
                    retorno2.ID = 0;
                }
            }

            return View("Arquivo", retorno2);
        }

        #endregion

        #region Importa Loggers

        public ActionResult ImportLoggers()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            Session["sDataInicial"] = DateTime.Today.ToShortDateString();
            Session["sDataFinal"] = DateTime.Today.AddDays(1).ToShortDateString();
            List<Entrega> listaEntregas = null;
            Session["ListaEntregas"] = listaEntregas;
            return View("ImportLoggers", listaEntregas);
        }

        public List<VU_Lista_Entregas> ListaEntregas(DateTime dataInicial, DateTime dataFinal)
        {
            string empresa = Session["empresaApolo"].ToString();

            Session["sDataInicial"] = dataInicial;
            Session["sDataFinal"] = dataFinal;

            var listaEntregas = apolo.VU_Lista_Entregas
                .Where(e => e.Data_Nascimento >= dataInicial && e.Data_Nascimento <= dataFinal)
                .OrderBy(o => o.Data_Nascimento).ThenBy(o => o.Placa)
                .ToList();

            Session["ListaEntregas"] = listaEntregas;

            return listaEntregas;
        }

        public ActionResult ListaEntregasLoggers(string dataIni, string dataFim)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(dataIni);
            DateTime dataFinal = Convert.ToDateTime(dataFim);
            return View("ImportLoggers", ListaEntregas(dataInicial, dataFinal));
        }

        [HttpPost]
        public ActionResult ImportaLogger(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            List<VU_Lista_Entregas> listaEntregas = null;

            string entregaSelecionada = model["entregaSelecionada"];
            string numeroLogger = model["numeroLogger"];
            Session["marcado"] = entregaSelecionada;

            if (entregaSelecionada == null)
            {
                Session["marcado"] = "";
                ViewBag.erro = "Necessário selecionar uma Entrega primeiro!";
                listaEntregas = (List<VU_Lista_Entregas>)Session["ListaEntregas"];
                return View("ImportLoggers", listaEntregas);
            }

            if (numeroLogger == "")
            {
                ViewBag.erro = "Necessário informar o número do Logger!";
                listaEntregas = (List<VU_Lista_Entregas>)Session["ListaEntregas"];
                return View("ImportLoggers", listaEntregas);
            }

            string retorno = "";
            retorno = ImportaArquivoLogger(entregaSelecionada, numeroLogger);

            if (retorno.Equals(""))
            {
                ViewBag.erro = "";
                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";
            }
            else
            {
                ViewBag.erro = retorno;
                ViewBag.fileName = "";
            }

            listaEntregas = (List<VU_Lista_Entregas>)Session["ListaEntregas"];
            return View("ImportLoggers", listaEntregas);
        }

        public string ImportaArquivoLogger(string entregaSelecionada, string numeroLogger)
        {
            string caminho = @"C:\inetpub\wwwroot\Relatorios\Logger_" + Session["login"].ToString() + "_"
                + "_" + DateTime.Now.ToString("dd-MM-yyy")
                + "_" + DateTime.Now.ToString("mm-ss")
                + "_" + DateTime.Now.Millisecond
                + ".xlsx";

            Request.Files[0].SaveAs(caminho);
            caminho = VerificaFormatoArquivo(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            if (arquivo.Length > 0)
            {
                string retorno = "";
                retorno = ModeloLogger(arquivo, entregaSelecionada, numeroLogger);
                if (retorno.Equals("Modelo Azul"))
                {
                    retorno = ModeloLoggerAzul(arquivo, entregaSelecionada, numeroLogger);
                }
                return retorno;
            }
            else
            {
                return "Selecione um arquivo para ser importado!";
            }
        }

        public string ModeloLogger(Stream arquivo, string entregaSelecionada, string numeroLogger)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                DateTime dataEntrega = Convert.ToDateTime(entregaSelecionada.Substring(0,10));
                int lenPlaca = entregaSelecionada.Length - 20;
                string placa = entregaSelecionada.Substring(20, lenPlaca);

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";
                
                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Verifica Arquivo
                Row linhaVerificaArquivo = sheetData.Elements<Row>().Where(r => r.RowIndex == 1).First();
                Cell celulaVerificaArquivo = linhaVerificaArquivo.Elements<Cell>().Where(c => c.CellReference == "A1").First();
                string VerificaArquivo = FormulaPPCPController.FromExcelTextBollean(celulaVerificaArquivo, spreadsheetDocument.WorkbookPart).Trim();

                if (VerificaArquivo.Equals("Test Report"))
                {
                    var listaLinhas = sheetData.Descendants<Row>().ToList();

                    int existe = 0;
                    existe = bdHLBAPP.Dados_Loggers
                        .Where(l => l.NumeroLogger == numeroLogger && l.DataEntrega == dataEntrega
                            && l.PlacaVeiculo == placa)
                        .Count();

                    if (existe == 0)
                    {
                        // Navega nas linhas da Planilha de Recria
                        foreach (var linha in listaLinhas)
                        {
                            if (linha.RowIndex >= 12)
                            {
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    Dados_Loggers dadoLogger = new Dados_Loggers();

                                    dadoLogger.Usuario = usuario;
                                    dadoLogger.DataHoraImportacao = DateTime.Today;
                                    dadoLogger.DataEntrega = dataEntrega;
                                    dadoLogger.PlacaVeiculo = placa;
                                    dadoLogger.NumeroLogger = numeroLogger;

                                    string dataHoraLoggerCelula = 
                                        FormulaPPCPController.FromExcelTextBollean(
                                            linha.Elements<Cell>().Where(c => c.CellReference == "F" + linha.RowIndex).First(), 
                                            spreadsheetDocument.WorkbookPart).Trim();
                                    DateTime dataHoraLogger = 
                                        Convert.ToDateTime(dataHoraLoggerCelula.Substring(0,2) + "/" +
                                        dataHoraLoggerCelula.Substring(3,2) + "/" +
                                        dataHoraLoggerCelula.Substring(6,2) + " " +
                                        dataHoraLoggerCelula.Substring(9,8));
                                    dadoLogger.DataHoraLogger = dataHoraLogger;

                                    string temperaturaCelula = 
                                        FormulaPPCPController.FromExcelTextBollean(
                                            linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).First(), 
                                            spreadsheetDocument.WorkbookPart).Trim();
                                    decimal temperatura = Convert.ToDecimal(temperaturaCelula.Substring(0,4).Replace(".",","));
                                    dadoLogger.Temperatura = temperatura;

                                    string humidadeCelula = 
                                        FormulaPPCPController.FromExcelTextBollean(
                                            linha.Elements<Cell>().Where(c => c.CellReference == "D" + linha.RowIndex).First(), 
                                            spreadsheetDocument.WorkbookPart).Trim();
                                    decimal humidade = Convert.ToDecimal(humidadeCelula.Substring(0,4).Replace(".",","));
                                    dadoLogger.Umidade = humidade;
                                    
                                    bdHLBAPP.Dados_Loggers.AddObject(dadoLogger);
                                }
                            }
                        }

                        arquivo.Close();

                        bdHLBAPP.SaveChanges();
                    }
                    else
                    {
                        return "Logger  " + numeroLogger + " do caminhão de placa " + placa + " já inserido na Entrega do dia " + dataEntrega.ToShortDateString() + "!";
                    }
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                    if (VerificaArquivo.Equals("DATA LOGGER SamplingRate:60;"))
                    {
                        return "Modelo Azul";
                    }

                    return "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                }

                //return View("Index", "");
                return "";
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                //return View("Index", "");
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        public string ModeloLoggerAzul(Stream arquivo, string entregaSelecionada, string numeroLogger)
        {
            try
            {
                string usuario = Session["usuario"].ToString();

                DateTime dataEntrega = Convert.ToDateTime(entregaSelecionada.Substring(0, 10));
                string placa = entregaSelecionada.Substring(20, 8);

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Verifica Arquivo
                Row linhaVerificaArquivo = sheetData.Elements<Row>().Where(r => r.RowIndex == 1).First();
                Cell celulaVerificaArquivo = linhaVerificaArquivo.Elements<Cell>().Where(c => c.CellReference == "A1").First();
                string VerificaArquivo = FormulaPPCPController.FromExcelTextBollean(celulaVerificaArquivo, spreadsheetDocument.WorkbookPart).Trim();

                if (VerificaArquivo.Equals("DATA LOGGER SamplingRate:60;"))
                {
                    var listaLinhas = sheetData.Descendants<Row>().ToList();

                    int existe = 0;
                    existe = bdHLBAPP.Dados_Loggers
                        .Where(l => l.NumeroLogger == numeroLogger && l.DataEntrega == dataEntrega
                            && l.PlacaVeiculo == placa)
                        .Count();

                    if (existe == 0)
                    {
                        // Navega nas linhas da Planilha de Recria
                        foreach (var linha in listaLinhas)
                        {
                            if (linha.RowIndex >= 32)
                            {
                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                        .First().InnerText != "")
                                {
                                    Dados_Loggers dadoLogger = new Dados_Loggers();

                                    dadoLogger.Usuario = usuario;
                                    dadoLogger.DataHoraImportacao = DateTime.Now;
                                    dadoLogger.DataEntrega = dataEntrega;
                                    dadoLogger.PlacaVeiculo = placa;
                                    dadoLogger.NumeroLogger = numeroLogger;

                                    string dataLoggerCelula =
                                        FormulaPPCPController.FromExcelTextBollean(
                                            linha.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).First(),
                                            spreadsheetDocument.WorkbookPart).Trim();
                                    string horaLoggerCelula =
                                        FormulaPPCPController.FromExcelTextBollean(
                                            linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First(),
                                            spreadsheetDocument.WorkbookPart).Trim();
                                    DateTime dataHoraLogger =
                                        Convert.ToDateTime(dataLoggerCelula + " " + horaLoggerCelula);
                                    dadoLogger.DataHoraLogger = dataHoraLogger;

                                    string temperaturaCelula =
                                        FormulaPPCPController.FromExcelTextBollean(
                                            linha.Elements<Cell>().Where(c => c.CellReference == "E" + linha.RowIndex).First(),
                                            spreadsheetDocument.WorkbookPart).Trim();
                                    decimal temperatura = Convert.ToDecimal(temperaturaCelula.Replace(".", ","));
                                    dadoLogger.Temperatura = temperatura;

                                    string humidadeCelula =
                                        FormulaPPCPController.FromExcelTextBollean(
                                            linha.Elements<Cell>().Where(c => c.CellReference == "D" + linha.RowIndex).First(),
                                            spreadsheetDocument.WorkbookPart).Trim();
                                    decimal humidade = Convert.ToDecimal(humidadeCelula.Replace(".", ","));
                                    dadoLogger.Umidade = humidade;

                                    bdHLBAPP.Dados_Loggers.AddObject(dadoLogger);
                                }
                            }
                        }

                        arquivo.Close();

                        bdHLBAPP.SaveChanges();
                    }
                    else
                    {
                        return "Logger  " + numeroLogger + " do caminhão de placa " + placa + " já inserido na Entrega do dia " + dataEntrega.ToShortDateString() + "!";
                    }
                }
                else
                {
                    //ViewBag.fileName = "";
                    //ViewBag.erro = "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                    return "Arquivo " + Request.Files[0].FileName + " não é do modelo selecionado! Verifique!";
                }

                //return View("Index", "");
                return "";
            }
            catch (Exception e)
            {
                //ViewBag.fileName = "";
                //ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                //return View("Index", "");
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

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

        public void InsereLOG(string codigoCliente, string linhagem, string lote, DateTime dataAlojamento,
            string usuario, string operacao, string tipoPlanilha)
        {
            LOG_Importacao_Dados_Assist_Tec log = new LOG_Importacao_Dados_Assist_Tec();

            log.CodigoCliente = codigoCliente;
            log.Linhagem = linhagem;
            log.Lote = lote;
            log.DataAlojamento = dataAlojamento;
            log.Usuario = usuario;
            log.DataHoraImportacao = DateTime.Now;
            log.Operacao = operacao;
            log.TipoPlanilha = tipoPlanilha;

            bdHLBAPP.LOG_Importacao_Dados_Assist_Tec.AddObject(log);
        }

        public ActionResult ConfirmaSubstituicao()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string codigoCliente = Session["codigoCliente"].ToString();
            string linhagem = Session["linhagem"].ToString();
            string numLote = Session["numLote"].ToString();
            DateTime dataAloj = Convert.ToDateTime(Session["dataAloj"].ToString());
            string usuario = Session["usuario"].ToString();

            var listaImportado = bdHLBAPP.Dados_Assistencia_Tecnica
                        .Where(d => d.CodigoCliente == codigoCliente && d.Lote == numLote && d.DataAlojamento == dataAloj
                                && d.Tipo == "Produção")
                        .ToList();

            foreach (var item in listaImportado)
            {
                bdHLBAPP.Dados_Assistencia_Tecnica.DeleteObject(item);
            }

            InsereLOG(codigoCliente, linhagem, numLote, dataAloj, usuario, "Exclusão dos Dados", "Produção");

            ViewBag.fileName = "Dados excluídos com sucesso!";

            bdHLBAPP.SaveChanges();

            return View("Index", "");
        }

        public ActionResult CancelaSubstituicao()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View("Index", "");
        }

        #endregion

        #region SAC e RRC

        #region Carregamento de Listas

        public void CarregaEmpresasVendedores(bool todas)
        {
            List<SelectListItem> listaEmpresas = new List<SelectListItem>();

            if (todas)
            {
                if (Session["empresa"].ToString().Length > 2)
                {
                    listaEmpresas.Add(new SelectListItem
                    {
                        Text = "(Todas)",
                        Value = "(Todas)",
                        Selected = true
                    });
                }
            }

            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                listaEmpresas.Add(new SelectListItem
                {
                    Text = Session["empresa"].ToString().Substring(i, 2),
                    Value = Session["empresa"].ToString().Substring(i, 2),
                    Selected = false
                });
            }

            Session["ListaEmpresasRelComercial"] = listaEmpresas;
        }

        public void CarregaUnidadesMantiqueira()
        {
            List<SelectListItem> listaUnidades = new List<SelectListItem>();

            listaUnidades.Add(new SelectListItem
            {
                Text = "MG",
                Value = "MG",
                Selected = true
            });

            listaUnidades.Add(new SelectListItem
            {
                Text = "MT",
                Value = "MT",
                Selected = false
            });

            Session["ListaUnidadesMantiqueira"] = listaUnidades;
        }

        public void CarregaLinhagens()
        {
            List<SelectListItem> listaLinhagens = new List<SelectListItem>();

            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                string empresa = Session["empresa"].ToString().Substring(i, 2);
                DateTime dataAtual = DateTime.Today;

                var lista = bdHLBAPP.Tabela_Precos
                    .Where(w => w.Empresa == empresa && w.Tipo == "Faturamento"
                        && !w.Produto.Contains("Ovos")
                        && dataAtual >= w.DataInicial && dataAtual <= w.DataFinal)
                    .GroupBy(g => g.Produto)
                    .OrderBy(o => o.Key)
                    .ToList();

                foreach (var item in lista)
                {
                    listaLinhagens.Add(new SelectListItem
                    {
                        Text = item.Key,
                        Value = item.Key,
                        Selected = false
                    });   
                }
            }

            Session["ListaLinhagens"] = listaLinhagens;
        }

        public void CarregaTipoPeriodo()
        {
            List<SelectListItem> listaTipoPeriodo = new List<SelectListItem>();

            listaTipoPeriodo.Add(new SelectListItem
            {
                Text = "Diario",
                Value = "Diario",
                Selected = true
            });

            listaTipoPeriodo.Add(new SelectListItem
            {
                Text = "Semanal",
                Value = "Semanal",
                Selected = false
            });

            Session["ListaTipoPeriodo"] = listaTipoPeriodo;
        }

        public void CarregaTipoFase()
        {
            List<SelectListItem> listaTipoFase = new List<SelectListItem>();

            listaTipoFase.Add(new SelectListItem
            {
                Text = "Recria",
                Value = "Recria",
                Selected = true
            });

            listaTipoFase.Add(new SelectListItem
            {
                Text = "Producao",
                Value = "Producao",
                Selected = false
            });

            Session["ListaTipoFase"] = listaTipoFase;
        }

        public List<SelectListItem> AtualizaDDL(string text, List<SelectListItem> lista)
        {
            List<SelectListItem> listItens = lista;

            foreach (var item in listItens)
            {
                if (item.Value == text)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            return listItens;
        }

        #endregion

        #region Métodos Index

        public ActionResult SAC()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaEmpresasVendedores(true);

            return View("SAC");
        }

        public ActionResult RRC()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaEmpresasVendedores(true);

            Session["sTipoDataRRC"] = "";

            return View("RRC");
        }

        public ActionResult SACMantiqueira()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaUnidadesMantiqueira();

            return View();
        }

        public ActionResult AcompanhamentoLote()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            if (Session["nomeGranja"] == null) Session["nomeGranja"] = "";
            if (Session["lote"] == null) Session["lote"] = "";
            if (Session["dataNascimento"] == null) Session["dataNascimento"] = DateTime.Today.ToShortDateString();
            if (Session["qtdeFemeasAlojadas"] == null) Session["qtdeFemeasAlojadas"] = "";

            CarregaLinhagens();
            CarregaTipoPeriodo();
            CarregaTipoFase();

            return View();
        }

        #endregion

        #region Metódos Downloads

        [HttpPost]
        public ActionResult DownloadSAC(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string destino = "";
            string pesquisa = "";

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_SAC_" + Session["empresa"].ToString() + "_" + Session["login"].ToString() + Session.SessionID + ".xlsm";
            pesquisa = "*Relatorio_SAC_" + Session["empresa"].ToString() + "_" + Session["login"].ToString() + "*.xlsm";
            
            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("SAC");
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("SAC");
            }

            string empresa = "";

            if (model["Empresa"] != null)
            {
                empresa = model["Empresa"].ToString();
            }
            else
            {
                empresa = Session["empresaLayout"].ToString();
            }

            AtualizaDDL(empresa, (List<SelectListItem>)Session["ListaEmpresasRelComercial"]);

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            string empresaLayout = "";
            if (empresa == "(Todas)")
                empresaLayout = "BR";
            else
                empresaLayout = empresa;

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_SAC_" + empresaLayout + ".xlsm", destino);

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

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Dados"];

            worksheet.Cells[3, 3] = empresa;
            worksheet.Cells[4, 3] = Convert.ToDateTime(model["dataIni"].ToString());
            worksheet.Cells[5, 3] = Convert.ToDateTime(model["dataFim"].ToString());

            RunMacro(oExcel, new Object[] { "Atualizar_Dados" });

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            #region Dados CHIC

            DateTime dataInicial = Convert.ToDateTime(model["dataIni"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFim"].ToString());

            string commandTextCHICCabecalho =
                "select " +
                    "o.orderno `Nº Pedido`, " +
                //"(select SUM(b1.quantity) from booked b1 where b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc not like '%Extra%') `Qtde. Vendida`, " +
                //"(select SUM(b1.quantity) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('DO','DN','DV','HE') " +
                //    "and b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc like '%Extra%') `Qtde. Bonificada`, " +
                    "v.desc `Linhagem`, " +
                    "b.price `Valor Unit.`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "IIF(SUBSTR(i.form,1,1) = 'H', b.cal_date, b.cal_date+21) `Nascimento`, " +
                    "s.inv_comp `Empresa`, " +
                    "s.salesman `Ved. / Repres.`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "o.Status, " +
                    "IIF('VAXX' $ i.item_desc,1,0) `Vaxxitek`, " +
                    "(select COUNT(1) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('VC') " +
                        "and b1.orderno = o.orderno and i1.item_desc like '%HVT%') - IIF('VAXX' $ i.item_desc,1,0) `Marek`, " +
                    "(select COUNT(1) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('VC') " +
                        "and b1.orderno = o.orderno and i1.item_desc not like '%HVT%' and i1.item_desc like '%RISP%') - " +
                        "IIF('VAXX' $ i.item_desc,1,0) `Rispens`, " +
                    "(select COUNT(1) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('VC') " +
                        "and b1.orderno = o.orderno and i1.item_desc like '%BOU%') `Bouba`, " +
                    "(select COUNT(1) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('VC') " +
                        "and b1.orderno = o.orderno and i1.item_desc like '%COCCIDIOSE%') `Coccidiose`, " +
                    "(select COUNT(1) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('VC') " +
                        "and b1.orderno = o.orderno and i1.item_desc like '%LARINGO%') `Laringo`, " +
                    "(select COUNT(1) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('VC') " +
                        "and b1.orderno = o.orderno and i1.item_desc like '%SALMONELLA%') `Salmonella`, " +
                    "(select COUNT(1) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('SV') " +
                        "and b1.orderno = o.orderno and i1.item_desc like '%INFRAVER%') `Trat. Infraverm.`, " +
                    "(select max(trim(i1.item_desc)) from booked b1, items i1 where b1.item = i1.item_no and " +
                        "i1.form in ('CX') and b1.orderno = o.orderno and i1.item_no not in ('600','610')) `Embalagem`, " +
                    "(select max(b1.quantity) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('CX') " +
                        "and b1.orderno = o.orderno and i1.item_no in ('600','610')) `Pintos/Ovos p/ Caixa`, " +
                    "b.Location ";

            string commandTextCHICTabelas =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoins =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "b.customer = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltros =
                //"b.price > 0 and ";
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV','HE') and ";

            string dataInicialStr = dataInicial.ToString("MM/dd/yyyy");
            string dataFinalStr = dataFinal.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametros =
                    "0 < (select COUNT(1) from booked b1, items i1 " +
                            "where b1.orderno = o.orderno and b1.item = i1.item_no and " +
                //"b1.cal_date+21 between DATE()+60 and DATE()+240) ";
                            "IIF(SUBSTR(i1.form,1,1) = 'H', b1.cal_date, b1.cal_date+21) between {" + dataInicialStr + "} and {" + dataFinalStr + "}) and " +
                            "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') ";

            string commandTextCHICAgrupamento =
                "group by " +
                    "o.orderno, " +
                    "v.desc, " +
                    "b.price, " +
                    "o.delivery, " +
                    "b.cal_date, " +
                    "o.Status, " +
                    "i.item_desc, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "s.inv_comp, " +
                    "s.salesman, " +
                    "b.item, " +
                    "i.form, " +
                //"b.book_id, " +
                    "b.orderno, " +
                    "b.Location ";

            string commandTextCHICOrdenacao =
                "order by " +
                    "8, 9, 7";

            #endregion

            #region SQL Dados CHIC - Faturamento

            string commandTextCHICCabecalhoFaturamentoDados =
                "select * ";

            string commandTextCHICTabelasFaturamentoDados =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamentoDados =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados = "";

            string commandTextCHICCondicaoParametrosFaturamentoDados =
                    "b.cal_date between {" + dataInicialStr + "} and {" + dataFinalStr + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') ";

            string commandTextCHICAgrupamentoFaturamentoDados = "";

            string commandTextCHICOrdenacaoFaturamentoDados = " Union ";

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select * ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 = "";

            string dataInicialStrCalDate = dataInicial.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrCalDate = dataFinal.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "";

            #endregion

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("CHIC"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                else if (item.Name.Equals("CHIC_Faturamento_Dados"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamentoDados + commandTextCHICTabelasFaturamentoDados +
                        commandTextCHICCondicaoJoinsFaturamentoDados +
                        commandTextCHICCondicaoFiltrosFaturamentoDados + commandTextCHICCondicaoParametrosFaturamentoDados +
                        commandTextCHICAgrupamentoFaturamentoDados +
                        commandTextCHICOrdenacaoFaturamentoDados +
                        commandTextCHICCabecalhoFaturamentoDados02 + commandTextCHICTabelasFaturamentoDados02 +
                        commandTextCHICCondicaoJoinsFaturamentoDados02 +
                        commandTextCHICCondicaoFiltrosFaturamentoDados02 + commandTextCHICCondicaoParametrosFaturamentoDados02 +
                        commandTextCHICAgrupamentoFaturamentoDados02 +
                        commandTextCHICOrdenacaoFaturamentoDados02;
            }

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

            return File(destino, "Download", "Relatorio_SAC_" + empresa + "_" + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsm");
        }

        [HttpPost]
        public ActionResult DownloadSACMantiqueira(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string destino = "";
            string pesquisa = "";

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\SAC\\Relatorio_SAC_Mantiqueira_" + Session["empresa"].ToString() + "_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
            pesquisa = "*Relatorio_SAC_Mantiqueira_" + Session["empresa"].ToString() + "_" + Session["login"].ToString() + "*.xlsx";

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("SAC");
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("SAC");
            }

            string empresa = "";

            if (model["Empresa"] != null)
            {
                empresa = model["Empresa"].ToString();
            }

            AtualizaDDL(empresa, (List<SelectListItem>)Session["ListaUnidadesMantiqueira"]);

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\SAC", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\SAC\\Relatorio_SAC_Mantiqueira.xlsx", destino);

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

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            #region Dados

            DateTime dataInicial = Convert.ToDateTime(model["dataIni"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFim"].ToString());

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VW_Dados_SAC_MANTIQUEIRA ";

            string commandTextCHICCondicaoJoins = "";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "where [Primeiro Nascimento] >= '" + dataInicialStr +
                        "' and [Último Nascimento] <= '" + dataFinalStr + "' " +
                        " and UF = '" + empresa + "' ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "[Primeiro Nascimento]";

            #endregion

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if ((item.Name.Equals("srv-sql Apolo10")) || (item.Name.Equals("srv-sql Apolo101")))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
            }

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

            return File(destino, "Download", "Acompanhamento Mortalidades " + empresa + "_" + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        [HttpPost]
        public ActionResult DownloadRRC(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string destino = "";
            string pesquisa = "";

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_RRC_" + Session["empresa"].ToString() + "_" + Session["login"].ToString() + Session.SessionID + ".xlsm";
            pesquisa = "*Relatorio_RRC_" + Session["empresa"].ToString() + "_" + Session["login"].ToString() + "*.xlsm";

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("SAC");
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("SAC");
            }

            if (model["tipoData"] != null)
                Session["sTipoDataConf"] = model["tipoData"].ToString();
            else
            {
                ViewBag.erro = "Por favor, selecionar o Tipo de Data!";
                return View("SAC");
            }
            string opcaoData = model["tipoData"];
            Session["sTipoDataRRC"] = opcaoData;

            string empresa = "";

            if (model["Empresa"] != null)
            {
                empresa = model["Empresa"].ToString();
            }
            else
            {
                empresa = Session["empresaLayout"].ToString();
            }

            AtualizaDDL(empresa, (List<SelectListItem>)Session["ListaEmpresasRelComercial"]);

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_RRC.xlsm", destino);

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

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VW_Dados_RRC ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = Convert.ToDateTime(model["dataIni"].ToString()).ToString("yyyy-MM-dd") + " 00:00:00";
            string dataFinalStr = Convert.ToDateTime(model["dataFim"].ToString()).ToString("yyyy-MM-dd") + " 23:59:59";

            string filtroData = "";
            if (opcaoData.Equals("Inclusão"))
                filtroData = " [Data Inclusão] between '" + dataInicialStr + "' and '" + dataFinalStr + "' ";
            else
                filtroData = " [Data da RRC] between '" + dataInicialStr + "' and '" + dataFinalStr + "' ";

            string commandTextCHICCondicaoParametros =
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    filtroData;

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao = "";

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Dados"];

            worksheet.Cells[3, 4] = empresa;
            worksheet.Cells[4, 3] = "Data de " + opcaoData;
            worksheet.Cells[5, 3] = "Data de " + opcaoData;
            worksheet.Cells[4, 4] = Convert.ToDateTime(model["dataIni"].ToString());
            worksheet.Cells[5, 4] = Convert.ToDateTime(model["dataFim"].ToString());

            //RunMacro(oExcel, new Object[] { "Atualizar_Dados" });

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("srv-sql Apolo10 VW_Dados_SAC"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
            }

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

            return File(destino, "Download", "Relatorio_RRC_" + empresa + "_" + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsm");
        }

        [HttpPost]
        public ActionResult DownloadPlanilhaAcompanhamentoLote(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            #region Nome da Granja

            if (model["nomeGranja"] != null)
                Session["nomeGranja"] = model["nomeGranja"].ToString();
            else
            {
                ViewBag.erro = "Por favor, inserir o Nome da Granja!";
                return View("AcompanhamentoLote");
            }

            #endregion

            #region Lote

            if (model["lote"] != null)
                Session["lote"] = model["lote"].ToString();
            else
            {
                ViewBag.erro = "Por favor, inserir a Identificação do Lote no Cliente!";
                return View("AcompanhamentoLote");
            }

            #endregion

            #region Data de Nascimento

            if (model["dataNascimento"] != null)
                Session["dataNascimento"] = Convert.ToDateTime(model["dataNascimento"].ToString())
                    .ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir a Data de Nascimento!";
                return View("AcompanhamentoLote");
            }

            #endregion

            #region Qtde. Fêmeas Alojadas

            if (model["qtdeFemeasAlojadas"] != null)
                Session["qtdeFemeasAlojadas"] = model["qtdeFemeasAlojadas"].ToString();
            else
            {
                ViewBag.erro = "Por favor, inserir a Quantidade de Fêmeas Alojadas!";
                return View("AcompanhamentoLote");
            }

            #endregion

            #region Linhagem

            if (model["linhagem"] != null)
            {
                Session["linhagem"] = model["linhagem"].ToString();
                AtualizaDDL(model["linhagem"], (List<SelectListItem>)Session["ListaLinhagens"]);
            }

            #endregion

            #region Empresa

            string linhagem = model["linhagem"];
            string empresa = bdHLBAPP.Tabela_Precos
                .Where(w => w.Produto == linhagem)
                .FirstOrDefault().Empresa;

            #endregion

            #region Tipo de Período

            if (model["tipoPeriodo"] != null)
            {
                Session["tipoPeriodo"] = model["tipoPeriodo"].ToString();
                AtualizaDDL(model["tipoPeriodo"], (List<SelectListItem>)Session["ListaTipoPeriodo"]);
            }

            #endregion

            #region Tipo de Fase

            if (model["tipoFase"] != null)
            {
                Session["tipoFase"] = model["tipoFase"].ToString();
                AtualizaDDL(model["tipoFase"], (List<SelectListItem>)Session["ListaTipoFase"]);
            }

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            destino = "C:\\inetpub\\wwwroot\\Downloads\\Acompanhamento_Lotes_Clientes\\" +
                empresa + "\\Acompanhamento_" + model["tipoPeriodo"] + "_" + model["tipoFase"] + "_" + empresa + 
                "_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
            pesquisa = "*Acompanhamento_" + model["tipoPeriodo"] + "_" + model["tipoFase"] + "_" + empresa +
                "_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Downloads\\Acompanhamento_Lotes_Clientes\\" 
                + empresa, pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Downloads\\Acompanhamento_Lotes_Clientes\\" +
                empresa + "\\Acompanhamento_" + model["tipoPeriodo"] + "_" + model["tipoFase"] + "_" +
                empresa + ".xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Semanal"];

            worksheet.Cells[4, 5] = model["nomeGranja"];
            worksheet.Cells[5, 5] = model["lote"];
            worksheet.Cells[6, 5] = Convert.ToDateTime(model["dataNascimento"].ToString());
            worksheet.Cells[7, 5] = model["qtdeFemeasAlojadas"];
            worksheet.Cells[8, 5] = model["linhagem"];

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            return File(destino, "Download", 
                model["nomeGranja"] + "_" + model["linhagem"] + "_" 
                + Convert.ToDateTime(model["dataNascimento"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        #endregion

        #endregion

        #region Relatório de Visitas Técnicas

        public ActionResult RelVisitaTecnica()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaLinhagens();

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelVisitaTecnica(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string destino = "";
            string pesquisa = "";

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Visita_Tecnica_" 
                + Session["login"].ToString() + Session.SessionID + ".xlsx";
            pesquisa = "*Relatorio_Visita_Tecnica_" + Session["login"].ToString() + "*.xlsx";

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Inicial da Visita!";
                return View("RelVisitaTecnica");
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final da Visita!";
                return View("RelVisitaTecnica");
            }

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Visita_Tecnica.xlsx", destino);

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

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Relatório de Visitas"];

            DateTime dataInicial = Convert.ToDateTime(model["dataIni"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFim"].ToString());

            worksheet.Cells[2, 5] = "Período de " + dataInicial.ToString("dd/MM/yyyy")
                + " a " + dataFinal.ToString("dd/MM/yyyy");
            
            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            #region Dados

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VW_Formulario_Assistencia_Tecnica ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string filtroLinhagem = RetornaFiltroLinhagens("Linhagem");

            string commandTextCHICCondicaoParametros =
                    "[Data Visita] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    filtroLinhagem + " ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "[Ano Mês], [Técnico], [Data Visita]";

            #endregion

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("HLBAPP"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
            }

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

            return File(destino, "Download", "Relatorio_Visita_Tecnica_" 
                + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string RetornaFiltroLinhagens(string campo)
        {
            string retorno = campo + " in (";

            List<SelectListItem> listaLinhagens = (List<SelectListItem>)Session["ListaLinhagens"];

            foreach (var item in listaLinhagens)
            {
                retorno = retorno + "'" + item.Text + "'";

                if (listaLinhagens.IndexOf(item) != (listaLinhagens.Count - 1))
                    retorno = retorno + ",";
            }

            retorno = retorno + ")";

            return retorno;
        }

        #endregion
    }
}
