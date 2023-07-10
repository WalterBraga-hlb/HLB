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
    [KnownType(typeof(ITEM_COB_BANC))]
    [KnownType(typeof(EMPRESA_FILIAL))]
    public partial class COB_BANC: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string BcoNum
        {
            get { return _bcoNum; }
            set
            {
                if (_bcoNum != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'BcoNum' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _bcoNum = value;
                    OnPropertyChanged("BcoNum");
                }
            }
        }
        private string _bcoNum;
    
        [DataMember]
        public string CobBancTipo
        {
            get { return _cobBancTipo; }
            set
            {
                if (_cobBancTipo != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'CobBancTipo' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _cobBancTipo = value;
                    OnPropertyChanged("CobBancTipo");
                }
            }
        }
        private string _cobBancTipo;
    
        [DataMember]
        public string CobBancRemNum
        {
            get { return _cobBancRemNum; }
            set
            {
                if (_cobBancRemNum != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'CobBancRemNum' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _cobBancRemNum = value;
                    OnPropertyChanged("CobBancRemNum");
                }
            }
        }
        private string _cobBancRemNum;
    
        [DataMember]
        public Nullable<System.DateTime> CobBancDataEmis
        {
            get { return _cobBancDataEmis; }
            set
            {
                if (_cobBancDataEmis != value)
                {
                    _cobBancDataEmis = value;
                    OnPropertyChanged("CobBancDataEmis");
                }
            }
        }
        private Nullable<System.DateTime> _cobBancDataEmis;
    
        [DataMember]
        public string AgNum
        {
            get { return _agNum; }
            set
            {
                if (_agNum != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'AgNum' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _agNum = value;
                    OnPropertyChanged("AgNum");
                }
            }
        }
        private string _agNum;
    
        [DataMember]
        public string MovCtrlBancNum
        {
            get { return _movCtrlBancNum; }
            set
            {
                if (_movCtrlBancNum != value)
                {
                    _movCtrlBancNum = value;
                    OnPropertyChanged("MovCtrlBancNum");
                }
            }
        }
        private string _movCtrlBancNum;
    
        [DataMember]
        public Nullable<decimal> CobBancTxaDescPerc
        {
            get { return _cobBancTxaDescPerc; }
            set
            {
                if (_cobBancTxaDescPerc != value)
                {
                    _cobBancTxaDescPerc = value;
                    OnPropertyChanged("CobBancTxaDescPerc");
                }
            }
        }
        private Nullable<decimal> _cobBancTxaDescPerc;
    
        [DataMember]
        public string CobBancTxaDescPer
        {
            get { return _cobBancTxaDescPer; }
            set
            {
                if (_cobBancTxaDescPer != value)
                {
                    _cobBancTxaDescPer = value;
                    OnPropertyChanged("CobBancTxaDescPer");
                }
            }
        }
        private string _cobBancTxaDescPer;
    
        [DataMember]
        public string CobBancRemNumAlt
        {
            get { return _cobBancRemNumAlt; }
            set
            {
                if (_cobBancRemNumAlt != value)
                {
                    _cobBancRemNumAlt = value;
                    OnPropertyChanged("CobBancRemNumAlt");
                }
            }
        }
        private string _cobBancRemNumAlt;
    
        [DataMember]
        public Nullable<decimal> CobBancValDesc
        {
            get { return _cobBancValDesc; }
            set
            {
                if (_cobBancValDesc != value)
                {
                    _cobBancValDesc = value;
                    OnPropertyChanged("CobBancValDesc");
                }
            }
        }
        private Nullable<decimal> _cobBancValDesc;
    
        [DataMember]
        public Nullable<decimal> CobBancPercJuros
        {
            get { return _cobBancPercJuros; }
            set
            {
                if (_cobBancPercJuros != value)
                {
                    _cobBancPercJuros = value;
                    OnPropertyChanged("CobBancPercJuros");
                }
            }
        }
        private Nullable<decimal> _cobBancPercJuros;
    
        [DataMember]
        public string CobBancPerJuros
        {
            get { return _cobBancPerJuros; }
            set
            {
                if (_cobBancPerJuros != value)
                {
                    _cobBancPerJuros = value;
                    OnPropertyChanged("CobBancPerJuros");
                }
            }
        }
        private string _cobBancPerJuros;
    
        [DataMember]
        public Nullable<decimal> CobBancValJuros
        {
            get { return _cobBancValJuros; }
            set
            {
                if (_cobBancValJuros != value)
                {
                    _cobBancValJuros = value;
                    OnPropertyChanged("CobBancValJuros");
                }
            }
        }
        private Nullable<decimal> _cobBancValJuros;
    
        [DataMember]
        public Nullable<decimal> CobBancTxaBoleto
        {
            get { return _cobBancTxaBoleto; }
            set
            {
                if (_cobBancTxaBoleto != value)
                {
                    _cobBancTxaBoleto = value;
                    OnPropertyChanged("CobBancTxaBoleto");
                }
            }
        }
        private Nullable<decimal> _cobBancTxaBoleto;
    
        [DataMember]
        public Nullable<decimal> CobBancValBoleto
        {
            get { return _cobBancValBoleto; }
            set
            {
                if (_cobBancValBoleto != value)
                {
                    _cobBancValBoleto = value;
                    OnPropertyChanged("CobBancValBoleto");
                }
            }
        }
        private Nullable<decimal> _cobBancValBoleto;
    
        [DataMember]
        public Nullable<decimal> CobBancValIof
        {
            get { return _cobBancValIof; }
            set
            {
                if (_cobBancValIof != value)
                {
                    _cobBancValIof = value;
                    OnPropertyChanged("CobBancValIof");
                }
            }
        }
        private Nullable<decimal> _cobBancValIof;
    
        [DataMember]
        public Nullable<decimal> CobBancValCpmf
        {
            get { return _cobBancValCpmf; }
            set
            {
                if (_cobBancValCpmf != value)
                {
                    _cobBancValCpmf = value;
                    OnPropertyChanged("CobBancValCpmf");
                }
            }
        }
        private Nullable<decimal> _cobBancValCpmf;
    
        [DataMember]
        public Nullable<decimal> CobBancValOrig
        {
            get { return _cobBancValOrig; }
            set
            {
                if (_cobBancValOrig != value)
                {
                    _cobBancValOrig = value;
                    OnPropertyChanged("CobBancValOrig");
                }
            }
        }
        private Nullable<decimal> _cobBancValOrig;
    
        [DataMember]
        public Nullable<decimal> CobBancValFinal
        {
            get { return _cobBancValFinal; }
            set
            {
                if (_cobBancValFinal != value)
                {
                    _cobBancValFinal = value;
                    OnPropertyChanged("CobBancValFinal");
                }
            }
        }
        private Nullable<decimal> _cobBancValFinal;
    
        [DataMember]
        public Nullable<decimal> CobBancPrazoMedio
        {
            get { return _cobBancPrazoMedio; }
            set
            {
                if (_cobBancPrazoMedio != value)
                {
                    _cobBancPrazoMedio = value;
                    OnPropertyChanged("CobBancPrazoMedio");
                }
            }
        }
        private Nullable<decimal> _cobBancPrazoMedio;
    
        [DataMember]
        public Nullable<decimal> CobBancValPrazoMedio
        {
            get { return _cobBancValPrazoMedio; }
            set
            {
                if (_cobBancValPrazoMedio != value)
                {
                    _cobBancValPrazoMedio = value;
                    OnPropertyChanged("CobBancValPrazoMedio");
                }
            }
        }
        private Nullable<decimal> _cobBancValPrazoMedio;
    
        [DataMember]
        public string USERModalidade
        {
            get { return _uSERModalidade; }
            set
            {
                if (_uSERModalidade != value)
                {
                    _uSERModalidade = value;
                    OnPropertyChanged("USERModalidade");
                }
            }
        }
        private string _uSERModalidade;
    
        [DataMember]
        public Nullable<System.DateTime> CobBancDataPag
        {
            get { return _cobBancDataPag; }
            set
            {
                if (_cobBancDataPag != value)
                {
                    _cobBancDataPag = value;
                    OnPropertyChanged("CobBancDataPag");
                }
            }
        }
        private Nullable<System.DateTime> _cobBancDataPag;
    
        [DataMember]
        public string ContaFinCod
        {
            get { return _contaFinCod; }
            set
            {
                if (_contaFinCod != value)
                {
                    _contaFinCod = value;
                    OnPropertyChanged("ContaFinCod");
                }
            }
        }
        private string _contaFinCod;
    
        [DataMember]
        public string CobBancPathArqRem
        {
            get { return _cobBancPathArqRem; }
            set
            {
                if (_cobBancPathArqRem != value)
                {
                    _cobBancPathArqRem = value;
                    OnPropertyChanged("CobBancPathArqRem");
                }
            }
        }
        private string _cobBancPathArqRem;
    
        [DataMember]
        public string CobBancPathArqRemGer
        {
            get { return _cobBancPathArqRemGer; }
            set
            {
                if (_cobBancPathArqRemGer != value)
                {
                    _cobBancPathArqRemGer = value;
                    OnPropertyChanged("CobBancPathArqRemGer");
                }
            }
        }
        private string _cobBancPathArqRemGer;
    
        [DataMember]
        public Nullable<System.DateTime> CobBancDataHoraArqRemGer
        {
            get { return _cobBancDataHoraArqRemGer; }
            set
            {
                if (_cobBancDataHoraArqRemGer != value)
                {
                    _cobBancDataHoraArqRemGer = value;
                    OnPropertyChanged("CobBancDataHoraArqRemGer");
                }
            }
        }
        private Nullable<System.DateTime> _cobBancDataHoraArqRemGer;
    
        [DataMember]
        public string TipoRemCobBancCod
        {
            get { return _tipoRemCobBancCod; }
            set
            {
                if (_tipoRemCobBancCod != value)
                {
                    _tipoRemCobBancCod = value;
                    OnPropertyChanged("TipoRemCobBancCod");
                }
            }
        }
        private string _tipoRemCobBancCod;
    
        [DataMember]
        public string ConfRemBancCod
        {
            get { return _confRemBancCod; }
            set
            {
                if (_confRemBancCod != value)
                {
                    _confRemBancCod = value;
                    OnPropertyChanged("ConfRemBancCod");
                }
            }
        }
        private string _confRemBancCod;
    
        [DataMember]
        public Nullable<short> CobBancArqRemGerSeq
        {
            get { return _cobBancArqRemGerSeq; }
            set
            {
                if (_cobBancArqRemGerSeq != value)
                {
                    _cobBancArqRemGerSeq = value;
                    OnPropertyChanged("CobBancArqRemGerSeq");
                }
            }
        }
        private Nullable<short> _cobBancArqRemGerSeq;
    
        [DataMember]
        public string TipoLancCod
        {
            get { return _tipoLancCod; }
            set
            {
                if (_tipoLancCod != value)
                {
                    _tipoLancCod = value;
                    OnPropertyChanged("TipoLancCod");
                }
            }
        }
        private string _tipoLancCod;
    
        [DataMember]
        public string CobBancTipoTaxa
        {
            get { return _cobBancTipoTaxa; }
            set
            {
                if (_cobBancTipoTaxa != value)
                {
                    _cobBancTipoTaxa = value;
                    OnPropertyChanged("CobBancTipoTaxa");
                }
            }
        }
        private string _cobBancTipoTaxa;
    
        [DataMember]
        public string CobBancTipoOrcam
        {
            get { return _cobBancTipoOrcam; }
            set
            {
                if (_cobBancTipoOrcam != value)
                {
                    _cobBancTipoOrcam = value;
                    OnPropertyChanged("CobBancTipoOrcam");
                }
            }
        }
        private string _cobBancTipoOrcam;
    
        [DataMember]
        public string CobBancNumTipoOrcam
        {
            get { return _cobBancNumTipoOrcam; }
            set
            {
                if (_cobBancNumTipoOrcam != value)
                {
                    _cobBancNumTipoOrcam = value;
                    OnPropertyChanged("CobBancNumTipoOrcam");
                }
            }
        }
        private string _cobBancNumTipoOrcam;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<ITEM_COB_BANC> ITEM_COB_BANC
        {
            get
            {
                if (_iTEM_COB_BANC == null)
                {
                    _iTEM_COB_BANC = new TrackableCollection<ITEM_COB_BANC>();
                    _iTEM_COB_BANC.CollectionChanged += FixupITEM_COB_BANC;
                }
                return _iTEM_COB_BANC;
            }
            set
            {
                if (!ReferenceEquals(_iTEM_COB_BANC, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_iTEM_COB_BANC != null)
                    {
                        _iTEM_COB_BANC.CollectionChanged -= FixupITEM_COB_BANC;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (ITEM_COB_BANC item in _iTEM_COB_BANC)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _iTEM_COB_BANC = value;
                    if (_iTEM_COB_BANC != null)
                    {
                        _iTEM_COB_BANC.CollectionChanged += FixupITEM_COB_BANC;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (ITEM_COB_BANC item in _iTEM_COB_BANC)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("ITEM_COB_BANC");
                }
            }
        }
        private TrackableCollection<ITEM_COB_BANC> _iTEM_COB_BANC;
    
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
            ITEM_COB_BANC.Clear();
            EMPRESA_FILIAL = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupEMPRESA_FILIAL(EMPRESA_FILIAL previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.COB_BANC.Contains(this))
            {
                previousValue.COB_BANC.Remove(this);
            }
    
            if (EMPRESA_FILIAL != null)
            {
                if (!EMPRESA_FILIAL.COB_BANC.Contains(this))
                {
                    EMPRESA_FILIAL.COB_BANC.Add(this);
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
    
        private void FixupITEM_COB_BANC(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ITEM_COB_BANC item in e.NewItems)
                {
                    item.COB_BANC = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ITEM_COB_BANC", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ITEM_COB_BANC item in e.OldItems)
                {
                    if (ReferenceEquals(item.COB_BANC, this))
                    {
                        item.COB_BANC = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ITEM_COB_BANC", item);
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