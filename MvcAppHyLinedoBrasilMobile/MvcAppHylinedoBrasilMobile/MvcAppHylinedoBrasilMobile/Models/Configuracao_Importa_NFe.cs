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
    [KnownType(typeof(Recebimento_Documento))]
    public partial class Configuracao_Importa_NFe: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string Descricao
        {
            get { return _descricao; }
            set
            {
                if (_descricao != value)
                {
                    _descricao = value;
                    OnPropertyChanged("Descricao");
                }
            }
        }
        private string _descricao;
    
        [DataMember]
        public string TipoLancCod
        {
            get { return _tipoLancCod; }
            set
            {
                if (_tipoLancCod != value)
                {
                    _tipoLancCod = value;
                    OnPropertyChanged("TipoLancCod");
                }
            }
        }
        private string _tipoLancCod;
    
        [DataMember]
        public string ClasFiscCod
        {
            get { return _clasFiscCod; }
            set
            {
                if (_clasFiscCod != value)
                {
                    _clasFiscCod = value;
                    OnPropertyChanged("ClasFiscCod");
                }
            }
        }
        private string _clasFiscCod;
    
        [DataMember]
        public string DataMovimento
        {
            get { return _dataMovimento; }
            set
            {
                if (_dataMovimento != value)
                {
                    _dataMovimento = value;
                    OnPropertyChanged("DataMovimento");
                }
            }
        }
        private string _dataMovimento;
    
        [DataMember]
        public string NaturezaOperacao
        {
            get { return _naturezaOperacao; }
            set
            {
                if (_naturezaOperacao != value)
                {
                    _naturezaOperacao = value;
                    OnPropertyChanged("NaturezaOperacao");
                }
            }
        }
        private string _naturezaOperacao;
    
        [DataMember]
        public string LocArmazCod
        {
            get { return _locArmazCod; }
            set
            {
                if (_locArmazCod != value)
                {
                    _locArmazCod = value;
                    OnPropertyChanged("LocArmazCod");
                }
            }
        }
        private string _locArmazCod;
    
        [DataMember]
        public string ContaDebito
        {
            get { return _contaDebito; }
            set
            {
                if (_contaDebito != value)
                {
                    _contaDebito = value;
                    OnPropertyChanged("ContaDebito");
                }
            }
        }
        private string _contaDebito;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<Recebimento_Documento> Recebimento_Documento
        {
            get
            {
                if (_recebimento_Documento == null)
                {
                    _recebimento_Documento = new TrackableCollection<Recebimento_Documento>();
                    _recebimento_Documento.CollectionChanged += FixupRecebimento_Documento;
                }
                return _recebimento_Documento;
            }
            set
            {
                if (!ReferenceEquals(_recebimento_Documento, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_recebimento_Documento != null)
                    {
                        _recebimento_Documento.CollectionChanged -= FixupRecebimento_Documento;
                    }
                    _recebimento_Documento = value;
                    if (_recebimento_Documento != null)
                    {
                        _recebimento_Documento.CollectionChanged += FixupRecebimento_Documento;
                    }
                    OnNavigationPropertyChanged("Recebimento_Documento");
                }
            }
        }
        private TrackableCollection<Recebimento_Documento> _recebimento_Documento;

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
            Recebimento_Documento.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupRecebimento_Documento(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Recebimento_Documento item in e.NewItems)
                {
                    item.Configuracao_Importa_NFe = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Recebimento_Documento", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Recebimento_Documento item in e.OldItems)
                {
                    if (ReferenceEquals(item.Configuracao_Importa_NFe, this))
                    {
                        item.Configuracao_Importa_NFe = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Recebimento_Documento", item);
                    }
                }
            }
        }

        #endregion

    }
}
