using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Data.OleDb;
using System.Configuration;
using System.Timers;
using ImportaCHICService.Data;
using ImportaCHICService.Data.CHICDataSetTableAdapters;
using ImportaCHICService.Data.CHICPARDataSetTableAdapters;
using ImportaCHICService.Data.CHICBKPDataSetTableAdapters;
using System.Data.Objects;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Collections;
using ImportaCHICService.Embarcador;
using ImportaCHICService.Data.CHICParentDataSetTableAdapters;

namespace ImportaCHICService
{
    public partial class ImportaCHIC : ServiceBase
    {
        #region Objetos
        private Timer _oTimer;
        private Timer _oTimer02;
        private Timer _oTimerDia;
        private Timer _oTimerSemana;

        //ApoloServiceEntities apolo = new ApoloServiceEntities();

        CHICDataSet dsCHIC = new CHICDataSet();
        ordersTableAdapter orders = new ordersTableAdapter();
        int_commTableAdapter intcomm = new int_commTableAdapter();
        ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter booked =
            new ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter();
        custTableAdapter cust = new custTableAdapter();
        paycodesTableAdapter paycodes = new paycodesTableAdapter();
        salesmanTableAdapter salesman = new salesmanTableAdapter();
        salesman1TableAdapter salesman1 = new salesman1TableAdapter();
        itemsTableAdapter items = new itemsTableAdapter();
        custcustTableAdapter custcust = new custcustTableAdapter();
        vartablTableAdapter vartbl = new vartablTableAdapter();

        CHICPARDataSet dsCHICPAR = new CHICPARDataSet();
        custTableAdapterPAR custPAR = new custTableAdapterPAR();

        //HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
        
        public static int verifica = 0;
        public static int verificaAtualiza = 0;
        public static int verificaCliente = 0;
        public static int erro = 0;
        public static string numPedidoCHIC = "";
        public static string origemErro = "";
        public static string produtoErro = "";

        public static int servicoIniciado = 0;

        #endregion

        #region Metodos do Serviço

        public ImportaCHIC()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                servicoIniciado = 1;

                // Minuto
                _oTimer = new Timer(60 * 1000);
                _oTimer.Elapsed += Atualizacao_Tick;
                _oTimer.Enabled = true;
                _oTimer.Start();

                // Hora
                _oTimer02 = new Timer(3600 * 1000);
                _oTimer02.Elapsed += Atualizacao_02_Tick;
                _oTimer02.Enabled = true;
                _oTimer02.Start();
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry("Erro ao Iniciar o Serviço: " + ex.Message, EventLogEntryType.Error, 10);
            }
        }

        protected override void OnStop()
        {

        }

        private void Atualizacao_Tick(object sender, EventArgs e)
        {
            if (((DateTime.Now.Hour >= 4) && (DateTime.Now.Hour <= 23)))
            {
                string mensagem = "";

                if (verifica == 0)
                {
                    if (verificaCliente == 0)
                    {
                        string erroImportaClientes = "";

                        try
                        {
                            verificaCliente = 1;
                            //this.EventLog.WriteEntry("Importação de Clientes Iniciada.");
                            mensagem = "Clientes";

                            /**** 21/03/2021 - DESATIVADO DEVIDO A TROCA DE SISTEMA (CHIC P/ ANIPLAN - FLUIG) ****/

                            //erroImportaClientes = ImportaClientesAPOLO();
                            //this.EventLog.WriteEntry("Importação de Clientes Concluída.");
                            verificaCliente = 0;
                        }
                        catch (Exception ex)
                        {
                            int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                            //string msg = "Erro Linha: " + linenum.ToString() + " - " + ex.Message;
                            string msg = "Erro Linha: " + erroImportaClientes + " - " + ex.Message;
                            if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                            if (!msg.Contains("Timeout expirado."))
                                this.EventLog.WriteEntry("Erro ao realizar Importação de Clientes no CHIC: " +
                                    msg, EventLogEntryType.Error, 10);
                        }
                    }

                    if (servicoIniciado == 1)
                    {
                        try
                        {
                            verifica = 1;

                            // TESTE
                            //string retorno = "";
                            //DateTime dataInicial = DateTime.Today.AddYears(-1);
                            //DateTime dataFinal = DateTime.Today.AddYears(5);
                            //this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Iniciada.");
                            //retorno = ImportacaoProgDiariaTranspCHICPeriodo(dataInicial, dataFinal);
                            //if (retorno != "")
                            //{
                            //    this.EventLog.WriteEntry("Erro ao realizar Importação da Programação Diária de Transportes do CHIC :"
                            //        + retorno, EventLogEntryType.Error, 10);
                            //}

                            //this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Finalizada.");

                            #region CHIC para Apolo

                            /**** 21/03/2021 - DESATIVADO DEVIDO A TROCA DE SISTEMA (CHIC P/ ANIPLAN - FLUIG) ****/

                            //this.EventLog.WriteEntry("Importação de Pedidos do CHIC para o Apolo Iniciada.");
                            //mensagem = "Importação de Pedidos ao Iniciar Servico - ";
                            //ImportaPedidosCHIC();
                            //this.EventLog.WriteEntry("Importação de Pedidos do CHIC para o Apolo Concluída.");

                            #endregion

                            #region CHIC para WEB

                            //mensagem = "Atualização de Pedidos do CHIC para WEB (Novos, Qtdes, Datas e Preços) - ";
                            //this.EventLog.WriteEntry(mensagem + " Iniciada.");
                            //string retornoAtualiza = AtualizaWEBxCHIC();
                            //this.EventLog.WriteEntry(mensagem + " Concluída.");
                            //if (retornoAtualiza != "")
                            //{
                            //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                            //        retornoAtualiza, EventLogEntryType.Error, 10);
                            //}

                            //mensagem = "Atualização do CHIC para WEB - Pedidos Vendidos Novo - ";
                            //this.EventLog.WriteEntry(mensagem + " Iniciada.");
                            //retornoAtualiza = AtualizaPedidosVendidosWEBxCHIC();
                            //if (retornoAtualiza != "")
                            //{
                            //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                            //        retornoAtualiza, EventLogEntryType.Error, 10);
                            //}
                            //this.EventLog.WriteEntry(mensagem + " Finalizada.");

                            //mensagem = "Atualização do CHIC para WEB - Pedidos Reposição Novo - ";
                            //this.EventLog.WriteEntry(mensagem + " Iniciada.");
                            //retornoAtualiza = AtualizaPedidosReposicaoWEBxCHIC();
                            //if (retornoAtualiza != "")
                            //{
                            //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                            //        retornoAtualiza, EventLogEntryType.Error, 10);
                            //}
                            //this.EventLog.WriteEntry(mensagem + " Finalizada.");

                            #endregion

                            #region Atualiza Programação Diária de Transportes

                            //string retorno = "";
                            ////DateTime dataInicial = DateTime.Today.AddYears(-1);
                            ////DateTime dataFinal = DateTime.Today.AddYears(5);
                            //DateTime dataInicial = Convert.ToDateTime("01/01/2021");
                            //DateTime dataFinal = Convert.ToDateTime("31/12/2022");
                            //this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Iniciada.");
                            //retorno = ImportacaoProgDiariaTranspCHICPeriodo(dataInicial, dataFinal);
                            //if (retorno != "")
                            //{
                            //    this.EventLog.WriteEntry("Erro ao realizar Importação da Programação Diária de Transportes do CHIC :"
                            //        + retorno, EventLogEntryType.Error, 10);
                            //}

                            //this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Finalizada.");

                            #endregion

                            verifica = 0;
                            servicoIniciado = 0;
                        }
                        catch (Exception ex)
                        {
                            int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                            string msg = "Erro Linha: " + linenum.ToString() + " - " + ex.Message;
                            if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                msg, EventLogEntryType.Error, 10);
                        }
                    }

                    //if (verificaAtualiza == 0)
                    //{
                    //    try
                    //    {
                    //        verificaAtualiza = 1;
                    //        this.EventLog.WriteEntry("Atualização de Status do Pedido no CHIC Iniciada.");
                    //        mensagem = "Status";
                    //        AtulizaStatusPedidoCHIC();
                    //        this.EventLog.WriteEntry("Atualização de Status do Pedido no CHIC Concluída.");
                    //        verificaAtualiza = 0;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        this.EventLog.WriteEntry("Erro ao realizar Atualização de Status do Pedido no CHIC: " + ex.Message);
                    //    }
                    //}
                }

                //try
                //{
                //    if (((DateTime.Now.Hour == 5) && (DateTime.Now.Minute == 00)))
                //    {
                //        mensagem = "Atualização do CHIC para WEB - ";
                //        string retornoAtualiza = AtualizaWEBxCHIC();
                //        if (retornoAtualiza != "")
                //        {
                //            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                //                retornoAtualiza, EventLogEntryType.Error, 10);
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                //    string msg = "Erro Linha: " + linenum.ToString() + " - " + ex.Message;
                //    if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                //    this.EventLog.WriteEntry("Erro ao realizar Atualização do CHIC para WEB: " +
                //        msg, EventLogEntryType.Error, 10);
                //}

                string erro = "";

                try
                {
                    EnviaCurriculosFTPEmail();

                    if (((DateTime.Now.Hour == 4) && (DateTime.Now.Minute == 00)))
                    {
                        verifica = 1;
                        /**** 21/03/2021 - DESATIVADO DEVIDO A TROCA DE SISTEMA (CHIC P/ ANIPLAN - FLUIG) ****/
                        //this.EventLog.WriteEntry("Importação de Pedidos Iniciada.");
                        //erro = "Erro ao realizar Importação de Pedidos no CHIC: ";
                        //mensagem = "Pedidos";
                        //ImportaPedidosCHIC();
                        //this.EventLog.WriteEntry("Importação de Pedidos Concluída.");
                        verifica = 0;
                    }

                    if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        if (((DateTime.Now.Hour == 11) && (DateTime.Now.Minute == 30)))
                        {
                            this.EventLog.WriteEntry("Envio de Verificação Final Iniciada.");
                            mensagem = "Envio de Verificação Final";
                            erro = "Erro ao Envio de Verificação Final: ";

                            /**** 21/03/2021 - DESATIVADO DEVIDO A TROCA DE SISTEMA (CHIC P/ ANIPLAN - FLUIG) ****/
                            //EnviarVerificacaoFinal();
                            EnviarVerificacaoFinalAniPlan();
                            this.EventLog.WriteEntry("Envio de Verificação Final Concluída.");
                        }
                    }

                    if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                    {
                        if (((DateTime.Now.Hour == 15) && (DateTime.Now.Minute == 00)))
                        {
                            this.EventLog.WriteEntry("Envio de Verificação Final Planalto Iniciada.");
                            mensagem = "Envio de Verificação Final Planalto";
                            erro = "Erro ao Envio de Verificação Final Planalto: ";

                            /**** 21/03/2021 - DESATIVADO DEVIDO A TROCA DE SISTEMA (CHIC P/ ANIPLAN - FLUIG) ****/
                            //string retornoErro = EnviarVerificacaoFinalPlanalto();
                            string retornoErro = EnviarVerificacaoFinalPlanaltoAniPlan();
                            this.EventLog.WriteEntry("Envio de Verificação Final Planalto Concluída.");
                            if (retornoErro != "")
                            {
                                erro = erro + retornoErro;
                                this.EventLog.WriteEntry(erro, EventLogEntryType.Error, 10);
                            }
                        }
                    }

                    if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                    {
                        if (((DateTime.Now.Hour == 14) && (DateTime.Now.Minute == 00)))
                        {
                            mensagem = "Envio de Programação de Transportes Semanal para Vendedores";
                            this.EventLog.WriteEntry("Envio de Programação de Transportes Semanal para Vendedores Iniciada.");
                            erro = "Erro ao Enviar de Programação de Transportes Semanal para Vendedores: ";
                            string retornoErro = EnviarProgramacaoDiariaTransportesSemanal();
                            this.EventLog.WriteEntry("Envio de Programação de Transportes Semanal para Vendedores Iniciada.");
                            if (retornoErro != "")
                            {
                                erro = erro + retornoErro;
                                this.EventLog.WriteEntry(erro, EventLogEntryType.Error, 10);
                            }
                        }
                    }

                    /*if (((DateTime.Now.Hour == 16) && (DateTime.Now.Minute == 00)))
                    {
                        this.EventLog.WriteEntry("Envio das Confirmações para o Faturamento Iniciada.");
                        mensagem = "Envio das Confirmações para o Faturamento";
                        EnviaPedidosCHIC();
                        this.EventLog.WriteEntry("Envio das Confirmações para o Faturamento Concluída.");
                    }*/
                }
                catch (Exception ex)
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    string msg = "Erro Linha: " + linenum.ToString()
                        + " - " + erro
                        + " - " + ex.Message;
                    if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                    this.EventLog.WriteEntry(msg, EventLogEntryType.Error, 10);
                }
            }
        }

        private void Atualizacao_02_Tick(object sender, EventArgs e)
        {
            if (((DateTime.Now.Hour >= 4) && (DateTime.Now.Hour <= 23)))
            {
                #region Atualização do CHIC para WEB

                try
                {
                    if (DateTime.Now.Hour == 21)
                    {
                        string mensagem = "";
                        string retornoAtualiza = "";
                        //mensagem = "Atualização do CHIC para WEB Antigo - ";
                        //retornoAtualiza = AtualizaWEBxCHIC();
                        //if (retornoAtualiza != "")
                        //{
                        //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                        //        retornoAtualiza, EventLogEntryType.Error, 10);
                        //}

                        /**** 21/03/2021 - DESATIVADO DEVIDO A TROCA DE SISTEMA (CHIC P/ ANIPLAN - FLUIG) ****/

                        //mensagem = "Atualização do CHIC para WEB - Pedidos Vendidos Novo - ";
                        //this.EventLog.WriteEntry(mensagem + " Iniciada.");
                        //retornoAtualiza = AtualizaPedidosVendidosWEBxCHIC();
                        //if (retornoAtualiza != "")
                        //{
                        //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                        //        retornoAtualiza, EventLogEntryType.Error, 10);
                        //}
                        //this.EventLog.WriteEntry(mensagem + " Finalizada.");

                        //mensagem = "Atualização do CHIC para WEB - Pedidos Reposição Novo - ";
                        //this.EventLog.WriteEntry(mensagem + " Iniciada.");
                        //retornoAtualiza = AtualizaPedidosReposicaoWEBxCHIC();
                        //if (retornoAtualiza != "")
                        //{
                        //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                        //        retornoAtualiza, EventLogEntryType.Error, 10);
                        //}
                        //this.EventLog.WriteEntry(mensagem + " Finalizada.");
                    }
                }
                catch (Exception ex)
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    string msg = "Erro Linha: " + linenum.ToString() + " - " + ex.Message;
                    if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                    this.EventLog.WriteEntry("Erro ao realizar Atualização do CHIC para WEB: " +
                        msg, EventLogEntryType.Error, 10);
                }

                #endregion

                #region Atualiza Programação Diária de Transportes no Nascimento do dia

                if (DateTime.Now.Hour == 22)
                {
                    //DateTime dataInicial = DateTime.Today.AddDays(-30);
                    //this.EventLog.WriteEntry("Atualização na Programação Diária de Transportes Iniciada.");
                    //string retorno = AtualizarProgDiariaTranspDiaNascimentoPeriodo(dataInicial, DateTime.Today);
                    //if (retorno != "")
                    //{
                    //    this.EventLog.WriteEntry("Erro ao realizar Atualização na Programação Diária de "
                    //        + "Transportes: " +
                    //        retorno, EventLogEntryType.Error, 10);
                    //}

                    //this.EventLog.WriteEntry("Atualização na Programação Diária de Transportes Finalizada.");

                    /**** 21/03/2021 - DESATIVADO DEVIDO A TROCA DE SISTEMA (CHIC P/ ANIPLAN - FLUIG) ****/

                    //string retorno = "";
                    //DateTime dataInicial = DateTime.Today.AddYears(-1);
                    //DateTime dataFinal = DateTime.Today.AddYears(5);
                    //this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Iniciada.");
                    //retorno = ImportacaoProgDiariaTranspCHICPeriodo(dataInicial, dataFinal);
                    //if (retorno != "")
                    //{
                    //    this.EventLog.WriteEntry("Erro ao realizar Importação da Programação Diária de Transportes do CHIC :"
                    //        + retorno, EventLogEntryType.Error, 10);
                    //}

                    //this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Finalizada.");
                }

                #endregion

                #region Atualiza Relatório de Dias de Estoque - Diário - DESABILITADO CONFORME SOLICITAÇÃO DO DAVI EM 21/01/2020

                /*
                if (DateTime.Now.Hour == 5)
                {
                    try
                    {
                        #region Matrizes

                        string mensagem = "Relatório de Dias de Estoque - Diário - ";
                        //string emailsCopiaGranja = "";
                        string emailsCopiaGranja = "cgamboa@hyline.com.br;dmelo@hyline.com.br;"
                            + "lgasparino@hyline.com.br;cbarros@hyline.com.br;rpedro@hyline.com.br;"
                            + "snociti@hyline.com.br;";
                        string retorno = EnviarDiasEstoqueGranjas(emailsCopiaGranja, "Matrizes");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        retorno = "";
                        //string emailsCopiaIncubatorios = "";
                        string emailsCopiaIncubatorios = "bvieira@hyline.com.br;aneves@planaltopostura.com.br;"
                            + "sdoimo@hyline.com.br;incubacao-nm@hyline.com.br";
                        retorno = EnviarDiasEstoqueIncubatorios(emailsCopiaIncubatorios, "Matrizes");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        #endregion

                        #region Avós

                        retorno = "";
                        string emailsCopiaGranjaAvos = "aprates@hyline.com.br;lalmeida@hyline.com.br";
                        retorno = EnviarDiasEstoqueGranjas(emailsCopiaGranjaAvos, "Avós");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        retorno = "";
                        string emailsCopiaIncubatoriosAvos = "jsegura@hyline.com.br";
                        retorno = EnviarDiasEstoqueIncubatorios(emailsCopiaIncubatoriosAvos, "Avós");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                        string msg = "Erro Linha: " + linenum.ToString() + " - " + ex.Message;
                        if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                        this.EventLog.WriteEntry("Erro ao realizar Relatório de Dias de Estoque - Diário: " +
                            msg, EventLogEntryType.Error, 10);
                    }
                }
                */

                #endregion

                #region Atualiza Relatório de Dias de Estoque / Perdas - Semanal

                if (DateTime.Now.Hour == 5 && DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                {
                    try
                    {
                        #region Atualiza Relatório de Dias de Estoque - Semanal - DESABILITADO CONFORME SOLICITAÇÃO DO DAVI EM 21/01/2020
                        
                        #region Variáveis

                        string mensagem = "";
                        string emailsCopiaGranja = "";
                        string retorno = "";

                        #endregion

                        /*
                        #region Matrizes

                        string mensagem = "Relatório de Dias de Estoque - Semanal - ";
                        //string emailsCopiaGranja = "";
                        string emailsCopiaGranja = "cgamboa@hyline.com.br;dmelo@hyline.com.br;"
                            + "lgasparino@hyline.com.br;cbarros@hyline.com.br;rpedro@hyline.com.br;"
                            + "snociti@hyline.com.br;tlourenco@hyline.com.br;dnogueira@hyline.com.br";
                        string retorno = EnviarDiasEstoqueGranjas(emailsCopiaGranja, "Matrizes");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        retorno = "";
                        //string emailsCopiaIncubatorios = "";
                        string emailsCopiaIncubatorios = "bvieira@hyline.com.br;aneves@planaltopostura.com.br;"
                            + "sdoimo@hyline.com.br;tlourenco@hyline.com.br;"
                            + "dnogueira@hyline.com.br;flucio@hyline.com.br";
                        retorno = EnviarDiasEstoqueIncubatorios(emailsCopiaIncubatorios, "Matrizes");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        #endregion

                        #region Avós

                        retorno = "";
                        string emailsCopiaGranjaAvos = "aprates@hyline.com.br;lalmeida@hyline.com.br;"
                            + "tlourenco@hyline.com.br";
                        retorno = EnviarDiasEstoqueGranjas(emailsCopiaGranjaAvos, "Avós");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        retorno = "";
                        string emailsCopiaIncubatoriosAvos = "jsegura@hyline.com.br;tlourenco@hyline.com.br";
                        retorno = EnviarDiasEstoqueIncubatorios(emailsCopiaIncubatoriosAvos, "Avós");
                        if (retorno != "")
                        {
                            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                                retorno, EventLogEntryType.Error, 10);
                        }

                        #endregion
                        */

                        #endregion

                        #region Relatório Semanal de Perdas - Comercial - DESATIVADO CONFORME SOLICITAÇÃO DO TIAGO EM 03/02/2020

                        //mensagem = "Relatório Semanal de Perdas - Comercial - ";
                        //retorno = "";
                        //retorno = SendReportLossWeeklyComercial();
                        //if (retorno != "")
                        //{
                        //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                        //        retorno, EventLogEntryType.Error, 10);
                        //}

                        #endregion

                        #region Relatório Semanal de Perdas - Matriz - DESATIVADO CONFORME SOLICITAÇÃO DO TIAGO EM 03/02/2020

                        //mensagem = "Relatório Semanal de Perdas - Matriz - ";
                        //retorno = "";
                        //retorno = SendReportLossWeeklyMatriz();
                        //if (retorno != "")
                        //{
                        //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                        //        retorno, EventLogEntryType.Error, 10);
                        //}

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                        string msg = "Erro Linha: " + linenum.ToString() + " - " + ex.Message;
                        if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                        this.EventLog.WriteEntry("Erro ao realizar Relatório de Dias de Estoque - Semanal: " +
                            msg, EventLogEntryType.Error, 10);
                    }
                }

                #endregion

                #region Atualiza dos Pedido do CHIC para o Embarcador (DESABILITADO - CARGA CRIADA DIRETO DO WEB)

                //if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Hour == 5)
                //{
                //    string retorno = ImportaPedidosEmbarcador();
                //    this.EventLog.WriteEntry("Atualização dos Pedido do CHIC para o Embarcador Iniciada.");
                //    if (retorno != "")
                //    {
                //        this.EventLog.WriteEntry("Erro ao realizar Atualização dos Pedido do CHIC para "
                //            + "o Embarcador: " +
                //            retorno, EventLogEntryType.Error, 10);
                //    }
                //    this.EventLog.WriteEntry("Atualização dos Pedido do CHIC para o Embarcador Finalizada.");
                //}

                #endregion
            }
        }

        public void TesteAtualizacao()
        {
            if (((DateTime.Now.Hour >= 4) && (DateTime.Now.Hour <= 23)))
            {
                #region Atualização do CHIC para WEB (COMENTADA)

                //try
                //{
                //    if (DateTime.Now.Hour == 13)
                //    {
                //        string mensagem = "";
                //        string retornoAtualiza = "";
                //        //mensagem = "Atualização do CHIC para WEB Antigo - ";
                //        //retornoAtualiza = AtualizaWEBxCHIC();
                //        //if (retornoAtualiza != "")
                //        //{
                //        //    this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                //        //        retornoAtualiza, EventLogEntryType.Error, 10);
                //        //}

                //        mensagem = "Atualização do CHIC para WEB - Pedidos Vendidos Novo - ";
                //        this.EventLog.WriteEntry(mensagem + " Iniciada.");
                //        retornoAtualiza = AtualizaPedidosVendidosWEBxCHIC();
                //        if (retornoAtualiza != "")
                //        {
                //            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                //                retornoAtualiza, EventLogEntryType.Error, 10);
                //        }
                //        this.EventLog.WriteEntry(mensagem + " Finalizada.");

                //        mensagem = "Atualização do CHIC para WEB - Pedidos Reposição Novo - ";
                //        this.EventLog.WriteEntry(mensagem + " Iniciada.");
                //        retornoAtualiza = AtualizaPedidosReposicaoWEBxCHIC();
                //        if (retornoAtualiza != "")
                //        {
                //            this.EventLog.WriteEntry("Erro ao realizar " + mensagem +
                //                retornoAtualiza, EventLogEntryType.Error, 10);
                //        }
                //        this.EventLog.WriteEntry(mensagem + " Finalizada.");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                //    string msg = "Erro Linha: " + linenum.ToString() + " - " + ex.Message;
                //    if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;
                //    this.EventLog.WriteEntry("Erro ao realizar Atualização do CHIC para WEB: " +
                //        msg, EventLogEntryType.Error, 10);
                //}

                #endregion

                #region Atualiza Programação Diária de Transportes no Nascimento do dia

                if (DateTime.Now.Hour == 17)
                {
                    DateTime dataInicial = DateTime.Today.AddDays(1);
                    DateTime dataFinal = dataInicial.AddYears(5);
                    this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Iniciada.");
                    string retorno = ImportacaoProgDiariaTranspCHICPeriodo(dataInicial, dataFinal);
                    if (retorno != "")
                    {
                        this.EventLog.WriteEntry("Erro ao realizar Importação da Programação Diária de Transportes do CHIC :"
                            + retorno, EventLogEntryType.Error, 10);
                        this.EventLog.WriteEntry("Importação da Programação Diária de Transportes do CHIC Finalizada.");
                    }
                }

                #endregion
            }
        }

        #endregion

        #region CHIC X APOLO

        public void ImportaPedidosCHIC()
        {
            string emailVerificaEntidade = "";

            string execucaoProcedure = "";
            
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;
            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            try
            {
                erro = 1;
                //string teste2 = "30/60/90 DDL (Não aplicar Bouba)";
                //string teste = (teste2.Substring(0, (teste2.IndexOf("(")-1))).Trim();

                orders.FillByStatus(dsCHIC.orders, "SENT");
                //orders.FillByNumero(dsCHIC.orders, "99489");
                //orders.FillByNumero(dsCHIC.orders, "30042");

                string nfMaeAdiantamento = "";
                string anexos = "";

                CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                items.Fill(iDT);

                erro = 2;

                for (int i = 0; i < dsCHIC.orders.Rows.Count; i++)
                {
                    erro = 3;

                    numPedidoCHIC = dsCHIC.orders[i].orderno;

                    //if ((dsCHIC.orders[i].orderno.Equals("47425"))
                    //    ||
                    //   (dsCHIC.orders[i].orderno.Equals("45885")))
                    //{

                    intcomm.FillByOrderNo(dsCHIC.int_comm, dsCHIC.orders[i].orderno);

                    string custno = dsCHIC.orders[i].cust_no;

                    ENTIDADE1 entidade1 = apolo.ENTIDADE1
                        .Where(e1 => e1.EntCod == custno)
                        .First();

                    string tipoColabOvosBrasil = "";

                    if (entidade1.USERTipoColabOvosBRasil != null)
                    {
                        tipoColabOvosBrasil = entidade1.USERTipoColabOvosBRasil;
                    }

                    /*if ((tipoColabOvosBrasil.Equals("Participa Lista")) &&
                        (!dsCHIC.int_comm[0].listaiob))
                    {
                        ENTIDADE entidade = apolo.ENTIDADE
                            .Where(e => e.EntCod == custno)
                            .First();

                        WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                        apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                        email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                        email.WorkFlowEmailStat = "Enviar";
                        email.WorkFlowEmailAssunto = "**** PEDIDO " + dsCHIC.orders[i].orderno + " SEM OVOS BRASIL, PORÉM NA LISTA ****";
                        email.WorkFlowEmailData = DateTime.Now;
                        email.WorkFlowEmailParaNome = "Programação";
                        email.WorkFlowEmailParaEmail = "programacao@hyline.com.br";
                        email.WorkFlowEmailCopiaPara = "llopes@hyline.com.br";
                        email.WorkFlowEmailDeNome = "Serviço de Importação";
                        email.WorkFLowEmailDeEmail = "sistema@hyline.com.br";
                        email.WorkFlowEmailFormato = "Texto";

                        string corpoEmail = "";

                        corpoEmail = "O pedido " + dsCHIC.orders[i].orderno + " do cliente " + entidade.EntNome
                            + " não está com a opção 'Participa Lista IOB' marcada, porém o cliente contém na Lista IOB."
                            + (char)13 + (char)10 + (char)13 + (char)10
                            + "Realize o ajuste e faça a importação manual, pois esse pedido não será importado automaticamente." + erro.ToString() + (char)13 + (char)10
                            + (char)13 + (char)10 + (char)13 + (char)10
                            + "SISTEMA DE IMPORTAÇÃO CHIC / APOLO";

                        email.WorkFlowEmailCorpo = corpoEmail;

                        apolo.WORKFLOW_EMAIL.AddObject(email);

                        apolo.SaveChanges();
                    }
                    else
                    {*/
                    decimal ovosBrasil = 0;

                    if (dsCHIC.int_comm[0].invmess)
                        ovosBrasil = 0.05m;
                    if ((dsCHIC.int_comm[0].invmess1) ||
                        (dsCHIC.int_comm[0].listaiob) ||
                        (tipoColabOvosBrasil.Equals("Participa Lista")))
                        ovosBrasil = 0.01m;
                    if (dsCHIC.int_comm[0].invmess3)
                        ovosBrasil = 0.03m;

                    if (!dsCHIC.int_comm[0].IsnfmaeNull())
                        nfMaeAdiantamento = dsCHIC.int_comm[0].nfmae.Trim();

                    erro = 4;

                    booked.FillByOrderNo(dsCHIC.booked, dsCHIC.orders[i].orderno);

                    CHICDataSet.itemsDataTable itemDTList = new CHICDataSet.itemsDataTable();
                    items.Fill(itemDTList);

                    var listaItens = dsCHIC.booked
                        //.Where(w => itemDTList.Any(a => a.item_no == w.item
                        //&& (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))))
                        //.Where(w => w.price > 0)
                        .OrderBy(o => o.item_ord).ThenBy(t => t.alt_desc)
                        .ToList();

                    cust.FillByCustNo(dsCHIC.cust, dsCHIC.orders[i].cust_no);

                    paycodes.FillByCode(dsCHIC.paycodes, dsCHIC.orders[i].pay_terms.ToString());

                    salesman.FillByCod(dsCHIC.salesman, dsCHIC.orders[i].salesrep);

                    erro = 5;

                    string origem = "";
                    string empresa = "";
                    string empresa2 = "";
                    emailVerificaEntidade = "";
                    string emailFinanceiro = "";
                    string emailFaturamento = "";

                    if (dsCHIC.salesman.Rows.Count > 0)
                    {
                        if (dsCHIC.salesman[0].inv_comp.Equals("BR"))
                        {
                            origem = "HYLINE";
                            empresa = "1";
                            empresa2 = "1";
                            emailVerificaEntidade = "programacao@hyline.com.br";
                            emailFinanceiro = "financeiro@hyline.com.br";
                            emailFaturamento = "faturamento@hyline.com.br";
                        }
                        if (dsCHIC.salesman[0].inv_comp.Equals("LB"))
                        {
                            origem = "LOHMANN";
                            empresa = "12";
                            empresa2 = "21";
                            emailVerificaEntidade = "programacao@ltz.com.br";
                            emailFinanceiro = "financeiro@ltz.com.br";
                            emailFaturamento = "faturamento@hyline.com.br";
                        }
                        if (dsCHIC.salesman[0].inv_comp.Equals("HN"))
                        {
                            origem = "H&N";
                            empresa = "15";
                            empresa2 = "15";
                            emailVerificaEntidade = "programacao@hnavicultura.com.br";
                            emailFinanceiro = "financeiro@hnavicultura.com.br";
                            emailFaturamento = "faturamento@hyline.com.br";
                        }
                        if (dsCHIC.salesman[0].inv_comp.Equals("PL"))
                        {
                            origem = "PLANALTO";
                            empresa = "20";
                            empresa2 = "20";
                            emailVerificaEntidade = "programacao@planaltopostura.com.br";
                            emailFinanceiro = "financeiro@planaltopostura.com.br";
                            emailFaturamento = "faturamento@planaltopostura.com.br";
                        }
                    }

                    #region Carrega dados da empresa do Apolo

                    var codigoEmpresaCHIC = dsCHIC.salesman[0].inv_comp.Trim();
                    var codigoIncubatorioFLIP = listaItens.FirstOrDefault().location.Trim();
                    origem = codigoEmpresaCHIC + "-" + codigoIncubatorioFLIP;
                    var empresaApolo = apolo.EMPRESA_FILIAL
                        .Where(w => w.USERTipoUnidadeFLIP == "Incubatório"
                            && w.USERCodigoCHIC == codigoEmpresaCHIC
                            && w.USERFLIPCod == codigoIncubatorioFLIP)
                        .FirstOrDefault();

                    if (empresaApolo != null)
                    {
                        empresa = empresaApolo.EmpCod;
                    }

                    #endregion

                    #region Verifica NF Mãe

                    string continua = "Sim";
                    if (nfMaeAdiantamento != "")
                    {
                        nfMaeAdiantamento = nfMaeAdiantamento.Replace(".", "");
                        int tamanho = nfMaeAdiantamento.Length;
                        int qtdCompleta = 10 - tamanho;
                        StringBuilder completa = new StringBuilder().Insert(0, "0", qtdCompleta);
                        string nfMae = completa + nfMaeAdiantamento;

                        int existe = 0;
                        existe = apolo.NOTA_FISCAL
                            .Where(w => (w.EmpCod == empresa || w.EmpCod == empresa2)
                                && w.CtrlDFModForm == "NF-e"
                                && w.NFNum == nfMae
                                && w.EntCod == custno)
                            .Count();

                        if (existe == 0)
                        {
                            continua = "Não";

                            ENTIDADE entidade = apolo.ENTIDADE
                                .Where(e => e.EntCod == custno)
                                .First();

                            WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                            apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                            email.WorkFlowEmailStat = "Enviar";
                            email.WorkFlowEmailAssunto = "**** PEDIDO " + dsCHIC.orders[i].orderno + " COM NF MÃE INCORRETA ****";
                            email.WorkFlowEmailData = DateTime.Now;
                            email.WorkFlowEmailParaNome = "Programação";
                            email.WorkFlowEmailParaEmail = emailVerificaEntidade;
                            email.WorkFlowEmailCopiaPara = emailFinanceiro + ";" + emailFaturamento;
                            email.WorkFlowEmailDeNome = "Serviço de Importação";
                            email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                            email.WorkFlowEmailFormato = "Texto";

                            string corpoEmail = "";

                            corpoEmail = "O pedido " + dsCHIC.orders[i].orderno + " do cliente " + entidade.EntNome
                                + " está com a Nota Fiscal Mãe de Adiantamento incorreta (" + nfMae + ")."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "Realize o ajuste e faça a importação manual, pois esse pedido não será importado automaticamente." + (char)13 + (char)10
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA DE IMPORTAÇÃO CHIC / APOLO";

                            email.WorkFlowEmailCorpo = corpoEmail;

                            apolo.WORKFLOW_EMAIL.AddObject(email);

                            apolo.SaveChanges();
                        }
                    }

                    #endregion

                    #region Verifica Entidade

                    string entCod = dsCHIC.orders[i].cust_no.Trim();
                    string orderNo = dsCHIC.orders[i].orderno.Trim();
                    string pais = "BRA";

                    ENTIDADE verificaEntidade = apolo.ENTIDADE
                        .Where(w => w.EntCod == entCod)
                        .FirstOrDefault();

                    if (verificaEntidade != null)
                    {
                        CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == verificaEntidade.CidCod).FirstOrDefault();
                        pais = cidade.PaisSigla;

                        #region Verifica Natureza da Entidade

                        if ((verificaEntidade.EntNat == null) || (verificaEntidade.EntNat == "")
                            || (verificaEntidade.EntNat == "Nenhum"))
                        {
                            continua = "Não";

                            string assuntoEmail = "** ERRO CADASTRO DO CLIENTE " + entCod
                                + " - PEDIDO " + orderNo + " **";

                            string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                                + "O Pedido " + orderNo + " está com o cadastro do cliente sem Natureza!"
                                + " Ajustar no cadastro do Apolo para que possa ser importado o mesmo."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA WEB";

                            EnviaConfirmacaoEmail(anexos, emailVerificaEntidade, "DEPTO. PROGRAMAÇÃO",
                                "", "", emailFaturamento, corpoEmail, assuntoEmail, empresa);
                        }

                        #endregion
                    }

                    #endregion

                    #region Verifica Se Todos os Itens estão no mesmo Incubatório

                    int verificaIncubatorio = listaItens
                        .GroupBy(g => g.location)
                        .Count();

                    int existeEmailEnviado = apolo.WORKFLOW_EMAIL
                        .Where(w => w.WorkFlowEmailAssunto == "** PEDIDO INCUBATÓRIO DIFERENTES " + entCod
                            + " - PEDIDO " + orderNo + " **")
                        .Count();

                    if (verificaIncubatorio > 1)
                    {
                        //continua = "Não";

                        if (existeEmailEnviado == 0)
                        {
                            string assuntoEmail = "** PEDIDO INCUBATÓRIO DIFERENTES " + entCod
                                + " - PEDIDO " + orderNo + " **";

                            string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                                + "O Pedido " + orderNo + " está com mais de um Incubatório nos Itens do CHIC!"
                                + " Ajustar o Pedido no CHIC para que possa ser importado o mesmo."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA WEB";

                            EnviaConfirmacaoEmail(anexos, emailVerificaEntidade, "DEPTO. PROGRAMAÇÃO",
                                "", "", emailFaturamento, corpoEmail, assuntoEmail, empresa);
                        }
                    }

                    #endregion

                    #region Verifica Se Existe Veiculo relacionado do Pedido

                    int verificaDadosTransporte = hlbappService.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.CHICNum == orderNo
                            && hlbappService.Prog_Diaria_Transp_Veiculos
                                .Any(a => a.EmpresaTranportador == w.EmpresaTranportador
                                    && a.DataProgramacao == w.DataProgramacao
                                    && a.NumVeiculo == w.NumVeiculo))
                        .Count();

                    int existeEmailEnviadoDadosTransporte = apolo.WORKFLOW_EMAIL
                        .Where(w => w.WorkFlowEmailAssunto == "** PEDIDO SEM DADOS DE TRANSPORTE " + entCod
                            + " - PEDIDO " + orderNo + " **")
                        .Count();

                    if (verificaDadosTransporte == 0)
                    {
                        //continua = "Não";

                        if (existeEmailEnviadoDadosTransporte == 0)
                        {
                            string assuntoEmail = "** PEDIDO SEM DADOS DE TRANSPORTE " + entCod
                            + " - PEDIDO " + orderNo + " **";

                            string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                                + "O Pedido " + orderNo + " não tem os dados de transporte vinculados para importar para o Apolo!"
                                + " Por favor, insira os dados de transporte para que possa ser importado o mesmo."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA WEB";

                            EnviaConfirmacaoEmail(anexos, emailVerificaEntidade, "DEPTO. PROGRAMAÇÃO",
                                "", "", emailFaturamento, corpoEmail, assuntoEmail, empresa);
                        }
                    }

                    #endregion

                    #region Valor Total do Pintinho

                    decimal valorTotalPintinho = 0;
                    string orderno = dsCHIC.orders[i].orderno.Trim();

                    Pedido_Venda pedVenda = hlbappService.Pedido_Venda.Where(w => hlbappService.Item_Pedido_Venda
                        .Any(it => it.IDPedidoVenda == w.ID
                            && (it.OrderNoCHIC == orderno || it.OrderNoCHICReposicao == orderno)))
                        .FirstOrDefault();

                    if (pedVenda != null)
                    {
                        valorTotalPintinho = Convert.ToDecimal(pedVenda.ValorTotalPintinho);
                    }

                    #endregion

                    #region Verifica qual produto inserir quando for exportação

                    if (pais != "BRA")
                    {
                        string codigoProdutoApoloCHIC = "";
                        var itemProduto = listaItens
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && (a.form.Substring(0, 1) == "D" || a.form.Substring(0, 1) == "H")))
                            .FirstOrDefault();
                        if (itemProduto != null) codigoProdutoApoloCHIC = itemProduto.accountno.Trim();

                        if (codigoProdutoApoloCHIC != "")
                        {
                            PRODUTO produtoApolo = apolo.PRODUTO.Where(w => w.ProdCodEstr == codigoProdutoApoloCHIC)
                                .FirstOrDefault();

                            if (produtoApolo != null)
                            {
                                if (produtoApolo.TribACod != "1")
                                {
                                    PRODUTO produtoExportacao = apolo.PRODUTO
                                        .Where(w => 
                                            //w.ProdCodAlt1 == produtoApolo.ProdCodAlt1
                                            w.CategProdCod == produtoApolo.CategProdCod
                                            && w.FxaProdCod == produtoApolo.FxaProdCod
                                            && w.ProdNomeAlt2 == produtoApolo.ProdNomeAlt2
                                            && w.ProdCodDesenho == "EX"
                                            && w.TribACod == "1")
                                        .FirstOrDefault();

                                    if (produtoExportacao != null)
                                        continua = "Sim";
                                    else
                                    {
                                        string assuntoEmail = "** PRODUTO " + produtoApolo.ProdNome
                                            + " PARA EXPORTAÇÃO NÃO CADASTRADO NO APOLO **";

                                        string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                                            + "Não existe o produto " + produtoApolo.ProdNome + " para exportação no Apolo!"
                                            + " Por favor, realize a cópia do produto " + produtoApolo.ProdCodEstr
                                            + " e no produto novo informe no campo 'Cód. Desenho' EX !"
                                            //+ "e no campo 'Alternativo 1' o mesmo código do produto copiado!"
                                            + (char)13 + (char)10 + (char)13 + (char)10
                                            + "SISTEMA WEB";

                                        EnviaConfirmacaoEmail(anexos, emailVerificaEntidade, "",
                                            "", "", "fiscal@hyline.com.br;" + emailFaturamento,
                                            corpoEmail, assuntoEmail, empresa);

                                        continua = "Não";
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    if (continua == "Sim")
                    {
                        origemErro = origem;

                        erro = 6;

                        string condPag = "";
                        if (dsCHIC.orders[i].delivery.IndexOf("(") > 0)
                            condPag = (dsCHIC.orders[i].delivery.Substring(0, (dsCHIC.orders[i].delivery.IndexOf("(") - 1))).Trim();
                        else
                            condPag = dsCHIC.orders[i].delivery.Trim();

                        erro = 7;

                        for (int j = 0; j < listaItens.Count; j++)
                        {
                            string accountno = listaItens[j].accountno.Trim();

                            if (pais != "BRA")
                            {
                                PRODUTO produtoApolo = apolo.PRODUTO.Where(w => w.ProdCodEstr == accountno)
                                    .FirstOrDefault();

                                if (produtoApolo != null)
                                {
                                    PRODUTO produtoExportacao = apolo.PRODUTO
                                        .Where(w => w.CategProdCod == produtoApolo.CategProdCod
                                            && w.FxaProdCod == produtoApolo.FxaProdCod
                                            && w.ProdNomeAlt2 == produtoApolo.ProdNomeAlt2
                                            && w.ProdCodDesenho == "EX"
                                            && w.TribACod == "1")
                                        .FirstOrDefault();

                                    accountno = produtoExportacao.ProdCodEstr;
                                }
                            }

                            //if (listaItens[j].location.Trim().Equals("NM"))
                            //{
                            //    if (dsCHIC.salesman[0].inv_comp.Equals("LB"))
                            //    {
                            //        origem = "LOHMANN-PL";
                            //        empresa = "21";
                            //    }
                            //}

                            produtoErro = listaItens[j].accountno;

                            erro = 8;

                            items.FillByItemNo(dsCHIC.items, listaItens[j].item);

                            erro = 9;

                            execucaoProcedure = "exec USER_IMPORTA_PEDIDO_CHIC_SERVICE '" +
                                dsCHIC.orders[i].cust_no + "','" +
                                dsCHIC.orders[i].orderno + "','" +
                                (listaItens[j].cal_date.AddDays(21)).ToString("yyyy-MM-dd") + "','" +
                                (dsCHIC.orders[i].del_date.AddDays(21)).ToString("yyyy-MM-dd") + "'," +
                                "0" + "," +
                                "0" + ",'" +
                                dsCHIC.orders[i].pay_terms.ToString() + "','" +
                                listaItens[j].modifdby + "','" +
                                dsCHIC.cust[0].name + "','" +
                                condPag + "','" +
                                "" + "','" +
                                dsCHIC.cust[0].street_1 + "','" +
                                "" + "','" +
                                "" + "','" +
                                dsCHIC.cust[0].street_2 + "','" +
                                dsCHIC.cust[0].city + "','" +
                                dsCHIC.cust[0].state + "','" +
                                dsCHIC.cust[0].country + "','" +
                                dsCHIC.orders[i].status + "','" +
                                accountno + "','" +
                                dsCHIC.items[0].item_desc.Trim() + "','" +
                                listaItens[j].alt_desc.Trim() + "'," +
                                listaItens[j].quantity.ToString().Replace(",", ".") + "," +
                                listaItens[j].price.ToString().Replace(",", ".") + "," +
                                (listaItens[j].quantity * listaItens[j].price).ToString().Replace(",", ".") + ",'" +
                                DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                (j + 1) + ",'" +
                                "0" + dsCHIC.orders[i].salesrep + "','" +
                                origem + "','" +
                                "" + "'," +
                                ovosBrasil + ",'" +
                                nfMaeAdiantamento + "'," +
                                valorTotalPintinho.ToString().Replace(",", ".");

                            apolo.ImportaPedidosCHIC(
                                dsCHIC.orders[i].cust_no,
                                dsCHIC.orders[i].orderno,
                                (listaItens[j].cal_date.AddDays(21)),
                                dsCHIC.orders[i].del_date,
                                0,
                                0,
                                dsCHIC.orders[i].pay_terms.ToString(),
                                listaItens[j].modifdby,
                                dsCHIC.cust[0].name,
                                condPag,
                                "",
                                dsCHIC.cust[0].street_1,
                                "",
                                "",
                                dsCHIC.cust[0].street_2,
                                dsCHIC.cust[0].city,
                                dsCHIC.cust[0].state,
                                dsCHIC.cust[0].country,
                                dsCHIC.orders[i].status,
                                //listaItens[j].accountno,
                                accountno,
                                //"",
                                dsCHIC.items[0].item_desc.Trim(),
                                listaItens[j].alt_desc.Trim(),
                                listaItens[j].quantity,
                                listaItens[j].price,
                                (listaItens[j].quantity * listaItens[j].price),
                                DateTime.Now,
                                (j + 1),
                                "0" + dsCHIC.orders[i].salesrep,
                                origem,
                                "",
                                ovosBrasil,
                                nfMaeAdiantamento,
                                valorTotalPintinho);

                            erro = 10;

                            apolo.SaveChanges();

                            System.Threading.Thread.Sleep(5000);

                            erro = 11;
                        }
                        //}

                        string numeroPedCHICAtualizaStatus = dsCHIC.orders[i].orderno;

                        orders.UpdateStatus("VOID", numeroPedCHICAtualizaStatus);
                    }
                    //}
                }

            }
            catch (Exception e)
            {
                ApoloServiceEntities apoloSession = new ApoloServiceEntities();

                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apoloSession.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ APOLO ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Paulo Alves";
                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";
                email.WorkFlowEmailCopiaPara = emailVerificaEntidade;

                string corpoEmail = "";

                string innerException = "";

                if (e.InnerException != null)
                {
                    innerException = e.InnerException.Message;
                }

                corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Apolo: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "Linha do Erro: " + erro.ToString() + (char)13 + (char)10
                    + "Número do Pedido CHIC: " + numPedidoCHIC + (char)13 + (char)10
                    + "Origem: " + origemErro + (char)13 + (char)10
                    + "Produto: " + produtoErro + (char)13 + (char)10 + (char)13 + (char)10
                    + "Linha do Erro: " + linenum.ToString() + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 1: " + e.Message + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 2: " + innerException + (char)13 + (char)10 + (char)13 + (char)10
                    + "Execução Procedure: " + execucaoProcedure;

                email.WorkFlowEmailCorpo = corpoEmail;

                if (e.InnerException != null)
                {
                    if (e.InnerException.Message.Length >= 17)
                    {
                        if (e.InnerException.Message.Substring(0, 17) != "Timeout expirado.")
                        {
                            apoloSession.WORKFLOW_EMAIL.AddObject(email);
                            apoloSession.SaveChanges();
                            this.EventLog.WriteEntry("Erro ao realizar Importação de Pedidos no CHIC: "
                                + "Linha Código: " + linenum.ToString() + " / " + e.Message
                                + " / Erro Interno: " + e.InnerException.Message, EventLogEntryType.Error, 10);
                        }
                    }
                }
                else
                {
                    apoloSession.WORKFLOW_EMAIL.AddObject(email);
                    apoloSession.SaveChanges();
                    this.EventLog.WriteEntry("Erro ao realizar Importação de Pedidos no CHIC: "
                        + "Linha Código: " + linenum.ToString() + " / " + e.Message, EventLogEntryType.Error, 10);
                }

                orders.UpdateStatus("VOID", numPedidoCHIC);
            }
        }

        public void EnviaPedidosCHIC()
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                erro = 1;
                //string teste2 = "30/60/90 DDL (Não aplicar Bouba)";
                //string teste = (teste2.Substring(0, (teste2.IndexOf("(")-1))).Trim();

                string data = DateTime.Today.AddDays(1).ToString("MM/dd/yyyy");

                orders.FillByHatchDate(dsCHIC.orders, "SENT", data);
                //orders.FillByNumero(dsCHIC.orders, "46703");
                //orders.FillByNumero(dsCHIC.orders, "30042");

                string nfMaeAdiantamento = "";
                string anexos = "";

                erro = 2;

                for (int i = 0; i < dsCHIC.orders.Rows.Count; i++)
                {
                    erro = 3;

                    numPedidoCHIC = dsCHIC.orders[i].orderno;

                    //if ((dsCHIC.orders[i].orderno.Equals("47425"))
                    //    ||
                    //   (dsCHIC.orders[i].orderno.Equals("45885")))
                    //{

                    intcomm.FillByOrderNo(dsCHIC.int_comm, dsCHIC.orders[i].orderno);

                    string custno = dsCHIC.orders[i].cust_no;

                    ENTIDADE1 entidade1 = apolo.ENTIDADE1
                        .Where(e1 => e1.EntCod == custno)
                        .First();

                    string tipoColabOvosBrasil = "";

                    if (entidade1.USERTipoColabOvosBRasil != null)
                    {
                        tipoColabOvosBrasil = entidade1.USERTipoColabOvosBRasil;
                    }

                    decimal ovosBrasil = 0;

                    if (dsCHIC.int_comm[0].invmess)
                        ovosBrasil = 0.05m;
                    if ((dsCHIC.int_comm[0].invmess1) ||
                        (dsCHIC.int_comm[0].listaiob) ||
                        (tipoColabOvosBrasil.Equals("Participa Lista")))
                        ovosBrasil = 0.01m;
                    if (dsCHIC.int_comm[0].invmess3)
                        ovosBrasil = 0.03m;

                    if (!dsCHIC.int_comm[0].IsnfmaeNull())
                        nfMaeAdiantamento = dsCHIC.int_comm[0].nfmae.Trim();

                    erro = 4;

                    booked.FillByOrderNo(dsCHIC.booked, dsCHIC.orders[i].orderno);

                    cust.FillByCustNo(dsCHIC.cust, dsCHIC.orders[i].cust_no);

                    paycodes.FillByCode(dsCHIC.paycodes, dsCHIC.orders[i].pay_terms.ToString());

                    salesman.FillByCod(dsCHIC.salesman, dsCHIC.orders[i].salesrep);

                    erro = 5;

                    string origem = "";
                    string empresa = "";

                    if (dsCHIC.salesman.Rows.Count > 0)
                    {
                        if (dsCHIC.salesman[0].inv_comp.Equals("BR"))
                        {
                            origem = "HYLINE";
                            empresa = "1";
                        }
                        if (dsCHIC.salesman[0].inv_comp.Equals("LB"))
                        {
                            origem = "LOHMANN";
                            empresa = "12";
                        }
                        if (dsCHIC.salesman[0].inv_comp.Equals("HN"))
                        {
                            origem = "H&N";
                            empresa = "15";
                        }
                    }

                    string continua = "Sim";
                    if (nfMaeAdiantamento != "")
                    {
                        nfMaeAdiantamento = nfMaeAdiantamento.Replace(".", "");
                        int tamanho = nfMaeAdiantamento.Length;
                        int qtdCompleta = 10 - tamanho;
                        StringBuilder completa = new StringBuilder().Insert(0, "0", qtdCompleta);
                        string nfMae = completa + nfMaeAdiantamento;

                        int existe = 0;
                        existe = apolo.NOTA_FISCAL
                            .Where(w => w.EmpCod == empresa
                                && w.CtrlDFModForm == "NF-e"
                                && w.NFNum == nfMae
                                && w.EntCod == custno)
                            .Count();

                        if (existe == 0)
                        {
                            continua = "Não";

                            ENTIDADE entidade = apolo.ENTIDADE
                                .Where(e => e.EntCod == custno)
                                .First();

                            WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                            apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                            email.WorkFlowEmailStat = "Enviar";
                            email.WorkFlowEmailAssunto = "**** PEDIDO " + dsCHIC.orders[i].orderno + " COM NF MÃE INCORRETA ****";
                            email.WorkFlowEmailData = DateTime.Now;
                            email.WorkFlowEmailParaNome = "Programação";
                            email.WorkFlowEmailParaEmail = "programacao@hyline.com.br";
                            email.WorkFlowEmailCopiaPara = "llopes@hyline.com.br";
                            email.WorkFlowEmailDeNome = "Serviço de Importação";
                            email.WorkFLowEmailDeEmail = "sistema@hyline.com.br";
                            email.WorkFlowEmailFormato = "Texto";

                            string corpoEmail = "";

                            corpoEmail = "O pedido " + dsCHIC.orders[i].orderno + " do cliente " + entidade.EntNome
                                + " está com a Nota Fiscal Mãe de Adiantamento incorreta (" + nfMae + ")."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "Realize o ajuste e faça a importação manual, pois esse pedido não será importado automaticamente." + (char)13 + (char)10
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA DE IMPORTAÇÃO CHIC / APOLO";

                            email.WorkFlowEmailCorpo = corpoEmail;

                            apolo.WORKFLOW_EMAIL.AddObject(email);

                            apolo.SaveChanges();
                        }
                    }

                    if (continua == "Sim")
                    {
                        origemErro = origem;

                        erro = 6;

                        string condPag = "";
                        if (dsCHIC.orders[i].delivery.IndexOf("(") > 0)
                            condPag = (dsCHIC.orders[i].delivery.Substring(0, (dsCHIC.orders[i].delivery.IndexOf("(") - 1))).Trim();
                        else
                            condPag = dsCHIC.orders[i].delivery.Trim();

                        erro = 7;

                        for (int j = 0; j < dsCHIC.booked.Rows.Count; j++)
                        {
                            produtoErro = dsCHIC.booked[j].accountno;

                            erro = 8;

                            items.FillByItemNo(dsCHIC.items, dsCHIC.booked[j].item);

                            erro = 9;

                            erro = 10;

                            apolo.SaveChanges();

                            erro = 11;
                        }
                        //}

                        #region Gera confirmações e envia e-mail para o Faturamento

                        string destino = GeraRelConfirmacao(dsCHIC.orders[i].orderno, dsCHIC.cust[0].name.Trim(),
                            dsCHIC.salesman[0].inv_comp.Trim());

                        if (anexos == "")
                            anexos = destino;
                        else
                            anexos = anexos + "^" + destino;

                        #endregion
                    }
                    //}
                }

                #region Envia e-mail das confirmações para o Faturamento

                if (dsCHIC.orders.Rows.Count > 0)
                {
                    string assuntoEmail = "CONFIRMAÇÕES PARA FATURAMENTO DO DIA " + DateTime.Today.ToString("dd/MM/yyyy");

                    string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue em anexo as confirmações para Faturamento do dia " + DateTime.Today.AddDays(1).ToString("dd/MM/yyyy") + "."
                        + (char)13 + (char)10 + (char)13 + (char)10
                        + "Qualquer dúvida, entrar em contato com o Depto. de Programação." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    EnviaConfirmacaoEmail(anexos, "faturamento@hyline.com.br", "FATURAMENTO",
                        "", "", "", corpoEmail, assuntoEmail, "5");
                }

                #endregion
            }
            catch (Exception e)
            {
                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** ERRO ENVIO AUTOMÁTICO DE PEDIDOS CHIC P/ APOLO ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Paulo Alves";
                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistema@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                string corpoEmail = "";

                string innerException = "";

                if (e.InnerException != null)
                {
                    innerException = e.InnerException.Message;
                }

                corpoEmail = "Erro ao realizar Envio Automático de Pedidos do CHIC p/ o Apolo: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "Linha do Erro: " + erro.ToString() + (char)13 + (char)10
                    + "Número do Pedido CHIC: " + numPedidoCHIC + (char)13 + (char)10
                    + "Origem: " + origemErro + (char)13 + (char)10
                    + "Produto: " + produtoErro + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 1: " + e.Message + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 2: " + innerException;

                email.WorkFlowEmailCorpo = corpoEmail;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                this.EventLog.WriteEntry("Erro ao realizar Envio de Clientes no CHIC: " + e.Message, EventLogEntryType.Error, 10);
            }
        }

        public string ImportaPedidosCHIC(string numPedido)
        {
            string erroRetorno = "";

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                //string teste2 = "30/60/90 DDL (Não aplicar Bouba)";
                //string teste = (teste2.Substring(0, (teste2.IndexOf("(")-1))).Trim();

                //orders.FillByStatus(dsCHIC.orders, "SENT");
                orders.FillByNumero(dsCHIC.orders, numPedido);
                //orders.FillByNumero(dsCHIC.orders, "30042");

                string nfMaeAdiantamento = "";

                for (int i = 0; i < dsCHIC.orders.Rows.Count; i++)
                {
                    //if ((dsCHIC.orders[i].orderno.Equals("47425"))
                    //    ||
                    //   (dsCHIC.orders[i].orderno.Equals("45885")))
                    //{

                    intcomm.FillByOrderNo(dsCHIC.int_comm, dsCHIC.orders[i].orderno);

                    decimal ovosBrasil = 0;

                    if (dsCHIC.int_comm[0].invmess)
                        ovosBrasil = 0.05m;
                    if (dsCHIC.int_comm[0].invmess1)
                        ovosBrasil = 0.01m;
                    if (dsCHIC.int_comm[0].invmess3)
                        ovosBrasil = 0.03m;

                    if (!dsCHIC.int_comm[0].IsnfmaeNull())
                        nfMaeAdiantamento = dsCHIC.int_comm[0].nfmae.Trim();

                    booked.FillByOrderNo(dsCHIC.booked, dsCHIC.orders[i].orderno);

                    var listaItens = dsCHIC.booked
                        .OrderBy(o => o.item_ord).ThenBy(t => t.alt_desc)
                        .ToList();

                    cust.FillByCustNo(dsCHIC.cust, dsCHIC.orders[i].cust_no);

                    paycodes.FillByCode(dsCHIC.paycodes, dsCHIC.orders[i].pay_terms.ToString());

                    salesman.FillByCod(dsCHIC.salesman, dsCHIC.orders[i].salesrep);

                    string origem = "";

                    if (dsCHIC.salesman.Rows.Count > 0)
                    {
                        if (dsCHIC.salesman[0].inv_comp.Equals("BR"))
                            origem = "HYLINE";
                        if (dsCHIC.salesman[0].inv_comp.Equals("LB"))
                            origem = "LOHMANN";
                        if (dsCHIC.salesman[0].inv_comp.Equals("HN"))
                            origem = "H&N";
                        if (dsCHIC.salesman[0].inv_comp.Equals("PL"))
                            origem = "PLANALTO";
                    }

                    #region Carrega dados da empresa do Apolo

                    var codigoEmpresaCHIC = dsCHIC.salesman[0].inv_comp.Trim();
                    var codigoIncubatorioFLIP = listaItens.FirstOrDefault().location.Trim();
                    origem = codigoEmpresaCHIC + "-" + codigoIncubatorioFLIP;

                    #endregion

                    string condPag = "";
                    if (dsCHIC.orders[i].delivery.IndexOf("(") > 0)
                        condPag = (dsCHIC.orders[i].delivery.Substring(0, (dsCHIC.orders[i].delivery.IndexOf("(") - 1))).Trim();
                    else
                        condPag = dsCHIC.orders[i].delivery.Trim();

                    #region Valor Total do Pintinho

                    decimal valorTotalPintinho = 0;
                    string orderno = dsCHIC.orders[i].orderno.Trim();

                    Pedido_Venda pedVenda =
                        hlbappService.Pedido_Venda.Where(w => hlbappService.Item_Pedido_Venda
                            .Any(it => it.IDPedidoVenda == w.ID
                                && (it.OrderNoCHIC == orderno || it.OrderNoCHICReposicao == orderno)))
                            .FirstOrDefault();

                    if (pedVenda != null)
                    {
                        valorTotalPintinho = Convert.ToDecimal(pedVenda.ValorTotalPintinho);
                    }

                    #endregion

                    for (int j = 0; j < listaItens.Count; j++)
                    {
                        //if (listaItens[j].location.Trim().Equals("NM"))
                        //{
                        //    if (dsCHIC.salesman[0].inv_comp.Equals("LB"))
                        //    {
                        //        origem = "LOHMANN-PL";
                        //    }
                        //}

                        items.FillByItemNo(dsCHIC.items, listaItens[j].item);

                        apolo.ImportaPedidosCHIC(
                            dsCHIC.orders[i].cust_no,
                            dsCHIC.orders[i].orderno,
                            (listaItens[j].cal_date.AddDays(21)),
                            dsCHIC.orders[i].del_date,
                            0,
                            0,
                            dsCHIC.orders[i].pay_terms.ToString(),
                            listaItens[j].modifdby,
                            dsCHIC.cust[0].name,
                            condPag,
                            "",
                            dsCHIC.cust[0].street_1,
                            "",
                            "",
                            dsCHIC.cust[0].street_2,
                            dsCHIC.cust[0].city,
                            dsCHIC.cust[0].state,
                            dsCHIC.cust[0].country,
                            dsCHIC.orders[i].status,
                            listaItens[j].accountno,
                            //"",
                            dsCHIC.items[0].item_desc.Trim(),
                            listaItens[j].alt_desc.Trim(),
                            listaItens[j].quantity,
                            listaItens[j].price,
                            (listaItens[j].quantity * listaItens[j].price),
                            DateTime.Now,
                            (j + 1),
                            "0" + dsCHIC.orders[i].salesrep,
                            origem,
                            "",
                            ovosBrasil,
                            nfMaeAdiantamento,
                            valorTotalPintinho);
                    }
                    //}
                }
                return erroRetorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erroRetorno = "Erro ao Atualizar CHIC com Embarcador - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    erroRetorno = erroRetorno + " Erro Secundário: " + ex.InnerException.Message;

                return erroRetorno;
            }
        }

        public string ImportaClientesAPOLO()
        {
            string erro = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                var lista = apolo.ENTIDADE
                    .Where(e => e.EntCod != "" && //e.EntCod == "0009924" &&
                        apolo.LOG_ENTIDADE.Any(l => l.USEREXPORTADOCHIC == "Não" &&
                            (l.LogEntOper == "Alteração" || l.LogEntOper == "Inclusão" || l.LogEntOper == "Inscrição") && e.EntCod == l.LogEntCod))
                    .Join(
                        apolo.ENT_CATEG.Where(c => c.CategCodEstr == "01" || c.CategCodEstr == "01.01"),
                        e => e.EntCod,
                        c => c.EntCod,
                        (e, c) => new { ENTIDADE = e, ENT_CATEG = c })
                    .Join(
                        apolo.VEND_ENT.Where(v => v.VendEntPrinc == "Sim"),
                        ec => ec.ENTIDADE.EntCod,
                        v => v.EntCod,
                        (ec, v) => new { ec.ENTIDADE, ec.ENT_CATEG, VEND_ENT = v })
                    .Join(
                        apolo.ENTIDADE1,
                        e => e.ENTIDADE.EntCod,
                        e1 => e1.EntCod,
                        (e, e1) => new { e.ENTIDADE, e.ENT_CATEG, e.VEND_ENT, ENTIDADE1 = e1 })
                    .GroupJoin(
                        apolo.ENT_FONE.Where(f => f.EntFonePrinc == "Sim"),
                        ef => ef.ENTIDADE.EntCod,
                        f => f.EntCod,
                        (ef, f) => new { ef.ENTIDADE, ef.ENT_CATEG, ef.VEND_ENT, ef.ENTIDADE1, ENT_FONE = f })
                            .SelectMany(n => n.ENT_FONE.DefaultIfEmpty(),
                                        (n, f) => new { n.ENTIDADE, n.ENT_CATEG, n.VEND_ENT, n.ENTIDADE1, ENT_FONE = f })
                    .GroupJoin(
                        apolo.ENT_FONE.Where(f => f.EntFoneTipo == "Fax"),
                        ef => ef.ENTIDADE.EntCod,
                        f => f.EntCod,
                        (ef, f) => new { ef.ENTIDADE, ef.ENT_CATEG, ef.VEND_ENT, ef.ENTIDADE1, ef.ENT_FONE, ENT_FONE_FAX = f })
                            .SelectMany(n => n.ENT_FONE_FAX.DefaultIfEmpty(),
                                        (n, f) => new { n.ENTIDADE, n.ENT_CATEG, n.VEND_ENT, n.ENTIDADE1, n.ENT_FONE, ENT_FONE_FAX = f })
                    .GroupJoin(
                        apolo.CIDADE,
                        ecid => ecid.ENTIDADE.CidCod,
                        c => c.CidCod,
                        (ecid, c) => new { ecid.ENTIDADE, ecid.ENT_CATEG, ecid.VEND_ENT, ecid.ENTIDADE1, ecid.ENT_FONE, ecid.ENT_FONE_FAX, CIDADE = c })
                             .SelectMany(n => n.CIDADE.DefaultIfEmpty(),
                                        (n, c) => new { n.ENTIDADE, n.ENT_CATEG, n.VEND_ENT, n.ENTIDADE1, n.ENT_FONE, n.ENT_FONE_FAX, CIDADE = c })
                    .Select(c => new
                    {
                        c.ENTIDADE.EntCod,
                        c.ENTIDADE.EntNome,
                        c.ENTIDADE.EntLograd,
                        c.ENTIDADE.EntEnder,
                        c.ENTIDADE.EntEnderNo,
                        c.ENTIDADE.EntEnderComp,
                        c.ENTIDADE.EntBair,
                        c.ENTIDADE.EntTipoFJ,
                        c.CIDADE.CidNomeComp,
                        c.CIDADE.UfSigla,
                        c.CIDADE.PaisSigla,
                        c.ENTIDADE.EntCep,
                        c.VEND_ENT.VendCod,
                        c.ENT_FONE.EntFoneDDD,
                        c.ENT_FONE.EntFoneNum,
                        DDDFax = c.ENT_FONE_FAX.EntFoneDDD,
                        NumFax = c.ENT_FONE_FAX.EntFoneNum,
                        c.ENTIDADE.EntCpfCgc,
                        c.ENTIDADE.EntRgIe,
                        c.ENTIDADE.EntAgrop,
                        c.ENTIDADE.EntAgropInsc,
                        c.ENTIDADE1.USERTipoColabOvosBRasil,
                        c.ENTIDADE1.USERCodigoEstabelecimento,
                        c.ENTIDADE1.USERNumProtocolo,
                        c.ENTIDADE1.USERNumRegistro,
                        c.ENTIDADE1.USERNumRegistroValidade
                    }).ToList();

                foreach (var item in lista)
                {
                    // Atualizando CHIC Comercial
                    cust.FillByCustNo(dsCHIC.cust, item.EntCod);
                    //cust.FillByCustNo(dsCHIC.cust, "0009998");

                    PAIS pais = apolo.PAIS.Where(w => w.PaisSigla == item.PaisSigla).FirstOrDefault();

                    string ie = "";
                    if (item.EntAgrop == "Sim")
                        if (item.EntAgropInsc != null)
                            ie = item.EntAgropInsc;
                    else
                        if (item.EntRgIe != null) ie = item.EntRgIe;
                    string cidadeNomeComp = "";
                    if (item.CidNomeComp != null) cidadeNomeComp = item.CidNomeComp;
                    string uf = "";
                    if (item.UfSigla != null) uf = item.UfSigla;
                    string paisSigla = "";
                    if (item.PaisSigla != null) paisSigla = item.PaisSigla;
                    string paisNome = "";
                    if (pais != null) paisNome = pais.PaisNome;

                    string codEstabelecimento = "";
                    if (item.USERCodigoEstabelecimento != null) codEstabelecimento = item.USERCodigoEstabelecimento;
                    string numProtocolo = "";
                    if (item.USERNumProtocolo != null) numProtocolo = item.USERNumProtocolo;
                    string numRegistro = "";
                    if (item.USERNumRegistro != null) numRegistro = item.USERNumRegistro;
                    DateTime? dateRegistro = null;
                    if (item.USERNumRegistroValidade != null) 
                        dateRegistro = item.USERNumRegistroValidade;

                    if (dsCHIC.cust.Rows.Count == 0)
                    {
                        cust.Insert(item.EntCod,
                                    item.EntNome,
                                    ((item.EntLograd == null) ? string.Empty : item.EntLograd) + ". " + ((item.EntEnder == null) ? string.Empty : item.EntEnder) + ((item.EntEnderNo == null) ? string.Empty : ", " + item.EntEnderNo),
                                    ((item.EntEnderComp == null) ? string.Empty : item.EntEnderComp + " - ") + ((item.EntBair == null) ? string.Empty : item.EntBair),
                                    cidadeNomeComp,
                                    uf,
                                    ((item.EntCep == null) ? string.Empty : item.EntCep),
                                    paisSigla,
                            //pais.PaisNome,
                                    ((item.VendCod == null) ? string.Empty : item.VendCod.Substring(1, 6)),
                                    ((item.EntFoneDDD == null) ? string.Empty : "(" + item.EntFoneDDD + ") ") + item.EntFoneNum,
                                    ((item.DDDFax == null) ? string.Empty : "(" + item.DDDFax + ") ") + item.NumFax,
                                    ((item.EntCpfCgc == null || item.EntCpfCgc == "") ? string.Empty : ((item.EntTipoFJ == "Física") ? item.EntCpfCgc.Substring(0, 3) + "." + item.EntCpfCgc.Substring(3, 3) + "." + item.EntCpfCgc.Substring(6, 3) + "-" + item.EntCpfCgc.Substring(9, 2) : item.EntCpfCgc.Substring(0, 2) + "." + item.EntCpfCgc.Substring(2, 3) + "." + item.EntCpfCgc.Substring(5, 3) + "/" + item.EntCpfCgc.Substring(8, 4) + "-" + item.EntCpfCgc.Substring(12, 2))),
                                    ie,
                                    string.Empty);

                        bool participaListaIOB = false;

                        string tipoColabOvosBrasil = "";

                        if (item.USERTipoColabOvosBRasil != null)
                        {
                            tipoColabOvosBrasil = item.USERTipoColabOvosBRasil;
                        }

                        if (tipoColabOvosBrasil.Equals("Participa Lista"))
                        {
                            participaListaIOB = true;
                        }

                        custcust.Insert1(item.EntCod, 0, 0, participaListaIOB, codEstabelecimento, numProtocolo, 
                            numRegistro, dateRegistro);

                        #region Endereco de Entrega

                        var listaEnderecoEntrega = apolo.ENDER_ENT
                            .Where(w => w.EntCod == item.EntCod && w.EnderEntEntrega == "Sim")
                            .ToList();

                        foreach (var endEntrega in listaEnderecoEntrega)
                        {
                            shippingTableAdapter sTA = new shippingTableAdapter();
                            CHICDataSet.shippingDataTable sDT = new CHICDataSet.shippingDataTable();
                            //sTA.FillByCustNo(sDT, endEntrega.EntCod);
                            decimal contactno = 0;
                            //if (sDT.Count > 0)
                            //    contactno = sDT.Max(m => m.contact_no) + 1;
                            //else
                            //    contactno = 1;
                            contactno = endEntrega.EnderEntSeq;

                            CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == endEntrega.CidCod).FirstOrDefault();
                            string cidStr = "";
                            if (cidade != null) cidStr = cidade.CidNomeComp + " / " + cidade.UfSigla;

                            string enderecoStr = "";
                            string numeroStr = "";
                            string bairroStr = "";
                            string compStr = "";
                            string cnpjEE = "";
                            string ieEE = "";
                            if (endEntrega.EnderEntCod == null)
                            {
                                enderecoStr = ((endEntrega.EnderEntNome == null) ? string.Empty : endEntrega.EnderEntNome);
                                numeroStr = endEntrega.EnderEnt + " Nº " + endEntrega.EnderEntNo;
                                bairroStr = ((endEntrega.EnderEntBair == null) ? string.Empty
                                    : endEntrega.EnderEntBair);
                                compStr = ((endEntrega.EnderEntComp == null) ? string.Empty
                                    : endEntrega.EnderEntComp);
                            }
                            else
                            {
                                ENTIDADE entidadeEE = apolo.ENTIDADE
                                    .Where(w => w.EntCod == endEntrega.EnderEntCod)
                                    .FirstOrDefault();

                                if (entidadeEE != null)
                                {
                                    enderecoStr = ((entidadeEE.EntNome == null) ? string.Empty : entidadeEE.EntCod + " - " + entidadeEE.EntNome);
                                    numeroStr = entidadeEE.EntEnder + " Nº " + entidadeEE.EntEnderNo;
                                    bairroStr = ((entidadeEE.EntBair == null) ? string.Empty
                                        : entidadeEE.EntBair);
                                    compStr = ((entidadeEE.EntEnderComp == null) ? string.Empty
                                        : entidadeEE.EntEnderComp);

                                    if (entidadeEE.EntTipoFJ == "Física")
                                        cnpjEE = "CPF: " + Convert.ToUInt64(entidadeEE.EntCpfCgc).ToString(@"000\.000\.000\-00");
                                    else
                                        cnpjEE = "CNPJ: " + Convert.ToUInt64(entidadeEE.EntCpfCgc).ToString(@"00\.000\.000\/0000\-00");

                                    if (entidadeEE.EntRgIe == "" || entidadeEE.EntRgIe == null)
                                        ieEE = "IE: " + entidadeEE.EntAgropInsc;
                                    else
                                        ieEE = "IE: " + entidadeEE.EntRgIe;
                                }
                            }

                            sTA.Insert(endEntrega.EntCod, contactno,
                                enderecoStr,
                                numeroStr,
                                bairroStr + " - " + compStr,
                                cidStr,
                                cnpjEE,
                                ieEE, "", "", "", "", "", "");
                        }

                        #endregion
                    }
                    else
                    {
                        cust.UpdateQuery(item.EntNome,
                                    ((item.EntLograd == null) ? string.Empty : item.EntLograd) + ". " + ((item.EntEnder == null) ? string.Empty : item.EntEnder) + ((item.EntEnderNo == null) ? string.Empty : ", " + item.EntEnderNo),
                                    ((item.EntEnderComp == null) ? string.Empty : item.EntEnderComp + " - ") + ((item.EntBair == null) ? string.Empty : item.EntBair),
                                    cidadeNomeComp,
                                    uf,
                                    ((item.EntCep == null) ? string.Empty : item.EntCep),
                                    paisSigla,
                            //pais.PaisNome,
                                    ((item.VendCod == null) ? string.Empty : item.VendCod.Substring(1, 6)),
                                    ((item.EntFoneDDD == null) ? string.Empty : "(" + item.EntFoneDDD + ") ") + item.EntFoneNum,
                                    ((item.DDDFax == null) ? string.Empty : "(" + item.DDDFax + ") ") + item.NumFax,
                                    ((item.EntCpfCgc == null || item.EntCpfCgc == "") ? string.Empty : ((item.EntTipoFJ == "Física") ? item.EntCpfCgc.Substring(0, 3) + "." + item.EntCpfCgc.Substring(3, 3) + "." + item.EntCpfCgc.Substring(6, 3) + "-" + item.EntCpfCgc.Substring(9, 2) : item.EntCpfCgc.Substring(0, 2) + "." + item.EntCpfCgc.Substring(2, 3) + "." + item.EntCpfCgc.Substring(5, 3) + "/" + item.EntCpfCgc.Substring(8, 4) + "-" + item.EntCpfCgc.Substring(12, 2))),
                                    ie,
                                    string.Empty, item.EntCod);

                        bool participaListaIOB = false;

                        string tipoColabOvosBrasil = "";

                        if (item.USERTipoColabOvosBRasil != null)
                        {
                            tipoColabOvosBrasil = item.USERTipoColabOvosBRasil;
                        }

                        if (tipoColabOvosBrasil.Equals("Participa Lista"))
                        {
                            participaListaIOB = true;
                        }

                        int existe = 0;

                        custcust.FillByCustNo(dsCHIC.custcust, item.EntCod);

                        existe = dsCHIC.custcust.Count();

                        if (existe > 0)
                        {
                            custcust.UpdateQuery(participaListaIOB, codEstabelecimento, numProtocolo, 
                                numRegistro, dateRegistro, item.EntCod);
                        }
                        else
                        {
                            custcust.Insert1(item.EntCod, 0, 0, participaListaIOB, codEstabelecimento, 
                                numProtocolo, numRegistro, dateRegistro);
                        }

                        #region Endereco de Entrega

                        var listaEnderecoEntrega = apolo.ENDER_ENT
                            .Where(w => w.EntCod == item.EntCod && w.EnderEntEntrega == "Sim"
                                && w.EnderEntDataValFinal == null)
                            .GroupBy(g => new
                            {
                                g.EntCod,
                                g.EnderEntNome,
                                g.EnderEnt,
                                g.EnderEntNo,
                                g.EnderEntBair,
                                g.EnderEntComp,
                                g.CidCod,
                                g.EnderEntCod
                            })
                            .Select(s => new
                            {
                                s.Key.EntCod,
                                s.Key.EnderEntNome,
                                s.Key.EnderEnt,
                                s.Key.EnderEntNo,
                                s.Key.EnderEntBair,
                                s.Key.EnderEntComp,
                                s.Key.CidCod,
                                s.Key.EnderEntCod,
                                EnderEntSeq = s.Min(m => m.EnderEntSeq)
                            })
                            .ToList();

                        shippingTableAdapter sTA = new shippingTableAdapter();
                        CHICDataSet.shippingDataTable sDT = new CHICDataSet.shippingDataTable();
                        sTA.DeleteByCustNo(item.EntCod);

                        foreach (var endEntrega in listaEnderecoEntrega)
                        {
                            sTA.FillByCustNo(sDT, endEntrega.EntCod);
                            CHICDataSet.shippingRow chicEE = 
                                //sDT.Where(w => w.address1.Trim().Contains(endEntrega.EnderEnt))
                                sDT.Where(w => w.contact_no == endEntrega.EnderEntSeq)
                                .FirstOrDefault();

                            string cidStr = "";
                            string enderecoStr = "";
                            string numeroStr = "";
                            string bairroStr = "";
                            string compStr = "";
                            string cnpjEE = "";
                            string ieEE = "";
                            if (endEntrega.EnderEntCod == null)
                            {
                                CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == endEntrega.CidCod)
                                    .FirstOrDefault();
                                if (cidade != null) cidStr = cidade.CidNomeComp + " / " + cidade.UfSigla;

                                enderecoStr = ((endEntrega.EnderEntNome == null) ? string.Empty : endEntrega.EnderEntNome);
                                numeroStr = endEntrega.EnderEnt + " Nº " + endEntrega.EnderEntNo;
                                bairroStr = ((endEntrega.EnderEntBair == null) ? string.Empty
                                    : endEntrega.EnderEntBair);
                                compStr = ((endEntrega.EnderEntComp == null) ? string.Empty
                                    : endEntrega.EnderEntComp);
                            }
                            else
                            {
                                ENTIDADE entidadeEE = apolo.ENTIDADE
                                    .Where(w => w.EntCod == endEntrega.EnderEntCod)
                                    .FirstOrDefault();

                                if (entidadeEE != null)
                                {
                                    CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidadeEE.CidCod)
                                        .FirstOrDefault();
                                    if (cidade != null) cidStr = cidade.CidNomeComp + " / " + cidade.UfSigla;

                                    enderecoStr = ((entidadeEE.EntNome == null) ? string.Empty : entidadeEE.EntCod + " - " + entidadeEE.EntNome);
                                    numeroStr = entidadeEE.EntEnder + " Nº " + entidadeEE.EntEnderNo;
                                    bairroStr = ((entidadeEE.EntBair == null) ? string.Empty
                                        : entidadeEE.EntBair);
                                    compStr = ((entidadeEE.EntEnderComp == null) ? string.Empty
                                        : entidadeEE.EntEnderComp);

                                    if (entidadeEE.EntTipoFJ == "Física")
                                        cnpjEE = "CPF: " + Convert.ToUInt64(entidadeEE.EntCpfCgc).ToString(@"000\.000\.000\-00");
                                    else
                                        cnpjEE = "CNPJ: " + Convert.ToUInt64(entidadeEE.EntCpfCgc).ToString(@"00\.000\.000\/0000\-00");

                                    if (entidadeEE.EntRgIe == "" || entidadeEE.EntRgIe == null)
                                        ieEE = "IE: " + entidadeEE.EntAgropInsc;
                                    else
                                        ieEE = "IE: " + entidadeEE.EntRgIe;
                                }
                            }

                            if (chicEE == null)
                            {
                                //decimal contactno = 0;
                                //if (sDT.Count > 0)
                                //    contactno = sDT.Max(m => m.contact_no) + 1;
                                //else
                                //    contactno = 1;
                                decimal contactno = endEntrega.EnderEntSeq;

                                sTA.Insert(endEntrega.EntCod, contactno,
                                    enderecoStr,
                                    numeroStr,
                                    bairroStr + " - " + compStr,
                                    cidStr,
                                    cnpjEE,
                                    ieEE, "", "", "", "", "", "");
                            }
                            else
                            {
                                chicEE.name = enderecoStr;
                                chicEE.address1 = numeroStr;
                                chicEE.address2 = bairroStr + " - " + compStr;
                                chicEE.address3 = cidStr;
                                chicEE.address4 = cnpjEE;
                                chicEE.address5 = ieEE;
                                sTA.Update(chicEE);
                            }
                        }

                        #endregion
                    }

                    // Atualizando CHIC Parents
                    custPAR.FillByCustoNo(dsCHICPAR.cust, item.EntCod);

                    if (dsCHICPAR.cust.Rows.Count == 0)
                    {
                        custPAR.Insert(item.EntCod,
                                    item.EntNome,
                                    ((item.EntLograd == null) ? string.Empty : item.EntLograd) + ". " + ((item.EntEnder == null) ? string.Empty : item.EntEnder) + ((item.EntEnderNo == null) ? string.Empty : ", " + item.EntEnderNo),
                                    ((item.EntEnderComp == null) ? string.Empty : item.EntEnderComp + " - ") + ((item.EntBair == null) ? string.Empty : item.EntBair),
                                    cidadeNomeComp,
                                    uf,
                                    ((item.EntCep == null) ? string.Empty : item.EntCep),
                            //item.PaisSigla,
                                    paisNome,
                                    ((item.VendCod == null) ? string.Empty : item.VendCod.Substring(1, 6)),
                                    ((item.EntFoneDDD == null) ? string.Empty : "(" + item.EntFoneDDD + ") ") + item.EntFoneNum,
                                    ((item.DDDFax == null) ? string.Empty : "(" + item.DDDFax + ") ") + item.NumFax,
                                    ((item.EntCpfCgc == null || item.EntCpfCgc == "") ? string.Empty : ((item.EntTipoFJ == "Física") ? item.EntCpfCgc.Substring(0, 3) + "." + item.EntCpfCgc.Substring(3, 3) + "." + item.EntCpfCgc.Substring(6, 3) + "-" + item.EntCpfCgc.Substring(9, 2) : item.EntCpfCgc.Substring(0, 2) + "." + item.EntCpfCgc.Substring(2, 3) + "." + item.EntCpfCgc.Substring(5, 3) + "/" + item.EntCpfCgc.Substring(8, 4) + "-" + item.EntCpfCgc.Substring(12, 2))),
                                    string.Empty,
                                    string.Empty);
                    }
                    else
                    {
                        custPAR.UpdateQuery(item.EntNome,
                                    ((item.EntLograd == null) ? string.Empty : item.EntLograd) + ". " + ((item.EntEnder == null) ? string.Empty : item.EntEnder) + ((item.EntEnderNo == null) ? string.Empty : ", " + item.EntEnderNo),
                                    ((item.EntEnderComp == null) ? string.Empty : item.EntEnderComp + " - ") + ((item.EntBair == null) ? string.Empty : item.EntBair),
                                    cidadeNomeComp,
                                    uf,
                                    ((item.EntCep == null) ? string.Empty : item.EntCep),
                            //item.PaisSigla,
                                    paisNome,
                                    ((item.VendCod == null) ? string.Empty : item.VendCod.Substring(1, 6)),
                                    ((item.EntFoneDDD == null) ? string.Empty : "(" + item.EntFoneDDD + ") ") + item.EntFoneNum,
                                    ((item.DDDFax == null) ? string.Empty : "(" + item.DDDFax + ") ") + item.NumFax,
                                    ((item.EntCpfCgc == null || item.EntCpfCgc == "") ? string.Empty : ((item.EntTipoFJ == "Física") ? item.EntCpfCgc.Substring(0, 3) + "." + item.EntCpfCgc.Substring(3, 3) + "." + item.EntCpfCgc.Substring(6, 3) + "-" + item.EntCpfCgc.Substring(9, 2) : item.EntCpfCgc.Substring(0, 2) + "." + item.EntCpfCgc.Substring(2, 3) + "." + item.EntCpfCgc.Substring(5, 3) + "/" + item.EntCpfCgc.Substring(8, 4) + "-" + item.EntCpfCgc.Substring(12, 2))),
                                    string.Empty,
                                    string.Empty, item.EntCod);
                    }

                    var listaLogEntidade = apolo.LOG_ENTIDADE.Where(l => l.LogEntCod == item.EntCod && l.USEREXPORTADOCHIC == "Não");

                    foreach (var logEntidade in listaLogEntidade)
                    {
                        #region Pesquisa no Google Maps e insere Longitude e Latitude

                        //ENTIDADE1 entidade1 = apolo.ENTIDADE1
                        //        .Where(e => e.EntCod == item.EntCod)
                        //        .FirstOrDefault();

                        //if ((entidade1.EntLatitudeDecimal == 0) ||
                        //    (entidade1.EntLatitudeDecimal == null) ||
                        //    (entidade1.EntLongitudeDecimal == 0) ||
                        //    (entidade1.EntLongitudeDecimal == null))
                        //{
                        //    List<string> localizacao = LocalizacaoGoogleMaps(item.CidNomeComp);

                        //    if (localizacao != null)
                        //    {
                        //        entidade1.EntLatitudeDecimal = Convert.ToDecimal(localizacao[0].Replace(".", ","));
                        //        entidade1.EntLongitudeDecimal = Convert.ToDecimal(localizacao[1].Replace(".", ","));
                        //    }
                        //}

                        #endregion

                        logEntidade.USEREXPORTADOCHIC = "Sim";
                    }

                    apolo.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erro = linenum.ToString();
            }

            return erro;
        }

        public void ImportaCondicoesRecebimentoAPOLO()
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            var lista = apolo.COND_PAG
                            .Where(c => c.CondPagOper == "Receber" && c.CondPagCod.Length == 4)
                            .Select(c => new
                            {
                                c.CondPagCod,
                                c.CondPagNome
                            });

            foreach (var item in lista)
            {
                paycodes.FillByCode(dsCHIC.paycodes, item.CondPagCod);

                if (dsCHIC.paycodes.Rows.Count == 0)
                {
                    paycodes.Insert(item.CondPagCod, item.CondPagNome);
                }
                else
                {
                    paycodes.UpdateQuery(item.CondPagNome, item.CondPagCod);
                }
            }
        }

        public void AtualizaEnderecoEntregaCHICtoApolo()
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            custTableAdapter cTA = new custTableAdapter();
            CHICDataSet.custDataTable cDT = new CHICDataSet.custDataTable();
            cTA.FillWithShippingAddress(cDT);

            var listaEntidadeCHIC = cDT
                .Where(w => w.custno.Trim() == "0001397")
                .ToList();

            foreach (var item in listaEntidadeCHIC)
            {
                string entCod = item.custno.Trim();

                int existeEntidadeApolo = apolo.ENTIDADE.Where(w => w.EntCod == entCod).Count();

                if (existeEntidadeApolo > 0)
                {
                    List<ENDER_ENT> listEndEntrega = apolo.ENDER_ENT
                        .Where(w => w.EntCod == entCod && w.EnderEntEntrega == "Sim")
                        .ToList();

                    foreach (var delEnderEntEntrega in listEndEntrega)
                    {
                        int existeNF = 0;

                        existeNF = apolo.NOTA_FISCAL
                            .Where(w => w.EntCod == delEnderEntEntrega.EntCod
                                && w.NFEnderEntSeqEntrega == delEnderEntEntrega.EnderEntSeq)
                            .Count();

                        if (existeNF == 0)
                        {
                            int existePV = 0;

                            existePV = apolo.PED_VENDA
                                .Where(w => w.EntCod == delEnderEntEntrega.EntCod
                                    && w.PedVendaEnderEntSeqEntrega == delEnderEntEntrega.EnderEntSeq)
                                .Count();

                            if (existePV == 0)
                                apolo.ENDER_ENT.DeleteObject(delEnderEntEntrega);
                        }
                    }

                    shippingTableAdapter sTA = new shippingTableAdapter();
                    CHICDataSet.shippingDataTable sDT = new CHICDataSet.shippingDataTable();
                    sTA.FillByCustNo(sDT, entCod);

                    var listaShip = sDT.OrderBy(o => o.contact_no).ToList();

                    //foreach (var shipItem in sDT)
                    //{
                    //    ENDER_ENT enderEnt = new ENDER_ENT();
                    //    enderEnt.EntCod = entCod;
                    //    enderEnt.EnderEntSeq = Convert.ToInt16(shipItem.contact_no);
                    //    enderEnt.EnderEntEntrega = "Sim";
                    //    enderEnt.EnderEntCobranca = "Não";
                    //    enderEnt.EnderEntNome = shipItem.name.Trim();
                    //    enderEnt.EnderEnt = shipItem.address1.Trim();
                    //    enderEnt.EnderEntTipoFJ = "Jurídica";
                    //    enderEnt.EnderEntFaturam = "Não";
                    //    enderEnt.EnderEntColeta = "Não";
                    //    enderEnt.EnderEntCertificado = "Não";
                    //    enderEnt.EnderEntDataValInic = Convert.ToDateTime("2000-01-01 00:00:00.000");
                    //    enderEnt.EnderEntTexto = shipItem.address2.Trim() + (char)13 + (char)10
                    //        + shipItem.address3.Trim() + (char)13 + (char)10
                    //        + shipItem.address4.Trim() + (char)13 + (char)10
                    //        + shipItem.address5.Trim();

                    //    apolo.ENDER_ENT.AddObject(enderEnt);
                    //}
                }
            }

            apolo.SaveChanges();
        }

        #endregion

        #region CHIC

        public void AtulizaStatusPedidoCHIC()
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            DateTime data = Convert.ToDateTime("01/06/2013");

            var lista2 = apolo.PED_VENDA
                    .Where(p => p.StatPedVendaCod == "08" && p.PedVendaDataEntrega >= data)// && p.EmpCod == "1" && p.PedVendaNum == "0040149")
                    .Join(
                        apolo.PED_VENDA1.Where(p1 => p1.USERPEDCHIC != null),
                        p => p.EmpCod + "|" + p.PedVendaNum,
                        p1 => p1.EmpCod + "|" + p1.PedVendaNum,
                        (p, p1) => new { PED_VENDA = p, PED_VENDA1 = p1 })
                    .Select(p => new
                        {
                            p.PED_VENDA1.USERPEDCHIC
                        }).ToList();

            foreach (var item in lista2)
            {
                orders.FillByNumero(dsCHIC.orders, item.USERPEDCHIC);

                if (dsCHIC.orders.Rows.Count > 0)
                {
                    orders.UpdateStatus("VOID", item.USERPEDCHIC);
                }
            }
        }

        public void EnviaEmailsLogin()
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            salesman1.FillByEmail(dsCHIC.salesman1);

            for (int i = 0; i < dsCHIC.salesman1.Count; i++)
            {
                //if ((!dsCHIC.salesman1[i].email.Trim().Equals("")) && (!dsCHIC.salesman1[i].inv_comp.Trim().Equals("BR")))
                //if (dsCHIC.salesman1[i].email.Trim().Equals("tiago.nascimento.dias@gmail.com"))
                if (!dsCHIC.salesman1[i].email.Trim().Equals(""))
                {
                    string empresa = "";

                    if (dsCHIC.salesman1[i].email.Trim().Equals("BR")) { empresa = "HYLINE DO BRASIL"; }
                    else if (dsCHIC.salesman1[i].email.Trim().Equals("LB")) { empresa = "LOHMANN DO BRASIL"; }
                    else if (dsCHIC.salesman1[i].email.Trim().Equals("HN")) { empresa = "H&N AVICULTURA"; }

                    WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                    ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    //email.WorkFlowEmailAssunto = "**** LOGIN PARA ACESSO AO HY-LINE APP ****";
                    email.WorkFlowEmailAssunto = "**** PLANILHA DE PEDIDO ATUALIZADA ****";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = dsCHIC.salesman1[i].email.Trim();
                    email.WorkFlowEmailParaEmail = dsCHIC.salesman1[i].email.Trim();
                    if (i == (dsCHIC.salesman1.Count - 1)) email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                    //email.WorkFlowEmailParaNome = "Paulo Alves";
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                    email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    string corpoEmail = "";

                    //corpoEmail = "Prezado," + (char)13 + (char)10 + (char)13 + (char)10
                    //    + "Para melhorarmos o controle de nossos processos, foi desenvolvida a ferramenta para preenchimento e importação de Pedidos. " + (char)13 + (char)10
                    //    + "Através dela iremos diminuir os erros para acelerar e melhorar os processos." + (char)13 + (char)10
                    //    + "Sendo assim, segue abaixo o login e senha para acesso ao site para dados da empresa " + empresa + "." + (char)13 + (char)10 + (char)13 + (char)10
                    //    + "Login: " + dsCHIC.salesman1[i].email.Trim() + (char)13 + (char)10
                    //    + "Senha: " + dsCHIC.salesman1[i].senha.Trim() + (char)13 + (char)10 + (char)13 + (char)10
                    //    + "Também, segue em anexo o manual para acesso ao site." + (char)13 + (char)10
                    //    + "Qualquer dúvida, entrar em contato pelo e-mail ti@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                    //    + "SISTEMA WEB";

                    corpoEmail = "Prezado," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Está disponível planilha atualiza com as seguintes correções: " + (char)13 + (char)10 + (char)13 + (char)10
                        + "* Atualização do cálculo para cobrança da Vacina sobre a quantidade de reposição." + (char)13 + (char)10
                        + "Qualquer cliente que não esteja aparecendo, entrar em contato com o responsável de sua empresa." + (char)13 + (char)10
                        + "Qualquer dúvida, entrar em contato pelo e-mail ti@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    //email.WorkFlowEmailArquivosAnexos = "\\\\srv-app-01\\W\\Relatorios_CHIC\\Manual_Formulario_Pedidos.pdf";

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();
                }
            }
        }

        public void AtualizaPrecoPedidos()
        {
            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            orders.FillOrdersMaior04052015(dsCHIC.orders);

            foreach (var pedido in dsCHIC.orders)
            {
                #region Variáveis do Pedido

                decimal valorUnitarioVacina = 0;

                salesman.FillByCod(dsCHIC.salesman, pedido.salesrep);

                string empresa = "BR";
                CHICDataSet.salesmanRow vendedor = null;
                if (dsCHIC.salesman.Count > 0)
                {
                    vendedor = dsCHIC.salesman[0];
                    empresa = vendedor.inv_comp;
                }

                string tipoPagamento = pedido.delivery.Trim() == "PAGTO ANTECIPADO" ? "PAGTO ANTECIPADO" : "Faturamento";
                string codEntidade = pedido.cust_no;

                UNID_FEDERACAO uf = apolo.UNID_FEDERACAO
                    .Where(u => apolo.CIDADE.Any(c => c.UfSigla == u.UfSigla
                            && apolo.ENTIDADE.Any(e => e.CidCod == c.CidCod
                                && e.EntCod == codEntidade)))
                    .FirstOrDefault();

                #endregion

                #region Vacinas (Salmonela / Coccidiose)

                CHICDataSet.bookedDataTable vacinas = new CHICDataSet.bookedDataTable();
                booked.FillByVariety(vacinas, "VACC", pedido.orderno);

                foreach (var item in vacinas)
                {
                    items.FillByItemNo(dsCHIC.items, item.item);
                    CHICDataSet.itemsRow linhagem = null;
                    if (dsCHIC.items.Count > 0)
                        linhagem = dsCHIC.items[0];

                    DateTime dataNascimento = item.cal_date.AddDays(21);

                    Tabela_Precos tabelaPreco = hlbappService.Tabela_Precos
                        .Where(w => w.Empresa == empresa
                            && linhagem.item_desc.Trim().Contains(w.Produto.ToUpper())
                            && w.Tipo == "Vacina"
                            && dataNascimento >= w.DataInicial && dataNascimento <= w.DataFinal
                            && w.Regiao == "Todas")
                        .FirstOrDefault();

                    if (tabelaPreco != null)
                        valorUnitarioVacina = valorUnitarioVacina + (decimal)tabelaPreco.ValorNormal;
                }

                #endregion

                #region Serviço

                decimal valorUnitarioServico = 0;

                CHICDataSet.bookedDataTable servicos = new CHICDataSet.bookedDataTable();
                booked.FillByVariety(servicos, "SERV", pedido.orderno);

                foreach (var item in servicos)
                {
                    items.FillByItemNo(dsCHIC.items, item.item);
                    CHICDataSet.itemsRow linhagem = null;
                    if (dsCHIC.items.Count > 0)
                        linhagem = dsCHIC.items[0];

                    DateTime dataNascimento = item.cal_date.AddDays(21);

                    Tabela_Precos tabelaPreco = hlbappService.Tabela_Precos
                        .Where(w => w.Empresa == empresa
                            && linhagem.item_desc.Trim().Contains(w.Produto.ToUpper())
                            && w.Tipo == "Serviço"
                            && dataNascimento >= w.DataInicial && dataNascimento <= w.DataFinal
                            && w.Regiao == "Todas")
                        .FirstOrDefault();

                    if (tabelaPreco != null)
                        valorUnitarioServico = valorUnitarioServico + (decimal)tabelaPreco.ValorNormal;
                }

                #endregion

                #region Ovos Brasil

                CHICDataSet.int_commDataTable ovosBrasil = new CHICDataSet.int_commDataTable();
                intcomm.FillByOrderNo(ovosBrasil, pedido.orderno);

                decimal valorOvosBrasil = 0;

                foreach (var item in ovosBrasil)
                {
                    if (item.invmess1 || item.listaiob)
                        valorOvosBrasil = 0.01m;
                }

                #endregion

                #region Quantidade Bonificada

                CHICDataSet.bookedDataTable bonificacao = new CHICDataSet.bookedDataTable();
                booked.FillByAltDesc(bonificacao, "Extra", pedido.orderno);

                decimal qtdeBonificada = 0;
                decimal bookIDBonificacao = 0;
                decimal percBonif = 0;

                foreach (var item in bonificacao)
                {
                    qtdeBonificada = qtdeBonificada + item.quantity;
                    bookIDBonificacao = item.book_id;
                    percBonif = Convert.ToDecimal(item.alt_desc.Substring(0, 1));
                }

                #endregion

                #region Pintainha

                CHICDataSet.bookedDataTable pintos = new CHICDataSet.bookedDataTable();
                booked.FillByPriceMaiorZero(pintos, pedido.orderno);

                var listaPintosAgrupados = pintos
                    .GroupBy(g => new
                    {
                        g.item,
                        g.cal_date
                    })
                    .Select(l => new
                    {
                        l.Key.cal_date,
                        l.Key.item,
                        qtde = l.Sum(s => s.quantity)
                    })
                    .ToList();

                //foreach (var item in pintos)
                foreach (var item in listaPintosAgrupados)
                {
                    decimal valorUnitario = 0;

                    //CHICDataSet.bookedRow item = dsCHIC.booked[0];

                    items.FillByItemNo(dsCHIC.items, item.item);
                    CHICDataSet.itemsRow linhagem = dsCHIC.items[0];

                    DateTime dataNascimento = item.cal_date.AddDays(21);

                    #region Vaxxitek

                    if (linhagem.item_desc.Trim().Contains("VAXX"))
                    {
                        Tabela_Precos tabelaPrecoVacina = hlbappService.Tabela_Precos
                            .Where(w => w.Empresa == empresa
                                && w.Produto == "Vaxxitek"
                                && w.Tipo == "Vacina"
                                && dataNascimento >= w.DataInicial && dataNascimento <= w.DataFinal
                                && w.Regiao == "Todas")
                            .FirstOrDefault();

                        if (tabelaPrecoVacina != null)
                            valorUnitarioVacina = valorUnitarioVacina + (decimal)tabelaPrecoVacina.ValorNormal;
                    }

                    #endregion

                    vartbl.FillByCod(dsCHIC.vartabl, linhagem.variety);
                    CHICDataSet.vartablRow descricaoLinhagem = dsCHIC.vartabl[0];

                    string descricaoVariety = descricaoLinhagem.desc.Trim();

                    Tabela_Precos tabelaPreco = hlbappService.Tabela_Precos
                        .Where(w => w.Empresa == empresa
                            && w.Produto == descricaoVariety
                            && w.Tipo == tipoPagamento
                            && dataNascimento >= w.DataInicial && dataNascimento <= w.DataFinal
                            && w.Regiao == uf.UfRegGeog)
                        .FirstOrDefault();

                    if (tabelaPreco != null)
                    {
                        if (item.qtde < 5000)
                            valorUnitario = (decimal)tabelaPreco.ValorMenor5000Aves;
                        else
                            valorUnitario = (decimal)tabelaPreco.ValorNormal;

                        percBonif = percBonif / 100.00m;
                        qtdeBonificada = Convert.ToInt32((item.qtde * (1.00m + percBonif))) - item.qtde;

                        decimal valorVacinasServicosSoma = 0;
                        valorVacinasServicosSoma = valorUnitarioVacina + valorUnitarioServico;
                        decimal valorVacinasServicosReal = 0;
                        valorVacinasServicosReal = ((item.qtde + qtdeBonificada) * valorVacinasServicosSoma) / item.qtde;

                        //valorUnitario = valorUnitario + valorUnitarioVacina + valorUnitarioServico + valorOvosBrasil;
                        valorUnitario = valorUnitario + valorVacinasServicosReal + valorOvosBrasil;

                        //item.price = valorUnitario;

                        //booked.Update(item);
                        foreach (var item2 in pintos)
                        {
                            booked.UpdateQuery(valorUnitario, item2.book_id);
                        }

                        booked.UpdateQuantity(qtdeBonificada, bookIDBonificacao);
                    }
                }

                #endregion
            }
        }

        public void AjustaErroPrecoPedidos()
        {
            orders.FillByErro(dsCHIC.orders);

            foreach (var pedido in dsCHIC.orders)
            {
                ImportaCHICService.Data.CHICBKPDataSetTableAdapters.bookedTableAdapter bookedBkp
                        = new Data.CHICBKPDataSetTableAdapters.bookedTableAdapter();

                #region Pintainha

                CHICDataSet.bookedDataTable pintos = new CHICDataSet.bookedDataTable();
                booked.FillByPriceMaiorZero(pintos, pedido.orderno);

                foreach (var item in pintos)
                {
                    CHICBKPDataSet.bookedDataTable bonificacaoBkp = new CHICBKPDataSet.bookedDataTable();
                    bookedBkp.FillByPriceMaiorZero(bonificacaoBkp, pedido.orderno, item.item, item.location);

                    if (bonificacaoBkp.Count > 0)
                        booked.UpdateQuery(bonificacaoBkp[0].price, item.book_id);
                }

                #endregion

                #region Quantidade Bonificada

                CHICDataSet.bookedDataTable bonificacao = new CHICDataSet.bookedDataTable();
                booked.FillByAltDesc(bonificacao, "Extra", pedido.orderno);

                foreach (var item in bonificacao)
                {
                    CHICBKPDataSet.bookedDataTable bonificacaoBkp = new CHICBKPDataSet.bookedDataTable();
                    bookedBkp.FillByAltDesc(bonificacaoBkp, "Extra", pedido.orderno, item.item, item.location);

                    if (bonificacaoBkp.Count > 0)
                        booked.UpdateQuantity(bonificacaoBkp[0].quantity, item.book_id);
                }

                #endregion
            }
        }

        public void AtualizaUltimoOrderNOCHIC()
        {
            ordersTableAdapter orderTA = new ordersTableAdapter();
            string orderNO = (Convert.ToInt32(orderTA.MaxOrderNo())).ToString();

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            HLBAPPServiceEntities hlbapp = new HLBAPPServiceEntities();
            CHIC_Ultimo_Numero chic = hlbappService.CHIC_Ultimo_Numero.FirstOrDefault();
            int ultimoOrderNOCHIC = Convert.ToInt32(orderNO);
            chic.UltimoOrderNOCHIC = ultimoOrderNOCHIC;
            hlbappService.SaveChanges();
        }

        public string AtualizaPedidosCHICNovoModelo()
        {
            /*
             * Paulo Alves - 24/10/2017
             * 
             * Os pedidos que nascerão a partir de 03/12/2017, terão as seguintes modificações:
             * 
             * - Os valores dos pintos, vacinas e serviços serão separados;
             * - As quantidades que receberão as vacinas e os serviços serão informados em cada item;
             * - Cada vacina terá um campo personalizado informando será informado se houve bonificação
             *   ou o cliente que irá enviar a mesma, para não realizar a cobrança;
             */

            string orderNoErro = "";
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                ordersTableAdapter oTA = new ordersTableAdapter();
                CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();
                //DateTime data = DateTime.Today;
                //DateTime data = Convert.ToDateTime("07/01/2018").AddDays(-21);
                oTA.FillModeloNovoMaior03122017(oDT);

                var listaOrders = oDT
                    //.Where(w => w.orderno == "67777")
                    .ToList();

                foreach (var order in listaOrders)
                {
                    #region Itens do Pedido

                    string orderNo = order.orderno.Trim();
                    orderNoErro = orderNo;

                    ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                                new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                    CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                    bTACommercial.FillByOrderNo(bDTCommercial, orderNo);

                    #endregion

                    if (bDTCommercial.Count > 0)
                    {
                        #region Dados do Pedido

                        itemsTableAdapter iTA = new itemsTableAdapter();
                        CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                        iTA.Fill(iDT);

                        salesmanTableAdapter sTA = new salesmanTableAdapter();
                        CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                        sTA.FillByCod(sDT, order.salesrep);
                        CHICDataSet.salesmanRow sR = sDT.FirstOrDefault();
                        string empresa = "";
                        if (sR != null) empresa = sR.inv_comp.Trim();

                        var listaProdutos = iDT.ToList();
                        var listaItens = bDTCommercial.ToList();

                        decimal qtdOvosVendidosCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                    && a.form.Substring(0, 1).Equals("D")))
                            .Sum(s => s.quantity);

                        int_commTableAdapter icTA = new int_commTableAdapter();
                        CHICDataSet.int_commDataTable icDT = new CHICDataSet.int_commDataTable();
                        icTA.FillByOrderNo(icDT, orderNo);
                        CHICDataSet.int_commRow icR = icDT.FirstOrDefault();

                        CHICDataSet.int_commDataTable icDTR = new CHICDataSet.int_commDataTable();
                        icTA.FillByNpedrepo(icDTR, Convert.ToDecimal(orderNo));
                        CHICDataSet.int_commRow icRReposicao = icDTR.FirstOrDefault();

                        #endregion

                        foreach (var item in listaItens)
                        {
                            #region Dados do item do pedido

                            #region Qtde Mesma Data para Verificar Preço do Pinto

                            int qtdTotalMesmaData = 0;
                            if (!order.delivery.Contains("DOA"))
                            {
                                if (listaItens.Count > 0)
                                    qtdTotalMesmaData = Convert.ToInt32(listaItens
                                         .Where(w => w.item == item.item).Sum(s => s.quantity));
                            }
                            else
                            {
                                string orderNoPrincipal = icR.npedrepo.ToString();
                                CHICDataSet.bookedDataTable bDTCommercialMain = new CHICDataSet.bookedDataTable();
                                bTACommercial.FillByOrderNo(bDTCommercialMain, orderNoPrincipal);
                                qtdTotalMesmaData = Convert.ToInt32(listaItens
                                         .Where(w => w.item == item.item).Sum(s => s.quantity));
                                qtdTotalMesmaData = qtdTotalMesmaData + Convert.ToInt32(bDTCommercialMain
                                         .Where(w => w.item == item.item).Sum(s => s.quantity));
                            }

                            #endregion

                            #region Pega Qtde da Reposição para inserir nas Vacinas

                            decimal qtdReposicao = 0;
                            if (icRReposicao != null)
                            {
                                string orderNoReposicao = icRReposicao.orderno;
                                CHICDataSet.bookedDataTable bDTCommercialReposicao = new CHICDataSet.bookedDataTable();
                                bTACommercial.FillByOrderNo(bDTCommercialReposicao, orderNoReposicao);
                                qtdReposicao = Convert.ToInt32(bDTCommercialReposicao
                                    .Where(w => iDT.Any(i => w.item == i.item_no
                                        && i.form.Substring(0, 1) == "D")).Sum(s => s.quantity));
                            }

                            #endregion

                            decimal qtdTotalVacina = 0;
                            if (!order.delivery.Contains("DOA"))
                                qtdTotalVacina = qtdOvosVendidosCHIC + qtdReposicao;
                            else
                                qtdTotalVacina = qtdOvosVendidosCHIC;

                            DateTime dataNascimento = item.cal_date.AddDays(21);

                            CHICDataSet.itemsRow iR = listaProdutos
                                .Where(w => w.item_no == item.item).FirstOrDefault();

                            #endregion

                            #region Preço Pinto

                            if (iR.form.Substring(0, 1).Equals("D")
                                && !item.alt_desc.Contains("Extra"))
                            {
                                vartablTableAdapter vTA = new vartablTableAdapter();
                                CHICDataSet.vartablDataTable vDT = new CHICDataSet.vartablDataTable();
                                vTA.FillByCod(vDT, iR.variety);
                                CHICDataSet.vartablRow vR = vDT.FirstOrDefault();

                                if (vR != null)
                                {
                                    decimal preco = CalculaValorLinhagemTabelaPrecoNovoPedido(order.cust_no.Trim(),
                                        vR.desc.Trim(), dataNascimento, dataNascimento, order.delivery.Trim(),
                                        qtdTotalMesmaData, empresa);

                                    if (preco > 0) bTACommercial.UpdateQuery(preco, item.book_id);
                                }
                            }

                            #endregion

                            #region Vacinas

                            if (iR.form.Equals("VC"))
                            {
                                PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                    .Where(w => w.USERCodigoCHIC == item.item).FirstOrDefault();

                                if (produtoApolo1 != null)
                                {
                                    PRODUTO produtoApolo = apolo.PRODUTO
                                        .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr).FirstOrDefault();

                                    decimal preco = CalculaValoresVacinasServicosNovoPV(produtoApolo.ProdNomeAlt2,
                                        dataNascimento, dataNascimento, empresa, "Vacina");

                                    if (preco > 0)
                                    {
                                        bTACommercial.UpdateQuery(preco, item.book_id);
                                    }

                                    bTACommercial.UpdateQuantity(qtdTotalVacina, item.book_id);
                                }
                            }

                            #endregion

                            #region Serviços

                            if (iR.form.Equals("SV"))
                            {
                                PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                    .Where(w => w.USERCodigoCHIC == item.item).FirstOrDefault();

                                if (produtoApolo1 != null)
                                {
                                    PRODUTO produtoApolo = apolo.PRODUTO
                                        .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr).FirstOrDefault();

                                    decimal preco = CalculaValoresVacinasServicosNovoPV(produtoApolo.ProdNomeAlt1,
                                        dataNascimento, dataNascimento, empresa, "Serviço");

                                    if (preco > 0)
                                    {
                                        string comment1 = item.comment_1.Trim();
                                        string percBonStr = comment1.Substring(7, 5);
                                        decimal percBon = 100;
                                        if (decimal.TryParse(percBonStr, out percBon))
                                            percBon = percBon;
                                        else
                                            percBon = 100;

                                        int qtdSevico = Convert.ToInt32(qtdTotalVacina * (percBon / 100.00m));

                                        bTACommercial.UpdateQuery(preco, item.book_id);
                                        bTACommercial.UpdateQuantity(qtdSevico, item.book_id);
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Atualizar CHIC com WEB - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                return retorno;
            }
        }

        public string AtualizaQtdeVacinasServicosPedidosCHICNovoModelo()
        {
            /*
             * Paulo Alves - 27/11/2017
             * 
             * Atualização da quantidade pedidos que nascerão a partir de 08/01/2018,
             * onde será verificado se o tipo de reposição for Mortalidade, não irá somar
             */

            string orderNoErro = "";
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            try
            {
                ordersTableAdapter oTA = new ordersTableAdapter();
                CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();
                //DateTime data = DateTime.Today;
                //DateTime data = Convert.ToDateTime("07/01/2018").AddDays(-21);
                oTA.FillModeloNovoMaior08012018(oDT);

                var listaOrders = oDT
                    //.Where(w => w.orderno == "74949")
                    .ToList();

                foreach (var order in listaOrders)
                {
                    #region Itens do Pedido

                    string orderNo = order.orderno.Trim();
                    orderNoErro = orderNo;

                    ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                                new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                    CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                    bTACommercial.FillByOrderNo(bDTCommercial, orderNo);

                    #endregion

                    if (bDTCommercial.Count > 0)
                    {
                        #region Dados do Pedido

                        itemsTableAdapter iTA = new itemsTableAdapter();
                        CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                        iTA.Fill(iDT);

                        salesmanTableAdapter sTA = new salesmanTableAdapter();
                        CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                        sTA.FillByCod(sDT, order.salesrep);
                        CHICDataSet.salesmanRow sR = sDT.FirstOrDefault();
                        string empresa = "";
                        if (sR != null) empresa = sR.inv_comp.Trim();

                        var listaProdutos = iDT.ToList();
                        var listaItens = bDTCommercial.ToList();

                        decimal qtdOvosVendidosCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                    && a.form.Substring(0, 1).Equals("D")))
                            .Sum(s => s.quantity);

                        var listaLinhagens = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                    && a.form.Substring(0, 1).Equals("D"))
                                    && !w.alt_desc.Contains("Extra"))
                            .ToList();

                        var listaVacinasServicos = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Equals("SV") || a.form.Equals("VC"))))
                            .ToList();

                        int_commTableAdapter icTA = new int_commTableAdapter();
                        CHICDataSet.int_commDataTable icDT = new CHICDataSet.int_commDataTable();
                        icTA.FillByOrderNo(icDT, orderNo);
                        CHICDataSet.int_commRow icR = icDT.FirstOrDefault();

                        CHICDataSet.int_commDataTable icDTR = new CHICDataSet.int_commDataTable();
                        icTA.FillByNpedrepo(icDTR, Convert.ToDecimal(orderNo));
                        CHICDataSet.int_commRow icRReposicao = icDTR.FirstOrDefault();

                        #endregion

                        decimal qtdTotalVacina = 0;

                        foreach (var item in listaLinhagens)
                        {
                            #region Dados do item do pedido

                            string variety = "";
                            vartablTableAdapter vartabl = new vartablTableAdapter();
                            CHICDataSet.vartablDataTable vartablDT =
                                new CHICDataSet.vartablDataTable();

                            CHICDataSet.itemsRow iR = listaProdutos
                                .Where(w => w.item_no == item.item).FirstOrDefault();

                            vartabl.FillByCod(vartablDT, iR.variety);
                            variety = vartablDT[0].variety;
                            string linhagem = vartablDT[0].desc.Trim();

                            Item_Pedido_Venda itemWEB = hlbappService.Item_Pedido_Venda
                                .Where(w => w.OrderNoCHIC == item.orderno
                                    && w.ProdCodEstr == linhagem).FirstOrDefault();

                            #region Qtde Mesma Data para Verificar Preço do Pinto

                            int qtdTotalMesmaData = 0;
                            if (!order.delivery.Contains("DOA"))
                            {
                                if (listaItens.Count > 0)
                                    qtdTotalMesmaData = Convert.ToInt32(listaItens
                                            .Where(w => w.item == item.item).Sum(s => s.quantity));
                            }
                            else
                            {
                                string orderNoPrincipal = icR.npedrepo.ToString();
                                CHICDataSet.bookedDataTable bDTCommercialMain = new CHICDataSet.bookedDataTable();
                                bTACommercial.FillByOrderNo(bDTCommercialMain, orderNoPrincipal);
                                qtdTotalMesmaData = Convert.ToInt32(listaItens
                                            .Where(w => w.item == item.item).Sum(s => s.quantity));
                                qtdTotalMesmaData = qtdTotalMesmaData + Convert.ToInt32(bDTCommercialMain
                                            .Where(w => w.item == item.item).Sum(s => s.quantity));
                            }

                            #endregion

                            #region Pega Qtde da Reposição para inserir nas Vacinas

                            decimal qtdReposicao = 0;
                            if (icRReposicao != null)
                            {
                                string orderNoReposicao = icRReposicao.orderno;
                                CHICDataSet.bookedDataTable bDTCommercialReposicao = new CHICDataSet.bookedDataTable();
                                bTACommercial.FillByOrderNo(bDTCommercialReposicao, orderNoReposicao);
                                qtdReposicao = Convert.ToInt32(bDTCommercialReposicao
                                    .Where(w => iDT.Any(i => w.item == i.item_no
                                        && i.form.Substring(0, 1) == "D")).Sum(s => s.quantity));
                            }

                            #endregion

                            //decimal qtdTotalVacina = 0;
                            string delivery = "";
                            string tipoReposicao = "";
                            if (itemWEB != null)
                                if (itemWEB.TipoReposicao != null) tipoReposicao = itemWEB.TipoReposicao;
                            if (order.delivery != null) delivery = order.delivery;
                            if (!delivery.Contains("DOA")
                                && tipoReposicao == "Acerto Comercial")
                                qtdTotalVacina = qtdTotalVacina + qtdOvosVendidosCHIC + qtdReposicao;
                            else
                                qtdTotalVacina = qtdTotalVacina + qtdOvosVendidosCHIC;

                            DateTime dataNascimento = item.cal_date.AddDays(21);

                            #endregion
                        }

                        foreach (var item in listaVacinasServicos)
                        {
                            CHICDataSet.itemsRow iR = listaProdutos
                                .Where(w => w.item_no == item.item).FirstOrDefault();

                            #region Vacinas

                            if (iR.form.Equals("VC"))
                            {
                                PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                    .Where(w => w.USERCodigoCHIC == item.item).FirstOrDefault();

                                if (produtoApolo1 != null)
                                {
                                    bTACommercial.UpdateQuantity(qtdTotalVacina, item.book_id);
                                }
                            }

                            #endregion

                            #region Serviços

                            if (iR.form.Equals("SV"))
                            {
                                PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                    .Where(w => w.USERCodigoCHIC == item.item).FirstOrDefault();

                                if (produtoApolo1 != null)
                                {
                                    string comment1 = item.comment_1.Trim();
                                    string percBonStr = "";
                                    if (comment1 != null && comment1 != "")
                                        percBonStr = comment1.Substring(7, 5);
                                    decimal percBon = 100;
                                    if (decimal.TryParse(percBonStr, out percBon))
                                        percBon = percBon;
                                    else
                                        percBon = 100;

                                    int qtdSevico = Convert.ToInt32(qtdTotalVacina * (percBon / 100.00m));

                                    bTACommercial.UpdateQuantity(qtdSevico, item.book_id);
                                }
                            }

                            #endregion
                        }
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Atualizar CHIC com WEB - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                return retorno;
            }
        }

        public void AtualizaPedidosLTZParaCH()
        {
            Data.CHICDataSetTableAdapters.bookedTableAdapter bTA =
                new Data.CHICDataSetTableAdapters.bookedTableAdapter();

            bTA.UpdateQuery1();
        }

        public void AtualizaPrecoTratamentoInfravermelhoChamado31610()
        {
            var listaItensTR = booked.GetDataAlteraPrecoTRChamado31610();

            foreach (var item in listaItensTR)
            {
                booked.UpdatePrice(0.15m, item.book_id);
            }
        }

        public void AtualizaPrecoVacinasChamado35671()
        {
            var listaItensTR = booked.GetDataAlteraPrecoVacinasChamado35671();

            foreach (var item in listaItensTR)
            {
                if (item.item == "135") // BIO COCCIVET R (COCCIDIOSE)- BIOVET
                    booked.UpdatePrice(0.15m, item.book_id);
                else if (item.item == "906") // HIPRACOX (COCCIDIOSE) - HIPRA
                    booked.UpdatePrice(0.15m, item.book_id);
                else if (item.item == "900") // INNOVAX ND (NEWCASTLE/HVT)-MSD
                    booked.UpdatePrice(0.09m, item.book_id);
                else if (item.item == "189") // POULVAC MAGNIPLEX (GUMBORO)-ZOETIS
                    booked.UpdatePrice(0.05m, item.book_id);
                else if (item.item == "188") // VECTORMUNE FP-MG (MG/BOUBA)-CEVA
                    booked.UpdatePrice(0.15m, item.book_id);
            }
        }

        #endregion

        #region Outros Métodos

        public List<string> LocalizacaoGoogleMaps(string cep)
        {
            try
            {
                var address = cep;
                var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyAZOLAWLFhkgRAggDKrxFew5l1zySK4HGk&address={0}&sensor=false", Uri.EscapeDataString(address));

                var request = WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());

                var result = xdoc.Element("GeocodeResponse").Element("result");
                var locationElement = result.Element("geometry").Element("location");
                var lat = locationElement.Element("lat");
                string latitude = lat.Value;
                var lng = locationElement.Element("lng");
                string longitude = lng.Value;

                List<string> retorno = new List<string>();
                retorno.Add(latitude);
                retorno.Add(longitude);

                return retorno;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void EnviaCurriculosFTPEmail()
        {
            try
            {
                #region Conecta no FTP e Lista os Arquivos

                string url = "ftp://hyline.tempsite.ws/Web/curriculos";
                string usuario = "hyline";
                string senha = "hlb1307";
                string local = @"\\srv-riosoft-01\W\Curriculos";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(usuario, senha);
                request.UseBinary = true;

                #endregion

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream rs = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(rs);

                        while (!reader.EndOfStream)
                        {
                            #region Crias os Arquivos e Salva em \\srv-riosoft-01\W\Curriculos

                            string arquivo = reader.ReadLine();
                            string nomeArquivo = arquivo.Substring(59, arquivo.Length - 59);

                            string caminhoArquivo = url + "/" + nomeArquivo;

                            FtpWebRequest requestDownload = (FtpWebRequest)WebRequest
                                .Create(new Uri(caminhoArquivo));
                            requestDownload.Method = WebRequestMethods.Ftp.DownloadFile;
                            requestDownload.Credentials = new NetworkCredential(usuario, senha);
                            requestDownload.UseBinary = true;

                            string localArquivo = local + @"\" + nomeArquivo;

                            using (FtpWebResponse responseDownload = (FtpWebResponse)requestDownload.GetResponse())
                            {
                                using (Stream rsDownload = responseDownload.GetResponseStream())
                                {
                                    using (FileStream ws = new FileStream(localArquivo, FileMode.Create))
                                    {
                                        byte[] buffer = new byte[2048];
                                        int bytesRead = rsDownload.Read(buffer, 0, buffer.Length);
                                        while (bytesRead > 0)
                                        {
                                            ws.Write(buffer, 0, bytesRead);
                                            bytesRead = rsDownload.Read(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region Deleta o Arquivo do FTP

                            FtpWebRequest requestDelete = (FtpWebRequest)WebRequest
                                .Create(new Uri(caminhoArquivo));
                            requestDelete.Method = WebRequestMethods.Ftp.DeleteFile;
                            requestDelete.Credentials = new NetworkCredential(usuario, senha);
                            requestDelete.UseBinary = true;

                            FtpWebResponse responseDelete = (FtpWebResponse)requestDelete.GetResponse();

                            #endregion

                            #region Envia para Email do RH

                            string assuntoEmail = "** CURRÍCULO ENVIADO PELO SITE **";

                            string corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                                + "Segue em anexo currículo enviado pelo site."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA WEB";

                            EnviaConfirmacaoEmail(localArquivo, "rh@hyline.com.br", "RH",
                                "", "", "", corpoEmail, assuntoEmail, "5");

                            #endregion
                        }
                    }
                }
            }
            catch
            {
                //throw;
            }
        }

        public string AtualizaExcel(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, string relatorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\" + relatorio + ".xlsx", destino);

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

            oBook.RefreshAll();

            System.Threading.Thread.Sleep(10000);

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

        #region Envio de Relatório Automáticos

        #region Verificação Final

        public string GeraRelatorioVerificacaoFinal(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicio, string empresa, string  empresaLayoutRelatorio, string vendedor,
            string nomeVendedor, string relatorio, string origem)
        {
            //string destino = "\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            //string empresaLayoutRelatorio = "";

            //if (empresa.Equals("(Todas)"))
            //    empresaLayoutRelatorio = "BR";
            //else
            //    empresaLayoutRelatorio = empresa;

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\" + relatorio + "_" 
                + empresaLayoutRelatorio + ".xlsx", destino);

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

            string commandTextCHICCabecalhoVendedor = "";
            if (origem == "Vendedor" || empresa == "LB")
                commandTextCHICCabecalhoVendedor = "o.delivery `Cond. Pagmto.`, ";



            string commandTextCHICCabecalhoMatriz = "";
            if (empresa != "PL")
                commandTextCHICCabecalhoMatriz = "min(i.form) `Matriz`, ";

            #region Faturamento

            DateTime dataInicialFaturamento = dataInicio.AddDays(5);
            DateTime dataFinalFaturamento = dataInicio.AddDays(11);

            Excel._Worksheet worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento"];

            worksheetFaturamento.Cells[2, 7] = dataInicialFaturamento.ToString("dd/MM/yyyy") + " à " +
                dataFinalFaturamento.ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetFaturamento.Cells[3, 7] = vendedor;
            else
                worksheetFaturamento.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoFaturamento =
                "select " +
                    "b.cal_date `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    commandTextCHICCabecalhoMatriz +
                    "v.desc `Linhagem`, " +
                    "cc.codestab `Cód. Estabelecimento`, " +
                    "cc.protoc `Nº Protocolo`, " +
                    "cc.registro `Nº Registro`, " +
                    "c.region `CPF/CNPJ`, " +
                    "cc.datereg `validade registro` ";

            string commandTextCHICTabelasFaturamento =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on b.customer = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join custcust cc on c.custno = cc.custno ";

            string commandTextCHICCondicaoJoinsFaturamento =
                "where ";
                    //"b.orderno = o.orderno and " +
                    //"b.item = i.item_no and  " +
                    //"i.variety = v.variety and " +
                    //"o.cust_no = c.custno and " +
                    //"o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento =
                    "trim(b.alt_desc) = '' and i.form in ('HE') and ";

            string dataInicialStrFaturamento = dataInicialFaturamento.ToString("MM/dd/yyyy");
            string dataFinalStrFaturamento = dataFinalFaturamento.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento =
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery, " +
                    "cc.codestab, " +
                    "cc.protoc, " +
                    "cc.registro," +
                    "c.region, " +
                    "cc.datereg " +

                    " Union ";

            string commandTextCHICCabecalhoFaturamento02 =
                "select " +
                    "b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    commandTextCHICCabecalhoMatriz +
                    "v.desc `Linhagem`, " +
                    "cc.codestab `Cód. Estabelecimento`, " +
                    "cc.protoc `Nº Protocolo`, " +
                    "cc.registro `Nº Registro`, " +
                    "c.region `CPF/CNPJ`, " +
                    "cc.datereg `validade registro` ";

            string commandTextCHICTabelasFaturamento02 =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on b.customer = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join custcust cc on c.custno = cc.custno ";

            string commandTextCHICCondicaoJoinsFaturamento02 =
                "where ";
                    //"b.orderno = o.orderno and " +
                    //"b.item = i.item_no and  " +
                    //"i.variety = v.variety and " +
                    //"o.cust_no = c.custno and " +
                    //"o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento02 =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrFaturamentoCalDate = dataInicialFaturamento.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrFaturamentoCalDate = dataFinalFaturamento.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento02 =
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento02 =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery, " +
                    "cc.codestab, " +
                    "cc.protoc, " +
                    "cc.registro," +
                    "c.region, " +
                    "cc.datereg ";

            string commandTextCHICOrdenacaoFaturamento =
                "order by " +
                    "1";

            #region Dados Faturamento

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
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
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
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 =
                "order by " +
                    "b.item_ord";

            #endregion

            #endregion

            #region Lotes Já Incubados

            DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
            DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

            Excel._Worksheet worksheetLotesJaIncubados = (Excel._Worksheet)oBook.Worksheets["Lotes Já Incubados"];

            worksheetLotesJaIncubados.Cells[2, 7] = dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " à " +
                dataFinalLotesJaIncubados.ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetLotesJaIncubados.Cells[3, 7] = vendedor;
            else
                worksheetLotesJaIncubados.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoLotesJaIncubados =
                "select " +
                    "b.cal_date `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    commandTextCHICCabecalhoMatriz +
                    "v.desc `Linhagem`, " +
                    "cc.codestab `Cód. Estabelecimento`, " +
                    "cc.protoc `Nº Protocolo`, " +
                    "cc.registro `Nº Registro`, " +
                    "c.region `CPF/CNPJ`, " +
                    "cc.datereg `validade registro` ";

            string commandTextCHICTabelasLotesJaIncubados =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on b.customer = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join custcust cc on c.custno = cc.custno ";

            string commandTextCHICCondicaoJoinsLotesJaIncubados =
                "where ";
                    //"b.orderno = o.orderno and " +
                    //"b.item = i.item_no and  " +
                    //"i.variety = v.variety and " +
                    //"o.cust_no = c.custno and " +
                    //"o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosLotesJaIncubados =
                    "trim(b.alt_desc) = '' and i.form in ('HE') and ";

            string dataInicialStrLotesJaIncubados = dataInicialLotesJaIncubados.ToString("MM/dd/yyyy");
            string dataFinalStrLotesJaIncubados = dataFinalLotesJaIncubados.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosLotesJaIncubados =
                    "b.cal_date between {" + dataInicialStrLotesJaIncubados + "} and {" + dataFinalStrLotesJaIncubados + "} and " +
                            "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                            "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoLotesJaIncubados =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery, " +
                    "cc.codestab, " +
                    "cc.protoc, " +
                    "cc.registro," +
                    "c.region, " +
                    "cc.datereg " +

                    " Union ";

            string commandTextCHICCabecalhoLotesJaIncubados02 =
                "select " +
                    "b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    commandTextCHICCabecalhoMatriz +
                    "v.desc `Linhagem`, " +
                    "cc.codestab `Cód. Estabelecimento`, " +
                    "cc.protoc `Nº Protocolo`, " +
                    "cc.registro `Nº Registro`, " +
                    "c.region `CPF/CNPJ`, " +
                    "cc.datereg `validade registro` ";

            string commandTextCHICTabelasLotesJaIncubados02 =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on b.customer = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join custcust cc on c.custno = cc.custno ";

            string commandTextCHICCondicaoJoinsLotesJaIncubados02 =
                "where ";
                    //"b.orderno = o.orderno and " +
                    //"b.item = i.item_no and  " +
                    //"i.variety = v.variety and " +
                    //"o.cust_no = c.custno and " +
                    //"o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosLotesJaIncubados02 =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrLotesJaIncubadosCalDate = dataInicialLotesJaIncubados.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrLotesJaIncubadosCalDate = dataFinalLotesJaIncubados.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosLotesJaIncubados02 =
                    "b.cal_date between {" + dataInicialStrLotesJaIncubadosCalDate + "} and {" + dataFinalStrLotesJaIncubadosCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoLotesJaIncubados02 =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery, " +
                    "cc.codestab, " +
                    "cc.protoc, " +
                    "cc.registro," +
                    "c.region, " +
                    "cc.datereg ";

            string commandTextCHICOrdenacaoLotesJaIncubados =
                "order by " +
                    "1";

            #region Dados Já Incubados

            string commandTextCHICCabecalhoLotesJaIncubadosDados =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi ";

            string commandTextCHICTabelasLotesJaIncubadosDados =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic  ";

            string commandTextCHICCondicaoJoinsLotesJaIncubadosDados =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.orderno = ic.orderno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosLotesJaIncubadosDados = "";

            string commandTextCHICCondicaoParametrosLotesJaIncubadosDados =
                    "b.cal_date between {" + dataInicialStrLotesJaIncubados + "} and {" + dataFinalStrLotesJaIncubados + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoLotesJaIncubadosDados = "";

            string commandTextCHICOrdenacaoLotesJaIncubadosDados = " Union ";

            string commandTextCHICCabecalhoLotesJaIncubadosDados02 =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi ";

            string commandTextCHICTabelasLotesJaIncubadosDados02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic  ";

            string commandTextCHICCondicaoJoinsLotesJaIncubadosDados02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.orderno = ic.orderno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosLotesJaIncubadosDados02 = "";

            string commandTextCHICCondicaoParametrosLotesJaIncubadosDados02 =
                    "b.cal_date between {" + dataInicialStrLotesJaIncubadosCalDate + "} and {" + dataFinalStrLotesJaIncubadosCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoLotesJaIncubadosDados02 = "";

            string commandTextCHICOrdenacaoLotesJaIncubadosDados02 =
                "order by " +
                    "b.item_ord";

            #endregion

            #endregion

            #region Incubação

            DateTime dataInicialIncubacao = (dataFinalLotesJaIncubados.AddDays(1)).AddDays(-21);
            DateTime dataFinalIncubacao = (dataFinalLotesJaIncubados.AddDays(14)).AddDays(-21);

            Excel._Worksheet worksheetIncubacao = (Excel._Worksheet)oBook.Worksheets["Incubação"];

            worksheetIncubacao.Cells[2, 7] = dataInicialIncubacao.ToString("dd/MM/yyyy") + " à " +
                dataFinalIncubacao.ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetIncubacao.Cells[3, 7] = vendedor;
            else
                worksheetIncubacao.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoIncubacao =
                "select " +
                    "b.cal_date `Incubação`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    commandTextCHICCabecalhoMatriz +
                    "v.desc `Linhagem`, " +
                    "cc.codestab `Cód. Estabelecimento`, " +
                    "cc.protoc `Nº Protocolo`, " +
                    "cc.registro `Nº Registro`, " +
                    "c.region `CPF/CNPJ`, " +
                    "cc.datereg `validade registro` ";

            string commandTextCHICTabelasIncubacao =
                "from " +
                    "booked b " +
                    "inner join orders o on b.orderno = o.orderno " +
                    "inner join items i on b.item = i.item_no " +
                    "inner join vartabl v on i.variety = v.variety " +
                    "inner join cust c on b.customer = c.custno " +
                    "inner join salesman s on o.salesrep = s.sl_code " +
                    "left join custcust cc on c.custno = cc.custno ";

            string commandTextCHICCondicaoJoinsIncubacao =
                "where ";
                    //"b.orderno = o.orderno and " +
                    //"b.item = i.item_no and  " +
                    //"i.variety = v.variety and " +
                    //"o.cust_no = c.custno and " +
                    //"o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosIncubacao =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrIncubacao = dataInicialIncubacao.ToString("MM/dd/yyyy");
            string dataFinalStrIncubacao = dataFinalIncubacao.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosIncubacao =
                    "b.cal_date between {" + dataInicialStrIncubacao + "} and {" + dataFinalStrIncubacao + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoIncubacao =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery, " +
                    "cc.codestab, " +
                    "cc.protoc, " +
                    "cc.registro," +
                    "c.region, " +
                    "cc.datereg ";

            string commandTextCHICOrdenacaoIncubacao =
                "order by " +
                    "1";

            #region Dados Incubação

            string commandTextCHICCabecalhoIncubacaoDados =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi ";

            string commandTextCHICTabelasIncubacaoDados =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic  ";

            string commandTextCHICCondicaoJoinsIncubacaoDados =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.orderno = ic.orderno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosIncubacaoDados = "";

            string commandTextCHICCondicaoParametrosIncubacaoDados =
                    "b.cal_date between {" + dataInicialStrIncubacao + "} and {" + dataFinalStrIncubacao + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoIncubacaoDados = "";

            string commandTextCHICOrdenacaoIncubacaoDados =
                "order by " +
                    "b.item_ord";

            #endregion

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
                        commandTextCHICCabecalhoFaturamento02 + commandTextCHICTabelasFaturamento02 +
                        commandTextCHICCondicaoJoinsFaturamento02 +
                        commandTextCHICCondicaoFiltrosFaturamento02 + commandTextCHICCondicaoParametrosFaturamento02 +
                        commandTextCHICAgrupamentoFaturamento02 +
                        commandTextCHICOrdenacaoFaturamento;
                else if (item.Name.Equals("CHIC_Ja_Incubados"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoLotesJaIncubados + commandTextCHICTabelasLotesJaIncubados +
                        commandTextCHICCondicaoJoinsLotesJaIncubados +
                        commandTextCHICCondicaoFiltrosLotesJaIncubados + commandTextCHICCondicaoParametrosLotesJaIncubados +
                        commandTextCHICAgrupamentoLotesJaIncubados +
                        commandTextCHICCabecalhoLotesJaIncubados02 + commandTextCHICTabelasLotesJaIncubados02 +
                        commandTextCHICCondicaoJoinsLotesJaIncubados02 +
                        commandTextCHICCondicaoFiltrosLotesJaIncubados02 + commandTextCHICCondicaoParametrosLotesJaIncubados02 +
                        commandTextCHICAgrupamentoLotesJaIncubados02 +
                        commandTextCHICOrdenacaoLotesJaIncubados;
                else if (item.Name.Equals("CHIC_Incubacao"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoIncubacao + commandTextCHICTabelasIncubacao +
                        commandTextCHICCondicaoJoinsIncubacao +
                        commandTextCHICCondicaoFiltrosIncubacao + commandTextCHICCondicaoParametrosIncubacao +
                        commandTextCHICAgrupamentoIncubacao +
                        commandTextCHICOrdenacaoIncubacao;
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
                else if (item.Name.Equals("CHIC_Ja_Incubados_Dados"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoLotesJaIncubadosDados + commandTextCHICTabelasLotesJaIncubadosDados +
                        commandTextCHICCondicaoJoinsLotesJaIncubadosDados +
                        commandTextCHICCondicaoFiltrosLotesJaIncubadosDados + commandTextCHICCondicaoParametrosLotesJaIncubadosDados +
                        commandTextCHICAgrupamentoLotesJaIncubadosDados +
                        commandTextCHICOrdenacaoLotesJaIncubadosDados +
                        commandTextCHICCabecalhoLotesJaIncubadosDados02 + commandTextCHICTabelasLotesJaIncubadosDados02 +
                        commandTextCHICCondicaoJoinsLotesJaIncubadosDados02 +
                        commandTextCHICCondicaoFiltrosLotesJaIncubadosDados02 + commandTextCHICCondicaoParametrosLotesJaIncubadosDados02 +
                        commandTextCHICAgrupamentoLotesJaIncubadosDados02 +
                        commandTextCHICOrdenacaoLotesJaIncubadosDados02;
                else if (item.Name.Equals("CHIC_Incubacao_Dados"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoIncubacaoDados + commandTextCHICTabelasIncubacaoDados +
                        commandTextCHICCondicaoJoinsIncubacaoDados +
                        commandTextCHICCondicaoFiltrosIncubacaoDados + commandTextCHICCondicaoParametrosIncubacaoDados +
                        commandTextCHICAgrupamentoIncubacaoDados +
                        commandTextCHICOrdenacaoIncubacaoDados;
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

        public string GeraRelatorioVerificacaoFinalPL(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicio, string empresa, string empresaLayoutRelatorio, string vendedor,
            string nomeVendedor, string relatorio, string origem)
        {
            //string destino = "\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            //string empresaLayoutRelatorio = "";

            //if (empresa.Equals("(Todas)"))
            //    empresaLayoutRelatorio = "BR";
            //else
            //    empresaLayoutRelatorio = empresa;

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\" + relatorio + "_"
                + empresaLayoutRelatorio + ".xlsx", destino);

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

            string commandTextCHICCabecalhoVendedor = "";
            if (origem == "Vendedor" || empresa == "LB")
                commandTextCHICCabecalhoVendedor = "o.delivery `Cond. Pagmto.`, ";

            //string commandTextCHICCabecalhoMatriz = "";
            //if (empresa != "PL")
            //    commandTextCHICCabecalhoMatriz = "min(i.form) `Matriz`, ";

            #region Faturamento Próxima Semana

            DateTime dataInicialFaturamento = dataInicio.AddDays(-21).AddDays(3);
            DateTime dataFinalFaturamento = dataInicio.AddDays(-21).AddDays(8);

            Excel._Worksheet worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento Próxima Semana"];

            worksheetFaturamento.Cells[2, 7] = dataInicialFaturamento.AddDays(21).ToString("dd/MM/yyyy") + " à " +
                dataFinalFaturamento.AddDays(21).ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetFaturamento.Cells[3, 7] = vendedor;
            else
                worksheetFaturamento.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoFaturamento =
                "select " +
                    "b.cal_date `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    //commandTextCHICCabecalhoMatriz +
                    "i.form, " +
                    "v.desc `Linhagem` ";

            string commandTextCHICTabelasFaturamento =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamento =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento =
                    "trim(b.alt_desc) = '' and i.form in ('HE') and ";

            string dataInicialStrFaturamento = dataInicialFaturamento.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrFaturamento = dataFinalFaturamento.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento =
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery " +

                    " Union ";

            string commandTextCHICCabecalhoFaturamento02 =
                "select " +
                    "b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    //commandTextCHICCabecalhoMatriz +
                    "i.form, " +
                    "v.desc `Linhagem` ";

            string commandTextCHICTabelasFaturamento02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamento02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento02 =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrFaturamentoCalDate = dataInicialFaturamento.ToString("MM/dd/yyyy");
            string dataFinalStrFaturamentoCalDate = dataFinalFaturamento.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento02 =
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento02 =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery ";

            string commandTextCHICOrdenacaoFaturamento =
                "order by " +
                    "1";

            #region Dados Faturamento

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
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
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
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 =
                    "order by b.item_ord";

            #endregion

            #endregion

            #region Programação Próxima 04 Semanas

            DateTime dataInicialIncubacao = dataInicio.AddDays(-21).AddDays(10);
            DateTime dataFinalIncubacao = dataInicialIncubacao.AddDays(30);

            Excel._Worksheet worksheetIncubacao = (Excel._Worksheet)oBook.Worksheets["Programação Próxima 04 Semanas"];

            worksheetIncubacao.Cells[2, 7] = dataInicialIncubacao.AddDays(21).ToString("dd/MM/yyyy") + " à " +
                dataFinalIncubacao.AddDays(21).ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetIncubacao.Cells[3, 7] = vendedor;
            else
                worksheetIncubacao.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoIncubacao =
                "select " +
                    "b.cal_date `Incubação`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    //commandTextCHICCabecalhoMatriz +
                    "i.form, " +
                    "v.desc `Linhagem` ";

            string commandTextCHICTabelasIncubacao =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsIncubacao =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosIncubacao =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrIncubacao = dataInicialIncubacao.ToString("MM/dd/yyyy");
            string dataFinalStrIncubacao = dataFinalIncubacao.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosIncubacao =
                    "b.cal_date between {" + dataInicialStrIncubacao + "} and {" + dataFinalStrIncubacao + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoIncubacao =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery ";

            string commandTextCHICOrdenacaoIncubacao =
                "order by " +
                    "1";

            #region Dados Incubação

            string commandTextCHICCabecalhoIncubacaoDados =
                "select b.*, o.*, i.*, v.*, c.*, s.*, ic.confassi ";

            string commandTextCHICTabelasIncubacaoDados =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s, int_comm ic ";

            string commandTextCHICCondicaoJoinsIncubacaoDados =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.orderno = ic.orderno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosIncubacaoDados = "";

            string commandTextCHICCondicaoParametrosIncubacaoDados =
                    "b.cal_date between {" + dataInicialStrIncubacao + "} and {" + dataFinalStrIncubacao + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoIncubacaoDados = "";

            string commandTextCHICOrdenacaoIncubacaoDados =
                "order by " +
                    "b.item_ord";

            #endregion

            #endregion

            #region Duplicatas a Receber

            string commandTextCHICCabecalhoDR =
                "select * ";

            string commandTextCHICTabelasDR =
                "from " +
                    "VU_Duplicatas_Receber_Por_Entidade  ";

            string commandTextCHICCondicaoJoinsDR =
                "where ";

            string commandTextCHICCondicaoFiltrosDR = "";

            string commandTextCHICCondicaoParametrosDR =
                    "([Vend. / Repres.] = '0" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoDR = "";

            string commandTextCHICOrdenacaoDR =
                "order by " +
                    "9, 1";

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
                        commandTextCHICCabecalhoFaturamento02 + commandTextCHICTabelasFaturamento02 +
                        commandTextCHICCondicaoJoinsFaturamento02 +
                        commandTextCHICCondicaoFiltrosFaturamento02 + commandTextCHICCondicaoParametrosFaturamento02 +
                        commandTextCHICAgrupamentoFaturamento02 +
                        commandTextCHICOrdenacaoFaturamento;
                else if (item.Name.Equals("CHIC_Incubacao"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoIncubacao + commandTextCHICTabelasIncubacao +
                        commandTextCHICCondicaoJoinsIncubacao +
                        commandTextCHICCondicaoFiltrosIncubacao + commandTextCHICCondicaoParametrosIncubacao +
                        commandTextCHICAgrupamentoIncubacao +
                        commandTextCHICOrdenacaoIncubacao;
                else if (item.Name.Equals("Duplicatas_Receber"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoDR + commandTextCHICTabelasDR +
                        commandTextCHICCondicaoJoinsDR +
                        commandTextCHICCondicaoFiltrosDR + commandTextCHICCondicaoParametrosDR +
                        commandTextCHICAgrupamentoDR +
                        commandTextCHICOrdenacaoDR;
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
                else if (item.Name.Equals("CHIC_Incubacao_Dados"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoIncubacaoDados + commandTextCHICTabelasIncubacaoDados +
                        commandTextCHICCondicaoJoinsIncubacaoDados +
                        commandTextCHICCondicaoFiltrosIncubacaoDados + commandTextCHICCondicaoParametrosIncubacaoDados +
                        commandTextCHICAgrupamentoIncubacaoDados +
                        commandTextCHICOrdenacaoIncubacaoDados;
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

        public string GeraRelatorioVerificacaoFinalLBAnual(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicio, string empresa, string empresaLayoutRelatorio, string vendedor,
            string nomeVendedor, string relatorio, string origem)
        {
            //string destino = "\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            //string empresaLayoutRelatorio = "";

            //if (empresa.Equals("(Todas)"))
            //    empresaLayoutRelatorio = "BR";
            //else
            //    empresaLayoutRelatorio = empresa;

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\" + relatorio + "_"
                + empresaLayoutRelatorio + ".xlsx", destino);

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

            string commandTextCHICCabecalhoVendedor = "";
            if (origem == "Vendedor" || empresa == "LB")
                commandTextCHICCabecalhoVendedor = "o.delivery `Cond. Pagmto.`, ";

            string commandTextCHICCabecalhoMatriz = "";
            if (empresa != "PL")
                commandTextCHICCabecalhoMatriz = "min(i.form) `Matriz`, ";

            #region Faturamento

            DateTime dataInicialFaturamento = Convert.ToDateTime("01/01/" + DateTime.Today.Year.ToString());
            DateTime dataFinalFaturamento = Convert.ToDateTime("31/12/" + DateTime.Today.Year.ToString());

            string commandTextCHICCabecalhoFaturamento =
                "select " +
                    "b.cal_date `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    commandTextCHICCabecalhoMatriz +
                    "v.desc `Linhagem` ";

            string commandTextCHICTabelasFaturamento =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamento =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento =
                    "trim(b.alt_desc) = '' and i.form in ('HE') and ";

            string dataInicialStrFaturamento = dataInicialFaturamento.ToString("MM/dd/yyyy");
            string dataFinalStrFaturamento = dataFinalFaturamento.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento =
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery " +

                    " Union ";

            string commandTextCHICCabecalhoFaturamento02 =
                "select " +
                    "b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                    commandTextCHICCabecalhoMatriz +
                    "v.desc `Linhagem` ";

            string commandTextCHICTabelasFaturamento02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamento02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento02 =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrFaturamentoCalDate = dataInicialFaturamento.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrFaturamentoCalDate = dataFinalFaturamento.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento02 =
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento02 =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery ";

            string commandTextCHICOrdenacaoFaturamento =
                "order by " +
                    "1";

            #region Dados Faturamento

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
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
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
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 =
                "order by " +
                    "b.item_ord";

            #endregion

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
                        commandTextCHICCabecalhoFaturamento02 + commandTextCHICTabelasFaturamento02 +
                        commandTextCHICCondicaoJoinsFaturamento02 +
                        commandTextCHICCondicaoFiltrosFaturamento02 + commandTextCHICCondicaoParametrosFaturamento02 +
                        commandTextCHICAgrupamentoFaturamento02 +
                        commandTextCHICOrdenacaoFaturamento;
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

        public string GeraRelatorioVerificacaoFinalPLAnual(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, string empresa, string empresaLayoutRelatorio, string vendedor,
            string nomeVendedor, string relatorio, string origem)
        {
            //string destino = "\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados\\Lista_Pedidos_" + empresa + ".xlsx";

            //string pesquisa = "*Lista_Pedidos_" + empresa + "*.xlsx";

            //string[] files = Directory.GetFiles("\\\\Srv-app-01\\w\\Relatorios_CHIC\\Pedidos_Importados", pesquisa);
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            //string empresaLayoutRelatorio = "";

            //if (empresa.Equals("(Todas)"))
            //    empresaLayoutRelatorio = "BR";
            //else
            //    empresaLayoutRelatorio = empresa;

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\" + relatorio + "_"
                + empresaLayoutRelatorio + ".xlsx", destino);

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

            string commandTextCHICCabecalhoVendedor = "";
            if (origem == "Vendedor" || empresa == "LB")
                commandTextCHICCabecalhoVendedor = "o.delivery `Cond. Pagmto.`, ";

            //string commandTextCHICCabecalhoMatriz = "";
            //if (empresa != "PL")
            //    commandTextCHICCabecalhoMatriz = "min(i.form) `Matriz`, ";

            #region Faturamento

            DateTime dataInicialFaturamento = Convert.ToDateTime("01/01/"+DateTime.Today.Year.ToString()).AddDays(-21).AddDays(3);
            DateTime dataFinalFaturamento = Convert.ToDateTime("31/12/" + DateTime.Today.Year.ToString()).AddDays(-21).AddDays(8);

            Excel._Worksheet worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento"];

            worksheetFaturamento.Cells[2, 7] = dataInicialFaturamento.AddDays(21).ToString("dd/MM/yyyy") + " à " +
                dataFinalFaturamento.AddDays(21).ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetFaturamento.Cells[3, 7] = vendedor;
            else
                worksheetFaturamento.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoFaturamento =
                "select " +
                    "b.cal_date `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                //commandTextCHICCabecalhoMatriz +
                    "i.form, " +
                    "v.desc `Linhagem` ";

            string commandTextCHICTabelasFaturamento =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamento =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento =
                    "trim(b.alt_desc) = '' and i.form in ('HE') and ";

            string dataInicialStrFaturamento = dataInicialFaturamento.AddDays(-21).ToString("MM/dd/yyyy");
            string dataFinalStrFaturamento = dataFinalFaturamento.AddDays(-21).ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento =
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery " +

                    " Union ";

            string commandTextCHICCabecalhoFaturamento02 =
                "select " +
                    "b.cal_date+21 `Nascimento`, " +
                    "o.del_date `Entrega`, " +
                    "o.orderno `Nº Pedido`, " +
                    "c.name `Cliente`, " +
                    "c.city `Cidade`, " +
                    "c.state `UF`, " +
                    commandTextCHICCabecalhoVendedor +
                //commandTextCHICCabecalhoMatriz +
                    "i.form, " +
                    "v.desc `Linhagem` ";

            string commandTextCHICTabelasFaturamento02 =
                "from " +
                    "booked b, orders o, items i, vartabl v, cust c, salesman s ";

            string commandTextCHICCondicaoJoinsFaturamento02 =
                "where " +
                    "b.orderno = o.orderno and " +
                    "b.item = i.item_no and  " +
                    "i.variety = v.variety and " +
                    "o.cust_no = c.custno and " +
                    "o.salesrep = s.sl_code and ";

            string commandTextCHICCondicaoFiltrosFaturamento02 =
                    "trim(b.alt_desc) = '' and i.form in ('DO','DN','DV') and ";

            string dataInicialStrFaturamentoCalDate = dataInicialFaturamento.ToString("MM/dd/yyyy");
            string dataFinalStrFaturamentoCalDate = dataFinalFaturamento.ToString("MM/dd/yyyy");

            string commandTextCHICCondicaoParametrosFaturamento02 =
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento02 =
                "group by " +
                    "b.cal_date, " +
                    "o.del_date, " +
                    "o.orderno, " +
                    "c.name, " +
                    "c.city, " +
                    "c.state, " +
                    "v.desc, " +
                    "i.form, " +
                    "o.delivery ";

            string commandTextCHICOrdenacaoFaturamento =
                "order by " +
                    "1";

            #region Dados Faturamento

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
                    "b.cal_date between {" + dataInicialStrFaturamento + "} and {" + dataFinalStrFaturamento + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
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
                    "b.cal_date between {" + dataInicialStrFaturamentoCalDate + "} and {" + dataFinalStrFaturamentoCalDate + "} and " +
                    "(s.inv_comp = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(s.sl_code = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamentoDados02 = "";

            string commandTextCHICOrdenacaoFaturamentoDados02 =
                    "order by b.item_ord";

            #endregion

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
                        commandTextCHICCabecalhoFaturamento02 + commandTextCHICTabelasFaturamento02 +
                        commandTextCHICCondicaoJoinsFaturamento02 +
                        commandTextCHICCondicaoFiltrosFaturamento02 + commandTextCHICCondicaoParametrosFaturamento02 +
                        commandTextCHICAgrupamentoFaturamento02 +
                        commandTextCHICOrdenacaoFaturamento;
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

        public void EnviarVerificacaoFinal()
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Inicializar Variáveis

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final";
                string empresa = "";

                DateTime dataInicio = DateTime.Today;
                //DateTime dataInicio = Convert.ToDateTime("26/11/2019");

                DateTime dataInicialFaturamento = dataInicio.AddDays(5);
                DateTime dataFinalFaturamento = dataInicio.AddDays(11);

                DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
                DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

                DateTime dataInicialIncubacao = (dataFinalLotesJaIncubados.AddDays(1)).AddDays(-21);
                DateTime dataFinalIncubacao = (dataFinalLotesJaIncubados.AddDays(14)).AddDays(-21);

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Hy-Line

                empresa = "BR";

                #region Vendedores / Representantes

                CHICDataSet.salesmanDataTable vendedores = new CHICDataSet.salesmanDataTable();
                salesman.FillByEmpresa(vendedores, empresa);

                var listaVendedores = vendedores.ToList();

                foreach (var itemVendedor in listaVendedores)
                {
                    /*string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõçZÁÉÍÓÚÀÈÌÒÙÂÊÎÔÛÃÕÇ\s]";
                    string replacement = "";
                    Regex rgx = new Regex(pattern);
                    string nameFileOld = itemVendedor.salesman.Trim().Replace("/", "").Replace("\\", "");
                    string nameFileNew = rgx.Replace(nameFileOld, replacement);*/

                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + itemVendedor.sl_code.Trim() + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinal("*" + itemVendedor.sl_code.Trim() + "*", true,
                        pasta, destino, dataInicio, empresa, empresa, itemVendedor.sl_code.Trim(),
                        itemVendedor.salesman.Trim(), "Verificacao_Final",
                        "Vendedor");

                    #region Envio de E-mail

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = itemVendedor.salesman.Trim();
                    email.WorkFlowEmailParaEmail = itemVendedor.email.Trim();
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                    //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                    email.WorkFlowEmailCopiaPara = "jcarchangelo@hyline.com.br;mchecco@hyline.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + itemVendedor.salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino;

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();

                    #endregion
                }

                #endregion

                #region Técnicos

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Tecnico_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinal("*Verificacao_Final_Tecnico_" + empresa + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, "(Todos)", "", "Verificacao_Final_Tecnico", "");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Assistência Técnica";
                email.WorkFlowEmailParaEmail = "a.tecnica@hyline.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "apadovan@hyline.com.br;bguastalli@hyline.com.br;"
                //    + "jcarchangelo@hyline.com.br;msantos@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados Técnicos," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório verificação final para auxiliá-los no acompanhamento técnico."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion

                #region Lohmann

                empresa = "LB";

                #region Vendedores / Representantes

                vendedores = new CHICDataSet.salesmanDataTable();
                salesman.FillByEmpresa(vendedores, empresa);

                var listaVendedoresLohmann = vendedores.ToList();

                string anexos = "";

                foreach (var itemVendedor in listaVendedoresLohmann)
                {
                    //destino = "\\\\Srv-app-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                    //empresa + "_" + itemVendedor.salesman.Trim().Replace("/", "").Replace("\\", "") + ".xlsx";

                    //string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]";
                    //string replacement = "";
                    //Regex rgx = new Regex(pattern);
                    //string nameFileOld = itemVendedor.salesman.Trim().Replace("/", "").Replace("\\", "");
                    //string nameFileNew = rgx.Replace(nameFileOld, replacement);

                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + itemVendedor.sl_code.Trim() + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinal("*" + itemVendedor.sl_code.Trim() + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, itemVendedor.sl_code.Trim(), itemVendedor.salesman.Trim(), "Verificacao_Final",
                        "Vendedor");

                    //if (anexos == "")
                    //    anexos = destino;
                    //else
                    //    anexos = anexos + "^" + destino;

                    #region Envio de E-mail

                    #region Verifica Se existe Supervisores para gerar a copia

                    string copiaPara = "";
                    string codigoVendedorApolo = "0" + itemVendedor.sl_code.Trim();
                    ApoloEntities2 apolo2 = new ApoloEntities2();
                    var listaSupVend = apolo2.SUP_VENDEDOR
                        .Where(w => w.VendCod == codigoVendedorApolo
                            && w.FxaCod.Equals("0000003"))
                        .ToList();

                    foreach (var sup in listaSupVend)
                    {
                        VENDEDOR supervisor = apolo.VENDEDOR
                            .Where(w => w.VendCod == sup.SupVendCod).FirstOrDefault();

                        if (supervisor != null)
                        {
                            if (supervisor.USERLoginSite != "")
                                copiaPara = copiaPara + supervisor.USERLoginSite + ";";
                        }
                    }

                    #endregion

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = itemVendedor.salesman.Trim();
                    email.WorkFlowEmailParaEmail = itemVendedor.email.Trim();
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                    //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                    email.WorkFlowEmailCopiaPara = copiaPara + "confirmacoes@ltz.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                    email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + itemVendedor.salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino;

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();

                    #endregion
                }

                #endregion

                #region Envio de E-mail - DESATIVADO POIS FOI SOLICITADO ENVIAR AOS VENDEDORES

                //email = new WORKFLOW_EMAIL();

                //numero = new ObjectParameter("codigo", typeof(global::System.String));

                //apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                //email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                //email.WorkFlowEmailStat = "Enviar";
                //email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                //email.WorkFlowEmailData = DateTime.Now;
                //email.WorkFlowEmailParaNome = "LOHMANN";
                //email.WorkFlowEmailParaEmail = "confirmacoes@ltz.com.br";
                ////email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                ////email.WorkFlowEmailCopiaPara = "esouza@ltz.com.br";
                //email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                //email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                //email.WorkFlowEmailFormato = "Texto";

                //corpoEmail = "";

                //corpoEmail = "Prezados ," + (char)13 + (char)10 + (char)13 + (char)10
                //        + "Segue anexo relatórios para Verificação Final antes do Faturamento de "
                //        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                //        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                //        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                //        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                //        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                //        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                //        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                //        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                //        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                //        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                //        + "SISTEMA WEB";

                //email.WorkFlowEmailCorpo = corpoEmail;
                //email.WorkFlowEmailArquivosAnexos = anexos;

                //apolo.WORKFLOW_EMAIL.AddObject(email);

                //apolo.SaveChanges();

                #endregion

                #region Técnicos

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Tecnico_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinal("*Verificacao_Final_Tecnico_" + empresa + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, "(Todos)", "", "Verificacao_Final_Tecnico", "");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Assistência Técnica";
                email.WorkFlowEmailParaEmail = "a.tecnica@ltz.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "lklassmann@ltz.com.br;asilva@ltz.com.br;"
                //    + "esouza@ltz.com.br;clima@ltz.com.br;mfraga@ltz.com.br";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados Técnicos," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório verificação final para auxiliá-los no acompanhamento técnico."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #region Anual

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Anual_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinalLBAnual("*Verificacao_Final_Anual_" + empresa + "*", 
                    true, pasta, destino, dataInicio, empresa, empresa, "(Todos)", "",
                    "Verificacao_Final_Anual", "");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL - " + DateTime.Today.Year.ToString();
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Comercial";
                email.WorkFlowEmailParaEmail = "lklassman@ltz.com.br";
                email.WorkFlowEmailCopiaPara = "esouza@ltz.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório com a Verificação Final do Ano " + DateTime.Today.Year.ToString() +"."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion

                #region H&N

                empresa = "HN";

                #region Separado

                vendedores = new CHICDataSet.salesmanDataTable();
                salesman.FillByEmpresa(vendedores, empresa);

                var listaVendedoresHN = vendedores.ToList();

                anexos = "";

                foreach (var itemVendedor in listaVendedoresHN)
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + itemVendedor.sl_code.Trim() + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinal("*" + itemVendedor.sl_code.Trim() + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, itemVendedor.sl_code.Trim(), itemVendedor.salesman.Trim(), "Verificacao_Final",
                        "Vendedor");

                    if (anexos == "")
                        anexos = destino;
                    else
                        anexos = anexos + "^" + destino;
                }

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "H&N";
                email.WorkFlowEmailParaEmail = "confirmacoes@hnavicultura.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "esouza@ltz.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB H&N";
                email.WorkFLowEmailDeEmail = "sistemas@hnavicultura.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados ," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatórios para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = anexos;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #region Consolidado

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Consolidado_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinal("*Verificacao_Final_Consolidado_" + empresa + "*",
                    true, pasta, destino, dataInicio,
                    empresa, empresa, "(Todos)", "", "Verificacao_Final_Consolidado", "Vendedor");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL CONSOLIDADA";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "H&N";
                email.WorkFlowEmailParaEmail = "hn@hnavicultura.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB H&N";
                email.WorkFLowEmailDeEmail = "sistemas@hnavicultura.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados ," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion
            }
            catch (Exception e)
            {

            }
        }

        public void EnviarVerificacaoFinalTeste()
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Inicializar Variáveis

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final";
                string empresa = "";

                DateTime dataInicio = DateTime.Today;
                //DateTime dataInicio = Convert.ToDateTime("15/09/2015");

                DateTime dataInicialFaturamento = dataInicio.AddDays(5);
                DateTime dataFinalFaturamento = dataInicio.AddDays(11);

                DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
                DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

                DateTime dataInicialIncubacao = (dataFinalLotesJaIncubados.AddDays(1)).AddDays(-21);
                DateTime dataFinalIncubacao = (dataFinalLotesJaIncubados.AddDays(14)).AddDays(-21);

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Empresa

                empresa = "PL";

                #region Vendedores / Representantes

                CHICDataSet.salesmanDataTable vendedores = new CHICDataSet.salesmanDataTable();
                salesman.FillByEmpresa(vendedores, empresa);

                var listaVendedores = vendedores
                    .Where(w => w.sl_code.Trim() == "000124")
                    .ToList();

                foreach (var itemVendedor in listaVendedores)
                {
                    /*string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõçZÁÉÍÓÚÀÈÌÒÙÂÊÎÔÛÃÕÇ\s]";
                    string replacement = "";
                    Regex rgx = new Regex(pattern);
                    string nameFileOld = itemVendedor.salesman.Trim().Replace("/", "").Replace("\\", "");
                    string nameFileNew = rgx.Replace(nameFileOld, replacement);*/

                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + itemVendedor.sl_code.Trim() + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinalPL("*" + itemVendedor.sl_code.Trim() + "*", true,
                        pasta, destino, dataInicio, empresa, empresa, itemVendedor.sl_code.Trim(),
                        itemVendedor.salesman.Trim(), "Verificacao_Final",
                        "Vendedor");

                    #region Envio de E-mail

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = itemVendedor.salesman.Trim();
                    //email.WorkFlowEmailParaEmail = itemVendedor.email.Trim();
                    email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                    //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                    //email.WorkFlowEmailCopiaPara = "jcarchangelo@hyline.com.br;mchecco@hyline.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + itemVendedor.salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino;

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();

                    #endregion
                }

                #endregion

                #endregion
            }
            catch (Exception e)
            {

            }
        }

        public string EnviarVerificacaoFinalPlanalto()
        {
            string erro = "";

            try
            {
                #region Inicializar Variáveis

                ApoloServiceEntities apoloLocal = new ApoloServiceEntities();

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final";
                string empresa = "";

                DateTime dataInicio = DateTime.Today;
                //DateTime dataInicio = Convert.ToDateTime("31/07/2020");

                DateTime dataInicialFaturamento = dataInicio.AddDays(3);
                DateTime dataFinalFaturamento = dataInicio.AddDays(8);

                DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
                DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

                DateTime dataInicialIncubacao = dataInicio.AddDays(10);
                DateTime dataFinalIncubacao = dataInicialIncubacao.AddDays(30);

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Planalto

                empresa = "PL";

                var listaVendedoresApolo = apoloLocal.VENDEDOR
                    .Where(w => w.USEREmpresa.Contains("PLANALTO") && w.USERRecebeEmailComercial.Equals("Sim")
                        && w.USERLoginSite != "" && w.USERLoginSite != null
                        //&& w.VendCod.Equals("0000134")
                        )
                    .ToList();

                foreach (var item in listaVendedoresApolo)
                {
                    string codVendCHIC = item.VendCod.Substring(1, 6);

                    #region Verificação Final Mensal

                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + codVendCHIC + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinalPL("*" + codVendCHIC + "*", true,
                        pasta, destino, dataInicio, empresa, empresa, codVendCHIC,
                        item.VendNome, "Verificacao_Final",
                        "Vendedor");

                    #endregion

                    #region Verificação Final Anual

                    string destinoAnual = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Anual_" +
                        empresa + "_" + codVendCHIC + ".xlsx";

                    destinoAnual = GeraRelatorioVerificacaoFinalPLAnual("*_Anual_" + codVendCHIC + "*", true,
                        pasta, destinoAnual, empresa, empresa, codVendCHIC,
                        item.VendNome, "Verificacao_Final_Anual",
                        "Vendedor");

                    #endregion

                    #region Envio de E-mail

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apoloLocal.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = item.VendNome;
                    email.WorkFlowEmailParaEmail = item.USERLoginSite;
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";

                    #region Verifica Se existe Supervisores para gerar a copia

                    string copiaPara = "";
                    ApoloEntities2 apolo2 = new ApoloEntities2();
                    var listaSupVend = apolo2.SUP_VENDEDOR
                        .Where(w => w.VendCod == item.VendCod
                            && w.FxaCod.Equals("0000003"))
                        .ToList();

                    foreach (var sup in listaSupVend)
                    {
                        VENDEDOR supervisor = apoloLocal.VENDEDOR
                            .Where(w => w.VendCod == sup.SupVendCod).FirstOrDefault();

                        if (supervisor != null)
                        {
                            if (supervisor.USERLoginSite != "")
                                copiaPara = copiaPara + supervisor.USERLoginSite + ";";
                        }
                    }

                    #endregion

                    copiaPara = copiaPara + "programacao@planaltopostura.com.br;olimpio.planaltopostura@gmail.com";

                    email.WorkFlowEmailCopiaPara = copiaPara;
                    email.WorkFlowEmailDeNome = "Sistema WEB Planalto";
                    email.WorkFLowEmailDeEmail = "sistemas@planaltopostura.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + item.VendNome + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final do Faturamento da Próxima Semana ("
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ")"
                        + ", a Programação das Próximas 04 Semanas ("
                        + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + ") " 
                        + "e as Duplicatas a Receber." + (char)13 + (char)10
                        + "Também, segue em anexo a Verificação Final do Ano de " + DateTime.Today.Year.ToString() + "."
                        + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino + ";" + destinoAnual;

                    apoloLocal.WORKFLOW_EMAIL.AddObject(email);

                    apoloLocal.SaveChanges();

                    #endregion
                }

                #endregion

            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erro = "Erro EnviarVerificacaoFinalPlanalto Linha Código: " + linenum.ToString()
                    + " / " + ex.Message;
                if (ex.InnerException != null)
                    erro = erro + " / " + ex.InnerException.Message;
            }
            
            return erro;
        }

        #region AniPlan

        public string GeraRelatorioVerificacaoFinalAniPlan(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicio, string empresa, string empresaLayoutRelatorio, string vendedor,
            string nomeVendedor, string relatorio, string origem, string modeloRelatorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Verificacao_Final\\" + relatorio + ".xlsx", destino);

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

            string commandTextCHICCabecalhoVendedor = "";
            string commandTextCHICCabecalhoMatriz = "";
            if (origem == "Vendedor" || empresa == "LB")
                commandTextCHICCabecalhoVendedor = "[Cond. Pagmto.], ";

            if (empresa != "PL" && origem != "Vendedor")
                commandTextCHICCabecalhoMatriz = "[Tipo de Matriz], ";

            #region Faturamento

            DateTime dataInicialFaturamento = dataInicio.AddDays(5);
            DateTime dataFinalFaturamento = dataInicio.AddDays(11);
            if (modeloRelatorio == "Anual")
            {
                dataInicialFaturamento = Convert.ToDateTime("01/01/" + DateTime.Today.Year.ToString());
                dataFinalFaturamento = Convert.ToDateTime("31/12/" + DateTime.Today.Year.ToString());
            }

            Excel._Worksheet worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento"];

            worksheetFaturamento.Cells[2, 7] = dataInicialFaturamento.ToString("dd/MM/yyyy") + " à " +
                dataFinalFaturamento.ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetFaturamento.Cells[3, 7] = vendedor;
            else
                worksheetFaturamento.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoFaturamento =
                "select " +
                    "[Ordem], " +
                    "[Tipo Pedido], " +
                    "[Nascimento], " +
                    "[Entrega], " +
                    "[ID Pedido], " +
                    "[Cliente], " +
                    "[Cidade], " +
                    "[UF], " +
                    "[Linhagem], " +
                    "[Incubatório], " +
                    "[Produto], " +
                    commandTextCHICCabecalhoMatriz +
                    "[Conf. Assinada], " +
                    commandTextCHICCabecalhoVendedor +
                    "[Preço do Produto], " +
                    "[Preço Vacinas e Serviços], " +
                    "[Preço Total], " +
                    "[Qtde. Vendida Total], " +
                    "[Qtde. Bonificada], " +
                    "[Qtde. Total], " +
                    "[Valor Total], " +
                    "[Vacina Primaria], " +
                    "[Secundária 1], " +
                    "[Secundária 2], " +
                    "[Secundária 3], " +
                    "[Secundária 4], " +
                    "[Secundária 5], " +
                    "[Secundária 6], " +
                    "[Qtde. Tratamento Infravermelho], " +
                    "[Tipo de Caixa], " +
                    "[Cód. Estabelecimento], " +
                    "[Nº Registro], " +
                    "[CPF / CNPJ], " +
                    "[Validade Registro] ";

            string commandTextCHICTabelasFaturamento =
                "from " +
                    "VU_Verificacao_Final ";

            string commandTextCHICCondicaoJoinsFaturamento =
                "where ";

            string commandTextCHICCondicaoFiltrosFaturamento = "";

            string dataInicialStrFaturamento = dataInicialFaturamento.ToString("yyyy-MM-dd");
            string dataFinalStrFaturamento = dataFinalFaturamento.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosFaturamento =
                    "[Nascimento] between '" + dataInicialStrFaturamento + "' and '" + dataFinalStrFaturamento + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento = "";

            string commandTextCHICOrdenacaoFaturamento =
                "order by [Nascimento], [Ordem]";

            #endregion

            #region Lotes Já Incubados

            DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
            DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

            if (modeloRelatorio != "Anual")
            {
                Excel._Worksheet worksheetLotesJaIncubados = (Excel._Worksheet)oBook.Worksheets["Lotes Já Incubados"];

                worksheetLotesJaIncubados.Cells[2, 7] = dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " à " +
                    dataFinalLotesJaIncubados.ToString("dd/MM/yyyy");
                if (nomeVendedor == "")
                    worksheetLotesJaIncubados.Cells[3, 7] = vendedor;
                else
                    worksheetLotesJaIncubados.Cells[3, 7] = nomeVendedor;
            }

            string commandTextCHICCabecalhoLotesJaIncubados =
                "select " +
                    "[Ordem], " +
                    "[Tipo Pedido], " +
                    "[Nascimento], " +
                    "[Entrega], " +
                    "[ID Pedido], " +
                    "[Cliente], " +
                    "[Cidade], " +
                    "[UF], " +
                    "[Linhagem], " +
                    "[Incubatório], " +
                    "[Produto], " +
                    commandTextCHICCabecalhoMatriz +
                    "[Conf. Assinada], " +
                    commandTextCHICCabecalhoVendedor +
                    "[Preço do Produto], " +
                    "[Preço Vacinas e Serviços], " +
                    "[Preço Total], " +
                    "[Qtde. Vendida Total], " +
                    "[Qtde. Bonificada], " +
                    "[Qtde. Total], " +
                    "[Valor Total], " +
                    "[Vacina Primaria], " +
                    "[Secundária 1], " +
                    "[Secundária 2], " +
                    "[Secundária 3], " +
                    "[Secundária 4], " +
                    "[Secundária 5], " +
                    "[Secundária 6], " +
                    "[Qtde. Tratamento Infravermelho], " +
                    "[Tipo de Caixa], " +
                    "[Cód. Estabelecimento], " +
                    "[Nº Registro], " +
                    "[CPF / CNPJ], " +
                    "[Validade Registro] ";

            string commandTextCHICTabelasLotesJaIncubados =
                "from " +
                    "VU_Verificacao_Final ";

            string commandTextCHICCondicaoJoinsLotesJaIncubados =
                "where ";

            string commandTextCHICCondicaoFiltrosLotesJaIncubados = "";

            string dataInicialStrLotesJaIncubados = dataInicialLotesJaIncubados.ToString("yyyy-MM-dd");
            string dataFinalStrLotesJaIncubados = dataFinalLotesJaIncubados.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosLotesJaIncubados =
                    "[Nascimento] between '" + dataInicialStrLotesJaIncubados + "' and '" + dataFinalStrLotesJaIncubados + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoLotesJaIncubados = "";

            string commandTextCHICOrdenacaoLotesJaIncubados =
                "order by [Nascimento], [Ordem]";

            #endregion

            #region Incubação

            DateTime dataInicialIncubacao = (dataFinalLotesJaIncubados.AddDays(1)).AddDays(-21);
            DateTime dataFinalIncubacao = (dataFinalLotesJaIncubados.AddDays(14)).AddDays(-21);

            if (modeloRelatorio != "Anual")
            {
                Excel._Worksheet worksheetIncubacao = (Excel._Worksheet)oBook.Worksheets["Incubação"];

                worksheetIncubacao.Cells[2, 7] = dataInicialIncubacao.ToString("dd/MM/yyyy") + " à " +
                    dataFinalIncubacao.ToString("dd/MM/yyyy");
                if (nomeVendedor == "")
                    worksheetIncubacao.Cells[3, 7] = vendedor;
                else
                    worksheetIncubacao.Cells[3, 7] = nomeVendedor;
            }

            string commandTextCHICCabecalhoIncubacao =
                "select " +
                    "[Ordem], " +
                    "[Tipo Pedido], " +
                    "[Nascimento], " +
                    "[Entrega], " +
                    "[ID Pedido], " +
                    "[Cliente], " +
                    "[Cidade], " +
                    "[UF], " +
                    "[Linhagem], " +
                    "[Incubatório], " +
                    "[Produto], " +
                    commandTextCHICCabecalhoMatriz +
                    "[Conf. Assinada], " +
                    commandTextCHICCabecalhoVendedor +
                    "[Preço do Produto], " +
                    "[Preço Vacinas e Serviços], " +
                    "[Preço Total], " +
                    "[Qtde. Vendida Total], " +
                    "[Qtde. Bonificada], " +
                    "[Qtde. Total], " +
                    "[Valor Total], " +
                    "[Vacina Primaria], " +
                    "[Secundária 1], " +
                    "[Secundária 2], " +
                    "[Secundária 3], " +
                    "[Secundária 4], " +
                    "[Secundária 5], " +
                    "[Secundária 6], " +
                    "[Qtde. Tratamento Infravermelho], " +
                    "[Tipo de Caixa], " +
                    "[Cód. Estabelecimento], " +
                    "[Nº Registro], " +
                    "[CPF / CNPJ], " +
                    "[Validade Registro] ";

            string commandTextCHICTabelasIncubacao =
                "from " +
                    "VU_Verificacao_Final ";

            string commandTextCHICCondicaoJoinsIncubacao =
                "where " +
                    "[Produto] = 'Pinto' and ";

            string commandTextCHICCondicaoFiltrosIncubacao = "";

            string dataInicialStrIncubacao = dataInicialIncubacao.ToString("yyyy-MM-dd");
            string dataFinalStrIncubacao = dataFinalIncubacao.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosIncubacao =
                    "[Nascimento]-21 between '" + dataInicialStrIncubacao + "' and '" + dataFinalStrIncubacao + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoIncubacao = "";

            string commandTextCHICOrdenacaoIncubacao =
                "order by [Nascimento], [Ordem]";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("Faturamento"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamento + commandTextCHICTabelasFaturamento +
                        commandTextCHICCondicaoJoinsFaturamento +
                        commandTextCHICCondicaoFiltrosFaturamento + commandTextCHICCondicaoParametrosFaturamento +
                        commandTextCHICAgrupamentoFaturamento +
                        commandTextCHICOrdenacaoFaturamento;
                else if (item.Name.Equals("Ja_Incubados"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoLotesJaIncubados + commandTextCHICTabelasLotesJaIncubados +
                        commandTextCHICCondicaoJoinsLotesJaIncubados +
                        commandTextCHICCondicaoFiltrosLotesJaIncubados + commandTextCHICCondicaoParametrosLotesJaIncubados +
                        commandTextCHICAgrupamentoLotesJaIncubados +
                        commandTextCHICOrdenacaoLotesJaIncubados;
                else if (item.Name.Equals("Incubacao"))
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

        public string GeraRelatorioVerificacaoFinalPLAniPlan(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicio, string empresa, string empresaLayoutRelatorio, string vendedor,
            string nomeVendedor, string relatorio, string origem, string modeloRelatorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Verificacao_Final\\" + relatorio + ".xlsx", destino);

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

            string commandTextCHICCabecalhoVendedor = "";
            if (origem == "Vendedor" || empresa == "LB")
                commandTextCHICCabecalhoVendedor = "[Cond. Pagmto.], ";

            #region Faturamento Próxima Semana

            DateTime dataInicialFaturamento = dataInicio.AddDays(3);
            DateTime dataFinalFaturamento = dataInicio.AddDays(8);

            Excel._Worksheet worksheetFaturamento;

            if (modeloRelatorio == "Anual")
            {
                worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento"];

                dataInicialFaturamento = Convert.ToDateTime("01/01/" + DateTime.Today.Year.ToString()).AddDays(-21).AddDays(3);
                dataFinalFaturamento = Convert.ToDateTime("31/12/" + DateTime.Today.Year.ToString()).AddDays(-21).AddDays(8);
                worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento"];
            }
            else
            {
                worksheetFaturamento = (Excel._Worksheet)oBook.Worksheets["Faturamento Próxima Semana"];
            }

            worksheetFaturamento.Cells[2, 7] = dataInicialFaturamento.ToString("dd/MM/yyyy") + " à " +
                dataFinalFaturamento.ToString("dd/MM/yyyy");
            if (nomeVendedor == "")
                worksheetFaturamento.Cells[3, 7] = vendedor;
            else
                worksheetFaturamento.Cells[3, 7] = nomeVendedor;

            string commandTextCHICCabecalhoFaturamento =
                "select " +
                    "[Ordem], " +
                    "[Tipo Pedido], " +
                    "[Nascimento], " +
                    "[Entrega], " +
                    "[ID Pedido], " +
                    "[Cliente], " +
                    "[Cidade], " +
                    "[UF], " +
                    "[Linhagem], " +
                    "[Incubatório], " +
                    "[Produto], " +
                    "[Conf. Assinada], " +
                    commandTextCHICCabecalhoVendedor +
                    "[Preço do Produto], " +
                    "[Preço Vacinas e Serviços], " +
                    "[Preço Total], " +
                    "[Qtde. Vendida Total], " +
                    "[Qtde. Bonificada], " +
                    "[Qtde. Total], " +
                    "[Valor Total], " +
                    "[Vacina Primaria], " +
                    "[Secundária 1], " +
                    "[Secundária 2], " +
                    "[Secundária 3], " +
                    "[Secundária 4], " +
                    "[Secundária 5], " +
                    "[Secundária 6], " +
                    "[Qtde. Tratamento Infravermelho], " +
                    "[Tipo de Caixa], " +
                    "[Cód. Estabelecimento], " +
                    "[Nº Registro], " +
                    "[CPF / CNPJ], " +
                    "[Validade Registro] ";

            string commandTextCHICTabelasFaturamento =
                "from " +
                    "VU_Verificacao_Final ";

            string commandTextCHICCondicaoJoinsFaturamento =
                "where ";

            string commandTextCHICCondicaoFiltrosFaturamento = "";

            string dataInicialStrFaturamento = dataInicialFaturamento.ToString("yyyy-MM-dd");
            string dataFinalStrFaturamento = dataFinalFaturamento.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosFaturamento =
                    "[Nascimento] between '" + dataInicialStrFaturamento + "' and '" + dataFinalStrFaturamento + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoFaturamento = "";

            string commandTextCHICOrdenacaoFaturamento =
                "order by [Nascimento], [Ordem]";

            #endregion

            #region Programação Próxima 04 Semanas

            DateTime dataInicialIncubacao = dataInicio.AddDays(10);
            DateTime dataFinalIncubacao = dataInicialIncubacao.AddDays(30);

            if (modeloRelatorio != "Anual")
            {
                Excel._Worksheet worksheetIncubacao = (Excel._Worksheet)oBook.Worksheets["Programação Próxima 04 Semanas"];

                worksheetIncubacao.Cells[2, 7] = dataInicialIncubacao.ToString("dd/MM/yyyy") + " à " +
                    dataFinalIncubacao.ToString("dd/MM/yyyy");
                if (nomeVendedor == "")
                    worksheetIncubacao.Cells[3, 7] = vendedor;
                else
                    worksheetIncubacao.Cells[3, 7] = nomeVendedor;
            }

            string commandTextCHICCabecalhoIncubacao =
                "select " +
                    "[Ordem], " +
                    "[Tipo Pedido], " +
                    "[Nascimento], " +
                    "[Entrega], " +
                    "[ID Pedido], " +
                    "[Cliente], " +
                    "[Cidade], " +
                    "[UF], " +
                    "[Linhagem], " +
                    "[Incubatório], " +
                    "[Produto], " +
                    "[Conf. Assinada], " +
                    commandTextCHICCabecalhoVendedor +
                    "[Preço do Produto], " +
                    "[Preço Vacinas e Serviços], " +
                    "[Preço Total], " +
                    "[Qtde. Vendida Total], " +
                    "[Qtde. Bonificada], " +
                    "[Qtde. Total], " +
                    "[Valor Total], " +
                    "[Vacina Primaria], " +
                    "[Secundária 1], " +
                    "[Secundária 2], " +
                    "[Secundária 3], " +
                    "[Secundária 4], " +
                    "[Secundária 5], " +
                    "[Secundária 6], " +
                    "[Qtde. Tratamento Infravermelho], " +
                    "[Tipo de Caixa], " +
                    "[Cód. Estabelecimento], " +
                    "[Nº Registro], " +
                    "[CPF / CNPJ], " +
                    "[Validade Registro] ";

            string commandTextCHICTabelasIncubacao =
                "from " +
                    "VU_Verificacao_Final ";

            string commandTextCHICCondicaoJoinsIncubacao =
                "where ";

            string commandTextCHICCondicaoFiltrosIncubacao = "";

            string dataInicialStrIncubacao = dataInicialIncubacao.ToString("yyyy-MM-dd");
            string dataFinalStrIncubacao = dataFinalIncubacao.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosIncubacao =
                    "[Nascimento] between '" + dataInicialStrIncubacao + "' and '" + dataFinalStrIncubacao + "' and " +
                    "(Empresa = '" + empresa + "' or '" + empresa + "' = '(Todas)') and " +
                    "(CodigoRepresentante = '" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoIncubacao = "";

            string commandTextCHICOrdenacaoIncubacao =
                "order by [Nascimento], [Ordem]";

            #endregion

            #region Duplicatas a Receber

            string commandTextCHICCabecalhoDR =
                "select * ";

            string commandTextCHICTabelasDR =
                "from " +
                    "VU_Duplicatas_Receber_Por_Entidade  ";

            string commandTextCHICCondicaoJoinsDR =
                "where ";

            string commandTextCHICCondicaoFiltrosDR = "";

            string commandTextCHICCondicaoParametrosDR =
                    "([Vend. / Repres.] = '0" + vendedor + "' or '" + vendedor + "' = '(Todos)') ";

            string commandTextCHICAgrupamentoDR = "";

            string commandTextCHICOrdenacaoDR =
                "order by " +
                    "9, 1";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("Faturamento"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFaturamento + commandTextCHICTabelasFaturamento +
                        commandTextCHICCondicaoJoinsFaturamento +
                        commandTextCHICCondicaoFiltrosFaturamento + commandTextCHICCondicaoParametrosFaturamento +
                        commandTextCHICAgrupamentoFaturamento +
                        commandTextCHICOrdenacaoFaturamento;
                else if (item.Name.Equals("Incubacao"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoIncubacao + commandTextCHICTabelasIncubacao +
                        commandTextCHICCondicaoJoinsIncubacao +
                        commandTextCHICCondicaoFiltrosIncubacao + commandTextCHICCondicaoParametrosIncubacao +
                        commandTextCHICAgrupamentoIncubacao +
                        commandTextCHICOrdenacaoIncubacao;
                else if (item.Name.Equals("Duplicatas_Receber"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoDR + commandTextCHICTabelasDR +
                        commandTextCHICCondicaoJoinsDR +
                        commandTextCHICCondicaoFiltrosDR + commandTextCHICCondicaoParametrosDR +
                        commandTextCHICAgrupamentoDR +
                        commandTextCHICOrdenacaoDR;
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

        public string EnviarVerificacaoFinalAniPlan()
        {
            string erro = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Inicializar Variáveis

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final";
                string empresa = "";

                DateTime dataInicio = DateTime.Today;
                //DateTime dataInicio = Convert.ToDateTime("26/11/2019");

                DateTime dataInicialFaturamento = dataInicio.AddDays(5);
                DateTime dataFinalFaturamento = dataInicio.AddDays(11);

                DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
                DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

                DateTime dataInicialIncubacao = (dataFinalLotesJaIncubados.AddDays(1)).AddDays(-21);
                DateTime dataFinalIncubacao = (dataFinalLotesJaIncubados.AddDays(14)).AddDays(-21);

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Hy-Line

                empresa = "BR";

                #region Vendedores / Representantes

                var listaVendedores = apolo.VU_Vendedores_Ativos.Where(w => w.CodigoCHIC == empresa).ToList();
                //var listaVendedores = apolo.VU_Vendedores_Ativos.Where(w => w.VendCod == "0").ToList();

                foreach (var itemVendedor in listaVendedores)
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        itemVendedor.CodigoCHIC + "_" + itemVendedor.VendCod + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinalAniPlan("*" + itemVendedor.VendCod + "*", true,
                        pasta, destino, dataInicio, empresa, empresa, itemVendedor.VendCod, itemVendedor.VendNome, "Verificacao_Final_AniPlan",
                        "Vendedor", "Semanal");

                    #region Envio de E-mail

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = itemVendedor.VendNome;
                    email.WorkFlowEmailParaEmail = itemVendedor.Email;
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                    email.WorkFlowEmailCopiaPara = "jcarchangelo@hyline.com.br;mchecco@hyline.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + itemVendedor.VendNome + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino;

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();

                    #endregion
                }

                #endregion

                #region Técnicos

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Tecnico_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinalAniPlan("*Verificacao_Final_Tecnico_" + empresa + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, "(Todos)", "", "Verificacao_Final_AniPlan", "", "Semanal");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Assistência Técnica";
                email.WorkFlowEmailParaEmail = "a.tecnica@hyline.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "apadovan@hyline.com.br;bguastalli@hyline.com.br;"
                //    + "jcarchangelo@hyline.com.br;msantos@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados Técnicos," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório verificação final para auxiliá-los no acompanhamento técnico."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion

                #region Lohmann

                empresa = "LB";

                #region Vendedores / Representantes

                listaVendedores = apolo.VU_Vendedores_Ativos.Where(w => w.CodigoCHIC == empresa).ToList();
                //listaVendedores = apolo.VU_Vendedores_Ativos.Where(w => w.CodigoCHIC == "0").ToList();

                string anexos = "";

                foreach (var itemVendedor in listaVendedores)
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + itemVendedor.VendCod + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinalAniPlan("*" + itemVendedor.VendCod + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, itemVendedor.VendCod, itemVendedor.VendNome, "Verificacao_Final_AniPlan",
                        "Vendedor", "Semanal");

                    #region Envio de E-mail

                    #region Verifica Se existe Supervisores para gerar a copia

                    string copiaPara = "";
                    string codigoVendedorApolo = itemVendedor.VendCod;
                    ApoloEntities2 apolo2 = new ApoloEntities2();
                    var listaSupVend = apolo2.SUP_VENDEDOR
                        .Where(w => w.VendCod == codigoVendedorApolo
                            && w.FxaCod.Equals("0000003"))
                        .ToList();

                    foreach (var sup in listaSupVend)
                    {
                        VENDEDOR supervisor = apolo.VENDEDOR
                            .Where(w => w.VendCod == sup.SupVendCod).FirstOrDefault();

                        if (supervisor != null)
                        {
                            if (supervisor.USERLoginSite != "")
                                copiaPara = copiaPara + supervisor.USERLoginSite + ";";
                        }
                    }

                    #endregion

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = itemVendedor.VendNome;
                    email.WorkFlowEmailParaEmail = itemVendedor.Email;
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                    email.WorkFlowEmailCopiaPara = "";
                    email.WorkFlowEmailCopiaPara = copiaPara + "confirmacoes@ltz.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                    email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + itemVendedor.VendNome + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino;

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();

                    #endregion
                }

                #endregion

                #region Técnicos

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Tecnico_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinalAniPlan("*Verificacao_Final_Tecnico_" + empresa + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, "(Todos)", "", "Verificacao_Final_AniPlan", "", "Semanal");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Assistência Técnica";
                email.WorkFlowEmailParaEmail = "a.tecnica@ltz.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "lklassmann@ltz.com.br;asilva@ltz.com.br;"
                //    + "esouza@ltz.com.br;clima@ltz.com.br;mfraga@ltz.com.br";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados Técnicos," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório verificação final para auxiliá-los no acompanhamento técnico."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #region Anual

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Anual_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinalAniPlan("*Verificacao_Final_Anual_" + empresa + "*",
                    true, pasta, destino, dataInicio, empresa, empresa, "(Todos)", "",
                    "Verificacao_Final_Anual_AniPlan", "", "Anual");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL - " + DateTime.Today.Year.ToString();
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Comercial";
                email.WorkFlowEmailParaEmail = "lklassman@ltz.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailCopiaPara = "esouza@ltz.com.br;garaujo@ltz.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório com a Verificação Final do Ano " + DateTime.Today.Year.ToString() + "."
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion

                #region H&N

                empresa = "HN";

                #region Separado

                listaVendedores = apolo.VU_Vendedores_Ativos.Where(w => w.CodigoCHIC == empresa).ToList();
                //listaVendedores = apolo.VU_Vendedores_Ativos.Where(w => w.VendCod == "0000141").ToList();

                anexos = "";

                foreach (var itemVendedor in listaVendedores)
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + itemVendedor.VendCod + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinalAniPlan("*" + itemVendedor.VendCod + "*", true, pasta, destino, dataInicio,
                        empresa, empresa, itemVendedor.VendCod, itemVendedor.VendNome, "Verificacao_Final_AniPlan",
                        "Vendedor", "Semanal");

                    if (anexos == "")
                        anexos = destino;
                    else
                        anexos = anexos + "^" + destino;
                }

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "H&N";
                email.WorkFlowEmailParaEmail = "confirmacoes@hnavicultura.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB H&N";
                email.WorkFLowEmailDeEmail = "sistemas@hnavicultura.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados ," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatórios para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = anexos;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #region Consolidado

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Consolidado_" +
                    empresa + ".xlsx";

                destino = GeraRelatorioVerificacaoFinalAniPlan("*Verificacao_Final_Consolidado_" + empresa + "*",
                    true, pasta, destino, dataInicio,
                    empresa, empresa, "(Todos)", "", "Verificacao_Final_AniPlan", "Vendedor", "Semanal");

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL CONSOLIDADA";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "H&N";
                email.WorkFlowEmailParaEmail = "hn@hnavicultura.com.br";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                //email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB H&N";
                email.WorkFLowEmailDeEmail = "sistemas@hnavicultura.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados ," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final antes do Faturamento de "
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ", Lotes Já Incubados para Nascimento entre "
                        + dataInicialLotesJaIncubados.ToString("dd/MM/yyyy") + " a "
                        + dataFinalLotesJaIncubados.ToString("dd/MM/yyyy")
                        + " e Incubação de " + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erro = "Erro EnviarVerificacaoFinalPlanalto Linha Código: " + linenum.ToString()
                    + " / " + ex.Message;
                if (ex.InnerException != null)
                    erro = erro + " / " + ex.InnerException.Message;
            }
            
            return erro;
        }

        public string EnviarVerificacaoFinalPlanaltoAniPlan()
        {
            string erro = "";

            try
            {
                #region Inicializar Variáveis

                ApoloServiceEntities apoloLocal = new ApoloServiceEntities();

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final";
                string empresa = "";

                DateTime dataInicio = DateTime.Today;
                //DateTime dataInicio = Convert.ToDateTime("16/04/2021");

                DateTime dataInicialFaturamento = dataInicio.AddDays(3);
                DateTime dataFinalFaturamento = dataInicio.AddDays(8);

                DateTime dataInicialLotesJaIncubados = dataFinalFaturamento.AddDays(1);
                DateTime dataFinalLotesJaIncubados = dataFinalFaturamento.AddDays(14);

                DateTime dataInicialIncubacao = dataInicio.AddDays(10);
                DateTime dataFinalIncubacao = dataInicialIncubacao.AddDays(30);

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Planalto

                empresa = "PL";

                var listaVendedoresApolo = apoloLocal.VENDEDOR
                    .Where(w => w.USEREmpresa.Contains("PLANALTO") && w.USERRecebeEmailComercial.Equals("Sim")
                        && w.USERLoginSite != "" && w.USERLoginSite != null
                        //&& w.VendCod.Equals("0000134")
                        )
                    .ToList();

                foreach (var item in listaVendedoresApolo)
                {
                    string codVendCHIC = item.VendCod;

                    #region Verificação Final Mensal

                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_" +
                        empresa + "_" + codVendCHIC + ".xlsx";

                    destino = GeraRelatorioVerificacaoFinalPLAniPlan("*" + codVendCHIC + "*", true, pasta, destino, dataInicio, empresa, empresa, codVendCHIC,
                        item.VendNome, "Verificacao_Final_AniPlan_PL", "Vendedor", "Semanal");

                    #endregion

                    #region Verificação Final Anual

                    string destinoAnual = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\Verificacao_Final\\Verificacao_Final_Anual_" +
                        empresa + "_" + codVendCHIC + ".xlsx";

                    destinoAnual = GeraRelatorioVerificacaoFinalPLAniPlan("*_Anual_" + codVendCHIC + "*", true, pasta, destinoAnual, dataInicio, empresa, empresa, 
                        codVendCHIC, item.VendNome, "Verificacao_Final_Anual_AniPlan", "Vendedor", "Anual");

                    #endregion

                    #region Envio de E-mail

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apoloLocal.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "VERIFICAÇÃO FINAL";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = item.VendNome;
                    email.WorkFlowEmailParaEmail = item.USERLoginSite;
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";

                    #region Verifica Se existe Supervisores para gerar a copia

                    string copiaPara = "";
                    ApoloEntities2 apolo2 = new ApoloEntities2();
                    var listaSupVend = apolo2.SUP_VENDEDOR
                        .Where(w => w.VendCod == item.VendCod
                            && w.FxaCod.Equals("0000003"))
                        .ToList();

                    foreach (var sup in listaSupVend)
                    {
                        VENDEDOR supervisor = apoloLocal.VENDEDOR
                            .Where(w => w.VendCod == sup.SupVendCod).FirstOrDefault();

                        if (supervisor != null)
                        {
                            if (supervisor.USERLoginSite != "")
                                copiaPara = copiaPara + supervisor.USERLoginSite + ";";
                        }
                    }

                    #endregion

                    copiaPara = copiaPara + "programacao@planaltopostura.com.br;olimpio.planaltopostura@gmail.com";

                    email.WorkFlowEmailCopiaPara = copiaPara;
                    email.WorkFlowEmailDeNome = "Sistema WEB Planalto";
                    email.WorkFLowEmailDeEmail = "sistemas@planaltopostura.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + item.VendNome + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório para Verificação Final do Faturamento da Próxima Semana ("
                        + dataInicialFaturamento.ToString("dd/MM/yyyy") + " a "
                        + dataFinalFaturamento.ToString("dd/MM/yyyy") + ")"
                        + ", a Programação das Próximas 04 Semanas ("
                        + dataInicialIncubacao.ToString("dd/MM/yyyy") + " a "
                        + dataFinalIncubacao.ToString("dd/MM/yyyy") + ") "
                        + "e as Duplicatas a Receber." + (char)13 + (char)10
                        + "Também, segue em anexo a Verificação Final do Ano de " + DateTime.Today.Year.ToString() + "."
                        + (char)13 + (char)10 + (char)13 + (char)10
                        + "OBS.: POR FAVOR, CONFIRMEM SE AS QUANTIDADES, LINHAGENS, VACINAS, PREÇO, "
                        + "CONDIÇÕES DE PAGAMENTO E RAZÕES SOCIAIS CONFEREM COM O SOLICITADO / NEGOCIADO COM O CLIENTE, "
                        + "E CASO HAJA ALGUMA DIVERGÊNCIA NOS INFORMEM EM TEMPO HÁBIL PARA AS DEVIDAS CORREÇÕES ANTES DA "
                        + "INCUBAÇÃO E FATURAMENTO." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino + ";" + destinoAnual;

                    apoloLocal.WORKFLOW_EMAIL.AddObject(email);

                    apoloLocal.SaveChanges();

                    #endregion
                }

                #endregion

            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erro = "Erro EnviarVerificacaoFinalPlanalto Linha Código: " + linenum.ToString()
                    + " / " + ex.Message;
                if (ex.InnerException != null)
                    erro = erro + " / " + ex.InnerException.Message;
            }

            return erro;
        }

        #endregion

        #endregion

        #region Confirmações

        public string GeraRelConfirmacao(string orderNo, string cliente, string empresa)
        {
            //string caminho = @"\\srv-app-01\W\Confirmacoes\" + cliente.Replace("\\", "").Replace("/", "") + "_" + orderNo + ".pdf";
            string caminho = @"\\srv-riosoft-01\W\Confirmacoes\Pedido_" + orderNo + ".pdf";

            //CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportaCHIC)).CodeBase);
            //path = path.Replace("file:\\", "");
            //MyReport.Load(path + "\\Reports\\ConfirmacaoPedido_" + empresa + ".rpt");
            //MyReport.SetParameterValue("@pPedido", orderNo);
            //MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, caminho);

            return caminho;
        }

        public string EnviaConfirmacaoEmail(string caminho, string enderecoEmail, string nome, string orderno,
            string cliente, string copiaPara, string corpo, string assunto, string empresaApolo)
        {
            try
            {
                ApoloServiceEntities apoloSession = new ApoloServiceEntities();

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                apoloSession.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                //email.WorkFlowEmailAssunto = " **TESTE ** - CONFIRMAÇÃO DO PEDIDO " + orderno + " - " + cliente;
                if (assunto.Length > 80)
                    email.WorkFlowEmailAssunto = assunto.Substring(0, 80);
                else
                    email.WorkFlowEmailAssunto = assunto;
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = nome;
                email.WorkFlowEmailParaEmail = enderecoEmail;
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                /*if (copiaPara != "")
                    email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br;" + copiaPara;
                else
                    email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";*/
                email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";
                email.WorkFlowEmailDocEmpCod = empresaApolo;

                /*string corpoEmail = "";

                string stringChar = "" + (char)13 + (char)10;

                corpoEmail = "Prezado(s) faturista(s)," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue em anexo a Confirmação do pedido " + orderno + " do cliente " + cliente + " para ser faturado." + (char)13 + (char)10
                    + "Qualquer dúvida, entrar em contato pelo e-mail programacao@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";*/

                //email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailCorpo = corpo;
                email.WorkFlowEmailArquivosAnexos = caminho;

                apoloSession.WORKFLOW_EMAIL.AddObject(email);

                apoloSession.SaveChanges();

                return "";
            }
            catch (Exception e)
            {
                return "Erro ao enviar e-mail: " + e.Message;
            }
        }

        #endregion

        #region Dias de Estoque

        public string EnviarDiasEstoqueGranjas(string emailsCopia, string geracao)
        {
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Inicializar Variáveis

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque";

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";
                string emailPrincipal = "";

                #endregion

                #region Gerar Relatório - Granjas

                if (geracao == "Matrizes")
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque\\"
                        + "Relatorio_Dias_Estoque_Granjas.xlsx";

                    destino = AtualizaExcel("*Relatorio_Dias_Estoque_Granjas*",
                        true, pasta, destino, "Relatorio_Dias_Estoque_Granjas");
                    emailPrincipal = "tlourenco@hyline.com.br";
                }
                else
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque\\"
                        + "Relatorio_Dias_Estoque_Granjas_Avos.xlsx";

                    destino = AtualizaExcel("*Relatorio_Dias_Estoque_Granjas_Avos*",
                        true, pasta, destino, "Relatorio_Dias_Estoque_Granjas_Avos");

                    emailPrincipal = "jpereira@hyline.com.br";
                }

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "** DIAS ESTOQUE > 3 DIAS (GRANJAS " + geracao.ToUpper()
                    + ") - EMISSÃO: "
                    + DateTime.Today.ToShortDateString() + " **";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Produção";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailParaEmail = emailPrincipal;
                email.WorkFlowEmailCopiaPara = emailsCopia;
                email.WorkFlowEmailDeNome = "Sistema WEB";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório de Lotes com Dias de Estoque maior de que 03 dias nas Granjas!"
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Gerar Relatório de Dias de Estoquem - Granjas - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                return retorno;
            }
        }

        public string EnviarDiasEstoqueIncubatorios(string emailsCopia, string geracao)
        {
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Inicializar Variáveis

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque";

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";
                string emailPrincipal = "";
                string diasEstoque = "";

                #endregion

                #region Gerar Relatório - Incubatórios

                if (geracao == "Matrizes")
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque\\"
                        + "Relatorio_Dias_Estoque_Incubatorios.xlsx";

                    destino = AtualizaExcel("*Relatorio_Dias_Estoque_Incubatorios*",
                        true, pasta, destino, "Relatorio_Dias_Estoque_Incubatorios");
                    emailPrincipal = "tlourenco@hyline.com.br";
                    diasEstoque = "10";
                }
                else
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque\\"
                        + "Relatorio_Dias_Estoque_Incubatorio_Avos.xlsx";

                    destino = AtualizaExcel("*Relatorio_Dias_Estoque_Incubatorio_Avos*",
                        true, pasta, destino, "Relatorio_Dias_Estoque_Incubatorio_Avos");
                    emailPrincipal = "jpereira@hyline.com.br";
                    diasEstoque = "25";
                }

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "** DIAS ESTOQUE > " + diasEstoque + " DIAS (INCUBATÓRIOS "
                    + geracao.ToUpper() + ") - EMISSÃO: "
                    + DateTime.Today.ToShortDateString() + " **";

                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Produção";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailParaEmail = emailPrincipal;
                email.WorkFlowEmailCopiaPara = emailsCopia;
                email.WorkFlowEmailDeNome = "Sistema WEB";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório de Lotes com Dias de Estoque maior de que 10 dias nos Incubatórios!"
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Gerar Relatório de Dias de Estoque - Incubatórios - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                return retorno;
            }
        }

        #endregion

        #region Relatório de Perdas

        public string SendReportLossWeeklyComercial()
        {
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Atualiza Relatório

                string relatorio = "Weekly_Loss_Report_Comercial";
                string destino = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque\\"
                            + relatorio + ".xlsx";
                string pesquisa = "*" + relatorio + "*";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque";
                bool deletaArquivoAntigo = true;

                string[] files = Directory.GetFiles(pasta, pesquisa);

                if (deletaArquivoAntigo)
                {
                    foreach (var item in files)
                    {
                        System.IO.File.Delete(item);
                    }
                }

                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\" + relatorio + ".xlsx", destino);

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

                #region Parâmetros

                DateTime dataInicial = DateTime.Today.AddDays(-7);
                DateTime dataFinal = DateTime.Today.AddDays(-1);

                string dataInicialStr = dataInicial.ToString("MM/dd/yyyy");
                string dataFinalStr = dataFinal.ToString("MM/dd/yyyy");

                //string linhagem = "";
                //if (empresa == "BR")
                //    linhagem = "('W-36','BRWN','W-80')";
                //else if (empresa == "LB" || empresa == "PL")
                //    linhagem = "('LSLC','LBWN')";
                //else if (empresa == "HN")
                //    linhagem = "('H&N','HNBR')";

                #endregion

                #region SQL DescarteHE

                string commandTextCHICCabecalhoDHE =
                    "select * ";

                string commandTextCHICTabelasDHE =
                    "from " +
                        "VU_Descarte_Ovos_Incubaveis ";

                string commandTextCHICCondicaoJoinsDHE = "where ";

                string commandTextCHICCondicaoFiltrosDHE = "";

                string commandTextCHICCondicaoParametrosDHE =
                        "[Data Descarte] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                        "Incubatorio <> 'PH' ";
                //"[Data Descarte] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                //"Linhagem in " + linhagem + " ";

                string commandTextCHICAgrupamentoDHE = "";

                string commandTextCHICOrdenacaoDHE =
                    "order by " +
                        "3, 2, 4, 5";

                #endregion

                #region SQL Destroyed

                string commandTextCHICCabecalhoD =
                    "select * ";

                string commandTextCHICTabelasD =
                    "from " +
                        "VU_Destroyed ";

                string commandTextCHICCondicaoJoinsD = "where ";

                string commandTextCHICCondicaoFiltrosD = "";

                string commandTextCHICCondicaoParametrosD =
                        "[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                        "Incubatorio <> 'PH' ";
                //"[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                //"Variety in " + linhagem + " ";

                string commandTextCHICAgrupamentoD = "";

                string commandTextCHICOrdenacaoD =
                    "order by " +
                        "3, 2, 4";

                #endregion

                #region SQL Doações

                string commandTextCHICCabecalhoB =
                    "select * ";

                string commandTextCHICTabelasB =
                    "from " +
                        "VU_Doacao_Pintainhas ";

                string commandTextCHICCondicaoJoinsB = "where ";

                string commandTextCHICCondicaoFiltrosB = "";

                string commandTextCHICCondicaoParametrosB =
                    //"Data between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    //"Empresa = '" + empresa + "' ";
                    "Data between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "Geracao = '4' ";

                string commandTextCHICAgrupamentoB = "";

                string commandTextCHICOrdenacaoB =
                    "order by " +
                        "3, 2";

                #endregion

                Connections lista = oBook.Connections;

                foreach (Excel.WorkbookConnection item in lista)
                {
                    item.OLEDBConnection.BackgroundQuery = false;
                    if (item.Name.Equals("DescarteHE"))
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalhoDHE + commandTextCHICTabelasDHE + commandTextCHICCondicaoJoinsDHE +
                            commandTextCHICCondicaoFiltrosDHE + commandTextCHICCondicaoParametrosDHE +
                            commandTextCHICAgrupamentoDHE +
                            commandTextCHICOrdenacaoDHE;
                    else if (item.Name.Equals("Destroyed"))
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalhoD + commandTextCHICTabelasD + commandTextCHICCondicaoJoinsD +
                            commandTextCHICCondicaoFiltrosD + commandTextCHICCondicaoParametrosD +
                            commandTextCHICAgrupamentoD +
                            commandTextCHICOrdenacaoD;
                    else if (item.Name.Equals("Doações"))
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalhoB + commandTextCHICTabelasB + commandTextCHICCondicaoJoinsB +
                            commandTextCHICCondicaoFiltrosB + commandTextCHICCondicaoParametrosB +
                            commandTextCHICAgrupamentoB +
                            commandTextCHICOrdenacaoB;
                }

                oBook.RefreshAll();

                System.Threading.Thread.Sleep(10000);

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

                #region Inicializar Variáveis E-mail

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "** RELATÓRIO SEMANAL DE PERDAS - COMERCIAL - "
                    + dataInicial.ToShortDateString() + " A "
                    + dataFinal.ToShortDateString() + " **";

                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Gerência";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailParaEmail = "tlourenco@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "dnogueira@hyline.com.br;bvieira@hyline.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório semanal de perdas!"
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Gerar Relatório Semanal de Perdas - Comercial - Erro Linha: "
                    + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                return retorno;
            }
        }

        public string SendReportLossWeeklyMatriz()
        {
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Atualiza Relatório

                string relatorio = "Weekly_Loss_Report_Matriz";
                string destino = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque\\"
                            + relatorio + ".xlsx";
                string pesquisa = "*" + relatorio + "*";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_FLIP\\DiasEstoque";
                bool deletaArquivoAntigo = true;

                string[] files = Directory.GetFiles(pasta, pesquisa);

                if (deletaArquivoAntigo)
                {
                    foreach (var item in files)
                    {
                        System.IO.File.Delete(item);
                    }
                }

                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\" + relatorio + ".xlsx", destino);

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

                #region Parâmetros

                DateTime dataInicial = DateTime.Today.AddDays(-7);
                DateTime dataFinal = DateTime.Today.AddDays(-1);

                string dataInicialStr = dataInicial.ToString("MM/dd/yyyy");
                string dataFinalStr = dataFinal.ToString("MM/dd/yyyy");

                //string linhagem = "";
                //if (empresa == "BR")
                //    linhagem = "('W-36','BRWN','W-80')";
                //else if (empresa == "LB" || empresa == "PL")
                //    linhagem = "('LSLC','LBWN')";
                //else if (empresa == "HN")
                //    linhagem = "('H&N','HNBR')";

                #endregion

                #region SQL DescarteHE

                string commandTextCHICCabecalhoDHE =
                    "select * ";

                string commandTextCHICTabelasDHE =
                    "from " +
                        "VU_Descarte_Ovos_Incubaveis ";

                string commandTextCHICCondicaoJoinsDHE = "where ";

                string commandTextCHICCondicaoFiltrosDHE = "";

                string commandTextCHICCondicaoParametrosDHE =
                        "[Data Descarte] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                        "Incubatorio = 'PH' ";
                //"[Data Descarte] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                //"Linhagem in " + linhagem + " ";

                string commandTextCHICAgrupamentoDHE = "";

                string commandTextCHICOrdenacaoDHE =
                    "order by " +
                        "3, 2, 4, 5";

                #endregion

                #region SQL Destroyed

                string commandTextCHICCabecalhoD =
                    "select * ";

                string commandTextCHICTabelasD =
                    "from " +
                        "VU_Destroyed ";

                string commandTextCHICCondicaoJoinsD = "where ";

                string commandTextCHICCondicaoFiltrosD = "";

                string commandTextCHICCondicaoParametrosD =
                        "[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                        "Incubatorio = 'PH' ";
                //"[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                //"Variety in " + linhagem + " ";

                string commandTextCHICAgrupamentoD = "";

                string commandTextCHICOrdenacaoD =
                    "order by " +
                        "3, 2, 4";

                #endregion

                #region SQL Doações

                string commandTextCHICCabecalhoB =
                    "select * ";

                string commandTextCHICTabelasB =
                    "from " +
                        "VU_Doacao_Pintainhas ";

                string commandTextCHICCondicaoJoinsB = "where ";

                string commandTextCHICCondicaoFiltrosB = "";

                string commandTextCHICCondicaoParametrosB =
                    //"Data between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    //"Empresa = '" + empresa + "' ";
                    "Data between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "Geracao <> '4' ";

                string commandTextCHICAgrupamentoB = "";

                string commandTextCHICOrdenacaoB =
                    "order by " +
                        "3, 2";

                #endregion

                Connections lista = oBook.Connections;

                foreach (Excel.WorkbookConnection item in lista)
                {
                    item.OLEDBConnection.BackgroundQuery = false;
                    if (item.Name.Equals("DescarteHE"))
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalhoDHE + commandTextCHICTabelasDHE + commandTextCHICCondicaoJoinsDHE +
                            commandTextCHICCondicaoFiltrosDHE + commandTextCHICCondicaoParametrosDHE +
                            commandTextCHICAgrupamentoDHE +
                            commandTextCHICOrdenacaoDHE;
                    else if (item.Name.Equals("Destroyed"))
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalhoD + commandTextCHICTabelasD + commandTextCHICCondicaoJoinsD +
                            commandTextCHICCondicaoFiltrosD + commandTextCHICCondicaoParametrosD +
                            commandTextCHICAgrupamentoD +
                            commandTextCHICOrdenacaoD;
                    else if (item.Name.Equals("Doações"))
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalhoB + commandTextCHICTabelasB + commandTextCHICCondicaoJoinsB +
                            commandTextCHICCondicaoFiltrosB + commandTextCHICCondicaoParametrosB +
                            commandTextCHICAgrupamentoB +
                            commandTextCHICOrdenacaoB;
                }

                oBook.RefreshAll();

                System.Threading.Thread.Sleep(10000);

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

                #region Inicializar Variáveis E-mail

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "** RELATÓRIO SEMANAL DE PERDAS - MATRIZ - "
                    + dataInicial.ToShortDateString() + " A "
                    + dataFinal.ToShortDateString() + " **";

                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Gerência";
                //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailParaEmail = "tlourenco@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "dnogueira@hyline.com.br;jpereira@hyline.com.br";
                email.WorkFlowEmailDeNome = "Sistema WEB";
                email.WorkFLowEmailDeEmail = "web@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório semanal de perdas!"
                    + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Gerar Relatório Semanal de Perdas - Matriz - Erro Linha: "
                    + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                return retorno;
            }
        }

        #endregion

        #region Programação Diária de Transportes - Semanal

        public string GeraProgramacaoDiariaTransportesPorPeriodo(string pesquisa, bool deletaArquivoAntigo, 
            string pasta, string destino, DateTime dataInicial, DateTime dataFinal, string vendedor, string empresa)
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
                "where " +
                    "EmpresaTranportador in ('TR', 'PL', 'HN') and ";

            string commandTextCHICCondicaoParametrosFaturamentoDados02 =
                "[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                "Empresa = '" + empresa + "' and "+
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
            
            //System.Threading.Thread.Sleep(3000);

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

        public string EnviarProgramacaoDiariaTransportesSemanal()
        {
            string erro = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Inicializar Variáveis

                string destino = "";
                string pasta = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\PDT";
                string empresa = "";

                DateTime dataInicial = DateTime.Today.AddDays(3);
                DateTime dataFinal = DateTime.Today.AddDays(10);
                //DateTime dataInicial = DateTime.Today.AddDays(-3);
                //DateTime dataFinal = DateTime.Today.AddDays(4);

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                string corpoEmail = "";

                #endregion

                #region Hy-Line

                empresa = "BR";

                #region Vendedores / Representantes

                CHICDataSet.salesmanDataTable vendedores = new CHICDataSet.salesmanDataTable();
                salesman.FillByEmpresa(vendedores, empresa);

                var listaVendedores = vendedores.ToList();

                foreach (var itemVendedor in listaVendedores)
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\PDT\\Programacao_Transportes_Periodo_" +
                        empresa + "_" + itemVendedor.sl_code.Trim() + ".xlsx";

                    destino = GeraProgramacaoDiariaTransportesPorPeriodo("*" + itemVendedor.sl_code.Trim() + "*", true,
                        pasta, destino, dataInicial, dataFinal, itemVendedor.sl_code.Trim(), empresa);

                    #region Envio de E-mail

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "PROGRAMAÇÃO DE TRANSPORTES - " + dataInicial.ToShortDateString()
                        + " a " + dataFinal.ToShortDateString();
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = itemVendedor.salesman.Trim();
                    email.WorkFlowEmailParaEmail = itemVendedor.email.Trim();
                    //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                    email.WorkFlowEmailCopiaPara = "programacao@hyline.com.br";
                    //email.WorkFlowEmailCopiaPara = "jcarchangelo@hyline.com.br;mchecco@hyline.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB HyLine";
                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + itemVendedor.salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório com a Programação de Transportes de "
                        + dataInicial.ToString("dd/MM/yyyy") + " a "
                        + dataFinal.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino;

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();

                    #endregion
                }

                #endregion

                #region Técnicos

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\PDT\\Programacao_Transportes_Periodo_Geral_" +
                    empresa + ".xlsx";

                destino = GeraProgramacaoDiariaTransportesPorPeriodo("*Programacao_Transportes_Periodo_Geral_" 
                    + empresa + "*", true, pasta, destino, dataInicial, dataFinal, "(Todos)", empresa);

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "PROGRAMAÇÃO DE TRANSPORTES - " + dataInicial.ToShortDateString()
                    + " a " + dataFinal.ToShortDateString();
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Assistência Técnica";
                email.WorkFlowEmailParaEmail = "a.tecnica@hyline.com.br";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB Hy-Line";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados Técnicos," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório com a Programação de Transportes de "
                        + dataInicial.ToString("dd/MM/yyyy") + " a "
                        + dataFinal.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion

                #region Lohmann

                empresa = "LB";

                #region Vendedores / Representantes

                vendedores = new CHICDataSet.salesmanDataTable();
                salesman.FillByEmpresa(vendedores, empresa);

                listaVendedores = vendedores.ToList();

                foreach (var itemVendedor in listaVendedores)
                {
                    destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\PDT\\Programacao_Transportes_Periodo_" +
                        empresa + "_" + itemVendedor.sl_code.Trim() + ".xlsx";

                    destino = GeraProgramacaoDiariaTransportesPorPeriodo("*" + itemVendedor.sl_code.Trim() + "*", true,
                        pasta, destino, dataInicial, dataFinal, itemVendedor.sl_code.Trim(), empresa);

                    #region Envio de E-mail

                    #region Verifica Se existe Supervisores para gerar a copia

                    string copiaPara = "";
                    string codigoVendedorApolo = "0" + itemVendedor.sl_code.Trim();
                    ApoloEntities2 apolo2 = new ApoloEntities2();
                    var listaSupVend = apolo2.SUP_VENDEDOR
                        .Where(w => w.VendCod == codigoVendedorApolo
                            && w.FxaCod.Equals("0000003"))
                        .ToList();

                    foreach (var sup in listaSupVend)
                    {
                        VENDEDOR supervisor = apolo.VENDEDOR
                            .Where(w => w.VendCod == sup.SupVendCod).FirstOrDefault();

                        if (supervisor != null)
                        {
                            copiaPara = copiaPara + supervisor.USERLoginSite + ";";
                        }
                    }

                    #endregion

                    email = new WORKFLOW_EMAIL();

                    numero = new ObjectParameter("codigo", typeof(global::System.String));

                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "PROGRAMAÇÃO DE TRANSPORTES - " + dataInicial.ToShortDateString()
                        + " a " + dataFinal.ToShortDateString();
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = itemVendedor.salesman.Trim();
                    email.WorkFlowEmailParaEmail = itemVendedor.email.Trim();
                    email.WorkFlowEmailCopiaPara = copiaPara + "confirmacoes@ltz.com.br";
                    email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                    email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                    email.WorkFlowEmailFormato = "Texto";

                    corpoEmail = "";

                    corpoEmail = "Prezado " + itemVendedor.salesman.Trim() + "," + (char)13 + (char)10 + (char)13 + (char)10
                        + "Segue anexo relatório com a Programação de Transportes de "
                        + dataInicial.ToString("dd/MM/yyyy") + " a "
                        + dataFinal.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                        + "SISTEMA WEB";

                    email.WorkFlowEmailCorpo = corpoEmail;
                    email.WorkFlowEmailArquivosAnexos = destino;

                    apolo.WORKFLOW_EMAIL.AddObject(email);

                    apolo.SaveChanges();

                    #endregion
                }

                #endregion

                #region Técnicos

                destino = "\\\\srv-riosoft-01\\w\\Relatorios_CHIC\\PDT\\Programacao_Transportes_Periodo_Geral_" +
                    empresa + ".xlsx";

                destino = GeraProgramacaoDiariaTransportesPorPeriodo("*Programacao_Transportes_Periodo_Geral_" + empresa + "*", true,
                        pasta, destino, dataInicial, dataFinal, "(Todos)", empresa);

                #region Envio de E-mail

                email = new WORKFLOW_EMAIL();

                numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "PROGRAMAÇÃO DE TRANSPORTES - " + dataInicial.ToShortDateString()
                    + " a " + dataFinal.ToShortDateString();
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Assistência Técnica";
                email.WorkFlowEmailParaEmail = "a.tecnica@ltz.com.br";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDeNome = "Sistema WEB Lohmann";
                email.WorkFLowEmailDeEmail = "sistemas@ltz.com.br";
                email.WorkFlowEmailFormato = "Texto";

                corpoEmail = "";

                corpoEmail = "Prezados Técnicos," + (char)13 + (char)10 + (char)13 + (char)10
                    + "Segue anexo relatório com a Programação de Transportes de "
                        + dataInicial.ToString("dd/MM/yyyy") + " a "
                        + dataFinal.ToString("dd/MM/yyyy") + "." + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA WEB";

                email.WorkFlowEmailCorpo = corpoEmail;
                email.WorkFlowEmailArquivosAnexos = destino;

                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erro = "Erro EnviarProgramacaoDiariaTransportesSemanal Linha Código: " + linenum.ToString()
                    + " / " + ex.Message;
                if (ex.InnerException != null)
                    erro = erro + " / " + ex.InnerException.Message;
            }

            return erro;
        }

        #endregion

        #endregion

        #region WEB X CHIC

        public string AtualizaWEBxCHIC()
        {
            string orderNoErro = "";
            string retorno = "";

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Atualiza Data, Qtdes e Preços do CHIC p/ WEB

                //DateTime dataFinalModeloAntigo = Convert.ToDateTime("02/12/2017");

                //var listWEB = hlbappService.LOG_Item_Pedido_Venda
                //    .Where(a => (a.Operacao.Contains("Importado") || a.Operacao.Contains("Reprovad"))
                //        //&& a.OrderNoCHIC == "68010"
                //        //&& a.IDPedidoVenda == 8891
                //        && hlbappService.Item_Pedido_Venda
                //            .Any(i => i.IDPedidoVenda == a.IDPedidoVenda
                //                 && i.DataEntregaFinal >= DateTime.Today
                //                 && i.DataEntregaFinal <= dataFinalModeloAntigo)
                //        && a.OrderNoCHIC != null && a.OrderNoCHIC != ""
                //        //&& a.DataHora >= DateTime.Today
                //        && hlbappService.Pedido_Venda.Any(p => p.ID == a.IDPedidoVenda
                //            && (p.Status.Contains("Importado") || p.Status.Contains("Reprovad")))
                //            //&& !p.Usuario.Contains("CHIC - "))
                //        && hlbappService.LOG_Item_Pedido_Venda
                //            .Where(a2 => a2.IDPedidoVenda == a.IDPedidoVenda
                //                && a2.Sequencia == a.Sequencia).Max(m => m.DataHora) == a.DataHora)
                //    .ToList();

                //foreach (var item in listWEB)
                //{
                //    Pedido_Venda pedVenda = hlbappService.Pedido_Venda.Where(w => w.ID == item.IDPedidoVenda)
                //        .FirstOrDefault();

                //    Item_Pedido_Venda itemPedidoVenda = hlbappService.Item_Pedido_Venda
                //        .Where(w => w.IDPedidoVenda == item.IDPedidoVenda)
                //        .FirstOrDefault();

                //    string orderNo = itemPedidoVenda.OrderNoCHIC;
                //    orderNoErro = itemPedidoVenda.OrderNoCHIC;

                //    if (itemPedidoVenda.OrderNoCHIC.Equals("64956"))
                //        orderNoErro = itemPedidoVenda.OrderNoCHIC;

                //    ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                //        new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                //    CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                //    bTACommercial.FillByOrderNo(bDTCommercial, orderNo);

                //    if (bDTCommercial.Count > 0)
                //    {
                //        DateTime dataModifyCHIC = bDTCommercial.Max(m => m.datemodi);

                //        DateTime dataHoraSemHora = Convert.ToDateTime(item.DataHora.ToShortDateString());

                //        DateTime dataHoraLOG = DateTime.Now;
                //        if (dataHoraSemHora == dataModifyCHIC)
                //        {
                //            dataHoraSemHora = dataHoraSemHora.AddDays(-1);
                //            dataHoraLOG = dataHoraLOG.AddDays(1);
                //        }

                //        //if (dataHoraSemHora < dataModifyCHIC)
                //        if (1 == 1)
                //        {
                //            List<Item_Pedido_Venda> listIpv = hlbappService.Item_Pedido_Venda
                //                .Where(w => w.IDPedidoVenda == item.IDPedidoVenda)
                //                .ToList();

                //            if (listIpv.Count > 0
                //                && pedVenda.Status.Contains("Importado") || pedVenda.Status.Contains("Reprovad"))
                //            {
                //                ordersTableAdapter oTACommercial = new ordersTableAdapter();
                //                CHICDataSet.ordersDataTable oDTCommercial = new CHICDataSet.ordersDataTable();
                //                oTACommercial.FillByOrderNo(oDTCommercial, orderNo);
                //                DateTime delDate = oDTCommercial[0].del_date;
                //                DateTime dataAntiga = listIpv.Max(m => m.DataEntregaInicial);
                //                DateTime dataAntigaFinal = listIpv.Max(m => m.DataEntregaFinal);

                //                vartablTableAdapter vartabl = new vartablTableAdapter();
                //                CHICDataSet.vartablDataTable vartablDT =
                //                    new CHICDataSet.vartablDataTable();

                //                string variety = "";

                //                #region Ajusta Qtds

                //                itemsTableAdapter iTA = new itemsTableAdapter();
                //                CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                //                iTA.Fill(iDT);

                //                decimal qtdOvosCHICVerifica = bDTCommercial
                //                        .Where(w => iDT.Any(a => a.item_no == w.item
                //                                && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))))
                //                        .Sum(s => s.quantity);

                //                int? qtdOvosWEBVerifica = listIpv.Sum(s => s.QtdeLiquida + s.QtdeBonificada);

                //                if (qtdOvosCHICVerifica != qtdOvosWEBVerifica)
                //                {
                //                    #region Insere LOG - Pedido_Venda

                //                    LOG_Pedido_Venda logPV = new LOG_Pedido_Venda();

                //                    logPV = new LOG_Pedido_Venda();
                //                    logPV.DataPedido = pedVenda.DataPedido;
                //                    logPV.Usuario = "Serviço";
                //                    logPV.DataHora = dataHoraLOG;
                //                    logPV.CodigoCliente = pedVenda.CodigoCliente;
                //                    logPV.OvosBrasil = pedVenda.OvosBrasil;
                //                    logPV.CondicaoPagamento = pedVenda.CondicaoPagamento;
                //                    logPV.Observacoes = pedVenda.Observacoes;
                //                    logPV.Vendedor = pedVenda.Vendedor;
                //                    logPV.Status = pedVenda.Status;
                //                    logPV.Operacao = "Importado p/ WEB";
                //                    logPV.IDPedidoVenda = pedVenda.ID;
                //                    logPV.Motivo = "Atualização das Qtdes. do CHIC p/ WEB";
                //                    logPV.Projecao = pedVenda.Projecao;

                //                    hlbappService.LOG_Pedido_Venda.AddObject(logPV);
                //                    hlbappService.SaveChanges();

                //                    #endregion

                //                    foreach (var ipv in listIpv)
                //                    {
                //                        vartabl.FillByDesc(vartablDT, ipv.ProdCodEstr.Replace(" - Ovos", ""));
                //                        variety = vartablDT[0].variety;

                //                        decimal qtdOvosVendidosCHIC = bDTCommercial
                //                            .Where(w => iDT.Any(a => a.item_no == w.item
                //                                    && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                //                                    && a.variety == variety)
                //                                && !w.alt_desc.Contains("Extra"))
                //                            .Sum(s => s.quantity);

                //                        decimal qtdOvosBonifCHIC = bDTCommercial
                //                            .Where(w => iDT.Any(a => a.item_no == w.item
                //                                    && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                //                                    && a.variety == variety)
                //                                && w.alt_desc.Contains("Extra"))
                //                            .Sum(s => s.quantity);

                //                        if (qtdOvosVendidosCHIC != ipv.QtdeLiquida)
                //                        {
                //                            ipv.QtdeLiquida = Convert.ToInt32(qtdOvosVendidosCHIC);
                //                        }

                //                        if (qtdOvosBonifCHIC != ipv.QtdeBonificada)
                //                        {
                //                            ipv.QtdeBonificada = Convert.ToInt32(qtdOvosBonifCHIC);
                //                            if (ipv.QtdeLiquida > 0)
                //                            {
                //                                ipv.PercBonificacao =
                //                                    ((ipv.QtdeBonificada * 1.00m) / (ipv.QtdeLiquida * 1.00m)) * 100.00m;
                //                            }
                //                        }

                //                        #region Insere LOG - Item_Ped_Venda

                //                        LOG_Item_Pedido_Venda logItemPV = new LOG_Item_Pedido_Venda();
                //                        logItemPV.IDPedidoVenda = ipv.IDPedidoVenda;
                //                        logItemPV.Sequencia = ipv.Sequencia;
                //                        logItemPV.ProdCodEstr = ipv.ProdCodEstr;
                //                        logItemPV.DataEntregaInicial = ipv.DataEntregaInicial;
                //                        logItemPV.DataEntregaFinal = ipv.DataEntregaFinal;
                //                        logItemPV.QtdeLiquida = ipv.QtdeLiquida;
                //                        logItemPV.PercBonificacao = ipv.PercBonificacao;
                //                        logItemPV.QtdeBonificada = ipv.QtdeBonificada;
                //                        logItemPV.QtdeReposicao = ipv.QtdeReposicao;
                //                        logItemPV.PrecoUnitario = ipv.PrecoUnitario;
                //                        logItemPV.DataHora = dataHoraLOG;
                //                        logItemPV.Operacao = "Importado p/ WEB";
                //                        logItemPV.IDItPedVenda = ipv.ID;
                //                        logItemPV.IDLogPedidoVenda = logPV.ID;
                //                        logItemPV.OrderNoCHIC = ipv.OrderNoCHIC;
                //                        logItemPV.OrderNoCHICReposicao = ipv.OrderNoCHICReposicao;
                //                        logItemPV.PrecoPinto = ipv.PrecoPinto;
                //                        logItemPV.TipoReposicao = ipv.TipoReposicao;
                //                        logItemPV.ValorTotal = ipv.ValorTotal;

                //                        hlbappService.LOG_Item_Pedido_Venda.AddObject(logItemPV);

                //                        #endregion
                //                    }

                //                    #region Insere LOG - Vacina Primária

                //                    Vacinas_Primaria_Pedido_Venda vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                //                            .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                //                    LOG_Vacinas_Primaria_Pedido_Venda logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                //                    if (vacPrimObj != null)
                //                    {
                //                        logVacPrim.IDPedidoVenda = vacPrimObj.IDPedidoVenda;
                //                        logVacPrim.ProdCodEstr = vacPrimObj.ProdCodEstr;
                //                        logVacPrim.DataHora = dataHoraLOG;
                //                        logVacPrim.Operacao = "Importado p/ WEB";
                //                        logVacPrim.IDVacPrimPedVenda = vacPrimObj.ID;
                //                        logVacPrim.IDLogPedidoVenda = logPV.ID;
                //                        logVacPrim.PrecoUnitario = vacPrimObj.PrecoUnitario;
                //                        logVacPrim.Bonificada = vacPrimObj.Bonificada;

                //                        hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                //                        hlbappService.SaveChanges();

                //                        var listaVacSec = hlbappService.Vacinas_Secundaria_Pedido_Venda
                //                            .Where(w => w.IDVacPrimPedVenda == vacPrimObj.ID).ToList();

                //                        foreach (var vacSec in listaVacSec)
                //                        {
                //                            #region Insere LOG - Vacina_Secundaria_Pedido_Venda

                //                            LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                //                            logVacSec.IDVacPrimPedVenda = vacSec.IDVacPrimPedVenda;
                //                            logVacSec.Sequencia = vacSec.Sequencia;
                //                            logVacSec.ProdCodEstr = vacSec.ProdCodEstr;
                //                            logVacSec.DataHora = DateTime.Now;
                //                            logVacSec.Operacao = "Importado p/ WEB";
                //                            logVacSec.IDVacSecPedVenda = vacSec.ID;
                //                            logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;

                //                            hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);

                //                            #endregion
                //                        }
                //                    }

                //                    #endregion

                //                    #region Insere LOG - Servico_Pedido_Venda

                //                    Servicos_Pedido_Venda serv = hlbappService.Servicos_Pedido_Venda
                //                                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                //                    if (serv != null)
                //                    {
                //                        LOG_Servicos_Pedido_Venda logServ = new LOG_Servicos_Pedido_Venda();
                //                        logServ.IDPedidoVenda = serv.IDPedidoVenda;
                //                        logServ.ProdCodEstr = serv.ProdCodEstr;
                //                        logServ.PercAplicacaoServico = serv.PercAplicacaoServico;
                //                        logServ.DataHora = dataHoraLOG;
                //                        logServ.Operacao = "Importado p/ WEB";
                //                        logServ.IDServPedVenda = serv.ID;
                //                        logServ.IDLogPedidoVenda = logPV.ID;
                //                        logServ.PrecoUnitario = serv.PrecoUnitario;
                //                        logServ.Bonificada = serv.Bonificada;

                //                        hlbappService.LOG_Servicos_Pedido_Venda.AddObject(logServ);
                //                    }

                //                    #endregion

                //                    #region Envia E-mail p/ Representante / Vendedor Avisando (Somente Nutribastos para teste)

                //                    if (pedVenda.Vendedor.Equals("000083"))
                //                    {
                //                        string caminho = "";

                //                        CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                //                        salesman.FillByCod(sDT, pedVenda.Vendedor);
                //                        string nome = sDT[0].salesman.Trim();
                //                        string enderecoEmail = sDT[0].email.Trim();
                //                        string empresaApolo = "";
                //                        if (sDT[0].inv_comp.Trim().Equals("BR")) empresaApolo = "5";
                //                        else if (sDT[0].inv_comp.Trim().Equals("LB")) empresaApolo = "7";
                //                        else if (sDT[0].inv_comp.Trim().Equals("HN")) empresaApolo = "14";
                //                        else if (sDT[0].inv_comp.Trim().Equals("PL")) empresaApolo = "20";

                //                        string corpoEmail = "Prezado(s)," + (char)13 + (char)10
                //                            + (char)13 + (char)10
                //                            + "Foi realizada " + logPV.Motivo + " do pedido CHIC Nº" + orderNoErro
                //                            + " - ID WEB " + logPV.IDPedidoVenda.ToString() + "."
                //                            + (char)13 + (char)10
                //                            + "Qualquer dúvida, entrar em contato o Gerente Comercial para confirmar "
                //                            + " a alteração." + (char)13 + (char)10 + (char)13 + (char)10
                //                            + "SISTEMA WEB";

                //                        string assunto = "**** " + logPV.Motivo.ToUpper() + " - CHIC " + orderNoErro
                //                            + " / ID " + logPV.IDPedidoVenda.ToString() + " ****";

                //                        EnviaConfirmacaoEmail(caminho, enderecoEmail, nome, "", "", "", corpoEmail,
                //                            assunto, empresaApolo);
                //                    }

                //                    #endregion
                //                }

                //                #endregion

                //                #region Ajusta Datas

                //                if ((delDate != dataAntiga)
                //                    || (delDate != dataAntigaFinal))
                //                {
                //                    #region Insere LOG - Pedido_Venda

                //                    //Pedido_Venda pedVenda = hlbappService.Pedido_Venda.Where(w => w.ID == item.IDPedidoVenda).FirstOrDefault();
                //                    LOG_Pedido_Venda logPV = new LOG_Pedido_Venda();

                //                    logPV = new LOG_Pedido_Venda();
                //                    logPV.DataPedido = pedVenda.DataPedido;
                //                    logPV.Usuario = "Serviço";
                //                    logPV.DataHora = dataHoraLOG;
                //                    logPV.CodigoCliente = pedVenda.CodigoCliente;
                //                    logPV.OvosBrasil = pedVenda.OvosBrasil;
                //                    logPV.CondicaoPagamento = pedVenda.CondicaoPagamento;
                //                    logPV.Observacoes = pedVenda.Observacoes;
                //                    logPV.Vendedor = pedVenda.Vendedor;
                //                    logPV.Status = pedVenda.Status;
                //                    logPV.Operacao = "Importado p/ WEB";
                //                    logPV.IDPedidoVenda = pedVenda.ID;
                //                    logPV.Motivo = "Atualização de Data de Entrega do CHIC p/ WEB de "
                //                        + dataAntiga.ToShortDateString() + " para "
                //                        + delDate.ToShortDateString();
                //                    logPV.Projecao = pedVenda.Projecao;

                //                    hlbappService.LOG_Pedido_Venda.AddObject(logPV);
                //                    hlbappService.SaveChanges();

                //                    #endregion

                //                    foreach (var ipv in listIpv)
                //                    {
                //                        vartabl.FillByDesc(vartablDT, ipv.ProdCodEstr.Replace(" - Ovos", ""));
                //                        variety = vartablDT[0].variety;

                //                        var bRow2 = bDTCommercial
                //                            .Where(w => iDT.Any(a => a.item_no == w.item
                //                                    && (a.form.Substring(0, 1).Equals("H")
                //                                    || a.form.Substring(0, 1).Equals("D"))
                //                                    && a.variety.Trim() == variety))
                //                            .FirstOrDefault();

                //                        ipv.DataEntregaInicial = delDate;
                //                        ipv.DataEntregaFinal = delDate;
                //                        if (bRow2 != null)
                //                            ipv.DataNascimento = bRow2.cal_date.AddDays(21);

                //                        #region Insere LOG - Item_Ped_Venda

                //                        LOG_Item_Pedido_Venda logItemPV = new LOG_Item_Pedido_Venda();
                //                        logItemPV.IDPedidoVenda = ipv.IDPedidoVenda;
                //                        logItemPV.Sequencia = ipv.Sequencia;
                //                        logItemPV.ProdCodEstr = ipv.ProdCodEstr;
                //                        logItemPV.DataEntregaInicial = ipv.DataEntregaInicial;
                //                        logItemPV.DataEntregaFinal = ipv.DataEntregaFinal;
                //                        logItemPV.QtdeLiquida = ipv.QtdeLiquida;
                //                        logItemPV.PercBonificacao = ipv.PercBonificacao;
                //                        logItemPV.QtdeBonificada = ipv.QtdeBonificada;
                //                        logItemPV.QtdeReposicao = ipv.QtdeReposicao;
                //                        logItemPV.PrecoUnitario = ipv.PrecoUnitario;
                //                        logItemPV.DataHora = dataHoraLOG;
                //                        logItemPV.Operacao = "Importado p/ WEB";
                //                        logItemPV.IDItPedVenda = ipv.ID;
                //                        logItemPV.IDLogPedidoVenda = logPV.ID;
                //                        logItemPV.OrderNoCHIC = ipv.OrderNoCHIC;
                //                        logItemPV.OrderNoCHICReposicao = ipv.OrderNoCHICReposicao;
                //                        logItemPV.PrecoPinto = ipv.PrecoPinto;
                //                        logItemPV.TipoReposicao = ipv.TipoReposicao;
                //                        logItemPV.ValorTotal = ipv.ValorTotal;

                //                        hlbappService.LOG_Item_Pedido_Venda.AddObject(logItemPV);

                //                        #endregion
                //                    }

                //                    #region Insere LOG - Vacina Primária

                //                    Vacinas_Primaria_Pedido_Venda vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                //                            .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                //                    LOG_Vacinas_Primaria_Pedido_Venda logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                //                    if (vacPrimObj != null)
                //                    {
                //                        logVacPrim.IDPedidoVenda = vacPrimObj.IDPedidoVenda;
                //                        logVacPrim.ProdCodEstr = vacPrimObj.ProdCodEstr;
                //                        logVacPrim.DataHora = dataHoraLOG;
                //                        logVacPrim.Operacao = "Importado p/ WEB";
                //                        logVacPrim.IDVacPrimPedVenda = vacPrimObj.ID;
                //                        logVacPrim.IDLogPedidoVenda = logPV.ID;
                //                        logVacPrim.PrecoUnitario = vacPrimObj.PrecoUnitario;
                //                        logVacPrim.Bonificada = vacPrimObj.Bonificada;

                //                        hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                //                        hlbappService.SaveChanges();

                //                        var listaVacSec = hlbappService.Vacinas_Secundaria_Pedido_Venda
                //                            .Where(w => w.IDVacPrimPedVenda == vacPrimObj.ID).ToList();

                //                        foreach (var vacSec in listaVacSec)
                //                        {
                //                            #region Insere LOG - Vacina_Secundaria_Pedido_Venda

                //                            LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                //                            logVacSec.IDVacPrimPedVenda = vacSec.IDVacPrimPedVenda;
                //                            logVacSec.Sequencia = vacSec.Sequencia;
                //                            logVacSec.ProdCodEstr = vacSec.ProdCodEstr;
                //                            logVacSec.DataHora = DateTime.Now;
                //                            logVacSec.Operacao = "Importado p/ WEB";
                //                            logVacSec.IDVacSecPedVenda = vacSec.ID;
                //                            logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;
                //                            logVacSec.PrecoUnitario = vacSec.PrecoUnitario;
                //                            logVacSec.Bonificada = vacSec.Bonificada;

                //                            hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);

                //                            #endregion
                //                        }
                //                    }

                //                    #endregion

                //                    #region Insere LOG - Servico_Pedido_Venda

                //                    Servicos_Pedido_Venda serv = hlbappService.Servicos_Pedido_Venda
                //                                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                //                    if (serv != null)
                //                    {
                //                        LOG_Servicos_Pedido_Venda logServ = new LOG_Servicos_Pedido_Venda();
                //                        logServ.IDPedidoVenda = serv.IDPedidoVenda;
                //                        logServ.ProdCodEstr = serv.ProdCodEstr;
                //                        logServ.PercAplicacaoServico = serv.PercAplicacaoServico;
                //                        logServ.DataHora = dataHoraLOG;
                //                        logServ.Operacao = "Importado p/ WEB";
                //                        logServ.IDServPedVenda = serv.ID;
                //                        logServ.IDLogPedidoVenda = logPV.ID;
                //                        logServ.PrecoUnitario = serv.PrecoUnitario;
                //                        logServ.Bonificada = serv.Bonificada;

                //                        hlbappService.LOG_Servicos_Pedido_Venda.AddObject(logServ);
                //                    }

                //                    #endregion

                //                    #region Envia E-mail p/ Representante / Vendedor Avisando (Somente Nutribastos para teste)

                //                    //if (oDTCommercial[0].salesrep.Equals("000083"))
                //                    if (pedVenda.Vendedor.Equals("000083"))
                //                    {
                //                        string caminho = "";

                //                        CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                //                        salesman.FillByCod(sDT, pedVenda.Vendedor);
                //                        string nome = sDT[0].salesman.Trim();
                //                        string enderecoEmail = sDT[0].email.Trim();
                //                        string empresaApolo = "";
                //                        if (sDT[0].inv_comp.Trim().Equals("BR")) empresaApolo = "5";
                //                        else if (sDT[0].inv_comp.Trim().Equals("LB")) empresaApolo = "7";
                //                        else if (sDT[0].inv_comp.Trim().Equals("HN")) empresaApolo = "14";
                //                        else if (sDT[0].inv_comp.Trim().Equals("PL")) empresaApolo = "20";

                //                        string corpoEmail = "Prezado(s)," + (char)13 + (char)10
                //                            + (char)13 + (char)10
                //                            + "Foi realizada " + logPV.Motivo + " do pedido CHIC Nº" + orderNoErro
                //                            + " - ID WEB " + logPV.IDPedidoVenda.ToString() + "."
                //                            + (char)13 + (char)10
                //                            + "Qualquer dúvida, entrar em contato o Gerente Comercial para confirmar "
                //                            + " a alteração." + (char)13 + (char)10 + (char)13 + (char)10
                //                            + "SISTEMA WEB";

                //                        string assunto = "**** " + logPV.Motivo.ToUpper() + " - CHIC " + orderNoErro
                //                            + " / ID " + logPV.IDPedidoVenda.ToString() + " ****";

                //                        EnviaConfirmacaoEmail(caminho, enderecoEmail, nome, "", "", "", corpoEmail,
                //                            assunto, empresaApolo);
                //                    }

                //                    #endregion
                //                }

                //                #endregion

                //                #region Ajusta Precos

                //                vartabl.FillByDesc(vartablDT, item.ProdCodEstr.Replace(" - Ovos", ""));
                //                variety = vartablDT[0].variety;

                //                decimal precoCHICVerifica = 0;
                //                var bRow = bDTCommercial
                //                    .Where(w => iDT.Any(a => a.item_no == w.item
                //                            && (a.form.Substring(0, 1).Equals("H")
                //                            || a.form.Substring(0, 1).Equals("D"))
                //                            && a.variety.Trim() == variety))
                //                    .FirstOrDefault();
                //                if (bRow != null)
                //                    precoCHICVerifica = bRow.price;

                //                decimal? precoWEBVerifica = listIpv
                //                    .Where(w => w.ProdCodEstr == item.ProdCodEstr)
                //                    .Sum(s => s.PrecoUnitario);

                //                if (listIpv.Where(w => w.PrecoPinto == null).Count() == 0)
                //                    precoWEBVerifica = listIpv
                //                        .Where(w => w.ProdCodEstr == item.ProdCodEstr)
                //                        .Sum(s => s.PrecoPinto);

                //                if (precoCHICVerifica != precoWEBVerifica)
                //                {
                //                    #region Insere LOG - Pedido_Venda

                //                    //Pedido_Venda pedVenda = hlbappService.Pedido_Venda.Where(w => w.ID == item.IDPedidoVenda).FirstOrDefault();
                //                    LOG_Pedido_Venda logPV = new LOG_Pedido_Venda();

                //                    logPV = new LOG_Pedido_Venda();
                //                    logPV.DataPedido = pedVenda.DataPedido;
                //                    logPV.Usuario = "Serviço";
                //                    logPV.DataHora = dataHoraLOG;
                //                    logPV.CodigoCliente = pedVenda.CodigoCliente;
                //                    logPV.OvosBrasil = pedVenda.OvosBrasil;
                //                    logPV.CondicaoPagamento = pedVenda.CondicaoPagamento;
                //                    logPV.Observacoes = pedVenda.Observacoes;
                //                    logPV.Vendedor = pedVenda.Vendedor;
                //                    logPV.Status = pedVenda.Status;
                //                    logPV.Operacao = "Importado p/ WEB";
                //                    logPV.IDPedidoVenda = pedVenda.ID;
                //                    logPV.Motivo = "Atualização dos Preços. do CHIC p/ WEB";
                //                    logPV.Projecao = pedVenda.Projecao;

                //                    hlbappService.LOG_Pedido_Venda.AddObject(logPV);
                //                    hlbappService.SaveChanges();

                //                    #endregion

                //                    foreach (var ipv in listIpv)
                //                    {
                //                        //ipv.PrecoUnitario = precoCHICVerifica;
                //                        ipv.PrecoPinto = precoCHICVerifica;
                //                        decimal precoCHICTotal = 0;
                //                        if (bDTCommercial.Where(w => w.price > 0).Count() > 0)
                //                        {
                //                            precoCHICTotal = bDTCommercial.Where(w => w.price > 0)
                //                                .Sum(s => s.price);
                //                        }
                //                        ipv.PrecoUnitario = precoCHICTotal;

                //                        #region Insere LOG - Item_Ped_Venda

                //                        LOG_Item_Pedido_Venda logItemPV = new LOG_Item_Pedido_Venda();
                //                        logItemPV.IDPedidoVenda = ipv.IDPedidoVenda;
                //                        logItemPV.Sequencia = ipv.Sequencia;
                //                        logItemPV.ProdCodEstr = ipv.ProdCodEstr;
                //                        logItemPV.DataEntregaInicial = ipv.DataEntregaInicial;
                //                        logItemPV.DataEntregaFinal = ipv.DataEntregaFinal;
                //                        logItemPV.QtdeLiquida = ipv.QtdeLiquida;
                //                        logItemPV.PercBonificacao = ipv.PercBonificacao;
                //                        logItemPV.QtdeBonificada = ipv.QtdeBonificada;
                //                        logItemPV.QtdeReposicao = ipv.QtdeReposicao;
                //                        logItemPV.PrecoUnitario = ipv.PrecoUnitario;
                //                        logItemPV.DataHora = dataHoraLOG;
                //                        logItemPV.Operacao = "Importado p/ WEB";
                //                        logItemPV.IDItPedVenda = ipv.ID;
                //                        logItemPV.IDLogPedidoVenda = logPV.ID;
                //                        logItemPV.OrderNoCHIC = ipv.OrderNoCHIC;
                //                        logItemPV.OrderNoCHICReposicao = ipv.OrderNoCHICReposicao;
                //                        logItemPV.PrecoPinto = ipv.PrecoPinto;
                //                        logItemPV.TipoReposicao = ipv.TipoReposicao;
                //                        logItemPV.ValorTotal = ipv.ValorTotal;

                //                        hlbappService.LOG_Item_Pedido_Venda.AddObject(logItemPV);

                //                        #endregion

                //                    }

                //                    #region Insere LOG - Vacina Primária

                //                    Vacinas_Primaria_Pedido_Venda vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                //                            .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                //                    LOG_Vacinas_Primaria_Pedido_Venda logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                //                    if (vacPrimObj != null)
                //                    {
                //                        logVacPrim.IDPedidoVenda = vacPrimObj.IDPedidoVenda;
                //                        logVacPrim.ProdCodEstr = vacPrimObj.ProdCodEstr;
                //                        logVacPrim.DataHora = dataHoraLOG;
                //                        logVacPrim.Operacao = "Importado p/ WEB";
                //                        logVacPrim.IDVacPrimPedVenda = vacPrimObj.ID;
                //                        logVacPrim.IDLogPedidoVenda = logPV.ID;
                //                        logVacPrim.Bonificada = vacPrimObj.Bonificada;
                //                        logVacPrim.PrecoUnitario = vacPrimObj.PrecoUnitario;

                //                        hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                //                        hlbappService.SaveChanges();

                //                        var listaVacSec = hlbappService.Vacinas_Secundaria_Pedido_Venda
                //                            .Where(w => w.IDVacPrimPedVenda == vacPrimObj.ID).ToList();

                //                        foreach (var vacSec in listaVacSec)
                //                        {
                //                            #region Insere LOG - Vacina_Secundaria_Pedido_Venda

                //                            LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                //                            logVacSec.IDVacPrimPedVenda = vacSec.IDVacPrimPedVenda;
                //                            logVacSec.Sequencia = vacSec.Sequencia;
                //                            logVacSec.ProdCodEstr = vacSec.ProdCodEstr;
                //                            logVacSec.DataHora = DateTime.Now;
                //                            logVacSec.Operacao = "Importado p/ WEB";
                //                            logVacSec.IDVacSecPedVenda = vacSec.ID;
                //                            logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;
                //                            logVacSec.Bonificada = vacSec.Bonificada;
                //                            logVacSec.PrecoUnitario = vacSec.PrecoUnitario;

                //                            hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);

                //                            #endregion
                //                        }
                //                    }

                //                    #endregion

                //                    #region Insere LOG - Servico_Pedido_Venda

                //                    Servicos_Pedido_Venda serv = hlbappService.Servicos_Pedido_Venda
                //                                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                //                    if (serv != null)
                //                    {
                //                        LOG_Servicos_Pedido_Venda logServ = new LOG_Servicos_Pedido_Venda();
                //                        logServ.IDPedidoVenda = serv.IDPedidoVenda;
                //                        logServ.ProdCodEstr = serv.ProdCodEstr;
                //                        logServ.PercAplicacaoServico = serv.PercAplicacaoServico;
                //                        logServ.DataHora = dataHoraLOG;
                //                        logServ.Operacao = "Importado p/ WEB";
                //                        logServ.IDServPedVenda = serv.ID;
                //                        logServ.IDLogPedidoVenda = logPV.ID;
                //                        logServ.Bonificada = serv.Bonificada;
                //                        logServ.PrecoUnitario = serv.PrecoUnitario;

                //                        hlbappService.LOG_Servicos_Pedido_Venda.AddObject(logServ);
                //                    }

                //                    #endregion

                //                    #region Envia E-mail p/ Representante / Vendedor Avisando (Somente Nutribastos para teste)

                //                    if (pedVenda.Vendedor.Equals("000083"))
                //                    {
                //                        string caminho = "";

                //                        CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                //                        salesman.FillByCod(sDT, pedVenda.Vendedor);
                //                        string nome = sDT[0].salesman.Trim();
                //                        string enderecoEmail = sDT[0].email.Trim();
                //                        string empresaApolo = "";
                //                        if (sDT[0].inv_comp.Trim().Equals("BR")) empresaApolo = "5";
                //                        else if (sDT[0].inv_comp.Trim().Equals("LB")) empresaApolo = "7";
                //                        else if (sDT[0].inv_comp.Trim().Equals("HN")) empresaApolo = "14";
                //                        else if (sDT[0].inv_comp.Trim().Equals("PL")) empresaApolo = "20";

                //                        string corpoEmail = "Prezado(s)," + (char)13 + (char)10
                //                            + (char)13 + (char)10
                //                            + "Foi realizada " + logPV.Motivo + " do pedido CHIC Nº" + orderNoErro
                //                            + " - ID WEB " + logPV.IDPedidoVenda.ToString() + "."
                //                            + (char)13 + (char)10
                //                            + "Qualquer dúvida, entrar em contato o Gerente Comercial para confirmar "
                //                            + " a alteração." + (char)13 + (char)10 + (char)13 + (char)10
                //                            + "SISTEMA WEB";

                //                        string assunto = "**** " + logPV.Motivo.ToUpper() + " - CHIC " + orderNoErro
                //                            + " / ID " + logPV.IDPedidoVenda.ToString() + " ****";

                //                        EnviaConfirmacaoEmail(caminho, enderecoEmail, nome, "", "", "", corpoEmail,
                //                            assunto, empresaApolo);
                //                    }

                //                    #endregion
                //                }

                //                #endregion

                //                #region Status

                //                Pedido_Venda pedidoVenda = hlbappService.Pedido_Venda
                //                    .Where(w => w.ID == item.IDPedidoVenda).FirstOrDefault();

                //                if (pedidoVenda.Status.Contains("Reprovad"))
                //                {
                //                    var listaItens = hlbappService.Item_Pedido_Venda
                //                        .Where(w => w.IDPedidoVenda == pedidoVenda.ID).ToList();

                //                    foreach (var itemPV in listaItens)
                //                    {
                //                        itemPV.Importar = 0;
                //                    }

                //                    int existeCancelado = listaItens
                //                        .Where(w => w.IDPedidoVenda == pedidoVenda.ID
                //                        && w.OrderNoCHIC == "Cancelado").Count();

                //                    int existeNaoCancelado = listaItens
                //                        .Where(w => w.IDPedidoVenda == pedidoVenda.ID
                //                        && w.OrderNoCHIC != "Cancelado").Count();

                //                    string status = "Importado Total";
                //                    if (existeCancelado > 0 && existeNaoCancelado > 0)
                //                        status = "Importado Parcial";

                //                    //pedidoVenda.Status = status;
                //                    string motivo = "Status alterado para " + status + " por motivo de reprovação.";
                                    
                //                    InsereLOGPVWeb(pedidoVenda.ID, "Serviço", "Alteração", motivo);
                //                }

                //                #endregion
                //            }
                //        }
                //    }
                //}

                //hlbappService.SaveChanges();

                #endregion

                #region Insere Pedido do CHIC que não tem no WEB

                ordersTableAdapter oTA = new ordersTableAdapter();
                CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();
                DateTime data = DateTime.Today;
                //DateTime data = Convert.ToDateTime("01/09/2016");
                //oTA.FillSalesByHathDate(oDT, data);
                oTA.FillModeloNovoMaior08012018(oDT);
                //oTA.FillByOrderNo(oDT, "85051");

                //var listaOrders = oDT.Where(w => !w.delivery.Trim().ToUpper().Contains("DOA")
                var listaOrders = oDT.Where(w => w.salesrep.Trim() != ""
                    ).ToList();

                foreach (var item in listaOrders)
                {
                    string orderNo = item.orderno.Trim();
                    orderNoErro = orderNo;

                    int existe = hlbappService.Item_Pedido_Venda
                        .Where(w => w.OrderNoCHIC == orderNo || w.OrderNoCHICReposicao == orderNo)
                        .Count();

                    if (existe == 0)
                    {
                        ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                            new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                        CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                        bTACommercial.FillByOrderNo(bDTCommercial, orderNo);

                        if (bDTCommercial.Count > 0)
                        {
                            #region Carrega tabela Custom Pedido

                            int_commTableAdapter icTA = new int_commTableAdapter();
                            CHICDataSet.int_commDataTable icDT = new CHICDataSet.int_commDataTable();
                            icTA.FillByOrderNo(icDT, orderNo);

                            itemsTableAdapter iTA = new itemsTableAdapter();
                            CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                            iTA.Fill(iDT);

                            #endregion

                            #region Caso Exista pedido Mãe para Doação, relacionar o mesmo no pedido mãe

                            int existePedidoMae = 0;
                            if (item.delivery.Trim().ToUpper().Contains("DOA"))
                            {
                                if (icDT.Count > 0)
                                {
                                    string numPedidoMae = icDT[0].npedrepo.ToString();
                                    existePedidoMae = hlbappService.Item_Pedido_Venda
                                        .Where(w => w.OrderNoCHIC == numPedidoMae).Count();

                                    if (existePedidoMae > 0)
                                    {
                                        var listaItens = bDTCommercial
                                            .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H") 
                                                    || a.form.Substring(0, 1).Equals("D")))
                                                && !w.alt_desc.Contains("Extra"))
                                            .ToList();

                                        foreach (var booked in listaItens)
                                        {
                                            string variety = iDT.Where(w => w.item_no == booked.item)
                                                .FirstOrDefault().variety;

                                            vartablTableAdapter vartbTA = new vartablTableAdapter();
                                            CHICDataSet.vartablDataTable varDT =
                                                new CHICDataSet.vartablDataTable();

                                            vartbTA.FillByVariety(varDT, variety);
                                            string linhagemWEB = varDT[0].desc.Trim();

                                            Item_Pedido_Venda itemPedidoMae = hlbappService.Item_Pedido_Venda
                                                .Where(w => w.OrderNoCHIC == numPedidoMae
                                                    && w.ProdCodEstr == linhagemWEB)
                                                .FirstOrDefault();

                                            if (itemPedidoMae != null)
                                            {
                                                itemPedidoMae.OrderNoCHICReposicao = orderNo;
                                                itemPedidoMae.QtdeReposicao = Convert.ToInt32(booked.quantity);
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            if (existePedidoMae == 0)
                            {
                                #region Insere Pedido

                                string usuarioCHIC = "";
                                if (bDTCommercial.Count > 0)
                                    usuarioCHIC = bDTCommercial.OrderByDescending(o => o.datecrtd)
                                        .FirstOrDefault().creatdby.Trim();

                                Pedido_Venda pedidoVenda = new Pedido_Venda();
                                pedidoVenda.CodigoCliente = item.cust_no.Trim();
                                pedidoVenda.DataPedido = item.order_date;
                                pedidoVenda.DataHora = DateTime.Now;
                                pedidoVenda.Usuario = "CHIC - " + usuarioCHIC;
                                if (!item.com1.Trim().Equals(""))
                                    pedidoVenda.Observacoes = item.com1.Trim();
                                if (!item.com2.Trim().Equals(""))
                                    pedidoVenda.Observacoes = pedidoVenda.Observacoes + (char)10 + item.com2.Trim();
                                if (!item.com3.Trim().Equals(""))
                                    pedidoVenda.Observacoes = pedidoVenda.Observacoes + (char)10 + item.com3.Trim();

                                #region Ovos Brasil

                                if (icDT.Count > 0)
                                {
                                    if (icDT[0].invmess1)
                                        pedidoVenda.OvosBrasil = 1;
                                    else
                                        pedidoVenda.OvosBrasil = 0;

                                    pedidoVenda.Observacoes = pedidoVenda.Observacoes + (char)10 + (char)13
                                        + icDT[0].comments.Trim();
                                }
                                else
                                {
                                    pedidoVenda.OvosBrasil = 0;
                                }

                                #endregion

                                #region Carrega Pedido de Reposição Caso Exista

                                CHICDataSet.int_commDataTable icDTReposicao = new CHICDataSet.int_commDataTable();
                                icTA.FillByNpedrepo(icDTReposicao, Convert.ToDecimal(orderNo));

                                string orderNoCHICReposicao = null;
                                if (icDTReposicao.Count > 0)
                                {
                                    orderNoCHICReposicao = icDTReposicao[0].orderno;
                                }

                                #endregion

                                pedidoVenda.CondicaoPagamento = item.delivery.Trim();
                                pedidoVenda.Vendedor = item.salesrep.Trim();
                                pedidoVenda.Status = "Importado Total";
                                pedidoVenda.EnderEntSeq = Convert.ToInt32(item.contact_no);

                                ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == pedidoVenda.CodigoCliente)
                                    .FirstOrDefault();

                                pedidoVenda.NomeCliente = entidade.EntNome;

                                salesmanTableAdapter slTA = new salesmanTableAdapter();
                                CHICDataSet.salesmanDataTable slDT = new CHICDataSet.salesmanDataTable();
                                slTA.FillByCod(slDT, pedidoVenda.Vendedor);

                                pedidoVenda.Empresa = slDT[0].inv_comp.Trim();
                                pedidoVenda.Projecao = "Não";

                                hlbappService.Pedido_Venda.AddObject(pedidoVenda);
                                hlbappService.SaveChanges();

                                #region Insere LOG - Pedido_Venda

                                DateTime dataCriacaoCHIC = bDTCommercial.Min(m => m.datemodi);

                                LOG_Pedido_Venda logPV = new LOG_Pedido_Venda();
                                logPV.DataPedido = pedidoVenda.DataPedido;
                                logPV.Usuario = "Serviço";
                                logPV.DataHora = DateTime.Now;
                                logPV.CodigoCliente = pedidoVenda.CodigoCliente;
                                logPV.OvosBrasil = pedidoVenda.OvosBrasil;
                                logPV.CondicaoPagamento = pedidoVenda.CondicaoPagamento;
                                logPV.Observacoes = pedidoVenda.Observacoes;
                                logPV.Vendedor = pedidoVenda.Vendedor;
                                logPV.Status = pedidoVenda.Status;
                                logPV.Operacao = pedidoVenda.Status;
                                logPV.IDPedidoVenda = pedidoVenda.ID;
                                logPV.Motivo = "Importação do CHIC p/ WEB. Pedido criado no CHIC por "
                                        + usuarioCHIC + " em " + dataCriacaoCHIC.ToShortDateString();
                                logPV.Projecao = pedidoVenda.Projecao;

                                hlbappService.LOG_Pedido_Venda.AddObject(logPV);
                                hlbappService.SaveChanges();

                                #endregion

                                #endregion

                                #region Insere item

                                var listaItens = bDTCommercial
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                        && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D")))
                                        && !w.alt_desc.Contains("Extra"))
                                    .ToList();

                                int sequencia = 0;

                                foreach (var booked in listaItens)
                                {
                                    sequencia = sequencia + 1;

                                    decimal qtdOvosBonifCHIC = bDTCommercial
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                                && a.item_no == booked.item)
                                            && w.alt_desc.Contains("Extra"))
                                        .Sum(s => s.quantity);

                                    decimal precoTotal = bDTCommercial
                                        .Where(w => w.price > 0)
                                        .Sum(s => s.price);

                                    decimal valorTotal = bDTCommercial
                                        .Where(w => w.price > 0)
                                        .Sum(s => s.price * s.quantity);

                                    string variety = iDT.Where(w => w.item_no == booked.item)
                                        .FirstOrDefault().variety;

                                    string form = iDT.Where(w => w.item_no == booked.item)
                                        .FirstOrDefault().form.Trim();

                                    vartablTableAdapter vartbTA = new vartablTableAdapter();
                                    CHICDataSet.vartablDataTable varDT =
                                        new CHICDataSet.vartablDataTable();

                                    vartbTA.FillByVariety(varDT, variety);

                                    #region Verifica Se é macho para inserir e descrição na frente
                                    // 21/05/2018 - Solicita por André / Débora, pois estavam confundindo com os pedidos de fêmeas
                                    string macho = "";
                                    if (form == "DM") macho = " - Machos";
                                    if (form.Substring(0, 1) == "H") macho = " - Ovos";

                                    #endregion

                                    Item_Pedido_Venda itemPV = new Item_Pedido_Venda();
                                    itemPV.IDPedidoVenda = pedidoVenda.ID;
                                    itemPV.Sequencia = sequencia;
                                    itemPV.ProdCodEstr = varDT[0].desc.Trim() + macho;
                                    itemPV.DataEntregaInicial = booked.cal_date.AddDays(22);
                                    itemPV.DataEntregaFinal = booked.cal_date.AddDays(22);
                                    itemPV.QtdeLiquida = Convert.ToInt32(booked.quantity);
                                    itemPV.QtdeReposicao = 0;
                                    //itemPV.PrecoUnitario = booked.price;
                                    itemPV.PrecoPinto = booked.price;
                                    itemPV.PrecoUnitario = precoTotal;
                                    itemPV.OrderNoCHIC = booked.orderno.Trim();
                                    //itemPV.OrderNoCHICReposicao = icDT[0].npedrepo.ToString();
                                    itemPV.Alterado = 0;
                                    itemPV.Importar = 0;
                                    itemPV.TipoReposicao = "";
                                    itemPV.ValorTotal = valorTotal;

                                    itemPV.QtdeBonificada = Convert.ToInt32(qtdOvosBonifCHIC);
                                    if (itemPV.QtdeBonificada != 0)
                                        itemPV.PercBonificacao = ((itemPV.QtdeBonificada * 1.00m)
                                            / (itemPV.QtdeLiquida * 1.00m)) * 100.00m;
                                    else
                                        itemPV.QtdeBonificada = 0;

                                    if (orderNoCHICReposicao != null)
                                    {
                                        CHICDataSet.bookedDataTable bDTReposicao = new CHICDataSet.bookedDataTable();
                                        bTACommercial.FillByOrderNo(bDTReposicao, orderNoCHICReposicao);
                                        CHICDataSet.bookedRow bRowReposicao = bDTReposicao
                                            .Where(w => w.item == booked.item).FirstOrDefault();

                                        if (bRowReposicao != null)
                                        {
                                            itemPV.OrderNoCHICReposicao = orderNoCHICReposicao;
                                            itemPV.QtdeReposicao = Convert.ToInt32(bRowReposicao.quantity);
                                            if (bRowReposicao.comment_1.Contains("Acerto"))
                                                itemPV.TipoReposicao = "Acerto Comercial";
                                            else if (bRowReposicao.comment_1.Contains("Mortalidade"))
                                                itemPV.TipoReposicao = "Mortalidade";
                                        }
                                    }

                                    #region Campos Customizados do Item

                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, booked.bookkey);

                                    if (ciDT.Count > 0)
                                    {
                                        itemPV.Sobra = (ciDT[0].sobra.Trim() == "Sim" ? 1 : 0);
                                    }

                                    #endregion

                                    hlbappService.Item_Pedido_Venda.AddObject(itemPV);
                                    hlbappService.SaveChanges();

                                    #region Insere LOG - Item_Ped_Venda

                                    LOG_Item_Pedido_Venda logItemPV = new LOG_Item_Pedido_Venda();
                                    logItemPV.IDPedidoVenda = itemPV.IDPedidoVenda;
                                    logItemPV.Sequencia = itemPV.Sequencia;
                                    logItemPV.ProdCodEstr = itemPV.ProdCodEstr;
                                    logItemPV.DataEntregaInicial = itemPV.DataEntregaInicial;
                                    logItemPV.DataEntregaFinal = itemPV.DataEntregaFinal;
                                    logItemPV.QtdeLiquida = itemPV.QtdeLiquida;
                                    logItemPV.PercBonificacao = itemPV.PercBonificacao;
                                    logItemPV.QtdeBonificada = itemPV.QtdeBonificada;
                                    logItemPV.QtdeReposicao = itemPV.QtdeReposicao;
                                    logItemPV.PrecoUnitario = itemPV.PrecoUnitario;
                                    logItemPV.DataHora = DateTime.Now;
                                    logItemPV.Operacao = pedidoVenda.Status;
                                    logItemPV.OrderNoCHIC = itemPV.OrderNoCHIC;
                                    logItemPV.IDItPedVenda = itemPV.ID;
                                    logItemPV.IDLogPedidoVenda = logPV.ID;
                                    logItemPV.OrderNoCHIC = itemPV.OrderNoCHIC;
                                    logItemPV.OrderNoCHICReposicao = itemPV.OrderNoCHICReposicao;
                                    logItemPV.PrecoPinto = itemPV.PrecoPinto;
                                    logItemPV.TipoReposicao = itemPV.TipoReposicao;
                                    logItemPV.ValorTotal = itemPV.ValorTotal;

                                    hlbappService.LOG_Item_Pedido_Venda.AddObject(logItemPV);
                                    hlbappService.SaveChanges();

                                    #endregion
                                }

                                #endregion

                                #region Insere Vacinas

                                var listaVacinas = bDTCommercial
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                        && (a.form.Equals("VC"))))
                                    .ToList();

                                int temVaxxitek = bDTCommercial
                                    .Where(w => w.item.Equals("161"))
                                    .Count();

                                int temVectormuneMG = bDTCommercial
                                    .Where(w => w.item.Equals("188"))
                                    .Count();

                                #region Vacina Primária

                                Vacinas_Primaria_Pedido_Venda vacPrim = null;
                                LOG_Vacinas_Primaria_Pedido_Venda logVacPrim =
                                    new LOG_Vacinas_Primaria_Pedido_Venda();

                                foreach (var vacina in listaVacinas)
                                {
                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, vacina.bookkey);

                                    int tipoCobranca = 0;
                                    if (ciDT.Count > 0)
                                    {
                                        CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                        if (ciR.cobvcsv.Trim().Equals("Bonificação"))
                                            tipoCobranca = 1;
                                        else if (ciR.cobvcsv.Trim().Equals("Cliente Envia"))
                                            tipoCobranca = 2;
                                    }

                                    string codigoCHIC = vacina.item.Trim();

                                    PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                        .Where(w => w.USERCodigoCHIC == codigoCHIC)
                                        .FirstOrDefault();

                                    if (produtoApolo1 != null)
                                    {
                                        PRODUTO produtoApolo = apolo.PRODUTO
                                            .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr)
                                            .FirstOrDefault();

                                        existe = 0;
                                        existe = apolo.PROD_GRUPO_SUBGRUPO
                                            .Where(w => w.GrpProdCod == "041"
                                                && w.SubGrpProdCod == "042"
                                                && ((w.ProdCodEstr == produtoApolo.ProdCodEstr && temVaxxitek == 0)
                                                    || (w.ProdCodEstr == produtoApolo.ProdCodEstr && produtoApolo.ProdCodEstr == "003.006.046" && temVaxxitek > 0)))
                                            .Count();

                                        if (existe > 0
                                            //&& ((vacina.item.Equals("165") && temVaxxitek == 0 && temVectormuneMG == 0) || (!vacina.item.Equals("165")))
                                            )
                                        {
                                            vacPrim = new Vacinas_Primaria_Pedido_Venda();

                                            vacPrim.IDPedidoVenda = pedidoVenda.ID;
                                            vacPrim.ProdCodEstr = produtoApolo.ProdCodEstr;
                                            vacPrim.SeqItemPedVenda = 0;
                                            vacPrim.PrecoUnitario = vacina.price;

                                            decimal valorTabela = CalculaValoresVacinasServicosNovoPV(
                                                produtoApolo.ProdNomeAlt2, vacina.cal_date, vacina.cal_date,
                                                pedidoVenda.Empresa, "Vacina");

                                            //if (valorTabela > 0 && vacina.price == 0 && vacina.quantity > 0)
                                            //    vacPrim.Bonificada = 1;
                                            //else
                                            //    vacPrim.Bonificada = 0;
                                            vacPrim.Bonificada = tipoCobranca;

                                            hlbappService.Vacinas_Primaria_Pedido_Venda.AddObject(vacPrim);
                                            hlbappService.SaveChanges();

                                            #region Insere LOG - Vacina Primária

                                            logVacPrim =
                                                new LOG_Vacinas_Primaria_Pedido_Venda();

                                            if (vacPrim != null)
                                            {
                                                logVacPrim.IDPedidoVenda = vacPrim.IDPedidoVenda;
                                                logVacPrim.ProdCodEstr = vacPrim.ProdCodEstr;
                                                logVacPrim.DataHora = DateTime.Now;
                                                logVacPrim.Operacao = pedidoVenda.Status;
                                                logVacPrim.IDVacPrimPedVenda = vacPrim.ID;
                                                logVacPrim.IDLogPedidoVenda = logPV.ID;
                                                logVacPrim.SeqItemPedVenda = vacPrim.SeqItemPedVenda;
                                                logVacPrim.PrecoUnitario = vacPrim.PrecoUnitario;
                                                logVacPrim.Bonificada = vacPrim.Bonificada;

                                                hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                                                hlbappService.SaveChanges();
                                            }

                                            #endregion
                                        }
                                    }
                                }

                                #endregion

                                #region Vacina Secundária

                                int seqVacSec = 0;

                                if (vacPrim != null)
                                {
                                    foreach (var vacina in listaVacinas)
                                    {
                                        custitemTableAdapter ciTA = new custitemTableAdapter();
                                        CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                        ciTA.FillByBookkey(ciDT, vacina.bookkey);

                                        int tipoCobranca = 0;
                                        if (ciDT.Count > 0)
                                        {
                                            CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                            if (ciR.cobvcsv.Trim().Equals("Bonificação"))
                                                tipoCobranca = 1;
                                            else if (ciR.cobvcsv.Trim().Equals("Cliente Envia"))
                                                tipoCobranca = 2;
                                        }

                                        string codigoCHIC = vacina.item.Trim();

                                        PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                            .Where(w => w.USERCodigoCHIC == codigoCHIC)
                                            .FirstOrDefault();

                                        if (produtoApolo1 != null)
                                        {
                                            PRODUTO produtoApolo = apolo.PRODUTO
                                                .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr)
                                                .FirstOrDefault();

                                            existe = 0;
                                            existe = apolo.PROD_GRUPO_SUBGRUPO
                                                .Where(w => w.GrpProdCod == "041"
                                                    && w.SubGrpProdCod == "043"
                                                    && w.ProdCodEstr == produtoApolo.ProdCodEstr)
                                                .Count();

                                            if (existe > 0)
                                            {
                                                Vacinas_Secundaria_Pedido_Venda vacSec =
                                                    new Vacinas_Secundaria_Pedido_Venda();

                                                seqVacSec = seqVacSec + 1;
                                                vacSec.ProdCodEstr = produtoApolo.ProdCodEstr;
                                                vacSec.Sequencia = seqVacSec;
                                                vacSec.SeqItemPedVenda = 0;
                                                vacSec.IDVacPrimPedVenda = vacPrim.ID;
                                                vacSec.PrecoUnitario = vacina.price;

                                                decimal valorTabela = CalculaValoresVacinasServicosNovoPV(
                                                    produtoApolo.ProdNomeAlt2, vacina.cal_date, vacina.cal_date,
                                                    pedidoVenda.Empresa, "Vacina");

                                                //if (valorTabela > 0 && vacina.price == 0 && vacina.quantity > 0)
                                                //    vacSec.Bonificada = 1;
                                                //else
                                                //    vacSec.Bonificada = 0;
                                                vacSec.Bonificada = tipoCobranca;

                                                hlbappService.Vacinas_Secundaria_Pedido_Venda.AddObject(vacSec);

                                                #region Insere LOG - Vacina_Secundaria_Pedido_Venda

                                                LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                                                logVacSec.IDVacPrimPedVenda = vacSec.IDVacPrimPedVenda;
                                                logVacSec.Sequencia = vacSec.Sequencia;
                                                logVacSec.ProdCodEstr = vacSec.ProdCodEstr;
                                                logVacSec.DataHora = DateTime.Now;
                                                logVacSec.Operacao = pedidoVenda.Status;
                                                logVacSec.IDVacSecPedVenda = vacSec.ID;
                                                logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;
                                                logVacSec.SeqItemPedVenda = vacSec.SeqItemPedVenda;
                                                logVacSec.PrecoUnitario = vacSec.PrecoUnitario;
                                                logVacSec.Bonificada = vacSec.Bonificada;

                                                hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);

                                                #endregion
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                #region Insere Serviço

                                var listaServico = bDTCommercial
                                    .Where(w => w.item.Trim() == "169")
                                    .ToList();

                                foreach (var servico in listaServico)
                                {
                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, servico.bookkey);

                                    int tipoCobranca = 0;
                                    if (ciDT.Count > 0)
                                    {
                                        CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                        if (ciR.cobvcsv.Trim().Equals("Bonificação"))
                                            tipoCobranca = 1;
                                        else if (ciR.cobvcsv.Trim().Equals("Cliente Envia"))
                                            tipoCobranca = 2;
                                    }

                                    string codCHIC = servico.item.Trim();

                                    PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                        .Where(w => w.USERCodigoCHIC == codCHIC)
                                        .FirstOrDefault();

                                    PRODUTO produtoApolo = apolo.PRODUTO
                                        .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr)
                                        .FirstOrDefault();

                                    Servicos_Pedido_Venda serv = new Servicos_Pedido_Venda();
                                    serv.IDPedidoVenda = pedidoVenda.ID;
                                    serv.ProdCodEstr = produtoApolo.ProdCodEstr;
                                    serv.PercAplicacaoServico = 100;
                                    serv.PrecoUnitario = servico.price;

                                    decimal valorTabela = CalculaValoresVacinasServicosNovoPV(
                                        "Tratamento Infravermelho", servico.cal_date, servico.cal_date,
                                        pedidoVenda.Empresa, "Serviço");

                                    //if (valorTabela > 0 && servico.price == 0 && servico.quantity > 0)
                                    //    serv.Bonificada = 1;
                                    //else
                                    //    serv.Bonificada = 0;
                                    serv.Bonificada = tipoCobranca;

                                    hlbappService.Servicos_Pedido_Venda.AddObject(serv);
                                    hlbappService.SaveChanges();

                                    #region Insere LOG - Servico_Pedido_Venda

                                    LOG_Servicos_Pedido_Venda logServ = new LOG_Servicos_Pedido_Venda();
                                    logServ.IDPedidoVenda = serv.IDPedidoVenda;
                                    logServ.ProdCodEstr = serv.ProdCodEstr;
                                    logServ.PercAplicacaoServico = serv.PercAplicacaoServico;
                                    logServ.DataHora = DateTime.Now;
                                    logServ.Operacao = pedidoVenda.Status;
                                    logServ.IDServPedVenda = serv.ID;
                                    logServ.IDLogPedidoVenda = logPV.ID;
                                    logServ.PrecoUnitario = serv.PrecoUnitario;
                                    logServ.Bonificada = serv.Bonificada;

                                    hlbappService.LOG_Servicos_Pedido_Venda.AddObject(logServ);
                                    hlbappService.SaveChanges();

                                    #endregion
                                }

                                #endregion
                            }
                        }
                    }
                }

                hlbappService.SaveChanges();

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Atualizar CHIC com WEB - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message + " / Pedido: " + orderNoErro;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                #region Envio de E-mail

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ WEB - MANUAIS NO CHIC ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Paulo Alves";
                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                string corpoEmail = "";
                string innerException = "";

                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }

                corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o WEB: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "Linha do Erro: " + erro.ToString() + (char)13 + (char)10
                    + "Número do Pedido CHIC: " + orderNoErro + (char)13 + (char)10
                    + "Linha do Erro: " + linenum.ToString() + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 1: " + ex.Message + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 2: " + innerException;

                email.WorkFlowEmailCorpo = corpoEmail;
                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                return retorno;
            }
        }

        public string AtualizaPedidosVendidosWEBxCHIC()
        {
            string orderNoErro = "";
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            try
            {
                #region Atualiza Pedidos de Venda do CHIC para o WEB

                #region Carrega variaveis e lista dos itens a serem verificados

                string motivo = "";
                string usuarioCHIC = "";
                DateTime dataFiltro = DateTime.Today.AddDays(-30);
                //DateTime dataFiltro = Convert.ToDateTime("01/01/2019");
                DateTime dataInicioModeloNovo = Convert.ToDateTime("03/12/2017");

                var listaWEB = hlbappService.Item_Pedido_Venda
                    .Where(a => a.DataEntregaFinal >= dataFiltro
                        //&& a.IDPedidoVenda == 41264
                        && hlbappService.Pedido_Venda.Any(p => p.ID == a.IDPedidoVenda
                            && (p.Status.Contains("Importado") || p.Status.Contains("Reprovad")))
                        && ((a.OrderNoCHIC != null && a.OrderNoCHIC != ""
                            && a.OrderNoCHIC != "Cancelado"
                            //&& a.OrderNoCHIC == "91216"
                            )
                            //|| (a.OrderNoCHICReposicao != null && a.OrderNoCHICReposicao != "")))
                            )
                        && a.DataEntregaFinal >= dataInicioModeloNovo)
                        //&& hlbappService.VU_Verifica_Pedidos_Num_CHIC_Duplicados
                        //    .Any(c => c.ID != a.IDPedidoVenda))
                    .GroupBy(g =>
                        new
                        {
                            g.OrderNoCHIC,
                            g.IDPedidoVenda
                        })
                    .Select(s =>
                        new
                        {
                            s.Key.OrderNoCHIC,
                            s.Key.IDPedidoVenda
                        })
                    .ToList();

                #endregion

                foreach (var pedido in listaWEB)
                {
                    motivo = "Dados Atualizados: ";

                    #region Carrega objetos

                    string orderNo = pedido.OrderNoCHIC;
                    orderNoErro = pedido.OrderNoCHIC;

                    if (pedido.OrderNoCHIC.Equals("75504"))
                        orderNoErro = pedido.OrderNoCHIC;

                    Pedido_Venda pedVenda = hlbappService.Pedido_Venda.Where(w => w.ID == pedido.IDPedidoVenda)
                        .FirstOrDefault();

                    ordersTableAdapter oTACommercial = new ordersTableAdapter();
                    CHICDataSet.ordersDataTable oDTCommercial = new CHICDataSet.ordersDataTable();
                    oTACommercial.FillByOrderNo(oDTCommercial, orderNo);
                    CHICDataSet.ordersRow oR = oDTCommercial.FirstOrDefault();

                    #endregion

                    if (oR != null)
                    {
                        #region Carrega Itens do CHIC

                        ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                            new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                        CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                        bTACommercial.FillByOrderNo(bDTCommercial, orderNo);

                        usuarioCHIC = "";
                        if (bDTCommercial.Count > 0)
                            usuarioCHIC = bDTCommercial.OrderByDescending(o => o.datemodi)
                                .FirstOrDefault().modifdby.Trim();

                        itemsTableAdapter iTA = new itemsTableAdapter();
                        CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                        iTA.Fill(iDT);

                        var listaVacinasCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && a.form == "VC")).ToList();

                        var listaServicosCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && a.form == "SV")).ToList();

                        #endregion

                        #region Carrega Dados Localização Cliente

                        string custNo = oR.cust_no.Trim();
                        ENTIDADE entidade = apolo.ENTIDADE
                            .Where(e1 => e1.EntCod == custNo).First();

                        CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                        UNID_FEDERACAO uf = apolo.UNID_FEDERACAO
                            .Where(w => w.UfSigla == cidade.UfSigla && w.PaisSigla == cidade.PaisSigla)
                            .FirstOrDefault();

                        #endregion

                        #region Cliente

                        if (oR.cust_no.Trim() != pedVenda.CodigoCliente)
                        {
                            pedVenda.CodigoCliente = oR.cust_no.Trim();
                            pedVenda.NomeCliente = entidade.EntNome;
                            motivo = motivo + " cliente";
                        }

                        #endregion

                        #region Endereço de Entrega

                        if (oR.contact_no != pedVenda.EnderEntSeq)
                        {
                            pedVenda.EnderEntSeq = Convert.ToInt32(oR.contact_no);
                            if (motivo == "Dados Atualizados: ") motivo = motivo + " endereço de entrega";
                            else motivo = motivo + ", endereço de entrega";
                        }

                        #endregion

                        #region Condição de Pagamento

                        if (oR.delivery.Trim() != pedVenda.CondicaoPagamento)
                        {
                            pedVenda.CondicaoPagamento = oR.delivery.Trim();
                            if (motivo == "Dados Atualizados: ") motivo = motivo + " condição de pagamento";
                            else motivo = motivo + ", condição de pagamento";
                        }

                        #endregion

                        #region Vendedor

                        if (oR.salesrep.Trim() != pedVenda.Vendedor)
                        {
                            pedVenda.Vendedor = oR.salesrep.Trim();
                            if (motivo == "Dados Atualizados: ") motivo = motivo + " vendedor";
                            else motivo = motivo + ", vendedor";
                        }

                        #endregion

                        #region Observação

                        int_commTableAdapter icTA = new int_commTableAdapter();
                        CHICDataSet.int_commDataTable icDT = new CHICDataSet.int_commDataTable();
                        icTA.FillByOrderNo(icDT, orderNo);
                        CHICDataSet.int_commRow icR = icDT.FirstOrDefault();

                        if (icR != null)
                        {
                            if (icR.comments.Trim() != pedVenda.Observacoes)
                            {
                                pedVenda.Observacoes = icR.comments.Trim();
                                if (motivo == "Dados Atualizados: ") motivo = motivo + " observações";
                                else motivo = motivo + ", observações";
                            }
                        }

                        #endregion

                        #region Status

                        if (pedVenda.Status.Contains("Reprovad"))
                        {
                            var listaItens = hlbappService.Item_Pedido_Venda
                                .Where(w => w.IDPedidoVenda == pedVenda.ID).ToList();

                            foreach (var itemPV in listaItens)
                            {
                                itemPV.Importar = 0;
                            }

                            int existeCancelado = listaItens
                                .Where(w => w.IDPedidoVenda == pedVenda.ID
                                && w.OrderNoCHIC == "Cancelado").Count();

                            int existeNaoCancelado = listaItens
                                .Where(w => w.IDPedidoVenda == pedVenda.ID
                                && w.OrderNoCHIC != "Cancelado").Count();

                            string status = "Importado Total";
                            if (existeCancelado > 0 && existeNaoCancelado > 0)
                                status = "Importado Parcial";
                            
                            //pedVenda.Status = status;
                            //if (motivo == "Dados Atualizados: ") 
                            //    motivo = motivo + " status alterado para " + status + " por motivo de reprovação";
                            //else motivo = motivo + ", status alterado para " + status + " por motivo de reprovação";
                        }

                        #endregion

                        #region Carrega Dados de Reposição se Existir

                        CHICDataSet.int_commDataTable icDTReposicao = new CHICDataSet.int_commDataTable();
                        icTA.FillByNpedrepo(icDTReposicao, Convert.ToDecimal(oR.orderno));
                        CHICDataSet.int_commRow icRowReposicao = icDTReposicao.FirstOrDefault();

                        #endregion

                        hlbappService.SaveChanges();

                        #region Insere LOG - Pedido_Venda - DESATIVADO

                        //LOG_Pedido_Venda logPV = new LOG_Pedido_Venda();

                        //logPV = new LOG_Pedido_Venda();
                        //logPV.DataPedido = pedVenda.DataPedido;
                        ////logPV.Usuario = "Serviço";
                        //logPV.Usuario = "CHIC - " + usuarioCHIC;
                        //logPV.DataHora = DateTime.Now;
                        //logPV.CodigoCliente = pedVenda.CodigoCliente;
                        //logPV.OvosBrasil = pedVenda.OvosBrasil;
                        //logPV.CondicaoPagamento = pedVenda.CondicaoPagamento;
                        //logPV.Observacoes = pedVenda.Observacoes;
                        //logPV.Vendedor = pedVenda.Vendedor;
                        //logPV.Status = pedVenda.Status;
                        //logPV.Operacao = "Importado p/ WEB";
                        //logPV.IDPedidoVenda = pedVenda.ID;
                        ////logPV.Motivo = "Atualização da Qtde. Líquida do CHIC p/ WEB";

                        //hlbappService.LOG_Pedido_Venda.AddObject(logPV);
                        //hlbappService.SaveChanges();

                        #endregion

                        #region Vacinas

                        decimal valorTotalVacinas = 0;

                        Vacinas_Primaria_Pedido_Venda vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                        if (vacPrimObj != null)
                        {
                            #region Vacina Primária

                            #region Carrega Dados da Vacina no Apolo

                            PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                .Where(w => w.ProdCodEstr == vacPrimObj.ProdCodEstr)
                                .FirstOrDefault();

                            PRODUTO produtoApolo = new PRODUTO();

                            #endregion

                            if (produtoApolo1 != null)
                            {
                                produtoApolo = apolo.PRODUTO
                                    .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr)
                                    .FirstOrDefault();

                                #region Verifica se é Vaxxitek

                                bool temVaxxitek = false;
                                if (produtoApolo != null)
                                    if (produtoApolo.ProdNomeAlt1.Contains("VAXXITEK"))
                                        temVaxxitek = true;

                                #endregion

                                string codigoVacinaPrimaria = produtoApolo1.USERCodigoCHIC;
                                //if ((bDTCommercial.Where(w => w.item.Equals("161") || w.item.Equals("165")).Count() > 1)
                                //    &&
                                //    (produtoApolo1.USERCodigoCHIC != "161"))
                                if (!temVaxxitek && bDTCommercial.Where(w => w.item.Equals("161")).Count() > 1)
                                {
                                    vacPrimObj.ProdCodEstr = "003.006.046";
                                    codigoVacinaPrimaria = "161";
                                    if (motivo == "Dados Atualizados: ") motivo = motivo
                                        + " alterada vacina primária para: VAXXITEK";
                                    else motivo = motivo + " alterada vacina primária para: VAXXITEK";
                                }

                                // 16/12/2019 - Alteração da Vacina 165 que virou secundária para a 909 que é a nova primária de Rispens
                                if ((bDTCommercial.Where(w => w.item.Equals("165")).Count() > 0)
                                    &&
                                    (produtoApolo1.USERCodigoCHIC == "909"))
                                {
                                    codigoVacinaPrimaria = "909";
                                    CHICDataSet.bookedRow bVacinaPrimaria909 = bDTCommercial.Where(w => w.item == "165").FirstOrDefault();
                                    bTACommercial.UpdateItemNo("909", bVacinaPrimaria909.book_id);
                                    bTACommercial.FillByOrderNo(bDTCommercial, orderNo);
                                    listaVacinasCHIC = bDTCommercial
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                            && a.form == "VC")).ToList();
                                }

                                CHICDataSet.bookedRow bVacinaPrimaria = bDTCommercial
                                    .Where(w => ((w.item == codigoVacinaPrimaria && !temVaxxitek)
                                        || (w.item == codigoVacinaPrimaria && codigoVacinaPrimaria == "161" && temVaxxitek))).FirstOrDefault();

                                if (bVacinaPrimaria != null)
                                {
                                    // Se a vacina é a mesma
                                    #region Valor da Vacina

                                    if (bVacinaPrimaria.price != vacPrimObj.PrecoUnitario)
                                    {
                                        vacPrimObj.PrecoUnitario = bVacinaPrimaria.price;
                                        if (motivo == "Dados Atualizados: ") motivo = motivo + " preço vacina da primária: "
                                            + produtoApolo.ProdNomeAlt1;
                                        else motivo = motivo + ", preço vacina da primária: "
                                            + produtoApolo.ProdNomeAlt1;
                                    }

                                    #endregion

                                    #region Tipo de Cobrança

                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, bVacinaPrimaria.bookkey);

                                    if (ciDT.Count > 0)
                                    {
                                        CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                        int tipoCobrancaCHIC = 0;
                                        if (ciR.cobvcsv.Trim() == "Bonificação") tipoCobrancaCHIC = 1;
                                        else if (ciR.cobvcsv.Trim() == "Cliente Envia") tipoCobrancaCHIC = 2;

                                        if (tipoCobrancaCHIC != vacPrimObj.Bonificada)
                                        {
                                            vacPrimObj.Bonificada = tipoCobrancaCHIC;
                                            if (motivo == "Dados Atualizados: ") motivo = motivo
                                                + " tipo de cobrança da vacina da primária: "
                                                + produtoApolo.ProdNomeAlt1;
                                            else motivo = motivo + ", tipo de cobrança vacina da primária: "
                                                + produtoApolo.ProdNomeAlt1;
                                        }
                                    }
                                    else
                                        vacPrimObj.Bonificada = 0;

                                    #endregion

                                    valorTotalVacinas = bVacinaPrimaria.price;
                                }
                                else
                                {
                                    #region Altera a Vacina Primária para ficar igual a do CHIC

                                    int existeVacinaPrimaria = 0;
                                    foreach (var vacinaCHIC in listaVacinasCHIC)
                                    {
                                        PRODUTO1 vacinaPrimariaApolo = apolo.PRODUTO1
                                            .Where(w => apolo.PROD_GRUPO_SUBGRUPO
                                                .Any(a => w.ProdCodEstr == a.ProdCodEstr
                                                    && a.GrpProdCod == "041" && a.SubGrpProdCod == "042")
                                                && ((w.USERCodigoCHIC == vacinaCHIC.item && !temVaxxitek) || 
                                                (w.USERCodigoCHIC == vacinaCHIC.item && vacinaCHIC.item == "161" && temVaxxitek)))
                                            .FirstOrDefault();

                                        if (vacinaPrimariaApolo != null)
                                        {
                                            existeVacinaPrimaria = 1;

                                            PRODUTO vacinaPrimariaApoloObj = apolo.PRODUTO
                                                .Where(w => w.ProdCodEstr == vacinaPrimariaApolo.ProdCodEstr)
                                                .FirstOrDefault();

                                            vacPrimObj.ProdCodEstr = vacinaPrimariaApolo.ProdCodEstr;
                                            vacPrimObj.PrecoUnitario = vacinaCHIC.price;
                                            vacPrimObj.Bonificada = 0;

                                            custitemTableAdapter ciTA = new custitemTableAdapter();
                                            CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                            ciTA.FillByBookkey(ciDT, vacinaCHIC.bookkey);
                                            CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                            if (ciR != null)
                                            {
                                                if (ciR.cobvcsv.Equals("Bonificação")) vacPrimObj.Bonificada = 1;
                                                else if (ciR.cobvcsv.Equals("Cliente Envia")) vacPrimObj.Bonificada = 2;
                                            }

                                            if (motivo == "Dados Atualizados: ") motivo = motivo
                                                + " inserida vacina primária: "
                                                + vacinaPrimariaApoloObj.ProdNomeAlt1;
                                            else motivo = motivo + ", inserida vacina primária: "
                                                + vacinaPrimariaApoloObj.ProdNomeAlt1;

                                            valorTotalVacinas = vacinaCHIC.price;
                                        }
                                    }

                                    #endregion

                                    #region Se não existe vacina primária no CHIC, deleta ela e as secundárias amarradas

                                    if (existeVacinaPrimaria == 0)
                                    {
                                        HLBAPPServiceEntities hlbappServiceSession = new HLBAPPServiceEntities();

                                        var vacPrimObjDelete = hlbappServiceSession.Vacinas_Primaria_Pedido_Venda
                                            .Where(w => w.ID == vacPrimObj.ID).FirstOrDefault();

                                        var listaVacinasSecundarias = hlbappServiceSession.Vacinas_Secundaria_Pedido_Venda
                                            .Where(w => w.IDVacPrimPedVenda == vacPrimObj.ID).ToList();

                                        foreach (var vacSecObj in listaVacinasSecundarias)
                                        {
                                            hlbappServiceSession.Vacinas_Secundaria_Pedido_Venda.DeleteObject(vacSecObj);
                                        }

                                        // Se vacina não existe no CHIC, será alterada pela que está no CHIC
                                        hlbappServiceSession.Vacinas_Primaria_Pedido_Venda.DeleteObject(vacPrimObjDelete);

                                        PRODUTO vacinaPrimariaApoloObj = apolo.PRODUTO
                                            .Where(w => w.ProdCodEstr == vacPrimObj.ProdCodEstr)
                                            .FirstOrDefault();

                                        if (motivo == "Dados Atualizados: ") motivo = motivo
                                                + " deletada vacina primária: "
                                                + vacinaPrimariaApoloObj.ProdNomeAlt1;
                                        else motivo = motivo + ", deletada vacina primária: "
                                            + vacinaPrimariaApoloObj.ProdNomeAlt1;

                                        hlbappServiceSession.SaveChanges();
                                    }

                                    #endregion
                                }
                            }

                            #region Insere LOG - Vacina Primária - DESATIVADO

                            //LOG_Vacinas_Primaria_Pedido_Venda logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                            //logVacPrim.IDPedidoVenda = vacPrimObj.IDPedidoVenda;
                            //logVacPrim.ProdCodEstr = vacPrimObj.ProdCodEstr;
                            //logVacPrim.DataHora = DateTime.Now;
                            //logVacPrim.Operacao = "Importado p/ WEB";
                            //logVacPrim.IDVacPrimPedVenda = vacPrimObj.ID;
                            //logVacPrim.IDLogPedidoVenda = logPV.ID;
                            //logVacPrim.PrecoUnitario = vacPrimObj.PrecoUnitario;
                            //logVacPrim.Bonificada = vacPrimObj.Bonificada;

                            //hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                            //hlbappService.SaveChanges();

                            #endregion

                            #endregion

                            #region Vacinas Secundárias

                            var listaVacSec = hlbappService.Vacinas_Secundaria_Pedido_Venda.Where(w => w.IDVacPrimPedVenda == vacPrimObj.ID).ToList();

                            foreach (var vacSec in listaVacSec)
                            {
                                #region Carrega Dados da Vacina no Apolo

                                produtoApolo1 = null;
                                produtoApolo1 = apolo.PRODUTO1
                                    .Where(w => w.ProdCodEstr == vacSec.ProdCodEstr)
                                    .FirstOrDefault();

                                produtoApolo = null;
                                produtoApolo = apolo.PRODUTO
                                    .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr)
                                    .FirstOrDefault();

                                #endregion

                                if (produtoApolo1 != null)
                                {
                                    CHICDataSet.bookedRow bVacinaSec = bDTCommercial
                                        .Where(w => w.item == produtoApolo1.USERCodigoCHIC).FirstOrDefault();

                                    if (bVacinaSec != null)
                                    {
                                        // Se a vacina é a mesma
                                        #region Valor da Vacina

                                        if (bVacinaSec.price != vacSec.PrecoUnitario)
                                        {
                                            vacSec.PrecoUnitario = bVacinaSec.price;
                                            if (motivo == "Dados Atualizados: ") motivo = motivo + " preço vacina da secundária: "
                                                + produtoApolo.ProdNomeAlt1;
                                            else motivo = motivo + ", preço vacina da secundária: "
                                                + produtoApolo.ProdNomeAlt1;
                                        }

                                        #endregion

                                        #region Tipo de Cobrança

                                        custitemTableAdapter ciTA = new custitemTableAdapter();
                                        CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                        ciTA.FillByBookkey(ciDT, bVacinaSec.bookkey);

                                        if (ciDT.Count > 0)
                                        {
                                            CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                            int tipoCobrancaCHIC = 0;
                                            if (ciR.cobvcsv.Trim() == "Bonificação") tipoCobrancaCHIC = 1;
                                            else if (ciR.cobvcsv.Trim() == "Cliente Envia") tipoCobrancaCHIC = 2;

                                            if (tipoCobrancaCHIC != vacSec.Bonificada)
                                            {
                                                vacSec.Bonificada = tipoCobrancaCHIC;
                                                if (motivo == "Dados Atualizados: ") motivo = motivo
                                                    + " tipo de cobrança da vacina da secundária: "
                                                    + produtoApolo.ProdNomeAlt1;
                                                else motivo = motivo + ", tipo de cobrança vacina da secundária: "
                                                    + produtoApolo.ProdNomeAlt1;
                                            }
                                        }
                                        else
                                            vacSec.Bonificada = 0;

                                        #endregion

                                        valorTotalVacinas = valorTotalVacinas + bVacinaSec.price;
                                    }
                                    else
                                    {
                                        // Se vacina não existe no CHIC, será deletada do Web
                                        hlbappService.Vacinas_Secundaria_Pedido_Venda.DeleteObject(vacSec);

                                        PRODUTO vacinaSecApoloObj = apolo.PRODUTO
                                            .Where(w => w.ProdCodEstr == vacSec.ProdCodEstr)
                                            .FirstOrDefault();

                                        if (motivo == "Dados Atualizados: ") motivo = motivo
                                                + " deletada vacina secundária: "
                                                + vacinaSecApoloObj.ProdNomeAlt1;
                                        else motivo = motivo + ", deletada vacina secundária: "
                                            + vacinaSecApoloObj.ProdNomeAlt1;
                                    }
                                }

                                #region Insere LOG - Vacina_Secundaria_Pedido_Venda - DESATIVADO

                                //LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                                //logVacSec.IDVacPrimPedVenda = vacSec.IDVacPrimPedVenda;
                                //logVacSec.Sequencia = vacSec.Sequencia;
                                //logVacSec.ProdCodEstr = vacSec.ProdCodEstr;
                                //logVacSec.DataHora = DateTime.Now;
                                //logVacSec.Operacao = "Importado p/ WEB";
                                //logVacSec.IDVacSecPedVenda = vacSec.ID;
                                //logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;
                                //logVacSec.PrecoUnitario = vacSec.PrecoUnitario;
                                //logVacSec.Bonificada = vacSec.Bonificada;

                                //hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);

                                #endregion
                            }

                            #region Verifica Vacinas Secundárias que tem no CHIC e não no WEB para inserir

                            vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                            if (vacPrimObj != null)
                            {
                                foreach (var vacinaCHIC in listaVacinasCHIC)
                                {
                                    PRODUTO1 vacinaSecApolo = apolo.PRODUTO1
                                        .Where(w => apolo.PROD_GRUPO_SUBGRUPO
                                            .Any(a => w.ProdCodEstr == a.ProdCodEstr
                                                && a.GrpProdCod == "041" && a.SubGrpProdCod == "043")
                                            && w.USERCodigoCHIC == vacinaCHIC.item)
                                        .FirstOrDefault();

                                    if (vacinaSecApolo != null)
                                    {
                                        int existePedido = listaVacSec
                                            .Where(w => w.ProdCodEstr == vacinaSecApolo.ProdCodEstr)
                                            .Count();

                                        if (existePedido == 0)
                                        {
                                            PRODUTO vacinaSecApoloObj = apolo.PRODUTO
                                                .Where(w => w.ProdCodEstr == vacinaSecApolo.ProdCodEstr)
                                                .FirstOrDefault();

                                            Vacinas_Secundaria_Pedido_Venda vacinaSecP =
                                                new Vacinas_Secundaria_Pedido_Venda();
                                            vacinaSecP.IDVacPrimPedVenda = vacPrimObj.ID;
                                            vacinaSecP.ProdCodEstr = vacinaSecApolo.ProdCodEstr;
                                            vacinaSecP.PrecoUnitario = vacinaCHIC.price;
                                            vacinaSecP.Bonificada = 0;

                                            custitemTableAdapter ciTA = new custitemTableAdapter();
                                            CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                            ciTA.FillByBookkey(ciDT, vacinaCHIC.bookkey);
                                            CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                            if (ciR != null)
                                            {
                                                if (ciR.cobvcsv.Equals("Bonificação")) vacinaSecP.Bonificada = 1;
                                                else if (ciR.cobvcsv.Equals("Cliente Envia")) vacinaSecP.Bonificada = 2;
                                            }

                                            if (motivo == "Dados Atualizados: ") motivo = motivo
                                                + " inserida vacina secundária: "
                                                + vacinaSecApoloObj.ProdNomeAlt1;
                                            else motivo = motivo + ", inserida vacina secundária: "
                                                + vacinaSecApoloObj.ProdNomeAlt1;

                                            hlbappService.Vacinas_Secundaria_Pedido_Venda.AddObject(vacinaSecP);

                                            valorTotalVacinas = valorTotalVacinas + vacinaCHIC.price;
                                        }
                                    }
                                }
                            }

                            #endregion

                            #endregion
                        }
                        else
                        {
                            #region Se existe vacina Primária no CHIC e não no WEB, inserir no WEB

                            vacPrimObj = new Vacinas_Primaria_Pedido_Venda();
                            LOG_Vacinas_Primaria_Pedido_Venda logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                            foreach (var vacinaCHIC in listaVacinasCHIC)
                            {
                                PRODUTO1 vacinaPrimariaApolo = apolo.PRODUTO1
                                    .Where(w => apolo.PROD_GRUPO_SUBGRUPO.Any(a => w.ProdCodEstr == a.ProdCodEstr && a.GrpProdCod == "041" && a.SubGrpProdCod == "042")
                                        && (
                                            (apolo.PRODUTO.Any(b => b.ProdCodEstr == w.ProdCodEstr && !b.ProdNomeAlt1.Contains("VAXXITEK")) && w.USERCodigoCHIC == vacinaCHIC.item)
                                            ||
                                            (apolo.PRODUTO.Any(b => b.ProdCodEstr == w.ProdCodEstr && b.ProdNomeAlt1.Contains("VAXXITEK")) && w.USERCodigoCHIC == vacinaCHIC.item
                                                && vacinaCHIC.item == "161")
                                        )
                                     )
                                    .FirstOrDefault();

                                if (vacinaPrimariaApolo != null)
                                {
                                    PRODUTO vacinaPrimariaApoloObj = apolo.PRODUTO
                                        .Where(w => w.ProdCodEstr == vacinaPrimariaApolo.ProdCodEstr)
                                        .FirstOrDefault();

                                    vacPrimObj.ProdCodEstr = vacinaPrimariaApolo.ProdCodEstr;
                                    vacPrimObj.PrecoUnitario = vacinaCHIC.price;
                                    vacPrimObj.Bonificada = 0;

                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, vacinaCHIC.bookkey);
                                    CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                    if (ciR != null)
                                    {
                                        if (ciR.cobvcsv.Equals("Bonificação")) vacPrimObj.Bonificada = 1;
                                        else if (ciR.cobvcsv.Equals("Cliente Envia")) vacPrimObj.Bonificada = 2;
                                    }

                                    if (motivo == "Dados Atualizados: ") motivo = motivo
                                        + " inserida vacina primária: "
                                        + vacinaPrimariaApoloObj.ProdNomeAlt1;
                                    else motivo = motivo + ", inserida vacina primária: "
                                        + vacinaPrimariaApoloObj.ProdNomeAlt1;

                                    valorTotalVacinas = vacinaCHIC.price;

                                    #region Insere LOG - Vacina Primária - DESATIVADO

                                    //logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                                    //logVacPrim.IDPedidoVenda = vacPrimObj.IDPedidoVenda;
                                    //logVacPrim.ProdCodEstr = vacPrimObj.ProdCodEstr;
                                    //logVacPrim.DataHora = DateTime.Now;
                                    //logVacPrim.Operacao = "Importado p/ WEB";
                                    //logVacPrim.IDVacPrimPedVenda = vacPrimObj.ID;
                                    //logVacPrim.IDLogPedidoVenda = logPV.ID;
                                    //logVacPrim.PrecoUnitario = vacPrimObj.PrecoUnitario;
                                    //logVacPrim.Bonificada = vacPrimObj.Bonificada;

                                    //hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                                    //hlbappService.SaveChanges();

                                    #endregion
                                }
                            }

                            #endregion

                            #region Verifica Vacinas Secundárias que tem no CHIC e não no WEB para inserir

                            vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                            if (vacPrimObj != null)
                            {
                                foreach (var vacinaCHIC in listaVacinasCHIC)
                                {
                                    PRODUTO1 vacinaSecApolo = apolo.PRODUTO1
                                        .Where(w => apolo.PROD_GRUPO_SUBGRUPO
                                            .Any(a => w.ProdCodEstr == a.ProdCodEstr
                                                && a.GrpProdCod == "041" && a.SubGrpProdCod == "043")
                                            && w.USERCodigoCHIC == vacinaCHIC.item)
                                        .FirstOrDefault();

                                    if (vacinaSecApolo != null)
                                    {
                                        PRODUTO vacinaSecApoloObj = apolo.PRODUTO
                                            .Where(w => w.ProdCodEstr == vacinaSecApolo.ProdCodEstr)
                                            .FirstOrDefault();

                                        Vacinas_Secundaria_Pedido_Venda vacinaSecP =
                                            new Vacinas_Secundaria_Pedido_Venda();
                                        vacinaSecP.IDVacPrimPedVenda = vacPrimObj.ID;
                                        vacinaSecP.ProdCodEstr = vacinaSecApolo.ProdCodEstr;
                                        vacinaSecP.PrecoUnitario = vacinaCHIC.price;
                                        vacinaSecP.Bonificada = 0;

                                        custitemTableAdapter ciTA = new custitemTableAdapter();
                                        CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                        ciTA.FillByBookkey(ciDT, vacinaCHIC.bookkey);
                                        CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                        if (ciR != null)
                                        {
                                            if (ciR.cobvcsv.Equals("Bonificação")) vacinaSecP.Bonificada = 1;
                                            else if (ciR.cobvcsv.Equals("Cliente Envia")) vacinaSecP.Bonificada = 2;
                                        }

                                        if (motivo == "Dados Atualizados: ") motivo = motivo
                                            + " inserida vacina secundária: "
                                            + vacinaSecApoloObj.ProdNomeAlt1;
                                        else motivo = motivo + ", inserida vacina secundária: "
                                            + vacinaSecApoloObj.ProdNomeAlt1;

                                        hlbappService.Vacinas_Secundaria_Pedido_Venda.AddObject(vacinaSecP);

                                        valorTotalVacinas = valorTotalVacinas + vacinaCHIC.price;

                                        #region Insere LOG - Vacina_Secundaria_Pedido_Venda - DESATIVADO

                                        //LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                                        //logVacSec.IDVacPrimPedVenda = vacinaSecP.IDVacPrimPedVenda;
                                        //logVacSec.Sequencia = vacinaSecP.Sequencia;
                                        //logVacSec.ProdCodEstr = vacinaSecP.ProdCodEstr;
                                        //logVacSec.DataHora = DateTime.Now;
                                        //logVacSec.Operacao = "Importado p/ WEB";
                                        //logVacSec.IDVacSecPedVenda = vacinaSecP.ID;
                                        //logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;
                                        //logVacSec.PrecoUnitario = vacinaSecP.PrecoUnitario;
                                        //logVacSec.Bonificada = vacinaSecP.Bonificada;

                                        //hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        #region Serviços

                        decimal valorTotalServicos = 0;
                        decimal percServicos = 0;

                        Servicos_Pedido_Venda serv = hlbappService.Servicos_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                        if (serv != null)
                        {
                            #region Carrega Dados do Serviço no Apolo

                            PRODUTO1 produtoApolo1S = apolo.PRODUTO1
                                .Where(w => w.ProdCodEstr == serv.ProdCodEstr)
                                .FirstOrDefault();

                            PRODUTO produtoApoloS = apolo.PRODUTO
                                .Where(w => w.ProdCodEstr == produtoApolo1S.ProdCodEstr)
                                .FirstOrDefault();

                            #endregion

                            if (produtoApolo1S != null)
                            {
                                CHICDataSet.bookedRow bServico = bDTCommercial
                                    .Where(w => w.item == produtoApolo1S.USERCodigoCHIC).FirstOrDefault();

                                if (bServico != null)
                                {
                                    // Se o serviço é o mesmo
                                    #region Valor do Serviço

                                    if (bServico.price != serv.PrecoUnitario)
                                    {
                                        serv.PrecoUnitario = bServico.price;
                                        if (motivo == "Dados Atualizados: ") motivo = motivo + " preço do serviço: "
                                            + produtoApoloS.ProdNomeAlt1;
                                        else motivo = motivo + ", preço do serviço: "
                                            + produtoApoloS.ProdNomeAlt1;
                                    }

                                    #endregion

                                    #region % do Serviço

                                    decimal qtdCHIC = bDTCommercial
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H")
                                                || a.form.Substring(0, 1).Equals("D"))))
                                        .Sum(s => s.quantity);

                                    //decimal percServ = Math.Round(bServico.quantity / (qtdCHIC * 100.00m), 2);
                                    decimal percServ = 100;
                                    if (!bServico.comment_1.Trim().Equals(""))
                                        //if (Decimal.TryParse(bServico.comment_1.Trim().Substring(7, 3).Replace("%", ""), out percServ))
                                        if (Decimal.TryParse(bServico.comment_1.Trim().Substring(14, 4).Replace("%", ""), out percServ))
                                            percServ = percServ;

                                    if (percServ == 0) percServ = 100;

                                    if (percServ != serv.PercAplicacaoServico)
                                    {
                                        serv.PercAplicacaoServico = percServ;
                                        if (motivo == "Dados Atualizados: ") motivo = motivo + " % do serviço: "
                                            + produtoApoloS.ProdNomeAlt1;
                                        else motivo = motivo + ", % do serviço: "
                                            + produtoApoloS.ProdNomeAlt1;
                                    }

                                    #endregion

                                    #region Tipo de Cobrança

                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, bServico.bookkey);

                                    if (ciDT.Count > 0)
                                    {
                                        CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                        int tipoCobrancaCHIC = 0;
                                        if (ciR.cobvcsv.Trim() == "Bonificação") tipoCobrancaCHIC = 1;
                                        else if (ciR.cobvcsv.Trim() == "Cliente Envia") tipoCobrancaCHIC = 2;

                                        if (tipoCobrancaCHIC != serv.Bonificada)
                                        {
                                            serv.Bonificada = tipoCobrancaCHIC;
                                            if (motivo == "Dados Atualizados: ") motivo = motivo
                                                + " tipo de cobrança do serviço: "
                                                + produtoApoloS.ProdNomeAlt1;
                                            else motivo = motivo + ", tipo de cobrança do serviço: "
                                                + produtoApoloS.ProdNomeAlt1;
                                        }
                                    }
                                    else
                                        serv.Bonificada = 0;

                                    #endregion

                                    valorTotalServicos = bServico.price;
                                    percServicos = percServ;
                                }
                                else
                                {
                                    // Se serviço não existe no CHIC, será deletado do Web
                                    hlbappService.Servicos_Pedido_Venda.DeleteObject(serv);
                                }
                            }

                            #region Insere LOG - Servico_Pedido_Venda - DESATIVADO

                            //if (serv != null)
                            //{
                            //    LOG_Servicos_Pedido_Venda logServ = new LOG_Servicos_Pedido_Venda();
                            //    logServ.IDPedidoVenda = serv.IDPedidoVenda;
                            //    logServ.ProdCodEstr = serv.ProdCodEstr;
                            //    logServ.PercAplicacaoServico = serv.PercAplicacaoServico;
                            //    logServ.DataHora = DateTime.Now;
                            //    logServ.Operacao = "Importado p/ WEB";
                            //    logServ.IDServPedVenda = serv.ID;
                            //    logServ.IDLogPedidoVenda = logPV.ID;
                            //    logServ.PrecoUnitario = serv.PrecoUnitario;
                            //    logServ.Bonificada = serv.Bonificada;

                            //    hlbappService.LOG_Servicos_Pedido_Venda.AddObject(logServ);
                            //}

                            #endregion
                        }
                        else
                        {
                            #region Se existe Serviço no CHIC e não no WEB, inserir no WEB

                            serv = new Servicos_Pedido_Venda();

                            foreach (var servicoCHIC in listaServicosCHIC)
                            {
                                PRODUTO1 servicoApolo = apolo.PRODUTO1
                                    .Where(w => w.USERCodigoCHIC == servicoCHIC.item)
                                    .FirstOrDefault();

                                if (servicoApolo != null)
                                {
                                    PRODUTO servicoApoloObj = apolo.PRODUTO
                                        .Where(w => w.ProdCodEstr == servicoApolo.ProdCodEstr)
                                        .FirstOrDefault();

                                    serv.IDPedidoVenda = pedVenda.ID;
                                    serv.ProdCodEstr = servicoApolo.ProdCodEstr;
                                    serv.PrecoUnitario = servicoCHIC.price;
                                    serv.Bonificada = 0;

                                    PRODUTO1 produtoApolo1S = apolo.PRODUTO1
                                        .Where(w => w.ProdCodEstr == serv.ProdCodEstr)
                                        .FirstOrDefault();

                                    CHICDataSet.bookedRow bServico = bDTCommercial
                                    .Where(w => w.item == produtoApolo1S.USERCodigoCHIC).FirstOrDefault();

                                    decimal percServ = 100;
                                    if (!bServico.comment_1.Trim().Equals(""))
                                        //if (Decimal.TryParse(bServico.comment_1.Trim().Substring(7, 3).Replace("%", ""), out percServ))
                                        if (Decimal.TryParse(bServico.comment_1.Trim().Substring(14, 3).Replace("%", ""), out percServ))
                                            percServ = percServ;

                                    if (percServ == 0) percServ = 100;

                                    serv.PercAplicacaoServico = percServ;

                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, servicoCHIC.bookkey);
                                    CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                    if (ciR != null)
                                    {
                                        if (ciR.cobvcsv.Equals("Bonificação")) serv.Bonificada = 1;
                                        else if (ciR.cobvcsv.Equals("Cliente Envia")) serv.Bonificada = 2;
                                    }

                                    if (motivo == "Dados Atualizados: ") motivo = motivo
                                        + " inserido serviço: "
                                        + servicoApoloObj.ProdNomeAlt1;
                                    else motivo = motivo + ", inserido serviço: "
                                        + servicoApoloObj.ProdNomeAlt1;

                                    valorTotalServicos = servicoCHIC.price;

                                    hlbappService.Servicos_Pedido_Venda.AddObject(serv);

                                    #region Insere LOG - Servico_Pedido_Venda - DESATIVADO

                                    //if (serv != null)
                                    //{
                                    //    LOG_Servicos_Pedido_Venda logServ = new LOG_Servicos_Pedido_Venda();
                                    //    logServ.IDPedidoVenda = serv.IDPedidoVenda;
                                    //    logServ.ProdCodEstr = serv.ProdCodEstr;
                                    //    logServ.PercAplicacaoServico = serv.PercAplicacaoServico;
                                    //    logServ.DataHora = DateTime.Now;
                                    //    logServ.Operacao = "Importado p/ WEB";
                                    //    logServ.IDServPedVenda = serv.ID;
                                    //    logServ.IDLogPedidoVenda = logPV.ID;
                                    //    logServ.PrecoUnitario = serv.PrecoUnitario;
                                    //    logServ.Bonificada = serv.Bonificada;

                                    //    hlbappService.LOG_Servicos_Pedido_Venda.AddObject(logServ);
                                    //}

                                    #endregion
                                }
                            }

                            #endregion
                        }

                        //logPV.Motivo = motivo;
                        hlbappService.SaveChanges();

                        #endregion

                        #region Dados dos Itens que existem no WEB

                        var listaItensWEB = hlbappService.Item_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID
                                && w.OrderNoCHIC != "Cancelado")
                            .ToList();

                        foreach (var item in listaItensWEB)
                        {
                            #region Carrega dados do Item do CHIC

                            string variety = "";
                            vartablTableAdapter vartabl = new vartablTableAdapter();
                            CHICDataSet.vartablDataTable vartablDT =
                                new CHICDataSet.vartablDataTable();

                            vartabl.FillByDesc(vartablDT, item.ProdCodEstr.Replace(" - Ovos", "").Replace(" - Machos", "").Replace(" - Macho", ""));
                            variety = vartablDT[0].variety;
                            string linhagem = vartablDT[0].desc.Trim();

                            CHICDataSet.bookedRow bRowVendido = bDTCommercial
                                .Where(w => iDT.Any(a => a.item_no == w.item
                                        && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                        && a.variety == variety)
                                    && !w.alt_desc.Contains("Extra"))
                                .FirstOrDefault();

                            #endregion

                            if (bRowVendido != null)
                            {
                                #region Ajusta Datas

                                DateTime caldateWeb = DateTime.Today;
                                if (!item.ProdCodEstr.Contains("Ovos"))
                                {
                                    if (uf.UfRegGeog == "Norte" || uf.UfRegGeog == "Nordeste")
                                        caldateWeb = item.DataEntregaInicial.AddDays(-23);
                                    else
                                        caldateWeb = item.DataEntregaInicial.AddDays(-22);
                                }
                                else
                                    caldateWeb = item.DataEntregaInicial;

                                if (bRowVendido.cal_date != caldateWeb)
                                {
                                    DateTime caldateCHIC = DateTime.Today;
                                    if (!item.ProdCodEstr.Contains("Ovos"))
                                    {
                                        if (uf.UfRegGeog == "Norte" || uf.UfRegGeog == "Nordeste")
                                            caldateCHIC = bRowVendido.cal_date.AddDays(23);
                                        else
                                            caldateCHIC = bRowVendido.cal_date.AddDays(22);
                                    }
                                    else
                                        caldateCHIC = bRowVendido.cal_date;

                                    item.DataEntregaInicial = caldateCHIC;
                                    item.DataEntregaFinal = caldateCHIC;
                                    if (motivo == "Dados Atualizados: ")
                                        motivo = motivo + " data do nascimento / incubação do item " + linhagem;
                                    else motivo = motivo + ", data do nascimento / incubação do item " + linhagem;
                                }

                                #endregion

                                #region Qtd Vendida

                                decimal qtdOvosVendidosCHIC = bDTCommercial
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                            && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                            && a.variety == variety)
                                        && !w.alt_desc.Contains("Extra"))
                                    .Sum(s => s.quantity);

                                if (qtdOvosVendidosCHIC != item.QtdeLiquida)
                                {
                                    item.QtdeLiquida = Convert.ToInt32(qtdOvosVendidosCHIC);
                                    item.ValorTotal = Math.Round(
                                        Convert.ToDecimal(item.QtdeLiquida * item.PrecoPinto), 2);
                                    if (motivo == "Dados Atualizados: ")
                                        motivo = motivo + " qtde. líquida do item " + linhagem;
                                    else motivo = motivo + ", qtde. líquida do item " + linhagem;
                                }

                                #endregion

                                #region Qtd Bonificada

                                decimal qtdOvosBonifCHIC = bDTCommercial
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                            && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                            && a.variety == variety)
                                        && w.alt_desc.Contains("Extra"))
                                    .Sum(s => s.quantity);

                                if (qtdOvosBonifCHIC != item.QtdeBonificada)
                                {
                                    CHICDataSet.bookedRow bRowBonificacao = bDTCommercial
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                                && a.variety == variety)
                                            && w.alt_desc.Contains("Extra"))
                                        .FirstOrDefault();

                                    item.QtdeBonificada = Convert.ToInt32(qtdOvosBonifCHIC);
                                    //decimal percBonificado =
                                    //    (Convert.ToDecimal(item.QtdeBonificada) / (item.QtdeLiquida * 100.00m)) * 100.00m;
                                    decimal percBonificado = 2;
                                    if (bRowBonificacao != null)
                                        if (!bRowBonificacao.alt_desc.Trim().Equals(""))
                                            if (Decimal.TryParse(bRowBonificacao.alt_desc.Trim().Substring(0, 4), out percBonificado))
                                                percBonificado = percBonificado;

                                    item.PercBonificacao = percBonificado;
                                    if (motivo == "Dados Atualizados: ")
                                        motivo = motivo + " qtde. e % de bonificação do item " + linhagem;
                                    else motivo = motivo + ", qtde. e % de bonificação do item " + linhagem;
                                }

                                #endregion

                                #region Se existe a Reposição no CHIC e não do WEB e estiver amarrada ao pedido de venda, atualizar no WEB

                                if (icRowReposicao != null)
                                {
                                    CHICDataSet.bookedDataTable bDTReposicao = new CHICDataSet.bookedDataTable();
                                    bTACommercial.FillByOrderNo(bDTReposicao, icRowReposicao.orderno);
                                    CHICDataSet.bookedRow bRowReposicao = bDTReposicao
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H")
                                                    || a.form.Substring(0, 1).Equals("D"))
                                                    && a.variety == variety)
                                            && !w.alt_desc.Contains("Extra")).FirstOrDefault();

                                    if (bRowReposicao != null)
                                    {
                                        Item_Pedido_Venda existeItemReposicao = hlbappService.Item_Pedido_Venda
                                            .Where(w => w.OrderNoCHICReposicao == icRowReposicao.orderno
                                                && w.ProdCodEstr == item.ProdCodEstr)
                                            .FirstOrDefault();

                                        if (existeItemReposicao == null)
                                        {
                                            item.OrderNoCHICReposicao = icRowReposicao.orderno;
                                            item.QtdeReposicao = Convert.ToInt32(bRowReposicao.quantity);

                                            if (motivo == "Dados Atualizados: ")
                                                motivo = motivo + " relacionada a reposição da linhagem " + linhagem;
                                            else motivo = motivo + ", relacionada a reposição da linhagem " + linhagem;
                                        }
                                    }
                                }

                                #endregion

                                #region Preço do Pinto

                                if (bRowVendido.price != item.PrecoPinto)
                                {
                                    item.PrecoPinto = bRowVendido.price;
                                    //item.ValorTotal = Math.Round(
                                    //    Convert.ToDecimal(item.QtdeLiquida * item.PrecoPinto), 2);
                                    if (motivo == "Dados Atualizados: ")
                                        motivo = motivo + " preço do pinto do item " + linhagem;
                                    else motivo = motivo + ", preço do pinto do item " + linhagem;
                                }

                                #endregion

                                #region Atualiza Valor Total e Preco Unitário

                                int qtdeReposicao = 0;
                                if (item.TipoReposicao == "Acerto Comercial")
                                    qtdeReposicao = Convert.ToInt32(item.QtdeReposicao);

                                item.ValorTotal = (item.QtdeLiquida * item.PrecoPinto)
                                    + ((item.QtdeLiquida + item.QtdeBonificada + qtdeReposicao) * valorTotalVacinas)
                                    + (Math.Round(Convert.ToDecimal((item.QtdeLiquida + item.QtdeBonificada
                                        + qtdeReposicao)
                                        * (percServicos / 100.00m)), 0) * valorTotalServicos);

                                //if (motivo == "Dados Atualizados: ")
                                //    motivo = motivo + " valor total do item " + linhagem;
                                //else motivo = motivo + ", valor total do item " + linhagem;

                                int qtdeCalculoValorTotal = 1;
                                if (item.QtdeLiquida > 0)
                                    qtdeCalculoValorTotal = item.QtdeLiquida;
                                else if (item.QtdeReposicao > 0)
                                    qtdeCalculoValorTotal = Convert.ToInt32(item.QtdeReposicao);
                                item.PrecoUnitario = item.ValorTotal / qtdeCalculoValorTotal;

                                //if (motivo == "Dados Atualizados: ")
                                //    motivo = motivo + " preço unitário do item " + linhagem;
                                //else motivo = motivo + ", preço unitário do item " + linhagem;


                                #endregion

                                #region Campos Customizados do Item

                                custitemTableAdapter ciTA = new custitemTableAdapter();
                                CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                ciTA.FillByBookkey(ciDT, bRowVendido.bookkey);

                                if (ciDT.Count > 0)
                                {
                                    if (item.Sobra != (ciDT[0].sobra.Trim() == "Sim" ? 1 : 0))
                                    {
                                        item.Sobra = (ciDT[0].sobra.Trim() == "Sim" ? 1 : 0);
                                        if (motivo == "Dados Atualizados: ")
                                            motivo = motivo + " item " + linhagem + " marcado como Sobra";
                                        else motivo = motivo + ", item " + linhagem + " marcado como Sobra";
                                    }
                                }

                                #endregion

                                #region Insere LOG - Item_Ped_Venda - DESATIVADO

                                //LOG_Item_Pedido_Venda logItemPV = new LOG_Item_Pedido_Venda();
                                //logItemPV.IDPedidoVenda = item.IDPedidoVenda;
                                //logItemPV.Sequencia = item.Sequencia;
                                //logItemPV.ProdCodEstr = item.ProdCodEstr;
                                //logItemPV.DataEntregaInicial = item.DataEntregaInicial;
                                //logItemPV.DataEntregaFinal = item.DataEntregaFinal;
                                //logItemPV.QtdeLiquida = item.QtdeLiquida;
                                //logItemPV.PercBonificacao = item.PercBonificacao;
                                //logItemPV.QtdeBonificada = item.QtdeBonificada;
                                //logItemPV.QtdeReposicao = item.QtdeReposicao;
                                //logItemPV.PrecoUnitario = item.PrecoUnitario;
                                //logItemPV.DataHora = DateTime.Now;
                                //logItemPV.Operacao = "Importado p/ WEB";
                                //logItemPV.IDItPedVenda = item.ID;
                                //logItemPV.IDLogPedidoVenda = logPV.ID;
                                //logItemPV.OrderNoCHIC = item.OrderNoCHIC;
                                //logItemPV.OrderNoCHICReposicao = item.OrderNoCHICReposicao;
                                //logItemPV.PrecoPinto = item.PrecoPinto;
                                //logItemPV.TipoReposicao = item.TipoReposicao;
                                //logItemPV.ValorTotal = item.ValorTotal;

                                //hlbappService.LOG_Item_Pedido_Venda.AddObject(logItemPV);

                                #endregion
                            }
                            else
                            {
                                #region Itens que tem no WEB e não no CHIC, serão deletados

                                #region Carrega Dados Se Existir Reposição

                                if (icRowReposicao != null)
                                {
                                    CHICDataSet.bookedDataTable bDTReposicao = new CHICDataSet.bookedDataTable();
                                    bTACommercial.FillByOrderNo(bDTReposicao, icRowReposicao.orderno);
                                    CHICDataSet.bookedRow bRowReposicao = bDTReposicao
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H") 
                                                    || a.form.Substring(0, 1).Equals("D"))
                                                    && a.variety == variety)
                                            && !w.alt_desc.Contains("Extra")).FirstOrDefault();

                                    if (bRowReposicao != null)
                                    {
                                        string corpoEmail = "";

                                        #region Deleta item da reposição ou a reposição inteira se tiver só ele e envia e-mail para a programação avisando

                                        int existeOutros = bDTReposicao
                                            .Where(w => iDT.Any(a => a.item_no == w.item
                                                    && (a.form.Substring(0, 1).Equals("H")
                                                        || a.form.Substring(0, 1).Equals("D")))
                                                    && w.book_id != bRowReposicao.book_id
                                                && !w.alt_desc.Contains("Extra")).Count();

                                        custitemTableAdapter ciTA = new custitemTableAdapter();

                                        if (existeOutros > 0)
                                        {
                                            corpoEmail = "Prezado(s)," + (char)13 + (char)10
                                                + (char)13 + (char)10
                                                + "Como o item " + linhagem + " não existe mais no pedido do CHIC " 
                                                + oR.orderno + " e existe a reposição no CHIC " 
                                                + bRowReposicao.orderno + ", foi excluído item " + linhagem
                                                + " da reposição. Segue abaixo os dados do item excluído: "
                                                + (char)13 + (char)10 + (char)13 + (char)10
                                                + "Linhagem: " + linhagem + (char)13 + (char)10
                                                + "Quantidade: " + bRowReposicao.quantity + (char)13 + (char)10
                                                + (char)13 + (char)10 + (char)13 + (char)10
                                                + "Sendo assim, caso essa reposição deva existir, "
                                                + "gerar um novo pedido no CHIC para a mesma. "
                                                + (char)13 + (char)10 + (char)13 + (char)10
                                                + "SISTEMA WEB";

                                            ciTA.DeleteByBookKey(bRowReposicao.bookkey);
                                            bTACommercial.DeleteByBookID(bRowReposicao.book_id);
                                        }
                                        else
                                        {
                                            corpoEmail = "Prezado(s)," + (char)13 + (char)10
                                                + (char)13 + (char)10
                                                + "Como o item " + linhagem + " não existe mais no pedido do CHIC "
                                                + oR.orderno + " e existe a reposição no CHIC "
                                                + bRowReposicao.orderno + ", foi excluído o pedido de reposição. "
                                                + "Segue abaixo os dados da reposição excluída: "
                                                + (char)13 + (char)10 + (char)13 + (char)10
                                                + "Linhagem: " + linhagem + (char)13 + (char)10
                                                + "Quantidade: " + bRowReposicao.quantity + (char)13 + (char)10
                                                + (char)13 + (char)10 + (char)13 + (char)10
                                                + "Sendo assim, caso essa reposição deva existir, "
                                                + "gerar um novo pedido no CHIC para a mesma. "
                                                + (char)13 + (char)10 + (char)13 + (char)10
                                                + "SISTEMA WEB";

                                            foreach (var itemReposicao in bDTReposicao)
                                            {
                                                ciTA.DeleteByBookKey(itemReposicao.bookkey);
                                            }
                                            bTACommercial.DeleteByOrderNO(icRowReposicao.orderno);
                                            icTA.DeleteByOrderNo(icRowReposicao.orderno);
                                            oTACommercial.DeleteByOrderNo(icRowReposicao.orderno);
                                        }

                                        #endregion

                                        item.QtdeReposicao = 0;
                                        item.OrderNoCHICReposicao = "Cancelado";

                                        #region Envia E-mail para Programação avisando da deleção do item

                                        CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                                        salesman.FillByCod(sDT, pedVenda.Vendedor);
                                        string enderecoEmail = "";
                                        if (sDT[0].inv_comp.Trim() == "BR")
                                            enderecoEmail = "programacao@hyline.com.br";
                                        else if (sDT[0].inv_comp.Trim() == "LB")
                                            enderecoEmail = "programacao@ltz.com.br";
                                        else if (sDT[0].inv_comp.Trim() == "HN")
                                            enderecoEmail = "programacao@hnavicultura.com.br";
                                        else if (sDT[0].inv_comp.Trim() == "PL")
                                            enderecoEmail = "programacao@planaltopostura.com.br";
                                        
                                        string empresaApolo = "";
                                        if (sDT[0].inv_comp.Trim().Equals("BR")) empresaApolo = "5";
                                        else if (sDT[0].inv_comp.Trim().Equals("LB")) empresaApolo = "7";
                                        else if (sDT[0].inv_comp.Trim().Equals("HN")) empresaApolo = "14";
                                        else if (sDT[0].inv_comp.Trim().Equals("PL")) empresaApolo = "20";

                                        EnviaConfirmacaoEmail("", enderecoEmail, enderecoEmail, "", "", "", 
                                            corpoEmail, 
                                            "** REPOSIÇÃO DELETADA DO CHIC POR INCONSISTÊNCIA COM O WEB **", 
                                            empresaApolo);

                                        #endregion
                                    }
                                }

                                #endregion

                                item.QtdeLiquida = 0;
                                item.OrderNoCHIC = "Cancelado";

                                if (motivo == "Dados Atualizados: ")
                                    motivo = motivo + " cancelamento do item " + linhagem + " por não existir no CHIC";
                                else motivo = motivo + ", cancelamento do item " + linhagem + " por não existir no CHIC";

                                #endregion
                            }
                        }

                        #endregion

                        #region Dados dos Itens que não existem no WEB, porém existe no CHIC para inserir

                        var listItensCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D")))
                                && !w.alt_desc.Contains("Extra"))
                            .ToList();

                        var listaItensWEBTotal = hlbappService.Item_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID)
                            .ToList();

                        foreach (var item in listItensCHIC)
                        {
                            #region Carrega dados do Item do CHIC

                            vartablTableAdapter vartabl = new vartablTableAdapter();
                            CHICDataSet.vartablDataTable vartablDT =
                                new CHICDataSet.vartablDataTable();
                            vartbl.Fill(vartablDT);
                            CHICDataSet.itemsRow iRow = iDT.Where(w => w.item_no == item.item).FirstOrDefault();
                            CHICDataSet.vartablRow vRow = vartablDT
                                .Where(w => iDT.Any(a => w.variety == a.variety
                                    && a.item_no == item.item))
                                .FirstOrDefault();

                            string linhagem = vRow.desc.Trim();
                            if (iRow.form.Substring(0, 1).Equals("H"))
                                linhagem = linhagem + " - Ovos";

                            #region Verifica Se é macho para inserir e descrição na frente
                            // 21/05/2018 - Solicita por André / Débora, pois estavam confundindo com os pedidos de fêmeas
                            if (iRow.form == "DM")
                                linhagem = linhagem + " - Machos";

                            #endregion

                            #endregion

                            #region Verifica se o item existe na WEB

                            int existeWeb = 0;
                            existeWeb = listaItensWEB.Where(w => w.ProdCodEstr == linhagem).Count();

                            #endregion

                            if (existeWeb == 0)
                            {
                                Item_Pedido_Venda itemNovoWEB = new Item_Pedido_Venda();

                                itemNovoWEB.IDPedidoVenda = pedVenda.ID;
                                itemNovoWEB.ProdCodEstr = linhagem;
                                DateTime dataWeb = DateTime.Today;

                                #region Calcula Data
                                if (!iRow.form.Substring(0, 1).Equals("H"))
                                {
                                    if (uf.UfRegGeog == "Norte" || uf.UfRegGeog == "Nordeste")
                                        dataWeb = item.cal_date.AddDays(23);
                                    else
                                        dataWeb = item.cal_date.AddDays(22);
                                }
                                else
                                    dataWeb = item.cal_date;

                                #endregion

                                itemNovoWEB.DataEntregaInicial = dataWeb;
                                itemNovoWEB.DataEntregaFinal = dataWeb;
                                itemNovoWEB.QtdeLiquida = Convert.ToInt32(item.quantity);

                                #region Qtde. Bonificada

                                decimal qtdOvosBonifCHIC = bDTCommercial
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                            && (a.form.Substring(0, 1).Equals("H") 
                                                    || a.form.Substring(0, 1).Equals("D"))
                                            && a.variety == iRow.variety)
                                        && w.alt_desc.Contains("Extra"))
                                    .Sum(s => s.quantity);

                                itemNovoWEB.QtdeBonificada = Convert.ToInt32(qtdOvosBonifCHIC);

                                #endregion

                                #region % Bonificada

                                CHICDataSet.bookedRow bRowBonificacao = bDTCommercial
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                                && a.variety == iRow.variety)
                                            && w.alt_desc.Contains("Extra"))
                                        .FirstOrDefault();

                                decimal percBonificado = 2;
                                if (bRowBonificacao != null)
                                    if (!bRowBonificacao.alt_desc.Trim().Equals(""))
                                        if (Decimal.TryParse(bRowBonificacao.alt_desc.Trim().Substring(0, 4), out percBonificado))
                                            percBonificado = percBonificado;

                                itemNovoWEB.PercBonificacao = percBonificado;

                                #endregion

                                itemNovoWEB.QtdeReposicao = 0;
                                itemNovoWEB.PrecoPinto = item.price;

                                #region Atualiza Valor Total e Preco Unitário

                                int qtdeReposicao = Convert.ToInt32(itemNovoWEB.QtdeReposicao);

                                itemNovoWEB.ValorTotal = (itemNovoWEB.QtdeLiquida * itemNovoWEB.PrecoPinto)
                                    + ((itemNovoWEB.QtdeLiquida + itemNovoWEB.QtdeBonificada + qtdeReposicao) 
                                        * valorTotalVacinas)
                                    + (Math.Round(Convert.ToDecimal((itemNovoWEB.QtdeLiquida 
                                        + itemNovoWEB.QtdeBonificada + qtdeReposicao)
                                        * (percServicos / 100.00m)), 0) * valorTotalServicos);

                                int qtdeCalculoValorTotal = 1;
                                if (itemNovoWEB.QtdeLiquida > 0)
                                    qtdeCalculoValorTotal = itemNovoWEB.QtdeLiquida;
                                else if (itemNovoWEB.QtdeReposicao > 0)
                                    qtdeCalculoValorTotal = Convert.ToInt32(itemNovoWEB.QtdeReposicao);
                                itemNovoWEB.PrecoUnitario = itemNovoWEB.ValorTotal / qtdeCalculoValorTotal;

                                //if (motivo == "Dados Atualizados: ")
                                //    motivo = motivo + " preço unitário do item " + linhagem;
                                //else motivo = motivo + ", preço unitário do item " + linhagem;

                                itemNovoWEB.Sequencia = listaItensWEBTotal.Count + 1;
                                itemNovoWEB.OrderNoCHIC = item.orderno;
                                itemNovoWEB.Alterado = 0;
                                itemNovoWEB.Importar = 0;

                                #endregion

                                #region Campos Customizados do Item

                                custitemTableAdapter ciTA = new custitemTableAdapter();
                                CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                ciTA.FillByBookkey(ciDT, item.bookkey);

                                if (ciDT.Count > 0)
                                {
                                    itemNovoWEB.Sobra = (ciDT[0].sobra.Trim() == "Sim" ? 1 : 0);
                                }

                                #endregion

                                #region Carrega Dados Se Existir Reposição

                                if (icRowReposicao != null)
                                {
                                    CHICDataSet.bookedDataTable bDTReposicao = new CHICDataSet.bookedDataTable();
                                    bTACommercial.FillByOrderNo(bDTReposicao, icRowReposicao.orderno);
                                    CHICDataSet.bookedRow bRowReposicao = bDTReposicao
                                        .Where(w => w.item == item.item).FirstOrDefault();

                                    if (bRowReposicao != null)
                                    {
                                        itemNovoWEB.QtdeReposicao = Convert.ToInt32(bRowReposicao.quantity);
                                        itemNovoWEB.OrderNoCHICReposicao = bRowReposicao.orderno;
                                        if (bRowReposicao.comment_1.Contains("Acerto"))
                                            itemNovoWEB.TipoReposicao = "Acerto Comercial";
                                        else if (bRowReposicao.comment_1.Contains("Mortalidade"))
                                            itemNovoWEB.TipoReposicao = "Mortalidade";
                                    }
                                }

                                #endregion

                                hlbappService.Item_Pedido_Venda.AddObject(itemNovoWEB);

                                if (motivo == "Dados Atualizados: ")
                                    motivo = motivo + " adicionado item " + linhagem;
                                else motivo = motivo + ", adicionado item " + linhagem;
                            }
                        }

                        #endregion

                        //logPV.Motivo = motivo;

                        #region Envia E-mail p/ Representante / Vendedor Avisando (Somente Nutribastos para teste)

                        if (pedVenda.Vendedor.Equals("000083") && motivo != "Dados Atualizados: ")
                        {
                            string caminho = "";

                            CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                            salesman.FillByCod(sDT, pedVenda.Vendedor);
                            string nome = sDT[0].salesman.Trim();
                            string enderecoEmail = sDT[0].email.Trim();
                            string empresaApolo = "";
                            if (sDT[0].inv_comp.Trim().Equals("BR")) empresaApolo = "5";
                            else if (sDT[0].inv_comp.Trim().Equals("LB")) empresaApolo = "7";
                            else if (sDT[0].inv_comp.Trim().Equals("HN")) empresaApolo = "14";
                            else if (sDT[0].inv_comp.Trim().Equals("PL")) empresaApolo = "20";

                            string corpoEmail = "Prezado(s)," + (char)13 + (char)10
                                + (char)13 + (char)10
                                + "Foram realizadas as seguintes alterações no pedido CHIC Nº" + orderNoErro
                                + " - ID WEB " + pedVenda.ID.ToString() + "."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + motivo
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "Qualquer dúvida, entrar em contato o Gerente Comercial para confirmar "
                                + " a alteração." + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA WEB";

                            string assunto = "**** " + ("Importado p/ WEB").ToUpper() + " - CHIC " + orderNoErro
                                + " / ID " + pedVenda.ID.ToString() + " ****";

                            EnviaConfirmacaoEmail(caminho, enderecoEmail, nome, "", "", "", corpoEmail,
                                assunto, empresaApolo);
                        }

                        #endregion

                        #region Insere LOG se existir motivo

                        if (motivo != "Dados Atualizados: ")
                            InsereLOGPVWeb(pedVenda.ID, "CHIC - " + usuarioCHIC, "Importado p/ WEB",
                                motivo);

                        #endregion
                    }
                }

                hlbappService.SaveChanges();

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Atualizar CHIC com WEB - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                #region Envio de E-mail

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ WEB - VENDIDOS ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Paulo Alves";
                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                string corpoEmail = "";
                string innerException = "";

                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }

                corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o WEB: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "Linha do Erro: " + erro.ToString() + (char)13 + (char)10
                    + "Número do Pedido CHIC: " + orderNoErro + (char)13 + (char)10
                    + "Linha do Erro: " + linenum.ToString() + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 1: " + ex.Message + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 2: " + innerException;

                email.WorkFlowEmailCorpo = corpoEmail;
                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                return retorno;
            }
        }

        public string AtualizaPedidosReposicaoWEBxCHIC()
        {
            string orderNoErro = "";
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            try
            {
                #region Atualiza Pedidos de Reposicao do CHIC para o WEB

                #region Carrega variaveis e lista dos itens a serem verificados

                string motivo = "";
                string usuarioCHIC = "";
                DateTime dataFiltro = DateTime.Today.AddDays(-30);
                //DateTime dataFiltro = Convert.ToDateTime("05/12/2017");
                DateTime dataInicioModeloNovo = Convert.ToDateTime("03/12/2017");

                var listaWEB = hlbappService.Item_Pedido_Venda
                    .Where(a => a.DataEntregaFinal >= dataFiltro
                        && hlbappService.Pedido_Venda.Any(p => p.ID == a.IDPedidoVenda
                            && (p.Status.Contains("Importado")
                                    || p.Status.Contains("Reprovad")))
                        && ((a.OrderNoCHICReposicao != null && a.OrderNoCHICReposicao != ""
                            && a.OrderNoCHIC != "Cancelado"
                            //&& a.OrderNoCHIC == "90060"
                            ))
                        && a.DataEntregaFinal >= dataInicioModeloNovo)
                    .GroupBy(g =>
                        new
                        {
                            g.OrderNoCHICReposicao,
                            g.IDPedidoVenda
                        })
                    .Select(s =>
                        new
                        {
                            s.Key.OrderNoCHICReposicao,
                            s.Key.IDPedidoVenda
                        })
                    .ToList();

                #endregion

                foreach (var pedido in listaWEB)
                {
                    motivo = "Dados Atualizados: ";

                    #region Carrega objetos

                    string orderNo = pedido.OrderNoCHICReposicao;
                    orderNoErro = pedido.OrderNoCHICReposicao;

                    if (pedido.OrderNoCHICReposicao == "64956")
                        orderNoErro = pedido.OrderNoCHICReposicao;

                    Pedido_Venda pedVenda = hlbappService.Pedido_Venda.Where(w => w.ID == pedido.IDPedidoVenda)
                        .FirstOrDefault();

                    ordersTableAdapter oTACommercial = new ordersTableAdapter();
                    CHICDataSet.ordersDataTable oDTCommercial = new CHICDataSet.ordersDataTable();
                    oTACommercial.FillByOrderNo(oDTCommercial, orderNo);
                    CHICDataSet.ordersRow oR = oDTCommercial.FirstOrDefault();

                    #endregion

                    if (oR != null)
                    {
                        #region Carrega Itens do CHIC

                        ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                            new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                        CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                        bTACommercial.FillByOrderNo(bDTCommercial, orderNo);

                        //usuarioCHIC = bDTCommercial.Max(m => m.modifdby).Trim();
                        usuarioCHIC = "";
                        if (bDTCommercial.Count > 0)
                            usuarioCHIC = bDTCommercial.OrderByDescending(o => o.datemodi)
                                .FirstOrDefault().modifdby.Trim();

                        itemsTableAdapter iTA = new itemsTableAdapter();
                        CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                        iTA.Fill(iDT);

                        var listaVacinasCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && a.form == "VC")).ToList();

                        var listaServicosCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && a.form == "SV")).ToList();

                        #endregion

                        #region Carrega Dados Localização Cliente

                        string custNo = oR.cust_no.Trim();
                        ENTIDADE entidade = apolo.ENTIDADE
                            .Where(e1 => e1.EntCod == custNo).First();

                        CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                        UNID_FEDERACAO uf = apolo.UNID_FEDERACAO
                            .Where(w => w.UfSigla == cidade.UfSigla && w.PaisSigla == cidade.PaisSigla)
                            .FirstOrDefault();

                        #endregion
                        
                        #region Status

                        if (pedVenda.Status.Contains("Reprovad"))
                        {
                            var listaItens = hlbappService.Item_Pedido_Venda
                                .Where(w => w.IDPedidoVenda == pedVenda.ID).ToList();

                            foreach (var itemPV in listaItens)
                            {
                                itemPV.Importar = 0;
                            }

                            int existeCancelado = listaItens
                                .Where(w => w.IDPedidoVenda == pedVenda.ID
                                && w.OrderNoCHIC == "Cancelado").Count();

                            int existeNaoCancelado = listaItens
                                .Where(w => w.IDPedidoVenda == pedVenda.ID
                                && w.OrderNoCHIC != "Cancelado").Count();

                            string status = "Importado Total";
                            if (existeCancelado > 0 && existeNaoCancelado > 0)
                                status = "Importado Parcial";

                            //pedVenda.Status = status;
                            //if (motivo == "Dados Atualizados: ")
                            //    motivo = motivo + " status alterado para " + status + " por motivo de reprovação";
                            //else motivo = motivo + ", status alterado para " + status + " por motivo de reprovação";
                        }

                        #endregion

                        #region Insere LOG - Pedido_Venda - DESATIVADO

                        //LOG_Pedido_Venda logPV = new LOG_Pedido_Venda();

                        //logPV = new LOG_Pedido_Venda();
                        //logPV.DataPedido = pedVenda.DataPedido;
                        ////logPV.Usuario = "Serviço";
                        //logPV.Usuario = "CHIC - " + usuarioCHIC;
                        //logPV.DataHora = DateTime.Now;
                        //logPV.CodigoCliente = pedVenda.CodigoCliente;
                        //logPV.OvosBrasil = pedVenda.OvosBrasil;
                        //logPV.CondicaoPagamento = pedVenda.CondicaoPagamento;
                        //logPV.Observacoes = pedVenda.Observacoes;
                        //logPV.Vendedor = pedVenda.Vendedor;
                        //logPV.Status = pedVenda.Status;
                        //logPV.Operacao = "Importado p/ WEB";
                        //logPV.IDPedidoVenda = pedVenda.ID;
                        ////logPV.Motivo = "Atualização da Qtde. Líquida do CHIC p/ WEB";

                        //hlbappService.LOG_Pedido_Venda.AddObject(logPV);
                        //hlbappService.SaveChanges();

                        #endregion

                        #region Calcula Valor das Vacinas

                        decimal valorTotalVacinas = 0;

                        Vacinas_Primaria_Pedido_Venda vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                        if (vacPrimObj != null)
                        {
                            #region Calcula Vacina Primária

                            #region Carrega Dados da Vacina no Apolo

                            PRODUTO1 produtoApolo1 = apolo.PRODUTO1
                                .Where(w => w.ProdCodEstr == vacPrimObj.ProdCodEstr)
                                .FirstOrDefault();

                            PRODUTO produtoApolo = apolo.PRODUTO
                                .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr)
                                .FirstOrDefault();

                            #endregion

                            // 16/12/2019 - Alteração da Vacina 165 que virou secundária para a 909 que é a nova primária de Rispens
                            if ((bDTCommercial.Where(w => w.item.Equals("165")).Count() > 0)
                                &&
                                (produtoApolo1.USERCodigoCHIC == "909"))
                            {
                                CHICDataSet.bookedRow bVacinaPrimaria909 = bDTCommercial.Where(w => w.item == "165").FirstOrDefault();
                                bTACommercial.UpdateItemNo("909", bVacinaPrimaria909.book_id);
                                bTACommercial.FillByOrderNo(bDTCommercial, orderNo);
                                listaVacinasCHIC = bDTCommercial
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                        && a.form == "VC")).ToList();
                            }

                            if (produtoApolo1 != null)
                            {
                                #region Verifica se é Vaxxitek

                                bool temVaxxitek = false;
                                if (produtoApolo != null)
                                    if (produtoApolo.ProdNomeAlt1.Contains("VAXXITEK"))
                                        temVaxxitek = true;

                                #endregion

                                CHICDataSet.bookedRow bVacinaPrimaria = bDTCommercial
                                    .Where(w => ((w.item == produtoApolo1.USERCodigoCHIC && !temVaxxitek)
                                        || (w.item == produtoApolo1.USERCodigoCHIC && produtoApolo1.USERCodigoCHIC == "161" && temVaxxitek))).FirstOrDefault();

                                if (bVacinaPrimaria != null)
                                {
                                    valorTotalVacinas = bVacinaPrimaria.price;
                                }
                                else
                                {
                                    #region Altera a Vacina Primária para ficar igual a do CHIC

                                    //int existeVacinaPrimaria = 0;
                                    foreach (var vacinaCHIC in listaVacinasCHIC)
                                    {
                                        PRODUTO1 vacinaPrimariaApolo = apolo.PRODUTO1
                                            .Where(w => apolo.PROD_GRUPO_SUBGRUPO
                                                .Any(a => w.ProdCodEstr == a.ProdCodEstr
                                                    && a.GrpProdCod == "041" && a.SubGrpProdCod == "042")
                                                && ((w.USERCodigoCHIC == vacinaCHIC.item && !temVaxxitek) ||
                                                (w.USERCodigoCHIC == vacinaCHIC.item && vacinaCHIC.item == "161" && temVaxxitek)))
                                            .FirstOrDefault();

                                        if (vacinaPrimariaApolo != null)
                                        {
                                            valorTotalVacinas = vacinaCHIC.price;
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            #region Calcula Vacinas Secundárias

                            var listaVacSec = hlbappService.Vacinas_Secundaria_Pedido_Venda
                                .Where(w => w.IDVacPrimPedVenda == vacPrimObj.ID).ToList();

                            foreach (var vacSec in listaVacSec)
                            {
                                #region Carrega Dados da Vacina no Apolo

                                produtoApolo1 = null;
                                produtoApolo1 = apolo.PRODUTO1
                                    .Where(w => w.ProdCodEstr == vacSec.ProdCodEstr)
                                    .FirstOrDefault();

                                produtoApolo = null;
                                produtoApolo = apolo.PRODUTO
                                    .Where(w => w.ProdCodEstr == produtoApolo1.ProdCodEstr)
                                    .FirstOrDefault();

                                #endregion

                                if (produtoApolo1 != null)
                                {
                                    CHICDataSet.bookedRow bVacinaSec = bDTCommercial
                                        .Where(w => w.item == produtoApolo1.USERCodigoCHIC).FirstOrDefault();

                                    if (bVacinaSec != null)
                                    {
                                        valorTotalVacinas = valorTotalVacinas + bVacinaSec.price;
                                    }
                                }
                            }

                            #region Verifica Vacinas Secundárias que tem no CHIC e não no WEB para inserir

                            vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                            if (vacPrimObj != null)
                            {
                                foreach (var vacinaCHIC in listaVacinasCHIC)
                                {
                                    PRODUTO1 vacinaSecApolo = apolo.PRODUTO1
                                        .Where(w => apolo.PROD_GRUPO_SUBGRUPO
                                            .Any(a => w.ProdCodEstr == a.ProdCodEstr
                                                && a.GrpProdCod == "041" && a.SubGrpProdCod == "043")
                                            && w.USERCodigoCHIC == vacinaCHIC.item)
                                        .FirstOrDefault();

                                    if (vacinaSecApolo != null)
                                    {
                                        int existePedido = listaVacSec
                                            .Where(w => w.ProdCodEstr == vacinaSecApolo.ProdCodEstr)
                                            .Count();

                                        if (existePedido == 0)
                                        {
                                            valorTotalVacinas = valorTotalVacinas + vacinaCHIC.price;
                                        }
                                    }
                                }
                            }

                            #endregion

                            #endregion
                        }
                        else
                        {
                            #region Se existe vacina Primária no CHIC e não no WEB, inserir no WEB

                            vacPrimObj = new Vacinas_Primaria_Pedido_Venda();
                            LOG_Vacinas_Primaria_Pedido_Venda logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                            foreach (var vacinaCHIC in listaVacinasCHIC)
                            {
                                PRODUTO1 vacinaPrimariaApolo = apolo.PRODUTO1
                                    .Where(w => apolo.PROD_GRUPO_SUBGRUPO.Any(a => w.ProdCodEstr == a.ProdCodEstr && a.GrpProdCod == "041" && a.SubGrpProdCod == "042")
                                        && (
                                            (apolo.PRODUTO.Any(b => b.ProdCodEstr == w.ProdCodEstr && !b.ProdNomeAlt1.Contains("VAXXITEK")) && w.USERCodigoCHIC == vacinaCHIC.item)
                                            ||
                                            (apolo.PRODUTO.Any(b => b.ProdCodEstr == w.ProdCodEstr && b.ProdNomeAlt1.Contains("VAXXITEK")) && w.USERCodigoCHIC == vacinaCHIC.item
                                                && vacinaCHIC.item == "161")
                                        )
                                     )
                                    .FirstOrDefault();

                                if (vacinaPrimariaApolo != null)
                                {
                                    PRODUTO vacinaPrimariaApoloObj = apolo.PRODUTO
                                        .Where(w => w.ProdCodEstr == vacinaPrimariaApolo.ProdCodEstr)
                                        .FirstOrDefault();

                                    vacPrimObj.ProdCodEstr = vacinaPrimariaApolo.ProdCodEstr;
                                    vacPrimObj.PrecoUnitario = vacinaCHIC.price;
                                    vacPrimObj.Bonificada = 0;

                                    custitemTableAdapter ciTA = new custitemTableAdapter();
                                    CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                    ciTA.FillByBookkey(ciDT, vacinaCHIC.bookkey);
                                    CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                    if (ciR != null)
                                    {
                                        if (ciR.cobvcsv.Equals("Bonificação")) vacPrimObj.Bonificada = 1;
                                        else if (ciR.cobvcsv.Equals("Cliente Envia")) vacPrimObj.Bonificada = 2;
                                    }

                                    if (motivo == "Dados Atualizados: ") motivo = motivo
                                        + " inserida vacina primária: "
                                        + vacinaPrimariaApoloObj.ProdNomeAlt1;
                                    else motivo = motivo + ", inserida vacina primária: "
                                        + vacinaPrimariaApoloObj.ProdNomeAlt1;

                                    valorTotalVacinas = vacinaCHIC.price;

                                    #region Insere LOG - Vacina Primária - DESATIVADO

                                    //logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                                    //logVacPrim.IDPedidoVenda = vacPrimObj.IDPedidoVenda;
                                    //logVacPrim.ProdCodEstr = vacPrimObj.ProdCodEstr;
                                    //logVacPrim.DataHora = DateTime.Now;
                                    //logVacPrim.Operacao = "Importado p/ WEB";
                                    //logVacPrim.IDVacPrimPedVenda = vacPrimObj.ID;
                                    //logVacPrim.IDLogPedidoVenda = logPV.ID;
                                    //logVacPrim.PrecoUnitario = vacPrimObj.PrecoUnitario;
                                    //logVacPrim.Bonificada = vacPrimObj.Bonificada;

                                    //hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                                    //hlbappService.SaveChanges();

                                    #endregion
                                }
                            }

                            #endregion

                            #region Verifica Vacinas Secundárias que tem no CHIC e não no WEB para inserir

                            vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                            if (vacPrimObj != null)
                            {
                                foreach (var vacinaCHIC in listaVacinasCHIC)
                                {
                                    PRODUTO1 vacinaSecApolo = apolo.PRODUTO1
                                        .Where(w => apolo.PROD_GRUPO_SUBGRUPO
                                            .Any(a => w.ProdCodEstr == a.ProdCodEstr
                                                && a.GrpProdCod == "041" && a.SubGrpProdCod == "043")
                                            && w.USERCodigoCHIC == vacinaCHIC.item)
                                        .FirstOrDefault();

                                    if (vacinaSecApolo != null)
                                    {
                                        PRODUTO vacinaSecApoloObj = apolo.PRODUTO
                                            .Where(w => w.ProdCodEstr == vacinaSecApolo.ProdCodEstr)
                                            .FirstOrDefault();

                                        Vacinas_Secundaria_Pedido_Venda vacinaSecP =
                                            new Vacinas_Secundaria_Pedido_Venda();
                                        vacinaSecP.IDVacPrimPedVenda = vacPrimObj.ID;
                                        vacinaSecP.ProdCodEstr = vacinaSecApolo.ProdCodEstr;
                                        vacinaSecP.PrecoUnitario = vacinaCHIC.price;
                                        vacinaSecP.Bonificada = 0;

                                        custitemTableAdapter ciTA = new custitemTableAdapter();
                                        CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                        ciTA.FillByBookkey(ciDT, vacinaCHIC.bookkey);
                                        CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                        if (ciR != null)
                                        {
                                            if (ciR.cobvcsv.Equals("Bonificação")) vacinaSecP.Bonificada = 1;
                                            else if (ciR.cobvcsv.Equals("Cliente Envia")) vacinaSecP.Bonificada = 2;
                                        }

                                        if (motivo == "Dados Atualizados: ") motivo = motivo
                                            + " inserida vacina secundária: "
                                            + vacinaSecApoloObj.ProdNomeAlt1;
                                        else motivo = motivo + ", inserida vacina secundária: "
                                            + vacinaSecApoloObj.ProdNomeAlt1;

                                        hlbappService.Vacinas_Secundaria_Pedido_Venda.AddObject(vacinaSecP);

                                        valorTotalVacinas = valorTotalVacinas + vacinaCHIC.price;

                                        #region Insere LOG - Vacina_Secundaria_Pedido_Venda - DESATIVADO

                                        //LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                                        //logVacSec.IDVacPrimPedVenda = vacinaSecP.IDVacPrimPedVenda;
                                        //logVacSec.Sequencia = vacinaSecP.Sequencia;
                                        //logVacSec.ProdCodEstr = vacinaSecP.ProdCodEstr;
                                        //logVacSec.DataHora = DateTime.Now;
                                        //logVacSec.Operacao = "Importado p/ WEB";
                                        //logVacSec.IDVacSecPedVenda = vacinaSecP.ID;
                                        //logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;
                                        //logVacSec.PrecoUnitario = vacinaSecP.PrecoUnitario;
                                        //logVacSec.Bonificada = vacinaSecP.Bonificada;

                                        //hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        #region Calcula Valor dos Serviços

                        decimal valorTotalServicos = 0;
                        decimal percServicos = 0;

                        Servicos_Pedido_Venda serv = hlbappService.Servicos_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

                        if (serv != null)
                        {
                            #region Carrega Dados do Serviço no Apolo

                            PRODUTO1 produtoApolo1S = apolo.PRODUTO1
                                .Where(w => w.ProdCodEstr == serv.ProdCodEstr)
                                .FirstOrDefault();

                            PRODUTO produtoApoloS = apolo.PRODUTO
                                .Where(w => w.ProdCodEstr == produtoApolo1S.ProdCodEstr)
                                .FirstOrDefault();

                            #endregion

                            if (produtoApolo1S != null)
                            {
                                CHICDataSet.bookedRow bServico = bDTCommercial
                                    .Where(w => w.item == produtoApolo1S.USERCodigoCHIC).FirstOrDefault();

                                if (bServico != null)
                                {
                                    #region % do Serviço

                                    decimal qtdCHIC = bDTCommercial
                                        .Where(w => iDT.Any(a => a.item_no == w.item
                                                && (a.form.Substring(0, 1).Equals("H")
                                                || a.form.Substring(0, 1).Equals("D"))))
                                        .Sum(s => s.quantity);

                                    //decimal percServ = Math.Round(bServico.quantity / (qtdCHIC * 100.00m), 2);
                                    decimal percServ = 100;
                                    if (!bServico.comment_1.Trim().Equals(""))
                                        //if (Decimal.TryParse(bServico.comment_1.Trim().Substring(7, 3)
                                        if (Decimal.TryParse(bServico.comment_1.Trim().Substring(14, 4)
                                                .Replace("%", ""), out percServ))
                                            percServ = percServ;

                                    if (percServ == 0) percServ = 100;

                                    #endregion

                                    valorTotalServicos = bServico.price;
                                    percServicos = percServ;
                                }
                            }
                        }
                        else
                        {
                            #region Se existe Serviço no CHIC e não no WEB, inserir no WEB

                            serv = new Servicos_Pedido_Venda();

                            foreach (var servicoCHIC in listaServicosCHIC)
                            {
                                PRODUTO1 servicoApolo = apolo.PRODUTO1
                                    .Where(w => w.USERCodigoCHIC == servicoCHIC.item)
                                    .FirstOrDefault();

                                if (servicoApolo != null)
                                {
                                    valorTotalServicos = servicoCHIC.price;
                                }
                            }

                            #endregion
                        }

                        hlbappService.SaveChanges();

                        #endregion

                        #region Dados dos Itens que existem no WEB

                        var listaItensWEB = hlbappService.Item_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID
                                && w.OrderNoCHICReposicao != "Cancelado")
                            .ToList();

                        foreach (var item in listaItensWEB)
                        {
                            #region Carrega dados do Item do CHIC

                            string variety = "";
                            vartablTableAdapter vartabl = new vartablTableAdapter();
                            CHICDataSet.vartablDataTable vartablDT =
                                new CHICDataSet.vartablDataTable();

                            vartabl.FillByDesc(vartablDT, item.ProdCodEstr.Replace(" - Ovos", "").Replace(" - Machos", "").Replace(" - Macho", ""));
                            variety = vartablDT[0].variety;
                            string linhagem = vartablDT[0].desc.Trim();

                            CHICDataSet.bookedRow bRowVendido = bDTCommercial
                                .Where(w => iDT.Any(a => a.item_no == w.item
                                        && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                        && a.variety == variety)
                                    && !w.alt_desc.Contains("Extra"))
                                .FirstOrDefault();

                            #endregion

                            if (bRowVendido != null)
                            {
                                #region Ajusta Datas

                                DateTime caldateWeb = DateTime.Today;
                                if (!item.ProdCodEstr.Contains("Ovos"))
                                {
                                    if (uf.UfRegGeog == "Norte" || uf.UfRegGeog == "Nordeste")
                                        caldateWeb = item.DataEntregaInicial.AddDays(-23);
                                    else
                                        caldateWeb = item.DataEntregaInicial.AddDays(-22);
                                }
                                else
                                    caldateWeb = item.DataEntregaInicial;

                                #endregion

                                #region Qtd Reposicao

                                decimal qtdOvosVendidosCHIC = bDTCommercial
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                            && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))
                                            && a.variety == variety)
                                        && !w.alt_desc.Contains("Extra"))
                                    .Sum(s => s.quantity);

                                if (qtdOvosVendidosCHIC != item.QtdeReposicao)
                                {
                                    item.QtdeReposicao = Convert.ToInt32(qtdOvosVendidosCHIC);
                                    item.ValorTotal = Math.Round(
                                        Convert.ToDecimal(item.QtdeLiquida * item.PrecoPinto), 2);
                                    if (motivo == "Dados Atualizados: ") motivo = motivo + " qtde. reposição do item " + linhagem;
                                    else motivo = motivo + ", qtde. reposição do item " + linhagem;
                                }

                                #endregion

                                #region Tipo de Reposicao

                                custitemTableAdapter ciTA = new custitemTableAdapter();
                                CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                ciTA.FillByBookkey(ciDT, bRowVendido.bookkey);

                                if (ciDT.Count > 0)
                                {
                                    CHICDataSet.custitemRow ciR = ciDT.FirstOrDefault();
                                    string tipoReposicaoCHIC = ciR.tprepo.Trim();

                                    if (tipoReposicaoCHIC != item.TipoReposicao)
                                    {
                                        item.TipoReposicao = tipoReposicaoCHIC;
                                        if (motivo == "Dados Atualizados: ")
                                            motivo = motivo + " tipo de reposição do item " + linhagem;
                                        else motivo = motivo + ", ipo de reposição do item " + linhagem;
                                    }
                                }

                                #endregion

                                #region Atualiza Valor Total e Preco Unitário

                                int qtdeReposicao = 0;
                                if (item.TipoReposicao == "Acerto Comercial")
                                    qtdeReposicao = Convert.ToInt32(item.QtdeReposicao);

                                item.ValorTotal = (item.QtdeLiquida * item.PrecoPinto)
                                    + ((item.QtdeLiquida + item.QtdeBonificada + qtdeReposicao) * valorTotalVacinas)
                                    + (Math.Round(Convert.ToDecimal((item.QtdeLiquida + item.QtdeBonificada
                                        + qtdeReposicao)
                                        * (percServicos / 100.00m)), 0) * valorTotalServicos);

                                int qtdeCalculoValorTotal = 1;
                                if (item.QtdeLiquida > 0)
                                    qtdeCalculoValorTotal = item.QtdeLiquida;
                                else if (item.QtdeReposicao > 0)
                                    qtdeCalculoValorTotal = Convert.ToInt32(item.QtdeReposicao);
                                item.PrecoUnitario = item.ValorTotal / qtdeCalculoValorTotal;

                                #endregion

                                #region Campos Customizados do Item

                                ciDT = new CHICDataSet.custitemDataTable();
                                ciTA.FillByBookkey(ciDT, bRowVendido.bookkey);

                                if (ciDT.Count > 0)
                                {
                                    if (item.Sobra != (ciDT[0].sobra.Trim() == "Sim" ? 1 : 0))
                                    {
                                        item.Sobra = (ciDT[0].sobra.Trim() == "Sim" ? 1 : 0);
                                        if (motivo == "Dados Atualizados: ")
                                            motivo = motivo + " item " + linhagem + " marcado como Sobra";
                                        else motivo = motivo + ", item " + linhagem + " marcado como Sobra";
                                    }
                                }

                                #endregion

                                #region Insere LOG - Item_Ped_Venda - DESATIVADO

                                //LOG_Item_Pedido_Venda logItemPV = new LOG_Item_Pedido_Venda();
                                //logItemPV.IDPedidoVenda = item.IDPedidoVenda;
                                //logItemPV.Sequencia = item.Sequencia;
                                //logItemPV.ProdCodEstr = item.ProdCodEstr;
                                //logItemPV.DataEntregaInicial = item.DataEntregaInicial;
                                //logItemPV.DataEntregaFinal = item.DataEntregaFinal;
                                //logItemPV.QtdeLiquida = item.QtdeLiquida;
                                //logItemPV.PercBonificacao = item.PercBonificacao;
                                //logItemPV.QtdeBonificada = item.QtdeBonificada;
                                //logItemPV.QtdeReposicao = item.QtdeReposicao;
                                //logItemPV.PrecoUnitario = item.PrecoUnitario;
                                //logItemPV.DataHora = DateTime.Now;
                                //logItemPV.Operacao = "Importado p/ WEB";
                                //logItemPV.IDItPedVenda = item.ID;
                                //logItemPV.IDLogPedidoVenda = logPV.ID;
                                //logItemPV.OrderNoCHIC = item.OrderNoCHIC;
                                //logItemPV.OrderNoCHICReposicao = item.OrderNoCHICReposicao;
                                //logItemPV.PrecoPinto = item.PrecoPinto;
                                //logItemPV.TipoReposicao = item.TipoReposicao;
                                //logItemPV.ValorTotal = item.ValorTotal;

                                //hlbappService.LOG_Item_Pedido_Venda.AddObject(logItemPV);

                                #endregion
                            }
                            else
                            {
                                #region Itens que tem no WEB e não no CHIC, serão deletados

                                if (item.QtdeLiquida == 0)
                                {
                                    Item_Pedido_Venda deletaItem = hlbappService.Item_Pedido_Venda
                                        .Where(w => w.ID == item.ID).FirstOrDefault();
                                    hlbappService.Item_Pedido_Venda.DeleteObject(deletaItem);

                                    if (motivo == "Dados Atualizados: ")
                                        motivo = motivo + " exclusão do item " + linhagem + " por não existir no CHIC";
                                    else motivo = motivo + ", exclusão do item " + linhagem + " por não existir no CHIC";
                                }
                                else
                                {
                                    item.QtdeReposicao = 0;
                                    item.OrderNoCHICReposicao = "";

                                    if (motivo == "Dados Atualizados: ")
                                        motivo = motivo + " exclusão da reposição do item " + linhagem + " por não existir no CHIC";
                                    else motivo = motivo + ", exclusão da reposição do item " + linhagem + " por não existir no CHIC";
                                }

                                #endregion
                            }
                        }

                        #endregion

                        #region Dados dos Itens que não existem no WEB, porém existe no CHIC para inserir

                        var listItensCHIC = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D")))
                                && !w.alt_desc.Contains("Extra"))
                            .ToList();

                        var listaItensWEBTotal = hlbappService.Item_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedVenda.ID)
                            .ToList();

                        foreach (var item in listItensCHIC)
                        {
                            #region Carrega dados do Item do CHIC

                            vartablTableAdapter vartabl = new vartablTableAdapter();
                            CHICDataSet.vartablDataTable vartablDT =
                                new CHICDataSet.vartablDataTable();
                            vartbl.Fill(vartablDT);
                            CHICDataSet.itemsRow iRow = iDT.Where(w => w.item_no == item.item).FirstOrDefault();
                            CHICDataSet.vartablRow vRow = vartablDT
                                .Where(w => iDT.Any(a => w.variety == a.variety
                                    && a.item_no == item.item))
                                .FirstOrDefault();

                            string linhagem = vRow.desc.Trim();
                            if (iRow.form.Substring(0, 1).Equals("H"))
                                linhagem = linhagem + " - Ovos";

                            #region Verifica Se é macho para inserir e descrição na frente
                            // 21/05/2018 - Solicita por André / Débora, pois estavam confundindo com os pedidos de fêmeas
                            if (iRow.form == "DM")
                                linhagem = linhagem + " - Machos";

                            #endregion

                            #endregion

                            #region Verifica se o item existe na WEB

                            int existeWeb = 0;
                            existeWeb = listaItensWEB.Where(w => w.ProdCodEstr == linhagem).Count();

                            #endregion

                            if (existeWeb == 0)
                            {
                                Item_Pedido_Venda itemNovoWEB = new Item_Pedido_Venda();

                                itemNovoWEB.IDPedidoVenda = pedVenda.ID;
                                itemNovoWEB.ProdCodEstr = linhagem;
                                DateTime dataWeb = DateTime.Today;

                                #region Calcula Data
                                if (!iRow.form.Substring(0, 1).Equals("H"))
                                {
                                    if (uf.UfRegGeog == "Norte" || uf.UfRegGeog == "Nordeste")
                                        dataWeb = item.cal_date.AddDays(23);
                                    else
                                        dataWeb = item.cal_date.AddDays(22);
                                }
                                else
                                    dataWeb = item.cal_date;

                                #endregion

                                itemNovoWEB.DataEntregaInicial = dataWeb;
                                itemNovoWEB.DataEntregaFinal = dataWeb;
                                itemNovoWEB.QtdeReposicao = Convert.ToInt32(item.quantity);

                                itemNovoWEB.PrecoPinto = item.price;

                                #region Atualiza Valor Total e Preco Unitário

                                int qtdeReposicao = Convert.ToInt32(itemNovoWEB.QtdeReposicao);

                                itemNovoWEB.ValorTotal = (itemNovoWEB.QtdeLiquida * itemNovoWEB.PrecoPinto)
                                    + ((itemNovoWEB.QtdeLiquida + itemNovoWEB.QtdeBonificada + qtdeReposicao)
                                        * valorTotalVacinas)
                                    + (Math.Round(Convert.ToDecimal((itemNovoWEB.QtdeLiquida
                                        + itemNovoWEB.QtdeBonificada + qtdeReposicao)
                                        * (percServicos / 100.00m)), 0) * valorTotalServicos);

                                int qtdeCalculoValorTotal = 1;
                                if (itemNovoWEB.QtdeLiquida > 0)
                                    qtdeCalculoValorTotal = itemNovoWEB.QtdeLiquida;
                                else if (itemNovoWEB.QtdeReposicao > 0)
                                    qtdeCalculoValorTotal = Convert.ToInt32(itemNovoWEB.QtdeReposicao);
                                itemNovoWEB.PrecoUnitario = itemNovoWEB.ValorTotal / qtdeCalculoValorTotal;

                                //if (motivo == "Dados Atualizados: ")
                                //    motivo = motivo + " preço unitário do item " + linhagem;
                                //else motivo = motivo + ", preço unitário do item " + linhagem;

                                itemNovoWEB.Sequencia = listaItensWEBTotal.Count + 1;
                                itemNovoWEB.OrderNoCHICReposicao = item.orderno;
                                itemNovoWEB.Alterado = 0;
                                itemNovoWEB.Importar = 0;

                                #endregion

                                #region Campos Customizados do Item

                                custitemTableAdapter ciTA = new custitemTableAdapter();
                                CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                                ciTA.FillByBookkey(ciDT, item.bookkey);

                                if (ciDT.Count > 0)
                                {
                                    itemNovoWEB.Sobra = (ciDT[0].sobra.Trim() == "Sim" ? 1 : 0);
                                }

                                #endregion

                                if (item.comment_1.Contains("Acerto"))
                                    itemNovoWEB.TipoReposicao = "Acerto Comercial";
                                else if (item.comment_1.Contains("Mortalidade"))
                                    itemNovoWEB.TipoReposicao = "Mortalidade";

                                hlbappService.Item_Pedido_Venda.AddObject(itemNovoWEB);

                                if (motivo == "Dados Atualizados: ")
                                    motivo = motivo + " adicionado item " + linhagem + " da reposição ";
                                else motivo = motivo + ", adicionado item " + linhagem + " da reposição ";
                            }
                        }

                        #endregion

                        //logPV.Motivo = motivo;

                        #region Envia E-mail p/ Representante / Vendedor Avisando (Somente Nutribastos para teste)

                        if (pedVenda.Vendedor.Equals("000083") && motivo != "Dados Atualizados: ")
                        {
                            string caminho = "";

                            CHICDataSet.salesmanDataTable sDT = new CHICDataSet.salesmanDataTable();
                            salesman.FillByCod(sDT, pedVenda.Vendedor);
                            string nome = sDT[0].salesman.Trim();
                            string enderecoEmail = sDT[0].email.Trim();
                            string empresaApolo = "";
                            if (sDT[0].inv_comp.Trim().Equals("BR")) empresaApolo = "5";
                            else if (sDT[0].inv_comp.Trim().Equals("LB")) empresaApolo = "7";
                            else if (sDT[0].inv_comp.Trim().Equals("HN")) empresaApolo = "14";
                            else if (sDT[0].inv_comp.Trim().Equals("PL")) empresaApolo = "20";

                            string corpoEmail = "Prezado(s)," + (char)13 + (char)10
                                + (char)13 + (char)10
                                + "Foram realizadas as seguintes alterações no pedido CHIC Nº" + orderNoErro
                                + " - ID WEB " + pedVenda.ID.ToString() + "."
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + motivo
                                + (char)13 + (char)10 + (char)13 + (char)10
                                + "Qualquer dúvida, entrar em contato o Gerente Comercial para confirmar "
                                + " a alteração." + (char)13 + (char)10 + (char)13 + (char)10
                                + "SISTEMA WEB";

                            string assunto = "**** " + ("Importado p/ WEB").ToUpper() + " - CHIC " + orderNoErro
                                + " / ID " + pedVenda.ID.ToString() + " ****";

                            EnviaConfirmacaoEmail(caminho, enderecoEmail, nome, "", "", "", corpoEmail,
                                assunto, empresaApolo);
                        }

                        #endregion

                        #region Insere LOG se existir motivo

                        if (motivo != "Dados Atualizados: ")
                            InsereLOGPVWeb(pedVenda.ID, "CHIC - " + usuarioCHIC, "Importado p/ WEB",
                                motivo);

                        #endregion
                    }
                }

                hlbappService.SaveChanges();

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Atualizar CHIC com WEB - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                #region Envio de E-mail

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ WEB - REPOSIÇÃO ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Paulo Alves";
                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                string corpoEmail = "";
                string innerException = "";

                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }

                corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o WEB: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "Linha do Erro: " + erro.ToString() + (char)13 + (char)10
                    + "Número do Pedido CHIC: " + orderNoErro + (char)13 + (char)10
                    + "Linha do Erro: " + linenum.ToString() + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 1: " + ex.Message + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 2: " + innerException;

                email.WorkFlowEmailCorpo = corpoEmail;
                apolo.WORKFLOW_EMAIL.AddObject(email);

                apolo.SaveChanges();

                #endregion

                return retorno;
            }
        }

        public void AjustaPrecosPedidosCHICparaWEB()
        {

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            DateTime data = Convert.ToDateTime("04/10/2016");
            HLBAPPServiceEntities hlbapp = new HLBAPPServiceEntities();
            var listaPedidosWEB = hlbappService.Pedido_Venda
                .Where(w => w.Status == "Importado Total"
                    && hlbappService.Item_Pedido_Venda.Any(a => w.ID == a.IDPedidoVenda
                        && a.DataEntregaInicial >= data
                        && a.OrderNoCHIC != null && a.OrderNoCHIC != ""))
                //&& a.OrderNoCHIC == "58347"))
                .ToList();

            foreach (var pedido in listaPedidosWEB)
            {
                var listaItensPedidos = hlbappService.Item_Pedido_Venda
                    .Where(w => w.IDPedidoVenda == pedido.ID).ToList();

                foreach (var item in listaItensPedidos)
                {
                    vartablTableAdapter vTA = new vartablTableAdapter();
                    CHICDataSet.vartablDataTable vDT = new CHICDataSet.vartablDataTable();
                    vTA.Fill(vDT);
                    itemsTableAdapter iTA = new itemsTableAdapter();
                    CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                    iTA.Fill(iDT);

                    ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTA =
                    new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                    CHICDataSet.bookedDataTable bDT = new CHICDataSet.bookedDataTable();
                    bTA.FillByPriceMaiorZero(bDT, item.OrderNoCHIC);
                    CHICDataSet.bookedRow bRow = bDT.Where(b => iDT.Any(i => b.item == i.item_no
                        && vDT.Any(v => v.variety == i.variety && v.desc.Trim() == item.ProdCodEstr)))
                        .FirstOrDefault();

                    if (bRow != null)
                    {
                        if (bRow.price != item.PrecoUnitario)
                        {
                            item.PrecoUnitario = bRow.price;
                        }
                    }
                }
            }

            hlbappService.SaveChanges();
        }

        #endregion

        #region WEB

        #region Programação Diária de Transportes

        public void InserePedidosCHICProgDiariaTransp(DateTime data)
        {
            #region Insere Pedidos da Programação Diária de Transporte

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            #region Filtra Pedidos

            ordersTableAdapter oTA = new ordersTableAdapter();
            CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();
            //DateTime data = DateTime.Today;
            oTA.FillSalesByHatchDate2(oDT, data);

            var listaOrders = oDT.ToList();

            #endregion

            foreach (var order in listaOrders)
            {
                #region Dados Item

                itemsTableAdapter iTA = new itemsTableAdapter();
                CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                iTA.Fill(iDT);

                ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                    new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                bTACommercial.FillByOrderNo(bDTCommercial, order.orderno.Trim());

                var listaItens = bDTCommercial
                    .Where(w => iDT.Any(a => a.item_no == w.item
                        && (a.form.Substring(0, 1).Equals("H") || a.form.Substring(0, 1).Equals("D"))))
                    .GroupBy(g => new 
                        {
                            g.item,
                            g.location
                        })
                    .Select(s => new 
                        {
                            s.Key.item,
                            s.Key.location,
                            qtde = s.Sum(u => u.quantity),
                            price = s.Max(m => m.price)
                        })
                    .ToList();

                #endregion

                foreach (var item in listaItens)
                {
                    #region Verifica Se existe já lançado

                    bool existe = true;

                    Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.CHICNum == order.orderno.Trim()).FirstOrDefault();

                    if (prodDiariaTranspPedido == null)
                    {
                        existe = false;
                        prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                    }

                    #endregion

                    #region Insere Programação Diária

                    prodDiariaTranspPedido.DataProgramacao = bDTCommercial[0].cal_date.AddDays(21);
                    prodDiariaTranspPedido.CodigoCliente = order.cust_no.Trim();

                    ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                        .FirstOrDefault();

                    prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                    prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);

                    #region Local de Entrega

                    if (order.contact_no != 0)
                    {
                        shippingTableAdapter sTA = new shippingTableAdapter();
                        CHICDataSet.shippingDataTable sDT = new CHICDataSet.shippingDataTable();
                        sTA.FillByCustNo(sDT, order.cust_no);

                        if (sDT.Count > 0)
                        {
                            CHICDataSet.shippingRow enderecoEntrega = sDT
                                .Where(w => w.contact_no == order.contact_no).FirstOrDefault();

                            if (enderecoEntrega != null)
                            {
                                prodDiariaTranspPedido.LocalEntrega =
                                    enderecoEntrega.address2.Trim() + " - " + enderecoEntrega.address3.Trim();
                            }
                        }
                    }
                    else
                    {
                        CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                        prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                            cidade.PaisSigla;
                    }

                    #endregion

                    prodDiariaTranspPedido.Produto = iDT.Where(w => w.item_no == item.item)
                        .FirstOrDefault().form.Trim();
                    prodDiariaTranspPedido.Linhagem = iDT.Where(w => w.item_no == item.item)
                        .FirstOrDefault().variety.Trim();
                    prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                    prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                    prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                    ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                        .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                    if (fone != null)
                    {
                        prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                    }

                    prodDiariaTranspPedido.DataEntrega = order.del_date;
                    prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                    salesmanTableAdapter slTA = new salesmanTableAdapter();
                    CHICDataSet.salesmanDataTable slDT = new CHICDataSet.salesmanDataTable();
                    slTA.FillByCod(slDT, prodDiariaTranspPedido.CodigoRepresentante);

                    prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim();

                    if (!existe)
                    {
                        prodDiariaTranspPedido.NumVeiculo = 0;
                        prodDiariaTranspPedido.Embalagem = "";
                        prodDiariaTranspPedido.Status = "Pendente";
                        hlbappService.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);
                    }

                    #endregion
                }
            }

            hlbappService.SaveChanges();

            #endregion

            #region Insere Veículos da Programação Diária de Transporte

            var listaPedidos = hlbappService.Prog_Diaria_Transp_Pedidos
                .Where(w => w.DataProgramacao == data).ToList();

            for (int i = 1; i <= 8; i++)
            {
                Prog_Diaria_Transp_Veiculos progVeiculo = hlbappService.Prog_Diaria_Transp_Veiculos
                    .Where(w => w.DataProgramacao == data && w.NumVeiculo == i).FirstOrDefault();

                bool existe = true;

                if (progVeiculo == null)
                {
                    existe = false;
                    progVeiculo = new Prog_Diaria_Transp_Veiculos();
                }

                progVeiculo.DataProgramacao = data;
                progVeiculo.NumVeiculo = i;
                progVeiculo.QuantidadeTotal = listaPedidos.Where(w => w.NumVeiculo == i).Sum(s => s.Quantidade);

                if (progVeiculo.QuantidadePorCaixa != null)
                {
                    decimal qtdCaixaDecimal = Convert.ToDecimal(progVeiculo.QuantidadeTotal) /
                        Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);
                    int qtdCaixaInt = Convert.ToInt32(progVeiculo.QuantidadeTotal) /
                        Convert.ToInt32(progVeiculo.QuantidadePorCaixa);

                    if ((qtdCaixaDecimal - qtdCaixaInt) > 0)
                        progVeiculo.QunatidadeCaixa = qtdCaixaInt + 1;
                    else
                        progVeiculo.QunatidadeCaixa = qtdCaixaInt;
                }

                progVeiculo.ValorTotal = listaPedidos.Where(w => w.NumVeiculo == i).Sum(s => s.ValorTotal);

                if (!existe) hlbappService.Prog_Diaria_Transp_Veiculos.AddObject(progVeiculo);
            }

            hlbappService.SaveChanges();

            #endregion
        }

        public void AtualizaValoresVeiculos(DateTime data)
        {
            #region Atualiza Veículos da Programação Diária de Transporte

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            var listaPedidos = hlbappService.Prog_Diaria_Transp_Pedidos
                .Where(w => w.DataProgramacao == data).ToList();

            #region Atualização Transema

            for (int i = 0; i <= 10; i++)
            {
                Prog_Diaria_Transp_Veiculos progVeiculo = hlbappService.Prog_Diaria_Transp_Veiculos
                    .Where(w => w.DataProgramacao == data && w.NumVeiculo == i
                        && w.EmpresaTranportador == "TR").FirstOrDefault();

                bool existe = true;

                if (progVeiculo == null)
                {
                    existe = false;
                    progVeiculo = new Prog_Diaria_Transp_Veiculos();
                }

                progVeiculo.DataProgramacao = data;
                progVeiculo.NumVeiculo = i;
                progVeiculo.QuantidadeTotal = listaPedidos
                    .Where(w => w.NumVeiculo == i && w.EmpresaTranportador == "TR").Sum(s => s.Quantidade);
                if (progVeiculo.QuantidadePorCaixa != null)
                {
                    if (progVeiculo.QuantidadePorCaixa > 0)
                    {
                        decimal qtdCaixaDecimal = Convert.ToDecimal(progVeiculo.QuantidadeTotal) /
                            Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);
                        int qtdCaixaInt = Convert.ToInt32(progVeiculo.QuantidadeTotal) /
                            Convert.ToInt32(progVeiculo.QuantidadePorCaixa);

                        if ((qtdCaixaDecimal - qtdCaixaInt) > 0)
                            progVeiculo.QunatidadeCaixa = qtdCaixaInt + 1;
                        else
                            progVeiculo.QunatidadeCaixa = qtdCaixaInt;
                    }
                }
                else
                    progVeiculo.QuantidadePorCaixa = 100;

                progVeiculo.EmpresaTranportador = "TR";
                progVeiculo.EntCod = "0000807";

                //progVeiculo.ValorTotal = listaPedidos.Where(w => w.NumVeiculo == i).Sum(s => s.ValorTotal);

                if (!existe) hlbappService.Prog_Diaria_Transp_Veiculos.AddObject(progVeiculo);

                List<Prog_Diaria_Transp_Pedidos> listPedidosVeiculos = hlbappService.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.DataProgramacao == data && w.NumVeiculo == i
                        && w.EmpresaTranportador == "TR"
                        && w.Status == "Pendente").ToList();

                foreach (var item in listPedidosVeiculos)
                {
                    if (item.NumVeiculo != 0 && item.Embalagem != "" && progVeiculo.QuantidadePorCaixa > 0)
                        item.Status = "Preenchido";
                }
            }

            #endregion

            #region Atualização Planalto

            var listaVeiculosPlanalto = hlbappService.Prog_Diaria_Transp_Veiculos
                .Where(w => w.EmpresaTranportador != "TR"
                    && w.DataProgramacao == data)
                .ToList();

            foreach (var item in listaVeiculosPlanalto)
            {
                var listaPedidosVeiculo = listaPedidos
                    .Where(w => w.NumVeiculo == item.NumVeiculo
                        && w.EmpresaTranportador.Equals(item.EmpresaTranportador)).ToList();

                item.QuantidadeTotal = listaPedidosVeiculo.Sum(s => s.Quantidade);
                item.QunatidadeCaixa = listaPedidosVeiculo.Sum(s => s.QuantidadeCaixa);
                item.QunatidadeCaixa = (item.QunatidadeCaixa == 0 ? 1 : item.QunatidadeCaixa);
                item.QuantidadePorCaixa = item.QuantidadeTotal / item.QunatidadeCaixa;
                decimal calculoQtdeCaixa = Convert.ToDecimal(item.QuantidadeTotal / (item.QunatidadeCaixa * 1.00m));
                if ((calculoQtdeCaixa - item.QuantidadePorCaixa) > 0)
                    item.QuantidadePorCaixa = item.QuantidadePorCaixa + 1;
                item.ValorTotal = listaPedidosVeiculo.Sum(s => s.KM) * item.ValorKM;
            }

            #endregion

            hlbappService.SaveChanges();

            #endregion
        }

        public string AtualizarProgDiariaTranspDiaNascimento(DateTime data)
        {
            string retorno = "";

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Insere Pedidos da Programação Diária de Transporte - CHIC Matrizes

                ordersTableAdapter oTA = new ordersTableAdapter();
                CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();

                itemsTableAdapter iTA = new itemsTableAdapter();
                CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                iTA.Fill(iDT);

                #region Deleta Pedidos não Existentes

                var listaPedidosData = hlbappService.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.DataProgramacao == data
                        && w.CHICNum == "85051"
                        && w.CHICOrigem == "Matriz"
                        ).ToList();

                foreach (var item in listaPedidosData)
                {
                    if (item.CHICNum != null && item.CHICNum != "")
                    {
                        oTA.FillByOrderNo(oDT, item.CHICNum);

                        if (oDT.Count == 0)
                        {
                            hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                            InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Pedido não encontrado no CHIC!");
                        }
                        else
                        {
                            #region Carrega Itens CHIC

                            CHICDataSet.ordersRow oRow = oDT.FirstOrDefault();
                            ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTA = 
                                new ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter();
                            CHICDataSet.bookedDataTable bDT = new CHICDataSet.bookedDataTable();
                            bTA.FillByOrderNo(bDT, oRow.orderno);

                            #endregion

                            #region Ajusta pedidos de Ovos como Nascimento e não Retirada

                            DateTime setDateErro = data.AddDays(-21);
                            int existeErro = bDT.Where(w => iDT.Any(a => a.item_no == w.item
                                && (a.form.Substring(0, 1).Equals("H")))
                                && w.cal_date == setDateErro)
                            .Count();

                            if (existeErro > 0)
                            {
                                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Ajuste de pedido de Ovos como Nascimento e não Retirada!");
                            }

                            #endregion

                            #region Ajusta Pedido que foi alterada data

                            existeErro = 0;
                            existeErro = bDT.Where(w => iDT.Any(a => a.item_no == w.item
                                && (a.form.Substring(0, 1).Equals("D")))
                                && w.cal_date != setDateErro)
                            .Count();

                            if (existeErro > 0)
                            {
                                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Ajuste de pedido que foi alterado a data!");
                            }

                            #endregion

                            #region Verifica se a Linhagem ainda existe no Pedido no mesmo incubatório

                            existeErro = 0;
                            existeErro = bDT.Where(w => iDT.Any(a => a.item_no == w.item
                                    && a.variety.Trim() == item.Linhagem
                                    && a.form.Trim() == item.Produto)
                                    && w.location.Trim() == item.LocalNascimento)
                                .Count();

                            if (existeErro == 0)
                            {
                                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Verificação se a Linhagem ainda existe no Pedido no mesmo incubatório!");
                            }

                            #endregion

                            #region Verifica se o Incubatório ainda existe no Pedido

                            existeErro = 0;
                            existeErro = bDT.Where(w => w.location.Trim() == item.LocalNascimento
                                    && item.LocalNascimento != null)
                                .Count();

                            if (existeErro == 0)
                            {
                                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Verificação se o Incubatório ainda existe no Pedido!");
                            }

                            #endregion
                        }
                    }
                }

                hlbappService.SaveChanges();

                #endregion

                #region Pintos

                #region Filtra Pedidos

                //DateTime data = DateTime.Today;
                //oTA.FillSalesByHatchDate2(oDT, data);
                DateTime setDate = data.AddDays(-21);
                //oTA.FillSalesByCalDate(oDT, setDate);

                //var listaOrders = oDT
                //    //.Where(w => (w.orderno == "85051"))
                //    .ToList();

                oDT = new CHICDataSet.ordersDataTable();
                var listaOrders = oDT.ToList();

                #endregion

                foreach (var order in listaOrders)
                {
                    #region Dados Item

                    iTA.Fill(iDT);

                    ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                        new ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter();
                    CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                    bTACommercial.FillByOrderNo(bDTCommercial, order.orderno);

                    var listaItens = bDTCommercial
                        .Where(w => iDT.Any(a => a.item_no == w.item
                            && (a.form.Substring(0, 1).Equals("D")
                            //&& a.variety != "DKBW" && a.variety != "DKBB")
                            )))
                        .Join(
                            iDT,
                            b => b.item,
                            i => i.item_no,
                            (b, i) => new { BOOKED = b, ITEM = i })
                        .GroupBy(g => new
                        {
                            //g.BOOKED.item,
                            g.BOOKED.location,
                            g.ITEM.variety,
                            g.ITEM.form
                        })
                        .Select(s => new
                        {
                            //s.Key.item,
                            s.Key.location,
                            s.Key.variety,
                            s.Key.form,
                            qtdeBonif = s.Sum(w => w.BOOKED.alt_desc.Contains("Extra") ? w.BOOKED.quantity : 0),
                            qtdeVend = s.Sum(w => !w.BOOKED.alt_desc.Contains("Extra") ? w.BOOKED.quantity : 0),
                            qtde = s.Sum(u => u.BOOKED.quantity),
                            price = s.Max(m => m.BOOKED.price)
                        })
                        .ToList();

                    #endregion

                    #region Dados Custom Table

                    int_commTableAdapter icTA = new int_commTableAdapter();
                    CHICDataSet.int_commDataTable icDT = new CHICDataSet.int_commDataTable();
                    icTA.FillByOrderNo(icDT, order.orderno);

                    salesmanTableAdapter slTA = new salesmanTableAdapter();
                    CHICDataSet.salesmanDataTable slDT = new CHICDataSet.salesmanDataTable();
                    slTA.FillByCod(slDT, order.salesrep);

                    string codigoCliente = order.cust_no.Trim();

                    #region Verifica Transportadora

                    CIDADE cidadeEntidade = apolo.CIDADE
                        .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                            && a.EntCod == codigoCliente)).FirstOrDefault();

                    CHICDataSet.int_commRow iR = icDT.FirstOrDefault();
                    string empresaTransportadoraCHIC = "";
                    if (cidadeEntidade.UfSigla == "EX")
                    {
                        empresaTransportadoraCHIC = "EX";
                    }
                    else
                    {
                        if (iR != null)
                        {
                            string transportadoraPedido = iR.tranport.Trim();
                            string invComp = "";
                            if (slDT.Count > 0) invComp = slDT[0].inv_comp;

                            if (transportadoraPedido.Equals("Planalto")
                                || (invComp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                empresaTransportadoraCHIC = "PL";
                            else if (((transportadoraPedido.Equals("H&N"))
                                        || (!invComp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                && data >= Convert.ToDateTime("31/07/2017")) // Data da Implantação
                                empresaTransportadoraCHIC = "HN";
                            else
                                empresaTransportadoraCHIC = "TR";
                        }
                        else
                            empresaTransportadoraCHIC = "TR";
                    }

                    #endregion

                    #endregion

                    #region Verifica se é Reposição

                    CHICDataSet.int_commDataTable icDTVerificaRepo = new CHICDataSet.int_commDataTable();
                    icTA.FillByOrderNo(icDTVerificaRepo, order.orderno.Trim());

                    bool eReposicao = false;
                    if (icDTVerificaRepo.Count > 0)
                    {
                        if (icDTVerificaRepo[0].npedrepo > 0) eReposicao = true;
                    }

                    #endregion

                    foreach (var item in listaItens)
                    {
                        #region Verifica Se existe já lançado

                        string linhagem = item.variety.Trim();
                        string produto = item.form.Trim();

                        bool entrou = false;
                        if (order.orderno.Equals("69400"))
                            entrou = true;

                        int? numVeiculo = 0;
                        string embalagem = "";
                        string status = "Pendente";
                        string observacao = "";
                        int? ordem = 0;
                        string inicioCarregamentoEsperado = "";
                        string chegadaClienteEsperada = "";
                        int? km = 0;
                        string inicioCarregamentoReal = "";
                        string chegadaCarregamentoReal = "";
                        int? qtdCaixas = 0;
                        string numRoteiroEntregaFluig = "";

                        List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                            .Where(w => w.CHICNum == order.orderno.Trim()
                            && w.Linhagem == linhagem && w.Produto == produto
                            && w.LocalNascimento == item.location
                            && w.CHICOrigem == "Matriz"
                            ).ToList();

                        if (listProdDiariaTranspPedido.Count > 1)
                        {
                            foreach (var pedido in listProdDiariaTranspPedido)
                            {
                                numVeiculo = pedido.NumVeiculo;
                                embalagem = pedido.Embalagem;
                                status = pedido.Status;
                                observacao = pedido.Observacao;
                                ordem = pedido.Ordem;
                                inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                                chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                                km = pedido.KM;
                                inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                                chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                                qtdCaixas = pedido.QuantidadeCaixa;
                                numRoteiroEntregaFluig = pedido.NumRoteiroEntregaFluig;

                                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(pedido, "Exclusão", "Exclusão do Pedido duplicado para lançamento de novo copiando os dados de transporte!");
                            }
                        }

                        hlbappService.SaveChanges();

                        Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                            .Where(w => w.CHICNum == order.orderno.Trim()
                            && w.Linhagem == linhagem && w.Produto == produto
                            && w.LocalNascimento == item.location
                            && w.CHICOrigem == "Matriz"
                            ).FirstOrDefault();

                        if (prodDiariaTranspPedido != null)
                        {
                            numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                            embalagem = prodDiariaTranspPedido.Embalagem;
                            status = prodDiariaTranspPedido.Status;
                            observacao = prodDiariaTranspPedido.Observacao;
                            ordem = prodDiariaTranspPedido.Ordem;
                            inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                            chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                            km = prodDiariaTranspPedido.KM;
                            inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                            chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                            qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;
                            numRoteiroEntregaFluig = prodDiariaTranspPedido.NumRoteiroEntregaFluig;

                            hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                            InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Exclusão", "Exclusão do Pedido para lançamento de novo copiando os dados de transporte!");
                        }

                        #endregion

                        if (!eReposicao)
                        {
                            #region Insere Programação Diária

                            prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                            prodDiariaTranspPedido.CHICOrigem = "Matriz";
                            prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                            prodDiariaTranspPedido.Embalagem = embalagem;
                            prodDiariaTranspPedido.Status = status;
                            prodDiariaTranspPedido.Observacao = observacao;
                            prodDiariaTranspPedido.Ordem = ordem;
                            prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                            prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                            prodDiariaTranspPedido.KM = km;
                            prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                            prodDiariaTranspPedido.ChegadaClienteReal = chegadaCarregamentoReal;
                            prodDiariaTranspPedido.QuantidadeCaixa = qtdCaixas;
                            prodDiariaTranspPedido.NumRoteiroEntregaFluig = numRoteiroEntregaFluig;

                            prodDiariaTranspPedido.DataProgramacao = bDTCommercial[0].cal_date.AddDays(21);
                            prodDiariaTranspPedido.CodigoCliente = codigoCliente;

                            ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                                .FirstOrDefault();

                            //if (entidade.EntNome.Length > 15)
                            ////    prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0,15) + "...";
                            //else
                            if (entidade != null)
                                prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                            else
                                prodDiariaTranspPedido.NomeCliente = "NÃO EXISTE ENTIDADE "
                                    + prodDiariaTranspPedido.CodigoCliente + " NO APOLO! VERIFICAR COM A PROGRAMAÇÃO!";

                            string condPag = "";
                            if (order.delivery.IndexOf("(") > 0)
                                condPag = (order.delivery.Substring(0, (order.delivery.IndexOf("(") - 1))).Trim();
                            else
                                condPag = order.delivery.Trim();
                            prodDiariaTranspPedido.CondicaoPagamento = condPag;

                            #region Local de Entrega

                            if (order.contact_no != 0)
                            {
                                shippingTableAdapter sTA = new shippingTableAdapter();
                                CHICDataSet.shippingDataTable sDT = new CHICDataSet.shippingDataTable();
                                sTA.FillByCustNo(sDT, order.cust_no);

                                if (sDT.Count > 0)
                                {
                                    CHICDataSet.shippingRow enderecoEntrega = sDT
                                        .Where(w => w.contact_no == order.contact_no).FirstOrDefault();

                                    if (enderecoEntrega != null)
                                    {
                                        prodDiariaTranspPedido.LocalEntrega =
                                            enderecoEntrega.address2.Trim() + " - " + enderecoEntrega.address3.Trim();
                                    }
                                }
                            }
                            else
                            {
                                if (entidade != null)
                                {
                                    CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                                    //prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                                    //    cidade.PaisSigla;
                                    if (cidade != null)
                                        prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                                    else
                                        prodDiariaTranspPedido.LocalEntrega = "";
                                }
                                else
                                    prodDiariaTranspPedido.LocalEntrega = "";
                            }

                            #endregion

                            prodDiariaTranspPedido.Produto = produto;
                            prodDiariaTranspPedido.Linhagem = linhagem;
                            prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                            prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                            prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                            if (entidade != null)
                            {
                                ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                                    .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                                if (fone != null)
                                {
                                    prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                                }
                            }

                            prodDiariaTranspPedido.DataEntrega = order.del_date;
                            prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                            if (icDT.Count > 0)
                            {
                                prodDiariaTranspPedido.ObservacaoCHIC = icDT[0].hatchinf.Trim();
                                prodDiariaTranspPedido.ObsProgramacao = icDT[0].comments.Trim();
                            }
                            else
                            {
                                prodDiariaTranspPedido.ObservacaoCHIC = "";
                                prodDiariaTranspPedido.ObsProgramacao = "";
                            }

                            #region Debicagem

                            int existeDebicagem = bDTCommercial.Where(w => w.item == "169").Count();
                            if (existeDebicagem > 0)
                                prodDiariaTranspPedido.Debicagem = "X";
                            else
                                prodDiariaTranspPedido.Debicagem = "";

                            #endregion

                            if (slDT.Count > 0)
                            {
                                if (slDT[0].salesman.Trim().Length > 15)
                                    prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim().Substring(0, 15) + "...";
                                else
                                    prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim();

                                prodDiariaTranspPedido.Empresa = slDT[0].inv_comp.Trim();
                            }
                            else
                            {
                                prodDiariaTranspPedido.NomeRepresentante = "SEM REPRESENTANTE";
                                prodDiariaTranspPedido.Empresa = "BR";
                            }

                            prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                            #region Campos Novos para Programação Fluig

                            int idPV = 0;
                            //if (order.po_number.Trim() != "")
                            //{
                            //    if (int.TryParse(order.po_number.Trim(), out idPV))
                            //    {
                            //        var existePVWEB = hlbappService.Pedido_Venda.Where(w => w.ID == idPV).FirstOrDefault();
                            //        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = idPV;
                            //    }
                            //    else
                            //    {
                            //        var orderno = order.orderno.Trim();
                            //        var existePVWEB = hlbappService.Item_Pedido_Venda
                            //            .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                            //            .FirstOrDefault();
                            //        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                            //    }
                            //}
                            //else
                            //{
                            var orderno = order.orderno.Trim();
                            var existePVWEB = hlbappService.Item_Pedido_Venda
                                .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                                .FirstOrDefault();
                            if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                            //}
                            prodDiariaTranspPedido.EnderEntSeq = Convert.ToInt32(order.contact_no);
                            prodDiariaTranspPedido.QtdeVendida = Convert.ToInt32(item.qtdeVend);
                            prodDiariaTranspPedido.QtdeBonificada = Convert.ToInt32(item.qtdeBonif);

                            var qtdeVendidaParaCalculoPercBonificacao = listaItens
                                .Where(w => w.variety == item.variety && w.price > 0)
                                .Sum(s => s.qtdeVend);
                            if (qtdeVendidaParaCalculoPercBonificacao == 0) qtdeVendidaParaCalculoPercBonificacao = 1;

                            if (prodDiariaTranspPedido.QtdeBonificada != 0 && prodDiariaTranspPedido.QtdeVendida != 0)
                                //prodDiariaTranspPedido.PercBonificacao = Convert.ToInt32(((prodDiariaTranspPedido.QtdeBonificada * 1.00m)
                                //    / (prodDiariaTranspPedido.QtdeVendida * 1.00m)) * 100.00m);
                                prodDiariaTranspPedido.PercBonificacao = Convert.ToInt32(((prodDiariaTranspPedido.QtdeBonificada * 1.00m)
                                    / (qtdeVendidaParaCalculoPercBonificacao * 1.00m)) * 100.00m);
                            prodDiariaTranspPedido.MotivoSobra = "";
                            prodDiariaTranspPedido.QtdeReposicao = 0;
                            prodDiariaTranspPedido.PrecoProduto = item.price;

                            #region Carrega Pedido de Reposição Caso Exista

                            CHICDataSet.int_commDataTable icDTReposicao = new CHICDataSet.int_commDataTable();
                            icTA.FillByNpedrepo(icDTReposicao, Convert.ToDecimal(order.orderno.Trim()));

                            string orderNoCHICReposicao = null;
                            if (icDTReposicao.Count > 0)
                            {
                                orderNoCHICReposicao = icDTReposicao[0].orderno;
                            }

                            if (orderNoCHICReposicao != null)
                            {
                                CHICDataSet.bookedDataTable bDTReposicao = new CHICDataSet.bookedDataTable();
                                bTACommercial.FillByOrderNo(bDTReposicao, orderNoCHICReposicao);
                                CHICDataSet.bookedRow bRowReposicao = bDTReposicao
                                    .Where(w => iDT.Any(a => a.item_no == w.item
                                                    && a.form == item.form
                                                    && a.variety == item.variety)
                                            && w.location == item.location)
                                    .FirstOrDefault();

                                if (bRowReposicao != null)
                                {
                                    prodDiariaTranspPedido.CHICNumReposicao = orderNoCHICReposicao;
                                    prodDiariaTranspPedido.QtdeReposicao = Convert.ToInt32(bRowReposicao.quantity);
                                    if (bRowReposicao.comment_1.Contains("Acerto"))
                                        prodDiariaTranspPedido.MotivoReposicao = "Acerto Comercial";
                                    else if (bRowReposicao.comment_1.Contains("Mortalidade"))
                                        prodDiariaTranspPedido.MotivoReposicao = "Mortalidade";
                                }
                            }

                            #endregion

                            #region Campos Customizados do Item - Sobra

                            custitemTableAdapter ciTA = new custitemTableAdapter();
                            CHICDataSet.custitemDataTable ciDT = new CHICDataSet.custitemDataTable();
                            ciTA.FillByVarietyFormLocation(ciDT, item.variety, item.form, item.location, order.orderno.Trim());

                            if (ciDT.Count > 0)
                            {
                                if (ciDT[0].sobra.Trim() == "Sim")
                                    prodDiariaTranspPedido.QtdeSobra = Convert.ToInt32(item.qtde);
                            }

                            #endregion

                            prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde) + prodDiariaTranspPedido.QtdeReposicao;

                            #endregion

                            hlbappService.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);
                            InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Inclusão", "Nova inclusão do pedido no Web!");

                            #endregion
                        }
                    }
                }

                #endregion

                #region Ovos

                #region Filtra Pedidos

                //DateTime data = DateTime.Today;
                //oTA.FillSalesByHatchDate2(oDT, data);
                setDate = data;
                oTA.FillSalesByCalDate(oDT, setDate);

                listaOrders = oDT
                    .Where(w => w.orderno == "85051")
                    .ToList();

                #endregion

                foreach (var order in listaOrders)
                {
                    #region Dados Item

                    ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter bTACommercial =
                        new ImportaCHICService.Data.CHICDataSetTableAdapters.bookedTableAdapter();
                    CHICDataSet.bookedDataTable bDTCommercial = new CHICDataSet.bookedDataTable();
                    bTACommercial.FillByOrderNo(bDTCommercial, order.orderno);

                    var listaItens = bDTCommercial
                        .Where(w => iDT.Any(a => a.item_no == w.item
                            && (a.form.Substring(0, 1).Equals("H")
                            //&& a.variety != "DKBW" && a.variety != "DKBB"
                            )))
                        .Join(
                            iDT,
                            b => b.item,
                            i => i.item_no,
                            (b, i) => new { BOOKED = b, ITEM = i })
                        .GroupBy(g => new
                        {
                            //g.BOOKED.item,
                            g.BOOKED.location,
                            g.ITEM.variety,
                            g.ITEM.form
                        })
                        .Select(s => new
                        {
                            //s.Key.item,
                            s.Key.location,
                            s.Key.variety,
                            s.Key.form,
                            qtde = s.Sum(u => u.BOOKED.quantity),
                            price = s.Max(m => m.BOOKED.price)
                        })
                        .ToList();

                    #endregion

                    #region Dados Custom Table

                    int_commTableAdapter icTA = new int_commTableAdapter();
                    CHICDataSet.int_commDataTable icDT = new CHICDataSet.int_commDataTable();
                    icTA.FillByOrderNo(icDT, order.orderno);

                    salesmanTableAdapter slTA = new salesmanTableAdapter();
                    CHICDataSet.salesmanDataTable slDT = new CHICDataSet.salesmanDataTable();
                    slTA.FillByCod(slDT, order.salesrep);

                    string codigoCliente = order.cust_no.Trim();

                    #region Verifica Transportadora

                    CIDADE cidadeEntidade = apolo.CIDADE
                        .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                            && a.EntCod == codigoCliente)).FirstOrDefault();

                    CHICDataSet.int_commRow iR = icDT.FirstOrDefault();
                    string empresaTransportadoraCHIC = "";
                    if (cidadeEntidade.UfSigla == "EX")
                    {
                        empresaTransportadoraCHIC = "EX";
                    }
                    else
                    {
                        if (iR != null)
                        {
                            string transportadoraPedido = iR.tranport.Trim();
                            string invComp = "";
                            if (slDT.Count > 0) invComp = slDT[0].inv_comp;

                            if (transportadoraPedido.Equals("Planalto")
                                || (invComp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                empresaTransportadoraCHIC = "PL";
                            else if (((transportadoraPedido.Equals("H&N"))
                                        || (!invComp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                && data >= Convert.ToDateTime("31/07/2017")) // Data da Implantação
                                empresaTransportadoraCHIC = "HN";
                            else
                                empresaTransportadoraCHIC = "TR";
                        }
                        else
                            empresaTransportadoraCHIC = "TR";
                    }

                    #endregion

                    #endregion

                    foreach (var item in listaItens)
                    {
                        #region Verifica Se existe já lançado

                        string linhagem = item.variety;
                        string produto = item.form;

                        int? numVeiculo = 0;
                        string embalagem = "";
                        string status = "Pendente";
                        string observacao = "";
                        int? ordem = 0;
                        string inicioCarregamentoEsperado = "";
                        string chegadaClienteEsperada = "";
                        int? km = 0;
                        string inicioCarregamentoReal = "";
                        string chegadaClienteReal = "";
                        string chegadaCarregamentoReal = "";
                        int? qtdCaixas = 0;
                        string numRoteiroEntregaFluig = "";

                        List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                            .Where(w => w.CHICNum == order.orderno.Trim()
                            && w.Linhagem == linhagem && w.Produto == produto
                            //&& w.LocalNascimento == item.location).ToList();
                            && w.CHICOrigem == "Matriz"
                            ).ToList();

                        if (listProdDiariaTranspPedido.Count > 1)
                        {
                            foreach (var pedido in listProdDiariaTranspPedido)
                            {
                                numVeiculo = pedido.NumVeiculo;
                                embalagem = pedido.Embalagem;
                                status = pedido.Status;
                                observacao = pedido.Observacao;
                                ordem = pedido.Ordem;
                                inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                                chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                                km = pedido.KM;
                                inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                                chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                                qtdCaixas = pedido.QuantidadeCaixa;
                                numRoteiroEntregaFluig = pedido.NumRoteiroEntregaFluig;

                                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(pedido, "Exclusão", "Exclusão do Pedido duplicado para lançamento de novo copiando os dados de transporte!");
                            }
                        }

                        hlbappService.SaveChanges();

                        Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                            .Where(w => w.CHICNum == order.orderno.Trim()
                            && w.Linhagem == linhagem && w.Produto == produto
                            //&& w.LocalNascimento == item.location).FirstOrDefault();
                            && w.CHICOrigem == "Matriz"
                            ).FirstOrDefault();

                        if (prodDiariaTranspPedido != null)
                        {
                            numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                            embalagem = prodDiariaTranspPedido.Embalagem;
                            status = prodDiariaTranspPedido.Status;
                            observacao = prodDiariaTranspPedido.Observacao;
                            ordem = prodDiariaTranspPedido.Ordem;
                            inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                            chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                            km = prodDiariaTranspPedido.KM;
                            inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                            chegadaClienteReal = prodDiariaTranspPedido.ChegadaClienteReal;
                            chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                            qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;
                            numRoteiroEntregaFluig = prodDiariaTranspPedido.NumRoteiroEntregaFluig;

                            hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                            InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Exclusão", "Exclusão do Pedido para lançamento de novo copiando os dados de transporte!");
                        }

                        #endregion

                        #region Insere Programação Diária

                        prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                        prodDiariaTranspPedido.CHICOrigem = "Matriz";
                        prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                        prodDiariaTranspPedido.Embalagem = embalagem;
                        prodDiariaTranspPedido.Status = status;
                        prodDiariaTranspPedido.Observacao = observacao;
                        prodDiariaTranspPedido.Ordem = ordem;
                        prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                        prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                        prodDiariaTranspPedido.KM = km;
                        prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                        prodDiariaTranspPedido.ChegadaClienteReal = chegadaClienteReal;
                        prodDiariaTranspPedido.NumRoteiroEntregaFluig = numRoteiroEntregaFluig;

                        prodDiariaTranspPedido.DataProgramacao = bDTCommercial[0].cal_date;
                        prodDiariaTranspPedido.CodigoCliente = order.cust_no.Trim();

                        ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                            .FirstOrDefault();

                        if (entidade != null)
                        {
                            //if (entidade.EntNome.Length > 15)
                            //    prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0, 15) + "...";
                            //else
                                prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                        }

                        string condPag = "";
                        if (order.delivery.IndexOf("(") > 0)
                            condPag = (order.delivery.Substring(0, (order.delivery.IndexOf("(") - 1))).Trim();
                        else
                            condPag = order.delivery.Trim();
                        prodDiariaTranspPedido.CondicaoPagamento = condPag;

                        prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);
                        prodDiariaTranspPedido.QtdeVendida = Convert.ToInt32(item.qtde);
                        prodDiariaTranspPedido.MotivoSobra = "";
                        prodDiariaTranspPedido.QtdeReposicao = 0;
                        prodDiariaTranspPedido.PrecoProduto = item.price;

                        int idPV = 0;
                        //if (order.po_number.Trim() != "")
                        //{
                        //    if (int.TryParse(order.po_number.Trim(), out idPV))
                        //    {
                        //        var existePVWEB = hlbappService.Pedido_Venda.Where(w => w.ID == idPV).FirstOrDefault();
                        //        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = idPV;
                        //    }
                        //    else
                        //    {
                        //        var orderno = order.orderno.Trim();
                        //        var existePVWEB = hlbappService.Item_Pedido_Venda
                        //            .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                        //            .FirstOrDefault();
                        //        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                        //    }
                        //}
                        //else
                        //{
                        var orderno = order.orderno.Trim();
                        var existePVWEB = hlbappService.Item_Pedido_Venda
                            .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                            .FirstOrDefault();
                        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                        //}
                        prodDiariaTranspPedido.EnderEntSeq = Convert.ToInt32(order.contact_no);

                        #region Local de Entrega

                        if (order.contact_no != 0)
                        {
                            shippingTableAdapter sTA = new shippingTableAdapter();
                            CHICDataSet.shippingDataTable sDT = new CHICDataSet.shippingDataTable();
                            sTA.FillByCustNo(sDT, order.cust_no);

                            if (sDT.Count > 0)
                            {
                                CHICDataSet.shippingRow enderecoEntrega = sDT
                                    .Where(w => w.contact_no == order.contact_no).FirstOrDefault();

                                if (enderecoEntrega != null)
                                {
                                    prodDiariaTranspPedido.LocalEntrega =
                                        enderecoEntrega.address2.Trim() + " - " + enderecoEntrega.address3.Trim();
                                }
                            }
                        }
                        else
                        {
                            if (entidade != null)
                            {
                                CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                                //prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                                //    cidade.PaisSigla;
                                prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                            }
                        }

                        #endregion

                        prodDiariaTranspPedido.Produto = produto;
                        prodDiariaTranspPedido.Linhagem = linhagem;
                        prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                        prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                        prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                        if (entidade != null)
                        {
                            ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                                .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                            if (fone != null)
                            {
                                prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                            }
                        }

                        prodDiariaTranspPedido.DataEntrega = order.del_date;
                        prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                        if (icDT.Count > 0)
                        {
                            prodDiariaTranspPedido.ObservacaoCHIC = icDT[0].hatchinf.Trim();
                            prodDiariaTranspPedido.ObsProgramacao = icDT[0].comments.Trim();
                        }
                        else
                        {
                            prodDiariaTranspPedido.ObservacaoCHIC = "";
                            prodDiariaTranspPedido.ObsProgramacao = "";
                        }

                        #region Debicagem

                        int existeDebicagem = bDTCommercial.Where(w => w.item == "169").Count();
                        if (existeDebicagem > 0)
                            prodDiariaTranspPedido.Debicagem = "X";
                        else
                            prodDiariaTranspPedido.Debicagem = "";

                        #endregion

                        if (slDT.Count > 0)
                        {
                            if (slDT[0].salesman.Trim().Length > 15)
                                prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim().Substring(0, 15) + "...";
                            else
                                prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim();

                            prodDiariaTranspPedido.Empresa = slDT[0].inv_comp.Trim();
                        }
                        else
                        {
                            prodDiariaTranspPedido.NomeRepresentante = "";
                            prodDiariaTranspPedido.Empresa = "BR";
                        }

                        prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                        hlbappService.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);
                        InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Inclusão", "Nova inclusão do pedido no Web!");

                        #endregion
                    }
                }

                #endregion

                hlbappService.SaveChanges();

                #endregion

                #region Insere Pedidos da Programação Diária de Transporte - CHIC Avós (DESATIVADA DEVIDO A MIGRAÇÃO AO POULTRY SUITE)

                //ordersParentTableAdapter opTA = new ordersParentTableAdapter();
                //CHICParentDataSet.ordersParentDataTable opDT = new CHICParentDataSet.ordersParentDataTable();

                //itemsParentTableAdapter ipTA = new itemsParentTableAdapter();
                //CHICParentDataSet.itemsParentDataTable ipDT = new CHICParentDataSet.itemsParentDataTable();
                //ipTA.Fill(ipDT);

                //#region Deleta Pedidos não Existentes

                //var listaPedidosDataAvos = hlbappService.Prog_Diaria_Transp_Pedidos
                //    .Where(w => w.DataProgramacao == data
                //        //&& w.CHICNum == "74040"
                //        && w.CHICOrigem == "Avós"
                //        ).ToList();

                //foreach (var item in listaPedidosDataAvos)
                //{
                //    if (item.CHICNum != null && item.CHICNum != "")
                //    {
                //        opTA.FillByOrderNo(opDT, item.CHICNum);

                //        if (opDT.Count == 0)
                //        {
                //            hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                //        }
                //        else
                //        {
                //            #region Carrega Itens CHIC

                //            CHICParentDataSet.ordersParentRow opRow = opDT.FirstOrDefault();
                //            bookedParentTableAdapter bpTA = new bookedParentTableAdapter();
                //            CHICParentDataSet.bookedParentDataTable bpDT = new CHICParentDataSet.bookedParentDataTable();
                //            bpTA.FillByOrderNo(bpDT, opRow.orderno);

                //            #endregion

                //            #region Ajusta pedidos de Ovos como Nascimento e não Retirada

                //            DateTime setDateErro = data.AddDays(-21);
                //            int existeErro = bpDT.Where(w => ipDT.Any(a => a.item_no == w.item
                //                && (a.form.Substring(0, 1).Equals("H")))
                //                && w.cal_date == setDateErro)
                //            .Count();

                //            if (existeErro > 0)
                //            {
                //                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                //            }

                //            #endregion

                //            #region Ajusta Pedido que foi alterada data

                //            existeErro = 0;
                //            existeErro = bpDT.Where(w => ipDT.Any(a => a.item_no == w.item
                //                && (a.form.Substring(0, 1).Equals("D")))
                //                && w.cal_date != setDateErro)
                //            .Count();

                //            if (existeErro > 0)
                //            {
                //                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                //            }

                //            #endregion

                //            #region Verifica se a Linhagem ainda existe no Pedido no mesmo incubatório

                //            existeErro = 0;
                //            existeErro = bpDT.Where(w => ipDT.Any(a => a.item_no == w.item
                //                    && a.variety.Trim() == item.Linhagem
                //                    && a.form.Trim() == item.Produto)
                //                    && w.location.Trim() == item.LocalNascimento)
                //                .Count();

                //            if (existeErro == 0)
                //            {
                //                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                //            }

                //            #endregion

                //            #region Verifica se o Incubatório ainda existe no Pedido

                //            existeErro = 0;
                //            existeErro = bpDT.Where(w => w.location.Trim() == item.LocalNascimento
                //                    && item.LocalNascimento != null)
                //                .Count();

                //            if (existeErro == 0)
                //            {
                //                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                //            }

                //            #endregion
                //        }
                //    }
                //}

                //hlbappService.SaveChanges();

                //#endregion

                //#region Pintos

                //#region Filtra Pedidos

                ////DateTime data = DateTime.Today;
                ////oTA.FillSalesByHatchDate2(oDT, data);
                //setDate = data.AddDays(-21);
                //opTA.FillSalesByCalDate(opDT, setDate);

                //var listaOrdersParent = opDT
                //    //.Where(w => w.orderno == "81060")
                //    .ToList();

                //#endregion

                //foreach (var order in listaOrdersParent)
                //{
                //    #region Dados Item

                //    ipTA.Fill(ipDT);

                //    bookedParentTableAdapter bpTACommercial =
                //        new Data.CHICParentDataSetTableAdapters.bookedParentTableAdapter();
                //    CHICParentDataSet.bookedParentDataTable bpDTCommercial = new CHICParentDataSet.bookedParentDataTable();
                //    bpTACommercial.FillByOrderNo(bpDTCommercial, order.orderno);

                //    var listaItens = bpDTCommercial
                //        .Where(w => ipDT.Any(a => a.item_no == w.item
                //            && (a.form.Substring(0, 1).Equals("P")
                //            //&& a.variety != "DKBW" && a.variety != "DKBB")
                //            )))
                //        .Join(
                //            ipDT,
                //            b => b.item,
                //            i => i.item_no,
                //            (b, i) => new { BOOKED = b, ITEM = i })
                //        .GroupBy(g => new
                //        {
                //            //g.BOOKED.item,
                //            g.BOOKED.location,
                //            g.ITEM.variety,
                //            g.ITEM.form
                //        })
                //        .Select(s => new
                //        {
                //            //s.Key.item,
                //            s.Key.location,
                //            s.Key.variety,
                //            s.Key.form,
                //            qtde = s.Sum(u => u.BOOKED.quantity),
                //            price = s.Max(m => m.BOOKED.price)
                //        })
                //        .ToList();

                //    #endregion

                //    #region Dados Custom Table

                //    int_commParentTableAdapter icpTA = new int_commParentTableAdapter();
                //    CHICParentDataSet.int_commParentDataTable icpDT = new CHICParentDataSet.int_commParentDataTable();
                //    icpTA.FillByOrderNo(icpDT, order.orderno);

                //    salesmanParentTableAdapter slpTA = new salesmanParentTableAdapter();
                //    CHICParentDataSet.salesmanParentDataTable slpDT = new CHICParentDataSet.salesmanParentDataTable();
                //    slpTA.FillByCod(slpDT, order.salesrep);

                //    custParentTableAdapter cpTA = new custParentTableAdapter();
                //    CHICParentDataSet.custParentDataTable cpDT = new CHICParentDataSet.custParentDataTable();
                //    cpTA.FillByCustNo(cpDT, order.cust_no);

                //    if (slpDT.Count == 0)
                //    {
                //        slpTA.FillByCod(slpDT, cpDT[0].salesman);
                //    }

                //    string codigoCliente = order.cust_no.Trim();

                //    #region Verifica Transportadora

                //    CIDADE cidadeEntidade = apolo.CIDADE
                //        .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                //            && a.EntCod == codigoCliente)).FirstOrDefault();

                //    CHICParentDataSet.int_commParentRow ipR = icpDT.FirstOrDefault();
                //    string empresaTransportadoraCHIC = "";
                //    //if (order.cust_no.Trim() == "0000178")
                //    if (cpDT.FirstOrDefault().name.Contains("HY LINE DO BRASIL"))
                //    {
                //        empresaTransportadoraCHIC = "AI";
                //    }
                //    else
                //    {
                //        empresaTransportadoraCHIC = "EX";
                //    }

                //    #endregion

                //    #endregion

                //    foreach (var item in listaItens)
                //    {
                //        #region Verifica Se existe já lançado

                //        string linhagem = item.variety.Trim();
                //        string produto = item.form.Trim();

                //        bool entrou = false;
                //        if (order.orderno.Equals("69400"))
                //            entrou = true;

                //        int? numVeiculo = 0;
                //        string embalagem = "";
                //        string status = "Pendente";
                //        string observacao = "";
                //        int? ordem = 0;
                //        string inicioCarregamentoEsperado = "";
                //        string chegadaClienteEsperada = "";
                //        int? km = 0;
                //        string inicioCarregamentoReal = "";
                //        string chegadaCarregamentoReal = "";
                //        int? qtdCaixas = 0;

                //        List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                //            .Where(w => w.CHICNum == order.orderno.Trim()
                //            && w.Linhagem == linhagem && w.Produto == produto
                //            && w.LocalNascimento == item.location
                //            && w.CHICOrigem == "Avós"
                //            ).ToList();

                //        if (listProdDiariaTranspPedido.Count > 1)
                //        {
                //            foreach (var pedido in listProdDiariaTranspPedido)
                //            {
                //                numVeiculo = pedido.NumVeiculo;
                //                embalagem = pedido.Embalagem;
                //                status = pedido.Status;
                //                observacao = pedido.Observacao;
                //                ordem = pedido.Ordem;
                //                inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                //                chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                //                km = pedido.KM;
                //                inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                //                chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                //                qtdCaixas = pedido.QuantidadeCaixa;

                //                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                //            }
                //        }

                //        hlbappService.SaveChanges();

                //        Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                //            .Where(w => w.CHICNum == order.orderno.Trim()
                //            && w.Linhagem == linhagem && w.Produto == produto
                //            && w.LocalNascimento == item.location
                //            && w.CHICOrigem == "Avós"
                //            ).FirstOrDefault();

                //        if (prodDiariaTranspPedido != null)
                //        {
                //            numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                //            embalagem = prodDiariaTranspPedido.Embalagem;
                //            status = prodDiariaTranspPedido.Status;
                //            observacao = prodDiariaTranspPedido.Observacao;
                //            ordem = prodDiariaTranspPedido.Ordem;
                //            inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                //            chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                //            km = prodDiariaTranspPedido.KM;
                //            inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                //            chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                //            qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;

                //            hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                //        }

                //        #endregion

                //        #region Insere Programação Diária

                //        prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                //        prodDiariaTranspPedido.CHICOrigem = "Avós";
                //        prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                //        prodDiariaTranspPedido.Embalagem = embalagem;
                //        prodDiariaTranspPedido.Status = status;
                //        prodDiariaTranspPedido.Observacao = observacao;
                //        prodDiariaTranspPedido.Ordem = ordem;
                //        prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                //        prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                //        prodDiariaTranspPedido.KM = km;
                //        prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                //        prodDiariaTranspPedido.ChegadaClienteReal = chegadaCarregamentoReal;
                //        prodDiariaTranspPedido.QuantidadeCaixa = qtdCaixas;

                //        prodDiariaTranspPedido.DataProgramacao = bpDTCommercial[0].cal_date.AddDays(21);
                //        prodDiariaTranspPedido.CodigoCliente = codigoCliente;

                //        ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                //            .FirstOrDefault();

                //        //if (entidade.EntNome.Length > 15)
                //        ////    prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0,15) + "...";
                //        //else
                //        if (entidade != null)
                //            prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                //        else
                //            prodDiariaTranspPedido.NomeCliente = "NÃO EXISTE ENTIDADE "
                //                + prodDiariaTranspPedido.CodigoCliente + " NO APOLO! VERIFICAR COM A PROGRAMAÇÃO!";
                //        prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);

                //        #region Local de Entrega

                //        if (entidade != null)
                //        {
                //            CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                //            if (cidade != null)
                //                prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                //            else
                //                prodDiariaTranspPedido.LocalEntrega = "";
                //        }
                //        else
                //            prodDiariaTranspPedido.LocalEntrega = "";

                //        #endregion

                //        prodDiariaTranspPedido.Produto = produto;
                //        prodDiariaTranspPedido.Linhagem = linhagem;
                //        prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                //        prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                //        prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                //        if (entidade != null)
                //        {
                //            ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                //                .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                //            if (fone != null)
                //            {
                //                prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                //            }
                //        }

                //        prodDiariaTranspPedido.DataEntrega = order.del_date;
                //        prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                //        if (icpDT.Count > 0)
                //            prodDiariaTranspPedido.ObservacaoCHIC = icpDT[0].hatchinf.Trim();
                //        else
                //            prodDiariaTranspPedido.ObservacaoCHIC = "";

                //        #region Debicagem

                //        int existeDebicagem = bpDTCommercial.Where(w => w.item == "169").Count();
                //        if (existeDebicagem > 0)
                //            prodDiariaTranspPedido.Debicagem = "X";
                //        else
                //            prodDiariaTranspPedido.Debicagem = "";

                //        #endregion

                //        if (slpDT.Count > 0)
                //        {
                //            prodDiariaTranspPedido.NomeRepresentante = slpDT[0].salesman.Trim();
                //            prodDiariaTranspPedido.Empresa = slpDT[0].inv_comp.Trim();
                //        }
                //        else
                //        {
                //            prodDiariaTranspPedido.NomeRepresentante = "";
                //            prodDiariaTranspPedido.Empresa = order.salesrep;
                //        }

                //        prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                //        hlbappService.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);

                //        #endregion
                //    }
                //}

                //#endregion

                //#region Ovos

                //#region Filtra Pedidos

                ////DateTime data = DateTime.Today;
                ////oTA.FillSalesByHatchDate2(oDT, data);
                //setDate = data;
                //opTA.FillSalesByCalDate(opDT, setDate);

                //listaOrdersParent = opDT
                //    //.Where(w => w.orderno == "59994")
                //    .ToList();

                //#endregion

                //foreach (var order in listaOrders)
                //{
                //    #region Dados Item

                //    bookedParentTableAdapter bpTACommercial =
                //        new Data.CHICParentDataSetTableAdapters.bookedParentTableAdapter();
                //    CHICParentDataSet.bookedParentDataTable bpDTCommercial = new CHICParentDataSet.bookedParentDataTable();
                //    bpTACommercial.FillByOrderNo(bpDTCommercial, order.orderno);

                //    var listaItens = bpDTCommercial
                //        .Where(w => ipDT.Any(a => a.item_no == w.item
                //            && (a.form.Substring(0, 1).Equals("H")
                //            //&& a.variety != "DKBW" && a.variety != "DKBB"
                //            )))
                //        .Join(
                //            ipDT,
                //            b => b.item,
                //            i => i.item_no,
                //            (b, i) => new { BOOKED = b, ITEM = i })
                //        .GroupBy(g => new
                //        {
                //            //g.BOOKED.item,
                //            g.BOOKED.location,
                //            g.ITEM.variety,
                //            g.ITEM.form
                //        })
                //        .Select(s => new
                //        {
                //            //s.Key.item,
                //            s.Key.location,
                //            s.Key.variety,
                //            s.Key.form,
                //            qtde = s.Sum(u => u.BOOKED.quantity),
                //            price = s.Max(m => m.BOOKED.price)
                //        })
                //        .ToList();

                //    #endregion

                //    #region Dados Custom Table

                //    int_commParentTableAdapter icpTA = new int_commParentTableAdapter();
                //    CHICParentDataSet.int_commParentDataTable icpDT = new CHICParentDataSet.int_commParentDataTable();
                //    icpTA.FillByOrderNo(icpDT, order.orderno);

                //    salesmanParentTableAdapter slpTA = new salesmanParentTableAdapter();
                //    CHICParentDataSet.salesmanParentDataTable slpDT = new CHICParentDataSet.salesmanParentDataTable();
                //    slpTA.FillByCod(slpDT, order.salesrep);

                //    custParentTableAdapter cpTA = new custParentTableAdapter();
                //    CHICParentDataSet.custParentDataTable cpDT = new CHICParentDataSet.custParentDataTable();
                //    cpTA.FillByCustNo(cpDT, order.cust_no);

                //    string codigoCliente = order.cust_no.Trim();

                //    #region Verifica Transportadora

                //    CIDADE cidadeEntidade = apolo.CIDADE
                //        .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                //            && a.EntCod == codigoCliente)).FirstOrDefault();

                //    CHICParentDataSet.int_commParentRow ipR = icpDT.FirstOrDefault();
                //    string empresaTransportadoraCHIC = "";
                //    if (cidadeEntidade.UfSigla == "EX")
                //    {
                //        empresaTransportadoraCHIC = "EX";
                //    }
                //    else
                //    {
                //        //if (order.cust_no.Trim() == "0000178")
                //        if (cpDT.FirstOrDefault().name.Contains("HY LINE DO BRASIL"))
                //            empresaTransportadoraCHIC = "AI";
                //    }

                //    #endregion

                //    #endregion

                //    foreach (var item in listaItens)
                //    {
                //        #region Verifica Se existe já lançado

                //        string linhagem = item.variety;
                //        string produto = item.form;

                //        int? numVeiculo = 0;
                //        string embalagem = "";
                //        string status = "Pendente";
                //        string observacao = "";
                //        int? ordem = 0;
                //        string inicioCarregamentoEsperado = "";
                //        string chegadaClienteEsperada = "";
                //        int? km = 0;
                //        string inicioCarregamentoReal = "";
                //        string chegadaClienteReal = "";
                //        string chegadaCarregamentoReal = "";
                //        int? qtdCaixas = 0;

                //        List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                //            .Where(w => w.CHICNum == order.orderno.Trim()
                //            && w.Linhagem == linhagem && w.Produto == produto
                //            //&& w.LocalNascimento == item.location).ToList();
                //            && w.CHICOrigem == "Avós"
                //            ).ToList();

                //        if (listProdDiariaTranspPedido.Count > 1)
                //        {
                //            foreach (var pedido in listProdDiariaTranspPedido)
                //            {
                //                numVeiculo = pedido.NumVeiculo;
                //                embalagem = pedido.Embalagem;
                //                status = pedido.Status;
                //                observacao = pedido.Observacao;
                //                ordem = pedido.Ordem;
                //                inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                //                chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                //                km = pedido.KM;
                //                inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                //                chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                //                qtdCaixas = pedido.QuantidadeCaixa;

                //                hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                //            }
                //        }

                //        hlbappService.SaveChanges();

                //        Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbappService.Prog_Diaria_Transp_Pedidos
                //            .Where(w => w.CHICNum == order.orderno.Trim()
                //            && w.Linhagem == linhagem && w.Produto == produto
                //            //&& w.LocalNascimento == item.location).FirstOrDefault();
                //            && w.CHICOrigem == "Avós"
                //            ).FirstOrDefault();

                //        if (prodDiariaTranspPedido != null)
                //        {
                //            numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                //            embalagem = prodDiariaTranspPedido.Embalagem;
                //            status = prodDiariaTranspPedido.Status;
                //            observacao = prodDiariaTranspPedido.Observacao;
                //            ordem = prodDiariaTranspPedido.Ordem;
                //            inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                //            chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                //            km = prodDiariaTranspPedido.KM;
                //            inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                //            chegadaClienteReal = prodDiariaTranspPedido.ChegadaClienteReal;
                //            chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                //            qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;

                //            hlbappService.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                //        }

                //        #endregion

                //        #region Insere Programação Diária

                //        prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                //        prodDiariaTranspPedido.CHICOrigem = "Avós";
                //        prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                //        prodDiariaTranspPedido.Embalagem = embalagem;
                //        prodDiariaTranspPedido.Status = status;
                //        prodDiariaTranspPedido.Observacao = observacao;
                //        prodDiariaTranspPedido.Ordem = ordem;
                //        prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                //        prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                //        prodDiariaTranspPedido.KM = km;
                //        prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                //        prodDiariaTranspPedido.ChegadaClienteReal = chegadaClienteReal;

                //        prodDiariaTranspPedido.DataProgramacao = bpDTCommercial[0].cal_date;
                //        prodDiariaTranspPedido.CodigoCliente = order.cust_no.Trim();

                //        ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                //            .FirstOrDefault();

                //        if (entidade != null)
                //        {
                //            if (entidade.EntNome.Length > 15)
                //                prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0, 15) + "...";
                //            else
                //                prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                //        }
                //        prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);

                //        #region Local de Entrega

                //        if (entidade != null)
                //        {
                //            CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                //            //prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                //            //    cidade.PaisSigla;
                //            prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                //        }

                //        #endregion

                //        prodDiariaTranspPedido.Produto = produto;
                //        prodDiariaTranspPedido.Linhagem = linhagem;
                //        prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                //        prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                //        prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                //        if (entidade != null)
                //        {
                //            ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                //                .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                //            if (fone != null)
                //            {
                //                prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                //            }
                //        }

                //        prodDiariaTranspPedido.DataEntrega = order.del_date;
                //        prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                //        if (icpDT.Count > 0)
                //            prodDiariaTranspPedido.ObservacaoCHIC = icpDT[0].hatchinf.Trim();
                //        else
                //            prodDiariaTranspPedido.ObservacaoCHIC = "";

                //        #region Debicagem

                //        int existeDebicagem = bpDTCommercial.Where(w => w.item == "169").Count();
                //        if (existeDebicagem > 0)
                //            prodDiariaTranspPedido.Debicagem = "X";
                //        else
                //            prodDiariaTranspPedido.Debicagem = "";

                //        #endregion

                //        if (slpDT[0].salesman.Trim().Length > 15)
                //            prodDiariaTranspPedido.NomeRepresentante = slpDT[0].salesman.Trim().Substring(0, 15) + "...";
                //        else
                //            prodDiariaTranspPedido.NomeRepresentante = slpDT[0].salesman.Trim();

                //        #region Verifica Transportadora

                //        prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                //        #endregion

                //        prodDiariaTranspPedido.Empresa = slpDT[0].inv_comp.Trim();

                //        hlbappService.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);

                //        #endregion
                //    }
                //}

                //#endregion

                //hlbappService.SaveChanges();

                #endregion

                #region Insere Veículos da Programação Diária de Transporte

                AtualizaValoresVeiculos(data);

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = " - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                this.EventLog.WriteEntry("Erro ao realizar Importação da Programação Diária de Transportes do CHIC do dia "
                    + data.ToShortDateString() + ": " + retorno, EventLogEntryType.Error, 10);

                return retorno;
            }
        }

        public string AtualizarProgDiariaTranspDiaNascimentoPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            string retorno = "";

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            var listaPedidosData = hlbappService.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.DataProgramacao >= dataInicial
                            && w.DataProgramacao <= dataFinal
                            ).ToList();

            foreach (var item in listaPedidosData)
            {
                retorno = AtualizarProgDiariaTranspDiaNascimento(Convert.ToDateTime(item.DataProgramacao));
            }

            return retorno;
        }

        public string ImportacaoProgDiariaTranspCHICPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            string retorno = "";

            #region Insere LOG de Atualização

            HLBAPPServiceEntities hlbappSession = new HLBAPPServiceEntities();
            LOG_Atualizacao_CHIC_SQLServer logAtualizacao = new LOG_Atualizacao_CHIC_SQLServer();
            logAtualizacao.DataHoraInicio = DateTime.Now;
            logAtualizacao.Usuario = "Serviço";
            logAtualizacao.Periodo = "Geral";
            hlbappSession.LOG_Atualizacao_CHIC_SQLServer.AddObject(logAtualizacao);
            hlbappSession.SaveChanges();

            #endregion

            while (dataInicial <= dataFinal)
			{
                retorno = AtualizarProgDiariaTranspDiaNascimento(dataInicial);
                if (retorno != "") break;
                dataInicial = dataInicial.AddDays(1);
			}

            #region Finaliza LOG de Atualização

            if (retorno == "")
            {
                logAtualizacao.DataHoraFim = DateTime.Now;
                logAtualizacao.Observacao = "Atualizado com Sucesso!";
                hlbappSession.SaveChanges();
            }
            else
            {
                logAtualizacao.DataHoraFim = DateTime.Now;
                logAtualizacao.Observacao = "Erro ao realizar Importação da Programação Diária de Transportes: " + retorno;
                hlbappSession.SaveChanges();
            }

            #endregion

            return retorno;
        }

        public void InsereLOG_Prog_Diaria_Transp_Pedidos(Prog_Diaria_Transp_Pedidos pdtp, string operacao, string observacao)
        {
            HLBAPPServiceEntities hlbappSession = new HLBAPPServiceEntities();

            LOG_Prog_Diaria_Transp_Pedidos log = new LOG_Prog_Diaria_Transp_Pedidos();
            log.DataHora = DateTime.Now;
            log.Usuario = "Serviço";
            log.Operacao = operacao;
            log.ObsLog = observacao;
            log.DataProgramacao = pdtp.DataProgramacao;
            log.CodigoCliente = pdtp.CodigoCliente;
            log.NomeCliente = pdtp.NomeCliente;
            log.NumVeiculo = pdtp.NumVeiculo;
            log.Quantidade = pdtp.Quantidade;
            log.LocalEntrega = pdtp.LocalEntrega;
            log.Produto = pdtp.Produto;
            log.Linhagem = pdtp.Linhagem;
            log.Embalagem = pdtp.Embalagem;
            log.ValorTotal = pdtp.ValorTotal;
            log.NFEspecie = pdtp.NFEspecie;
            log.NFSerie = pdtp.NFSerie;
            log.NFNum = pdtp.NFNum;
            log.CHICNum = pdtp.CHICNum;
            log.LocalNascimento = pdtp.LocalNascimento;
            log.TelefoneCliente = pdtp.TelefoneCliente;
            log.InicioCarregamentoEsperado = pdtp.InicioCarregamentoEsperado;
            log.DataEntrega = pdtp.DataEntrega;
            log.ChegadaClienteEsperado = pdtp.ChegadaClienteEsperado;
            log.KM = pdtp.KM;
            log.CodigoRepresentante = pdtp.CodigoRepresentante;
            log.NomeRepresentante = pdtp.NomeRepresentante;
            log.InicioCarregamentoReal = pdtp.InicioCarregamentoReal;
            log.ChegadaClienteReal = pdtp.ChegadaClienteReal;
            log.Observacao = pdtp.Observacao;
            log.Status = pdtp.Status;
            log.ObservacaoCHIC = pdtp.ObservacaoCHIC;
            log.Debicagem = pdtp.Debicagem;
            log.Ordem = pdtp.Ordem;
            log.EmpresaTranportador = pdtp.EmpresaTranportador;
            log.Empresa = pdtp.Empresa;
            log.QuantidadeCaixa = pdtp.QuantidadeCaixa;
            log.CHICOrigem = pdtp.CHICOrigem;
            log.NumRoteiroEntregaFluig = pdtp.NumRoteiroEntregaFluig;
            log.DataChegadaClienteReal = pdtp.DataChegadaClienteReal;
            log.IDProgDiariaTranspPedidos = pdtp.ID;
            log.QtdeVendida = pdtp.QtdeVendida;
            log.QtdeBonificada = pdtp.QtdeBonificada;
            log.QtdeReposicao = pdtp.QtdeReposicao;
            log.QtdeSobra = pdtp.QtdeSobra;
            log.MotivoSobra = pdtp.MotivoSobra;
            log.CHICNumReposicao = pdtp.CHICNumReposicao;
            log.MotivoReposicao = pdtp.MotivoReposicao;
            log.IDPedidoVenda = pdtp.IDPedidoVenda;
            log.EnderEntSeq = pdtp.EnderEntSeq;
            log.PercBonificacao = pdtp.PercBonificacao;
            log.PrecoProduto = pdtp.PrecoProduto;
            log.ObsProgramacao = pdtp.ObsProgramacao;
            log.CondicaoPagamento = pdtp.CondicaoPagamento;

            hlbappSession.LOG_Prog_Diaria_Transp_Pedidos.AddObject(log);
            hlbappSession.SaveChanges();
        }

        #endregion

        #region Pedidos WEB

        public static decimal CalculaValoresVacinasServicosNovoPV(string vacinaServico, DateTime dataInicial,
            DateTime dataFinal, string empresa, string tipo)
        {
            HLBAPPServiceEntities hlbappSession = new HLBAPPServiceEntities();

            decimal valorVacinasServicos = 0;

            Tabela_Precos tabelaPreco = hlbappSession.Tabela_Precos
                .Where(w => w.Tipo == tipo && w.Produto == vacinaServico
                    && w.Regiao == "Todas" && w.Empresa == empresa
                    && dataInicial >= w.DataInicial && dataFinal <= w.DataFinal)
                .FirstOrDefault();

            if (tabelaPreco != null)
            {
                valorVacinasServicos = Convert.ToDecimal(tabelaPreco.ValorNormal);
            }

            return valorVacinasServicos;
        }

        public decimal CalculaValorLinhagemTabelaPrecoNovoPedido(string codigoCliente, string linhagem,
            DateTime dataInicial, DateTime dataFinal, string condPag, int qtdTotalMesmaData,
            string empresa)
        {
            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            decimal valor = 0;

            if (!condPag.Equals("PAGTO ANTECIPADO")) condPag = "Faturamento";

            CIDADE cidade = apolo.CIDADE
                .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                    && a.EntCod == codigoCliente)).FirstOrDefault();

            UNID_FEDERACAO uf = apolo
                .UNID_FEDERACAO.Where(w => w.UfSigla == cidade.UfSigla).FirstOrDefault();

            Tabela_Precos precoLinhagem = hlbappService
                .Tabela_Precos.Where(w => w.Tipo == condPag
                    && w.Produto == linhagem
                    && w.Regiao == uf.UfSigla && w.Empresa == empresa
                    && dataInicial >= w.DataInicial && dataFinal <= w.DataFinal)
                .FirstOrDefault();

            if (precoLinhagem == null)
            {
                precoLinhagem =
                    hlbappService.Tabela_Precos.Where(w => w.Tipo == condPag && w.Produto == linhagem
                        && w.Regiao == uf.UfRegGeog && w.Empresa == empresa
                        && dataInicial >= w.DataInicial && dataFinal <= w.DataFinal)
                .FirstOrDefault();
            }

            if (precoLinhagem != null)
            {
                if (qtdTotalMesmaData < 5000)
                    valor = Convert.ToDecimal(precoLinhagem.ValorMenor5000Aves);
                else
                    valor = Convert.ToDecimal(precoLinhagem.ValorNormal);
            }

            return valor;
        }

        public void InsereLOGPVWeb(int idPedidoVenda, string usuario, string operacao, string motivo)
        {
            HLBAPPServiceEntities hlbappService = new HLBAPPServiceEntities();
            hlbappService.CommandTimeout = 1000;

            Pedido_Venda pedVenda = hlbappService.Pedido_Venda.Where(w => w.ID == idPedidoVenda).FirstOrDefault();

            #region Insere LOG - Pedido_Venda

            LOG_Pedido_Venda logPV = new LOG_Pedido_Venda();

            logPV = new LOG_Pedido_Venda();
            logPV.DataPedido = pedVenda.DataPedido;
            logPV.Usuario = usuario;
            logPV.DataHora = DateTime.Now;
            logPV.CodigoCliente = pedVenda.CodigoCliente;
            logPV.OvosBrasil = pedVenda.OvosBrasil;
            logPV.CondicaoPagamento = pedVenda.CondicaoPagamento;
            logPV.Observacoes = pedVenda.Observacoes;
            logPV.Vendedor = pedVenda.Vendedor;
            logPV.Status = pedVenda.Status;
            logPV.Operacao = operacao;
            logPV.IDPedidoVenda = pedVenda.ID;
            logPV.Motivo = motivo;
            logPV.Projecao = pedVenda.Projecao;

            hlbappService.LOG_Pedido_Venda.AddObject(logPV);
            hlbappService.SaveChanges();

            #endregion

            #region Insere LOG - Item_Pedido_Venda

            var listaItensWEB = hlbappService.Item_Pedido_Venda
                .Where(w => w.IDPedidoVenda == pedVenda.ID
                    && w.OrderNoCHIC != "Cancelado")
                .ToList();

            foreach (var item in listaItensWEB)
            {
                LOG_Item_Pedido_Venda logItemPV = new LOG_Item_Pedido_Venda();
                logItemPV.IDPedidoVenda = item.IDPedidoVenda;
                logItemPV.Sequencia = item.Sequencia;
                logItemPV.ProdCodEstr = item.ProdCodEstr;
                logItemPV.DataEntregaInicial = item.DataEntregaInicial;
                logItemPV.DataEntregaFinal = item.DataEntregaFinal;
                logItemPV.QtdeLiquida = item.QtdeLiquida;
                logItemPV.PercBonificacao = item.PercBonificacao;
                logItemPV.QtdeBonificada = item.QtdeBonificada;
                logItemPV.QtdeReposicao = item.QtdeReposicao;
                logItemPV.PrecoUnitario = item.PrecoUnitario;
                logItemPV.DataHora = DateTime.Now;
                logItemPV.Operacao = operacao;
                logItemPV.IDItPedVenda = item.ID;
                logItemPV.IDLogPedidoVenda = logPV.ID;
                logItemPV.OrderNoCHIC = item.OrderNoCHIC;
                logItemPV.OrderNoCHICReposicao = item.OrderNoCHICReposicao;
                logItemPV.PrecoPinto = item.PrecoPinto;
                logItemPV.TipoReposicao = item.TipoReposicao;
                logItemPV.ValorTotal = item.ValorTotal;

                hlbappService.LOG_Item_Pedido_Venda.AddObject(logItemPV);
            }

            #endregion

            #region Insere LOG - Vacinas Primárias

            Vacinas_Primaria_Pedido_Venda vacPrimObj = hlbappService.Vacinas_Primaria_Pedido_Venda
                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

            if (vacPrimObj != null)
            {
                LOG_Vacinas_Primaria_Pedido_Venda logVacPrim = new LOG_Vacinas_Primaria_Pedido_Venda();

                logVacPrim.IDPedidoVenda = vacPrimObj.IDPedidoVenda;
                logVacPrim.ProdCodEstr = vacPrimObj.ProdCodEstr;
                logVacPrim.DataHora = DateTime.Now;
                logVacPrim.Operacao = operacao;
                logVacPrim.IDVacPrimPedVenda = vacPrimObj.ID;
                logVacPrim.IDLogPedidoVenda = logPV.ID;
                logVacPrim.PrecoUnitario = vacPrimObj.PrecoUnitario;
                logVacPrim.Bonificada = vacPrimObj.Bonificada;

                hlbappService.LOG_Vacinas_Primaria_Pedido_Venda.AddObject(logVacPrim);
                hlbappService.SaveChanges();

                #region Insere LOG - Vacina_Secundaria_Pedido_Venda

                var listaVacSec = hlbappService.Vacinas_Secundaria_Pedido_Venda
                    .Where(w => w.IDVacPrimPedVenda == vacPrimObj.ID).ToList();

                foreach (var vacSec in listaVacSec)
                {
                    LOG_Vacinas_Secundaria_Pedido_Venda logVacSec = new LOG_Vacinas_Secundaria_Pedido_Venda();
                    logVacSec.IDVacPrimPedVenda = vacSec.IDVacPrimPedVenda;
                    logVacSec.Sequencia = vacSec.Sequencia;
                    logVacSec.ProdCodEstr = vacSec.ProdCodEstr;
                    logVacSec.DataHora = DateTime.Now;
                    logVacSec.Operacao = operacao;
                    logVacSec.IDVacSecPedVenda = vacSec.ID;
                    logVacSec.IDVacPrimLogPedidoVenda = logVacPrim.ID;
                    logVacSec.PrecoUnitario = vacSec.PrecoUnitario;
                    logVacSec.Bonificada = vacSec.Bonificada;

                    hlbappService.LOG_Vacinas_Secundaria_Pedido_Venda.AddObject(logVacSec);
                }

                #endregion
            }

            #endregion

            #region Insere LOG - Servico_Pedido_Venda

            Servicos_Pedido_Venda serv = hlbappService.Servicos_Pedido_Venda
                .Where(w => w.IDPedidoVenda == pedVenda.ID).FirstOrDefault();

            if (serv != null)
            {
                LOG_Servicos_Pedido_Venda logServ = new LOG_Servicos_Pedido_Venda();
                logServ.IDPedidoVenda = serv.IDPedidoVenda;
                logServ.ProdCodEstr = serv.ProdCodEstr;
                logServ.PercAplicacaoServico = serv.PercAplicacaoServico;
                logServ.DataHora = DateTime.Now;
                logServ.Operacao = operacao;
                logServ.IDServPedVenda = serv.ID;
                logServ.IDLogPedidoVenda = logPV.ID;
                logServ.PrecoUnitario = serv.PrecoUnitario;
                logServ.Bonificada = serv.Bonificada;

                hlbappService.LOG_Servicos_Pedido_Venda.AddObject(logServ);
            }

            #endregion

            hlbappService.SaveChanges();
        }

        #endregion

        #endregion

        #region Importação Embarcador (Sistema de Rastreamento de Transporte)

        // Método que importa os pedidos do CHIC para o Embarcador
        public void ImportaPedidosEmbarcadorTeste()
        {
            string erroPedido = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Localiza os pedidos a serem importados

                //DateTime data = DateTime.Today.AddDays(-21);
                DateTime data = Convert.ToDateTime("11/12/2017").AddDays(-21);

                CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                items.Fill(iDT);

                CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();
                //orders.FillSalesByCalDate(oDT, data);
                //orders.FillByOrderNo(oDT, "74903"); // Teste de entidade não cadastrada
                orders.FillByOrderNo(oDT, "75603");
                //orders.FillTesteEmbarcador(oDT);

                var listaPedidosCHIC = oDT
                    //.Where(w => w.orderno == "77495")
                    .ToList();

                #endregion

                #region Montagem e carregamento dos parâmetros

                //Cria um arrayList dos pedidos
                foreach (var pedidoCHIC in listaPedidosCHIC)
                {
                    erroPedido = pedidoCHIC.orderno;

                    OrderedDictionary parametros = new OrderedDictionary();

                    ArrayList arrayPedidos = new ArrayList();

                    OrderedDictionary pedido = new OrderedDictionary();

                    #region Carrega os dados dos itens

                    CHICDataSet.bookedDataTable bDT = new CHICDataSet.bookedDataTable();
                    booked.FillByOrderNo(bDT, pedidoCHIC.orderno);

                    var listaItensVendidos = bDT.Where(w =>
                        iDT.Any(a => a.item_no == w.item
                            //&& (a.form.Substring(0, 1) == "D" || a.form.Substring(0, 1) == "H")))
                            && (a.form.Substring(0, 1) == "D")))
                        .OrderBy(o => o.item_ord)
                        .GroupBy(g => new
                        {
                            g.item
                        })
                        .Select(s => new
                        {
                            s.Key.item,
                            qtde = s.Sum(m => m.quantity)
                        })
                        .ToList();

                    #endregion

                    #region Carrega dados do primeiro item

                    CHICDataSet.bookedRow bR = bDT.Where(w =>
                        iDT.Any(a => a.item_no == w.item
                            && (a.form.Substring(0, 1) == "D" || a.form.Substring(0, 1) == "H")))
                        .OrderByDescending(o => o.quantity).FirstOrDefault();

                    string incubatorio = "";
                    if (bR != null)
                        incubatorio = bR.location.Trim();

                    #endregion

                    //if ((incubatorio == "CH" || incubatorio == "AJ") && listaItensVendidos.Count > 0)
                    if (listaItensVendidos.Count > 0)
                    {
                        bool pedidoApagado = true;

                        #region Verifica se existe o pedido no Embarcador

                        OrderedDictionary parametrosBuscaPedido = new OrderedDictionary();
                        parametrosBuscaPedido.Add("CODIGO", pedidoCHIC.orderno);
                        parametrosBuscaPedido.Add("NR_ITEM", bR.item);
                        //parametrosBuscaPedido.Add("NR_ITEM", 1);

                        XDocument xmlExistePedido = Embarcador.Embarcador.buscaPedido(parametrosBuscaPedido);

                        #region Verifica retorno se existe pedido

                        foreach (XElement retorno in xmlExistePedido.Descendants("return"))
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                var listaSubItens = item.Nodes()
                                    .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                #region Carrega valores do retorno

                                XElement objIDCarga = (XElement)listaSubItens[0];
                                XElement objPlaca = (XElement)listaSubItens[1];
                                XElement objQuantidade = (XElement)listaSubItens[2];
                                XElement objPeso = (XElement)listaSubItens[3];

                                #endregion

                                // Se existe o pedido, deleta ele para inserir novamente
                                if (objQuantidade.Value != "" && objPeso.Value != "")
                                {
                                    bool cargaRemovida = true;

                                    if (objIDCarga.Value != "")
                                    {
                                        #region Se existe carga, primeiro remove da carga

                                        #region Carrega parâmetros

                                        OrderedDictionary parametrosRemovePedidosCarga = new OrderedDictionary();
                                        parametrosRemovePedidosCarga.Add("ID_CARGA", objIDCarga.Value);
                                        parametrosRemovePedidosCarga.Add("PLACA", objPlaca.Value);

                                        ArrayList arrayPedidosRC = new ArrayList();
                                        OrderedDictionary pedidoRC = new OrderedDictionary();
                                        pedidoRC.Add("CODIGO", pedidoCHIC.orderno);
                                        pedidoRC.Add("NR_ITEM", bR.item);
                                        //pedidoRC.Add("NR_ITEM", 1);
                                        arrayPedidosRC.Add(pedidoRC);

                                        parametrosRemovePedidosCarga.Add("PEDIDO", arrayPedidosRC);

                                        #endregion

                                        cargaRemovida = Convert.ToBoolean(Embarcador.Embarcador
                                            .removePedidosCarga(parametrosRemovePedidosCarga));

                                        #endregion
                                    }

                                    #region Deleta pedido

                                    if (cargaRemovida)
                                    {
                                        OrderedDictionary parametrosApagaPedido = new OrderedDictionary();
                                        parametrosApagaPedido.Add("CODIGO", pedidoCHIC.orderno);
                                        parametrosApagaPedido.Add("NR_ITEM", bR.item);
                                        //parametrosApagaPedido.Add("NR_ITEM", 1);
                                        parametrosApagaPedido.Add("APAGA_CARREGADO", false);

                                        pedidoApagado = Convert.ToBoolean(Embarcador.Embarcador
                                            .apagaPedido(parametrosApagaPedido));
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion

                        #endregion

                        if (!pedidoApagado)
                            return;

                        #region Carrega parâmetros do pedido

                        bool pinto = true;
                        CHICDataSet.itemsRow iPrimeiro = iDT.Where(w => w.item_no == bR.item).FirstOrDefault();
                        if (iPrimeiro.form.Substring(0, 1) == "H") pinto = false;

                        int codigoIncubatorio = 0;
                        if (incubatorio == "CH") codigoIncubatorio = 2;
                        else if (incubatorio == "PH") codigoIncubatorio = 4;
                        else if (incubatorio == "NM") codigoIncubatorio = 3;
                        else if (incubatorio == "AJ") codigoIncubatorio = 1;

                        //Cria um arrayList com os dados da unidade base
                        ArrayList arrayUnidadeBase = new ArrayList();
                        OrderedDictionary unidadeBase = new OrderedDictionary();
                        unidadeBase.Add("codigo", codigoIncubatorio);
                        unidadeBase.Add("diferenciador", "");
                        arrayUnidadeBase.Add(unidadeBase);
                        pedido.Add("base", arrayUnidadeBase);

                        //Cria um arrayList com os dados da origem
                        ArrayList arrayOrigem = new ArrayList();
                        OrderedDictionary origem = new OrderedDictionary();
                        origem.Add("codigo", codigoIncubatorio);
                        origem.Add("diferenciador", "");
                        arrayOrigem.Add(origem);
                        pedido.Add("origem", arrayOrigem);

                        int codigodestino = Convert.ToInt32(pedidoCHIC.cust_no.Trim());
                        //Cria um arrayList com os dados da unidade base
                        ArrayList arrayDestino = new ArrayList();
                        OrderedDictionary destino = new OrderedDictionary();
                        destino.Add("codigo", codigodestino);
                        destino.Add("diferenciador", "");
                        destino.Add("NOME", "");
                        destino.Add("CIDADE", "");
                        destino.Add("UF", "");
                        destino.Add("TELEFONE", "");
                        destino.Add("ENDERECO", "");
                        destino.Add("NUMERO", "");
                        destino.Add("BAIRRO", "");
                        destino.Add("CEP", "");
                        destino.Add("COMPLEMENTO", "");
                        destino.Add("LATITUDE", "");
                        destino.Add("LONGITUDE", "");
                        destino.Add("TIPO", "");
                        destino.Add("CPF_CPNJ", "");
                        destino.Add("PESSOA", "");
                        arrayDestino.Add(destino);
                        pedido.Add("destino", arrayDestino);

                        //Cria um arrayList com os dados do transbordo
                        ArrayList arrayTransbordo = new ArrayList();
                        OrderedDictionary transbordo = new OrderedDictionary();
                        transbordo.Add("codigo", "");
                        transbordo.Add("diferenciador", "");
                        arrayTransbordo.Add(transbordo);
                        pedido.Add("TRANSBORDO", arrayTransbordo);

                        pedido.Add("codigo", pedidoCHIC.orderno);
                        if (pinto)
                            pedido.Add("data_embarque", bR.cal_date.AddDays(21).ToShortDateString());
                        else
                            pedido.Add("data_embarque", bR.cal_date.ToShortDateString());
                        pedido.Add("data_entrega", pedidoCHIC.del_date.ToShortDateString());
                        pedido.Add("tipo_data_entrega", "E");
                        pedido.Add("tipo_pedido", 107);
                        pedido.Add("TIPO_CARGA", 1561); // Transporte de Pintos
                        //pedido.Add("TIPO_CARGA", 2623); // Transporte de Pintos - Integração
                        //pedido.Add("tipo_operacao", 1783);
                        pedido.Add("tipo_operacao", 2623);
                        pedido.Add("EMPACOTAMENTO", "");
                        pedido.Add("MICRO_REGIAO", 0);
                        pedido.Add("OBSERVACAO", "");
                        pedido.Add("representante", pedidoCHIC.salesrep.Trim());
                        pedido.Add("CLIENTE_UNICO", "");
                        pedido.Add("PRIORIDADE", "");
                        pedido.Add("COD_CARGA", "");
                        pedido.Add("ALIAS_CARGA", "");

                        #endregion

                        ArrayList arrayProdutos = new ArrayList();

                        int qtdeItens = 1;

                        foreach (var item in listaItensVendidos)
                        {
                            #region Carrega Dados do Item

                            CHICDataSet.itemsRow iR = iDT.Where(w => w.item_no == item.item).FirstOrDefault();
                            string descricao = "";
                            if (iR != null)
                                descricao = iR.variety.Trim() + " - " + iR.form.Trim();

                            // Quantidade criptografada para mascarar. Solicitado por Davi Nogueira.
                            int qtdeCrypto = 0;
                            qtdeCrypto = Convert.ToInt32(item.qtde) * 17;

                            #endregion

                            #region Carrega parâmetros do item

                            OrderedDictionary produto = new OrderedDictionary();
                            produto.Add("codigo", item.item);
                            //produto.Add("item", qtdeItens);
                            produto.Add("item", item.item);
                            produto.Add("descricao", descricao);
                            produto.Add("quantidade", qtdeCrypto);
                            produto.Add("PESO_UNITARIO", 1);
                            produto.Add("PESO_TOTAL", 1);
                            produto.Add("VOLUME", "");
                            produto.Add("OBSERVACAO", "");
                            produto.Add("TIPO_CARGA", 1561);
                            //if (pinto)
                            //    produto.Add("data_embarque", bR.cal_date.AddDays(21).ToShortDateString());
                            //else
                            //    produto.Add("data_embarque", bR.cal_date.ToShortDateString());
                            //produto.Add("data_entrega", pedidoCHIC.del_date.ToShortDateString());
                            if (pinto)
                            {
                                produto.Add("data_embarque", bR.cal_date.AddDays(21).ToShortDateString());
                                produto.Add("data_entrega", bR.cal_date.AddDays(21).ToShortDateString());
                            }
                            else
                            {
                                produto.Add("data_embarque", bR.cal_date.ToShortDateString());
                                produto.Add("data_entrega", bR.cal_date.ToShortDateString());
                            }
                            produto.Add("TIPO_DATA_ENTREGA", "E");
                            produto.Add("PRIORIDADE", "B");
                            arrayProdutos.Add(produto);

                            #endregion

                            qtdeItens = qtdeItens + 1;
                        }

                        pedido.Add("produto", arrayProdutos);

                        arrayPedidos.Add(pedido);

                        parametros.Add("pedido", arrayPedidos);

                        #region Execução do WebService

                        XDocument xmlRetorno = Embarcador.Embarcador.inserePedidosLote(parametros);

                        foreach (XElement retorno in xmlRetorno.Descendants("return"))
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                var listaSubItens = item.Nodes()
                                    .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                bool entidadeResolvida = false;

                                foreach (XElement subItem in listaSubItens)
                                {
                                    var listaErros = subItem.Nodes()
                                        .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                    XElement objParametroPedido = (XElement)listaErros[0];
                                    XElement objParametroErro = (XElement)listaErros[1];
                                    XElement objMsgErro = (XElement)listaErros[2];

                                    if (objMsgErro.Value.Contains("Unidade")
                                        && objMsgErro.Value.Contains("Nao Encontrada"))
                                    {
                                        #region Insere Unidade

                                        #region Carrega Entidade

                                        ObjectParameter value = new ObjectParameter("numero", typeof(global::System.String));
                                        apolo.CONCAT_ZERO_ESQUERDA(objParametroErro.Value, 7, value);
                                        string codigoEntidadeApolo = value.Value.ToString();

                                        ENTIDADE entidadeApolo = apolo.ENTIDADE
                                            .Where(w => w.EntCod == codigoEntidadeApolo).FirstOrDefault();

                                        #endregion

                                        if (entidadeApolo != null)
                                        {
                                            #region Carrega Dados Entidade

                                            CIDADE cidade = apolo.CIDADE
                                                .Where(w => w.CidCod == entidadeApolo.CidCod).FirstOrDefault();

                                            #endregion

                                            #region Carrega Parâmetros

                                            //Cria um arrayList com os dados da unidade pai
                                            ArrayList arrayUnidadePai = new ArrayList();
                                            OrderedDictionary arrayUnidadePaiItens = new OrderedDictionary();
                                            arrayUnidadePaiItens.Add("cod_unidade", codigoIncubatorio);
                                            arrayUnidadePaiItens.Add("diferenciador", "");
                                            arrayUnidadePai.Add(arrayUnidadePaiItens);

                                            //Cria um arrayList com os dados da referencia da unidade
                                            ArrayList arrayReferencia = new ArrayList();
                                            OrderedDictionary arrayReferenciaItens = new OrderedDictionary();
                                            arrayReferenciaItens.Add("lat", 0);
                                            arrayReferenciaItens.Add("lon", 0);
                                            arrayReferencia.Add(arrayReferenciaItens);

                                            //Cria um arrayList com os dados do(s) tipo(s) de operacao da unidade
                                            ArrayList arrayTipoOperacao = new ArrayList();
                                            OrderedDictionary arrayTipoOperacaoItens = new OrderedDictionary();
                                            arrayTipoOperacaoItens.Add("codigo", 2623);
                                            arrayTipoOperacaoItens.Add("origem", false);
                                            arrayTipoOperacaoItens.Add("destino", true);
                                            arrayTipoOperacaoItens.Add("passagem", false);
                                            arrayTipoOperacao.Add(arrayTipoOperacaoItens);

                                            string complemento = "";
                                            if (entidadeApolo.EntEnderComp != null) complemento = entidadeApolo.EntEnderComp;
                                            string cep = "";
                                            if (entidadeApolo.EntCep != null) cep = entidadeApolo.EntCep;

                                            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
                                            OrderedDictionary parametrosUnidade = new OrderedDictionary();
                                            parametrosUnidade.Add("cod_unidade", Convert.ToInt32(entidadeApolo.EntCod));
                                            parametrosUnidade.Add("diferenciador", "");
                                            parametrosUnidade.Add("descricao", entidadeApolo.EntNome);
                                            parametrosUnidade.Add("responsavel", "");
                                            parametrosUnidade.Add("telefone", "");
                                            parametrosUnidade.Add("endereco", entidadeApolo.EntEnder);
                                            parametrosUnidade.Add("observacao", "");
                                            parametrosUnidade.Add("unidade_pai", arrayUnidadePai);
                                            parametrosUnidade.Add("cidade", cidade.CidNomeComp);
                                            parametrosUnidade.Add("uf", cidade.UfSigla);
                                            parametrosUnidade.Add("tipo", 2622);
                                            parametrosUnidade.Add("zona", "");
                                            parametrosUnidade.Add("regiao", "");
                                            parametrosUnidade.Add("referencia", arrayReferencia);
                                            parametrosUnidade.Add("tipo_operacao", arrayTipoOperacao);
                                            parametrosUnidade.Add("cnpj", "");
                                            parametrosUnidade.Add("numero", entidadeApolo.EntEnderNo);
                                            parametrosUnidade.Add("bairro", entidadeApolo.EntBair);
                                            parametrosUnidade.Add("cep", cep);
                                            parametrosUnidade.Add("complemento", complemento);
                                            parametrosUnidade.Add("tipo_pessoa", entidadeApolo.EntTipoFJ.Substring(0, 1));
                                            parametrosUnidade.Add("rg_ie", "");

                                            #endregion

                                            #region Execução WebService

                                            string retornoEntidade = Embarcador.Embarcador
                                                .insereAtualizaUnidade(parametrosUnidade);

                                            xmlRetorno = Embarcador.Embarcador.inserePedidosLote(parametros);

                                            entidadeResolvida = true;

                                            #endregion
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        if (objMsgErro.Value != "")
                                        {
                                            if (!entidadeResolvida && objMsgErro.Value.Contains("Unidade"))
                                            {
                                                #region Envia E-mail caso haja erro

                                                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                                                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                                                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                                                email.WorkFlowEmailStat = "Enviar";
                                                email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****";
                                                email.WorkFlowEmailData = DateTime.Now;
                                                email.WorkFlowEmailParaNome = "Paulo Alves";
                                                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                                                email.WorkFlowEmailDeNome = "Serviço de Importação";
                                                email.WorkFLowEmailDeEmail = "sistema@hyline.com.br";
                                                email.WorkFlowEmailFormato = "Texto";
                                                email.WorkFlowEmailCopiaPara = "";

                                                string corpoEmail = "";

                                                corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                                                    + "Número do Pedido CHIC: " + objParametroPedido.Value + (char)13 + (char)10
                                                    + "Parâmetro do Erro: " + objParametroErro.Value + (char)13 + (char)10
                                                    + "Mensagem do Erro: " + objMsgErro.Value;

                                                email.WorkFlowEmailCorpo = corpoEmail;

                                                apolo.WORKFLOW_EMAIL.AddObject(email);
                                                apolo.SaveChanges();

                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                //retorno = "Erro ao Atualizar CHIC com WEB - Erro Linha: " + linenum.ToString()
                //    + " / Erro primário: " + ex.Message;
                //if (ex.InnerException != null)
                //    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                //return retorno;
            }
        }

        // Método que importa os pedidos do CHIC para o Embarcador
        public string ImportaPedidoEmbarcador(string ordernoCHIC, string origemChamada)
        {
            string erroPedido = "";
            string erroRetorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Localiza os pedidos a serem importados

                //DateTime data = DateTime.Today.AddDays(-21);
                DateTime data = Convert.ToDateTime("11/12/2017").AddDays(-21);

                CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                items.Fill(iDT);

                CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();
                //orders.FillSalesByCalDate(oDT, data);
                //orders.FillByOrderNo(oDT, "74903"); // Teste de entidade não cadastrada
                orders.FillByOrderNo(oDT, ordernoCHIC);
                //orders.FillTesteEmbarcador(oDT);

                var listaPedidosCHIC = oDT
                    //.Where(w => w.orderno == "77495")
                    .ToList();

                #endregion

                #region Montagem e carregamento dos parâmetros

                //Cria um arrayList dos pedidos
                foreach (var pedidoCHIC in listaPedidosCHIC)
                {
                    erroPedido = pedidoCHIC.orderno;

                    OrderedDictionary parametros = new OrderedDictionary();

                    ArrayList arrayPedidos = new ArrayList();

                    OrderedDictionary pedido = new OrderedDictionary();

                    #region Carrega os dados dos itens

                    CHICDataSet.bookedDataTable bDT = new CHICDataSet.bookedDataTable();
                    booked.FillByOrderNo(bDT, pedidoCHIC.orderno);

                    var listaItensVendidos = bDT.Where(w =>
                        iDT.Any(a => a.item_no == w.item
                            //&& (a.form.Substring(0, 1) == "D" || a.form.Substring(0, 1) == "H")))
                            && (a.form.Substring(0, 1) == "D")))
                        .OrderBy(o => o.item_ord)
                        .GroupBy(g => new
                        {
                            g.item
                        })
                        .Select(s => new
                        {
                            s.Key.item,
                            qtde = s.Sum(m => m.quantity)
                        })
                        .ToList();

                    #endregion

                    #region Carrega dados do primeiro item

                    CHICDataSet.bookedRow bR = bDT.Where(w =>
                        iDT.Any(a => a.item_no == w.item
                            && (a.form.Substring(0, 1) == "D" || a.form.Substring(0, 1) == "H")))
                        .OrderByDescending(o => o.quantity).FirstOrDefault();

                    string incubatorio = "";
                    if (bR != null)
                        incubatorio = bR.location.Trim();

                    #endregion

                    //if ((incubatorio == "CH" || incubatorio == "AJ") && listaItensVendidos.Count > 0)
                    if (listaItensVendidos.Count > 0)
                    {
                        bool pedidoApagado = true;

                        if (!pedidoApagado)
                            return erroRetorno;

                        #region Carrega parâmetros do pedido

                        bool pinto = true;
                        CHICDataSet.itemsRow iPrimeiro = iDT.Where(w => w.item_no == bR.item).FirstOrDefault();
                        if (iPrimeiro.form.Substring(0, 1) == "H") pinto = false;

                        int codigoIncubatorio = 0;
                        if (incubatorio == "CH") codigoIncubatorio = 2;
                        else if (incubatorio == "PH") codigoIncubatorio = 4;
                        else if (incubatorio == "NM") codigoIncubatorio = 3;
                        else if (incubatorio == "AJ") codigoIncubatorio = 1;

                        //Cria um arrayList com os dados da unidade base
                        ArrayList arrayUnidadeBase = new ArrayList();
                        OrderedDictionary unidadeBase = new OrderedDictionary();
                        unidadeBase.Add("codigo", codigoIncubatorio);
                        unidadeBase.Add("diferenciador", "");
                        arrayUnidadeBase.Add(unidadeBase);
                        pedido.Add("base", arrayUnidadeBase);

                        //Cria um arrayList com os dados da origem
                        ArrayList arrayOrigem = new ArrayList();
                        OrderedDictionary origem = new OrderedDictionary();
                        origem.Add("codigo", codigoIncubatorio);
                        origem.Add("diferenciador", "");
                        arrayOrigem.Add(origem);
                        pedido.Add("origem", arrayOrigem);

                        int codigodestino = Convert.ToInt32(pedidoCHIC.cust_no.Trim());
                        //Cria um arrayList com os dados da unidade base
                        ArrayList arrayDestino = new ArrayList();
                        OrderedDictionary destino = new OrderedDictionary();
                        destino.Add("codigo", codigodestino);
                        destino.Add("diferenciador", "");
                        destino.Add("NOME", "");
                        destino.Add("CIDADE", "");
                        destino.Add("UF", "");
                        destino.Add("TELEFONE", "");
                        destino.Add("ENDERECO", "");
                        destino.Add("NUMERO", "");
                        destino.Add("BAIRRO", "");
                        destino.Add("CEP", "");
                        destino.Add("COMPLEMENTO", "");
                        destino.Add("LATITUDE", "");
                        destino.Add("LONGITUDE", "");
                        destino.Add("TIPO", "");
                        destino.Add("CPF_CPNJ", "");
                        destino.Add("PESSOA", "");
                        arrayDestino.Add(destino);
                        pedido.Add("destino", arrayDestino);

                        //Cria um arrayList com os dados do transbordo
                        ArrayList arrayTransbordo = new ArrayList();
                        OrderedDictionary transbordo = new OrderedDictionary();
                        transbordo.Add("codigo", "");
                        transbordo.Add("diferenciador", "");
                        arrayTransbordo.Add(transbordo);
                        pedido.Add("TRANSBORDO", arrayTransbordo);

                        pedido.Add("codigo", pedidoCHIC.orderno);
                        if (pinto)
                            pedido.Add("data_embarque", bR.cal_date.AddDays(21).ToShortDateString());
                        else
                            pedido.Add("data_embarque", bR.cal_date.ToShortDateString());
                        pedido.Add("data_entrega", pedidoCHIC.del_date.ToShortDateString());
                        pedido.Add("tipo_data_entrega", "E");
                        pedido.Add("tipo_pedido", 107);
                        pedido.Add("TIPO_CARGA", 1561); // Transporte de Pintos
                        //pedido.Add("TIPO_CARGA", 2623); // Transporte de Pintos - Integração
                        //pedido.Add("tipo_operacao", 1783);
                        pedido.Add("tipo_operacao", 2623);
                        pedido.Add("EMPACOTAMENTO", "");
                        pedido.Add("MICRO_REGIAO", 0);
                        pedido.Add("OBSERVACAO", "");
                        pedido.Add("representante", pedidoCHIC.salesrep.Trim());
                        pedido.Add("CLIENTE_UNICO", "");
                        pedido.Add("PRIORIDADE", "");
                        pedido.Add("COD_CARGA", "");
                        pedido.Add("ALIAS_CARGA", "");

                        #endregion

                        ArrayList arrayProdutos = new ArrayList();

                        int qtdeItens = 1;

                        foreach (var item in listaItensVendidos)
                        {
                            #region Verifica se existe o pedido no Embarcador

                            OrderedDictionary parametrosBuscaPedido = new OrderedDictionary();
                            parametrosBuscaPedido.Add("CODIGO", pedidoCHIC.orderno);
                            parametrosBuscaPedido.Add("NR_ITEM", item.item);
                            //parametrosBuscaPedido.Add("NR_ITEM", 1);

                            XDocument xmlExistePedido = Embarcador.Embarcador.buscaPedido(parametrosBuscaPedido);

                            #region Verifica retorno se existe pedido

                            foreach (XElement retorno in xmlExistePedido.Descendants("return"))
                            {
                                var listaItens = retorno.Nodes();

                                foreach (XElement itemE in listaItens)
                                {
                                    var listaSubItens = itemE.Nodes()
                                        .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                    #region Carrega valores do retorno

                                    XElement objIDCarga = (XElement)listaSubItens[0];
                                    XElement objPlaca = (XElement)listaSubItens[1];
                                    XElement objQuantidade = (XElement)listaSubItens[2];
                                    XElement objPeso = (XElement)listaSubItens[3];

                                    #endregion

                                    // Se existe o pedido, deleta ele para inserir novamente
                                    if (objQuantidade.Value != "" && objPeso.Value != "")
                                    {
                                        bool cargaRemovida = true;

                                        if (objIDCarga.Value != "")
                                        {
                                            #region Se existe carga, primeiro remove da carga

                                            #region Carrega parâmetros

                                            OrderedDictionary parametrosRemovePedidosCarga = new OrderedDictionary();
                                            parametrosRemovePedidosCarga.Add("ID_CARGA", objIDCarga.Value);
                                            parametrosRemovePedidosCarga.Add("PLACA", objPlaca.Value);

                                            ArrayList arrayPedidosRC = new ArrayList();
                                            OrderedDictionary pedidoRC = new OrderedDictionary();
                                            pedidoRC.Add("CODIGO", pedidoCHIC.orderno);
                                            pedidoRC.Add("NR_ITEM", item.item);
                                            //pedidoRC.Add("NR_ITEM", 1);
                                            arrayPedidosRC.Add(pedidoRC);

                                            parametrosRemovePedidosCarga.Add("PEDIDO", arrayPedidosRC);

                                            #endregion

                                            string cargaRemovidaStr = Embarcador.Embarcador
                                                .removePedidosCarga(parametrosRemovePedidosCarga);

                                            //cargaRemovida = Convert.ToBoolean(cargaRemovidaStr);

                                            if (!Boolean.TryParse(cargaRemovidaStr, out cargaRemovida))
                                            {
                                                string corpoEmail = "";

                                                corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                                                    + "Número do Pedido CHIC: " + ordernoCHIC + (char)13 + (char)10
                                                    + "Processo ao ser realizado: remoção do pedido de carga existente para atualização" + (char)13 + (char)10
                                                    + "Mensagem do Erro: " + cargaRemovidaStr;

                                                if (origemChamada == "Automático")
                                                {
                                                    #region Envia E-mail caso haja erro

                                                    WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                                                    ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                                                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                                                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                                                    email.WorkFlowEmailStat = "Enviar";
                                                    email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****";
                                                    email.WorkFlowEmailData = DateTime.Now;
                                                    email.WorkFlowEmailParaNome = "Paulo Alves";
                                                    email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                                                    email.WorkFlowEmailDeNome = "Serviço de Importação";
                                                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                                                    email.WorkFlowEmailFormato = "Texto";
                                                    email.WorkFlowEmailCopiaPara = "logistica@hyline.com.br";

                                                    email.WorkFlowEmailCorpo = corpoEmail;

                                                    apolo.WORKFLOW_EMAIL.AddObject(email);
                                                    apolo.SaveChanges();

                                                    #endregion
                                                }
                                                else
                                                {
                                                    return erroRetorno = corpoEmail;
                                                }
                                            }

                                            #endregion
                                        }

                                        #region Deleta pedido

                                        if (cargaRemovida)
                                        {
                                            OrderedDictionary parametrosApagaPedido = new OrderedDictionary();
                                            parametrosApagaPedido.Add("CODIGO", pedidoCHIC.orderno);
                                            parametrosApagaPedido.Add("NR_ITEM", item.item);
                                            //parametrosApagaPedido.Add("NR_ITEM", 1);
                                            parametrosApagaPedido.Add("APAGA_CARREGADO", false);

                                            pedidoApagado = Convert.ToBoolean(Embarcador.Embarcador
                                                .apagaPedido(parametrosApagaPedido));
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion

                            #endregion

                            #region Carrega Dados do Item

                            CHICDataSet.itemsRow iR = iDT.Where(w => w.item_no == item.item).FirstOrDefault();
                            string descricao = "";
                            if (iR != null)
                                descricao = iR.variety.Trim() + " - " + iR.form.Trim();

                            // Quantidade criptografada para mascarar. Solicitado por Davi Nogueira.
                            int qtdeCrypto = 0;
                            qtdeCrypto = Convert.ToInt32(item.qtde) * 17;

                            #endregion

                            #region Carrega parâmetros do item

                            OrderedDictionary produto = new OrderedDictionary();
                            produto.Add("codigo", item.item);
                            //produto.Add("item", qtdeItens);
                            produto.Add("item", item.item);
                            produto.Add("descricao", descricao);
                            produto.Add("quantidade", qtdeCrypto);
                            produto.Add("PESO_UNITARIO", 1);
                            produto.Add("PESO_TOTAL", 1);
                            produto.Add("VOLUME", "");
                            produto.Add("OBSERVACAO", "");
                            produto.Add("TIPO_CARGA", 1561);
                            //if (pinto)
                            //    produto.Add("data_embarque", bR.cal_date.AddDays(21).ToShortDateString());
                            //else
                            //    produto.Add("data_embarque", bR.cal_date.ToShortDateString());
                            //produto.Add("data_entrega", pedidoCHIC.del_date.ToShortDateString());
                            if (pinto)
                            {
                                produto.Add("data_embarque", bR.cal_date.AddDays(21).ToShortDateString());
                                produto.Add("data_entrega", bR.cal_date.AddDays(21).ToShortDateString());
                            }
                            else
                            {
                                produto.Add("data_embarque", bR.cal_date.ToShortDateString());
                                produto.Add("data_entrega", bR.cal_date.ToShortDateString());
                            }
                            produto.Add("TIPO_DATA_ENTREGA", "E");
                            produto.Add("PRIORIDADE", "B");
                            arrayProdutos.Add(produto);

                            #endregion

                            qtdeItens = qtdeItens + 1;
                        }

                        pedido.Add("produto", arrayProdutos);

                        arrayPedidos.Add(pedido);

                        parametros.Add("pedido", arrayPedidos);

                        #region Execução do WebService

                        XDocument xmlRetorno = Embarcador.Embarcador.inserePedidosLote(parametros);

                        foreach (XElement retorno in xmlRetorno.Descendants("return"))
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                var listaSubItens = item.Nodes()
                                    .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                bool entidadeResolvida = false;

                                foreach (XElement subItem in listaSubItens)
                                {
                                    var listaErros = subItem.Nodes()
                                        .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                    XElement objParametroPedido = (XElement)listaErros[0];
                                    XElement objParametroErro = (XElement)listaErros[1];
                                    XElement objMsgErro = (XElement)listaErros[2];

                                    if (objMsgErro.Value.Contains("Unidade")
                                        && objMsgErro.Value.Contains("Nao Encontrada"))
                                    {
                                        #region Insere Unidade

                                        #region Carrega Entidade

                                        ObjectParameter value = new ObjectParameter("numero", typeof(global::System.String));
                                        apolo.CONCAT_ZERO_ESQUERDA(objParametroErro.Value, 7, value);
                                        string codigoEntidadeApolo = value.Value.ToString();

                                        ENTIDADE entidadeApolo = apolo.ENTIDADE
                                            .Where(w => w.EntCod == codigoEntidadeApolo).FirstOrDefault();

                                        #endregion

                                        if (entidadeApolo != null)
                                        {
                                            #region Carrega Dados Entidade

                                            CIDADE cidade = apolo.CIDADE
                                                .Where(w => w.CidCod == entidadeApolo.CidCod).FirstOrDefault();

                                            #endregion

                                            #region Carrega Parâmetros

                                            //Cria um arrayList com os dados da unidade pai
                                            ArrayList arrayUnidadePai = new ArrayList();
                                            OrderedDictionary arrayUnidadePaiItens = new OrderedDictionary();
                                            arrayUnidadePaiItens.Add("cod_unidade", codigoIncubatorio);
                                            arrayUnidadePaiItens.Add("diferenciador", "");
                                            arrayUnidadePai.Add(arrayUnidadePaiItens);

                                            //Cria um arrayList com os dados da referencia da unidade
                                            ArrayList arrayReferencia = new ArrayList();
                                            OrderedDictionary arrayReferenciaItens = new OrderedDictionary();
                                            arrayReferenciaItens.Add("lat", 0);
                                            arrayReferenciaItens.Add("lon", 0);
                                            arrayReferencia.Add(arrayReferenciaItens);

                                            //Cria um arrayList com os dados do(s) tipo(s) de operacao da unidade
                                            ArrayList arrayTipoOperacao = new ArrayList();
                                            OrderedDictionary arrayTipoOperacaoItens = new OrderedDictionary();
                                            arrayTipoOperacaoItens.Add("codigo", 2623);
                                            arrayTipoOperacaoItens.Add("origem", false);
                                            arrayTipoOperacaoItens.Add("destino", true);
                                            arrayTipoOperacaoItens.Add("passagem", false);
                                            arrayTipoOperacao.Add(arrayTipoOperacaoItens);

                                            string complemento = "";
                                            if (entidadeApolo.EntEnderComp != null) complemento = entidadeApolo.EntEnderComp;
                                            string cep = "";
                                            if (entidadeApolo.EntCep != null) cep = entidadeApolo.EntCep;
                                            string numeroEnder = "";
                                            if (entidadeApolo.EntEnderNo != null) numeroEnder = entidadeApolo.EntEnderNo;
                                            string bairro = "";
                                            if (entidadeApolo.EntBair != null) bairro = entidadeApolo.EntBair;

                                            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
                                            OrderedDictionary parametrosUnidade = new OrderedDictionary();
                                            parametrosUnidade.Add("cod_unidade", Convert.ToInt32(entidadeApolo.EntCod));
                                            parametrosUnidade.Add("diferenciador", "");
                                            parametrosUnidade.Add("descricao", entidadeApolo.EntNome);
                                            parametrosUnidade.Add("responsavel", "");
                                            parametrosUnidade.Add("telefone", "");
                                            parametrosUnidade.Add("endereco", entidadeApolo.EntEnder);
                                            parametrosUnidade.Add("observacao", "");
                                            parametrosUnidade.Add("unidade_pai", arrayUnidadePai);
                                            parametrosUnidade.Add("cidade", cidade.CidNomeComp);
                                            parametrosUnidade.Add("uf", cidade.UfSigla);
                                            parametrosUnidade.Add("tipo", 2622);
                                            parametrosUnidade.Add("zona", "");
                                            parametrosUnidade.Add("regiao", "");
                                            parametrosUnidade.Add("referencia", arrayReferencia);
                                            parametrosUnidade.Add("tipo_operacao", arrayTipoOperacao);
                                            parametrosUnidade.Add("cnpj", "");
                                            parametrosUnidade.Add("numero", numeroEnder);
                                            parametrosUnidade.Add("bairro", bairro);
                                            parametrosUnidade.Add("cep", cep);
                                            parametrosUnidade.Add("complemento", complemento);
                                            parametrosUnidade.Add("tipo_pessoa", entidadeApolo.EntTipoFJ.Substring(0, 1));
                                            parametrosUnidade.Add("rg_ie", "");

                                            #endregion

                                            #region Execução WebService

                                            string retornoEntidade = Embarcador.Embarcador
                                                .insereAtualizaUnidade(parametrosUnidade);

                                            if (retornoEntidade.Contains("BAD") 
                                                || retornoEntidade.Contains("Msg. do Erro"))
                                            {
                                                erroRetorno = "Erro ao cadastrar entidade "
                                                    + entidadeApolo.EntCod + " - "
                                                    + entidadeApolo.EntNome + " no Embarcador por não existir: "
                                                    + retornoEntidade;

                                                if (origemChamada == "Automático")
                                                {
                                                    #region Envia E-mail caso haja erro

                                                    WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                                                    ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                                                    apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                                                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                                                    email.WorkFlowEmailStat = "Enviar";
                                                    email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****";
                                                    email.WorkFlowEmailData = DateTime.Now;
                                                    email.WorkFlowEmailParaNome = "Paulo Alves";
                                                    email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                                                    email.WorkFlowEmailDeNome = "Serviço de Importação";
                                                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                                                    email.WorkFlowEmailFormato = "Texto";
                                                    email.WorkFlowEmailCopiaPara = "logistica@hyline.com.br";

                                                    email.WorkFlowEmailCorpo = erroRetorno;

                                                    apolo.WORKFLOW_EMAIL.AddObject(email);
                                                    apolo.SaveChanges();

                                                    #endregion
                                                }
                                                else
                                                {
                                                    return erroRetorno;
                                                }
                                            }

                                            xmlRetorno = Embarcador.Embarcador.inserePedidosLote(parametros);

                                            entidadeResolvida = true;

                                            #endregion
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        if (objMsgErro.Value != "")
                                        {
                                            if (!entidadeResolvida && objMsgErro.Value.Contains("Unidade"))
                                            {
                                                #region Verifica Se unidade esta como destino. Caso não, atualize a mesma.

                                                bool unidadeAtualizada = false;

                                                if (objMsgErro.Value.Contains("Unidade de Destino Nao Esta Cadastrada Como Destino na Operacao"))
                                                {
                                                    #region Insere Unidade

                                                    #region Carrega Entidade

                                                    ObjectParameter value = new ObjectParameter("numero", typeof(global::System.String));
                                                    apolo.CONCAT_ZERO_ESQUERDA(objParametroErro.Value, 7, value);
                                                    string codigoEntidadeApolo = value.Value.ToString();

                                                    ENTIDADE entidadeApolo = apolo.ENTIDADE
                                                        .Where(w => w.EntCod == codigoEntidadeApolo).FirstOrDefault();

                                                    #endregion

                                                    if (entidadeApolo != null)
                                                    {
                                                        #region Carrega Dados Entidade

                                                        CIDADE cidade = apolo.CIDADE
                                                            .Where(w => w.CidCod == entidadeApolo.CidCod).FirstOrDefault();

                                                        #endregion

                                                        #region Carrega Parâmetros

                                                        //Cria um arrayList com os dados da unidade pai
                                                        ArrayList arrayUnidadePai = new ArrayList();
                                                        OrderedDictionary arrayUnidadePaiItens = new OrderedDictionary();
                                                        arrayUnidadePaiItens.Add("cod_unidade", codigoIncubatorio);
                                                        arrayUnidadePaiItens.Add("diferenciador", "");
                                                        arrayUnidadePai.Add(arrayUnidadePaiItens);

                                                        //Cria um arrayList com os dados da referencia da unidade
                                                        ArrayList arrayReferencia = new ArrayList();
                                                        OrderedDictionary arrayReferenciaItens = new OrderedDictionary();
                                                        arrayReferenciaItens.Add("lat", 0);
                                                        arrayReferenciaItens.Add("lon", 0);
                                                        arrayReferencia.Add(arrayReferenciaItens);

                                                        //Cria um arrayList com os dados do(s) tipo(s) de operacao da unidade
                                                        ArrayList arrayTipoOperacao = new ArrayList();
                                                        OrderedDictionary arrayTipoOperacaoItens = new OrderedDictionary();
                                                        arrayTipoOperacaoItens.Add("codigo", 2623);
                                                        arrayTipoOperacaoItens.Add("origem", false);
                                                        arrayTipoOperacaoItens.Add("destino", true);
                                                        arrayTipoOperacaoItens.Add("passagem", false);
                                                        arrayTipoOperacao.Add(arrayTipoOperacaoItens);

                                                        string complemento = "";
                                                        if (entidadeApolo.EntEnderComp != null) complemento = entidadeApolo.EntEnderComp;
                                                        string cep = "";
                                                        if (entidadeApolo.EntCep != null) cep = entidadeApolo.EntCep;
                                                        string numeroEnder = "";
                                                        if (entidadeApolo.EntEnderNo != null) numeroEnder = entidadeApolo.EntEnderNo;
                                                        string bairro = "";
                                                        if (entidadeApolo.EntBair != null) bairro = entidadeApolo.EntBair;

                                                        //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
                                                        OrderedDictionary parametrosUnidade = new OrderedDictionary();
                                                        parametrosUnidade.Add("cod_unidade", Convert.ToInt32(entidadeApolo.EntCod));
                                                        parametrosUnidade.Add("diferenciador", "");
                                                        parametrosUnidade.Add("descricao", entidadeApolo.EntNome);
                                                        parametrosUnidade.Add("responsavel", "");
                                                        parametrosUnidade.Add("telefone", "");
                                                        parametrosUnidade.Add("endereco", entidadeApolo.EntEnder);
                                                        parametrosUnidade.Add("observacao", "");
                                                        parametrosUnidade.Add("unidade_pai", arrayUnidadePai);
                                                        parametrosUnidade.Add("cidade", cidade.CidNomeComp);
                                                        parametrosUnidade.Add("uf", cidade.UfSigla);
                                                        parametrosUnidade.Add("tipo", 2622);
                                                        parametrosUnidade.Add("zona", "");
                                                        parametrosUnidade.Add("regiao", "");
                                                        parametrosUnidade.Add("referencia", arrayReferencia);
                                                        parametrosUnidade.Add("tipo_operacao", arrayTipoOperacao);
                                                        parametrosUnidade.Add("cnpj", "");
                                                        parametrosUnidade.Add("numero", numeroEnder);
                                                        parametrosUnidade.Add("bairro", bairro);
                                                        parametrosUnidade.Add("cep", cep);
                                                        parametrosUnidade.Add("complemento", complemento);
                                                        parametrosUnidade.Add("tipo_pessoa", entidadeApolo.EntTipoFJ.Substring(0, 1));
                                                        parametrosUnidade.Add("rg_ie", "");

                                                        #endregion

                                                        #region Execução WebService

                                                        string retornoEntidade = Embarcador.Embarcador
                                                            .insereAtualizaUnidade(parametrosUnidade);

                                                        if (retornoEntidade.Contains("BAD")
                                                            || retornoEntidade.Contains("Msg. do Erro"))
                                                        {
                                                            erroRetorno = "Erro ao cadastrar entidade "
                                                                + entidadeApolo.EntCod + " - "
                                                                + entidadeApolo.EntNome + " no Embarcador por não existir: "
                                                                + retornoEntidade;

                                                            if (origemChamada == "Automático")
                                                            {
                                                                #region Envia E-mail caso haja erro

                                                                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                                                                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                                                                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                                                                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                                                                email.WorkFlowEmailStat = "Enviar";
                                                                email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****";
                                                                email.WorkFlowEmailData = DateTime.Now;
                                                                email.WorkFlowEmailParaNome = "Paulo Alves";
                                                                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                                                                email.WorkFlowEmailDeNome = "Serviço de Importação";
                                                                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                                                                email.WorkFlowEmailFormato = "Texto";
                                                                email.WorkFlowEmailCopiaPara = "logistica@hyline.com.br";

                                                                email.WorkFlowEmailCorpo = erroRetorno;

                                                                apolo.WORKFLOW_EMAIL.AddObject(email);
                                                                apolo.SaveChanges();

                                                                #endregion
                                                            }
                                                            else
                                                            {
                                                                return erroRetorno;
                                                            }
                                                        }

                                                        xmlRetorno = Embarcador.Embarcador.inserePedidosLote(parametros);

                                                        entidadeResolvida = true;

                                                        #endregion
                                                    }

                                                    #endregion

                                                    unidadeAtualizada = true;
                                                }

                                                if (!unidadeAtualizada)
                                                {
                                                    string corpoEmail = "";

                                                    corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                                                        + "Número do Pedido CHIC: " + objParametroPedido.Value + (char)13 + (char)10
                                                        + "Parâmetro do Erro: " + objParametroErro.Value + (char)13 + (char)10
                                                        + "Mensagem do Erro: " + objMsgErro.Value;

                                                    if (origemChamada == "Automático")
                                                    {
                                                        #region Envia E-mail caso haja erro

                                                        WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                                                        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                                                        apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                                                        email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                                                        email.WorkFlowEmailStat = "Enviar";
                                                        email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****";
                                                        email.WorkFlowEmailData = DateTime.Now;
                                                        email.WorkFlowEmailParaNome = "Paulo Alves";
                                                        email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                                                        email.WorkFlowEmailDeNome = "Serviço de Importação";
                                                        email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                                                        email.WorkFlowEmailFormato = "Texto";
                                                        email.WorkFlowEmailCopiaPara = "logistica@hyline.com.br";

                                                        email.WorkFlowEmailCorpo = corpoEmail;

                                                        apolo.WORKFLOW_EMAIL.AddObject(email);
                                                        apolo.SaveChanges();

                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        return erroRetorno = corpoEmail;
                                                    }
                                                }

                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                return erroRetorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erroRetorno = "Erro ao Atualizar CHIC com Embarcador - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    erroRetorno = erroRetorno + " Erro Secundário: " + ex.InnerException.Message;

                #region Envio de E-mail

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Paulo Alves";
                email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";

                string corpoEmail = "";
                string innerException = "";

                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }

                corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "Linha do Erro: " + erro.ToString() + (char)13 + (char)10
                    + "Número do Pedido CHIC: " + erroPedido + (char)13 + (char)10
                    + "Linha do Erro: " + linenum.ToString() + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 1: " + ex.Message + (char)13 + (char)10 + (char)13 + (char)10
                    + "Erro 2: " + innerException;

                email.WorkFlowEmailCorpo = corpoEmail;
                apolo.WORKFLOW_EMAIL.AddObject(email);

                #endregion

                return erroRetorno;
            }
        }

        public string ImportaPedidosEmbarcador()
        {
            string erroPedido = "";
            string retorno = "";

            ApoloServiceEntities apolo = new ApoloServiceEntities();
            apolo.CommandTimeout = 1000;

            try
            {
                #region Localiza os pedidos a serem importados

                //DateTime data = DateTime.Today.AddDays(-21);
                //DateTime data = Convert.ToDateTime("11/12/2017").AddDays(-21);

                DateTime dataInicial = DateTime.Today.AddDays(3).AddDays(-21);
                DateTime dataFinal = DateTime.Today.AddDays(9).AddDays(-21);

                //DateTime dataInicial = Convert.ToDateTime("16/05/2018").AddDays(-21);
                //DateTime dataFinal = Convert.ToDateTime("19/05/2018").AddDays(-21);

                CHICDataSet.itemsDataTable iDT = new CHICDataSet.itemsDataTable();
                items.Fill(iDT);

                CHICDataSet.ordersDataTable oDT = new CHICDataSet.ordersDataTable();
                //orders.FillSalesByCalDate(oDT, data);
                //orders.FillByOrderNo(oDT, "74903"); // Teste de entidade não cadastrada
                //orders.FillByOrderNo(oDT, "75965");
                //orders.FillTesteEmbarcador(oDT);
                orders.FillByCalDateIniAndCalDateFim(oDT, dataInicial, dataFinal);

                var listaPedidosCHIC = oDT
                    //.Where(w => w.orderno == "78115")
                    .ToList();

                #endregion

                #region Chama o método de importação do pedido

                //Cria um arrayList dos pedidos
                foreach (var pedidoCHIC in listaPedidosCHIC)
                {
                    retorno = ImportaPedidoEmbarcador(pedidoCHIC.orderno, "Automático");
                }

                #endregion

                #region Envia E-mail após concluída a importação para o departamento de logística

                WORKFLOW_EMAIL email = new WORKFLOW_EMAIL();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;
                string corpoEmail = "";

                apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** IMPORTAÇÃO DOS PEDIDOS DO CHIC PARA O EMBARCADOR ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Paulo Alves";
                email.WorkFlowEmailParaEmail = "logistica@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";
                email.WorkFlowEmailCopiaPara = "";

                corpoEmail = "Prezados, " + (char)13 + (char)10 + (char)13 + (char)10
                    + "A Importação dos pedidos do CHIC para o Embarcador realizada com sucesso!" 
                        + (char)13 + (char)10 + (char)13 + (char)10
                    + "SISTEMA";

                email.WorkFlowEmailCorpo = corpoEmail;

                apolo.WORKFLOW_EMAIL.AddObject(email);
                apolo.SaveChanges();

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao Atualizar CHIC com Embarcador - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " Erro Secundário: " + ex.InnerException.Message;

                return retorno;
            }
        }

        public XDocument buscaPedido(string numPedido, int item)
        {
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("CODIGO", numPedido);
            parametros.Add("NR_ITEM", item);

            return Embarcador.Embarcador.buscaPedido(parametros);
        }

        public string removePedidosCarga(string numPedido, int item)
        {
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("CODIGO", numPedido);
            parametros.Add("NR_ITEM", item);

            return Embarcador.Embarcador.removePedidosCarga(parametros);
        }

        public void ChamadaTeste()
        {
            //string retorno = Embarcador.Embarcador.buscaCargaCodigo(9019078);
            XDocument xmlExistePedido = Embarcador.Embarcador.buscaPedido("80815", "708");
        }

        #endregion
    }
}