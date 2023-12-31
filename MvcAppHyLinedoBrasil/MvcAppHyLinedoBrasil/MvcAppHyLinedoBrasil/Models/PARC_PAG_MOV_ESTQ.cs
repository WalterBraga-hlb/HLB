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
    public partial class PARC_PAG_MOV_ESTQ: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public short ParcPagMovEstqSeq
        {
            get { return _parcPagMovEstqSeq; }
            set
            {
                if (_parcPagMovEstqSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ParcPagMovEstqSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _parcPagMovEstqSeq = value;
                    OnPropertyChanged("ParcPagMovEstqSeq");
                }
            }
        }
        private short _parcPagMovEstqSeq;
    
        [DataMember]
        public string ParcPagMovEstqEspec
        {
            get { return _parcPagMovEstqEspec; }
            set
            {
                if (_parcPagMovEstqEspec != value)
                {
                    _parcPagMovEstqEspec = value;
                    OnPropertyChanged("ParcPagMovEstqEspec");
                }
            }
        }
        private string _parcPagMovEstqEspec;
    
        [DataMember]
        public string ParcPagMovEstqSerie
        {
            get { return _parcPagMovEstqSerie; }
            set
            {
                if (_parcPagMovEstqSerie != value)
                {
                    _parcPagMovEstqSerie = value;
                    OnPropertyChanged("ParcPagMovEstqSerie");
                }
            }
        }
        private string _parcPagMovEstqSerie;
    
        [DataMember]
        public string ParcPagMovEstqNum
        {
            get { return _parcPagMovEstqNum; }
            set
            {
                if (_parcPagMovEstqNum != value)
                {
                    _parcPagMovEstqNum = value;
                    OnPropertyChanged("ParcPagMovEstqNum");
                }
            }
        }
        private string _parcPagMovEstqNum;
    
        [DataMember]
        public Nullable<System.DateTime> ParcPagMovEstqDataEmissao
        {
            get { return _parcPagMovEstqDataEmissao; }
            set
            {
                if (_parcPagMovEstqDataEmissao != value)
                {
                    _parcPagMovEstqDataEmissao = value;
                    OnPropertyChanged("ParcPagMovEstqDataEmissao");
                }
            }
        }
        private Nullable<System.DateTime> _parcPagMovEstqDataEmissao;
    
        [DataMember]
        public Nullable<decimal> ParcPagMovEstqVal
        {
            get { return _parcPagMovEstqVal; }
            set
            {
                if (_parcPagMovEstqVal != value)
                {
                    _parcPagMovEstqVal = value;
                    OnPropertyChanged("ParcPagMovEstqVal");
                }
            }
        }
        private Nullable<decimal> _parcPagMovEstqVal;
    
        [DataMember]
        public System.DateTime ParcPagMovEstqDataVenc
        {
            get { return _parcPagMovEstqDataVenc; }
            set
            {
                if (_parcPagMovEstqDataVenc != value)
                {
                    _parcPagMovEstqDataVenc = value;
                    OnPropertyChanged("ParcPagMovEstqDataVenc");
                }
            }
        }
        private System.DateTime _parcPagMovEstqDataVenc;
    
        [DataMember]
        public Nullable<decimal> ParcPagMovEstqValPag
        {
            get { return _parcPagMovEstqValPag; }
            set
            {
                if (_parcPagMovEstqValPag != value)
                {
                    _parcPagMovEstqValPag = value;
                    OnPropertyChanged("ParcPagMovEstqValPag");
                }
            }
        }
        private Nullable<decimal> _parcPagMovEstqValPag;
    
        [DataMember]
        public Nullable<System.DateTime> ParcPagMovEstqDataPag
        {
            get { return _parcPagMovEstqDataPag; }
            set
            {
                if (_parcPagMovEstqDataPag != value)
                {
                    _parcPagMovEstqDataPag = value;
                    OnPropertyChanged("ParcPagMovEstqDataPag");
                }
            }
        }
        private Nullable<System.DateTime> _parcPagMovEstqDataPag;
    
        [DataMember]
        public Nullable<System.DateTime> ParcPagMovEstqDataProrrog
        {
            get { return _parcPagMovEstqDataProrrog; }
            set
            {
                if (_parcPagMovEstqDataProrrog != value)
                {
                    _parcPagMovEstqDataProrrog = value;
                    OnPropertyChanged("ParcPagMovEstqDataProrrog");
                }
            }
        }
        private Nullable<System.DateTime> _parcPagMovEstqDataProrrog;
    
        [DataMember]
        public string ParcPagMovEstqBcoNum
        {
            get { return _parcPagMovEstqBcoNum; }
            set
            {
                if (_parcPagMovEstqBcoNum != value)
                {
                    _parcPagMovEstqBcoNum = value;
                    OnPropertyChanged("ParcPagMovEstqBcoNum");
                }
            }
        }
        private string _parcPagMovEstqBcoNum;
    
        [DataMember]
        public string ParcPagMovEstqAgNum
        {
            get { return _parcPagMovEstqAgNum; }
            set
            {
                if (_parcPagMovEstqAgNum != value)
                {
                    _parcPagMovEstqAgNum = value;
                    OnPropertyChanged("ParcPagMovEstqAgNum");
                }
            }
        }
        private string _parcPagMovEstqAgNum;
    
        [DataMember]
        public string ParcPagMovEstqTipoCobCod
        {
            get { return _parcPagMovEstqTipoCobCod; }
            set
            {
                if (_parcPagMovEstqTipoCobCod != value)
                {
                    _parcPagMovEstqTipoCobCod = value;
                    OnPropertyChanged("ParcPagMovEstqTipoCobCod");
                }
            }
        }
        private string _parcPagMovEstqTipoCobCod;

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
    
            if (previousValue != null && previousValue.PARC_PAG_MOV_ESTQ.Contains(this))
            {
                previousValue.PARC_PAG_MOV_ESTQ.Remove(this);
            }
    
            if (MOV_ESTQ != null)
            {
                if (!MOV_ESTQ.PARC_PAG_MOV_ESTQ.Contains(this))
                {
                    MOV_ESTQ.PARC_PAG_MOV_ESTQ.Add(this);
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
