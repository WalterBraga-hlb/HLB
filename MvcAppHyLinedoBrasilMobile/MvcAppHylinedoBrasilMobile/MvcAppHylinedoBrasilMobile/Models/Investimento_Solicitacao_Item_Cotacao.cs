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
    [KnownType(typeof(Investimento_Solicitacao_Item))]
    public partial class Investimento_Solicitacao_Item_Cotacao: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int IDInvestimentoSolicitacaoItem
        {
            get { return _iDInvestimentoSolicitacaoItem; }
            set
            {
                if (_iDInvestimentoSolicitacaoItem != value)
                {
                    ChangeTracker.RecordOriginalValue("IDInvestimentoSolicitacaoItem", _iDInvestimentoSolicitacaoItem);
                    if (!IsDeserializing)
                    {
                        if (Investimento_Solicitacao_Item != null && Investimento_Solicitacao_Item.ID != value)
                        {
                            Investimento_Solicitacao_Item = null;
                        }
                    }
                    _iDInvestimentoSolicitacaoItem = value;
                    OnPropertyChanged("IDInvestimentoSolicitacaoItem");
                }
            }
        }
        private int _iDInvestimentoSolicitacaoItem;
    
        [DataMember]
        public int SequenciaItem
        {
            get { return _sequenciaItem; }
            set
            {
                if (_sequenciaItem != value)
                {
                    _sequenciaItem = value;
                    OnPropertyChanged("SequenciaItem");
                }
            }
        }
        private int _sequenciaItem;
    
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
        public string FornecedorCodigo
        {
            get { return _fornecedorCodigo; }
            set
            {
                if (_fornecedorCodigo != value)
                {
                    _fornecedorCodigo = value;
                    OnPropertyChanged("FornecedorCodigo");
                }
            }
        }
        private string _fornecedorCodigo;
    
        [DataMember]
        public string FornecedorDescricao
        {
            get { return _fornecedorDescricao; }
            set
            {
                if (_fornecedorDescricao != value)
                {
                    _fornecedorDescricao = value;
                    OnPropertyChanged("FornecedorDescricao");
                }
            }
        }
        private string _fornecedorDescricao;
    
        [DataMember]
        public decimal Valor
        {
            get { return _valor; }
            set
            {
                if (_valor != value)
                {
                    _valor = value;
                    OnPropertyChanged("Valor");
                }
            }
        }
        private decimal _valor;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Investimento_Solicitacao_Item Investimento_Solicitacao_Item
        {
            get { return _investimento_Solicitacao_Item; }
            set
            {
                if (!ReferenceEquals(_investimento_Solicitacao_Item, value))
                {
                    var previousValue = _investimento_Solicitacao_Item;
                    _investimento_Solicitacao_Item = value;
                    FixupInvestimento_Solicitacao_Item(previousValue);
                    OnNavigationPropertyChanged("Investimento_Solicitacao_Item");
                }
            }
        }
        private Investimento_Solicitacao_Item _investimento_Solicitacao_Item;

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
            Investimento_Solicitacao_Item = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupInvestimento_Solicitacao_Item(Investimento_Solicitacao_Item previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Investimento_Solicitacao_Item_Cotacao.Contains(this))
            {
                previousValue.Investimento_Solicitacao_Item_Cotacao.Remove(this);
            }
    
            if (Investimento_Solicitacao_Item != null)
            {
                if (!Investimento_Solicitacao_Item.Investimento_Solicitacao_Item_Cotacao.Contains(this))
                {
                    Investimento_Solicitacao_Item.Investimento_Solicitacao_Item_Cotacao.Add(this);
                }
    
                IDInvestimentoSolicitacaoItem = Investimento_Solicitacao_Item.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Investimento_Solicitacao_Item")
                    && (ChangeTracker.OriginalValues["Investimento_Solicitacao_Item"] == Investimento_Solicitacao_Item))
                {
                    ChangeTracker.OriginalValues.Remove("Investimento_Solicitacao_Item");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Investimento_Solicitacao_Item", previousValue);
                }
                if (Investimento_Solicitacao_Item != null && !Investimento_Solicitacao_Item.ChangeTracker.ChangeTrackingEnabled)
                {
                    Investimento_Solicitacao_Item.StartTracking();
                }
            }
        }

        #endregion

    }
}
