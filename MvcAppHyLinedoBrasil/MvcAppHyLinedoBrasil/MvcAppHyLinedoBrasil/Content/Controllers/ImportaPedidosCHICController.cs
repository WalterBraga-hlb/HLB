using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHyLinedoBrasil.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.ComponentModel;
using MvcAppHyLinedoBrasil.Models;
using MvcAppHyLinedoBrasil.Models.Apolo;
using MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA;
using MvcAppHyLinedoBrasil.Data.CHICDataSetTableAdapters;
using System.Data.Objects;
using Microsoft.Web.Administration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class ImportaPedidosCHICController : Controller
    {

        #region Objects

        CHICDataSet chic = new CHICDataSet();

        custTableAdapter cust = new custTableAdapter();
        ordersTableAdapter order = new ordersTableAdapter();
        bookedTableAdapter booked = new bookedTableAdapter();
        vartablTableAdapter vartabl = new vartablTableAdapter();
        itemsTableAdapter items = new itemsTableAdapter();
        int_commTableAdapter int_comm = new int_commTableAdapter();
        tablesTableAdapter tables = new tablesTableAdapter();
        salesmanTableAdapter salesman = new salesmanTableAdapter();

        ordersConfTableAdapter ordersConf = new ordersConfTableAdapter();

        public static string mensagemErro;
        public static int qtdErros;
        LayoutDb bd = new LayoutDb();
        public static string caminho;

        ApoloEntities apolo = new ApoloEntities();
        FinanceiroEntities apolo2 = new FinanceiroEntities();
        HLBAPPEntities hlbapp = new HLBAPPEntities();

        #endregion

        //
        // GET: /ImportaPedidosCHIC/

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

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            Session["BoubaMarcada"] = "";
            bd.Database.ExecuteSqlCommand("delete from LayoutPedidoPlanilhas");
            bd.SaveChanges();
            return View(bd.PedidoPlanilha);
        }

        [HttpPost]
        public ActionResult ImportaDadosPlanilhaPedido()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            qtdErros = 0;
            mensagemErro = "A planilha não pode ser importada. Seguem abaixo os erros: <br /><br />";

            caminho = @"C:\inetpub\wwwroot\Relatorios\Planilha_Importa_Pedido_" + Session["login"].ToString() + Session.SessionID + ".xlsm";

            //string pesquisa = "*Planilha_Importa_Pedido_" + Session["login"].ToString() + "*.xlsm";

            //string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            //foreach (var item in files)
            //{
            //    System.IO.File.Delete(item);
            //}

            Request.Files[0].SaveAs(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            bd.Database.ExecuteSqlCommand("delete from LayoutPedidoPlanilhas");
            bd.SaveChanges();

            LayoutPedidoPlanilha pedidoPlanilha = new LayoutPedidoPlanilha();

            //caminho = @"\\srv-fls-03\W\Relatorios_CHIC\Pedidos_Importados\" + DateTime.Now.ToLongTimeString().Replace(":", "-") + "_" + Request.Files[0].FileName;

            //string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]";
            string pattern = @"(?i)[^0-9a-z\s]";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string nameFileOld = Request.Files[0].FileName;
            string nameFileNew = rgx.Replace(nameFileOld, replacement);

            string pattern2 = @"(?i)[^0-9a-z]";
            Regex rgx2 = new Regex(pattern2);
            string replacement2 = "_";
            string nameFileNew2 = rgx2.Replace(nameFileNew, replacement2);

            nameFileNew2 = nameFileNew2.Replace("xlsm", "");

            caminho = @"\\srv-fls-03\W\Relatorios_CHIC\Pedidos_Importados\" + 
                DateTime.Now.ToLongTimeString().Replace(":", "-") + "_" + nameFileNew2 + ".xlsm";
                
            Request.Files[0].SaveAs(caminho);

            try
            {
                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                //System.IO.Packaging.Package arquivo3 = System.IO.Packaging.Package.Open(arquivo, FileMode.Open, FileAccess.ReadWrite);

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                // Navega entre cada Planilha
                foreach (var planilha in lista)
                {
                    // Caso a planilha exista, ele irá percorrer as linhas da planilha para verificar os filhos
                    if (planilha.Name == "Pedido")
                    {   
                        string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == planilha.Name)
                                                    .First().Id;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                        .GetPartById(relationshipId);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        var listaLinhas = sheetData.Descendants<Row>().ToList();

                        Row linhaEmpresa = sheetData.Elements<Row>().Where(r => r.RowIndex == 1).First();
                        Cell celulaEmpresa = linhaEmpresa.Elements<Cell>().Where(c => c.CellReference == "B1").First();
                        //string codigoEntidade = FromExcelTextBollean(celulaEntidade, spreadsheetDocument.WorkbookPart);
                        pedidoPlanilha.Empresa = FromExcelTextBollean(celulaEmpresa, spreadsheetDocument.WorkbookPart);

                        Row linhaEmailVendedor = sheetData.Elements<Row>().Where(r => r.RowIndex == 1).First();
                        Cell celulaEmailVendedor = linhaEmailVendedor.Elements<Cell>().Where(c => c.CellReference == "A1").First();
                        //string codigoEntidade = FromExcelTextBollean(celulaEntidade, spreadsheetDocument.WorkbookPart);
                        pedidoPlanilha.EmailVendedor = FromExcelTextBollean(celulaEmailVendedor, spreadsheetDocument.WorkbookPart);

                        // Pega os dados da Operação.
                        Row linhaOperacao = sheetData.Elements<Row>().Where(r => r.RowIndex == 2).First();
                        Cell celulaOperacao = linhaOperacao.Elements<Cell>().Where(c => c.CellReference == "G2").First();
                        //string codigoEntidade = FromExcelTextBollean(celulaEntidade, spreadsheetDocument.WorkbookPart);
                        pedidoPlanilha.Operacao = FromExcelTextBollean(celulaOperacao, spreadsheetDocument.WorkbookPart);

                        if (pedidoPlanilha.Operacao.Equals(""))
                        {
                            mensagemErro = mensagemErro + "* Operação não selecionada! <br />";
                            qtdErros++;
                        }

                        // Pega os dados da planilha como Cliente, Endereço, etc.
                        Row linhaEntidade = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                        Cell celulaEntidade = linhaEntidade.Elements<Cell>().Where(c => c.CellReference == "U7").First();
                        //string codigoEntidade = FromExcelTextBollean(celulaEntidade, spreadsheetDocument.WorkbookPart);
                        pedidoPlanilha.CodigoCliente = celulaEntidade.Descendants<CellValue>().FirstOrDefault().Text;

                        if (pedidoPlanilha.CodigoCliente.Equals("") && !pedidoPlanilha.Operacao.Equals("Cancelar")) 
                        {
                            mensagemErro = mensagemErro + "* Cliente não selecionado! <br />";
                            qtdErros++;
                        }

                        Row linhaDescricaoEntidade = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                        Cell celulaDescricaoEntidade = linhaDescricaoEntidade.Elements<Cell>().Where(c => c.CellReference == "E7").First();
                        pedidoPlanilha.DescricaoCliente = FromExcelTextBollean(celulaDescricaoEntidade, spreadsheetDocument.WorkbookPart);

                        Row linhaCidadePlanilha = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                        Cell celulaCidadePlanilha = linhaCidadePlanilha.Elements<Cell>().Where(c => c.CellReference == "E8").First();
                        pedidoPlanilha.Cidade = celulaCidadePlanilha.Descendants<CellValue>().FirstOrDefault().Text;

                        Row linhaUF = sheetData.Elements<Row>().Where(r => r.RowIndex == 8).First();
                        Cell celulaUF = linhaUF.Elements<Cell>().Where(c => c.CellReference == "U8").First();
                        pedidoPlanilha.Estado = celulaUF.Descendants<CellValue>().FirstOrDefault().Text;

                        // Pega o "X" da opcao do Endereço Diferente do Faturamento
                        Row linhaSelecionaEndereco = sheetData.Elements<Row>().Where(r => r.RowIndex == 12).First();
                        Cell celulaSelecionaEndereco = linhaSelecionaEndereco.Elements<Cell>().Where(c => c.CellReference == "E12").First();
                        string selecionaEndereco = FromExcelTextBollean(celulaSelecionaEndereco, spreadsheetDocument.WorkbookPart);

                        Row linhaSelecionaEnderecoMesmoFaturamento = sheetData.Elements<Row>().Where(r => r.RowIndex == 12).First();
                        Cell celulaSelecionaEnderecoMesmoFaturamento = linhaSelecionaEnderecoMesmoFaturamento.Elements<Cell>().Where(c => c.CellReference == "Q12").First();
                        string selecionaEnderecoMesmoFaturamento = FromExcelTextBollean(celulaSelecionaEnderecoMesmoFaturamento, spreadsheetDocument.WorkbookPart);

                        if (selecionaEndereco.Equals("") && selecionaEnderecoMesmoFaturamento.Equals("") && !pedidoPlanilha.Operacao.Equals("Cancelar")) 
                        {
                            mensagemErro = mensagemErro + "* Nenhuma Opção de Endereço selecionada! <br />";
                            qtdErros++;
                        }

                        // Pega o Endereço diferente caso esteja marcado
                        if (selecionaEndereco.Equals("X"))
                        {
                            Row linhaEndereco = sheetData.Elements<Row>().Where(r => r.RowIndex == 16).First();
                            Cell celulaEndereco = linhaEndereco.Elements<Cell>().Where(c => c.CellReference == "G16").First();
                            string endereco = FromExcelTextBollean(celulaEndereco, spreadsheetDocument.WorkbookPart);

                            if (endereco.Equals("") && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                            {
                                mensagemErro = mensagemErro + "* Linha 16 do Novo Endereço Vazia! <br />";
                                qtdErros++;
                            }

                            Row linhaCidade = sheetData.Elements<Row>().Where(r => r.RowIndex == 17).First();
                            Cell celulaCidade = linhaCidade.Elements<Cell>().Where(c => c.CellReference == "G17").First();
                            string cidade = FromExcelTextBollean(celulaCidade, spreadsheetDocument.WorkbookPart);

                            if (cidade.Equals("") && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                            {
                                mensagemErro = mensagemErro + "* Linha 17 de Novo Endereço vazia! <br />";
                                qtdErros++;
                            }
                        }

                        // Verifica a Vacina selecionada
                        Row linhaVacina = sheetData.Elements<Row>().Where(r => r.RowIndex == 44).First();
                        Cell celulaVacina = linhaVacina.Elements<Cell>().Where(c => c.CellReference == "F44").First();
                        pedidoPlanilha.Vacina = FromExcelTextBollean(celulaVacina, spreadsheetDocument.WorkbookPart);

                        string bouba = "";
                        string gombouro = "";
                        string coccidiose = "";
                        string laringo = "";
                        string salmonela = "";
                        if (!pedidoPlanilha.Vacina.Equals(""))
                        {
                            // Verifica se Bouba está selecionada
                            Row linhaBouba = sheetData.Elements<Row>().Where(r => r.RowIndex == 44).First();
                            Cell celulaBouba = linhaBouba.Elements<Cell>().Where(c => c.CellReference == "I44").First();
                            bouba = FromExcelTextBollean(celulaBouba, spreadsheetDocument.WorkbookPart);

                            if (bouba == "X")
                            {
                                pedidoPlanilha.Bouba = 1;
                                //Session["BoubaMarcada"] = "Sim";
                            }
                            else if (!Session["BoubaMarcada"].ToString().Equals("Sim"))
                            {
                                pedidoPlanilha.Bouba = 0;
                                Session["BoubaMarcada"] = "Não";
                            }

                            // Verifica se Gombouro está selecionada
                            if (pedidoPlanilha.Vacina.Equals("Marek"))
                            {
                                Row linhaGombouro = sheetData.Elements<Row>().Where(r => r.RowIndex == 44).First();
                                Cell celulaGombouro = linhaGombouro.Elements<Cell>().Where(c => c.CellReference == "K44").First();
                                gombouro = FromExcelTextBollean(celulaGombouro, spreadsheetDocument.WorkbookPart);
                                pedidoPlanilha.Gombouro = gombouro == "X" ? 1 : 0;
                            }

                            // Verifica se Coccidiose está selecionada
                            Row linhaCoccidiose = sheetData.Elements<Row>().Where(r => r.RowIndex == 44).First();
                            Cell celulaCoccidiose = linhaCoccidiose.Elements<Cell>().Where(c => c.CellReference == "N44").First();
                            coccidiose = FromExcelTextBollean(celulaCoccidiose, spreadsheetDocument.WorkbookPart);
                            pedidoPlanilha.Coccidiose = coccidiose == "X" ? 1 : 0;

                            if (pedidoPlanilha.Vacina.Equals("Rispens"))
                            {
                                // Verifica se Laringo (s/ HVT) está selecionada
                                Row linhaLaringo = sheetData.Elements<Row>().Where(r => r.RowIndex == 44).First();
                                Cell celulaLaringo = linhaLaringo.Elements<Cell>().Where(c => c.CellReference == "Q44").First();
                                laringo = FromExcelTextBollean(celulaLaringo, spreadsheetDocument.WorkbookPart);
                                pedidoPlanilha.Laringo = laringo == "X" ? 1 : 0;
                            }

                            // Verifica se Salmonela está selecionada
                            Row linhaSalmonela = sheetData.Elements<Row>().Where(r => r.RowIndex == 44).First();
                            Cell celulaSalmonela = linhaSalmonela.Elements<Cell>().Where(c => c.CellReference == "S44").First();
                            salmonela = FromExcelTextBollean(celulaSalmonela, spreadsheetDocument.WorkbookPart);
                            pedidoPlanilha.Salmonela = salmonela == "X" ? 1 : 0;

                            if (pedidoPlanilha.Salmonela.Equals(1) && pedidoPlanilha.Coccidiose.Equals(1) && pedidoPlanilha.Empresa.Equals("LB") && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                            {
                                mensagemErro = mensagemErro + "* Não é possível aplicar as Vacinas Salmonela e Coccidiose nas linhagens Lohmann! Selecione somente uma! <br />";
                                qtdErros++;
                            }
                        }

                        // Verifica se Serviços está selecionada
                        Row linhaServicos = sheetData.Elements<Row>().Where(r => r.RowIndex == 46).First();
                        Cell celulaServicos = linhaServicos.Elements<Cell>().Where(c => c.CellReference == "F46").First();
                        string servicos = FromExcelTextBollean(celulaServicos, spreadsheetDocument.WorkbookPart);

                        if (servicos.Equals("X"))
                        {
                            pedidoPlanilha.TratamentoInfravermelho = 1;

                            // Verifica Qtde. de Pintinhos que terá o Serviço
                            Row linhaQtdeServico = sheetData.Elements<Row>().Where(r => r.RowIndex == 46).First();

                            /*
                             * 31/10/2014 - Solicitação realizado de acordo com reunião com o comercial.
                             * 
                             * Calcular quantidade de pintinhos em porcentagem e não em valor.
                             */

                            //Cell celulaQtdeServico = linhaQtdeServico.Elements<Cell>().Where(c => c.CellReference == "M46").First();
                            //int qtdePintinhosTratInfraVermelho = 0;
                            //if (FromExcelTextBollean(celulaQtdeServico, spreadsheetDocument.WorkbookPart).Equals(""))
                            //    qtdePintinhosTratInfraVermelho = 0;
                            //else
                            //    qtdePintinhosTratInfraVermelho = Convert.ToInt32(FromExcelTextBollean(celulaQtdeServico, spreadsheetDocument.WorkbookPart));
                            //pedidoPlanilha.QtdePintinhosTratInfraVerm = qtdePintinhosTratInfraVermelho;

                            decimal percPintinhosTratInfraVermelho = 0;

                            if (linhaQtdeServico.Elements<Cell>()
                                .Where(c => c.CellReference.Value == "M46")
                                .First().InnerText != "")
                            {
                                percPintinhosTratInfraVermelho = Decimal.Round(Convert.ToDecimal(double.Parse(linhaQtdeServico.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "M46")
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 4);
                            }
                            else
                            {
                                percPintinhosTratInfraVermelho = 0;
                            }

                            pedidoPlanilha.PercPintinhosTratInfraVerm = percPintinhosTratInfraVermelho;

                            if (pedidoPlanilha.PercPintinhosTratInfraVerm.Equals(0) && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                            {
                                mensagemErro = mensagemErro + "* Não existe Quantidade de Pintainhas para o Serviço! <br />";
                                qtdErros++;
                            }
                        }

                        // Pega Embalagem Selecionada
                        Row linhaEmbalagem = sheetData.Elements<Row>().Where(r => r.RowIndex == 48).First();
                        Cell celulaEmbalagem = linhaEmbalagem.Elements<Cell>().Where(c => c.CellReference == "F48").First();
                        pedidoPlanilha.Embalagem = FromExcelTextBollean(celulaEmbalagem, spreadsheetDocument.WorkbookPart);

                        /**** 
                         * 07/07/2014 - Solicitado por Renata Casadei / Miriene Gomes
                         * 
                         * Retirar obrigatoriedade da Embalagem na planilha.
                        ****/
                        //if (pedidoPlanilha.Embalagem.Equals(""))
                        //{
                        //    mensagemErro = mensagemErro + "* Nenhuma Embalagem selecionada! <br />";
                        //    qtdErros++;
                        //}

                        // Verifica se Ovos Brasil está selecionado
                        Row linhaOvosBrasil = sheetData.Elements<Row>().Where(r => r.RowIndex == 48).First();
                        Cell celulaOvosBrasil = linhaOvosBrasil.Elements<Cell>().Where(c => c.CellReference == "O48").First();
                        string ovosBrasil = FromExcelTextBollean(celulaOvosBrasil, spreadsheetDocument.WorkbookPart);

                        if (!pedidoPlanilha.CodigoCliente.Equals(""))
                        {
                            ENTIDADE1 entidade1 = apolo2.ENTIDADE1
                                .Where(e1 => e1.EntCod == pedidoPlanilha.CodigoCliente)
                                .First();

                            string tipoColabOvosBrasil = "";

                            if (entidade1.USERTipoColabOvosBRasil != null)
                            {
                                tipoColabOvosBrasil = entidade1.USERTipoColabOvosBRasil;
                            }

                            if ((ovosBrasil.Equals("")) && (tipoColabOvosBrasil.Equals("Participal Lista")))
                            {
                                mensagemErro = mensagemErro + "* Cliente é Colaborar Oficial da IOB! Selecionar a opção obrigatoriamente! <br />";
                                qtdErros++;
                            }
                            else
                            {
                                pedidoPlanilha.OvosBrasil = ovosBrasil == "X" ? 1 : 0;
                            }
                        }

                        // Pega Condição de Pagamento Selecionada
                        Row linhaCondPag = sheetData.Elements<Row>().Where(r => r.RowIndex == 50).First();
                        Cell celulaCondPag = linhaCondPag.Elements<Cell>().Where(c => c.CellReference == "E50").First();
                        pedidoPlanilha.CondicaoPagamento = FromExcelTextBollean(celulaCondPag, spreadsheetDocument.WorkbookPart);

                        if (pedidoPlanilha.CondicaoPagamento.Equals("") && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                        {
                            mensagemErro = mensagemErro + "* Nenhuma Condição de Pagamento selecionada! <br />";
                            qtdErros++;
                        }

                        // Pega Observação 01
                        Row linhaObs1 = sheetData.Elements<Row>().Where(r => r.RowIndex == 53).First();
                        Cell celulaObs1 = linhaObs1.Elements<Cell>().Where(c => c.CellReference == "D53").First();
                        pedidoPlanilha.Observacao = FromExcelTextBollean(celulaObs1, spreadsheetDocument.WorkbookPart);

                        //if (obs1.Equals("") && operacao.Equals("Alterar"))
                        //{
                        //    mensagemErro = mensagemErro + "* Quando a Operação é alteração, é necessário informar na Observação quais foram as mesmas! <br />";
                        //    qtdErros++;
                        //}

                        // Pega Observação 02
                        Row linhaObs2 = sheetData.Elements<Row>().Where(r => r.RowIndex == 54).First();
                        Cell celulaObs2 = linhaObs2.Elements<Cell>().Where(c => c.CellReference == "D54").First();
                        pedidoPlanilha.Observacao = pedidoPlanilha.Observacao + " " + FromExcelTextBollean(celulaObs2, spreadsheetDocument.WorkbookPart);

                        // Pega Vendedor / Representante
                        Row linhaRepresentante = sheetData.Elements<Row>().Where(r => r.RowIndex == 56).First();
                        Cell celulaRepresentante = linhaRepresentante.Elements<Cell>().Where(c => c.CellReference == "E56").First();
                        pedidoPlanilha.Vendedor = FromExcelTextBollean(celulaRepresentante, spreadsheetDocument.WorkbookPart);

                        // Pega Número Pedido do Representante
                        Row linhaNumeroPedidoRepresentante = sheetData.Elements<Row>().Where(r => r.RowIndex == 56).First();
                        Cell celulaNumeroPedidoRepresentante = linhaNumeroPedidoRepresentante.Elements<Cell>().Where(c => c.CellReference == "U56").First();
                        pedidoPlanilha.NumeroPedidoRepresentante = FromExcelTextBollean(celulaNumeroPedidoRepresentante, spreadsheetDocument.WorkbookPart);

                        // Navega nas linhas da Planilha
                        foreach (var linha in listaLinhas)
                        {
                            if ((linha.RowIndex >= 23) && (linha.RowIndex <= 42))
                            {
                                // Recupera o Código do Produto Filho da Planilha caso exista
                                int existe = 0;
                                existe = linha.Elements<Cell>().Where(c => c.CellReference == "D" + linha.RowIndex).First().Descendants<CellValue>().Count();

                                if (existe > 0)
                                {
                                    //LayoutPedidoPlanilha item = pedidoPlanilha;
                                    LayoutPedidoPlanilha item = new LayoutPedidoPlanilha();

                                    item.Empresa = pedidoPlanilha.Empresa;
                                    item.EmailVendedor = pedidoPlanilha.EmailVendedor;
                                    item.Operacao = pedidoPlanilha.Operacao;
                                    item.CodigoCliente = pedidoPlanilha.CodigoCliente;
                                    item.DescricaoCliente = pedidoPlanilha.DescricaoCliente;
                                    item.Cidade = pedidoPlanilha.Cidade;
                                    item.Estado = pedidoPlanilha.Estado;
                                    item.Vacina = pedidoPlanilha.Vacina;
                                    item.Bouba = pedidoPlanilha.Bouba;
                                    item.Gombouro = pedidoPlanilha.Gombouro;
                                    item.Coccidiose = pedidoPlanilha.Coccidiose;
                                    item.Laringo = pedidoPlanilha.Laringo;
                                    item.Salmonela = pedidoPlanilha.Salmonela;
                                    item.TratamentoInfravermelho = pedidoPlanilha.TratamentoInfravermelho;
                                    item.QtdePintinhosTratInfraVerm = pedidoPlanilha.QtdePintinhosTratInfraVerm;
                                    item.Embalagem = pedidoPlanilha.Embalagem;
                                    item.OvosBrasil = pedidoPlanilha.OvosBrasil;
                                    item.CondicaoPagamento = pedidoPlanilha.CondicaoPagamento;
                                    item.Observacao = pedidoPlanilha.Observacao;
                                    item.Vendedor = pedidoPlanilha.Vendedor;
                                    item.NumeroPedidoRepresentante = pedidoPlanilha.NumeroPedidoRepresentante;

                                    mensagemErro = mensagemErro + "<br />";

                                    if (!item.Operacao.Equals("Incluir Novo"))
                                    {
                                        existe = 0;
                                        existe = linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First().Descendants<CellValue>().Count();

                                        if (existe > 0)
                                        {
                                            item.NumeroPedidoCHIC = Convert.ToInt32(FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "C" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart));

                                            order.FillByOrderNo(chic.orders, item.NumeroPedidoCHIC.ToString());

                                            if (chic.orders.Count == 0)
                                            {
                                                mensagemErro = mensagemErro + "* Linha " + linha.RowIndex.ToString() + ": O Número do Pedido no CHIC não existe na base! Verifique! <br />";
                                                qtdErros++;
                                            }
                                            else if (chic.orders[0].cust_no.Trim() != item.CodigoCliente && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                                            {
                                                mensagemErro = mensagemErro + "* Linha " + linha.RowIndex.ToString() + ": O pedido " + item.NumeroPedidoCHIC.ToString() + " não pertence ao Cliente! Verifique! <br />";
                                                qtdErros++;
                                            }
                                        }
                                        else
                                        {
                                            mensagemErro = mensagemErro + "* Linha " + linha.RowIndex.ToString() + ": Operação " + item.Operacao + " selecionada, porém não existe o Número do Pedido no CHIC para prosseguir! Verifique! <br />";
                                            qtdErros++;
                                        }
                                    }

                                    item.DataInicial = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                    .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                                    .First().Descendants<CellValue>().FirstOrDefault().Text));

                                    existe = 0;
                                    existe = linha.Elements<Cell>().Where(c => c.CellReference == "G" + linha.RowIndex).First().Descendants<CellValue>().Count();

                                    if (existe > 0)
                                    {
                                        item.DataFinal = FromExcelSerialDate(Convert.ToInt32(linha.Elements<Cell>()
                                                                        .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                                                        .First().Descendants<CellValue>().FirstOrDefault().Text));
                                    }
                                    else
                                    {
                                        item.DataFinal = item.DataInicial;
                                    }

                                    item.Linhagem = FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "I" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart);

                                    if (item.Linhagem.Equals("") && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                                    {
                                        mensagemErro = mensagemErro + "* Linha " + linha.RowIndex.ToString() + ": Linhagem não selecionada! <br />";
                                        qtdErros++;
                                    }

                                    /*if (FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart).Equals("") && !pedidoPlanilha.Operacao.Equals("Cancelar"))
                                    {
                                        mensagemErro = mensagemErro + "* Linha " + linha.RowIndex.ToString() + ": Quantidade Líquida não informada! <br />";
                                        qtdErros++;
                                    }
                                    else
                                    {
                                        item.QtdeLiquida = Convert.ToInt32(FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "L" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart));
                                    }*/

                                    if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "L" + linha.RowIndex)
                                                .First().InnerText != "")
                                    {
                                        item.QtdeLiquida = Convert.ToInt32(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "L" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text);
                                    }
                                    else
                                    {
                                        mensagemErro = mensagemErro + "* Linha " + linha.RowIndex.ToString() + ": Quantidade Líquida não informada! <br />";
                                        qtdErros++;
                                    }

                                    item.QtdeBonificacao = Convert.ToInt32(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "O" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));

                                    decimal qtdeBoni = item.QtdeBonificacao;
                                    decimal qtdLiq = item.QtdeLiquida;
                                    item.PercBonificacao = (qtdeBoni / qtdLiq) * 100;

                                    if (!FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart).Equals(""))
                                    {
                                        item.QtdeReposicao = Convert.ToInt32(FromExcelTextBollean(linha.Elements<Cell>().Where(c => c.CellReference == "P" + linha.RowIndex).First(), spreadsheetDocument.WorkbookPart));
                                    }

                                    if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                                .First().InnerText != "")
                                    {
                                        item.QtdeTotal = Convert.ToInt32(double.Parse(linha.Elements<Cell>()
                                                            .Where(c => c.CellReference == "Q" + linha.RowIndex)
                                                            .First().Descendants<CellValue>().FirstOrDefault().Text
                                                            .Replace(".", ",")));
                                    }
                                    else
                                    {
                                        item.QtdeTotal = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "S" + linha.RowIndex)
                                                .First().InnerText != "")
                                    {
                                        item.ValorUnitario = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "S" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 4);
                                    }
                                    else
                                    {
                                        item.ValorUnitario = 0;
                                    }

                                    if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "U" + linha.RowIndex)
                                                .First().InnerText != "")
                                    {
                                        item.ValorTotal = Decimal.Round(Convert.ToDecimal(double.Parse(linha.Elements<Cell>()
                                                                .Where(c => c.CellReference.Value == "U" + linha.RowIndex)
                                                                .First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ","))), 4);
                                    }
                                    else
                                    {
                                        item.ValorTotal = 0;
                                    }

                                    //item.QtdeTotal = item.QtdeLiquida + item.QtdeBonificacao + item.QtdeReposicao;
                                    //item.ValorUnitario = item.ValorTotal / item.QtdeLiquida;

                                    bd.PedidoPlanilha.Add(item);
                                }
                            }
                        }
                    }
                }

                if (qtdErros > 0)
                {
                    if (Session["BoubaMarcada"].ToString().Equals("Não"))
                    {
                        mensagemErro = mensagemErro + "* Existe Vacina marcada, porém a Bouba não está selecionada! Verifique se está correto!!! <br />";
                        qtdErros++;
                    }
                    mensagemErro = mensagemErro + "<br />* Total de Erros:" + qtdErros.ToString() + "<br />";
                    ViewBag.fileName = "";
                    ViewBag.mensagemErro = mensagemErro;
                    ViewBag.qtdErros = qtdErros;
                }
                else
                {
                    if (Session["BoubaMarcada"].ToString().Equals("Não"))
                    {
                        mensagemErro = mensagemErro + "* Existe Vacina marcada, porém a Bouba não está selecionada! <br />" +
                            "Caso queira Importar mesmo sem a Bouba, clique no Botão 'Confirmar Importação para o CHIC'! <br />" +
                            "Caso NÃO queira Importar clique no Botão 'Enviar E-mail p/ Criador da Planilha c/ Erros' para enviar " + 
                            "essa mensagem ao Vendedor / Representante marcar a opção e retornar a planilha para nova Importação!";
                        ViewBag.fileName = "";
                        ViewBag.mensagemErro = mensagemErro;
                    }
                    else
                    {
                        mensagemErro = "";
                    }

                    ViewBag.qtdErros = 0;
                }

                bd.SaveChanges();

                arquivo.Close();

                //ServerManager serverManager = new ServerManager();
                //ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
                //foreach (ApplicationPool appPool in applicationPoolCollection)
                //{
                //    if (appPool.Name.Equals("Hyline do Brasil - Apps"))
                //    {
                //        appPool.Recycle();
                //    }
                //}

                var listaRetorno = bd.PedidoPlanilha.ToList();

                return View("Index", listaRetorno);
            }
            catch (Exception e)
            {
                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                arquivo.Close();
                return View("Index", "");
            }
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

        public static DateTime FromExcelSerialDate(int SerialDate)
        {
            if (SerialDate > 59) SerialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(SerialDate);
        }

        [HttpPost]
        public ActionResult ImportaCHIC(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            try
            {
                string log = "";

                string tipoColabOvosBrasil = "";

                LayoutPedidoPlanilha pedido = bd.PedidoPlanilha.First();

                if (pedido.Operacao.Equals("Cancelar"))
                    mensagemErro = "Pedidos do cliente " + pedido.DescricaoCliente + " cancelado(s)! Seguem abaixo o(s) número(s): <br /><br />";
                else if (pedido.Operacao.Equals("Alterar"))
                    mensagemErro = "Pedidos do cliente " + pedido.DescricaoCliente + " alterado(s)! Seguem abaixo o(s) número(s): <br /><br />";
                else
                    mensagemErro = "Pedidos do cliente " + pedido.DescricaoCliente + " importado(s)! Seguem abaixo o(s) número(s) gerado(s): <br /><br />";

                var lista = bd.PedidoPlanilha.ToList();

                string orderNo = "";
                string orderNoReposicao = "";

                string empresa = "";
                string copiaPara = "";
                string nomeVendedor = "";
                string emailVendedor = "";
                string corpoEmail = "";
                string corpoOperacao = "";
                string operacao = "";

                int codigoItem = 0;

                foreach (var item in lista)
                {
                    string custoNo = item.CodigoCliente;

                    ENTIDADE1 entidade1 = apolo2.ENTIDADE1
                                .Where(e1 => e1.EntCod == custoNo)
                                .First();

                    if (entidade1.USERTipoColabOvosBRasil != null)
                    {
                        tipoColabOvosBrasil = entidade1.USERTipoColabOvosBRasil;
                    }

                    string motivo = "";

                    string numeroPedidoRepresentante = item.NumeroPedidoRepresentante;

                    #region Incluir Novo
                    if (item.Operacao.Equals("Incluir Novo"))
                    {
                        #region Dados do Pedido (orders)
                        DateTime orderDate = DateTime.Now;
                        string delivery = item.CondicaoPagamento;

                        int tamanho = item.Observacao.Length;

                        string com1 = "";
                        string com2 = "";
                        string com3 = "";
                        if (tamanho <= 80)
                        {
                            com1 = item.Observacao.Substring(0, tamanho);
                        }
                        if ((tamanho > 80) && (tamanho <= 160))
                        {
                            com1 = item.Observacao.Substring(0, 80);
                            com2 = item.Observacao.Substring(80, tamanho - 80);
                        }
                        if ((tamanho > 160) && (tamanho <= 240))
                        {
                            com1 = item.Observacao.Substring(0, 80);
                            com2 = item.Observacao.Substring(80, tamanho - 80);
                            com3 = item.Observacao.Substring(160, tamanho - 160);
                        }
                        string salesrep = item.Vendedor.Substring(0, 6);

                        DateTime cal_date = item.DataInicial.AddDays(-22);

                        int existe = 0;
                        existe = Convert.ToInt32(booked.FillSameOrderByCalDateAndCust(cal_date, custoNo, item.Empresa));

                        string item_ord = "";

                        if (existe == 0)
                        {
                            orderNo = (Convert.ToInt32(order.MaxOrderNo()) + 1).ToString();
                            order.Insert(orderNo, 0, Convert.ToDateTime("01/01/1988"), orderDate, custoNo, "Y", numeroPedidoRepresentante, 0, item.DataInicial, delivery,
                                //com1, com2, com3, String.Empty, String.Empty, 0, salesrep, String.Empty, String.Empty);
                                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, 0, salesrep, String.Empty, String.Empty);

                            log = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção Cabeçalho|" +
                                orderNo + "|" +
                                orderDate + "|" +
                                custoNo + "|" +
                                item.DataInicial.ToShortDateString() + " " + item.DataInicial.ToShortTimeString() + "|" +
                                delivery + "|" +
                                com1 + "|" +
                                com2 + "|" +
                                com3 + "|" +
                                salesrep + "\n\r";

                            item_ord = "01";
                        }
                        else
                        {
                            orderNo = booked.ReturnOrderNoByCalDateAndCust(cal_date, custoNo, item.Empresa).ToString();
                            item_ord = "02";
                        }
                        #endregion

                        #region Item 01 - Produto Vendido
                        /**** Dados dos Itens do Pedido (booked) ****/
                        //int booked_id = ((int)booked.MaxBookID()) + 1;
                        tables.Fill(chic.tables);
                        int booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                        vartabl.FillByDesc(chic.vartabl, item.Linhagem.Replace(" - Ovos", ""));
                        string varietyShort = chic.vartabl[0].variety;

                        string form = item.Linhagem.Contains("Ovos") ? "HN" : "DV";

                        items.FillByVarietyAndForm(chic.items, varietyShort, form);

                        // Localizando o Item
                        string itemno = "";
                        string accountno = "";
                        string descricao = "";
                        bool caixaPlastica = false;
                        bool vaxxitek = false;
                        for (int i = 0; i < chic.items.Count; i++)
                        {
                            caixaPlastica = chic.items[i].item_desc.Contains("-P") ? true : false;
                            if (caixaPlastica.Equals(false))
                                caixaPlastica = chic.items[i].item_desc.Contains("- P") ? true : false;
                            vaxxitek = chic.items[i].item_desc.Contains("VAXX") ? true : false;

                            if ((item.Embalagem.Equals("Plástica")) &&
                                (item.Vacina.Equals("Vaxxitek")) &&
                                (caixaPlastica) && (vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                            else if ((!item.Embalagem.Equals("Plástica")) &&
                                     (item.Vacina.Equals("Vaxxitek")) &&
                                     (!caixaPlastica) && (vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                            else if ((item.Embalagem.Equals("Plástica")) &&
                                     (!item.Vacina.Equals("Vaxxitek")) &&
                                     (caixaPlastica) && (!vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                            else if ((!item.Embalagem.Equals("Plástica")) &&
                                     (!item.Vacina.Equals("Vaxxitek")) &&
                                     (!caixaPlastica) && (!vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                        }

                        int quantity = item.QtdeLiquida - item.QtdeReposicao;

                        decimal price = 0;

                        if (tipoColabOvosBrasil.Equals("Participa Lista") &&
                            !item.OvosBrasil.Equals(1))
                        {
                            price = item.ValorUnitario + 0.0100m;
                        }
                        else
                        {
                            price = item.ValorUnitario;
                        }

                        string creatdby = Session["login"].ToString();
                        DateTime datecrtd = DateTime.Now;

                        booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                            String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                            item.DataInicial, 0, String.Empty, 0);

                        tables.UpdateQuery(Convert.ToDecimal(booked_id));

                        log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                            Session["login"].ToString() + "|" +
                            "Inserção Produto" + "|" +
                            booked_id.ToString() + "|" +
                            cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                            itemno + "|" +
                            descricao.Trim() + "|" +
                            quantity.ToString() + "|" +
                            price.ToString() + "|" +
                            accountno + "|" +
                            itemno + "\r\n";

                        #endregion

                        #region Item 02 - Bonificação
                        /**** Dados dos Itens do Pedido (booked) ****/
                        //booked_id = ((int)booked.MaxBookID()) + 1;
                        tables.Fill(chic.tables);
                        booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                        quantity = item.QtdeBonificacao;
                        price = 0;
                        //item_ord = "02";

                        string alt_desc = item.PercBonificacao.ToString() + "% Extra " + varietyShort;

                        booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                            String.Empty, "CH", accountno, alt_desc, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                            item.DataInicial, 0, String.Empty, 0);

                        tables.UpdateQuery(Convert.ToDecimal(booked_id));

                        log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                            Session["login"].ToString() + "|" +
                            "Inserção Bonificação" + "|" +
                            booked_id.ToString() + "|" +
                            cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                            itemno + "|" +
                            alt_desc + "|" +
                            quantity.ToString() + "|" +
                            price.ToString() + "|" +
                            accountno + "|" +
                            itemno + "\r\n";

                        #endregion

                        #region Item 03 - Vacinas
                        /**** Dados dos Itens do Pedido (booked) ****/
                        string filtroVacina = "";

                        if (existe == 0)
                        {
                            if (!item.Vacina.Equals(""))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                form = "VC";
                                varietyShort = "VACC";
                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";
                                filtroVacina = "";

                                filtroVacina = filtroVacina + ((!item.Vacina.Equals("Rispens") && !item.Vacina.Equals("Vaxxitek")) ? "HVT" : "");
                                filtroVacina = filtroVacina + (!filtroVacina.Equals("") ? "/RISP" : "RISP");
                                filtroVacina = filtroVacina + (item.Bouba.Equals(1) ? "/BOU" : "");
                                filtroVacina = filtroVacina + ((item.Gombouro.Equals(1) && !item.Vacina.Equals("Vaxxitek") && !item.Vacina.Equals("Vectormune")) ? "/GUMB" : "");
                                //filtroVacina = filtroVacina + (item.Coccidiose.Equals(1) ? "/COCC" : "");

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "03";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Vacina" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Coccidiose
                            if (item.Coccidiose.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                //filtroVacina = "APLIC VAC COCCIDIOSE";
                                if (item.Empresa.Equals("BR"))
                                    filtroVacina = "VAC COCCIDIOSE P/CONTA DA HY-LINE";
                                else if (item.Empresa.Equals("LB"))
                                    filtroVacina = "VAC COCCIDIOSE P/CONTA DA LOHMANN";
                                else
                                    filtroVacina = "VAC COCCIDIOSE P/ CONTA DA H&N";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "04";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Coccidiose" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Laringo
                            if (item.Laringo.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                filtroVacina = "APLIC VAC LARINGOTRAQUEITE";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "05";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Laringo" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Salmonela
                            if (item.Salmonela.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                //filtroVacina = "APLIC VAC CONTRA SALMONELLA";

                                if (item.Empresa.Equals("BR"))
                                    filtroVacina = "VAC SALMONELLA P/ CONTA HY LINE";
                                else if (item.Empresa.Equals("LB"))
                                    filtroVacina = "VAC SALMONELLA P/ CONTA LOHMANN";
                                else
                                    filtroVacina = "VAC SALMONELLA P/ CONTA  H&N";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "06";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Salmonela" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                        }
                        #endregion

                        #region Item 04 - Serviços

                        if (existe == 0)
                        {
                            if (item.TratamentoInfravermelho.Equals(1))
                            {
                                /**** Dados dos Itens do Pedido (booked) ****/
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";

                                itemno = "169";
                                //accountno = chic.items[0].account_no;

                                string comment_1 = "Tratar " + item.QtdePintinhosTratInfraVerm.ToString() + " pintos.";

                                item_ord = "06";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", comment_1, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Serviço" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    "TRATAMENTO INFRAVERMELHO|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                        }
                        #endregion

                        #region Item 05 - Embalagem

                        if (existe == 0)
                        {
                            if (!item.Embalagem.Equals(""))
                            {
                                /**** Dados dos Itens do Pedido (booked) ****/
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";

                                itemno = item.Embalagem.Equals("Plástica") ? "601" : "602";
                                //accountno = chic.items[0].account_no;
                                items.FillByItemNo(chic.items, itemno);

                                item_ord = "07";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Embalagem" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                        }
                        #endregion

                        #region Item 06 - Hidratante

                        if (existe == 0)
                        {
                            if ((!item.Estado.Equals("SP")) &&
                                (!item.Estado.Equals("RJ")) &&
                                (!item.Estado.Equals("MG")) &&
                                (!item.Estado.Equals("ES")) &&
                                (!item.Estado.Equals("PR")) &&
                                (!item.Estado.Equals("GO")))
                            {
                                /**** Dados dos Itens do Pedido (booked) ****/
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";

                                itemno = "172";
                                //accountno = chic.items[0].account_no;
                                items.FillByItemNo(chic.items, itemno);

                                item_ord = "08";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Hidratante" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                        }
                        #endregion

                        #region Inserir Ovos Brasil

                        string intComm = "";

                        int_comm.FillByOrderNo(chic.int_comm, orderNo);

                        if (item.OvosBrasil.Equals(1))
                        {
                            log = log +
                                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção" + "|" +
                                "Ovos Brasil: Sim";

                            if (chic.int_comm.Count > 0)
                                int_comm.DeleteQuery(orderNo);

                            intComm = item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString();
                            intComm = intComm + "\n\r\n\r Observação: " + item.Observacao;

                            if (tipoColabOvosBrasil.Equals("Participa Lista"))
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, false, false, true, "", false, 0,
                                    "");
                            }
                            else
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, true, false, false, "", false, 0,
                                    "");
                            }
                        }
                        else
                        {
                            log = log +
                                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção" + "|" +
                                "Ovos Brasil: Não";

                            if (chic.int_comm.Count > 0)
                                int_comm.DeleteQuery(orderNo);

                            intComm = item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString();
                            intComm = intComm + "\n\r\n\r Observação: " + item.Observacao;

                            if (tipoColabOvosBrasil.Equals("Participa Lista"))
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, false, false, true,
                                    "", false, 0,
                                    "");
                            }
                            else
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, false,
                                    false, false, "", false, 0, "");
                            }
                        }

                        #endregion

                        #region Reposição

                        if (item.QtdeReposicao > 0)
                        {
                            existe = 0;

                            #region Dados do Pedido (orders)
                            delivery = "DOAÇÃO";

                            orderNoReposicao = (Convert.ToInt32(order.MaxOrderNo()) + 1).ToString();
                            order.Insert(orderNoReposicao, 0, Convert.ToDateTime("01/01/1988"), orderDate, custoNo, "Y", numeroPedidoRepresentante, 0, item.DataInicial, delivery,
                                //com1, com2, com3, String.Empty, String.Empty, 0, salesrep, String.Empty, String.Empty);
                                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, 0, salesrep, String.Empty, String.Empty);

                            log = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção Cabeçalho|" +
                                orderNoReposicao + "|" +
                                orderDate + "|" +
                                custoNo + "|" +
                                item.DataInicial.ToShortDateString() + " " + item.DataInicial.ToShortTimeString() + "|" +
                                delivery + "|" +
                                com1 + "|" +
                                com2 + "|" +
                                com3 + "|" +
                                salesrep + "\n\r";

                            item_ord = "01";
                            #endregion

                            #region Item 01 - Produto Vendido
                            /**** Dados dos Itens do Pedido (booked) ****/
                            //int booked_id = ((int)booked.MaxBookID()) + 1;
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            vartabl.FillByDesc(chic.vartabl, item.Linhagem.Replace(" - Ovos", ""));
                            varietyShort = chic.vartabl[0].variety;

                            form = item.Linhagem.Contains("Ovos") ? "HN" : "DV";

                            items.FillByVarietyAndForm(chic.items, varietyShort, form);

                            // Localizando o Item
                            itemno = "";
                            accountno = "";
                            descricao = "";
                            caixaPlastica = false;
                            vaxxitek = false;
                            for (int i = 0; i < chic.items.Count; i++)
                            {
                                caixaPlastica = chic.items[i].item_desc.Contains("-P") ? true : false;
                                if (caixaPlastica.Equals(false))
                                    caixaPlastica = chic.items[i].item_desc.Contains("- P") ? true : false;
                                vaxxitek = chic.items[i].item_desc.Contains("VAXX") ? true : false;

                                if ((item.Embalagem.Equals("Plástica")) &&
                                    (item.Vacina.Equals("Vaxxitek")) &&
                                    (caixaPlastica) && (vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                                else if ((!item.Embalagem.Equals("Plástica")) &&
                                         (item.Vacina.Equals("Vaxxitek")) &&
                                         (!caixaPlastica) && (vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                                else if ((item.Embalagem.Equals("Plástica")) &&
                                         (!item.Vacina.Equals("Vaxxitek")) &&
                                         (caixaPlastica) && (!vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                                else if ((!item.Embalagem.Equals("Plástica")) &&
                                         (!item.Vacina.Equals("Vaxxitek")) &&
                                         (!caixaPlastica) && (!vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                            }
                            quantity = item.QtdeReposicao;

                            price = 0;

                            if (tipoColabOvosBrasil.Equals("Participa Lista") &&
                                !item.OvosBrasil.Equals(1))
                            {
                                price = item.ValorUnitario + 0.0100m;
                            }
                            else
                            {
                                price = item.ValorUnitario;
                            }

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção Produto" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                descricao.Trim() + "|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";

                            #endregion

                            #region Item 03 - Vacinas
                            /**** Dados dos Itens do Pedido (booked) ****/
                            filtroVacina = "";

                            if (!item.Vacina.Equals(""))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                form = "VC";
                                varietyShort = "VACC";
                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";
                                filtroVacina = "";

                                filtroVacina = filtroVacina + ((!item.Vacina.Equals("Rispens") && !item.Vacina.Equals("Vaxxitek")) ? "HVT" : "");
                                filtroVacina = filtroVacina + (!filtroVacina.Equals("") ? "/RISP" : "RISP");
                                filtroVacina = filtroVacina + (item.Bouba.Equals(1) ? "/BOU" : "");
                                filtroVacina = filtroVacina + ((item.Gombouro.Equals(1) && !item.Vacina.Equals("Vaxxitek") && !item.Vacina.Equals("Vectormune")) ? "/GUMB" : "");
                                //filtroVacina = filtroVacina + (item.Coccidiose.Equals(1) ? "/COCC" : "");

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "03";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Vacina" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Coccidiose
                            if (item.Coccidiose.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                //filtroVacina = "APLIC VAC COCCIDIOSE";
                                if (item.Empresa.Equals("BR"))
                                    filtroVacina = "VAC COCCIDIOSE P/CONTA DA HY-LINE";
                                else if (item.Empresa.Equals("LB"))
                                    filtroVacina = "VAC COCCIDIOSE P/CONTA DA LOHMANN";
                                else
                                    filtroVacina = "VAC COCCIDIOSE P/ CONTA DA H&N";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "04";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Coccidiose" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Laringo
                            if (item.Laringo.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                filtroVacina = "APLIC VAC LARINGOTRAQUEITE";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "05";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Laringo" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Salmonela
                            if (item.Salmonela.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                //filtroVacina = "APLIC VAC CONTRA SALMONELLA";

                                if (item.Empresa.Equals("BR"))
                                    filtroVacina = "VAC SALMONELLA P/ CONTA HY LINE";
                                else if (item.Empresa.Equals("LB"))
                                    filtroVacina = "VAC SALMONELLA P/ CONTA LOHMANN";
                                else
                                    filtroVacina = "VAC SALMONELLA P/ CONTA  H&N";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "06";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Salmonela" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                            #endregion

                            #region Item 04 - Serviços

                            if (item.TratamentoInfravermelho.Equals(1))
                            {
                                /**** Dados dos Itens do Pedido (booked) ****/
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";

                                itemno = "169";
                                //accountno = chic.items[0].account_no;

                                string comment_1 = "Tratar " + item.QtdePintinhosTratInfraVerm.ToString() + " pintos.";

                                item_ord = "06";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", comment_1, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Serviço" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    "TRATAMENTO INFRAVERMELHO|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                            #endregion

                            #region Item 05 - Embalagem

                            if (existe == 0)
                            {
                                if (!item.Embalagem.Equals(""))
                                {
                                    /**** Dados dos Itens do Pedido (booked) ****/
                                    tables.Fill(chic.tables);
                                    booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                    quantity = 0;
                                    price = 0;

                                    // Localizando o Item
                                    itemno = "";
                                    accountno = "";

                                    itemno = item.Embalagem.Equals("Plástica") ? "601" : "602";
                                    //accountno = chic.items[0].account_no;
                                    items.FillByItemNo(chic.items, itemno);

                                    item_ord = "07";

                                    booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                        String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                        item.DataInicial, 0, String.Empty, 0);

                                    tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                    log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                        Session["login"].ToString() + "|" +
                                        "Inserção Embalagem" + "|" +
                                        booked_id.ToString() + "|" +
                                        cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                        itemno + "|" +
                                        chic.items[0].item_desc.Trim() + "|" +
                                        quantity.ToString() + "|" +
                                        price.ToString() + "|" +
                                        accountno + "|" +
                                        itemno + "\r\n";
                                }
                            }
                            #endregion

                            #region Item 06 - Hidratante

                            if (existe == 0)
                            {
                                if ((!item.Estado.Equals("SP")) &&
                                    (!item.Estado.Equals("RJ")) &&
                                    (!item.Estado.Equals("MG")) &&
                                    (!item.Estado.Equals("ES")) &&
                                    (!item.Estado.Equals("PR")) &&
                                    (!item.Estado.Equals("GO")))
                                {
                                    /**** Dados dos Itens do Pedido (booked) ****/
                                    tables.Fill(chic.tables);
                                    booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                    quantity = 0;
                                    price = 0;

                                    // Localizando o Item
                                    itemno = "";
                                    accountno = "";

                                    itemno = "172";
                                    //accountno = chic.items[0].account_no;
                                    items.FillByItemNo(chic.items, itemno);

                                    item_ord = "08";

                                    booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                        String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                        item.DataInicial, 0, String.Empty, 0);

                                    tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                    log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                        Session["login"].ToString() + "|" +
                                        "Inserção Hidratante" + "|" +
                                        booked_id.ToString() + "|" +
                                        cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                        itemno + "|" +
                                        chic.items[0].item_desc.Trim() + "|" +
                                        quantity.ToString() + "|" +
                                        price.ToString() + "|" +
                                        accountno + "|" +
                                        itemno + "\r\n";
                                }
                            }
                            #endregion

                            #region Inserir Tabela Customizada Pedido

                            intComm = "";

                            int_comm.FillByOrderNo(chic.int_comm, orderNoReposicao);

                            log = log +
                                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção" + "|" +
                                "Ovos Brasil: Não";

                            if (chic.int_comm.Count > 0)
                                int_comm.DeleteQuery(orderNoReposicao);

                            intComm = item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString();
                            intComm = intComm + "\n\r\n\r Observação: " + item.Observacao;

                            decimal orderNoPrincipal = Convert.ToDecimal(orderNo);

                            if (tipoColabOvosBrasil.Equals("Participa Lista"))
                            {
                                int_comm.InsertQuery(orderNoReposicao, intComm, log, false, String.Empty, false,
                                    false, false, true, "", false, orderNoPrincipal, "");
                            }
                            else
                            {
                                int_comm.InsertQuery(orderNoReposicao, intComm, log, false, String.Empty, false,
                                    false, false, false, "", false, orderNoPrincipal, "");
                            }

                            #endregion
                        }

                        #endregion

                        #region LOG

                        LOG_LayoutPedidoPlanilhas logHLBAPP = new LOG_LayoutPedidoPlanilhas();

                        logHLBAPP.Usuario = Session["login"].ToString();
                        logHLBAPP.DataHora = DateTime.Now;

                        logHLBAPP.Bouba = item.Bouba;
                        logHLBAPP.Cidade = item.Cidade;
                        logHLBAPP.Coccidiose = item.Coccidiose;
                        logHLBAPP.CodigoCliente = item.CodigoCliente;
                        logHLBAPP.CondicaoPagamento = item.CondicaoPagamento;
                        logHLBAPP.DataFinal = item.DataFinal;
                        logHLBAPP.DataInicial = item.DataInicial;
                        logHLBAPP.DescricaoCliente = item.DescricaoCliente;
                        logHLBAPP.EmailVendedor = item.EmailVendedor;
                        logHLBAPP.Embalagem = item.Embalagem;
                        logHLBAPP.Empresa = item.Empresa;
                        logHLBAPP.Estado = item.Estado;
                        logHLBAPP.Gombouro = item.Gombouro;
                        logHLBAPP.Laringo = item.Laringo;
                        logHLBAPP.Linhagem = item.Linhagem;
                        logHLBAPP.NumeroPedidoCHIC = Convert.ToInt32(orderNo);
                        logHLBAPP.Observacao = item.Observacao;
                        logHLBAPP.Operacao = item.Operacao;
                        logHLBAPP.PercBonificacao = item.PercBonificacao;
                        logHLBAPP.QtdeBonificacao = item.QtdeBonificacao;
                        logHLBAPP.QtdeLiquida = item.QtdeLiquida;
                        logHLBAPP.QtdePintinhosTratInfraVerm = item.QtdePintinhosTratInfraVerm;
                        logHLBAPP.QtdeReposicao = item.QtdeReposicao;
                        logHLBAPP.QtdeTotal = item.QtdeTotal;
                        logHLBAPP.Salmonela = item.Salmonela;
                        logHLBAPP.TratamentoInfravermelho = item.TratamentoInfravermelho;
                        logHLBAPP.Vacina = item.Vacina;
                        if (tipoColabOvosBrasil.Equals("Participa Lista") &&
                            !item.OvosBrasil.Equals(1))
                        {
                            item.ValorUnitario = item.ValorUnitario + 0.0100m;
                            logHLBAPP.ValorTotal = item.QtdeLiquida * item.ValorUnitario;
                            logHLBAPP.OvosBrasil = 2;
                        }
                        else
                        {
                            logHLBAPP.ValorTotal = item.ValorTotal;
                            logHLBAPP.OvosBrasil = item.OvosBrasil;
                        }
                        logHLBAPP.ValorUnitario = item.ValorUnitario;
                        logHLBAPP.Vendedor = item.Vendedor;
                        logHLBAPP.CaminhoArquivo = caminho;

                        hlbapp.LOG_LayoutPedidoPlanilhas.AddObject(logHLBAPP);

                        #endregion

                        mensagemErro = mensagemErro + "* Pedido " + orderNo + " - " +
                            item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString() +
                            " - " + item.Linhagem + " - Qtde.: " + String.Format("{0:0,0}", item.QtdeLiquida) + " Valor Total: " +
                            String.Format("{0:C}", item.ValorTotal) + " <br />";

                        if (item.QtdeReposicao > 0)
                        {
                            mensagemErro = mensagemErro + "*** Pedido " + orderNoReposicao + " de Reposição referente ao Pedido acima - " +
                                item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString() +
                                " - " + item.Linhagem + " - Qtde.: " + String.Format("{0:0,0}", item.QtdeReposicao) + " Valor Total: " +
                                String.Format("{0:C}", (item.QtdeReposicao * item.ValorUnitario)) + " <br />";
                        }
                    }
                    #endregion

                    #region Alterar
                    else if (item.Operacao.Equals("Alterar"))
                    {
                        motivo = model["motivo"].ToString();

                        #region Dados do Pedido (orders)

                        order.FillByOrderNo(chic.orders, item.NumeroPedidoCHIC.ToString());

                        if (!orderNo.Equals("") && item.NumeroPedidoCHIC.ToString() == orderNo)
                        {
                            codigoItem = codigoItem++;
                        }
                        else
                        {
                            codigoItem = 1;
                        }

                        string item_ord = "0" + codigoItem.ToString();

                        orderNo = chic.orders[0].orderno;
                        DateTime orderDate = chic.orders[0].order_date;
                        custoNo = item.CodigoCliente;
                        string delivery = item.CondicaoPagamento;

                        int tamanho = item.Observacao.Length;

                        string com1 = "";
                        string com2 = "";
                        string com3 = "";
                        if (tamanho <= 80)
                        {
                            com1 = item.Observacao.Substring(0, tamanho);
                        }
                        if ((tamanho > 80) && (tamanho <= 160))
                        {
                            com1 = item.Observacao.Substring(0, 80);
                            com2 = item.Observacao.Substring(80, tamanho - 80);
                        }
                        if ((tamanho > 160) && (tamanho <= 240))
                        {
                            com1 = item.Observacao.Substring(0, 80);
                            com2 = item.Observacao.Substring(80, tamanho - 80);
                            com3 = item.Observacao.Substring(160, tamanho - 160);
                        }

                        string salesrep = item.Vendedor.Substring(0, 6);

                        order.UpdateQuery(orderDate, custoNo, item.DataInicial, delivery,
                            //com1, com2, com3, salesrep, orderNo);
                            String.Empty, String.Empty, String.Empty, salesrep, orderNo);

                        log = "\r\n\r\n" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                            Session["login"].ToString() + "|" +
                            "Alteração Cabeçalho|" +
                            orderNo + "|" +
                            orderDate + "|" +
                            custoNo + "|" +
                            item.DataInicial.ToShortDateString() + " " + item.DataInicial.ToShortTimeString() + "|" +
                            delivery + "|" +
                            com1 + "|" +
                            com2 + "|" +
                            com3 + "|" +
                            salesrep + "\n\r";
                        #endregion

                        #region Deleta os Items para adicioná-los novamente

                        //booked.DeleteQuery(orderNo);
                        booked.DeletePriceEqualsZero(orderNo);

                        #endregion

                        #region Item 01 - Produto Vendido
                        /**** Dados dos Itens do Pedido (booked) ****/
                        tables.Fill(chic.tables);
                        int booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;
                        DateTime cal_date = item.DataInicial.AddDays(-22);

                        vartabl.FillByDesc(chic.vartabl, item.Linhagem.Replace(" - Ovos", ""));
                        string varietyShort = chic.vartabl[0].variety;

                        string form = item.Linhagem.Contains("Ovos") ? "HN" : "DV";

                        items.FillByVarietyAndForm(chic.items, varietyShort, form);

                        // Localizando o Item
                        string itemno = "";
                        string accountno = "";
                        string descricao = "";
                        bool caixaPlastica = false;
                        bool vaxxitek = false;
                        for (int i = 0; i < chic.items.Count; i++)
                        {
                            caixaPlastica = chic.items[i].item_desc.Contains("-P") ? true : false;
                            if (caixaPlastica.Equals(false))
                                caixaPlastica = chic.items[i].item_desc.Contains("- P") ? true : false;
                            vaxxitek = chic.items[i].item_desc.Contains("VAXX") ? true : false;

                            if ((item.Embalagem.Equals("Plástica")) &&
                                (item.Vacina.Equals("Vaxxitek")) &&
                                (caixaPlastica) && (vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                            else if ((!item.Embalagem.Equals("Plástica")) &&
                                     (item.Vacina.Equals("Vaxxitek")) &&
                                     (!caixaPlastica) && (vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                            else if ((item.Embalagem.Equals("Plástica")) &&
                                     (!item.Vacina.Equals("Vaxxitek")) &&
                                     (caixaPlastica) && (!vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                            else if ((!item.Embalagem.Equals("Plástica")) &&
                                     (!item.Vacina.Equals("Vaxxitek")) &&
                                     (!caixaPlastica) && (!vaxxitek))
                            {
                                itemno = chic.items[i].item_no;
                                accountno = chic.items[i].account_no;
                                descricao = chic.items[i].item_desc;
                            }
                        }

                        int quantity = item.QtdeLiquida - item.QtdeReposicao;
                        decimal price = 0;

                        if ((tipoColabOvosBrasil.Equals("Participa Lista")) && (!item.OvosBrasil.Equals(1)))
                        {
                            price = item.ValorUnitario + 0.0100m;
                        }
                        else
                        {
                            price = item.ValorUnitario;
                        }

                        string creatdby = Session["login"].ToString();
                        DateTime datecrtd = DateTime.Now;

                        items.FillByVariety(chic.items, varietyShort);

                        for (int i = 0; i < chic.items.Count; i++)
                        {
                            booked.DeleteByItemOrderNo(orderNo, chic.items[i].item_no);
                        }

                        booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                            String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                            item.DataInicial, 0, String.Empty, 0);

                        tables.UpdateQuery(Convert.ToDecimal(booked_id));

                        log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                            Session["login"].ToString() + "|" +
                            "Alteração Produto" + "|" +
                            booked_id.ToString() + "|" +
                            cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                            itemno + "|" +
                            descricao.Trim() + "|" +
                            quantity.ToString() + "|" +
                            price.ToString() + "|" +
                            accountno + "|" +
                            itemno + "\r\n";

                        #endregion

                        #region Item 02 - Bonificação
                        /**** Dados dos Itens do Pedido (booked) ****/
                        tables.Fill(chic.tables);
                        booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                        quantity = item.QtdeBonificacao;
                        price = 0;
                        //item_ord = "02";

                        string alt_desc = item.PercBonificacao.ToString() + "% Extra " + varietyShort;

                        booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                            String.Empty, "CH", accountno, alt_desc, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                            item.DataInicial, 0, String.Empty, 0);

                        tables.UpdateQuery(Convert.ToDecimal(booked_id));

                        log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                            Session["login"].ToString() + "|" +
                            "Alteração Bonificação" + "|" +
                            booked_id.ToString() + "|" +
                            cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                            itemno + "|" +
                            alt_desc + "|" +
                            quantity.ToString() + "|" +
                            price.ToString() + "|" +
                            accountno + "|" +
                            itemno + "\r\n";

                        #endregion

                        #region Item 03 - Vacinas
                        /**** Dados dos Itens do Pedido (booked) ****/
                        string filtroVacina = "";

                        if (!item.Vacina.Equals(""))
                        {
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            form = "VC";
                            varietyShort = "VACC";
                            quantity = 0;
                            price = 0;

                            // Localizando o Item
                            itemno = "";
                            accountno = "";
                            filtroVacina = "";

                            filtroVacina = filtroVacina + ((!item.Vacina.Equals("Rispens") && !item.Vacina.Equals("Vaxxitek")) ? "HVT" : "");
                            filtroVacina = filtroVacina + (!filtroVacina.Equals("") ? "/RISP" : "RISP");
                            filtroVacina = filtroVacina + (item.Bouba.Equals(1) ? "/BOU" : "");
                            filtroVacina = filtroVacina + ((item.Gombouro.Equals(1) && !item.Vacina.Equals("Vaxxitek") && !item.Vacina.Equals("Vectormune")) ? "/GUMB" : "");
                            //filtroVacina = filtroVacina + (item.Coccidiose.Equals(1) ? "/COCC" : "");

                            items.FillByVacinas(chic.items, form, filtroVacina);

                            itemno = chic.items[0].item_no;
                            //accountno = chic.items[0].account_no;

                            item_ord = "03";

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Alteração Vacina" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                chic.items[0].item_desc.Trim() + "|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";
                        }

                        // Coccidiose
                        if (item.Coccidiose.Equals(1))
                        {
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            filtroVacina = "";
                            //filtroVacina = "APLIC VAC COCCIDIOSE";
                            if (item.Empresa.Equals("BR"))
                                filtroVacina = "VAC COCCIDIOSE P/CONTA DA HY-LINE";
                            else if (item.Empresa.Equals("LB"))
                                filtroVacina = "VAC COCCIDIOSE P/CONTA DA LOHMANN";
                            else
                                filtroVacina = "VAC COCCIDIOSE P/ CONTA DA H&N";

                            items.FillByVacinas(chic.items, form, filtroVacina);

                            itemno = chic.items[0].item_no;
                            //accountno = chic.items[0].account_no;

                            item_ord = "04";

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção Coccidiose" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                chic.items[0].item_desc.Trim() + "|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";
                        }

                        // Laringo
                        if (item.Laringo.Equals(1))
                        {
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            filtroVacina = "";
                            filtroVacina = (!item.Laringo.Equals(1) ? "APLIC VAC LARINGOTRAQUEITE" : "");

                            items.FillByVacinas(chic.items, form, filtroVacina);

                            itemno = chic.items[0].item_no;
                            //accountno = chic.items[0].account_no;

                            item_ord = "05";

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Alteração Laringo" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                chic.items[0].item_desc.Trim() + "|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";
                        }

                        // Salmonela
                        if (item.Salmonela.Equals(1))
                        {
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            filtroVacina = "";
                            //filtroVacina = (!item.Laringo.Equals(1) ? "APLIC VAC CONTRA SALMONELLA" : "");
                            if (item.Empresa.Equals("BR"))
                                filtroVacina = "VAC SALMONELLA P/ CONTA HY LINE";
                            else if (item.Empresa.Equals("LB"))
                                filtroVacina = "VAC SALMONELLA P/ CONTA LOHMANN";
                            else
                                filtroVacina = "VAC SALMONELLA P/ CONTA  H&N";

                            items.FillByVacinas(chic.items, form, filtroVacina);

                            itemno = chic.items[0].item_no;
                            //accountno = chic.items[0].account_no;

                            item_ord = "06";

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Alteração Salmonela" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                chic.items[0].item_desc.Trim() + "|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";
                        }
                        #endregion

                        #region Item 04 - Serviços

                        if (item.TratamentoInfravermelho.Equals(1))
                        {
                            /**** Dados dos Itens do Pedido (booked) ****/
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            quantity = 0;
                            price = 0;

                            // Localizando o Item
                            itemno = "";
                            accountno = "";

                            itemno = "169";
                            //accountno = chic.items[0].account_no;

                            string comment_1 = "Tratar " + item.QtdePintinhosTratInfraVerm.ToString() + " pintos.";

                            item_ord = "06";

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", comment_1, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Alteração Serviço" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                "TRATAMENTO INFRAVERMELHO|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";
                        }
                        #endregion

                        #region Item 05 - Embalagem
                        /**** Dados dos Itens do Pedido (booked) ****/
                        if (!item.Embalagem.Equals(""))
                        {
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            quantity = 0;
                            price = 0;

                            // Localizando o Item
                            itemno = "";
                            accountno = "";

                            itemno = item.Embalagem.Equals("Plástica") ? "601" : "602";
                            //accountno = chic.items[0].account_no;

                            items.FillByItemNo(chic.items, itemno);

                            item_ord = "07";

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNo, "O", String.Empty, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Alteração Embalagem" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                chic.items[0].item_desc.Trim() + "|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";
                        }
                        #endregion

                        #region Inserir Ovos Brasil

                        int_comm.FillByOrderNo(chic.int_comm, orderNo);

                        if (entidade1.USERTipoColabOvosBRasil != null)
                        {
                            tipoColabOvosBrasil = entidade1.USERTipoColabOvosBRasil;
                        }

                        string logAntigo = "";
                        string intComm = "";

                        if (item.OvosBrasil.Equals(1))
                        {
                            log = log +
                                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Alteração" + "|" +
                                "Ovos Brasil: Sim";

                            if (chic.int_comm.Count > 0)
                            {
                                logAntigo = chic.int_comm[0].changelg;
                                log = logAntigo + "\r\n" + log;
                                intComm = "Motivo da Alteração feita no dia " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + ": " + motivo + "\r\n\r\n";
                                intComm = intComm + "Observação da Planilha: " + item.Observacao + "\r\n\r\n" + chic.int_comm[0].comments;
                                int_comm.DeleteQuery(orderNo);
                            }

                            if (tipoColabOvosBrasil.Equals("Participa Lista"))
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, false,
                                    false, true, "", false, 0, "");
                            }
                            else
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, true,
                                    false, false, "", false, 0, "");
                            }
                        }
                        else
                        {
                            log = log +
                                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Alteração" + "|" +
                                "Ovos Brasil: Não";

                            if (chic.int_comm.Count > 0)
                            {
                                logAntigo = chic.int_comm[0].changelg;
                                log = logAntigo + "\r\n" + log;
                                intComm = "Motivo da Alteração feita no dia " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + ": " + motivo + "\r\n\r\n";
                                intComm = intComm + "Observação da Planilha: " + item.Observacao + "\r\n\r\n" + chic.int_comm[0].comments;
                                int_comm.DeleteQuery(orderNo);
                            }

                            if (tipoColabOvosBrasil.Equals("Participa Lista"))
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, false,
                                    false, true, "", false, 0, "");
                            }
                            else
                            {
                                int_comm.InsertQuery(orderNo, intComm, log, false, String.Empty, false, false,
                                    false, false, "", false, 0, "");
                            }
                        }

                        #endregion

                        #region Reposição

                        if (item.QtdeReposicao > 0)
                        {
                            int existe = 0;

                            #region Deleta Pedidos de Reposição Anteriores

                            CHICDataSet.int_commDataTable intCommReposicaoDT = new CHICDataSet.int_commDataTable();

                            decimal orderNoPrincipal = Convert.ToDecimal(orderNo);

                            int_comm.FillByOrderNoMain(intCommReposicaoDT, orderNoPrincipal);

                            foreach (var itemReposicao in intCommReposicaoDT.ToList())
                            {
                                int_comm.DeleteQuery(itemReposicao.orderno);
                                booked.DeleteQuery(itemReposicao.orderno);
                                order.DeleteQuery(itemReposicao.orderno);
                            }

                            #endregion

                            #region Dados do Pedido (orders)
                            delivery = "DOAÇÃO";

                            orderNoReposicao = (Convert.ToInt32(order.MaxOrderNo()) + 1).ToString();
                            order.Insert(orderNoReposicao, 0, Convert.ToDateTime("01/01/1988"), orderDate, custoNo, "Y", numeroPedidoRepresentante, 0, item.DataInicial, delivery,
                                //com1, com2, com3, String.Empty, String.Empty, 0, salesrep, String.Empty, String.Empty);
                                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, 0, salesrep, String.Empty, String.Empty);

                            log = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção Cabeçalho|" +
                                orderNoReposicao + "|" +
                                orderDate + "|" +
                                custoNo + "|" +
                                item.DataInicial.ToShortDateString() + " " + item.DataInicial.ToShortTimeString() + "|" +
                                delivery + "|" +
                                com1 + "|" +
                                com2 + "|" +
                                com3 + "|" +
                                salesrep + "\n\r";

                            item_ord = "01";
                            #endregion

                            #region Item 01 - Produto Vendido
                            /**** Dados dos Itens do Pedido (booked) ****/
                            //int booked_id = ((int)booked.MaxBookID()) + 1;
                            tables.Fill(chic.tables);
                            booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                            vartabl.FillByDesc(chic.vartabl, item.Linhagem.Replace(" - Ovos", ""));
                            varietyShort = chic.vartabl[0].variety;

                            form = item.Linhagem.Contains("Ovos") ? "HN" : "DV";

                            items.FillByVarietyAndForm(chic.items, varietyShort, form);

                            // Localizando o Item
                            itemno = "";
                            accountno = "";
                            descricao = "";
                            caixaPlastica = false;
                            vaxxitek = false;
                            for (int i = 0; i < chic.items.Count; i++)
                            {
                                caixaPlastica = chic.items[i].item_desc.Contains("-P") ? true : false;
                                if (caixaPlastica.Equals(false))
                                    caixaPlastica = chic.items[i].item_desc.Contains("- P") ? true : false;
                                vaxxitek = chic.items[i].item_desc.Contains("VAXX") ? true : false;

                                if ((item.Embalagem.Equals("Plástica")) &&
                                    (item.Vacina.Equals("Vaxxitek")) &&
                                    (caixaPlastica) && (vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                                else if ((!item.Embalagem.Equals("Plástica")) &&
                                         (item.Vacina.Equals("Vaxxitek")) &&
                                         (!caixaPlastica) && (vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                                else if ((item.Embalagem.Equals("Plástica")) &&
                                         (!item.Vacina.Equals("Vaxxitek")) &&
                                         (caixaPlastica) && (!vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                                else if ((!item.Embalagem.Equals("Plástica")) &&
                                         (!item.Vacina.Equals("Vaxxitek")) &&
                                         (!caixaPlastica) && (!vaxxitek))
                                {
                                    itemno = chic.items[i].item_no;
                                    accountno = chic.items[i].account_no;
                                    descricao = chic.items[i].item_desc;
                                }
                            }
                            quantity = item.QtdeReposicao;

                            price = 0;

                            if (tipoColabOvosBrasil.Equals("Participa Lista") &&
                                !item.OvosBrasil.Equals(1))
                            {
                                price = item.ValorUnitario + 0.0100m;
                            }
                            else
                            {
                                price = item.ValorUnitario;
                            }

                            booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                item.DataInicial, 0, String.Empty, 0);

                            tables.UpdateQuery(Convert.ToDecimal(booked_id));

                            log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção Produto" + "|" +
                                booked_id.ToString() + "|" +
                                cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                itemno + "|" +
                                descricao.Trim() + "|" +
                                quantity.ToString() + "|" +
                                price.ToString() + "|" +
                                accountno + "|" +
                                itemno + "\r\n";

                            #endregion

                            #region Item 03 - Vacinas
                            /**** Dados dos Itens do Pedido (booked) ****/
                            filtroVacina = "";

                            if (!item.Vacina.Equals(""))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                form = "VC";
                                varietyShort = "VACC";
                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";
                                filtroVacina = "";

                                filtroVacina = filtroVacina + ((!item.Vacina.Equals("Rispens") && !item.Vacina.Equals("Vaxxitek")) ? "HVT" : "");
                                filtroVacina = filtroVacina + (!filtroVacina.Equals("") ? "/RISP" : "RISP");
                                filtroVacina = filtroVacina + (item.Bouba.Equals(1) ? "/BOU" : "");
                                filtroVacina = filtroVacina + ((item.Gombouro.Equals(1) && !item.Vacina.Equals("Vaxxitek") && !item.Vacina.Equals("Vectormune")) ? "/GUMB" : "");
                                //filtroVacina = filtroVacina + (item.Coccidiose.Equals(1) ? "/COCC" : "");

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "03";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Vacina" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Coccidiose
                            if (item.Coccidiose.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                //filtroVacina = "APLIC VAC COCCIDIOSE";
                                if (item.Empresa.Equals("BR"))
                                    filtroVacina = "VAC COCCIDIOSE P/CONTA DA HY-LINE";
                                else if (item.Empresa.Equals("LB"))
                                    filtroVacina = "VAC COCCIDIOSE P/CONTA DA LOHMANN";
                                else
                                    filtroVacina = "VAC COCCIDIOSE P/ CONTA DA H&N";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "04";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Coccidiose" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Laringo
                            if (item.Laringo.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                filtroVacina = "APLIC VAC LARINGOTRAQUEITE";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "05";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Laringo" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }

                            // Salmonela
                            if (item.Salmonela.Equals(1))
                            {
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                filtroVacina = "";
                                //filtroVacina = "APLIC VAC CONTRA SALMONELLA";

                                if (item.Empresa.Equals("BR"))
                                    filtroVacina = "VAC SALMONELLA P/ CONTA HY LINE";
                                else if (item.Empresa.Equals("LB"))
                                    filtroVacina = "VAC SALMONELLA P/ CONTA LOHMANN";
                                else
                                    filtroVacina = "VAC SALMONELLA P/ CONTA  H&N";

                                items.FillByVacinas(chic.items, form, filtroVacina);

                                itemno = chic.items[0].item_no;
                                //accountno = chic.items[0].account_no;

                                item_ord = "06";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Salmonela" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    chic.items[0].item_desc.Trim() + "|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                            #endregion

                            #region Item 04 - Serviços

                            if (item.TratamentoInfravermelho.Equals(1))
                            {
                                /**** Dados dos Itens do Pedido (booked) ****/
                                tables.Fill(chic.tables);
                                booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                quantity = 0;
                                price = 0;

                                // Localizando o Item
                                itemno = "";
                                accountno = "";

                                itemno = "169";
                                //accountno = chic.items[0].account_no;

                                string comment_1 = "Tratar " + item.QtdePintinhosTratInfraVerm.ToString() + " pintos.";

                                item_ord = "06";

                                booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", comment_1, String.Empty,
                                    String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                    item.DataInicial, 0, String.Empty, 0);

                                tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                    Session["login"].ToString() + "|" +
                                    "Inserção Serviço" + "|" +
                                    booked_id.ToString() + "|" +
                                    cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                    itemno + "|" +
                                    "TRATAMENTO INFRAVERMELHO|" +
                                    quantity.ToString() + "|" +
                                    price.ToString() + "|" +
                                    accountno + "|" +
                                    itemno + "\r\n";
                            }
                            #endregion

                            #region Item 05 - Embalagem

                            if (existe == 0)
                            {
                                if (!item.Embalagem.Equals(""))
                                {
                                    /**** Dados dos Itens do Pedido (booked) ****/
                                    tables.Fill(chic.tables);
                                    booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                    quantity = 0;
                                    price = 0;

                                    // Localizando o Item
                                    itemno = "";
                                    accountno = "";

                                    itemno = item.Embalagem.Equals("Plástica") ? "601" : "602";
                                    //accountno = chic.items[0].account_no;
                                    items.FillByItemNo(chic.items, itemno);

                                    item_ord = "07";

                                    booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                        String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                        item.DataInicial, 0, String.Empty, 0);

                                    tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                    log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                        Session["login"].ToString() + "|" +
                                        "Inserção Embalagem" + "|" +
                                        booked_id.ToString() + "|" +
                                        cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                        itemno + "|" +
                                        chic.items[0].item_desc.Trim() + "|" +
                                        quantity.ToString() + "|" +
                                        price.ToString() + "|" +
                                        accountno + "|" +
                                        itemno + "\r\n";
                                }
                            }
                            #endregion

                            #region Item 06 - Hidratante

                            if (existe == 0)
                            {
                                if ((!item.Estado.Equals("SP")) &&
                                    (!item.Estado.Equals("RJ")) &&
                                    (!item.Estado.Equals("MG")) &&
                                    (!item.Estado.Equals("ES")) &&
                                    (!item.Estado.Equals("PR")) &&
                                    (!item.Estado.Equals("GO")))
                                {
                                    /**** Dados dos Itens do Pedido (booked) ****/
                                    tables.Fill(chic.tables);
                                    booked_id = Convert.ToInt32(chic.tables[0].lastno) + 1;

                                    quantity = 0;
                                    price = 0;

                                    // Localizando o Item
                                    itemno = "";
                                    accountno = "";

                                    itemno = "172";
                                    //accountno = chic.items[0].account_no;
                                    items.FillByItemNo(chic.items, itemno);

                                    item_ord = "08";

                                    booked.Insert(booked_id, cal_date, custoNo, itemno, quantity, price, orderNoReposicao, "O", String.Empty, String.Empty,
                                        String.Empty, "CH", accountno, String.Empty, item_ord, creatdby, datecrtd, creatdby, datecrtd,
                                        item.DataInicial, 0, String.Empty, 0);

                                    tables.UpdateQuery(Convert.ToDecimal(booked_id));

                                    log = log + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                        Session["login"].ToString() + "|" +
                                        "Inserção Hidratante" + "|" +
                                        booked_id.ToString() + "|" +
                                        cal_date.ToShortDateString() + " " + cal_date.ToShortTimeString() + "|" +
                                        itemno + "|" +
                                        chic.items[0].item_desc.Trim() + "|" +
                                        quantity.ToString() + "|" +
                                        price.ToString() + "|" +
                                        accountno + "|" +
                                        itemno + "\r\n";
                                }
                            }
                            #endregion

                            #region Inserir Tabela Customizada Pedido

                            intComm = "";

                            int_comm.FillByOrderNo(chic.int_comm, orderNoReposicao);

                            log = log +
                                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "|" +
                                Session["login"].ToString() + "|" +
                                "Inserção" + "|" +
                                "Ovos Brasil: Não";

                            if (chic.int_comm.Count > 0)
                                int_comm.DeleteQuery(orderNoReposicao);

                            intComm = item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString();
                            intComm = intComm + "\n\r\n\r Observação: " + item.Observacao;

                            if (tipoColabOvosBrasil.Equals("Participa Lista"))
                            {
                                int_comm.InsertQuery(orderNoReposicao, intComm, log, false, String.Empty, false,
                                    false, false, true, "", false, orderNoPrincipal, "");
                            }
                            else
                            {
                                int_comm.InsertQuery(orderNoReposicao, intComm, log, false, String.Empty, false,
                                    false, false, false, "", false, orderNoPrincipal, "");
                            }

                            #endregion
                        }

                        #endregion

                        #region LOG

                        LOG_LayoutPedidoPlanilhas logHLBAPP = new LOG_LayoutPedidoPlanilhas();

                        logHLBAPP.Usuario = Session["login"].ToString();
                        logHLBAPP.DataHora = DateTime.Now;

                        logHLBAPP.Bouba = item.Bouba;
                        logHLBAPP.Cidade = item.Cidade;
                        logHLBAPP.Coccidiose = item.Coccidiose;
                        logHLBAPP.CodigoCliente = item.CodigoCliente;
                        logHLBAPP.CondicaoPagamento = item.CondicaoPagamento;
                        logHLBAPP.DataFinal = item.DataFinal;
                        logHLBAPP.DataInicial = item.DataInicial;
                        logHLBAPP.DescricaoCliente = item.DescricaoCliente;
                        logHLBAPP.EmailVendedor = item.EmailVendedor;
                        logHLBAPP.Embalagem = item.Embalagem;
                        logHLBAPP.Empresa = item.Empresa;
                        logHLBAPP.Estado = item.Estado;
                        logHLBAPP.Gombouro = item.Gombouro;
                        logHLBAPP.Laringo = item.Laringo;
                        logHLBAPP.Linhagem = item.Linhagem;
                        logHLBAPP.NumeroPedidoCHIC = item.NumeroPedidoCHIC;
                        logHLBAPP.Observacao = item.Observacao;
                        logHLBAPP.Operacao = item.Operacao;
                        logHLBAPP.PercBonificacao = item.PercBonificacao;
                        logHLBAPP.QtdeBonificacao = item.QtdeBonificacao;
                        logHLBAPP.QtdeLiquida = item.QtdeLiquida;
                        logHLBAPP.QtdePintinhosTratInfraVerm = item.QtdePintinhosTratInfraVerm;
                        logHLBAPP.QtdeReposicao = item.QtdeReposicao;
                        logHLBAPP.QtdeTotal = item.QtdeTotal;
                        logHLBAPP.Salmonela = item.Salmonela;
                        logHLBAPP.TratamentoInfravermelho = item.TratamentoInfravermelho;
                        logHLBAPP.Vacina = item.Vacina;
                        if (tipoColabOvosBrasil.Equals("Participa Lista") &&
                            !item.OvosBrasil.Equals(1))
                        {
                            item.ValorUnitario = item.ValorUnitario + 0.0100m;
                            logHLBAPP.ValorTotal = item.QtdeLiquida * item.ValorUnitario;
                            logHLBAPP.OvosBrasil = 2;
                        }
                        else
                        {
                            logHLBAPP.ValorTotal = item.ValorTotal;
                            logHLBAPP.OvosBrasil = item.OvosBrasil;
                        }
                        logHLBAPP.ValorUnitario = item.ValorUnitario;
                        logHLBAPP.Vendedor = item.Vendedor;
                        logHLBAPP.MotivoOperacao = motivo;
                        logHLBAPP.CaminhoArquivo = caminho;

                        hlbapp.LOG_LayoutPedidoPlanilhas.AddObject(logHLBAPP);

                        //hlbapp.SaveChanges();

                        #endregion

                        mensagemErro = mensagemErro + "* Pedido " + orderNo + " - " +
                            item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString() +
                            " - " + item.Linhagem + " - Qtde.: " + item.QtdeLiquida.ToString() + " Valor Total: " +
                            item.ValorTotal.ToString() + " <br />";
                    }
                    #endregion

                    #region Cancelar
                    else if (item.Operacao.Equals("Cancelar"))
                    {
                        motivo = model["motivo"].ToString();

                        orderNo = item.NumeroPedidoCHIC.ToString();

                        #region Deleta Pedidos de Reposição

                        CHICDataSet.int_commDataTable intCommReposicaoDT = new CHICDataSet.int_commDataTable();

                        decimal orderNoPrincipal = Convert.ToDecimal(orderNo);

                        int_comm.FillByOrderNoMain(intCommReposicaoDT, orderNoPrincipal);

                        foreach (var itemReposicao in intCommReposicaoDT.ToList())
                        {
                            int_comm.DeleteQuery(itemReposicao.orderno);
                            booked.DeleteQuery(itemReposicao.orderno);
                            order.DeleteQuery(itemReposicao.orderno);
                        }

                        #endregion

                        int_comm.DeleteQuery(item.NumeroPedidoCHIC.ToString());
                        booked.DeleteQuery(item.NumeroPedidoCHIC.ToString());
                        order.DeleteQuery(item.NumeroPedidoCHIC.ToString());

                        #region LOG

                        LOG_LayoutPedidoPlanilhas logHLBAPP = new LOG_LayoutPedidoPlanilhas();

                        logHLBAPP.Usuario = Session["login"].ToString();
                        logHLBAPP.DataHora = DateTime.Now;

                        logHLBAPP.Bouba = item.Bouba;
                        logHLBAPP.Cidade = item.Cidade;
                        logHLBAPP.Coccidiose = item.Coccidiose;
                        logHLBAPP.CodigoCliente = item.CodigoCliente;
                        logHLBAPP.CondicaoPagamento = item.CondicaoPagamento;
                        logHLBAPP.DataFinal = item.DataFinal;
                        logHLBAPP.DataInicial = item.DataInicial;
                        logHLBAPP.DescricaoCliente = item.DescricaoCliente;
                        logHLBAPP.EmailVendedor = item.EmailVendedor;
                        logHLBAPP.Embalagem = item.Embalagem;
                        logHLBAPP.Empresa = item.Empresa;
                        logHLBAPP.Estado = item.Estado;
                        logHLBAPP.Gombouro = item.Gombouro;
                        logHLBAPP.Laringo = item.Laringo;
                        logHLBAPP.Linhagem = item.Linhagem;
                        logHLBAPP.NumeroPedidoCHIC = item.NumeroPedidoCHIC;
                        logHLBAPP.Observacao = item.Observacao;
                        logHLBAPP.Operacao = item.Operacao;
                        logHLBAPP.OvosBrasil = item.OvosBrasil;
                        logHLBAPP.PercBonificacao = item.PercBonificacao;
                        logHLBAPP.QtdeBonificacao = item.QtdeBonificacao;
                        logHLBAPP.QtdeLiquida = item.QtdeLiquida;
                        logHLBAPP.QtdePintinhosTratInfraVerm = item.QtdePintinhosTratInfraVerm;
                        logHLBAPP.QtdeReposicao = item.QtdeReposicao;
                        logHLBAPP.QtdeTotal = item.QtdeTotal;
                        logHLBAPP.Salmonela = item.Salmonela;
                        logHLBAPP.TratamentoInfravermelho = item.TratamentoInfravermelho;
                        logHLBAPP.Vacina = item.Vacina;
                        logHLBAPP.ValorTotal = item.ValorTotal;
                        logHLBAPP.ValorUnitario = item.ValorUnitario;
                        logHLBAPP.Vendedor = item.Vendedor;
                        logHLBAPP.MotivoOperacao = motivo;
                        logHLBAPP.CaminhoArquivo = caminho;

                        hlbapp.LOG_LayoutPedidoPlanilhas.AddObject(logHLBAPP);

                        //hlbapp.SaveChanges();

                        #endregion

                        mensagemErro = mensagemErro + "* Pedido " + orderNo + " - " +
                            item.DataInicial.ToShortDateString() + " à " + item.DataFinal.ToShortDateString() +
                            " - " + item.Linhagem + " - Qtde.: " + item.QtdeLiquida.ToString() + " Valor Total: " +
                            item.ValorTotal.ToString() + " <br />";
                    }
                    #endregion

                    #region Envio de E-mails - Antigo

                    //WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                    //email.WorkFlowEmailCopiaPara = "";

                    //string empresa = "";

                    //if (item.Empresa.Equals("BR")) { empresa = "HYLINE DO BRASIL"; }
                    //else if (item.Empresa.Equals("LB"))
                    //{
                    //    empresa = "LOHMANN DO BRASIL";
                    //    email.WorkFlowEmailCopiaPara = "lklassmann@ltz.com.br;esouza@ltz.com.br;";
                    //}
                    //else if (item.Empresa.Equals("HN")) { empresa = "H&N AVICULTURA"; }

                    //ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                    //apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    //email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    //email.WorkFlowEmailStat = "Enviar";
                    ////email.WorkFlowEmailAssunto = "**** LOGIN PARA ACESSO AO HY-LINE APP ****";
                    //email.WorkFlowEmailData = DateTime.Now;
                    //email.WorkFlowEmailParaNome = item.Vendedor;
                    //email.WorkFlowEmailParaEmail = item.EmailVendedor;
                    ////email.WorkFlowEmailParaNome = "Programação";
                    //email.WorkFlowEmailCopiaPara = email.WorkFlowEmailCopiaPara + "programacao@hyline.com.br";
                    //email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                    //email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                    //email.WorkFlowEmailFormato = "Texto";

                    //string corpoEmail = "";

                    //string corpoOperacao = "";

                    //if (item.Operacao.Equals("Incluir Novo"))
                    //{
                    //    email.WorkFlowEmailAssunto = "**** PEDIDO " + orderNo + " INCLUÍDO conforme planilha em Anexo ****";
                    //    corpoOperacao = "A planilha enviada em anexo foi importada para o CHIC com o número de Pedido " + orderNo + "." + (char)13 + (char)10
                    //        + "Guarde esse número para que possa ter o controle, inclusive caso faça alguma alteração, é necessário este número." + (char)13 + (char)10;
                    //}
                    //else if (item.Operacao.Equals("Alterar"))
                    //{
                    //    email.WorkFlowEmailAssunto = "**** PEDIDO " + orderNo + " ALTERADO conforme planilha em Anexo ****";
                    //    corpoOperacao = "A alteração do Pedido " + orderNo + " conforme planilha em anexo foi importada no CHIC." + (char)13 + (char)10;
                    //}
                    //else if (item.Operacao.Equals("Cancelar"))
                    //{
                    //    email.WorkFlowEmailAssunto = "**** PEDIDO " + orderNo + " CANCELADO conforme planilha em Anexo ****";
                    //    corpoOperacao = "O Cancelamento do Pedido " + orderNo + " conforme planilha em anexo foi realizado." + (char)13 + (char)10;
                    //}

                    //string stringChar = "" + (char)13 + (char)10;

                    //corpoEmail = "Prezado " + item.Vendedor + "," + (char)13 + (char)10 + (char)13 + (char)10
                    //    + corpoOperacao
                    //    + "Segue abaixo detalhes:" + (char)13 + (char)10 + (char)13 + (char)10
                    //    + empresa + (char)13 + (char)10 + (char)13 + (char)10
                    //    + mensagemErro.Replace("<br />", stringChar) + (char)13 + (char)10 + (char)13 + (char)10
                    //    + "Qualquer dúvida, entrar em contato pelo e-mail programacao@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                    //    + "SISTEMA WEB";

                    //email.WorkFlowEmailCorpo = corpoEmail;
                    //email.WorkFlowEmailArquivosAnexos = caminho;

                    //apolo.WORKFLOW_EMAIL.AddObject(email);

                    //apolo.SaveChanges();

                    #endregion

                    #region Envio de E-mails - Novo

                    if (item.Empresa.Equals("BR")) 
                    { 
                        empresa = "HYLINE DO BRASIL";
                        copiaPara = "confirmacoes@hyline.com.br;";
                    }
                    else if (item.Empresa.Equals("LB"))
                    {
                        empresa = "LOHMANN DO BRASIL";
                        copiaPara = "confirmacoes@ltz.com.br;";
                    }
                    else if (item.Empresa.Equals("HN")) { empresa = "H&N AVICULTURA"; }

                    nomeVendedor = item.Vendedor;
                    emailVendedor = item.EmailVendedor;

                    operacao = item.Operacao;

                    if (operacao.Equals("Cancelar") || operacao.Equals("Alterar"))
                        motivo = " - Motivo da operação " + operacao + ": " + motivo;

                    corpoOperacao = corpoOperacao + "Pedido " + orderNo + " - " + item.Linhagem + (char)13 + (char)10
                            + " - Qtde.Bonificada: " + item.QtdeBonificacao.ToString() + (char)13 + (char)10
                            + " - Qtde.Reposição: " + item.QtdeReposicao.ToString() + (char)13 + (char)10
                            + " - Qtde.Total: " + item.QtdeTotal.ToString() + (char)13 + (char)10
                            + " - Valor Unitário: " + item.ValorUnitario.ToString() + (char)13 + (char)10
                            + " - Valor Total: " + item.ValorTotal.ToString() + (char)13 + (char)10;

                    if (tipoColabOvosBrasil.Equals("Participa Lista") &&
                            !item.OvosBrasil.Equals(1))
                    {
                        corpoOperacao = corpoOperacao + "OBS: Adicionado R$ 0,01 a mais no valor porque cliente assinou a lista de Ovos Brasil." + (char)13 + (char)10;
                    }

                    corpoOperacao = corpoOperacao + motivo + (char)13 + (char)10
                            + (char)13 + (char)10 + (char)13 + (char)10;
                    
                    #endregion
                }

                #region Envio de E-mails - Novo Geral

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                email.WorkFlowEmailCopiaPara = copiaPara;

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                //email.WorkFlowEmailAssunto = "**** LOGIN PARA ACESSO AO HY-LINE APP ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = nomeVendedor;
                email.WorkFlowEmailParaEmail = emailVendedor;
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailParaNome = "Programação";
                email.WorkFlowEmailCopiaPara = email.WorkFlowEmailCopiaPara + "programacao@hyline.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                if (operacao.Equals("Incluir Novo"))
                {
                    email.WorkFlowEmailAssunto = "**** PLANILHA EM ANEXO " + orderNo + " INCLUÍDA ****";
                    corpoOperacao = "A planilha enviada em anexo foi importada para o CHIC e gerou os seguintes pedidos: " + (char)13 + (char)10
                        + (char)13 + (char)10 + corpoOperacao;
                }
                else if (operacao.Equals("Alterar"))
                {
                    email.WorkFlowEmailAssunto = "**** PLANILHA EM ANEXO " + orderNo + " ALTERADA ****";
                    corpoOperacao = "A planilha enviada em anexo foi importada para o CHIC e alterou os seguintes pedidos: " + (char)13 + (char)10
                       + (char)13 + (char)10 + corpoOperacao;
                }
                else if (operacao.Equals("Cancelar"))
                {
                    email.WorkFlowEmailAssunto = "**** PLANILHA EM ANEXO " + orderNo + " CANCELADA ****";
                    corpoOperacao = "A planilha enviada em anexo foi importada para o CHIC e cancelou os seguintes pedidos: " + (char)13 + (char)10
                       + (char)13 + (char)10 + corpoOperacao;
                }

                string stringChar = "" + (char)13 + (char)10;

                corpoEmail = "Prezado " + nomeVendedor + "," + (char)13 + (char)10 + (char)13 + (char)10
                    + corpoOperacao
                    + "Qualquer dúvida, entrar em contato pelo e-mail programacao@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = caminho;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                ViewBag.fileName = mensagemErro;
                bd.Database.ExecuteSqlCommand("delete from LayoutPedidoPlanilhas");
                bd.SaveChanges();

                hlbapp.SaveChanges();

                var listaRetorno = bd.PedidoPlanilha.ToList();

                return View("Index", listaRetorno);
            }
            catch (Exception e)
            {
                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao realizar a importação: " + e.Message;
                return View("Index", "");
            }
        }

        [HttpPost]
        public ActionResult EnviaEmailErro()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            try
            {
                LayoutPedidoPlanilha pedido = bd.PedidoPlanilha.First();

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                string empresa = "";

                if (pedido.Empresa.Equals("BR")) { empresa = "[HLB - Erro]"; }
                else if (pedido.Empresa.Equals("LB")) { empresa = "[LTZ - Erro]"; }
                else if (pedido.Empresa.Equals("HN")) { empresa = "[H&N - Erro]"; }

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = empresa + " ERRO AO IMPORTAR PLANILHA";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = pedido.Vendedor;
                email.WorkFlowEmailParaEmail = pedido.EmailVendedor;
                email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                string corpoEmail = "";

                string stringChar = "" + (char)13 + (char)10;

                corpoEmail = "Prezado " + pedido.Vendedor + "," + (char)13 + (char)10 + (char)13 + (char)10
                    + "A planilha enviada para " + pedido.Operacao + " pedido " + pedido.NumeroPedidoCHIC.ToString() + " em anexo foi importada com erros." + (char)13 + (char)10
                    + "Segue abaixo detalhes:" + (char)13 + (char)10 + (char)13 + (char)10
                    + mensagemErro.Replace("<br />", stringChar) + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato pelo e-mail programacao@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = caminho;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                var listaRetorno = bd.PedidoPlanilha.ToList();

                #region LOG

                LOG_LayoutPedidoPlanilhas logHLBAPP = new LOG_LayoutPedidoPlanilhas();

                logHLBAPP.Usuario = Session["login"].ToString();
                logHLBAPP.DataHora = DateTime.Now;

                logHLBAPP.Bouba = pedido.Bouba;
                logHLBAPP.Cidade = pedido.Cidade;
                logHLBAPP.Coccidiose = pedido.Coccidiose;
                logHLBAPP.CodigoCliente = pedido.CodigoCliente;
                logHLBAPP.CondicaoPagamento = pedido.CondicaoPagamento;
                logHLBAPP.DataFinal = pedido.DataFinal;
                logHLBAPP.DataInicial = pedido.DataInicial;
                logHLBAPP.DescricaoCliente = pedido.DescricaoCliente;
                logHLBAPP.EmailVendedor = pedido.EmailVendedor;
                logHLBAPP.Embalagem = pedido.Embalagem;
                logHLBAPP.Empresa = pedido.Empresa;
                logHLBAPP.Estado = pedido.Estado;
                logHLBAPP.Gombouro = pedido.Gombouro;
                logHLBAPP.Laringo = pedido.Laringo;
                logHLBAPP.Linhagem = pedido.Linhagem;
                logHLBAPP.NumeroPedidoCHIC = pedido.NumeroPedidoCHIC;
                logHLBAPP.Observacao = pedido.Observacao;
                logHLBAPP.Operacao = "Envio Email - Erros";
                logHLBAPP.OvosBrasil = pedido.OvosBrasil;
                logHLBAPP.PercBonificacao = pedido.PercBonificacao;
                logHLBAPP.QtdeBonificacao = pedido.QtdeBonificacao;
                logHLBAPP.QtdeLiquida = pedido.QtdeLiquida;
                logHLBAPP.QtdePintinhosTratInfraVerm = pedido.QtdePintinhosTratInfraVerm;
                logHLBAPP.QtdeReposicao = pedido.QtdeReposicao;
                logHLBAPP.QtdeTotal = pedido.QtdeTotal;
                logHLBAPP.Salmonela = pedido.Salmonela;
                logHLBAPP.TratamentoInfravermelho = pedido.TratamentoInfravermelho;
                logHLBAPP.Vacina = pedido.Vacina;
                logHLBAPP.ValorTotal = pedido.ValorTotal;
                logHLBAPP.ValorUnitario = pedido.ValorUnitario;
                logHLBAPP.Vendedor = pedido.Vendedor;
                logHLBAPP.MotivoOperacao = mensagemErro.Replace("<br />", stringChar);
                logHLBAPP.CaminhoArquivo = caminho;

                hlbapp.LOG_LayoutPedidoPlanilhas.AddObject(logHLBAPP);

                hlbapp.SaveChanges();

                #endregion

                ViewBag.mensagemEnvio = "E-mail enviado com sucesso!";

                return View("Index", listaRetorno);
            }
            catch (Exception e)
            {
                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao enviar e-mail: " + e.Message;
                return View("Index", "");
            }
        }

        public ActionResult Confirmacoes()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string opcaoData = "Nascimento";
            DateTime dataInicial = DateTime.Today;
            DateTime dataFinal = DateTime.Today;
            string cliente = "00";
            string vendedor = "";
            string empresa = "";
            string status = "";

            CarregaListaEstados();
            CarregaListaVendedores();
            Session["descricaoConf"] = "";
            Session["estadoConf"] = "";
            Session["marcadoConf"] = cliente;
            Session["vendedorConf"] = vendedor;
            Session["sTipoDataConf"] = opcaoData;
            Session["sEmpresaConf"] = empresa;
            Session["sDataInicialConf"] = dataInicial.ToShortDateString();
            Session["sDataFinalConf"] = dataFinal.ToShortDateString();
            Session["sDataInicialFat"] = dataInicial.AddDays(1).ToShortDateString();
            Session["sDataFinalFat"] = dataFinal.AddDays(1).ToShortDateString();
            List<Cliente> listaExibeClientes = null;
            Session["ListaClientesConf"] = listaExibeClientes;
            Session["sTipoAgrupamentoPedidos"] = "";
            Session["sEnviarEmailEmpresa"] = "";
            Session["sEnviarEmailVendedor"] = "";
            CarregaEmpresasVendedores();

            ordersConf.FillByConf(chic.ordersConf, opcaoData, dataInicial, dataFinal, 
                opcaoData, dataInicial, dataFinal, opcaoData, dataInicial,
                dataFinal, cliente, cliente, vendedor, vendedor, empresa, empresa, false);

            //CHICDataSet.ordersConfDataTable ordersconf = new CHICDataSet.ordersConfDataTable();

            //string retorno = GeraRelConfirmacao("52980", "Teste");

            return View("Confirmacoes", chic.ordersConf);
        }

        public void CarregaListaEstados()
        {
            List<SelectListItem> listaEstados = new List<SelectListItem>();

            var lista = apolo2.CIDADE.GroupBy(g => g.UfSigla).OrderBy(o => o.Key).ToList();

            listaEstados.Add(new SelectListItem { Text = "(Todos)", Value = "(Todos)", Selected = true });

            foreach (var item in lista)
            {
                listaEstados.Add(new SelectListItem { Text = item.Key, Value = item.Key, Selected = false });
            }

            Session["ListaEstadosConf"] = listaEstados;
        }

        public void AtualizaEstadoSelecionado(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaEstadosConf"];

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

            Session["ListaEstadosConf"] = estados;
        }

        public void CarregaListaVendedores()
        {
            List<SelectListItem> listaVendedores = new List<SelectListItem>();

            salesman.Fill(chic.salesman);

            var lista = chic.salesman;

            listaVendedores.Add(new SelectListItem { Text = "(Todos)", Value = "", Selected = true });

            foreach (var item in lista.OrderBy(o => o.salesman).ToList())
            {
                listaVendedores.Add(new SelectListItem { Text = item.salesman, Value = item.sl_code, Selected = false });
            }

            Session["ListaVendedoresConf"] = listaVendedores;
        }

        public void AtualizaVendedorSelecionado(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaVendedoresConf"];

            foreach (var item in estados)
            {
                if (item.Value == modelo)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaVendedoresConf"] = estados;
        }

        public void CarregaEmpresasVendedores()
        {
            List<SelectListItem> listaEmpresas = new List<SelectListItem>();
            List<SelectListItem> listaVendedores = new List<SelectListItem>();

            if (Session["empresa"].ToString().Length > 2)
            {
                listaEmpresas.Add(new SelectListItem
                {
                    Text = "(Todas)",
                    Value = "(Todas)",
                    Selected = true
                });
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoListaVendedoresRelComercial",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                listaVendedores.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                listaEmpresas.Add(new SelectListItem
                {
                    Text = Session["empresa"].ToString().Substring(i, 2),
                    Value = Session["empresa"].ToString().Substring(i, 2),
                    Selected = false
                });

                if (MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-AcessoListaVendedoresRelComercial",
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    CHICDataSet.salesmanDataTable vendedores = new CHICDataSet.salesmanDataTable();
                    salesman.FillByEmpresa(vendedores, Session["empresa"].ToString().Substring(i, 2));

                    foreach (var item in vendedores)
                    {
                        listaVendedores.Add(new SelectListItem
                        {
                            Text = item.inv_comp.Trim() + " - " + item.sl_code.Trim() + " - "
                                + item.salesman.Trim(),
                            Value = item.sl_code.Trim(),
                            Selected = false
                        });
                    }
                }
            }

            Session["ListaEmpresasRelComercial"] = listaEmpresas;
            Session["ListaVendedoresRelComercial"] = listaVendedores;
        }

        public void AtualizaEmpresaSelecionada(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaEmpresasRelComercial"];

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

            Session["ListaEmpresasRelComercial"] = estados;
        }

        public List<Cliente> ListaClientes(string descricao, string Text)
        {
            string empresa = Session["empresaApolo"].ToString();

            AtualizaEstadoSelecionado(Text);
            Session["descricaoConf"] = descricao;
            Session["estadoConf"] = Text;

            var listaClientes = apolo2.ENTIDADE
                .Where(e => apolo2.ENT_CATEG.Any(c => c.EntCod == e.EntCod && (c.CategCodEstr == "01" || c.CategCodEstr == "01.01"))
                    && (e.EntNome.Contains(descricao) || e.EntNomeFant.Contains(descricao)) && e.StatEntCod != "05"
                    && apolo2.VEND_ENT.Any(ve => ve.EntCod == e.EntCod &&
                        apolo2.VENDEDOR.Any(v => v.VendCod == ve.VendCod && (v.USEREmpresa == empresa || empresa == "TODAS"))))
                //&& bdApolo.CIDADE.Any(cid => cid.CidCod == e.CidCod && cid.CidNomeComp == cidade && cid.UfSigla == estado))
                .Join(apolo2.CIDADE.Where(cid => cid.UfSigla == Text || Text == "(Todos)"),
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

            if (listaClientes.Count == 1) Session["marcadoConf"] = listaClientes.FirstOrDefault().EntCod;

            Session["ListaClientesConf"] = listaExibeClientes;

            return listaExibeClientes;
        }

        public ActionResult ListaClientesConferencia(string descricao, string Text)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string opcaoData = Session["sTipoDataConf"].ToString();
            DateTime dataInicial = Convert.ToDateTime(Session["sDataInicialConf"]);
            DateTime dataFinal = Convert.ToDateTime(Session["sDataFinalConf"]);
            string cliente = Session["marcadoConf"].ToString();
            string vendedor = Session["vendedorConf"].ToString();
            string empresa = Session["sEmpresaConf"].ToString();
            string status = "";

            ordersConf.FillByConf(chic.ordersConf, opcaoData, dataInicial, dataFinal
                , opcaoData, dataInicial, dataFinal, opcaoData, dataInicial,
                dataFinal, cliente, cliente, vendedor, vendedor, empresa, empresa, false);

            ListaClientes(descricao, Text);

            return View("Confirmacoes", chic.ordersConf);
        }

        [HttpPost]
        public ActionResult ListaPedidos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            List<Cliente> listaExibeClientes = null;
            string vendedor = model["Text"];
            Session["vendedorConf"] = vendedor;
            AtualizaVendedorSelecionado(vendedor);

            if (model["empresa"] != null)
                Session["sEmpresaConf"] = model["empresa"].ToString();
            else
            {
                ViewBag.erro = "Por favor, selecionar a opção de Empresa!";
                return View("Confirmacoes", chic.ordersConf);
            }

            if (model["dataIni"] != null)
                Session["sDataInicialConf"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("Confirmacoes", chic.ordersConf);
            }

            if (model["dataFim"] != null)
                Session["sDataFinalConf"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("Confirmacoes", chic.ordersConf);
            }

            if (model["tipoData"] != null)
                Session["sTipoDataConf"] = model["tipoData"].ToString();
            else
            {
                ViewBag.erro = "Por favor, selecionar o Tipo de Data!";
                return View("Confirmacoes", chic.ordersConf);
            }

            string empresa = model["empresa"];
            string opcaoData = model["tipoData"];
            DateTime dataInicial = Convert.ToDateTime(model["dataIni"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFim"].ToString());
            string cliente = "";
            if (model["clienteSelecionado"] != null)
            {
                cliente = model["clienteSelecionado"];
                Session["marcadoConf"] = cliente;
            }
            string status = "";

            ordersConf.FillByConf(chic.ordersConf, opcaoData, dataInicial, dataFinal
                , opcaoData, dataInicial, dataFinal, opcaoData, dataInicial,
                dataFinal, cliente, cliente, vendedor, vendedor, empresa, empresa, false);

            return View("Confirmacoes", chic.ordersConf);
        }

        public string GeraRelConfirmacao(string orderNo, string cliente, string empresa)
        {
            //string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõçZÁÉÍÓÚÀÈÌÒÙÂÊÎÔÛÃÕÇ\s]";
            //string replacement = "";
            //Regex rgx = new Regex(pattern);
            //string nameFileOld = cliente.Replace("\\", "").Replace("/", "");
            //string nameFileNew = rgx.Replace(nameFileOld, replacement);

            string pattern = @"(?i)[^0-9a-z\s]";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string nameFileOld = cliente.Replace("\\", "").Replace("/", "");
            string nameFileNew = rgx.Replace(nameFileOld, replacement);

            string pattern2 = @"(?i)[^0-9a-z]";
            Regex rgx2 = new Regex(pattern2);
            string replacement2 = "_";
            string nameFileNew2 = rgx2.Replace(nameFileNew, replacement2);

            //string caminho = @"\\srv-fls-03\W\Conf\" + cliente.Replace("\\","").Replace("/","") + "_" + orderNo + ".pdf";
            //string caminho = @"\\srv-fls-03\W\Conf\Pedido_" + orderNo + ".pdf";
            string caminho = @"\\srv-fls-03\W\Conf\" + nameFileNew2 + "_" + orderNo + ".pdf";

            CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //MyReport.Load(Server.MapPath("~/Views/ImportaPedidosCHIC/ConfirmacaoPedido_" + empresa + ".rpt"));
            MyReport.Load("C:\\inetpub\\wwwroot\\Relatorios\\Crystal\\ConfirmacaoPedido_" + empresa + ".rpt");
            MyReport.SetParameterValue("@pPedido", orderNo);
            MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, caminho);

            MyReport.Close();
            MyReport.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            return caminho;
        }

        public string EnviaConfirmacaoEmail(string caminho, string enderecoEmail, string representante, string orderno, 
            string cliente, string copiaPara, string corpo, string assunto)
        {
            try
            {
                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                //email.WorkFlowEmailAssunto = " **TESTE ** - CONFIRMAÇÃO DO PEDIDO " + orderno + " - " + cliente;
                email.WorkFlowEmailAssunto = assunto;
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = representante;
                email.WorkFlowEmailParaEmail = enderecoEmail;
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //if (copiaPara != "")
                //    email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br;" + copiaPara;
                //else
                //    email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                email.WorkFlowEmailCopiaPara = copiaPara;
                email.WorkFlowEmailDeNome = "Sistema WEB";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                string corpoEmail = "";

                string stringChar = "" + (char)13 + (char)10;

                corpoEmail = "Prezado " + representante + "," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo a Confirmação do pedido " + orderno + " do cliente " + cliente + "." + (char)13 + (char)10
                    + "Por favor, retornar assinado para concluir a venda!" + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato pelo e-mail programacao@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                //email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailCorpo = corpo;
                email.WorkFlowEmailArquivosAnexos = caminho;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #region LOG

                LOG_LayoutPedidoPlanilhas logHLBAPP = new LOG_LayoutPedidoPlanilhas();

                logHLBAPP.Usuario = Session["login"].ToString();
                logHLBAPP.DataHora = DateTime.Now;

                logHLBAPP.Bouba = null;
                logHLBAPP.Cidade = null;
                logHLBAPP.Coccidiose = null;
                logHLBAPP.CodigoCliente = null;
                logHLBAPP.CondicaoPagamento = null;
                logHLBAPP.DataFinal = null;
                logHLBAPP.DataInicial = null;
                logHLBAPP.DescricaoCliente = cliente;
                logHLBAPP.EmailVendedor = enderecoEmail;
                logHLBAPP.Embalagem = null;
                logHLBAPP.Empresa = null;
                logHLBAPP.Estado = null;
                logHLBAPP.Gombouro = null;
                logHLBAPP.Laringo = null;
                logHLBAPP.Linhagem = null;
                logHLBAPP.NumeroPedidoCHIC = Convert.ToInt32(orderno);
                logHLBAPP.Observacao = "";
                logHLBAPP.Operacao = "Envio de Confirmação";
                logHLBAPP.OvosBrasil = null;
                logHLBAPP.PercBonificacao = null;
                logHLBAPP.QtdeBonificacao = null;
                logHLBAPP.QtdeLiquida = null;
                logHLBAPP.QtdePintinhosTratInfraVerm = null;
                logHLBAPP.QtdeReposicao = null;
                logHLBAPP.QtdeTotal = null;
                logHLBAPP.Salmonela = null;
                logHLBAPP.TratamentoInfravermelho = null;
                logHLBAPP.Vacina = null;
                logHLBAPP.ValorTotal = null;
                logHLBAPP.ValorUnitario = null;
                logHLBAPP.Vendedor = representante;
                logHLBAPP.MotivoOperacao = null;
                logHLBAPP.CaminhoArquivo = caminho;

                hlbapp.LOG_LayoutPedidoPlanilhas.AddObject(logHLBAPP);

                hlbapp.SaveChanges();

                #endregion

                return "";
            }
            catch (Exception e)
            {
                return "Erro ao enviar e-mail: " + e.Message;
            }
        }

        public ActionResult EnviaConfirmacoes(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            try
            {
                System.Globalization.CultureInfo calendarioLocal = new CultureInfo("PT-BR");
                int semanaDoAnoInicial = calendarioLocal.Calendar.GetWeekOfYear(Convert.ToDateTime(Session["sDataInicialConf"].ToString()),
                    System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                    System.DayOfWeek.Sunday);
                int semanaDoAnoFinal = calendarioLocal.Calendar.GetWeekOfYear(Convert.ToDateTime(Session["sDataFinalConf"].ToString()),
                    System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                    System.DayOfWeek.Sunday);

                DateTime dataInicial = Convert.ToDateTime(Session["sDataInicialConf"].ToString());
                DateTime dataFinal = Convert.ToDateTime(Session["sDataFinalConf"].ToString());

                string tipoAgrupamentoPedidos = "";
                string enviarEmailEmpresa = "";
                string enviarEmailVendedor = "";

                if (model["tipoAgrupamentoPedidos"] != null)
                {
                    tipoAgrupamentoPedidos = model["tipoAgrupamentoPedidos"];
                    Session["sTipoAgrupamentoPedidos"] = tipoAgrupamentoPedidos;
                }
                else
                {
                    ViewBag.erro = "Por favor, selecionar o Tipo de Agrupamento dos Pedidos para Enviar por E-mail!";
                    return View("Confirmacoes", chic.ordersConf);
                }

                if ((model["enviarEmailEmpresa"] != null) || (model["enviarEmailVendedor"] != null))
                {
                    enviarEmailEmpresa = model["enviarEmailEmpresa"].Replace("true,false", "true");
                    Session["sEnviarEmailEmpresa"] = enviarEmailEmpresa;
                    enviarEmailVendedor = model["enviarEmailVendedor"].Replace("true,false", "true");
                    Session["sEnviarEmailVendedor"] = enviarEmailVendedor;
                }
                else
                {
                    ViewBag.erro = "Por favor, selecione pelo menos uma opção para o Envio do E-mail!";
                    return View("Confirmacoes", chic.ordersConf);
                }

                string anexos = "";
                string erro = "";

                string empresaAnterior = "";
                string vendedorAnterior = "";

                string assuntoEmail = "";
                string corpoEmail = "";
                string emailRetorno = "";
                
                string emailGrupoEmpresa = "";
                if ((Session["sEmpresaConf"].ToString().Equals("BR")) && (enviarEmailEmpresa.Equals("true")))
                {
                    emailGrupoEmpresa = "confirmacoes@hyline.com.br";
                    emailRetorno = "programacao@hyline.com.br";
                }
                else if ((Session["sEmpresaConf"].ToString().Equals("LB")) && (enviarEmailEmpresa.Equals("true")))
                {
                    emailGrupoEmpresa = "confirmacoes@ltz.com.br";
                    emailRetorno = "programacao@ltz.com.br";
                }
                else if ((Session["sEmpresaConf"].ToString().Equals("HN")) && (enviarEmailEmpresa.Equals("true")))
                {
                    emailGrupoEmpresa = "confirmacoes@hnavicultura.com.br";
                    emailRetorno = "programacao@hnavicultura.com.br";
                }
                else if ((Session["sEmpresaConf"].ToString().Equals("PL")) && (enviarEmailEmpresa.Equals("true")))
                {
                    emailGrupoEmpresa = "programacao@planaltopostura.com.br";
                    emailRetorno = "programacao@planaltopostura.com.br";
                }
                else if (enviarEmailEmpresa.Equals("true"))
                {
                    ViewBag.erro = "Para enviar e-mail para o Grupo de Empresas, é necessário selecionar pelo menos uma " 
                        + "no filtro dos Pedidos para confirmação!";
                    return View("Confirmacoes", chic.ordersConf);
                }

                var fileIds = model["id"].Split(',');
                var selectedIndices = model["importa"].Replace("true,false", "true")
                            .Split(',')
                            .Select((item, index) => new { item = item, index = index })
                            .Where(row => row.item == "true")
                            .Select(row => row.index).ToArray();
                //if (1 == 2)
                //{
                    foreach (var index in selectedIndices)
                    {
                        int fileId;
                        if (int.TryParse(fileIds[index], out fileId))
                        {
                            string orderno = fileId.ToString();

                            order.FillByOrderNo(chic.orders, orderno);
                            cust.FillByCustoNo(chic.cust, chic.orders[0].cust_no);
                            salesman.FillByCode(chic.salesman, chic.orders[0].salesrep.Trim());

                            string caminho = GeraRelConfirmacao(orderno, chic.cust[0].name.Trim(), chic.salesman[0].inv_comp.Trim());

                            if (tipoAgrupamentoPedidos.Equals("Pedido"))
                            {
                                assuntoEmail = "CONFIRMAÇÃO DO PEDIDO " + orderno + " - " + chic.cust[0].name.Trim();

                                corpoEmail = "Prezado " + chic.salesman[0].salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                                    + "Segue em anexo a Confirmação do pedido " + orderno + " do cliente " + chic.cust[0].name.Trim() + "." + (char)13 + (char)10
                                    + "Por favor, retornar assinado para concluir a venda!" + (char)13 + (char)10 + (char)13 + (char)10
                                    + "Qualquer dúvida, entrar em contato pelo e-mail " + emailRetorno + "." + (char)13 + (char)10 + (char)13 + (char)10
                                    + "SISTEMA WEB";

                                if (enviarEmailVendedor == "true")
                                    erro = EnviaConfirmacaoEmail(caminho, chic.salesman[0].email.Trim(), chic.salesman[0].salesman.Trim(),
                                        orderno, chic.cust[0].name.Trim(), emailGrupoEmpresa + ";" + emailRetorno, 
                                        corpoEmail, assuntoEmail);
                                else
                                    erro = EnviaConfirmacaoEmail(caminho, emailGrupoEmpresa, emailGrupoEmpresa,
                                        orderno, chic.cust[0].name.Trim(), emailRetorno, corpoEmail, assuntoEmail);
                            }
                            else if (tipoAgrupamentoPedidos.Equals("Vendedor"))
                            {
                                if ((chic.salesman[0].sl_code.Trim() == vendedorAnterior) || (vendedorAnterior == ""))
                                {
                                    if (vendedorAnterior == "")
                                        anexos = caminho;
                                    else
                                        anexos = anexos + "^" + caminho;
                                }
                                else
                                {
                                    CHICDataSet.salesmanDataTable vendedorAnteriorDT = new CHICDataSet.salesmanDataTable();

                                    salesman.FillByCode(vendedorAnteriorDT, vendedorAnterior);

                                    //assuntoEmail = "CONFIRMAÇÕES SEMANAS " 
                                    //    + semanaDoAnoInicial.ToString() + "/" 
                                    //    + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy")
                                    //    + " a "
                                    //    + semanaDoAnoFinal.ToString() + "/" 
                                    //    + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToString("yyyy");

                                    assuntoEmail = "CONFIRMAÇÕES DE "
                                        + dataInicial.ToShortDateString() + " a " 
                                        + dataFinal.ToShortDateString();

                                    corpoEmail = "Prezado " + vendedorAnteriorDT[0].salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                                       + "Segue em anexo as confirmações dos lotes programados no período das semanas de "
                                        + semanaDoAnoInicial.ToString() + " a " + semanaDoAnoFinal.ToString() + "/"
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy") + " ("
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToShortDateString() + " a "
                                        + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToShortDateString()
                                        + ") conforme as disponibilidades de produto,espaço de incubação e rota."
                                        + (char)13 + (char)10
                                        + "Qualquer dúvida, entrar em contato pelo e-mail " + emailRetorno + "." + (char)13 + (char)10 + (char)13 + (char)10
                                        + "SISTEMA WEB";

                                    if (enviarEmailVendedor == "true")
                                        erro = EnviaConfirmacaoEmail(anexos, vendedorAnteriorDT[0].email.Trim(), vendedorAnteriorDT[0].salesman.Trim(),
                                            orderno, chic.cust[0].name.Trim(), emailGrupoEmpresa + ";" + emailRetorno, 
                                            corpoEmail, assuntoEmail);
                                    else
                                        erro = EnviaConfirmacaoEmail(anexos, emailGrupoEmpresa, emailGrupoEmpresa,
                                            orderno, chic.cust[0].name.Trim(), emailRetorno, corpoEmail, assuntoEmail);

                                    anexos = caminho;
                                }
                            }
                            else if (tipoAgrupamentoPedidos.Equals("Empresa"))
                            {
                                if ((chic.salesman[0].inv_comp.Trim() == empresaAnterior) || (empresaAnterior == ""))
                                {
                                    if (empresaAnterior == "")
                                        anexos = caminho;
                                    else
                                        anexos = anexos + "^" + caminho;
                                }
                                else
                                {
                                    //assuntoEmail = "CONFIRMAÇÕES SEMANAS "
                                    //    + semanaDoAnoInicial.ToString() + "/" + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy")
                                    //    + " a "
                                    //    + semanaDoAnoFinal.ToString() + "/" + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToString("yyyy");

                                    assuntoEmail = "CONFIRMAÇÕES DE "
                                        + dataInicial.ToShortDateString() + " a " 
                                        + dataFinal.ToShortDateString();

                                    corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                                        + "Segue em anexo as confirmações dos lotes programados no período das semanas de "
                                        + semanaDoAnoInicial.ToString() + " a " + semanaDoAnoFinal.ToString() + "/"
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy") + " ("
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToShortDateString() + " a "
                                        + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToShortDateString()
                                        + ") conforme as disponibilidades de produto,espaço de incubação e rota."
                                        + (char)13 + (char)10
                                        + "Qualquer dúvida, entrar em contato pelo e-mail " + emailRetorno + "." + (char)13 + (char)10 + (char)13 + (char)10
                                        + "SISTEMA WEB";

                                    erro = EnviaConfirmacaoEmail(anexos, emailGrupoEmpresa, emailGrupoEmpresa,
                                            orderno, chic.cust[0].name.Trim(), emailRetorno, corpoEmail, assuntoEmail);

                                    anexos = caminho;
                                }
                            }

                            if (index == selectedIndices.Last())
                            {
                                if (tipoAgrupamentoPedidos.Equals("Pedido"))
                                {
                                    assuntoEmail = "CONFIRMAÇÃO DO PEDIDO " + orderno + " - " + chic.cust[0].name.Trim();

                                    corpoEmail = "Prezado " + chic.salesman[0].salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                                        + "Segue em anexo a Confirmação do pedido " + orderno + " do cliente " + chic.cust[0].name.Trim() + "." + (char)13 + (char)10
                                        + "Por favor, retornar assinado para concluir a venda!" + (char)13 + (char)10 + (char)13 + (char)10
                                        + "Qualquer dúvida, entrar em contato pelo e-mail " + emailRetorno + "." + (char)13 + (char)10 + (char)13 + (char)10
                                        + "SISTEMA WEB";

                                    if (enviarEmailVendedor == "true")
                                        erro = EnviaConfirmacaoEmail(caminho, chic.salesman[0].email.Trim(), chic.salesman[0].salesman.Trim(),
                                            orderno, chic.cust[0].name.Trim(), emailGrupoEmpresa + ";" + emailRetorno, 
                                            corpoEmail, assuntoEmail);
                                    else
                                        erro = EnviaConfirmacaoEmail(caminho, emailGrupoEmpresa, emailGrupoEmpresa,
                                            orderno, chic.cust[0].name.Trim(), "", corpoEmail, assuntoEmail);
                                }
                                else if (tipoAgrupamentoPedidos.Equals("Vendedor"))
                                {
                                    //assuntoEmail = "CONFIRMAÇÕES SEMANAS "
                                    //    + semanaDoAnoInicial.ToString() + "/" + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy")
                                    //    + " a "
                                    //    + semanaDoAnoFinal.ToString() + "/" + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToString("yyyy");

                                    assuntoEmail = "CONFIRMAÇÕES DE "
                                        + dataInicial.ToShortDateString() + " a " 
                                        + dataFinal.ToShortDateString();

                                    corpoEmail = "Prezado " + chic.salesman[0].salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                                        + "Segue em anexo as confirmações dos lotes programados no período das semanas de "
                                        + semanaDoAnoInicial.ToString() + " a " + semanaDoAnoFinal.ToString() + "/"
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy") + " ("
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToShortDateString() + " a "
                                        + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToShortDateString()
                                        + ") conforme as disponibilidades de produto,espaço de incubação e rota."
                                        + (char)13 + (char)10
                                        + "Qualquer dúvida, entrar em contato pelo e-mail " + emailRetorno + "." + (char)13 + (char)10 + (char)13 + (char)10
                                        + "SISTEMA WEB";

                                    if (enviarEmailVendedor == "true")
                                        erro = EnviaConfirmacaoEmail(anexos, chic.salesman[0].email.Trim(), chic.salesman[0].salesman.Trim(),
                                            orderno, chic.cust[0].name.Trim(), emailGrupoEmpresa + ";" + emailRetorno, 
                                            corpoEmail, assuntoEmail);
                                    else
                                        erro = EnviaConfirmacaoEmail(anexos, emailGrupoEmpresa, emailGrupoEmpresa,
                                            orderno, chic.cust[0].name.Trim(), emailRetorno, corpoEmail, assuntoEmail);

                                    anexos = caminho;
                                }
                                else if (tipoAgrupamentoPedidos.Equals("Empresa"))
                                {
                                    //assuntoEmail = "CONFIRMAÇÕES SEMANAS "
                                    //    + semanaDoAnoInicial.ToString() + "/" + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy")
                                    //    + " a "
                                    //    + semanaDoAnoFinal.ToString() + "/" + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToString("yyyy");

                                    assuntoEmail = "CONFIRMAÇÕES DE "
                                        + dataInicial.ToShortDateString() + " a " 
                                        + dataFinal.ToShortDateString();

                                    corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                                        + "Segue em anexo as confirmações dos lotes programados no período das semanas de "
                                        + semanaDoAnoInicial.ToString() + " a " + semanaDoAnoFinal.ToString() + "/" 
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToString("yyyy") + " ("
                                        + Convert.ToDateTime(Session["sDataInicialConf"].ToString()).ToShortDateString() + " a "
                                        + Convert.ToDateTime(Session["sDataFinalConf"].ToString()).ToShortDateString()
                                        + ") conforme as disponibilidades de produto,espaço de incubação e rota." 
                                        + (char)13 + (char)10
                                        + "Qualquer dúvida, entrar em contato pelo e-mail " + emailRetorno + "." + (char)13 + (char)10 + (char)13 + (char)10
                                        + "SISTEMA WEB";

                                    erro = EnviaConfirmacaoEmail(anexos, emailGrupoEmpresa, emailGrupoEmpresa,
                                            orderno, chic.cust[0].name.Trim(), emailRetorno, corpoEmail, assuntoEmail);

                                    anexos = caminho;
                                }
                            }

                            //order.UpdateStatus("INVO", orderno);
                            //int_comm.UpdateConfirmacaoEnviada(true, orderno);

                            empresaAnterior = chic.salesman[0].inv_comp.Trim();
                            vendedorAnterior = chic.salesman[0].sl_code.Trim();
                        }
                    }
                //}

                foreach (var index in selectedIndices)
                {
                    int fileId;
                    if (int.TryParse(fileIds[index], out fileId))
                    {
                        string orderno = fileId.ToString();

                        int_comm.UpdateConfirmacaoEnviada(true, orderno);
                    }
                }

                ViewBag.fileName = "Pedidos confirmados com sucesso!";

                string opcaoData = "Nascimento";
                DateTime dataInicialHoje = DateTime.Today;
                DateTime dataFinalHoje = DateTime.Today;
                string cliente = "00";
                string vendedor = "";
                string empresa = "";
                string status = "";

                ordersConf.FillByConf(chic.ordersConf, opcaoData, dataInicialHoje, dataFinalHoje
                    , opcaoData, dataInicialHoje, dataFinalHoje, opcaoData, dataInicialHoje,
                dataFinalHoje, cliente, cliente, vendedor, vendedor, empresa, empresa, false);

                return View("Confirmacoes", chic.ordersConf);
            }
            catch (Exception e)
            {
                string opcaoData = "Nascimento";
                DateTime dataInicial = DateTime.Today;
                DateTime dataFinal = DateTime.Today;
                string cliente = "00";
                string vendedor = "";
                string empresa = "";
                string status = "";

                ordersConf.FillByConf(chic.ordersConf, opcaoData, dataInicial, dataFinal
                    , opcaoData, dataInicial, dataFinal, opcaoData, dataInicial,
                dataFinal, cliente, cliente, vendedor, vendedor, empresa, empresa, false);

                string msg = "";

                if (e.InnerException == null)
                    msg = "Erro na importação: " + e.Message;
                else
                    msg = "Erro na importação: " + e.Message + " / " + e.InnerException.Message;

                ViewBag.erro = msg;

                return View("Confirmacoes", chic.ordersConf);
            }
        }

        public ActionResult EnviarConfirmacoesFaturamento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string opcaoData = "Nascimento";
            DateTime dataInicial = DateTime.Today;
            DateTime dataFinal = DateTime.Today;
            string cliente = "";
            string vendedor = "";
            string empresa = "";
            string status = "";

            string anexos = "";
            string anexosBR = "";
            string anexosLB = "";
            string anexosHN = "";
            string anexosPL = "";
            string anexosNG = "";
            string anexosNM = "";

            if (model["dataIniFat"] != null)
            {
                Session["sDataInicialFat"] = Convert.ToDateTime(model["dataIniFat"].ToString()).ToShortDateString();
                dataInicial = Convert.ToDateTime(model["dataIniFat"].ToString());
            }
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("Confirmacoes", chic.ordersConf);
            }

            if (model["dataFimFat"] != null)
            {
                Session["sDataFinalFat"] = Convert.ToDateTime(model["dataFimFat"].ToString()).ToShortDateString();
                dataFinal = Convert.ToDateTime(model["dataFimFat"].ToString());
            }
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("Confirmacoes", chic.ordersConf);
            }

            if (model["Empresa"] != null)
            {
                empresa = model["Empresa"].ToString();
            }
            else
            {
                empresa = Session["empresaLayout"].ToString();
            }

            CarregaListaEstados();
            CarregaListaVendedores();
            Session["descricaoConf"] = "";
            Session["estadoConf"] = "";
            Session["marcadoConf"] = cliente;
            Session["vendedorConf"] = vendedor;
            Session["sTipoDataConf"] = opcaoData;
            Session["sEmpresaConf"] = empresa;
            Session["sDataInicialConf"] = dataInicial.ToShortDateString();
            Session["sDataFinalConf"] = dataFinal.ToShortDateString();
            Session["sDataInicialFat"] = dataInicial.ToShortDateString();
            Session["sDataFinalFat"] = dataFinal.ToShortDateString();
            List<Cliente> listaExibeClientes = null;
            Session["ListaClientesConf"] = listaExibeClientes;
            Session["sTipoAgrupamentoPedidos"] = "";
            Session["sEnviarEmailEmpresa"] = "";
            Session["sEnviarEmailVendedor"] = "";

            //ordersConf.FillByFat(chic.ordersConf, opcaoData, dataInicial, dataFinal,
            //    opcaoData, dataInicial, dataFinal, opcaoData, dataInicial,
            //    dataFinal, cliente, cliente, vendedor, vendedor, empresa, empresa);

            order.FillByFat(chic.orders, dataInicial, dataFinal, dataInicial, dataFinal);

            string empresaDireitos = Session["empresa"].ToString();
            string copiaParaFaturamento = "";
            if (empresaDireitos.Contains("BR") && (empresa.Equals("BR") || empresa.Equals("(Todas)")))
            {
                if (copiaParaFaturamento == "")
                    copiaParaFaturamento = "programacao@hyline.com.br";
                else
                    copiaParaFaturamento = ";programacao@hyline.com.br";
            }
            else if (empresaDireitos.Contains("LB") && (empresa.Equals("LB") || empresa.Equals("(Todas)")))
            {
                if (copiaParaFaturamento == "")
                    copiaParaFaturamento = "programacao@ltz.com.br";
                else
                    copiaParaFaturamento = copiaParaFaturamento + ";programacao@ltz.com.br";
            }
            else if (empresaDireitos.Contains("HN") && (empresa.Equals("HN") || empresa.Equals("(Todas)")))
            {
                if (copiaParaFaturamento == "")
                    copiaParaFaturamento = "programacao@hnavicultura.com.br";
                else
                    copiaParaFaturamento = copiaParaFaturamento + ";programacao@hnavicultura.com.br";
            }
            else if (empresaDireitos.Contains("PL") && (empresa.Equals("PL") || empresa.Equals("(Todas)")))
            {
                if (copiaParaFaturamento == "")
                    copiaParaFaturamento = "programacao@planaltopostura.com.br";
                else
                    copiaParaFaturamento = copiaParaFaturamento + ";programacao@planaltopostura.com.br";
            }

            for (int i = 0; i < chic.orders.Count; i++)
            {
                #region Gera confirmações e envia e-mail para o Faturamento

                cust.FillByCustoNo(chic.cust, chic.orders[i].cust_no);

                salesman.FillByCode(chic.salesman, chic.orders[i].salesrep);

                booked.FillByOrderNo(chic.booked, chic.orders[i].orderno);

                string destino = "";

                if (empresaDireitos.Contains(chic.salesman[0].inv_comp.Trim())
                    && (empresa == chic.salesman[0].inv_comp.Trim() || empresa == "(Todas)"))
                {
                    destino = GeraRelConfirmacao(chic.orders[i].orderno, chic.cust[0].name.Trim(),
                        chic.salesman[0].inv_comp.Trim());

                    if (anexos == "")
                        anexos = destino;
                    else
                        anexos = anexos + "^" + destino;

                    if (chic.booked.Where(w => w.location.Trim().Equals("NM")).Count() > 0)
                        if (anexosNM == "")
                            anexosNM = destino;
                        else
                            anexosNM = anexosNM + "^" + destino;
                    else
                        if (anexosNG == "")
                            anexosNG = destino;
                        else
                            anexosNG = anexosNG + "^" + destino;

                    if (chic.salesman[0].inv_comp.Trim() == "BR")
                        if (anexosBR == "")
                            anexosBR = destino;
                        else
                            anexosBR = anexosBR + "^" + destino;
                    else if (chic.salesman[0].inv_comp.Trim() == "LB")
                        if (anexosLB == "")
                            anexosLB = destino;
                        else
                            anexosLB = anexosLB + "^" + destino;
                    else if (chic.salesman[0].inv_comp.Trim() == "HN")
                        if (anexosHN == "")
                            anexosHN = destino;
                        else
                            anexosHN = anexosHN + "^" + destino;
                    else if (chic.salesman[0].inv_comp.Trim() == "PL")
                        if (anexosPL == "")
                            anexosPL = destino;
                        else
                            anexosPL = anexosPL + "^" + destino;
                }

                #endregion
            }

            #region Envia e-mail das confirmações para o Faturamento EW

            if (chic.orders.Rows.Count > 0 && anexosNG != "")
            {
                string assuntoEmail = "CONFIRMAÇÕES PARA FATURAMENTO DE " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy");

                string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo as confirmações para Faturamento de " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy") + "."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato com o Depto. de Programação." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                string anexosEW = "";
                if (anexosBR != "")
                    if (anexosEW == "")
                        anexosEW = anexosBR;
                    else
                        anexosEW = anexosEW + "^" + anexosBR;
                if (anexosLB != "")
                    if (anexosEW == "")
                        anexosEW = anexosLB;
                    else
                        anexosEW = anexosEW + "^" + anexosLB;
                if (anexosHN != "")
                    if (anexosEW == "")
                        anexosEW = anexosHN;
                    else
                        anexosEW = anexosEW + "^" + anexosHN;

                EnviaConfirmacaoEmail(anexosNG, "faturamento@hyline.com.br", "FATURAMENTO",
                    "", "", copiaParaFaturamento, corpoEmail, assuntoEmail);
            }

            #endregion

            #region Envia e-mail das confirmações para o Faturamento PLANALTO

            if (chic.orders.Rows.Count > 0 && anexosNM != "")
            {
                string assuntoEmail = "CONFIRMAÇÕES PARA FATURAMENTO DE " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy");

                string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo as confirmações para Faturamento de " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy") + "."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato com o Depto. de Programação." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                EnviaConfirmacaoEmail(anexosNM, "faturamento@planaltopostura.com.br", "FATURAMENTO",
                    "", "", "programacao@planaltopostura.com.br", corpoEmail, assuntoEmail);
            }

            #endregion

            #region Envia e-mail das confirmações para o Financeiro Hy-Line

            if (anexosBR != "")
            {
                string assuntoEmail = "HLB - CONFIRMAÇÕES PARA FATURAMENTO DE " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy");

                string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo as confirmações para Faturamento de " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy") + "."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato com o Depto. de Programação." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                EnviaConfirmacaoEmail(anexosBR, "financeiro@hyline.com.br", "FINANCEIRO HY-LINE",
                    "", "", "programacao@hyline.com.br", corpoEmail, assuntoEmail);
            }

            #endregion

            #region Envia e-mail das confirmações para o Financeiro Lohmann

            if (anexosLB != "")
            {
                string assuntoEmail = "LTZ - CONFIRMAÇÕES PARA FATURAMENTO DE " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy");

                string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo as confirmações para Faturamento de " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy") + "."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato com o Depto. de Programação." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                EnviaConfirmacaoEmail(anexosLB, "financeiro@ltz.com.br", "FINANCEIRO LOHMANN",
                    "", "", "programacao@ltz.com.br", corpoEmail, assuntoEmail);
            }

            #endregion

            #region Envia e-mail das confirmações para o Financeiro H&N

            if (anexosHN != "")
            {
                string assuntoEmail = "H&N - CONFIRMAÇÕES PARA FATURAMENTO DE " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy");

                string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo as confirmações para Faturamento de " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy") + "."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato com o Depto. de Programação." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                EnviaConfirmacaoEmail(anexosHN, "financeiro@hnavicultura.com.br", "FINANCEIRO H&N",
                    "", "", "programacao@hnavicultura.com.br", corpoEmail, assuntoEmail);
            }

            #endregion

            #region Envia e-mail das confirmações para o Financeiro PLANALTO

            if (anexosPL != "")
            {
                string assuntoEmail = "PLANALTO - CONFIRMAÇÕES PARA FATURAMENTO DE " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy");

                string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo as confirmações para Faturamento de " + dataInicial.ToString("dd/MM/yyyy") + " até "
                    + dataFinal.ToString("dd/MM/yyyy") + "."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato com o Depto. de Programação." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                EnviaConfirmacaoEmail(anexosPL, "financeiro@planaltopostura.com.br", "FINANCEIRO PLANALTO POSTURA",
                    "", "", "programacao@planaltopostura.com.br", corpoEmail, assuntoEmail);
            }

            #endregion

            ViewBag.fileNameFat = "Pedidos enviados p/ o Faturamento e Financeiros com sucesso!";

            return View("Confirmacoes", chic.ordersConf);
        }
    }
}