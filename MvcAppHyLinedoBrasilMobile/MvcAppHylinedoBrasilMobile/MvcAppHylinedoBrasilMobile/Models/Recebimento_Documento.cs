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
    [KnownType(typeof(Configuracao_Importa_NFe))]
    public partial class Recebimento_Documento: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string ChaveEletronica
        {
            get { return _chaveEletronica; }
            set
            {
                if (_chaveEletronica != value)
                {
                    _chaveEletronica = value;
                    OnPropertyChanged("ChaveEletronica");
                }
            }
        }
        private string _chaveEletronica;
    
        [DataMember]
        public string NumeroPedidoCompra
        {
            get { return _numeroPedidoCompra; }
            set
            {
                if (_numeroPedidoCompra != value)
                {
                    _numeroPedidoCompra = value;
                    OnPropertyChanged("NumeroPedidoCompra");
                }
            }
        }
        private string _numeroPedidoCompra;
    
        [DataMember]
        public Nullable<System.DateTime> DataEntrada
        {
            get { return _dataEntrada; }
            set
            {
                if (_dataEntrada != value)
                {
                    _dataEntrada = value;
                    OnPropertyChanged("DataEntrada");
                }
            }
        }
        private Nullable<System.DateTime> _dataEntrada;
    
        [DataMember]
        public Nullable<int> IDConfigImportaNFe
        {
            get { return _iDConfigImportaNFe; }
            set
            {
                if (_iDConfigImportaNFe != value)
                {
                    ChangeTracker.RecordOriginalValue("IDConfigImportaNFe", _iDConfigImportaNFe);
                    if (!IsDeserializing)
                    {
                        if (Configuracao_Importa_NFe != null && Configuracao_Importa_NFe.ID != value)
                        {
                            Configuracao_Importa_NFe = null;
                        }
                    }
                    _iDConfigImportaNFe = value;
                    OnPropertyChanged("IDConfigImportaNFe");
                }
            }
        }
        private Nullable<int> _iDConfigImportaNFe;
    
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
        public System.DateTime DataHoraCadastro
        {
            get { return _dataHoraCadastro; }
            set
            {
                if (_dataHoraCadastro != value)
                {
                    _dataHoraCadastro = value;
                    OnPropertyChanged("DataHoraCadastro");
                }
            }
        }
        private System.DateTime _dataHoraCadastro;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Configuracao_Importa_NFe Configuracao_Importa_NFe
        {
            get { return _configuracao_Importa_NFe; }
            set
            {
                if (!ReferenceEquals(_configuracao_Importa_NFe, value))
                {
                    var previousValue = _configuracao_Importa_NFe;
                    _configuracao_Importa_NFe = value;
                    FixupConfiguracao_Importa_NFe(previousValue);
                    OnNavigationPropertyChanged("Configuracao_Importa_NFe");
                }
            }
        }
        private Configuracao_Importa_NFe _configuracao_Importa_NFe;

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
            Configuracao_Importa_NFe = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupConfiguracao_Importa_NFe(Configuracao_Importa_NFe previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Recebimento_Documento.Contains(this))
            {
                previousValue.Recebimento_Documento.Remove(this);
            }
    
            if (Configuracao_Importa_NFe != null)
            {
                if (!Configuracao_Importa_NFe.Recebimento_Documento.Contains(this))
                {
                    Configuracao_Importa_NFe.Recebimento_Documento.Add(this);
                }
    
                IDConfigImportaNFe = Configuracao_Importa_NFe.ID;
            }
            else if (!skipKeys)
            {
                IDConfigImportaNFe = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Configuracao_Importa_NFe")
                    && (ChangeTracker.OriginalValues["Configuracao_Importa_NFe"] == Configuracao_Importa_NFe))
                {
                    ChangeTracker.OriginalValues.Remove("Configuracao_Importa_NFe");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Configuracao_Importa_NFe", previousValue);
                }
                if (Configuracao_Importa_NFe != null && !Configuracao_Importa_NFe.ChangeTracker.ChangeTrackingEnabled)
                {
                    Configuracao_Importa_NFe.StartTracking();
                }
            }
        }

        #endregion

    }
}
