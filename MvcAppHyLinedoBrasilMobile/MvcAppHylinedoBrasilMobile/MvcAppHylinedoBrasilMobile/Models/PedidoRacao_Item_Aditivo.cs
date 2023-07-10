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
    [KnownType(typeof(PedidoRacao))]
    [KnownType(typeof(PedidoRacao_Item))]
    public partial class PedidoRacao_Item_Aditivo: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int IDPedidoRacao
        {
            get { return _iDPedidoRacao; }
            set
            {
                if (_iDPedidoRacao != value)
                {
                    ChangeTracker.RecordOriginalValue("IDPedidoRacao", _iDPedidoRacao);
                    if (!IsDeserializing)
                    {
                        if (PedidoRacao != null && PedidoRacao.ID != value)
                        {
                            PedidoRacao = null;
                        }
                    }
                    _iDPedidoRacao = value;
                    OnPropertyChanged("IDPedidoRacao");
                }
            }
        }
        private int _iDPedidoRacao;
    
        [DataMember]
        public int IDPedidoRacao_Item
        {
            get { return _iDPedidoRacao_Item; }
            set
            {
                if (_iDPedidoRacao_Item != value)
                {
                    ChangeTracker.RecordOriginalValue("IDPedidoRacao_Item", _iDPedidoRacao_Item);
                    if (!IsDeserializing)
                    {
                        if (PedidoRacao_Item != null && PedidoRacao_Item.ID != value)
                        {
                            PedidoRacao_Item = null;
                        }
                    }
                    _iDPedidoRacao_Item = value;
                    OnPropertyChanged("IDPedidoRacao_Item");
                }
            }
        }
        private int _iDPedidoRacao_Item;
    
        [DataMember]
        public string ProdCodEstr
        {
            get { return _prodCodEstr; }
            set
            {
                if (_prodCodEstr != value)
                {
                    _prodCodEstr = value;
                    OnPropertyChanged("ProdCodEstr");
                }
            }
        }
        private string _prodCodEstr;
    
        [DataMember]
        public Nullable<decimal> QtdeKgPorTon
        {
            get { return _qtdeKgPorTon; }
            set
            {
                if (_qtdeKgPorTon != value)
                {
                    _qtdeKgPorTon = value;
                    OnPropertyChanged("QtdeKgPorTon");
                }
            }
        }
        private Nullable<decimal> _qtdeKgPorTon;
    
        [DataMember]
        public Nullable<int> Sequencia
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
        private Nullable<int> _sequencia;
    
        [DataMember]
        public Nullable<int> SeqItem
        {
            get { return _seqItem; }
            set
            {
                if (_seqItem != value)
                {
                    _seqItem = value;
                    OnPropertyChanged("SeqItem");
                }
            }
        }
        private Nullable<int> _seqItem;
    
        [DataMember]
        public string Origem
        {
            get { return _origem; }
            set
            {
                if (_origem != value)
                {
                    _origem = value;
                    OnPropertyChanged("Origem");
                }
            }
        }
        private string _origem;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public PedidoRacao PedidoRacao
        {
            get { return _pedidoRacao; }
            set
            {
                if (!ReferenceEquals(_pedidoRacao, value))
                {
                    var previousValue = _pedidoRacao;
                    _pedidoRacao = value;
                    FixupPedidoRacao(previousValue);
                    OnNavigationPropertyChanged("PedidoRacao");
                }
            }
        }
        private PedidoRacao _pedidoRacao;
    
        [DataMember]
        public PedidoRacao_Item PedidoRacao_Item
        {
            get { return _pedidoRacao_Item; }
            set
            {
                if (!ReferenceEquals(_pedidoRacao_Item, value))
                {
                    var previousValue = _pedidoRacao_Item;
                    _pedidoRacao_Item = value;
                    FixupPedidoRacao_Item(previousValue);
                    OnNavigationPropertyChanged("PedidoRacao_Item");
                }
            }
        }
        private PedidoRacao_Item _pedidoRacao_Item;

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
            PedidoRacao = null;
            PedidoRacao_Item = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupPedidoRacao(PedidoRacao previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PedidoRacao_Item_Aditivo.Contains(this))
            {
                previousValue.PedidoRacao_Item_Aditivo.Remove(this);
            }
    
            if (PedidoRacao != null)
            {
                if (!PedidoRacao.PedidoRacao_Item_Aditivo.Contains(this))
                {
                    PedidoRacao.PedidoRacao_Item_Aditivo.Add(this);
                }
    
                IDPedidoRacao = PedidoRacao.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PedidoRacao")
                    && (ChangeTracker.OriginalValues["PedidoRacao"] == PedidoRacao))
                {
                    ChangeTracker.OriginalValues.Remove("PedidoRacao");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PedidoRacao", previousValue);
                }
                if (PedidoRacao != null && !PedidoRacao.ChangeTracker.ChangeTrackingEnabled)
                {
                    PedidoRacao.StartTracking();
                }
            }
        }
    
        private void FixupPedidoRacao_Item(PedidoRacao_Item previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PedidoRacao_Item_Aditivo.Contains(this))
            {
                previousValue.PedidoRacao_Item_Aditivo.Remove(this);
            }
    
            if (PedidoRacao_Item != null)
            {
                if (!PedidoRacao_Item.PedidoRacao_Item_Aditivo.Contains(this))
                {
                    PedidoRacao_Item.PedidoRacao_Item_Aditivo.Add(this);
                }
    
                IDPedidoRacao_Item = PedidoRacao_Item.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PedidoRacao_Item")
                    && (ChangeTracker.OriginalValues["PedidoRacao_Item"] == PedidoRacao_Item))
                {
                    ChangeTracker.OriginalValues.Remove("PedidoRacao_Item");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PedidoRacao_Item", previousValue);
                }
                if (PedidoRacao_Item != null && !PedidoRacao_Item.ChangeTracker.ChangeTrackingEnabled)
                {
                    PedidoRacao_Item.StartTracking();
                }
            }
        }

        #endregion

    }
}