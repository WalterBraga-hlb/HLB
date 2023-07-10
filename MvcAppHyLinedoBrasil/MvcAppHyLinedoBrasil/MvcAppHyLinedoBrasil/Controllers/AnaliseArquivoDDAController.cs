using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHyLinedoBrasil.Models;
using MvcAppHyLinedoBrasil.Models.HLBAPP;
using MvcAppHyLinedoBrasil.Models.Apolo;
using System.IO;
using System.Collections;
using System.Data.Objects;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class AnaliseArquivoDDAController : Controller
    {
        LayoutDb bd = new LayoutDb();
        FinanceiroEntities bdFinanceiro = new FinanceiroEntities();
        HLBAPPEntities1 hlbapp = new HLBAPPEntities1();
        MvcAppHyLinedoBrasil.Models.Apolo.ApoloEntities apolo =
                    new MvcAppHyLinedoBrasil.Models.Apolo.ApoloEntities();
        //System.Linq.IOrderedQueryable<LayoutDDASegmentoG> lista;
        //List<LayoutDDASegmentoG> lista = new List<LayoutDDASegmentoG>();

        //
        // GET: /AnaliseArquivoDDA/

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

            string userSession = Session["login"].ToString() + Session.SessionID;

            bd.Database.ExecuteSqlCommand("delete from LayoutDDASegmentoGs where UserSession = '" + userSession + "'");
            bd.SaveChanges();
            return View(bd.LinhasSegmentoG.Where(w => w.UserSession == userSession).ToList());
        }

        [HttpPost]
        public ActionResult AnaliseArquivo()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            try
            {
                bd.Database.ExecuteSqlCommand("delete from LayoutDDASegmentoGs where UserSession like '" +
                    Session["login"].ToString() + "%'");
                bd.SaveChanges();
                List<string> textLinha = new List<string>();
                ViewBag.fileName = Request.Files[0].FileName;
                Session["fileName"] = Request.Files[0].FileName;

                //if (bd.ArquivosLidosDDA.Count(a => a.Arquivo == Request.Files[0].FileName) == 0)
                //{

                //}

                StreamReader arquivo = new StreamReader(Request.Files[0].InputStream);
                string text;
                string cnpj = "";
                string empresa = "";
                string linhaAnterior = "";

                LayoutDDASegmentoG layout = new LayoutDDASegmentoG();

                while ((text = arquivo.ReadLine()) != null)
                {
                    if (text.Substring(13, 1) == "0")
                    {
                        cnpj = text.Substring(19, 14);
                    }

                    if (text.Substring(13, 1) == "G")
                    {
                        linhaAnterior = text.Substring(13, 1);

                        empresa = text.Substring(190, 14);

                        MvcAppHyLinedoBrasil.Models.EMPRESA_FILIAL ef = bdFinanceiro.EMPRESA_FILIAL
                            .Where(w => w.EmpCpfCgc == empresa && w.EmpCod != "2").FirstOrDefault();
                        if (ef != null)
                            empresa = ef.EmpCod;
                        else
                            empresa = empresa.Substring(0, 2) + "." +
                                    empresa.Substring(2, 3) + "." + empresa.Substring(5, 3) + "/" +
                                    empresa.Substring(8, 4) + "-" + empresa.Substring(12, 2);

                        layout = new LayoutDDASegmentoG();

                        // Dados de Controle
                        //layout.Empresa = empresa.Substring(0, 2) + "." + 
                        //    empresa.Substring(2, 3) + "." + empresa.Substring(5, 3) + "/" + 
                        //    empresa.Substring(8, 4) + "-" + empresa.Substring(12, 2);
                        layout.Empresa = empresa;
                        layout.BancoCompensacao = text.Substring(0, 3);
                        layout.Lote = Convert.ToInt32(text.Substring(3, 4));
                        layout.Registro = text.Substring(7, 1);
                        layout.NumeroRegistro = Convert.ToInt32(text.Substring(8, 5));
                        layout.Segmento = text.Substring(13, 1);
                        layout.CNAB = text.Substring(14, 1);
                        layout.Movimento = text.Substring(15, 2);

                        // Dados de Título
                        layout.BancoCedente = text.Substring(17, 3);
                        layout.CodigoMoeda = Convert.ToInt32(text.Substring(20, 1));
                        layout.DigitoVerificadorCodigoBarras = Convert.ToInt32(text.Substring(21, 1));
                        layout.ValorImpressoCodigoBarras = text.Substring(22, 14);
                        layout.CampoLivre = text.Substring(36, 25);
                        layout.TipoInscricao = Convert.ToInt32(text.Substring(61, 1));
                        layout.Inscricao = text.Substring(62, 15);
                        layout.NomeCedente = text.Substring(77, 30);
                        layout.DataVencimento = text.Substring(107, 8) != "00000000" ? 
                            Convert.ToDateTime(text.Substring(107, 2) + "/" + text.Substring(109, 2) + "/" + 
                            text.Substring(111, 4)) : Convert.ToDateTime("01/01/1889");
                        layout.ValorTitulo = Convert.ToDecimal(text.Substring(115, 13) + "," + 
                            text.Substring(128, 2));
                        layout.QuantidadeMoeda = Convert.ToDecimal(text.Substring(130, 10) + "," + 
                            text.Substring(140, 5));
                        layout.NumeroDocumento = text.Substring(147, 15);
                        layout.Filler = text.Substring(162, 1);
                        layout.ValorAbatimento = Convert.ToDecimal(text.Substring(163, 13) + "," + 
                            text.Substring(176, 2));
                        layout.CodigoCarteira = text.Substring(178, 1);
                        layout.EspecieTitulo = Convert.ToInt32(text.Substring(179, 2));
                        layout.DataEmissaoTitulo = text.Substring(181, 8) != "00000000" ? 
                            Convert.ToDateTime(text.Substring(181, 2) + "/" + text.Substring(183, 2) + "/" + 
                            text.Substring(185, 4)) : Convert.ToDateTime("01/01/1889");
                        layout.JurosMora = Convert.ToDecimal(text.Substring(189, 13) + "," + 
                            text.Substring(202, 2));
                        layout.CodigoDesconto1 = Convert.ToInt32(text.Substring(204, 1));
                        layout.DataDesconto1 = text.Substring(205, 8) != "00000000" ? 
                            Convert.ToDateTime(text.Substring(205, 2) + "/" + text.Substring(207, 2) + "/" + 
                            text.Substring(209, 4)) : Convert.ToDateTime("01/01/1889");
                        layout.Desconto1 = Convert.ToDecimal(text.Substring(213, 13) + "," + text.Substring(226, 2));
                        layout.CodigoProtesto = Convert.ToInt32(text.Substring(228, 1));
                        layout.PrazoProtesto = Convert.ToInt32(text.Substring(229, 2));
                        layout.DataLimite = text.Substring(231, 8) != "00000000" ? 
                            Convert.ToDateTime(text.Substring(231, 2) + "/" + text.Substring(233, 2) + "/" + 
                            text.Substring(235, 4)) : Convert.ToDateTime("01/01/1889");
                        layout.UserSession = Session["login"].ToString() + Session.SessionID;
                        layout.EnviaEmailFiscal = "";
                    }

                    if (linhaAnterior.Equals("G") && text.Substring(13, 1) == "H")
                    {
                        layout.Sacador = text.Substring(33, 30);
                        bd.LinhasSegmentoG.Add(layout);
                    }
                    else if (text.Substring(13, 1) == "G")
                    {
                        layout.Sacador = "";
                        bd.LinhasSegmentoG.Add(layout);
                    }

                    linhaAnterior = text.Substring(13, 1);
                }

                bd.SaveChanges();
                VerificaParcelaApolo();
                VerificaEnvioEmailFiscal();

                ViewBag.CNPJ = cnpj.Substring(0, 2) + "." + cnpj.Substring(2, 3) + "." + cnpj.Substring(5, 3) + "/" + cnpj.Substring(8, 4) + "-" + cnpj.Substring(12,2);

                bd.ArquivosLidosDDA.Add(new ArquivosLidos
                {
                    Arquivo = Request.Files[0].FileName,
                    DataLeitura = DateTime.Now,
                });
                bd.SaveChanges();

                string userSession = Session["login"].ToString() + Session.SessionID;

                var lista = bd.LinhasSegmentoG
                    .Where(w => w.UserSession == userSession)
                    .OrderBy(l => l.NomeCedente)
                    .ThenBy(l => l.NumeroDocumento)
                    .ThenBy(l => l.DataVencimento);

                //return PartialView("_ListaDDA", lista);
                //return View("_ListaDDA", lista);
                return View("Index", lista);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        public int CalculaDigitoVerificadorLinhaDigitavel(string campo)
        {
            // ** Modo Antigo **
            //int mult = 2;
            //int soma = 0;
            //int count = campo.Length - 1;
            //int x = 0;

            //while (count > 0)
            //{
            //    x = Convert.ToInt32(campo.Substring(count, 1)) * mult;

            //    if (x > 10) { x = (x % 10) + 1; }

            //    soma = soma + x;

            //    if (mult == 2) { mult = 1; } else { mult = 2; }

            //    count--;
            //}

            //return soma = 10 - (soma % 10);

            int dVCampo = 0;
            int seq = campo.Length;
            int coeficiente = 2;
            int numero = 0;
            int num = 0;

            while (seq != 0)
            {
                numero = Convert.ToInt32(campo.Substring(seq - 1, 1)) * coeficiente;

                if (numero.ToString().Length == 2)
                {
                    dVCampo = dVCampo + Convert.ToInt32(numero.ToString().Substring(0, 1)) + Convert.ToInt32(numero.ToString().Substring(1, 1));
                }
                else
                {
                    dVCampo = dVCampo + numero;
                }

                if (coeficiente == 2)
                {
                    coeficiente = 1;
                }
                else
                {
                    coeficiente = 2;
                }

                seq = seq - 1;
            }

            if ((dVCampo > 0) && (dVCampo <= 10)) num = 10;
            if ((dVCampo > 10) && (dVCampo <= 20)) num = 20;
            if ((dVCampo > 20) && (dVCampo <= 30)) num = 30;
            if ((dVCampo > 30) && (dVCampo <= 40)) num = 40;
            if ((dVCampo > 40) && (dVCampo <= 50)) num = 50;
            if ((dVCampo > 50) && (dVCampo <= 60)) num = 60;
            if ((dVCampo > 60) && (dVCampo <= 70)) num = 70;
            if ((dVCampo > 70) && (dVCampo <= 80)) num = 80;

            dVCampo = num - dVCampo;

            return dVCampo;
        }

        public string MontaLinhaDigitavel(LayoutDDASegmentoG linha)
        {
            /*
             * Linha Digitável
             * 
             * BBBMC.CCCCd CCCCC.CCCCCd CCCCC.CCCCCd D VVVVVVVVVVVVVV
             * 
             * onde:
             * 
             * B - número do banco
             * M - Moeda (sempre 9 - real)
             * V - valor
             * C - campo livre - depende do banco
             * D - Digito verificador do código de barras
             * d - digito verificados da linha digitáveis
            */

            return linha.BancoCedente + linha.CodigoMoeda + linha.CampoLivre.Substring(0, 1) + linha.CampoLivre.Substring(1, 4) + CalculaDigitoVerificadorLinhaDigitavel(linha.BancoCedente + linha.CodigoMoeda + linha.CampoLivre.Substring(0, 1) + linha.CampoLivre.Substring(1, 4)) + // Campo 1
                    linha.CampoLivre.Substring(5, 10) + CalculaDigitoVerificadorLinhaDigitavel(linha.CampoLivre.Substring(5, 10)) + // Campo 2
                    linha.CampoLivre.Substring(15, 10) + CalculaDigitoVerificadorLinhaDigitavel(linha.CampoLivre.Substring(15, 10)) + // Campo 3
                    linha.DigitoVerificadorCodigoBarras + // Campo 4
                    linha.ValorImpressoCodigoBarras; // Campo 5
        }

        public string MontaCodigoBarras(LayoutDDASegmentoG linha)
        {
            /* 
             * Código de Barras
             * 
             * Posição	Conteúdo
             * 1 a 3	Número do banco
             * 4	    Código da Moeda - 9 para Real
             * 5	    Digito verificador do Código de Barras
             * 6 a 9	Fator de Vencimento (diferença em dias entre o vencimento e 07/10/1997)
             * 10 a 19	Valor (8 inteiros e 2 decimais)
             * 20 a 44	Campo Livre definido por cada banco 
            */

            return linha.BancoCedente + linha.CodigoMoeda + linha.DigitoVerificadorCodigoBarras + linha.ValorImpressoCodigoBarras + linha.CampoLivre;
        }

        public void VerificaParcelaApolo()
        {
            string empresaParcela = "";
            int chave = 0;

            try
            {
                string userSession = Session["login"].ToString() + Session.SessionID;

                var lista = bd.LinhasSegmentoG
                    .Where(w => w.UserSession == userSession)
                    .ToList();

                foreach (var item in lista)
                {
                    if (item.NomeCedente == "MATER MARAVALHA INDUSTRIA E CO")
                    {
                        empresaParcela = "";
                    }

                    item.ImportaNoApolo = "Sim";
                    item.TituloNoApolo = "Não";

                    string teste = "";
                    if (item.NumeroDocumento.Trim().Equals("1001957105"))
                        teste = item.NumeroDocumento.Trim();

                    string LinhaDigitavel = MontaLinhaDigitavel(item);
                    string CodigoBarras = MontaCodigoBarras(item);
                    //string CodigoBarras = "34197546800005096001128233336980332062686000";
                    //string LinhaDigitavel = "00190438353045351912300005348115154530000032164";

                    //string cnpjEmpresa = item.Empresa.Replace(".","").Replace("/","").Replace("-","");

                    MvcAppHyLinedoBrasil.Models.EMPRESA_FILIAL ef = bdFinanceiro.EMPRESA_FILIAL
                        .Where(w => w.EmpCod == item.Empresa.Trim()).FirstOrDefault();

                    if (ef != null)
                    {
                        var listaParcelas = bdFinanceiro.PARC_DOC_FIN
                            .Where(p => p.EmpCod == ef.EmpCod && p.ParcDocFinDataVenc == item.DataVencimento
                                && p.ParcDocFinValOrig == item.ValorTitulo
                                && p.ParcDocFinProjecao == "Não")
                            .ToList();

                        if (listaParcelas.Count > 0)
                        {
                            PARC_DOC_FIN parcela = listaParcelas.FirstOrDefault();
                            if (listaParcelas.Count > 1)
                            {
                                parcela = listaParcelas
                                    .Where(w => 
                                        (w.ParcDocFinDupNum.Contains(item.NumeroDocumento.Trim())
                                            || w.ParcDocFinEntNome.Contains(item.NomeCedente.Substring(0, 4))))
                                    .FirstOrDefault();
                            }

                            if (parcela == null)
                            {
                                item.ImportaNoApolo = "Nao";
                                item.TituloNoApolo = "Duplicar";
                            }
                            else
                            {
                                if (parcela.EmpCod.Equals("4") && parcela.DocFinChv.Equals(18123))
                                {
                                    empresaParcela = parcela.EmpCod;
                                    chave = parcela.DocFinChv;
                                }

                                //ENTIDADE entidade = bdFinanceiro.ENTIDADE
                                //    .Where(e => e.EntNome.Contains(item.NomeCedente.Substring(0, 4)))
                                //    .FirstOrDefault();

                                var listaParcelaComEntidade = bdFinanceiro.PARC_DOC_FIN
                                    .Where(p => p.EmpCod == ef.EmpCod
                                        && p.ParcDocFinEntNome.Contains(item.NomeCedente.Substring(0, 4))
                                        && p.ParcDocFinDataVenc == item.DataVencimento
                                        && p.ParcDocFinValOrig == item.ValorTitulo
                                        && p.ParcDocFinProjecao == "Não")
                                    .ToList();

                                if (listaParcelaComEntidade.Count > 0)
                                {
                                    PARC_DOC_FIN parcelaComEntidade = listaParcelaComEntidade.FirstOrDefault();
                                    if (listaParcelaComEntidade.Count > 1)
                                    {
                                        parcelaComEntidade = listaParcelaComEntidade
                                            .Where(w => w.ParcDocFinDupNum.Contains(item.NumeroDocumento.Trim()))
                                            .FirstOrDefault();
                                    }

                                    if (parcelaComEntidade == null)
                                    {
                                        item.ImportaNoApolo = "Nao";
                                        item.TituloNoApolo = "Duplicar";
                                    }
                                    else
                                    {
                                        if (parcelaComEntidade.ParcDocFinDataPag == null)
                                        {
                                            item.TituloNoApolo = "Sim";

                                            if ((parcelaComEntidade.ParcDocFinCodLeit != null)
                                                && (!parcelaComEntidade.ParcDocFinCodLeit.Equals("")))
                                            {
                                                item.ImportaNoApolo = "Nao";
                                            }
                                        }
                                        else
                                        {
                                            item.ImportaNoApolo = "Nao";
                                            item.TituloNoApolo = "Cifrao";
                                        }

                                        item.EmpresaApolo = parcelaComEntidade.EmpCod;
                                        item.ChaveDocApolo = parcelaComEntidade.DocFinChv;
                                        item.SeqDocApolo = parcelaComEntidade.ParcDocFinSeq;
                                        item.SeqDesmPagDocApolo = parcelaComEntidade.ParcDocFinDesmPag;
                                        item.DupNumDocApolo = parcelaComEntidade.ParcDocFinDupNum;
                                    }
                                }
                                else
                                {
                                    if (parcela.ParcDocFinDataPag == null)
                                    {
                                        if ((parcela.ParcDocFinCodLeit != null)
                                            && (!parcela.ParcDocFinCodLeit.Equals("")))
                                        {
                                            item.TituloNoApolo = "Sim";
                                            item.ImportaNoApolo = "Nao";
                                        }
                                        else
                                        {
                                            item.TituloNoApolo = "Aviso";
                                        }
                                    }
                                    else
                                    {
                                        item.ImportaNoApolo = "Nao";
                                        item.TituloNoApolo = "Cifrao";
                                    }

                                    item.EmpresaApolo = parcela.EmpCod;
                                    item.ChaveDocApolo = parcela.DocFinChv;
                                    item.SeqDocApolo = parcela.ParcDocFinSeq;
                                    item.SeqDesmPagDocApolo = parcela.ParcDocFinDesmPag;
                                    item.DupNumDocApolo = parcela.ParcDocFinDupNum;
                                }
                            }
                        }
                        else
                        {
                            listaParcelas = bdFinanceiro.PARC_DOC_FIN
                                .Where(p => p.ParcDocFinEntNome.Contains(item.NomeCedente.Substring(0, 4))
                                        && p.ParcDocFinDataVenc == item.DataVencimento
                                        && p.ParcDocFinValOrig == item.ValorTitulo
                                        && p.ParcDocFinProjecao == "Não")
                                .ToList();

                            if (listaParcelas.Count > 0)
                            {
                                PARC_DOC_FIN parcela = listaParcelas.FirstOrDefault();

                                if (listaParcelas.Count > 1)
                                {
                                    parcela = listaParcelas
                                        .Where(w => w.ParcDocFinDupNum.Contains(item.NumeroDocumento))
                                        .FirstOrDefault();
                                }

                                if (parcela == null)
                                {
                                    item.ImportaNoApolo = "Nao";
                                    item.TituloNoApolo = "Duplicar";
                                }
                                else
                                {
                                    if (parcela.ParcDocFinDataPag == null)
                                    {
                                        if ((parcela.ParcDocFinCodLeit != null)
                                            && (!parcela.ParcDocFinCodLeit.Equals("")))
                                        {
                                            item.TituloNoApolo = "Sim";
                                            item.ImportaNoApolo = "Nao";
                                        }
                                        else
                                        {
                                            item.TituloNoApolo = "Aviso";
                                        }
                                    }
                                    else
                                    {
                                        item.ImportaNoApolo = "Nao";
                                        item.TituloNoApolo = "Cifrao";
                                    }

                                    item.EmpresaApolo = parcela.EmpCod;
                                    item.ChaveDocApolo = parcela.DocFinChv;
                                    item.SeqDocApolo = parcela.ParcDocFinSeq;
                                    item.SeqDesmPagDocApolo = parcela.ParcDocFinDesmPag;
                                    item.DupNumDocApolo = parcela.ParcDocFinDupNum;
                                }
                            }
                            else
                            {
                                item.TituloNoApolo = "Nao";
                                item.ImportaNoApolo = "Nao";
                            }
                        }
                    }
                    else
                    {
                        item.TituloNoApolo = "Nao";
                        item.ImportaNoApolo = "Nao";
                    }

                    if (item.DataVencimento < DateTime.Today)
                    {
                        item.ImportaNoApolo = "Nao";
                        item.EnviaEmailFiscal = "";
                    }

                    item.LinhaDigitavel = LinhaDigitavel;
                }
                bd.SaveChanges();
            }
            catch (Exception e)
            {
                
            }
        }

        public void VerificaEnvioEmailFiscal()
        {
            string userSession = Session["login"].ToString() + Session.SessionID;

            var lista = bd.LinhasSegmentoG
                .Where(w => w.UserSession == userSession && w.TituloNoApolo == "Nao")
                .ToList();

            foreach (var item in lista)
            {
                string teste = "";
                if (item.NumeroDocumento.Trim().Equals("CONTR.ABR"))
                    teste = item.NumeroDocumento.Trim();

                LayoutDDASegmentoGs_EnvioEmailFiscal fiscalEmail =
                    hlbapp.LayoutDDASegmentoGs_EnvioEmailFiscal
                    .Where(w => w.DataVencimento == item.DataVencimento
                        && w.ValorTitulo == item.ValorTitulo
                        && w.Empresa == item.Empresa
                        && w.NomeCedente == item.NomeCedente)
                    .FirstOrDefault();

                if (fiscalEmail == null)
                {
                    if (item.DataVencimento < DateTime.Today)
                        item.EnviaEmailFiscal = "";
                    else
                    {
                        TimeSpan dias = item.DataVencimento.Subtract(DateTime.Today);
                        if (dias.Days <= 7)
                            item.EnviaEmailFiscal = "Sim";
                        else
                            item.EnviaEmailFiscal = "";
                    }
                }
                else
                {
                    if (item.DataVencimento < DateTime.Today)
                        item.EnviaEmailFiscal = "";
                    else
                        item.EnviaEmailFiscal = "Enviado";
                }
            }

            bd.SaveChanges();
        }

        [HttpPost]
        public ActionResult GerarAcoesSelecionados(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

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
                            foreach (LayoutDDASegmentoG item in bd.LinhasSegmentoG.ToList())
                            {
                                if (fileId == item.ID)
                                {
                                    //string cnpjEmpresa = item.Empresa.Replace(".", "").Replace("/", "").Replace("-", "");

                                    MvcAppHyLinedoBrasil.Models.EMPRESA_FILIAL ef = bdFinanceiro.EMPRESA_FILIAL
                                        .Where(w => w.EmpCod == item.Empresa).FirstOrDefault();

                                    if (ef != null)
                                    {
                                        //PARC_DOC_FIN parcela = new PARC_DOC_FIN();

                                        //if (item.TituloNoApolo.Equals("Sim"))
                                        //    parcela = bdFinanceiro.PARC_DOC_FIN
                                        //        .Where(p => p.EmpCod == ef.EmpCod
                                        //            && p.ParcDocFinEntNome.Contains(item.NomeCedente.Substring(0, 4))
                                        //            && p.ParcDocFinDataVenc == item.DataVencimento
                                        //            && p.ParcDocFinValOrig == item.ValorTitulo
                                        //            && p.ParcDocFinProjecao == "Não")
                                        //        .FirstOrDefault();
                                        //else
                                        //    parcela = bdFinanceiro.PARC_DOC_FIN
                                        //        .Where(p => p.EmpCod == ef.EmpCod
                                        //            && p.ParcDocFinDataVenc == item.DataVencimento
                                        //            && p.ParcDocFinValOrig == item.ValorTitulo
                                        //            && p.ParcDocFinProjecao == "Não")
                                        //        .FirstOrDefault();

                                        PARC_DOC_FIN parcela = bdFinanceiro.PARC_DOC_FIN
                                            .Where(p => p.EmpCod == item.EmpresaApolo
                                                && p.DocFinChv == item.ChaveDocApolo
                                                && p.ParcDocFinDesmPag == item.SeqDesmPagDocApolo
                                                && p.ParcDocFinDupNum == item.DupNumDocApolo
                                                && p.ParcDocFinSeq == item.SeqDocApolo)
                                            .FirstOrDefault();

                                        if (parcela != null)
                                        {
                                            if (parcela.ParcDocFinDataPag == null)
                                            {
                                                parcela.ParcDocFinSegPag = "Bloquetos de Cobrança";
                                                parcela.ParcDocFinUsoLeitor = "Linha Digitável";
                                                parcela.ParcDocFinCodLeit = MontaLinhaDigitavel(item);
                                                item.ImportaNoApolo = "OK";
                                            }
                                            else
                                            {
                                                item.ImportaNoApolo = "Sem";
                                            }
                                        }
                                        //else
                                        //{
                                        //    parcela = bdFinanceiro.PARC_DOC_FIN
                                        //        .Where(p => p.ParcDocFinEntNome
                                        //                .Contains(item.NomeCedente.Substring(0, 4))
                                        //            && p.ParcDocFinDataVenc == item.DataVencimento
                                        //            && p.ParcDocFinValOrig == item.ValorTitulo
                                        //            && p.ParcDocFinProjecao == "Não")
                                        //        .FirstOrDefault();

                                        //    if (parcela != null)
                                        //    {
                                        //        if (parcela.ParcDocFinDataPag == null)
                                        //        {
                                        //            parcela.ParcDocFinSegPag = "Bloquetos de Cobrança";
                                        //            parcela.ParcDocFinUsoLeitor = "Linha Digitável";
                                        //            parcela.ParcDocFinCodLeit = MontaLinhaDigitavel(item);
                                        //            item.ImportaNoApolo = "OK";
                                        //        }
                                        //        else
                                        //        {
                                        //            item.ImportaNoApolo = "Sem";
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                            }

                            bdFinanceiro.SaveChanges();
                            bd.SaveChanges();
                        }
                    }
                }

                #endregion

                #region Envia E-mail p/ Fiscal

                if (model["emailFiscal"] != null)
                {
                    var fileIdsEmailFiscal = model["idEmailFiscal"].Split(',');
                    var selectedIndicesEmailFiscal = model["emailFiscal"].Replace("true,false", "true")
                                .Split(',')
                                .Select((item, index) => new { item = item, index = index })
                                .Where(row => row.item == "true")
                                .Select(row => row.index).ToArray();

                    foreach (var index in selectedIndicesEmailFiscal)
                    {
                        int fileId;
                        if (int.TryParse(fileIdsEmailFiscal[index], out fileId))
                        {
                            foreach (LayoutDDASegmentoG item in bd.LinhasSegmentoG.ToList())
                            {
                                if (fileId == item.ID)
                                {
                                    #region Gera LOG de Envio

                                    LayoutDDASegmentoGs_EnvioEmailFiscal layout =
                                        new LayoutDDASegmentoGs_EnvioEmailFiscal();

                                    // Dados de Controle
                                    layout.Empresa = item.Empresa;
                                    layout.BancoCompensacao = item.BancoCompensacao;
                                    layout.Lote = item.Lote;
                                    layout.Registro = item.Registro;
                                    layout.NumeroRegistro = item.NumeroRegistro;
                                    layout.Segmento = item.Segmento;
                                    layout.CNAB = item.CNAB;
                                    layout.Movimento = item.Movimento;

                                    // Dados de Título
                                    layout.BancoCedente = item.BancoCedente;
                                    layout.CodigoMoeda = item.CodigoMoeda;
                                    layout.DigitoVerificadorCodigoBarras = item.DigitoVerificadorCodigoBarras;
                                    layout.ValorImpressoCodigoBarras = item.ValorImpressoCodigoBarras;
                                    layout.CampoLivre = item.CampoLivre;
                                    layout.TipoInscricao = item.TipoInscricao;
                                    layout.Inscricao = item.Inscricao;
                                    layout.NomeCedente = item.NomeCedente;
                                    layout.DataVencimento = item.DataVencimento;
                                    layout.ValorTitulo = item.ValorTitulo;
                                    layout.QuantidadeMoeda = item.QuantidadeMoeda;
                                    layout.NumeroDocumento = item.NumeroDocumento;
                                    layout.Filler = item.Filler;
                                    layout.ValorAbatimento = item.ValorAbatimento;
                                    layout.CodigoCarteira = item.CodigoCarteira;
                                    layout.EspecieTitulo = item.EspecieTitulo;
                                    layout.DataEmissaoTitulo = item.DataEmissaoTitulo;
                                    layout.JurosMora = item.JurosMora;
                                    layout.CodigoDesconto1 = item.CodigoDesconto1;
                                    layout.DataDesconto1 = item.DataDesconto1;
                                    layout.Desconto1 = item.Desconto1;
                                    layout.CodigoProtesto = item.CodigoProtesto;
                                    layout.PrazoProtesto = item.PrazoProtesto;
                                    layout.DataLimite = item.DataLimite;
                                    layout.UserSession = item.UserSession;
                                    layout.ImportaNoApolo = item.ImportaNoApolo;
                                    layout.TituloNoApolo = item.TituloNoApolo;
                                    layout.Sacador = item.Sacador;
                                    layout.DataHora = DateTime.Now;
                                    layout.Usuario = Session["login"].ToString();

                                    hlbapp.LayoutDDASegmentoGs_EnvioEmailFiscal.AddObject(layout);

                                    hlbapp.SaveChanges();

                                    #endregion

                                    #region Envia E-mail

                                    //string cnpjEmpresa = item.Empresa.Replace(".", "").Replace("/", "").Replace("-", "");

                                    MvcAppHyLinedoBrasil.Models.EMPRESA_FILIAL ef = bdFinanceiro.EMPRESA_FILIAL
                                        .Where(w => w.EmpCod == item.Empresa).FirstOrDefault();

                                    WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                                    ObjectParameter numero =
                                        new ObjectParameter("codigo", typeof(global::System.String));

                                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);
 
                                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                                    email.WorkFlowEmailStat = "Enviar";
                                    email.WorkFlowEmailData = DateTime.Now;
                                    email.WorkFlowEmailParaNome = "DEPTO. FISCAL";
                                    email.WorkFlowEmailParaEmail = "fiscal@hyline.com.br";
                                    email.WorkFlowEmailCopiaPara = "financeiro@hyline.com.br";
                                    email.WorkFlowEmailDeNome = "DEPTO. FINANCEIRO";
                                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                                    email.WorkFlowEmailFormato = "Texto";
                                    email.WorkFlowEmailDocEmpCod = "5";

                                    email.WorkFlowEmailAssunto = "**** DOC. "
                                        + ef.EmpCod + " - " + item.NumeroDocumento.Trim() + " - VENCTO "
                                        + item.DataVencimento.ToShortDateString()
                                        + " NÃO DIGITADO NO APOLO ****";

                                    string corpoEmail = "Prezado(s), " + (char)13 + (char)10 + (char)13 + (char)10
                                        + "O documento Emp. " + ef.EmpCod + " - " + item.NumeroDocumento.Trim() + " - "
                                        + (item.Sacador.Trim() == "" ? item.NomeCedente.Trim() : item.Sacador.Trim())
                                        + " - R$ "
                                        + String.Format("{0:N}", item.ValorTitulo.ToString())
                                        + " que consta no Arquivo DDA não foi digitado no Apolo." + (char)13 + (char)10
                                        + "Por favor, verificar, pois o vencimento é em "
                                        + item.DataVencimento.ToShortDateString() + "."
                                        + (char)13 + (char)10 + (char)13 + (char)10
                                        + "Arquivo: " + Session["fileName"].ToString()
                                        + (char)13 + (char)10 + (char)13 + (char)10
                                        + "DEPTO. FINANCEIRO";

                                    email.WorkFlowEmailCorpo = corpoEmail;

                                    apolo.WORKFLOW_EMAIL.AddObject(email);

                                    apolo.SaveChanges();

                                    #endregion

                                    item.EnviaEmailFiscal = "Enviado";
                                }
                            }
                        }
                    }
                }

                #endregion

                ViewBag.mensagemImportacao = "Ações nos Títulos geradads com sucesso! - Arquivo: "
                    + Session["fileName"].ToString();

                string userSession = Session["login"].ToString() + Session.SessionID;

                //VerificaParcelaApolo();
                //VerificaEnvioEmailFiscal();

                var lista = bd.LinhasSegmentoG
                    .Where(w => w.UserSession == userSession)
                    .OrderBy(l => l.NomeCedente)
                    .ThenBy(l => l.NumeroDocumento)
                    .ThenBy(l => l.DataVencimento);

                return View("Index", lista);
            }
            catch (Exception e)
            {
                if (e.InnerException == null)
                    ViewBag.mensagemImportacao = "Erro na importação: " + e.Message + " - Arquivo: "
                    + Session["fileName"].ToString();
                else
                    ViewBag.mensagemImportacao = "Erro na importação: " + e.Message
                        + " / Erro Interno: " + e.InnerException.Message + " - Arquivo: "
                    + Session["fileName"].ToString();
                return View("Index", bd.LinhasSegmentoG.ToList());
            }
        }
    }
}
