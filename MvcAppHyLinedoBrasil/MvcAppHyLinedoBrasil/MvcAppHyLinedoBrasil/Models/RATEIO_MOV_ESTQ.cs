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
    [KnownType(typeof(MOV_ESTQ_CLASSE_REC_DESP))]
    [KnownType(typeof(CLASSE_REC_DESP))]
    public partial class RATEIO_MOV_ESTQ: IObjectWithChangeTracker, INotifyPropertyChanged
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
                        if (MOV_ESTQ_CLASSE_REC_DESP != null && MOV_ESTQ_CLASSE_REC_DESP.EmpCod != value)
                        {
                            MOV_ESTQ_CLASSE_REC_DESP = null;
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
                        if (MOV_ESTQ_CLASSE_REC_DESP != null && MOV_ESTQ_CLASSE_REC_DESP.MovEstqChv != value)
                        {
                            MOV_ESTQ_CLASSE_REC_DESP = null;
                        }
                    }
                    _movEstqChv = value;
                    OnPropertyChanged("MovEstqChv");
                }
            }
        }
        private int _movEstqChv;
    
        [DataMember]
        public string ClasseRecDespCodEstr
        {
            get { return _classeRecDespCodEstr; }
            set
            {
                if (_classeRecDespCodEstr != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ClasseRecDespCodEstr' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (MOV_ESTQ_CLASSE_REC_DESP != null && MOV_ESTQ_CLASSE_REC_DESP.ClasseRecDespCodEstr != value)
                        {
                            MOV_ESTQ_CLASSE_REC_DESP = null;
                        }
                        if (CLASSE_REC_DESP != null && CLASSE_REC_DESP.ClasseRecDespCodEstr != value)
                        {
                            CLASSE_REC_DESP = null;
                        }
                    }
                    _classeRecDespCodEstr = value;
                    OnPropertyChanged("ClasseRecDespCodEstr");
                }
            }
        }
        private string _classeRecDespCodEstr;
    
        [DataMember]
        public string CCtrlCodEstr
        {
            get { return _cCtrlCodEstr; }
            set
            {
                if (_cCtrlCodEstr != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'CCtrlCodEstr' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _cCtrlCodEstr = value;
                    OnPropertyChanged("CCtrlCodEstr");
                }
            }
        }
        private string _cCtrlCodEstr;
    
        [DataMember]
        public Nullable<decimal> RatMovEstqVal
        {
            get { return _ratMovEstqVal; }
            set
            {
                if (_ratMovEstqVal != value)
                {
                    _ratMovEstqVal = value;
                    OnPropertyChanged("RatMovEstqVal");
                }
            }
        }
        private Nullable<decimal> _ratMovEstqVal;
    
        [DataMember]
        public Nullable<decimal> RatMovEstqPerc
        {
            get { return _ratMovEstqPerc; }
            set
            {
                if (_ratMovEstqPerc != value)
                {
                    _ratMovEstqPerc = value;
                    OnPropertyChanged("RatMovEstqPerc");
                }
            }
        }
        private Nullable<decimal> _ratMovEstqPerc;

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
    
        [DataMember]
        public MOV_ESTQ_CLASSE_REC_DESP MOV_ESTQ_CLASSE_REC_DESP
        {
            get { return _mOV_ESTQ_CLASSE_REC_DESP; }
            set
            {
                if (!ReferenceEquals(_mOV_ESTQ_CLASSE_REC_DESP, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EmpCod != value.EmpCod || MovEstqChv != value.MovEstqChv || ClasseRecDespCodEstr != value.ClasseRecDespCodEstr)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _mOV_ESTQ_CLASSE_REC_DESP;
                    _mOV_ESTQ_CLASSE_REC_DESP = value;
                    FixupMOV_ESTQ_CLASSE_REC_DESP(previousValue);
                    OnNavigationPropertyChanged("MOV_ESTQ_CLASSE_REC_DESP");
                }
            }
        }
        private MOV_ESTQ_CLASSE_REC_DESP _mOV_ESTQ_CLASSE_REC_DESP;
    
        [DataMember]
        public CLASSE_REC_DESP CLASSE_REC_DESP
        {
            get { return _cLASSE_REC_DESP; }
            set
            {
                if (!ReferenceEquals(_cLASSE_REC_DESP, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (ClasseRecDespCodEstr != value.ClasseRecDespCodEstr)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _cLASSE_REC_DESP;
                    _cLASSE_REC_DESP = value;
                    FixupCLASSE_REC_DESP(previousValue);
                    OnNavigationPropertyChanged("CLASSE_REC_DESP");
                }
            }
        }
        private CLASSE_REC_DESP _cLASSE_REC_DESP;

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
            MOV_ESTQ_CLASSE_REC_DESP = null;
            CLASSE_REC_DESP = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupMOV_ESTQ(MOV_ESTQ previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.RATEIO_MOV_ESTQ.Contains(this))
            {
                previousValue.RATEIO_MOV_ESTQ.Remove(this);
            }
    
            if (MOV_ESTQ != null)
            {
                if (!MOV_ESTQ.RATEIO_MOV_ESTQ.Contains(this))
                {
                    MOV_ESTQ.RATEIO_MOV_ESTQ.Add(this);
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
    
        private void FixupMOV_ESTQ_CLASSE_REC_DESP(MOV_ESTQ_CLASSE_REC_DESP previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.RATEIO_MOV_ESTQ.Contains(this))
            {
                previousValue.RATEIO_MOV_ESTQ.Remove(this);
            }
    
            if (MOV_ESTQ_CLASSE_REC_DESP != null)
            {
                if (!MOV_ESTQ_CLASSE_REC_DESP.RATEIO_MOV_ESTQ.Contains(this))
                {
                    MOV_ESTQ_CLASSE_REC_DESP.RATEIO_MOV_ESTQ.Add(this);
                }
    
                EmpCod = MOV_ESTQ_CLASSE_REC_DESP.EmpCod;
                MovEstqChv = MOV_ESTQ_CLASSE_REC_DESP.MovEstqChv;
                ClasseRecDespCodEstr = MOV_ESTQ_CLASSE_REC_DESP.ClasseRecDespCodEstr;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("MOV_ESTQ_CLASSE_REC_DESP")
                    && (ChangeTracker.OriginalValues["MOV_ESTQ_CLASSE_REC_DESP"] == MOV_ESTQ_CLASSE_REC_DESP))
                {
                    ChangeTracker.OriginalValues.Remove("MOV_ESTQ_CLASSE_REC_DESP");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("MOV_ESTQ_CLASSE_REC_DESP", previousValue);
                }
                if (MOV_ESTQ_CLASSE_REC_DESP != null && !MOV_ESTQ_CLASSE_REC_DESP.ChangeTracker.ChangeTrackingEnabled)
                {
                    MOV_ESTQ_CLASSE_REC_DESP.StartTracking();
                }
            }
        }
    
        private void FixupCLASSE_REC_DESP(CLASSE_REC_DESP previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.RATEIO_MOV_ESTQ.Contains(this))
            {
                previousValue.RATEIO_MOV_ESTQ.Remove(this);
            }
    
            if (CLASSE_REC_DESP != null)
            {
                if (!CLASSE_REC_DESP.RATEIO_MOV_ESTQ.Contains(this))
                {
                    CLASSE_REC_DESP.RATEIO_MOV_ESTQ.Add(this);
                }
    
                ClasseRecDespCodEstr = CLASSE_REC_DESP.ClasseRecDespCodEstr;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("CLASSE_REC_DESP")
                    && (ChangeTracker.OriginalValues["CLASSE_REC_DESP"] == CLASSE_REC_DESP))
                {
                    ChangeTracker.OriginalValues.Remove("CLASSE_REC_DESP");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("CLASSE_REC_DESP", previousValue);
                }
                if (CLASSE_REC_DESP != null && !CLASSE_REC_DESP.ChangeTracker.ChangeTrackingEnabled)
                {
                    CLASSE_REC_DESP.StartTracking();
                }
            }
        }

        #endregion

    }
}