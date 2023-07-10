using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using ImportaIncubacao.Data.Apolo;
using ImportaIncubacao;
using System.Data.Objects;
using MvcAppHyLinedoBrasil.Models.XML;
using MvcAppHyLinedoBrasil.Models.HLBAPP;
using MvcAppHyLinedoBrasil.Models.Apolo;
using System.Data.SqlClient;
using System.Data;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class ImportaXMLFiscalController : Controller
    {
        #region Importa CT-e

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            //DateTime dataEmissao = Convert.ToDateTime("19/12/2017");
            //CalculaVencimento(dataEmissao, "0022905");
            Session["ListaFinalidadeCTe"] = CarregaFinalidadesCTe();
            Session["ListaTipoFreteCTe"] = CarregaTipoFreteCTe();

            return View();
        }

        [HttpPost]
        //public ActionResult ImportaXML(string data, string codigoEmpresa, string finalidade, string tipoFrete)
        public ActionResult ImportaXML(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string msgRetorno = "";
            //MvcAppHyLinedoBrasil.Models.SequenciaLinha retorno = new MvcAppHyLinedoBrasil.Models.SequenciaLinha();
            //retorno.ID = 1;

            #region Carrega Variáveis

            string data = model["dataEntrada"].ToString();
            //string codigoEmpresa = model["codigoEmpresa"].ToString();
            string codigoEmpresa = null;
            string finalidade = model["ddlFinalidadeCTe"].ToString();
            string tipoFrete = model["ddlTipoFreteCTe"].ToString();

            #endregion

            #region Verifica se campos estão inseridos

            if (finalidade == null || finalidade == "")
            {
                msgRetorno = "Obrigatório selecionar a finalidade do CT-e!";
                ViewBag.msg = "<h4 style='color: Red;'>" + msgRetorno + "</h4>";
                return View("Index");
            }

            #endregion

            for (int i = 0; i < Request.Files.Count; i++)
            {
                string msg = "";

                HttpPostedFileBase itemArq = Request.Files[i];
                StreamReader arquivo = new StreamReader(itemArq.InputStream);

                #region Carrega a Chave do XML

                string chaveCTE = "";
                XmlDocument oXML = new XmlDocument();
                oXML.Load(arquivo);

                XmlNode cte = null;

                XmlNodeList cteList = oXML.ChildNodes;

                foreach (XmlNode item in cteList)
                {
                    if (item.Name.Equals("cteProc"))
                        cte = item.FirstChild.FirstChild;
                }

                if (cte != null)
                    foreach (XmlAttribute infCTe in cte.Attributes)
                        if (infCTe.Name.Equals("Id")) chaveCTE = infCTe.InnerText.Replace("CTe", "");

                arquivo.BaseStream.Position = 0;

                #endregion

                if (chaveCTE == "")
                {
                    #region Verifica se é CT-e

                    //retorno.ID = 0;
                    //retorno.Importado = "O XML " + Request.Files[0].FileName + " não é de NF-e! Verifique!"; ;
                    //return View("Arquivo", retorno);
                    msgRetorno = msgRetorno + "<h4 style='color: Red;'>O XML " + itemArq.FileName + " não é de CT-e! Verifique!</h4>";

                    #endregion
                }
                else
                {
                    msg = ImportaXMLCTe(data, codigoEmpresa, finalidade, tipoFrete, arquivo);

                    if (msg != "")
                        msgRetorno = msgRetorno + "<h4 style='color: Red;'>Erro ao importar chave " + chaveCTE + ": " + msg + "</h4>";
                    else
                        msgRetorno = msgRetorno + "<h4 style='color: Blue;'>XML de chave " + chaveCTE + " importado com sucesso!</h4>";

                    //ViewBag.OK = msgRetorno;
                    //retorno.Importado = msgRetorno;
                    //return View("Arquivo", retorno);
                }
            }

            ViewBag.msg = msgRetorno;
            return View("Index");
        }

        public string ImportaXMLCTe(string data, string codigoEmpresa, string finalidade, string tipoFrete, StreamReader arquivo)
        {
            string msgRetorno = "";

            try
            {
                Apolo10EntitiesService bdApolo = new Apolo10EntitiesService();
                ImportaIncubacaoService servico = new ImportaIncubacaoService();
                Models.FinanceiroEntities apolo = new Models.FinanceiroEntities();

                #region Inicializa a lista de notas vinculadas ao CT-e

                List<MOV_ESTQ_DOC_COMPLEM> listaNFes = new List<MOV_ESTQ_DOC_COMPLEM>();

                #endregion

                #region Lê arquivo XML

                string cnpjEmitente = "";
                string serie = "";
                string numCTE = "";
                DateTime dataEmissao = new DateTime();
                string cnpjRemetente = "";
                string codCidadeIBGERemetente = "";
                string codCidadeIBGEDestinatario = "";
                decimal valor = 0;
                string tribBICMS = "";
                decimal valBaseICMS = 0;
                decimal percICMS = 0;
                decimal valICMS = 0;
                string chaveNFE = "";
                string chaveCTE = "";
                string chaveCTEComp = "";

                XmlDocument oXML = new XmlDocument();
                oXML.Load(arquivo);

                XmlNode cte = null;

                XmlNodeList cteList = oXML.ChildNodes;

                foreach (XmlNode item in cteList)
                {
                    if (item.Name.Equals("cteProc"))
                        cte = item.FirstChild.FirstChild;
                }

                if (cte != null)
                {
                    foreach (XmlAttribute infCte in cte.Attributes)
                    {
                        if (infCte.Name.Equals("Id")) chaveCTE = infCte.InnerText.Replace("CTe", "");
                    }

                    XmlNodeList childNodes = cte.ChildNodes;

                    foreach (XmlNode item in childNodes)
                    {
                        #region ide
                        if (item.Name.Equals("ide"))
                        {
                            foreach (XmlNode ide in item.ChildNodes)
                            {
                                if (ide.Name.Equals("serie")) serie = ide.InnerText;
                                if (ide.Name.Equals("nCT")) numCTE = ide.InnerText;
                                if (ide.Name.Equals("dhEmi")) dataEmissao =
                                    Convert.ToDateTime(Convert.ToDateTime(ide.InnerText).ToShortDateString());
                            }
                        }
                        #endregion

                        if (item.Name.Equals("emit")) cnpjEmitente = item.FirstChild.InnerText;
                        //if (item.Name.Equals("rem")) cnpjRemetente = item.FirstChild.InnerText;

                        #region rem
                        if (item.Name.Equals("rem"))
                        {
                            cnpjRemetente = item.FirstChild.InnerText;
                            foreach (XmlNode rem in item.ChildNodes)
                            {
                                if (rem.Name.Equals("enderReme"))
                                {
                                    foreach (XmlNode enderReme in rem.ChildNodes)
                                    {
                                        if (enderReme.Name.Equals("cMun"))
                                            codCidadeIBGERemetente = enderReme.InnerText;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region dest
                        if (item.Name.Equals("dest"))
                        {
                            foreach (XmlNode dest in item.ChildNodes)
                            {
                                if (dest.Name.Equals("enderDest"))
                                {
                                    foreach (XmlNode enderDest in dest.ChildNodes)
                                    {
                                        if (enderDest.Name.Equals("cMun"))
                                            codCidadeIBGEDestinatario = enderDest.InnerText;
                                    }
                                }
                            }
                        }
                        #endregion

                        if (item.Name.Equals("vPrest")) valor = Convert.ToDecimal(item.FirstChild.InnerText.Replace(".", ","));
                        if (item.Name.Equals("infCteComp")) chaveCTEComp = item.FirstChild.InnerText;

                        #region imp
                        if (item.Name.Equals("imp"))
                            foreach (XmlNode imp in item.ChildNodes)
                            {
                                if (imp.Name.Equals("ICMS"))
                                    foreach (XmlNode icms in imp.ChildNodes)
                                    {
                                        foreach (XmlNode icmsTrib in icms.ChildNodes)
                                        {
                                            if (icmsTrib.Name.Equals("CST")) tribBICMS = icmsTrib.InnerText;
                                            if (icmsTrib.Name.Equals("vBC")) valBaseICMS = Convert.ToDecimal(icmsTrib.InnerText.Replace(".", ","));
                                            if (icmsTrib.Name.Equals("pICMS")) percICMS = Convert.ToDecimal(icmsTrib.InnerText.Replace(".", ","));
                                            if (icmsTrib.Name.Equals("vICMS")) valICMS = Convert.ToDecimal(icmsTrib.InnerText.Replace(".", ","));
                                        }
                                    }
                            }
                        #endregion

                        #region infCTeNorm
                        if (item.Name.Equals("infCTeNorm"))
                            foreach (XmlNode infCTeNorm in item.ChildNodes)
                            {
                                if (infCTeNorm.Name.Equals("infDoc"))
                                {
                                    int seq = 1;
                                    // Lista NFes relacionadas
                                    foreach (XmlNode infNFe in infCTeNorm.ChildNodes)
                                    {
                                        if (infNFe.Name.Equals("infNFe"))
                                        {
                                            if (infNFe.FirstChild != null)
                                            {
                                                chaveNFE = infNFe.FirstChild.InnerText;

                                                //Localiza dados do Apolo
                                                NOTA_FISCAL_ELETRONICA_TRANS nfe = bdApolo
                                                    .NOTA_FISCAL_ELETRONICA_TRANS
                                                    .Where(w => w.NFETransChvAcesso == chaveNFE)
                                                    .FirstOrDefault();

                                                if (nfe != null)
                                                {
                                                    // Cria objeto e insere na lista
                                                    MOV_ESTQ_DOC_COMPLEM movEstqDocComplem = new MOV_ESTQ_DOC_COMPLEM();
                                                    movEstqDocComplem.MovEstqDocComplemSeq = seq;
                                                    movEstqDocComplem.MovEstqDocComplemEspec = nfe.CtrlDFModForm;
                                                    movEstqDocComplem.MovEstqDocComplemSerie = nfe.CtrlDFSerie;
                                                    movEstqDocComplem.MovEstqDocComplemNum = nfe.NFNum;
                                                    movEstqDocComplem.MovEstqDocComplemBaseII = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValII = 0;
                                                    movEstqDocComplem.MovEstqDocComplemBaseIcms = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValIcms = 0;
                                                    movEstqDocComplem.MovEstqDocComplemBaseIcmsST = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValIcmsRetST = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValMerc = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValFinalMerc = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValFrete = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValSeguro = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValOutra = 0;
                                                    movEstqDocComplem.MovEstqDocComplemBaseIpi = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValIpi = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValDoc = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValSiscomex = 0;
                                                    movEstqDocComplem.MovEstqDocComplemBasePis = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValPis = 0;
                                                    movEstqDocComplem.MovEstqDocComplemBaseCofins = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValCofins = 0;
                                                    movEstqDocComplem.MovEstqDocComplemValCap = 0;
                                                    movEstqDocComplem.MovEstqDocComplemDataEmissao = nfe.NFETransDataEmis;

                                                    listaNFes.Add(movEstqDocComplem);
                                                }

                                                seq = seq + 1;
                                            }
                                        }
                                    }

                                    //if (infCTeNorm.FirstChild != null)
                                    //    if (infCTeNorm.FirstChild.FirstChild != null)
                                    //        chaveNFE = infCTeNorm.FirstChild.FirstChild.InnerText;
                                }
                            }
                        #endregion
                    }
                }

                #endregion

                #region Insere no Apolo

                #region Verifica Configurações

                string cFOP = "";
                string conta = "";
                string rateio = "";
                string entCod = "";
                int movEstqChv = 0;
                string empresaStr = "";

                MOV_ESTQ_NF_ELETRONICA cteMov = bdApolo.MOV_ESTQ_NF_ELETRONICA
                    .Where(w => w.MovEstqNFEImpChvAcesso == chaveCTE).FirstOrDefault();

                if (cteMov != null)
                {
                    msgRetorno = "CT-e " + chaveCTE + " já cadastrado no Apolo! (Empresa "
                        + cteMov.EmpCod + " - Chave " + cteMov.MovEstqChv.ToString() + ")";
                    //ViewBag.Erro = msgRetorno;
                    //retorno.ID = 0;
                    //retorno.Importado = msgRetorno;
                    //return View("Arquivo", retorno);
                    return msgRetorno;
                }

                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL.Where(w => w.EmpCpfCgc == cnpjRemetente)
                    .FirstOrDefault();

                if (codigoEmpresa != null)
                    empresa = bdApolo.EMPRESA_FILIAL.Where(w => w.EmpCod == codigoEmpresa)
                        .FirstOrDefault();

                if (empresa == null)
                {
                    if (codigoEmpresa == null)
                        msgRetorno = "CNPJ " + cnpjRemetente + " da Empresa não cadastrado no Apolo! Verifique!";
                    else
                        msgRetorno = "Empresa " + codigoEmpresa + " não cadastrada no Apolo! Verifique!";
                    //retorno.ID = 0;
                    //retorno.Importado = msgRetorno;
                    //return View("Arquivo", retorno);
                    return msgRetorno;
                }

                string codCidadeRemetente = null;
                Models.CIDADE cidadeRemetente = apolo.CIDADE
                    .Where(w => w.CidCodMunDipj == codCidadeIBGERemetente).FirstOrDefault();
                if (cidadeRemetente != null) codCidadeRemetente = cidadeRemetente.CidCod;

                string codCidadeDestinatario = null;
                Models.CIDADE cidadeDestinatario = apolo.CIDADE
                    .Where(w => w.CidCodMunDipj == codCidadeIBGEDestinatario).FirstOrDefault();
                if (cidadeDestinatario != null) codCidadeDestinatario = cidadeDestinatario.CidCod;

                #endregion

                #region Carrega Variaveis Constantes

                string tipoLanc = "E0000332";
                string especie = "CT-e";
                string produto = "998.018";
                string clasFiscal = "0000122";
                string ncm = "99999999";
                string locArmaz = "01";
                string pisCofinsTributacao = "";
                decimal valBasePisCofins = 0;
                decimal percPis = 0;
                decimal percCofins = 0;
                decimal valPis = 0;
                decimal valCofins = 0;
                string condPag = "0059";
                string usuario = (Session["login"].ToString() == "palves" ? "RIOSOFT"
                    : Session["login"].ToString().ToUpper());
                string unidMed = "UN";
                short posicaoUnidMed = 1;
                string tribCod = "0";
                if (tribBICMS != ""
                    // 08/12/2017 - Solicitado por Marcelo Notti - O CST 90 é incorreto. Sendo assim,
                    // deve mudar para 40 caso seja 90.
                    && tribBICMS != "90")
                    tribCod = tribCod + tribBICMS;
                else
                    tribCod = tribCod + "40";

                short sequencia = 1;
                decimal quantidade = 1;

                /**
                * 10/03/2017 - Solicitado por Marcelo Notti
                *      H&N mudou para Lucro Real, ou seja, Pis Cofins = HLB e LTZ
                */
                //if (empresa.EmpNome.Contains("LOHMANN") || empresa.EmpNome.Contains("HY-LINE"))
                //{
                pisCofinsTributacao = "55";
                valBasePisCofins = valor;
                percPis = 1.65m;
                percCofins = 7.6m;
                valPis = valBasePisCofins * (percPis / 100.00m);
                valCofins = valBasePisCofins * (percCofins / 100.00m);
                //}
                //else
                //    pisCofinsTributacao = "70";

                #endregion

                if (chaveNFE != "")
                {
                    #region CT-e Principal

                    #region Carrega NF-e se for Saída

                    NOTA_FISCAL_ELETRONICA_TRANS nfe = bdApolo.NOTA_FISCAL_ELETRONICA_TRANS
                        .Where(w => w.NFETransChvAcesso == chaveNFE).FirstOrDefault();

                    #endregion

                    #region Carrega NF-e se for Entrada

                    MOV_ESTQ_NF_ELETRONICA nfeEntrada = bdApolo.MOV_ESTQ_NF_ELETRONICA
                        .Where(w => w.MovEstqNFEImpChvAcesso == chaveNFE).FirstOrDefault();

                    #endregion

                    if (nfe != null || nfeEntrada != null)
                    //if (nfe != null)
                    {
                        #region Para os CT-es da Planalto, verifica se as notas são Saída de pintos para importar

                        if (nfe != null)
                        {
                            int existePinto = bdApolo.ITEM_NF
                                .Where(w => w.EmpCod == nfe.EmpCod && w.CtrlDFModForm == nfe.CtrlDFModForm
                                    && w.CtrlDFSerie == nfe.CtrlDFSerie && w.NFNum == nfe.NFNum
                                    && w.ItNFProdNome.Contains("PINT")).Count();

                            if (empresa.EmpNome.Contains("PLANALTO") && existePinto == 0)
                            {
                                msgRetorno = "NF-e " + chaveNFE + " não é de pintos! Verifique!";
                                //retorno.ID = 0;
                                //retorno.Importado = msgRetorno;
                                //return View("Arquivo", retorno);
                                return msgRetorno;
                            }
                        }

                        #endregion

                        #region Carrega Configurações Apolo

                        #region Carrega Entidade Emitente do CT-e

                        ImportaIncubacao.Data.Apolo.ENTIDADE emitente = bdApolo.ENTIDADE
                            .Where(w => w.EntCpfCgc == cnpjEmitente && w.EntNat == "Transportador"
                                && !w.StatEntCod.Equals("05"))
                            .FirstOrDefault();

                        if (emitente == null)
                        {
                            msgRetorno = "O transportado com o CNPJ " + cnpjEmitente + " não cadastrado no Apolo ou as configurações não estão incorretas"
                                + " (Natureza tem que ser 'Transportador' e Status como 'Ativo')! Verifique!";
                            //ViewBag.Erro = msgRetorno;
                            //retorno.ID = 0;
                            //retorno.Importado = msgRetorno;
                            //return View("Arquivo", retorno);
                            return msgRetorno;
                        }

                        entCod = emitente.EntCod;
                        ImportaIncubacao.Data.Apolo.ENTIDADE1 emitente1 = bdApolo.ENTIDADE1
                            .Where(w => w.EntCod == entCod).FirstOrDefault();

                        #endregion

                        #region Configuração Tipo de Lançamento - Emitente

                        if (emitente1.USERTipoLancCTE != null && emitente1.USERTipoLancCTE != "")
                            tipoLanc = emitente1.USERTipoLancCTE;

                        #endregion

                        #region Configurações de CFOP, Conta e Rateio se a origem for Saída

                        if (nfe != null)
                        {
                            #region Carrega Entidade Destinatário do CT-e

                            ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = bdApolo.ENTIDADE1.Where(w => w.EntCod == nfe.EntCod).FirstOrDefault();

                            #endregion

                            #region Configuração CFOP - Destinatário

                            if (entidade1.USERCTeCFOP != null)
                            {
                                if (!entidade1.USERCTeCFOP.Equals(""))
                                {
                                    /**
                                     * 10/03/2017 - Solicitado por Marcelo Notti
                                     *      H&N mudou para Lucro Real, ou seja, Pis Cofins = HLB e LTZ
                                     */
                                    //if (empresa.EmpNome.Contains("LOHMANN") || empresa.EmpNome.Contains("HY-LINE"))
                                    //    cFOP = entidade1.USERCTeCFOP;
                                    //else
                                    //    cFOP = entidade1.USERCTeCFOP.Substring(0, 5);
                                    cFOP = entidade1.USERCTeCFOP;
                                }
                                else
                                {
                                    /**
                                     * 10/03/2017 - Solicitado por Marcelo Notti
                                     *      H&N mudou para Lucro Real, ou seja, Pis Cofins = HLB e LTZ
                                     */
                                    //if (empresa.EmpNome.Contains("LOHMANN") || empresa.EmpNome.Contains("HY-LINE"))
                                    //    cFOP = "1.352.001";
                                    //else
                                    //    cFOP = "1.352";
                                    if (cidadeRemetente.UfSigla == cidadeDestinatario.UfSigla)
                                        cFOP = "1.352.001";
                                    else
                                        cFOP = "2.352.001";
                                }
                            }
                            else
                            {
                                /**
                                * 10/03/2017 - Solicitado por Marcelo Notti
                                *      H&N mudou para Lucro Real, ou seja, Pis Cofins = HLB e LTZ
                                */
                                //if (empresa.EmpNome.Contains("LOHMANN") || empresa.EmpNome.Contains("HY-LINE"))
                                //    cFOP = "1.352.001";
                                //else
                                //    cFOP = "1.352";
                                if (cidadeRemetente.UfSigla == cidadeDestinatario.UfSigla)
                                    cFOP = "1.352.001";
                                else
                                    cFOP = "2.352.001";
                            }

                            #endregion

                            #region Configuração Conta - Destinatário

                            if (entidade1.USERCTeConta != "" && entidade1.USERCTeConta != null)
                                conta = entidade1.USERCTeConta;
                            else
                                conta = "3.988";

                            #endregion

                            #region Configuração Rateio - Destinatário

                            if (entidade1.USERCTeRateio != "" && entidade1.USERCTeRateio != null)
                                rateio = entidade1.USERCTeRateio;
                            else
                            {
                                if (empresa.EmpNome.Contains("HY-LINE")
                                    || empresa.EmpNome.Contains("PLANALTO"))
                                    rateio = "3.03.0001";
                                else if (empresa.EmpNome.Contains("LOHMANN"))
                                    rateio = "3.03.0021";
                                else
                                    rateio = "3.03.0024";
                            }

                            #endregion
                        }

                        #endregion

                        #region Configurações de CFOP, Conta e Rateio se a origem for Entrada

                        if (nfeEntrada != null)
                        {
                            #region Carrega Movimentação de Estoque de Origem

                            MOV_ESTQ movEstqOrigem = bdApolo.MOV_ESTQ
                                .Where(w => w.EmpCod == nfeEntrada.EmpCod
                                    && w.MovEstqChv == nfeEntrada.MovEstqChv).FirstOrDefault();

                            #endregion

                            #region Carrega Entidade Destinatário do CT-e

                            ImportaIncubacao.Data.Apolo.ENTIDADE entidade = bdApolo.ENTIDADE.Where(w => w.EntCod == movEstqOrigem.EntCod).FirstOrDefault();

                            #endregion

                            #region Verifica se a Nota de Origem é de Insumo

                            int existeInsumo = bdApolo.ITEM_MOV_ESTQ
                                .Where(w => w.EmpCod == nfeEntrada.EmpCod
                                    && w.MovEstqChv == nfeEntrada.MovEstqChv
                                    && bdApolo.PROD_GRUPO_SUBGRUPO
                                        .Any(p => p.ProdCodEstr == w.ProdCodEstr
                                            && p.GrpProdCod == "003")).Count();

                            #endregion

                            #region Configuração CFOP

                            if (existeInsumo > 0)
                            {
                                if (cidadeRemetente.UfSigla == cidadeDestinatario.UfSigla)
                                    cFOP = "1.352";
                                else
                                    cFOP = "2.352";
                            }

                            #endregion

                            #region Configuração Conta

                            #region Se a nota de origem for de Insumo, a conta será a da nota de origem

                            if (existeInsumo > 0)
                            {
                                MOV_ESTQ_CLASSE_REC_DESP movEstqCRCOrigem = bdApolo.MOV_ESTQ_CLASSE_REC_DESP
                                    .Where(w => w.EmpCod == nfeEntrada.EmpCod
                                        && w.MovEstqChv == nfeEntrada.MovEstqChv).FirstOrDefault();

                                if (movEstqCRCOrigem != null) conta = movEstqCRCOrigem.ClasseRecDespCodEstr;
                            }

                            #endregion

                            #endregion
                        }

                        #endregion

                        #region Configuração ICMS - Emitente ou Origem Destino da Mercadoria

                        if (valICMS == 0 && tribBICMS != "40")
                        {
                            if (emitente1.USERCTeAliqICMS > 0)
                            {
                                percICMS = Convert.ToDecimal(emitente1.USERCTeAliqICMS);
                                tribCod = "000";
                                tribBICMS = "00";
                                valBaseICMS = valor;
                                valICMS = valor * (percICMS / 100.00m);
                            }
                            else
                            {
                                if (tribBICMS == "60")
                                {
                                    Models.CLAS_FISCAL_AUX clasFiscalAux = apolo.CLAS_FISCAL_AUX
                                        .Where(w => w.ClasFiscCod == clasFiscal
                                            && w.ClasAuxOper == "Saída"
                                            && w.ClasAuxPaisSiglaOrig == cidadeRemetente.PaisSigla
                                            && w.ClasAuxUfSiglaOrig == cidadeRemetente.UfSigla
                                            && w.ClasAuxPaisSiglaDest == cidadeDestinatario.PaisSigla
                                            && w.ClasAuxUfSiglaDest == cidadeDestinatario.UfSigla
                                            && w.TribBCod == tribBICMS).FirstOrDefault();

                                    if (clasFiscalAux != null)
                                    {
                                        percICMS = Convert.ToDecimal(clasFiscalAux.ClasAuxPercIcmsInteres);
                                        tribCod = "000";
                                        tribBICMS = "00";
                                        valBaseICMS = valor;
                                        valICMS = valor * (percICMS / 100.00m);
                                    }
                                    else
                                    {
                                        msgRetorno = "Não existe configuração de ICMS de 'Saída' na classificação fiscal " + clasFiscal
                                            + " de Origem " + cidadeRemetente.PaisSigla + " - " + cidadeRemetente.UfSigla
                                            + " para Destino " + cidadeDestinatario.PaisSigla + " - " + cidadeDestinatario.UfSigla
                                            + " com a tributação " + tribBICMS + "! Realize a configuração e importe o XML novamente!";
                                        //ViewBag.Erro = msgRetorno;
                                        //retorno.ID = 0;
                                        //retorno.Importado = msgRetorno;
                                        //return View("Arquivo", retorno);
                                        return msgRetorno;
                                    }
                                }
                            }
                        }

                        #endregion

                        #endregion

                        #region Insere Movimentação de Estoque no Apolo

                        #region MOV_ESTQ

                        MOV_ESTQ movestq = servico.InsereMovEstq(empresa.EmpCod, tipoLanc, entCod, dataEmissao,
                            usuario);

                        if (data != null && data != "")
                        {
                            movestq.MovEstqDataMovimento = Convert.ToDateTime(data);
                            movestq.MovEstqDataEntrada = movestq.MovEstqDataMovimento;
                        }

                        movestq.MovEstqDocEmpCod = movestq.EmpCod;
                        movestq.MovEstqDocEspec = especie;
                        movestq.MovEstqDocSerie = serie;
                        movestq.MovEstqDocNum = numCTE;
                        movestq.CondPagCod = condPag;
                        movestq.MovEstqValMerc = valor;
                        movestq.MovEstqBaseIcms = valBaseICMS;
                        movestq.MovEstqValIcms = valICMS;
                        movestq.MovEstqValFinalMerc = valor;
                        movestq.MovEstqValDoc = valor;
                        movestq.MovEstqValLib = valor;
                        movestq.MovEstqValOrig = valor;
                        movestq.MovEstqValBasePis = valBasePisCofins;
                        movestq.MovEstqValBaseCofins = valBasePisCofins;
                        movestq.MovEstqValDocLiq = valor;
                        //movestq.MovEstqObs = "XML Importado via Sistema WEB";
                        movestq.MovEstqObs = finalidade;
                        movestq.MovEstqPercDescGer = 0;
                        movestq.MovEstqPercDescGerProd = 0;
                        movestq.MovEstqPercDescGerServ = 0;
                        movestq.MovEstqValDescGer = 0;
                        movestq.MovEstqValDescGerProd = 0;
                        movestq.MovEstqValDescGerServ = 0;
                        movestq.MovEstq = "Não";
                        movestq.MovEstqCidCodOrig = codCidadeRemetente;
                        movestq.MovEstqCidCodDest = codCidadeDestinatario;
                        movestq.CidCodInicPrestServ = codCidadeRemetente;
                        movestq.CidCodFinalPrestServ = codCidadeDestinatario;
                        movestq.MovEstqGeraFiscal = "Sim";

                        movestq.MovEstqNFEmisProp = "Não";

                        // 08/12/2020 - Chamado 57934
                        movestq.MovEstqIndTipoFrete = tipoFrete;

                        bdApolo.MOV_ESTQ.AddObject(movestq);

                        #endregion

                        #region MOV_ESTQ_NF_ELETRONICA

                        MOV_ESTQ_NF_ELETRONICA chaveMov = new MOV_ESTQ_NF_ELETRONICA();
                        chaveMov.EmpCod = movestq.EmpCod;
                        chaveMov.MovEstqChv = movestq.MovEstqChv;
                        chaveMov.UsuCod = usuario;
                        chaveMov.MovEstqNFEImpData = movestq.MovEstqDataMovimento;
                        chaveMov.MovEstqNFEImpChvAcesso = chaveCTE;
                        chaveMov.MovEstqNFEImpStatus = "Importado";

                        bdApolo.MOV_ESTQ_NF_ELETRONICA.AddObject(chaveMov);

                        #endregion

                        #region ITEM_MOV_ESTQ

                        ITEM_MOV_ESTQ itemMovEstq = servico.InsereItemMovEstq(movestq.MovEstqChv, movestq.EmpCod,
                            movestq.TipoLancCod, movestq.EntCod, movestq.MovEstqDataMovimento, produto, cFOP, quantidade, valor,
                            unidMed, posicaoUnidMed, tribCod, ncm, clasFiscal);

                        itemMovEstq.ItMovEstqPercIcms = percICMS;
                        itemMovEstq.ItMovEstqBaseIcms = valBaseICMS;
                        itemMovEstq.ItMovEstqValIcms = valICMS;
                        itemMovEstq.ItMovEstqValIcmsRec = valICMS;
                        itemMovEstq.ItMovEstqValICMSOrig = valICMS;
                        itemMovEstq.ItMovEstqConfTribPisCod = pisCofinsTributacao;
                        itemMovEstq.ItMovEstqConfTribCofinsCod = pisCofinsTributacao;
                        itemMovEstq.ItMovEstqValBasePis = valBasePisCofins;
                        itemMovEstq.ItMovEstqValBaseCofins = valBasePisCofins;
                        itemMovEstq.ItMovEstqPercPis = percPis;
                        itemMovEstq.ItMovEstqPercCofins = percCofins;
                        itemMovEstq.ItMovEstqValPis = valPis;
                        itemMovEstq.ItMovEstqValPisRec = valPis;
                        itemMovEstq.ItMovEstqValPISOrig = valPis;
                        itemMovEstq.ItMovEstqValCofins = valCofins;
                        itemMovEstq.ItMovEstqValCofinsRec = valCofins;
                        itemMovEstq.ItMovEstqValCOFINSOrig = valCofins;
                        itemMovEstq.ItMovEstqCustoUnit = valor;
                        itemMovEstq.ItMovEstqValProd = valor;

                        #region Inicializa Valores Item_Mov_Estq

                        itemMovEstq.ItMovEstqValAcrescFin = 0;
                        itemMovEstq.ItMovEstqValDescEspec = 0;
                        itemMovEstq.ItMovEstqValDespDiv = 0;
                        itemMovEstq.ItMovEstqValDescGer = 0;
                        itemMovEstq.ItMovEstqPercAcrescFin = 0;
                        itemMovEstq.ItMovEstqPercDescEspec = 0;
                        itemMovEstq.ItMovEstqValEmbalagem = 0;
                        itemMovEstq.ItMovEstqValFrete = 0;
                        itemMovEstq.ItMovEstqValSeguro = 0;
                        itemMovEstq.ItMovEstqValOutra = 0;
                        itemMovEstq.ItMovEstqValServ = 0;
                        itemMovEstq.ItMovEstqPercRedBaseIcms = 0;
                        itemMovEstq.ItMovEstqValRedBaseIcms = 0;
                        itemMovEstq.ItMovEstqBaseIcmsRed = valBaseICMS;
                        itemMovEstq.ItMovEstqST = "F";
                        itemMovEstq.ItMovEstqValIcmsRetST = 0;
                        itemMovEstq.ItMovEstqValIcmsRetSTRec = 0;
                        itemMovEstq.ItMovEstqBaseIpi = 0;
                        itemMovEstq.ItMovEstqPercIpi = 0;
                        itemMovEstq.ItMovEstqValIpi = 0;
                        itemMovEstq.ItMovEstqIpiBaseIcms = "Não";
                        itemMovEstq.ItMovEstqValIpiRec = 0;
                        itemMovEstq.ItMovEstqValBaseIss = 0;
                        itemMovEstq.ItMovEstqPercIss = 0;
                        itemMovEstq.ItMovEstqValIss = 0;
                        itemMovEstq.ItMovEstqValIrrf = 0;
                        itemMovEstq.ItMovEstqPercIrrf = 0;
                        itemMovEstq.ItMovEstqValBaseInss = 0;
                        itemMovEstq.ItMovEstqPercInss = 0;
                        itemMovEstq.ItMovEstqValInss = 0;
                        itemMovEstq.ItMovEstqQtdUnidMed = 0;
                        itemMovEstq.ItMovEstqCredIpiCompraCom = "Não";
                        itemMovEstq.ItMovEstqCredIpiCompraComPerc = 0;
                        itemMovEstq.ItMovEstqCalcDifIcms = "Não";
                        itemMovEstq.ItMovEstqPercDifIcms = 0;
                        itemMovEstq.ItMovEstqQtdDesm = 0;
                        itemMovEstq.ItMovEstqRejPat = "Não";
                        itemMovEstq.ItMovEstqBaseII = 0;
                        itemMovEstq.ItMovEstqPercII = 0;
                        itemMovEstq.ItMovEstqValIIRec = 0;
                        itemMovEstq.ItMovEstqValProdDoc = 0;
                        itemMovEstq.ItMovEstqFreteDocOrigVal = 0;
                        itemMovEstq.ItMovEstqSegDocOrigVal = 0;
                        itemMovEstq.ItMovEstqOutraDespDocOrigVal = 0;
                        itemMovEstq.ItMovEstqValProdFOB = 0;
                        itemMovEstq.ItMovEstqValSiscomex = 0;
                        itemMovEstq.ItMovEstqValCalcIssDedTot = 0;
                        itemMovEstq.ItMovEstqValDifIcms = 0;
                        itemMovEstq.ItMovEstqValBaseCsll = 0;
                        itemMovEstq.ItMovEstqPercCsllRF = 0;
                        itemMovEstq.ItMovEstqValCsllRF = 0;
                        itemMovEstq.ItMovEstqPercCofinsRF = 0;
                        itemMovEstq.ItMovEstqValCofinsRF = 0;
                        itemMovEstq.ItMovEstqPercPisRF = 0;
                        itemMovEstq.ItMovEstqValPisRF = 0;
                        itemMovEstq.ItMovEstqValAcrescCustoComp = 0;
                        itemMovEstq.ItMovEstqValDescCustoComp = 0;
                        itemMovEstq.ItMovEstqPercRedBasePis = 0;
                        itemMovEstq.ItMovEstqValRedBasePis = 0;
                        itemMovEstq.ItMovEstqBasePisRed = valBasePisCofins;
                        itemMovEstq.ItMovEstqPercRedBaseCofins = 0;
                        itemMovEstq.ItMovEstqValRedBaseCofins = 0;
                        itemMovEstq.ItMovEstqBaseCofinsRed = valBasePisCofins;
                        itemMovEstq.ItMovEstqPat = "Não";
                        itemMovEstq.ItMovEstqValDescGer = 0;
                        itemMovEstq.ItMovEstqRepIcmsDifPercDesc = 0;
                        itemMovEstq.ItMovEstqRepIcmsDifValDesc = 0;
                        itemMovEstq.ItMovEstqRepIcmsRedValDesc = 0;
                        itemMovEstq.ItMovEstqCalcSTPrecoLista = "Não";
                        itemMovEstq.ItMovEstqMargLucroST = 0;
                        itemMovEstq.ItMovEstqPrecoListaST = 0;
                        itemMovEstq.ItMovEstqPercRedIcmsST = 0;
                        itemMovEstq.ItMovEstqValBaseIcmsST = 0;
                        itemMovEstq.ItMovEstqPercIcmsST = 0;

                        itemMovEstq.ItMovEstqQtdBemPat = 1;
                        //itemMovEstq.TribBModBCCod = "3";
                        itemMovEstq.ItMovEstqConfTribTipoIpi = "IPI";
                        itemMovEstq.ItMovEstqConfTribTipoPis = "PIS";
                        itemMovEstq.ItMovEstqConfTribTipoCofins = "COFINS";
                        itemMovEstq.ItMovEstqConfTribIpiCod = "02";
                        itemMovEstq.ItMovEstqRedCOFINS = "Nenhum";
                        itemMovEstq.ItMovEstqRedPIS = "Nenhum";

                        itemMovEstq.ItMovEstq = "Não";

                        itemMovEstq.ItMovEstqBaseCustoMed = 0;
                        itemMovEstq.ItMovEstqValBaseIrrf = 0;
                        itemMovEstq.ItMovEstqCustoUnitSegIndEcon = 0;
                        itemMovEstq.ItMovEstqBaseCMedSegIndEcon = 0;
                        itemMovEstq.ItMovEstqValII = 0;
                        itemMovEstq.ItMovEstqValIcmsST = 0;
                        itemMovEstq.ItMovEstqValEmbalagemST = 0;
                        itemMovEstq.ItMovEstqValIcmsEmbalagemST = 0;
                        itemMovEstq.ItMovEstqValFreteST = 0;
                        itemMovEstq.ItMovEstqValIcmsFreteST = 0;
                        itemMovEstq.ItMovEstqValSeguroST = 0;
                        itemMovEstq.ItMovEstqValIcmsSeguroST = 0;
                        itemMovEstq.ItMovEstqValDespesaST = 0;
                        itemMovEstq.ItMovEstqValIcmsDespesaST = 0;
                        itemMovEstq.ItMovEstqValCalcFreteST = 0;
                        itemMovEstq.ItMovEstqValIcmsCalcFreteST = 0;
                        itemMovEstq.ItMovEstqPesoLiq = 0;
                        itemMovEstq.ItMovEstqPesoBruto = 0;
                        itemMovEstq.ItMovEstqCustoUnitLiq = 0;
                        itemMovEstq.ItMovEstqPercRedBaseInss = 0;
                        itemMovEstq.ItMovEstqValRedBaseInss = 0;
                        itemMovEstq.ItMovEstqBaseInssRed = 0;
                        itemMovEstq.ItMovEstqValRecAntIcmsST = 0;
                        itemMovEstq.ItMovEstqPercRedII = 0;
                        itemMovEstq.ItMovEstqValRedII = 0;
                        itemMovEstq.ItMovEstqBaseIIRed = 0;
                        itemMovEstq.ItMovEstqValIIOrig = 0;
                        itemMovEstq.ItMovEstqPercRedIPI = 0;
                        itemMovEstq.ItMovEstqValRedIPI = 0;
                        itemMovEstq.ItMovEstqValIPIOrig = 0;
                        itemMovEstq.ItMovEstqPercRedISS = 0;
                        itemMovEstq.ItMovEstqValRedISS = 0;
                        itemMovEstq.ItMovEstqBaseISSRed = 0;
                        itemMovEstq.ItMovEstqValISSOrig = 0;
                        itemMovEstq.ItMovEstqValINSSOrig = 0;
                        itemMovEstq.ItMovEstqValDescIcms = 0;
                        itemMovEstq.ItMovEstqValReembolso = 0;
                        itemMovEstq.ItMovEstqPercDiferimICMS = 0;
                        itemMovEstq.ItMovEstqValDiferimIcms = 0;
                        itemMovEstq.ItMovEstqValICMSDevido = 0;
                        itemMovEstq.ItMovEstqValCredPresumICMS = 0;
                        itemMovEstq.ItMovEstqValICMSRecolher = 0;
                        itemMovEstq.ItMovEstqQtdProdST = 0;
                        itemMovEstq.ItMovEstqQtdCalcProdST = 0;
                        itemMovEstq.ItMovEstqValGlosa = 0;
                        itemMovEstq.ItMovEstqPercIcmsExonerado = 0;
                        itemMovEstq.ItMovEstqValBaseIcmsOper = 0;
                        itemMovEstq.ItMovEstqPercIcmsOper = 0;
                        itemMovEstq.ItMovEstqValIcmsOper = 0;
                        itemMovEstq.ItMovEstqPrecoVendaVarejo = 0;
                        itemMovEstq.ItMovEstqValIcmsRetSTRecX = 0;
                        itemMovEstq.ItMovEstqValIcmsRecX = 0;
                        itemMovEstq.ItMovEstqBaseIcmsX = 0;
                        itemMovEstq.ItMovEstqQtdBaseIpiPauta = 0;
                        itemMovEstq.ItMovEstqValUnitIpiPauta = 0;
                        itemMovEstq.ItMovEstqQtdBasePisPauta = 0;
                        itemMovEstq.ItMovEstqValUnitPisPauta = 0;
                        itemMovEstq.ItMovEstqQtdBaseCofinsPauta = 0;
                        itemMovEstq.ItMovEstqValUnitCofinsPauta = 0;
                        itemMovEstq.ItMovEstqQtdRed = 0;
                        itemMovEstq.ItMovEstqQtdCalcRed = 0;
                        itemMovEstq.ItMovEstqQtdDesmCalc = 0;
                        itemMovEstq.ItMovEstqBaseFunrural = 0;
                        itemMovEstq.ItMovEstqPercFunrural = 0;
                        itemMovEstq.ItMovEstqValFunrural = 0;
                        itemMovEstq.ItMovEstqBaseSegCustoMed = 0;
                        itemMovEstq.ItMovEstqSegCustoUnit = 0;
                        itemMovEstq.ItMovEstqSegCustoUnitSegInd = 0;
                        itemMovEstq.ItMovEstqBaseSegCUnitSegInd = 0;
                        itemMovEstq.ItMovEstqPesoCubado = 0;
                        itemMovEstq.ItMovEstqValDedISS = 0;
                        itemMovEstq.ItMovEstqBaseISSDeduz = 0;
                        itemMovEstq.ItMovEstqValUnitProd = 0;
                        itemMovEstq.ItMovEstqValBasePisRF = 0;
                        itemMovEstq.ItMovEstqValBaseCofinsRF = 0;
                        itemMovEstq.ItMovEstqValDescIcmsZFM = 0;
                        itemMovEstq.ItMovEstqValDescPisZFM = 0;
                        itemMovEstq.ItMovEstqValDescCofinsZFM = 0;
                        itemMovEstq.ItMovEstqBaseIcmsRedDest = 0;
                        itemMovEstq.ItMovEstqPercRedBaseIcmsDest = 0;
                        itemMovEstq.ItMovEstqMargLucroSTDemo = 0;
                        itemMovEstq.ItMovEstqPrecoListaSTDemo = 0;
                        itemMovEstq.ItMovEstqValBaseIcmsSTDemo = 0;
                        itemMovEstq.ItMovEstqPercIcmsSTDemo = 0;
                        itemMovEstq.ItMovEstqValIcmsStDemo = 0;
                        itemMovEstq.ItMovEstqValIcmsRetSTDemo = 0;
                        itemMovEstq.ITMOVESTQQTDRECINTSIS = 0;
                        itemMovEstq.ItMovEstqTxaMarMerc = 0;
                        itemMovEstq.ItMovEstqValIcmsTxaMarMerc = 0;
                        itemMovEstq.ItMovEstqQtdFCI = 0;
                        itemMovEstq.ItMovEstqQtdCalcFCI = 0;

                        itemMovEstq.ItMovEstqSeqNF = 1;

                        #endregion

                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                        #endregion

                        #region LOC_ARMAZ_ITEM_MOV_ESTQ

                        LOC_ARMAZ_ITEM_MOV_ESTQ locaArmaz = servico.InsereLocalArmazenagem(itemMovEstq.MovEstqChv,
                            itemMovEstq.EmpCod, sequencia, itemMovEstq.ProdCodEstr, quantidade, quantidade, locArmaz);

                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locaArmaz);

                        #endregion

                        #region PARC_PAG_MOV_ESTQ

                        PARC_PAG_MOV_ESTQ parcela = new PARC_PAG_MOV_ESTQ();
                        parcela.EmpCod = movestq.EmpCod;
                        parcela.MovEstqChv = movestq.MovEstqChv;
                        parcela.ParcPagMovEstqSeq = 1;
                        parcela.ParcPagMovEstqEspec = movestq.MovEstqDocEspec;
                        parcela.ParcPagMovEstqSerie = movestq.MovEstqDocSerie;
                        parcela.ParcPagMovEstqNum = movestq.MovEstqDocNum + "-A";
                        parcela.ParcPagMovEstqDataEmissao = movestq.MovEstqDataEmissao;
                        parcela.ParcPagMovEstqVal = movestq.MovEstqValMerc;
                        parcela.ParcPagMovEstqDataVenc = CalculaVencimento(dataEmissao, movestq.EntCod);
                        parcela.ParcPagMovEstqValPag = 0;
                        parcela.ParcPagMovEstqDataProrrog = parcela.ParcPagMovEstqDataVenc;

                        bdApolo.PARC_PAG_MOV_ESTQ.AddObject(parcela);

                        #endregion

                        #region RATEIO_MOV_ESTQ

                        if (nfe != null)
                        {
                            #region Se NF-e de origem for Saída, insere de acordo com as configurações acima

                            RATEIO_MOV_ESTQ rateioME = new RATEIO_MOV_ESTQ();
                            rateioME.EmpCod = movestq.EmpCod;
                            rateioME.MovEstqChv = movestq.MovEstqChv;
                            rateioME.ClasseRecDespCodEstr = conta;
                            rateioME.CCtrlCodEstr = rateio;
                            rateioME.RatMovEstqVal = valor;
                            rateioME.RatMovEstqPerc = 100;

                            bdApolo.RATEIO_MOV_ESTQ.AddObject(rateioME);

                            #endregion
                        }
                        else
                        {
                            #region Se NF-e de origem for Entrada, copia o rateio da nota de origem

                            var listaRateioOrigemEntrada = bdApolo.RATEIO_MOV_ESTQ
                                .Where(w => w.EmpCod == nfeEntrada.EmpCod
                                    && w.MovEstqChv == nfeEntrada.MovEstqChv).ToList();

                            foreach (var rateioOrigemEntrada in listaRateioOrigemEntrada)
                            {
                                RATEIO_MOV_ESTQ rateioME = new RATEIO_MOV_ESTQ();
                                rateioME.EmpCod = movestq.EmpCod;
                                rateioME.MovEstqChv = movestq.MovEstqChv;
                                rateioME.ClasseRecDespCodEstr = conta;
                                rateioME.CCtrlCodEstr = rateioOrigemEntrada.CCtrlCodEstr;
                                rateioME.RatMovEstqPerc = rateioOrigemEntrada.RatMovEstqPerc;
                                rateioME.RatMovEstqVal = valor * (rateioME.RatMovEstqPerc / 100.00m);

                                bdApolo.RATEIO_MOV_ESTQ.AddObject(rateioME);
                            }

                            #endregion
                        }

                        #endregion

                        #region MOV_ESTQ_CLASSE_REC_DESP

                        MOV_ESTQ_CLASSE_REC_DESP movEstqCRD = new MOV_ESTQ_CLASSE_REC_DESP();
                        movEstqCRD.EmpCod = movestq.EmpCod;
                        movEstqCRD.MovEstqChv = movestq.MovEstqChv;
                        movEstqCRD.ClasseRecDespCodEstr = conta;
                        movEstqCRD.MovEstqClasseRecDespVal = valor;
                        movEstqCRD.MovEstqClasseRecDespPerc = 100;

                        bdApolo.MOV_ESTQ_CLASSE_REC_DESP.AddObject(movEstqCRD);

                        #endregion

                        #region MOV_ESTQ_DOC_COMPLEM - Vínculo das Notas Fiscais para do CT-e

                        foreach (var item in listaNFes)
                        {
                            item.EmpCod = movestq.EmpCod;
                            item.MovEstqChv = movestq.MovEstqChv;

                            bdApolo.MOV_ESTQ_DOC_COMPLEM.AddObject(item);
                        }

                        #endregion

                        #endregion

                        #region Insere LOG_MOV_ESTQ

                        LOG_MOV_ESTQ logMovEstq = new LOG_MOV_ESTQ();

                        ObjectParameter chave = new ObjectParameter("codigo", typeof(global::System.String));
                        Apolo10EntitiesService apoloI = new Apolo10EntitiesService();
                        apoloI.gerar_codigo(movestq.EmpCod, "LOG_MOV_ESTQ", chave);

                        logMovEstq.LogMovEstqSeq = Convert.ToInt32(chave.Value);
                        logMovEstq.LogMovEstqUsuCod = movestq.UsuCod;
                        logMovEstq.LogMovEstqDataHora = DateTime.Now;
                        logMovEstq.LogMovEstqEmpCod = movestq.EmpCod;
                        logMovEstq.LogMovEstqChv = movestq.MovEstqChv;
                        logMovEstq.LogMovEstqOper = "Inclusão";
                        logMovEstq.LogMovEstqDocEspec = movestq.MovEstqDocEspec;
                        logMovEstq.LogMovEstqDocSerie = movestq.MovEstqDocSerie;
                        logMovEstq.LogMovEstqDocNum = movestq.MovEstqDocNum;
                        logMovEstq.LogMovEstqObs = movestq.MovEstqObs;

                        bdApolo.LOG_MOV_ESTQ.AddObject(logMovEstq);

                        #endregion

                        #region Carrega Dados da Movimentação Gerada

                        movEstqChv = movestq.MovEstqChv;
                        empresaStr = movestq.EmpCod;

                        bdApolo.SaveChanges();

                        #endregion

                        #region Integra com Financeiro

                        bdApolo.integ_estoque_financ_ins(movestq.MovEstqChv, movestq.EmpCod);

                        #endregion

                        #region Integra com o Fiscal

                        ObjectParameter empP = new ObjectParameter("empcod", movestq.EmpCod);
                        ObjectParameter msg = new ObjectParameter("msg", "");
                        bdApolo.INTEG_ESTQ_FISCAL(empP, movestq.MovEstqChv, usuario, msg);

                        if (msg.Value != "")
                        {
                            msgRetorno = "Integração Fiscal não pode ser feita na movimentação  "
                                + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                                + msg.Value;
                            //ViewBag.Erro = "Integração Fiscal não pode ser feita na movimentação  "
                            //    + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            //    + msg.Value;
                            //retorno.ID = 0;
                            //retorno.Importado = msgRetorno;
                            //return View("Arquivo", retorno);
                            return msgRetorno;
                        }

                        #endregion

                        #region Integra com o Contábil

                        ObjectParameter vContaBloqueada = new ObjectParameter("vContaBloqueada", "");
                        ObjectParameter vMensagem = new ObjectParameter("vMensagem", "");
                        ObjectParameter vValorDebCredInv = new ObjectParameter("vValorDebCredInv", "");
                        ObjectParameter vStatus = new ObjectParameter("vStatus", "");
                        ObjectParameter vAnoMesRelac = new ObjectParameter("vAnoMesRelac", "");
                        ObjectParameter vSequenciaRelac = new ObjectParameter("vSequenciaRelac", 0);
                        /*
                            * 03/12/2018 - Trocada a procedure da integração de acordo com as atualizações da Riosoft. 
                        */

                        //bdApolo.VERIFICAR_LANC_CONTABIL(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
                        //    movestq.MovEstqDocEspec, movestq.MovEstqDocSerie, movestq.MovEstqDocNum, movestq.MovEstqChv,
                        //    0, 0, vContaBloqueada, vMensagem, vValorDebCredInv, vStatus, "", "",
                        //    vAnoMesRelac, vSequenciaRelac);

                        bdApolo.INTEGRAR_LANC_CONTAB(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
                            movestq.MovEstqDocEspec, movestq.MovEstqDocSerie, movestq.MovEstqDocNum, movestq.MovEstqChv,
                            0, 0, vContaBloqueada, vMensagem, vValorDebCredInv, vStatus, "", "",
                            vAnoMesRelac, vSequenciaRelac);

                        if (vMensagem.Value != "")
                        {
                            msgRetorno = "Integração Contábil não pode ser feita na movimentação "
                                + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                                + vMensagem.Value;
                            //ViewBag.Erro = "Integração Contábil não pode ser feita na movimentação " 
                            //    + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            //    + vMensagem.Value;
                            //retorno.ID = 0;
                            //retorno.Importado = msgRetorno;
                            //return View("Arquivo", retorno);
                            return msgRetorno;
                        }

                        #endregion
                    }
                    else
                    {
                        msgRetorno = "<h4 style='color: Red;'>NF-e " + chaveNFE + " não cadastrada no Apolo! Verifique!</h4>";
                        return msgRetorno;
                        //ViewBag.Erro = msgRetorno;
                        //retorno.ID = 0;
                        //retorno.Importado = msgRetorno;
                        //return View("Arquivo", retorno);
                    }

                    #endregion
                }
                else if (chaveCTEComp != "")
                {
                    #region CT-e Complementar

                    #region Verifica se o CT-e Principal foi digitado no Apolo

                    MOV_ESTQ_NF_ELETRONICA cteCompMov = bdApolo.MOV_ESTQ_NF_ELETRONICA
                        .Where(w => w.MovEstqNFEImpChvAcesso == chaveCTEComp).FirstOrDefault();

                    if (cteCompMov == null)
                    {
                        msgRetorno = "CT-e Principal " + chaveCTEComp + " não cadastrado no Apolo! "
                            + "Para inserir complemento é necessário o principal inserido!";
                        //ViewBag.Erro = msgRetorno;
                        //retorno.ID = 0;
                        //retorno.Importado = msgRetorno;
                        //return View("Arquivo", retorno);
                        return msgRetorno;
                    }

                    #endregion

                    #region Insere Movimentação de Estoque no Apolo

                    MOV_ESTQ movestqCtePrincipal = bdApolo.MOV_ESTQ.Where(w => w.EmpCod == cteCompMov.EmpCod
                        && w.MovEstqChv == cteCompMov.MovEstqChv).FirstOrDefault();

                    //valBaseICMS = Convert.ToDecimal(movestqCtePrincipal.MovEstqBaseIcms);
                    //valICMS = Convert.ToDecimal(movestqCtePrincipal.MovEstqValIcms);

                    MOV_ESTQ movestq = servico.InsereMovEstq(empresa.EmpCod, tipoLanc, movestqCtePrincipal.EntCod,
                        dataEmissao, usuario);

                    if (data != null && data != "")
                    {
                        movestq.MovEstqDataMovimento = Convert.ToDateTime(data);
                        movestq.MovEstqDataEntrada = movestq.MovEstqDataMovimento;
                    }

                    movestq.MovEstqDocEmpCod = movestq.EmpCod;
                    movestq.MovEstqDocEspec = especie;
                    movestq.MovEstqDocSerie = serie;
                    movestq.MovEstqDocNum = numCTE;
                    movestq.CondPagCod = condPag;
                    movestq.MovEstqValMerc = valor;
                    movestq.MovEstqBaseIcms = valBaseICMS;
                    movestq.MovEstqValIcms = valICMS;
                    movestq.MovEstqValFinalMerc = valor;
                    movestq.MovEstqValDoc = valor;
                    movestq.MovEstqValLib = valor;
                    movestq.MovEstqValOrig = valor;
                    movestq.MovEstqValBasePis = valBasePisCofins;
                    movestq.MovEstqValBaseCofins = valBasePisCofins;
                    movestq.MovEstqValDocLiq = valor;
                    //movestq.MovEstqObs = "XML Importado via Sistema WEB";
                    movestq.MovEstqObs = finalidade;
                    movestq.MovEstqPercDescGer = 0;
                    movestq.MovEstqPercDescGerProd = 0;
                    movestq.MovEstqPercDescGerServ = 0;
                    movestq.MovEstqValDescGer = 0;
                    movestq.MovEstqValDescGerProd = 0;
                    movestq.MovEstqValDescGerServ = 0;
                    movestq.MovEstqCidCodOrig = codCidadeRemetente;
                    movestq.MovEstqCidCodDest = codCidadeDestinatario;
                    movestq.CidCodInicPrestServ = codCidadeRemetente;
                    movestq.CidCodFinalPrestServ = codCidadeDestinatario;
                    movestq.USERChaveCTePrincipal = chaveCTEComp;
                    movestq.MovEstqGeraFiscal = "Sim";

                    movestq.MovEstq = "Não";

                    movestq.MovEstqNFEmisProp = "Não";

                    movestq.MovEstqIndTipoFrete = tipoFrete;

                    bdApolo.MOV_ESTQ.AddObject(movestq);

                    MOV_ESTQ_NF_ELETRONICA chaveMov = new MOV_ESTQ_NF_ELETRONICA();
                    chaveMov.EmpCod = movestq.EmpCod;
                    chaveMov.MovEstqChv = movestq.MovEstqChv;
                    chaveMov.UsuCod = usuario;
                    chaveMov.MovEstqNFEImpData = movestq.MovEstqDataMovimento;
                    chaveMov.MovEstqNFEImpChvAcesso = chaveCTE;
                    chaveMov.MovEstqNFEImpStatus = "Importado";

                    bdApolo.MOV_ESTQ_NF_ELETRONICA.AddObject(chaveMov);

                    ITEM_MOV_ESTQ itemCtePrincipal = bdApolo.ITEM_MOV_ESTQ
                        .Where(w => w.EmpCod == movestqCtePrincipal.EmpCod
                            && w.MovEstqChv == movestqCtePrincipal.MovEstqChv).FirstOrDefault();

                    percICMS = Convert.ToDecimal(itemCtePrincipal.ItMovEstqPercIcms);
                    tribCod = itemCtePrincipal.TribCod;
                    tribBICMS = itemCtePrincipal.TribBCod;

                    ITEM_MOV_ESTQ itemMovEstq = servico.InsereItemMovEstq(movestq.MovEstqChv, movestq.EmpCod,
                            movestq.TipoLancCod, movestq.EntCod, movestq.MovEstqDataMovimento, produto,
                            itemCtePrincipal.NatOpCodEstr,
                            quantidade, valor, unidMed, posicaoUnidMed, tribCod, ncm, clasFiscal);

                    itemMovEstq.ItMovEstqPercIcms = percICMS;
                    itemMovEstq.ItMovEstqBaseIcms = valBaseICMS;
                    itemMovEstq.ItMovEstqValIcms = valICMS;
                    itemMovEstq.ItMovEstqValIcmsRec = valICMS;
                    itemMovEstq.ItMovEstqValICMSOrig = valICMS;
                    itemMovEstq.ItMovEstqConfTribPisCod = pisCofinsTributacao;
                    itemMovEstq.ItMovEstqConfTribCofinsCod = pisCofinsTributacao;
                    itemMovEstq.ItMovEstqValBasePis = valBasePisCofins;
                    itemMovEstq.ItMovEstqValBaseCofins = valBasePisCofins;
                    itemMovEstq.ItMovEstqPercPis = percPis;
                    itemMovEstq.ItMovEstqPercCofins = percCofins;
                    itemMovEstq.ItMovEstqValPis = valPis;
                    itemMovEstq.ItMovEstqValPisRec = valPis;
                    itemMovEstq.ItMovEstqValPISOrig = valPis;
                    itemMovEstq.ItMovEstqValCofins = valCofins;
                    itemMovEstq.ItMovEstqValCofinsRec = valCofins;
                    itemMovEstq.ItMovEstqValCOFINSOrig = valCofins;
                    itemMovEstq.ItMovEstqCustoUnit = valor;
                    itemMovEstq.ItMovEstqValProd = valor;

                    #region Inicializa Valores Item_Mov_Estq

                    itemMovEstq.ItMovEstqValAcrescFin = 0;
                    itemMovEstq.ItMovEstqValDescEspec = 0;
                    itemMovEstq.ItMovEstqValDespDiv = 0;
                    itemMovEstq.ItMovEstqValDescGer = 0;
                    itemMovEstq.ItMovEstqPercAcrescFin = 0;
                    itemMovEstq.ItMovEstqPercDescEspec = 0;
                    itemMovEstq.ItMovEstqValEmbalagem = 0;
                    itemMovEstq.ItMovEstqValFrete = 0;
                    itemMovEstq.ItMovEstqValSeguro = 0;
                    itemMovEstq.ItMovEstqValOutra = 0;
                    itemMovEstq.ItMovEstqValServ = 0;
                    itemMovEstq.ItMovEstqPercRedBaseIcms = 0;
                    itemMovEstq.ItMovEstqValRedBaseIcms = 0;
                    itemMovEstq.ItMovEstqBaseIcmsRed = valBaseICMS;
                    itemMovEstq.ItMovEstqST = "F";
                    itemMovEstq.ItMovEstqValIcmsRetST = 0;
                    itemMovEstq.ItMovEstqValIcmsRetSTRec = 0;
                    itemMovEstq.ItMovEstqBaseIpi = 0;
                    itemMovEstq.ItMovEstqPercIpi = 0;
                    itemMovEstq.ItMovEstqValIpi = 0;
                    itemMovEstq.ItMovEstqIpiBaseIcms = "Não";
                    itemMovEstq.ItMovEstqValIpiRec = 0;
                    itemMovEstq.ItMovEstqValBaseIss = 0;
                    itemMovEstq.ItMovEstqPercIss = 0;
                    itemMovEstq.ItMovEstqValIss = 0;
                    itemMovEstq.ItMovEstqValIrrf = 0;
                    itemMovEstq.ItMovEstqPercIrrf = 0;
                    itemMovEstq.ItMovEstqValBaseInss = 0;
                    itemMovEstq.ItMovEstqPercInss = 0;
                    itemMovEstq.ItMovEstqValInss = 0;
                    itemMovEstq.ItMovEstqQtdUnidMed = 0;
                    itemMovEstq.ItMovEstqCredIpiCompraCom = "Não";
                    itemMovEstq.ItMovEstqCredIpiCompraComPerc = 0;
                    itemMovEstq.ItMovEstqCalcDifIcms = "Não";
                    itemMovEstq.ItMovEstqPercDifIcms = 0;
                    itemMovEstq.ItMovEstqQtdDesm = 0;
                    itemMovEstq.ItMovEstqRejPat = "Não";
                    itemMovEstq.ItMovEstqBaseII = 0;
                    itemMovEstq.ItMovEstqPercII = 0;
                    itemMovEstq.ItMovEstqValIIRec = 0;
                    itemMovEstq.ItMovEstqValProdDoc = 0;
                    itemMovEstq.ItMovEstqFreteDocOrigVal = 0;
                    itemMovEstq.ItMovEstqSegDocOrigVal = 0;
                    itemMovEstq.ItMovEstqOutraDespDocOrigVal = 0;
                    itemMovEstq.ItMovEstqValProdFOB = 0;
                    itemMovEstq.ItMovEstqValSiscomex = 0;
                    itemMovEstq.ItMovEstqValCalcIssDedTot = 0;
                    itemMovEstq.ItMovEstqValDifIcms = 0;
                    itemMovEstq.ItMovEstqValBaseCsll = 0;
                    itemMovEstq.ItMovEstqPercCsllRF = 0;
                    itemMovEstq.ItMovEstqValCsllRF = 0;
                    itemMovEstq.ItMovEstqPercCofinsRF = 0;
                    itemMovEstq.ItMovEstqValCofinsRF = 0;
                    itemMovEstq.ItMovEstqPercPisRF = 0;
                    itemMovEstq.ItMovEstqValPisRF = 0;
                    itemMovEstq.ItMovEstqValAcrescCustoComp = 0;
                    itemMovEstq.ItMovEstqValDescCustoComp = 0;
                    itemMovEstq.ItMovEstqPercRedBasePis = 0;
                    itemMovEstq.ItMovEstqValRedBasePis = 0;
                    itemMovEstq.ItMovEstqBasePisRed = valBasePisCofins;
                    itemMovEstq.ItMovEstqPercRedBaseCofins = 0;
                    itemMovEstq.ItMovEstqValRedBaseCofins = 0;
                    itemMovEstq.ItMovEstqBaseCofinsRed = valBasePisCofins;
                    itemMovEstq.ItMovEstqPat = "Não";
                    itemMovEstq.ItMovEstqValDescGer = 0;
                    itemMovEstq.ItMovEstqRepIcmsDifPercDesc = 0;
                    itemMovEstq.ItMovEstqRepIcmsDifValDesc = 0;
                    itemMovEstq.ItMovEstqRepIcmsRedValDesc = 0;
                    itemMovEstq.ItMovEstqCalcSTPrecoLista = "Não";
                    itemMovEstq.ItMovEstqMargLucroST = 0;
                    itemMovEstq.ItMovEstqPrecoListaST = 0;
                    itemMovEstq.ItMovEstqPercRedIcmsST = 0;
                    itemMovEstq.ItMovEstqValBaseIcmsST = 0;
                    itemMovEstq.ItMovEstqPercIcmsST = 0;

                    itemMovEstq.ItMovEstqQtdBemPat = 1;
                    //itemMovEstq.TribBModBCCod = "3";
                    itemMovEstq.ItMovEstqConfTribTipoIpi = "IPI";
                    itemMovEstq.ItMovEstqConfTribTipoPis = "PIS";
                    itemMovEstq.ItMovEstqConfTribTipoCofins = "COFINS";
                    itemMovEstq.ItMovEstqConfTribIpiCod = "02";
                    itemMovEstq.ItMovEstqRedCOFINS = "Nenhum";
                    itemMovEstq.ItMovEstqRedPIS = "Nenhum";

                    itemMovEstq.ItMovEstq = "Não";

                    itemMovEstq.ItMovEstqBaseCustoMed = 0;
                    itemMovEstq.ItMovEstqValBaseIrrf = 0;
                    itemMovEstq.ItMovEstqCustoUnitSegIndEcon = 0;
                    itemMovEstq.ItMovEstqBaseCMedSegIndEcon = 0;
                    itemMovEstq.ItMovEstqValII = 0;
                    itemMovEstq.ItMovEstqValIcmsST = 0;
                    itemMovEstq.ItMovEstqValEmbalagemST = 0;
                    itemMovEstq.ItMovEstqValIcmsEmbalagemST = 0;
                    itemMovEstq.ItMovEstqValFreteST = 0;
                    itemMovEstq.ItMovEstqValIcmsFreteST = 0;
                    itemMovEstq.ItMovEstqValSeguroST = 0;
                    itemMovEstq.ItMovEstqValIcmsSeguroST = 0;
                    itemMovEstq.ItMovEstqValDespesaST = 0;
                    itemMovEstq.ItMovEstqValIcmsDespesaST = 0;
                    itemMovEstq.ItMovEstqValCalcFreteST = 0;
                    itemMovEstq.ItMovEstqValIcmsCalcFreteST = 0;
                    itemMovEstq.ItMovEstqPesoLiq = 0;
                    itemMovEstq.ItMovEstqPesoBruto = 0;
                    itemMovEstq.ItMovEstqCustoUnitLiq = 0;
                    itemMovEstq.ItMovEstqPercRedBaseInss = 0;
                    itemMovEstq.ItMovEstqValRedBaseInss = 0;
                    itemMovEstq.ItMovEstqBaseInssRed = 0;
                    itemMovEstq.ItMovEstqValRecAntIcmsST = 0;
                    itemMovEstq.ItMovEstqPercRedII = 0;
                    itemMovEstq.ItMovEstqValRedII = 0;
                    itemMovEstq.ItMovEstqBaseIIRed = 0;
                    itemMovEstq.ItMovEstqValIIOrig = 0;
                    itemMovEstq.ItMovEstqPercRedIPI = 0;
                    itemMovEstq.ItMovEstqValRedIPI = 0;
                    itemMovEstq.ItMovEstqValIPIOrig = 0;
                    itemMovEstq.ItMovEstqPercRedISS = 0;
                    itemMovEstq.ItMovEstqValRedISS = 0;
                    itemMovEstq.ItMovEstqBaseISSRed = 0;
                    itemMovEstq.ItMovEstqValISSOrig = 0;
                    itemMovEstq.ItMovEstqValINSSOrig = 0;
                    itemMovEstq.ItMovEstqValDescIcms = 0;
                    itemMovEstq.ItMovEstqValReembolso = 0;
                    itemMovEstq.ItMovEstqPercDiferimICMS = 0;
                    itemMovEstq.ItMovEstqValDiferimIcms = 0;
                    itemMovEstq.ItMovEstqValICMSDevido = 0;
                    itemMovEstq.ItMovEstqValCredPresumICMS = 0;
                    itemMovEstq.ItMovEstqValICMSRecolher = 0;
                    itemMovEstq.ItMovEstqQtdProdST = 0;
                    itemMovEstq.ItMovEstqQtdCalcProdST = 0;
                    itemMovEstq.ItMovEstqValGlosa = 0;
                    itemMovEstq.ItMovEstqPercIcmsExonerado = 0;
                    itemMovEstq.ItMovEstqValBaseIcmsOper = 0;
                    itemMovEstq.ItMovEstqPercIcmsOper = 0;
                    itemMovEstq.ItMovEstqValIcmsOper = 0;
                    itemMovEstq.ItMovEstqPrecoVendaVarejo = 0;
                    itemMovEstq.ItMovEstqValIcmsRetSTRecX = 0;
                    itemMovEstq.ItMovEstqValIcmsRecX = 0;
                    itemMovEstq.ItMovEstqBaseIcmsX = 0;
                    itemMovEstq.ItMovEstqQtdBaseIpiPauta = 0;
                    itemMovEstq.ItMovEstqValUnitIpiPauta = 0;
                    itemMovEstq.ItMovEstqQtdBasePisPauta = 0;
                    itemMovEstq.ItMovEstqValUnitPisPauta = 0;
                    itemMovEstq.ItMovEstqQtdBaseCofinsPauta = 0;
                    itemMovEstq.ItMovEstqValUnitCofinsPauta = 0;
                    itemMovEstq.ItMovEstqQtdRed = 0;
                    itemMovEstq.ItMovEstqQtdCalcRed = 0;
                    itemMovEstq.ItMovEstqQtdDesmCalc = 0;
                    itemMovEstq.ItMovEstqBaseFunrural = 0;
                    itemMovEstq.ItMovEstqPercFunrural = 0;
                    itemMovEstq.ItMovEstqValFunrural = 0;
                    itemMovEstq.ItMovEstqBaseSegCustoMed = 0;
                    itemMovEstq.ItMovEstqSegCustoUnit = 0;
                    itemMovEstq.ItMovEstqSegCustoUnitSegInd = 0;
                    itemMovEstq.ItMovEstqBaseSegCUnitSegInd = 0;
                    itemMovEstq.ItMovEstqPesoCubado = 0;
                    itemMovEstq.ItMovEstqValDedISS = 0;
                    itemMovEstq.ItMovEstqBaseISSDeduz = 0;
                    itemMovEstq.ItMovEstqValUnitProd = 0;
                    itemMovEstq.ItMovEstqValBasePisRF = 0;
                    itemMovEstq.ItMovEstqValBaseCofinsRF = 0;
                    itemMovEstq.ItMovEstqValDescIcmsZFM = 0;
                    itemMovEstq.ItMovEstqValDescPisZFM = 0;
                    itemMovEstq.ItMovEstqValDescCofinsZFM = 0;
                    itemMovEstq.ItMovEstqBaseIcmsRedDest = 0;
                    itemMovEstq.ItMovEstqPercRedBaseIcmsDest = 0;
                    itemMovEstq.ItMovEstqMargLucroSTDemo = 0;
                    itemMovEstq.ItMovEstqPrecoListaSTDemo = 0;
                    itemMovEstq.ItMovEstqValBaseIcmsSTDemo = 0;
                    itemMovEstq.ItMovEstqPercIcmsSTDemo = 0;
                    itemMovEstq.ItMovEstqValIcmsStDemo = 0;
                    itemMovEstq.ItMovEstqValIcmsRetSTDemo = 0;
                    itemMovEstq.ITMOVESTQQTDRECINTSIS = 0;
                    itemMovEstq.ItMovEstqTxaMarMerc = 0;
                    itemMovEstq.ItMovEstqValIcmsTxaMarMerc = 0;
                    itemMovEstq.ItMovEstqQtdFCI = 0;
                    itemMovEstq.ItMovEstqQtdCalcFCI = 0;

                    itemMovEstq.ItMovEstqSeqNF = 1;

                    #endregion

                    bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                    LOC_ARMAZ_ITEM_MOV_ESTQ locPrin = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                        .Where(w => w.EmpCod == movestqCtePrincipal.EmpCod
                            && w.MovEstqChv == movestqCtePrincipal.MovEstqChv).FirstOrDefault();

                    LOC_ARMAZ_ITEM_MOV_ESTQ locaArmaz = servico.InsereLocalArmazenagem(itemMovEstq.MovEstqChv,
                        itemMovEstq.EmpCod, locPrin.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                        Convert.ToDecimal(locPrin.LocArmazItMovEstqQtd), Convert.ToDecimal(locPrin.LocArmazItMovEstqQtd), locPrin.LocArmazCodEstr);

                    bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locaArmaz);

                    PARC_PAG_MOV_ESTQ parcela = new PARC_PAG_MOV_ESTQ();
                    parcela.EmpCod = movestq.EmpCod;
                    parcela.MovEstqChv = movestq.MovEstqChv;
                    parcela.ParcPagMovEstqSeq = 1;
                    parcela.ParcPagMovEstqEspec = movestq.MovEstqDocEspec;
                    parcela.ParcPagMovEstqSerie = movestq.MovEstqDocSerie;
                    parcela.ParcPagMovEstqNum = movestq.MovEstqDocNum + "-A";
                    parcela.ParcPagMovEstqDataEmissao = movestq.MovEstqDataEmissao;
                    parcela.ParcPagMovEstqVal = movestq.MovEstqValMerc;
                    parcela.ParcPagMovEstqDataVenc =
                        CalculaVencimento(Convert.ToDateTime(movestq.MovEstqDataEmissao), movestq.EntCod);
                    parcela.ParcPagMovEstqValPag = 0;
                    parcela.ParcPagMovEstqDataProrrog = parcela.ParcPagMovEstqDataVenc;

                    bdApolo.PARC_PAG_MOV_ESTQ.AddObject(parcela);

                    RATEIO_MOV_ESTQ rateioPrin = bdApolo.RATEIO_MOV_ESTQ
                        .Where(w => w.EmpCod == movestqCtePrincipal.EmpCod
                            && w.MovEstqChv == movestqCtePrincipal.MovEstqChv).FirstOrDefault();

                    RATEIO_MOV_ESTQ rateioME = new RATEIO_MOV_ESTQ();
                    rateioME.EmpCod = movestq.EmpCod;
                    rateioME.MovEstqChv = movestq.MovEstqChv;
                    rateioME.ClasseRecDespCodEstr = rateioPrin.ClasseRecDespCodEstr;
                    rateioME.CCtrlCodEstr = rateioPrin.CCtrlCodEstr;
                    rateioME.RatMovEstqVal = valor;
                    rateioME.RatMovEstqPerc = rateioPrin.RatMovEstqPerc;

                    bdApolo.RATEIO_MOV_ESTQ.AddObject(rateioME);

                    MOV_ESTQ_CLASSE_REC_DESP movEstqCRD = new MOV_ESTQ_CLASSE_REC_DESP();
                    movEstqCRD.EmpCod = movestq.EmpCod;
                    movEstqCRD.MovEstqChv = movestq.MovEstqChv;
                    movEstqCRD.ClasseRecDespCodEstr = rateioPrin.ClasseRecDespCodEstr;
                    movEstqCRD.MovEstqClasseRecDespVal = valor;
                    movEstqCRD.MovEstqClasseRecDespPerc = rateioPrin.RatMovEstqPerc;

                    bdApolo.MOV_ESTQ_CLASSE_REC_DESP.AddObject(movEstqCRD);

                    #endregion

                    #region MOV_ESTQ_DOC_COMPLEM - Vínculo das Notas Fiscais para do CT-e

                    listaNFes = bdApolo.MOV_ESTQ_DOC_COMPLEM
                        .Where(w => w.EmpCod == movestqCtePrincipal.EmpCod
                            && w.MovEstqChv == movestqCtePrincipal.MovEstqChv).ToList();

                    foreach (var item in listaNFes)
                    {
                        // Cria objeto e insere na lista
                        MOV_ESTQ_DOC_COMPLEM movEstqDocComplem = new MOV_ESTQ_DOC_COMPLEM();
                        movEstqDocComplem.EmpCod = movestq.EmpCod;
                        movEstqDocComplem.MovEstqChv = movestq.MovEstqChv;
                        movEstqDocComplem.MovEstqDocComplemSeq = item.MovEstqDocComplemSeq;
                        movEstqDocComplem.MovEstqDocComplemEspec = item.MovEstqDocComplemEspec;
                        movEstqDocComplem.MovEstqDocComplemSerie = item.MovEstqDocComplemSerie;
                        movEstqDocComplem.MovEstqDocComplemNum = item.MovEstqDocComplemNum;
                        movEstqDocComplem.MovEstqDocComplemBaseII = item.MovEstqDocComplemBaseII;
                        movEstqDocComplem.MovEstqDocComplemValII = item.MovEstqDocComplemValII;
                        movEstqDocComplem.MovEstqDocComplemBaseIcms = item.MovEstqDocComplemBaseIcms;
                        movEstqDocComplem.MovEstqDocComplemValIcms = item.MovEstqDocComplemValIcms;
                        movEstqDocComplem.MovEstqDocComplemBaseIcmsST = item.MovEstqDocComplemBaseIcmsST;
                        movEstqDocComplem.MovEstqDocComplemValIcmsRetST = item.MovEstqDocComplemValIcmsRetST;
                        movEstqDocComplem.MovEstqDocComplemValMerc = item.MovEstqDocComplemValMerc;
                        movEstqDocComplem.MovEstqDocComplemValFinalMerc = item.MovEstqDocComplemValFinalMerc;
                        movEstqDocComplem.MovEstqDocComplemValFrete = item.MovEstqDocComplemValFrete;
                        movEstqDocComplem.MovEstqDocComplemValSeguro = item.MovEstqDocComplemValSeguro;
                        movEstqDocComplem.MovEstqDocComplemValOutra = item.MovEstqDocComplemValOutra;
                        movEstqDocComplem.MovEstqDocComplemBaseIpi = item.MovEstqDocComplemBaseIpi;
                        movEstqDocComplem.MovEstqDocComplemValIpi = item.MovEstqDocComplemValIpi;
                        movEstqDocComplem.MovEstqDocComplemValDoc = item.MovEstqDocComplemValDoc;
                        movEstqDocComplem.MovEstqDocComplemValSiscomex = item.MovEstqDocComplemValSiscomex;
                        movEstqDocComplem.MovEstqDocComplemBasePis = item.MovEstqDocComplemBasePis;
                        movEstqDocComplem.MovEstqDocComplemValPis = item.MovEstqDocComplemValPis;
                        movEstqDocComplem.MovEstqDocComplemBaseCofins = item.MovEstqDocComplemBaseCofins;
                        movEstqDocComplem.MovEstqDocComplemValCofins = item.MovEstqDocComplemValCofins;
                        movEstqDocComplem.MovEstqDocComplemValCap = item.MovEstqDocComplemValCap;
                        movEstqDocComplem.MovEstqDocComplemDataEmissao = item.MovEstqDocComplemDataEmissao;

                        bdApolo.MOV_ESTQ_DOC_COMPLEM.AddObject(movEstqDocComplem);
                    }

                    #endregion

                    #region Insere LOG_MOV_ESTQ

                    LOG_MOV_ESTQ logMovEstq = new LOG_MOV_ESTQ();

                    ObjectParameter chave = new ObjectParameter("codigo", typeof(global::System.String));
                    Apolo10EntitiesService apoloI = new Apolo10EntitiesService();
                    apoloI.gerar_codigo(movestq.EmpCod, "LOG_MOV_ESTQ", chave);

                    logMovEstq.LogMovEstqSeq = Convert.ToInt32(chave.Value);
                    logMovEstq.LogMovEstqUsuCod = movestq.UsuCod;
                    logMovEstq.LogMovEstqDataHora = DateTime.Now;
                    logMovEstq.LogMovEstqEmpCod = movestq.EmpCod;
                    logMovEstq.LogMovEstqChv = movestq.MovEstqChv;
                    logMovEstq.LogMovEstqOper = "Inclusão";
                    logMovEstq.LogMovEstqDocEspec = movestq.MovEstqDocEspec;
                    logMovEstq.LogMovEstqDocSerie = movestq.MovEstqDocSerie;
                    logMovEstq.LogMovEstqDocNum = movestq.MovEstqDocNum;
                    logMovEstq.LogMovEstqObs = movestq.MovEstqObs;

                    bdApolo.LOG_MOV_ESTQ.AddObject(logMovEstq);

                    #endregion

                    #region Carrega Dados da Movimentação Gerada

                    movEstqChv = movestq.MovEstqChv;
                    empresaStr = movestq.EmpCod;

                    bdApolo.SaveChanges();

                    #endregion

                    #region Integra com o Financeiro

                    bdApolo.integ_estoque_financ_ins(movestq.MovEstqChv, movestq.EmpCod);

                    #endregion

                    #region Integra com o Fiscal

                    ObjectParameter empP = new ObjectParameter("empcod", movestq.EmpCod);
                    ObjectParameter msg = new ObjectParameter("msg", "");
                    bdApolo.INTEG_ESTQ_FISCAL(empP, movestq.MovEstqChv, usuario, msg);

                    if (msg.Value != "")
                    {
                        msgRetorno = "Integração Fiscal não pode ser feita na movimentação  "
                            + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            + msg.Value;
                        //ViewBag.Erro = msgRetorno;
                        //retorno.ID = 0;
                        //retorno.Importado = msgRetorno;
                        //return View("Arquivo", retorno);
                        return msgRetorno;
                    }

                    #endregion

                    #region Integra com o Contábil

                    ObjectParameter vContaBloqueada = new ObjectParameter("vContaBloqueada", "");
                    ObjectParameter vMensagem = new ObjectParameter("vMensagem", "");
                    ObjectParameter vValorDebCredInv = new ObjectParameter("vValorDebCredInv", "");
                    ObjectParameter vStatus = new ObjectParameter("vStatus", "");
                    ObjectParameter vAnoMesRelac = new ObjectParameter("vAnoMesRelac", "");
                    ObjectParameter vSequenciaRelac = new ObjectParameter("vSequenciaRelac", 0);
                    /*
                     * 03/12/2018 - Trocada a procedure da integração de acordo com as atualizações da Riosoft. 
                     */

                    //bdApolo.VERIFICAR_LANC_CONTABIL(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
                    //    movestq.MovEstqDocEspec, movestq.MovEstqDocSerie, movestq.MovEstqDocNum, movestq.MovEstqChv,
                    //    0, 0, vContaBloqueada, vMensagem, vValorDebCredInv, vStatus, "", "",
                    //    vAnoMesRelac, vSequenciaRelac);

                    bdApolo.INTEGRAR_LANC_CONTAB(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
                        movestq.MovEstqDocEspec, movestq.MovEstqDocSerie, movestq.MovEstqDocNum, movestq.MovEstqChv,
                        0, 0, vContaBloqueada, vMensagem, vValorDebCredInv, vStatus, "", "",
                        vAnoMesRelac, vSequenciaRelac);

                    if (vMensagem.Value != "")
                    {
                        msgRetorno = "Integração Contábil não pode ser feita na movimentação "
                            + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            + vMensagem.Value;
                        //ViewBag.Erro = msgRetorno;
                        //retorno.ID = 0;
                        //retorno.Importado = msgRetorno;
                        //return View("Arquivo", retorno);
                        return msgRetorno;
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    msgRetorno = "Arquivo não pode ser importado! Verifique!";
                    //ViewBag.Erro = msgRetorno;
                    //retorno.ID = 0;
                    //retorno.Importado = msgRetorno;
                    //return View("Arquivo", retorno);
                    return msgRetorno;
                }

                return msgRetorno;

                #endregion
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                msgRetorno = "";

                if (ex.InnerException == null)
                    msgRetorno = "Erro ao importar arquivo: " + ex.Message;
                else
                    msgRetorno = "Erro ao importar arquivo: " + ex.Message 
                        + " / Erro Interno: " + ex.InnerException.Message;

                msgRetorno = msgRetorno + " / Linha Erro: " + linenum.ToString();

                //retorno.ID = 0;
                //retorno.Importado = msgRetorno;
                //return View("Arquivo", retorno);
                return msgRetorno;
    }
}

        #endregion

        #region Importa NF-e

        public ActionResult NFe()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            Session["ListaConfiguracaoNFe"] = CarregaListaConfiguracaoNFe();

            return View();
        }

        [HttpPost]
        //public ActionResult ImportaNFe(string data, string pedCompNum, string tipoNFe)
        public ActionResult ImportaNFe()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            HLBAPPEntities1 hlbpp = new HLBAPPEntities1();

            string msgRetorno = "";
            //MvcAppHyLinedoBrasil.Models.SequenciaLinha retorno = new MvcAppHyLinedoBrasil.Models.SequenciaLinha();
            //retorno.ID = 1;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase itemArq = Request.Files[i];
                //StreamReader arquivo = new StreamReader(Request.Files[0].InputStream);
                //int codTipoNFe =Convert.ToInt32(tipoNFe);

                string msg = "";

                StreamReader arquivo = new StreamReader(itemArq.InputStream);

                #region Carrega a Chave do XML

                string chaveNFE = "";
                XmlDocument oXML = new XmlDocument();
                oXML.Load(arquivo);

                XmlNode nfe = null;

                XmlNodeList nfeList = oXML.ChildNodes;

                foreach (XmlNode item in nfeList)
                {
                    if (item.Name.Equals("nfeProc"))
                        nfe = item.FirstChild.FirstChild;
                }

                if (nfe != null)
                    foreach (XmlAttribute infNFe in nfe.Attributes)
                        if (infNFe.Name.Equals("Id")) chaveNFE = infNFe.InnerText.Replace("NFe", "");

                arquivo.BaseStream.Position = 0;

                #endregion

                if (chaveNFE == "")
                {
                    #region Verifica se é NF-e

                    //retorno.ID = 0;
                    //retorno.Importado = "O XML " + Request.Files[0].FileName + " não é de NF-e! Verifique!"; ;
                    //return View("Arquivo", retorno);
                    msgRetorno = msgRetorno + "<h4 style='color: Red;'>O XML " + itemArq.FileName + " não é de NF-e! Verifique!</h4>";
                    
                    #endregion
                }
                else
                {
                    #region Localiza dados do Rececimento de Documentos

                    Recebimento_Documento recDoc = hlbpp.Recebimento_Documento
                        .Where(w => w.ChaveEletronica == chaveNFE).FirstOrDefault();

                    #endregion

                    if (recDoc != null)
                        msg = ImportaXMLNFe(Convert.ToDateTime(recDoc.DataEntrada).ToShortDateString(),
                            recDoc.NumeroPedidoCompra, arquivo, Convert.ToInt32(recDoc.IDConfigImportaNFe));
                    else
                        msg = "O XML com a chave " + chaveNFE + " não está cadastrada no Recebimento de Documentos!"
                            + " Realize o cadastro e importe novamente!";

                    if (msg != "")
                    {
                        //retorno.ID = 0;
                        //retorno.Importado = msgRetorno;
                        msgRetorno = msgRetorno + "<h4 style='color: Red;'>" + msg + "</h4>";
                    }
                    else
                        //retorno.Importado = "NF-e " + chaveNFE + " importada com sucesso!";
                        msgRetorno = msgRetorno + "<h4 style='color: Blue;'>NF-e " + chaveNFE + " importada com sucesso!</h4>";
                }
            }

            //return View("Arquivo", retorno);
            ViewBag.msg = msgRetorno;
            return View("NFe");
        }

        public string ImportaXMLNFe(string data, string pedCompNum, StreamReader arquivo, 
            int tipoNFe)
        {
            string msgRetorno = "";
            
            Apolo10EntitiesService bdApolo = new Apolo10EntitiesService();
            ImportaIncubacaoService servico = new ImportaIncubacaoService();
            Models.FinanceiroEntities apolo = new Models.FinanceiroEntities();
            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            try
            {
                #region Lê arquivo XML

                string chaveNFE = "";
                string cnpjEmitente = "";
                string serie = "";
                string numNFE = "";
                DateTime dataEmissao = new DateTime();
                string cnpjDestinatario = "";
                List<det> listaProd = new List<det>();
                decimal valorTotalXML = 0;

                XmlDocument oXML = new XmlDocument();
                oXML.Load(arquivo);

                XmlNode nfe = null;

                XmlNodeList nfeList = oXML.ChildNodes;

                foreach (XmlNode item in nfeList)
                {
                    if (item.Name.Equals("nfeProc"))
                        nfe = item.FirstChild.FirstChild;
                }

                if (nfe != null)
                {
                    foreach (XmlAttribute infNFe in nfe.Attributes)
                    {
                        if (infNFe.Name.Equals("Id")) chaveNFE = infNFe.InnerText.Replace("NFe", "");
                    }

                    XmlNodeList childNodes = nfe.ChildNodes;

                    foreach (XmlNode item in childNodes)
                    {
                        #region ide
                        if (item.Name.Equals("ide"))
                        {
                            foreach (XmlNode ide in item.ChildNodes)
                            {
                                if (ide.Name.Equals("serie")) serie = ide.InnerText;
                                if (ide.Name.Equals("nNF")) numNFE = ide.InnerText;
                                if (ide.Name.Equals("dhEmi")) dataEmissao =
                                    Convert.ToDateTime(Convert.ToDateTime(ide.InnerText).ToShortDateString());
                            }
                        }
                        #endregion

                        if (item.Name.Equals("emit")) cnpjEmitente = item.FirstChild.InnerText;

                        if (item.Name.Equals("dest")) cnpjDestinatario = item.FirstChild.InnerText;

                        #region det - Itens
                        if (item.Name.Contains("det"))
                        {
                            det itemProd = new det();

                            foreach (XmlNode det in item.ChildNodes)
                            {
                                #region prod
                                if (det.Name.Equals("prod"))
                                {
                                    foreach (XmlNode prod in det.ChildNodes)
                                    {
                                        if (prod.Name.Equals("NCM")) itemProd.NCM = prod.InnerText;
                                        if (prod.Name.Equals("xProd")) itemProd.Descricao = prod.InnerText;
                                        if (prod.Name.Equals("uCom")) itemProd.UnidadeMedida = prod.InnerText;
                                        if (prod.Name.Equals("qCom")) itemProd.Qtde = Convert.ToDecimal(prod.InnerText.Replace(".", ","));
                                        if (prod.Name.Equals("vUnCom")) itemProd.ValorUnitario = Convert.ToDecimal(prod.InnerText.Replace(".", ","));
                                        if (prod.Name.Equals("vProd")) itemProd.ValorTotalProduto = Convert.ToDecimal(prod.InnerText.Replace(".", ","));
                                    }
                                }
                                #endregion

                                #region imposto
                                if (det.Name.Equals("imposto"))
                                {
                                    foreach (XmlNode imposto in det.ChildNodes)
                                    {
                                        #region ICMS

                                        if (imposto.Name.Equals("ICMS"))
                                        {
                                            foreach (XmlNode icms in imposto.ChildNodes)
                                            {
                                                foreach (XmlNode icmsTrib in icms.ChildNodes)
                                                {
                                                    if (icmsTrib.Name.Equals("CST")) itemProd.IcmsCST = icmsTrib.InnerText;
                                                    if (icmsTrib.Name.Equals("vBC")) itemProd.IcmsBC = Convert.ToDecimal(icmsTrib.InnerText.Replace(".", ","));
                                                    if (icmsTrib.Name.Equals("pICMS")) itemProd.IcmsPerc = Convert.ToDecimal(icmsTrib.InnerText.Replace(".", ","));
                                                    if (icmsTrib.Name.Equals("vICMS")) itemProd.IcmsValor = Convert.ToDecimal(icmsTrib.InnerText.Replace(".", ","));
                                                }
                                            }
                                        }

                                        #endregion

                                        #region PIS

                                        if (imposto.Name.Equals("PIS"))
                                        {
                                            foreach (XmlNode pis in imposto.ChildNodes)
                                            {
                                                foreach (XmlNode pisTrib in pis.ChildNodes)
                                                {
                                                    if (pisTrib.Name.Equals("CST")) itemProd.PisCST = pisTrib.InnerText;
                                                    if (pisTrib.Name.Equals("vBC")) itemProd.PisBC = Convert.ToDecimal(pisTrib.InnerText.Replace(".", ","));
                                                    if (pisTrib.Name.Equals("pICMS")) itemProd.PisPerc = Convert.ToDecimal(pisTrib.InnerText.Replace(".", ","));
                                                    if (pisTrib.Name.Equals("vICMS")) itemProd.PisValor = Convert.ToDecimal(pisTrib.InnerText.Replace(".", ","));
                                                }
                                            }
                                        }

                                        #endregion

                                        #region COFINS

                                        if (imposto.Name.Equals("COFINS"))
                                        {
                                            foreach (XmlNode cofins in imposto.ChildNodes)
                                            {
                                                foreach (XmlNode cofinsTrib in cofins.ChildNodes)
                                                {
                                                    if (cofinsTrib.Name.Equals("CST")) itemProd.CofinsCST = cofinsTrib.InnerText;
                                                    if (cofinsTrib.Name.Equals("vBC")) itemProd.CofinsBC = Convert.ToDecimal(cofinsTrib.InnerText.Replace(".", ","));
                                                    if (cofinsTrib.Name.Equals("pICMS")) itemProd.CofinsPerc = Convert.ToDecimal(cofinsTrib.InnerText.Replace(".", ","));
                                                    if (cofinsTrib.Name.Equals("vICMS")) itemProd.CofinsValor = Convert.ToDecimal(cofinsTrib.InnerText.Replace(".", ","));
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                }
                                #endregion
                            }

                            listaProd.Add(itemProd);
                        }
                        #endregion

                        #region total
                        
                        if (item.Name.Equals("total"))
                        {
                            foreach (XmlNode iCMSTot in item.FirstChild.ChildNodes)
                            {
                                if (iCMSTot.Name.Equals("vNF")) valorTotalXML = Convert.ToDecimal(iCMSTot.InnerText.Replace(".", ","));
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                #region Carrega Configurações Tabela NF-e

                Configuracao_Importa_NFe tabelaConfigNFe = hlbapp.Configuracao_Importa_NFe
                    .Where(w => w.ID == tipoNFe).FirstOrDefault();

                #endregion

                #region Insere no Apolo

                #region Carrega Variaveis Constantes

                string tipoLanc = tabelaConfigNFe.TipoLancCod;
                string especie = "NF-e";
                string locArmaz = tabelaConfigNFe.LocArmazCod;
                string usuario = (Session["login"].ToString() == "palves" ? "RIOSOFT"
                    : Session["login"].ToString().ToUpper());

                Models.EMPRESA_FILIAL empresa = apolo.EMPRESA_FILIAL
                    .Where(w => w.EmpCpfCgc == cnpjDestinatario).FirstOrDefault();

                short sequencia = 1;

                #endregion

                #region Verifica Configurações

                #region Verifica se NF-e já foi inserida

                MOV_ESTQ_NF_ELETRONICA nfeMov = bdApolo.MOV_ESTQ_NF_ELETRONICA
                    .Where(w => w.MovEstqNFEImpChvAcesso == chaveNFE).FirstOrDefault();

                if (nfeMov != null)
                {
                    msgRetorno = "NF-e " + chaveNFE + " já cadastrada no Apolo! (Empresa "
                        + nfeMov.EmpCod + " - Chave " + nfeMov.MovEstqChv.ToString() + ")";
                    return msgRetorno;
                }

                #endregion

                #region Verifica se o fornecedor existe no Apolo

                ImportaIncubacao.Data.Apolo.ENTIDADE entidade = bdApolo.ENTIDADE.Where(w => w.EntCpfCgc == cnpjEmitente)
                    .FirstOrDefault();

                if (entidade == null)
                {
                    msgRetorno = "CNPJ " + cnpjEmitente + " do forncedor não cadastrado no Apolo! Verifique!";
                    return msgRetorno;
                }

                #endregion

                #region Carrega CFOP

                string cFOP = "1" + tabelaConfigNFe.ClasFiscCod;
                Models.CIDADE cidadeEmpresa = apolo.CIDADE.Where(w => w.CidCod == empresa.CidCod).FirstOrDefault();
                Models.CIDADE cidadeEntidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                if (cidadeEmpresa.UfSigla != cidadeEntidade.UfSigla) cFOP = "2" + tabelaConfigNFe.NaturezaOperacao;

                #endregion

                #region Verificações do Pedido de Compra

                Models.PED_COMP pedCompra = apolo.PED_COMP
                    .Where(w => w.EmpCod == empresa.EmpCod
                        && w.PedCompNum == pedCompNum).FirstOrDefault();

                if (pedCompra == null)
                    return "O pedido de compra " + pedCompNum + " não existe na empresa " + empresa.EmpCod + "!";

                string condPag = "";
                Models.COND_PAG_PED_COMP listCondPedComp = apolo.COND_PAG_PED_COMP
                    .Where(w => w.EmpCod == pedCompra.EmpCod && w.PedCompNum == pedCompra.PedCompNum).FirstOrDefault();

                if (listCondPedComp != null) condPag = listCondPedComp.CondPagCod;

                #endregion

                #endregion

                if (chaveNFE != "")
                {
                    #region Insere Movimentação de Estoque no Apolo

                    #region MOV_ESTQ

                    MOV_ESTQ movestq = servico.InsereMovEstq(empresa.EmpCod, tipoLanc, entidade.EntCod, dataEmissao,
                        usuario);

                    if (data != null)
                    {
                        movestq.MovEstqDataMovimento = Convert.ToDateTime(data);
                        movestq.MovEstqDataEntrada = movestq.MovEstqDataMovimento;
                    }

                    movestq.MovEstqDocEmpCod = movestq.EmpCod;
                    movestq.MovEstqDocEspec = especie;
                    movestq.MovEstqDocSerie = serie;
                    movestq.MovEstqDocNum = numNFE;
                    movestq.CondPagCod = condPag;
                    movestq.TipoPagRecCod = null;

                    movestq.MovEstqObs = "XML Importado via Sistema WEB";
                    movestq.MovEstqPercDescGer = 0;
                    movestq.MovEstqPercDescGerProd = 0;
                    movestq.MovEstqPercDescGerServ = 0;
                    movestq.MovEstqValDescGer = 0;
                    movestq.MovEstqValDescGerProd = 0;
                    movestq.MovEstqValDescGerServ = 0;
                    movestq.MovEstq = "Sim";
                    movestq.MovEstqValFrete = 0;
                    movestq.MovEstqValSeguro = 0;
                    movestq.MovEstqValOutra = 0;
                    movestq.MovEstqValDespDiv = 0;
                    movestq.MovEstqOrigMod = "Estoque";
                    movestq.MovEstqDataCompet = movestq.MovEstqDataMovimento;

                    #region Incializa Campos MOV_ESTQ

                    movestq.MovEstqValEmbalagem = 0;
                    movestq.MovEstqValDoc = valorTotalXML;
                    movestq.MovEstqPercFreteEmbut = 0;
                    movestq.MovEstqValFreteEmbut = 0;
                    movestq.MovEstqPercFinancEmbut = 0;
                    movestq.MovEstqValFinancEmbut = 0;
                    movestq.MovEstqSegIndEconValCambio = 0;
                    movestq.MovEstqValLib = valorTotalXML;
                    movestq.MovEstqValOrig = valorTotalXML;
                    movestq.MovEstqIndEconValCambioDoc = 0;
                    movestq.MovEstqFreteDocIndEconCamb = 0;
                    movestq.MovEstqSegDocVal = 0;
                    movestq.MovEstqSegDocIndEconCamb = 0;
                    movestq.MovEstqOutraDespDocOrigVal = 0;
                    movestq.MovEstqValSiscomex = 0;
                    movestq.MovEstqPercCsllRF = 0;
                    movestq.MovEstqPercCofinsRF = 0;
                    movestq.MovEstqPercPisRF = 0;
                    movestq.MovEstqValMercST = 0;
                    movestq.MovEstqValCap = 0;
                    movestq.MovEstqValRecAntIcmsST = 0;
                    movestq.MovEstqValDescIcms = 0;
                    movestq.MovEstqValReembolso = 0;
                    movestq.MovEstqGeraFiscal = "Sim";
                    movestq.MovEstqValDocLiq = valorTotalXML;
                    movestq.MovEstqValPedagio = 0;
                    movestq.MovEstqValPisProdRF = 0;
                    movestq.MovEstqValCofinsProdRF = 0;
                    movestq.MovEstqValBasePISRF = 0;
                    movestq.MovEstqValBaseCofinsRF = 0;
                    movestq.MovEstqValBasePisProdRF = 0;
                    movestq.MovEstqValBaseCofinsProdRF = 0;
                    movestq.MovEstqPercAcrescFin = 0;
                    movestq.MovEstqValAcrescFin = 0;
                    movestq.MovEstqValPisProdDedTot = "Sim";
                    movestq.MovEstqValCofinsDedTot = "Sim";
                    movestq.MovEstqPesoCubado = 0;
                    movestq.MovEstqNatFrete = "N";
                    movestq.MovEstqValDescIcmsZFM = 0;
                    movestq.MovEstqValDescPisZFM = 0;
                    movestq.MovEstqValDescCofinsZFM = 0;
                    movestq.MovEstqTxaMarMerc = 0;
                    movestq.MovEstqRatTxaMarMercPorPeso = "Sim";
                    movestq.MovEstqRatSiscomexPorPeso = "Sim";
                    movestq.MovEstqRatDespImportPorPeso = "Sim";
                    movestq.MovEstqBaseFCPIcms = movestq.MovEstqBaseIcms;
                    movestq.MovEstqValFCPIcms = 0;
                    movestq.MovEstqBaseFCPIcmsST = 0;
                    movestq.MovEstqValFCPIcmsST = 0;
                    movestq.MovEstqValServPrestSeg15 = 0;
                    movestq.MovEstqValServPrestSeg20 = 0;
                    movestq.MovEstqValServPrestSeg25 = 0;
                    movestq.MovEstqValAdicServPrestSeg = 0;
                    movestq.MovEstqValAdicNaoRetServ = 0;
                    movestq.MovEstqValBaseInssNaoDev = 0;
                    movestq.MovEstqValInssNaoDev = 0;
                    movestq.MovEstqValBaseIrrfNaoDev = 0;
                    movestq.MovEstqValIrrfNaoDev = 0;
                    movestq.MovEstqValBasePisNaoDev = 0;
                    movestq.MovEstqValPisNaoDev = 0;
                    movestq.MovEstqValBaseCofinsNaoDev = 0;
                    movestq.MovEstqValCofinsNaoDev = 0;
                    movestq.MovEstqValBaseCsllNaoDev = 0;
                    movestq.MovEstqValCsllNaoDev = 0;
                    movestq.MovEstqDataCompet = movestq.MovEstqDataMovimento;

                    movestq.MovEstqNFEmisProp = "Não";

                    #endregion

                    bdApolo.MOV_ESTQ.AddObject(movestq);

                    #endregion

                    #region MOV_ESTQ_NF_ELETRONICA

                    MOV_ESTQ_NF_ELETRONICA chaveMov = new MOV_ESTQ_NF_ELETRONICA();
                    chaveMov.EmpCod = movestq.EmpCod;
                    chaveMov.MovEstqChv = movestq.MovEstqChv;
                    chaveMov.UsuCod = usuario;
                    chaveMov.MovEstqNFEImpData = movestq.MovEstqDataMovimento;
                    chaveMov.MovEstqNFEImpChvAcesso = chaveNFE;
                    chaveMov.MovEstqNFEImpStatus = "Importado";

                    bdApolo.MOV_ESTQ_NF_ELETRONICA.AddObject(chaveMov);

                    #endregion

                    #region Insere Itens

                    foreach (var item in listaProd)
                    {
                        #region Carrega Variáveis do Item do Pedido de Compra

                        Models.ITEM_PED_COMP itemPedComp = apolo.ITEM_PED_COMP
                            .Where(w => w.EmpCod == empresa.EmpCod && w.PedCompNum == pedCompNum
                                && apolo.PRODUTO.Any(a => w.ProdCodEstr == a.ProdCodEstr &&
                                    apolo.CLAS_FISCAL.Any(n => n.ClasFiscCod == a.ClasFiscCod
                                        && n.ClasFiscCodNbm == item.NCM))
                                && w.ItPedCompSaldoQtd >= item.Qtde
                                && w.ItPedCompAprovUsuCod != null)
                            .FirstOrDefault();

                        #endregion

                        if (itemPedComp != null)
                        {
                            #region Cria variáveis

                            string pisCofinsTributacao = "";
                            string conta = tabelaConfigNFe.ContaDebito;

                            #endregion

                            #region Carrega Tributação

                            if (item.PisValor > 0) cFOP = cFOP + ".001";

                            Models.PRODUTO produtoObj = apolo.PRODUTO.Where(w => w.ProdCodEstr == itemPedComp.ProdCodEstr).FirstOrDefault();
                            Models.NAT_OPERACAO natOperacao = apolo.NAT_OPERACAO.Where(w => w.NatOpCodEstr == cFOP).FirstOrDefault();

                            if (natOperacao == null)
                                return "Não existe natureza de operação " + cFOP + "! Verifique!";

                            pisCofinsTributacao = natOperacao.NatOpConfTribPisCod;

                            Models.CLAS_FISCAL clasFiscal = apolo.CLAS_FISCAL.Where(w => w.ClasFiscCod == produtoObj.ClasFiscCod).FirstOrDefault();
                            Models.CLAS_FISCAL_AUX clasFiscalAux = new Models.CLAS_FISCAL_AUX();
                            Models.CLAS_FISCAL_PIS clasFiscalPis = new Models.CLAS_FISCAL_PIS();
                            Models.CLAS_FISCAL_COFINS clasFiscalCofins = new Models.CLAS_FISCAL_COFINS();
                            if (clasFiscal == null)
                                return "Não existe classificação fiscal no produto " + produtoObj.ProdCodEstr + "! Verifique!";
                            else
                            {
                                #region Carrega ICMS

                                clasFiscalAux = apolo.CLAS_FISCAL_AUX.Where(w => w.ClasFiscCod == clasFiscal.ClasFiscCod
                                        && w.ClasAuxUfSiglaOrig == cidadeEntidade.UfSigla && w.ClasAuxPaisSiglaOrig == cidadeEntidade.PaisSigla
                                        && w.ClasAuxUfSiglaDest == cidadeEmpresa.UfSigla && w.ClasAuxPaisSiglaDest == cidadeEmpresa.PaisSigla
                                        && w.ClasAuxOper == "Entrada")
                                    .FirstOrDefault();

                                if (clasFiscalAux == null)
                                    return "Não existe configuração de ICMS de Entrada na classificação fiscal " + clasFiscal.ClasFiscCod 
                                        + " da origem " + cidadeEmpresa.PaisSigla + "-" + cidadeEmpresa.UfSigla
                                        + " para o destino " + cidadeEntidade.PaisSigla + "-" + cidadeEntidade.UfSigla
                                        + "! Verifique!";

                                #endregion

                                #region Carrega PIS

                                if (natOperacao.NatOpIncidePis == "Sim")
                                {
                                    clasFiscalPis = apolo.CLAS_FISCAL_PIS.Where(w => w.ClasFiscCod == clasFiscal.ClasFiscCod
                                            && w.EmpCod == empresa.EmpCod && w.ClasFiscPisOper == "Entrada")
                                        .FirstOrDefault();

                                    if (clasFiscalPis == null)
                                        return "Não existe configuração de PIS de Entrada na classificação fiscal " + clasFiscal.ClasFiscCod
                                            + "! Verifique!";
                                }

                                #endregion

                                #region Carrega COFINS

                                if (natOperacao.NatOpIncideCofins == "Sim")
                                {
                                    clasFiscalCofins = apolo.CLAS_FISCAL_COFINS.Where(w => w.ClasFiscCod == clasFiscal.ClasFiscCod
                                            && w.EmpCod == empresa.EmpCod && w.ClasFiscCofinsOper == "Entrada")
                                        .FirstOrDefault();

                                    if (clasFiscalCofins == null)
                                        return "Não existe configuração de COFINS de Entrada na classificação fiscal " + clasFiscal.ClasFiscCod
                                            + "! Verifique!";
                                }

                                #endregion
                            }

                            #endregion

                            #region Localiza Unidade de Medida do XML no Apolo

                            PROD_UNID_MED prodUnidMed = bdApolo.PROD_UNID_MED
                                .Where(w => w.ProdUnidMedCod.Contains(item.UnidadeMedida)
                                    && w.ProdCodEstr == itemPedComp.ProdCodEstr).FirstOrDefault();

                            if (prodUnidMed == null)
                                return "A unidade de medida " + item.UnidadeMedida + " do item " + item.Descricao + " não está cadastrado! Verifique!";

                            #endregion

                            #region ITEM_MOV_ESTQ

                            string tribCod = "0" + clasFiscalAux.TribBCod;

                            ITEM_MOV_ESTQ itemMovEstq = servico.InsereItemMovEstq(movestq.MovEstqChv, movestq.EmpCod,
                                movestq.TipoLancCod, movestq.EntCod, movestq.MovEstqDataMovimento, itemPedComp.ProdCodEstr,
                                cFOP, item.Qtde, item.ValorUnitario, prodUnidMed.ProdUnidMedCod, prodUnidMed.ProdUnidMedPos,
                                tribCod, item.NCM, clasFiscal.ClasFiscCod);

                            #region Calcula_Item_Estq

                            // OBS.: Verificar se a alteração no ApoloModel.edmx está feita para resolver o problema de casas decimais no retorno.

                            #region Inicializa Variáveis Calcula_Item_Estq

                            decimal valproduto = itemMovEstq.ItMovEstqValProd;
                            decimal percacrescfin = 0;
                            decimal valacrescfin = 0;
                            decimal percdescespec = 0;
                            decimal valdescespec = 0;
                            string produto = itemMovEstq.ProdCodEstr;
                            decimal baseipi = 0;
                            decimal percipi = 0;
                            decimal valipi = 0;
                            string ipiinclusoicms = "";
                            decimal baseicms = 0;
                            decimal percredbaseicms = 0;
                            decimal valredbaseicms = 0;
                            decimal baseicmsred = 0;
                            decimal percicms = 0;
                            decimal valicms = 0;
                            decimal valipirec = 0;
                            decimal valicmsrec = 0;
                            string prodchamou = "Sim";
                            string proddivnome = "";
                            decimal baseii = 0;
                            decimal percii = 0;
                            decimal valii = 0;
                            decimal valiirec = 0;
                            string clasfisccod = itemMovEstq.ClasFiscCod;
                            string tribbcod = itemMovEstq.TribBCod;
                            string operacao = clasFiscalAux.ClasAuxOper;
                            string empcod = itemMovEstq.EmpCod;
                            string entcod = itemMovEstq.EntCod;
                            DateTime dataemis = Convert.ToDateTime(movestq.MovEstqDataEmissao);
                            string tipolanccod = itemMovEstq.TipoLancCod;
                            string natopcodestr = itemMovEstq.NatOpCodEstr;
                            string credipicompracom = "";
                            decimal perccredipicompracom = 0;
                            string calcdificms = "";
                            decimal percdificms = 0;
                            decimal valdificms = 0;
                            string impnorm = "Normal";
                            decimal quantidade = item.Qtde;
                            short posicao = 0;
                            decimal margemlucro = 0;
                            decimal precolista = 0;
                            decimal valembalagemst = 0;
                            decimal valfretest = 0;
                            decimal valsegurost = 0;
                            decimal valdespesast = 0;
                            decimal valicmsorig = 0;
                            decimal baseipired = 0;
                            decimal percredipi = 0;
                            decimal valipired = 0;
                            decimal valipiorig = 0;
                            decimal baseiired = 0;
                            decimal percredii = 0;
                            decimal valiired = 0;
                            decimal valiiorig = 0;
                            decimal baseicmsst = 0;
                            decimal percicmsexon = 0;
                            string fabricante = "";
                            string reduzbaseicmsvirtual = "";
                            int qtdmesescomod = 0;
                            decimal fatcalcimpcomod = 0;

                            ObjectParameter rbaseipi = new ObjectParameter("rbaseipi", typeof(global::System.Decimal));
                            ObjectParameter rpercipi = new ObjectParameter("rpercipi", typeof(global::System.Decimal));
                            ObjectParameter rvalipi = new ObjectParameter("rvalipi", typeof(global::System.Decimal));
                            ObjectParameter rbaseicms = new ObjectParameter("rbaseicms", typeof(global::System.Decimal));
                            ObjectParameter rpercredbaseicms = new ObjectParameter("rpercredbaseicms", typeof(global::System.Decimal));
                            ObjectParameter rvalredbaseicms = new ObjectParameter("rvalredbaseicms", typeof(global::System.Decimal));
                            ObjectParameter rbaseicmsred = new ObjectParameter("rbaseicmsred", typeof(global::System.Decimal));
                            ObjectParameter rpercicms = new ObjectParameter("rpercicms", typeof(global::System.Decimal));
                            ObjectParameter rvalicms = new ObjectParameter("rvalicms", typeof(global::System.Decimal));
                            ObjectParameter rvalipirec = new ObjectParameter("rvalipirec", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsrec = new ObjectParameter("rvalicmsrec", typeof(global::System.Decimal));
                            ObjectParameter rvalacrescfin = new ObjectParameter("rvalacrescfin", typeof(global::System.Decimal));
                            ObjectParameter rvaldescespec = new ObjectParameter("rvaldescespec", typeof(global::System.Decimal));
                            ObjectParameter rbaseii = new ObjectParameter("rbaseii", typeof(global::System.Decimal));
                            ObjectParameter rpercii = new ObjectParameter("rpercii", typeof(global::System.Decimal));
                            ObjectParameter rvalii = new ObjectParameter("rvalii", typeof(global::System.Decimal));
                            ObjectParameter rvaliirec = new ObjectParameter("rvaliirec", typeof(global::System.Decimal));
                            ObjectParameter rclasfisccod = new ObjectParameter("rclasfisccod", typeof(global::System.String));
                            ObjectParameter rtribbcod = new ObjectParameter("rtribbcod", typeof(global::System.String));
                            ObjectParameter rbasepis = new ObjectParameter("rbasepis", typeof(global::System.Decimal));
                            ObjectParameter rpercpis = new ObjectParameter("rpercpis", typeof(global::System.Decimal));
                            ObjectParameter rvalpis = new ObjectParameter("rvalpis", typeof(global::System.Decimal));
                            ObjectParameter rbasecofins = new ObjectParameter("rbasecofins", typeof(global::System.Decimal));
                            ObjectParameter rperccofins = new ObjectParameter("rperccofins", typeof(global::System.Decimal));
                            ObjectParameter rvalcofins = new ObjectParameter("rvalcofins", typeof(global::System.Decimal));
                            ObjectParameter rperccredipicompracom = new ObjectParameter("rperccredipicompracom", typeof(global::System.Decimal));
                            ObjectParameter rpercdificms = new ObjectParameter("rpercdificms", typeof(global::System.Decimal));
                            ObjectParameter rvaldificms = new ObjectParameter("rvaldificms", typeof(global::System.Decimal));
                            ObjectParameter rvalpisrec = new ObjectParameter("rvalpisrec", typeof(global::System.Decimal));
                            ObjectParameter rvalcofinsrec = new ObjectParameter("rvalcofinsrec", typeof(global::System.Decimal));
                            ObjectParameter rpercredbasepis = new ObjectParameter("rpercredbasepis", typeof(global::System.Decimal));
                            ObjectParameter rvalredbasepis = new ObjectParameter("rvalredbasepis", typeof(global::System.Decimal));
                            ObjectParameter rbasepisred = new ObjectParameter("rbasepisred", typeof(global::System.Decimal));
                            ObjectParameter rpercredbasecofins = new ObjectParameter("rpercredbasecofins", typeof(global::System.Decimal));
                            ObjectParameter rvalredbasecofins = new ObjectParameter("rvalredbasecofins", typeof(global::System.Decimal));
                            ObjectParameter rbasecofinsred = new ObjectParameter("rbasecofinsred", typeof(global::System.Decimal));
                            ObjectParameter rpercdescrepicms = new ObjectParameter("rpercdescrepicms", typeof(global::System.Decimal));
                            ObjectParameter rvaldescicmsdif = new ObjectParameter("rvaldescicmsdif", typeof(global::System.Decimal));
                            ObjectParameter rvaldescicmsred = new ObjectParameter("rvaldescicmsred", typeof(global::System.Decimal));
                            ObjectParameter rcalcprecolista = new ObjectParameter("rcalcprecolista", typeof(global::System.String));
                            ObjectParameter rprecolista = new ObjectParameter("rprecolista", typeof(global::System.Decimal));
                            ObjectParameter rbaseicmsst = new ObjectParameter("rbaseicmsst", typeof(global::System.Decimal));
                            ObjectParameter rpercicmsst = new ObjectParameter("rpercicmsst", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsst = new ObjectParameter("rvalicmsst", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsretst = new ObjectParameter("rvalicmsretst", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsembalagemst = new ObjectParameter("rvalicmsembalagemst", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsfretest = new ObjectParameter("rvalicmsfretest", typeof(global::System.Decimal));
                            ObjectParameter rvalicmssegurost = new ObjectParameter("rvalicmssegurost", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsdespesast = new ObjectParameter("rvalicmsdespesast", typeof(global::System.Decimal));
                            ObjectParameter rpercredicmsst = new ObjectParameter("rpercredicmsst", typeof(global::System.Decimal));
                            ObjectParameter rpercmarglucro = new ObjectParameter("rpercmarglucro", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsorig = new ObjectParameter("rvalicmsorig", typeof(global::System.Decimal));
                            ObjectParameter rvalpisorig = new ObjectParameter("rvalpisorig", typeof(global::System.Decimal));
                            ObjectParameter rvalcofinsorig = new ObjectParameter("rvalcofinsorig", typeof(global::System.Decimal));
                            ObjectParameter rbaseipired = new ObjectParameter("rbaseipired", typeof(global::System.Decimal));
                            ObjectParameter rpercredipi = new ObjectParameter("rpercredipi", typeof(global::System.Decimal));
                            ObjectParameter rvalipired = new ObjectParameter("rvalipired", typeof(global::System.Decimal));
                            ObjectParameter rvalipiorig = new ObjectParameter("rvalipiorig", typeof(global::System.Decimal));
                            ObjectParameter rbaseiired = new ObjectParameter("rbaseiired", typeof(global::System.Decimal));
                            ObjectParameter rpercredii = new ObjectParameter("rpercredii", typeof(global::System.Decimal));
                            ObjectParameter rvaliired = new ObjectParameter("rvaliired", typeof(global::System.Decimal));
                            ObjectParameter rvaliiorig = new ObjectParameter("rvaliiorig", typeof(global::System.Decimal));
                            ObjectParameter rvalpercdiferimicms = new ObjectParameter("rvalpercdiferimicms", typeof(global::System.Decimal));
                            ObjectParameter rvaldiferimicms = new ObjectParameter("rvaldiferimicms", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsdevido = new ObjectParameter("rvalicmsdevido", typeof(global::System.Decimal));
                            ObjectParameter rvalcredpresumicms = new ObjectParameter("rvalcredpresumicms", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsrecolher = new ObjectParameter("rvalicmsrecolher", typeof(global::System.Decimal));
                            ObjectParameter rpercicmsexon = new ObjectParameter("rpercicmsexon", typeof(global::System.Decimal));
                            ObjectParameter rqtdbaseipipauta = new ObjectParameter("rqtdbaseipipauta", typeof(global::System.Decimal));
                            ObjectParameter rvalunitipipauta = new ObjectParameter("rvalunitipipauta", typeof(global::System.Decimal));
                            ObjectParameter rqtdbasepispauta = new ObjectParameter("rqtdbasepispauta", typeof(global::System.Decimal));
                            ObjectParameter rvalunitpispauta = new ObjectParameter("rvalunitpispauta", typeof(global::System.Decimal));
                            ObjectParameter rqtdbasecofinspauta = new ObjectParameter("rqtdbasecofinspauta", typeof(global::System.Decimal));
                            ObjectParameter rvalunitcofinspauta = new ObjectParameter("rvalunitcofinspauta", typeof(global::System.Decimal));
                            ObjectParameter rpercfunrural = new ObjectParameter("rpercfunrural", typeof(global::System.Decimal));
                            ObjectParameter rbasefunrural = new ObjectParameter("rbasefunrural", typeof(global::System.Decimal));
                            ObjectParameter rvalfunrural = new ObjectParameter("rvalfunrural", typeof(global::System.Decimal));
                            ObjectParameter rbasepisrf = new ObjectParameter("rbasepisrf", typeof(global::System.Decimal));
                            ObjectParameter rpercpisrf = new ObjectParameter("rpercpisrf", typeof(global::System.Decimal));
                            ObjectParameter rvalorpisrf = new ObjectParameter("rvalorpisrf", typeof(global::System.Decimal));
                            ObjectParameter rbasecofinsrf = new ObjectParameter("rbasecofinsrf", typeof(global::System.Decimal));
                            ObjectParameter rperccofinsrf = new ObjectParameter("rperccofinsrf", typeof(global::System.Decimal));
                            ObjectParameter rvalorcofinsrf = new ObjectParameter("rvalorcofinsrf", typeof(global::System.Decimal));
                            ObjectParameter rvalicmsvirtual = new ObjectParameter("rvalicmsvirtual", typeof(global::System.Decimal));

                            #endregion

                            #region Execute Calcula_Item_Estq

                            bdApolo.calcula_item_estq(valproduto,
                                percacrescfin,
                                valacrescfin,
                                percdescespec,
                                valdescespec,
                                produto,
                                baseipi,
                                percipi,
                                valipi,
                                ipiinclusoicms,
                                baseicms,
                                percredbaseicms,
                                valredbaseicms,
                                baseicmsred,
                                percicms,
                                valicms,
                                valipirec,
                                valicmsrec,
                                prodchamou,
                                proddivnome,
                                baseii,
                                percii,
                                valii,
                                valiirec,
                                clasfisccod,
                                tribbcod,
                                operacao,
                                empcod,
                                entcod,
                                dataemis,
                                tipolanccod,
                                natopcodestr,
                                credipicompracom,
                                perccredipicompracom,
                                calcdificms,
                                percdificms,
                                valdificms,
                                impnorm,
                                quantidade,
                                posicao,
                                margemlucro,
                                precolista,
                                valembalagemst,
                                valfretest,
                                valsegurost,
                                valdespesast,
                                valicmsorig,
                                baseipired,
                                percredipi,
                                valipired,
                                valipiorig,
                                baseiired,
                                percredii,
                                valiired,
                                valiiorig,
                                baseicmsst,
                                percicmsexon,
                                fabricante,
                                reduzbaseicmsvirtual,
                                qtdmesescomod,
                                fatcalcimpcomod,
                                rbaseipi,
                                rpercipi,
                                rvalipi,
                                rbaseicms,
                                rpercredbaseicms,
                                rvalredbaseicms,
                                rbaseicmsred,
                                rpercicms,
                                rvalicms,
                                rvalipirec,
                                rvalicmsrec,
                                rvalacrescfin,
                                rvaldescespec,
                                rbaseii,
                                rpercii,
                                rvalii,
                                rvaliirec,
                                rclasfisccod,
                                rtribbcod,
                                rbasepis,
                                rpercpis,
                                rvalpis,
                                rbasecofins,
                                rperccofins,
                                rvalcofins,
                                rperccredipicompracom,
                                rpercdificms,
                                rvaldificms,
                                rvalpisrec,
                                rvalcofinsrec,
                                rpercredbasepis,
                                rvalredbasepis,
                                rbasepisred,
                                rpercredbasecofins,
                                rvalredbasecofins,
                                rbasecofinsred,
                                rpercdescrepicms,
                                rvaldescicmsdif,
                                rvaldescicmsred,
                                rcalcprecolista,
                                rprecolista,
                                rbaseicmsst,
                                rpercicmsst,
                                rvalicmsst,
                                rvalicmsretst,
                                rvalicmsembalagemst,
                                rvalicmsfretest,
                                rvalicmssegurost,
                                rvalicmsdespesast,
                                rpercredicmsst,
                                rpercmarglucro,
                                rvalicmsorig,
                                rvalpisorig,
                                rvalcofinsorig,
                                rbaseipired,
                                rpercredipi,
                                rvalipired,
                                rvalipiorig,
                                rbaseiired,
                                rpercredii,
                                rvaliired,
                                rvaliiorig,
                                rvalpercdiferimicms,
                                rvaldiferimicms,
                                rvalicmsdevido,
                                rvalcredpresumicms,
                                rvalicmsrecolher,
                                rpercicmsexon,
                                rqtdbaseipipauta,
                                rvalunitipipauta,
                                rqtdbasepispauta,
                                rvalunitpispauta,
                                rqtdbasecofinspauta,
                                rvalunitcofinspauta,
                                rpercfunrural,
                                rbasefunrural,
                                rvalfunrural,
                                rbasepisrf,
                                rpercpisrf,
                                rvalorpisrf,
                                rbasecofinsrf,
                                rperccofinsrf,
                                rvalorcofinsrf,
                                rvalicmsvirtual);

                            #endregion

                            #endregion

                            itemMovEstq.ItMovEstqUnidMedCodVal = itemMovEstq.ItMovEstqUnidMedCod;
                            itemMovEstq.ItMovEstqUnidMedPosVal = itemMovEstq.ItMovEstqUnidMedPos;
                            itemMovEstq.ItMovEstqPercIcms = Convert.ToDecimal(rpercicms.Value);
                            itemMovEstq.ItMovEstqBaseIcms = Convert.ToDecimal(rbaseicms.Value);
                            itemMovEstq.ItMovEstqValIcms = Convert.ToDecimal(rvalicms.Value);
                            itemMovEstq.ItMovEstqValIcmsRec = Convert.ToDecimal(rvalicmsrec.Value);
                            itemMovEstq.ItMovEstqValICMSOrig = Convert.ToDecimal(rvalicmsorig.Value);
                            itemMovEstq.ItMovEstqConfTribPisCod = pisCofinsTributacao;
                            itemMovEstq.ItMovEstqConfTribCofinsCod = pisCofinsTributacao;
                            itemMovEstq.ItMovEstqValBasePis = Convert.ToDecimal(rbasepis.Value);
                            itemMovEstq.ItMovEstqValBaseCofins = Convert.ToDecimal(rbasecofins.Value);
                            itemMovEstq.ItMovEstqPercPis = Convert.ToDecimal(rpercpis.Value);
                            itemMovEstq.ItMovEstqPercCofins = Convert.ToDecimal(rperccofins.Value);
                            itemMovEstq.ItMovEstqValPis = Convert.ToDecimal(rvalpis.Value);
                            itemMovEstq.ItMovEstqValPisRec = Convert.ToDecimal(rvalpisrec.Value);
                            itemMovEstq.ItMovEstqValPISOrig = Convert.ToDecimal(rvalpisorig.Value);
                            itemMovEstq.ItMovEstqValCofins = Convert.ToDecimal(rvalcofins.Value);
                            itemMovEstq.ItMovEstqValCofinsRec = Convert.ToDecimal(rvalcofinsrec.Value);
                            itemMovEstq.ItMovEstqValCOFINSOrig = Convert.ToDecimal(rvalcofinsorig.Value);
                            itemMovEstq.ItMovEstqCustoUnit = item.ValorUnitario;
                            itemMovEstq.ItMovEstqValProd = item.ValorTotalProduto;
                            itemMovEstq.EmpPedComp = itemPedComp.EmpCod;
                            itemMovEstq.ItPedCompSeq = itemPedComp.ItPedCompSeq;
                            itemMovEstq.ProdCodEstrPedComp = itemPedComp.ProdCodEstr;
                            itemMovEstq.PedCompNum = itemPedComp.PedCompNum;

                            #region Inicializa Valores Item_Mov_Estq

                            itemMovEstq.ItMovEstqValAcrescFin = 0;
                            itemMovEstq.ItMovEstqValDescEspec = 0;
                            itemMovEstq.ItMovEstqValDespDiv = 0;
                            itemMovEstq.ItMovEstqValDescGer = 0;
                            itemMovEstq.ItMovEstqPercAcrescFin = 0;
                            itemMovEstq.ItMovEstqPercDescEspec = 0;
                            itemMovEstq.ItMovEstqValEmbalagem = 0;
                            itemMovEstq.ItMovEstqValFrete = 0;
                            itemMovEstq.ItMovEstqValSeguro = 0;
                            itemMovEstq.ItMovEstqValOutra = 0;
                            itemMovEstq.ItMovEstqValServ = 0;
                            itemMovEstq.ItMovEstqPercRedBaseIcms = Convert.ToDecimal(rpercredbaseicms.Value);
                            itemMovEstq.ItMovEstqValRedBaseIcms = Convert.ToDecimal(rvalredbaseicms.Value);
                            itemMovEstq.ItMovEstqBaseIcmsRed = Convert.ToDecimal(rbaseicmsred.Value);
                            itemMovEstq.ItMovEstqST = "F";
                            itemMovEstq.ItMovEstqValIcmsRetST = 0;
                            itemMovEstq.ItMovEstqValIcmsRetSTRec = 0;
                            itemMovEstq.ItMovEstqBaseIpi = 0;
                            itemMovEstq.ItMovEstqPercIpi = 0;
                            itemMovEstq.ItMovEstqValIpi = 0;
                            itemMovEstq.ItMovEstqIpiBaseIcms = "Não";
                            itemMovEstq.ItMovEstqValIpiRec = 0;
                            itemMovEstq.ItMovEstqValBaseIss = 0;
                            itemMovEstq.ItMovEstqPercIss = 0;
                            itemMovEstq.ItMovEstqValIss = 0;
                            itemMovEstq.ItMovEstqValIrrf = 0;
                            itemMovEstq.ItMovEstqPercIrrf = 0;
                            itemMovEstq.ItMovEstqValBaseInss = 0;
                            itemMovEstq.ItMovEstqPercInss = 0;
                            itemMovEstq.ItMovEstqValInss = 0;
                            itemMovEstq.ItMovEstqQtdUnidMed = 0;
                            itemMovEstq.ItMovEstqCredIpiCompraCom = "Não";
                            itemMovEstq.ItMovEstqCredIpiCompraComPerc = 0;
                            itemMovEstq.ItMovEstqCalcDifIcms = "Não";
                            itemMovEstq.ItMovEstqPercDifIcms = 0;
                            itemMovEstq.ItMovEstqQtdDesm = 0;
                            itemMovEstq.ItMovEstqRejPat = "Não";
                            itemMovEstq.ItMovEstqBaseII = 0;
                            itemMovEstq.ItMovEstqPercII = 0;
                            itemMovEstq.ItMovEstqValIIRec = 0;
                            itemMovEstq.ItMovEstqValProdDoc = 0;
                            itemMovEstq.ItMovEstqFreteDocOrigVal = 0;
                            itemMovEstq.ItMovEstqSegDocOrigVal = 0;
                            itemMovEstq.ItMovEstqOutraDespDocOrigVal = 0;
                            itemMovEstq.ItMovEstqValProdFOB = 0;
                            itemMovEstq.ItMovEstqValSiscomex = 0;
                            itemMovEstq.ItMovEstqValCalcIssDedTot = 0;
                            itemMovEstq.ItMovEstqValDifIcms = 0;
                            itemMovEstq.ItMovEstqValBaseCsll = 0;
                            itemMovEstq.ItMovEstqPercCsllRF = 0;
                            itemMovEstq.ItMovEstqValCsllRF = 0;
                            itemMovEstq.ItMovEstqPercCofinsRF = 0;
                            itemMovEstq.ItMovEstqValCofinsRF = 0;
                            itemMovEstq.ItMovEstqPercPisRF = 0;
                            itemMovEstq.ItMovEstqValPisRF = 0;
                            itemMovEstq.ItMovEstqValAcrescCustoComp = 0;
                            itemMovEstq.ItMovEstqValDescCustoComp = 0;
                            itemMovEstq.ItMovEstqPercRedBasePis = 0;
                            itemMovEstq.ItMovEstqValRedBasePis = 0;
                            itemMovEstq.ItMovEstqBasePisRed = Convert.ToDecimal(rbasepisred.Value);
                            itemMovEstq.ItMovEstqPercRedBaseCofins = 0;
                            itemMovEstq.ItMovEstqValRedBaseCofins = 0;
                            itemMovEstq.ItMovEstqBaseCofinsRed = Convert.ToDecimal(rbasecofinsred.Value);
                            itemMovEstq.ItMovEstqPat = "Não";
                            itemMovEstq.ItMovEstqValDescGer = 0;
                            itemMovEstq.ItMovEstqRepIcmsDifPercDesc = 0;
                            itemMovEstq.ItMovEstqRepIcmsDifValDesc = 0;
                            itemMovEstq.ItMovEstqRepIcmsRedValDesc = 0;
                            itemMovEstq.ItMovEstqCalcSTPrecoLista = "Não";
                            itemMovEstq.ItMovEstqMargLucroST = 0;
                            itemMovEstq.ItMovEstqPrecoListaST = 0;
                            itemMovEstq.ItMovEstqPercRedIcmsST = 0;
                            itemMovEstq.ItMovEstqValBaseIcmsST = 0;
                            itemMovEstq.ItMovEstqPercIcmsST = 0;

                            itemMovEstq.ItMovEstqQtdBemPat = 1;
                            //itemMovEstq.TribBModBCCod = "3";
                            itemMovEstq.ItMovEstqConfTribTipoIpi = "IPI";
                            itemMovEstq.ItMovEstqConfTribTipoPis = "PIS";
                            itemMovEstq.ItMovEstqConfTribTipoCofins = "COFINS";
                            itemMovEstq.ItMovEstqConfTribIpiCod = "02";
                            itemMovEstq.ItMovEstqRedCOFINS = "Nenhum";
                            itemMovEstq.ItMovEstqRedPIS = "Nenhum";

                            itemMovEstq.ItMovEstq = "Sim";

                            itemMovEstq.ItMovEstqBaseCustoMed = Convert.ToDecimal(itemMovEstq.ItMovEstqValProd - itemMovEstq.ItMovEstqValIcms);
                            itemMovEstq.ItMovEstqValBaseIrrf = 0;
                            itemMovEstq.ItMovEstqCustoUnitSegIndEcon = 0;
                            itemMovEstq.ItMovEstqBaseCMedSegIndEcon = 0;
                            itemMovEstq.ItMovEstqValII = 0;
                            itemMovEstq.ItMovEstqValIcmsST = 0;
                            itemMovEstq.ItMovEstqValEmbalagemST = 0;
                            itemMovEstq.ItMovEstqValIcmsEmbalagemST = 0;
                            itemMovEstq.ItMovEstqValFreteST = 0;
                            itemMovEstq.ItMovEstqValIcmsFreteST = 0;
                            itemMovEstq.ItMovEstqValSeguroST = 0;
                            itemMovEstq.ItMovEstqValIcmsSeguroST = 0;
                            itemMovEstq.ItMovEstqValDespesaST = 0;
                            itemMovEstq.ItMovEstqValIcmsDespesaST = 0;
                            itemMovEstq.ItMovEstqValCalcFreteST = 0;
                            itemMovEstq.ItMovEstqValIcmsCalcFreteST = 0;
                            itemMovEstq.ItMovEstqPesoLiq = 0;
                            itemMovEstq.ItMovEstqPesoBruto = 0;
                            itemMovEstq.ItMovEstqCustoUnitLiq = 0;
                            itemMovEstq.ItMovEstqPercRedBaseInss = 0;
                            itemMovEstq.ItMovEstqValRedBaseInss = 0;
                            itemMovEstq.ItMovEstqBaseInssRed = 0;
                            itemMovEstq.ItMovEstqValRecAntIcmsST = 0;
                            itemMovEstq.ItMovEstqPercRedII = 0;
                            itemMovEstq.ItMovEstqValRedII = 0;
                            itemMovEstq.ItMovEstqBaseIIRed = 0;
                            itemMovEstq.ItMovEstqValIIOrig = 0;
                            itemMovEstq.ItMovEstqPercRedIPI = 0;
                            itemMovEstq.ItMovEstqValRedIPI = 0;
                            itemMovEstq.ItMovEstqValIPIOrig = 0;
                            itemMovEstq.ItMovEstqPercRedISS = 0;
                            itemMovEstq.ItMovEstqValRedISS = 0;
                            itemMovEstq.ItMovEstqBaseISSRed = 0;
                            itemMovEstq.ItMovEstqValISSOrig = 0;
                            itemMovEstq.ItMovEstqValINSSOrig = 0;
                            itemMovEstq.ItMovEstqValDescIcms = 0;
                            itemMovEstq.ItMovEstqValReembolso = 0;
                            itemMovEstq.ItMovEstqPercDiferimICMS = 0;
                            itemMovEstq.ItMovEstqValDiferimIcms = 0;
                            itemMovEstq.ItMovEstqValICMSDevido = 0;
                            itemMovEstq.ItMovEstqValCredPresumICMS = 0;
                            itemMovEstq.ItMovEstqValICMSRecolher = 0;
                            itemMovEstq.ItMovEstqQtdProdST = 0;
                            itemMovEstq.ItMovEstqQtdCalcProdST = 0;
                            itemMovEstq.ItMovEstqValGlosa = 0;
                            itemMovEstq.ItMovEstqPercIcmsExonerado = 0;
                            itemMovEstq.ItMovEstqValBaseIcmsOper = 0;
                            itemMovEstq.ItMovEstqPercIcmsOper = 0;
                            itemMovEstq.ItMovEstqValIcmsOper = 0;
                            itemMovEstq.ItMovEstqPrecoVendaVarejo = 0;
                            itemMovEstq.ItMovEstqValIcmsRetSTRecX = 0;
                            itemMovEstq.ItMovEstqValIcmsRecX = 0;
                            itemMovEstq.ItMovEstqBaseIcmsX = 0;
                            itemMovEstq.ItMovEstqQtdBaseIpiPauta = 0;
                            itemMovEstq.ItMovEstqValUnitIpiPauta = 0;
                            itemMovEstq.ItMovEstqQtdBasePisPauta = 0;
                            itemMovEstq.ItMovEstqValUnitPisPauta = 0;
                            itemMovEstq.ItMovEstqQtdBaseCofinsPauta = 0;
                            itemMovEstq.ItMovEstqValUnitCofinsPauta = 0;
                            itemMovEstq.ItMovEstqQtdRed = 0;
                            itemMovEstq.ItMovEstqQtdCalcRed = 0;
                            itemMovEstq.ItMovEstqQtdDesmCalc = 0;
                            itemMovEstq.ItMovEstqBaseFunrural = 0;
                            itemMovEstq.ItMovEstqPercFunrural = 0;
                            itemMovEstq.ItMovEstqValFunrural = 0;
                            itemMovEstq.ItMovEstqBaseSegCustoMed = 0;
                            itemMovEstq.ItMovEstqSegCustoUnit = 0;
                            itemMovEstq.ItMovEstqSegCustoUnitSegInd = 0;
                            itemMovEstq.ItMovEstqBaseSegCUnitSegInd = 0;
                            itemMovEstq.ItMovEstqPesoCubado = 0;
                            itemMovEstq.ItMovEstqValDedISS = 0;
                            itemMovEstq.ItMovEstqBaseISSDeduz = 0;
                            itemMovEstq.ItMovEstqValUnitProd = 0;
                            itemMovEstq.ItMovEstqValBasePisRF = 0;
                            itemMovEstq.ItMovEstqValBaseCofinsRF = 0;
                            itemMovEstq.ItMovEstqValDescIcmsZFM = 0;
                            itemMovEstq.ItMovEstqValDescPisZFM = 0;
                            itemMovEstq.ItMovEstqValDescCofinsZFM = 0;
                            itemMovEstq.ItMovEstqBaseIcmsRedDest = 0;
                            itemMovEstq.ItMovEstqPercRedBaseIcmsDest = 0;
                            itemMovEstq.ItMovEstqMargLucroSTDemo = 0;
                            itemMovEstq.ItMovEstqPrecoListaSTDemo = 0;
                            itemMovEstq.ItMovEstqValBaseIcmsSTDemo = 0;
                            itemMovEstq.ItMovEstqPercIcmsSTDemo = 0;
                            itemMovEstq.ItMovEstqValIcmsStDemo = 0;
                            itemMovEstq.ItMovEstqValIcmsRetSTDemo = 0;
                            itemMovEstq.ITMOVESTQQTDRECINTSIS = 0;
                            itemMovEstq.ItMovEstqTxaMarMerc = 0;
                            itemMovEstq.ItMovEstqValIcmsTxaMarMerc = 0;
                            itemMovEstq.ItMovEstqQtdFCI = 0;
                            itemMovEstq.ItMovEstqQtdCalcFCI = 0;
                            itemMovEstq.ItMovEstqSegCustoUnitSegInd = 0;
                            itemMovEstq.ItMovEstqBaseCMedSegIndEcon = 0;
                            itemMovEstq.ItMovEstqEntCodOrig = movestq.EntCod;
                            itemMovEstq.ItMovEstqValCap = 0;
                            itemMovEstq.ItMovEstqRatRecAntIcmsST = "Não";
                            itemMovEstq.ItMovEstqRedII = "Nenhum";
                            itemMovEstq.ItMovEstqRedIPI = "Nenhum";
                            itemMovEstq.ItMovEstqBaseIPIRed = itemMovEstq.ItMovEstqBaseIcms;
                            itemMovEstq.ItMovEstqRedICMS = "Base de Cálculo";
                            itemMovEstq.ItMovEstqRedISS = "Nenhum";
                            itemMovEstq.ItMovEstqRedINSS = "Nenhum";
                            itemMovEstq.ItMovEstqItContratoSeq = 1;
                            itemMovEstq.ItMovEstqItContratoVersaoNum = 1;
                            itemMovEstq.ItMovEstqDaeGnrePago = "Não";
                            itemMovEstq.ItMovEstqDeduzValIcmsRetSt = "Não";
                            itemMovEstq.ItMovEstqGeraRMLaudo = "Não";
                            itemMovEstq.ItMovEstqValPedagio = 0;
                            itemMovEstq.ItMovEstqValSeloCtrl = 0;
                            //itemMovEstq.TribBModBCCod = "3";
                            itemMovEstq.ItMovEstqAtualizaFicTecProd = "Nenhum";
                            itemMovEstq.ItMovEstqFatCalcImpComod = 0;
                            itemMovEstq.ItMovEstqNumFCI = "";
                            itemMovEstq.ItMovEstqPercICMSIntUFDest = 0;
                            itemMovEstq.ItMovEstqPercICMSInterest = 0;
                            itemMovEstq.ItMovEstqValICMSPartUFDest = 0;
                            itemMovEstq.ItMovEstqValICMSPartUFRem = 0;
                            itemMovEstq.ITMOVESTQCUSTOUNITICMSREC = itemMovEstq.ItMovEstqCustoUnit;
                            itemMovEstq.ItMovEstqBaseFCPIcms = 0;
                            itemMovEstq.ItMovEstqPercFCPIcms = 0;
                            itemMovEstq.ItMovEstqBaseFCPIcmsST = 0;
                            itemMovEstq.ItMovEstqPercFCPIcmsST = 0;
                            itemMovEstq.ItMovEstqValServPrestSeg15 = 0;
                            itemMovEstq.ItMovEstqValServPrestSeg20 = 0;
                            itemMovEstq.ItMovEstqValServPrestSeg25 = 0;
                            itemMovEstq.ItMovEstqValAdicServPrestSeg = 0;
                            itemMovEstq.ItMovEstqValAdicNaoRetServ = 0;
                            itemMovEstq.ItMovEstqPercInssNaoDev = 0;
                            itemMovEstq.ItMovEstqValBaseInssNaoDev = 0;
                            itemMovEstq.ItMovEstqValInssNaoDev = 0;
                            itemMovEstq.ItMovEstqValBaseIrrfNaoDev = 0;
                            itemMovEstq.ItMovEstqPercIrrfNaoDev = 0;
                            itemMovEstq.ItMovEstqValIrrfNaoDev = 0;
                            itemMovEstq.ItMovEstqValBasePisNaoDev = 0;
                            itemMovEstq.ItMovEstqPercPisNaoDev = 0;
                            itemMovEstq.ItMovEstqValPisNaoDev = 0;
                            itemMovEstq.ItMovEstqValBaseCofins = 0;
                            itemMovEstq.ItMovEstqValCofinsNaoDev = 0;
                            itemMovEstq.ItMovEstqPercCofinsNaoDev = 0;
                            itemMovEstq.ItMovEstqValCsllNaoDev = 0;
                            itemMovEstq.ItMovEstqPercCsllNaoDev = 0;
                            itemMovEstq.ItMovEstqValCsllNaoDev = 0;

                            itemMovEstq.ItMovEstqSeqNF = 1;

                            #endregion

                            bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                            #endregion

                            #region LOC_ARMAZ_ITEM_MOV_ESTQ

                            LOC_ARMAZ_ITEM_MOV_ESTQ locaArmaz = servico.InsereLocalArmazenagem(itemMovEstq.MovEstqChv,
                                itemMovEstq.EmpCod, sequencia, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqQtdCalcProd, 
                                itemMovEstq.ItMovEstqQtdProd, locArmaz);

                            bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locaArmaz);

                            #endregion

                            #region Carrega Dados Conta

                            if (tabelaConfigNFe.ContaDebito == "Produto")
                            {
                                Models.PROD_CONTA_CONTAB prodContaContab = apolo.PROD_CONTA_CONTAB
                                    .Where(w => w.EmpCod == empresa.EmpCod
                                        && w.ProdCodEstr == itemMovEstq.ProdCodEstr)
                                    .FirstOrDefault();

                                if (prodContaContab == null)
                                    return "Não existe conta contábil no produto " + produtoObj.ProdCodEstr + " para a empresa "
                                        + empresa.EmpCod + "! Verifique!";

                                Models.CLASSE_REC_DESP_CTA_CONTAB classeCC = apolo.CLASSE_REC_DESP_CTA_CONTAB
                                    .Where(w => w.EmpCod == empresa.EmpPlanoCta
                                        && w.PlanoCtaCodRed == prodContaContab.ProdCContabDeb).FirstOrDefault();

                                if (classeCC == null)
                                    return "Não existe classe de receita / despesa vinculada a conta contábil " + prodContaContab.ProdCContabDeb + " para a empresa "
                                        + empresa.EmpCod + "! Verifique!";

                                conta = classeCC.ClasseRecDespCodEstr;
                            }

                            #endregion

                            #region MOV_ESTQ_CLASSE_REC_DESP

                            MOV_ESTQ_CLASSE_REC_DESP movEstqCRD = new MOV_ESTQ_CLASSE_REC_DESP();
                            movEstqCRD.EmpCod = movestq.EmpCod;
                            movEstqCRD.MovEstqChv = movestq.MovEstqChv;
                            movEstqCRD.ClasseRecDespCodEstr = conta;
                            movEstqCRD.MovEstqClasseRecDespVal = itemMovEstq.ItMovEstqValProd;
                            movEstqCRD.MovEstqClasseRecDespPerc = Math.Round((1.00m / listaProd.Count()) * 100.00m, 2);

                            bdApolo.MOV_ESTQ_CLASSE_REC_DESP.AddObject(movEstqCRD);

                            #endregion

                            sequencia++;
                        }
                        else
                        {
                            msgRetorno = "O item " + item.Descricao + " não está de acordo com o pedido " + pedCompNum + "!"
                                + " Verifique as configurações de NCM do produto, o saldo do produto no pedido e se o pedido está aprovado!";
                            return msgRetorno;
                        }
                    }

                    #endregion

                    #region PARC_PAG_MOV_ESTQ

                    #region Carrega Parcelas do Pedido

                    List<Models.PARC_PAG_PED_COMP> parcelasPedComp = apolo.PARC_PAG_PED_COMP
                        .Where(w => w.EmpCod == empresa.EmpCod && w.PedCompNum == pedCompNum).ToList();

                    #endregion

                    short seqParcela = 1;
                    
                    foreach (var parcelaIC in parcelasPedComp)
                    {
                        int dias = Convert.ToInt32(parcelaIC.ParcPagPedCompDiasParc);

                        Models.COND_PAG condicaoPag = apolo.COND_PAG
                            .Where(w => w.CondPagCod == condPag).FirstOrDefault();

                        PARC_PAG_MOV_ESTQ parcela = new PARC_PAG_MOV_ESTQ();
                        parcela.EmpCod = movestq.EmpCod;
                        parcela.MovEstqChv = movestq.MovEstqChv;
                        parcela.ParcPagMovEstqSeq = seqParcela;
                        parcela.ParcPagMovEstqEspec = movestq.MovEstqDocEspec;
                        parcela.ParcPagMovEstqSerie = movestq.MovEstqDocSerie;
                        parcela.ParcPagMovEstqNum = movestq.MovEstqDocNum + "-A";
                        parcela.ParcPagMovEstqDataEmissao = movestq.MovEstqDataEmissao;
                        if (condicaoPag.CondPagTipoParc == "Percentual")
                            parcela.ParcPagMovEstqVal = valorTotalXML * (parcelaIC.ParcPagPedCompPercFrac / 100.00m);
                        else
                            parcela.ParcPagMovEstqVal = valorTotalXML / parcelaIC.ParcPagPedCompPercFrac;
                        if (dias > 0)
                            parcela.ParcPagMovEstqDataVenc = Convert.ToDateTime(movestq.MovEstqDataEmissao).AddDays(dias);
                        else
                            parcela.ParcPagMovEstqDataVenc = parcelaIC.ParcPagPedCompDataVenc;
                        parcela.ParcPagMovEstqValPag = 0;
                        parcela.ParcPagMovEstqDataProrrog = parcela.ParcPagMovEstqDataVenc;

                        bdApolo.PARC_PAG_MOV_ESTQ.AddObject(parcela);

                        seqParcela++;
                    }

                    #endregion

                    #endregion

                    #region Insere LOG_MOV_ESTQ

                    LOG_MOV_ESTQ logMovEstq = new LOG_MOV_ESTQ();

                    ObjectParameter chave = new ObjectParameter("codigo", typeof(global::System.String));
                    Apolo10EntitiesService apoloI = new Apolo10EntitiesService();
                    apoloI.gerar_codigo(movestq.EmpCod, "LOG_MOV_ESTQ", chave);

                    logMovEstq.LogMovEstqSeq = Convert.ToInt32(chave.Value);
                    logMovEstq.LogMovEstqUsuCod = movestq.UsuCod;
                    logMovEstq.LogMovEstqDataHora = DateTime.Now;
                    logMovEstq.LogMovEstqEmpCod = movestq.EmpCod;
                    logMovEstq.LogMovEstqChv = movestq.MovEstqChv;
                    logMovEstq.LogMovEstqOper = "Inclusão";
                    logMovEstq.LogMovEstqDocEspec = movestq.MovEstqDocEspec;
                    logMovEstq.LogMovEstqDocSerie = movestq.MovEstqDocSerie;
                    logMovEstq.LogMovEstqDocNum = movestq.MovEstqDocNum;
                    logMovEstq.LogMovEstqObs = movestq.MovEstqObs;

                    bdApolo.LOG_MOV_ESTQ.AddObject(logMovEstq);

                    #endregion

                    #region Salva e Atualiza Valores MOV_ESTQ

                    bdApolo.SaveChanges();

                    bdApolo.calcula_mov_estq(movestq.EmpCod, movestq.MovEstqChv);

                    #endregion

                    #region Integra com o Estoque

                    var listaItensMovEstq = apolo.ITEM_MOV_ESTQ
                        .Where(w => w.EmpCod == movestq.EmpCod
                            && w.MovEstqChv == movestq.MovEstqChv).ToList();

                    foreach (var item in listaItensMovEstq)
                    {
                        bdApolo.atualiza_saldoestqdata(movestq.EmpCod, movestq.MovEstqChv, item.ProdCodEstr, item.ItMovEstqSeq,
                            item.ItMovEstqDataMovimento, "INS");
                    }

                    #endregion

                    #region Integra com Financeiro

                    bdApolo.integ_estoque_financ_ins(movestq.MovEstqChv, movestq.EmpCod);

                    #endregion

                    #region Integra com o Fiscal

                    ObjectParameter empP = new ObjectParameter("empcod", movestq.EmpCod);
                    ObjectParameter msg = new ObjectParameter("msg", "");
                    bdApolo.INTEG_ESTQ_FISCAL(empP, movestq.MovEstqChv, usuario, msg);

                    if (msg.Value != "")
                    {
                        msgRetorno = "Integração Fiscal não pode ser feita na movimentação  "
                            + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            + msg.Value;
                        return msgRetorno;
                    }

                    #endregion

                    #region Integra com o Contábil

                    ObjectParameter vContaBloqueada = new ObjectParameter("vContaBloqueada", "");
                    ObjectParameter vMensagem = new ObjectParameter("vMensagem", "");
                    ObjectParameter vValorDebCredInv = new ObjectParameter("vValorDebCredInv", "");
                    ObjectParameter vStatus = new ObjectParameter("vStatus", "");
                    ObjectParameter vAnoMesRelac = new ObjectParameter("vAnoMesRelac", "");
                    ObjectParameter vSequenciaRelac = new ObjectParameter("vSequenciaRelac", 0);

                    /*
                     * 03/12/2018 - Trocada a procedure da integração de acordo com as atualizações da Riosoft. 
                     */

                    //bdApolo.VERIFICAR_LANC_CONTABIL(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
                    //    movestq.MovEstqDocEspec, movestq.MovEstqDocSerie, movestq.MovEstqDocNum, movestq.MovEstqChv,
                    //    0, 0, vContaBloqueada, vMensagem, vValorDebCredInv, vStatus, "", "",
                    //    vAnoMesRelac, vSequenciaRelac);

                    bdApolo.INTEGRAR_LANC_CONTAB(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
                        movestq.MovEstqDocEspec, movestq.MovEstqDocSerie, movestq.MovEstqDocNum, movestq.MovEstqChv,
                        0, 0, vContaBloqueada, vMensagem, vValorDebCredInv, vStatus, "", "",
                        vAnoMesRelac, vSequenciaRelac);

                    if (vMensagem.Value != "")
                    {
                        msgRetorno = "Integração Contábil não pode ser feita na movimentação "
                            + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            + vMensagem.Value;
                        return msgRetorno;
                    }

                    #endregion
                }
                else
                {
                    msgRetorno = "Arquivo não pode ser importado! Verifique!";
                    return msgRetorno;
                }

                #endregion

                return msgRetorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                if (ex.InnerException == null)
                    msgRetorno = "Erro ao importar arquivo: " + ex.Message;
                else
                    msgRetorno = "Erro ao importar arquivo: " + ex.Message
                        + " / Erro Interno: " + ex.InnerException.Message;

                msgRetorno = msgRetorno + " / Linha Erro: " + linenum.ToString();

                return msgRetorno;
            }
        }

        #endregion

        #region Other Méthods

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

        public DateTime CalculaVencimento(DateTime dataEmissao, string entCod)
        {
            Apolo10EntitiesService bdApolo = new Apolo10EntitiesService();

            DateTime dataVencimento = new DateTime();
            short? diaVencimento = 0;
            //int mesVencimento = 0;
            DateTime dataNova = new DateTime();

            List<ENT_OBJ> listCalculoParcela = bdApolo.ENT_OBJ
                .Where(w => w.EntCod == entCod
                    && bdApolo.OBJETO.Any(o => o.ObjCodEstr == o.ObjCodEstr
                        && o.ObjNome == "PARCELA")).ToList();

            if (listCalculoParcela.Count > 0)
            {
                foreach (var item in listCalculoParcela)
                {
                    if (dataEmissao.Day >= item.USERParcelaDiaInicial
                        && dataEmissao.Day <= item.USERParcelaDiaFinal)
                    {
                        diaVencimento = item.USERParcelaDiaVencimento;

                        if (item.USERParcelaMesVencimento.Equals("Próximo"))
                            //mesVencimento = dataEmissao.AddMonths(1).Month;
                            dataNova = dataEmissao.AddMonths(1);
                        else
                            //mesVencimento = dataEmissao.Month;
                            dataNova = dataEmissao;
                    }
                }
                dataVencimento = Convert.ToDateTime(
                    //diaVencimento.ToString() + "/" + mesVencimento.ToString() + "/" + dataEmissao.Year.ToString());
                    diaVencimento.ToString() + "/" + dataNova.Month.ToString() + "/" + dataNova.Year.ToString());
            }
            else
            {
                dataVencimento = dataEmissao.AddDays(7);
            }

            return dataVencimento;
        }

        public List<SelectListItem> CarregaListaConfiguracaoNFe()
        {
            List<SelectListItem> ddlConfigNFe = new List<SelectListItem>();

            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            var lista = hlbapp.Configuracao_Importa_NFe
                .OrderBy(o => o.Descricao)
                .ToList();

            foreach (var item in lista)
            {
                ddlConfigNFe.Add(new SelectListItem
                {
                    Text = item.Descricao,
                    Value = item.ID.ToString(),
                    Selected = false
                });
            }

            return ddlConfigNFe;
        }

        public List<SelectListItem> CarregaFinalidadesCTe()
        {
            List<SelectListItem> ddl = new List<SelectListItem>();

            ddl.Add(new SelectListItem { Text = "(Selecione uma finalidade para o CT-e)", Value = "", Selected = false });
            ddl.Add(new SelectListItem { Text = "OVOS INCUBATÓRIO NOVA GRANADA", Value = "OVOS INCUBATÓRIO NOVA GRANADA", Selected = false });
            ddl.Add(new SelectListItem { Text = "OVOS INCUBATÓRIO NOVO MUNDO", Value = "OVOS INCUBATÓRIO NOVO MUNDO", Selected = false });
            ddl.Add(new SelectListItem { Text = "OVOS INCUBATÓRIO HYGEN", Value = "OVOS INCUBATÓRIO HYGEN", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO PROD. OB", Value = "RAÇÃO PROD. OB", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO PROD. CD. GOMES", Value = "RAÇÃO PROD. CD. GOMES", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO RECRIA BWK", Value = "RAÇÃO RECRIA BWK", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO PROD. BWK", Value = "RAÇÃO PROD. BWK", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO RECRIA OB", Value = "RAÇÃO RECRIA OB", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO RECRIA TB", Value = "RAÇÃO RECRIA TB", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO RECRIA BP", Value = "RAÇÃO RECRIA BP", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO RECRIA UBE", Value = "RAÇÃO RECRIA UBE", Selected = false });
            ddl.Add(new SelectListItem { Text = "RAÇÃO PROD. UBE", Value = "RAÇÃO PROD. UBE", Selected = false });
            ddl.Add(new SelectListItem { Text = "FRETE DE VENDAS", Value = "FRETE DE VENDAS", Selected = false });

            return ddl;
        }

        public List<SelectListItem> CarregaTipoFreteCTe()
        {
            List<SelectListItem> ddl = new List<SelectListItem>();

            ddl.Add(new SelectListItem { Text = "(Selecione um tipo de frete para o CT-e)", Value = "", Selected = false });
            ddl.Add(new SelectListItem { Text = "Sem Frete", Value = "Sem Frete", Selected = false });
            ddl.Add(new SelectListItem { Text = "Destinatário", Value = "Destinatário", Selected = false });
            ddl.Add(new SelectListItem { Text = "Emitente", Value = "Emitente", Selected = false });
            ddl.Add(new SelectListItem { Text = "Terceiros", Value = "Terceiros", Selected = false });
            ddl.Add(new SelectListItem { Text = "Prop. Dest", Value = "Prop. Dest", Selected = false });
            ddl.Add(new SelectListItem { Text = "Prop. Remet", Value = "Prop. Remet", Selected = false });

            return ddl;
        }

        #endregion
    }
}
