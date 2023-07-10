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
    [KnownType(typeof(PED_COMP))]
    public partial class PARC_PAG_PED_COMP: IObjectWithChangeTracker, INotifyPropertyChanged
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
                        if (PED_COMP != null && PED_COMP.EmpCod != value)
                        {
                            PED_COMP = null;
                        }
                    }
                    _empCod = value;
                    OnPropertyChanged("EmpCod");
                }
            }
        }
        private string _empCod;
    
        [DataMember]
        public string PedCompNum
        {
            get { return _pedCompNum; }
            set
            {
                if (_pedCompNum != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PedCompNum' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (PED_COMP != null && PED_COMP.PedCompNum != value)
                        {
                            PED_COMP = null;
                        }
                    }
                    _pedCompNum = value;
                    OnPropertyChanged("PedCompNum");
                }
            }
        }
        private string _pedCompNum;
    
        [DataMember]
        public short ParcPagPedCompSeq
        {
            get { return _parcPagPedCompSeq; }
            set
            {
                if (_parcPagPedCompSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ParcPagPedCompSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _parcPagPedCompSeq = value;
                    OnPropertyChanged("ParcPagPedCompSeq");
                }
            }
        }
        private short _parcPagPedCompSeq;
    
        [DataMember]
        public string ParcPagPedCompNumDup
        {
            get { return _parcPagPedCompNumDup; }
            set
            {
                if (_parcPagPedCompNumDup != value)
                {
                    _parcPagPedCompNumDup = value;
                    OnPropertyChanged("ParcPagPedCompNumDup");
                }
            }
        }
        private string _parcPagPedCompNumDup;
    
        [DataMember]
        public Nullable<short> ParcPagPedCompDiasParc
        {
            get { return _parcPagPedCompDiasParc; }
            set
            {
                if (_parcPagPedCompDiasParc != value)
                {
                    _parcPagPedCompDiasParc = value;
                    OnPropertyChanged("ParcPagPedCompDiasParc");
                }
            }
        }
        private Nullable<short> _parcPagPedCompDiasParc;
    
        [DataMember]
        public Nullable<decimal> ParcPagPedCompPercFrac
        {
            get { return _parcPagPedCompPercFrac; }
            set
            {
                if (_parcPagPedCompPercFrac != value)
                {
                    _parcPagPedCompPercFrac = value;
                    OnPropertyChanged("ParcPagPedCompPercFrac");
                }
            }
        }
        private Nullable<decimal> _parcPagPedCompPercFrac;
    
        [DataMember]
        public Nullable<decimal> ParcPagPedCompVal
        {
            get { return _parcPagPedCompVal; }
            set
            {
                if (_parcPagPedCompVal != value)
                {
                    _parcPagPedCompVal = value;
                    OnPropertyChanged("ParcPagPedCompVal");
                }
            }
        }
        private Nullable<decimal> _parcPagPedCompVal;
    
        [DataMember]
        public System.DateTime ParcPagPedCompDataVenc
        {
            get { return _parcPagPedCompDataVenc; }
            set
            {
                if (_parcPagPedCompDataVenc != value)
                {
                    _parcPagPedCompDataVenc = value;
                    OnPropertyChanged("ParcPagPedCompDataVenc");
                }
            }
        }
        private System.DateTime _parcPagPedCompDataVenc;
    
        [DataMember]
        public string ParcPagPedCompAdiant
        {
            get { return _parcPagPedCompAdiant; }
            set
            {
                if (_parcPagPedCompAdiant != value)
                {
                    _parcPagPedCompAdiant = value;
                    OnPropertyChanged("ParcPagPedCompAdiant");
                }
            }
        }
        private string _parcPagPedCompAdiant;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public PED_COMP PED_COMP
        {
            get { return _pED_COMP; }
            set
            {
                if (!ReferenceEquals(_pED_COMP, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EmpCod != value.EmpCod || PedCompNum != value.PedCompNum)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _pED_COMP;
                    _pED_COMP = value;
                    FixupPED_COMP(previousValue);
                    OnNavigationPropertyChanged("PED_COMP");
                }
            }
        }
        private PED_COMP _pED_COMP;

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
            PED_COMP = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupPED_COMP(PED_COMP previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PARC_PAG_PED_COMP.Contains(this))
            {
                previousValue.PARC_PAG_PED_COMP.Remove(this);
            }
    
            if (PED_COMP != null)
            {
                if (!PED_COMP.PARC_PAG_PED_COMP.Contains(this))
                {
                    PED_COMP.PARC_PAG_PED_COMP.Add(this);
                }
    
                EmpCod = PED_COMP.EmpCod;
                PedCompNum = PED_COMP.PedCompNum;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PED_COMP")
                    && (ChangeTracker.OriginalValues["PED_COMP"] == PED_COMP))
                {
                    ChangeTracker.OriginalValues.Remove("PED_COMP");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PED_COMP", previousValue);
                }
                if (PED_COMP != null && !PED_COMP.ChangeTracker.ChangeTrackingEnabled)
                {
                    PED_COMP.StartTracking();
                }
            }
        }

        #endregion

    }
}
