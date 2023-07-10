﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace MvcAppHyLinedoBrasil.Models.HLBAPP
{
    public partial class HLBAPPEntities1 : ObjectContext
    {
        public const string ConnectionString = "name=HLBAPPEntities1";
        public const string ContainerName = "HLBAPPEntities1";
    
        #region Constructors
    
        public HLBAPPEntities1()
            : base(ConnectionString, ContainerName)
        {
            Initialize();
        }
    
        public HLBAPPEntities1(string connectionString)
            : base(connectionString, ContainerName)
        {
            Initialize();
        }
    
        public HLBAPPEntities1(EntityConnection connection)
            : base(connection, ContainerName)
        {
            Initialize();
        }
    
        private void Initialize()
        {
            // Creating proxies requires the use of the ProxyDataContractResolver and
            // may allow lazy loading which can expand the loaded graph during serialization.
            ContextOptions.ProxyCreationEnabled = false;
            ObjectMaterialized += new ObjectMaterializedEventHandler(HandleObjectMaterialized);
        }
    
        private void HandleObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity as IObjectWithChangeTracker;
            if (entity != null)
            {
                bool changeTrackingEnabled = entity.ChangeTracker.ChangeTrackingEnabled;
                try
                {
                    entity.MarkAsUnchanged();
                }
                finally
                {
                    entity.ChangeTracker.ChangeTrackingEnabled = changeTrackingEnabled;
                }
                this.StoreReferenceKeyValues(entity);
            }
        }
    
        #endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<Dados_Loggers> Dados_Loggers
        {
            get { return _dados_Loggers  ?? (_dados_Loggers = CreateObjectSet<Dados_Loggers>("Dados_Loggers")); }
        }
        private ObjectSet<Dados_Loggers> _dados_Loggers;
    
        public ObjectSet<LayoutDDASegmentoGs_EnvioEmailFiscal> LayoutDDASegmentoGs_EnvioEmailFiscal
        {
            get { return _layoutDDASegmentoGs_EnvioEmailFiscal  ?? (_layoutDDASegmentoGs_EnvioEmailFiscal = CreateObjectSet<LayoutDDASegmentoGs_EnvioEmailFiscal>("LayoutDDASegmentoGs_EnvioEmailFiscal")); }
        }
        private ObjectSet<LayoutDDASegmentoGs_EnvioEmailFiscal> _layoutDDASegmentoGs_EnvioEmailFiscal;
    
        public ObjectSet<Prog_Diaria_Transp_Pedidos> Prog_Diaria_Transp_Pedidos
        {
            get { return _prog_Diaria_Transp_Pedidos  ?? (_prog_Diaria_Transp_Pedidos = CreateObjectSet<Prog_Diaria_Transp_Pedidos>("Prog_Diaria_Transp_Pedidos")); }
        }
        private ObjectSet<Prog_Diaria_Transp_Pedidos> _prog_Diaria_Transp_Pedidos;
    
        public ObjectSet<Prog_Diaria_Transp_Veiculos> Prog_Diaria_Transp_Veiculos
        {
            get { return _prog_Diaria_Transp_Veiculos  ?? (_prog_Diaria_Transp_Veiculos = CreateObjectSet<Prog_Diaria_Transp_Veiculos>("Prog_Diaria_Transp_Veiculos")); }
        }
        private ObjectSet<Prog_Diaria_Transp_Veiculos> _prog_Diaria_Transp_Veiculos;
    
        public ObjectSet<Tabela_Precos> Tabela_Precos
        {
            get { return _tabela_Precos  ?? (_tabela_Precos = CreateObjectSet<Tabela_Precos>("Tabela_Precos")); }
        }
        private ObjectSet<Tabela_Precos> _tabela_Precos;
    
        public ObjectSet<LINHAGEM_CONCORRENTE> LINHAGEM_CONCORRENTE
        {
            get { return _lINHAGEM_CONCORRENTE  ?? (_lINHAGEM_CONCORRENTE = CreateObjectSet<LINHAGEM_CONCORRENTE>("LINHAGEM_CONCORRENTE")); }
        }
        private ObjectSet<LINHAGEM_CONCORRENTE> _lINHAGEM_CONCORRENTE;
    
        public ObjectSet<Configuracao_Importa_NFe> Configuracao_Importa_NFe
        {
            get { return _configuracao_Importa_NFe  ?? (_configuracao_Importa_NFe = CreateObjectSet<Configuracao_Importa_NFe>("Configuracao_Importa_NFe")); }
        }
        private ObjectSet<Configuracao_Importa_NFe> _configuracao_Importa_NFe;
    
        public ObjectSet<Recebimento_Documento> Recebimento_Documento
        {
            get { return _recebimento_Documento  ?? (_recebimento_Documento = CreateObjectSet<Recebimento_Documento>("Recebimento_Documento")); }
        }
        private ObjectSet<Recebimento_Documento> _recebimento_Documento;
    
        public ObjectSet<Dados_Assistencia_Tecnica> Dados_Assistencia_Tecnica
        {
            get { return _dados_Assistencia_Tecnica  ?? (_dados_Assistencia_Tecnica = CreateObjectSet<Dados_Assistencia_Tecnica>("Dados_Assistencia_Tecnica")); }
        }
        private ObjectSet<Dados_Assistencia_Tecnica> _dados_Assistencia_Tecnica;
    
        public ObjectSet<LOG_Importacao_Dados_Assist_Tec> LOG_Importacao_Dados_Assist_Tec
        {
            get { return _lOG_Importacao_Dados_Assist_Tec  ?? (_lOG_Importacao_Dados_Assist_Tec = CreateObjectSet<LOG_Importacao_Dados_Assist_Tec>("LOG_Importacao_Dados_Assist_Tec")); }
        }
        private ObjectSet<LOG_Importacao_Dados_Assist_Tec> _lOG_Importacao_Dados_Assist_Tec;
    
        public ObjectSet<Empresas> Empresas
        {
            get { return _empresas  ?? (_empresas = CreateObjectSet<Empresas>("Empresas")); }
        }
        private ObjectSet<Empresas> _empresas;
    
        public ObjectSet<LOG_Prog_Diaria_Transp_Veiculos> LOG_Prog_Diaria_Transp_Veiculos
        {
            get { return _lOG_Prog_Diaria_Transp_Veiculos  ?? (_lOG_Prog_Diaria_Transp_Veiculos = CreateObjectSet<LOG_Prog_Diaria_Transp_Veiculos>("LOG_Prog_Diaria_Transp_Veiculos")); }
        }
        private ObjectSet<LOG_Prog_Diaria_Transp_Veiculos> _lOG_Prog_Diaria_Transp_Veiculos;
    
        public ObjectSet<VU_Resumo_Dados_Lotes_Clientes> VU_Resumo_Dados_Lotes_Clientes
        {
            get { return _vU_Resumo_Dados_Lotes_Clientes  ?? (_vU_Resumo_Dados_Lotes_Clientes = CreateObjectSet<VU_Resumo_Dados_Lotes_Clientes>("VU_Resumo_Dados_Lotes_Clientes")); }
        }
        private ObjectSet<VU_Resumo_Dados_Lotes_Clientes> _vU_Resumo_Dados_Lotes_Clientes;
    
        public ObjectSet<LINHAGEM_GRUPO> LINHAGEM_GRUPO
        {
            get { return _lINHAGEM_GRUPO  ?? (_lINHAGEM_GRUPO = CreateObjectSet<LINHAGEM_GRUPO>("LINHAGEM_GRUPO")); }
        }
        private ObjectSet<LINHAGEM_GRUPO> _lINHAGEM_GRUPO;
    
        public ObjectSet<Lancamentos_Classificadora_Excel_02> Lancamentos_Classificadora_Excel_02
        {
            get { return _lancamentos_Classificadora_Excel_02  ?? (_lancamentos_Classificadora_Excel_02 = CreateObjectSet<Lancamentos_Classificadora_Excel_02>("Lancamentos_Classificadora_Excel_02")); }
        }
        private ObjectSet<Lancamentos_Classificadora_Excel_02> _lancamentos_Classificadora_Excel_02;
    
        public ObjectSet<Languages> Languages
        {
            get { return _languages  ?? (_languages = CreateObjectSet<Languages>("Languages")); }
        }
        private ObjectSet<Languages> _languages;
    
        public ObjectSet<TIPO_CLASSFICACAO_OVO> TIPO_CLASSFICACAO_OVO
        {
            get { return _tIPO_CLASSFICACAO_OVO  ?? (_tIPO_CLASSFICACAO_OVO = CreateObjectSet<TIPO_CLASSFICACAO_OVO>("TIPO_CLASSFICACAO_OVO")); }
        }
        private ObjectSet<TIPO_CLASSFICACAO_OVO> _tIPO_CLASSFICACAO_OVO;
    
        public ObjectSet<PDI> PDI
        {
            get { return _pDI  ?? (_pDI = CreateObjectSet<PDI>("PDI")); }
        }
        private ObjectSet<PDI> _pDI;
    
        public ObjectSet<LOG_Atualizacao_CHIC_SQLServer> LOG_Atualizacao_CHIC_SQLServer
        {
            get { return _lOG_Atualizacao_CHIC_SQLServer  ?? (_lOG_Atualizacao_CHIC_SQLServer = CreateObjectSet<LOG_Atualizacao_CHIC_SQLServer>("LOG_Atualizacao_CHIC_SQLServer")); }
        }
        private ObjectSet<LOG_Atualizacao_CHIC_SQLServer> _lOG_Atualizacao_CHIC_SQLServer;
    
        public ObjectSet<LOG_Prog_Diaria_Transp_Pedidos> LOG_Prog_Diaria_Transp_Pedidos
        {
            get { return _lOG_Prog_Diaria_Transp_Pedidos  ?? (_lOG_Prog_Diaria_Transp_Pedidos = CreateObjectSet<LOG_Prog_Diaria_Transp_Pedidos>("LOG_Prog_Diaria_Transp_Pedidos")); }
        }
        private ObjectSet<LOG_Prog_Diaria_Transp_Pedidos> _lOG_Prog_Diaria_Transp_Pedidos;
    
        public ObjectSet<LOG_Prog_Diaria_Transp_Veiculos_TARGET> LOG_Prog_Diaria_Transp_Veiculos_TARGET
        {
            get { return _lOG_Prog_Diaria_Transp_Veiculos_TARGET  ?? (_lOG_Prog_Diaria_Transp_Veiculos_TARGET = CreateObjectSet<LOG_Prog_Diaria_Transp_Veiculos_TARGET>("LOG_Prog_Diaria_Transp_Veiculos_TARGET")); }
        }
        private ObjectSet<LOG_Prog_Diaria_Transp_Veiculos_TARGET> _lOG_Prog_Diaria_Transp_Veiculos_TARGET;
    
        public ObjectSet<Envio_Confirmacao> Envio_Confirmacao
        {
            get { return _envio_Confirmacao  ?? (_envio_Confirmacao = CreateObjectSet<Envio_Confirmacao>("Envio_Confirmacao")); }
        }
        private ObjectSet<Envio_Confirmacao> _envio_Confirmacao;
    
        public ObjectSet<Envio_Confirmacao_PV> Envio_Confirmacao_PV
        {
            get { return _envio_Confirmacao_PV  ?? (_envio_Confirmacao_PV = CreateObjectSet<Envio_Confirmacao_PV>("Envio_Confirmacao_PV")); }
        }
        private ObjectSet<Envio_Confirmacao_PV> _envio_Confirmacao_PV;
    
        public ObjectSet<Envio_Confirmacao_Email> Envio_Confirmacao_Email
        {
            get { return _envio_Confirmacao_Email  ?? (_envio_Confirmacao_Email = CreateObjectSet<Envio_Confirmacao_Email>("Envio_Confirmacao_Email")); }
        }
        private ObjectSet<Envio_Confirmacao_Email> _envio_Confirmacao_Email;
    
        public ObjectSet<PAIS> PAIS
        {
            get { return _pAIS  ?? (_pAIS = CreateObjectSet<PAIS>("PAIS")); }
        }
        private ObjectSet<PAIS> _pAIS;
    
        public ObjectSet<Entity> Entity
        {
            get { return _entity  ?? (_entity = CreateObjectSet<Entity>("Entity")); }
        }
        private ObjectSet<Entity> _entity;
    
        public ObjectSet<Lotes_Clientes> Lotes_Clientes
        {
            get { return _lotes_Clientes  ?? (_lotes_Clientes = CreateObjectSet<Lotes_Clientes>("Lotes_Clientes")); }
        }
        private ObjectSet<Lotes_Clientes> _lotes_Clientes;

        #endregion

        #region Function Imports
    
        /// <summary>
        /// Nenhuma Documentação de Metadados disponível.
        /// </summary>
        /// <param name="pDataInicial">Nenhuma Documentação de Metadados disponível.</param>
        /// <param name="pDataFinal">Nenhuma Documentação de Metadados disponível.</param>
        /// <param name="pUsuario">Nenhuma Documentação de Metadados disponível.</param>
        public virtual int Integra_Baixa_Ordem_Producao(Nullable<System.DateTime> pDataInicial, Nullable<System.DateTime> pDataFinal, string pUsuario)
        {
    
            ObjectParameter pDataInicialParameter;
    
            if (pDataInicial.HasValue)
            {
                pDataInicialParameter = new ObjectParameter("pDataInicial", pDataInicial);
            }
            else
            {
                pDataInicialParameter = new ObjectParameter("pDataInicial", typeof(System.DateTime));
            }
    
            ObjectParameter pDataFinalParameter;
    
            if (pDataFinal.HasValue)
            {
                pDataFinalParameter = new ObjectParameter("pDataFinal", pDataFinal);
            }
            else
            {
                pDataFinalParameter = new ObjectParameter("pDataFinal", typeof(System.DateTime));
            }
    
            ObjectParameter pUsuarioParameter;
    
            if (pUsuario != null)
            {
                pUsuarioParameter = new ObjectParameter("pUsuario", pUsuario);
            }
            else
            {
                pUsuarioParameter = new ObjectParameter("pUsuario", typeof(string));
            }
            return base.ExecuteFunction("Integra_Baixa_Ordem_Producao", pDataInicialParameter, pDataFinalParameter, pUsuarioParameter);
        }

        #endregion

    }
}