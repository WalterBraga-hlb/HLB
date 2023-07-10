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
    public partial class LOG_Vacinas_Secundaria_Pedido_Venda: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int IDVacPrimPedVenda
        {
            get { return _iDVacPrimPedVenda; }
            set
            {
                if (_iDVacPrimPedVenda != value)
                {
                    _iDVacPrimPedVenda = value;
                    OnPropertyChanged("IDVacPrimPedVenda");
                }
            }
        }
        private int _iDVacPrimPedVenda;
    
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
        public System.DateTime DataHora
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
        private System.DateTime _dataHora;
    
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
        public Nullable<int> IDVacSecPedVenda
        {
            get { return _iDVacSecPedVenda; }
            set
            {
                if (_iDVacSecPedVenda != value)
                {
                    _iDVacSecPedVenda = value;
                    OnPropertyChanged("IDVacSecPedVenda");
                }
            }
        }
        private Nullable<int> _iDVacSecPedVenda;
    
        [DataMember]
        public Nullable<int> IDVacPrimLogPedidoVenda
        {
            get { return _iDVacPrimLogPedidoVenda; }
            set
            {
                if (_iDVacPrimLogPedidoVenda != value)
                {
                    _iDVacPrimLogPedidoVenda = value;
                    OnPropertyChanged("IDVacPrimLogPedidoVenda");
                }
            }
        }
        private Nullable<int> _iDVacPrimLogPedidoVenda;
    
        [DataMember]
        public Nullable<int> SeqItemPedVenda
        {
            get { return _seqItemPedVenda; }
            set
            {
                if (_seqItemPedVenda != value)
                {
                    _seqItemPedVenda = value;
                    OnPropertyChanged("SeqItemPedVenda");
                }
            }
        }
        private Nullable<int> _seqItemPedVenda;
    
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
