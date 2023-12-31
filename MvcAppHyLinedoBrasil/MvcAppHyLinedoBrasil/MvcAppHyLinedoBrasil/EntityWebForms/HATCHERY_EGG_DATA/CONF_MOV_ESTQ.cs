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
    [KnownType(typeof(CONF_MOV_ESTQ_VALORES))]
    public partial class CONF_MOV_ESTQ: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string EmpCod
        {
            get { return _empCod; }
            set
            {
                if (_empCod != value)
                {
                    _empCod = value;
                    OnPropertyChanged("EmpCod");
                }
            }
        }
        private string _empCod;
    
        [DataMember]
        public int MovEstqChv
        {
            get { return _movEstqChv; }
            set
            {
                if (_movEstqChv != value)
                {
                    _movEstqChv = value;
                    OnPropertyChanged("MovEstqChv");
                }
            }
        }
        private int _movEstqChv;
    
        [DataMember]
        public System.DateTime DataHoraConferencia
        {
            get { return _dataHoraConferencia; }
            set
            {
                if (_dataHoraConferencia != value)
                {
                    _dataHoraConferencia = value;
                    OnPropertyChanged("DataHoraConferencia");
                }
            }
        }
        private System.DateTime _dataHoraConferencia;
    
        [DataMember]
        public string UsuarioConferencia
        {
            get { return _usuarioConferencia; }
            set
            {
                if (_usuarioConferencia != value)
                {
                    _usuarioConferencia = value;
                    OnPropertyChanged("UsuarioConferencia");
                }
            }
        }
        private string _usuarioConferencia;
    
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

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<CONF_MOV_ESTQ_VALORES> CONF_MOV_ESTQ_VALORES
        {
            get
            {
                if (_cONF_MOV_ESTQ_VALORES == null)
                {
                    _cONF_MOV_ESTQ_VALORES = new TrackableCollection<CONF_MOV_ESTQ_VALORES>();
                    _cONF_MOV_ESTQ_VALORES.CollectionChanged += FixupCONF_MOV_ESTQ_VALORES;
                }
                return _cONF_MOV_ESTQ_VALORES;
            }
            set
            {
                if (!ReferenceEquals(_cONF_MOV_ESTQ_VALORES, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_cONF_MOV_ESTQ_VALORES != null)
                    {
                        _cONF_MOV_ESTQ_VALORES.CollectionChanged -= FixupCONF_MOV_ESTQ_VALORES;
                    }
                    _cONF_MOV_ESTQ_VALORES = value;
                    if (_cONF_MOV_ESTQ_VALORES != null)
                    {
                        _cONF_MOV_ESTQ_VALORES.CollectionChanged += FixupCONF_MOV_ESTQ_VALORES;
                    }
                    OnNavigationPropertyChanged("CONF_MOV_ESTQ_VALORES");
                }
            }
        }
        private TrackableCollection<CONF_MOV_ESTQ_VALORES> _cONF_MOV_ESTQ_VALORES;

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
            CONF_MOV_ESTQ_VALORES.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupCONF_MOV_ESTQ_VALORES(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (CONF_MOV_ESTQ_VALORES item in e.NewItems)
                {
                    item.CONF_MOV_ESTQ = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CONF_MOV_ESTQ_VALORES", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CONF_MOV_ESTQ_VALORES item in e.OldItems)
                {
                    if (ReferenceEquals(item.CONF_MOV_ESTQ, this))
                    {
                        item.CONF_MOV_ESTQ = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CONF_MOV_ESTQ_VALORES", item);
                    }
                }
            }
        }

        #endregion

    }
}
