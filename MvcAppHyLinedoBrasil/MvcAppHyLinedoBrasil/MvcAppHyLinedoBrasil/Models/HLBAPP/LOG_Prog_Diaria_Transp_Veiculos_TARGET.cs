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

namespace MvcAppHyLinedoBrasil.Models.HLBAPP
{
    [DataContract(IsReference = true)]
    public partial class LOG_Prog_Diaria_Transp_Veiculos_TARGET: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public Nullable<int> IDProgDiariaTranspVeiculos
        {
            get { return _iDProgDiariaTranspVeiculos; }
            set
            {
                if (_iDProgDiariaTranspVeiculos != value)
                {
                    _iDProgDiariaTranspVeiculos = value;
                    OnPropertyChanged("IDProgDiariaTranspVeiculos");
                }
            }
        }
        private Nullable<int> _iDProgDiariaTranspVeiculos;
    
        [DataMember]
        public string Metodo
        {
            get { return _metodo; }
            set
            {
                if (_metodo != value)
                {
                    _metodo = value;
                    OnPropertyChanged("Metodo");
                }
            }
        }
        private string _metodo;
    
        [DataMember]
        public Nullable<int> IdOperacaoTransporte
        {
            get { return _idOperacaoTransporte; }
            set
            {
                if (_idOperacaoTransporte != value)
                {
                    _idOperacaoTransporte = value;
                    OnPropertyChanged("IdOperacaoTransporte");
                }
            }
        }
        private Nullable<int> _idOperacaoTransporte;
    
        [DataMember]
        public Nullable<System.DateTime> DataHoraRegistro
        {
            get { return _dataHoraRegistro; }
            set
            {
                if (_dataHoraRegistro != value)
                {
                    _dataHoraRegistro = value;
                    OnPropertyChanged("DataHoraRegistro");
                }
            }
        }
        private Nullable<System.DateTime> _dataHoraRegistro;
    
        [DataMember]
        public string NumeroCIOT
        {
            get { return _numeroCIOT; }
            set
            {
                if (_numeroCIOT != value)
                {
                    _numeroCIOT = value;
                    OnPropertyChanged("NumeroCIOT");
                }
            }
        }
        private string _numeroCIOT;
    
        [DataMember]
        public string ProtocoloCIOT
        {
            get { return _protocoloCIOT; }
            set
            {
                if (_protocoloCIOT != value)
                {
                    _protocoloCIOT = value;
                    OnPropertyChanged("ProtocoloCIOT");
                }
            }
        }
        private string _protocoloCIOT;
    
        [DataMember]
        public Nullable<int> DispensadoPelaANTT
        {
            get { return _dispensadoPelaANTT; }
            set
            {
                if (_dispensadoPelaANTT != value)
                {
                    _dispensadoPelaANTT = value;
                    OnPropertyChanged("DispensadoPelaANTT");
                }
            }
        }
        private Nullable<int> _dispensadoPelaANTT;
    
        [DataMember]
        public string ObservacoesANTT
        {
            get { return _observacoesANTT; }
            set
            {
                if (_observacoesANTT != value)
                {
                    _observacoesANTT = value;
                    OnPropertyChanged("ObservacoesANTT");
                }
            }
        }
        private string _observacoesANTT;
    
        [DataMember]
        public Nullable<int> IdCompraValePedagio
        {
            get { return _idCompraValePedagio; }
            set
            {
                if (_idCompraValePedagio != value)
                {
                    _idCompraValePedagio = value;
                    OnPropertyChanged("IdCompraValePedagio");
                }
            }
        }
        private Nullable<int> _idCompraValePedagio;
    
        [DataMember]
        public Nullable<int> ModoCompraValePedagio
        {
            get { return _modoCompraValePedagio; }
            set
            {
                if (_modoCompraValePedagio != value)
                {
                    _modoCompraValePedagio = value;
                    OnPropertyChanged("ModoCompraValePedagio");
                }
            }
        }
        private Nullable<int> _modoCompraValePedagio;
    
        [DataMember]
        public Nullable<int> IdParcelasOperacaoTransporte
        {
            get { return _idParcelasOperacaoTransporte; }
            set
            {
                if (_idParcelasOperacaoTransporte != value)
                {
                    _idParcelasOperacaoTransporte = value;
                    OnPropertyChanged("IdParcelasOperacaoTransporte");
                }
            }
        }
        private Nullable<int> _idParcelasOperacaoTransporte;
    
        [DataMember]
        public string Observacoes
        {
            get { return _observacoes; }
            set
            {
                if (_observacoes != value)
                {
                    _observacoes = value;
                    OnPropertyChanged("Observacoes");
                }
            }
        }
        private string _observacoes;

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
