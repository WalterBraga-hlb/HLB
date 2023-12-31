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
    [KnownType(typeof(Investimento_Solicitacao))]
    [KnownType(typeof(Investimento_Solicitacao_Item_Cotacao))]
    public partial class Investimento_Solicitacao_Item: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int IDInvestimentoSolicitacao
        {
            get { return _iDInvestimentoSolicitacao; }
            set
            {
                if (_iDInvestimentoSolicitacao != value)
                {
                    ChangeTracker.RecordOriginalValue("IDInvestimentoSolicitacao", _iDInvestimentoSolicitacao);
                    if (!IsDeserializing)
                    {
                        if (Investimento_Solicitacao != null && Investimento_Solicitacao.ID != value)
                        {
                            Investimento_Solicitacao = null;
                        }
                    }
                    _iDInvestimentoSolicitacao = value;
                    OnPropertyChanged("IDInvestimentoSolicitacao");
                }
            }
        }
        private int _iDInvestimentoSolicitacao;
    
        [DataMember]
        public int Sequencia
        {
            get { return _sequencia; }
            set
            {
                if (_sequencia != value)
                {
                    _sequencia = value;
                    OnPropertyChanged("Sequencia");
                }
            }
        }
        private int _sequencia;
    
        [DataMember]
        public string Categoria
        {
            get { return _categoria; }
            set
            {
                if (_categoria != value)
                {
                    _categoria = value;
                    OnPropertyChanged("Categoria");
                }
            }
        }
        private string _categoria;
    
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
        public string CodigoProdutoApolo
        {
            get { return _codigoProdutoApolo; }
            set
            {
                if (_codigoProdutoApolo != value)
                {
                    _codigoProdutoApolo = value;
                    OnPropertyChanged("CodigoProdutoApolo");
                }
            }
        }
        private string _codigoProdutoApolo;
    
        [DataMember]
        public Nullable<int> IDCotacaoEscolhida
        {
            get { return _iDCotacaoEscolhida; }
            set
            {
                if (_iDCotacaoEscolhida != value)
                {
                    _iDCotacaoEscolhida = value;
                    OnPropertyChanged("IDCotacaoEscolhida");
                }
            }
        }
        private Nullable<int> _iDCotacaoEscolhida;
    
        [DataMember]
        public string RazaoNaoTer03Cotacoes
        {
            get { return _razaoNaoTer03Cotacoes; }
            set
            {
                if (_razaoNaoTer03Cotacoes != value)
                {
                    _razaoNaoTer03Cotacoes = value;
                    OnPropertyChanged("RazaoNaoTer03Cotacoes");
                }
            }
        }
        private string _razaoNaoTer03Cotacoes;
    
        [DataMember]
        public string RazaoExcederOrcamentoOuNaoUtilizarMenorCotacao
        {
            get { return _razaoExcederOrcamentoOuNaoUtilizarMenorCotacao; }
            set
            {
                if (_razaoExcederOrcamentoOuNaoUtilizarMenorCotacao != value)
                {
                    _razaoExcederOrcamentoOuNaoUtilizarMenorCotacao = value;
                    OnPropertyChanged("RazaoExcederOrcamentoOuNaoUtilizarMenorCotacao");
                }
            }
        }
        private string _razaoExcederOrcamentoOuNaoUtilizarMenorCotacao;
    
        [DataMember]
        public string EmpresaPedidoCompraApolo
        {
            get { return _empresaPedidoCompraApolo; }
            set
            {
                if (_empresaPedidoCompraApolo != value)
                {
                    _empresaPedidoCompraApolo = value;
                    OnPropertyChanged("EmpresaPedidoCompraApolo");
                }
            }
        }
        private string _empresaPedidoCompraApolo;
    
        [DataMember]
        public string NumeroPedidoCompraApolo
        {
            get { return _numeroPedidoCompraApolo; }
            set
            {
                if (_numeroPedidoCompraApolo != value)
                {
                    _numeroPedidoCompraApolo = value;
                    OnPropertyChanged("NumeroPedidoCompraApolo");
                }
            }
        }
        private string _numeroPedidoCompraApolo;
    
        [DataMember]
        public int Qtde
        {
            get { return _qtde; }
            set
            {
                if (_qtde != value)
                {
                    _qtde = value;
                    OnPropertyChanged("Qtde");
                }
            }
        }
        private int _qtde;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Investimento_Solicitacao Investimento_Solicitacao
        {
            get { return _investimento_Solicitacao; }
            set
            {
                if (!ReferenceEquals(_investimento_Solicitacao, value))
                {
                    var previousValue = _investimento_Solicitacao;
                    _investimento_Solicitacao = value;
                    FixupInvestimento_Solicitacao(previousValue);
                    OnNavigationPropertyChanged("Investimento_Solicitacao");
                }
            }
        }
        private Investimento_Solicitacao _investimento_Solicitacao;
    
        [DataMember]
        public TrackableCollection<Investimento_Solicitacao_Item_Cotacao> Investimento_Solicitacao_Item_Cotacao
        {
            get
            {
                if (_investimento_Solicitacao_Item_Cotacao == null)
                {
                    _investimento_Solicitacao_Item_Cotacao = new TrackableCollection<Investimento_Solicitacao_Item_Cotacao>();
                    _investimento_Solicitacao_Item_Cotacao.CollectionChanged += FixupInvestimento_Solicitacao_Item_Cotacao;
                }
                return _investimento_Solicitacao_Item_Cotacao;
            }
            set
            {
                if (!ReferenceEquals(_investimento_Solicitacao_Item_Cotacao, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_investimento_Solicitacao_Item_Cotacao != null)
                    {
                        _investimento_Solicitacao_Item_Cotacao.CollectionChanged -= FixupInvestimento_Solicitacao_Item_Cotacao;
                    }
                    _investimento_Solicitacao_Item_Cotacao = value;
                    if (_investimento_Solicitacao_Item_Cotacao != null)
                    {
                        _investimento_Solicitacao_Item_Cotacao.CollectionChanged += FixupInvestimento_Solicitacao_Item_Cotacao;
                    }
                    OnNavigationPropertyChanged("Investimento_Solicitacao_Item_Cotacao");
                }
            }
        }
        private TrackableCollection<Investimento_Solicitacao_Item_Cotacao> _investimento_Solicitacao_Item_Cotacao;

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
            Investimento_Solicitacao = null;
            Investimento_Solicitacao_Item_Cotacao.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupInvestimento_Solicitacao(Investimento_Solicitacao previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Investimento_Solicitacao_Item.Contains(this))
            {
                previousValue.Investimento_Solicitacao_Item.Remove(this);
            }
    
            if (Investimento_Solicitacao != null)
            {
                if (!Investimento_Solicitacao.Investimento_Solicitacao_Item.Contains(this))
                {
                    Investimento_Solicitacao.Investimento_Solicitacao_Item.Add(this);
                }
    
                IDInvestimentoSolicitacao = Investimento_Solicitacao.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Investimento_Solicitacao")
                    && (ChangeTracker.OriginalValues["Investimento_Solicitacao"] == Investimento_Solicitacao))
                {
                    ChangeTracker.OriginalValues.Remove("Investimento_Solicitacao");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Investimento_Solicitacao", previousValue);
                }
                if (Investimento_Solicitacao != null && !Investimento_Solicitacao.ChangeTracker.ChangeTrackingEnabled)
                {
                    Investimento_Solicitacao.StartTracking();
                }
            }
        }
    
        private void FixupInvestimento_Solicitacao_Item_Cotacao(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Investimento_Solicitacao_Item_Cotacao item in e.NewItems)
                {
                    item.Investimento_Solicitacao_Item = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Investimento_Solicitacao_Item_Cotacao", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Investimento_Solicitacao_Item_Cotacao item in e.OldItems)
                {
                    if (ReferenceEquals(item.Investimento_Solicitacao_Item, this))
                    {
                        item.Investimento_Solicitacao_Item = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Investimento_Solicitacao_Item_Cotacao", item);
                    }
                }
            }
        }

        #endregion

    }
}
