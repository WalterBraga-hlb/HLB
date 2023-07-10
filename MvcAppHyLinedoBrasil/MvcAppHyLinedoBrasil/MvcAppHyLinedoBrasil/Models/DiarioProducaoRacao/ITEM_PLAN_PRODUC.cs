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
    [KnownType(typeof(PLAN_PRODUC))]
    [KnownType(typeof(PLAN_PRODUC_FIC_TEC))]
    [KnownType(typeof(PROD_UNID_MED))]
    [KnownType(typeof(PRODUTO))]
    public partial class ITEM_PLAN_PRODUC: IObjectWithChangeTracker, INotifyPropertyChanged
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
                        if (PLAN_PRODUC != null && PLAN_PRODUC.EmpCod != value)
                        {
                            PLAN_PRODUC = null;
                        }
                    }
                    _empCod = value;
                    OnPropertyChanged("EmpCod");
                }
            }
        }
        private string _empCod;
    
        [DataMember]
        public string PlanProducNum
        {
            get { return _planProducNum; }
            set
            {
                if (_planProducNum != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PlanProducNum' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (PLAN_PRODUC != null && PLAN_PRODUC.PlanProducNum != value)
                        {
                            PLAN_PRODUC = null;
                        }
                    }
                    _planProducNum = value;
                    OnPropertyChanged("PlanProducNum");
                }
            }
        }
        private string _planProducNum;
    
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
                        if (PROD_UNID_MED != null && PROD_UNID_MED.ProdCodEstr != value)
                        {
                            var previousValue = _pROD_UNID_MED;
                            _pROD_UNID_MED = null;
                            FixupPROD_UNID_MED(previousValue, skipKeys: true);
                            OnNavigationPropertyChanged("PROD_UNID_MED");
                        }
                        if (PRODUTO != null && PRODUTO.ProdCodEstr != value)
                        {
                            PRODUTO = null;
                        }
                    }
                    _prodCodEstr = value;
                    OnPropertyChanged("ProdCodEstr");
                }
            }
        }
        private string _prodCodEstr;
    
        [DataMember]
        public short ItPlanProducSeq
        {
            get { return _itPlanProducSeq; }
            set
            {
                if (_itPlanProducSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ItPlanProducSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _itPlanProducSeq = value;
                    OnPropertyChanged("ItPlanProducSeq");
                }
            }
        }
        private short _itPlanProducSeq;
    
        [DataMember]
        public string ProdGradeCorCod
        {
            get { return _prodGradeCorCod; }
            set
            {
                if (_prodGradeCorCod != value)
                {
                    _prodGradeCorCod = value;
                    OnPropertyChanged("ProdGradeCorCod");
                }
            }
        }
        private string _prodGradeCorCod;
    
        [DataMember]
        public string ItPlanProducUnidMedCod
        {
            get { return _itPlanProducUnidMedCod; }
            set
            {
                if (_itPlanProducUnidMedCod != value)
                {
                    ChangeTracker.RecordOriginalValue("ItPlanProducUnidMedCod", _itPlanProducUnidMedCod);
                    if (!IsDeserializing)
                    {
                        if (PROD_UNID_MED != null && PROD_UNID_MED.ProdUnidMedCod != value)
                        {
                            var previousValue = _pROD_UNID_MED;
                            _pROD_UNID_MED = null;
                            FixupPROD_UNID_MED(previousValue, skipKeys: true);
                            OnNavigationPropertyChanged("PROD_UNID_MED");
                        }
                    }
                    _itPlanProducUnidMedCod = value;
                    OnPropertyChanged("ItPlanProducUnidMedCod");
                }
            }
        }
        private string _itPlanProducUnidMedCod;
    
        [DataMember]
        public Nullable<short> ItPlanProducUnidMedPos
        {
            get { return _itPlanProducUnidMedPos; }
            set
            {
                if (_itPlanProducUnidMedPos != value)
                {
                    ChangeTracker.RecordOriginalValue("ItPlanProducUnidMedPos", _itPlanProducUnidMedPos);
                    if (!IsDeserializing)
                    {
                        if (PROD_UNID_MED != null && PROD_UNID_MED.ProdUnidMedPos != value)
                        {
                            var previousValue = _pROD_UNID_MED;
                            _pROD_UNID_MED = null;
                            FixupPROD_UNID_MED(previousValue, skipKeys: true);
                            OnNavigationPropertyChanged("PROD_UNID_MED");
                        }
                    }
                    _itPlanProducUnidMedPos = value;
                    OnPropertyChanged("ItPlanProducUnidMedPos");
                }
            }
        }
        private Nullable<short> _itPlanProducUnidMedPos;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtd
        {
            get { return _itPlanProducQtd; }
            set
            {
                if (_itPlanProducQtd != value)
                {
                    _itPlanProducQtd = value;
                    OnPropertyChanged("ItPlanProducQtd");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtd;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdEstq
        {
            get { return _itPlanProducQtdEstq; }
            set
            {
                if (_itPlanProducQtdEstq != value)
                {
                    _itPlanProducQtdEstq = value;
                    OnPropertyChanged("ItPlanProducQtdEstq");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdEstq;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdReserv
        {
            get { return _itPlanProducQtdReserv; }
            set
            {
                if (_itPlanProducQtdReserv != value)
                {
                    _itPlanProducQtdReserv = value;
                    OnPropertyChanged("ItPlanProducQtdReserv");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdReserv;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdEmp
        {
            get { return _itPlanProducQtdEmp; }
            set
            {
                if (_itPlanProducQtdEmp != value)
                {
                    _itPlanProducQtdEmp = value;
                    OnPropertyChanged("ItPlanProducQtdEmp");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdEmp;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdNec
        {
            get { return _itPlanProducQtdNec; }
            set
            {
                if (_itPlanProducQtdNec != value)
                {
                    _itPlanProducQtdNec = value;
                    OnPropertyChanged("ItPlanProducQtdNec");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdNec;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdDisp
        {
            get { return _itPlanProducQtdDisp; }
            set
            {
                if (_itPlanProducQtdDisp != value)
                {
                    _itPlanProducQtdDisp = value;
                    OnPropertyChanged("ItPlanProducQtdDisp");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdDisp;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdComp
        {
            get { return _itPlanProducQtdComp; }
            set
            {
                if (_itPlanProducQtdComp != value)
                {
                    _itPlanProducQtdComp = value;
                    OnPropertyChanged("ItPlanProducQtdComp");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdComp;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdNecPeso
        {
            get { return _itPlanProducQtdNecPeso; }
            set
            {
                if (_itPlanProducQtdNecPeso != value)
                {
                    _itPlanProducQtdNecPeso = value;
                    OnPropertyChanged("ItPlanProducQtdNecPeso");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdNecPeso;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducIndRetalho
        {
            get { return _itPlanProducIndRetalho; }
            set
            {
                if (_itPlanProducIndRetalho != value)
                {
                    _itPlanProducIndRetalho = value;
                    OnPropertyChanged("ItPlanProducIndRetalho");
                }
            }
        }
        private Nullable<decimal> _itPlanProducIndRetalho;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdNecPesoTot
        {
            get { return _itPlanProducQtdNecPesoTot; }
            set
            {
                if (_itPlanProducQtdNecPesoTot != value)
                {
                    _itPlanProducQtdNecPesoTot = value;
                    OnPropertyChanged("ItPlanProducQtdNecPesoTot");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdNecPesoTot;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducCapHrMaq
        {
            get { return _itPlanProducCapHrMaq; }
            set
            {
                if (_itPlanProducCapHrMaq != value)
                {
                    _itPlanProducCapHrMaq = value;
                    OnPropertyChanged("ItPlanProducCapHrMaq");
                }
            }
        }
        private Nullable<decimal> _itPlanProducCapHrMaq;
    
        [DataMember]
        public string ItPlanProducConsidEstq
        {
            get { return _itPlanProducConsidEstq; }
            set
            {
                if (_itPlanProducConsidEstq != value)
                {
                    _itPlanProducConsidEstq = value;
                    OnPropertyChanged("ItPlanProducConsidEstq");
                }
            }
        }
        private string _itPlanProducConsidEstq;
    
        [DataMember]
        public string PedVendaNum
        {
            get { return _pedVendaNum; }
            set
            {
                if (_pedVendaNum != value)
                {
                    _pedVendaNum = value;
                    OnPropertyChanged("PedVendaNum");
                }
            }
        }
        private string _pedVendaNum;
    
        [DataMember]
        public Nullable<short> ItPedVendaSeq
        {
            get { return _itPedVendaSeq; }
            set
            {
                if (_itPedVendaSeq != value)
                {
                    _itPedVendaSeq = value;
                    OnPropertyChanged("ItPedVendaSeq");
                }
            }
        }
        private Nullable<short> _itPedVendaSeq;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdComplementar
        {
            get { return _itPlanProducQtdComplementar; }
            set
            {
                if (_itPlanProducQtdComplementar != value)
                {
                    _itPlanProducQtdComplementar = value;
                    OnPropertyChanged("ItPlanProducQtdComplementar");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdComplementar;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdEstqMin
        {
            get { return _itPlanProducQtdEstqMin; }
            set
            {
                if (_itPlanProducQtdEstqMin != value)
                {
                    _itPlanProducQtdEstqMin = value;
                    OnPropertyChanged("ItPlanProducQtdEstqMin");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdEstqMin;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdEstqUtil
        {
            get { return _itPlanProducQtdEstqUtil; }
            set
            {
                if (_itPlanProducQtdEstqUtil != value)
                {
                    _itPlanProducQtdEstqUtil = value;
                    OnPropertyChanged("ItPlanProducQtdEstqUtil");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdEstqUtil;
    
        [DataMember]
        public Nullable<decimal> ItPlanProducQtdEstqUso
        {
            get { return _itPlanProducQtdEstqUso; }
            set
            {
                if (_itPlanProducQtdEstqUso != value)
                {
                    _itPlanProducQtdEstqUso = value;
                    OnPropertyChanged("ItPlanProducQtdEstqUso");
                }
            }
        }
        private Nullable<decimal> _itPlanProducQtdEstqUso;
    
        [DataMember]
        public Nullable<int> IdProdId
        {
            get { return _idProdId; }
            set
            {
                if (_idProdId != value)
                {
                    _idProdId = value;
                    OnPropertyChanged("IdProdId");
                }
            }
        }
        private Nullable<int> _idProdId;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public PLAN_PRODUC PLAN_PRODUC
        {
            get { return _pLAN_PRODUC; }
            set
            {
                if (!ReferenceEquals(_pLAN_PRODUC, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EmpCod != value.EmpCod || PlanProducNum != value.PlanProducNum)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _pLAN_PRODUC;
                    _pLAN_PRODUC = value;
                    FixupPLAN_PRODUC(previousValue);
                    OnNavigationPropertyChanged("PLAN_PRODUC");
                }
            }
        }
        private PLAN_PRODUC _pLAN_PRODUC;
    
        [DataMember]
        public TrackableCollection<PLAN_PRODUC_FIC_TEC> PLAN_PRODUC_FIC_TEC
        {
            get
            {
                if (_pLAN_PRODUC_FIC_TEC == null)
                {
                    _pLAN_PRODUC_FIC_TEC = new TrackableCollection<PLAN_PRODUC_FIC_TEC>();
                    _pLAN_PRODUC_FIC_TEC.CollectionChanged += FixupPLAN_PRODUC_FIC_TEC;
                }
                return _pLAN_PRODUC_FIC_TEC;
            }
            set
            {
                if (!ReferenceEquals(_pLAN_PRODUC_FIC_TEC, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_pLAN_PRODUC_FIC_TEC != null)
                    {
                        _pLAN_PRODUC_FIC_TEC.CollectionChanged -= FixupPLAN_PRODUC_FIC_TEC;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (PLAN_PRODUC_FIC_TEC item in _pLAN_PRODUC_FIC_TEC)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _pLAN_PRODUC_FIC_TEC = value;
                    if (_pLAN_PRODUC_FIC_TEC != null)
                    {
                        _pLAN_PRODUC_FIC_TEC.CollectionChanged += FixupPLAN_PRODUC_FIC_TEC;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (PLAN_PRODUC_FIC_TEC item in _pLAN_PRODUC_FIC_TEC)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("PLAN_PRODUC_FIC_TEC");
                }
            }
        }
        private TrackableCollection<PLAN_PRODUC_FIC_TEC> _pLAN_PRODUC_FIC_TEC;
    
        [DataMember]
        public PROD_UNID_MED PROD_UNID_MED
        {
            get { return _pROD_UNID_MED; }
            set
            {
                if (!ReferenceEquals(_pROD_UNID_MED, value))
                {
                    var previousValue = _pROD_UNID_MED;
                    _pROD_UNID_MED = value;
                    FixupPROD_UNID_MED(previousValue);
                    OnNavigationPropertyChanged("PROD_UNID_MED");
                }
            }
        }
        private PROD_UNID_MED _pROD_UNID_MED;
    
        [DataMember]
        public PRODUTO PRODUTO
        {
            get { return _pRODUTO; }
            set
            {
                if (!ReferenceEquals(_pRODUTO, value))
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
                    var previousValue = _pRODUTO;
                    _pRODUTO = value;
                    FixupPRODUTO(previousValue);
                    OnNavigationPropertyChanged("PRODUTO");
                }
            }
        }
        private PRODUTO _pRODUTO;

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
            PLAN_PRODUC = null;
            PLAN_PRODUC_FIC_TEC.Clear();
            PROD_UNID_MED = null;
            PRODUTO = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupPLAN_PRODUC(PLAN_PRODUC previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ITEM_PLAN_PRODUC.Contains(this))
            {
                previousValue.ITEM_PLAN_PRODUC.Remove(this);
            }
    
            if (PLAN_PRODUC != null)
            {
                if (!PLAN_PRODUC.ITEM_PLAN_PRODUC.Contains(this))
                {
                    PLAN_PRODUC.ITEM_PLAN_PRODUC.Add(this);
                }
    
                EmpCod = PLAN_PRODUC.EmpCod;
                PlanProducNum = PLAN_PRODUC.PlanProducNum;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PLAN_PRODUC")
                    && (ChangeTracker.OriginalValues["PLAN_PRODUC"] == PLAN_PRODUC))
                {
                    ChangeTracker.OriginalValues.Remove("PLAN_PRODUC");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PLAN_PRODUC", previousValue);
                }
                if (PLAN_PRODUC != null && !PLAN_PRODUC.ChangeTracker.ChangeTrackingEnabled)
                {
                    PLAN_PRODUC.StartTracking();
                }
            }
        }
    
        private void FixupPROD_UNID_MED(PROD_UNID_MED previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ITEM_PLAN_PRODUC.Contains(this))
            {
                previousValue.ITEM_PLAN_PRODUC.Remove(this);
            }
    
            if (PROD_UNID_MED != null)
            {
                if (!PROD_UNID_MED.ITEM_PLAN_PRODUC.Contains(this))
                {
                    PROD_UNID_MED.ITEM_PLAN_PRODUC.Add(this);
                }
    
                ProdCodEstr = PROD_UNID_MED.ProdCodEstr;
                ItPlanProducUnidMedCod = PROD_UNID_MED.ProdUnidMedCod;
                ItPlanProducUnidMedPos = PROD_UNID_MED.ProdUnidMedPos;
            }
            else if (!skipKeys)
            {
                ItPlanProducUnidMedCod = null;
                ItPlanProducUnidMedPos = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PROD_UNID_MED")
                    && (ChangeTracker.OriginalValues["PROD_UNID_MED"] == PROD_UNID_MED))
                {
                    ChangeTracker.OriginalValues.Remove("PROD_UNID_MED");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PROD_UNID_MED", previousValue);
                }
                if (PROD_UNID_MED != null && !PROD_UNID_MED.ChangeTracker.ChangeTrackingEnabled)
                {
                    PROD_UNID_MED.StartTracking();
                }
            }
        }
    
        private void FixupPRODUTO(PRODUTO previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ITEM_PLAN_PRODUC.Contains(this))
            {
                previousValue.ITEM_PLAN_PRODUC.Remove(this);
            }
    
            if (PRODUTO != null)
            {
                if (!PRODUTO.ITEM_PLAN_PRODUC.Contains(this))
                {
                    PRODUTO.ITEM_PLAN_PRODUC.Add(this);
                }
    
                ProdCodEstr = PRODUTO.ProdCodEstr;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PRODUTO")
                    && (ChangeTracker.OriginalValues["PRODUTO"] == PRODUTO))
                {
                    ChangeTracker.OriginalValues.Remove("PRODUTO");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PRODUTO", previousValue);
                }
                if (PRODUTO != null && !PRODUTO.ChangeTracker.ChangeTrackingEnabled)
                {
                    PRODUTO.StartTracking();
                }
            }
        }
    
        private void FixupPLAN_PRODUC_FIC_TEC(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PLAN_PRODUC_FIC_TEC item in e.NewItems)
                {
                    item.ITEM_PLAN_PRODUC = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PLAN_PRODUC_FIC_TEC", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PLAN_PRODUC_FIC_TEC item in e.OldItems)
                {
                    if (ReferenceEquals(item.ITEM_PLAN_PRODUC, this))
                    {
                        item.ITEM_PLAN_PRODUC = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PLAN_PRODUC_FIC_TEC", item);
                        // Delete the dependent end of this identifying association. If the current state is Added,
                        // allow the relationship to be changed without causing the dependent to be deleted.
                        if (item.ChangeTracker.State != ObjectState.Added)
                        {
                            item.MarkAsDeleted();
                        }
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }

        #endregion

    }
}