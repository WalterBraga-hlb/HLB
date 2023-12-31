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
    [KnownType(typeof(Envio_Confirmacao_PV))]
    [KnownType(typeof(Envio_Confirmacao_Email))]
    public partial class Envio_Confirmacao: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string TipoAgrupamento
        {
            get { return _tipoAgrupamento; }
            set
            {
                if (_tipoAgrupamento != value)
                {
                    _tipoAgrupamento = value;
                    OnPropertyChanged("TipoAgrupamento");
                }
            }
        }
        private string _tipoAgrupamento;
    
        [DataMember]
        public Nullable<int> EnviarEmpresa
        {
            get { return _enviarEmpresa; }
            set
            {
                if (_enviarEmpresa != value)
                {
                    _enviarEmpresa = value;
                    OnPropertyChanged("EnviarEmpresa");
                }
            }
        }
        private Nullable<int> _enviarEmpresa;
    
        [DataMember]
        public Nullable<int> EnviarVendedor
        {
            get { return _enviarVendedor; }
            set
            {
                if (_enviarVendedor != value)
                {
                    _enviarVendedor = value;
                    OnPropertyChanged("EnviarVendedor");
                }
            }
        }
        private Nullable<int> _enviarVendedor;
    
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
        public Nullable<System.DateTime> DataHoraInicio
        {
            get { return _dataHoraInicio; }
            set
            {
                if (_dataHoraInicio != value)
                {
                    _dataHoraInicio = value;
                    OnPropertyChanged("DataHoraInicio");
                }
            }
        }
        private Nullable<System.DateTime> _dataHoraInicio;
    
        [DataMember]
        public Nullable<System.DateTime> DataHoraFim
        {
            get { return _dataHoraFim; }
            set
            {
                if (_dataHoraFim != value)
                {
                    _dataHoraFim = value;
                    OnPropertyChanged("DataHoraFim");
                }
            }
        }
        private Nullable<System.DateTime> _dataHoraFim;
    
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
        public string Mensagem
        {
            get { return _mensagem; }
            set
            {
                if (_mensagem != value)
                {
                    _mensagem = value;
                    OnPropertyChanged("Mensagem");
                }
            }
        }
        private string _mensagem;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<Envio_Confirmacao_PV> Envio_Confirmacao_PV
        {
            get
            {
                if (_envio_Confirmacao_PV == null)
                {
                    _envio_Confirmacao_PV = new TrackableCollection<Envio_Confirmacao_PV>();
                    _envio_Confirmacao_PV.CollectionChanged += FixupEnvio_Confirmacao_PV;
                }
                return _envio_Confirmacao_PV;
            }
            set
            {
                if (!ReferenceEquals(_envio_Confirmacao_PV, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_envio_Confirmacao_PV != null)
                    {
                        _envio_Confirmacao_PV.CollectionChanged -= FixupEnvio_Confirmacao_PV;
                    }
                    _envio_Confirmacao_PV = value;
                    if (_envio_Confirmacao_PV != null)
                    {
                        _envio_Confirmacao_PV.CollectionChanged += FixupEnvio_Confirmacao_PV;
                    }
                    OnNavigationPropertyChanged("Envio_Confirmacao_PV");
                }
            }
        }
        private TrackableCollection<Envio_Confirmacao_PV> _envio_Confirmacao_PV;
    
        [DataMember]
        public TrackableCollection<Envio_Confirmacao_Email> Envio_Confirmacao_Email
        {
            get
            {
                if (_envio_Confirmacao_Email == null)
                {
                    _envio_Confirmacao_Email = new TrackableCollection<Envio_Confirmacao_Email>();
                    _envio_Confirmacao_Email.CollectionChanged += FixupEnvio_Confirmacao_Email;
                }
                return _envio_Confirmacao_Email;
            }
            set
            {
                if (!ReferenceEquals(_envio_Confirmacao_Email, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_envio_Confirmacao_Email != null)
                    {
                        _envio_Confirmacao_Email.CollectionChanged -= FixupEnvio_Confirmacao_Email;
                    }
                    _envio_Confirmacao_Email = value;
                    if (_envio_Confirmacao_Email != null)
                    {
                        _envio_Confirmacao_Email.CollectionChanged += FixupEnvio_Confirmacao_Email;
                    }
                    OnNavigationPropertyChanged("Envio_Confirmacao_Email");
                }
            }
        }
        private TrackableCollection<Envio_Confirmacao_Email> _envio_Confirmacao_Email;

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
            Envio_Confirmacao_PV.Clear();
            Envio_Confirmacao_Email.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupEnvio_Confirmacao_PV(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Envio_Confirmacao_PV item in e.NewItems)
                {
                    item.Envio_Confirmacao = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Envio_Confirmacao_PV", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Envio_Confirmacao_PV item in e.OldItems)
                {
                    if (ReferenceEquals(item.Envio_Confirmacao, this))
                    {
                        item.Envio_Confirmacao = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Envio_Confirmacao_PV", item);
                    }
                }
            }
        }
    
        private void FixupEnvio_Confirmacao_Email(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Envio_Confirmacao_Email item in e.NewItems)
                {
                    item.Envio_Confirmacao = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Envio_Confirmacao_Email", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Envio_Confirmacao_Email item in e.OldItems)
                {
                    if (ReferenceEquals(item.Envio_Confirmacao, this))
                    {
                        item.Envio_Confirmacao = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Envio_Confirmacao_Email", item);
                    }
                }
            }
        }

        #endregion

    }
}
