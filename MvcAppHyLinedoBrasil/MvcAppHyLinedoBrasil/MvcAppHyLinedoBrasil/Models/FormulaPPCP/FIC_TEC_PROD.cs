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
    [KnownType(typeof(FIC_TEC_PROD_DATA))]
    [KnownType(typeof(PROD_UNID_MED1))]
    [KnownType(typeof(PRODUTO))]
    [KnownType(typeof(PRODUTO1))]
    public partial class FIC_TEC_PROD: IObjectWithChangeTracker, INotifyPropertyChanged
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
                    _ficTecProdSeq = value;
                    OnPropertyChanged("FicTecProdSeq");
                }
            }
        }
        private int _ficTecProdSeq;
    
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
        public string FicTecProdCodEstr
        {
            get { return _ficTecProdCodEstr; }
            set
            {
                if (_ficTecProdCodEstr != value)
                {
                    ChangeTracker.RecordOriginalValue("FicTecProdCodEstr", _ficTecProdCodEstr);
                    if (!IsDeserializing)
                    {
                        if (PROD_UNID_MED != null && PROD_UNID_MED.ProdCodEstr != value)
                        {
                            var previousValue = _pROD_UNID_MED;
                            _pROD_UNID_MED = null;
                            FixupPROD_UNID_MED(previousValue, skipKeys: true);
                            OnNavigationPropertyChanged("PROD_UNID_MED");
                        }
                        if (PRODUTO2 != null && PRODUTO2.ProdCodEstr != value)
                        {
                            PRODUTO2 = null;
                        }
                    }
                    _ficTecProdCodEstr = value;
                    OnPropertyChanged("FicTecProdCodEstr");
                }
            }
        }
        private string _ficTecProdCodEstr;
    
        [DataMember]
        public string FicTecProdGradeCorCod
        {
            get { return _ficTecProdGradeCorCod; }
            set
            {
                if (_ficTecProdGradeCorCod != value)
                {
                    _ficTecProdGradeCorCod = value;
                    OnPropertyChanged("FicTecProdGradeCorCod");
                }
            }
        }
        private string _ficTecProdGradeCorCod;
    
        [DataMember]
        public Nullable<decimal> FicTecProdQtd
        {
            get { return _ficTecProdQtd; }
            set
            {
                if (_ficTecProdQtd != value)
                {
                    _ficTecProdQtd = value;
                    OnPropertyChanged("FicTecProdQtd");
                }
            }
        }
        private Nullable<decimal> _ficTecProdQtd;
    
        [DataMember]
        public Nullable<decimal> FicTecProdPerc
        {
            get { return _ficTecProdPerc; }
            set
            {
                if (_ficTecProdPerc != value)
                {
                    _ficTecProdPerc = value;
                    OnPropertyChanged("FicTecProdPerc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdPerc;
    
        [DataMember]
        public string FicTecProdPercTipo
        {
            get { return _ficTecProdPercTipo; }
            set
            {
                if (_ficTecProdPercTipo != value)
                {
                    _ficTecProdPercTipo = value;
                    OnPropertyChanged("FicTecProdPercTipo");
                }
            }
        }
        private string _ficTecProdPercTipo;
    
        [DataMember]
        public Nullable<decimal> FicTecProdQtdCalc
        {
            get { return _ficTecProdQtdCalc; }
            set
            {
                if (_ficTecProdQtdCalc != value)
                {
                    _ficTecProdQtdCalc = value;
                    OnPropertyChanged("FicTecProdQtdCalc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdQtdCalc;
    
        [DataMember]
        public Nullable<decimal> FicTecProdCustoPerc
        {
            get { return _ficTecProdCustoPerc; }
            set
            {
                if (_ficTecProdCustoPerc != value)
                {
                    _ficTecProdCustoPerc = value;
                    OnPropertyChanged("FicTecProdCustoPerc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdCustoPerc;
    
        [DataMember]
        public string FicTecProdCustoPercTipo
        {
            get { return _ficTecProdCustoPercTipo; }
            set
            {
                if (_ficTecProdCustoPercTipo != value)
                {
                    _ficTecProdCustoPercTipo = value;
                    OnPropertyChanged("FicTecProdCustoPercTipo");
                }
            }
        }
        private string _ficTecProdCustoPercTipo;
    
        [DataMember]
        public Nullable<decimal> FicTecProdCustoQtd
        {
            get { return _ficTecProdCustoQtd; }
            set
            {
                if (_ficTecProdCustoQtd != value)
                {
                    _ficTecProdCustoQtd = value;
                    OnPropertyChanged("FicTecProdCustoQtd");
                }
            }
        }
        private Nullable<decimal> _ficTecProdCustoQtd;
    
        [DataMember]
        public Nullable<System.DateTime> FicTecProdDataInic
        {
            get { return _ficTecProdDataInic; }
            set
            {
                if (_ficTecProdDataInic != value)
                {
                    _ficTecProdDataInic = value;
                    OnPropertyChanged("FicTecProdDataInic");
                }
            }
        }
        private Nullable<System.DateTime> _ficTecProdDataInic;
    
        [DataMember]
        public Nullable<System.DateTime> FicTecProdDataFim
        {
            get { return _ficTecProdDataFim; }
            set
            {
                if (_ficTecProdDataFim != value)
                {
                    _ficTecProdDataFim = value;
                    OnPropertyChanged("FicTecProdDataFim");
                }
            }
        }
        private Nullable<System.DateTime> _ficTecProdDataFim;
    
        [DataMember]
        public string FicTecProdCompCusto
        {
            get { return _ficTecProdCompCusto; }
            set
            {
                if (_ficTecProdCompCusto != value)
                {
                    _ficTecProdCompCusto = value;
                    OnPropertyChanged("FicTecProdCompCusto");
                }
            }
        }
        private string _ficTecProdCompCusto;
    
        [DataMember]
        public string FicTecProdGeraOP
        {
            get { return _ficTecProdGeraOP; }
            set
            {
                if (_ficTecProdGeraOP != value)
                {
                    _ficTecProdGeraOP = value;
                    OnPropertyChanged("FicTecProdGeraOP");
                }
            }
        }
        private string _ficTecProdGeraOP;
    
        [DataMember]
        public Nullable<decimal> FicTecProdPercPartic
        {
            get { return _ficTecProdPercPartic; }
            set
            {
                if (_ficTecProdPercPartic != value)
                {
                    _ficTecProdPercPartic = value;
                    OnPropertyChanged("FicTecProdPercPartic");
                }
            }
        }
        private Nullable<decimal> _ficTecProdPercPartic;
    
        [DataMember]
        public string FicTecProdPartVenda
        {
            get { return _ficTecProdPartVenda; }
            set
            {
                if (_ficTecProdPartVenda != value)
                {
                    _ficTecProdPartVenda = value;
                    OnPropertyChanged("FicTecProdPartVenda");
                }
            }
        }
        private string _ficTecProdPartVenda;
    
        [DataMember]
        public Nullable<decimal> FicTecProdPartVendaPercDesc
        {
            get { return _ficTecProdPartVendaPercDesc; }
            set
            {
                if (_ficTecProdPartVendaPercDesc != value)
                {
                    _ficTecProdPartVendaPercDesc = value;
                    OnPropertyChanged("FicTecProdPartVendaPercDesc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdPartVendaPercDesc;
    
        [DataMember]
        public Nullable<decimal> FicTecProdPartVendaPercAcresc
        {
            get { return _ficTecProdPartVendaPercAcresc; }
            set
            {
                if (_ficTecProdPartVendaPercAcresc != value)
                {
                    _ficTecProdPartVendaPercAcresc = value;
                    OnPropertyChanged("FicTecProdPartVendaPercAcresc");
                }
            }
        }
        private Nullable<decimal> _ficTecProdPartVendaPercAcresc;
    
        [DataMember]
        public string FicTecProdUnidMedCodDig
        {
            get { return _ficTecProdUnidMedCodDig; }
            set
            {
                if (_ficTecProdUnidMedCodDig != value)
                {
                    ChangeTracker.RecordOriginalValue("FicTecProdUnidMedCodDig", _ficTecProdUnidMedCodDig);
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
                    _ficTecProdUnidMedCodDig = value;
                    OnPropertyChanged("FicTecProdUnidMedCodDig");
                }
            }
        }
        private string _ficTecProdUnidMedCodDig;
    
        [DataMember]
        public Nullable<short> FicTecProdUnidMedPosDig
        {
            get { return _ficTecProdUnidMedPosDig; }
            set
            {
                if (_ficTecProdUnidMedPosDig != value)
                {
                    ChangeTracker.RecordOriginalValue("FicTecProdUnidMedPosDig", _ficTecProdUnidMedPosDig);
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
                    _ficTecProdUnidMedPosDig = value;
                    OnPropertyChanged("FicTecProdUnidMedPosDig");
                }
            }
        }
        private Nullable<short> _ficTecProdUnidMedPosDig;
    
        [DataMember]
        public Nullable<decimal> FicTecProdUnidMedQtdDig
        {
            get { return _ficTecProdUnidMedQtdDig; }
            set
            {
                if (_ficTecProdUnidMedQtdDig != value)
                {
                    _ficTecProdUnidMedQtdDig = value;
                    OnPropertyChanged("FicTecProdUnidMedQtdDig");
                }
            }
        }
        private Nullable<decimal> _ficTecProdUnidMedQtdDig;
    
        [DataMember]
        public string FicTecProdAcessorio
        {
            get { return _ficTecProdAcessorio; }
            set
            {
                if (_ficTecProdAcessorio != value)
                {
                    _ficTecProdAcessorio = value;
                    OnPropertyChanged("FicTecProdAcessorio");
                }
            }
        }
        private string _ficTecProdAcessorio;
    
        [DataMember]
        public Nullable<short> FicTecProdPartVendaUnidMedPos
        {
            get { return _ficTecProdPartVendaUnidMedPos; }
            set
            {
                if (_ficTecProdPartVendaUnidMedPos != value)
                {
                    _ficTecProdPartVendaUnidMedPos = value;
                    OnPropertyChanged("FicTecProdPartVendaUnidMedPos");
                }
            }
        }
        private Nullable<short> _ficTecProdPartVendaUnidMedPos;
    
        [DataMember]
        public string FicTecProdDiscOpt
        {
            get { return _ficTecProdDiscOpt; }
            set
            {
                if (_ficTecProdDiscOpt != value)
                {
                    _ficTecProdDiscOpt = value;
                    OnPropertyChanged("FicTecProdDiscOpt");
                }
            }
        }
        private string _ficTecProdDiscOpt;
    
        [DataMember]
        public Nullable<decimal> FicTecProdPercRatDif
        {
            get { return _ficTecProdPercRatDif; }
            set
            {
                if (_ficTecProdPercRatDif != value)
                {
                    _ficTecProdPercRatDif = value;
                    OnPropertyChanged("FicTecProdPercRatDif");
                }
            }
        }
        private Nullable<decimal> _ficTecProdPercRatDif;
    
        [DataMember]
        public string FicTecProdFormula
        {
            get { return _ficTecProdFormula; }
            set
            {
                if (_ficTecProdFormula != value)
                {
                    _ficTecProdFormula = value;
                    OnPropertyChanged("FicTecProdFormula");
                }
            }
        }
        private string _ficTecProdFormula;
    
        [DataMember]
        public string FicTecProdTerc
        {
            get { return _ficTecProdTerc; }
            set
            {
                if (_ficTecProdTerc != value)
                {
                    _ficTecProdTerc = value;
                    OnPropertyChanged("FicTecProdTerc");
                }
            }
        }
        private string _ficTecProdTerc;
    
        [DataMember]
        public Nullable<int> FicTecProdNumSeq
        {
            get { return _ficTecProdNumSeq; }
            set
            {
                if (_ficTecProdNumSeq != value)
                {
                    _ficTecProdNumSeq = value;
                    OnPropertyChanged("FicTecProdNumSeq");
                }
            }
        }
        private Nullable<int> _ficTecProdNumSeq;
    
        [DataMember]
        public string FicTecProdSubstPor
        {
            get { return _ficTecProdSubstPor; }
            set
            {
                if (_ficTecProdSubstPor != value)
                {
                    ChangeTracker.RecordOriginalValue("FicTecProdSubstPor", _ficTecProdSubstPor);
                    if (!IsDeserializing)
                    {
                        if (PRODUTO != null && PRODUTO.ProdCodEstr != value)
                        {
                            PRODUTO = null;
                        }
                    }
                    _ficTecProdSubstPor = value;
                    OnPropertyChanged("FicTecProdSubstPor");
                }
            }
        }
        private string _ficTecProdSubstPor;
    
        [DataMember]
        public Nullable<int> FicTecProdSubstPorSeq
        {
            get { return _ficTecProdSubstPorSeq; }
            set
            {
                if (_ficTecProdSubstPorSeq != value)
                {
                    _ficTecProdSubstPorSeq = value;
                    OnPropertyChanged("FicTecProdSubstPorSeq");
                }
            }
        }
        private Nullable<int> _ficTecProdSubstPorSeq;
    
        [DataMember]
        public string FicTecProdGeraNFRetorno
        {
            get { return _ficTecProdGeraNFRetorno; }
            set
            {
                if (_ficTecProdGeraNFRetorno != value)
                {
                    _ficTecProdGeraNFRetorno = value;
                    OnPropertyChanged("FicTecProdGeraNFRetorno");
                }
            }
        }
        private string _ficTecProdGeraNFRetorno;
    
        [DataMember]
        public string FicTecProdOperCalculo
        {
            get { return _ficTecProdOperCalculo; }
            set
            {
                if (_ficTecProdOperCalculo != value)
                {
                    _ficTecProdOperCalculo = value;
                    OnPropertyChanged("FicTecProdOperCalculo");
                }
            }
        }
        private string _ficTecProdOperCalculo;
    
        [DataMember]
        public string fictecprodcalccustovalinsumo
        {
            get { return _fictecprodcalccustovalinsumo; }
            set
            {
                if (_fictecprodcalccustovalinsumo != value)
                {
                    _fictecprodcalccustovalinsumo = value;
                    OnPropertyChanged("fictecprodcalccustovalinsumo");
                }
            }
        }
        private string _fictecprodcalccustovalinsumo;
    
        [DataMember]
        public string FicTecProdCompCustoAnaFinanc
        {
            get { return _ficTecProdCompCustoAnaFinanc; }
            set
            {
                if (_ficTecProdCompCustoAnaFinanc != value)
                {
                    _ficTecProdCompCustoAnaFinanc = value;
                    OnPropertyChanged("FicTecProdCompCustoAnaFinanc");
                }
            }
        }
        private string _ficTecProdCompCustoAnaFinanc;
    
        [DataMember]
        public string FicTecProdBloqueada
        {
            get { return _ficTecProdBloqueada; }
            set
            {
                if (_ficTecProdBloqueada != value)
                {
                    _ficTecProdBloqueada = value;
                    OnPropertyChanged("FicTecProdBloqueada");
                }
            }
        }
        private string _ficTecProdBloqueada;
    
        [DataMember]
        public string FicTecProdParCalcLoteApon
        {
            get { return _ficTecProdParCalcLoteApon; }
            set
            {
                if (_ficTecProdParCalcLoteApon != value)
                {
                    _ficTecProdParCalcLoteApon = value;
                    OnPropertyChanged("FicTecProdParCalcLoteApon");
                }
            }
        }
        private string _ficTecProdParCalcLoteApon;
    
        [DataMember]
        public string FicTecProdCondPesoBrtAtend
        {
            get { return _ficTecProdCondPesoBrtAtend; }
            set
            {
                if (_ficTecProdCondPesoBrtAtend != value)
                {
                    _ficTecProdCondPesoBrtAtend = value;
                    OnPropertyChanged("FicTecProdCondPesoBrtAtend");
                }
            }
        }
        private string _ficTecProdCondPesoBrtAtend;
    
        [DataMember]
        public string FicTecProdBaseNumLote
        {
            get { return _ficTecProdBaseNumLote; }
            set
            {
                if (_ficTecProdBaseNumLote != value)
                {
                    _ficTecProdBaseNumLote = value;
                    OnPropertyChanged("FicTecProdBaseNumLote");
                }
            }
        }
        private string _ficTecProdBaseNumLote;
    
        [DataMember]
        public string FicTecProdImpEtiq
        {
            get { return _ficTecProdImpEtiq; }
            set
            {
                if (_ficTecProdImpEtiq != value)
                {
                    _ficTecProdImpEtiq = value;
                    OnPropertyChanged("FicTecProdImpEtiq");
                }
            }
        }
        private string _ficTecProdImpEtiq;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<FIC_TEC_PROD_DATA> FIC_TEC_PROD_DATA
        {
            get
            {
                if (_fIC_TEC_PROD_DATA == null)
                {
                    _fIC_TEC_PROD_DATA = new TrackableCollection<FIC_TEC_PROD_DATA>();
                    _fIC_TEC_PROD_DATA.CollectionChanged += FixupFIC_TEC_PROD_DATA;
                }
                return _fIC_TEC_PROD_DATA;
            }
            set
            {
                if (!ReferenceEquals(_fIC_TEC_PROD_DATA, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_fIC_TEC_PROD_DATA != null)
                    {
                        _fIC_TEC_PROD_DATA.CollectionChanged -= FixupFIC_TEC_PROD_DATA;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (FIC_TEC_PROD_DATA item in _fIC_TEC_PROD_DATA)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _fIC_TEC_PROD_DATA = value;
                    if (_fIC_TEC_PROD_DATA != null)
                    {
                        _fIC_TEC_PROD_DATA.CollectionChanged += FixupFIC_TEC_PROD_DATA;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (FIC_TEC_PROD_DATA item in _fIC_TEC_PROD_DATA)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("FIC_TEC_PROD_DATA");
                }
            }
        }
        private TrackableCollection<FIC_TEC_PROD_DATA> _fIC_TEC_PROD_DATA;
    
        [DataMember]
        public PROD_UNID_MED1 PROD_UNID_MED
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
        private PROD_UNID_MED1 _pROD_UNID_MED;
    
        [DataMember]
        public PRODUTO PRODUTO
        {
            get { return _pRODUTO; }
            set
            {
                if (!ReferenceEquals(_pRODUTO, value))
                {
                    var previousValue = _pRODUTO;
                    _pRODUTO = value;
                    FixupPRODUTO(previousValue);
                    OnNavigationPropertyChanged("PRODUTO");
                }
            }
        }
        private PRODUTO _pRODUTO;
    
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
    
        [DataMember]
        public PRODUTO PRODUTO2
        {
            get { return _pRODUTO2; }
            set
            {
                if (!ReferenceEquals(_pRODUTO2, value))
                {
                    var previousValue = _pRODUTO2;
                    _pRODUTO2 = value;
                    FixupPRODUTO2(previousValue);
                    OnNavigationPropertyChanged("PRODUTO2");
                }
            }
        }
        private PRODUTO _pRODUTO2;

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
            FIC_TEC_PROD_DATA.Clear();
            PROD_UNID_MED = null;
            PRODUTO = null;
            PRODUTO1 = null;
            PRODUTO2 = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupPROD_UNID_MED(PROD_UNID_MED1 previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.FIC_TEC_PROD.Contains(this))
            {
                previousValue.FIC_TEC_PROD.Remove(this);
            }
    
            if (PROD_UNID_MED != null)
            {
                if (!PROD_UNID_MED.FIC_TEC_PROD.Contains(this))
                {
                    PROD_UNID_MED.FIC_TEC_PROD.Add(this);
                }
    
                FicTecProdCodEstr = PROD_UNID_MED.ProdCodEstr;
                FicTecProdUnidMedCodDig = PROD_UNID_MED.ProdUnidMedCod;
                FicTecProdUnidMedPosDig = PROD_UNID_MED.ProdUnidMedPos;
            }
            else if (!skipKeys)
            {
                FicTecProdUnidMedCodDig = null;
                FicTecProdUnidMedPosDig = null;
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
    
        private void FixupPRODUTO(PRODUTO previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.FIC_TEC_PROD.Contains(this))
            {
                previousValue.FIC_TEC_PROD.Remove(this);
            }
    
            if (PRODUTO != null)
            {
                if (!PRODUTO.FIC_TEC_PROD.Contains(this))
                {
                    PRODUTO.FIC_TEC_PROD.Add(this);
                }
    
                FicTecProdSubstPor = PRODUTO.ProdCodEstr;
            }
            else if (!skipKeys)
            {
                FicTecProdSubstPor = null;
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
    
        private void FixupPRODUTO1(PRODUTO1 previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.FIC_TEC_PROD.Contains(this))
            {
                previousValue.FIC_TEC_PROD.Remove(this);
            }
    
            if (PRODUTO1 != null)
            {
                if (!PRODUTO1.FIC_TEC_PROD.Contains(this))
                {
                    PRODUTO1.FIC_TEC_PROD.Add(this);
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
    
        private void FixupPRODUTO2(PRODUTO previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.FIC_TEC_PROD1.Contains(this))
            {
                previousValue.FIC_TEC_PROD1.Remove(this);
            }
    
            if (PRODUTO2 != null)
            {
                if (!PRODUTO2.FIC_TEC_PROD1.Contains(this))
                {
                    PRODUTO2.FIC_TEC_PROD1.Add(this);
                }
    
                FicTecProdCodEstr = PRODUTO2.ProdCodEstr;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("PRODUTO2")
                    && (ChangeTracker.OriginalValues["PRODUTO2"] == PRODUTO2))
                {
                    ChangeTracker.OriginalValues.Remove("PRODUTO2");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("PRODUTO2", previousValue);
                }
                if (PRODUTO2 != null && !PRODUTO2.ChangeTracker.ChangeTrackingEnabled)
                {
                    PRODUTO2.StartTracking();
                }
            }
        }
    
        private void FixupFIC_TEC_PROD_DATA(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (FIC_TEC_PROD_DATA item in e.NewItems)
                {
                    item.FIC_TEC_PROD = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("FIC_TEC_PROD_DATA", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (FIC_TEC_PROD_DATA item in e.OldItems)
                {
                    if (ReferenceEquals(item.FIC_TEC_PROD, this))
                    {
                        item.FIC_TEC_PROD = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("FIC_TEC_PROD_DATA", item);
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