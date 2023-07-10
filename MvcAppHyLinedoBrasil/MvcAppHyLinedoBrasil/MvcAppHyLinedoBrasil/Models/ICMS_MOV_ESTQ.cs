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

namespace MvcAppHyLinedoBrasil.Models
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MOV_ESTQ))]
    public partial class ICMS_MOV_ESTQ: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public string EmpCod
        {
            get { return _empCod; }
            set
            {
                if (_empCod != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'EmpCod' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (MOV_ESTQ != null && MOV_ESTQ.EmpCod != value)
                        {
                            MOV_ESTQ = null;
                        }
                    }
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'MovEstqChv' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (MOV_ESTQ != null && MOV_ESTQ.MovEstqChv != value)
                        {
                            MOV_ESTQ = null;
                        }
                    }
                    _movEstqChv = value;
                    OnPropertyChanged("MovEstqChv");
                }
            }
        }
        private int _movEstqChv;
    
        [DataMember]
        public decimal IcmsMovEstqPerc
        {
            get { return _icmsMovEstqPerc; }
            set
            {
                if (_icmsMovEstqPerc != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'IcmsMovEstqPerc' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _icmsMovEstqPerc = value;
                    OnPropertyChanged("IcmsMovEstqPerc");
                }
            }
        }
        private decimal _icmsMovEstqPerc;
    
        [DataMember]
        public Nullable<decimal> IcmsMovEstqBaseCalc
        {
            get { return _icmsMovEstqBaseCalc; }
            set
            {
                if (_icmsMovEstqBaseCalc != value)
                {
                    _icmsMovEstqBaseCalc = value;
                    OnPropertyChanged("IcmsMovEstqBaseCalc");
                }
            }
        }
        private Nullable<decimal> _icmsMovEstqBaseCalc;
    
        [DataMember]
        public Nullable<decimal> IcmsMovEstqValor
        {
            get { return _icmsMovEstqValor; }
            set
            {
                if (_icmsMovEstqValor != value)
                {
                    _icmsMovEstqValor = value;
                    OnPropertyChanged("IcmsMovEstqValor");
                }
            }
        }
        private Nullable<decimal> _icmsMovEstqValor;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public MOV_ESTQ MOV_ESTQ
        {
            get { return _mOV_ESTQ; }
            set
            {
                if (!ReferenceEquals(_mOV_ESTQ, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EmpCod != value.EmpCod || MovEstqChv != value.MovEstqChv)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _mOV_ESTQ;
                    _mOV_ESTQ = value;
                    FixupMOV_ESTQ(previousValue);
                    OnNavigationPropertyChanged("MOV_ESTQ");
                }
            }
        }
        private MOV_ESTQ _mOV_ESTQ;

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
    
        // This entity type is the dependent end in at least one association that performs cascade deletes.
        // This event handler will process notifications that occur when the principal end is deleted.
        internal void HandleCascadeDelete(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
            }
        }
    
        protected virtual void ClearNavigationProperties()
        {
            MOV_ESTQ = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupMOV_ESTQ(MOV_ESTQ previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ICMS_MOV_ESTQ.Contains(this))
            {
                previousValue.ICMS_MOV_ESTQ.Remove(this);
            }
    
            if (MOV_ESTQ != null)
            {
                if (!MOV_ESTQ.ICMS_MOV_ESTQ.Contains(this))
                {
                    MOV_ESTQ.ICMS_MOV_ESTQ.Add(this);
                }
    
                EmpCod = MOV_ESTQ.EmpCod;
                MovEstqChv = MOV_ESTQ.MovEstqChv;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("MOV_ESTQ")
                    && (ChangeTracker.OriginalValues["MOV_ESTQ"] == MOV_ESTQ))
                {
                    ChangeTracker.OriginalValues.Remove("MOV_ESTQ");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("MOV_ESTQ", previousValue);
                }
                if (MOV_ESTQ != null && !MOV_ESTQ.ChangeTracker.ChangeTrackingEnabled)
                {
                    MOV_ESTQ.StartTracking();
                }
            }
        }

        #endregion

    }
}
