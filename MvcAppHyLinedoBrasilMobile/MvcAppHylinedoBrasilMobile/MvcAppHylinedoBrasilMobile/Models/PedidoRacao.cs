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
    [KnownType(typeof(PedidoRacao_Item))]
    [KnownType(typeof(PedidoRacao_Item_Aditivo))]
    public partial class PedidoRacao: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public Nullable<System.DateTime> DataPedido
        {
            get { return _dataPedido; }
            set
            {
                if (_dataPedido != value)
                {
                    _dataPedido = value;
                    OnPropertyChanged("DataPedido");
                }
            }
        }
        private Nullable<System.DateTime> _dataPedido;
    
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
        public string StatusPedido
        {
            get { return _statusPedido; }
            set
            {
                if (_statusPedido != value)
                {
                    _statusPedido = value;
                    OnPropertyChanged("StatusPedido");
                }
            }
        }
        private string _statusPedido;
    
        [DataMember]
        public string RotaEntregaCod
        {
            get { return _rotaEntregaCod; }
            set
            {
                if (_rotaEntregaCod != value)
                {
                    _rotaEntregaCod = value;
                    OnPropertyChanged("RotaEntregaCod");
                }
            }
        }
        private string _rotaEntregaCod;
    
        [DataMember]
        public Nullable<int> Ordem
        {
            get { return _ordem; }
            set
            {
                if (_ordem != value)
                {
                    _ordem = value;
                    OnPropertyChanged("Ordem");
                }
            }
        }
        private Nullable<int> _ordem;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<PedidoRacao_Item> PedidoRacao_Item
        {
            get
            {
                if (_pedidoRacao_Item == null)
                {
                    _pedidoRacao_Item = new TrackableCollection<PedidoRacao_Item>();
                    _pedidoRacao_Item.CollectionChanged += FixupPedidoRacao_Item;
                }
                return _pedidoRacao_Item;
            }
            set
            {
                if (!ReferenceEquals(_pedidoRacao_Item, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_pedidoRacao_Item != null)
                    {
                        _pedidoRacao_Item.CollectionChanged -= FixupPedidoRacao_Item;
                    }
                    _pedidoRacao_Item = value;
                    if (_pedidoRacao_Item != null)
                    {
                        _pedidoRacao_Item.CollectionChanged += FixupPedidoRacao_Item;
                    }
                    OnNavigationPropertyChanged("PedidoRacao_Item");
                }
            }
        }
        private TrackableCollection<PedidoRacao_Item> _pedidoRacao_Item;
    
        [DataMember]
        public TrackableCollection<PedidoRacao_Item_Aditivo> PedidoRacao_Item_Aditivo
        {
            get
            {
                if (_pedidoRacao_Item_Aditivo == null)
                {
                    _pedidoRacao_Item_Aditivo = new TrackableCollection<PedidoRacao_Item_Aditivo>();
                    _pedidoRacao_Item_Aditivo.CollectionChanged += FixupPedidoRacao_Item_Aditivo;
                }
                return _pedidoRacao_Item_Aditivo;
            }
            set
            {
                if (!ReferenceEquals(_pedidoRacao_Item_Aditivo, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_pedidoRacao_Item_Aditivo != null)
                    {
                        _pedidoRacao_Item_Aditivo.CollectionChanged -= FixupPedidoRacao_Item_Aditivo;
                    }
                    _pedidoRacao_Item_Aditivo = value;
                    if (_pedidoRacao_Item_Aditivo != null)
                    {
                        _pedidoRacao_Item_Aditivo.CollectionChanged += FixupPedidoRacao_Item_Aditivo;
                    }
                    OnNavigationPropertyChanged("PedidoRacao_Item_Aditivo");
                }
            }
        }
        private TrackableCollection<PedidoRacao_Item_Aditivo> _pedidoRacao_Item_Aditivo;

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
            PedidoRacao_Item.Clear();
            PedidoRacao_Item_Aditivo.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupPedidoRacao_Item(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PedidoRacao_Item item in e.NewItems)
                {
                    item.PedidoRacao = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PedidoRacao_Item", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PedidoRacao_Item item in e.OldItems)
                {
                    if (ReferenceEquals(item.PedidoRacao, this))
                    {
                        item.PedidoRacao = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PedidoRacao_Item", item);
                    }
                }
            }
        }
    
        private void FixupPedidoRacao_Item_Aditivo(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PedidoRacao_Item_Aditivo item in e.NewItems)
                {
                    item.PedidoRacao = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PedidoRacao_Item_Aditivo", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PedidoRacao_Item_Aditivo item in e.OldItems)
                {
                    if (ReferenceEquals(item.PedidoRacao, this))
                    {
                        item.PedidoRacao = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PedidoRacao_Item_Aditivo", item);
                    }
                }
            }
        }

        #endregion

    }
}
