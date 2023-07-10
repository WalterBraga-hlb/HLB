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
    [KnownType(typeof(Envio_Confirmacao))]
    public partial class Envio_Confirmacao_PV: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public Nullable<int> IDEnvioConfirmacao
        {
            get { return _iDEnvioConfirmacao; }
            set
            {
                if (_iDEnvioConfirmacao != value)
                {
                    ChangeTracker.RecordOriginalValue("IDEnvioConfirmacao", _iDEnvioConfirmacao);
                    if (!IsDeserializing)
                    {
                        if (Envio_Confirmacao != null && Envio_Confirmacao.ID != value)
                        {
                            Envio_Confirmacao = null;
                        }
                    }
                    _iDEnvioConfirmacao = value;
                    OnPropertyChanged("IDEnvioConfirmacao");
                }
            }
        }
        private Nullable<int> _iDEnvioConfirmacao;
    
        [DataMember]
        public Nullable<int> IDPedidoVenda
        {
            get { return _iDPedidoVenda; }
            set
            {
                if (_iDPedidoVenda != value)
                {
                    _iDPedidoVenda = value;
                    OnPropertyChanged("IDPedidoVenda");
                }
            }
        }
        private Nullable<int> _iDPedidoVenda;
    
        [DataMember]
        public string TipoPedido
        {
            get { return _tipoPedido; }
            set
            {
                if (_tipoPedido != value)
                {
                    _tipoPedido = value;
                    OnPropertyChanged("TipoPedido");
                }
            }
        }
        private string _tipoPedido;
    
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
    
        [DataMember]
        public string Vendedor
        {
            get { return _vendedor; }
            set
            {
                if (_vendedor != value)
                {
                    _vendedor = value;
                    OnPropertyChanged("Vendedor");
                }
            }
        }
        private string _vendedor;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Envio_Confirmacao Envio_Confirmacao
        {
            get { return _envio_Confirmacao; }
            set
            {
                if (!ReferenceEquals(_envio_Confirmacao, value))
                {
                    var previousValue = _envio_Confirmacao;
                    _envio_Confirmacao = value;
                    FixupEnvio_Confirmacao(previousValue);
                    OnNavigationPropertyChanged("Envio_Confirmacao");
                }
            }
        }
        private Envio_Confirmacao _envio_Confirmacao;

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
            Envio_Confirmacao = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupEnvio_Confirmacao(Envio_Confirmacao previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Envio_Confirmacao_PV.Contains(this))
            {
                previousValue.Envio_Confirmacao_PV.Remove(this);
            }
    
            if (Envio_Confirmacao != null)
            {
                if (!Envio_Confirmacao.Envio_Confirmacao_PV.Contains(this))
                {
                    Envio_Confirmacao.Envio_Confirmacao_PV.Add(this);
                }
    
                IDEnvioConfirmacao = Envio_Confirmacao.ID;
            }
            else if (!skipKeys)
            {
                IDEnvioConfirmacao = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Envio_Confirmacao")
                    && (ChangeTracker.OriginalValues["Envio_Confirmacao"] == Envio_Confirmacao))
                {
                    ChangeTracker.OriginalValues.Remove("Envio_Confirmacao");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Envio_Confirmacao", previousValue);
                }
                if (Envio_Confirmacao != null && !Envio_Confirmacao.ChangeTracker.ChangeTrackingEnabled)
                {
                    Envio_Confirmacao.StartTracking();
                }
            }
        }

        #endregion

    }
}
