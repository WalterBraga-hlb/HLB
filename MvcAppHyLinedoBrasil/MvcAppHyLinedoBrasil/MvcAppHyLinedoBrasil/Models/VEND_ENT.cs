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
    [KnownType(typeof(VENDEDOR))]
    [KnownType(typeof(ENTIDADE))]
    public partial class VEND_ENT: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public string EntCod
        {
            get { return _entCod; }
            set
            {
                if (_entCod != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'EntCod' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (ENTIDADE != null && ENTIDADE.EntCod != value)
                        {
                            ENTIDADE = null;
                        }
                    }
                    _entCod = value;
                    OnPropertyChanged("EntCod");
                }
            }
        }
        private string _entCod;
    
        [DataMember]
        public string VendCod
        {
            get { return _vendCod; }
            set
            {
                if (_vendCod != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'VendCod' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (VENDEDOR != null && VENDEDOR.VendCod != value)
                        {
                            VENDEDOR = null;
                        }
                    }
                    _vendCod = value;
                    OnPropertyChanged("VendCod");
                }
            }
        }
        private string _vendCod;
    
        [DataMember]
        public string VendEntPrinc
        {
            get { return _vendEntPrinc; }
            set
            {
                if (_vendEntPrinc != value)
                {
                    _vendEntPrinc = value;
                    OnPropertyChanged("VendEntPrinc");
                }
            }
        }
        private string _vendEntPrinc;
    
        [DataMember]
        public string CCtrlCodEstr
        {
            get { return _cCtrlCodEstr; }
            set
            {
                if (_cCtrlCodEstr != value)
                {
                    _cCtrlCodEstr = value;
                    OnPropertyChanged("CCtrlCodEstr");
                }
            }
        }
        private string _cCtrlCodEstr;
    
        [DataMember]
        public System.Guid RowGuid
        {
            get { return _rowGuid; }
            set
            {
                if (_rowGuid != value)
                {
                    _rowGuid = value;
                    OnPropertyChanged("RowGuid");
                }
            }
        }
        private System.Guid _rowGuid;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public VENDEDOR VENDEDOR
        {
            get { return _vENDEDOR; }
            set
            {
                if (!ReferenceEquals(_vENDEDOR, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (VendCod != value.VendCod)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _vENDEDOR;
                    _vENDEDOR = value;
                    FixupVENDEDOR(previousValue);
                    OnNavigationPropertyChanged("VENDEDOR");
                }
            }
        }
        private VENDEDOR _vENDEDOR;
    
        [DataMember]
        public ENTIDADE ENTIDADE
        {
            get { return _eNTIDADE; }
            set
            {
                if (!ReferenceEquals(_eNTIDADE, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EntCod != value.EntCod)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _eNTIDADE;
                    _eNTIDADE = value;
                    FixupENTIDADE(previousValue);
                    OnNavigationPropertyChanged("ENTIDADE");
                }
            }
        }
        private ENTIDADE _eNTIDADE;

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
            VENDEDOR = null;
            ENTIDADE = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupVENDEDOR(VENDEDOR previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.VEND_ENT.Contains(this))
            {
                previousValue.VEND_ENT.Remove(this);
            }
    
            if (VENDEDOR != null)
            {
                if (!VENDEDOR.VEND_ENT.Contains(this))
                {
                    VENDEDOR.VEND_ENT.Add(this);
                }
    
                VendCod = VENDEDOR.VendCod;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("VENDEDOR")
                    && (ChangeTracker.OriginalValues["VENDEDOR"] == VENDEDOR))
                {
                    ChangeTracker.OriginalValues.Remove("VENDEDOR");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("VENDEDOR", previousValue);
                }
                if (VENDEDOR != null && !VENDEDOR.ChangeTracker.ChangeTrackingEnabled)
                {
                    VENDEDOR.StartTracking();
                }
            }
        }
    
        private void FixupENTIDADE(ENTIDADE previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.VEND_ENT.Contains(this))
            {
                previousValue.VEND_ENT.Remove(this);
            }
    
            if (ENTIDADE != null)
            {
                if (!ENTIDADE.VEND_ENT.Contains(this))
                {
                    ENTIDADE.VEND_ENT.Add(this);
                }
    
                EntCod = ENTIDADE.EntCod;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("ENTIDADE")
                    && (ChangeTracker.OriginalValues["ENTIDADE"] == ENTIDADE))
                {
                    ChangeTracker.OriginalValues.Remove("ENTIDADE");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("ENTIDADE", previousValue);
                }
                if (ENTIDADE != null && !ENTIDADE.ChangeTracker.ChangeTrackingEnabled)
                {
                    ENTIDADE.StartTracking();
                }
            }
        }

        #endregion

    }
}
