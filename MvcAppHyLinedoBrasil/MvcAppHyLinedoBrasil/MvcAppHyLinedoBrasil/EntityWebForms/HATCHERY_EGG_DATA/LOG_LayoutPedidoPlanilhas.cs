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

namespace MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA
{
    [DataContract(IsReference = true)]
    public partial class LOG_LayoutPedidoPlanilhas: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string CodigoCliente
        {
            get { return _codigoCliente; }
            set
            {
                if (_codigoCliente != value)
                {
                    _codigoCliente = value;
                    OnPropertyChanged("CodigoCliente");
                }
            }
        }
        private string _codigoCliente;
    
        [DataMember]
        public string DescricaoCliente
        {
            get { return _descricaoCliente; }
            set
            {
                if (_descricaoCliente != value)
                {
                    _descricaoCliente = value;
                    OnPropertyChanged("DescricaoCliente");
                }
            }
        }
        private string _descricaoCliente;
    
        [DataMember]
        public string Cidade
        {
            get { return _cidade; }
            set
            {
                if (_cidade != value)
                {
                    _cidade = value;
                    OnPropertyChanged("Cidade");
                }
            }
        }
        private string _cidade;
    
        [DataMember]
        public string Estado
        {
            get { return _estado; }
            set
            {
                if (_estado != value)
                {
                    _estado = value;
                    OnPropertyChanged("Estado");
                }
            }
        }
        private string _estado;
    
        [DataMember]
        public string Vacina
        {
            get { return _vacina; }
            set
            {
                if (_vacina != value)
                {
                    _vacina = value;
                    OnPropertyChanged("Vacina");
                }
            }
        }
        private string _vacina;
    
        [DataMember]
        public Nullable<int> Bouba
        {
            get { return _bouba; }
            set
            {
                if (_bouba != value)
                {
                    _bouba = value;
                    OnPropertyChanged("Bouba");
                }
            }
        }
        private Nullable<int> _bouba;
    
        [DataMember]
        public Nullable<int> Gombouro
        {
            get { return _gombouro; }
            set
            {
                if (_gombouro != value)
                {
                    _gombouro = value;
                    OnPropertyChanged("Gombouro");
                }
            }
        }
        private Nullable<int> _gombouro;
    
        [DataMember]
        public Nullable<int> Coccidiose
        {
            get { return _coccidiose; }
            set
            {
                if (_coccidiose != value)
                {
                    _coccidiose = value;
                    OnPropertyChanged("Coccidiose");
                }
            }
        }
        private Nullable<int> _coccidiose;
    
        [DataMember]
        public Nullable<int> Laringo
        {
            get { return _laringo; }
            set
            {
                if (_laringo != value)
                {
                    _laringo = value;
                    OnPropertyChanged("Laringo");
                }
            }
        }
        private Nullable<int> _laringo;
    
        [DataMember]
        public Nullable<int> Salmonela
        {
            get { return _salmonela; }
            set
            {
                if (_salmonela != value)
                {
                    _salmonela = value;
                    OnPropertyChanged("Salmonela");
                }
            }
        }
        private Nullable<int> _salmonela;
    
        [DataMember]
        public Nullable<int> TratamentoInfravermelho
        {
            get { return _tratamentoInfravermelho; }
            set
            {
                if (_tratamentoInfravermelho != value)
                {
                    _tratamentoInfravermelho = value;
                    OnPropertyChanged("TratamentoInfravermelho");
                }
            }
        }
        private Nullable<int> _tratamentoInfravermelho;
    
        [DataMember]
        public Nullable<int> QtdePintinhosTratInfraVerm
        {
            get { return _qtdePintinhosTratInfraVerm; }
            set
            {
                if (_qtdePintinhosTratInfraVerm != value)
                {
                    _qtdePintinhosTratInfraVerm = value;
                    OnPropertyChanged("QtdePintinhosTratInfraVerm");
                }
            }
        }
        private Nullable<int> _qtdePintinhosTratInfraVerm;
    
        [DataMember]
        public Nullable<int> OvosBrasil
        {
            get { return _ovosBrasil; }
            set
            {
                if (_ovosBrasil != value)
                {
                    _ovosBrasil = value;
                    OnPropertyChanged("OvosBrasil");
                }
            }
        }
        private Nullable<int> _ovosBrasil;
    
        [DataMember]
        public string Embalagem
        {
            get { return _embalagem; }
            set
            {
                if (_embalagem != value)
                {
                    _embalagem = value;
                    OnPropertyChanged("Embalagem");
                }
            }
        }
        private string _embalagem;
    
        [DataMember]
        public string CondicaoPagamento
        {
            get { return _condicaoPagamento; }
            set
            {
                if (_condicaoPagamento != value)
                {
                    _condicaoPagamento = value;
                    OnPropertyChanged("CondicaoPagamento");
                }
            }
        }
        private string _condicaoPagamento;
    
        [DataMember]
        public string Observacao
        {
            get { return _observacao; }
            set
            {
                if (_observacao != value)
                {
                    _observacao = value;
                    OnPropertyChanged("Observacao");
                }
            }
        }
        private string _observacao;
    
        [DataMember]
        public string Vendedor
        {
            get { return _vendedor; }
            set
            {
                if (_vendedor != value)
                {
                    _vendedor = value;
                    OnPropertyChanged("Vendedor");
                }
            }
        }
        private string _vendedor;
    
        [DataMember]
        public Nullable<int> NumeroPedidoCHIC
        {
            get { return _numeroPedidoCHIC; }
            set
            {
                if (_numeroPedidoCHIC != value)
                {
                    _numeroPedidoCHIC = value;
                    OnPropertyChanged("NumeroPedidoCHIC");
                }
            }
        }
        private Nullable<int> _numeroPedidoCHIC;
    
        [DataMember]
        public Nullable<System.DateTime> DataInicial
        {
            get { return _dataInicial; }
            set
            {
                if (_dataInicial != value)
                {
                    _dataInicial = value;
                    OnPropertyChanged("DataInicial");
                }
            }
        }
        private Nullable<System.DateTime> _dataInicial;
    
        [DataMember]
        public Nullable<System.DateTime> DataFinal
        {
            get { return _dataFinal; }
            set
            {
                if (_dataFinal != value)
                {
                    _dataFinal = value;
                    OnPropertyChanged("DataFinal");
                }
            }
        }
        private Nullable<System.DateTime> _dataFinal;
    
        [DataMember]
        public string Linhagem
        {
            get { return _linhagem; }
            set
            {
                if (_linhagem != value)
                {
                    _linhagem = value;
                    OnPropertyChanged("Linhagem");
                }
            }
        }
        private string _linhagem;
    
        [DataMember]
        public Nullable<int> QtdeLiquida
        {
            get { return _qtdeLiquida; }
            set
            {
                if (_qtdeLiquida != value)
                {
                    _qtdeLiquida = value;
                    OnPropertyChanged("QtdeLiquida");
                }
            }
        }
        private Nullable<int> _qtdeLiquida;
    
        [DataMember]
        public Nullable<decimal> PercBonificacao
        {
            get { return _percBonificacao; }
            set
            {
                if (_percBonificacao != value)
                {
                    _percBonificacao = value;
                    OnPropertyChanged("PercBonificacao");
                }
            }
        }
        private Nullable<decimal> _percBonificacao;
    
        [DataMember]
        public Nullable<int> QtdeBonificacao
        {
            get { return _qtdeBonificacao; }
            set
            {
                if (_qtdeBonificacao != value)
                {
                    _qtdeBonificacao = value;
                    OnPropertyChanged("QtdeBonificacao");
                }
            }
        }
        private Nullable<int> _qtdeBonificacao;
    
        [DataMember]
        public Nullable<int> QtdeReposicao
        {
            get { return _qtdeReposicao; }
            set
            {
                if (_qtdeReposicao != value)
                {
                    _qtdeReposicao = value;
                    OnPropertyChanged("QtdeReposicao");
                }
            }
        }
        private Nullable<int> _qtdeReposicao;
    
        [DataMember]
        public Nullable<int> QtdeTotal
        {
            get { return _qtdeTotal; }
            set
            {
                if (_qtdeTotal != value)
                {
                    _qtdeTotal = value;
                    OnPropertyChanged("QtdeTotal");
                }
            }
        }
        private Nullable<int> _qtdeTotal;
    
        [DataMember]
        public Nullable<decimal> ValorUnitario
        {
            get { return _valorUnitario; }
            set
            {
                if (_valorUnitario != value)
                {
                    _valorUnitario = value;
                    OnPropertyChanged("ValorUnitario");
                }
            }
        }
        private Nullable<decimal> _valorUnitario;
    
        [DataMember]
        public Nullable<decimal> ValorTotal
        {
            get { return _valorTotal; }
            set
            {
                if (_valorTotal != value)
                {
                    _valorTotal = value;
                    OnPropertyChanged("ValorTotal");
                }
            }
        }
        private Nullable<decimal> _valorTotal;
    
        [DataMember]
        public string EmailVendedor
        {
            get { return _emailVendedor; }
            set
            {
                if (_emailVendedor != value)
                {
                    _emailVendedor = value;
                    OnPropertyChanged("EmailVendedor");
                }
            }
        }
        private string _emailVendedor;
    
        [DataMember]
        public string MotivoOperacao
        {
            get { return _motivoOperacao; }
            set
            {
                if (_motivoOperacao != value)
                {
                    _motivoOperacao = value;
                    OnPropertyChanged("MotivoOperacao");
                }
            }
        }
        private string _motivoOperacao;
    
        [DataMember]
        public string CaminhoArquivo
        {
            get { return _caminhoArquivo; }
            set
            {
                if (_caminhoArquivo != value)
                {
                    _caminhoArquivo = value;
                    OnPropertyChanged("CaminhoArquivo");
                }
            }
        }
        private string _caminhoArquivo;

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