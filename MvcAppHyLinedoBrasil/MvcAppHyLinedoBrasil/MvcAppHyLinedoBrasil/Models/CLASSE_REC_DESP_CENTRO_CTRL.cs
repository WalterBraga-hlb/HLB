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
    [KnownType(typeof(CLASSE_REC_DESP))]
    public partial class CLASSE_REC_DESP_CENTRO_CTRL: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
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
        public Nullable<decimal> ClasseRecDespCCtrlPerc
        {
            get { return _classeRecDespCCtrlPerc; }
            set
            {
                if (_classeRecDespCCtrlPerc != value)
                {
                    _classeRecDespCCtrlPerc = value;
                    OnPropertyChanged("ClasseRecDespCCtrlPerc");
                }
            }
        }
        private Nullable<decimal> _classeRecDespCCtrlPerc;
    
        [DataMember]
        public string PlanoCtaEmpCod
        {
            get { return _planoCtaEmpCod; }
            set
            {
                if (_planoCtaEmpCod != value)
                {
                    _planoCtaEmpCod = value;
                    OnPropertyChanged("PlanoCtaEmpCod");
                }
            }
        }
        private string _planoCtaEmpCod;
    
        [DataMember]
        public string ClasseRecDespCCtrlCContab
        {
            get { return _classeRecDespCCtrlCContab; }
            set
            {
                if (_classeRecDespCCtrlCContab != value)
                {
                    _classeRecDespCCtrlCContab = value;
                    OnPropertyChanged("ClasseRecDespCCtrlCContab");
                }
            }
        }
        private string _classeRecDespCCtrlCContab;
    
        [DataMember]
        public System.DateTime ClasseRecDespCCtrlDataInic
        {
            get { return _classeRecDespCCtrlDataInic; }
            set
            {
                if (_classeRecDespCCtrlDataInic != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ClasseRecDespCCtrlDataInic' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _classeRecDespCCtrlDataInic = value;
                    OnPropertyChanged("ClasseRecDespCCtrlDataInic");
                }
            }
        }
        private System.DateTime _classeRecDespCCtrlDataInic;
    
        [DataMember]
        public Nullable<System.DateTime> ClasseRecDespCCtrlDataFim
        {
            get { return _classeRecDespCCtrlDataFim; }
            set
            {
                if (_classeRecDespCCtrlDataFim != value)
                {
                    _classeRecDespCCtrlDataFim = value;
                    OnPropertyChanged("ClasseRecDespCCtrlDataFim");
                }
            }
        }
        private Nullable<System.DateTime> _classeRecDespCCtrlDataFim;

        #endregion

        #region Navigation Properties
    
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
            CLASSE_REC_DESP = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupCLASSE_REC_DESP(CLASSE_REC_DESP previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CLASSE_REC_DESP_CENTRO_CTRL.Contains(this))
            {
                previousValue.CLASSE_REC_DESP_CENTRO_CTRL.Remove(this);
            }
    
            if (CLASSE_REC_DESP != null)
            {
                if (!CLASSE_REC_DESP.CLASSE_REC_DESP_CENTRO_CTRL.Contains(this))
                {
                    CLASSE_REC_DESP.CLASSE_REC_DESP_CENTRO_CTRL.Add(this);
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
