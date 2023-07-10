//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace MvcAppHylinedoBrasilMobile.Models
{
    [DataContract(IsReference = true)]
    public partial class LOG_RDV_LANCAMENTO: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int ID
        {
            get { return _iD; }
            set
            {
                if (_iD != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ID' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _iD = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        private int _iD;
    
        [DataMember]
        public System.DateTime DataHoraOperacao
        {
            get { return _dataHoraOperacao; }
            set
            {
                if (_dataHoraOperacao != value)
                {
                    _dataHoraOperacao = value;
                    OnPropertyChanged("DataHoraOperacao");
                }
            }
        }
        private System.DateTime _dataHoraOperacao;
    
        [DataMember]
        public string UsuarioOperacao
        {
            get { return _usuarioOperacao; }
            set
            {
                if (_usuarioOperacao != value)
                {
                    _usuarioOperacao = value;
                    OnPropertyChanged("UsuarioOperacao");
                }
            }
        }
        private string _usuarioOperacao;
    
        [DataMember]
        public string Operacao
        {
            get { return _operacao; }
            set
            {
                if (_operacao != value)
                {
                    _operacao = value;
                    OnPropertyChanged("Operacao");
                }
            }
        }
        private string _operacao;
    
        [DataMember]
        public string Empresa
        {
            get { return _empresa; }
            set
            {
                if (_empresa != value)
                {
                    _empresa = value;
                    OnPropertyChanged("Empresa");
                }
            }
        }
        private string _empresa;
    
        [DataMember]
        public string Usuario
        {
            get { return _usuario; }
            set
            {
                if (_usuario != value)
                {
                    _usuario = value;
                    OnPropertyChanged("Usuario");
                }
            }
        }
        private string _usuario;
    
        [DataMember]
        public Nullable<System.DateTime> DataHora
        {
            get { return _dataHora; }
            set
            {
                if (_dataHora != value)
                {
                    _dataHora = value;
                    OnPropertyChanged("DataHora");
                }
            }
        }
        private Nullable<System.DateTime> _dataHora;
    
        [DataMember]
        public string NomeUsuario
        {
            get { return _nomeUsuario; }
            set
            {
                if (_nomeUsuario != value)
                {
                    _nomeUsuario = value;
                    OnPropertyChanged("NomeUsuario");
                }
            }
        }
        private string _nomeUsuario;
    
        [DataMember]
        public System.DateTime DataRDV
        {
            get { return _dataRDV; }
            set
            {
                if (_dataRDV != value)
                {
                    _dataRDV = value;
                    OnPropertyChanged("DataRDV");
                }
            }
        }
        private System.DateTime _dataRDV;
    
        [DataMember]
        public string TipoDespesa
        {
            get { return _tipoDespesa; }
            set
            {
                if (_tipoDespesa != value)
                {
                    _tipoDespesa = value;
                    OnPropertyChanged("TipoDespesa");
                }
            }
        }
        private string _tipoDespesa;
    
        [DataMember]
        public string Descricao
        {
            get { return _descricao; }
            set
            {
                if (_descricao != value)
                {
                    _descricao = value;
                    OnPropertyChanged("Descricao");
                }
            }
        }
        private string _descricao;
    
        [DataMember]
        public string CodCidade
        {
            get { return _codCidade; }
            set
            {
                if (_codCidade != value)
                {
                    _codCidade = value;
                    OnPropertyChanged("CodCidade");
                }
            }
        }
        private string _codCidade;
    
        [DataMember]
        public string NomeCidade
        {
            get { return _nomeCidade; }
            set
            {
                if (_nomeCidade != value)
                {
                    _nomeCidade = value;
                    OnPropertyChanged("NomeCidade");
                }
            }
        }
        private string _nomeCidade;
    
        [DataMember]
        public decimal ValorDespesa
        {
            get { return _valorDespesa; }
            set
            {
                if (_valorDespesa != value)
                {
                    _valorDespesa = value;
                    OnPropertyChanged("ValorDespesa");
                }
            }
        }
        private decimal _valorDespesa;
    
        [DataMember]
        public byte[] ImagemRecibo
        {
            get { return _imagemRecibo; }
            set
            {
                if (_imagemRecibo != value)
                {
                    _imagemRecibo = value;
                    OnPropertyChanged("ImagemRecibo");
                }
            }
        }
        private byte[] _imagemRecibo;
    
        [DataMember]
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        private string _status;
    
        [DataMember]
        public string UsuarioAprovacao
        {
            get { return _usuarioAprovacao; }
            set
            {
                if (_usuarioAprovacao != value)
                {
                    _usuarioAprovacao = value;
                    OnPropertyChanged("UsuarioAprovacao");
                }
            }
        }
        private string _usuarioAprovacao;
    
        [DataMember]
        public Nullable<System.DateTime> DataAprovacao
        {
            get { return _dataAprovacao; }
            set
            {
                if (_dataAprovacao != value)
                {
                    _dataAprovacao = value;
                    OnPropertyChanged("DataAprovacao");
                }
            }
        }
        private Nullable<System.DateTime> _dataAprovacao;
    
        [DataMember]
        public string EmpresaDoc
        {
            get { return _empresaDoc; }
            set
            {
                if (_empresaDoc != value)
                {
                    _empresaDoc = value;
                    OnPropertyChanged("EmpresaDoc");
                }
            }
        }
        private string _empresaDoc;
    
        [DataMember]
        public Nullable<int> ChaveDoc
        {
            get { return _chaveDoc; }
            set
            {
                if (_chaveDoc != value)
                {
                    _chaveDoc = value;
                    OnPropertyChanged("ChaveDoc");
                }
            }
        }
        private Nullable<int> _chaveDoc;
    
        [DataMember]
        public string FormaPagamento
        {
            get { return _formaPagamento; }
            set
            {
                if (_formaPagamento != value)
                {
                    _formaPagamento = value;
                    OnPropertyChanged("FormaPagamento");
                }
            }
        }
        private string _formaPagamento;
    
        [DataMember]
        public Nullable<decimal> ValorMoedaEstrangeira
        {
            get { return _valorMoedaEstrangeira; }
            set
            {
                if (_valorMoedaEstrangeira != value)
                {
                    _valorMoedaEstrangeira = value;
                    OnPropertyChanged("ValorMoedaEstrangeira");
                }
            }
        }
        private Nullable<decimal> _valorMoedaEstrangeira;
    
        [DataMember]
        public string IndEconCod
        {
            get { return _indEconCod; }
            set
            {
                if (_indEconCod != value)
                {
                    _indEconCod = value;
                    OnPropertyChanged("IndEconCod");
                }
            }
        }
        private string _indEconCod;
    
        [DataMember]
        public string IndEconNome
        {
            get { return _indEconNome; }
            set
            {
                if (_indEconNome != value)
                {
                    _indEconNome = value;
                    OnPropertyChanged("IndEconNome");
                }
            }
        }
        private string _indEconNome;
    
        [DataMember]
        public string NumeroFechamentoRDV
        {
            get { return _numeroFechamentoRDV; }
            set
            {
                if (_numeroFechamentoRDV != value)
                {
                    _numeroFechamentoRDV = value;
                    OnPropertyChanged("NumeroFechamentoRDV");
                }
            }
        }
        private string _numeroFechamentoRDV;
    
        [DataMember]
        public string Motivo
        {
            get { return _motivo; }
            set
            {
                if (_motivo != value)
                {
                    _motivo = value;
                    OnPropertyChanged("Motivo");
                }
            }
        }
        private string _motivo;
    
        [DataMember]
        public string MesAnoFatura
        {
            get { return _mesAnoFatura; }
            set
            {
                if (_mesAnoFatura != value)
                {
                    _mesAnoFatura = value;
                    OnPropertyChanged("MesAnoFatura");
                }
            }
        }
        private string _mesAnoFatura;
    
        [DataMember]
        public Nullable<int> AnoMes
        {
            get { return _anoMes; }
            set
            {
                if (_anoMes != value)
                {
                    _anoMes = value;
                    OnPropertyChanged("AnoMes");
                }
            }
        }
        private Nullable<int> _anoMes;
    
        [DataMember]
        public Nullable<decimal> QtdeDiarias
        {
            get { return _qtdeDiarias; }
            set
            {
                if (_qtdeDiarias != value)
                {
                    _qtdeDiarias = value;
                    OnPropertyChanged("QtdeDiarias");
                }
            }
        }
        private Nullable<decimal> _qtdeDiarias;
    
        [DataMember]
        public Nullable<decimal> ValorDiaria
        {
            get { return _valorDiaria; }
            set
            {
                if (_valorDiaria != value)
                {
                    _valorDiaria = value;
                    OnPropertyChanged("ValorDiaria");
                }
            }
        }
        private Nullable<decimal> _valorDiaria;
    
        [DataMember]
        public string CodPais
        {
            get { return _codPais; }
            set
            {
                if (_codPais != value)
                {
                    _codPais = value;
                    OnPropertyChanged("CodPais");
                }
            }
        }
        private string _codPais;
    
        [DataMember]
        public string NomePais
        {
            get { return _nomePais; }
            set
            {
                if (_nomePais != value)
                {
                    _nomePais = value;
                    OnPropertyChanged("NomePais");
                }
            }
        }
        private string _nomePais;
    
        [DataMember]
        public string Banco
        {
            get { return _banco; }
            set
            {
                if (_banco != value)
                {
                    _banco = value;
                    OnPropertyChanged("Banco");
                }
            }
        }
        private string _banco;
    
        [DataMember]
        public string TipoGastoFatura
        {
            get { return _tipoGastoFatura; }
            set
            {
                if (_tipoGastoFatura != value)
                {
                    _tipoGastoFatura = value;
                    OnPropertyChanged("TipoGastoFatura");
                }
            }
        }
        private string _tipoGastoFatura;
    
        [DataMember]
        public string EmpresaFatura
        {
            get { return _empresaFatura; }
            set
            {
                if (_empresaFatura != value)
                {
                    _empresaFatura = value;
                    OnPropertyChanged("EmpresaFatura");
                }
            }
        }
        private string _empresaFatura;
    
        [DataMember]
        public string NumeroCartao
        {
            get { return _numeroCartao; }
            set
            {
                if (_numeroCartao != value)
                {
                    _numeroCartao = value;
                    OnPropertyChanged("NumeroCartao");
                }
            }
        }
        private string _numeroCartao;
    
        [DataMember]
        public string TipoCombustivel
        {
            get { return _tipoCombustivel; }
            set
            {
                if (_tipoCombustivel != value)
                {
                    _tipoCombustivel = value;
                    OnPropertyChanged("TipoCombustivel");
                }
            }
        }
        private string _tipoCombustivel;
    
        [DataMember]
        public Nullable<decimal> QtdeLitros
        {
            get { return _qtdeLitros; }
            set
            {
                if (_qtdeLitros != value)
                {
                    _qtdeLitros = value;
                    OnPropertyChanged("QtdeLitros");
                }
            }
        }
        private Nullable<decimal> _qtdeLitros;
    
        [DataMember]
        public Nullable<decimal> ValorLitro
        {
            get { return _valorLitro; }
            set
            {
                if (_valorLitro != value)
                {
                    _valorLitro = value;
                    OnPropertyChanged("ValorLitro");
                }
            }
        }
        private Nullable<decimal> _valorLitro;
    
        [DataMember]
        public string Placa
        {
            get { return _placa; }
            set
            {
                if (_placa != value)
                {
                    _placa = value;
                    OnPropertyChanged("Placa");
                }
            }
        }
        private string _placa;
    
        [DataMember]
        public Nullable<decimal> Km
        {
            get { return _km; }
            set
            {
                if (_km != value)
                {
                    _km = value;
                    OnPropertyChanged("Km");
                }
            }
        }
        private Nullable<decimal> _km;
    
        [DataMember]
        public int IDRDV
        {
            get { return _iDRDV; }
            set
            {
                if (_iDRDV != value)
                {
                    _iDRDV = value;
                    OnPropertyChanged("IDRDV");
                }
            }
        }
        private int _iDRDV;

        #endregion

        #region ChangeTracking
    
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
        private ObjectChangeTracker _changeTracker;
    
        [DataMember]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        protected bool IsDeserializing { get; private set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        protected virtual void ClearNavigationProperties()
        {
        }

        #endregion

    }
}
