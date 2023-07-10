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

namespace MvcAppHylinedoBrasilMobile.Models.bdApolo2
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(FUNCIONARIO))]
    public partial class GRP_FUNC: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public string FuncCod
        {
            get { return _funcCod; }
            set
            {
                if (_funcCod != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'FuncCod' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (FUNCIONARIO != null && FUNCIONARIO.FuncCod != value)
                        {
                            FUNCIONARIO = null;
                        }
                    }
                    _funcCod = value;
                    OnPropertyChanged("FuncCod");
                }
            }
        }
        private string _funcCod;
    
        [DataMember]
        public string GrpFuncCod
        {
            get { return _grpFuncCod; }
            set
            {
                if (_grpFuncCod != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'GrpFuncCod' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (FUNCIONARIO1 != null && FUNCIONARIO1.FuncCod != value)
                        {
                            FUNCIONARIO1 = null;
                        }
                    }
                    _grpFuncCod = value;
                    OnPropertyChanged("GrpFuncCod");
                }
            }
        }
        private string _grpFuncCod;
    
        [DataMember]
        public Nullable<System.DateTime> GrpFuncDataInic
        {
            get { return _grpFuncDataInic; }
            set
            {
                if (_grpFuncDataInic != value)
                {
                    _grpFuncDataInic = value;
                    OnPropertyChanged("GrpFuncDataInic");
                }
            }
        }
        private Nullable<System.DateTime> _grpFuncDataInic;
    
        [DataMember]
        public Nullable<System.DateTime> GrpFuncDataFim
        {
            get { return _grpFuncDataFim; }
            set
            {
                if (_grpFuncDataFim != value)
                {
                    _grpFuncDataFim = value;
                    OnPropertyChanged("GrpFuncDataFim");
                }
            }
        }
        private Nullable<System.DateTime> _grpFuncDataFim;
    
        [DataMember]
        public string GrpFuncObs
        {
            get { return _grpFuncObs; }
            set
            {
                if (_grpFuncObs != value)
                {
                    _grpFuncObs = value;
                    OnPropertyChanged("GrpFuncObs");
                }
            }
        }
        private string _grpFuncObs;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public FUNCIONARIO FUNCIONARIO
        {
            get { return _fUNCIONARIO; }
            set
            {
                if (!ReferenceEquals(_fUNCIONARIO, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (FuncCod != value.FuncCod)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _fUNCIONARIO;
                    _fUNCIONARIO = value;
                    FixupFUNCIONARIO(previousValue);
                    OnNavigationPropertyChanged("FUNCIONARIO");
                }
            }
        }
        private FUNCIONARIO _fUNCIONARIO;
    
        [DataMember]
        public FUNCIONARIO FUNCIONARIO1
        {
            get { return _fUNCIONARIO1; }
            set
            {
                if (!ReferenceEquals(_fUNCIONARIO1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (GrpFuncCod != value.FuncCod)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _fUNCIONARIO1;
                    _fUNCIONARIO1 = value;
                    FixupFUNCIONARIO1(previousValue);
                    OnNavigationPropertyChanged("FUNCIONARIO1");
                }
            }
        }
        private FUNCIONARIO _fUNCIONARIO1;

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
            FUNCIONARIO = null;
            FUNCIONARIO1 = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupFUNCIONARIO(FUNCIONARIO previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.GRP_FUNC.Contains(this))
            {
                previousValue.GRP_FUNC.Remove(this);
            }
    
            if (FUNCIONARIO != null)
            {
                if (!FUNCIONARIO.GRP_FUNC.Contains(this))
                {
                    FUNCIONARIO.GRP_FUNC.Add(this);
                }
    
                FuncCod = FUNCIONARIO.FuncCod;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("FUNCIONARIO")
                    && (ChangeTracker.OriginalValues["FUNCIONARIO"] == FUNCIONARIO))
                {
                    ChangeTracker.OriginalValues.Remove("FUNCIONARIO");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("FUNCIONARIO", previousValue);
                }
                if (FUNCIONARIO != null && !FUNCIONARIO.ChangeTracker.ChangeTrackingEnabled)
                {
                    FUNCIONARIO.StartTracking();
                }
            }
        }
    
        private void FixupFUNCIONARIO1(FUNCIONARIO previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.GRP_FUNC1.Contains(this))
            {
                previousValue.GRP_FUNC1.Remove(this);
            }
    
            if (FUNCIONARIO1 != null)
            {
                if (!FUNCIONARIO1.GRP_FUNC1.Contains(this))
                {
                    FUNCIONARIO1.GRP_FUNC1.Add(this);
                }
    
                GrpFuncCod = FUNCIONARIO1.FuncCod;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("FUNCIONARIO1")
                    && (ChangeTracker.OriginalValues["FUNCIONARIO1"] == FUNCIONARIO1))
                {
                    ChangeTracker.OriginalValues.Remove("FUNCIONARIO1");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("FUNCIONARIO1", previousValue);
                }
                if (FUNCIONARIO1 != null && !FUNCIONARIO1.ChangeTracker.ChangeTrackingEnabled)
                {
                    FUNCIONARIO1.StartTracking();
                }
            }
        }

        #endregion

    }
}