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
    [KnownType(typeof(Auditoria_Requisito))]
    [KnownType(typeof(Auditoria_Visita))]
    public partial class Auditoria_Visita_Requisito: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int IDVisita
        {
            get { return _iDVisita; }
            set
            {
                if (_iDVisita != value)
                {
                    ChangeTracker.RecordOriginalValue("IDVisita", _iDVisita);
                    if (!IsDeserializing)
                    {
                        if (Auditoria_Visita != null && Auditoria_Visita.ID != value)
                        {
                            Auditoria_Visita = null;
                        }
                    }
                    _iDVisita = value;
                    OnPropertyChanged("IDVisita");
                }
            }
        }
        private int _iDVisita;
    
        [DataMember]
        public int IDRequisito
        {
            get { return _iDRequisito; }
            set
            {
                if (_iDRequisito != value)
                {
                    ChangeTracker.RecordOriginalValue("IDRequisito", _iDRequisito);
                    if (!IsDeserializing)
                    {
                        if (Auditoria_Requisito != null && Auditoria_Requisito.ID != value)
                        {
                            Auditoria_Requisito = null;
                        }
                    }
                    _iDRequisito = value;
                    OnPropertyChanged("IDRequisito");
                }
            }
        }
        private int _iDRequisito;
    
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
        public string SolucaoNaoConforme
        {
            get { return _solucaoNaoConforme; }
            set
            {
                if (_solucaoNaoConforme != value)
                {
                    _solucaoNaoConforme = value;
                    OnPropertyChanged("SolucaoNaoConforme");
                }
            }
        }
        private string _solucaoNaoConforme;
    
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
        public string UsuarioResolucao
        {
            get { return _usuarioResolucao; }
            set
            {
                if (_usuarioResolucao != value)
                {
                    _usuarioResolucao = value;
                    OnPropertyChanged("UsuarioResolucao");
                }
            }
        }
        private string _usuarioResolucao;
    
        [DataMember]
        public Nullable<System.DateTime> DataHoraResolucao
        {
            get { return _dataHoraResolucao; }
            set
            {
                if (_dataHoraResolucao != value)
                {
                    _dataHoraResolucao = value;
                    OnPropertyChanged("DataHoraResolucao");
                }
            }
        }
        private Nullable<System.DateTime> _dataHoraResolucao;
    
        [DataMember]
        public string ObservacaoResolucao
        {
            get { return _observacaoResolucao; }
            set
            {
                if (_observacaoResolucao != value)
                {
                    _observacaoResolucao = value;
                    OnPropertyChanged("ObservacaoResolucao");
                }
            }
        }
        private string _observacaoResolucao;
    
        [DataMember]
        public string StatusResolucao
        {
            get { return _statusResolucao; }
            set
            {
                if (_statusResolucao != value)
                {
                    _statusResolucao = value;
                    OnPropertyChanged("StatusResolucao");
                }
            }
        }
        private string _statusResolucao;
    
        [DataMember]
        public string UsuarioAprovacao
        {
            get { return _usuarioAprovacao; }
            set
            {
                if (_usuarioAprovacao != value)
                {
                    _usuarioAprovacao = value;
                    OnPropertyChanged("UsuarioAprovacao");
                }
            }
        }
        private string _usuarioAprovacao;
    
        [DataMember]
        public Nullable<System.DateTime> DataHoraAprovacao
        {
            get { return _dataHoraAprovacao; }
            set
            {
                if (_dataHoraAprovacao != value)
                {
                    _dataHoraAprovacao = value;
                    OnPropertyChanged("DataHoraAprovacao");
                }
            }
        }
        private Nullable<System.DateTime> _dataHoraAprovacao;
    
        [DataMember]
        public string UsuarioEnvio
        {
            get { return _usuarioEnvio; }
            set
            {
                if (_usuarioEnvio != value)
                {
                    _usuarioEnvio = value;
                    OnPropertyChanged("UsuarioEnvio");
                }
            }
        }
        private string _usuarioEnvio;
    
        [DataMember]
        public Nullable<System.DateTime> DataHoraEnvio
        {
            get { return _dataHoraEnvio; }
            set
            {
                if (_dataHoraEnvio != value)
                {
                    _dataHoraEnvio = value;
                    OnPropertyChanged("DataHoraEnvio");
                }
            }
        }
        private Nullable<System.DateTime> _dataHoraEnvio;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Auditoria_Requisito Auditoria_Requisito
        {
            get { return _auditoria_Requisito; }
            set
            {
                if (!ReferenceEquals(_auditoria_Requisito, value))
                {
                    var previousValue = _auditoria_Requisito;
                    _auditoria_Requisito = value;
                    FixupAuditoria_Requisito(previousValue);
                    OnNavigationPropertyChanged("Auditoria_Requisito");
                }
            }
        }
        private Auditoria_Requisito _auditoria_Requisito;
    
        [DataMember]
        public Auditoria_Visita Auditoria_Visita
        {
            get { return _auditoria_Visita; }
            set
            {
                if (!ReferenceEquals(_auditoria_Visita, value))
                {
                    var previousValue = _auditoria_Visita;
                    _auditoria_Visita = value;
                    FixupAuditoria_Visita(previousValue);
                    OnNavigationPropertyChanged("Auditoria_Visita");
                }
            }
        }
        private Auditoria_Visita _auditoria_Visita;

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
            Auditoria_Requisito = null;
            Auditoria_Visita = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupAuditoria_Requisito(Auditoria_Requisito previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Auditoria_Visita_Requisito.Contains(this))
            {
                previousValue.Auditoria_Visita_Requisito.Remove(this);
            }
    
            if (Auditoria_Requisito != null)
            {
                if (!Auditoria_Requisito.Auditoria_Visita_Requisito.Contains(this))
                {
                    Auditoria_Requisito.Auditoria_Visita_Requisito.Add(this);
                }
    
                IDRequisito = Auditoria_Requisito.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Auditoria_Requisito")
                    && (ChangeTracker.OriginalValues["Auditoria_Requisito"] == Auditoria_Requisito))
                {
                    ChangeTracker.OriginalValues.Remove("Auditoria_Requisito");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Auditoria_Requisito", previousValue);
                }
                if (Auditoria_Requisito != null && !Auditoria_Requisito.ChangeTracker.ChangeTrackingEnabled)
                {
                    Auditoria_Requisito.StartTracking();
                }
            }
        }
    
        private void FixupAuditoria_Visita(Auditoria_Visita previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Auditoria_Visita_Requisito.Contains(this))
            {
                previousValue.Auditoria_Visita_Requisito.Remove(this);
            }
    
            if (Auditoria_Visita != null)
            {
                if (!Auditoria_Visita.Auditoria_Visita_Requisito.Contains(this))
                {
                    Auditoria_Visita.Auditoria_Visita_Requisito.Add(this);
                }
    
                IDVisita = Auditoria_Visita.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Auditoria_Visita")
                    && (ChangeTracker.OriginalValues["Auditoria_Visita"] == Auditoria_Visita))
                {
                    ChangeTracker.OriginalValues.Remove("Auditoria_Visita");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Auditoria_Visita", previousValue);
                }
                if (Auditoria_Visita != null && !Auditoria_Visita.ChangeTracker.ChangeTrackingEnabled)
                {
                    Auditoria_Visita.StartTracking();
                }
            }
        }

        #endregion

    }
}