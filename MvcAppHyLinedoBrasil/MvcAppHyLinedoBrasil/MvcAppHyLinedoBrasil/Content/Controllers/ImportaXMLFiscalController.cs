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

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class ImportaXMLFiscalController : Controller
    {
        #region Variaveis

        Apolo10EntitiesService bdApolo = new Apolo10EntitiesService();
        ImportaIncubacaoService servico = new ImportaIncubacaoService();

        #endregion

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

            return View();
        }

        [HttpPost]
        public ActionResult ImportaXML(string data)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            string msgRetorno = "";
            MvcAppHyLinedoBrasil.Models.SequenciaLinha retorno = new MvcAppHyLinedoBrasil.Models.SequenciaLinha();
            retorno.ID = 1;

            try
            {
                #region Lê arquivo XML

                StreamReader arquivo = new StreamReader(Request.Files[0].InputStream);

                string cnpjEmitente = "";
                string serie = "";
                string numCTE = "";
                DateTime dataEmissao = new DateTime();
                string cnpjRemetente = "";
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
                            foreach (XmlNode ide in item.ChildNodes)
                            {
                                if (ide.Name.Equals("serie")) serie = ide.InnerText;
                                if (ide.Name.Equals("nCT")) numCTE = ide.InnerText;
                                if (ide.Name.Equals("dhEmi")) dataEmissao = 
                                    Convert.ToDateTime(Convert.ToDateTime(ide.InnerText).ToShortDateString());
                            }
                        #endregion

                        if (item.Name.Equals("emit")) cnpjEmitente = item.FirstChild.InnerText;
                        if (item.Name.Equals("rem")) cnpjRemetente = item.FirstChild.InnerText;
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
                                    if (infCTeNorm.FirstChild != null)
                                        if (infCTeNorm.FirstChild.FirstChild != null)
                                            chaveNFE = infCTeNorm.FirstChild.FirstChild.InnerText;
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
                    retorno.ID = 0;
                    retorno.Importado = msgRetorno;
                    return View("Arquivo", retorno);
                }

                EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL.Where(w => w.EmpCpfCgc == cnpjRemetente)
                    .FirstOrDefault();

                if (empresa == null)
                {
                    //ViewBag.Erro = "CNPJ " + cnpjRemetente + " da Empresa não cadastrado no Apolo! Verifique!";
                    msgRetorno = "CNPJ " + cnpjRemetente + " da Empresa não cadastrado no Apolo! Verifique!";
                    retorno.ID = 0;
                    retorno.Importado = msgRetorno;
                    return View("Arquivo", retorno);
                }

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
                if (tribBICMS != "")
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

                    NOTA_FISCAL_ELETRONICA_TRANS nfe = bdApolo.NOTA_FISCAL_ELETRONICA_TRANS
                        .Where(w => w.NFETransChvAcesso == chaveNFE).FirstOrDefault();

                    if (nfe != null)
                    {
                        #region Carrega Configurações Apolo

                        //entCod = nfe.EntCod;

                        ENTIDADE emitente = bdApolo.ENTIDADE
                            .Where(w => w.EntCpfCgc == cnpjEmitente && w.EntNat == "Transportador"
                                && !w.StatEntCod.Equals("05"))
                            .FirstOrDefault();
                        entCod = emitente.EntCod;
                        ENTIDADE1 emitente1 = bdApolo.ENTIDADE1
                            .Where(w => w.EntCod == entCod).FirstOrDefault();
                        if (emitente1.USERTipoLancCTE != null && emitente1.USERTipoLancCTE != "")
                            tipoLanc = emitente1.USERTipoLancCTE;

                        ENTIDADE1 entidade1 = bdApolo.ENTIDADE1.Where(w => w.EntCod == nfe.EntCod).FirstOrDefault();
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
                                cFOP = "1.352.001";
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
                            cFOP = "1.352.001";
                        }

                        if (entidade1.USERCTeConta != null)
                            if (!entidade1.USERCTeConta.Equals(""))
                                conta = entidade1.USERCTeConta;
                            else
                                conta = "3.988";
                        else
                            conta = "3.988";
                        if (entidade1.USERCTeRateio != null)
                            if (!entidade1.USERCTeRateio.Equals(""))
                                rateio = entidade1.USERCTeRateio;
                            else
                            {
                                if (empresa.EmpNome.Contains("HY-LINE"))
                                    rateio = "3.03.0001";
                                else if (empresa.EmpNome.Contains("LOHMANN"))
                                    rateio = "3.03.0021";
                                else
                                    rateio = "3.03.0024";
                            }
                        else
                        {
                            if (empresa.EmpNome.Contains("HY-LINE"))
                                rateio = "3.03.0001";
                            else if (empresa.EmpNome.Contains("LOHMANN"))
                                rateio = "3.03.0021";
                            else
                                rateio = "3.03.0024";
                        }

                        if (valICMS == 0)
                        {
                            if (emitente1.USERCTeAliqICMS != null)
                                if (!emitente1.USERCTeAliqICMS.Equals(""))
                                {
                                    percICMS = Convert.ToDecimal(emitente1.USERCTeAliqICMS);
                                    tribCod = "000";
                                    tribBICMS = "00";
                                    valBaseICMS = valor;
                                    valICMS = valor * (percICMS / 100.00m);
                                }
                        }

                        #endregion

                        #region Insere Movimentação de Estoque no Apolo

                        MOV_ESTQ movestq = servico.InsereMovEstq(empresa.EmpCod, tipoLanc, entCod, dataEmissao,
                            usuario);

                        if (data != null)
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
                        movestq.MovEstqObs = "XML Importado via Sistema WEB";
                        movestq.MovEstqPercDescGer = 0;
                        movestq.MovEstqPercDescGerProd = 0;
                        movestq.MovEstqPercDescGerServ = 0;
                        movestq.MovEstqValDescGer = 0;
                        movestq.MovEstqValDescGerProd = 0;
                        movestq.MovEstqValDescGerServ = 0;
                        movestq.MovEstq = "Não";

                        bdApolo.MOV_ESTQ.AddObject(movestq);

                        MOV_ESTQ_NF_ELETRONICA chaveMov = new MOV_ESTQ_NF_ELETRONICA();
                        chaveMov.EmpCod = movestq.EmpCod;
                        chaveMov.MovEstqChv = movestq.MovEstqChv;
                        chaveMov.UsuCod = usuario;
                        chaveMov.MovEstqNFEImpData = movestq.MovEstqDataMovimento;
                        chaveMov.MovEstqNFEImpChvAcesso = chaveCTE;
                        chaveMov.MovEstqNFEImpStatus = "Importado";

                        bdApolo.MOV_ESTQ_NF_ELETRONICA.AddObject(chaveMov);

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

                        LOC_ARMAZ_ITEM_MOV_ESTQ locaArmaz = servico.InsereLocalArmazenagem(itemMovEstq.MovEstqChv,
                            itemMovEstq.EmpCod, sequencia, itemMovEstq.ProdCodEstr, quantidade, locArmaz);

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
                        parcela.ParcPagMovEstqDataVenc = CalculaVencimento(dataEmissao, movestq.EntCod);
                        parcela.ParcPagMovEstqValPag = 0;
                        parcela.ParcPagMovEstqDataProrrog = parcela.ParcPagMovEstqDataVenc;

                        bdApolo.PARC_PAG_MOV_ESTQ.AddObject(parcela);

                        RATEIO_MOV_ESTQ rateioME = new RATEIO_MOV_ESTQ();
                        rateioME.EmpCod = movestq.EmpCod;
                        rateioME.MovEstqChv = movestq.MovEstqChv;
                        rateioME.ClasseRecDespCodEstr = conta;
                        rateioME.CCtrlCodEstr = rateio;
                        rateioME.RatMovEstqVal = valor;
                        rateioME.RatMovEstqPerc = 100;

                        bdApolo.RATEIO_MOV_ESTQ.AddObject(rateioME);

                        MOV_ESTQ_CLASSE_REC_DESP movEstqCRD = new MOV_ESTQ_CLASSE_REC_DESP();
                        movEstqCRD.EmpCod = movestq.EmpCod;
                        movEstqCRD.MovEstqChv = movestq.MovEstqChv;
                        movEstqCRD.ClasseRecDespCodEstr = conta;
                        movEstqCRD.MovEstqClasseRecDespVal = valor;
                        movEstqCRD.MovEstqClasseRecDespPerc = 100;

                        bdApolo.MOV_ESTQ_CLASSE_REC_DESP.AddObject(movEstqCRD);

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

                        movEstqChv = movestq.MovEstqChv;
                        empresaStr = movestq.EmpCod;

                        bdApolo.SaveChanges();

                        bdApolo.integ_estoque_financ_ins(movestq.MovEstqChv, movestq.EmpCod);

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
                            retorno.ID = 0;
                            retorno.Importado = msgRetorno;
                            return View("Arquivo", retorno);
                        }

                        ObjectParameter vContaBloqueada = new ObjectParameter("vContaBloqueada", "");
                        ObjectParameter vMensagem = new ObjectParameter("vMensagem", "");
                        ObjectParameter vValorDebCredInv = new ObjectParameter("vValorDebCredInv", "");
                        ObjectParameter vStatus = new ObjectParameter("vStatus", "");
                        ObjectParameter vAnoMesRelac = new ObjectParameter("vAnoMesRelac", "");
                        ObjectParameter vSequenciaRelac = new ObjectParameter("vSequenciaRelac", 0);
                        bdApolo.VERIFICAR_LANC_CONTABIL(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
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
                            retorno.ID = 0;
                            retorno.Importado = msgRetorno;
                            return View("Arquivo", retorno);
                        }

                        //bdApolo.calcula_mov_estq(movestq.EmpCod, movestq.MovEstqChv);

                        //bdApolo.SaveChanges();
                    }
                    else
                    {
                        msgRetorno = "NF-e " + chaveNFE + " não cadastrada no Apolo! Verifique!";
                        //ViewBag.Erro = msgRetorno;
                        retorno.ID = 0;
                        retorno.Importado = msgRetorno;
                        return View("Arquivo", retorno);
                    }

                    #endregion
                }
                else if (chaveCTEComp != "")
                {
                    #region CT-e Complementar

                    MOV_ESTQ_NF_ELETRONICA cteCompMov = bdApolo.MOV_ESTQ_NF_ELETRONICA
                        .Where(w => w.MovEstqNFEImpChvAcesso == chaveCTEComp).FirstOrDefault();

                    if (cteCompMov == null)
                    {
                        msgRetorno = "CT-e Principal " + chaveCTE + " não cadastrado no Apolo! "
                            + "Para inserir complemento é necessário o principal inserido!";
                        //ViewBag.Erro = msgRetorno;
                        retorno.ID = 0;
                        retorno.Importado = msgRetorno;
                        return View("Arquivo", retorno);
                    }

                    #region Insere Movimentação de Estoque no Apolo

                    MOV_ESTQ movestqCtePrincipal = bdApolo.MOV_ESTQ.Where(w => w.EmpCod == cteCompMov.EmpCod
                        && w.MovEstqChv == cteCompMov.MovEstqChv).FirstOrDefault();

                    //valBaseICMS = Convert.ToDecimal(movestqCtePrincipal.MovEstqBaseIcms);
                    //valICMS = Convert.ToDecimal(movestqCtePrincipal.MovEstqValIcms);

                    MOV_ESTQ movestq = servico.InsereMovEstq(empresa.EmpCod, tipoLanc, movestqCtePrincipal.EntCod,
                        dataEmissao, usuario);

                    if (data != null)
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
                    movestq.MovEstqObs = "XML Importado via Sistema WEB";
                    movestq.MovEstqPercDescGer = 0;
                    movestq.MovEstqPercDescGerProd = 0;
                    movestq.MovEstqPercDescGerServ = 0;
                    movestq.MovEstqValDescGer = 0;
                    movestq.MovEstqValDescGerProd = 0;
                    movestq.MovEstqValDescGerServ = 0;

                    movestq.MovEstq = "Não";

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
                        Convert.ToDecimal(locPrin.LocArmazItMovEstqQtd), locPrin.LocArmazCodEstr);

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

                    movEstqChv = movestq.MovEstqChv;
                    empresaStr = movestq.EmpCod;

                    bdApolo.SaveChanges();

                    bdApolo.integ_estoque_financ_ins(movestq.MovEstqChv, movestq.EmpCod);

                    ObjectParameter empP = new ObjectParameter("empcod", movestq.EmpCod);
                    ObjectParameter msg = new ObjectParameter("msg", "");
                    bdApolo.INTEG_ESTQ_FISCAL(empP, movestq.MovEstqChv, usuario, msg);

                    if (msg.Value != "")
                    {
                        msgRetorno = "Integração Fiscal não pode ser feita na movimentação  "
                            + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            + msg.Value;
                        //ViewBag.Erro = msgRetorno;
                        retorno.ID = 0;
                        retorno.Importado = msgRetorno;
                        return View("Arquivo", retorno);
                    }

                    ObjectParameter vContaBloqueada = new ObjectParameter("vContaBloqueada", "");
                    ObjectParameter vMensagem = new ObjectParameter("vMensagem", "");
                    ObjectParameter vValorDebCredInv = new ObjectParameter("vValorDebCredInv", "");
                    ObjectParameter vStatus = new ObjectParameter("vStatus", "");
                    ObjectParameter vAnoMesRelac = new ObjectParameter("vAnoMesRelac", "");
                    ObjectParameter vSequenciaRelac = new ObjectParameter("vSequenciaRelac", 0);
                    bdApolo.VERIFICAR_LANC_CONTABIL(usuario, "Estoque", "", movestq.TipoLancCod, movestq.EmpCod,
                        movestq.MovEstqDocEspec, movestq.MovEstqDocSerie, movestq.MovEstqDocNum, movestq.MovEstqChv,
                        0, 0, vContaBloqueada, vMensagem, vValorDebCredInv, vStatus, "", "",
                        vAnoMesRelac, vSequenciaRelac);

                    if (vMensagem.Value != "")
                    {
                        msgRetorno = "Integração Contábil não pode ser feita na movimentação "
                            + movestq.MovEstqChv + " da empresa " + movestq.EmpCod + ": "
                            + vMensagem.Value;
                        //ViewBag.Erro = msgRetorno;
                        retorno.ID = 0;
                        retorno.Importado = msgRetorno;
                        return View("Arquivo", retorno);
                    }

                    #endregion
                }
                else
                {
                    msgRetorno = "Arquivo não pode ser importado! Verifique!";
                    //ViewBag.Erro = msgRetorno;
                    retorno.ID = 0;
                    retorno.Importado = msgRetorno;
                    return View("Arquivo", retorno);
                }

                #endregion

                msgRetorno = "XML de chave " + chaveCTE + " importado com sucesso! "
                    + "Gerada a chave " + movEstqChv.ToString() + " na empresa " + empresaStr + ".";
                //ViewBag.OK = msgRetorno;
                retorno.Importado = msgRetorno;
                return View("Arquivo", retorno);
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

                retorno.ID = 0;
                retorno.Importado = msgRetorno;
                return View("Arquivo", retorno);
            }
        }

        public DateTime CalculaVencimento(DateTime dataEmissao, string entCod)
        {
            DateTime dataVencimento = new DateTime();
            short? diaVencimento = 0;
            int mesVencimento = 0;

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
                            mesVencimento = dataEmissao.AddMonths(1).Month;
                        else
                            mesVencimento = dataEmissao.Month;
                    }
                }
                dataVencimento = Convert.ToDateTime(
                    diaVencimento.ToString() + "/" + mesVencimento.ToString() + "/" + dataEmissao.Year.ToString());
            }
            else
            {
                dataVencimento = dataEmissao.AddDays(7);
            }

            return dataVencimento;
        }
    }
}
