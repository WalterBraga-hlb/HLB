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
    [KnownType(typeof(Pedido_Venda))]
    public partial class Servicos_Pedido_Venda: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int IDPedidoVenda
        {
            get { return _iDPedidoVenda; }
            set
            {
                if (_iDPedidoVenda != value)
                {
                    ChangeTracker.RecordOriginalValue("IDPedidoVenda", _iDPedidoVenda);
                    if (!IsDeserializing)
                    {
                        if (Pedido_Venda != null && Pedido_Venda.ID != value)
                        {
                            Pedido_Venda = null;
                        }
                    }
                    _iDPedidoVenda = value;
                    OnPropertyChanged("IDPedidoVenda");
                }
            }
        }
        private int _iDPedidoVenda;
    
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
        public Nullable<decimal> PercAplicacaoServico
        {
            get { return _percAplicacaoServico; }
            set
            {
                if (_percAplicacaoServico != value)
                {
                    _percAplicacaoServico = value;
                    OnPropertyChanged("PercAplicacaoServico");
                }
            }
        }
        private Nullable<decimal> _percAplicacaoServico;
    
        [DataMember]
        public Nullable<int> Bonificada
        {
            get { return _bonificada; }
            set
            {
                if (_bonificada != value)
                {
                    _bonificada = value;
                    OnPropertyChanged("Bonificada");
                }
            }
        }
        private Nullable<int> _bonificada;
    
        [DataMember]
        public Nullable<decimal> PrecoUnitario
        {
            get { return _precoUnitario; }
            set
            {
                if (_precoUnitario != value)
                {
                    _precoUnitario = value;
                    OnPropertyChanged("PrecoUnitario");
                }
            }
        }
        private Nullable<decimal> _precoUnitario;
    
        [DataMember]
        public string MascaraTI
        {
            get { return _mascaraTI; }
            set
            {
                if (_mascaraTI != value)
                {
                    _mascaraTI = value;
                    OnPropertyChanged("MascaraTI");
                }
            }
        }
        private string _mascaraTI;
    
        [DataMember]
        public string PedidoPS
        {
            get { return _pedidoPS; }
            set
            {
                if (_pedidoPS != value)
                {
                    _pedidoPS = value;
                    OnPropertyChanged("PedidoPS");
                }
            }
        }
        private string _pedidoPS;
    
        [DataMember]
        public Nullable<int> ItPedidoPS
        {
            get { return _itPedidoPS; }
            set
            {
                if (_itPedidoPS != value)
                {
                    _itPedidoPS = value;
                    OnPropertyChanged("ItPedidoPS");
                }
            }
        }
        private Nullable<int> _itPedidoPS;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Pedido_Venda Pedido_Venda
        {
            get { return _pedido_Venda; }
            set
            {
                if (!ReferenceEquals(_pedido_Venda, value))
                {
                    var previousValue = _pedido_Venda;
                    _pedido_Venda = value;
                    FixupPedido_Venda(previousValue);
                    OnNavigationPropertyChanged("Pedido_Venda");
                }
            }
        }
        private Pedido_Venda _pedido_Venda;

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
            Pedido_Venda = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupPedido_Venda(Pedido_Venda previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Servicos_Pedido_Venda.Contains(this))
            {
                previousValue.Servicos_Pedido_Venda.Remove(this);
            }
    
            if (Pedido_Venda != null)
            {
                if (!Pedido_Venda.Servicos_Pedido_Venda.Contains(this))
                {
                    Pedido_Venda.Servicos_Pedido_Venda.Add(this);
                }
    
                IDPedidoVenda = Pedido_Venda.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Pedido_Venda")
                    && (ChangeTracker.OriginalValues["Pedido_Venda"] == Pedido_Venda))
                {
                    ChangeTracker.OriginalValues.Remove("Pedido_Venda");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Pedido_Venda", previousValue);
                }
                if (Pedido_Venda != null && !Pedido_Venda.ChangeTracker.ChangeTrackingEnabled)
                {
                    Pedido_Venda.StartTracking();
                }
            }
        }

        #endregion

    }
}
