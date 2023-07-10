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
    public partial class Tabela_Precos: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string Produto
        {
            get { return _produto; }
            set
            {
                if (_produto != value)
                {
                    _produto = value;
                    OnPropertyChanged("Produto");
                }
            }
        }
        private string _produto;
    
        [DataMember]
        public string Tipo
        {
            get { return _tipo; }
            set
            {
                if (_tipo != value)
                {
                    _tipo = value;
                    OnPropertyChanged("Tipo");
                }
            }
        }
        private string _tipo;
    
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
        public string Regiao
        {
            get { return _regiao; }
            set
            {
                if (_regiao != value)
                {
                    _regiao = value;
                    OnPropertyChanged("Regiao");
                }
            }
        }
        private string _regiao;
    
        [DataMember]
        public Nullable<decimal> ValorMenor5000Aves
        {
            get { return _valorMenor5000Aves; }
            set
            {
                if (_valorMenor5000Aves != value)
                {
                    _valorMenor5000Aves = value;
                    OnPropertyChanged("ValorMenor5000Aves");
                }
            }
        }
        private Nullable<decimal> _valorMenor5000Aves;
    
        [DataMember]
        public Nullable<decimal> ValorNormal
        {
            get { return _valorNormal; }
            set
            {
                if (_valorNormal != value)
                {
                    _valorNormal = value;
                    OnPropertyChanged("ValorNormal");
                }
            }
        }
        private Nullable<decimal> _valorNormal;

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
