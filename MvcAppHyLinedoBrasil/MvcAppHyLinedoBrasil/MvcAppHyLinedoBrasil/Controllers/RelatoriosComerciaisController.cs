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

            CarregaEmpresasVendedores(true);
            CarregaLinhagens();
            CarregaListaEstados();
            CarregaClientes();

            return View();
        }

        public ActionResult Main()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            if (Session["sDataInicial"] == null) Session["sDataInicial"] = DateTime.Today.ToShortDateString();
            if (Session["sDataFinal"] == null) Session["sDataFinal"] = DateTime.Today.ToShortDateString();

            return View();
        }

        public ActionResult ListagemPedidosEmail()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaEmpresasVendedores(true);

            return View("ListagemPedidosEmail");
        }

        #region Carrega DropDownList

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

        public void CarregaEmpresasVendedores(bool todos)
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
                    (System.Collections.ArrayList)Session["Direitos"])
                && todos)
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
                    //CHICDataSet.salesmanDataTable vendedores = new CHICDataSet.salesmanDataTable();
                    //salesman.FillByEmpresa(vendedores, Session["empresa"].ToString().Substring(i, 2));

                    //foreach (var item in vendedores)
                    //{
                    //    listaVendedores.Add(new SelectListItem
                    //    {
                    //        Text = item.inv_comp.Trim() + " - " + item.sl_code.Trim() + " - "
                    //            + item.salesman.Trim(),
                    //        Value = item.sl_code.Trim(),
                    //        Selected = false
                    //    });
                    //}

                    ApoloEntities apolo = new ApoloEntities();
                    HLBAPPEntities1 hlbapp = new HLBAPPEntities1();
                    var primeiraEmpresasAcesso = Session["empresa"].ToString().Substring(i, 2);
                    var empresaConfig = hlbapp.Empresas.Where(w => w.CodigoCHIC == primeiraEmpresasAcesso).FirstOrDefault().DescricaoApoloVendedor;
                    var vendedores = apolo.VENDEDOR.Where(w => w.USEREmpresa == empresaConfig).OrderBy(o => o.VendNome).ToList();

                    foreach (var item in vendedores)
                    {
                        listaVendedores.Add(new SelectListItem
                        {
                            Text = item.VendCod + " - " + item.VendNome,
                            Value = item.VendCod,
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

        public List<SelectListItem> CarregaTipoProduto()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            lista.Add(new SelectListItem
            {
                Text = "(Todos)",
                Value = "",
                Selected = true
            });

            lista.Add(new SelectListItem
            {
                Text = "Pintos",
                Value = "D",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "Ovos",
                Value = "H",
                Selected = false
            });

            return lista;
        }

        public List<SelectListItem> CarregaLaboratorios()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            lista.Add(new SelectListItem
            {
                Text = "(Todos)",
                Value = "",
                Selected = true
            });

            lista.Add(new SelectListItem
            {
                Text = "BIOVET",
                Value = "BIOVET",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "BOEHRINGER",
                Value = "BOEHRINGER",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "CEVA",
                Value = "CEVA",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "HIPRA",
                Value = "HIPRA",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "MSD",
                Value = "MSD",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "ZOETIS",
                Value = "ZOETIS",
                Selected = false
            });

            return lista;
        }

        public List<SelectListItem> CarregaOrigensRelatorioVacinas()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            //lista.Add(new SelectListItem
            //{
            //    Text = "CHIC",
            //    Value = "CHIC",
            //    Selected = false
            //});

            lista.Add(new SelectListItem
            {
                Text = "AniPlan",
                Value = "AniPlan",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "Rastreabilidade",
                Value = "Rastreabilidade",
                Selected = false
            });

            return lista;
        }

        public List<SelectListItem> CarregaTipoRelatorioProgDiarioTransp()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            lista.Add(new SelectListItem
            {
                Text = "Diário",
                Value = "Diario",
                Selected = true
            });

            lista.Add(new SelectListItem
            {
                Text = "Período",
                Value = "Periodo",
                Selected = true
            });

            return lista;
        }

        public List<SelectListItem> CarregaListaEmpresaTransportador()
        {
            List<SelectListItem> listaItens = new List<SelectListItem>();

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspTransema",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                listaItens.Add(new SelectListItem
                {
                    Text = "Transema",
                    Value = "TR",
                    Selected = false
                });
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspPlanalto",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                listaItens.Add(new SelectListItem
                {
                    Text = "Planalto",
                    Value = "PL",
                    Selected = false
                });
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspH&N",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                listaItens.Add(new SelectListItem
                {
                    Text = "H&N",
                    Value = "HN",
                    Selected = false
                });
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspExportacao",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                listaItens.Add(new SelectListItem
                {
                    Text = "Exportação",
                    Value = "EX",
                    Selected = false
                });
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspAlojInterno",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                listaItens.Add(new SelectListItem
                {
                    Text = "Alojamento Interno",
                    Value = "AI",
                    Selected = false
                });
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspTransfOvos",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                listaItens.Add(new SelectListItem
                {
                    Text = "Transferência de Ovos",
                    Value = "TO",
                    Selected = false
                });
            }

            return listaItens;
        }

        public List<SelectListItem> CarregaEmpresas()
        {
            List<SelectListItem> listaEmpresas = new List<SelectListItem>();
            
            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

                string empresaCod = Session["empresa"].ToString().Substring(i, 2);

                Empresas empresa = hlbapp.Empresas.Where(w => w.CodigoCHIC == empresaCod).FirstOrDefault();

                listaEmpresas.Add(new SelectListItem
                {
                    Text = empresa.DescricaoApoloVendedor,
                    Value = empresaCod,
                    Selected = false
                });
            }

            return listaEmpresas;
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

        #endregion

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
                //if (!vendedor.Equals("(Todos)"))
                //{
                //    salesman.FillByCode(chic.salesman, vendedor);

                //    if (chic.salesman[0].inv_comp.Trim() == model["Empresa"].ToString() || model["Empresa"].ToString() == "(Todas)")
                //    {
                //        Session["sEmpresa"] = model["Empresa"].ToString();
                //        empresa = model["Empresa"].ToString();
                //    }
                //    else
                //    {
                //        ViewBag.erro = "O Vendedor selecionado não pertence a Empresa selecionada. Verifique!";
                //        return View("Index");
                //    }
                //}
                //else
                //{
                    empresa = model["Empresa"].ToString();
                //}
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

            var dataVerificacao = Convert.ToDateTime("01/03/2021");

            if (Convert.ToDateTime(model["dataIni"]) < dataVerificacao && Convert.ToDateTime(model["dataFim"]) > dataVerificacao)
            {
                //ViewBag.erro = "Não é possível gerar o relatório para o período de " + Convert.ToDateTime(model["dataIni"]).ToString("dd/MM/yyyy")
                //    + " até " + Convert.ToDateTime(model["dataFim"]).ToString("dd/MM/yyyy") + " porque foi realizada a migração para o sistema AniPlan "
                //    + " que inicializa com pedidos de 01/03/2021 em diante! Por favor, caso queiram dados antes de 01/03/2021, gerar com a data final"
                //    + " até 28/02/2021! Caso queira os dados novos, gerar com a data inicial de 01/03/2021!";
                ViewBag.erro = "Período lançado incorreto! Devido troca do sistema, relatórios “antigos” devem ter como limite de data final 28/02/21 e " +
                    "“novos” inicial a partir de 01/03/21.";
                return View("Index");
            }

            /**** 21/03/2021 - DESATIVADO DEVIDO A MIGRAÇÃO DO CHIC P/ ANIPLAN ****/

            if (Convert.ToDateTime(model["dataFim"]) < dataVerificacao)
            {
                vendedor = vendedor.Substring(1, 6);
                destino = GeraRelatorioListaPedidos(pesquisa, true, pasta, destino,
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    empresa, vendedor, linhagem, uf, cliente);
            }
            else
            {
                destino = GeraRelatorioListaPedidosAniPlan(pesquisa, true, pasta, destino,
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    empresa, vendedor, linhagem, uf, cliente);
            }

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
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Pedidos_Importados";

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
                        destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + item.Text.Trim() + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx";

                        /**** 21/03/2021 - DESATIVADO DEVIDO A MIGRAÇÃO DO CHIC P/ ANIPLAN ****/
                        //destino = GeraRelatorioListaPedidos("", false, pasta, destino, Convert.ToDateTime(model["dataIni"].ToString()),
                        //    Convert.ToDateTime(model["dataFim"].ToString()), item.Text.Trim(),
                        //        itemVendedor.sl_code.Trim(), "(Todas)", "(Todos)", "(Todos)");

                        destino = GeraRelatorioListaPedidosAniPlan("", false, pasta, destino, Convert.ToDateTime(model["dataIni"].ToString()),
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
                        email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
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
            //string destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
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
                    "s.salesman `Ved. / Repres.`, " +
                    "cc.codestab `Cód. Estabelecimento`, " + 
                    "cc.protoc `Nº Protocolo`, " + 
                    "cc.registro `Nº Registro`, " +
                    "c.region `CPF/CNPJ`, " +
                    "cc.datereg `validade registro`, " +
                    "b.location `incub.` ";

            string commandTextCHICTabelas =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on b.customer = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join custcust cc on c.custno = cc.custno ";

            string commandTextCHICCondicaoJoins =
                "where ";
                    //"c.custno = cc.custno and " +
                    //"b.orderno = o.orderno and " +
                    //"b.item = i.item_no and  " +
                    //"i.variety = v.variety and " +
                    //"b.customer = c.custno and " +
                    //"o.salesrep = s.sl_code and ";

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
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = 'Todos)') ";

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
                    "o.delivery, " +
                    "cc.codestab, " +
                    "cc.protoc, " +
                    "cc.registro," +
                    "c.region, " +
                    "cc.datereg, " +
                    "b.location " +

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
                    "s.salesman `Ved. / Repres.`, " +
                    "cc.codestab `Cód. Estabelecimento`, " +
                    "cc.protoc `Nº Protocolo`, " +
                    "cc.registro `Nº Registro`, " +
                    "c.region `CPF/CNPJ`, " +
                    "cc.datereg `validade registro`, " +
                    "b.location `incub.` ";

            string commandTextCHICTabelas02 =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on b.customer = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join custcust cc on c.custno = cc.custno ";

            string commandTextCHICCondicaoJoins02 =
                "where ";
                    //"c.custno = cc.custno and " +
                    //"b.orderno = o.orderno and " +
                    //"b.item = i.item_no and  " +
                    //"i.variety = v.variety and " +
                    //"b.customer = c.custno and " +
                    //"o.salesrep = s.sl_code and ";

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
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = 'Todos)') ";

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
                    "o.delivery, " +
                    "cc.codestab, " +
                    "cc.protoc, " +
                    "cc.registro, " +
                    "c.region, " +
                    "cc.datereg, " +
                    "b.location ";

            string commandTextCHICOrdenacao =
                "order by " +
                    "1, 3";

            #endregion

            #region SQL Dados

            string commandTextCHICCabecalhoFaturamentoDados =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi, ic.codestab, ci.sobra ";

            string commandTextCHICTabelasFaturamentoDados =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on o.cust_no = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join int_comm ic on o.orderno = ic.orderno " +
                    "left join custitem ci on b.bookkey = ci.bookkey ";

            string commandTextCHICCondicaoJoinsFaturamentoDados =
                "where ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados = "";

            string commandTextCHICCondicaoParametrosFaturamentoDados =
                    "b.cal_date between {" + dataInicialStr + "} and {" + dataFinalStr + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(c.state = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "(c.custno = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = 'Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados = "";

            string commandTextCHICOrdenacaoFaturamentoDados = " Union ";

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi, ic.codestab, ci.sobra ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on o.cust_no = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join int_comm ic on o.orderno = ic.orderno " +
                    "left join custitem ci on b.bookkey = ci.bookkey ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 =
                "where ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 = "";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(c.state = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "(c.custno = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = 'Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "";

            #endregion

            #region SQL Cancelados

            string commandTextCHICCabecalhoCancelados =
                "select " +
                    "* ";

            string commandTextCHICTabelasCancelados =
                "from " +
                    "VU_Lista_Pedidos_Cancelados_WEB ";

            string commandTextCHICCondicaoJoinsCancelados =
                "where ";

            string commandTextCHICCondicaoFiltrosCancelados = "";

            string dataInicialStrSql = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrSql = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosCancelados =
                    "[data entrega inicial] >= '" + dataInicialStrSql + "' and  " +
                    "[data entrega final] <= '" + dataFinalStrSql + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "([uf] = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "([codigo cliente] = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "([codigo vendedor] = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoCancelados =
                "order by " +
                    "2, " +
                    "ID";

            #endregion

            #region SQL Pedidos WEB

            string commandTextCHICCabecalhoPedidosWEB =
                "select " +
                    "* ";

            string commandTextCHICTabelasPedidosWEB =
                "from " +
                    "VU_Lista_Pedidos_WEB ";

            string commandTextCHICCondicaoJoinsPedidosWEB =
                "where ";

            string commandTextCHICCondicaoFiltrosPedidosWEB = "";

            string commandTextCHICCondicaoParametrosPedidosWEB =
                    "[data nascimento] >= '" + dataInicialStrSql + "' and  " +
                    "[data nascimento] <= '" + dataFinalStrSql + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "([uf] = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "([codigo cliente] = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "([codigo vendedor] = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoPedidosWEB =
                "order by " +
                    "2, " +
                    "ID";

            #endregion

            string dataExibicao = dataInicial.ToString("dd/MM/yyyy") + " à " +
                dataFinal.ToString("dd/MM/yyyy");
            string vendedorExibicao = "";
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
            if (nomeVendedor == "")
                vendedorExibicao = vendedor;
            else
                vendedorExibicao = nomeVendedor;

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Pedidos - CHIC"];
            worksheet.Cells[2, 7] = dataExibicao;
            worksheet.Cells[3, 7] = vendedorExibicao;

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
                else if (item.Name.Equals("Cancelamentos_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoCancelados + commandTextCHICTabelasCancelados +
                        commandTextCHICCondicaoJoinsCancelados + commandTextCHICCondicaoFiltrosCancelados + 
                        commandTextCHICCondicaoParametrosCancelados + commandTextCHICAgrupamentoCancelados;
                else if (item.Name.Equals("Pedidos_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoPedidosWEB + commandTextCHICTabelasPedidosWEB +
                        commandTextCHICCondicaoJoinsPedidosWEB + commandTextCHICCondicaoFiltrosPedidosWEB +
                        commandTextCHICCondicaoParametrosPedidosWEB + commandTextCHICAgrupamentoPedidosWEB;
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
            //string destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
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

        #region Lista de Pedidos Por Representante

        public ActionResult ListaPedidosRepresentante()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaEmpresasVendedores(false);
            CarregaLinhagens();
            CarregaListaEstados();
            CarregaClientes();

            return View();
        }

        [HttpPost]
        public ActionResult DownloadListaPedidosRepresentante(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            string empresa = Session["empresaLayout"].ToString();

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

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Pedidos_Representante_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Relatorio_Pedidos_Representante_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            #region Carrega Nome Vendedor

            string nomeVendedor = "";
            string codigoVendedorApolo = "0" + vendedor;
            MvcAppHyLinedoBrasil.Models.VENDEDOR vendedorObj = bdApolo.VENDEDOR
                .Where(w => w.VendCod == codigoVendedorApolo).FirstOrDefault();
            if (vendedorObj != null) nomeVendedor = vendedorObj.VendNome;

            #endregion

            destino = GerarListaPedidosRepresentante(pesquisa, true, pasta, destino,
                Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                vendedor, nomeVendedor);

            return File(destino, "Download", "Pedidos_Representante_"
                + codigoVendedorApolo + "_"
                + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GerarListaPedidosRepresentante(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string vendedor, 
            string nomeVendedor)
        {
            #region Dados Excel

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Pedidos_Representante.xlsx", destino);

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

            #region Carrega Dados Planilha

            //string nomeVendedor = "";
            string codigoVendedorApolo = "0" + vendedor;
            //VENDEDOR vendedorObj = bdApolo.VENDEDOR
            //    .Where(w => w.VendCod == codigoVendedorApolo).FirstOrDefault();
            //if (vendedorObj != null) nomeVendedor = vendedorObj.VendNome;

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Dados"];

            worksheet.Cells[4, 3] = nomeVendedor;
            worksheet.Cells[5, 4] = dataInicial;
            worksheet.Cells[6, 4] = dataFinal;

            #endregion

            #region Dados SQL

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VW_Dados_SAC_Representante ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = Convert.ToDateTime(dataInicial).ToString("yyyy-MM-dd") + " 00:00:00";
            string dataFinalStr = Convert.ToDateTime(dataFinal).ToString("yyyy-MM-dd") + " 23:59:59";

            string commandTextCHICCondicaoParametros =
                    //"(Empresa = '" + empresa + "' or '" + empresa + "' = '(TODAS)') and " +
                    "[Nascimento] >= '" + dataInicialStr + "' and " +
                    "[Nascimento] <= '" + dataFinalStr + "' and " +
                    "[Cód. Vendedor] = '" + codigoVendedorApolo + "' ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao = "";

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

            #endregion

            #region Comandos Excel Final

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

        #endregion

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
            Session["ListaTipoProduto"] = CarregaTipoProduto();

            return View();
        }

        [HttpPost]
        public ActionResult GerarPlanejamentoIncubacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            #region Datas

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

            #region Tipo de Produto

            string tipoProduto = "";
            if (model["TipoProduto"] != null)
            {
                tipoProduto = model["TipoProduto"];
            }
            AtualizaDDL(tipoProduto, (List<SelectListItem>)Session["ListaTipoProduto"]);

            #endregion

            #region Incubatório - CheckBoxs

            string incubatorios = "";
            foreach (var item in (List<SelectListItem>)Session["ListaIncubatorios"])
            {
                if (model[item.Value].ToString().Contains("true"))
                {
                    incubatorios = incubatorios + item.Value;
                    AtualizaDDL(item.Value, (List<SelectListItem>)Session["ListaIncubatorios"]);
                }
            }

            #endregion

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Planejamento_Incubacao_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Planejamento_Incubacao_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            var dataVerificacao = Convert.ToDateTime("01/03/2021");

            if (Convert.ToDateTime(model["dataIni"]) < dataVerificacao && Convert.ToDateTime(model["dataFim"]) > dataVerificacao)
            {
                ViewBag.erro = "Período lançado incorreto! Devido troca do sistema, relatórios “antigos” devem ter como limite de data final 28/02/21 e " +
                    "“novos” inicial a partir de 01/03/21.";
                return View("Index");
            }

            /**** 21/03/2021 - DESATIVADO DEVIDO A MIGRAÇÃO DO CHIC P/ ANIPLAN ****/

            if (Convert.ToDateTime(model["dataFim"]) < dataVerificacao)
            {
                destino = GeraPlanejamentoIncubacao(pesquisa, true, pasta, destino,
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    incubatorios, tipoProduto);
            }
            else
            {
                destino = GeraPlanejamentoIncubacaoAniPlan(pesquisa, true, pasta, destino,
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    incubatorios, tipoProduto);
            }

            return File(destino, "Download", "Planejamento_Incubacao_"
                + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraPlanejamentoIncubacao(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorios, string tipoProduto)
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
                "select *, " +
                    " IIF(Substr(i.form,1,1) = 'H','Ovo',IIF(Substr(i.form,1,1) = 'D','Pinto','     ')) Classificacao ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, capacity cp ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and " +
                    "b.location = cp.hatchery and " +
                    "((cp.dayno between 1 and 2 and substr(i.form,1,1) <> 'H') or (cp.dayno between 1 and 5 and substr(i.form,1,1) = 'H')) and ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 = "";

            string filtroTipoProduto = "";
            if (tipoProduto != "")
                filtroTipoProduto = "and 0 < (select count(1) from booked b1, items i1 where b1.orderno = o.orderno and " +
                    "b1.item = i1.item_no and substr(i1.form,1,1) = '" + tipoProduto + "') ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                    //"(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todas)') ";
                    "AT(rtrim(b.location), '" + incubatorios + "') > 0 " +
                    filtroTipoProduto;

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
                "select ic.orderno, ic.hatchinf, ic.codestab ";

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
                    //"(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todas)') ";
                    "AT(rtrim(b.location), '" + incubatorios + "') > 0 " +
                    filtroTipoProduto;

            string commandTextCHICAgrupamentoComentarios02 = "";

            string commandTextCHICOrdenacaoComentarios02 = "";

            #endregion

            #region SQL Pedidos WEB

            string commandTextPedidoWEBCabecalho =
                "select * ";

            string commandTextPedidoWEBTabelas =
                "from " +
                    "Item_Pedido_Venda ";

            string commandTextPedidoWEBCondicaoJoins =
                "where ";

            string commandTextPedidoWEBCondicaoFiltros = "";

            string dataInicialStrEntrega = dataInicial.AddDays(-30).ToString("yyyy-MM-dd") + " 00:00:00";
            string dataFinalStrEntrega = dataFinal.AddDays(30).ToString("yyyy-MM-dd") + " 23:59:59";

            string commandTextPedidoWEBCondicaoParametros =
                "DataEntregaInicial >= '" + dataInicialStrEntrega + "' and " +
                "DataEntregaFinal <= '" + dataFinalStrEntrega + "'";

            string commandTextPedidoWEBAgrupamento = "";

            string commandTextPedidoWEBOrdenacao = "";

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
                else if (item.Name.Equals("Pedidos_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextPedidoWEBCabecalho + commandTextPedidoWEBTabelas +
                        commandTextPedidoWEBCondicaoJoins +
                        commandTextPedidoWEBCondicaoFiltros + commandTextPedidoWEBCondicaoParametros +
                        commandTextPedidoWEBAgrupamento +
                        commandTextPedidoWEBOrdenacao;
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

        #region Planejamento Vacinação

        public ActionResult PlanejamentoVacinacao()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaListaIncubatorios();

            return View();
        }

        [HttpPost]
        public ActionResult GerarPlanejamentoVacinacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            #region Datas

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

            #region Incubatório - DropDownList (DESATIVADO)

            //string incubatorio = "";
            //if (model["Incubatorio"] != null)
            //{
            //    incubatorio = model["Incubatorio"];
            //}
            //else
            //{
            //    incubatorio = "(Todos)";
            //}
            //AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);

            #endregion

            #region Incubatório - CheckBoxs

            string incubatorios = "";
            foreach (var item in (List<SelectListItem>)Session["ListaIncubatorios"])
            {
                if (model[item.Value].ToString().Contains("true"))
                {
                    incubatorios = incubatorios + item.Value;
                    AtualizaDDL(item.Value, (List<SelectListItem>)Session["ListaIncubatorios"]);
                }
            }

            #endregion

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Planejamento_Vacinacao_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Planejamento_Vacinacao_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            var dataVerificacao = Convert.ToDateTime("01/03/2021");

            if (Convert.ToDateTime(model["dataIni"]) < dataVerificacao && Convert.ToDateTime(model["dataFim"]) > dataVerificacao)
            {
                ViewBag.erro = "Período lançado incorreto! Devido troca do sistema, relatórios “antigos” devem ter como limite de data final 28/02/21 e " +
                    "“novos” inicial a partir de 01/03/21.";
                return View("Index");
            }

            /**** 21/03/2021 - DESATIVADO DEVIDO A MIGRAÇÃO DO CHIC P/ ANIPLAN ****/

            if (Convert.ToDateTime(model["dataFim"]) < dataVerificacao)
            {
                destino = GeraPlanejamentoVacinacao(pesquisa, true, pasta, destino,
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    incubatorios);
            }
            else
            {
                destino = GeraPlanejamentoVacinacaoAniPlan(pesquisa, true, pasta, destino,
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    incubatorios);
            }

            return File(destino, "Download", "Planejamento_Vacinacao_"
                + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraPlanejamentoVacinacao(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorios)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Planejamento_Vacinacao"
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

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select * ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "booked b ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 =
                "inner join orders o on b.orderno = o.orderno " +
                "inner join items i on b.item = i.item_no " +
                "inner join vartabl v on i.variety = v.variety " +
                "inner join cust c on o.cust_no = c.custno " +
                "inner join salesman s on o.salesrep = s.sl_code " +
                "left join custitem ci on b.bookkey = ci.bookkey ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 =
                "where " +
                    "i.form = 'VC' and ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                //"(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todas)') ";
                    "AT(rtrim(b.location), '" + incubatorios + "') > 0 ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("CHIC_Faturamento_Dados"))
                    item.OLEDBConnection.CommandText =
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

        #endregion

        #region Lista de Vacinas e Serviços

        public ActionResult ListaVacinasServicos()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaEmpresasVendedores(true);
            CarregaListaIncubatorios();
            Session["ListaLaboratorios"] = CarregaLaboratorios();
            Session["ListaOrigens"] = CarregaOrigensRelatorioVacinas();

            return View();
        }

        [HttpPost]
        public ActionResult GerarListaVacinasServicos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            #region Empresa

            string empresa = "";

            if (model["Empresa"] != null)
            {
                empresa = model["Empresa"].ToString();
            }
            else
            {
                empresa = Session["empresaLayout"].ToString();
            }

            AtualizaEmpresaSelecionada(empresa);

            #endregion

            #region Datas

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

            #region Incubatório - DropDownList (DESATIVADO)

            //string incubatorio = "";
            //if (model["Incubatorio"] != null)
            //{
            //    incubatorio = model["Incubatorio"];
            //}
            //else
            //{
            //    incubatorio = "(Todos)";
            //}
            //AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);

            #endregion

            #region Incubatório - CheckBoxs

            string incubatorios = "";
            foreach (var item in (List<SelectListItem>)Session["ListaIncubatorios"])
            {
                if (model[item.Value].ToString().Contains("true"))
                {
                    incubatorios = incubatorios + item.Value;
                    AtualizaDDL(item.Value, (List<SelectListItem>)Session["ListaIncubatorios"]);
                }
            }

            #endregion

            #region Laboratório

            string laboratorio = "(Todos)";

            if (model["Laboratorio"] != null)
            {
                laboratorio = model["Laboratorio"].ToString();
            }

            AtualizaDDL(laboratorio, (List<SelectListItem>)Session["ListaLaboratorios"]);

            #endregion

            #region Origem Relatório

            string origem = "(Todos)";

            if (model["Origem"] != null)
            {
                origem = model["Origem"].ToString();
            }

            AtualizaDDL(origem, (List<SelectListItem>)Session["ListaOrigens"]);

            #endregion

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais\\ListaVacinasServicos_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*ListaVacinasServicos_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            if (origem == "CHIC")
            {
                destino = GeraListaVacinasServicos(pesquisa, true, pasta, destino, empresa.Replace(",", ""),
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    incubatorios, laboratorio);
            }
            else if (origem == "AniPlan")
            {
                destino = GeraListaVacinasServicosAniPlan(pesquisa, true, pasta, destino, empresa.Replace(",", ""),
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    incubatorios, laboratorio);
            }
            else
            {
                destino = GeraListaVacinasWEB(pesquisa, true, pasta, destino, empresa.Replace(",", ""),
                    Convert.ToDateTime(model["dataIni"]), Convert.ToDateTime(model["dataFim"]),
                    incubatorios, laboratorio);
            }

            return File(destino, "Download", "ListaVacinas_" + origem + "_" + empresa.Replace("(","").Replace(")","").Replace(",","") + "_"
                + Convert.ToDateTime(model["dataIni"]).ToString("yyyy-MM-dd") +
                "_a_" + Convert.ToDateTime(model["dataFim"]).ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraListaVacinasServicos(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, string empresa, DateTime dataInicial, DateTime dataFinal, string incubatorios, string laboratorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais\\ListaVacinasServicos.xlsx", destino);

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

            #region CHIC_Vacinas_Servicos

            string dataInicialStr = dataInicial.ToString("MM/dd/yyyy");
            string dataFinalStr = dataFinal.ToString("MM/dd/yyyy");
            string dataInicialStrCalDate = dataInicial.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrCalDate = dataFinal.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select " +
                    "s.inv_comp `empresa`, " +
                    "b.cal_date `data incubação`, " +
                    "b.cal_date+21 `data nascimento`, " +
                    "o.orderno `nº pedido`, " +
                    "o.cust_no `cód. cliente`, " +
                    "c.name `nome cliente`, " +
                    "c.city `cidade`, " +
                    "c.state `uf`, " +
                    "c.country `país`, " +
                    "b.location `inc.`, " +
                    "b.item_ord `ordem`, " +
                    //"b.item `cód. produto`, " +
                    "IIF(b.item = '165', IIF(b.location = 'AJ', '909', '901'), b.item) `cód. produto`, " +
                    "i.account_no `cód. apolo`, " +
                    "IIF(b.item = '165', IIF(b.location = 'AJ', 'CRYOMAREX (RISPENS) - BOEHRINGER                    ', 'RISMAVAC (RISPENS) - MSD                    '), i.item_desc) `descricao produto`, " +
                    "IIF(ci.bookkey > 0,ci.cobvcsv, 'Normal') `tipo cobrança vacina`, " +
                    "o.delivery `cond. pag.`, " +
                    "b.price `valor unitário`, " +
                    "b.quantity `qtde. aplicada (01 dose para cada 01 pintainha)`, " +
                    "b.price * b.quantity `valor total` ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "booked b ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 =
                "inner join orders o on b.orderno = o.orderno " +
                "inner join items i on b.item = i.item_no " +
                "inner join vartabl v on i.variety = v.variety " +
                "inner join cust c on o.cust_no = c.custno " +
                "inner join salesman s on o.salesrep = s.sl_code " +
                "left join custitem ci on b.bookkey = ci.bookkey ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 =
                "where " +
                    // 17/04/2020 - Solicitado por Marcelo Notti: excluir os serviços e os probióticos (NEOFLORA e POLTRYSTAR)
                    "i.form in ('VC','SV') and ";
                    //"i.form in ('VC') and " +
                    //"i.item_desc not like '%(SIMBIOTICO)%' and ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                //"(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todas)') ";
                    "AT(rtrim(b.location), '" + incubatorios + "') > 0 and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(IIF(b.item = '165', IIF(b.location = 'AJ', 'CRYOMAREX (RISPENS) - BOEHRINGER', 'RISMAVAC (RISPENS) - MSD'), i.item_desc) like '%" 
                        + laboratorio + "%' or '" + laboratorio + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "order by b.cal_date, o.orderno, b.item_ord";

            #endregion

            #region CHIC_Resumo_Vacinas_Servicos

            string commandTextCHICCabecalhoFaturamentoDadosRes =
                "select " +
                    "s.inv_comp `empresa`, " +
                    "c.country `país`, " +
                    //"b.item `cód. produto`, " +
                    "MAX(IIF(b.item = '165', IIF(b.location = 'AJ', '909', '901'), b.item)) `cód. produto`, " +
                    "i.account_no `cód. apolo`, " +
                    "MAX(IIF(b.item = '165', IIF(b.location = 'AJ', 'CRYOMAREX (RISPENS) - BOEHRINGER                    ', 'RISMAVAC (RISPENS) - MSD                    '), i.item_desc)) `descricao produto`, " +
                    "i.form `tipo`, " +
                    "SUM(b.quantity) `qtde. total`, " +
                    "SUM(IIF(trim(ci.cobvcsv) = 'Cliente Envia',b.quantity, 0)) `qtde. cliente envia`, " +
                    //"SUM(IIF(b.price > 0,b.quantity, 0))-SUM(IIF(trim(ci.cobvcsv) = 'Cliente Envia' and b.price > 0,b.quantity, 0)) `qtde. normal`, " +
                    "SUM(IIF(IIF(ci.bookkey > 0,ci.cobvcsv, 'Normal') = 'Normal',b.quantity, 0))-SUM(IIF(b.price > 0,b.quantity, 0)) `qtde. normal`, " +
                    //"SUM(IIF(b.price = 0,b.quantity, 0))-SUM(IIF(trim(ci.cobvcsv) = 'Cliente Envia' and b.price = 0,b.quantity, 0)) `qtde. bonificada`, " +
                    "SUM(IIF(trim(ci.cobvcsv) = 'Bonificação',b.quantity, 0)) `qtde. bonificada`, " +
                    "SUM(IIF(b.price > 0,b.quantity, 0)) `qtde. cobrada`, " +
                    //"AVG(b.price) `valor unitário`, " +
                    "SUM(b.price * b.quantity) `valor cobrado` ";

            string commandTextCHICTabelasFaturamentoDadosRes =
                "from " +
                    "booked b ";

            string commandTextCHICCondicaoJoinsFaturamentoDadosRes =
                "inner join orders o on b.orderno = o.orderno " +
                "inner join items i on b.item = i.item_no " +
                "inner join vartabl v on i.variety = v.variety " +
                "inner join cust c on o.cust_no = c.custno " +
                "inner join salesman s on o.salesrep = s.sl_code " +
                "left join custitem ci on b.bookkey = ci.bookkey ";

            string commandTextCHICCondicaoFiltrosFaturamentoDadosRes =
                "where " +
                    //"i.form in ('VC') and " +
                    "i.form in ('VC','SV') and ";
                    //"i.item_desc not like '%(SIMBIOTICO)%' and ";

            string commandTextCHICCondicaoParametrosFaturamentoDadosRes =
                    "b.cal_date between {" + dataInicialStrCalDate + "} and {" + dataFinalStrCalDate + "} and " +
                //"(b.location = '" + incubatorio + "' or '" + incubatorio + "' = '(Todas)') ";
                    "AT(rtrim(b.location), '" + incubatorios + "') > 0 and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(IIF(b.item = '165', IIF(b.location = 'AJ', 'CRYOMAREX (RISPENS) - BOEHRINGER', 'RISMAVAC (RISPENS) - MSD'), i.item_desc) like '%"
                        + laboratorio + "%' or '" + laboratorio + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDadosRes =
                "group by s.inv_comp, c.country, b.item, i.account_no, i.item_desc, i.form ";

            string commandTextCHICOrdenacaoFaturamentoDadosRes = "order by s.inv_comp, i.form, i.item_desc";

            #endregion

            #region Dados_NF

            string dataInicialNFStr = dataInicial.AddDays(-30).ToString("yyyy-MM-dd");
            string dataFinalNFStr = dataFinal.AddDays(30).ToString("yyyy-MM-dd");
            
            string commandTextCHICCabecalhoFaturamentoDadosNF =
                "select " +
                    "NF.EmpCod, " +
                    "NF.CtrlDFModForm, " +
                    "NF.CtrlDFSerie, " +
                    "NF.NFNum, " +
                    "PV1.USERPEDCHIC ";

            string commandTextCHICTabelasFaturamentoDadosNF =
                "from " +
                    "NOTA_FISCAL NF With(Nolock) ";

            string commandTextCHICCondicaoJoinsFaturamentoDadosNF =
                "inner join PED_VENDA1 PV1 With(Nolock) on NF.EmpCod = PV1.EmpCod and NF.NFPedVenda = PV1.PedVendaNum ";

            string commandTextCHICCondicaoFiltrosFaturamentoDadosNF =
                "where ";
            
            string commandTextCHICCondicaoParametrosFaturamentoDadosNF =
                    "NF.NFDataEmis between '" + dataInicialNFStr + "' and '" + dataFinalNFStr + "' ";

            string commandTextCHICAgrupamentoFaturamentoDadosNF = "";

            string commandTextCHICOrdenacaoFaturamentoDadosNF = "";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("CHIC_Vacinas_Servicos"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamentoDados02 + commandTextCHICTabelasFaturamentoDados02 +
                        commandTextCHICCondicaoJoinsFaturamentoDados02 +
                        commandTextCHICCondicaoFiltrosFaturamentoDados02 + commandTextCHICCondicaoParametrosFaturamentoDados02 +
                        commandTextCHICAgrupamentoFaturamentoDados02 +
                        commandTextCHICOrdenacaoFaturamentoDados02;
                if (item.Name.Equals("CHIC_Resumo_Vacinas_Servicos"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamentoDadosRes + commandTextCHICTabelasFaturamentoDadosRes +
                        commandTextCHICCondicaoJoinsFaturamentoDadosRes +
                        commandTextCHICCondicaoFiltrosFaturamentoDadosRes + commandTextCHICCondicaoParametrosFaturamentoDadosRes +
                        commandTextCHICAgrupamentoFaturamentoDadosRes +
                        commandTextCHICOrdenacaoFaturamentoDadosRes;
                if (item.Name.Equals("Dados_NF"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamentoDadosNF + commandTextCHICTabelasFaturamentoDadosNF +
                        commandTextCHICCondicaoJoinsFaturamentoDadosNF +
                        commandTextCHICCondicaoFiltrosFaturamentoDadosNF + commandTextCHICCondicaoParametrosFaturamentoDadosNF +
                        commandTextCHICAgrupamentoFaturamentoDadosNF +
                        commandTextCHICOrdenacaoFaturamentoDadosNF;
            }

            oBook.RefreshAll();

            #region Filtrar Tabela Dinâmica
            
            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["VACINAS POR CIDADE X UF"];
            Excel.PivotTable pvt = worksheet.PivotTables("Tabela dinâmica1") as Excel.PivotTable;
            List<string> ListToFilter = new List<string>();
            ListToFilter.Add("TRATAMENTO INFRAVERMELHO");
            ListToFilter.Add("USO DE HIDRATANTE");
            ListToFilter.Add("NEOFLORA (SIMBIOTICO) - BIOSYN");
            ListToFilter.Add("POULTRY STAR (SIMBIOTICO) - BIOMIN");

            //Excel.PivotFields _PivotFields = (Excel.PivotFields)pvt.get_PageFields(Missing.Value);
            Excel.PivotFields _PivotFields = (Excel.PivotFields)pvt.get_ColumnFields(Missing.Value);

            foreach (Excel.PivotField _PivotField in _PivotFields)
            {
                if (string.Compare(_PivotField.Caption, "descricao produto", true) == 0)
                {
                    Excel.PivotItems _PivotItems = (Excel.PivotItems)_PivotField.PivotItems(Missing.Value);
                    foreach (Excel.PivotItem _PivotItem in _PivotItems)
                    {
                        if (ListToFilter.Contains(_PivotItem.Caption))
                            _PivotItem.Visible = false;
                        else
                            _PivotItem.Visible = true;
                    }
                }
            }

            #endregion

            #region Esconder outras abas, Bloquear Planilha e Pasta de Trabalho

            Excel._Worksheet worksheetResumo = (Excel._Worksheet)oBook.Worksheets["RESUMO DE VACINAS E SERVIÇOS"];
            Excel._Worksheet worksheetLista = (Excel._Worksheet)oBook.Worksheets["RELATÓRIO DE VACINAS E SERVIÇOS"];
            worksheetResumo.Visible = Excel.XlSheetVisibility.xlSheetHidden;
            worksheetLista.Visible = Excel.XlSheetVisibility.xlSheetHidden;
            worksheet.Protect(dataFinal.ToString("ddMMyyyy#"), true, true, false, false, false, false, false, false, false, false, false, false, false, true, false);
            oBook.Protect(dataFinal.ToString("ddMMyyyy#"));

            #endregion

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

        public string GeraListaVacinasWEB(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, string empresa, DateTime dataInicial, DateTime dataFinal, string incubatorios, string laboratorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais\\ListaVacinas_WEB.xlsx", destino);

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

            #region VU_Lista_Vacinas_WEB

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select " +
                    "* ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "VU_Lista_Vacinas_WEB ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 = "";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 =
                "where ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "CHARINDEX([Inc.], '" + incubatorios + "') > 0 and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(Laboratorio = '" + laboratorio + "' or '" + laboratorio + "' = '') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "order by [Data Nascimento], [Nº Pedido], Vacina";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Lista_Vacinas_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamentoDados02 + commandTextCHICTabelasFaturamentoDados02 +
                        commandTextCHICCondicaoJoinsFaturamentoDados02 +
                        commandTextCHICCondicaoFiltrosFaturamentoDados02 + commandTextCHICCondicaoParametrosFaturamentoDados02 +
                        commandTextCHICAgrupamentoFaturamentoDados02 +
                        commandTextCHICOrdenacaoFaturamentoDados02;
            }

            oBook.RefreshAll();

            #region Esconder outras abas, Bloquear Planilha e Pasta de Trabalho

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["VACINAS POR CIDADE X UF"];
            Excel._Worksheet worksheetLista = (Excel._Worksheet)oBook.Worksheets["RELATÓRIO DE VACINAS"];
            worksheetLista.Visible = Excel.XlSheetVisibility.xlSheetHidden;
            worksheet.Protect(dataFinal.ToString("ddMMyyyy#"), true, true, false, false, false, false, false, false, false, false, false, false, false, true, false);
            oBook.Protect(dataFinal.ToString("ddMMyyyy#"));

            #endregion

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

        #region Programação de Transportes

        public ActionResult ProgramacaoTransportes()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaListaIncubatorios();
            Session["ListaTipoRelatorioProgDiarioTransp"] = CarregaTipoRelatorioProgDiarioTransp();
            Session["ListaEmpresaTransportador"] = CarregaListaEmpresaTransportador();
            Session["ListaEmpresas"] = CarregaEmpresas();
            CarregaEmpresasVendedores(true);

            return View();
        }

        [HttpPost]
        public ActionResult GerarProgramacaoDiariaTransportes(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            #region Tipo de Relatório

            string tipoRelatorio = "";
            if (model["TipoRelatorio"] != null)
            {
                tipoRelatorio = model["TipoRelatorio"];
            }
            AtualizaDDL(tipoRelatorio, (List<SelectListItem>)Session["ListaTipoRelatorioProgDiarioTransp"]);

            #endregion

            #region Datas

            DateTime dataIni = new DateTime();
            if (tipoRelatorio == "Periodo")
            {
                if (model["dataIni"] != null)
                {
                    Session["sDataInicial"] = Convert.ToDateTime(model["dataIni"].ToString()).ToShortDateString();
                    dataIni = Convert.ToDateTime(model["dataIni"].ToString());
                }
                else
                {
                    ViewBag.erro = "Por favor, inserir data Inicial!";
                    return View("Index");
                }
            }
            else
            {
                if (model["dataNascimento"] != null)
                {
                    Session["sDataInicial"] = Convert.ToDateTime(model["dataNascimento"].ToString()).ToShortDateString();
                    dataIni = Convert.ToDateTime(model["dataNascimento"].ToString());
                }
                else
                {
                    ViewBag.erro = "Por favor, inserir a data!";
                    return View("Index");
                }
            }

            DateTime dataFim = new DateTime();
            if (model["dataFim"] != null)
            {
                Session["sDataFinal"] = Convert.ToDateTime(model["dataFim"].ToString()).ToShortDateString();
                dataFim = Convert.ToDateTime(model["dataFim"].ToString());
            }
            else
            {
                if (tipoRelatorio == "Periodo")
                {
                    ViewBag.erro = "Por favor, inserir Data Final!";
                    return View("Index");
                }
            }

            #endregion

            #region Incubatório - CheckBoxs

            string incubatorios = "";
            foreach (var item in (List<SelectListItem>)Session["ListaIncubatorios"])
            {
                if (model[item.Value].ToString().Contains("true"))
                {
                    incubatorios = incubatorios + item.Value;
                    AtualizaDDL(item.Value, (List<SelectListItem>)Session["ListaIncubatorios"]);
                }
            }

            #endregion

            #region Empresas - CheckBoxs

            string empresas = "";
            foreach (var item in (List<SelectListItem>)Session["ListaEmpresas"])
            {
                if (model[item.Value].ToString().Contains("true"))
                {
                    empresas = empresas + item.Value;
                    AtualizaDDL(item.Value, (List<SelectListItem>)Session["ListaEmpresas"]);
                }
            }

            #endregion

            #region Empresas Tranportador - CheckBoxs

            string empresasTransportador = "";
            foreach (var item in (List<SelectListItem>)Session["ListaEmpresaTransportador"])
            {
                if (model[item.Value + "T"].ToString().Contains("true"))
                {
                    empresasTransportador = empresasTransportador + item.Value;
                    AtualizaDDL(item.Value, (List<SelectListItem>)Session["ListaEmpresaTransportador"]);
                }
            }

            #endregion

            #region Vendedor

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

            #endregion

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Programacao_Transportes_" + tipoRelatorio + "_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Programacao_Transportes_" + tipoRelatorio + "_"
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string nomeArquivo = "";
            if (tipoRelatorio == "Diario")
            {
                nomeArquivo = dataIni.ToString("yyyy-MM-dd");
                destino = GeraProgramacaoDiariaTransportesDiario(pesquisa, true, pasta, destino, dataIni, incubatorios,
                    empresas, empresasTransportador, vendedor);
            }
            else
            {
                nomeArquivo = dataIni.ToString("yyyy-MM-dd") + "_a_" + dataFim.ToString("yyyy-MM-dd");
                destino = GeraProgramacaoDiariaTransportesPorPeriodo(pesquisa, true, pasta, destino, dataIni, dataFim, incubatorios,
                    empresas, empresasTransportador, vendedor);
            }

            return File(destino, "Download", "Programacao_Transportes_" + tipoRelatorio + "_"
                + nomeArquivo + ".xlsx");
        }

        public string GeraProgramacaoDiariaTransportesPorPeriodo(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorios, string empresas, string empresasTranportador, string vendedor)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Programacao_Transportes_Periodo"
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

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select * ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "VU_Prog_Diaria_Transp_Pedidos_Periodo_Excel ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 = "";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 =
                "where ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 = 
                "[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                "CHARINDEX(Empresa, '" + empresas + "') > 0 and " +
                "(CHARINDEX(Nasc, '" + incubatorios + "') > 0 or Nasc is null) and " +
                "CHARINDEX(EmpresaTranportador, '" + empresasTranportador + "') > 0 and " +
                "(CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "order by 2, 5, 4";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Prog_Diaria_Transp_Pedidos_Periodo_Excel"))
                    item.OLEDBConnection.CommandText =
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

        public string GeraProgramacaoDiariaTransportesDiario(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, string incubatorios, string empresas, string empresasTranportador, string vendedor)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #region Excel

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Programacao_Transportes_Diario.xlsx", destino);

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

            // Parâmetros
            string dataStrSQLServer = dataInicial.ToString("yyyy-MM-dd");
            
            //Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Prog Diária Transp - Resumido"];

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;

                string commandTextCHICCabecalho = "";
                string commandTextCHICTabelas = "";
                string commandTextCHICCondicaoJoins = "";
                string commandTextCHICCondicaoFiltros = "";
                string commandTextCHICCondicaoParametros = "";
                string commandTextCHICAgrupamento = "";
                string commandTextCHICOrdenacao = "";

                if (item.Name.Equals("Pedidos1"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Prog_Diaria_Transp_Pedidos_Completo_Excel ";

                    commandTextCHICCondicaoJoins = "";

                    commandTextCHICCondicaoFiltros = "where ";

                    commandTextCHICCondicaoParametros =
                            "DataProgramacaoFiltro = '" + dataStrSQLServer + "' and " +
                            "CHARINDEX(Empresa, '" + empresas + "') > 0 and " +
                            "(CHARINDEX(Nasc, '" + incubatorios + "') > 0 or Nasc is null) and " +
                            "CHARINDEX(EmpresaTranportador, '" + empresasTranportador + "') > 0 and " +
                            "(CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "order by EmpresaTranportador, 2, 1";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("Veiculos1"))
                {
                    commandTextCHICCabecalho =
                        "exec ";

                    commandTextCHICTabelas =
                        "Rel_Prog_Diaria_Transp_Veiculos_Excel ";

                    commandTextCHICCondicaoJoins = "";

                    commandTextCHICCondicaoFiltros = "";

                    commandTextCHICCondicaoParametros =
                            "'" + incubatorios + "'," +
                            "'" + empresas + "'," +
                            "'" + empresasTranportador + "'," +
                            "'" + dataStrSQLServer   + "'," +
                            "'" + vendedor + "'";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
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

            #endregion

            return destino;
        }

        #endregion

        #region Funções Gerais

        private void RunMacro(object oApp, object[] oRunArgs)
        {
            oApp.GetType().InvokeMember("Run",
                System.Reflection.BindingFlags.Default |
                System.Reflection.BindingFlags.InvokeMethod,
                null, oApp, oRunArgs);
        }

        #endregion

        #region Relatórios AniPlan

        public string GeraRelatorioListaPedidosAniPlan(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string empresa, string vendedor,
            string linhagem, string uf, string cliente)
        {
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

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais\\Lista_Pedidos_AniPlan.xlsx", destino);

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

            #region SQL AniPlan

            string commandTextCHICCabecalho =
                "select " +
                    "* ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Verificacao_Final ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "Nascimento between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(Linhagem = '" + linhagem + "' or '" + linhagem + "' = '(Todas)') and " +
                    "(UF = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "(CodigoCliente = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "(CodigoRepresentante = '" + vendedor.Substring(1, 6) + "' or CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "3, 5, 1";

            #endregion

            #region SQL Cancelados

            string commandTextCHICCabecalhoCancelados =
                "select " +
                    "* ";

            string commandTextCHICTabelasCancelados =
                "from " +
                    "VU_Lista_Pedidos_Cancelados_WEB ";

            string commandTextCHICCondicaoJoinsCancelados =
                "where ";

            string commandTextCHICCondicaoFiltrosCancelados = "";

            string dataInicialStrSql = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrSql = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosCancelados =
                    "[data entrega inicial] >= '" + dataInicialStrSql + "' and  " +
                    "[data entrega final] <= '" + dataFinalStrSql + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "([uf] = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "([codigo cliente] = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "([codigo vendedor] = '" + vendedor + "' or [codigo vendedor] = '" + vendedor.Substring(1, 6) + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoCancelados =
                "order by " +
                    "2, " +
                    "ID";

            #endregion

            #region SQL Pedidos WEB

            string commandTextCHICCabecalhoPedidosWEB =
                "select " +
                    "* ";

            string commandTextCHICTabelasPedidosWEB =
                "from " +
                    "VU_Lista_Pedidos_WEB ";

            string commandTextCHICCondicaoJoinsPedidosWEB =
                "where ";

            string commandTextCHICCondicaoFiltrosPedidosWEB = "";

            string commandTextCHICCondicaoParametrosPedidosWEB =
                    "[data nascimento] >= '" + dataInicialStrSql + "' and  " +
                    "[data nascimento] <= '" + dataFinalStrSql + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "([uf] = '" + uf + "' or '" + uf + "' = '(Todos)') and " +
                    "([codigo cliente] = '" + cliente + "' or '" + cliente + "' = '(Todos)') and " +
                    "([codigo vendedor] = '" + vendedor + "' or [codigo vendedor] = '" + vendedor.Substring(1, 6) + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoPedidosWEB =
                "order by " +
                    "2, " +
                    "ID";

            #endregion

            string dataExibicao = dataInicial.ToString("dd/MM/yyyy") + " à " +
                dataFinal.ToString("dd/MM/yyyy");
            string vendedorExibicao = "";
            string nomeVendedor = "";
            if (vendedor != "(Todos)")
            {
                var vendedorApolo = "0" + vendedor;
                var vendedorObj = apolo.VENDEDOR.Where(w => w.VendCod == vendedorApolo).FirstOrDefault();

                if (vendedorObj != null)
                {
                    nomeVendedor = vendedorObj.VendNome;
                }
            }
            if (nomeVendedor == "")
                vendedorExibicao = vendedor;
            else
                vendedorExibicao = nomeVendedor;

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Pedidos - AniPlan"];
            worksheet.Cells[2, 7] = dataExibicao;
            worksheet.Cells[3, 7] = vendedorExibicao;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("Faturamento"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros +
                        commandTextCHICAgrupamento + commandTextCHICOrdenacao;
                else if (item.Name.Equals("Cancelamentos_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoCancelados + commandTextCHICTabelasCancelados +
                        commandTextCHICCondicaoJoinsCancelados + commandTextCHICCondicaoFiltrosCancelados +
                        commandTextCHICCondicaoParametrosCancelados + commandTextCHICAgrupamentoCancelados;
                else if (item.Name.Equals("Pedidos_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoPedidosWEB + commandTextCHICTabelasPedidosWEB +
                        commandTextCHICCondicaoJoinsPedidosWEB + commandTextCHICCondicaoFiltrosPedidosWEB +
                        commandTextCHICCondicaoParametrosPedidosWEB + commandTextCHICAgrupamentoPedidosWEB;
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

        public string GeraPlanejamentoIncubacaoAniPlan(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorios, string tipoProduto)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais\\Planejamento_Incubacao_AniPlan.xlsx", destino);

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

            string dataInicialStrCalDate = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrCalDate = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select " +
                    "* ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "VU_Planejamento_Incubacao_AniPlan V ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 =
                "inner join Apolo10.dbo.TAB_SEQUENCIA T With(Nolock) on T.TabSeqCod <= 10 ";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 = 
                "where ";

            string filtroTipoProduto = "";
            if (tipoProduto != "")
            {
                string tipoProdutoAniPlan = "Pinto";
                if (tipoProduto == "H") tipoProdutoAniPlan = "Ovo";
                    filtroTipoProduto = " and Tipo = '" + tipoProdutoAniPlan + "' ";
            }

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "[Data de Nascimento] between '" + dataInicialStrCalDate + "' and '" + dataFinalStrCalDate + "' and " +
                    "CHARINDEX([Inc.], '" + incubatorios + "') > 0 " +
                    filtroTipoProduto;

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("Pedidos_AniPlan"))
                    item.OLEDBConnection.CommandText =
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

        public string GeraPlanejamentoVacinacaoAniPlan(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorios)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais\\Planejamento_Vacinacao_AniPlan.xlsx", destino);

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

            string dataInicialStrCalDate = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrCalDate = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select * ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "VU_Planejamento_Incubacao_AniPlan V ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 = "";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 =
                "where ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "[Data de Nascimento] between '" + dataInicialStrCalDate + "' and '" + dataFinalStrCalDate + "' and " +
                    "CHARINDEX([Inc.], '" + incubatorios + "') > 0 ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("Pedidos_AniPlan"))
                    item.OLEDBConnection.CommandText =
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

        public string GeraListaVacinasServicosAniPlan(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, string empresa, DateTime dataInicial, DateTime dataFinal, string incubatorios, string laboratorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\RelatoriosComerciais\\ListaVacinasServicos_AniPlan.xlsx", destino);

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

            #region ANIPLAN_Vacinas_Servicos

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCabecalhoFaturamentoDados02 =
                "select " +
                    "pais [país], " +
                    "Empresa [empresa], " +
                    "DataIncubacao [data incubação], " +
                    "DataNascimento [data nascimento], " +
                    "IDPedidoVenda [nº pedido], " +
                    "NFNumVenda [nº nf (venda)], " +
                    "NFNumReposicao [nº nf (reposição)], " +
                    "CodigoCliente [cód. cliente], " +
                    "NomeCliente [nome cliente], " +
                    "cidade [cidade], " +
                    "uf [uf], " +
                    "LocalNascimento [inc.], " +
                    "Codigo [cód. apolo], " +
                    "ProdNomeAlt1 [descricao produto], " +
                    "TipoCobrancaVacina [tipo cobrança vacina], " +
                    "CondicaoPagamento [cond. pag.], " +
                    "ValorUnit [valor unitário], " +
                    "QtdeTotal [qtde. aplicada (01 dose para cada 01 pintainha)], " +
                    "QtdeDoses [qtde. doses por ampola], " +
                    "QtdeDosesPreparadas [qtde. doses preparada para o pedido], " +
                    "ValorTotal [valor total], " +
                    "QtdeClienteEnvia [qtde. aplicada (01 dose para cada 01 pintainha) - Cliente Envia] ";

            string commandTextCHICTabelasFaturamentoDados02 =
                "from " +
                    "VU_Lista_Vacinas_Pedidos_AniPlan ";

            string commandTextCHICCondicaoJoinsFaturamentoDados02 = "";

            string commandTextCHICCondicaoFiltrosFaturamentoDados02 =
                "where ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                    "DataProgramacao between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "CHARINDEX(LocalNascimento, '" + incubatorios + "') > 0 and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(MarcaProdNome like '%"+ laboratorio + "%' or '" + laboratorio + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 = "order by DataProgramacao, IDPedidoVenda";

            #endregion

            #region ANIPLAN_Resumo_Vacinas_Servicos

            string commandTextCHICCabecalhoFaturamentoDadosRes =
                "select " +
                    "P.pais [país], " +
                    "P.Empresa [empresa], " +
                    "A.Codigo [cód. apolo], " +
                    "PR.ProdNomeAlt1 [descricao produto], " +
                    "A.Tipo [tipo], " +
                    "SUM((P.QtdeVendida + P.QtdeBonificada + P.QtdeReposicao) * IIF(ISNULL(A.PercAplicacao,1)=0,1,(ISNULL(A.PercAplicacao,1) / 100.0))) [qtde. total], " +
                    "SUM(IIF(A.Bonificada = 2, (P.QtdeVendida + P.QtdeBonificada + P.QtdeReposicao) * IIF(ISNULL(A.PercAplicacao,1)=0,1,(ISNULL(A.PercAplicacao,1) / 100.0)), 0)) [qtde. cliente envia], " +
                    "SUM(IIF(ISNULL(A.Bonificada,0) = 0, (P.QtdeVendida + P.QtdeBonificada + P.QtdeReposicao) * IIF(ISNULL(A.PercAplicacao,1)=0,1,(ISNULL(A.PercAplicacao,1) / 100.0)), 0)) [qtde. normal], " +
                    "SUM(IIF(A.Bonificada = 1, (P.QtdeVendida + P.QtdeBonificada + P.QtdeReposicao) * IIF(ISNULL(A.PercAplicacao,1)=0,1,(ISNULL(A.PercAplicacao,1) / 100.0)), 0)) [qtde. bonificada], " +
                    "SUM(IIF(ISNULL(A.Bonificada,0) = 0 and A.PrecoUnitario > 0, (P.QtdeVendida + P.QtdeBonificada + P.QtdeReposicao) * IIF(ISNULL(A.PercAplicacao,1)=0,1,(ISNULL(A.PercAplicacao,1) / 100.0)), 0)) [qtde. cobrada], " +
                    "SUM(IIF(ISNULL(A.Bonificada,0) = 0 and A.PrecoUnitario > 0, A.PrecoUnitario, 0)) / IIF(SUM(IIF(ISNULL(A.Bonificada,0) = 0 and A.PrecoUnitario > 0, 1, 0)) = 0, 1, SUM(IIF(ISNULL(A.Bonificada,0) = 0 and A.PrecoUnitario > 0, 1, 0))) [valor unit.], " +
                    "SUM(IIF(ISNULL(A.Bonificada,0) = 0 and A.PrecoUnitario > 0, ((P.QtdeVendida + P.QtdeBonificada + P.QtdeReposicao) * IIF(ISNULL(A.PercAplicacao,1)=0,1,(ISNULL(A.PercAplicacao,1) / 100.0))) * A.PrecoUnitario, 0)) [valor cobrado] ";

            string commandTextCHICTabelasFaturamentoDadosRes =
                "from " +
                    "VU_Pedidos_Vendas_CHIC_Matrizes P ";

            string commandTextCHICCondicaoJoinsFaturamentoDadosRes =
                "inner join Prog_Diaria_Transp_Ped_It_Adic A With(Nolock) on P.IDPedidoVenda = A.IDPedidoVenda and A.Tipo in ('Vacina','Serviço') " +
                "inner join Apolo10.dbo.PRODUTO PR With(Nolock) on A.Codigo = PR.ProdCodEstr " +
                "inner join Apolo10.dbo.MARCA_PROD MP With(Nolock) on PR.MarcaProdCod = MP.MarcaProdCod ";

            string commandTextCHICCondicaoFiltrosFaturamentoDadosRes =
                "where ";

            string commandTextCHICCondicaoParametrosFaturamentoDadosRes =
                    "P.DataProgramacao between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "CHARINDEX(P.LocalNascimento, '" + incubatorios + "') > 0 and " +
                    "(P.Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(MP.MarcaProdNome like '%" + laboratorio + "%' or '" + laboratorio + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDadosRes =
                "group by " +
                    "P.pais, " +
                    "P.Empresa, " +
                    "A.Codigo, " +
                    "PR.ProdNomeAlt1, " +
                    "A.Tipo ";

            string commandTextCHICOrdenacaoFaturamentoDadosRes = "order by P.Empresa, A.Tipo, PR.ProdNomeAlt1";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("ANIPLAN_Vacinas_Servicos"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamentoDados02 + commandTextCHICTabelasFaturamentoDados02 +
                        commandTextCHICCondicaoJoinsFaturamentoDados02 +
                        commandTextCHICCondicaoFiltrosFaturamentoDados02 + commandTextCHICCondicaoParametrosFaturamentoDados02 +
                        commandTextCHICAgrupamentoFaturamentoDados02 +
                        commandTextCHICOrdenacaoFaturamentoDados02;
                if (item.Name.Equals("ANIPLAN_Resumo_Vacinas_Servicos"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamentoDadosRes + commandTextCHICTabelasFaturamentoDadosRes +
                        commandTextCHICCondicaoJoinsFaturamentoDadosRes +
                        commandTextCHICCondicaoFiltrosFaturamentoDadosRes + commandTextCHICCondicaoParametrosFaturamentoDadosRes +
                        commandTextCHICAgrupamentoFaturamentoDadosRes +
                        commandTextCHICOrdenacaoFaturamentoDadosRes;
            }

            oBook.RefreshAll();

            #region Filtrar Tabela Dinâmica

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["VACINAS POR CIDADE X UF"];
            Excel.PivotTable pvt = worksheet.PivotTables("Tabela dinâmica1") as Excel.PivotTable;
            List<string> ListToFilter = new List<string>();
            ListToFilter.Add("TRATAMENTO INFRAVERMELHO");
            ListToFilter.Add("USO DE HIDRATANTE");
            ListToFilter.Add("NEOFLORA (SIMBIOTICO) - BIOSYN");
            ListToFilter.Add("POULTRY STAR (SIMBIOTICO) - BIOMIN");

            Excel.PivotFields _PivotFields = (Excel.PivotFields)pvt.get_ColumnFields(Missing.Value);

            foreach (Excel.PivotField _PivotField in _PivotFields)
            {
                if (string.Compare(_PivotField.Caption, "descricao produto", true) == 0)
                {
                    Excel.PivotItems _PivotItems = (Excel.PivotItems)_PivotField.PivotItems(Missing.Value);
                    foreach (Excel.PivotItem _PivotItem in _PivotItems)
                    {
                        if (ListToFilter.Contains(_PivotItem.Caption))
                            _PivotItem.Visible = false;
                        else
                            _PivotItem.Visible = true;
                    }
                }
            }

            #endregion

            #region Esconder outras abas, Bloquear Planilha e Pasta de Trabalho

            Excel._Worksheet worksheetResumo = (Excel._Worksheet)oBook.Worksheets["RESUMO DE VACINAS E SERVIÇOS"];
            Excel._Worksheet worksheetLista = (Excel._Worksheet)oBook.Worksheets["RELATÓRIO DE VACINAS E SERVIÇOS"];
            worksheetResumo.Visible = Excel.XlSheetVisibility.xlSheetHidden;
            worksheetLista.Visible = Excel.XlSheetVisibility.xlSheetHidden;
            worksheet.Protect(dataFinal.ToString("ddMMyyyy#"), true, true, false, false, false, false, false, false, false, false, false, false, false, true, false);
            oBook.Protect(dataFinal.ToString("ddMMyyyy#"));

            #endregion

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
