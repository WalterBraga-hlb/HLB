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

namespace MvcAppHyLinedoBrasil.Models.DiarioProducaoRacao
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(REQ_MAT))]
    public partial class RATEIO_REQ_MAT: IObjectWithChangeTracker, INotifyPropertyChanged
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
                        if (REQ_MAT != null && REQ_MAT.EmpCod != value)
                        {
                            REQ_MAT = null;
                        }
                    }
                    _empCod = value;
                    OnPropertyChanged("EmpCod");
                }
            }
        }
        private string _empCod;
    
        [DataMember]
        public string ReqMatNum
        {
            get { return _reqMatNum; }
            set
            {
                if (_reqMatNum != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ReqMatNum' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (REQ_MAT != null && REQ_MAT.ReqMatNum != value)
                        {
                            REQ_MAT = null;
                        }
                    }
                    _reqMatNum = value;
                    OnPropertyChanged("ReqMatNum");
                }
            }
        }
        private string _reqMatNum;
    
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
        public Nullable<decimal> RatReqMatPerc
        {
            get { return _ratReqMatPerc; }
            set
            {
                if (_ratReqMatPerc != value)
                {
                    _ratReqMatPerc = value;
                    OnPropertyChanged("RatReqMatPerc");
                }
            }
        }
        private Nullable<decimal> _ratReqMatPerc;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public REQ_MAT REQ_MAT
        {
            get { return _rEQ_MAT; }
            set
            {
                if (!ReferenceEquals(_rEQ_MAT, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EmpCod != value.EmpCod || ReqMatNum != value.ReqMatNum)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _rEQ_MAT;
                    _rEQ_MAT = value;
                    FixupREQ_MAT(previousValue);
                    OnNavigationPropertyChanged("REQ_MAT");
                }
            }
        }
        private REQ_MAT _rEQ_MAT;

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
            REQ_MAT = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupREQ_MAT(REQ_MAT previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.RATEIO_REQ_MAT.Contains(this))
            {
                previousValue.RATEIO_REQ_MAT.Remove(this);
            }
    
            if (REQ_MAT != null)
            {
                if (!REQ_MAT.RATEIO_REQ_MAT.Contains(this))
                {
                    REQ_MAT.RATEIO_REQ_MAT.Add(this);
                }
    
                EmpCod = REQ_MAT.EmpCod;
                ReqMatNum = REQ_MAT.ReqMatNum;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("REQ_MAT")
                    && (ChangeTracker.OriginalValues["REQ_MAT"] == REQ_MAT))
                {
                    ChangeTracker.OriginalValues.Remove("REQ_MAT");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("REQ_MAT", previousValue);
                }
                if (REQ_MAT != null && !REQ_MAT.ChangeTracker.ChangeTrackingEnabled)
                {
                    REQ_MAT.StartTracking();
                }
            }
        }

        #endregion

    }
}
