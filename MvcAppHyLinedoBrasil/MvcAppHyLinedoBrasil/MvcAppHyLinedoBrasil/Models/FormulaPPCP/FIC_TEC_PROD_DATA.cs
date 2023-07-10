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

namespace MvcAppHyLinedoBrasil.Models.FormulaPPCP
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(FIC_TEC_PROD))]
    public partial class FIC_TEC_PROD_DATA: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
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
                        if (FIC_TEC_PROD != null && FIC_TEC_PROD.ProdCodEstr != value)
                        {
                            FIC_TEC_PROD = null;
                        }
                    }
                    _prodCodEstr = value;
                    OnPropertyChanged("ProdCodEstr");
                }
            }
        }
        private string _prodCodEstr;
    
        [DataMember]
        public int FicTecProdSeq
        {
            get { return _ficTecProdSeq; }
            set
            {
                if (_ficTecProdSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'FicTecProdSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (FIC_TEC_PROD != null && FIC_TEC_PROD.FicTecProdSeq != value)
                        {
                            FIC_TEC_PROD = null;
                        }
                    }
                    _ficTecProdSeq = value;
                    OnPropertyChanged("FicTecProdSeq");
                }
            }
        }
        private int _ficTecProdSeq;
    
        [DataMember]
        public int FicTecProdDataSeq
        {
            get { return _ficTecProdDataSeq; }
            set
            {
                if (_ficTecProdDataSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'FicTecProdDataSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _ficTecProdDataSeq = value;
                    OnPropertyChanged("FicTecProdDataSeq");
                }
            }
        }
        private int _ficTecProdDataSeq;
    
        [DataMember]
        public Nullable<System.DateTime> FicTecProdDataInicio
        {
            get { return _ficTecProdDataInicio; }
            set
            {
                if (_ficTecProdDataInicio != value)
                {
                    _ficTecProdDataInicio = value;
                    OnPropertyChanged("FicTecProdDataInicio");
                }
            }
        }
        private Nullable<System.DateTime> _ficTecProdDataInicio;
    
        [DataMember]
        public Nullable<System.DateTime> FicTecProdDataFinal
        {
            get { return _ficTecProdDataFinal; }
            set
            {
                if (_ficTecProdDataFinal != value)
                {
                    _ficTecProdDataFinal = value;
                    OnPropertyChanged("FicTecProdDataFinal");
                }
            }
        }
        private Nullable<System.DateTime> _ficTecProdDataFinal;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataQtd
        {
            get { return _ficTecProdDataQtd; }
            set
            {
                if (_ficTecProdDataQtd != value)
                {
                    _ficTecProdDataQtd = value;
                    OnPropertyChanged("FicTecProdDataQtd");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataQtd;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataPerc
        {
            get { return _ficTecProdDataPerc; }
            set
            {
                if (_ficTecProdDataPerc != value)
                {
                    _ficTecProdDataPerc = value;
                    OnPropertyChanged("FicTecProdDataPerc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataPerc;
    
        [DataMember]
        public string FicTecProdDataPercTipo
        {
            get { return _ficTecProdDataPercTipo; }
            set
            {
                if (_ficTecProdDataPercTipo != value)
                {
                    _ficTecProdDataPercTipo = value;
                    OnPropertyChanged("FicTecProdDataPercTipo");
                }
            }
        }
        private string _ficTecProdDataPercTipo;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataQtdCalc
        {
            get { return _ficTecProdDataQtdCalc; }
            set
            {
                if (_ficTecProdDataQtdCalc != value)
                {
                    _ficTecProdDataQtdCalc = value;
                    OnPropertyChanged("FicTecProdDataQtdCalc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataQtdCalc;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataCustoPerc
        {
            get { return _ficTecProdDataCustoPerc; }
            set
            {
                if (_ficTecProdDataCustoPerc != value)
                {
                    _ficTecProdDataCustoPerc = value;
                    OnPropertyChanged("FicTecProdDataCustoPerc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataCustoPerc;
    
        [DataMember]
        public string FicTecProdDataCustoPercTipo
        {
            get { return _ficTecProdDataCustoPercTipo; }
            set
            {
                if (_ficTecProdDataCustoPercTipo != value)
                {
                    _ficTecProdDataCustoPercTipo = value;
                    OnPropertyChanged("FicTecProdDataCustoPercTipo");
                }
            }
        }
        private string _ficTecProdDataCustoPercTipo;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataCustoQtd
        {
            get { return _ficTecProdDataCustoQtd; }
            set
            {
                if (_ficTecProdDataCustoQtd != value)
                {
                    _ficTecProdDataCustoQtd = value;
                    OnPropertyChanged("FicTecProdDataCustoQtd");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataCustoQtd;
    
        [DataMember]
        public string FicTecProdDataCompCusto
        {
            get { return _ficTecProdDataCompCusto; }
            set
            {
                if (_ficTecProdDataCompCusto != value)
                {
                    _ficTecProdDataCompCusto = value;
                    OnPropertyChanged("FicTecProdDataCompCusto");
                }
            }
        }
        private string _ficTecProdDataCompCusto;
    
        [DataMember]
        public string FicTecProdDataGeraOP
        {
            get { return _ficTecProdDataGeraOP; }
            set
            {
                if (_ficTecProdDataGeraOP != value)
                {
                    _ficTecProdDataGeraOP = value;
                    OnPropertyChanged("FicTecProdDataGeraOP");
                }
            }
        }
        private string _ficTecProdDataGeraOP;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataPercPartic
        {
            get { return _ficTecProdDataPercPartic; }
            set
            {
                if (_ficTecProdDataPercPartic != value)
                {
                    _ficTecProdDataPercPartic = value;
                    OnPropertyChanged("FicTecProdDataPercPartic");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataPercPartic;
    
        [DataMember]
        public string FicTecProdDataPartVenda
        {
            get { return _ficTecProdDataPartVenda; }
            set
            {
                if (_ficTecProdDataPartVenda != value)
                {
                    _ficTecProdDataPartVenda = value;
                    OnPropertyChanged("FicTecProdDataPartVenda");
                }
            }
        }
        private string _ficTecProdDataPartVenda;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataPartVenPcDesc
        {
            get { return _ficTecProdDataPartVenPcDesc; }
            set
            {
                if (_ficTecProdDataPartVenPcDesc != value)
                {
                    _ficTecProdDataPartVenPcDesc = value;
                    OnPropertyChanged("FicTecProdDataPartVenPcDesc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataPartVenPcDesc;
    
        [DataMember]
        public Nullable<decimal> FicTecProdDataPartVenPcAcresc
        {
            get { return _ficTecProdDataPartVenPcAcresc; }
            set
            {
                if (_ficTecProdDataPartVenPcAcresc != value)
                {
                    _ficTecProdDataPartVenPcAcresc = value;
                    OnPropertyChanged("FicTecProdDataPartVenPcAcresc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdDataPartVenPcAcresc;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public FIC_TEC_PROD FIC_TEC_PROD
        {
            get { return _fIC_TEC_PROD; }
            set
            {
                if (!ReferenceEquals(_fIC_TEC_PROD, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (ProdCodEstr != value.ProdCodEstr || FicTecProdSeq != value.FicTecProdSeq)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _fIC_TEC_PROD;
                    _fIC_TEC_PROD = value;
                    FixupFIC_TEC_PROD(previousValue);
                    OnNavigationPropertyChanged("FIC_TEC_PROD");
                }
            }
        }
        private FIC_TEC_PROD _fIC_TEC_PROD;

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
            FIC_TEC_PROD = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupFIC_TEC_PROD(FIC_TEC_PROD previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.FIC_TEC_PROD_DATA.Contains(this))
            {
                previousValue.FIC_TEC_PROD_DATA.Remove(this);
            }
    
            if (FIC_TEC_PROD != null)
            {
                if (!FIC_TEC_PROD.FIC_TEC_PROD_DATA.Contains(this))
                {
                    FIC_TEC_PROD.FIC_TEC_PROD_DATA.Add(this);
                }
    
                ProdCodEstr = FIC_TEC_PROD.ProdCodEstr;
                FicTecProdSeq = FIC_TEC_PROD.FicTecProdSeq;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("FIC_TEC_PROD")
                    && (ChangeTracker.OriginalValues["FIC_TEC_PROD"] == FIC_TEC_PROD))
                {
                    ChangeTracker.OriginalValues.Remove("FIC_TEC_PROD");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("FIC_TEC_PROD", previousValue);
                }
                if (FIC_TEC_PROD != null && !FIC_TEC_PROD.ChangeTracker.ChangeTrackingEnabled)
                {
                    FIC_TEC_PROD.StartTracking();
                }
            }
        }

        #endregion

    }
}