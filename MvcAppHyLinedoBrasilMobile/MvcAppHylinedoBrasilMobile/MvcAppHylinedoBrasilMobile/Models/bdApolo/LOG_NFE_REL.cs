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

namespace MvcAppHylinedoBrasilMobile.Models.bdApolo
{
    [DataContract(IsReference = true)]
    public partial class LOG_NFE_REL: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int LogNFERelSeq
        {
            get { return _logNFERelSeq; }
            set
            {
                if (_logNFERelSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'LogNFERelSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _logNFERelSeq = value;
                    OnPropertyChanged("LogNFERelSeq");
                }
            }
        }
        private int _logNFERelSeq;
    
        [DataMember]
        public string LogNFERelEmpCod
        {
            get { return _logNFERelEmpCod; }
            set
            {
                if (_logNFERelEmpCod != value)
                {
                    _logNFERelEmpCod = value;
                    OnPropertyChanged("LogNFERelEmpCod");
                }
            }
        }
        private string _logNFERelEmpCod;
    
        [DataMember]
        public string LogNFERelModelo
        {
            get { return _logNFERelModelo; }
            set
            {
                if (_logNFERelModelo != value)
                {
                    _logNFERelModelo = value;
                    OnPropertyChanged("LogNFERelModelo");
                }
            }
        }
        private string _logNFERelModelo;
    
        [DataMember]
        public string LogNFERelSerie
        {
            get { return _logNFERelSerie; }
            set
            {
                if (_logNFERelSerie != value)
                {
                    _logNFERelSerie = value;
                    OnPropertyChanged("LogNFERelSerie");
                }
            }
        }
        private string _logNFERelSerie;
    
        [DataMember]
        public string LogNFERelNum
        {
            get { return _logNFERelNum; }
            set
            {
                if (_logNFERelNum != value)
                {
                    _logNFERelNum = value;
                    OnPropertyChanged("LogNFERelNum");
                }
            }
        }
        private string _logNFERelNum;
    
        [DataMember]
        public Nullable<System.DateTime> LogNFERelDtHoraImp
        {
            get { return _logNFERelDtHoraImp; }
            set
            {
                if (_logNFERelDtHoraImp != value)
                {
                    _logNFERelDtHoraImp = value;
                    OnPropertyChanged("LogNFERelDtHoraImp");
                }
            }
        }
        private Nullable<System.DateTime> _logNFERelDtHoraImp;
    
        [DataMember]
        public Nullable<int> LogNFERelQtdCopia
        {
            get { return _logNFERelQtdCopia; }
            set
            {
                if (_logNFERelQtdCopia != value)
                {
                    _logNFERelQtdCopia = value;
                    OnPropertyChanged("LogNFERelQtdCopia");
                }
            }
        }
        private Nullable<int> _logNFERelQtdCopia;
    
        [DataMember]
        public string LogNFERelString
        {
            get { return _logNFERelString; }
            set
            {
                if (_logNFERelString != value)
                {
                    _logNFERelString = value;
                    OnPropertyChanged("LogNFERelString");
                }
            }
        }
        private string _logNFERelString;
    
        [DataMember]
        public string LogNFERelUsuCod
        {
            get { return _logNFERelUsuCod; }
            set
            {
                if (_logNFERelUsuCod != value)
                {
                    _logNFERelUsuCod = value;
                    OnPropertyChanged("LogNFERelUsuCod");
                }
            }
        }
        private string _logNFERelUsuCod;

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
