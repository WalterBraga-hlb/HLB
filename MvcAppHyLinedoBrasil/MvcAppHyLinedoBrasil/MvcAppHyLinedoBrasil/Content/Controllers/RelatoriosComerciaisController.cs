using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using MvcAppHyLinedoBrasil.Data.CHICDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Models.Apolo;
using System.Data.Objects;
using MvcAppHyLinedoBrasil.Models.HLBAPP;
using MvcAppHyLinedoBrasil.Models;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class RelatoriosComerciaisController : Controller
    {
        //
        // GET: /RelatoriosComerciais/

        public CHICDataSet chic = new CHICDataSet();
        public salesmanTableAdapter salesman = new salesmanTableAdapter();
        public ApoloEntities apolo = new ApoloEntities();
        FinanceiroEntities bdApolo = new FinanceiroEntities();

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

            CarregaEmpresasVendedores();
            CarregaLinhagens();
            CarregaListaEstados();
            CarregaClientes();

            return View();
        }

        public ActionResult Main()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");
            return View();
        }

        public ActionResult ListagemPedidosEmail()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaEmpresasVendedores();

            return View("ListagemPedidosEmail");
        }

        public void CarregaLinhagens()
        {
            List<SelectListItem> listaLinhagens = new List<SelectListItem>();

            if (Session["empresa"].ToString().Contains("PL"))
            {
                listaLinhagens.Add(new SelectListItem
                {
                    Text = "(Todas)",
                    Value = "(Todas)",
                    Selected = true
                });

                HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

                var listaLinhagensTB = hlbapp.Tabela_Precos
                    .Where(w => w.Empresa.Equals("PL") && w.Tipo.Equals("Faturamento"))
                    .GroupBy(g => g.Produto)
                    .ToList();

                foreach (var item in listaLinhagensTB)
                {
                    listaLinhagens.Add(new SelectListItem
                    {
                        Text = item.Key,
                        Value = item.Key,
                        Selected = false
                    });
                }
            }

            Session["ListaLinhagensRelComercial"] = listaLinhagens;
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

        public void CarregaListaIncubatorios()
        {
            FLIPDataSet.HATCHERY_CODESDataTable hDT = new FLIPDataSet.HATCHERY_CODESDataTable();
            HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
            hTA.Fill(hDT);

            var listaIncubatorios = hDT.OrderBy(o => o.HATCH_DESC).ToList();
            List<SelectListItem> listaItens = new List<SelectListItem>();

            foreach (var item in listaIncubatorios)
            {
                if (MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC,
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    listaItens.Add(new SelectListItem
                    {
                        Text = item.HATCH_DESC,
                        Value = (item.HATCH_LOC == "TB" ? "AJ" : item.HATCH_LOC),
                        Selected = false
                    });
                }
            }

            Session["ListaIncubatorios"] = listaItens;
        }

        public void CarregaClientes()
        {
            List<SelectListItem> listaClientesDDL = new List<SelectListItem>();
            List<String> listaClientesOriginalApolo = new List<string>();

            string login = "";

            if (Session["login"].ToString().Contains("@"))
            {
                login = Session["login"].ToString();
            }

            string empresaApoloSession = Session["empresaApolo"].ToString();

            //string codigoCliente = Session["selectedCustomer"].ToString();

            var listaClientes = bdApolo.ENTIDADE
                .Where(w => bdApolo.VEND_ENT.Any(a => a.EntCod == w.EntCod
                    && bdApolo.VENDEDOR.Any(n => n.VendCod == a.VendCod
                        && (n.USERLoginSite == login || login == "")
                        && (empresaApoloSession.Contains(n.USEREmpresa) || empresaApoloSession.Equals("TODAS"))))
                    //&& w.EntCod == codigoCliente
                    && w.StatEntCod != "05")
                .Join(
                    bdApolo.ENT_CATEG.Where(c => c.CategCodEstr == "01" || c.CategCodEstr == "01.01"),
                    e => e.EntCod,
                    c => c.EntCod,
                    (e, c) => new { ENTIDADE = e, ENT_CATEG = c })
                .GroupJoin(
                    bdApolo.CIDADE,
                    ecid => ecid.ENTIDADE.CidCod,
                    c => c.CidCod,
                    (ecid, c) => new { ENTIDADE = ecid, CIDADE = c })
                         .SelectMany(n => n.CIDADE.DefaultIfEmpty(),
                                    (n, c) => new { n.ENTIDADE, CIDADE = c })
                .OrderBy(o => o.ENTIDADE.ENTIDADE.EntNome)
                .Select(c => new
                {
                    c.ENTIDADE.ENTIDADE.EntCod,
                    c.ENTIDADE.ENTIDADE.EntNome,
                    c.ENTIDADE.ENTIDADE.EntEnder,
                    c.ENTIDADE.ENTIDADE.EntEnderNo,
                    c.ENTIDADE.ENTIDADE.EntEnderComp,
                    c.ENTIDADE.ENTIDADE.EntBair,
                    c.CIDADE.CidNomeComp,
                    c.CIDADE.UfSigla,
                    c.CIDADE.PaisSigla,
                    c.ENTIDADE.ENTIDADE.EntCpfCgc,
                    c.ENTIDADE.ENTIDADE.EntRgIe,
                    c.ENTIDADE.ENTIDADE.EntAgropInsc,
                    c.ENTIDADE.ENTIDADE.EntTipoFJ
                }).ToList();

            listaClientesDDL.Add(new SelectListItem { Text = "(Todos)", Value = "(Todos)", Selected = true });

            foreach (var item in listaClientes)
            {
                string cidadeStr = "";
                //CIDADE cidade = apoloStatic.CIDADE.Where(w => w.CidCod == item.CidCod).FirstOrDefault();
                if (item.CidNomeComp != null)
                    cidadeStr = " - " + item.CidNomeComp + " - " + item.UfSigla
                         + " - " + item.PaisSigla;

                //bool select = false;
                //if (item.EntCod == codigoCliente)
                //    select = true;

                string ie = "";
                if (item.EntRgIe != "" && item.EntRgIe != null)
                    ie = item.EntRgIe;
                else
                    ie = item.EntAgropInsc;

                string tipoNacional = " - CNPJ: ";
                if (item.EntTipoFJ.Equals("Física")) tipoNacional = " - CPF: ";

                string tipoEstadual = " - IE: ";
                if (item.EntTipoFJ.Equals("Física")) tipoEstadual = " - RG: ";

                listaClientesDDL.Add(new SelectListItem
                {
                    Text = item.EntCod + " - " + item.EntNome + cidadeStr,
                    Value = item.EntCod,
                    Selected = false
                });

                listaClientesOriginalApolo.Add(item.EntCod);
            }

            Session["ListaClientes"] = listaClientesDDL;
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

        public void AtualizaVendedorSelecionado(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaVendedoresRelComercial"];

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

            Session["ListaVendedoresRelComercial"] = estados;
        }

        public void AtualizaUFSelecionado(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaVendedoresRelComercial"];

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

            Session["ListaEstados"] = estados;
        }

        public void AtualizaLinhagemSelecionada(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaLinhagensRelComercial"];

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

            Session["ListaLinhagensRelComercial"] = estados;
        }

        public void AtualizaCliente(string modelo)
        {
            List<SelectListItem> estados = (List<SelectListItem>)Session["ListaClientes"];

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

            Session["ListaClientes"] = estados;
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

        [HttpPost]
        public ActionResult GerarListaPedidos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("Index");
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("Index");
            }

            string vendedor = "";

            if (model["Vendedor"] != null)
            {
                vendedor = model["Vendedor"];
            }
            else
            {
                vendedor = "(Todos)";
            }

            AtualizaVendedorSelecionado(vendedor);

            string uf = "";

            if (model["UF"] != null)
            {
                uf = model["UF"];
            }
            else
            {
                uf = "(Todos)";
            }

            string cliente = "";

            if (model["Cliente"] != null)
            {
                cliente = model["Cliente"];
            }
            else
            {
                cliente = "(Todos)";
            }

            AtualizaUFSelecionado(uf);

            string linhagem = "";

            if (model["Linhagem"] != null)
            {
                linhagem = model["Linhagem"];
            }
            else
            {
                linhagem = "(Todas)";
            }

            AtualizaLinhagemSelecionada(linhagem);

            string empresa = "";

            if (model["Empresa"] != null)
            {
                if (!vendedor.Equals("(Todos)"))
                {
                    salesman.FillByCode(chic.salesman, vendedor);

                    if (chic.salesman[0].inv_comp.Trim() == model["Empresa"].ToString() || model["Empresa"].ToString() == "(Todas)")
                    {
                        Session["sEmpresa"] = model["Empresa"].ToString();
                        empresa = model["Empresa"].ToString();
                    }
                    else
                    {
                        ViewBag.erro = "O Vendedor selecionado não pertence a Empresa selecionada. Verifique!";
                        return View("Index");
                    }
                }
                else
                {
                    empresa = model["Empresa"].ToString();
                }
            }
            else
            {
                empresa = Session["empresaLayout"].ToString();
            }

            AtualizaEmpresaSelecionada(empresa);

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Lista_Pedidos_" 
                //+ Session["empresaLayout"].ToString() + "_" 
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Lista_Pedidos_" 
                //+ Session["empresaLayout"].ToString() + "_" 
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            destino = GeraRelatorioListaPedidos(pesquisa, true, pasta, destino, 
                Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                empresa, vendedor, linhagem, uf, cliente);

            return File(destino, "Download", "Lista_Pedidos_" 
                //+ empresa + "_" 
                + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        [HttpPost]
        public ActionResult EnviarListaPedidosEmail(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            try
            {
                string mensagem = "E-mail(s) enviado(s) com sucesso! Segue abaixo as Vendedores que receberam: <br /><br />";

                #region Tratamento de Parâmetros

                if (model["dataIni"] != null)
                    Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
                else
                {
                    ViewBag.erro = "Por favor, inserir data Inicial!";
                    return View("ListagemPedidosEmail");
                }

                if (model["dataFim"] != null)
                    Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
                else
                {
                    ViewBag.erro = "Por favor, inserir Data Final!";
                    return View("ListagemPedidosEmail");
                }

                string vendedor = "";

                if (model["Vendedor"] != null)
                {
                    vendedor = model["Vendedor"];
                }
                else
                {
                    vendedor = "(Todos)";
                }

                AtualizaVendedorSelecionado(vendedor);

                string empresa = "";

                if (model["Empresa"] != null)
                {
                    if (!vendedor.Equals("(Todos)"))
                    {
                        salesman.FillByCode(chic.salesman, vendedor);

                        if (chic.salesman[0].inv_comp.Trim() == model["Empresa"].ToString() || model["Empresa"].ToString() == "(Todas)")
                        {
                            Session["sEmpresa"] = model["Empresa"].ToString();
                            empresa = model["Empresa"].ToString();
                        }
                        else
                        {
                            ViewBag.erro = "O Vendedor selecionado não pertence a Empresa selecionada. Verifique!";
                            return View("ListagemPedidosEmail");
                        }
                    }
                    else
                    {
                        Session["sEmpresa"] = model["Empresa"].ToString();
                        empresa = model["Empresa"].ToString();
                    }
                }
                else
                {
                    empresa = Session["empresaLayout"].ToString();
                }

                AtualizaEmpresaSelecionada(empresa);

                #endregion

                List<SelectListItem> listaEmpresas = new List<SelectListItem>();

                string destino = "";
                string pasta = "\\\\srv-fls-03\\w\\Relatorios_CHIC\\Pedidos_Importados";

                if (empresa.Equals("(Todas)"))
                {
                    listaEmpresas = (List<SelectListItem>)Session["ListaEmpresasRelComercial"];
                }
                else
                {
                    listaEmpresas.Add(new SelectListItem
                    {
                        Text = empresa,
                        Value = empresa,
                        Selected = true
                    });
                }

                foreach (var item in listaEmpresas)
                {
                    CHICDataSet.salesmanDataTable vendedores = new CHICDataSet.salesmanDataTable();
                    salesman.FillByEmpresa(vendedores, item.Text);

                    var listaVendedores = vendedores
                        .Where(v => v.sl_code == vendedor || vendedor == "(Todos)")
                        .ToList();

                    if (listaVendedores.Count > 0)
                        mensagem = "<br /><br />" + mensagem + "Empresa " + item.Text + ": <br /><br />";

                    foreach (var itemVendedor in listaVendedores)
                    {
                        destino = "\\\\srv-fls-03\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + item.Text.Trim() + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx";

                        destino = GeraRelatorioListaPedidos("", false, pasta, destino, Convert.ToDateTime(model["dataIni"].ToString()),
                            Convert.ToDateTime(model["dataFim"].ToString()), item.Text.Trim(),
                                itemVendedor.sl_code.Trim(), "(Todas)", "(Todos)", "(Todos)");

                        #region Envio de E-mail

                        WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                        string empresaDescricao = "";
                        if (item.Text.Equals("BR")) { empresaDescricao = "HY-LINE"; }
                        else if (item.Text.Equals("LB")) { empresaDescricao = "LOHMANN"; }
                        else if (item.Text.Equals("HN")) { empresaDescricao = "H&N"; }
                        else if (item.Text.Equals("PL")) { empresaDescricao = "PLANALTO"; }

                        string empresaApolo = "";
                        if (item.Text.Equals("BR")) { empresaApolo = "5"; }
                        else if (item.Text.Equals("LB")) { empresaApolo = "7"; }
                        else if (item.Text.Equals("HN")) { empresaApolo = "14"; }
                        else if (item.Text.Equals("PL")) { empresaApolo = "20"; }

                        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                        apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                        email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                        email.WorkFlowEmailStat = "Enviar";
                        email.WorkFlowEmailAssunto = "LISTA DE PEDIDOS " + empresaDescricao + " ATUALIZADA";
                        email.WorkFlowEmailData = DateTime.Now;
                        email.WorkFlowEmailParaNome = itemVendedor.salesman.Trim();
                        email.WorkFlowEmailParaEmail = itemVendedor.email.Trim();
                        //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                        email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                        email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                        email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                        email.WorkFlowEmailFormato = "Texto";
                        email.WorkFlowEmailDocEmpCod = empresaApolo;

                        string corpoEmail = "";

                        string stringChar = "" + (char)13 + (char)10;

                        corpoEmail = "Prezado " + itemVendedor.salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                            + "Segue em anexo a Lista de Pedidos da " + empresaDescricao + " de seus clientes para verificação." + (char)13 + (char)10
                            + "Essa lista é para seu acompanhamento, inclusive para saber qual o número do Pedido gerado em nosso sistema para poder realizar Alterações e Cancelamento caso necessário." + (char)13 + (char)10
                            + "Qualquer dúvida, entrar em contato pelo e-mail programacao@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                            + "SISTEMA WEB";

                        email.WorkFlowEmailCorpo = corpoEmail;
                        email.WorkFlowEmailArquivosAnexos = destino;

                        apolo.WORKFLOW_EMAIL.AddObject(email);

                        apolo.SaveChanges();

                        mensagem = mensagem + itemVendedor.sl_code.Trim() + " - " + itemVendedor.salesman.Trim() + " - E-mail: " +
                            itemVendedor.email.Trim() + "<br />";

                        #endregion
                    }
                }

                ViewBag.fileName = mensagem;
                return View("ListagemPedidosEmail", "");
            }
            catch (Exception e)
            {
                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao enviar e-mail: " + e.Message;
                return View("ListagemPedidosEmail", "");
            }
        }

        public string GeraRelatorioListaPedidos(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string empresa, string vendedor,
            string linhagem, string uf, string cliente)
        {
            //string destino = "\\\\srv-fls-03\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\srv-fls-03\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            string empresaLayoutRelatorio = "";

            //if (empresa.Equals("(Todas)") || empresa.Equals("PL"))
            if (empresa.Equals("(Todas)"))
                empresaLayoutRelatorio = "BR";
            else
                empresaLayoutRelatorio = empresa;

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Lista_Pedidos" 
                //+ empresaLayoutRelatorio 
                + ".xlsx", destino);

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

            #region SQL Exibição

            string commandTextCHICCabecalho =
                "select " +
                    "b.cal_date `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "v.desc `Linhagem`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "min(i.form) `Matriz`, " +
                    "s.salesman `Ved. / Repres.` ";

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
                    "trim(b.alt_desc) = '' and i.form in ('HE','HV','HN') and b.order_type = 'O' and ";

            string dataInicialStr = dataInicial.ToString("MM/dd/yyyy");
            string dataFinalStr = dataFinal.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametros =
                    "b.cal_date between {" + dataInicialStr + "} and {" + dataFinalStr + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(v.desc = '" + linhagem + "' or '" + linhagem + "' = '(Todas)') and " +
                    "(c.state = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "(c.custno = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamento =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "s.salesman, " +
                    "o.delivery " +

                    " Union ";

            string commandTextCHICCabecalho02 =
                "select " +
                    "b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "v.desc `Linhagem`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "min(i.form) `Matriz`, " +
                    "s.salesman `Ved. / Repres.` ";

            string commandTextCHICTabelas02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoins02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "b.customer = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltros02 =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrCalDate = dataInicial.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrCalDate = dataFinal.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametros02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(v.desc = '" + linhagem + "' or '" + linhagem + "' = '(Todas)') and " +
                    "(c.state = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "(c.custno = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamento02 =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "s.salesman, " +
                    "o.delivery ";

            string commandTextCHICOrdenacao =
                "order by " +
                    "1, 3";

            #endregion

            #region SQL Dados

            string commandTextCHICCabecalhoFaturamentoDados =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi ";

            string commandTextCHICTabelasFaturamentoDados =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic ";

            string commandTextCHICCondicaoJoinsFaturamentoDados =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.orderno = ic.orderno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados = "";

            string commandTextCHICCondicaoParametrosFaturamentoDados =
                    "b.cal_date between {" + dataInicialStr + "} and {" + dataFinalStr + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(c.state = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "(c.custno = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados = "";

            string commandTextCHICOrdenacaoFaturamentoDados = " Union ";

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.orderno = ic.orderno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 = "";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(c.state = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "(c.custno = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "";

            #endregion

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Pedidos"];

            //worksheet.Cells[2, 8] = dataInicial;
            //worksheet.Cells[2, 10] = dataFinal;

            string nomeVendedor = "";
            if (vendedor != "(Todos)")
            {
                salesmanTableAdapter sTA = new salesmanTableAdapter();
                CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                sTA.FillByCod(sDT, vendedor);
                if (sDT.Count > 0)
                {
                    nomeVendedor = sDT[0].salesman.Trim();
                }
            }

            worksheet.Cells[2, 7] = dataInicial.ToString("dd/MM/yyyy") + " à " +
                dataFinal.ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheet.Cells[3, 7] = vendedor;
            else
                worksheet.Cells[3, 7] = nomeVendedor;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("CHIC_Faturamento"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros +
                        commandTextCHICAgrupamento +
                        commandTextCHICCabecalho02 + commandTextCHICTabelas02 + commandTextCHICCondicaoJoins02 +
                        commandTextCHICCondicaoFiltros02 + commandTextCHICCondicaoParametros02 +
                        commandTextCHICAgrupamento02 +
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

            return destino;
        }

        public string GeraRelatorioVerificacaoFinal(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string empresa, string vendedor,
            string nomeVendedor)
        {
            //string destino = "\\\\srv-fls-03\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\srv-fls-03\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            string empresaLayoutRelatorio = "";

            if (empresa.Equals("(Todas)"))
                empresaLayoutRelatorio = "BR";
            else
                empresaLayoutRelatorio = empresa;

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Verificacao_Final_" + empresaLayoutRelatorio + ".xlsx", destino);

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

            #region Faturamento

            DateTime dataInicialFaturamento = DateTime.Today.AddDays(5);
            DateTime dataFinalFaturamento = DateTime.Today.AddDays(11);

            Excel._Worksheet worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento"];

            worksheetFaturamento.Cells[2, 7] = dataInicialFaturamento.ToString("dd/MM/yyyy") + " à " +
                dataFinalFaturamento.ToString("dd/MM/yyyy");
            worksheetFaturamento.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoFaturamento =
                "select " +
                    "IIF(SUBSTR(i.form,1,1) = 'H', b.cal_date, b.cal_date+21) `Nascimento`, " +
                //"b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "v.desc `Linhagem`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "b.price `Valor Unit.`, " +
                    "(select SUM(b1.quantity) from booked b1 where b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc not like '%Extra%') `Qtde. Vendida`, " +
                    "(select SUM(b1.quantity) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('DO','DN','DV','HE') " +
                        "and b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc like '%Extra%') `Qtde. Bonificada`, " +
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
                        "and b1.orderno = o.orderno and i1.item_desc like '%INFRAVER%') `Trat. Infraverm.` ";

            string commandTextCHICTabelasFaturamento =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamento =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "b.customer = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento =
                //"b.price > 0 and ";
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV','HE') and ";

            string dataInicialStrFaturamento = dataInicialFaturamento.ToString("MM/dd/yyyy");
            string dataFinalStrFaturamento = dataFinalFaturamento.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento =
                    "0 < (select COUNT(1) from booked b1, items i1 " +
                            "where b1.orderno = o.orderno and b1.item = i1.item_no and " +
                //"b1.cal_date+21 between DATE()+60 and DATE()+240) ";
                            "IIF(SUBSTR(i1.form,1,1) = 'H', b1.cal_date, b1.cal_date+21) between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "}) and " +
                            "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                            "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento =
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
                    "o.del_date, " +
                    "b.orderno ";

            string commandTextCHICOrdenacaoFaturamento =
                "order by " +
                    "1";

            #endregion

            #region Lotes Já Incubados

            DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
            DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

            Excel._Worksheet worksheetLotesJaIncubados = (Excel._Worksheet)oBook.Worksheets["Lotes Já Incubados"];

            worksheetLotesJaIncubados.Cells[2, 7] = dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " à " +
                dataFinalLotesJaIncubados.ToString("dd/MM/yyyy");
            worksheetLotesJaIncubados.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoLotesJaIncubados =
                "select " +
                    "IIF(SUBSTR(i.form,1,1) = 'H', b.cal_date, b.cal_date+21) `Nascimento`, " +
                //"b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "v.desc `Linhagem`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "b.price `Valor Unit.`, " +
                    "(select SUM(b1.quantity) from booked b1 where b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc not like '%Extra%') `Qtde. Vendida`, " +
                    "(select SUM(b1.quantity) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('DO','DN','DV','HE') " +
                        "and b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc like '%Extra%') `Qtde. Bonificada`, " +
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
                        "and b1.orderno = o.orderno and i1.item_desc like '%INFRAVER%') `Trat. Infraverm.` ";

            string commandTextCHICTabelasLotesJaIncubados =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsLotesJaIncubados =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "b.customer = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosLotesJaIncubados =
                //"b.price > 0 and ";
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV','HE') and ";

            string dataInicialStrLotesJaIncubados = dataInicialLotesJaIncubados.ToString("MM/dd/yyyy");
            string dataFinalStrLotesJaIncubados = dataFinalLotesJaIncubados.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosLotesJaIncubados =
                    "0 < (select COUNT(1) from booked b1, items i1 " +
                            "where b1.orderno = o.orderno and b1.item = i1.item_no and " +
                //"b1.cal_date+21 between DATE()+60 and DATE()+240) ";
                            "IIF(SUBSTR(i1.form,1,1) = 'H', b1.cal_date, b1.cal_date+21) between {" + dataInicialStrLotesJaIncubados + "} and {" + dataFinalStrLotesJaIncubados + "}) and " +
                            "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                            "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoLotesJaIncubados =
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
                    "o.del_date, " +
                    "b.orderno ";

            string commandTextCHICOrdenacaoLotesJaIncubados =
                "order by " +
                    "1";

            #endregion

            #region Incubação

            DateTime dataInicialIncubacao = (dataFinalLotesJaIncubados.AddDays(1)).AddDays(-21);
            DateTime dataFinalIncubacao = (dataFinalLotesJaIncubados.AddDays(14)).AddDays(-21);

            Excel._Worksheet worksheetIncubacao = (Excel._Worksheet)oBook.Worksheets["Incubação"];

            worksheetIncubacao.Cells[2, 7] = dataInicialIncubacao.ToString("dd/MM/yyyy") + " à " +
                dataFinalIncubacao.ToString("dd/MM/yyyy");
            worksheetIncubacao.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoIncubacao =
                "select " +
                //"IIF(SUBSTR(i.form,1,1) = 'H', b.cal_date, b.cal_date+21) `Nascimento`, " +
                    "b.cal_date `Incubação`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "v.desc `Linhagem`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "b.price `Valor Unit.`, " +
                    "(select SUM(b1.quantity) from booked b1 where b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc not like '%Extra%') `Qtde. Vendida`, " +
                    "(select SUM(b1.quantity) from booked b1, items i1 where b1.item = i1.item_no and i1.form in ('DO','DN','DV','HE') " +
                        "and b1.orderno = b.orderno and b1.item = b.item and b1.alt_desc like '%Extra%') `Qtde. Bonificada`, " +
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
                        "and b1.orderno = o.orderno and i1.item_desc like '%INFRAVER%') `Trat. Infraverm.` ";

            string commandTextCHICTabelasIncubacao =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsIncubacao =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "b.customer = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosIncubacao =
                //"b.price > 0 and ";
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV','HE') and ";

            string dataInicialStrIncubacao = dataInicialIncubacao.ToString("MM/dd/yyyy");
            string dataFinalStrIncubacao = dataFinalIncubacao.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosIncubacao =
                    "0 < (select COUNT(1) from booked b1, items i1 " +
                            "where b1.orderno = o.orderno and b1.item = i1.item_no and " +
                //"b1.cal_date+21 between DATE()+60 and DATE()+240) ";
                            "b1.cal_date between {" + dataInicialStrIncubacao + "} and {" + dataFinalStrIncubacao + "}) and " +
                            "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                            "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoIncubacao =
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
                    "o.del_date, " +
                    "b.orderno ";

            string commandTextCHICOrdenacaoIncubacao =
                "order by " +
                    "1";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("CHIC_Faturamento"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamento + commandTextCHICTabelasFaturamento +
                        commandTextCHICCondicaoJoinsFaturamento +
                        commandTextCHICCondicaoFiltrosFaturamento + commandTextCHICCondicaoParametrosFaturamento +
                        commandTextCHICAgrupamentoFaturamento +
                        commandTextCHICOrdenacaoFaturamento;
                else if (item.Name.Equals("CHIC_Ja_Incubados"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoLotesJaIncubados + commandTextCHICTabelasLotesJaIncubados +
                        commandTextCHICCondicaoJoinsLotesJaIncubados +
                        commandTextCHICCondicaoFiltrosLotesJaIncubados + commandTextCHICCondicaoParametrosLotesJaIncubados +
                        commandTextCHICAgrupamentoLotesJaIncubados +
                        commandTextCHICOrdenacaoLotesJaIncubados;
                else if (item.Name.Equals("CHIC_Incubacao"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoIncubacao + commandTextCHICTabelasIncubacao +
                        commandTextCHICCondicaoJoinsIncubacao +
                        commandTextCHICCondicaoFiltrosIncubacao + commandTextCHICCondicaoParametrosIncubacao +
                        commandTextCHICAgrupamentoIncubacao +
                        commandTextCHICOrdenacaoIncubacao;
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

            return destino;
        }

        #region Lista de Pedidos - Matriz

        public ActionResult ListaPedidosMatriz()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View("ListagemPedidosMatriz");
        }

        public string GeraRelatorioListaPedidosMatriz(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string empresa)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Lista_Pedidos_Matriz.xlsx", destino);

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

            #region Empresa LTZHN

            string commandTextCHICCabecalho =
                "select " +
                    "o.orderno `Nº Pedido`, " +
                    "b.quantity, " +
                    "b.price `Valor Unit.`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "IIF(SUBSTR(i.form,1,1) = 'H', b.cal_date, b.cal_date+21) `Nascimento`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "o.Status, " +
                    "IIF(EMPTY(b.alt_desc),i.item_desc,b.alt_desc) `Descrição`, " +
                    "b.item_ord, " +
                    "c.country, " +
                    "i.variety, " +
                    "i.form ";

            string commandTextCHICTabelas =
                "from " +
                    "booked b, orders o, items i, cust c ";

            string commandTextCHICCondicaoJoins =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "b.customer = c.custno and ";

            string commandTextCHICCondicaoFiltros =
                    "0 = (select count(1) from booked b1, items i1 where o.orderno = b1.orderno and " +
                    "b1.item = i1.item_no and i1.variety in ('W36M','W36F','BRLF','BRLM','W-98','W-36')) and " +
                    "c.custno not in ('0000178','0003203','0003317','0003731') and ";

            string dataInicialStr = dataInicial.ToString("MM/dd/yyyy");
            string dataFinalStr = dataFinal.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametros =
                    "0 < (select COUNT(1) from booked b1, items i1 " +
                            "where b1.orderno = o.orderno and b1.item = i1.item_no and " +
                            "IIF(SUBSTR(i1.form,1,1) = 'H', b1.cal_date, b1.cal_date+21) " +
                            "between {" + dataInicialStr + "} and {" + dataFinalStr + "}) ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "7, 1, 11, 2 desc";

            #endregion

            #region Empresa HLB

            string commandTextCHICCabecalhoHLB =
                "select " +
                    "o.orderno `Nº Pedido`, " +
                    "b.quantity, " +
                    "b.price `Valor Unit.`, " +
                    "o.delivery `Cond. Pagmto.`, " +
                    "IIF(SUBSTR(i.form,1,1) = 'H', b.cal_date, b.cal_date+21) `Nascimento`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    "o.Status, " +
                    "IIF(EMPTY(b.alt_desc),i.item_desc,b.alt_desc) `Descrição`, " +
                    "b.item_ord, " +
                    "c.country, " +
                    "i.variety, " +
                    "i.form ";

            string commandTextCHICTabelasHLB =
                "from " +
                    "booked b, orders o, items i, cust c ";

            string commandTextCHICCondicaoJoinsHLB =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "b.customer = c.custno and ";

            string commandTextCHICCondicaoFiltrosHLB = "";

            string dataInicialStrHLB = dataInicial.ToString("MM/dd/yyyy");
            string dataFinalStrHLB = dataFinal.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosHLB =
                    "0 < (select COUNT(1) from booked b1, items i1 " +
                            "where b1.orderno = o.orderno and b1.item = i1.item_no and " +
                            "IIF(SUBSTR(i1.form,1,1) = 'H', b1.cal_date, b1.cal_date+21) " +
                            "between {" + dataInicialStrHLB + "} and {" + dataFinalStrHLB + "}) ";

            string commandTextCHICAgrupamentoHLB = "";

            string commandTextCHICOrdenacaoHLB =
                "order by " +
                    "7, 1, 11, 2 desc";

            #endregion

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Orders"];

            worksheet.Cells[2, 5] = dataInicial;
            worksheet.Cells[2, 7] = dataFinal;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;

                if (item.Name.Equals("CHIC"))
                    if (empresa == "LTZHN")
                    {
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                            commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                            commandTextCHICOrdenacao;
                    }
                    else
                    {
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalhoHLB + commandTextCHICTabelasHLB + commandTextCHICCondicaoJoinsHLB +
                            commandTextCHICCondicaoFiltrosHLB + commandTextCHICCondicaoParametrosHLB +
                            commandTextCHICAgrupamentoHLB + commandTextCHICOrdenacaoHLB;
                    }
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

            return destino;
        }

        [HttpPost]
        public ActionResult GerarListaPedidosMatriz(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("Index");
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("Index");
            }

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Lista_Pedidos_Matriz_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Lista_Pedidos_Matriz_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string empresa = "";
            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-RelatoriosComerciaisListaPedidosMatrizHNLTZ", (System.Collections.ArrayList)Session["Direitos"]))
                empresa = "LTZHN";
            else
                empresa = "HLB";

            destino = GeraRelatorioListaPedidosMatriz(pesquisa, true, pasta, destino, Convert.ToDateTime(model["dataIni"]),
                Convert.ToDateTime(model["dataFim"]), empresa);

            return File(destino, "Download", "Orders_Report_" + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        #endregion

        #region Planejamento Incubação

        public ActionResult PlanejamentoIncubacao()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaListaIncubatorios();

            return View();
        }

        [HttpPost]
        public ActionResult GerarPlanejamentoIncubacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            if (model["dataIni"] != null)
                Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir data Inicial!";
                return View("Index");
            }

            if (model["dataFim"] != null)
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
            else
            {
                ViewBag.erro = "Por favor, inserir Data Final!";
                return View("Index");
            }

            string incubatorio = "";
            if (model["Incubatorio"] != null)
            {
                incubatorio = model["Incubatorio"];
            }
            else
            {
                incubatorio = "(Todos)";
            }
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);
            
            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Planejamento_Incubacao_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Planejamento_Incubacao_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            destino = GeraPlanejamentoIncubacao(pesquisa, true, pasta, destino,
                Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                incubatorio);

            return File(destino, "Download", "Planejamento_Incubacao_"
                + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraPlanejamentoIncubacao(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Planejamento_Incubacao"
                + ".xlsx", destino);

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

            #region SQL Dados

            string dataInicialStr = dataInicial.ToString("MM/dd/yyyy");
            string dataFinalStr = dataFinal.ToString("MM/dd/yyyy");
            string dataInicialStrCalDate = dataInicial.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrCalDate = dataFinal.AddDays(-21).ToString("MM/dd/yyyy");

            //string commandTextCHICCabecalhoFaturamentoDados =
            //    "select * ";

            //string commandTextCHICTabelasFaturamentoDados =
            //    "from " +
            //        "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            //string commandTextCHICCondicaoJoinsFaturamentoDados =
            //    "where " +
            //        "b.orderno = o.orderno and " +
            //        "b.item = i.item_no and  " +
            //        "i.variety = v.variety and " +
            //        "o.cust_no = c.custno and " +
            //        "o.salesrep = s.sl_code and ";

            //string commandTextCHICCondicaoFiltrosFaturamentoDados =
            //        "trim(b.alt_desc) = '' and i.form in ('HE','HV','HN') and b.order_type = 'O' and ";

            //string commandTextCHICCondicaoParametrosFaturamentoDados =
            //        "b.cal_date between {" + dataInicialStr + "} and {" + dataFinalStr + "} and " +
            //        "(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todos)') ";

            //string commandTextCHICAgrupamentoFaturamentoDados = "";

            //string commandTextCHICOrdenacaoFaturamentoDados = " Union ";

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

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                    "(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todas)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "";

            #endregion

            #region SQL Comentarios

            //string commandTextCHICCabecalhoComentarios =
            //    "select ic.orderno, ic.hatchinf ";

            //string commandTextCHICTabelasComentarios =
            //    "from " +
            //        "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic ";

            //string commandTextCHICCondicaoJoinsComentarios =
            //    "where " +
            //        "ic.orderno = o.orderno and " +
            //        "b.orderno = o.orderno and " +
            //        "b.item = i.item_no and  " +
            //        "i.variety = v.variety and " +
            //        "o.cust_no = c.custno and " +
            //        "o.salesrep = s.sl_code and ";

            //string commandTextCHICCondicaoFiltrosComentarios = "";

            //string commandTextCHICCondicaoParametrosComentarios =
            //        "b.cal_date between {" + dataInicialStr + "} and {" + dataFinalStr + "} and " +
            //        "(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todos)') ";

            //string commandTextCHICAgrupamentoComentarios = "";

            //string commandTextCHICOrdenacaoComentarios = " Union ";

            string commandTextCHICCabecalhoComentarios02 =
                "select ic.orderno, ic.hatchinf ";

            string commandTextCHICTabelasComentarios02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic ";

            string commandTextCHICCondicaoJoinsComentarios02 =
                "where " +
                    "ic.orderno = o.orderno and " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosComentarios02 = "";

            string commandTextCHICCondicaoParametrosComentarios02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                    "(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todas)') ";

            string commandTextCHICAgrupamentoComentarios02 = "";

            string commandTextCHICOrdenacaoComentarios02 = "";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("CHIC_Faturamento_Dados"))
                    item.OLEDBConnection.CommandText =
                        //commandTextCHICCabecalhoFaturamentoDados + commandTextCHICTabelasFaturamentoDados +
                        //commandTextCHICCondicaoJoinsFaturamentoDados +
                        //commandTextCHICCondicaoFiltrosFaturamentoDados + commandTextCHICCondicaoParametrosFaturamentoDados +
                        //commandTextCHICAgrupamentoFaturamentoDados +
                        //commandTextCHICOrdenacaoFaturamentoDados +
                        commandTextCHICCabecalhoFaturamentoDados02 + commandTextCHICTabelasFaturamentoDados02 +
                        commandTextCHICCondicaoJoinsFaturamentoDados02 +
                        commandTextCHICCondicaoFiltrosFaturamentoDados02 + commandTextCHICCondicaoParametrosFaturamentoDados02 +
                        commandTextCHICAgrupamentoFaturamentoDados02 +
                        commandTextCHICOrdenacaoFaturamentoDados02;
                else if (item.Name.Equals("CHIC_Faturamento_Dados_Comment"))
                    item.OLEDBConnection.CommandText =
                        //commandTextCHICCabecalhoComentarios + commandTextCHICTabelasComentarios +
                        //commandTextCHICCondicaoJoinsComentarios +
                        //commandTextCHICCondicaoFiltrosComentarios + commandTextCHICCondicaoParametrosComentarios +
                        //commandTextCHICAgrupamentoComentarios +
                        //commandTextCHICOrdenacaoComentarios +
                        commandTextCHICCabecalhoComentarios02 + commandTextCHICTabelasComentarios02 +
                        commandTextCHICCondicaoJoinsComentarios02 +
                        commandTextCHICCondicaoFiltrosComentarios02 + commandTextCHICCondicaoParametrosComentarios02 +
                        commandTextCHICAgrupamentoComentarios02 +
                        commandTextCHICOrdenacaoComentarios02;
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

            return destino;
        }

        #endregion
    }
}
