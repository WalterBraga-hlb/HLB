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
    [KnownType(typeof(EMPRESA_FILIAL))]
    [KnownType(typeof(PRODUTO1))]
    public partial class PROD_CONTA_CONTAB: IObjectWithChangeTracker, INotifyPropertyChanged
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
                        if (EMPRESA_FILIAL != null && EMPRESA_FILIAL.EmpCod != value)
                        {
                            EMPRESA_FILIAL = null;
                        }
                    }
                    _empCod = value;
                    OnPropertyChanged("EmpCod");
                }
            }
        }
        private string _empCod;
    
        [DataMember]
        public string ProdCodEstr
        {
            get { return _prodCodEstr; }
            set
            {
                if (_prodCodEstr != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ProdCodEstr' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (PRODUTO1 != null && PRODUTO1.ProdCodEstr != value)
                        {
                            PRODUTO1 = null;
                        }
                    }
                    _prodCodEstr = value;
                    OnPropertyChanged("ProdCodEstr");
                }
            }
        }
        private string _prodCodEstr;
    
        [DataMember]
        public short ProdCContabSeq
        {
            get { return _prodCContabSeq; }
            set
            {
                if (_prodCContabSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ProdCContabSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _prodCContabSeq = value;
                    OnPropertyChanged("ProdCContabSeq");
                }
            }
        }
        private short _prodCContabSeq;
    
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
        public string ProdCContabCred
        {
            get { return _prodCContabCred; }
            set
            {
                if (_prodCContabCred != value)
                {
                    _prodCContabCred = value;
                    OnPropertyChanged("ProdCContabCred");
                }
            }
        }
        private string _prodCContabCred;
    
        [DataMember]
        public string ProdCContabCred2
        {
            get { return _prodCContabCred2; }
            set
            {
                if (_prodCContabCred2 != value)
                {
                    _prodCContabCred2 = value;
                    OnPropertyChanged("ProdCContabCred2");
                }
            }
        }
        private string _prodCContabCred2;
    
        [DataMember]
        public string ProdCContabCred3
        {
            get { return _prodCContabCred3; }
            set
            {
                if (_prodCContabCred3 != value)
                {
                    _prodCContabCred3 = value;
                    OnPropertyChanged("ProdCContabCred3");
                }
            }
        }
        private string _prodCContabCred3;
    
        [DataMember]
        public string ProdCContabDeb
        {
            get { return _prodCContabDeb; }
            set
            {
                if (_prodCContabDeb != value)
                {
                    _prodCContabDeb = value;
                    OnPropertyChanged("ProdCContabDeb");
                }
            }
        }
        private string _prodCContabDeb;
    
        [DataMember]
        public string ProdCContabDeb2
        {
            get { return _prodCContabDeb2; }
            set
            {
                if (_prodCContabDeb2 != value)
                {
                    _prodCContabDeb2 = value;
                    OnPropertyChanged("ProdCContabDeb2");
                }
            }
        }
        private string _prodCContabDeb2;
    
        [DataMember]
        public string ProdCContabDeb3
        {
            get { return _prodCContabDeb3; }
            set
            {
                if (_prodCContabDeb3 != value)
                {
                    _prodCContabDeb3 = value;
                    OnPropertyChanged("ProdCContabDeb3");
                }
            }
        }
        private string _prodCContabDeb3;
    
        [DataMember]
        public Nullable<System.DateTime> ProdCContabDtValInic
        {
            get { return _prodCContabDtValInic; }
            set
            {
                if (_prodCContabDtValInic != value)
                {
                    _prodCContabDtValInic = value;
                    OnPropertyChanged("ProdCContabDtValInic");
                }
            }
        }
        private Nullable<System.DateTime> _prodCContabDtValInic;
    
        [DataMember]
        public Nullable<System.DateTime> ProdCContabDtValFim
        {
            get { return _prodCContabDtValFim; }
            set
            {
                if (_prodCContabDtValFim != value)
                {
                    _prodCContabDtValFim = value;
                    OnPropertyChanged("ProdCContabDtValFim");
                }
            }
        }
        private Nullable<System.DateTime> _prodCContabDtValFim;
    
        [DataMember]
        public string ProdCContabCred4
        {
            get { return _prodCContabCred4; }
            set
            {
                if (_prodCContabCred4 != value)
                {
                    _prodCContabCred4 = value;
                    OnPropertyChanged("ProdCContabCred4");
                }
            }
        }
        private string _prodCContabCred4;
    
        [DataMember]
        public string ProdCContabDeb4
        {
            get { return _prodCContabDeb4; }
            set
            {
                if (_prodCContabDeb4 != value)
                {
                    _prodCContabDeb4 = value;
                    OnPropertyChanged("ProdCContabDeb4");
                }
            }
        }
        private string _prodCContabDeb4;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public EMPRESA_FILIAL EMPRESA_FILIAL
        {
            get { return _eMPRESA_FILIAL; }
            set
            {
                if (!ReferenceEquals(_eMPRESA_FILIAL, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EmpCod != value.EmpCod)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _eMPRESA_FILIAL;
                    _eMPRESA_FILIAL = value;
                    FixupEMPRESA_FILIAL(previousValue);
                    OnNavigationPropertyChanged("EMPRESA_FILIAL");
                }
            }
        }
        private EMPRESA_FILIAL _eMPRESA_FILIAL;
    
        [DataMember]
        public PRODUTO1 PRODUTO1
        {
            get { return _pRODUTO1; }
            set
            {
                if (!ReferenceEquals(_pRODUTO1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (ProdCodEstr != value.ProdCodEstr)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _pRODUTO1;
                    _pRODUTO1 = value;
                    FixupPRODUTO1(previousValue);
                    OnNavigationPropertyChanged("PRODUTO1");
                }
            }
        }
        private PRODUTO1 _pRODUTO1;

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
            EMPRESA_FILIAL = null;
            PRODUTO1 = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupEMPRESA_FILIAL(EMPRESA_FILIAL previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PROD_CONTA_CONTAB.Contains(this))
            {
                previousValue.PROD_CONTA_CONTAB.Remove(this);
            }
    
            if (EMPRESA_FILIAL != null)
            {
                if (!EMPRESA_FILIAL.PROD_CONTA_CONTAB.Contains(this))
                {
                    EMPRESA_FILIAL.PROD_CONTA_CONTAB.Add(this);
                }
    
                EmpCod = EMPRESA_FILIAL.EmpCod;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("EMPRESA_FILIAL")
                    && (ChangeTracker.OriginalValues["EMPRESA_FILIAL"] == EMPRESA_FILIAL))
                {
                    ChangeTracker.OriginalValues.Remove("EMPRESA_FILIAL");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("EMPRESA_FILIAL", previousValue);
                }
                if (EMPRESA_FILIAL != null && !EMPRESA_FILIAL.ChangeTracker.ChangeTrackingEnabled)
                {
                    EMPRESA_FILIAL.StartTracking();
                }
            }
        }
    
        private void FixupPRODUTO1(PRODUTO1 previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PROD_CONTA_CONTAB.Contains(this))
            {
                previousValue.PROD_CONTA_CONTAB.Remove(this);
            }
    
            if (PRODUTO1 != null)
            {
                if (!PRODUTO1.PROD_CONTA_CONTAB.Contains(this))
                {
                    PRODUTO1.PROD_CONTA_CONTAB.Add(this);
                }
    
                ProdCodEstr = PRODUTO1.ProdCodEstr;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PRODUTO1")
                    && (ChangeTracker.OriginalValues["PRODUTO1"] == PRODUTO1))
                {
                    ChangeTracker.OriginalValues.Remove("PRODUTO1");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PRODUTO1", previousValue);
                }
                if (PRODUTO1 != null && !PRODUTO1.ChangeTracker.ChangeTrackingEnabled)
                {
                    PRODUTO1.StartTracking();
                }
            }
        }

        #endregion

    }
}
