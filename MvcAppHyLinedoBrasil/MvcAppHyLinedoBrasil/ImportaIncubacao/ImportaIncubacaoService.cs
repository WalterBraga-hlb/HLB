using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using ImportaIncubacao.Data.HATCHERY_EGG_DATA;
using ImportaIncubacao.Data;
using ImportaIncubacao.Data.FLIPDataSetTableAdapters;
using ImportaIncubacao.Data.Apolo;
using System.Data.Objects;
using System.Globalization;
using ImportaIncubacao.Data.FLIP;

namespace ImportaIncubacao
{
    public partial class ImportaIncubacaoService : ServiceBase
    {
        #region Objetos

        private Timer _oTimer;
        private Timer _oTimer2;

        HLBAPPEntities bdSQLServer = new HLBAPPEntities();
        public Apolo10EntitiesService bdApolo = new Apolo10EntitiesService();

        FLIPDataSet flipDataSet = new FLIPDataSet();

        SETDAY_DATATableAdapter setDayData = new SETDAY_DATATableAdapter();
        HATCHERY_FLOCK_DATATableAdapter hatcheryFlockData = new HATCHERY_FLOCK_DATATableAdapter();
        HATCHERY_EGG_DATATableAdapter hatcheryEggData = new HATCHERY_EGG_DATATableAdapter();
        EGGINV_DATATableAdapter eggInvData = new EGGINV_DATATableAdapter();
        FLOCKSTableAdapter flocks = new FLOCKSTableAdapter();
        FLOCK_DATATableAdapter flockData = new FLOCK_DATATableAdapter();
        FLOCKS_DATATableAdapter flocksData = new FLOCKS_DATATableAdapter();
        FARMS_IMPORTTableAdapter farms = new FARMS_IMPORTTableAdapter();

        AVG_LST4WK_HATCHTableAdapter avgLst4WkHatch = new AVG_LST4WK_HATCHTableAdapter();

        public string lote;
        public int? qtdOvosConf;
        public string status;
        public int ID;
        public static int VerificaExecucao;
        public static int VerificaExecucaoPlanalto;

        #endregion

        public ImportaIncubacaoService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // TODO: Add code here to start your service.
                _oTimer = new Timer(3600 * 1000); // de uma em uma hora.
                //_oTimer = new Timer(60 * 1000);
                _oTimer.Elapsed += Atualizacao_Tick;
                _oTimer.Enabled = true;
                _oTimer.Start();

                //_oTimer = new Timer(3600 * 1000); // de um em um minuto.
                _oTimer2 = new Timer(1 * 1000);
                _oTimer2.Elapsed += AtualizaMinuto_Tick;
                _oTimer2.Enabled = true;
                _oTimer2.Start();
                VerificaExecucao = 0;
                VerificaExecucaoPlanalto = 0;
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry("Erro ao Iniciar o Serviço: " + ex.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
        }

        private void Atualizacao_Tick(object sender, EventArgs e)
        {
            string erro = "";

            try
            {
                AtualizaTodasIncubacoesWEBparaFLIP();
                //erro = AtualizaNascimentosWEBparaFLIPNM();
                erro = RefreshHatchingEggsAll();
                if (erro != "")
                    this.EventLog.WriteEntry("Erro ao realizar Integrações dos Nascimentos do WEB para o FLIP: "
                        + erro, EventLogEntryType.Error, 10);
            }
            catch (Exception ex)
            {
                if (erro == "")
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                    erro = "Linha: " + linenum.ToString();
                    if (ex.InnerException == null)
                        erro = erro + ex.Message;
                    else
                        erro = erro + ex.Message + " / " + ex.InnerException.Message;
                }

                this.EventLog.WriteEntry("Erro ao realizar Integrações do WEB para o FLIP: "
                    + erro, EventLogEntryType.Error, 10);
            }
        }

        public string ImportaIncubacaoFLIP()
        {
            try
            {
                //this.EventLog.WriteEntry("Importação Iniciada.");

                DateTime data = Convert.ToDateTime("30/06/2013");
                //DateTime data = Convert.ToDateTime("14/02/2014");

                var lista = bdSQLServer.HATCHERY_EGG_DATA
                    //.Where(h => h.Set_date > data && h.Status == "Pendente")// && h.Flock_id == "HLP04-P044292W")
                    .Where(h => h.Set_date > data)// && h.Flock_id == "HLP04-P044292W")
                    //.Where(h => h.Set_date <= data && h.Status == "Pendente")//&& h.Flock_id == "HLP03-P034742L")
                    .GroupBy(h => new
                    {
                        h.Company,
                        h.Region,
                        h.Location,
                        h.Set_date,
                        h.Hatch_loc,
                        h.Flock_id,
                        h.Lay_date,
                        h.Machine,
                        h.Track_no
                    })
                    .Select(h => new //HATCHERY_EGG_DATA
                    {
                        type = h.Key,
                        soma = h.Sum(x => x.Eggs_rcvd),
                        estimate = h.Max(x => x.Estimate),
                        observacao = h.Max(x => x.Observacao),
                        Status = h.Max(x => x.Status)
                    })
                    .ToList();

                foreach (var item in lista)
                {
                    lote = item.type.Flock_id;
                    qtdOvosConf = item.soma;
                    status = item.Status;

                    decimal qtdOvosImportados = 0;

                    if (item.Status == "Importado")
                    {
                        qtdOvosImportados = Convert.ToDecimal(hatcheryEggData.QtdOvos(item.type.Company, item.type.Region,
                                item.type.Location, item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id,
                                item.type.Lay_date, item.type.Machine, item.type.Track_no));
                    }

                    if (((item.Status == "Importado") && (qtdOvosImportados != item.soma)) || (item.Status == "Pendente"))
                    {
                        //if ((item.Status == "Importado") && (qtdOvosImportados != item.soma))
                        //{
                            hatcheryEggData.DeleteQuery(item.type.Company, item.type.Region, item.type.Location,
                                item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine,
                                item.type.Track_no);
                        //}

                        int qtdeOvos = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, item.type.Hatch_loc));

                        /**** AJUSTE EGG INVENTORY PARA INCLUIR INCUBAÇÃO ****/

                        if (item.type.Set_date < Convert.ToDateTime("08/02/2014"))
                        //if (item.type.Set_date < Convert.ToDateTime("01/03/2014"))
                        {
                            int tamanho = item.type.Flock_id.Length - 6;

                            if (qtdeOvos == 0)
                            {
                                eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                    item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                    item.type.Track_no, item.type.Lay_date, item.soma, "O", null, null, null, null, null, null, null, null,
                                    item.type.Hatch_loc, null);
                            }
                            else
                            {
                                int qtdeOvosAjuste = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, item.type.Hatch_loc));

                                if (qtdeOvosAjuste < item.soma)
                                {
                                    eggInvData.UpdateQueryEggs(item.soma, item.type.Company, item.type.Region, item.type.Location,
                                        item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                        item.type.Track_no, item.type.Lay_date, "O", item.type.Hatch_loc);
                                }
                            }
                        }
                        /****/

                        qtdeOvos = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, item.type.Hatch_loc));

                        if (qtdeOvos >= item.soma)
                        {
                            // Insere na tabela da Data de Incubação
                            int existe = Convert.ToInt32(setDayData.ExisteSetDayData(item.type.Set_date, item.type.Hatch_loc));

                            if (existe == 0)
                            {
                                decimal sequencia = Convert.ToDecimal(setDayData.UltimaSequenciaSetDayData(item.type.Hatch_loc)) + 1;

                                setDayData.InsertQuery(item.type.Company, item.type.Region, item.type.Location, item.type.Set_date, item.type.Hatch_loc, sequencia);
                            }

                            existe = 0;
                            existe = Convert.ToInt32(hatcheryFlockData.ExisteHatcheryFlockData(item.type.Company, item.type.Region, item.type.Location,
                                item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id));

                            if (existe == 0)
                            {
                                hatcheryFlockData.InsertQuery(item.type.Company, item.type.Region, item.type.Location,
                                    item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.estimate);
                            }

                            //eggInvData.UpdateHatchLoc(item.type.Hatch_loc, item.type.Flock_id, item.type.Track_no,
                            //    item.type.Lay_date, "CH");

                            hatcheryEggData.Insert(item.type.Company, item.type.Region, item.type.Location, item.type.Set_date,
                                item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.soma, null,
                                item.type.Machine, item.type.Track_no, null, null, null, null, null, null,
                                null, null, item.observacao, null);

                            var lista2 = bdSQLServer.HATCHERY_EGG_DATA
                            .Where(h => h.Company == item.type.Company &&
                                h.Region == item.type.Region &&
                                h.Location == item.type.Location &&
                                h.Set_date == item.type.Set_date &&
                                h.Hatch_loc == item.type.Hatch_loc &&
                                h.Flock_id == item.type.Flock_id &&
                                h.Lay_date == item.type.Lay_date &&
                                h.Machine == item.type.Machine &&
                                h.Track_no == item.type.Track_no)
                            .ToList();

                            foreach (var item2 in lista2)
                            {
                                item2.Status = "Importado";
                            }

                            bdSQLServer.SaveChanges();
                        }
                    }
                }

                //this.EventLog.WriteEntry("Importação Concluída.");

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string ImportaIncubacaoEstoqueFuturo()
        {
            try
            {
                var lista = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.ImportadoApolo.Equals("Estoque Futuro")
                        && !h.Status.Equals("Importado"))
                    .OrderBy(o => o.Set_date)
                    .ThenBy(t => t.Flock_id)
                    .ThenBy(t => t.Lay_date)
                    .ToList();

                foreach (var item in lista)
                {
                    int posicaoHifen = item.Flock_id.IndexOf("-") + 1;
                    int tamanho = item.Flock_id.Length - posicaoHifen;
                    string flock = item.Flock_id.Substring(posicaoHifen, tamanho);

                    CTRL_LOTE_LOC_ARMAZ_WEB estoque = bdSQLServer.CTRL_LOTE_LOC_ARMAZ_WEB
                        .Where(w => w.Local == item.Hatch_loc
                            && w.LoteCompleto == flock
                            && w.DataProducao == item.Lay_date)
                        .FirstOrDefault();

                    if (estoque != null)
                    {
                        if (estoque.Qtde >= Convert.ToDecimal(item.Eggs_rcvd))
                        {
                            item.Status = "Importado";
                            bdSQLServer.SaveChanges();
                        }
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void AtualizaIdadeLinhagens()
        {
            DateTime data = Convert.ToDateTime("01/01/2014");
            DateTime dataNascimentoLote;

            var lista = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Set_date >= data)
                .ToList();

            foreach (var item in lista)
            {
                int tamanho = item.Flock_id.Length - 6;
                flocks.FillBy(flipDataSet.FLOCKS, "HYBR", "BR", "PP", item.Flock_id.Substring(0, 5), item.Flock_id.Substring(6, tamanho));
                if (flipDataSet.FLOCKS.Count > 0)
                {
                    dataNascimentoLote = flipDataSet.FLOCKS[0].HATCH_DATE;
                    item.Age = ((item.Lay_date - dataNascimentoLote).Days) / 7;
                    item.Variety = flipDataSet.FLOCKS[0].VARIETY;
                    item.Egg_key = flipDataSet.FLOCKS[0].NUM_1.ToString();
                }
            }

            bdSQLServer.SaveChanges();
        }

        public void ReintegraEggInvComProblema(string location)
        {
            string hatchLoc = "";

            if (location.Equals("PP"))
                hatchLoc = "CH";
            else
                hatchLoc = "PH";

            flockData.FillByLocationWithoutEggInv(flipDataSet.FLOCK_DATA, location);

            for (int i = 0; i < flipDataSet.FLOCK_DATA.Count; i++)
            {
                string trackNO = "EXP" + flipDataSet.FLOCK_DATA[i].TRX_DATE.ToString("yyMMdd");

                eggInvData.Insert(
                    flipDataSet.FLOCK_DATA[i].COMPANY,
                    flipDataSet.FLOCK_DATA[i].REGION,
                    flipDataSet.FLOCK_DATA[i].LOCATION,
                    flipDataSet.FLOCK_DATA[i].FARM_ID,
                    flipDataSet.FLOCK_DATA[i].FLOCK_ID,
                    trackNO,
                    flipDataSet.FLOCK_DATA[i].TRX_DATE,
                    flipDataSet.FLOCK_DATA[i].NUM_1,
                    "D",
                    flipDataSet.FLOCK_DATA[i].FLOCK_KEY,
                    null,
                    "",
                    "",
                    flipDataSet.FLOCK_DATA[i].NUM_1,
                    null,
                    null,
                    null,
                    hatchLoc,
                    null);
            }
        }

        private void AtualizaMinuto_Tick(object sender, EventArgs e)
        {
            string erro = "";

            try
            {
                //this.EventLog.WriteEntry("Reimportação do EggInventory CH pendentes iniciada.");
                //ReintegraEggInvComProblema("PP");
                //this.EventLog.WriteEntry("Reimportação do EggInventory CH pendentes finalizada.");
                //this.EventLog.WriteEntry("Reimportação do EggInventory PH pendentes iniciada.");
                //ReintegraEggInvComProblema("GP");
                //this.EventLog.WriteEntry("Reimportação do EggInventory PH pendentes finalizada.");
                if (VerificaExecucaoPlanalto == 0)
                {
                    //this.EventLog.WriteEntry("Importação do Diário de Produção para o Apolo iniciada.");
                    VerificaExecucaoPlanalto = 1;
                    erro = "Erro ao realizar Ajuste do Diário de Produção para o Apolo da Planalto: ";
                    VerificaExecucaoPlanalto = 0;
                    //this.EventLog.WriteEntry("Importação do Diário de Produção para o Apolo finalizada.");
                }
                if (VerificaExecucao == 0)
                {
                    //this.EventLog.WriteEntry("Importação do Diário de Produção para o Apolo iniciada.");
                    VerificaExecucao = 1;
                    erro = "Erro ao realizar Importação do Diário de Produção para o Apolo: ";

                    //if (DateTime.Now.Hour == 2 && DateTime.Now.Minute == 0)
                    //{
                    //    AjustaDEOxApolo();
                    //}

                    //AjustaDiarioProducaoPlanalto();
                    InsereProducaoEstoqueApolo();
                    InsertProductionCLFLOCKS("HYCL", "CL"); // Chile
                    InsertProductionHCFLOCKS("HYCO", "CO"); // Colombia
                    InsertProductionHCFLOCKS("HYCO", "EC"); // Ecuador
                    ImportaDadosNascimentoFLIPparaWEB();
                    VerificaExecucao = 0;
                    //this.EventLog.WriteEntry("Importação do Diário de Produção para o Apolo finalizada.");
                }
                //this.EventLog.WriteEntry("Ajuste do Egg Inventory de acordo com o Apolo iniciado.");
                //AjustaEggInvFLIP();
                //this.EventLog.WriteEntry("Ajuste do Egg Inventory de acordo com o Apolo finalizado.");

                if (((DateTime.Now.Hour == 3) && (DateTime.Now.Minute == 00)))
                {
                    //this.EventLog.WriteEntry("Importação das Incubações pendentes iniciada.");
                    erro = "Erro ao realizar Atualização do Estoque Futuro: ";
                    ImportaIncubacaoEstoqueFuturo();
                    //erro = "Erro ao realizar Importação da Incubação do WEB para o FLIP: ";
                    //ImportaIncubacaoFLIP();
                    //this.EventLog.WriteEntry("Importação das Incubações pendentes finalizada.");
                    //this.EventLog.WriteEntry("Atualização da Tabela de Saldo do Apolo iniciada.");
                    //erro = "Erro ao realizar Importação da Incubação do WEB para o FLIP: ";
                    //AtualizaTabelaSaldo();
                    //this.EventLog.WriteEntry("Atualização da Tabela de Saldo do Apolo finalizada.");
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        this.EventLog.WriteEntry("Erro linha: " 
                            + linenum.ToString() + " / "
                            + erro + ex.Message + " / " 
                            + ex.InnerException.Message, EventLogEntryType.Error, 10);
                    else
                        this.EventLog.WriteEntry("Erro linha: "
                            + linenum.ToString() + " / "
                            + erro 
                            + ex.Message, EventLogEntryType.Error, 10);
                else
                    this.EventLog.WriteEntry("Erro linha: "
                            + linenum.ToString() + " / "
                            + erro + ex.Message, EventLogEntryType.Error, 10);

                VerificaExecucao = 0;
            }
        }

        public void ReinciarServico()
        {
            int timeoutMilliseconds = 2000;

            ServiceController service = new ServiceController("ImportaIncubacao");

            int millisec1 = Environment.TickCount;
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            while (!this.CanStop)
            {
                if (this.CanStop)
                {
                    this.Stop();

                    // count the rest of the timeout
                    int millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
        }

        public MOV_ESTQ InsereMovEstq(string empresa, string tipoLanc, string entCod, DateTime dataMovimentacao, 
            string usuario)
        {
            MOV_ESTQ movEstq = new MOV_ESTQ();

            ObjectParameter chave = new ObjectParameter("codigo", typeof(global::System.String));

            ENTIDADE entidade = bdApolo.ENTIDADE.Where(w => w.EntCod == entCod).FirstOrDefault();
            CIDADE cidade = bdApolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();

            bdApolo.gerar_codigo(empresa, "MOV_ESTQ", chave);

            movEstq.EmpCod = empresa;
            movEstq.MovEstqChv = (int)chave.Value;
            movEstq.TipoLancCod = tipoLanc;
            movEstq.MovEstqDataMovimento = dataMovimentacao;
            movEstq.MovEstqDataEmissao = dataMovimentacao;
            movEstq.MovEstqDocEmpCod = "3";
            movEstq.MovEstqDocEspec = "FLIP";
            movEstq.MovEstqDocSerie = "0";
            movEstq.MovEstqDocNum = dataMovimentacao.ToShortDateString();
            movEstq.EntCod = entCod;
            movEstq.MovEstqRatValDespDiv = "Sim";
            movEstq.UsuCod = usuario;
            movEstq.MovEstqDataHoraDig = DateTime.Now;
            movEstq.MovEstqIntegFisc = "Não";
            movEstq.MovEstqValIssDedTot = "Não";
            movEstq.MovEstqValInssDedTot = "Não";
            movEstq.MovEstq = "Sim";
            movEstq.MovEstqValIrrfDedTot = "Não";
            movEstq.MovEstqOrig = "Estoque";
            movEstq.MovEstqRejPat = "Não";
            movEstq.MovEstqDataEntrada = dataMovimentacao;
            movEstq.TipoPagRecCod = "0000002";
            movEstq.MovEstqValCsllDedTot = "Não";
            movEstq.MovEstqValCofinsDedTot = "Não";
            movEstq.MovEstqValPisDedTot = "Não";
            movEstq.MovEstqIcmsFreteSomaIcmsST = "Não";
            movEstq.MovEstqRateioFretePorPeso = "Não";
            movEstq.MovEstqRateioCapPorPeso = "Sim";
            movEstq.MovEstqValPagRecAntIcmsST = "Não";
            movEstq.MovEstqSelec = "Não";
            movEstq.MovEstqGeraFiscal = "Não";
            movEstq.MovEstqValOutrDespCompValDoc = "Não";
            movEstq.MovEstqDesabRecalcVal = "Não";
            movEstq.MovEstqIndTipoFrete = "Sem Frete";
            movEstq.MovEstqValCofinsProdDedTot = "Não";
            movEstq.MovEstqValPisProdDedTot = "Não";
            movEstq.MovEstqValFunruralDedTot = "Não";
            movEstq.MovEstqEntNome = entidade.EntNome;
            movEstq.MovEstqEntCpfCgc = entidade.EntCpfCgc;
            movEstq.MovEstqRGIE = entidade.EntRgIe;
            movEstq.MovEstqEntEnder = entidade.EntEnder;
            movEstq.MovEstqEntEnderNo = entidade.EntEnderNo;
            movEstq.MovEstqEntBair = entidade.EntBair;
            movEstq.MovEstqCodPais = cidade.PaisSigla;
            movEstq.MovEstqCidNome = cidade.CidNome;
            movEstq.MovEstqUfSigla = cidade.UfSigla;
            movEstq.MovEstqCodCid = entidade.CidCod;
            movEstq.MovEstqDeduzPISParc = "Não";
            movEstq.MovEstqDeduzCofinsParc = "Não";
            movEstq.MovEstqDeduzCsllParc = "Não";
            movEstq.MovEstqDataEntrega = dataMovimentacao;
            movEstq.MOVESTQCONFINTEGSIS = "Regular";
            movEstq.MovEstqRatTxaMarMercPorPeso = "Não";
            movEstq.MovEstqRatSiscomexPorPeso = "Não";
            movEstq.MovEstqRatDespImportPorPeso = "Não";

            return movEstq;
        }

        public PRODUTO RetornaProdutoPelaLinha(string linha)
        {
            PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == linha).FirstOrDefault();

            if (produto == null)
            {
                return produto = bdApolo.PRODUTO.Where(p => p.ProdCodEstr == linha).First();
            }
            else
            {
                return produto;
            }
        }

        public short RetornaUltimaSequenciaItemMovEstq(string empresa, int chave)
        {
            int existe = bdApolo.ITEM_MOV_ESTQ
                .Where(i => i.EmpCod == empresa && i.MovEstqChv == chave)
                .Count();

            if (existe != 0)
                return bdApolo.ITEM_MOV_ESTQ
                    .Where(i => i.EmpCod == empresa && i.MovEstqChv == chave)
                    .OrderByDescending(s => s.ItMovEstqSeq)
                    .First().ItMovEstqSeq;
            else
                return 0;
        }

        public ITEM_MOV_ESTQ InsereItemMovEstq(int chave, string empresa, string tipoLanc, string entCod, 
            DateTime dataMovimentacao, string linha, string naturezaOperacao, decimal quantidade, 
            decimal valorUnitario, string unidadeMedida, short? posicaoUnidadeMedida, string tribCod,
            string itMovEstqClasFiscCodNbm, string clasFiscCod)
        {
            string mensagem = "";

            ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();

            itemMovEstq.EmpCod = empresa;

            PRODUTO produto = RetornaProdutoPelaLinha(linha);

            itemMovEstq.ProdCodEstr = produto.ProdCodEstr;
            itemMovEstq.MovEstqChv = chave;

            short sequencia = RetornaUltimaSequenciaItemMovEstq(empresa, chave);

            itemMovEstq.ItMovEstqSeq = ++sequencia;
            itemMovEstq.ItMovEstqDataMovimento = dataMovimentacao;
            itemMovEstq.TipoLancCod = tipoLanc;
            itemMovEstq.NatOpCodEstr = naturezaOperacao;
            itemMovEstq.ItMovEstqQtdCalcProd = quantidade;
            itemMovEstq.ItMovEstqValProd = valorUnitario * quantidade;

            PROD_UNID_MED prodUnidMed = produto.PROD_UNID_MED
                .Where(u => u.ProdUnidMedCod.Contains(unidadeMedida)
                    && u.ProdUnidMedPos == posicaoUnidadeMedida
                    && u.ProdCodEstr == produto.ProdCodEstr)
                .FirstOrDefault();

            if (prodUnidMed != null)
            {
                itemMovEstq.ItMovEstqUnidMedCod = unidadeMedida;
                itemMovEstq.ItMovEstqUnidMedPos = posicaoUnidadeMedida;
                itemMovEstq.ItMovEstqUnidMedPeso = prodUnidMed.ProdUnidMedPeso;
                itemMovEstq.ItMovEstqUnidMedPesoFD = prodUnidMed.ProdUnidMedPesoFD;
                itemMovEstq.ItMovEstqUnidMedCodVal = unidadeMedida;
                itemMovEstq.ItMovEstqUnidMedPosVal = posicaoUnidadeMedida;
            }
            else
            {
                itemMovEstq.ItMovEstqObs = "Unidade de Medida não cadastrada no Produto! Verifique!";
                return itemMovEstq;
            }

            if (posicaoUnidadeMedida != 1)
            {
                itemMovEstq.ItMovEstqQtdProd = quantidade * Convert.ToDecimal(prodUnidMed.ProdUnidMedPeso);
            }
            else
            {
                itemMovEstq.ItMovEstqQtdProd = quantidade;
            }

            itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;
            itemMovEstq.ItMovEstqCustoUnit = valorUnitario;
            itemMovEstq.ItMovEstqServ = "Não";
            itemMovEstq.TribCod = tribCod;
            itemMovEstq.ItMovEstqClasFiscCodNbm = itMovEstqClasFiscCodNbm;
            itemMovEstq.ItMovEstq = "Sim";
            itemMovEstq.ItMovEstqSeqItOrig = itemMovEstq.ItMovEstqSeq;
            itemMovEstq.ItMovEstqSeqDesm = 1;
            itemMovEstq.TribACod = tribCod.Substring(0, 1);
            itemMovEstq.TribBCod = tribCod.Substring(1, 2);
            itemMovEstq.TribCod = tribCod;
            itemMovEstq.ClasFiscCod = clasFiscCod;
            itemMovEstq.ItMovEstqUnidMedPosVal = itemMovEstq.ItMovEstqUnidMedPos;
            itemMovEstq.EntCod = entCod;
            itemMovEstq.ItMovEstqProdNome = produto.ProdNome;
            itemMovEstq.ItMovEstqChvOrd = chave;
            itemMovEstq.ItMovEstqMotDesonICMS = "Nenhum";
            itemMovEstq.USERCalculadoSaldoServico = "Não";

            mensagem = "OK" + itemMovEstq.ProdCodEstr;

            return itemMovEstq;
        }

        public CTRL_LOTE_ITEM_MOV_ESTQ InsereLote(int chave, string empresa, string tipoLanc, short sequencia, 
            string prodCodEstr, string numLote, DateTime dataProducao, decimal quantidade, string operacao, 
            string unidadeMedida, short? posicaoUnidadeMedida, string localArmazenagem)
        {
            /*CTRL_LOTE existeLote = bdApolo.CTRL_LOTE
                .Where(c => c.EmpCod == empresa && c.ProdCodEstr == prodCodEstr && c.CtrlLoteNum == numLote &&
                    c.CtrlLoteDataValid == dataProducao).FirstOrDefault();

            if (existeLote == null)
            {
                CTRL_LOTE lote = new CTRL_LOTE();

                lote.EmpCod = empresa;
                lote.ProdCodEstr = prodCodEstr;
                lote.CtrlLoteNum = numLote;
                lote.CtrlLoteDataValid = dataProducao;
                lote.CtrlLoteDataFab = dataProducao;
                lote.CtrlLoteQtdSaldo = 0;
                lote.CtrlLoteUnidMedCod = unidadeMedida;
                lote.CtrlLoteUnidMedPos = posicaoUnidadeMedida;
                lote.CtrlLoteQtdSaldoCalc = lote.CtrlLoteQtdSaldo;

                bdApolo.CTRL_LOTE.AddObject(lote);
            }
            else
            {
                if (operacao.Equals("Entrada"))
                {
                    existeLote.CtrlLoteQtdSaldo = existeLote.CtrlLoteQtdSaldo + quantidade;
                    existeLote.CtrlLoteQtdSaldoCalc = existeLote.CtrlLoteQtdSaldo;
                }
                else
                {
                    existeLote.CtrlLoteQtdSaldo = existeLote.CtrlLoteQtdSaldo - quantidade;
                    existeLote.CtrlLoteQtdSaldoCalc = existeLote.CtrlLoteQtdSaldo;
                }
            }*/

            CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = new CTRL_LOTE_ITEM_MOV_ESTQ();

            loteItemMovEstq.EmpCod = empresa;
            loteItemMovEstq.ProdCodEstr = prodCodEstr;
            loteItemMovEstq.CtrlLoteNum = numLote;
            loteItemMovEstq.CtrlLoteDataValid = dataProducao;
            loteItemMovEstq.MovEstqChv = chave;
            loteItemMovEstq.ItMovEstqSeq = sequencia;
            loteItemMovEstq.CtrlLoteItMovEstqQtd = quantidade;
            loteItemMovEstq.CtrlLoteItMovEstqOper = operacao;
            loteItemMovEstq.CtrlLoteItMovEstqDataFab = dataProducao;
            loteItemMovEstq.CtrlLoteItMovEstqUnidMedCod = unidadeMedida;
            loteItemMovEstq.CtrlLoteItMovEstqUnidMedPos = posicaoUnidadeMedida;
            loteItemMovEstq.LocArmazCodEstr = localArmazenagem;
            loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

            return loteItemMovEstq;
        }

        public LOC_ARMAZ_ITEM_MOV_ESTQ InsereLocalArmazenagem(int chave, string empresa, short sequencia, 
            string prodCodEstr, decimal quantidade, decimal qtdeCalculada, string localArmazenagem)
        {
            LOC_ARMAZ_ITEM_MOV_ESTQ local = new LOC_ARMAZ_ITEM_MOV_ESTQ();

            local.EmpCod = empresa;
            local.MovEstqChv = chave;
            local.ProdCodEstr = prodCodEstr;
            local.ItMovEstqSeq = sequencia;
            local.LocArmazCodEstr = localArmazenagem;
            local.LocArmazItMovEstqQtd = qtdeCalculada;
            local.LocArmazItMovEstqQtdCalc = quantidade;

            return local;
        }

        public void InsereProducaoEstoqueApoloPorGranja(string farm, string location, string codApolo, 
            string terceiro, string tipoLancEntrada)
        {
            bdApolo.CommandTimeout = 10000;
            bdSQLServer.CommandTimeout = 10000;

            try
            {
                flocksData.Fill(flipDataSet.FLOCKS_DATA, farm, location, 0);

                FARMSTableAdapter fTA = new FARMSTableAdapter();
                
                if (flipDataSet.FLOCKS_DATA.Count > 0)
                {
                    EMPRESA_FILIAL empresaFilial = bdApolo.EMPRESA_FILIAL
                            .Where(e => e.EmpCod == codApolo)
                            .FirstOrDefault();

                    if (empresaFilial != null)
                    {
                        #region Atualiza FLOCK_DATA WEB

                        #region Carrega Variáveis e Objetos - **** NÃO INTEGRA MAIS COM O APOLO ****

                        /*
                        string empresa = empresaFilial.EmpCod;
                        //string tipoLanc = "E0000011";
                        string tipoLanc = tipoLancEntrada;
                        string entCod = "";

                        if (terceiro.Equals("SIM"))
                        {
                            ENTIDADE1 entidade1 = bdApolo.ENTIDADE1
                                .Where(e => e.USERFLIPCodigo == farm)
                                .FirstOrDefault();

                            entCod = entidade1.EntCod;
                        }
                        else
                        {
                            entCod = empresaFilial.EntCod;
                        }

                        string naturezaOperacao = "1.556.001";
                        decimal valorUnitario = 0;
                        if (farm.Equals("PL"))
                            valorUnitario = 0.90m;
                        else
                            valorUnitario = 0.25m;
                        string unidadeMedida = "UN";
                        short? posicaoUnidadeMedida = 1;
                        string tribCod = "040";
                        string itMovEstqClasFiscCodNbm = "04079000";
                        string clasFiscCod = "0000129";
                        string operacao = "Entrada";
                        string usuario = "RIOSOFT";

                        LOC_ARMAZ localArmazCadastro = bdApolo.LOC_ARMAZ
                            .Where(l => l.USERCodigoFLIP == farm && l.USERTipoProduto == "Ovos Incubáveis")
                            .FirstOrDefault();

                        string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                        DateTime dataAnterior = Convert.ToDateTime("01/01/2014");
                        DateTime dataAtual;

                        string linhaAnterior = "";
                        string linhaAtual = "";

                        //decimal qtdTotalItem = 0;

                        MOV_ESTQ movEstq = new MOV_ESTQ();
                        ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
                        LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = new LOC_ARMAZ_ITEM_MOV_ESTQ();
                        CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = new CTRL_LOTE_ITEM_MOV_ESTQ();
                         * */

                        #endregion

                        #region Carrega Variáveis e Objetos

                        DateTime dataAnterior = Convert.ToDateTime("01/01/2014");
                        DateTime dataAtual;

                        string linhaAnterior = "";
                        string linhaAtual = "";

                        #endregion

                        for (int i = 0; i < flipDataSet.FLOCKS_DATA.Count; i++)
                        {
                            #region Atualiza tabela EGGINV_DATA

                            decimal? existeEggInvData = 0;
                            existeEggInvData = eggInvData.ExisteEggInvData(flipDataSet.FLOCKS_DATA[i].FLOCK_ID, flipDataSet.FLOCKS_DATA[i].TRX_DATE);

                            if (existeEggInvData == 0)
                            {
                                string track_no = "EXP" + flipDataSet.FLOCKS_DATA[i].TRX_DATE.ToString("YYMMDD");

                                string hatchLOC = "";

                                if (flipDataSet.FLOCKS_DATA[i].LOCATION.Equals("GP"))
                                    hatchLOC = "PH";
                                else
                                    hatchLOC = "CH";

                                eggInvData.Insert(flipDataSet.FLOCKS_DATA[i].COMPANY, flipDataSet.FLOCKS_DATA[i].REGION,
                                    flipDataSet.FLOCKS_DATA[i].LOCATION, flipDataSet.FLOCKS_DATA[i].FARM_ID, flipDataSet.FLOCKS_DATA[i].FLOCK_ID,
                                    track_no, flipDataSet.FLOCKS_DATA[i].TRX_DATE, flipDataSet.FLOCKS_DATA[i].NUM_1, "O", null, null, null,
                                    null, null, null, null, null, hatchLOC, null);
                            }

                            #endregion

                            dataAtual = flipDataSet.FLOCKS_DATA[i].TRX_DATE;
                            linhaAtual = flipDataSet.FLOCKS_DATA[i].VARIETY;

                            if (!ExisteFechamentoEstoque(dataAtual, empresaFilial.USERFLIPCod))
                            {
                                string flockID = flipDataSet.FLOCKS_DATA[i].FLOCK_ID;
                                DateTime trxDate = flipDataSet.FLOCKS_DATA[i].TRX_DATE;
                                string flockKey = flipDataSet.FLOCKS_DATA[i].FLOCK_KEY;

                                decimal quantidade = flipDataSet.FLOCKS_DATA[i].NUM_1;
                                string numLote = flipDataSet.FLOCKS_DATA[i].FLOCK_ID;

                                int age = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsAGENull()) 
                                    age = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].AGE);
                                int henMort = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsHEN_MORTNull())
                                    henMort = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].HEN_MORT);
                                int henWt = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsHEN_WTNull())
                                    henWt = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].HEN_WT);
                                int maleMort = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsMALE_MORTNull())
                                    maleMort = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].MALE_MORT);
                                decimal henFeedDel = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsHEN_FEED_DELNull())
                                    henFeedDel = Convert.ToDecimal(flipDataSet.FLOCKS_DATA[i].HEN_FEED_DEL);
                                int totalEggsProd = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsTOTAL_EGGS_PRODNull())
                                    totalEggsProd = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].TOTAL_EGGS_PROD);
                                decimal eggWt = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsEGG_WTNull())
                                    eggWt = Convert.ToDecimal(flipDataSet.FLOCKS_DATA[i].EGG_WT);
                                int hatchEggs = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_1Null())
                                    hatchEggs = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].NUM_1);
                                string comentarios = "";
                                if (!flipDataSet.FLOCKS_DATA[i].IsTEXT_1Null())
                                    comentarios = flipDataSet.FLOCKS_DATA[i].TEXT_1;
                                int numGalpao = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_2Null())
                                    numGalpao = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].NUM_2);

                                #region 20/04/2017 - Variáveis novas para WebService LTZ

                                #region Farm Name

                                string farmName = "";
                                FLIPDataSet.FARMSDataTable fDT = new FLIPDataSet.FARMSDataTable();
                                fTA.FillByFarmID(fDT, flipDataSet.FLOCKS_DATA[i].FARM_ID);
                                FLIPDataSet.FARMSRow fR = fDT.FirstOrDefault();
                                if (!fR.IsFARM_NAMENull())
                                    farmName = fR.FARM_NAME;

                                #endregion

                                int count_females = ACMFEMINV(trxDate, flockID);
                                int count_males = ACMMALEINV(trxDate, flockID);
                                int broken = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_12Null())
                                    broken = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].NUM_12);
                                int dirty = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_10Null())
                                    dirty = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].NUM_10);
                                int consume = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_9Null())
                                    consume = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].NUM_9);
                                int floor = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_11Null())
                                    floor = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].NUM_11);
                                int destroyed = 0;
                                decimal water_consumption = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_2Null())
                                    water_consumption = Convert.ToDecimal(flipDataSet.FLOCKS_DATA[i].NUM_2);
                                decimal uniformity = 0;
                                if (!flipDataSet.FLOCKS_DATA[i].IsNUM_15Null())
                                    uniformity = Convert.ToDecimal(flipDataSet.FLOCKS_DATA[i].NUM_15);

                                #endregion

                                #region *** NÃO VAI INTEGRAR MAIS PELO APOLO ****

                                /*

                                #region Carrega Lote

                                loteItemMovEstq = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                                    .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataAtual && l.EmpCod == empresaFilial.EmpCod
                                        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                            && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada))
                                    .FirstOrDefault();

                                #endregion

                                if (loteItemMovEstq == null)
                                {
                                    #region Carrega Produto

                                    string replace = "" + (char)13 + (char)10;

                                    PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == linhaAtual).FirstOrDefault();

                                    itemMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                        && im.ProdCodEstr == produto.ProdCodEstr
                                        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                            && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada))
                                    .FirstOrDefault();
                                    #endregion

                                    if (itemMovEstq == null)
                                    {
                                        #region Carrega Movimentação. Se não existe, insere.
                                        movEstq = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                            && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada)
                                        .FirstOrDefault();

                                        if (movEstq == null)
                                        {
                                            movEstq = InsereMovEstq(empresa, tipoLanc, entCod, dataAtual, usuario);

                                            bdApolo.MOV_ESTQ.AddObject(movEstq);

                                            bdApolo.SaveChanges();
                                        }
                                        #endregion

                                        #region Se Item não existe, insere item, local e lote
                                        itemMovEstq = InsereItemMovEstq(movEstq.MovEstqChv, empresa, tipoLanc, entCod, dataAtual, linhaAtual, naturezaOperacao,
                                            quantidade, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod, itMovEstqClasFiscCodNbm,
                                            clasFiscCod);

                                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                        localArmaz = InsereLocalArmazenagem(movEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                            quantidade, localArmazenagem);

                                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                        loteItemMovEstq = InsereLote(movEstq.MovEstqChv, empresa, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                            dataAtual, quantidade, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        bdApolo.SaveChanges();

                                        //bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //    dataAtual, "INS");

                                        bdApolo.calcula_mov_estq(empresa, movEstq.MovEstqChv);
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Se existe Item, insere lote e atuliza item e local

                                        loteItemMovEstq = InsereLote(itemMovEstq.MovEstqChv, empresa, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                            dataAtual, quantidade, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + quantidade;
                                        itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                        localArmaz = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                            && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                            && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                            .FirstOrDefault();

                                        localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + quantidade;
                                        localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                        bdApolo.SaveChanges();

                                        //bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //    dataAtual, "UPD");

                                        bdApolo.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region Se existe o lote e a quantidade é diferente, atualiza a quantidade

                                    if (loteItemMovEstq.CtrlLoteItMovEstqQtd != quantidade)
                                    {
                                        //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));
                                        //ObjectParameter rchave = new ObjectParameter("rchave", typeof(global::System.Int32));
                                        //ObjectParameter rdatamovimento = new ObjectParameter("rdatamovimento", typeof(global::System.DateTime));

                                        //bdApolo.analisa_alteracao_entrada_estq(loteItemMovEstq.EmpCod, loteItemMovEstq.MovEstqChv,
                                        //    loteItemMovEstq.ItMovEstqSeq, loteItemMovEstq.ProdCodEstr, dataAtual, quantidade,
                                        //    rmensagem, rchave, rdatamovimento);

                                        //if (rmensagem.Value.ToString().Equals("Não"))
                                        //{
                                            decimal qtdAntiga = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                            loteItemMovEstq.CtrlLoteItMovEstqQtd = quantidade;
                                            loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                            PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == linhaAtual).FirstOrDefault();

                                            itemMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                                && im.ProdCodEstr == produto.ProdCodEstr
                                                && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                                    && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada))
                                            .FirstOrDefault();

                                            itemMovEstq.ItMovEstqQtdProd = (itemMovEstq.ItMovEstqQtdProd - qtdAntiga) + quantidade;
                                            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                            localArmaz = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                                && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                                && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                                .FirstOrDefault();

                                            localArmaz.LocArmazItMovEstqQtd = (localArmaz.LocArmazItMovEstqQtd - qtdAntiga) + quantidade;
                                            localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                            bdApolo.SaveChanges();

                                            //bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                            //    dataAtual, "UPD");

                                            bdApolo.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);
                                        //}
                                        //else
                                        //{
                                        //    string corpo = "O Lote " + numLote + " da data de Produção " + dataAtual + " não foi alterado "
                                        //        + "porque não ficará com saldo possível!";

                                        //    EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR DIARIO DA GRANJA ****", "Paulo Alves",
                                        //        "palves@hyline.com.br", "", "");
                                        //}
                                    }

                                    #endregion
                                }

                                #region Código Antigo onde somente adiciona e não atualizava

                                //#region Insere Cabeçalho da Movimentação de Estoque
                                //if ((dataAnterior != dataAtual) || (dataAnterior == Convert.ToDateTime("01/01/2014")))
                                //{
                                //    bdApolo.SaveChanges();

                                //    bdApolo.calcula_mov_estq(empresa, movEstq.MovEstqChv);

                                //    movEstq = InsereMovEstq(empresa, tipoLanc, entCod, dataAtual, usuario);

                                //    bdApolo.MOV_ESTQ.AddObject(movEstq);
                                //}
                                //#endregion

                                //#region Insere Item da Movimentação de Estoque
                                //if ((linhaAnterior != linhaAtual) || (dataAnterior != dataAtual))
                                //{
                                //    itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                //    itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                //    itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                //    localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                //    localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                //    qtdTotalItem = 0;

                                //    bdApolo.SaveChanges();

                                //    bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataAtual, "INS");

                                //    itemMovEstq = InsereItemMovEstq(movEstq.MovEstqChv, empresa, tipoLanc, entCod, dataAtual, linhaAtual, naturezaOperacao,
                                //        quantidade, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod, itMovEstqClasFiscCodNbm,
                                //        clasFiscCod);

                                //    bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                //    localArmaz = InsereLocalArmazenagem(movEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                //        quantidade, localArmazenagem);

                                //    bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);
                                //}
                                //#endregion

                                //#region Insere os Lotes

                                //loteItemMovEstq = InsereLote(movEstq.MovEstqChv, empresa, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                //    dataAtual, quantidade, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                //bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                //qtdTotalItem = qtdTotalItem + quantidade;

                                //#endregion

                                //if ((flipDataSet.FLOCKS_DATA.Count == (i + 1)))
                                //{
                                //    itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                //    itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                //    itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                //    localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                //    localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                //    qtdTotalItem = 0;

                                //    bdApolo.SaveChanges();

                                //    bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataAtual, "INS");

                                //    bdApolo.calcula_mov_estq(empresa, movEstq.MovEstqChv);
                                //}

                                //if (terceiro.Equals("SIM"))
                                //{
                                //    int idade = Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].AGE);
                                //    LayoutDiarioExpedicaos deo = InsereDEOTerceiro(numLote, idade, dataAtual, quantidade, movEstq.MovEstqChv,
                                //        empresaFilial.USERFLIPCod);

                                //    bdSQLServer.LayoutDiarioExpedicaos.AddObject(deo);
                                //}
                                 * 
                                 * 

                                #endregion
                                 * 
                                 * */

                                #endregion

                                HLBAPPEntities hlbapp = new HLBAPPEntities();
                                hlbapp.CommandTimeout = 10000;

                                //FLOCK_DATA flockDataWEB = hlbapp.FLOCK_DATA.Where(w => w.Flock_ID == flockID
                                //    && w.Trx_Date == trxDate).FirstOrDefault();

                                var listaDEO = hlbapp.LayoutDiarioExpedicaos
                                    .Where(w => w.LoteCompleto == flockID && w.DataProducao == trxDate
                                        //&& w.TipoDEO == "Ovos Incubáveis")
                                        && w.Granja == farm
                                        && w.TipoDEO != "Inventário de Ovos")
                                    .ToList();

                                var listaDEOOvosComercio = hlbapp.LayoutDiarioExpedicaos
                                    .Where(w => w.LoteCompleto == "VARIOS"
                                        && w.Granja == farm
                                        && w.TipoDEO == "Ovos p/ Comércio")
                                    .ToList();

                                var saldoIncubaveis = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                                    .Where(w => w.LoteCompleto == flockID && w.DataProducao == trxDate
                                        && w.Local == farm)
                                    .ToList();

                                if (listaDEO.Count > 0)
                                {
                                    int qtdDEO = (int)listaDEO.Sum(s => s.QtdeOvos + (s.QtdDiferenca == null ? 0 : s.QtdDiferenca));
                                    int qtdDEOOvosComercio = (int)listaDEOOvosComercio.Sum(s => s.QtdeOvos);
                                    int ovosComercioTotalGranja = Convert.ToInt32(flocksData.QtdeOvosComercio(farm, location));

                                    if ((qtdDEO > hatchEggs)
                                        && (!flipDataSet.FARMS_IMPORT
                                            .Where(w => w.FARM == farm).FirstOrDefault().IsEMAILRESPNull()))
                                    {
                                        string emailResp = flipDataSet.FARMS_IMPORT
                                            .Where(w => w.FARM == farm).FirstOrDefault().EMAILRESP;

                                        string corpo = "Prezados," + (char)13 + (char)10 +
                                            "O lote " + flockID + " produzido dia " + trxDate.ToShortDateString() +
                                            " está com quantidade maior nos DEOs do que no Diário de Produção " +
                                            "do FLIP (Qtde. DEOS: " + qtdDEO.ToString() +
                                            " - Qtde. FLIP: " + hatchEggs.ToString() + ")." + (char)13 +
                                            "Por esse motivo não foi gerado estoque e não será possível gerar DEOs" +
                                            " e Incubações!!!" + (char)13 +
                                            "Verificar e corrigir!!!" + (char)13 + +(char)10 +
                                            "SISTEMA";

                                        EnviarEmail(corpo, "**** ERRO AO GERAR ESTOQUE DIÁRIO DE PRODUÇÃO ****",
                                            "Diário Produção", emailResp, "", "");
                                    }
                                    //else if ((consume > qtdDEOOvosComercio && saldoIncubaveis.Count > 0)
                                    else if ((qtdDEOOvosComercio > ovosComercioTotalGranja)
                                        && (!flipDataSet.FARMS_IMPORT
                                            .Where(w => w.FARM == farm).FirstOrDefault().IsEMAILRESPNull()))
                                    {
                                        string emailResp = flipDataSet.FARMS_IMPORT
                                            .Where(w => w.FARM == farm).FirstOrDefault().EMAILRESP;

                                        string corpo = "Prezados," + (char)13 + (char)10 +
                                            "O lote " + flockID + " produzido dia " + trxDate.ToShortDateString() +
                                            " está com quantidade de OVOS DE COMÉRCIO maior nos DEOs do que no Diário de Produção " +
                                            "do FLIP (Qtde. DEOS: " + qtdDEOOvosComercio.ToString() +
                                            " - Qtde. FLIP: " + consume.ToString() + ")." + (char)13 +
                                            "Por esse motivo não foi gerado estoque e não será possível gerar DEOs" +
                                            " e Incubações!!!" + (char)13 +
                                            "Verificar e corrigir!!!" + (char)13 + +(char)10 +
                                            "SISTEMA";

                                        string copiaPara = "";
                                        if (listaDEOOvosComercio.Count > 0)
                                        {
                                            if (listaDEOOvosComercio.FirstOrDefault().Incubatorio == "CH"
                                                || listaDEOOvosComercio.FirstOrDefault().Incubatorio == "TB")
                                                copiaPara = "sdoimo@hyline.com.br";
                                            else
                                                copiaPara = "aneves@hyline.com.br";
                                        }

                                        EnviarEmail(corpo, "**** ERRO AO GERAR ESTOQUE DIÁRIO DE PRODUÇÃO ****",
                                            "Diário Produção", emailResp, copiaPara, "");
                                    }
                                    else
                                    {

                                        ImportaDiarioProducaoWEB(
                                            "HYBR",
                                            "BR",
                                            flipDataSet.FLOCKS_DATA[i].FARM_ID,
                                            flockID,
                                            numLote,
                                            flipDataSet.FLOCKS_DATA[i].VARIETY,
                                            Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].ACTIVE),
                                            age,
                                            trxDate,
                                            henMort,
                                            henWt,
                                            maleMort,
                                            henFeedDel,
                                            totalEggsProd,
                                            eggWt,
                                            hatchEggs,
                                            comentarios,
                                            count_females,
                                            count_males,
                                            broken,
                                            dirty,
                                            consume,
                                            floor,
                                            destroyed,
                                            water_consumption,
                                            uniformity,
                                            farmName,
                                            numGalpao);
                                    }
                                }
                                else
                                {
                                    ImportaDiarioProducaoWEB(
                                            "HYBR",
                                            "BR",
                                            flipDataSet.FLOCKS_DATA[i].FARM_ID,
                                            flockID,
                                            numLote,
                                            flipDataSet.FLOCKS_DATA[i].VARIETY,
                                            Convert.ToInt32(flipDataSet.FLOCKS_DATA[i].ACTIVE),
                                            age,
                                            trxDate,
                                            henMort,
                                            henWt,
                                            maleMort,
                                            henFeedDel,
                                            totalEggsProd,
                                            eggWt,
                                            hatchEggs,
                                            comentarios,
                                            count_females,
                                            count_males,
                                            broken,
                                            dirty,
                                            consume,
                                            floor,
                                            destroyed,
                                            water_consumption,
                                            uniformity,
                                            farmName,
                                            numGalpao);
                                }

                                linhaAnterior = flipDataSet.FLOCKS_DATA[i].VARIETY;
                                dataAnterior = flipDataSet.FLOCKS_DATA[i].TRX_DATE;
                            }
                        }

                        //bdApolo.SaveChanges();
                        //bdSQLServer.SaveChanges();

                        #endregion

                        #region Atualiza Núcleo, Idade e Média das Últimas 4 Semanas

                        for (int i = 0; i < flipDataSet.FLOCKS_DATA.Count; i++)
                        {
                            string flockKey = flipDataSet.FLOCKS_DATA[i].FLOCK_KEY;

                            dataAtual = flipDataSet.FLOCKS_DATA[i].TRX_DATE;

                            //if (!terceiro.Equals("SIM"))
                            //{
                            DateTime dataPrd = flipDataSet.FLOCKS_DATA[i].TRX_DATE;
                            string flockID = flipDataSet.FLOCKS_DATA[i].FLOCK_ID;

                            string flockIDHatch = flipDataSet.FLOCKS_DATA[i].FARM_ID + "-" + flockID;

                            #region **** NÃO INTEGRA MAIS COM O APOLO ****

                            /*
                            CTRL_LOTE lote = bdApolo.CTRL_LOTE
                                .Where(c => c.EmpCod == empresa && c.CtrlLoteNum == flockID
                                    && c.CtrlLoteDataValid == dataPrd)
                                //&& (c.USERGranjaNucleoFLIP == null || c.USERGranjaNucleoFLIP == ""))
                                .FirstOrDefault();

                            if (lote != null)
                            {
                                lote.USERGranjaNucleoFLIP = flipDataSet.FLOCKS_DATA[i].FARM_ID;
                                lote.USERIdateLoteFLIP = (short)flipDataSet.FLOCKS_DATA[i].AGE;
                                lote.USERPercMediaIncUlt4SemFLIP = AVG_LST4WK_HATCH(flipDataSet.FLOCKS_DATA[0].COMPANY, flockIDHatch);
                            }
                            //}
                             * */

                            #endregion

                            CTRL_LOTE_LOC_ARMAZ_WEB tabSaldo = bdSQLServer.CTRL_LOTE_LOC_ARMAZ_WEB
                                .Where(w => w.LoteCompleto == flockID && w.DataProducao == dataPrd)
                                .FirstOrDefault();

                            if (tabSaldo != null)
                            {
                                tabSaldo.Nucleo = flipDataSet.FLOCKS_DATA[i].FARM_ID;
                                tabSaldo.IdadeLote = (short)flipDataSet.FLOCKS_DATA[i].AGE;
                                tabSaldo.PercMediaIncUlt4SemFLIP = 
                                    AVG_LST4WK_HATCH(flipDataSet.FLOCKS_DATA[0].COMPANY, flockIDHatch);
                            }

                            flockData.AtualizaImportado(1, dataAtual, flockKey);
                        }

                        //bdApolo.SaveChanges();
                        bdSQLServer.SaveChanges();

                        #endregion

                        #region Rotina Atualiza Estoque caso houve deleção no FLIP

                        DateTime dataInicial = DateTime.Today.AddDays(-60);
                        DateTime dataFinal = DateTime.Today;
                        //DateTime dataInicial = Convert.ToDateTime("24/11/2014");
                        //DateTime dataFinal = Convert.ToDateTime("25/11/2014");

                        #region **** NÃO INTEGRA MAIS COM O APOLO ****

                        /*
                        var listaImportadosApolo = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                                    .Where(l => l.CtrlLoteDataValid >= dataInicial && l.CtrlLoteDataValid <= dataFinal
                                        && l.EmpCod == empresaFilial.EmpCod
                                        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                            && m.TipoLancCod == tipoLancEntrada))
                                    .ToList();
                         * */

                        #endregion

                        var listaImportadosWEB = bdSQLServer.FLOCK_DATA
                            .Where(w => w.Trx_Date >= dataInicial && w.Trx_Date <= dataFinal).ToList();

                        foreach (var item in listaImportadosWEB)
                        {
                            FLIPDataSet.FLOCK_DATADataTable flock = new FLIPDataSet.FLOCK_DATADataTable();

                            flockData.FillByFlockTrxDate(flock, item.Flock_ID, Convert.ToDateTime(item.Trx_Date));

                            if (flock.Count == 0)
                            {
                                #region **** NÃO INTEGRA MAIS COM O APOLO ****

                                /*

                                ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));
                                ObjectParameter rchave = new ObjectParameter("rchave", typeof(global::System.Int32));
                                ObjectParameter rdatamovimento = new ObjectParameter("rdatamovimento", typeof(global::System.DateTime));

                                bdApolo.analisa_delecao_entrada_estq(item.EmpCod, item.MovEstqChv, item.ProdCodEstr, item.CtrlLoteDataValid,
                                    item.CtrlLoteItMovEstqQtd, rmensagem, rchave, rdatamovimento);

                                if (rmensagem.Value.ToString().Equals("Não"))
                                {
                                    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(item);

                                    bdApolo.SaveChanges();

                                    int existe = 0;

                                    existe = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.Where(c => c.EmpCod == item.EmpCod && c.MovEstqChv == item.MovEstqChv
                                        && c.ProdCodEstr == item.ProdCodEstr && c.ItMovEstqSeq == item.ItMovEstqSeq).Count();

                                    if (existe == 0)
                                    {
                                        localArmaz = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(i => i.EmpCod == item.EmpCod
                                            && i.MovEstqChv == item.MovEstqChv && i.ProdCodEstr == item.ProdCodEstr
                                            && i.ItMovEstqSeq == item.ItMovEstqSeq && i.LocArmazCodEstr == item.LocArmazCodEstr).FirstOrDefault();

                                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.DeleteObject(localArmaz);

                                        itemMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == item.EmpCod && i.MovEstqChv == item.MovEstqChv
                                            && i.ProdCodEstr == item.ProdCodEstr && i.ItMovEstqSeq == item.ItMovEstqSeq).FirstOrDefault();

                                        bdApolo.ITEM_MOV_ESTQ.DeleteObject(itemMovEstq);

                                        bdApolo.SaveChanges();

                                        existe = 0;
                                        existe = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == item.EmpCod && i.MovEstqChv == item.MovEstqChv)
                                            .Count();

                                        if (existe == 0)
                                        {
                                            movEstq = bdApolo.MOV_ESTQ.Where(i => i.EmpCod == item.EmpCod && i.MovEstqChv == item.MovEstqChv)
                                                .FirstOrDefault();

                                            bdApolo.MOV_ESTQ.DeleteObject(movEstq);

                                            bdApolo.SaveChanges();
                                        }
                                    }

                                    itemMovEstq = bdApolo.ITEM_MOV_ESTQ
                                            .Where(i => i.EmpCod == item.EmpCod && i.ProdCodEstr == item.ProdCodEstr
                                                && i.ItMovEstqDataMovimento >= item.CtrlLoteDataValid)
                                            .OrderBy(i2 => i2.ItMovEstqDataMovimento)
                                            .FirstOrDefault();

                                    //bdApolo.atualiza_saldoestqdata(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr,
                                    //    itemMovEstq.ItMovEstqSeq, itemMovEstq.ItMovEstqDataMovimento, "UPD");
                                }
                                else
                                {
                                    string corpo = "Erro ao ajustar Lote que não existe mais no FLIP: Lote - "
                                        + item.CtrlLoteNum + " Data - " + item.CtrlLoteDataValid.ToShortDateString()
                                        + " Erro - " + rmensagem.Value.ToString();

                                    EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR DIARIO DA GRANJA ****", "Paulo Alves",
                                        "palves@hyline.com.br", "", "");
                                }
                                 * */

                                #endregion

                                DeletaDiarioProducaoWEB(item.Flock_ID, Convert.ToDateTime(item.Trx_Date));
                            }
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Exceção

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpo = "Erro ao Importar Diário da Granja " + farm + ": "
                    + ex.Message;
                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        corpo = (char)10 + (char)13 + corpo + ex.InnerException.Message;

                //EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR DIARIO DA GRANJA ****", "Paulo Alves",
                //    "palves@hyline.com.br", "", "");

                this.EventLog.WriteEntry(corpo, EventLogEntryType.Error, 10);

                #endregion
            }
        }

        public void InsereProducaoEstoqueApolo()
        {
            farms.FillFarmsAll(flipDataSet.FARMS_IMPORT);

            for (int i = 0; i < flipDataSet.FARMS_IMPORT.Count; i++)
            {
                //if (flipDataSet.FARMS_IMPORT[i].FARM == "HL")
                    InsereProducaoEstoqueApoloPorGranja(flipDataSet.FARMS_IMPORT[i].FARM, flipDataSet.FARMS_IMPORT[i].LOCATION,
                        flipDataSet.FARMS_IMPORT[i].CODAPOLO, flipDataSet.FARMS_IMPORT[i].TERCEIRO,
                        flipDataSet.FARMS_IMPORT[i].TIPOLANCENTRADAAPOLO);
            }
        }

        public decimal AVG_LST4WK_HATCH(string comp, string flockid)
        {
            int age = (int)hatcheryFlockData.MaxAge(comp, flockid);

            avgLst4WkHatch.FillAVG_LST4WK_HATCH(flipDataSet.AVG_LST4WK_HATCH, comp, flockid, age);

            decimal ihatch = flipDataSet.AVG_LST4WK_HATCH[0].ACTUAL;
            decimal ircv = flipDataSet.AVG_LST4WK_HATCH[0].EGGSRCVD;
            decimal idirts = flipDataSet.AVG_LST4WK_HATCH[0].DIRTS;

            decimal iavg = 0;
            if (ircv != 0)
             iavg = (ihatch / (ircv -idirts)) * 100.0m;

            return iavg;
        }

        public bool ExisteFechamentoEstoque(DateTime dataMov, string granja)
        {
            #region Fechamento Estoque Apolo - DESATIVADO

            //EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
            //                        .Where(e => e.USERFLIPCod == granja)
            //                        .FirstOrDefault();

            //if (empresa != null)
            //{
            //    int existe = 0;
            //    existe = bdApolo.Fech_Estq.Where(f => f.FechEstqData >= dataMov && f.EmpCod == empresa.EmpCod)
            //        .Count();

            //    if (existe > 0)
            //        return true;
            //    else
            //        return false;
            //}
            //else
            //    return false;

            #endregion

            #region Fechamento Estoque - Tabela FLIP

            FLIPDataSet.DATA_FECH_LANCDataTable DfDT = new FLIPDataSet.DATA_FECH_LANCDataTable();
            DATA_FECH_LANCTableAdapter DfTA = new DATA_FECH_LANCTableAdapter();
            DfTA.Fill(DfDT);

            string filtroLocal = "Granjas Matrizes";
            if (granja.Equals("SB")) filtroLocal = "Granjas Avos";

            if (DfDT.Count > 0)
            {
                FLIPDataSet.DATA_FECH_LANCRow DfRow = DfDT.Where(w => w.DATA_FECH_LANC >= dataMov
                    && w.LOCATION == filtroLocal)
                    .FirstOrDefault();

                if (DfRow != null)
                    return true;
                else
                    return false;
            }
            else
                return false;

            #endregion
        }

        public LayoutDiarioExpedicaos InsereDEOTerceiro(string lote, int idade, DateTime dataPrd, decimal qtdOvos, 
            int chaveMovEstq, string incubatorio)
        {
            flocks.FillByFlockID(flipDataSet.FLOCKS, lote);

            string nucleo = flipDataSet.FLOCKS[0].FARM_ID;
            string numLote = flipDataSet.FLOCKS[0].NUM_1.ToString();
            string linhagem = flipDataSet.FLOCKS[0].VARIETY;
            string granja = flipDataSet.FLOCKS[0].FARM_ID.Substring(0, 2);

            LayoutDiarioExpedicaos deo = new LayoutDiarioExpedicaos();

            deo.Nucleo = nucleo;
            deo.Galpao = RetornaNumeroGalpao(lote);
            deo.Lote = numLote;
            deo.Idade = idade;
            deo.Linhagem = linhagem;
            deo.LoteCompleto = lote;
            deo.DataProducao = dataPrd;
            deo.NumeroReferencia = DateTime.Now.DayOfYear.ToString();
            deo.QtdeOvos = Convert.ToDecimal(qtdOvos);
            deo.QtdeBandejas = Convert.ToDecimal(qtdOvos / 360);
            deo.Usuario = "SISTEMA SERVICE";
            deo.DataHora = DateTime.Now;
            deo.DataHoraCarreg = dataPrd;
            deo.NFNum = chaveMovEstq.ToString();
            deo.Granja = granja;
            deo.Importado = "Sim";
            deo.Incubatorio = incubatorio;
            deo.TipoDEO = "Ovos Incubáveis";
            deo.DataHoraRecebInc = Convert.ToDateTime("01/01/1899");
            deo.ResponsavelCarreg = "";
            deo.ResponsavelReceb = "";

            return deo;
        }

        public string RetornaNumeroGalpao(string lote)
        {
            int tamanho = lote.Length - 1;

            string galpao = "";

            for (int i = tamanho; i >= 0; i--)
            {
                double Num;
                bool isNum = double.TryParse(lote.Substring(i, 1), out Num);

                if ((isNum) && (galpao.Equals("")))
                {
                    galpao = "0" + lote.Substring(i, 1);
                }
            }

            return galpao;
        }

        public void EnviarEmail(string corpoEmail, string assunto, string paraNome, string paraEmail,
            string copiaPara, string anexo)
        {
            WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

            bdApolo.gerar_codigo("1", "WORKFLOW_EMAIL", numero);

            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
            email.WorkFlowEmailStat = "Enviar";
            //email.WorkFlowEmailAssunto = "**** LOGIN PARA ACESSO AO HY-LINE APP ****";
            email.WorkFlowEmailAssunto = assunto;
            email.WorkFlowEmailData = DateTime.Now;
            email.WorkFlowEmailParaNome = paraNome;
            email.WorkFlowEmailParaEmail = paraEmail;
            //email.WorkFlowEmailParaNome = "Paulo Alves";
            //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
            email.WorkFlowEmailCopiaPara = copiaPara;
            email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
            email.WorkFLowEmailDeEmail = "palves@hyline.com.br";
            email.WorkFlowEmailFormato = "Texto";
            email.WorkFlowEmailDocEmpCod = "5";

            //corpoEmail = "Prezado," + (char)13 + (char)10 + (char)13 + (char)10
            //    + "Para melhorarmos o controle de nossos processos, foi desenvolvida a ferramenta para preenchimento e importação de Pedidos. " + (char)13 + (char)10
            //    + "Através dela iremos diminuir os erros para acelerar e melhorar os processos." + (char)13 + (char)10
            //    + "Sendo assim, segue abaixo o login e senha para acesso ao site para dados da empresa " + empresa + "." + (char)13 + (char)10 + (char)13 + (char)10
            //    + "Login: " + dsCHIC.salesman1[i].email.Trim() + (char)13 + (char)10
            //    + "Senha: " + dsCHIC.salesman1[i].senha.Trim() + (char)13 + (char)10 + (char)13 + (char)10
            //    + "Também, segue em anexo o manual para acesso ao site." + (char)13 + (char)10
            //    + "Qualquer dúvida, entrar em contato pelo e-mail ti@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
            //    + "SISTEMA WEB";

            email.WorkFlowEmailCorpo = corpoEmail;
            email.WorkFlowEmailArquivosAnexos = anexo;

            bdApolo.WORKFLOW_EMAIL.AddObject(email);

            bdApolo.SaveChanges();
        }

        public void InventarioOvos()
        {
            try
            {   
                EMPRESA_FILIAL empresaFilial = bdApolo.EMPRESA_FILIAL
                        .Where(e => e.EmpCod == "20")
                        .FirstOrDefault();

                if (empresaFilial != null)
                {
                    #region Carrega Variáveis e Objetos

                    string empresa = empresaFilial.EmpCod;
                    //string tipoLanc = "E0000011";
                    string tipoLanc = "E0000170";
                    string tipoLancEntrada = "E0000170";
                    string entCod = "";

                    entCod = empresaFilial.EntCod;
                    //entCod = "0010199";

                    string naturezaOperacao = "1.556.001";
                    decimal valorUnitario = 0.90m;
                    string unidadeMedida = "UN";
                    short? posicaoUnidadeMedida = 1;
                    string tribCod = "040";
                    string itMovEstqClasFiscCodNbm = "04079000";
                    string clasFiscCod = "0000129";
                    string operacao = "Entrada";
                    string usuario = "RIOSOFT";

                    LOC_ARMAZ localArmazCadastro = bdApolo.LOC_ARMAZ
                        .Where(l => l.USERCodigoFLIP == "T1")
                        .FirstOrDefault();

                    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                    DateTime dataAnterior = Convert.ToDateTime("01/01/2014");
                    DateTime dataAtual;

                    string linhaAnterior = "";
                    string linhaAtual = "";

                    //decimal qtdTotalItem = 0;

                    MOV_ESTQ movEstq = new MOV_ESTQ();
                    ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
                    LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = new LOC_ARMAZ_ITEM_MOV_ESTQ();
                    CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = new CTRL_LOTE_ITEM_MOV_ESTQ();

                    //var lista = bdSQLServer.Inv_Ovos
                    //    .Where(o => o.LoteCompleto.Contains("SJ1"))
                    //    .OrderBy(i => i.DataProducao).ThenBy(t => t.Linhagem).ToList();

                    var listaPlanalto = bdSQLServer.Estq_Planalto
                        .Where(w => w.T1 != null)
                        .OrderBy(o => o.Linha).ThenBy(t => t.Lote_Completo).ThenBy(t => t.Data_Producao)
                        .ToList();

                    #endregion

                    foreach (var item in listaPlanalto)
                    {
                        //dataAtual = Convert.ToDateTime(item.DataProducao);
                        dataAtual = Convert.ToDateTime("02/06/2016");
                        DateTime dataProducao = item.Data_Producao;
                        linhaAtual = item.Linha;

                        if (!ExisteFechamentoEstoque(dataAtual, empresaFilial.USERFLIPCod))
                        {
                            #region Carrega Lote
                            //string flockKey = flipDataSet.FLOCKS_DATA[i].FLOCK_KEY;

                            decimal quantidade = Convert.ToDecimal(item.T1);
                            string numLote = item.Lote_Completo;

                            loteItemMovEstq = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                                .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataProducao 
                                    && l.EmpCod == empresaFilial.EmpCod 
                                    && l.LocArmazCodEstr == localArmaz.LocArmazCodEstr
                                    && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                        && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada))
                                .FirstOrDefault();

                            #endregion

                            if (loteItemMovEstq == null)
                            {
                                #region Carrega Produto

                                string replace = "" + (char)13 + (char)10;

                                PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == linhaAtual).FirstOrDefault();

                                itemMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                    && im.ProdCodEstr == produto.ProdCodEstr
                                    && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                        && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada))
                                .FirstOrDefault();
                                #endregion

                                if (itemMovEstq == null)
                                {
                                    #region Carrega Movimentação. Se não existe, insere.
                                    movEstq = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                        && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada)
                                    .FirstOrDefault();

                                    if (movEstq == null)
                                    {
                                        movEstq = InsereMovEstq(empresa, tipoLanc, entCod, dataAtual, usuario);
                                        movEstq.MovEstqObs = "Inventário de Ovos p/ Saldo Inicial da Planalto.";

                                        bdApolo.MOV_ESTQ.AddObject(movEstq);

                                        bdApolo.SaveChanges();
                                    }
                                    #endregion

                                    #region Se Item não existe, insere item, local e lote
                                    itemMovEstq = InsereItemMovEstq(movEstq.MovEstqChv, empresa, tipoLanc, entCod, dataAtual, linhaAtual, naturezaOperacao,
                                        quantidade, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod, itMovEstqClasFiscCodNbm,
                                        clasFiscCod);

                                    bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                    localArmaz = InsereLocalArmazenagem(movEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                        quantidade, quantidade, localArmazenagem);

                                    bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                    loteItemMovEstq = InsereLote(movEstq.MovEstqChv, empresa, tipoLanc, itemMovEstq.ItMovEstqSeq, 
                                        itemMovEstq.ProdCodEstr, numLote,
                                        dataProducao, quantidade, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                    bdApolo.SaveChanges();

                                    bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        dataAtual, "INS");

                                    bdApolo.calcula_mov_estq(empresa, movEstq.MovEstqChv);
                                    #endregion
                                }
                                else
                                {
                                    #region Se existe Item, insere lote e atualiza item e local

                                    loteItemMovEstq = InsereLote(itemMovEstq.MovEstqChv, empresa, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                        dataProducao, quantidade, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                    itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + quantidade;
                                    itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                    localArmaz = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                        && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                        && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                        .FirstOrDefault();

                                    if (localArmaz == null)
                                    {
                                        localArmaz = InsereLocalArmazenagem(itemMovEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                        quantidade, quantidade, localArmazenagem);

                                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);
                                    }
                                    else
                                    {
                                        localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + quantidade;
                                        localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;
                                    }

                                    bdApolo.SaveChanges();

                                    bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        dataAtual, "UPD");

                                    bdApolo.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                    #endregion
                                }
                            }
                            else
                            {
                                #region Se existe o lote e a quantidade é diferente, atualiza a quantidade

                                if (loteItemMovEstq.CtrlLoteItMovEstqQtd != quantidade)
                                {
                                    ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));
                                    ObjectParameter rchave = new ObjectParameter("rchave", typeof(global::System.Int32));
                                    ObjectParameter rdatamovimento = new ObjectParameter("rdatamovimento", typeof(global::System.DateTime));

                                    bdApolo.analisa_alteracao_entrada_estq(loteItemMovEstq.EmpCod, loteItemMovEstq.MovEstqChv,
                                        loteItemMovEstq.ItMovEstqSeq, loteItemMovEstq.ProdCodEstr, dataAtual, quantidade,
                                        rmensagem, rchave, rdatamovimento);

                                    if (rmensagem.Value.ToString().Equals("Não"))
                                    {
                                        decimal? qtdAntiga = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                        loteItemMovEstq.CtrlLoteItMovEstqQtd = quantidade;
                                        loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                        PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == linhaAtual).FirstOrDefault();

                                        itemMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                            && im.ProdCodEstr == produto.ProdCodEstr
                                            && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                                && m.MovEstqDataMovimento == dataAtual && m.TipoLancCod == tipoLancEntrada))
                                        .FirstOrDefault();

                                        itemMovEstq.ItMovEstqQtdProd = Convert.ToDecimal((itemMovEstq.ItMovEstqQtdProd - qtdAntiga) + quantidade);
                                        itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                        localArmaz = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                            && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                            && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                            .FirstOrDefault();

                                        localArmaz.LocArmazItMovEstqQtd = (localArmaz.LocArmazItMovEstqQtd - qtdAntiga) + quantidade;
                                        localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                        bdApolo.SaveChanges();

                                        bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                            dataAtual, "UPD");

                                        bdApolo.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);
                                    }
                                    else
                                    {
                                        string corpo = "O Lote " + numLote + " da data de Produção " + dataAtual + " não foi alterado "
                                            + "porque não ficará com saldo possível!";

                                        EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR DIARIO DA GRANJA ****", "Paulo Alves",
                                            "palves@hyline.com.br", "", "");
                                    }
                                }

                                #endregion
                            }

                            linhaAnterior = item.Linha;
                            //dataAnterior = Convert.ToDateTime(item.DataProducao);
                            dataAnterior = dataAtual;
                        }
                    }

                    bdApolo.SaveChanges();
                    bdSQLServer.SaveChanges();

                    #region Atualiza Núcleo, Idade e Média das Últimas 4 Semanas
                    foreach (var item in listaPlanalto)
                    {
                        DateTime dataPrd = Convert.ToDateTime(item.Data_Producao);

                        //flockData.FillByFlockTrxDate(flipDataSet.FLOCK_DATA, item.Lote_Completo, dataPrd);

                        //string flockKey = flipDataSet.FLOCK_DATA[0].FLOCK_KEY;

                        //dataAtual = flipDataSet.FLOCK_DATA[0].TRX_DATE;

                        ////if (!terceiro.Equals("SIM"))
                        ////{
                        //string flockID = flipDataSet.FLOCK_DATA[0].FLOCK_ID;
                        string flockID = item.Lote_Completo;

                        //string flockIDHatch = flipDataSet.FLOCK_DATA[0].FARM_ID + "-" + flockID;

                        CTRL_LOTE lote = bdApolo.CTRL_LOTE
                            .Where(c => c.EmpCod == empresa && c.CtrlLoteNum == flockID
                                && c.CtrlLoteDataValid == dataPrd)
                            //&& (c.USERGranjaNucleoFLIP == null || c.USERGranjaNucleoFLIP == ""))
                            .FirstOrDefault();

                        if (lote != null)
                        {
                            //lote.USERGranjaNucleoFLIP = flipDataSet.FLOCK_DATA[0].FARM_ID;
                            //lote.USERIdateLoteFLIP = (short)flipDataSet.FLOCK_DATA[0].AGE;
                            //lote.USERPercMediaIncUlt4SemFLIP = AVG_LST4WK_HATCH(flipDataSet.FLOCK_DATA[0].COMPANY, flockIDHatch);

                            lote.USERGranjaNucleoFLIP = item.Nucleo;
                            lote.USERIdateLoteFLIP = (short)item.Idade;
                            lote.USERPercMediaIncUlt4SemFLIP = 0;
                        }
                        //}

                        //flockData.AtualizaImportado(1, dataAtual, flockKey);
                    }
                    #endregion

                    bdApolo.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string corpo = "Erro ao Importar Diário da Granja " + "CH" + ": "
                    + ex.Message;

                EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR DIARIO DA GRANJA ****", "Paulo Alves",
                    "palves@hyline.com.br", "", "");
            }
        }

        public void AjustaEggInvFLIP()
        {
            //DateTime data = Convert.ToDateTime("15/02/2015");

            //var lista = bdApolo.CTRL_LOTE_LOC_ARMAZ
            //    .Where(c => c.CtrlLoteLocArmazQtdSaldo > 0 && c.EmpCod == "1"
            //        //&& c.LocArmazCodEstr == "04.011"
            //        //&& c.CtrlLoteNum == "P115251HB" && c.CtrlLoteDataValid == data
            //        && bdApolo.LOC_ARMAZ.Any(l => l.LocArmazCodEstr == c.LocArmazCodEstr
            //            //&& (l.USERCodigoFLIP.Equals("CH") || l.USERCodigoFLIP.Equals("PH") || l.USERCodigoFLIP.Equals("TB"))))
            //            //&& (!l.USERCodigoFLIP.Equals("CH") && !l.USERCodigoFLIP.Equals("PH") && !l.USERCodigoFLIP.Equals("TB"))))
            //            //&& (!l.USERCodigoFLIP.Equals("SB") && !l.USERCodigoFLIP.Equals("PH") && !l.USERCodigoFLIP.Equals("TB"))))
            //            //&& (l.USERCodigoFLIP.Equals("PH") || l.USERCodigoFLIP.Equals("SB"))))
            //            && l.USERCodigoFLIP.Equals("TB")))
            //    .ToList();

            //foreach (var item in lista)
            //{
            //    LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
            //            //.Where(l => l.LocArmazCodEstr == item.LocArmazCodEstr)
            //            //.Where(l => l.LocArmazCodEstr == "05.05")
            //            //.Where(l => l.LocArmazCodEstr == "01.07")
            //            .Where(l => l.LocArmazCodEstr == "15.01")
            //            .FirstOrDefault();

            //    //if ((locArmaz.USERCodigoFLIP == "CH") || (locArmaz.USERCodigoFLIP == "PH") || (locArmaz.USERCodigoFLIP == "TB"))
            //    //{
            //        int existe = Convert.ToInt32(eggInvData.ScalarQueryOpen2(item.CtrlLoteNum, item.CtrlLoteDataValid, locArmaz.USERCodigoFLIP));

            //        if (existe == 0)
            //        {
            //            flocks.FillByFlockIDAndLocation(flipDataSet.FLOCKS, item.CtrlLoteNum, locArmaz.USERGeracaoFLIP);
            //            string farmID = flipDataSet.FLOCKS[0].FARM_ID;
            //            string trackNO = "EXP" + item.CtrlLoteDataValid.ToString("yyMMdd");
                        
            //            //int? uSERQtdeIncNaoImportApolo = 0;
            //            //if (item.USERQtdeIncNaoImportApolo != null) uSERQtdeIncNaoImportApolo = item.USERQtdeIncNaoImportApolo;
            //            //decimal qtd = item.CtrlLoteLocArmazQtdSaldo - uSERQtdeIncNaoImportApolo;
            //            decimal? qtd = item.CtrlLoteLocArmazQtdSaldo;

            //            eggInvData.Insert("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, item.CtrlLoteNum,
            //                trackNO, item.CtrlLoteDataValid, qtd, "O", null,
            //                null, null, null, null, null, null, null, locArmaz.USERCodigoFLIP, null);
            //        }
            //        else
            //        {
            //            eggInvData.FillByFlockLayDateStatus(flipDataSet.EGGINV_DATA, item.CtrlLoteNum, "O", item.CtrlLoteDataValid);

            //            //FLIPDataSet.EGGINV_DATADataTable egginvdata = new FLIPDataSet.EGGINV_DATADataTable();

            //            var lista2 = flipDataSet.EGGINV_DATA.Where(e => e.LOCATION == locArmaz.USERGeracaoFLIP
            //                && e.HATCH_LOC == locArmaz.USERCodigoFLIP).ToList();

            //            flocks.FillByFlockIDAndLocation(flipDataSet.FLOCKS, item.CtrlLoteNum, locArmaz.USERGeracaoFLIP);
            //            string farmID = flipDataSet.FLOCKS[0].FARM_ID;
            //            string trackNO = "EXP" + item.CtrlLoteDataValid.ToString("yyMMdd");

            //            foreach (var item2 in lista2)
            //            {
            //                //if (item2.EGG_UNITS != item.CtrlLoteLocArmazQtdSaldo)
            //                //{
            //                    //int? uSERQtdeIncNaoImportApolo = 0;
            //                    //if (item.USERQtdeIncNaoImportApolo != null) uSERQtdeIncNaoImportApolo = item.USERQtdeIncNaoImportApolo;
            //                    //decimal qtd = item.CtrlLoteLocArmazQtdSaldo - uSERQtdeIncNaoImportApolo;

            //                    decimal qtd = item2.EGG_UNITS + item.CtrlLoteLocArmazQtdSaldo;
            //                    eggInvData.UpdateQueryEggs(qtd, "HYBR", "BR", locArmaz.USERGeracaoFLIP,
            //                        farmID, item.CtrlLoteNum, trackNO, item.CtrlLoteDataValid, "O", locArmaz.USERCodigoFLIP);
            //                //}
            //            }
            //        }
            //    //}
            //}
        }

        public void AjustaEggInvFLIPNegativo()
        {
            eggInvData.FillByNegative(flipDataSet.EGGINV_DATA);

            for (int i = 0; i < flipDataSet.EGGINV_DATA.Count; i++)
            {
                string numLote = flipDataSet.EGGINV_DATA[i].FLOCK_ID;
                DateTime dataPrd = flipDataSet.EGGINV_DATA[i].LAY_DATE;
                string codigoFLIP = flipDataSet.EGGINV_DATA[i].HATCH_LOC;

                LOC_ARMAZ local = bdApolo.LOC_ARMAZ
                    .Where(a => a.USERCodigoFLIP == codigoFLIP)
                    .FirstOrDefault();

                CTRL_LOTE_LOC_ARMAZ loteApolo = bdApolo.CTRL_LOTE_LOC_ARMAZ
                    .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataPrd
                        && l.EmpCod == "1" && l.LocArmazCodEstr == local.LocArmazCodEstr)
                    .FirstOrDefault();

                eggInvData.UpdateQueryEggs(loteApolo.CtrlLoteLocArmazQtdSaldo, "HYBR", "BR", flipDataSet.EGGINV_DATA[i].LOCATION,
                    flipDataSet.EGGINV_DATA[i].FARM_ID, flipDataSet.EGGINV_DATA[i].FLOCK_ID, flipDataSet.EGGINV_DATA[i].TRACK_NO,
                    flipDataSet.EGGINV_DATA[i].LAY_DATE, flipDataSet.EGGINV_DATA[i].STATUS, flipDataSet.EGGINV_DATA[i].HATCH_LOC);
            }
        }

        public void AjusteLotesNegativosApolo()
        {
            bdApolo.CommandTimeout = 100000;
            bdSQLServer.CommandTimeout = 100000;

            #region Lista de Lotes Negativos

            DateTime dataPrdParam = Convert.ToDateTime("13/01/2015");
            string loteParam = "P135242W";

            var listaLoteNegativos = bdApolo.CTRL_LOTE_LOC_ARMAZ
                .Where(c => //c.CtrlLoteLocArmazQtdSaldo < 0 && 
                    c.CtrlLoteNum == loteParam && c.CtrlLoteDataValid == dataPrdParam
                    && c.LocArmazCodEstr == "03.010.003"
                 )
                .OrderByDescending(o => o.CtrlLoteDataValid)
                .ToList();

            //var listaLoteNegativos = bdApolo.CTRL_LOTE.Where(c => c.EmpCod == "1"

            #endregion

            foreach (var loteNegativo in listaLoteNegativos)
            {
                #region Deleta as Movimentações de Estoque do Apolo relacionados ao lote

                DeletaEstoqueApolo(loteNegativo.ProdCodEstr, loteNegativo.CtrlLoteNum, loteNegativo.CtrlLoteDataValid);

                bdApolo.SaveChanges();

                #endregion

                #region Ajusta a tabela de Importação do DEO p/ o Apolo

                var listaDEOs = bdSQLServer.LayoutDiarioExpedicaos
                    .Where(l => l.LoteCompleto == loteNegativo.CtrlLoteNum
                        && l.DataProducao == loteNegativo.CtrlLoteDataValid
                        && (l.TipoDEO == "Ovos Incubáveis" || l.TipoDEO == "Transf. Ovos Incubáveis"))
                    .OrderBy(o => o.DataHoraCarreg)
                    .ToList();

                foreach (var deo in listaDEOs)
                {
                    #region Exclusão dos registros p/ Importação

                    var listaImportDEO = bdSQLServer.ImportaDiarioExpedicao
                        .Where(i => bdSQLServer.LayoutDEO_X_ImportaDEO.Any(x => x.CodItemImportaDEO == i.CodItemImportaDEO
                                    && x.CodItemDEO == deo.CodItemDEO))
                        .ToList();

                    foreach (var item in listaImportDEO)
                    {
                        LayoutDEO_X_ImportaDEO relacionamento = bdSQLServer.LayoutDEO_X_ImportaDEO
                            .Where(r => r.CodItemDEO == deo.CodItemDEO
                                && r.CodItemImportaDEO == item.CodItemImportaDEO)
                            .FirstOrDefault();

                        if (relacionamento != null)
                        {
                            bdSQLServer.LayoutDEO_X_ImportaDEO.DeleteObject(relacionamento);
                            bdSQLServer.ImportaDiarioExpedicao.DeleteObject(item);
                        }
                    }

                    bdSQLServer.SaveChanges();

                    #endregion
                }

                foreach (var deo in listaDEOs)
                {
                    #region Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                    #region Localiza os Lotes

                    LOC_ARMAZ localArmazenagem = bdApolo.LOC_ARMAZ
                        .Where(l => l.USERCodigoFLIP == deo.Granja && l.USERTipoProduto == "Ovos Incubáveis")
                        .FirstOrDefault();

                    var listaLotes = bdApolo.CTRL_LOTE_LOC_ARMAZ
                        .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                            && c.CtrlLoteDataValid <= deo.DataProducao
                            && c.EmpCod == "1"
                            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                            && c.CtrlLoteLocArmazQtdSaldo > 0
                            && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid
                                && l.USERGranjaNucleoFLIP.Contains(deo.Granja)))
                        .OrderByDescending(o => o.CtrlLoteDataValid)
                        .ToList();

                    #endregion

                    int saldo = Convert.ToInt32(deo.QtdeOvos);
                    int disponivel = 0;

                    foreach (var item in listaLotes)
                    {
                        #region Verifica quantidade já inserida

                        int saldoDisponivel = 0;
                        int qtdInseridaNaoBaixada = 0;

                        ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                            .Where(i => i.Granja == deo.Granja
                                && i.LoteCompleto == deo.LoteCompleto
                                //&& i.DataHoraCarreg == deo.DataHoraCarreg
                                && i.DataProducao == item.CtrlLoteDataValid
                                && i.Importado != "Conferido")
                            .FirstOrDefault();

                        if (importaDEO == null)
                        {
                            qtdInseridaNaoBaixada = 0;
                        }
                        else
                        {
                            qtdInseridaNaoBaixada = Convert.ToInt32(importaDEO.QtdeOvos);
                        }

                        saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                        #endregion

                        if (saldo > saldoDisponivel)
                        {
                            #region Se saldo maior que o disponível, insere o disponivel para a Data

                            saldo = saldo - saldoDisponivel;
                            disponivel = disponivel + saldoDisponivel;

                            importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = deo.DataHoraCarreg;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = Convert.ToInt32(deo.CodItemDEO);
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            #endregion
                        }
                        else if (saldo > 0)
                        {
                            #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                            importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = deo.DataHoraCarreg;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            disponivel = disponivel + saldo;
                            saldo = 0;
                            break;

                            #endregion
                        }
                    }

                    bdSQLServer.SaveChanges();

                    #endregion
                }

                #endregion

                #region Importa DEOs p/ o Apolo

                var listaDEOsImportacao = bdSQLServer.ImportaDiarioExpedicao
                    .Where(i => bdSQLServer.LayoutDEO_X_ImportaDEO.Any(x => x.CodItemImportaDEO == i.CodItemImportaDEO
                        && bdSQLServer.LayoutDiarioExpedicaos.Any(l => l.CodItemDEO == x.CodItemDEO
                            && l.LoteCompleto == loteNegativo.CtrlLoteNum
                            && l.DataProducao == loteNegativo.CtrlLoteDataValid
                            && (l.TipoDEO == "Ovos Incubáveis" || l.TipoDEO == "Transf. Ovos Incubáveis"))))
                    .OrderBy(o => new { o.DataHoraCarreg, o.DataProducao })
                    .ToList();

                foreach (var deoImportacao in listaDEOsImportacao)
                {
                    ImportaDEOApoloItem(deoImportacao);
                    deoImportacao.Importado = "Conferido";

                    bdSQLServer.SaveChanges();
                }

                #endregion

                #region Importa Incubações p/ o Apolo

                var listaIncubacoes = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Flock_id.Contains(loteNegativo.CtrlLoteNum)
                        && h.Lay_date == loteNegativo.CtrlLoteDataValid
                        && h.ImportadoApolo == "Sim")
                    .GroupBy(g => new { g.Set_date, g.Hatch_loc, g.Flock_id, g.Lay_date })
                    .ToList();

                foreach (var item in listaIncubacoes)
                {
                    ImportaIncubacaoApoloItem(item.Key.Hatch_loc, item.Key.Set_date, item.Key.Flock_id, item.Key.Lay_date);
                    //ImportaIncubacaoApolo(item.Key.Hatch_loc, item.Key.Set_date);
                }

                #endregion

                //ITEM_MOV_ESTQ itemEntradaDeOvos = bdApolo.ITEM_MOV_ESTQ
                //    .Where(i => i.EmpCod == "1" && i.ProdCodEstr == loteNegativo.ProdCodEstr
                //        && bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.Any(c => c.EmpCod == i.EmpCod
                //            && c.MovEstqChv == i.MovEstqChv && c.ProdCodEstr == i.ProdCodEstr
                //            && c.ItMovEstqSeq == i.ItMovEstqSeq
                //            && c.CtrlLoteNum == loteNegativo.CtrlLoteNum
                //            && c.CtrlLoteDataValid == loteNegativo.CtrlLoteDataValid)
                //        && bdApolo.TIPO_LANC.Any(t => i.TipoLancCod == t.TipoLancCod
                //            && t.TipoLancNome.Contains("ENTRADA DE OVOS")))
                //    .OrderBy(o => o.ItMovEstqDataMovimento)
                //    .FirstOrDefault();

                //bdApolo.atualiza_saldoestqdata(itemEntradaDeOvos.EmpCod, itemEntradaDeOvos.MovEstqChv, itemEntradaDeOvos.ProdCodEstr,
                //    itemEntradaDeOvos.ItMovEstqSeq, itemEntradaDeOvos.ItMovEstqDataMovimento, "UPD");
            }

            var listaLinhagens = bdApolo.PRODUTO.Where(p => p.ProdNome.Contains("ovos ferteis")
                && p.ProdNomeAlt1 != null && p.ProdNomeAlt1 != "").ToList();

            foreach (var linhagem in listaLinhagens)
            {
                ITEM_MOV_ESTQ itemEntradaDeOvos = bdApolo.ITEM_MOV_ESTQ
                    .Where(i => i.EmpCod == "1" && i.ProdCodEstr == linhagem.ProdCodEstr
                        && bdApolo.TIPO_LANC.Any(t => i.TipoLancCod == t.TipoLancCod
                            && t.TipoLancNome.Contains("ENTRADA DE OVOS")))
                    .OrderBy(o => o.ItMovEstqDataMovimento)
                    .FirstOrDefault();

                bdApolo.atualiza_saldoestqdata(itemEntradaDeOvos.EmpCod, itemEntradaDeOvos.MovEstqChv, itemEntradaDeOvos.ProdCodEstr,
                    itemEntradaDeOvos.ItMovEstqSeq, itemEntradaDeOvos.ItMovEstqDataMovimento, "UPD");
            }
        }

        public void DeletaEstoqueApolo(string prodcodestr, string lote, DateTime dataProducao)
        {
            ////PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == linha).FirstOrDefault();
            //PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdCodEstr == prodcodestr).FirstOrDefault();

            //#region Deleta as Saídas por Incubação

            ////var listaSaidasIncubacao = bdApolo.MOV_ESTQ
            ////    .Where(m => (m.TipoLancCod == "E0000503" || m.TipoLancCod == "E0000482" || m.TipoLancCod == "E0000470")
            ////        && bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.Any(c => m.EmpCod == c.EmpCod && m.MovEstqChv == c.MovEstqChv
            ////            && c.ProdCodEstr == produto.ProdCodEstr && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao))
            ////    .ToList();

            ////foreach (var item in listaSaidasIncubacao)
            ////{
            ////    ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

            ////    bdApolo.delete_movestq(item.EmpCod, item.MovEstqChv, "RIOSOFT", rmensagem);
            ////}

            //var listaSaidasIncubacao = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //    .Where(c => c.ProdCodEstr == produto.ProdCodEstr && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao
            //        && bdApolo.MOV_ESTQ.Any(m => (m.TipoLancCod == "E0000503" || m.TipoLancCod == "E0000482" || m.TipoLancCod == "E0000470")
            //            && m.EmpCod == c.EmpCod && m.MovEstqChv == c.MovEstqChv))
            //    .ToList();

            //foreach (var item in listaSaidasIncubacao)
            //{
            //    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(item);

            //    ITEM_MOV_ESTQ itemIncubacao = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == item.EmpCod && i.MovEstqChv == item.MovEstqChv
            //        && i.ProdCodEstr == item.ProdCodEstr && i.ItMovEstqSeq == item.ItMovEstqSeq).FirstOrDefault();

            //    itemIncubacao.ItMovEstqQtdProd = Convert.ToDecimal(itemIncubacao.ItMovEstqQtdProd - item.CtrlLoteItMovEstqQtd);
            //    itemIncubacao.ItMovEstqQtdCalcProd = itemIncubacao.ItMovEstqQtdProd;

            //    LOC_ARMAZ_ITEM_MOV_ESTQ localIncubacao = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(i => i.EmpCod == item.EmpCod && i.MovEstqChv == item.MovEstqChv
            //        && i.ProdCodEstr == item.ProdCodEstr && i.ItMovEstqSeq == item.ItMovEstqSeq).FirstOrDefault();

            //    localIncubacao.LocArmazItMovEstqQtd = localIncubacao.LocArmazItMovEstqQtd - item.CtrlLoteItMovEstqQtd;
            //    localIncubacao.LocArmazItMovEstqQtdCalc = localIncubacao.LocArmazItMovEstqQtd;

            //    bdApolo.SaveChanges();

            //    //bdApolo.atualiza_saldoestqdata(itemIncubacao.EmpCod, itemIncubacao.MovEstqChv, itemIncubacao.ProdCodEstr, itemIncubacao.ItMovEstqSeq,
            //            //itemIncubacao.ItMovEstqDataMovimento, "UPD");
            //}

            //#endregion

            //#region Deleta as Transferências de Ajapi

            //var listaTransferenciaAjapi = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
            //    .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
            //        && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
            //        && i.ItTransfEstqLocArmazEntrada == "15.01" 
            //            && bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.Any(l => l.EmpCod == i.EmpCod 
            //                && l.TransfEstqLocArmazNum == i.TransfEstqLocArmazNum
            //                && l.ProdCodEstr == i.ProdCodEstr 
            //                && l.ItTransfEstqLocArmazSeq == i.ItTransfEstqLocArmazSeq
            //                && l.ProdCodEstr == produto.ProdCodEstr
            //                && l.CtrlLoteNum == lote
            //                && l.CtrlLoteDataValid == dataProducao)))
            //    .ToList();

            //foreach (var item in listaTransferenciaAjapi)
            //{
            //    string numTransf = item.TransfEstqLocArmazNum.ToString();
            //    //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

            //    ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTrasnfEstqLocArmaz = bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ
            //        .Where(i => i.EmpCod == item.EmpCod && i.TransfEstqLocArmazNum == item.TransfEstqLocArmazNum
            //            && i.ProdCodEstr == produto.ProdCodEstr)
            //        .FirstOrDefault();

            //    IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE loteTELA = bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE
            //        .Where(c => c.EmpCod == itemTrasnfEstqLocArmaz.EmpCod
            //            && c.TransfEstqLocArmazNum == itemTrasnfEstqLocArmaz.TransfEstqLocArmazNum
            //            && c.ProdCodEstr == itemTrasnfEstqLocArmaz.ProdCodEstr
            //            && c.ItTransfEstqLocArmazSeq == itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazSeq
            //            && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao)
            //        .FirstOrDefault();

            //    if (loteTELA != null)
            //    {
            //        bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.DeleteObject(loteTELA);

            //        itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazQtd = itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazQtd - loteTELA.ItTransfEstqLocArmazLoteQtd;

            //        bdSQLServer.SaveChanges();
            //    }

            //    #region Deleta a Saída

            //    MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
            //        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

            //    ITEM_MOV_ESTQ saidaItem = bdApolo.ITEM_MOV_ESTQ
            //        .Where(i => i.EmpCod == saida.EmpCod && i.MovEstqChv == saida.MovEstqChv && i.ProdCodEstr == prodcodestr)
            //        .FirstOrDefault();

            //    LOC_ARMAZ_ITEM_MOV_ESTQ saidaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == saidaItem.EmpCod && c.MovEstqChv == saidaItem.MovEstqChv && c.ProdCodEstr == saidaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == saidaItem.ItMovEstqSeq)
            //        .FirstOrDefault();

            //    CTRL_LOTE_ITEM_MOV_ESTQ saidaLote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == saidaItem.EmpCod && c.MovEstqChv == saidaItem.MovEstqChv && c.ProdCodEstr == saidaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == saidaItem.ItMovEstqSeq && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao)
            //        .FirstOrDefault();

            //    if (saidaLote != null)
            //    {
            //        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(saidaLote);

            //        saidaItem.ItMovEstqQtdProd = COsaidaItem.ItMovEstqQtdProd - saidaLote.CtrlLoteItMovEstqQtd;
            //        saidaItem.ItMovEstqQtdCalcProd = saidaItem.ItMovEstqQtdProd;

            //        saidaLocal.LocArmazItMovEstqQtd = saidaLocal.LocArmazItMovEstqQtd - saidaLote.CtrlLoteItMovEstqQtd;
            //        saidaLocal.LocArmazItMovEstqQtdCalc = saidaLocal.LocArmazItMovEstqQtd;

            //        bdApolo.SaveChanges();

            //        //bdApolo.atualiza_saldoestqdata(saidaItem.EmpCod, saidaItem.MovEstqChv, saidaItem.ProdCodEstr, saidaItem.ItMovEstqSeq,
            //            //saidaItem.ItMovEstqDataMovimento, "UPD");
            //    }

            //    //bdApolo.delete_movestq(saida.EmpCod, saida.MovEstqChv, "RIOSOFT", rmensagem);

            //    #endregion

            //    #region Deleta a Entrada

            //    MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
            //        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

            //    ITEM_MOV_ESTQ entradaItem = bdApolo.ITEM_MOV_ESTQ
            //        .Where(i => i.EmpCod == entrada.EmpCod && i.MovEstqChv == entrada.MovEstqChv && i.ProdCodEstr == prodcodestr)
            //        .FirstOrDefault();

            //    LOC_ARMAZ_ITEM_MOV_ESTQ entradaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == entradaItem.EmpCod && c.MovEstqChv == entradaItem.MovEstqChv && c.ProdCodEstr == entradaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == entradaItem.ItMovEstqSeq)
            //        .FirstOrDefault();

            //    CTRL_LOTE_ITEM_MOV_ESTQ entradaLote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == entradaItem.EmpCod && c.MovEstqChv == entradaItem.MovEstqChv && c.ProdCodEstr == entradaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == entradaItem.ItMovEstqSeq && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao)
            //        .FirstOrDefault();

            //    if (saidaLote != null)
            //    {
            //        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(entradaLote);

            //        entradaItem.ItMovEstqQtdProd = entradaItem.ItMovEstqQtdProd - entradaLote.CtrlLoteItMovEstqQtd;
            //        entradaItem.ItMovEstqQtdCalcProd = entradaItem.ItMovEstqQtdProd;

            //        entradaLocal.LocArmazItMovEstqQtd = entradaLocal.LocArmazItMovEstqQtd - entradaLote.CtrlLoteItMovEstqQtd;
            //        entradaLocal.LocArmazItMovEstqQtdCalc = entradaLocal.LocArmazItMovEstqQtd;

            //        bdApolo.SaveChanges();

            //        //bdApolo.atualiza_saldoestqdata(entradaItem.EmpCod, entradaItem.MovEstqChv, entradaItem.ProdCodEstr, entradaItem.ItMovEstqSeq,
            //            //entradaItem.ItMovEstqDataMovimento, "UPD");
            //    }

            //    //bdApolo.delete_movestq(entrada.EmpCod, entrada.MovEstqChv, "RIOSOFT", rmensagem);

            //    #endregion
            //}

            //#endregion

            //#region Deleta as Transferências das Granjas

            //var listaTransferenciaGranjas = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
            //    .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
            //        && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
            //        && (i.ItTransfEstqLocArmazEntrada == "05.05" || i.ItTransfEstqLocArmazEntrada == "01.07") 
            //        && bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.Any(l => l.EmpCod == i.EmpCod
            //                && l.TransfEstqLocArmazNum == i.TransfEstqLocArmazNum
            //                && l.ProdCodEstr == i.ProdCodEstr
            //                && l.ItTransfEstqLocArmazSeq == i.ItTransfEstqLocArmazSeq
            //                && l.ProdCodEstr == produto.ProdCodEstr
            //                && l.CtrlLoteNum == lote
            //                && l.CtrlLoteDataValid == dataProducao)))
            //    .ToList();

            //foreach (var item in listaTransferenciaGranjas)
            //{
            //    string numTransf = item.TransfEstqLocArmazNum.ToString();
            //    //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

            //    ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTrasnfEstqLocArmaz = bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ
            //        .Where(i => i.EmpCod == item.EmpCod && i.TransfEstqLocArmazNum == item.TransfEstqLocArmazNum
            //            && i.ProdCodEstr == produto.ProdCodEstr)
            //        .FirstOrDefault();

            //    IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE loteTELA = bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE
            //        .Where(c => c.EmpCod == itemTrasnfEstqLocArmaz.EmpCod
            //            && c.TransfEstqLocArmazNum == itemTrasnfEstqLocArmaz.TransfEstqLocArmazNum
            //            && c.ProdCodEstr == itemTrasnfEstqLocArmaz.ProdCodEstr
            //            && c.ItTransfEstqLocArmazSeq == itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazSeq
            //            && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao)
            //        .FirstOrDefault();

            //    if (loteTELA != null)
            //    {
            //        bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.DeleteObject(loteTELA);

            //        itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazQtd = itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazQtd - loteTELA.ItTransfEstqLocArmazLoteQtd;

            //        bdSQLServer.SaveChanges();
            //    }

            //    #region Deleta a Saída

            //    MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
            //        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

            //    ITEM_MOV_ESTQ saidaItem = bdApolo.ITEM_MOV_ESTQ
            //        .Where(i => i.EmpCod == saida.EmpCod && i.MovEstqChv == saida.MovEstqChv && i.ProdCodEstr == prodcodestr)
            //        .FirstOrDefault();

            //    LOC_ARMAZ_ITEM_MOV_ESTQ saidaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == saidaItem.EmpCod && c.MovEstqChv == saidaItem.MovEstqChv && c.ProdCodEstr == saidaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == saidaItem.ItMovEstqSeq)
            //        .FirstOrDefault();
                
            //    CTRL_LOTE_ITEM_MOV_ESTQ saidaLote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == saidaItem.EmpCod && c.MovEstqChv == saidaItem.MovEstqChv && c.ProdCodEstr == saidaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == saidaItem.ItMovEstqSeq && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao)
            //        .FirstOrDefault();

            //    if (saidaLote != null)
            //    {
            //        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(saidaLote);

            //        saidaItem.ItMovEstqQtdProd = saidaItem.ItMovEstqQtdProd - saidaLote.CtrlLoteItMovEstqQtd;
            //        saidaItem.ItMovEstqQtdCalcProd = saidaItem.ItMovEstqQtdProd;

            //        saidaLocal.LocArmazItMovEstqQtd = saidaLocal.LocArmazItMovEstqQtd - saidaLote.CtrlLoteItMovEstqQtd;
            //        saidaLocal.LocArmazItMovEstqQtdCalc = saidaLocal.LocArmazItMovEstqQtd;

            //        bdApolo.SaveChanges();

            //        //bdApolo.atualiza_saldoestqdata(saidaItem.EmpCod, saidaItem.MovEstqChv, saidaItem.ProdCodEstr, saidaItem.ItMovEstqSeq,
            //            //saidaItem.ItMovEstqDataMovimento, "UPD");
            //    }

            //    //bdApolo.delete_movestq(saida.EmpCod, saida.MovEstqChv, "RIOSOFT", rmensagem);

            //    #endregion

            //    #region Deleta a Entrada

            //    MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
            //        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

            //    ITEM_MOV_ESTQ entradaItem = bdApolo.ITEM_MOV_ESTQ
            //        .Where(i => i.EmpCod == entrada.EmpCod && i.MovEstqChv == entrada.MovEstqChv && i.ProdCodEstr == prodcodestr)
            //        .FirstOrDefault();

            //    LOC_ARMAZ_ITEM_MOV_ESTQ entradaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == entradaItem.EmpCod && c.MovEstqChv == entradaItem.MovEstqChv && c.ProdCodEstr == entradaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == entradaItem.ItMovEstqSeq)
            //        .FirstOrDefault();

            //    CTRL_LOTE_ITEM_MOV_ESTQ entradaLote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == entradaItem.EmpCod && c.MovEstqChv == entradaItem.MovEstqChv && c.ProdCodEstr == entradaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == entradaItem.ItMovEstqSeq && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao)
            //        .FirstOrDefault();

            //    if (saidaLote != null)
            //    {
            //        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(entradaLote);

            //        entradaItem.ItMovEstqQtdProd = entradaItem.ItMovEstqQtdProd - entradaLote.CtrlLoteItMovEstqQtd;
            //        entradaItem.ItMovEstqQtdCalcProd = entradaItem.ItMovEstqQtdProd;

            //        entradaLocal.LocArmazItMovEstqQtd = entradaLocal.LocArmazItMovEstqQtd - entradaLote.CtrlLoteItMovEstqQtd;
            //        entradaLocal.LocArmazItMovEstqQtdCalc = entradaLocal.LocArmazItMovEstqQtd;

            //        bdApolo.SaveChanges();

            //        //bdApolo.atualiza_saldoestqdata(entradaItem.EmpCod, entradaItem.MovEstqChv, entradaItem.ProdCodEstr, entradaItem.ItMovEstqSeq,
            //            //entradaItem.ItMovEstqDataMovimento, "UPD");
            //    }

            //    //bdApolo.delete_movestq(entrada.EmpCod, entrada.MovEstqChv, "RIOSOFT", rmensagem);

            //    #endregion
            //}

            //#endregion

            //#region Deleta Movs. Pernetas

            //var listaMovTransfPerneta = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //    .Where(c => c.CtrlLoteDataValid == dataProducao && c.CtrlLoteNum == lote && c.EmpCod == "1"
            //        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == c.EmpCod && m.MovEstqChv == c.MovEstqChv
            //            && m.MovEstqDocEspec == "TLA"))
            //    .ToList();

            //foreach (var item in listaMovTransfPerneta)
            //{
            //    ITEM_MOV_ESTQ saidaItem = bdApolo.ITEM_MOV_ESTQ
            //        .Where(i => i.EmpCod == item.EmpCod && i.MovEstqChv == item.MovEstqChv && i.ProdCodEstr == prodcodestr)
            //        .FirstOrDefault();

            //    LOC_ARMAZ_ITEM_MOV_ESTQ saidaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == saidaItem.EmpCod && c.MovEstqChv == saidaItem.MovEstqChv && c.ProdCodEstr == saidaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == saidaItem.ItMovEstqSeq)
            //        .FirstOrDefault();

            //    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(item);

            //    saidaItem.ItMovEstqQtdProd = saidaItem.ItMovEstqQtdProd - item.CtrlLoteItMovEstqQtd;
            //    saidaItem.ItMovEstqQtdCalcProd = saidaItem.ItMovEstqQtdProd;

            //    saidaLocal.LocArmazItMovEstqQtd = saidaLocal.LocArmazItMovEstqQtd - item.CtrlLoteItMovEstqQtd;
            //    saidaLocal.LocArmazItMovEstqQtdCalc = saidaLocal.LocArmazItMovEstqQtd;

            //    bdApolo.SaveChanges();

            //    //bdApolo.atualiza_saldoestqdata(saidaItem.EmpCod, saidaItem.MovEstqChv, saidaItem.ProdCodEstr, saidaItem.ItMovEstqSeq,
            //        //saidaItem.ItMovEstqDataMovimento, "UPD");
            //}

            //#endregion

        }

        public void DeletaEstoqueApoloTotal()
        {
            int movestqchv = 0;
            try
            {
                ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                #region Deleta as Saídas por Incubação

                var listaSaidasIncubacao = bdApolo.MOV_ESTQ
                    .Where(m => (m.TipoLancCod == "E0000503" || m.TipoLancCod == "E0000482" || m.TipoLancCod == "E0000470"))
                    .ToList();

                foreach (var item in listaSaidasIncubacao)
                {
                    //bdApolo.delete_movestq(item.EmpCod, item.MovEstqChv, "RIOSOFT", rmensagem);
                    DeletaMovEstq(item);
                }

                //bdApolo.SaveChanges();

                #endregion

                #region Deleta as Transferências de Ajapi

                var listaTransferenciaAjapi = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                    .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
                        && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
                        && i.ItTransfEstqLocArmazEntrada == "15.01"))
                    .OrderByDescending(r => r.TransfEstqLocArmazData)
                    .ToList();

                foreach (var item in listaTransferenciaAjapi)
                {
                    string numTransf = item.TransfEstqLocArmazNum.ToString();
                    //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                    #region Deleta a Saída

                    MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (saida != null)
                    {
                        DeletaMovEstq(saida);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta a Entrada

                    MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (entrada != null)
                    {
                        DeletaMovEstq(entrada);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta Transferência

                    bdApolo.delete_transfestqlocarmaz(item.EmpCod, item.TransfEstqLocArmazNum, "RIOSOFT");

                    #endregion
                }

                #endregion

                #region Deleta as Transferências de Exportação

                var listaTransferenciaExportacao = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                    .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
                        && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
                        && i.ItTransfEstqLocArmazEntrada == "01.09"))
                    .OrderByDescending(r => r.TransfEstqLocArmazData)
                    .ToList();

                foreach (var item in listaTransferenciaExportacao)
                {
                    string numTransf = item.TransfEstqLocArmazNum.ToString();
                    //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                    #region Deleta a Saída

                    MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (saida != null)
                    {
                        DeletaMovEstq(saida);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta a Entrada

                    MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (entrada != null)
                    {
                        DeletaMovEstq(entrada);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta Transferência

                    bdApolo.delete_transfestqlocarmaz(item.EmpCod, item.TransfEstqLocArmazNum, "RIOSOFT");

                    #endregion
                }

                #endregion

                #region Deleta as Transferências de Comércio

                var listaTransferenciaComercio = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                    .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
                        && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
                        && i.ItTransfEstqLocArmazEntrada == "01.08"))
                    .OrderByDescending(r => r.TransfEstqLocArmazData)
                    .ToList();

                foreach (var item in listaTransferenciaComercio)
                {
                    string numTransf = item.TransfEstqLocArmazNum.ToString();
                    //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                    #region Deleta a Saída

                    MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (saida != null)
                    {
                        DeletaMovEstq(saida);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta a Entrada

                    MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (entrada != null)
                    {
                        DeletaMovEstq(entrada);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta Transferência

                    bdApolo.delete_transfestqlocarmaz(item.EmpCod, item.TransfEstqLocArmazNum, "RIOSOFT");

                    #endregion
                }

                #endregion

                #region Deleta as Transferências das Granjas

                var listaTransferenciaGranjas = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                    .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
                        && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
                        && (i.ItTransfEstqLocArmazEntrada == "05.05" || i.ItTransfEstqLocArmazEntrada == "01.07")))
                    .OrderByDescending(r => r.TransfEstqLocArmazData)
                    .ToList();

                foreach (var item in listaTransferenciaGranjas)
                {
                    string numTransf = item.TransfEstqLocArmazNum.ToString();
                    //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                    #region Deleta a Saída

                    MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (saida != null)
                    {
                        DeletaMovEstq(saida);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta a Entrada

                    MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                    if (entrada != null)
                    {
                        DeletaMovEstq(entrada);
                        bdApolo.SaveChanges();
                    }

                    #endregion

                    #region Deleta Transferência

                    bdApolo.delete_transfestqlocarmaz(item.EmpCod, item.TransfEstqLocArmazNum, "RIOSOFT");

                    #endregion
                }

                #endregion

                #region Deleta Movs. Pernetas

                var listaMovTransfPerneta = bdApolo.MOV_ESTQ
                    .Where(m => m.EmpCod == "1" && m.MovEstqDocEspec == "TLA")
                    .OrderByDescending(o => o.MovEstqDataMovimento)
                    .ToList();

                foreach (var item in listaMovTransfPerneta)
                {
                    DeletaMovEstq(item);
                    bdApolo.SaveChanges();
                }

                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(movestqchv.ToString() + " / " + e.Message);
            }
        }

        public List<ImportaDiarioExpedicao> CarregarItensDEOImport(DateTime dataFiltro, string granja,
            string tipoDEO)
        {
            DateTime data = Convert.ToDateTime(dataFiltro.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture));
            DateTime data01 = data.AddMinutes(1);

            return bdSQLServer.ImportaDiarioExpedicao
                .Where(d => d.DataHoraCarreg == data && d.Granja == granja && d.TipoDEO == tipoDEO)
                .OrderBy(o => o.Linhagem)
                .ToList();
        }

        public List<LayoutDiarioExpedicaos> CarregarItensDEO(DateTime dataFiltro, string granja,
            string tipoDEO)
        {
            DateTime data = Convert.ToDateTime(dataFiltro.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture));
            DateTime data01 = data.AddMinutes(1);

            return bdSQLServer.LayoutDiarioExpedicaos
                //.Where(d => d.DataHoraCarreg >= data && d.DataHoraCarreg <= data01 && d.Granja == granja)
                .Where(d => d.DataHoraCarreg == dataFiltro && d.Granja == granja && d.TipoDEO == tipoDEO)
                //.OrderBy(o => o.Linhagem)
                .OrderBy(o => o.ID)
                .ToList();
        }

        public void ImportaDEOApolo(string granja, DateTime dataHoraCarreg, string tipoDEO)
        {
            try
            {
                var lista = CarregarItensDEOImport(dataHoraCarreg, granja, tipoDEO);

                ImportaDiarioExpedicao importaDEO = lista.Where(l => l.NFNum != "").FirstOrDefault();

                if (importaDEO == null)
                {
                    importaDEO = lista.FirstOrDefault();
                }

                #region Ajusta DEO c/ Incubatórios Incompletos

                var listaDEO = CarregarItensDEO(dataHoraCarreg, granja, tipoDEO);
                string incubatorioMaior = listaDEO.Max(m => m.Incubatorio);
                if (incubatorioMaior != null && incubatorioMaior != "")
                {
                    string numIdentificacao = listaDEO.Max(m => m.NumIdentificacao);
                    foreach (var item in listaDEO)
                    {
                        item.Incubatorio = incubatorioMaior;
                        item.NumIdentificacao = numIdentificacao;
                    }

                    bdSQLServer.SaveChanges();

                    listaDEO = CarregarItensDEO(dataHoraCarreg, granja, tipoDEO);
                }

                #endregion

                #region Ajusta tabela ImportaDEO

                var listaDeleta = CarregarItensDEOImport(dataHoraCarreg, granja, tipoDEO);

                foreach (var item in listaDeleta)
                {
                    bdSQLServer.ImportaDiarioExpedicao.DeleteObject(item);
                }

                bdSQLServer.SaveChanges();

                var listaDEOpImport = listaDEO
                    .GroupBy(g => new { g.LoteCompleto, g.DataProducao, g.TipoOvo })
                    .OrderBy(o => o.Key.LoteCompleto).ThenBy(t => t.Key.DataProducao)
                    .ToList();

                foreach (var item in listaDEOpImport)
                {
                    var listaDEOItens = listaDEO.Where(w => w.LoteCompleto == item.Key.LoteCompleto
                        && w.DataProducao == item.Key.DataProducao
                        && w.TipoOvo == item.Key.TipoOvo).ToList();

                    #region Gera Código Relacionamento

                    ImportaDiarioExpedicao novoImporta = new ImportaDiarioExpedicao();

                    System.Data.Objects.ObjectParameter numeroIDEO =
                        new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                    bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numeroIDEO);

                    novoImporta.CodItemImportaDEO = Convert.ToInt32(numeroIDEO.Value);

                    #endregion

                    decimal qtdOvos = 0;

                    foreach (var itemLoteData in listaDEOItens)
                    {
                        qtdOvos = qtdOvos + itemLoteData.QtdeOvos;

                        #region Ajusta Linhagem

                        FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
                        FLIPDataSet.FLOCKSDataTable fDT = new FLIPDataSet.FLOCKSDataTable();
                        fTA.FillByFlockID(fDT, itemLoteData.LoteCompleto);

                        if (fDT.Count > 0)
                        {
                            FLIPDataSet.FLOCKSRow fRow = fDT.FirstOrDefault();
                            if (itemLoteData.Linhagem != fRow.VARIETY)
                                itemLoteData.Linhagem = fRow.VARIETY;
                        }

                        #endregion

                        #region Insere Relacionamento

                        LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                        deoXimporta.CodItemDEO = Convert.ToInt32(itemLoteData.CodItemDEO);
                        deoXimporta.CodItemImportaDEO = (int)novoImporta.CodItemImportaDEO;

                        bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                        #endregion
                    }

                    #region Insere ImportaDiarioExpedicao

                    LayoutDiarioExpedicaos primeiro = listaDEOItens.FirstOrDefault();

                    novoImporta.Nucleo = primeiro.Nucleo;
                    novoImporta.Galpao = primeiro.Galpao;
                    novoImporta.Lacre = primeiro.Lote;
                    novoImporta.Idade = primeiro.Idade;
                    novoImporta.Linhagem = primeiro.Linhagem;
                    novoImporta.Lote = primeiro.Lote;
                    novoImporta.LoteCompleto = primeiro.LoteCompleto;
                    novoImporta.DataProducao = primeiro.DataProducao;
                    novoImporta.NumeroReferencia = primeiro.NumeroReferencia;
                    novoImporta.QtdeOvos = qtdOvos;
                    novoImporta.QtdeBandejas = novoImporta.QtdeOvos / 150;
                    novoImporta.Usuario = primeiro.Usuario;
                    novoImporta.DataHora = DateTime.Now;
                    novoImporta.DataHoraCarreg = primeiro.DataHoraCarreg;
                    novoImporta.DataHoraRecebInc = primeiro.DataHoraRecebInc;
                    novoImporta.ResponsavelCarreg = primeiro.ResponsavelCarreg;
                    novoImporta.ResponsavelReceb = primeiro.ResponsavelReceb;
                    novoImporta.NFNum = primeiro.NFNum;
                    novoImporta.Granja = primeiro.Granja;
                    novoImporta.Importado = primeiro.Importado;
                    novoImporta.Incubatorio = primeiro.Incubatorio;
                    novoImporta.TipoDEO = primeiro.TipoDEO;
                    novoImporta.GTANum = primeiro.GTANum;
                    novoImporta.Lacre = primeiro.Lacre;
                    novoImporta.NumIdentificacao = primeiro.NumIdentificacao;
                    novoImporta.TipoOvo = primeiro.TipoOvo;

                    bdSQLServer.ImportaDiarioExpedicao.AddObject(novoImporta);

                    #endregion
                }

                bdSQLServer.SaveChanges();

                lista = CarregarItensDEOImport(dataHoraCarreg, granja, tipoDEO);

                importaDEO = lista.Where(l => l.NFNum != "").FirstOrDefault();

                if (importaDEO == null)
                {
                    importaDEO = lista.FirstOrDefault();
                }

                #endregion

                string empresaApolo = "1";
                if (granja.Equals("PL") && lista.Where(w => w.Linhagem.Contains("DK")).Count() > 0)
                    empresaApolo = "20";

                #region Deleta Apolo

                if (!importaDEO.NumIdentificacao.Equals(""))
                {
                    int numIdent = Convert.ToInt32(importaDEO.NumIdentificacao);

                    var listaTransferenciaAjapi = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                        .Where(t => t.TransfEstqLocArmazNum == numIdent)
                        .OrderByDescending(r => r.TransfEstqLocArmazData)
                        .ToList();

                    foreach (var item in listaTransferenciaAjapi)
                    {
                        string numTransf = item.TransfEstqLocArmazNum.ToString();
                        //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                        #region Deleta a Saída

                        MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == empresaApolo
                            && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                        if (saida != null)
                        {
                            DeletaMovEstq(saida);
                            bdApolo.SaveChanges();
                        }

                        #endregion

                        #region Deleta a Entrada

                        MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == empresaApolo
                            && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                        if (entrada != null)
                        {
                            DeletaMovEstq(entrada);
                            bdApolo.SaveChanges();
                        }

                        #endregion

                        #region Deleta Transferência

                        bdApolo.delete_transfestqlocarmaz(item.EmpCod, item.TransfEstqLocArmazNum, "RIOSOFT");

                        #endregion
                    }
                }

                #endregion

                var listaClassOvo = lista
                    .GroupBy(g => g.TipoOvo)
                    .OrderBy(o => o.Key)
                    .ToList();

                foreach (var tipoOvo in listaClassOvo)
                {
                    #region Carrega variáveis e objetos

                    var listaImportTipoOvo = lista
                        .Where(w => w.TipoOvo == tipoOvo.Key)
                        .OrderBy(o => o.Linhagem).ThenBy(o => o.LoteCompleto)
                        .ThenBy(t => t.DataProducao)
                        .ToList();

                    string linhagemAnterior = "";
                    MOV_ESTQ movEstq = new MOV_ESTQ();
                    ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
                    LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = new LOC_ARMAZ_ITEM_MOV_ESTQ();
                    CTRL_LOTE_ITEM_MOV_ESTQ lote = new CTRL_LOTE_ITEM_MOV_ESTQ();

                    TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();
                    ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();
                    decimal qtdTotalItem = 0;

                    DateTime dataAnterior = Convert.ToDateTime("01/01/2014");

                    LOC_ARMAZ localArmazCadastro = new LOC_ARMAZ();

                    EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL.Where(e => e.USERFLIPCod == "CH")
                        .FirstOrDefault();

                    string incubLocArmaz = "";
                    string incubSaida = "";
                    if (granja == "PL")
                    {
                        incubSaida = "NM";
                        if (importaDEO.TipoDEO.Equals("Transf. Ovos Incubáveis"))
                            incubLocArmaz = tipoOvo.Key;
                        else
                            incubLocArmaz = incubSaida;

                        if (lista.Where(w => w.Linhagem.Contains("DKB")).Count() > 0)
                            empresa = bdApolo.EMPRESA_FILIAL.Where(w => w.USERFLIPCod == granja)
                                .FirstOrDefault();
                    }
                    else
                    {
                        incubSaida = granja;
                        incubLocArmaz = importaDEO.Incubatorio;
                    }

                    if (importaDEO.TipoDEO.Equals("Transf. Ovos Incubáveis"))
                        localArmazCadastro = bdApolo.LOC_ARMAZ
                                .Where(l => l.USERCodigoFLIP == incubLocArmaz && l.USERTipoProduto == "Ovos Incubáveis")
                                .FirstOrDefault();
                    else
                        localArmazCadastro = bdApolo.LOC_ARMAZ
                                .Where(l => l.USERCodigoFLIP == incubLocArmaz && l.USERTipoProduto == importaDEO.TipoDEO)
                                .FirstOrDefault();

                    LOC_ARMAZ localArmazSaida = bdApolo.LOC_ARMAZ
                            .Where(l => l.USERCodigoFLIP == granja && l.USERTipoProduto == "Ovos Incubáveis")
                            .FirstOrDefault();

                    string tipoLanc = "";
                    if (importaDEO.TipoDEO.Equals("Transf. Ovos Incubáveis"))
                        if (granja == "PL")
                            tipoLanc = "E0000553";
                        else
                            tipoLanc = "E0000508";
                    else if (importaDEO.TipoDEO.Equals("Ovos Incubáveis"))
                        tipoLanc = localArmazSaida.USERTipoLancSaidaInc;
                    else
                        tipoLanc = localArmazSaida.USERTipoLancSaidaCom;

                    if (importaDEO.TipoDEO.Equals("Exportação"))
                        tipoLanc = localArmazCadastro.USERTipoLancSaidaCom;

                    string unidadeMedida = "UN";

                    DateTime dataMov = dataHoraCarreg;
                    string usuario = "RIOSOFT";

                    #endregion

                    #region Insere Nova Transferência

                    transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();

                    ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                    bdApolo.gerar_codigo("1", "TRANSF_ESTQ_LOC_ARMAZ", numero);

                    transfEstqLocArmaz.EmpCod = empresa.EmpCod;
                    transfEstqLocArmaz.TipoLancCod = tipoLanc;
                    transfEstqLocArmaz.TransfEstqLocArmazData = Convert.ToDateTime(dataMov.ToShortDateString());
                    transfEstqLocArmaz.TransfEstqLocArmazNum = Convert.ToInt32(numero.Value);
                    if (importaDEO.TipoDEO.Equals("Transf. Ovos Incubáveis"))
                        transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis do Incubatório de Nova Granada p/ Incubatório de Ajapi.";
                    else if (importaDEO.TipoDEO.Equals("Exportação"))
                        transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis para Exportação.";
                    else if (importaDEO.TipoDEO.Equals("Ovos p/ Comércio"))
                        transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis p/ Comercial.";
                    else
                        transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis da Granja p/ Incubatório.";

                    bdApolo.TRANSF_ESTQ_LOC_ARMAZ.AddObject(transfEstqLocArmaz);

                    #endregion

                    short ultimaSequencia = 0;

                    foreach (var item in listaImportTipoOvo)
                    {
                        string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                        #region Se mudou a linhagem da lista, insere um note Item

                        if (linhagemAnterior != item.Linhagem)
                        {
                            PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem)
                                .FirstOrDefault();

                            if (!linhagemAnterior.Equals(""))
                            {
                                itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                                qtdTotalItem = 0;

                                ultimaSequencia++;

                                bdApolo.SaveChanges();
                            }
                            else
                            {
                                ultimaSequencia = 1;
                            }

                            itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();

                            itemTransfEstqLocArmaz.EmpCod = transfEstqLocArmaz.EmpCod;
                            itemTransfEstqLocArmaz.TransfEstqLocArmazNum = transfEstqLocArmaz.TransfEstqLocArmazNum;
                            itemTransfEstqLocArmaz.ProdCodEstr = produto.ProdCodEstr;
                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq = ultimaSequencia;
                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida = localArmazSaida.LocArmazCodEstr;
                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada = localArmazCadastro.LocArmazCodEstr;
                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazObs = transfEstqLocArmaz.TransfEstqLocArmazObs;

                            bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.AddObject(itemTransfEstqLocArmaz);
                        }

                        #endregion

                        #region Insere o Lote

                        PROD_UNID_MED prodUnidMed = bdApolo.PROD_UNID_MED
                            .Where(p => p.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                && p.ProdUnidMedCod == unidadeMedida)
                            .FirstOrDefault();

                        IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE itemTransfEstqLocArmazLote = new IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE();

                        itemTransfEstqLocArmazLote.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                        itemTransfEstqLocArmazLote.TransfEstqLocArmazNum = itemTransfEstqLocArmaz.TransfEstqLocArmazNum;
                        itemTransfEstqLocArmazLote.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSeq = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq;
                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSaida = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida;
                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazEntrada = itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada;
                        itemTransfEstqLocArmazLote.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                        itemTransfEstqLocArmazLote.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd = item.QtdeOvos;
                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmLoteQtdCalc = itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd;
                        itemTransfEstqLocArmazLote.CtrlLoteNum = item.LoteCompleto;
                        itemTransfEstqLocArmazLote.CtrlLoteDataValid = item.DataProducao;

                        bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.AddObject(itemTransfEstqLocArmazLote);

                        #endregion

                        qtdTotalItem = qtdTotalItem + item.QtdeOvos;

                        #region Caso seja o último lote da linhagem, adiciona o total no item e salva

                        if (lista.IndexOf(item) == (lista.Count - 1))
                        {
                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                            qtdTotalItem = 0;

                            bdApolo.SaveChanges();
                        }

                        #endregion

                        #region Salva informações do DEO

                        var listaOriginal = bdSQLServer.LayoutDiarioExpedicaos
                            .Where(d => bdSQLServer.LayoutDEO_X_ImportaDEO.Any(l => l.CodItemDEO == d.CodItemDEO
                                && l.CodItemImportaDEO == item.CodItemImportaDEO))
                            .ToList();

                        foreach (var item2 in listaOriginal)
                        {
                            item2.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();
                            item2.Importado = "Conferido";
                            item2.NFNum = importaDEO.NFNum;
                            item2.GTANum = importaDEO.GTANum;
                            item2.Lacre = importaDEO.Lacre;
                        }

                        item.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();
                        item.Importado = "Conferido";
                        item.NFNum = importaDEO.NFNum;
                        item.GTANum = importaDEO.GTANum;
                        item.Lacre = importaDEO.Lacre;

                        #endregion

                        linhagemAnterior = item.Linhagem;
                        dataAnterior = item.DataHoraCarreg;
                    }

                    bdApolo.SaveChanges();

                    #region Integra a transferência com o Estoque

                    bdApolo.transflocarmaz_gera_movestq(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                        usuario);

                    bdSQLServer.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpo = "Erro ao Ajustar Diário da Granja Planalto: "
                    + (char)10 + (char)13 + linenum + (char)10 + (char)13
                    + ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message != null)
                    {
                        corpo = (char)10 + (char)13 + corpo + ex.InnerException.Message;
                        if (!ex.InnerException.Message.Substring(0, 16).Equals("Timeout expirado."))
                            EnviarEmail(corpo, "**** ERRO SERVICO ATUALIZAR DIARIO DA GRANJA DA PLANALTO****",
                                "Paulo Alves", "palves@hyline.com.br", "", "");
                    }
                }
                else
                {
                    EnviarEmail(corpo, "**** ERRO SERVICO ATUALIZAR DIARIO DA GRANJA DA PLANALTO****", "Paulo Alves",
                        "palves@hyline.com.br", "", "");
                }
            }
        }

        public void ImportaDEOApoloItem(ImportaDiarioExpedicao deoImportacao)
        {
            //int numTELA = Convert.ToInt32(deoImportacao.NumIdentificacao);

            //PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == deoImportacao.Linhagem).FirstOrDefault();

            //#region Ajusta Qtde. Transferencia Loc. Armaz

            //TRANSF_ESTQ_LOC_ARMAZ transfEstqLocaArmaz = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
            //    .Where(t => t.EmpCod == "1" && t.TransfEstqLocArmazNum == numTELA)
            //    .FirstOrDefault();

            //ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTrasnfEstqLocArmaz = bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ
            //    .Where(i => i.EmpCod == transfEstqLocaArmaz.EmpCod && i.TransfEstqLocArmazNum == transfEstqLocaArmaz.TransfEstqLocArmazNum
            //        && i.ProdCodEstr == produto.ProdCodEstr)
            //    .FirstOrDefault();

            //IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE loteTELA = bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE
            //    .Where(c => c.EmpCod == itemTrasnfEstqLocArmaz.EmpCod
            //        && c.TransfEstqLocArmazNum == itemTrasnfEstqLocArmaz.TransfEstqLocArmazNum
            //        && c.ProdCodEstr == itemTrasnfEstqLocArmaz.ProdCodEstr
            //        && c.ItTransfEstqLocArmazSeq == itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazSeq
            //        && c.CtrlLoteNum == deoImportacao.LoteCompleto && c.CtrlLoteDataValid == deoImportacao.DataProducao)
            //    .FirstOrDefault();

            //if (loteTELA != null)
            //{
            //    loteTELA.ItTransfEstqLocArmazLoteQtd = loteTELA.ItTransfEstqLocArmazLoteQtd + deoImportacao.QtdeOvos;
            //    loteTELA.ItTransfEstqLocArmLoteQtdCalc = loteTELA.ItTransfEstqLocArmazLoteQtd;
            //}
            //else
            //{
            //    loteTELA = new IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE();

            //    loteTELA.EmpCod = itemTrasnfEstqLocArmaz.EmpCod;
            //    loteTELA.TransfEstqLocArmazNum = itemTrasnfEstqLocArmaz.TransfEstqLocArmazNum;
            //    loteTELA.ProdCodEstr = itemTrasnfEstqLocArmaz.ProdCodEstr;
            //    loteTELA.ItTransfEstqLocArmazSeq = itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazSeq;
            //    loteTELA.ItTransfEstqLocArmazSaida = itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazSaida;
            //    loteTELA.ItTransfEstqLocArmazEntrada = itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazEntrada;
            //    loteTELA.ProdUnidMedCod = "UN";
            //    loteTELA.ProdUnidMedPos = 1;
            //    loteTELA.ItTransfEstqLocArmazLoteQtd = deoImportacao.QtdeOvos;
            //    loteTELA.ItTransfEstqLocArmLoteQtdCalc = loteTELA.ItTransfEstqLocArmazLoteQtd;
            //    loteTELA.CtrlLoteNum = deoImportacao.LoteCompleto;
            //    loteTELA.CtrlLoteDataValid = deoImportacao.DataProducao;

            //    bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.AddObject(loteTELA);
            //}

            //itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazQtd = itemTrasnfEstqLocArmaz.ItTransfEstqLocArmazQtd + loteTELA.ItTransfEstqLocArmazLoteQtd;

            //#endregion

            //#region Ajusta as Movimentações de Transferência

            //#region Ajusta a Saída

            //MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
            //        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == deoImportacao.NumIdentificacao).FirstOrDefault();

            //ITEM_MOV_ESTQ saidaItem = bdApolo.ITEM_MOV_ESTQ
            //    .Where(i => i.EmpCod == saida.EmpCod && i.MovEstqChv == saida.MovEstqChv && i.ProdCodEstr == produto.ProdCodEstr)
            //    .FirstOrDefault();

            //LOC_ARMAZ_ITEM_MOV_ESTQ saidaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //    .Where(c => c.EmpCod == saidaItem.EmpCod && c.MovEstqChv == saidaItem.MovEstqChv && c.ProdCodEstr == saidaItem.ProdCodEstr
            //        && c.ItMovEstqSeq == saidaItem.ItMovEstqSeq)
            //    .FirstOrDefault();

            //CTRL_LOTE_ITEM_MOV_ESTQ saidaLote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //    .Where(c => c.EmpCod == saidaItem.EmpCod && c.MovEstqChv == saidaItem.MovEstqChv && c.ProdCodEstr == saidaItem.ProdCodEstr
            //        && c.ItMovEstqSeq == saidaItem.ItMovEstqSeq && c.CtrlLoteNum == deoImportacao.LoteCompleto
            //        && c.CtrlLoteDataValid == deoImportacao.DataProducao)
            //    .FirstOrDefault();

            //if (saidaLote == null)
            //{
            //    saidaLote = InsereLote(saidaItem.MovEstqChv, saidaItem.EmpCod, saidaItem.TipoLancCod,
            //        saidaItem.ItMovEstqSeq, saidaItem.ProdCodEstr, deoImportacao.LoteCompleto, deoImportacao.DataProducao,
            //        deoImportacao.QtdeOvos, "Saída", saidaItem.ItMovEstqUnidMedCod, saidaItem.ItMovEstqUnidMedPos, saidaLocal.LocArmazCodEstr);

            //    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(saidaLote);
            //}
            //else
            //{
            //    saidaLote.CtrlLoteItMovEstqQtd = saidaLote.CtrlLoteItMovEstqQtd + deoImportacao.QtdeOvos;
            //    saidaLote.CtrlLoteItMovEstqQtdCalc = saidaLote.CtrlLoteItMovEstqQtd;
            //}

            //saidaItem.ItMovEstqQtdProd = saidaItem.ItMovEstqQtdProd + saidaLote.CtrlLoteItMovEstqQtd;
            //saidaItem.ItMovEstqQtdCalcProd = saidaItem.ItMovEstqQtdProd;

            //saidaLocal.LocArmazItMovEstqQtd = saidaLocal.LocArmazItMovEstqQtd + saidaLote.CtrlLoteItMovEstqQtd;
            //saidaLocal.LocArmazItMovEstqQtdCalc = saidaLocal.LocArmazItMovEstqQtd;

            //bdApolo.SaveChanges();

            ////bdApolo.atualiza_saldoestqdata(saidaItem.EmpCod, saidaItem.MovEstqChv, saidaItem.ProdCodEstr, saidaItem.ItMovEstqSeq,
            //    //saidaItem.ItMovEstqDataMovimento, "UPD");

            //#endregion

            //#region Ajusta a Entrada

            //MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
            //    && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == deoImportacao.NumIdentificacao).FirstOrDefault();

            //ITEM_MOV_ESTQ entradaItem = bdApolo.ITEM_MOV_ESTQ
            //    .Where(i => i.EmpCod == entrada.EmpCod && i.MovEstqChv == entrada.MovEstqChv && i.ProdCodEstr == produto.ProdCodEstr)
            //    .FirstOrDefault();

            //LOC_ARMAZ_ITEM_MOV_ESTQ entradaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //    .Where(c => c.EmpCod == entradaItem.EmpCod && c.MovEstqChv == entradaItem.MovEstqChv && c.ProdCodEstr == entradaItem.ProdCodEstr
            //        && c.ItMovEstqSeq == entradaItem.ItMovEstqSeq)
            //    .FirstOrDefault();

            //CTRL_LOTE_ITEM_MOV_ESTQ entradaLote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //        .Where(c => c.EmpCod == entradaItem.EmpCod && c.MovEstqChv == entradaItem.MovEstqChv && c.ProdCodEstr == entradaItem.ProdCodEstr
            //            && c.ItMovEstqSeq == entradaItem.ItMovEstqSeq && c.CtrlLoteNum == deoImportacao.LoteCompleto
            //            && c.CtrlLoteDataValid == deoImportacao.DataProducao)
            //        .FirstOrDefault();

            //if (entradaLote == null)
            //{
            //    entradaLote = InsereLote(entradaItem.MovEstqChv, entradaItem.EmpCod, entradaItem.TipoLancCod,
            //        entradaItem.ItMovEstqSeq, entradaItem.ProdCodEstr, deoImportacao.LoteCompleto, deoImportacao.DataProducao,
            //        deoImportacao.QtdeOvos, "Entrada", entradaItem.ItMovEstqUnidMedCod, entradaItem.ItMovEstqUnidMedPos,
            //        entradaLocal.LocArmazCodEstr);

            //    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(entradaLote);
            //}
            //else
            //{
            //    entradaLote.CtrlLoteItMovEstqQtd = entradaLote.CtrlLoteItMovEstqQtd + deoImportacao.QtdeOvos;
            //    entradaLote.CtrlLoteItMovEstqQtdCalc = entradaLote.CtrlLoteItMovEstqQtd;
            //}

            //entradaItem.ItMovEstqQtdProd = entradaItem.ItMovEstqQtdProd + entradaLote.CtrlLoteItMovEstqQtd;
            //entradaItem.ItMovEstqQtdCalcProd = entradaItem.ItMovEstqQtdProd;

            //entradaLocal.LocArmazItMovEstqQtd = entradaLocal.LocArmazItMovEstqQtd + entradaLote.CtrlLoteItMovEstqQtd;
            //entradaLocal.LocArmazItMovEstqQtdCalc = entradaLocal.LocArmazItMovEstqQtd;

            //bdApolo.SaveChanges();

            ////bdApolo.atualiza_saldoestqdata(entradaItem.EmpCod, entradaItem.MovEstqChv, entradaItem.ProdCodEstr, entradaItem.ItMovEstqSeq,
            //    //entradaItem.ItMovEstqDataMovimento, "UPD");

            //#endregion

            //#endregion
        }

        public void ImportaIncubacaoApolo(string hatchLoc, DateTime setDate)
        {
            #region Importa p/ Apolo

            #region Carrega variáveis e objetos

            DateTime dataIncubacao = setDate;
            string incubatorio = hatchLoc;

            string naturezaOperacao = "5.101";
            decimal valorUnitario = 0.25m;
            string unidadeMedida = "UN";
            short? posicaoUnidadeMedida = 1;
            string tribCod = "040";
            string itMovEstqClasFiscCodNbm = "04079000";
            string clasFiscCod = "0000129";
            string operacao = "Saída";

            ITEM_MOV_ESTQ itemMovEstq = null;

            string usuario = "RIOSOFT";
            
            EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
                .Where(ef => ef.USERFLIPCod == "CH")
                .FirstOrDefault();

            LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
                .Where(l => l.USERCodigoFLIP == incubatorio && l.USERTipoProduto == "Ovos Incubáveis")
                .FirstOrDefault();

            var listaIncubacao = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Hatch_loc == incubatorio && h.Set_date == dataIncubacao && h.ImportadoApolo == "Sim")
                .ToList();

            #endregion

            foreach (var item in listaIncubacao)
            {
                PRODUTO produto = produto = bdApolo.PRODUTO
                    .Where(p => p.ProdNomeAlt1 == item.Variety)
                    .FirstOrDefault();

                int tamanho = item.Flock_id.Length;
                tamanho = tamanho - 6;
                string flockID = item.Flock_id.Substring(6, tamanho);

                #region Insere Saida p/ Incubação

                // Verifica se Existe a movimentação neste Incubatório e Produto
                LOC_ARMAZ_ITEM_MOV_ESTQ locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                    .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                        && i.ProdCodEstr == produto.ProdCodEstr
                        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
                            && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
                                .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
                    .FirstOrDefault();

                if (locItemMovEstq != null)
                {
                    itemMovEstq = bdApolo.ITEM_MOV_ESTQ
                        .Where(im => im.EmpCod == locItemMovEstq.EmpCod && im.MovEstqChv == locItemMovEstq.MovEstqChv
                            && im.ProdCodEstr == locItemMovEstq.ProdCodEstr && im.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq)
                        .FirstOrDefault();

                    itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + Convert.ToDecimal(item.Eggs_rcvd);
                    itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                    locItemMovEstq.LocArmazItMovEstqQtd = locItemMovEstq.LocArmazItMovEstqQtd + Convert.ToDecimal(item.Eggs_rcvd);
                    locItemMovEstq.LocArmazItMovEstqQtdCalc = locItemMovEstq.LocArmazItMovEstqQtd;

                    CTRL_LOTE_ITEM_MOV_ESTQ lote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                        .Where(c => c.EmpCod == locItemMovEstq.EmpCod && c.MovEstqChv == locItemMovEstq.MovEstqChv
                            && c.ProdCodEstr == locItemMovEstq.ProdCodEstr && c.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq
                            && c.LocArmazCodEstr == locItemMovEstq.LocArmazCodEstr && c.CtrlLoteNum == flockID
                            && c.CtrlLoteDataValid == item.Lay_date)
                        .FirstOrDefault();

                    // Verifica se Existe o lote
                    if (lote != null)
                    {
                        lote.CtrlLoteItMovEstqQtd = lote.CtrlLoteItMovEstqQtd + Convert.ToDecimal(item.Eggs_rcvd);
                        lote.CtrlLoteItMovEstqQtdCalc = lote.CtrlLoteItMovEstqQtd;
                    }
                    else
                    {
                        lote = InsereLote(locItemMovEstq.MovEstqChv, locItemMovEstq.EmpCod, itemMovEstq.TipoLancCod,
                            locItemMovEstq.ItMovEstqSeq, locItemMovEstq.ProdCodEstr, flockID, item.Lay_date, Convert.ToDecimal(item.Eggs_rcvd), operacao,
                            itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos, locItemMovEstq.LocArmazCodEstr);

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                    }
                }
                else
                {
                    // Verifica se Existe a movimentação neste Incubatório e não no Produto
                    locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                        .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                            && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
                                && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
                                .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
                        .FirstOrDefault();

                    if (locItemMovEstq != null)
                    {
                        MOV_ESTQ movEstq = bdApolo.MOV_ESTQ
                            .Where(m => m.EmpCod == locItemMovEstq.EmpCod && m.MovEstqChv == locItemMovEstq.MovEstqChv)
                            .FirstOrDefault();

                        itemMovEstq = InsereItemMovEstq(movEstq.MovEstqChv,
                            movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
                            item.Variety, naturezaOperacao, Convert.ToDecimal(item.Eggs_rcvd), valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                            tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
                            InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
                            itemMovEstq.ProdCodEstr, Convert.ToDecimal(item.Eggs_rcvd), Convert.ToDecimal(item.Eggs_rcvd), locItemMovEstq.LocArmazCodEstr);

                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

                        CTRL_LOTE_ITEM_MOV_ESTQ lote = InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
                            itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
                            item.Lay_date, Convert.ToDecimal(item.Eggs_rcvd), operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
                            locArmazItemMovEstq.LocArmazCodEstr);

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                    }
                    else
                    {
                        MOV_ESTQ movEstq = InsereMovEstq(empresa.EmpCod, locArmaz.USERTipoLancSaidaInc, empresa.EntCod,
                            dataIncubacao, usuario);

                        bdApolo.MOV_ESTQ.AddObject(movEstq);

                        itemMovEstq = InsereItemMovEstq(movEstq.MovEstqChv,
                            movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
                            item.Variety, naturezaOperacao, Convert.ToDecimal(item.Eggs_rcvd), valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                            tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
                            InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
                            itemMovEstq.ProdCodEstr, Convert.ToDecimal(item.Eggs_rcvd), Convert.ToDecimal(item.Eggs_rcvd), locArmaz.LocArmazCodEstr);

                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

                        CTRL_LOTE_ITEM_MOV_ESTQ lote = InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
                            itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
                            item.Lay_date, Convert.ToDecimal(item.Eggs_rcvd), operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
                            locArmazItemMovEstq.LocArmazCodEstr);

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                    }
                }

                #endregion

                bdApolo.SaveChanges();
            }

            bdApolo.SaveChanges();

            if (itemMovEstq != null)
            {
                //var listaItensMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == itemMovEstq.EmpCod
                //&& i.MovEstqChv == itemMovEstq.MovEstqChv).ToList();

                //foreach (var item in listaItensMovEstq)
                //{
                //    bdApolo.atualiza_saldoestqdata(item.EmpCod, item.MovEstqChv, item.ProdCodEstr,
                //        item.ItMovEstqSeq, item.ItMovEstqDataMovimento, "INS");
                //}

                bdApolo.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);
            }

            bdSQLServer.SaveChanges();

            #endregion
        }

        public void ImportaIncubacaoApoloItem(string hatchLoc, DateTime setDate, string loteNum, 
            DateTime dataProducao)
        {
            #region Importa p/ Apolo

            #region Carrega variáveis e objetos

            DateTime dataIncubacao = setDate;
            string incubatorio = hatchLoc;

            string naturezaOperacao = "5.101";
            decimal valorUnitario = 0.25m;
            string unidadeMedida = "UN";
            short? posicaoUnidadeMedida = 1;
            string tribCod = "040";
            string itMovEstqClasFiscCodNbm = "04079000";
            string clasFiscCod = "0000129";
            string operacao = "Saída";

            ITEM_MOV_ESTQ itemMovEstq = null;

            string usuario = "RIOSOFT";

            EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
                .Where(ef => ef.USERFLIPCod == "CH")
                .FirstOrDefault();

            LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
                .Where(l => l.USERCodigoFLIP == incubatorio && l.USERTipoProduto == "Ovos Incubáveis")
                .FirstOrDefault();

            var listaIncubacao = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Hatch_loc == incubatorio && h.Set_date == dataIncubacao && h.ImportadoApolo == "Sim"
                    && h.Flock_id.Contains(loteNum) && h.Lay_date == dataProducao)
                .ToList();

            #endregion

            foreach (var item in listaIncubacao)
            {
                PRODUTO produto = produto = bdApolo.PRODUTO
                    .Where(p => p.ProdNomeAlt1 == item.Variety)
                    .FirstOrDefault();

                int tamanho = item.Flock_id.Length;
                tamanho = tamanho - 6;
                string flockID = item.Flock_id.Substring(6, tamanho);

                #region Insere Saida p/ Incubação

                // Verifica se Existe a movimentação neste Incubatório e Produto
                LOC_ARMAZ_ITEM_MOV_ESTQ locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                    .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                        && i.ProdCodEstr == produto.ProdCodEstr
                        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
                            && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
                                .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
                    .FirstOrDefault();

                if (locItemMovEstq != null)
                {
                    itemMovEstq = bdApolo.ITEM_MOV_ESTQ
                        .Where(im => im.EmpCod == locItemMovEstq.EmpCod && im.MovEstqChv == locItemMovEstq.MovEstqChv
                            && im.ProdCodEstr == locItemMovEstq.ProdCodEstr && im.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq)
                        .FirstOrDefault();

                    itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + Convert.ToDecimal(item.Eggs_rcvd);
                    itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                    locItemMovEstq.LocArmazItMovEstqQtd = locItemMovEstq.LocArmazItMovEstqQtd + Convert.ToDecimal(item.Eggs_rcvd);
                    locItemMovEstq.LocArmazItMovEstqQtdCalc = locItemMovEstq.LocArmazItMovEstqQtd;

                    CTRL_LOTE_ITEM_MOV_ESTQ lote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                        .Where(c => c.EmpCod == locItemMovEstq.EmpCod && c.MovEstqChv == locItemMovEstq.MovEstqChv
                            && c.ProdCodEstr == locItemMovEstq.ProdCodEstr && c.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq
                            && c.LocArmazCodEstr == locItemMovEstq.LocArmazCodEstr && c.CtrlLoteNum == flockID
                            && c.CtrlLoteDataValid == item.Lay_date)
                        .FirstOrDefault();

                    // Verifica se Existe o lote
                    if (lote != null)
                    {
                        lote.CtrlLoteItMovEstqQtd = lote.CtrlLoteItMovEstqQtd + Convert.ToDecimal(item.Eggs_rcvd);
                        lote.CtrlLoteItMovEstqQtdCalc = lote.CtrlLoteItMovEstqQtd;
                    }
                    else
                    {
                        lote = InsereLote(locItemMovEstq.MovEstqChv, locItemMovEstq.EmpCod, itemMovEstq.TipoLancCod,
                            locItemMovEstq.ItMovEstqSeq, locItemMovEstq.ProdCodEstr, flockID, item.Lay_date, Convert.ToDecimal(item.Eggs_rcvd), operacao,
                            itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos, locItemMovEstq.LocArmazCodEstr);

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                    }
                }
                else
                {
                    // Verifica se Existe a movimentação neste Incubatório e não no Produto
                    locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                        .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                            && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
                                && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
                                .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
                        .FirstOrDefault();

                    if (locItemMovEstq != null)
                    {
                        MOV_ESTQ movEstq = bdApolo.MOV_ESTQ
                            .Where(m => m.EmpCod == locItemMovEstq.EmpCod && m.MovEstqChv == locItemMovEstq.MovEstqChv)
                            .FirstOrDefault();

                        itemMovEstq = InsereItemMovEstq(movEstq.MovEstqChv,
                            movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
                            item.Variety, naturezaOperacao, Convert.ToDecimal(item.Eggs_rcvd), valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                            tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
                            InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
                            itemMovEstq.ProdCodEstr, Convert.ToDecimal(item.Eggs_rcvd), Convert.ToDecimal(item.Eggs_rcvd), locItemMovEstq.LocArmazCodEstr);

                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

                        CTRL_LOTE_ITEM_MOV_ESTQ lote = InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
                            itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
                            item.Lay_date, Convert.ToDecimal(item.Eggs_rcvd), operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
                            locArmazItemMovEstq.LocArmazCodEstr);

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                    }
                    else
                    {
                        MOV_ESTQ movEstq = InsereMovEstq(empresa.EmpCod, locArmaz.USERTipoLancSaidaInc, empresa.EntCod,
                            dataIncubacao, usuario);

                        bdApolo.MOV_ESTQ.AddObject(movEstq);

                        itemMovEstq = InsereItemMovEstq(movEstq.MovEstqChv,
                            movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
                            item.Variety, naturezaOperacao, Convert.ToDecimal(item.Eggs_rcvd), valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                            tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
                            InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
                            itemMovEstq.ProdCodEstr, Convert.ToDecimal(item.Eggs_rcvd), Convert.ToDecimal(item.Eggs_rcvd), locArmaz.LocArmazCodEstr);

                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

                        CTRL_LOTE_ITEM_MOV_ESTQ lote = InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
                            itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
                            item.Lay_date, Convert.ToDecimal(item.Eggs_rcvd), operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
                            locArmazItemMovEstq.LocArmazCodEstr);

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                    }
                }

                #endregion

                bdApolo.SaveChanges();
            }

            bdApolo.SaveChanges();

            //if (itemMovEstq != null)
            //{
            //    var listaItensMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == itemMovEstq.EmpCod
            //    && i.MovEstqChv == itemMovEstq.MovEstqChv).ToList();

            //    foreach (var item in listaItensMovEstq)
            //    {
            //        bdApolo.atualiza_saldoestqdata(item.EmpCod, item.MovEstqChv, item.ProdCodEstr,
            //            item.ItMovEstqSeq, item.ItMovEstqDataMovimento, "INS");
            //    }

            //    bdApolo.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);
            //}

            bdSQLServer.SaveChanges();

            #endregion
        }

        public void AjustaTabelaImportaDEOs(string granja, DateTime dataHoraCarreg)
        {
            DateTime data = Convert.ToDateTime(dataHoraCarreg.ToShortDateString());

            LOC_ARMAZ locarmaz = bdApolo.LOC_ARMAZ.Where(l => l.USERCodigoFLIP == granja).FirstOrDefault();

            #region Deleta as Saídas por Incubação

            var listaLotesDeletaInc = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.Granja == granja && l.DataHoraCarreg == dataHoraCarreg)
                .ToList();

            foreach (var deoItem in listaLotesDeletaInc)
            {
                var listaSaidasIncubacao = bdApolo.MOV_ESTQ
                    .Where(m => (m.TipoLancCod == "E0000503" || m.TipoLancCod == "E0000482" || m.TipoLancCod == "E0000470")
                        && bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.Any(c => c.EmpCod == m.EmpCod && c.MovEstqChv == m.MovEstqChv
                            && c.CtrlLoteNum == deoItem.LoteCompleto && c.CtrlLoteDataValid == deoItem.DataProducao))
                    .ToList();

                foreach (var item in listaSaidasIncubacao)
                {
                    //bdApolo.delete_movestq(item.EmpCod, item.MovEstqChv, "RIOSOFT", rmensagem);
                    DeletaMovEstq(item);
                }
            }

            bdApolo.SaveChanges();

            #endregion

            #region Deleta as Transferências de Ajapi

            var listaTransferenciaAjapi = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
                    && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
                    && i.ItTransfEstqLocArmazEntrada == "15.01")
                    && t.TransfEstqLocArmazData == data)
                .OrderByDescending(r => r.TransfEstqLocArmazData)
                .ToList();

            foreach (var item in listaTransferenciaAjapi)
            {
                string numTransf = item.TransfEstqLocArmazNum.ToString();
                //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                #region Deleta a Saída

                MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                    && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                if (saida != null)
                {
                    DeletaMovEstq(saida);
                    bdApolo.SaveChanges();
                }

                #endregion

                #region Deleta a Entrada

                MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                    && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                if (entrada != null)
                {
                    DeletaMovEstq(entrada);
                    bdApolo.SaveChanges();
                }

                #endregion

                #region Deleta Transferência

                bdApolo.delete_transfestqlocarmaz(item.EmpCod, item.TransfEstqLocArmazNum, "RIOSOFT");

                #endregion
            }

            #endregion

            #region Deleta as Transferências das Granjas

            var listaTransferenciaGranjas = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                .Where(t => bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.Any(i => i.EmpCod == t.EmpCod
                    && i.TransfEstqLocArmazNum == t.TransfEstqLocArmazNum
                    //&& (i.ItTransfEstqLocArmazEntrada == "05.05" || i.ItTransfEstqLocArmazEntrada == "01.07"))
                    && i.ItTransfEstqLocArmazEntrada == locarmaz.LocArmazCodEstr && i.ItTransfEstqLocArmazEntrada != "15.01")
                    && t.TransfEstqLocArmazData == data)
                .OrderByDescending(r => r.TransfEstqLocArmazData)
                .ToList();

            foreach (var item in listaTransferenciaGranjas)
            {
                string numTransf = item.TransfEstqLocArmazNum.ToString();
                //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                #region Deleta a Saída

                MOV_ESTQ saida = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                    && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                if (saida != null)
                {
                    DeletaMovEstq(saida);
                    bdApolo.SaveChanges();
                }

                #endregion

                #region Deleta a Entrada

                MOV_ESTQ entrada = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == "1"
                    && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99" && m.MovEstqDocNum == numTransf).FirstOrDefault();

                if (entrada != null)
                {
                    DeletaMovEstq(entrada);
                    bdApolo.SaveChanges();
                }

                #endregion

                #region Deleta Transferência

                bdApolo.delete_transfestqlocarmaz(item.EmpCod, item.TransfEstqLocArmazNum, "RIOSOFT");

                #endregion
            }

            #endregion

            #region Ajusta a tabela de Importação do DEO p/ o Apolo

            var listaDEOs = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.DataHoraCarreg == dataHoraCarreg && l.Granja == granja)
                .OrderBy(o => o.DataHoraCarreg)
                .ToList();

            foreach (var deo in listaDEOs)
            {
                #region Exclusão dos registros p/ Importação

                var listaImportDEO = bdSQLServer.ImportaDiarioExpedicao
                    .Where(i => bdSQLServer.LayoutDEO_X_ImportaDEO.Any(x => x.CodItemImportaDEO == i.CodItemImportaDEO
                                && x.CodItemDEO == deo.CodItemDEO))
                    .ToList();

                foreach (var item in listaImportDEO)
                {
                    LayoutDEO_X_ImportaDEO relacionamento = bdSQLServer.LayoutDEO_X_ImportaDEO
                        .Where(r => r.CodItemDEO == deo.CodItemDEO
                            && r.CodItemImportaDEO == item.CodItemImportaDEO)
                        .FirstOrDefault();

                    if (relacionamento != null)
                    {
                        bdSQLServer.LayoutDEO_X_ImportaDEO.DeleteObject(relacionamento);
                        bdSQLServer.ImportaDiarioExpedicao.DeleteObject(item);
                    }
                }

                bdSQLServer.SaveChanges();

                listaImportDEO = null;
                listaImportDEO = bdSQLServer.ImportaDiarioExpedicao
                    .Where(i => i.Granja == deo.Granja && i.DataHoraCarreg == deo.DataHoraCarreg
                        && i.LoteCompleto == deo.LoteCompleto && i.DataProducao == deo.DataProducao)
                    .ToList();

                foreach (var item in listaImportDEO)
                {
                    bdSQLServer.ImportaDiarioExpedicao.DeleteObject(item);
                }

                bdSQLServer.SaveChanges();

                #endregion
            }

            foreach (var deo in listaDEOs)
            {
                #region Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                #region Localiza os Lotes

                LOC_ARMAZ localArmazenagem = bdApolo.LOC_ARMAZ
                    .Where(l => l.USERCodigoFLIP == deo.Granja && l.USERTipoProduto == "Ovos Incubáveis")
                    .FirstOrDefault();

                int existe = bdApolo.CTRL_LOTE_LOC_ARMAZ
                    .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                        && c.CtrlLoteDataValid == deo.DataProducao
                        && c.EmpCod == "1"
                        && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                        && c.CtrlLoteLocArmazQtdSaldo > 0
                        && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                            && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                    //&& l.USERGranjaNucleoFLIP.Contains(granja)))
                    .Count();

                #endregion

                if (existe > 0)
                {
                    var listaLotes = bdApolo.CTRL_LOTE_LOC_ARMAZ
                        .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                            && c.CtrlLoteDataValid == deo.DataProducao
                            && c.EmpCod == "1"
                            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                            && c.CtrlLoteLocArmazQtdSaldo > 0
                            && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                        .OrderByDescending(o => o.CtrlLoteDataValid)
                        .ToList();

                    int saldo = Convert.ToInt32(deo.QtdeOvos);
                    int disponivel = 0;

                    foreach (var item in listaLotes)
                    {
                        #region Verifica quantidade já inserida

                        int saldoDisponivel = 0;
                        int qtdInseridaNaoBaixada = 0;

                        int existeDEOInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Count();

                        if (existeDEOInserido == 0)
                        {
                            qtdInseridaNaoBaixada = 0;
                        }
                        else
                        {
                            decimal qtdDeoInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Sum(s => s.QtdeOvos);

                            qtdInseridaNaoBaixada = Convert.ToInt32(qtdDeoInserido);
                        }

                        saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                        #endregion

                        if (saldo > saldoDisponivel)
                        {
                            #region Se saldo maior que o disponível, insere o disponivel para a Data

                            saldo = saldo - saldoDisponivel;
                            disponivel = disponivel + saldoDisponivel;

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = DateTime.Now;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = Convert.ToInt32(deo.CodItemDEO);
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            #endregion
                        }
                        else if (saldo > 0)
                        {
                            #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = deo.DataHoraCarreg;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            disponivel = disponivel + saldo;
                            saldo = 0;
                            break;

                            #endregion
                        }
                    }

                    bdSQLServer.SaveChanges();
                }

                #endregion
            }

            #endregion

            #region Importa DEOs p/ o Apolo - Granjas

            var listaDEOsImportacao = bdSQLServer.ImportaDiarioExpedicao
                .Where(i => i.TipoDEO == "Ovos Incubáveis" && i.DataHoraCarreg == dataHoraCarreg && i.Granja == granja)
                .GroupBy(g => new { g.Granja, g.DataHoraCarreg, g.TipoDEO })
                .OrderBy(o => new { o.Key.DataHoraCarreg })
                .ToList();

            foreach (var deoImportacao in listaDEOsImportacao)
            {
                ImportaDEOApolo(deoImportacao.Key.Granja, deoImportacao.Key.DataHoraCarreg, deoImportacao.Key.TipoDEO);

                bdSQLServer.SaveChanges();
            }

            #endregion

            #region Importa DEOs p/ o Apolo - Incubatório

            var listaDEOsImportacaoInc = bdSQLServer.ImportaDiarioExpedicao
                .Where(i => i.TipoDEO != "Ovos Incubáveis" && i.DataHoraCarreg == dataHoraCarreg)
                .GroupBy(g => new { g.Granja, g.DataHoraCarreg, g.TipoDEO })
                .OrderBy(o => new { o.Key.DataHoraCarreg })
                .ToList();

            foreach (var deoImportacao in listaDEOsImportacaoInc)
            {
                ImportaDEOApolo(deoImportacao.Key.Granja, deoImportacao.Key.DataHoraCarreg, deoImportacao.Key.TipoDEO);

                bdSQLServer.SaveChanges();
            }

            #endregion

            #region Importa Incubações p/ o Apolo

            var listaLotesImportaInc = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.Granja == granja && l.DataHoraCarreg == dataHoraCarreg)
                .ToList();

            foreach (var deoItem in listaLotesImportaInc)
            {
                string flock = deoItem.Nucleo + "-" + deoItem.LoteCompleto;

                var listaIncubacoes = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.ImportadoApolo == "Sim" && h.Flock_id == flock
                        && h.Lay_date == deoItem.DataProducao)// && h.Set_date >= setDate)
                    .GroupBy(g => new { g.Set_date, g.Hatch_loc })
                    .OrderBy(o => o.Key.Set_date)
                    .ToList();

                foreach (var item in listaIncubacoes)
                {
                    ImportaIncubacaoApolo(item.Key.Hatch_loc, item.Key.Set_date);
                }
            }

            #endregion
        }

        public void RefazEstoqueApolo()
        {
            bdApolo.CommandTimeout = 100000;
            bdSQLServer.CommandTimeout = 100000;

            DateTime dataFiltro = Convert.ToDateTime("09/02/2015 15:56:00");

            #region Deleta as Movimentações de Estoque do Apolo relacionados ao lote

            DeletaEstoqueApoloTotal();

            bdApolo.SaveChanges();

            #endregion

            #region Exclusão dos registros p/ Importação

            var listaImportDEO = bdSQLServer.ImportaDiarioExpedicao
                .ToList();

            foreach (var item in listaImportDEO)
            {
                bdSQLServer.ImportaDiarioExpedicao.DeleteObject(item);
            }

            var listaRelacionamento = bdSQLServer.LayoutDEO_X_ImportaDEO.ToList();

            foreach (var item in listaRelacionamento)
            {
                bdSQLServer.LayoutDEO_X_ImportaDEO.DeleteObject(item);
            }

            bdSQLServer.SaveChanges();

            #endregion

            #region Ajusta a tabela de Importação do DEO p/ o Apolo - Granjas - Modo Antigo

            var listaDEOs = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.TipoDEO == "Ovos Incubáveis"
                    && l.DataHora <= dataFiltro)
                .OrderBy(o => new { o.DataHoraCarreg, o.DataProducao, o.LoteCompleto })
                .ToList();

            foreach (var deo in listaDEOs)
            {
                #region Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                #region Localiza os Lotes

                LOC_ARMAZ localArmazenagem = bdApolo.LOC_ARMAZ
                    .Where(l => l.USERCodigoFLIP == deo.Granja && l.USERTipoProduto == "Ovos Incubáveis")
                    .FirstOrDefault();

                int existe = bdApolo.CTRL_LOTE_LOC_ARMAZ
                    .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                        && c.CtrlLoteDataValid == deo.DataProducao
                        && c.EmpCod == "1"
                        && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                        && c.CtrlLoteLocArmazQtdSaldo > 0
                        && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                            && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                    //&& l.USERGranjaNucleoFLIP.Contains(granja)))
                    .Count();

                #endregion

                if (existe > 0)
                {
                    var listaLotes = bdApolo.CTRL_LOTE_LOC_ARMAZ
                        .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                            //&& c.CtrlLoteDataValid == deo.DataProducao
                            && c.CtrlLoteDataValid <= deo.DataProducao
                            && c.EmpCod == "1"
                            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                            && c.CtrlLoteLocArmazQtdSaldo > 0
                            && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                        .OrderByDescending(o => o.CtrlLoteDataValid)
                        .ToList();

                    int saldo = Convert.ToInt32(deo.QtdeOvos);
                    int disponivel = 0;

                    foreach (var item in listaLotes)
                    {
                        #region Verifica quantidade já inserida

                        int saldoDisponivel = 0;
                        int qtdInseridaNaoBaixada = 0;

                        int existeDEOInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Count();

                        if (existeDEOInserido == 0)
                        {
                            qtdInseridaNaoBaixada = 0;
                        }
                        else
                        {
                            decimal qtdDeoInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Sum(s => s.QtdeOvos);

                            qtdInseridaNaoBaixada = Convert.ToInt32(qtdDeoInserido);
                        }

                        saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                        #endregion

                        if (saldo > saldoDisponivel)
                        {
                            #region Se saldo maior que o disponível, insere o disponivel para a Data

                            saldo = saldo - saldoDisponivel;
                            disponivel = disponivel + saldoDisponivel;

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = DateTime.Now;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = Convert.ToInt32(deo.CodItemDEO);
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            #endregion
                        }
                        else if (saldo > 0)
                        {
                            #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = deo.DataHoraCarreg;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            disponivel = disponivel + saldo;
                            saldo = 0;
                            break;

                            #endregion
                        }
                    }

                    bdSQLServer.SaveChanges();
                }

                #endregion
            }

            bdSQLServer.SaveChanges();

            #endregion

            #region Ajusta a tabela de Importação do DEO p/ o Apolo - Granjas - Modo Novo

            var listaDEOsNovo = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.TipoDEO == "Ovos Incubáveis"
                    && l.DataHora > dataFiltro)
                .OrderBy(o => new { o.DataHoraCarreg, o.DataProducao, o.LoteCompleto })
                .ToList();

            foreach (var deo in listaDEOsNovo)
            {
                #region Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                #region Localiza os Lotes

                LOC_ARMAZ localArmazenagem = bdApolo.LOC_ARMAZ
                    .Where(l => l.USERCodigoFLIP == deo.Granja && l.USERTipoProduto == "Ovos Incubáveis")
                    .FirstOrDefault();

                int existe = bdApolo.CTRL_LOTE_LOC_ARMAZ
                    .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                        && c.CtrlLoteDataValid == deo.DataProducao
                        && c.EmpCod == "1"
                        && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                        && c.CtrlLoteLocArmazQtdSaldo > 0
                        && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                            && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                    //&& l.USERGranjaNucleoFLIP.Contains(granja)))
                    .Count();

                #endregion

                if (existe > 0)
                {
                    var listaLotes = bdApolo.CTRL_LOTE_LOC_ARMAZ
                        .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                            && c.CtrlLoteDataValid == deo.DataProducao
                            && c.EmpCod == "1"
                            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                            && c.CtrlLoteLocArmazQtdSaldo > 0
                            && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                        .OrderByDescending(o => o.CtrlLoteDataValid)
                        .ToList();

                    int saldo = Convert.ToInt32(deo.QtdeOvos);
                    int disponivel = 0;

                    foreach (var item in listaLotes)
                    {
                        #region Verifica quantidade já inserida

                        int saldoDisponivel = 0;
                        int qtdInseridaNaoBaixada = 0;

                        int existeDEOInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Count();

                        if (existeDEOInserido == 0)
                        {
                            qtdInseridaNaoBaixada = 0;
                        }
                        else
                        {
                            decimal qtdDeoInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Sum(s => s.QtdeOvos);

                            qtdInseridaNaoBaixada = Convert.ToInt32(qtdDeoInserido);
                        }

                        saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                        #endregion

                        if (saldo > saldoDisponivel)
                        {
                            #region Se saldo maior que o disponível, insere o disponivel para a Data

                            saldo = saldo - saldoDisponivel;
                            disponivel = disponivel + saldoDisponivel;

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = DateTime.Now;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = Convert.ToInt32(deo.CodItemDEO);
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            #endregion
                        }
                        else if (saldo > 0)
                        {
                            #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = deo.DataHoraCarreg;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            disponivel = disponivel + saldo;
                            saldo = 0;
                            break;

                            #endregion
                        }
                    }

                    bdSQLServer.SaveChanges();
                }

                #endregion
            }

            bdSQLServer.SaveChanges();

            #endregion

            #region Importa DEOs p/ o Apolo - Granjas

            var listaDEOsImportacao = bdSQLServer.ImportaDiarioExpedicao
                .Where(i => i.TipoDEO == "Ovos Incubáveis")
                .GroupBy(g => new { g.Granja, g.DataHoraCarreg, g.TipoDEO })
                .OrderBy(o => new { o.Key.DataHoraCarreg })
                .ToList();

            foreach (var deoImportacao in listaDEOsImportacao)
            {
                ImportaDEOApolo(deoImportacao.Key.Granja, deoImportacao.Key.DataHoraCarreg, deoImportacao.Key.TipoDEO);

                bdSQLServer.SaveChanges();
            }

            #endregion

            #region Ajusta a tabela de Importação do DEO p/ o Apolo - Incubatorio - Modo Antigo

            var listaDEOsIncubatorio = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.TipoDEO != "Ovos Incubáveis"
                    && l.DataHora <= dataFiltro)
                .OrderBy(o => new { o.DataHoraCarreg, o.DataProducao, o.LoteCompleto })
                .ToList();

            foreach (var deo in listaDEOsIncubatorio)
            {
                #region Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                #region Localiza os Lotes

                LOC_ARMAZ localArmazenagem = bdApolo.LOC_ARMAZ
                    .Where(l => l.USERCodigoFLIP == deo.Granja && l.USERTipoProduto == "Ovos Incubáveis")
                    .FirstOrDefault();

                int existe = bdApolo.CTRL_LOTE_LOC_ARMAZ
                    .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                        && c.CtrlLoteDataValid == deo.DataProducao
                        && c.EmpCod == "1"
                        && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                        && c.CtrlLoteLocArmazQtdSaldo > 0
                        && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                            && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                    //&& l.USERGranjaNucleoFLIP.Contains(granja)))
                    .Count();

                #endregion

                if (existe > 0)
                {
                    var listaLotes = bdApolo.CTRL_LOTE_LOC_ARMAZ
                        .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                            //&& c.CtrlLoteDataValid == deo.DataProducao
                            && c.CtrlLoteDataValid <= deo.DataProducao
                            && c.EmpCod == "1"
                            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                            && c.CtrlLoteLocArmazQtdSaldo > 0
                            && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                        .OrderByDescending(o => o.CtrlLoteDataValid)
                        .ToList();

                    int saldo = Convert.ToInt32(deo.QtdeOvos);
                    int disponivel = 0;

                    foreach (var item in listaLotes)
                    {
                        #region Verifica quantidade já inserida

                        int saldoDisponivel = 0;
                        int qtdInseridaNaoBaixada = 0;

                        int existeDEOInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Count();

                        if (existeDEOInserido == 0)
                        {
                            qtdInseridaNaoBaixada = 0;
                        }
                        else
                        {
                            decimal qtdDeoInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Sum(s => s.QtdeOvos);

                            qtdInseridaNaoBaixada = Convert.ToInt32(qtdDeoInserido);
                        }

                        saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                        #endregion

                        if (saldo > saldoDisponivel)
                        {
                            #region Se saldo maior que o disponível, insere o disponivel para a Data

                            saldo = saldo - saldoDisponivel;
                            disponivel = disponivel + saldoDisponivel;

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = DateTime.Now;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = Convert.ToInt32(deo.CodItemDEO);
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            #endregion
                        }
                        else if (saldo > 0)
                        {
                            #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = deo.DataHoraCarreg;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            disponivel = disponivel + saldo;
                            saldo = 0;
                            break;

                            #endregion
                        }
                    }

                    bdSQLServer.SaveChanges();
                }

                #endregion
            }

            bdSQLServer.SaveChanges();

            #endregion

            #region Ajusta a tabela de Importação do DEO p/ o Apolo - Incubatorio - Modo Novo

            var listaDEOsIncubatorioNovo = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.TipoDEO != "Ovos Incubáveis"
                    && l.DataHora > dataFiltro)
                .OrderBy(o => new { o.DataHoraCarreg, o.DataProducao, o.LoteCompleto })
                .ToList();

            foreach (var deo in listaDEOsIncubatorioNovo)
            {
                #region Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                #region Localiza os Lotes

                LOC_ARMAZ localArmazenagem = bdApolo.LOC_ARMAZ
                    .Where(l => l.USERCodigoFLIP == deo.Granja && l.USERTipoProduto == "Ovos Incubáveis")
                    .FirstOrDefault();

                int existe = bdApolo.CTRL_LOTE_LOC_ARMAZ
                    .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                        && c.CtrlLoteDataValid == deo.DataProducao
                        && c.EmpCod == "1"
                        && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                        && c.CtrlLoteLocArmazQtdSaldo > 0
                        && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                            && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                    //&& l.USERGranjaNucleoFLIP.Contains(granja)))
                    .Count();

                #endregion

                if (existe > 0)
                {
                    var listaLotes = bdApolo.CTRL_LOTE_LOC_ARMAZ
                        .Where(c => c.CtrlLoteNum == deo.LoteCompleto
                            && c.CtrlLoteDataValid == deo.DataProducao
                            && c.EmpCod == "1"
                            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                            && c.CtrlLoteLocArmazQtdSaldo > 0
                            && bdApolo.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                        .OrderByDescending(o => o.CtrlLoteDataValid)
                        .ToList();

                    int saldo = Convert.ToInt32(deo.QtdeOvos);
                    int disponivel = 0;

                    foreach (var item in listaLotes)
                    {
                        #region Verifica quantidade já inserida

                        int saldoDisponivel = 0;
                        int qtdInseridaNaoBaixada = 0;

                        int existeDEOInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Count();

                        if (existeDEOInserido == 0)
                        {
                            qtdInseridaNaoBaixada = 0;
                        }
                        else
                        {
                            decimal qtdDeoInserido = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid
                                    && i.Importado != "Conferido")
                                .Sum(s => s.QtdeOvos);

                            qtdInseridaNaoBaixada = Convert.ToInt32(qtdDeoInserido);
                        }

                        saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                        #endregion

                        if (saldo > saldoDisponivel)
                        {
                            #region Se saldo maior que o disponível, insere o disponivel para a Data

                            saldo = saldo - saldoDisponivel;
                            disponivel = disponivel + saldoDisponivel;

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = DateTime.Now;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = Convert.ToInt32(deo.CodItemDEO);
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            #endregion
                        }
                        else if (saldo > 0)
                        {
                            #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                            ImportaDiarioExpedicao importaDEO = bdSQLServer.ImportaDiarioExpedicao
                                .Where(i => i.Granja == deo.Granja
                                    && i.LoteCompleto == deo.LoteCompleto
                                    && i.DataHoraCarreg == deo.DataHoraCarreg
                                    && i.DataProducao == item.CtrlLoteDataValid)
                                .FirstOrDefault();

                            if (importaDEO == null)
                            {
                                #region Se não existe o DEO de Importação, será adicionado

                                importaDEO = new ImportaDiarioExpedicao();

                                importaDEO.Nucleo = deo.Nucleo;
                                importaDEO.Galpao = deo.Galpao;
                                importaDEO.Lote = deo.Lote;
                                importaDEO.Idade = deo.Idade;
                                importaDEO.Linhagem = deo.Linhagem;
                                importaDEO.LoteCompleto = deo.LoteCompleto;
                                importaDEO.DataProducao = item.CtrlLoteDataValid;
                                importaDEO.NumeroReferencia = deo.NumeroReferencia;
                                importaDEO.QtdeOvos = saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                importaDEO.Usuario = deo.Usuario;
                                importaDEO.DataHora = deo.DataHoraCarreg;
                                importaDEO.DataHoraCarreg = deo.DataHoraCarreg;
                                importaDEO.DataHoraRecebInc = deo.DataHoraRecebInc;
                                importaDEO.ResponsavelCarreg = deo.ResponsavelCarreg;
                                importaDEO.ResponsavelReceb = deo.ResponsavelReceb;
                                importaDEO.Granja = deo.Granja;
                                importaDEO.NFNum = deo.NFNum;
                                importaDEO.Importado = "Ajuste";
                                importaDEO.TipoDEO = deo.TipoDEO;
                                importaDEO.GTANum = deo.GTANum;
                                importaDEO.Lacre = deo.Lacre;
                                importaDEO.NumIdentificacao = deo.NumIdentificacao;
                                importaDEO.Incubatorio = deo.Incubatorio;

                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                bdApolo.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                bdSQLServer.ImportaDiarioExpedicao.AddObject(importaDEO);

                                LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                #endregion
                            }
                            else
                            {
                                #region Se existe, será a atualizada a quantidade

                                LayoutDEO_X_ImportaDEO deoXimporta = bdSQLServer.LayoutDEO_X_ImportaDEO
                                    .Where(l => l.CodItemDEO == deo.CodItemDEO
                                        && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                    .FirstOrDefault();

                                if (deoXimporta == null)
                                {
                                    deoXimporta = new LayoutDEO_X_ImportaDEO();

                                    deoXimporta.CodItemDEO = (int)deo.CodItemDEO;
                                    deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                    bdSQLServer.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                }

                                importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                                importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                #endregion
                            }

                            disponivel = disponivel + saldo;
                            saldo = 0;
                            break;

                            #endregion
                        }
                    }

                    bdSQLServer.SaveChanges();
                }

                #endregion
            }

            bdSQLServer.SaveChanges();

            #endregion

            #region Importa DEOs p/ o Apolo - Incubatório

            var listaDEOsImportacaoInc = bdSQLServer.ImportaDiarioExpedicao
                .Where(i => i.TipoDEO != "Ovos Incubáveis")
                .GroupBy(g => new { g.Granja, g.DataHoraCarreg, g.TipoDEO })
                .OrderBy(o => new { o.Key.DataHoraCarreg })
                .ToList();

            foreach (var deoImportacao in listaDEOsImportacaoInc)
            {
                ImportaDEOApolo(deoImportacao.Key.Granja, deoImportacao.Key.DataHoraCarreg, deoImportacao.Key.TipoDEO);

                bdSQLServer.SaveChanges();
            }

            #endregion

            #region Importa Incubações p/ o Apolo

            //DateTime setDate = Convert.ToDateTime("17/12/2014");

            var listaIncubacoes = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.ImportadoApolo == "Sim")// && h.Set_date >= setDate)
                .GroupBy(g => new { g.Set_date, g.Hatch_loc })
                .OrderBy(o => o.Key.Set_date)
                .ToList();

            foreach (var item in listaIncubacoes)
            {
                ImportaIncubacaoApolo(item.Key.Hatch_loc, item.Key.Set_date);
            }

            #endregion

            #region Atualiza Tabelas de Saldo

            //var listaLinhagens = bdApolo.PRODUTO.Where(p => p.ProdNome.Contains("ovos ferteis")
            //    && p.ProdNomeAlt1 != null && p.ProdNomeAlt1 != "").ToList();

            //foreach (var linhagem in listaLinhagens)
            //{
            //    ITEM_MOV_ESTQ itemEntradaDeOvos = bdApolo.ITEM_MOV_ESTQ
            //        .Where(i => i.EmpCod == "1" && i.ProdCodEstr == linhagem.ProdCodEstr
            //            && bdApolo.TIPO_LANC.Any(t => i.TipoLancCod == t.TipoLancCod
            //                && t.TipoLancNome.Contains("ENTRADA DE OVOS")))
            //        .OrderBy(o => o.ItMovEstqDataMovimento)
            //        .FirstOrDefault();

            //    bdApolo.atualiza_saldoestqdata(itemEntradaDeOvos.EmpCod, itemEntradaDeOvos.MovEstqChv, itemEntradaDeOvos.ProdCodEstr,
            //        itemEntradaDeOvos.ItMovEstqSeq, itemEntradaDeOvos.ItMovEstqDataMovimento, "UPD");
            //}

            #endregion
        }

        public void DeletaMovEstq(MOV_ESTQ movestq)
        {
            var listaLotes = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                        .ToList();

            foreach (var lote in listaLotes)
            {
                bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(lote);
            }

            var listaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                .ToList();

            foreach (var local in listaLocal)
            {
                bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.DeleteObject(local);
            }

            var listaItens = bdApolo.ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                .ToList();

            foreach (var itens in listaItens)
            {
                bdApolo.ITEM_MOV_ESTQ.DeleteObject(itens);
            }

            bdApolo.MOV_ESTQ.DeleteObject(movestq);
        }

        public void AtualizaTabelaSaldo()
        {
            #region Atualiza Tabelas de Saldo

            bdApolo.CommandTimeout = 100000;
            bdSQLServer.CommandTimeout = 100000;

            var listaLinhagens = bdApolo.PRODUTO.Where(p => p.ProdNome.Contains("ovos ferteis")
                && p.ProdNomeAlt1 != null && p.ProdNomeAlt1 != "").ToList();

            foreach (var linhagem in listaLinhagens)
            {
                ITEM_MOV_ESTQ itemEntradaDeOvos = bdApolo.ITEM_MOV_ESTQ
                    .Where(i => i.EmpCod == "1" && i.ProdCodEstr == linhagem.ProdCodEstr
                        && i.USERCalculadoSaldoServico == "Não")
                    .OrderBy(o => o.ItMovEstqDataMovimento)
                    .FirstOrDefault();

                if (itemEntradaDeOvos != null)
                    bdApolo.atualiza_saldoestqdata(itemEntradaDeOvos.EmpCod, itemEntradaDeOvos.MovEstqChv, itemEntradaDeOvos.ProdCodEstr,
                        itemEntradaDeOvos.ItMovEstqSeq, itemEntradaDeOvos.ItMovEstqDataMovimento, "UPD");

                var listaItemMovEstq = bdApolo.ITEM_MOV_ESTQ
                    .Where(i => i.EmpCod == "1" && i.ProdCodEstr == linhagem.ProdCodEstr
                        && i.USERCalculadoSaldoServico == "Não")
                    .OrderBy(o => o.ItMovEstqDataMovimento)
                    .ToList();

                foreach (var item in listaItemMovEstq)
                {
                    item.USERCalculadoSaldoServico = "Sim";
                }


                bdApolo.SaveChanges();
            }

            #endregion
        }

        public void DeletaMovAjuste()
        {
            var listaMovEstq = bdApolo.MOV_ESTQ.Where(m => bdApolo.TIPO_LANC.Any(t => t.TipoLancCod == m.TipoLancCod
                && t.TipoLancNome.Contains("AJUSTE DE ENTRADA")) && m.MovEstq == "Sim" && m.EmpCod == "1").ToList();

            foreach (var item in listaMovEstq)
            {
                DeletaMovEstq(item);
            }

            bdApolo.SaveChanges();
        }

        public void AjustaTabelaImportaDEOs()
        {
            bdApolo.CommandTimeout = 100000;
            bdSQLServer.CommandTimeout = 100000;

            DateTime data = Convert.ToDateTime("22/02/2015 00:00:00");

            var listaDEOsAjuste = bdSQLServer.LayoutDiarioExpedicaos
                .Where(l => l.DataHora >= data && l.Granja != "CH")
                .GroupBy(g => new { g.Granja, g.DataHoraCarreg, g.LoteCompleto, g.DataProducao })
                .Where(l => bdSQLServer.ImportaDiarioExpedicao.Where(i => i.Granja == l.Key.Granja
                        && i.DataHoraCarreg == l.Key.DataHoraCarreg && i.LoteCompleto == l.Key.LoteCompleto
                        && i.DataProducao == l.Key.DataProducao).Sum(s => s.QtdeOvos) != l.Sum(s2 => s2.QtdeOvos))
                .OrderBy(o => o.Key.DataHoraCarreg)
                .ToList();

            foreach (var item in listaDEOsAjuste)
            {
                AjustaTabelaImportaDEOs(item.Key.Granja, item.Key.DataHoraCarreg);
            }
        }

        public void AjustaIncubacoes()
        {
            #region Importa Incubações p/ o Apolo

            DateTime setDate = Convert.ToDateTime("17/12/2014");

            var listaIncubacoes = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.ImportadoApolo == "Sim" && h.Set_date >= setDate)
                .GroupBy(g => new { g.Set_date, g.Hatch_loc })
                .OrderBy(o => o.Key.Set_date)
                .ToList();

            foreach (var item in listaIncubacoes)
            {
                ImportaIncubacaoApolo(item.Key.Hatch_loc, item.Key.Set_date);
            }

            #endregion
        }

        public void AjustaDiarioProducaoPlanalto()
        {
            try
            {
                Fech_Estq fechEstq = bdApolo.Fech_Estq.Where(f => f.EmpCod == "20").FirstOrDefault();
                DateTime dataFechamento = new DateTime();
                if (fechEstq != null)
                    dataFechamento = fechEstq.FechEstqData;
                else
                    dataFechamento = Convert.ToDateTime("2016-06-01");

                DateTime dataProducao = Convert.ToDateTime("2016-06-10");
                HLBAPPEntities hlbapp = new HLBAPPEntities();
                hlbapp.CommandTimeout = 10000;
                var lista = hlbapp.LayoutDiarioExpedicaos
                    .Where(w => w.Granja.Substring(0,2).Equals("PL") 
                        && w.TipoDEO.Equals("Ovos Incubáveis")
                        //&& w.LoteCompleto == "BFD29424DW" && w.DataProducao == dataProducao
                        && w.Linhagem.Substring(0,3) == "DKB"
                        && w.DataProducao > dataFechamento)
                    .GroupBy(g => new
                    {
                        g.LoteCompleto,
                        g.DataProducao
                    })
                    .Select(s => new
                    {
                        s.Key.LoteCompleto,
                        s.Key.DataProducao,
                        Qtde = s.Sum(u => u.QtdeOvos)
                    })
                    .ToList();

                foreach (var item in lista)
                {
                    //if (!ExisteFechamentoEstoque(item.DataProducao, "PL"))
                    //{
                        FLOCK_DATATableAdapter fdTA = new FLOCK_DATATableAdapter();
                        FLIPDataSet.FLOCK_DATADataTable fdDT = new FLIPDataSet.FLOCK_DATADataTable();
                        fdTA.FillByFlockTrxDate(fdDT, item.LoteCompleto, item.DataProducao);

                        if (fdDT.Count > 0)
                        {
                            FLIPDataSet.FLOCK_DATARow fdRow = fdDT.FirstOrDefault();
                            if (fdRow.NUM_1 != item.Qtde)
                            {
                                fdRow.NUM_1 = item.Qtde;
                                fdTA.Update(fdRow);
                            }
                        }
                        else
                        {
                            FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
                            FLIPDataSet.FLOCKSDataTable fDT = new FLIPDataSet.FLOCKSDataTable();
                            fTA.FillByFlockID(fDT, item.LoteCompleto);

                            FLIPDataSet.FLOCKSRow fRow = fDT.FirstOrDefault();

                            if (fRow != null)
                            {
                                int age = (((item.DataProducao - fRow.MOVE_DATE).Days) / 7) + 1;

                                EGGINV_DATATableAdapter eTA = new EGGINV_DATATableAdapter();
                                eTA.DeleteByFlockIDAndLayDateAndStatus(item.LoteCompleto, item.DataProducao, "O");

                                fdTA.Insert(fRow.COMPANY, fRow.REGION, fRow.LOCATION,
                                    fRow.FARM_ID, fRow.FLOCK_ID, 1, item.DataProducao, age, null, null, null,
                                        null, null, null, null, null, null, null, null, item.Qtde, null,
                                        null, null, null, null, null, 0, null, null, null, null, null, null, null,
                                        null, null, null);
                            }
                        }
                    //}
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpo = "Erro ao Ajustar Diário da Granja Planalto: "
                    + (char)10 + (char)13 + linenum + (char)10 + (char)13 
                    + ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message != null)
                    {
                        corpo = (char)10 + (char)13 + corpo + ex.InnerException.Message;
                        if (!ex.InnerException.Message.Substring(0, 16).Equals("Timeout expirado."))
                            EnviarEmail(corpo, "**** ERRO SERVICO ATUALIZAR DIARIO DA GRANJA DA PLANALTO****",
                                "Paulo Alves", "palves@hyline.com.br", "", "");
                    }
                }
                else
                {
                    EnviarEmail(corpo, "**** ERRO SERVICO ATUALIZAR DIARIO DA GRANJA DA PLANALTO****", "Paulo Alves",
                        "palves@hyline.com.br", "", "");
                }
            }
        }

        public void AjustaDEOxApolo()
        {
            #region Verifica Datas de Fechamento

            bdApolo.CommandTimeout = 10000;
            bdSQLServer.CommandTimeout = 10000;

            Fech_Estq fechEstq = bdApolo.Fech_Estq.Where(f => f.EmpCod == "20").FirstOrDefault();
            DateTime dataFechamentoPlanalto = new DateTime();
            if (fechEstq != null)
                dataFechamentoPlanalto = fechEstq.FechEstqData;
            else
                dataFechamentoPlanalto = Convert.ToDateTime("2016-06-01");

            fechEstq = bdApolo.Fech_Estq.Where(f => f.EmpCod == "1").FirstOrDefault();
            DateTime dataFechamentoNG = new DateTime();
            if (fechEstq != null)
                dataFechamentoNG = fechEstq.FechEstqData;
            else
                dataFechamentoNG = Convert.ToDateTime("2016-06-01");

            #endregion

            #region Ajusta DEO x Apolo

            var listaDEOsDiferenca = bdSQLServer.LayoutDiarioExpedicaos
                .Where(w => (
                    (w.DataHoraCarreg >= dataFechamentoPlanalto && w.Linhagem.Contains("DB")) 
                    ||
                    (w.DataHoraCarreg >= dataFechamentoNG && !w.Linhagem.Contains("DB")))
                    //&& w.NumIdentificacao == "11109"
                    //w.Observacao == "Erro Data 01 Minuto - 03."
                    && w.Importado == "Conferido")
                .GroupBy(g => new
                {
                    g.Granja,
                    g.DataHoraCarreg,
                    g.NumIdentificacao,
                    g.TipoDEO
                })
                .Select(s => new
                {
                    s.Key.Granja,
                    s.Key.DataHoraCarreg,
                    s.Key.NumIdentificacao,
                    s.Key.TipoDEO,
                    Qtde = s.Sum(u => u.QtdeOvos)
                })
                .OrderBy(o => o.Granja).ThenBy(t => t.DataHoraCarreg)
                .ToList();

            foreach (var item in listaDEOsDiferenca)
            {
                var listaMovEstq = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                    .Where(w => bdApolo.MOV_ESTQ.Any(m => w.EmpCod == m.EmpCod && w.MovEstqChv == m.MovEstqChv
                        && m.MovEstqDocEspec == "TLA" && m.MovEstqDocSerie == "99"
                        && m.MovEstqDocNum == item.NumIdentificacao))
                    .ToList();

                int qtdApolo = Convert.ToInt32(listaMovEstq.Sum(s => s.CtrlLoteItMovEstqQtd));

                if (item.Qtde != qtdApolo)
                {
                    ImportaDEOApolo(item.Granja, item.DataHoraCarreg, item.TipoDEO);
                }
            }

            #endregion
        }

        public string ImportaDiarioProducaoWEB(string company, string region, string farmID, string flockID, 
            string numLote, string variety, int active, int age, DateTime trxDate, int henMort, decimal henWt, 
            int maleMort, decimal henFeedDel, int totalEggsProd, decimal eggWt, int hatchEggs, string comentarios,
            // 20/04/2017 - Variáveis novas para inserir todos os valores para WebService LTZ
            int count_females, int count_males, int broken, int dirty, int consume, int floor, int destroyed,
            decimal water_consumption, decimal uniformity, string farmName, int numGalpao)
        {
            string retorno = "";

            //try
            //{
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            hlbapp.CommandTimeout = 10000;

            FLOCK_DATA flockData = hlbapp.FLOCK_DATA.Where(w => w.Flock_ID == flockID
                && w.Trx_Date == trxDate
                && w.Company == company
                && w.Region == region).FirstOrDefault();

            if (flockData == null)
                flockData = new FLOCK_DATA();

            flockData.Company = company;
            flockData.Region = region;
            flockData.Farm_ID = farmID;
            flockData.Flock_ID = flockID;
            flockData.NumLote = numLote;
            flockData.Variety = variety;
            flockData.Active = active;
            flockData.Age = age;
            flockData.Trx_Date = trxDate;
            flockData.Hen_Mort = henMort;
            flockData.Hen_Wt = henWt;
            flockData.Male_Mort = maleMort;
            flockData.Hen_Feed_Del = henFeedDel;
            flockData.Total_Eggs_Prod = totalEggsProd;
            flockData.Egg_Wt = eggWt;
            flockData.Hatch_Eggs = hatchEggs;
            flockData.Comentarios = comentarios;
            flockData.count_females = count_females;
            flockData.count_males = count_males;
            flockData.broken = broken;
            flockData.dirty = dirty;
            flockData.consume = consume;
            flockData.floor = floor;
            flockData.destroyed = destroyed;
            flockData.water_consumption = water_consumption;
            flockData.uniformity = uniformity;
            flockData.farm_name = farmName;
            flockData.num_galpao = numGalpao;

            if (flockData.ID == 0) hlbapp.FLOCK_DATA.AddObject(flockData);

            hlbapp.SaveChanges();

            return retorno;
            //}
            //catch (Exception ex)
            //{
            //    if (ex.InnerException == null)
            //        retorno = ex.Message;
            //    else
            //        retorno = ex.Message + " - Erro Interno: " + ex.InnerException.Message;

            //    return retorno;
            //}
        }

        public int ACMFEMINV(DateTime trxDate, string flockID)
        {
            int result = 0;

            #region Hens Moved

            FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
            FLIPDataSet.FLOCKSRow fR = fTA.GetDataByFlockID(flockID)[0];
            int hensMoved = 0;
            if (!fR.IsHENS_MOVEDNull()) hensMoved = Convert.ToInt32(fR.HENS_MOVED);

            #endregion

            #region Acum Mortality

            FLOCK_DATATableAdapter fdTA = new FLOCK_DATATableAdapter();
            int acmHenMort = Convert.ToInt32(fdTA.AcmHenMort(flockID, trxDate.AddDays(-1)));

            result = hensMoved - acmHenMort;

            #endregion

            return result;
        }

        public int ACMMALEINV(DateTime trxDate, string flockID)
        {
            int result = 0;

            #region Males Moved

            FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
            FLIPDataSet.FLOCKSRow fR = fTA.GetDataByFlockID(flockID)[0];
            int malesMoved = 0;
            if (!fR.IsMALES_MOVEDNull()) malesMoved = Convert.ToInt32(fR.MALES_MOVED);

            #endregion

            #region Acum Mortality

            FLOCK_DATATableAdapter fdTA = new FLOCK_DATATableAdapter();
            int acmMaleMort = Convert.ToInt32(fdTA.AcmMaleMort(flockID, trxDate.AddDays(-1)));

            result = malesMoved - acmMaleMort;

            #endregion

            return result;
        }

        public string DeletaDiarioProducaoWEB(string flockID, DateTime trxDate)
        {
            string retorno = "";

            try
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();
                hlbapp.CommandTimeout = 10000;

                FLOCK_DATA flockData = hlbapp.FLOCK_DATA.Where(w => w.Flock_ID == flockID
                    && w.Trx_Date == trxDate).FirstOrDefault();

                if (flockData != null) hlbapp.FLOCK_DATA.DeleteObject(flockData);

                hlbapp.SaveChanges();

                return retorno;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    retorno = ex.Message;
                else
                    retorno = ex.Message + " - Erro Interno: " + ex.InnerException.Message;

                return retorno;
            }
        }

        public void ImportaDadosNascimentoFLIPparaWEB()
        {
            FLIPDataSet.HATCHERY_FLOCK_DATA1DataTable fDT = new FLIPDataSet.HATCHERY_FLOCK_DATA1DataTable();
            HATCHERY_FLOCK_DATA1TableAdapter hTA = new HATCHERY_FLOCK_DATA1TableAdapter();
            // 09/04/2020 - Conforme conversado com a Sérica, incubações a partir de 23 / 03 / 2020 será realizados pelo WEB (Filtro dentro do Dataset)
            hTA.FillNaoImportadosWEB(fDT);
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            foreach (var item in fDT)
            {
                HATCHERY_FLOCK_SETTER_DATA nascimentoWEB = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                    .Where(w => w.Hatch_Loc == item.HATCH_LOC && w.Set_date == item.SET_DATE
                        && w.Flock_id == item.FLOCK_ID).FirstOrDefault();

                bool existe = true;
                string operacao = "Atualização";
                if (nascimentoWEB == null)
                {
                    operacao = "Inclusão";

                    nascimentoWEB = new HATCHERY_FLOCK_SETTER_DATA();
                    nascimentoWEB.Hatch_Loc = item.HATCH_LOC;
                    nascimentoWEB.Set_date = item.SET_DATE;
                    nascimentoWEB.Flock_id = item.FLOCK_ID;
                    
                    nascimentoWEB.Setter = "Todas";
                    nascimentoWEB.Hatcher = "Todos";
                    nascimentoWEB.ClassOvo = item.HATCH_LOC;

                   existe = false;
                }

                int posicaoHifen = item.FLOCK_ID.IndexOf("-") + 1;
                int tamanho = item.FLOCK_ID.Length - posicaoHifen;
                string flock = item.FLOCK_ID.Substring(posicaoHifen, tamanho);
                FLIPDataSet.FLOCKSDataTable fsDT = new FLIPDataSet.FLOCKSDataTable();
                flocks.FillByFlockID(fsDT, flock);
                nascimentoWEB.Variety = fsDT.FirstOrDefault().VARIETY;
                nascimentoWEB.NumLote = fsDT.FirstOrDefault().NUM_1.ToString();

                HATCHERY_EGG_DATATableAdapter eTA = new HATCHERY_EGG_DATATableAdapter();
                int qtdOvos = Convert.ToInt32(eTA.QtdeOvosByHatchLocSetDateFlockID(item.SET_DATE, item.HATCH_LOC, 
                    item.FLOCK_ID));
                nascimentoWEB.Qtde_Incubada = qtdOvos;
                if (!item.IsNUM_1Null()) nascimentoWEB.Eliminado = Convert.ToInt32(item.NUM_1);
                else nascimentoWEB.Eliminado = 0;
                if (!item.IsACTUALNull()) nascimentoWEB.Pintos_Vendaveis = Convert.ToInt32(item.ACTUAL);
                else nascimentoWEB.Pintos_Vendaveis = 0;
                if (!item.IsNUM_2Null()) nascimentoWEB.Refugo = Convert.ToInt32(item.NUM_2);
                else nascimentoWEB.Refugo = 0;
                if (!item.IsNUM_13Null()) nascimentoWEB.Amostra = Convert.ToInt32(item.NUM_13);
                else nascimentoWEB.Amostra = 0;
                if (!item.IsNUM_19Null()) nascimentoWEB.Infertil = Convert.ToInt32(item.NUM_19);
                else nascimentoWEB.Infertil = 0;
                if (!item.IsNUM_20Null()) nascimentoWEB.Inicial0a3 = Convert.ToInt32(item.NUM_20);
                else nascimentoWEB.Inicial0a3 = 0;
                if (!item.IsNUM_4Null()) nascimentoWEB.Inicial4a7 = Convert.ToInt32(item.NUM_4);
                else nascimentoWEB.Inicial4a7 = 0;
                if (!item.IsNUM_5Null()) nascimentoWEB.Media8a14 = Convert.ToInt32(item.NUM_5);
                else nascimentoWEB.Media8a14 = 0;
                if (!item.IsNUM_6Null()) nascimentoWEB.Tardia15a18 = Convert.ToInt32(item.NUM_6);
                else nascimentoWEB.Tardia15a18 = 0;
                if (!item.IsNUM_7Null()) nascimentoWEB.Tardia19a21 = Convert.ToInt32(item.NUM_7);
                else nascimentoWEB.Tardia19a21 = 0;
                if (!item.IsNUM_8Null()) nascimentoWEB.BicadoVivo = Convert.ToInt32(item.NUM_8);
                else nascimentoWEB.BicadoVivo = 0;
                if (!item.IsNUM_21Null()) nascimentoWEB.BicadoMorto = Convert.ToInt32(item.NUM_21);
                else nascimentoWEB.BicadoMorto = 0;
                if (!item.IsNUM_11Null()) nascimentoWEB.ContaminacaoBacteriana = Convert.ToInt32(item.NUM_11);
                else nascimentoWEB.ContaminacaoBacteriana = 0;
                if (!item.IsNUM_10Null()) nascimentoWEB.Fungo = Convert.ToInt32(item.NUM_10);
                else nascimentoWEB.Fungo = 0;
                if (!item.IsNUM_24Null()) nascimentoWEB.MaFormacaoCerebro = Convert.ToInt32(item.NUM_24);
                else nascimentoWEB.MaFormacaoCerebro = 0;
                if (!item.IsNUM_23Null()) nascimentoWEB.MaFormacaoVisceras = Convert.ToInt32(item.NUM_23);
                else nascimentoWEB.MaFormacaoVisceras = 0;
                if (!item.IsNUM_12Null()) nascimentoWEB.Anormalidade = Convert.ToInt32(item.NUM_12);
                else nascimentoWEB.Anormalidade = 0;
                if (!item.IsNUM_16Null()) nascimentoWEB.MalPosicionado = Convert.ToInt32(item.NUM_16);
                else nascimentoWEB.MalPosicionado = 0;
                if (!item.IsNUM_27Null()) nascimentoWEB.Infertilidade10Dias = Convert.ToInt32(item.NUM_27);
                else nascimentoWEB.Infertilidade10Dias = 0;
                nascimentoWEB.Pinto_Terceira = 0;

                if (!existe) hlbapp.HATCHERY_FLOCK_SETTER_DATA.AddObject(nascimentoWEB);

                InsereLOGHatcheryFlockSetterData(nascimentoWEB, DateTime.Now, operacao, "Serviço");

                item.NUM_26 = 1;
                hTA.Update(item);
            }

            hlbapp.SaveChanges();
        }

        public void AjustaDiarioProducaoJeriquara()
        {
            try
            {
                DateTime dataFechamento = Convert.ToDateTime("2016-06-01");
                
                HLBAPPEntities hlbapp = new HLBAPPEntities();
                hlbapp.CommandTimeout = 10000;
                var lista = hlbapp.FLOCK_DATA
                    .Where(w => w.Farm_ID.Substring(0,2).Equals("JR") 
                        //&& w.LoteCompleto == "BFD29424DW" && w.DataProducao == dataProducao
                        && w.Trx_Date > dataFechamento
                        && hlbapp.LayoutDiarioExpedicaos
                            .Any(l => l.LoteCompleto == w.Flock_ID
                                && l.DataProducao == w.Trx_Date
                                && l.TipoDEO.Equals("Ovos Incubáveis")
                                && l.Granja == w.Farm_ID))
                    .GroupBy(g => new
                    {
                        g.Flock_ID,
                        g.Trx_Date
                    })
                    .Select(s => new
                    {
                        LoteCompleto = s.Key.Flock_ID,
                        DataProducao = s.Key.Trx_Date,
                        Qtde = s.Sum(u => u.Hatch_Eggs)
                    })
                    .ToList();

                foreach (var item in lista)
                {
                    //if (!ExisteFechamentoEstoque(item.DataProducao, "PL"))
                    //{
                    DateTime dataProducaoItem = Convert.ToDateTime(item.DataProducao);

                        FLOCK_DATATableAdapter fdTA = new FLOCK_DATATableAdapter();
                        FLIPDataSet.FLOCK_DATADataTable fdDT = new FLIPDataSet.FLOCK_DATADataTable();
                        fdTA.FillByFlockTrxDate(fdDT, item.LoteCompleto,
                            dataProducaoItem);

                        string teste = "";
                    DateTime dataProducao = Convert.ToDateTime("2016-10-07 00:00:00.000");
                    if (item.LoteCompleto.Equals("JRP036173L")
                        && item.DataProducao.Equals(dataProducao))
                        teste = item.LoteCompleto;

                        if (fdDT.Count > 0)
                        {
                            FLIPDataSet.FLOCK_DATARow fdRow = fdDT.FirstOrDefault();
                            if (fdRow.IsNUM_1Null()) fdRow.NUM_1 = 0;
                            if (fdRow.NUM_1 != item.Qtde)
                            {
                                fdRow.NUM_1 = Convert.ToInt32(item.Qtde);
                                fdTA.Update(fdRow);
                            }
                        }
                        else
                        {
                            FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
                            FLIPDataSet.FLOCKSDataTable fDT = new FLIPDataSet.FLOCKSDataTable();
                            fTA.FillByFlockID(fDT, item.LoteCompleto);

                            FLIPDataSet.FLOCKSRow fRow = fDT.FirstOrDefault();

                            if (fRow != null)
                            {
                                int age = (((dataProducaoItem - fRow.MOVE_DATE).Days) 
                                    / 7) + 1;

                                EGGINV_DATATableAdapter eTA = new EGGINV_DATATableAdapter();
                                eTA.DeleteByFlockIDAndLayDateAndStatus
                                    (item.LoteCompleto, dataProducaoItem, "O");

                                fdTA.Insert(fRow.COMPANY, fRow.REGION, fRow.LOCATION,
                                    fRow.FARM_ID, fRow.FLOCK_ID, 1, dataProducaoItem, 
                                    age, null, null, null,
                                    null, null, null, null, null, null, null, null, 
                                    item.Qtde, null,
                                    null, null, null, null, null, 0, null, null, null, 
                                    null, null, null, null, null, null, null);
                            }
                        }
                    //}
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpo = "Erro ao Ajustar Diário da Granja Planalto: "
                    + (char)10 + (char)13 + linenum + (char)10 + (char)13 
                    + ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message != null)
                    {
                        corpo = (char)10 + (char)13 + corpo + ex.InnerException.Message;
                        if (!ex.InnerException.Message.Substring(0, 16).Equals("Timeout expirado."))
                            EnviarEmail(corpo, "**** ERRO SERVICO ATUALIZAR DIARIO DA GRANJA DA PLANALTO****",
                                "Paulo Alves", "palves@hyline.com.br", "", "");
                    }
                }
                else
                {
                    EnviarEmail(corpo, "**** ERRO SERVICO ATUALIZAR DIARIO DA GRANJA DA PLANALTO****", "Paulo Alves",
                        "palves@hyline.com.br", "", "");
                }
            }
        }

        public decimal CalculaMediaEstimadaPonderadaEclosao(string incubatorio, DateTime setDate,
            string loteCompleto)
        {
            var incubacao = bdSQLServer.HATCHERY_EGG_DATA
                .Where(w => w.Hatch_loc == incubatorio
                    && w.Set_date == setDate && w.Flock_id == loteCompleto)
                .GroupBy(g => new
                {
                    g.Hatch_loc,
                    g.Set_date,
                    g.Flock_id
                })
                .Select(s => new
                {
                    PintosEstimados = s.Sum(u => (u.Eggs_rcvd * (u.Estimate / 100.00m))),
                    TotalOvosIncubados = s.Sum(u => u.Eggs_rcvd)
                })
                .FirstOrDefault();

            decimal pintosEstimados = 0.00m;
            decimal totalOvosIncubados = 1.00m;
            if (incubacao != null)
            {
                pintosEstimados = Convert.ToDecimal(incubacao.PintosEstimados);
                totalOvosIncubados = Convert.ToDecimal(incubacao.TotalOvosIncubados);
            }

            return Convert.ToDecimal((pintosEstimados / totalOvosIncubados) * 100.00m);
        }

        #region Integrações FLIP

        #region Incubações

        public void AtualizaIncubacoesWEBparaFLIP(DateTime setDate, string incubatorio)
        {
            try
            {
                #region Carrega Variáveis

                HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();

                DateTime data = Convert.ToDateTime("01/07/2013");
                hatchCodes.FillByHatchLoc(flipDataSet.HATCHERY_CODES, incubatorio);

                string location = flipDataSet.HATCHERY_CODES[0].LOCATION;

                #endregion

                if ((setDate >= data) ||
                    ((setDate == Convert.ToDateTime("19/06/2013")) && (incubatorio == "CH")) ||
                    ((setDate == Convert.ToDateTime("20/11/2013")) && (incubatorio == "TB"))) 
                    // erro de fechamento, por isso o dia 19/06.
                {
                    #region Caso haja diferença de qtde de registros entre WEB e FLIP, deleta todas as Incubações do FLIP e inclui novamente

                    #region Carrega Lista do WEB para Conferência

                    var lista = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Set_date == setDate 
                            && h.Status == "Importado" 
                            && h.Hatch_loc == incubatorio)// && h.Flock_id == "HLP04-P044292W")
                        .GroupBy(h => new
                        {
                            h.Company,
                            h.Region,
                            h.Location,
                            h.Set_date,
                            h.Hatch_loc,
                            h.Flock_id,
                            h.Lay_date,
                            h.Machine,
                            h.Track_no
                        })
                        .Select(h => new //HATCHERY_EGG_DATA
                        {
                            type = h.Key,
                            soma = h.Sum(x => x.Eggs_rcvd),
                            estimate = h.Max(x => x.Estimate),
                            observacao = h.Max(x => x.Observacao)
                        })
                        .ToList();

                    #endregion

                    #region Verifica Qtde de Itens Incubados

                    int existeIncubacao = Convert.ToInt32(hatcheryEggData
                        .ExisteHatcheryEggDataForSetDate("HYBR", "BR", location, setDate, incubatorio));

                    #endregion

                    if (lista.Count != existeIncubacao)
                    {
                        #region Deleta todas as Incubações do FLIP e inclui novamente

                        hatcheryEggData.DeleteByHatchLocAndSetDate(setDate, incubatorio);

                        FLIPDataSet.HATCHERY_EGG_DATADataTable listaFLIP =
                            hatcheryEggData.GetDataBySetDate("HYBR", "BR", location, setDate, incubatorio);

                        foreach (var item in listaFLIP)
                        {
                            int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                                    h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                                    h.Hatch_loc == item.HATCH_LOC && h.Flock_id == item.FLOCK_ID &&
                                    h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                                    h.Track_no == item.TRACK_NO)
                                .Count();

                            if (existeHLBAPP == 0)
                            {
                                hatcheryEggData.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                                    item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                                existeHLBAPP = Convert.ToInt32(hatcheryEggData
                                    .ExisteHatcheryEggDataForFlockData(item.COMPANY,
                                        item.REGION, item.LOCATION, item.SET_DATE,
                                        item.HATCH_LOC, item.FLOCK_ID));

                                if (existeHLBAPP == 0)
                                {
                                    hatcheryFlockData.Delete(item.COMPANY,
                                        item.REGION, item.LOCATION, item.SET_DATE,
                                        item.HATCH_LOC, item.FLOCK_ID);
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion

                    #region Caso Não exista a Incubação no FLIP ou a Qtde é diferente, atualiza o FLIP conforme o WEB

                    foreach (var item in lista)
                    {
                        #region Carrega Qtd. Ovos do Lote ou Se existe ele digitado

                        decimal qtdOvos = Convert.ToDecimal(hatcheryEggData.QtdOvos(item.type.Company, 
                            item.type.Region, item.type.Location, item.type.Set_date, item.type.Hatch_loc, 
                            item.type.Flock_id, item.type.Lay_date, item.type.Machine, item.type.Track_no));

                        int existeInc = Convert.ToInt32(hatcheryEggData
                            .ExisteHatcheryEggDataAll(item.type.Company, item.type.Region, item.type.Location,
                                item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, 
                                item.type.Machine, item.type.Track_no));

                        #endregion

                        if ((qtdOvos != item.soma) || (existeInc == 0))
                        {
                            int start = item.type.Flock_id.IndexOf("-") + 1;
                            int tamanho = item.type.Flock_id.Length - start;

                            string lote = item.type.Flock_id.Substring(start, tamanho);
                            string farm = item.type.Flock_id.Substring(0, start - 1);

                            decimal qtdOvosSet = Convert.ToDecimal(eggInvData.QtdOvosByStatus(
                                lote, "S", item.type.Lay_date, incubatorio));

                            if (qtdOvosSet < item.soma)
                            {
                                if (qtdOvosSet == 0)
                                {
                                    eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                        farm, lote, item.type.Track_no, item.type.Lay_date, item.soma, "S", 
                                        null, null, null, null, null, null, null, null, item.type.Hatch_loc, null);
                                }
                                else
                                {
                                    decimal qtdUpdate = Convert.ToDecimal(qtdOvosSet + item.soma);

                                    eggInvData.UpdateQueryEggs(qtdUpdate, item.type.Company, item.type.Region, 
                                        item.type.Location, farm, lote, item.type.Track_no, item.type.Lay_date, "S", 
                                        item.type.Hatch_loc);
                                }
                            }

                            hatcheryEggData.Delete(item.type.Company, item.type.Region, item.type.Location,
                                item.type.Set_date, item.type.Hatch_loc,
                                item.type.Flock_id, item.type.Lay_date,
                                item.type.Machine, item.type.Track_no);

                            #region AJUSTE EGG INVENTORY PARA INCLUIR INCUBAÇÃO

                            int existeAjuste = Convert.ToInt32(eggInvData.ScalarQueryOpen3(item.type.Flock_id, 
                                item.type.Track_no, item.type.Lay_date, incubatorio));

                            if (existeAjuste == 0)
                            {
                                eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                    farm, lote,
                                    item.type.Track_no, item.type.Lay_date, item.soma, "O", null, null, null, null, 
                                    null, null, null, null, item.type.Hatch_loc, null);
                            }
                            else
                            {
                                int qtdeOvosAjuste = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, 
                                    item.type.Track_no, item.type.Lay_date, incubatorio));

                                if (qtdeOvosAjuste < item.soma)
                                {
                                    eggInvData.UpdateQueryEggs(item.soma, item.type.Company, item.type.Region, 
                                        item.type.Location, farm, lote, item.type.Track_no, item.type.Lay_date, "O", 
                                        item.type.Hatch_loc);
                                }
                            }

                            #endregion

                            int existe = Convert.ToInt32(eggInvData.ScalarQueryOpen3(item.type.Flock_id, 
                                item.type.Track_no, item.type.Lay_date, incubatorio));

                            if (existe > 0)
                            {
                                int qtdeOvos = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, 
                                    item.type.Track_no, item.type.Lay_date, incubatorio));

                                if ((qtdeOvos - item.soma) >= 0)
                                {
                                    decimal existeSetDay = Convert.ToDecimal(setDayData
                                        .ExisteSetDayData(item.type.Set_date, item.type.Hatch_loc));

                                    if (existeSetDay == 0)
                                    {
                                        decimal sequencia = Convert.ToDecimal(setDayData
                                            .UltimaSequenciaSetDayData(incubatorio)) + 1;

                                        setDayData.InsertQuery("HYBR", "BR", location, item.type.Set_date, 
                                            item.type.Hatch_loc, sequencia);
                                    }

                                    existe = Convert.ToInt32(hatcheryEggData
                                        .ExisteHatcheryEggDataAll(item.type.Company, item.type.Region, 
                                            item.type.Location, item.type.Set_date, item.type.Hatch_loc, 
                                            item.type.Flock_id, item.type.Lay_date, item.type.Machine,
                                            item.type.Track_no));

                                    if (existe == 1)
                                    {
                                        hatcheryEggData.Delete(item.type.Company, item.type.Region, 
                                            item.type.Location, item.type.Set_date, item.type.Hatch_loc, 
                                            item.type.Flock_id, item.type.Lay_date, item.type.Machine, 
                                            item.type.Track_no);
                                    }

                                    existe = Convert.ToInt32(hatcheryFlockData
                                        .ExisteHatcheryFlockData(item.type.Company, item.type.Region, 
                                            item.type.Location, item.type.Set_date, item.type.Hatch_loc, 
                                            item.type.Flock_id));

                                    if (existe == 0)
                                    {
                                        hatcheryFlockData.InsertQuery(item.type.Company, item.type.Region, 
                                            item.type.Location, item.type.Set_date, item.type.Hatch_loc, 
                                            item.type.Flock_id, item.estimate);
                                    }
                                    
                                    // 14/08/2014 - Ocorrência 99 - APONTES
                                    // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
                                    // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
                                    // o trigger de atualização da idade executar.
                                    else
                                    {
                                        decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(
                                            incubatorio, item.type.Set_date, item.type.Flock_id);

                                        hatcheryFlockData.UpdateEstimate(mediaIncubacao, item.type.Company, 
                                            item.type.Region, item.type.Location, item.type.Set_date, 
                                            item.type.Hatch_loc, item.type.Flock_id);
                                    }

                                    hatcheryEggData.Insert(item.type.Company, item.type.Region, item.type.Location, 
                                        item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, 
                                        item.type.Lay_date, item.soma, null, item.type.Machine, item.type.Track_no, 
                                        null, null, null, null, null, null, null, null, item.observacao, 
                                        "servico");

                                    var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id 
                                            && h.Lay_date == item.type.Lay_date && h.Set_date == item.type.Set_date 
                                            && h.Machine == item.type.Machine && h.Location == item.type.Location 
                                            && h.Region == item.type.Region && h.Company == item.type.Company 
                                            && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                                    foreach (var naoImportado in listaNaoImportadoFLIP)
                                    {
                                        naoImportado.ImportadoFLIP = "Sim";
                                    }

                                    bdSQLServer.SaveChanges();
                                }
                                else
                                {
                                    var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id 
                                            && h.Lay_date == item.type.Lay_date && h.Set_date == item.type.Set_date 
                                            && h.Machine == item.type.Machine && h.Location == item.type.Location 
                                            && h.Region == item.type.Region && h.Company == item.type.Company 
                                            && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                                    foreach (var naoImportado in listaNaoImportadoFLIP)
                                    {
                                        naoImportado.ImportadoFLIP = "Não";
                                    }

                                    bdSQLServer.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id 
                                            && h.Lay_date == item.type.Lay_date && h.Set_date == item.type.Set_date 
                                            && h.Machine == item.type.Machine && h.Location == item.type.Location 
                                            && h.Region == item.type.Region && h.Company == item.type.Company 
                                            && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                            foreach (var naoImportado in listaNaoImportadoFLIP)
                            {
                                naoImportado.ImportadoFLIP = "Sim";
                            }

                            bdSQLServer.SaveChanges();
                        }
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {

            }
        }

        public void AtualizaTodasIncubacoesWEBparaFLIP()
        {
            //DATA_FECH_LANCTableAdapter dfTA = new DATA_FECH_LANCTableAdapter();
            //FLIPDataSet.DATA_FECH_LANCDataTable dfDT = new FLIPDataSet.DATA_FECH_LANCDataTable();

            //dfTA.Fill(dfDT);

            //FLIPDataSet.DATA_FECH_LANCRow dfRow = dfDT.Where(w => w.LOCATION == "Incubatorio")
            //    .FirstOrDefault();

            HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
            FLIPDataSet.HATCHERY_CODESDataTable hcDT = new FLIPDataSet.HATCHERY_CODESDataTable();
            hatchCodes.Fill(hcDT);
            DateTime data = Convert.ToDateTime("15/06/2020");

            foreach (var hatch in hcDT)
            {
                DateTime dateOpenBalance = DateOpenBalanceHatchery(hatch.HATCH_LOC);

                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var listaVerificaIncubacoes = hlbapp.HATCHERY_EGG_DATA
                    .Where(w => w.Set_date >= dateOpenBalance
                        && w.Hatch_loc == hatch.HATCH_LOC
                        && w.Hatch_loc == "PM" && w.Set_date == data)
                    .GroupBy(g => new
                        {
                            g.Hatch_loc,
                            g.Set_date
                        })
                    .OrderBy(o => o.Key.Set_date).ThenBy(t => t.Key.Hatch_loc)
                    .ToList();

                foreach (var item in listaVerificaIncubacoes)
                {
                    //AtualizaIncubacoesWEBparaFLIP(item.Key.Set_date, item.Key.Hatch_loc);
                    RefreshSettingEggsFLIP(item.Key.Hatch_loc, item.Key.Set_date);
                }
            }
        }

        #endregion

        #region Nascimento

        public string AtualizaDadosNascimentoFLIP(string location, string incubatorio, DateTime dataIncubacao, 
            string loteCompleto, HATCHERY_FLOCK_SETTER_DATA hatchDataSetter, string operacao)
        {
            string erro = "";
            
            try
            {
                string eggKey = "HYBRBR" + location + dataIncubacao.ToString("MM/dd/yy") + incubatorio
                    + loteCompleto;

                int qtdPintosNascidos = Convert.ToInt32(hatchDataSetter.Pintos_Vendaveis + hatchDataSetter.Refugo
                    + hatchDataSetter.Pinto_Terceira);

                FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                hfdTA.FillByFlockData(hfdDT, "HYBR", "BR", location, dataIncubacao, incubatorio, loteCompleto);

                if (hfdDT.Count > 0)
                {
                    FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                    if (operacao.Equals("Inclusão"))
                    {
                        hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado);
                        hatchData.ACTUAL = hatchData.ACTUAL + qtdPintosNascidos;
                        hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32(hatchDataSetter.Refugo
                            + hatchDataSetter.Pinto_Terceira);
                        hatchData.NUM_17 = hatchData.NUM_17 + qtdPintosNascidos + Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        hatchData.TEXT_2 = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                        hatchData.TEXT_3 = hatchDataSetter.Horario_02_Retirada.Replace(":", "H");
                        hatchData.DATE_1 = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                    }
                    else if (operacao.Equals("Exclusão"))
                    {
                        hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado);
                        hatchData.ACTUAL = hatchData.ACTUAL - qtdPintosNascidos;
                        hatchData.NUM_2 = hatchData.NUM_2 - Convert.ToInt32(hatchDataSetter.Refugo
                            + hatchDataSetter.Pinto_Terceira);
                        hatchData.NUM_17 = hatchData.NUM_17 - qtdPintosNascidos - Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        hatchData.TEXT_2 = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                        hatchData.TEXT_3 = hatchDataSetter.Horario_02_Retirada.Replace(":", "H");
                        hatchData.DATE_1 = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                    }

                    hfdTA.Update(hatchData);
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                erro = "Linha: " + linenum.ToString();

                if (ex.InnerException == null)
                    erro = erro + " - " + ex.Message;
                else
                    erro = erro + " - " + ex.Message + " / " + ex.InnerException.Message;
            }

            return erro;
        }

        public string AtualizaNascimentoFLIP(DateTime setDate, string incubatorio)
        {
            string erro = "";

            try
            {
                #region Deleta Valores

                FLIPDataSet flip = new FLIPDataSet();

                HLBAPPEntities hlbapp = new HLBAPPEntities();

                HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
                hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, incubatorio);
                string location = flip.HATCHERY_CODES[0].LOCATION;

                var listaDelecao = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                    .Where(w => w.Set_date == setDate
                        && w.Hatch_Loc == incubatorio)
                    .GroupBy(g => new { g.Set_date, g.Flock_id })
                    .ToList();

                foreach (var item in listaDelecao)
                {
                    FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                    HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                    hfdTA.FillByFlockData(hfdDT, "HYBR", "BR", location, Convert.ToDateTime(item.Key.Set_date),
                        incubatorio, item.Key.Flock_id);

                    if (hfdDT.Count > 0)
                    {
                        FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                        hatchData.NUM_1 = 0;
                        hatchData.ACTUAL = 0;
                        hatchData.NUM_2 = 0;
                        hatchData.NUM_17 = 0;
                        hatchData.NUM_13 = 0;
                        hatchData.NUM_19 = 0;
                        hatchData.NUM_20 = 0;
                        hatchData.NUM_4 = 0;
                        hatchData.NUM_5 = 0;
                        hatchData.NUM_6 = 0;
                        hatchData.NUM_7 = 0;
                        hatchData.NUM_8 = 0;
                        hatchData.NUM_21 = 0;
                        hatchData.NUM_11 = 0;
                        hatchData.NUM_10 = 0;
                        hatchData.NUM_24 = 0;
                        hatchData.NUM_23 = 0;
                        hatchData.NUM_9 = 0;
                        hatchData.NUM_12 = 0;
                        hatchData.NUM_16 = 0;

                        hfdTA.Update(hatchData);
                    }
                }

                #endregion

                #region Insere Valores

                var lista = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                    .Where(w => w.Set_date == setDate
                        && w.Hatch_Loc == incubatorio)
                    .ToList();

                foreach (var item in lista)
                {
                    erro = AtualizaDadosNascimentoFLIP(location, incubatorio, setDate, item.Flock_id, item, "Inclusão");
                    if (erro != "") return erro;
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (erro == "")
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                    erro = "Linha: " + linenum.ToString();
                    if (ex.InnerException == null)
                        erro = erro + ex.Message;
                    else
                        erro = erro + ex.Message + " / " + ex.InnerException.Message;
                }
            }

            return erro;
        }

        public string AtualizaNascimentosWEBparaFLIPNM()
        {
            string erro = "";

            try
            {
                DATA_FECH_LANCTableAdapter dfTA = new DATA_FECH_LANCTableAdapter();
                FLIPDataSet.DATA_FECH_LANCDataTable dfDT = new FLIPDataSet.DATA_FECH_LANCDataTable();

                dfTA.Fill(dfDT);

                FLIPDataSet.DATA_FECH_LANCRow dfRow = dfDT.Where(w => w.LOCATION == "Incubatorio")
                    .FirstOrDefault();

                if (dfRow != null)
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    var listaVerificaNascimentos = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                        .Where(w => w.Set_date >= dfRow.DATA_FECH_LANC
                            && w.Hatch_Loc == "NM")
                        .GroupBy(g => new
                        {
                            g.Hatch_Loc,
                            g.Set_date
                        })
                        .OrderBy(o => o.Key.Set_date).ThenBy(t => t.Key.Hatch_Loc)
                        .ToList();

                    foreach (var item in listaVerificaNascimentos)
                    {
                        DateTime setDate = Convert.ToDateTime(item.Key.Set_date);
                        erro = AtualizaNascimentoFLIP(setDate, item.Key.Hatch_Loc);
                        if (erro != "") return erro;
                    }
                }
            }
            catch (Exception ex)
            {
                if (erro == "")
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                    erro = "Linha: " + linenum.ToString();
                    if (ex.InnerException == null)
                        erro = erro + ex.Message;
                    else
                        erro = erro + ex.Message + " / " + ex.InnerException.Message;
                }
            }

            return erro;
        }

        public string RefreshHatchingEggsAll()
        {
            string erro = "";

            try
            {
                HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
                FLIPDataSet.HATCHERY_CODESDataTable hcDT = new FLIPDataSet.HATCHERY_CODESDataTable();
                hatchCodes.Fill(hcDT);

                foreach (var hatch in hcDT)
                {
                    //if (hatch.HATCH_LOC != "CH" && hatch.HATCH_LOC != "TB" && hatch.HATCH_LOC != "PH")
                    if (hatch.HATCH_LOC != "PH")
                    {
                        HLBAPPEntities hlbapp = new HLBAPPEntities();

                        DateTime dateOpenBalance = DateOpenBalanceHatchery(hatch.HATCH_LOC);

                        var listaVerificaNascimentos = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                            .Where(w => w.Set_date >= dateOpenBalance
                                && w.Hatch_Loc == hatch.HATCH_LOC)
                            .GroupBy(g => new
                            {
                                g.Hatch_Loc,
                                g.Set_date
                            })
                            .OrderBy(o => o.Key.Set_date).ThenBy(t => t.Key.Hatch_Loc)
                            .ToList();

                        foreach (var item in listaVerificaNascimentos)
                        {
                            DateTime setDate = Convert.ToDateTime(item.Key.Set_date);
                            RefreshHatchingEggsFLIP(item.Key.Hatch_Loc, setDate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (erro == "")
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                    erro = "Linha: " + linenum.ToString();
                    if (ex.InnerException == null)
                        erro = erro + ex.Message;
                    else
                        erro = erro + ex.Message + " / " + ex.InnerException.Message;
                }
            }

            return erro;
        }

        #endregion

        #endregion

        public void InsereLOGHatcheryFlockSetterData(HATCHERY_FLOCK_SETTER_DATA hfsd, DateTime dataHora,
            string operacao, string usuario)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            LOG_HATCHERY_FLOCK_SETTER_DATA log = new LOG_HATCHERY_FLOCK_SETTER_DATA();
            log.Data_Hora = dataHora;
            log.Operacao = operacao;
            log.Usuario = usuario;
            log.Hatch_Loc = hfsd.Hatch_Loc;
            log.Set_date = hfsd.Set_date;
            log.Flock_id = hfsd.Flock_id;
            log.NumLote = hfsd.NumLote;
            log.Setter = hfsd.Setter;
            log.Hatcher = hfsd.Hatcher;
            log.ClassOvo = hfsd.ClassOvo;
            log.Eliminado = hfsd.Eliminado;
            log.Morto = hfsd.Morto;
            log.Macho = hfsd.Macho;
            log.Pintos_Vendaveis = hfsd.Pintos_Vendaveis;
            log.Refugo = hfsd.Refugo;
            log.Pinto_Terceira = hfsd.Pinto_Terceira;
            log.Qtde_Incubada = hfsd.Qtde_Incubada;
            log.DataRetiradaReal = hfsd.DataRetiradaReal;
            log.Horario_01_Retirada = hfsd.Horario_01_Retirada;
            log.Qtde_01_Retirada = hfsd.Qtde_01_Retirada;
            log.Horario_02_Retirada = hfsd.Horario_02_Retirada;
            log.Qtde_02_Retirada = hfsd.Qtde_02_Retirada;
            log.Variety = hfsd.Variety;
            log.De0a4 = hfsd.De0a4;
            log.De5a12 = hfsd.De5a12;
            log.De13a17 = hfsd.De13a17;
            log.De18a21 = hfsd.De18a21;
            log.BicadoVivo = hfsd.BicadoVivo;
            log.BicadoMorto = hfsd.BicadoMorto;
            log.ContaminacaoBacteriana = hfsd.ContaminacaoBacteriana;
            log.Fungo = hfsd.Fungo;
            log.MalPosicionado = hfsd.MalPosicionado;
            log.MalFormado = hfsd.MalFormado;
            log.Infertil = hfsd.Infertil;
            log.Inicial0a3 = hfsd.Inicial0a3;
            log.Inicial4a7 = hfsd.Inicial4a7;
            log.Media8a14 = hfsd.Media8a14;
            log.Tardia15a18 = hfsd.Tardia15a18;
            log.Tardia19a21 = hfsd.Tardia19a21;
            log.MaFormacaoCerebro = hfsd.MaFormacaoCerebro;
            log.MaFormacaoVisceras = hfsd.MaFormacaoVisceras;
            log.Hemorragico = hfsd.Hemorragico;
            log.Anormalidade = hfsd.Anormalidade;
            log.Amostra = hfsd.Amostra;
            log.Infertilidade10Dias = hfsd.Infertilidade10Dias;
            log.EliminadoCancelamento = hfsd.EliminadoCancelamento;

            hlbappSession.LOG_HATCHERY_FLOCK_SETTER_DATA.AddObject(log);
            hlbappSession.SaveChanges();
        }

        public void AtualizaNumGalpaoFLOCKDATAWEB()
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaFlockDataWeb = hlbapp.FLOCK_DATA
                .Where(w => w.num_galpao == null)
                .ToList();

            foreach (var item in listaFlockDataWeb)
            {
                FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
                FLIPDataSet.FLOCKSDataTable fDT = new FLIPDataSet.FLOCKSDataTable();
                fTA.FillByFlockID(fDT, item.Flock_ID);
                var lote = fDT.FirstOrDefault();

                if (lote != null)
                {
                    if (!lote.IsNUM_2Null())
                        item.num_galpao = Convert.ToInt32(lote.NUM_2);
                }
            }

            hlbapp.SaveChanges();
        }

        public void AtualizaLoteAndIdadePedidoRacaoItem()
        {
            string nucleoErro = "";
            string galpaoErro = "";
            DateTime dataPedidoErro;

            try
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var listaItensPedidoRacao = hlbapp.PedidoRacao_Item.ToList();

                foreach (var item in listaItensPedidoRacao)
                {
                    DateTime dataPedido = Convert.ToDateTime(hlbapp.PedidoRacao
                        .Where(w => w.ID == item.IDPedidoRacao).FirstOrDefault().DataInicial);

                    nucleoErro = item.Nucleo;
                    galpaoErro = item.Galpao;
                    dataPedidoErro = dataPedido;

                    FLOCKSTableAdapter fTA = new FLOCKSTableAdapter();
                    FLIPDataSet.FLOCKSDataTable fDT = new FLIPDataSet.FLOCKSDataTable();
                    fTA.FillByFarmIdAndNumGalpao(fDT, item.Nucleo, Convert.ToDecimal(item.Galpao));

                    var lote = fDT
                        .Where(w => w.HATCH_DATE <= dataPedido
                            && w.VARIETY == item.Linhagem)
                        .OrderByDescending(o => o.HATCH_DATE)
                        .FirstOrDefault();

                    if (lote != null)
                    {
                        string loteCompleto = lote.FLOCK_ID;
                        item.UltimoLoteGalpaoPorLinhagem = loteCompleto;

                        //FLOCK_DATATableAdapter fdTA = new FLOCK_DATATableAdapter();
                        //int age = Convert.ToInt32(fdTA.LastAge(item.Nucleo, Convert.ToInt32(item.Galpao), item.Linhagem, 
                        //    dataPedido));
                        if (!lote.IsHATCH_DATENull())
                        {
                            int age = Convert.ToInt32(Math.Floor((dataPedido - lote.HATCH_DATE).TotalDays / 7));
                            item.IdadeLote = age;
                        }
                    }
                }

                hlbapp.SaveChanges();
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        #region FLIP

        #region ALL - SETTING EGGS

        public DateTime DateOpenBalanceHatchery(string hatchLoc)
        {
            string company = GetCompanyAndRegionByHatchLoc(hatchLoc, "company");
            DateTime dateOpen = new DateTime();

            if (company == "HYBR")
            {
                DATA_FECH_LANCTableAdapter dfTA = new DATA_FECH_LANCTableAdapter();
                FLIPDataSet.DATA_FECH_LANCDataTable dfDT = new FLIPDataSet.DATA_FECH_LANCDataTable();

                dfTA.Fill(dfDT);

                FLIPDataSet.DATA_FECH_LANCRow dfRow = dfDT.Where(w => w.LOCATION == hatchLoc)
                    .FirstOrDefault();

                if (dfRow != null)
                    dateOpen = dfRow.DATA_FECH_LANC;
            }
            else if (company == "HYCL")
            {
                Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter dfTA = new Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
                Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable dfDT = new Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable();

                dfTA.Fill(dfDT);

                Data.FLIP.CLFLOCKS.DATA_FECH_LANCRow dfRow = dfDT.Where(w => w.LOCATION == "Planta de Incubación")
                    .FirstOrDefault();

                if (dfRow != null)
                    dateOpen = dfRow.DATA_FECH_LANC;
            }
            else if (company == "HYCO")
            {
                Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter dfTA = new Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
                Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable dfDT = new Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable();

                dfTA.Fill(dfDT);

                Data.FLIP.HCFLOCKS.DATA_FECH_LANCRow dfRow = dfDT.Where(w => w.LOCATION == hatchLoc)
                    .FirstOrDefault();

                if (dfRow != null)
                    dateOpen = dfRow.DATA_FECH_LANC;
            }

            return dateOpen;
        }

        public int ExistsHatcheryEggDataAll(string company, string region, string location, DateTime setDate,
            string hatchLoc, string flockID, DateTime layDate, string setter, string trackNO)
        {
            int exists = 0;

            if (company == "HYBR")
            {
                exists = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataAll(company, region, location, setDate,
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                exists = Convert.ToInt32(hedTA.ExistsHEDAll(company, region, location, setDate,
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();

                int posicaoHifen = flockID.IndexOf("-") + 1;
                int tamanho = flockID.Length - posicaoHifen;
                string flock = flockID.Substring(posicaoHifen, tamanho);

                fdTA.FillByFlockAndTrxDate(fdDT, flock, layDate);

                foreach (var item in fdDT)
                {
                    exists = exists + Convert.ToInt32(hedTA.ExistsHEDAll(company, region, location, setDate,
                        hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID, layDate, setter, trackNO));
                }
            }

            return exists;
        }

        public decimal GetQtySettedEggs(string company, string region, string location, DateTime setDate,
            string hatchLoc, string flockID, DateTime layDate, string setter, string trackNO)
        {
            decimal GetQtySettedEggs = 0;
            if (company == "HYBR")
            {
                GetQtySettedEggs = Convert.ToDecimal(hatcheryEggData.QtdOvos(company, region, location, setDate,
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();

                GetQtySettedEggs = Convert.ToDecimal(hedTA.EggsQty(company, region, location, setDate,
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();

                int posicaoHifen = flockID.IndexOf("-") + 1;
                int tamanho = flockID.Length - posicaoHifen;
                string flock = flockID.Substring(posicaoHifen, tamanho);

                fdTA.FillByFlockAndTrxDate(fdDT, flock, layDate);
                foreach (var item in fdDT)
                {
                    GetQtySettedEggs = GetQtySettedEggs + Convert.ToDecimal(hedTA.EggsQty(company, region, location, setDate,
                        hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID, layDate, setter, trackNO));
                }
            }

            return GetQtySettedEggs;
        }

        public string GetCompanyAndRegionByHatchLoc(string hatchLoc, string field)
        {
            string fieldValue = "";

            HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
            FLIPDataSet.HATCHERY_CODESDataTable hDT = new FLIPDataSet.HATCHERY_CODESDataTable();
            hTA.FillByHatchLoc(hDT, hatchLoc);
            if (hDT.Count > 0)
            {
                var hc = hDT.FirstOrDefault();
                fieldValue = hc[field].ToString();
            }

            return fieldValue;
        }

        public string GetLocation(string company, string hatchLoc)
        {
            string location = "";
            HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();

            if (company == "HYBR")
            {
                FLIPDataSet.HATCHERY_CODESDataTable hcDT =
                    new FLIPDataSet.HATCHERY_CODESDataTable();
                hatchCodes.FillByHatchLoc(hcDT, hatchLoc);
                location = hcDT[0].LOCATION;
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_CODESTableAdapter hcTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_CODESTableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                hcTA.FillByHatchLoc(cl.HATCHERY_CODES, hatchLoc);
                location = cl.HATCHERY_CODES[0].LOCATION;
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_CODESTableAdapter hcTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_CODESTableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                hcTA.FillByHatchLoc(cl.HATCHERY_CODES, hatchLoc);
                location = cl.HATCHERY_CODES[0].LOCATION;
            }

            return location;
        }

        public int ExistsHatcheryEggDataForSetDate(string company, string region, string location,
            DateTime setDate, string hatchLoc)
        {
            int exists = 0;

            if (company == "HYBR")
            {
                //exists = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForSetDate(company, region, location,
                //    setDate, hatchLoc));
                FLIPDataSet.HATCHERY_EGG_DATADataTable hedDT = new FLIPDataSet.HATCHERY_EGG_DATADataTable();
                hatcheryEggData.FillGroupBySetDate(hedDT, company, region, location, setDate, hatchLoc);
                exists = hedDT.Count;
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                exists = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForSetDate(company, region, location, setDate,
                    hatchLoc));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable hedDT = new HCFLOCKS.HATCHERY_EGG_DATADataTable();
                hedTA.ExistsHatcheryEggDataForSetDate(hedDT, company, region, location, setDate, hatchLoc);
                exists = hedDT.Count;
            }

            return exists;
        }

        public void DeleteFLIPIfnotExistsWEB(string company, string region,
            string location, DateTime setDate, string hatchLoc)
        {
            // Verifica se existe mais no FLIP do que no HLBAPP. 
            // Caso exista, serão deletados, pois no HLBAPP que é o correto.

            if (company == "HYBR")
            {
                #region HYBR

                //DeleteByHatchLocAndSetDate(company, setDate, hatchLoc);

                var listaFLIP = hatcheryEggData.GetDataBySetDate(company, region, location, setDate,
                    hatchLoc);

                foreach (var item in listaFLIP)
                {
                    int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                            h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                            h.Hatch_loc == item.HATCH_LOC && h.Flock_id == item.FLOCK_ID &&
                            h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                            h.Track_no == item.TRACK_NO)
                        .Count();

                    if (existeHLBAPP == 0)
                    {
                        hatcheryEggData.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                        existeHLBAPP = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForFlockData(item.COMPANY,
                            item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID));

                        if (existeHLBAPP == 0)
                        {
                            hatcheryFlockData.Delete(item.COMPANY,
                                item.REGION, item.LOCATION, item.SET_DATE,
                                item.HATCH_LOC, item.FLOCK_ID);
                        }
                    }
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter htdTA =
                    new Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter();
                var listaFLIP = hedTA.GetDataBySetDate(company, region, location, setDate, hatchLoc);

                foreach (var item in listaFLIP)
                {
                    int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                            h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                            h.Hatch_loc == item.HATCH_LOC && h.Flock_id == item.FLOCK_ID &&
                            h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                            h.Track_no == item.TRACK_NO)
                        .Count();

                    if (existeHLBAPP == 0)
                    {
                        hedTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                        string eggKey = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR")) 
                                + hatchLoc + item.FLOCK_ID;

                        htdTA.DeleteByEggKeyLayDateAndMachine(eggKey, item.LAY_DATE, item.MACHINE);

                        existeHLBAPP = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForFlockData(item.COMPANY,
                            item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID));

                        if (existeHLBAPP == 0)
                        {
                            hfdTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                                item.HATCH_LOC, item.FLOCK_ID);
                        }
                    }
                }

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                var listaFLIP = hedTA.GetDataBySetDate(company, region, location, setDate, hatchLoc);

                foreach (var item in listaFLIP)
                {
                    int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                            h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                            h.Hatch_loc == item.HATCH_LOC &&
                            //h.Flock_id == item.FLOCK_ID &&
                            h.Flock_id == (item.FLOCK_ID.Substring(0, 12) + item.FLOCK_ID.Substring(13, 3)) &&
                            h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                            h.Track_no == item.TRACK_NO)
                        .Count();

                    if (existeHLBAPP == 0)
                    {
                        #region HATCHERY_TRAN_DATA - Se tem no FLIP e não no Web, deleta do FLIP

                        ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter htdTA = new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter();

                        string eggKey = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR")) + hatchLoc + item.FLOCK_ID;

                        var listaItensDeletar = htdTA.GetDataByEggKey(eggKey);
                        foreach (var transfer in listaItensDeletar)
                        {
                            var existeTransfWeb = bdSQLServer.HATCHERY_TRAN_DATA
                                .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                                    && w.Flock_id == item.FLOCK_ID)
                            .FirstOrDefault();

                            if (existeTransfWeb == null)
                                htdTA.Delete(eggKey, transfer.LAY_DATE, transfer.MACHINE, transfer.HATCHER, transfer.TRACK_NO);
                        }

                        #endregion

                        hedTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                        existeHLBAPP = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForFlockData(item.COMPANY,
                            item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID));

                        if (existeHLBAPP == 0)
                        {
                            hfdTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                                item.HATCH_LOC, item.FLOCK_ID);
                        }
                    }
                }

                #endregion
            }
        }

        public void DeleteByHatchLocAndSetDate(string company, DateTime setDate, string hatchLoc)
        {
            if (company == "HYBR")
            {
                hatcheryEggData.DeleteByHatchLocAndSetDate(setDate, hatchLoc);
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                hedTA.DeleteByHatchLocAndSetDate(setDate, hatchLoc);
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                hedTA.DeleteByHatchLocAndSetDate(setDate, hatchLoc);
            }
        }

        public bool UpdateSetFLIP(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits, string machine, string trackNO,
            decimal estimate, string obs)
        {
            bool imported = false;

            if (company == "HYCL")
            {
                // Chile
                imported = UpdateSetFLIPCL(company, region, farmID, flockID, layDate, setDate, location, hatchLoc, eggUnits,
                    machine, trackNO, estimate, obs);
            }
            else if (company == "HYBR")
            {
                // Brasil
                imported = UpdateSetFLIPBR(company, region, farmID, flockID, layDate, setDate, location, hatchLoc, eggUnits,
                    machine, trackNO, estimate, obs);
            }
            else if (company == "HYCO")
            {
                // Colombia / Ecuador

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.EGGINV_DATATableAdapter eiTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.EGGINV_DATATableAdapter();

                fdTA.FillByFlockAndTrxDate(fdDT, flockID, layDate);

                #region Delete Qty

                foreach (var item in fdDT)
                {
                    imported = UpdateSetFLIPHC(company, region, farmID, item.FLOCK_ID, layDate, setDate, location, hatchLoc, 0,
                        machine, trackNO, estimate, obs);
                    //if (!imported) return imported;
                }

                #endregion

                #region Insert / Update Qty

                int balance = (int)eggUnits; //100 = 50 + 25 + 50
                foreach (var item in fdDT)
                {
                    if (balance > 0)
                    {
                        var listEggInv = eiTA.GetDataFlockAndTrxDate(company, region, location, farmID, item.FLOCK_ID, layDate);
                        var eggInv = listEggInv.Where(w => w.STATUS == "O").FirstOrDefault();
                        if (eggInv != null)
                        {
                            int settQty = 0;
                            if (balance > eggInv.EGG_UNITS)
                                settQty = (int)eggInv.EGG_UNITS;
                            else
                                settQty = balance;

                            balance = balance - settQty;

                            imported = UpdateSetFLIPHC(company, region, farmID, item.FLOCK_ID, layDate, setDate, location, hatchLoc, settQty,
                                machine, trackNO, estimate, obs);

                            if (!imported && balance == 0) return imported;
                        }
                    }
                }

                #endregion
            }

            return imported;
        }

        public void RefreshSettingEggsFLIP(string hatchLoc, DateTime setDate)
        {
            try
            {
                #region Load data components

                DateTime data = Convert.ToDateTime("01/07/2013");
                string incubatorio = hatchLoc;
                string company = GetCompanyAndRegionByHatchLoc(hatchLoc, "company");
                string region = GetCompanyAndRegionByHatchLoc(hatchLoc, "region");
                string location = GetLocation(company, incubatorio);

                #endregion

                if ((setDate >= data) ||
                    ((setDate == Convert.ToDateTime("19/06/2013")) && (incubatorio == "CH")) ||
                    ((setDate == Convert.ToDateTime("20/11/2013")) && (incubatorio == "TB"))) // erro de fechamento, por isso o dia 19/06.
                {
                    #region Load Hatchery Egg Data Web

                    var lista = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == company && h.Region == region
                            && h.Set_date == setDate && h.Status == "Importado"
                            && h.Hatch_loc == incubatorio)// && h.Flock_id == "HLP04-P044292W")
                        .GroupBy(h => new
                        {
                            h.Company,
                            h.Region,
                            h.Location,
                            h.Set_date,
                            h.Hatch_loc,
                            h.Flock_id,
                            h.Lay_date,
                            h.Machine,
                            h.Track_no//,
                            //h.ClassOvo
                        })
                        .Select(h => new //HATCHERY_EGG_DATA
                        {
                            type = h.Key,
                            soma = h.Sum(x => x.Eggs_rcvd),
                            estimate = h.Max(x => x.Estimate),
                            observacao = h.Max(x => x.Observacao)
                        })
                        .ToList();

                    #endregion

                    #region If exists in FLIP and not in WEB, update data

                    int existeIncubacao = ExistsHatcheryEggDataForSetDate(company, region, location, setDate, incubatorio);
                    int qtdLista = lista.Count;

                    // Verifica se existe mais no FLIP do que no HLBAPP. 
                    // Caso exista, serão deletados, pois no HLBAPP que é o correto.
                    if (qtdLista != existeIncubacao)
                    {
                        DeleteFLIPIfnotExistsWEB(company, region, location, setDate, incubatorio);
                    }

                    #endregion

                    foreach (var item in lista)
                    {
                        #region Load Data about Hatching Data

                        decimal qtdOvos = GetQtySettedEggs(item.type.Company, item.type.Region,
                            item.type.Location, item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id,
                            item.type.Lay_date, item.type.Machine, item.type.Track_no);

                        int existeInc = ExistsHatcheryEggDataAll(item.type.Company, item.type.Region, item.type.Location,
                            item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine,
                            item.type.Track_no);

                        #endregion

                        if ((qtdOvos != item.soma) || (existeInc == 0))
                        {
                            #region Load Farm and Lote data

                            int start = item.type.Flock_id.IndexOf("-") + 1;
                            int tamanho = item.type.Flock_id.Length - start;

                            string lote = item.type.Flock_id.Substring(start, tamanho);
                            string farm = item.type.Flock_id.Substring(0, start - 1);

                            #endregion

                            #region Update FLIP

                            bool imported = UpdateSetFLIP(company, region, farm, lote, item.type.Lay_date, item.type.Set_date,
                                item.type.Location, item.type.Hatch_loc, Convert.ToDecimal(item.soma), item.type.Machine,
                                item.type.Track_no, Convert.ToDecimal(item.estimate), item.observacao);

                            #endregion

                            #region Check Hatchery Egg Data in WEB if imported in FLIP

                            string importadoFLIP = "Não";
                            if (imported) importadoFLIP = "Sim";

                            var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                    && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                    && h.Location == item.type.Location && h.Region == item.type.Region
                                    && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                .ToList();

                            foreach (var naoImportado in listaNaoImportadoFLIP)
                            {
                                naoImportado.ImportadoFLIP = importadoFLIP;
                            }

                            bdSQLServer.SaveChanges();

                            #endregion
                        }
                        else
                        {
                            #region Check Hatchery Egg Data in WEB as imported in FLIP

                            var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                    && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                    && h.Location == item.type.Location && h.Region == item.type.Region
                                    && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc
                                    && h.ImportadoFLIP != "Sim")
                                .ToList();

                            foreach (var naoImportado in listaNaoImportadoFLIP)
                            {
                                naoImportado.ImportadoFLIP = "Sim";
                            }

                            bdSQLServer.SaveChanges();

                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Exceção

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpo = "Erro ao Importar Incubações " + hatchLoc + " - " + setDate.ToShortDateString() + ": "
                    + ex.Message;
                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        corpo = (char)10 + (char)13 + corpo + ex.InnerException.Message;

                corpo = (char)10 + (char)13 + corpo + " Linha do erro: " + linenum.ToString();

                EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR INCUBAÇÕES ****", "Paulo Alves",
                    "palves@hyline.com.br", "", "");

                this.EventLog.WriteEntry(corpo, EventLogEntryType.Error, 10);

                #endregion
            }
        }

        #endregion

        #region ALL - HATCHING EGGS

        public void UpdateHatchingDataFLIP(string company, string region, string location, string hatchLoc,
            DateTime setDate, string flockID, HATCHERY_FLOCK_SETTER_DATA hatchDataSetter, string operation)
        {
            #region Load General Variables

            string eggKey = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR"))
                + hatchLoc + flockID;

            int qtyHatchingChicks = Convert.ToInt32(hatchDataSetter.Pintos_Vendaveis + hatchDataSetter.Refugo
                + hatchDataSetter.Pinto_Terceira);

            #endregion

            if (company == "HYBR")
            {
                #region HYBR

                #region Load BD objects

                FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                hfdTA.FillByFlockData(hfdDT, company, region, location, setDate, hatchLoc, flockID);

                #endregion

                #region Nascimento Mais Cedo e Mais Tarde

                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var listaNascimentoLote = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                    .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate && w.Flock_id == flockID)
                    .ToList();

                List<DateTime> listaData = new List<DateTime>();

                foreach (var item in listaNascimentoLote)
                {
                    if (item.DataRetiradaReal != null)
                    {
                        string dataRetirada = Convert.ToDateTime(item.DataRetiradaReal).ToString("dd/MM/yyyy");
                        DateTime data = new DateTime();
                        if (DateTime.TryParse(dataRetirada + " " + item.Horario_01_Retirada,
                            out data))
                        {
                            listaData.Add(data);
                        }
                    }
                }

                DateTime? dataNascimentoMaisCedo = null;
                DateTime? dataNascimentoMaisTarde = null;
                if (hatchDataSetter.DataRetiradaReal != null)
                {
                    dataNascimentoMaisCedo = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                    dataNascimentoMaisTarde = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                }

                string horaNascimentoMaisCedo = "";
                string horaNascimentoMaisTarde = "";

                if (hatchDataSetter.Horario_01_Retirada != null)
                {
                    horaNascimentoMaisCedo = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                    horaNascimentoMaisTarde = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                }

                if (listaData.Count > 0)
                {
                    dataNascimentoMaisCedo = listaData.Min(m => m);
                    dataNascimentoMaisTarde = listaData.Max(m => m);
                    horaNascimentoMaisCedo = listaData.Min(m => m).ToString("HH:mm");
                    horaNascimentoMaisTarde = listaData.Max(m => m).ToString("HH:mm");
                }

                #endregion

                if (hfdDT.Count > 0)
                {
                    FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                    if (operation.Equals("Insert"))
                    {
                        #region Insert

                        hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado);
                        hatchData.ACTUAL = hatchData.ACTUAL + qtyHatchingChicks;
                        hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32(hatchDataSetter.Refugo
                            + hatchDataSetter.Pinto_Terceira);
                        hatchData.NUM_17 = hatchData.NUM_17 + qtyHatchingChicks
                            + Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (dataNascimentoMaisCedo != null) hatchData.DATE_1 = Convert.ToDateTime(dataNascimentoMaisCedo);
                        hatchData.TEXT_2 = horaNascimentoMaisCedo.Replace(":", "H");
                        if (dataNascimentoMaisTarde != null) hatchData.DATE_2 = Convert.ToDateTime(dataNascimentoMaisTarde);
                        hatchData.TEXT_3 = horaNascimentoMaisTarde.Replace(":", "H");
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 + Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }
                    else if (operation.Equals("Delete"))
                    {
                        #region Delete

                        hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado);
                        hatchData.ACTUAL = hatchData.ACTUAL - qtyHatchingChicks;
                        hatchData.NUM_2 = hatchData.NUM_2 - Convert.ToInt32(hatchDataSetter.Refugo
                            + hatchDataSetter.Pinto_Terceira);
                        hatchData.NUM_17 = hatchData.NUM_17 - qtyHatchingChicks
                            - Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (dataNascimentoMaisCedo != null) hatchData.DATE_1 = Convert.ToDateTime(dataNascimentoMaisCedo);
                        hatchData.TEXT_2 = horaNascimentoMaisCedo.Replace(":", "H");
                        if (dataNascimentoMaisTarde != null) hatchData.DATE_2 = Convert.ToDateTime(dataNascimentoMaisTarde);
                        hatchData.TEXT_3 = horaNascimentoMaisTarde.Replace(":", "H");
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 - Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }

                    hfdTA.Update(hatchData);
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                #region Load BD objects

                ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                hfdTA.FillByFlockData(hfdDT, company, region, location, setDate, hatchLoc, flockID);

                #endregion

                if (hfdDT.Count > 0)
                {
                    ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                    if (operation.Equals("Insert"))
                    {
                        #region Insert

                        if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                        hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado);
                        if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                        hatchData.ACTUAL = hatchData.ACTUAL + qtyHatchingChicks;
                        if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                        hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32(hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira);
                        if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                        hatchData.NUM_17 = hatchData.NUM_17 + qtyHatchingChicks + Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 + Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }
                    else if (operation.Equals("Delete"))
                    {
                        #region Delete

                        if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                        hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado);
                        if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                        hatchData.ACTUAL = hatchData.ACTUAL - qtyHatchingChicks;
                        if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                        hatchData.NUM_2 = hatchData.NUM_2 - Convert.ToInt32(hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira);
                        if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                        hatchData.NUM_17 = hatchData.NUM_17 - qtyHatchingChicks - Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 - Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }

                    hfdTA.Update(hatchData);
                }

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable hedDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable();

                hedTA.FillByFlockData(hedDT, company, region, location, setDate, hatchLoc, flockID, null);

                var listGroupByFlocks = hedDT
                    .GroupBy(g => new
                    {
                        g.FLOCK_ID
                    })
                    .Select(s => new
                    {
                        s.Key.FLOCK_ID,
                        HatchEggTotal = s.Sum(m => m.EGGS_RCVD)
                    })
                    .ToList();

                var hatchEggAll = listGroupByFlocks.Sum(s => s.HatchEggTotal);

                foreach (var item in listGroupByFlocks)
                {
                    #region Load BD objects

                    ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                        new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable();
                    ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                        new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                    hfdTA.FillByFlockData(hfdDT, company, region, location, setDate, hatchLoc, item.FLOCK_ID);

                    #endregion

                    if (hfdDT.Count > 0)
                    {
                        var flocksQty = item.HatchEggTotal / hatchEggAll;

                        ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                        if (operation.Equals("Insert"))
                        {
                            #region Insert

                            if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                            hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado * flocksQty);
                            if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                            hatchData.ACTUAL = hatchData.ACTUAL + (qtyHatchingChicks * flocksQty);
                            if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                            hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32((hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira) * flocksQty);
                            if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                            hatchData.NUM_17 = hatchData.NUM_17 + ((qtyHatchingChicks + Convert.ToInt32(hatchDataSetter.Macho)) * flocksQty);
                            if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                            hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra * flocksQty);
                            if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                            hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil * flocksQty);
                            if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                            hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3 * flocksQty);
                            if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                            hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7 * flocksQty);
                            if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                            hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14 * flocksQty);
                            if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                            hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18 * flocksQty);
                            if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                            hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21 * flocksQty);
                            if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                            hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo * flocksQty);
                            if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                            hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto * flocksQty);
                            if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                            hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana * flocksQty);
                            if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                            hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo * flocksQty);
                            if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                            hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro * flocksQty);
                            if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                            hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras * flocksQty);
                            if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                            hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico * flocksQty);
                            if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                            hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade * flocksQty);
                            if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                            hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado * flocksQty);
                            if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                            hatchData.NUM_28 = hatchData.NUM_28 + Convert.ToInt32(hatchDataSetter.EliminadoCancelamento * flocksQty);

                            #endregion
                        }
                        else if (operation.Equals("Delete"))
                        {
                            #region Delete

                            if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                            hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado) * flocksQty;
                            if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                            hatchData.ACTUAL = hatchData.ACTUAL - (qtyHatchingChicks * flocksQty);
                            if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                            hatchData.NUM_2 = hatchData.NUM_2 - ((Convert.ToInt32(hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira)) * flocksQty);
                            if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                            hatchData.NUM_17 = hatchData.NUM_17 - ((qtyHatchingChicks - Convert.ToInt32(hatchDataSetter.Macho)) * flocksQty);
                            if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                            hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra * flocksQty);
                            if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                            hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil * flocksQty);
                            if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                            hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3 * flocksQty);
                            if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                            hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7 * flocksQty);
                            if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                            hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14 * flocksQty);
                            if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                            hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18 * flocksQty);
                            if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                            hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21 * flocksQty);
                            if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                            hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo * flocksQty);
                            if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                            hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto * flocksQty);
                            if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                            hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana * flocksQty);
                            if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                            hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo * flocksQty);
                            if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                            hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro * flocksQty);
                            if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                            hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras * flocksQty);
                            if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                            hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico * flocksQty);
                            if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                            hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade * flocksQty);
                            if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                            hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado * flocksQty);
                            if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                            hatchData.NUM_28 = hatchData.NUM_28 - Convert.ToInt32(hatchDataSetter.EliminadoCancelamento * flocksQty);

                            #endregion
                        }

                        hfdTA.Update(hatchData);
                    }
                }

                #endregion
            }
        }

        public void RefreshHatchingEggsFLIP(string hatchLoc, DateTime setDate)
        {
            #region Load Values

            FLIPDataSet flip = new FLIPDataSet();
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            string company = GetCompanyAndRegionByHatchLoc(hatchLoc, "company");
            string region = GetCompanyAndRegionByHatchLoc(hatchLoc, "region");
            HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
            hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, hatchLoc);
            string location = flip.HATCHERY_CODES[0].LOCATION;

            var listDelete = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Set_date == setDate && w.Hatch_Loc == hatchLoc)
                .GroupBy(g => new { g.Set_date, g.Flock_id })
                .ToList();

            var listInsert = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Set_date == setDate && w.Hatch_Loc == hatchLoc)
                .ToList();

            #endregion

            // Desativar para não atualizar NG, Ajapi e Avós devido eles fazerem pelo FLIP
            if (hatchLoc != "CH" && hatchLoc != "TB" && hatchLoc != "PH")
            {
                #region Delete Values

                if (company == "HYBR")
                {
                    #region HYBR

                    #region Delete Values

                    foreach (var item in listDelete)
                    {
                        FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                        HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                        hfdTA.FillByFlockData(hfdDT, company, region, location, Convert.ToDateTime(item.Key.Set_date),
                            hatchLoc, item.Key.Flock_id);

                        if (hfdDT.Count > 0)
                        {
                            FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                            hatchData.NUM_1 = 0;
                            hatchData.ACTUAL = 0;
                            hatchData.NUM_2 = 0;
                            hatchData.NUM_17 = 0;
                            hatchData.NUM_13 = 0;
                            hatchData.NUM_19 = 0;
                            hatchData.NUM_20 = 0;
                            hatchData.NUM_4 = 0;
                            hatchData.NUM_5 = 0;
                            hatchData.NUM_6 = 0;
                            hatchData.NUM_7 = 0;
                            hatchData.NUM_8 = 0;
                            hatchData.NUM_21 = 0;
                            hatchData.NUM_11 = 0;
                            hatchData.NUM_10 = 0;
                            hatchData.NUM_24 = 0;
                            hatchData.NUM_23 = 0;
                            hatchData.NUM_9 = 0;
                            hatchData.NUM_12 = 0;
                            hatchData.NUM_16 = 0;
                            hatchData.NUM_28 = 0;

                            hfdTA.Update(hatchData);
                        }
                    }

                    #endregion

                    #endregion
                }
                else if (company == "HYCL")
                {
                    #region HYCL

                    #region Delete Values

                    foreach (var item in listDelete)
                    {
                        ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                            new ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable();
                        ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                            new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                        hfdTA.FillByFlockData(hfdDT, company, region, location, Convert.ToDateTime(item.Key.Set_date),
                            hatchLoc, item.Key.Flock_id);

                        if (hfdDT.Count > 0)
                        {
                            ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                            hatchData.NUM_1 = 0;
                            hatchData.ACTUAL = 0;
                            hatchData.NUM_2 = 0;
                            hatchData.NUM_17 = 0;
                            hatchData.NUM_13 = 0;
                            hatchData.NUM_19 = 0;
                            hatchData.NUM_20 = 0;
                            hatchData.NUM_4 = 0;
                            hatchData.NUM_5 = 0;
                            hatchData.NUM_6 = 0;
                            hatchData.NUM_7 = 0;
                            hatchData.NUM_8 = 0;
                            hatchData.NUM_21 = 0;
                            hatchData.NUM_11 = 0;
                            hatchData.NUM_10 = 0;
                            hatchData.NUM_24 = 0;
                            hatchData.NUM_23 = 0;
                            hatchData.NUM_9 = 0;
                            hatchData.NUM_12 = 0;
                            hatchData.NUM_16 = 0;
                            hatchData.NUM_28 = 0;

                            hfdTA.Update(hatchData);
                        }
                    }

                    #endregion

                    #endregion
                }
                else if (company == "HYCO")
                {
                    #region HYCO

                    #region Delete Values

                    foreach (var item in listDelete)
                    {
                        ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                            new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable();
                        ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                            new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                        hfdTA.FillByFlockData(hfdDT, company, region, location, Convert.ToDateTime(item.Key.Set_date),
                            hatchLoc, item.Key.Flock_id);

                        for (int i = 0; i < hfdDT.Count; i++)
                        {
                            ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[i];
                            hatchData.NUM_1 = 0;
                            hatchData.ACTUAL = 0;
                            hatchData.NUM_2 = 0;
                            hatchData.NUM_17 = 0;
                            hatchData.NUM_13 = 0;
                            hatchData.NUM_19 = 0;
                            hatchData.NUM_20 = 0;
                            hatchData.NUM_4 = 0;
                            hatchData.NUM_5 = 0;
                            hatchData.NUM_6 = 0;
                            hatchData.NUM_7 = 0;
                            hatchData.NUM_8 = 0;
                            hatchData.NUM_21 = 0;
                            hatchData.NUM_11 = 0;
                            hatchData.NUM_10 = 0;
                            hatchData.NUM_24 = 0;
                            hatchData.NUM_23 = 0;
                            hatchData.NUM_9 = 0;
                            hatchData.NUM_12 = 0;
                            hatchData.NUM_16 = 0;
                            hatchData.NUM_28 = 0;

                            hfdTA.Update(hatchData);
                        }
                    }

                    #endregion

                    #endregion
                }

                #endregion

                #region Insert Values

                foreach (var item in listInsert)
                {
                    UpdateHatchingDataFLIP(company, region, location, hatchLoc, setDate,
                        item.Flock_id, item, "Insert");
                }

                #endregion
            }
        }

        #endregion

        #region BRFLOCKS

        #region SETTING EGGS

        public void AjustaEggInvFLIP(string incubatorio, string lote, DateTime dataProducao, decimal qtdeOvos, DateTime dataIncubacao)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
                    .Where(l => l.USERCodigoFLIP == incubatorio)
                    .FirstOrDefault();

            ImportaIncubacao.Data.FLIPDataSetTableAdapters.FLOCKSTableAdapter flocksServico = new ImportaIncubacao.Data.FLIPDataSetTableAdapters.FLOCKSTableAdapter();
            ImportaIncubacao.Data.FLIPDataSet flipDataSetServico = new ImportaIncubacao.Data.FLIPDataSet();
            EGGINV_DATATableAdapter eggInvData = new EGGINV_DATATableAdapter();
            ImportaIncubacao.Data.FLIPDataSetTableAdapters.EGGINV_DATATableAdapter eggInvDataServico = new ImportaIncubacao.Data.FLIPDataSetTableAdapters.EGGINV_DATATableAdapter();

            flocksServico.FillByFlockIDAndLocation(flipDataSetServico.FLOCKS, lote, locArmaz.USERGeracaoFLIP);
            string farmID = flipDataSetServico.FLOCKS[0].FARM_ID;
            string trackNO = "EXP" + dataProducao.ToString("yyMMdd");
            string flockIDComplete = farmID + "-" + lote;

            eggInvData.Delete("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, lote, trackNO, dataProducao,
                        "O", incubatorio);

            if (incubatorio != "NM" ||
                (incubatorio == "NM" && dataIncubacao >= Convert.ToDateTime("01/01/2017")))
            {
                int existIncubacaoWEB = hlbapp.HATCHERY_EGG_DATA
                    .Where(w => w.Company == "HYBR" && w.Region == "BR" && w.Location == locArmaz.USERGeracaoFLIP
                        && w.Hatch_loc == incubatorio && w.Set_date == dataIncubacao
                        && w.Flock_id == flockIDComplete && w.Lay_date == dataProducao)
                    .Count();

                //FLIPDataSet.HATCHERY_EGG_DATADataTable hedDT = new FLIPDataSet.HATCHERY_EGG_DATADataTable();
                //var existeIncFLIP = hatcheryEggData.FillGroupBySetDate(hedDT, "HYBR", "BR", locArmaz.USERGeracaoFLIP, dataIncubacao, incubatorio);
                //var existeIncLoteDataFLIP = hedDT.Where(w => w.FLOCK_ID == flockIDComplete && w.LAY_DATE == dataProducao).Count();

                if (existIncubacaoWEB == 0)
                {
                    #region Cadastro Correto EGG_INV

                    var lista = bdSQLServer.CTRL_LOTE_LOC_ARMAZ_WEB
                        .Where(c => c.Qtde > 0 && c.LoteCompleto == lote && c.DataProducao == dataProducao
                            && ((incubatorio == "CH" && !c.Local.Equals("SB") && !c.Local.Equals("PH")
                                    && !c.Local.Equals("TB") && !c.Local.Equals("NM")
                                    && !c.Local.Equals("PL"))
                                || (incubatorio == "PH" && (c.Local.Equals("SB") || c.Local.Equals("PH")))
                                || (incubatorio == "TB" && c.Local.Equals("TB"))
                                || (incubatorio == "NM" && (c.Local.Equals("NM") || c.Local.Equals("PL")
                                        || c.Local.Equals("T0") || c.Local.Equals("T1")
                                        || c.Local.Equals("T2")))))
                        .ToList();

                    foreach (var item in lista)
                    {
                        int existe = Convert.ToInt32(eggInvDataServico
                            .ScalarQueryOpen2(item.LoteCompleto, item.DataProducao, locArmaz.USERCodigoFLIP));

                        if (existe == 0)
                        {
                            decimal? qtd = item.Qtde;

                            eggInvDataServico.Insert("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, item.LoteCompleto,
                                trackNO, item.DataProducao, qtd, "O", null,
                                null, null, null, null, null, null, null, locArmaz.USERCodigoFLIP, null);
                        }
                        else
                        {
                            eggInvDataServico.FillByFlockLayDateStatus(flipDataSetServico.EGGINV_DATA,
                                item.LoteCompleto, "O", item.DataProducao);

                            var lista2 = flipDataSetServico.EGGINV_DATA.Where(e => e.LOCATION == locArmaz.USERGeracaoFLIP
                                && e.HATCH_LOC == locArmaz.USERCodigoFLIP).ToList();

                            foreach (var item2 in lista2)
                            {
                                decimal? qtd = item2.EGG_UNITS + item.Qtde;
                                eggInvDataServico.UpdateQueryEggs(qtd, "HYBR", "BR", locArmaz.USERGeracaoFLIP,
                                    farmID, item.LoteCompleto, trackNO, item.DataProducao, "O", locArmaz.USERCodigoFLIP);
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region Lançamento para Correção dos Dados caso haja incubação lançada e não tem mais o saldo no incubatório

                    int existe = Convert.ToInt32(eggInvDataServico
                            .ScalarQueryOpen2(lote, dataProducao, locArmaz.USERCodigoFLIP));

                    if (existe == 0)
                    {
                        decimal? qtd = qtdeOvos;

                        eggInvDataServico.Insert("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, lote,
                            trackNO, dataProducao, qtd, "O", null,
                            null, null, null, null, null, null, null, locArmaz.USERCodigoFLIP, null);
                    }
                    else
                    {
                        eggInvDataServico.FillByFlockLayDateStatus(flipDataSetServico.EGGINV_DATA,
                            lote, "O", dataProducao);

                        var lista2 = flipDataSetServico.EGGINV_DATA.Where(e => e.LOCATION == locArmaz.USERGeracaoFLIP
                            && e.HATCH_LOC == locArmaz.USERCodigoFLIP).ToList();

                        foreach (var item2 in lista2)
                        {
                            decimal? qtd = item2.EGG_UNITS + qtdeOvos;
                            eggInvDataServico.UpdateQueryEggs(qtd, "HYBR", "BR", locArmaz.USERGeracaoFLIP,
                                farmID, lote, trackNO, dataProducao, "O", locArmaz.USERCodigoFLIP);
                        }
                    }

                    #endregion
                }
            }
            else
            {
                #region Lançamento para Correção dos Dados da Planalto até 31/12/2016 - Solicitado por Davi Nogueira

                int existe = Convert.ToInt32(eggInvDataServico
                        .ScalarQueryOpen2(lote, dataProducao, locArmaz.USERCodigoFLIP));

                if (existe == 0)
                {
                    decimal? qtd = qtdeOvos;

                    eggInvDataServico.Insert("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, lote,
                        trackNO, dataProducao, qtd, "O", null,
                        null, null, null, null, null, null, null, locArmaz.USERCodigoFLIP, null);
                }
                else
                {
                    eggInvDataServico.FillByFlockLayDateStatus(flipDataSetServico.EGGINV_DATA,
                        lote, "O", dataProducao);

                    var lista2 = flipDataSetServico.EGGINV_DATA.Where(e => e.LOCATION == locArmaz.USERGeracaoFLIP
                        && e.HATCH_LOC == locArmaz.USERCodigoFLIP).ToList();

                    foreach (var item2 in lista2)
                    {
                        decimal? qtd = item2.EGG_UNITS + qtdeOvos;
                        eggInvDataServico.UpdateQueryEggs(qtd, "HYBR", "BR", locArmaz.USERGeracaoFLIP,
                            farmID, lote, trackNO, dataProducao, "O", locArmaz.USERCodigoFLIP);
                    }
                }

                #endregion
            }
        }

        public bool UpdateSetFLIPBR(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits, string machine, string trackNO,
            decimal estimate, string obs)
        {
            bool imported = false;

            AjustaEggInvFLIP(hatchLoc, flockID, layDate, eggUnits, setDate);

            decimal existe = Convert.ToDecimal(setDayData.ExisteSetDayData(setDate, hatchLoc));

            if (existe == 0)
            {
                decimal sequencia = Convert.ToDecimal(setDayData.UltimaSequenciaSetDayData(hatchLoc)) + 1;

                setDayData.InsertQuery(company, region, location, setDate, hatchLoc, sequencia);
            }

            existe = 0;

            // Insere / Atualiza Incubação
            existe = ExistsHatcheryEggDataAll(company, region, location, setDate, hatchLoc, farmID + "-" + flockID,
                layDate, machine, trackNO);

            if (existe > 0)
            {
                //eggUnits = eggUnits + GetQtySettedEggs(company, region, location,
                //    setDate, hatchLoc, farmID + "-" + flockID, layDate, machine, trackNO);
                hatcheryEggData.Delete(company, region, location, setDate, hatchLoc, farmID + "-" + flockID,
                    layDate, machine, trackNO);
            }

            existe = 0;

            // Verifica se existe Dados do Nascimento
            existe = Convert.ToDecimal(hatcheryFlockData.ExisteHatcheryFlockData(company, region,
                location, setDate, hatchLoc, farmID + "-" + flockID));

            if (existe == 0)
            {
                hatcheryFlockData.InsertQuery(company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID, estimate);
            }
            // 14/08/2014 - Ocorrência 99 - APONTES
            // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
            // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
            // o trigger de atualização da idade executar.
            else
            {
                decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(hatchLoc, setDate,
                    farmID + "-" + flockID);

                hatcheryFlockData.UpdateEstimate(mediaIncubacao, company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID);
            }

            if (eggUnits > 0)
            {
                hatcheryEggData.Insert(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, layDate,
                    eggUnits, "", machine, trackNO, null, null, null, null, null, null, null, null, obs,
                    "servico");

                imported = true;
            }

            return imported;
        }

        #endregion

        #endregion

        #region CLFLOCKS

        #region FLOCKS DATA

        public void InsertProductionCLFLOCKS(string company, string region)
        {
            #region Initialize SQL Database

            HLBAPPEntities bdSQL = new HLBAPPEntities();
            bdSQL.CommandTimeout = 10000;

            #endregion

            #region Initialize Oracle Database

            CLFLOCKS cl = new CLFLOCKS();
            Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                new Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter();
            Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                new Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter();
            fTA.Fill(cl.FLOCKS);
            Data.FLIP.CLFLOCKSTableAdapters.FARMSTableAdapter fmTA =
                new Data.FLIP.CLFLOCKSTableAdapters.FARMSTableAdapter();
            fmTA.Fill(cl.FARMS);

            #endregion

            try
            {
                fdTA.FillByImported(cl.FLOCK_DATA, 0, company, region);

                if (cl.FLOCK_DATA.Count > 0)
                {
                    #region Atualiza FLOCK_DATA WEB

                    #region Carrega Variáveis e Objetos

                    DateTime dataAtual;

                    string linhaAtual = "";

                    #endregion

                    for (int i = 0; i < cl.FLOCK_DATA.Count; i++)
                    {
                        var flock = cl.FLOCKS
                            .Where(w => w.FLOCK_KEY == cl.FLOCK_DATA[i].FLOCK_KEY)
                            .FirstOrDefault();
                        dataAtual = cl.FLOCK_DATA[i].TRX_DATE;
                        linhaAtual = flock.VARIETY;

                        if (!ExistsEggInvClosedCLFLOCKS(dataAtual, "Granjas"))
                        {
                            string flockID = cl.FLOCK_DATA[i].FLOCK_ID;
                            DateTime trxDate = cl.FLOCK_DATA[i].TRX_DATE;
                            string flockKey = cl.FLOCK_DATA[i].FLOCK_KEY;

                            decimal quantidade = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_1Null())
                                quantidade = cl.FLOCK_DATA[i].NUM_1;
                            string numLote = flock.NUM_1.ToString();

                            int age = Convert.ToInt32(cl.FLOCK_DATA[i].AGE);
                            int henMort = 0;
                            if (!cl.FLOCK_DATA[i].IsHEN_MORTNull())
                                henMort = Convert.ToInt32(cl.FLOCK_DATA[i].HEN_MORT);
                            int henWt = 0;
                            if (!cl.FLOCK_DATA[i].IsHEN_WTNull())
                                henWt = Convert.ToInt32(cl.FLOCK_DATA[i].HEN_WT);
                            int maleMort = 0;
                            if (!cl.FLOCK_DATA[i].IsMALE_MORTNull())
                                maleMort = Convert.ToInt32(cl.FLOCK_DATA[i].MALE_MORT);
                            decimal henFeedDel = 0;
                            if (!cl.FLOCK_DATA[i].IsHEN_FEED_DELNull())
                                henFeedDel = Convert.ToDecimal(cl.FLOCK_DATA[i].HEN_FEED_DEL);
                            int totalEggsProd = 0;
                            if (!cl.FLOCK_DATA[i].IsTOTAL_EGGS_PRODNull())
                                totalEggsProd = Convert.ToInt32(cl.FLOCK_DATA[i].TOTAL_EGGS_PROD);
                            decimal eggWt = 0;
                            if (!cl.FLOCK_DATA[i].IsEGG_WTNull())
                                eggWt = Convert.ToDecimal(cl.FLOCK_DATA[i].EGG_WT);
                            int hatchEggs = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_1Null())
                                hatchEggs = Convert.ToInt32(cl.FLOCK_DATA[i].NUM_1);
                            string comentarios = "";
                            if (!cl.FLOCK_DATA[i].IsTEXT_2Null())
                                comentarios = cl.FLOCK_DATA[i].TEXT_2;
                            int numGalpao = 0;
                            if (!flock.IsNUM_2Null())
                                numGalpao = Convert.ToInt32(flock.NUM_2);

                            #region 20/04/2017 - Variáveis novas para WebService LTZ

                            #region Farm Name

                            string farmName = cl.FARMS
                                .Where(w => w.FARM_KEY == flock.FARM_KEY).FirstOrDefault().FARM_NAME;

                            #endregion

                            int count_females = ACMFEMINVCLFLOCKS(trxDate, flockID);
                            int count_males = ACMMALEINVCLFLOCKS(trxDate, flockID);
                            int broken = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_11Null())
                                broken = Convert.ToInt32(cl.FLOCK_DATA[i].NUM_12);
                            int dirty = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_10Null())
                                dirty = Convert.ToInt32(cl.FLOCK_DATA[i].NUM_10);
                            int consume = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_22Null())
                                consume = Convert.ToInt32(cl.FLOCK_DATA[i].NUM_22);
                            int floor = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_18Null())
                                floor = Convert.ToInt32(cl.FLOCK_DATA[i].NUM_18);
                            int destroyed = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_13Null())
                                destroyed = Convert.ToInt32(cl.FLOCK_DATA[i].NUM_13);
                            decimal water_consumption = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_2Null())
                                water_consumption = Convert.ToDecimal(cl.FLOCK_DATA[i].NUM_2);
                            decimal uniformity = 0;
                            if (!cl.FLOCK_DATA[i].IsNUM_7Null())
                                uniformity = Convert.ToDecimal(cl.FLOCK_DATA[i].NUM_7);

                            #endregion

                            #region Delete DEO Hatching Eggs if exists

                            string farm = cl.FLOCK_DATA[i].FARM_ID;
                            var deleteDEO = bdSQL.LayoutDiarioExpedicaos
                                .Where(w => w.Nucleo == farm
                                    && w.LoteCompleto == flockID
                                    && w.DataProducao == trxDate
                                    && w.TipoDEO == "Ovos Incubáveis")
                                .FirstOrDefault();

                            int hatchEggsOld = 0;
                            if (deleteDEO != null)
                            {
                                hatchEggsOld = Convert.ToInt32(deleteDEO.QtdeOvos);
                                if (hatchEggs == 0)
                                {
                                    bdSQL.LayoutDiarioExpedicaos.DeleteObject(deleteDEO);
                                    bdSQL.SaveChanges();
                                }
                            }

                            #endregion

                            #region Update DEO by Hatching Eggs automatically if old qty < new qty

                            LayoutDiarioExpedicaos deo = null;
                            if (hatchEggs > 0 && hatchEggsOld > hatchEggs)
                            {
                                var farm02 = farm.Substring(0, 2);
                                CTRL_LOTE_LOC_ARMAZ_WEB eggInv = bdSQL.CTRL_LOTE_LOC_ARMAZ_WEB
                                   .Where(w => w.Local == "EM"
                                            && w.LoteCompleto == flockID
                                            && w.DataProducao == trxDate)
                                    .FirstOrDefault();

                                var eggInvValue = 0;
                                if (eggInv != null) eggInvValue = (int)eggInv.Qtde;

                                if (eggInvValue == 0)
                                {
                                    #region Delete / Update DEO Sorting Eggs to Balance

                                    var sortingEggsList = bdSQL.LayoutDiarioExpedicaos
                                        .Where(w => w.Incubatorio == "EM"
                                                && w.LoteCompleto == flockID
                                                && w.DataProducao == trxDate
                                                && w.TipoDEO == "Classificação de Ovos")
                                        .OrderByDescending(o => o.QtdeOvos)
                                        .ToList();

                                    int difference = Math.Abs(hatchEggsOld - hatchEggs);
                                    int balance = difference;
                                    foreach (var item in sortingEggsList)
                                    {
                                        if (balance >= item.QtdeOvos)
                                        {
                                            bdSQL.LayoutDiarioExpedicaos.DeleteObject(item);
                                            balance = balance - (int)item.QtdeOvos;
                                        }
                                        else
                                        {
                                            if (balance > 0)
                                            {
                                                item.QtdeOvos = item.QtdeOvos - balance;
                                                balance = 0;
                                            }
                                        }
                                    }

                                    bdSQL.SaveChanges();

                                    #endregion
                                }

                                deo = InsertDEOHatchingEggs(
                                    cl.FLOCK_DATA[i].FARM_ID.Substring(0, 2),
                                    cl.FLOCK_DATA[i].FARM_ID, flockID, Convert.ToDecimal(numLote),
                                    flock.NUM_2, flock.VARIETY, age, trxDate, hatchEggs, "EM");
                            }

                            #endregion

                            ImportaDiarioProducaoWEB(
                                    company,
                                    region,
                                    cl.FLOCK_DATA[i].FARM_ID,
                                    flockID,
                                    numLote,
                                    flock.VARIETY,
                                    Convert.ToInt32(cl.FLOCK_DATA[i].ACTIVE),
                                    age,
                                    trxDate,
                                    henMort,
                                    henWt,
                                    maleMort,
                                    henFeedDel,
                                    totalEggsProd,
                                    eggWt,
                                    hatchEggs,
                                    comentarios,
                                    count_females,
                                    count_males,
                                    broken,
                                    dirty,
                                    consume,
                                    floor,
                                    destroyed,
                                    water_consumption,
                                    uniformity,
                                    farmName,
                                    numGalpao);

                            #region Insert DEO by Hatching Eggs automatically

                            if (hatchEggs > 0)
                            {
                                #region Update DEO by Hatching Eggs automatically if old qty > new qty

                                if (hatchEggsOld < hatchEggs)
                                {
                                    deo = InsertDEOHatchingEggs(
                                        cl.FLOCK_DATA[i].FARM_ID.Substring(0, 2),
                                        cl.FLOCK_DATA[i].FARM_ID, flockID, Convert.ToDecimal(numLote),
                                        flock.NUM_2, flock.VARIETY, age, trxDate, hatchEggs, "EM");
                                }

                                #endregion

                                if (deo != null)
                                    if (deo.ID == 0) bdSQL.LayoutDiarioExpedicaos.AddObject(deo);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Atualiza Núcleo, Idade e Média das Últimas 4 Semanas

                    for (int i = 0; i < cl.FLOCK_DATA.Count; i++)
                    {
                        string flockKey = cl.FLOCK_DATA[i].FLOCK_KEY;

                        dataAtual = cl.FLOCK_DATA[i].TRX_DATE;

                        DateTime dataPrd = cl.FLOCK_DATA[i].TRX_DATE;
                        string flockID = cl.FLOCK_DATA[i].FLOCK_ID;

                        string flockIDHatch = cl.FLOCK_DATA[i].FARM_ID + "-" + flockID;

                        CTRL_LOTE_LOC_ARMAZ_WEB tabSaldo = bdSQL.CTRL_LOTE_LOC_ARMAZ_WEB
                            .Where(w => w.LoteCompleto == flockID && w.DataProducao == dataPrd)
                            .FirstOrDefault();

                        if (tabSaldo != null)
                        {
                            tabSaldo.Nucleo = cl.FLOCK_DATA[i].FARM_ID;
                            tabSaldo.IdadeLote = (short)cl.FLOCK_DATA[i].AGE;
                            tabSaldo.PercMediaIncUlt4SemFLIP =
                                AVG_LST4WK_HATCHCLFLOCKS(cl.FLOCK_DATA[0].COMPANY, flockIDHatch);
                        }

                        fdTA.RefreshImported(1, dataAtual, flockKey);
                    }

                    bdSQL.SaveChanges();

                    #endregion

                    #region Rotina Atualiza Estoque caso houve deleção no FLIP

                    DateTime dataInicial = DateTime.Today.AddDays(-60);
                    DateTime dataFinal = DateTime.Today;
                    var listaImportadosWEB = bdSQLServer.FLOCK_DATA
                        .Where(w => w.Trx_Date >= dataInicial && w.Trx_Date <= dataFinal
                            && w.Company == company && w.Region == region).ToList();

                    foreach (var item in listaImportadosWEB)
                    {
                        CLFLOCKS.FLOCK_DATADataTable flock = new CLFLOCKS.FLOCK_DATADataTable();

                        fdTA.FillByFlockTrxDate(flock, item.Flock_ID, Convert.ToDateTime(item.Trx_Date));

                        if (flock.Count == 0)
                        {
                            DeletaDiarioProducaoWEB(item.Flock_ID, Convert.ToDateTime(item.Trx_Date));
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Exceção

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpo = "Erro ao Importar Diário da Granja do Chile: " + ex.Message;
                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        corpo = (char)10 + (char)13 + corpo + ex.InnerException.Message;

                EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR DIARIO DA GRANJA - CHILE ****", "Paulo Alves",
                    "palves@hyline.com.br", "", "");

                this.EventLog.WriteEntry(corpo, EventLogEntryType.Error, 10);

                #endregion
            }
        }

        public bool ExistsEggInvClosedCLFLOCKS(DateTime dataMov, string filtroLocal)
        {
            CLFLOCKS.DATA_FECH_LANCDataTable DfDT = new CLFLOCKS.DATA_FECH_LANCDataTable();
            Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter DfTA =
                new Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
            DfTA.Fill(DfDT);

            if (DfDT.Count > 0)
            {
                CLFLOCKS.DATA_FECH_LANCRow DfRow = DfDT.Where(w => w.DATA_FECH_LANC >= dataMov
                    && w.LOCATION == filtroLocal)
                    .FirstOrDefault();

                if (DfRow != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public int ACMFEMINVCLFLOCKS(DateTime trxDate, string flockID)
        {
            int result = 0;

            #region Hens Moved

            Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                new Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter();
            CLFLOCKS.FLOCKSRow fR = fTA.GetDataByFlockID(flockID)[0];
            int hensMoved = 0;
            if (!fR.IsHENS_MOVEDNull()) hensMoved = Convert.ToInt32(fR.HENS_MOVED);

            #endregion

            #region Acum Mortality

            Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                new Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter();
            int acmHenMort = Convert.ToInt32(fdTA.AcmHenMort(flockID, trxDate.AddDays(-1)));

            result = hensMoved - acmHenMort;

            #endregion

            return result;
        }

        public int ACMMALEINVCLFLOCKS(DateTime trxDate, string flockID)
        {
            int result = 0;

            #region Males Moved

            Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                new Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter();
            CLFLOCKS.FLOCKSRow fR = fTA.GetDataByFlockID(flockID)[0];
            int malesMoved = 0;
            if (!fR.IsMALES_MOVEDNull()) malesMoved = Convert.ToInt32(fR.MALES_MOVED);

            #endregion

            #region Acum Mortality

            Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                new Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter();
            int acmMaleMort = Convert.ToInt32(fdTA.AcmMaleMort(flockID, trxDate.AddDays(-1)));

            result = malesMoved - acmMaleMort;

            #endregion

            return result;
        }

        public decimal AVG_LST4WK_HATCHCLFLOCKS(string comp, string flockid)
        {
            CLFLOCKS cl = new CLFLOCKS();

            Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hTA =
                new Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
            int age = (int)hTA.MaxAge(comp, flockid);

            Data.FLIP.CLFLOCKSTableAdapters.AVG_LST4WK_HATCHTableAdapter avgTA =
                new Data.FLIP.CLFLOCKSTableAdapters.AVG_LST4WK_HATCHTableAdapter();
            avgTA.FillAVG_LST4WK_HATCH(cl.AVG_LST4WK_HATCH, comp, flockid, age);

            decimal ihatch = cl.AVG_LST4WK_HATCH[0].ACTUAL;
            decimal ircv = cl.AVG_LST4WK_HATCH[0].EGGSRCVD;
            decimal idirts = cl.AVG_LST4WK_HATCH[0].DIRTS;

            decimal iavg = 0;
            if (ircv != 0)
                iavg = (ihatch / (ircv - idirts)) * 100.0m;

            return iavg;
        }

        #endregion

        #region SETTING EGGS

        public bool UpdateSetFLIPCL(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits, string machine, string trackNO,
            decimal estimate, string obs)
        {
            bool imported = false;

            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.SETDAY_DATATableAdapter sTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.SETDAY_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();

            decimal existe = Convert.ToDecimal(sTA.ExistsSetDayData(setDate, hatchLoc));

            if (existe == 0)
            {
                decimal sequencia = Convert.ToDecimal(sTA.LastSequenceByHatchLoc(hatchLoc)) + 1;
                sTA.InsertQuery(company, region, location, setDate, hatchLoc, sequencia);
            }

            existe = 0;

            // Insere / Atualiza Incubação
            existe = ExistsHatcheryEggDataAll(company, region, location, setDate, hatchLoc,
                farmID + "-" + flockID, layDate, machine, trackNO);

            if (existe > 0)
            {
                //eggUnits = eggUnits + GetQtySettedEggs(company, region, location,
                //    setDate, hatchLoc, farmID + "-" + flockID, layDate, machine, trackNO);
                hedTA.Delete(company, region, location, setDate, hatchLoc, farmID + "-" + flockID,
                    layDate, machine, trackNO);
            }

            existe = 0;

            // Verifica se existe Dados do Nascimento
            existe = Convert.ToDecimal(hfdTA.ExistsHFD(company, region, location, setDate, hatchLoc,
                farmID + "-" + flockID));

            if (existe == 0)
            {
                hfdTA.InsertQuery(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, estimate);
            }
            // 14/08/2014 - Ocorrência 99 - APONTES
            // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
            // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
            // o trigger de atualização da idade executar.
            else
            {
                decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(hatchLoc, setDate,
                    farmID + "-" + flockID);

                hfdTA.UpdateEstimate(mediaIncubacao, company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID);
            }

            if (eggUnits > 0)
            {
                hedTA.Insert(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, layDate,
                    eggUnits, "", machine, trackNO, null, null, null, null, null, null, null, null, obs,
                    "servico");

                imported = true;
            }

            return imported;
        }

        #endregion

        #endregion

        #region HCFLOCKS

        #region FLOCKS DATA

        public void InsertProductionHCFLOCKS(string company, string region)
        {
            #region Initialize SQL Database

            HLBAPPEntities bdSQL = new HLBAPPEntities();
            bdSQL.CommandTimeout = 10000;

            #endregion

            #region Initialize Oracle Database

            HCFLOCKS hc = new HCFLOCKS();
            Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                new Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
            Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                new Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter();
            fTA.Fill(hc.FLOCKS);
            Data.FLIP.HCFLOCKSTableAdapters.FARMSTableAdapter fmTA =
                new Data.FLIP.HCFLOCKSTableAdapters.FARMSTableAdapter();
            fmTA.Fill(hc.FARMS);

            #endregion

            try
            {
                fdTA.FillByImported(hc.FLOCK_DATA, 0, company, region);

                if (hc.FLOCK_DATA.Count > 0)
                {
                    var listaAgrupaLote = hc.FLOCK_DATA
                        .GroupBy(g => new
                        {
                            g.FARM_ID,
                            flock = g.FLOCK_ID.Substring(0, 6) + g.FLOCK_ID.Substring(7, 3),
                            g.TRX_DATE,
                            g.AGE
                        })
                        .ToList();

                    #region Atualiza FLOCK_DATA WEB

                    #region Carrega Variáveis e Objetos

                    DateTime dataAtual;

                    string hatchLoc = "";
                    if (region == "CO")
                        hatchLoc = "PM";
                    else
                        hatchLoc = "MN";

                    #endregion

                    foreach (var lote in listaAgrupaLote)
                    {
                        var flock = hc.FLOCKS
                            .Where(w => 
                                w.FLOCK_ID.Substring(0, 6) + w.FLOCK_ID.Substring(7, 3) == lote.Key.flock)
                            .FirstOrDefault();
                        dataAtual = lote.Key.TRX_DATE;

                        if (!ExistsEggInvClosedHCFLOCKS(dataAtual, "Granjas"))
                        {
                            string flockID = lote.Key.flock;
                            DateTime trxDate = lote.Key.TRX_DATE;

                            Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT = new HCFLOCKS.FLOCK_DATADataTable();
                            fdTA.FillByFlockAndTrxDate(fdDT, flockID, trxDate);

                            //var listaLotes = hc.FLOCK_DATA
                            //    .Where(w => w.FARM_ID == lote.Key.FARM_ID
                            //        && w.FLOCK_ID.Substring(0, 6) + w.FLOCK_ID.Substring(7, 3) == lote.Key.flock
                            //        && w.TRX_DATE == trxDate)
                            //    .ToList();
                            var listaLotes = fdDT.ToList();

                            string numLote = flock.NUM_1.ToString();

                            int age = Convert.ToInt32(lote.Key.AGE);
                            int henMort = Convert.ToInt32(listaLotes.Sum(s => s.IsHEN_MORTNull() ? 0 : s.HEN_MORT));
                            int henWt = Convert.ToInt32(listaLotes.Average(s => s.IsHEN_WTNull() ? 0 : s.HEN_WT));
                            int maleMort = Convert.ToInt32(listaLotes.Sum(s => s.IsMALE_MORTNull() ? 0 : s.MALE_MORT));
                            int henFeedDel = Convert.ToInt32(listaLotes.Sum(s => s.IsHEN_FEED_DELNull() ? 0 : s.HEN_FEED_DEL));
                            int totalEggsProd = Convert.ToInt32(listaLotes.Sum(s => s.IsTOTAL_EGGS_PRODNull() ? 0 : s.TOTAL_EGGS_PROD));
                            int eggWt = Convert.ToInt32(listaLotes.Average(s => s.IsEGG_WTNull() ? 0 : s.EGG_WT));
                            int hatchEggs = Convert.ToInt32(listaLotes.Sum(s => s.IsNUM_1Null() ? 0 : s.NUM_1));
                            string comentarios = listaLotes.FirstOrDefault().IsTEXT_2Null() ? "" : listaLotes.FirstOrDefault().TEXT_2;
                            int numGalpao = 0;

                            #region 20/04/2017 - Variáveis novas para WebService LTZ

                            #region Farm Name

                            string farmName = hc.FARMS.Where(w => w.FARM_KEY == flock.FARM_KEY).FirstOrDefault().FARM_NAME;

                            #endregion

                            int count_females = 0;
                            foreach (var item in listaLotes) count_females = count_females + ACMFEMINVHCFLOCKS(trxDate, item.FLOCK_ID);
                            int count_males = 0;
                            foreach (var item in listaLotes) count_males = count_males + ACMMALEINVHCFLOCKS(trxDate, item.FLOCK_ID);
                            int broken = Convert.ToInt32(listaLotes.Sum(s => s.IsNUM_12Null() ? 0 : s.NUM_12));
                            int dirty = Convert.ToInt32(listaLotes.Sum(s => s.IsNUM_10Null() ? 0 : s.NUM_10));
                            int consume = Convert.ToInt32(listaLotes.Sum(s => s.IsNUM_22Null() ? 0 : s.NUM_22));
                            int floor = Convert.ToInt32(listaLotes.Sum(s => s.IsNUM_18Null() ? 0 : s.NUM_18));
                            int destroyed = Convert.ToInt32(listaLotes.Sum(s => s.IsNUM_13Null() ? 0 : s.NUM_13));
                            decimal water_consumption = Convert.ToInt32(listaLotes.Sum(s => s.IsNUM_2Null() ? 0 : s.NUM_2));
                            decimal uniformity = Convert.ToInt32(listaLotes.Average(s => s.IsNUM_7Null() ? 0 : s.NUM_7));
                            string farm = lote.Key.FARM_ID;

                            #endregion

                            #region Delete DEO Hatching Eggs if exists

                            var deleteDEO = bdSQL.LayoutDiarioExpedicaos
                                .Where(w => w.Nucleo == farm
                                    && w.LoteCompleto == flockID
                                    && w.DataProducao == trxDate
                                    && w.TipoDEO == "Ovos Incubáveis")
                                .FirstOrDefault();

                            int hatchEggsOld = 0;
                            if (deleteDEO != null)
                            {
                                hatchEggsOld = Convert.ToInt32(deleteDEO.QtdeOvos);
                                if (hatchEggs == 0)
                                {
                                    bdSQL.LayoutDiarioExpedicaos.DeleteObject(deleteDEO);
                                    bdSQL.SaveChanges();
                                }
                            }

                            #endregion

                            #region Update DEO by Hatching Eggs automatically if old qty < new qty

                            LayoutDiarioExpedicaos deo = null;
                            if (hatchEggs > 0 && hatchEggsOld > hatchEggs)
                            {
                                deo = InsertDEOHatchingEggs(
                                    farm.Substring(0, 2),
                                    farm, flockID, Convert.ToDecimal(numLote),
                                    flock.NUM_2, flock.VARIETY, age, trxDate, hatchEggs, hatchLoc);
                            }

                            #endregion

                            ImportaDiarioProducaoWEB(
                                    company,
                                    region,
                                    farm,
                                    flockID,
                                    numLote,
                                    flock.VARIETY,
                                    Convert.ToInt32(listaLotes.Max(m => m.ACTIVE)),
                                    age,
                                    trxDate,
                                    henMort,
                                    henWt,
                                    maleMort,
                                    henFeedDel,
                                    totalEggsProd,
                                    eggWt,
                                    hatchEggs,
                                    comentarios,
                                    count_females,
                                    count_males,
                                    broken,
                                    dirty,
                                    consume,
                                    floor,
                                    destroyed,
                                    water_consumption,
                                    uniformity,
                                    farmName,
                                    numGalpao);

                            #region Insert DEO by Hatching Eggs automatically

                            if (hatchEggs > 0)
                            {
                                #region Update DEO by Hatching Eggs automatically if old qty > new qty

                                if (hatchEggsOld < hatchEggs)
                                {
                                    deo = InsertDEOHatchingEggs(
                                        farm.Substring(0, 2),
                                        farm, flockID, Convert.ToDecimal(numLote),
                                        flock.NUM_2, flock.VARIETY, age, trxDate, hatchEggs, hatchLoc);
                                }

                                #endregion

                                if (deo != null)
                                    if (deo.ID == 0) bdSQL.LayoutDiarioExpedicaos.AddObject(deo);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Atualiza Núcleo, Idade e Média das Últimas 4 Semanas

                    foreach (var lote in listaAgrupaLote)
                    {
                        DateTime dataPrd = lote.Key.TRX_DATE;
                        string flockID = lote.Key.flock;
                        string flockIDHatch = lote.Key.FARM_ID + "-" + lote.Key.flock;

                        CTRL_LOTE_LOC_ARMAZ_WEB tabSaldo = bdSQL.CTRL_LOTE_LOC_ARMAZ_WEB
                            .Where(w => w.LoteCompleto == flockID && w.DataProducao == dataPrd)
                            .FirstOrDefault();

                        if (tabSaldo != null)
                        {
                            tabSaldo.Nucleo = lote.Key.FARM_ID;
                            tabSaldo.IdadeLote = (short)lote.Key.AGE;
                            tabSaldo.PercMediaIncUlt4SemFLIP =
                                AVG_LST4WK_HATCHHCFLOCKS(company, flockIDHatch);
                        }

                        var listaLotes = hc.FLOCK_DATA
                            .Where(w => w.FARM_ID == lote.Key.FARM_ID
                                &&w.FLOCK_ID.Substring(0, 6) + w.FLOCK_ID.Substring(7, 3) == lote.Key.flock
                                && w.TRX_DATE == lote.Key.TRX_DATE)
                            .ToList();

                        foreach (var item in listaLotes)
                        {
                            fdTA.RefreshImported(1, dataPrd, item.FLOCK_KEY);
                        }
                    }

                    bdSQL.SaveChanges();

                    #endregion

                    #region Rotina Atualiza Estoque caso houve deleção no FLIP

                    DateTime dataInicial = DateTime.Today.AddDays(-60);
                    DateTime dataFinal = DateTime.Today;
                    var listaImportadosWEB = bdSQLServer.FLOCK_DATA
                        .Where(w => w.Trx_Date >= dataInicial && w.Trx_Date <= dataFinal
                            && w.Company == company && w.Region == region).ToList();

                    foreach (var item in listaImportadosWEB)
                    {
                        HCFLOCKS.FLOCK_DATADataTable flock = new HCFLOCKS.FLOCK_DATADataTable();

                        fdTA.FillByFlockAndTrxDate(flock, item.Flock_ID, Convert.ToDateTime(item.Trx_Date));

                        if (flock.Count == 0)
                        {
                            DeletaDiarioProducaoWEB(item.Flock_ID, Convert.ToDateTime(item.Trx_Date));
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Exceção

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpo = "Erro ao Importar Diário da Granja da Colombia: " + ex.Message;
                if (ex.InnerException != null)
                    if (ex.InnerException.Message != null)
                        corpo = (char)10 + (char)13 + corpo + ex.InnerException.Message;

                corpo = corpo + (char)10 + (char)13 + (char)10 + (char)13 + "Linha de erro no código: " + linenum.ToString();

                EnviarEmail(corpo, "**** ERRO SERVICO IMPORTAR DIARIO DA GRANJA - COLOMBIA - " + region + " ****", "Paulo Alves",
                    "palves@hyline.com.br", "", "");

                this.EventLog.WriteEntry(corpo, EventLogEntryType.Error, 10);

                #endregion
            }
        }

        public bool ExistsEggInvClosedHCFLOCKS(DateTime dataMov, string filtroLocal)
        {
            HCFLOCKS.DATA_FECH_LANCDataTable DfDT = new HCFLOCKS.DATA_FECH_LANCDataTable();
            Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter DfTA =
                new Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
            DfTA.Fill(DfDT);

            if (DfDT.Count > 0)
            {
                HCFLOCKS.DATA_FECH_LANCRow DfRow = DfDT.Where(w => w.DATA_FECH_LANC >= dataMov
                    && w.LOCATION == filtroLocal)
                    .FirstOrDefault();

                if (DfRow != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public int ACMFEMINVHCFLOCKS(DateTime trxDate, string flockID)
        {
            int result = 0;

            #region Hens Moved

            Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                new Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter();
            HCFLOCKS.FLOCKSRow fR = fTA.GetDataByFlockID(flockID)[0];
            int hensMoved = 0;
            if (!fR.IsHENS_MOVEDNull()) hensMoved = Convert.ToInt32(fR.HENS_MOVED);

            #endregion

            #region Acum Mortality

            Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                new Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
            int acmHenMort = Convert.ToInt32(fdTA.AcmHenMort(flockID, trxDate.AddDays(-1)));

            result = hensMoved - acmHenMort;

            #endregion

            return result;
        }

        public int ACMMALEINVHCFLOCKS(DateTime trxDate, string flockID)
        {
            int result = 0;

            #region Males Moved

            Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                new Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter();
            HCFLOCKS.FLOCKSRow fR = fTA.GetDataByFlockID(flockID)[0];
            int malesMoved = 0;
            if (!fR.IsMALES_MOVEDNull()) malesMoved = Convert.ToInt32(fR.MALES_MOVED);

            #endregion

            #region Acum Mortality

            Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                new Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
            int acmMaleMort = Convert.ToInt32(fdTA.AcmMaleMort(flockID, trxDate.AddDays(-1)));

            result = malesMoved - acmMaleMort;

            #endregion

            return result;
        }

        public decimal AVG_LST4WK_HATCHHCFLOCKS(string comp, string flockid)
        {
            HCFLOCKS hc = new HCFLOCKS();

            Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hTA =
                new Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
            int age = (int)hTA.MaxAge(comp, flockid);

            Data.FLIP.HCFLOCKSTableAdapters.AVG_LST4WK_HATCHTableAdapter avgTA =
                new Data.FLIP.HCFLOCKSTableAdapters.AVG_LST4WK_HATCHTableAdapter();
            avgTA.FillAVG_LST4WK_HATCH(hc.AVG_LST4WK_HATCH, comp, flockid, age);

            decimal ihatch = hc.AVG_LST4WK_HATCH[0].ACTUAL;
            decimal ircv = hc.AVG_LST4WK_HATCH[0].EGGSRCVD;
            decimal idirts = hc.AVG_LST4WK_HATCH[0].DIRTS;

            decimal iavg = 0;
            if (ircv != 0)
                iavg = (ihatch / (ircv - idirts)) * 100.0m;

            return iavg;
        }

        #endregion

        #region SETTING EGGS

        public bool UpdateSetFLIPHC(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits,
            string machine, string trackNO, decimal estimate, string obs)
        {
            bool imported = false;

            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.SETDAY_DATATableAdapter sTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.SETDAY_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();

            decimal existe = Convert.ToDecimal(sTA.ExistsSetDayData(setDate, hatchLoc));

            if (existe == 0)
            {
                decimal sequencia = Convert.ToDecimal(sTA.LastSequenceByHatchLoc(hatchLoc)) + 1;
                sTA.InsertQuery(company, region, location, setDate, hatchLoc, sequencia);
            }

            existe = 0;

            // Insere / Atualiza Incubação
            existe = ExistsHatcheryEggDataAll(company, region, location, setDate, hatchLoc,
                farmID + "-" + flockID, layDate, machine, trackNO);

            if (existe > 0)
            {
                //eggUnits = eggUnits + GetQtySettedEggs(company, region, location,
                //    setDate, hatchLoc, farmID + "-" + flockID, layDate, machine, trackNO);
                hedTA.Delete(company, region, location, setDate, hatchLoc, farmID + "-" + flockID,
                    layDate, machine, trackNO);
            }

            existe = 0;

            // Verifica se existe Dados do Nascimento
            existe = Convert.ToDecimal(hfdTA.ExistsHFD(company, region, location, setDate, hatchLoc,
                farmID + "-" + flockID));

            if (existe == 0)
            {
                hfdTA.InsertQuery(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, estimate);
            }
            // 14/08/2014 - Ocorrência 99 - APONTES
            // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
            // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
            // o trigger de atualização da idade executar.
            else
            {
                decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(hatchLoc, setDate,
                    farmID + "-" + flockID);

                hfdTA.UpdateEstimate(mediaIncubacao, company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID);
            }

            if (eggUnits > 0)
            {
                hedTA.Insert(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, layDate,
                    eggUnits, "", machine, trackNO, null, null, null, null, null, null, null, null, obs,
                    "sistema");

                imported = true;
            }

            return imported;
        }

        #endregion

        #endregion

        #region WEB

        public LayoutDiarioExpedicaos InsertDEOHatchingEggs(string farm, string farmID, string flockID,
            decimal flockNumber, decimal shed, string variety, int age, DateTime trxDate, decimal qtyEggs,
            string hatchLoc)
        {
            HLBAPPEntities bdSQL = new HLBAPPEntities();
            bdSQL.CommandTimeout = 10000;

            LayoutDiarioExpedicaos deo = bdSQL.LayoutDiarioExpedicaos
                .Where(w => w.Nucleo == farmID && w.LoteCompleto == flockID
                    && w.DataProducao == trxDate
                    && w.Granja == farm
                    && w.Incubatorio == hatchLoc
                    && w.TipoDEO == "Ovos Incubáveis"
                    && w.TipoOvo == "").FirstOrDefault();

            if (deo == null) deo = new LayoutDiarioExpedicaos();

            deo.Granja = farm;
            deo.Nucleo = farmID;
            deo.Galpao = shed.ToString();
            deo.Lote = flockNumber.ToString();
            deo.Idade = age;
            deo.Linhagem = variety;
            deo.LoteCompleto = flockID;
            deo.DataProducao = trxDate;
            deo.NumeroReferencia = DateTime.Now.DayOfYear.ToString();
            deo.QtdeOvos = qtyEggs;
            deo.QtdeBandejas = (qtyEggs / 360);
            deo.Usuario = "SISTEMA SERVICE";
            deo.DataHora = DateTime.Now;
            deo.DataHoraCarreg = trxDate;
            deo.NFNum = "";
            deo.Importado = "Conferido";
            deo.Incubatorio = hatchLoc;
            deo.TipoDEO = "Ovos Incubáveis";
            deo.DataHoraRecebInc = Convert.ToDateTime("01/01/1899");
            deo.ResponsavelCarreg = "";
            deo.ResponsavelReceb = "";
            deo.Observacao = "Diario de envío generado automáticamente al importar datos de producción.";
            deo.TipoOvo = "";
            deo.QtdDiferenca = 0;
            deo.QtdeConferencia = 0;

            if (deo.ID > 0) bdSQL.SaveChanges();

            return deo;
        }

        #endregion

        #endregion

        public void CorrigiNascimentosWEBparaFLIPIncAvos()
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            DateTime data = Convert.ToDateTime("31/03/2019");
            var listaNascPH = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Hatch_Loc == "PH"
                    && w.Set_date >= data)
                .ToList();

            foreach (var item in listaNascPH)
            {
                HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                DateTime setDate = Convert.ToDateTime(item.Set_date);
                int existeFLIP = Convert.ToInt32(hfdTA.ExisteHatcheryFlockData("HYBR", "BR", "GP", setDate, item.Hatch_Loc, item.Flock_id));

                if (existeFLIP > 0)
                {
                    UpdateHatchingDataFLIP("HYBR", "BR", "GP", item.Hatch_Loc, setDate,
                        item.Flock_id, item, "Insert");
                }
            }
        }
    }
}
