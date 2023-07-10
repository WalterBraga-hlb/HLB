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

namespace MvcAppHylinedoBrasilMobile.Models.bdApolo
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(EMPRESA_FILIAL))]
    [KnownType(typeof(PRODUTO1))]
    [KnownType(typeof(ENTIDADE1))]
    public partial class CTRL_LOTE: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string CtrlLoteNum
        {
            get { return _ctrlLoteNum; }
            set
            {
                if (_ctrlLoteNum != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'CtrlLoteNum' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _ctrlLoteNum = value;
                    OnPropertyChanged("CtrlLoteNum");
                }
            }
        }
        private string _ctrlLoteNum;
    
        [DataMember]
        public System.DateTime CtrlLoteDataValid
        {
            get { return _ctrlLoteDataValid; }
            set
            {
                if (_ctrlLoteDataValid != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'CtrlLoteDataValid' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _ctrlLoteDataValid = value;
                    OnPropertyChanged("CtrlLoteDataValid");
                }
            }
        }
        private System.DateTime _ctrlLoteDataValid;
    
        [DataMember]
        public Nullable<System.DateTime> CtrlLoteDataFab
        {
            get { return _ctrlLoteDataFab; }
            set
            {
                if (_ctrlLoteDataFab != value)
                {
                    _ctrlLoteDataFab = value;
                    OnPropertyChanged("CtrlLoteDataFab");
                }
            }
        }
        private Nullable<System.DateTime> _ctrlLoteDataFab;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteQtdSaldo
        {
            get { return _ctrlLoteQtdSaldo; }
            set
            {
                if (_ctrlLoteQtdSaldo != value)
                {
                    _ctrlLoteQtdSaldo = value;
                    OnPropertyChanged("CtrlLoteQtdSaldo");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteQtdSaldo;
    
        [DataMember]
        public string CtrlLoteUnidMedCod
        {
            get { return _ctrlLoteUnidMedCod; }
            set
            {
                if (_ctrlLoteUnidMedCod != value)
                {
                    _ctrlLoteUnidMedCod = value;
                    OnPropertyChanged("CtrlLoteUnidMedCod");
                }
            }
        }
        private string _ctrlLoteUnidMedCod;
    
        [DataMember]
        public Nullable<short> CtrlLoteUnidMedPos
        {
            get { return _ctrlLoteUnidMedPos; }
            set
            {
                if (_ctrlLoteUnidMedPos != value)
                {
                    _ctrlLoteUnidMedPos = value;
                    OnPropertyChanged("CtrlLoteUnidMedPos");
                }
            }
        }
        private Nullable<short> _ctrlLoteUnidMedPos;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteLargMat
        {
            get { return _ctrlLoteLargMat; }
            set
            {
                if (_ctrlLoteLargMat != value)
                {
                    _ctrlLoteLargMat = value;
                    OnPropertyChanged("CtrlLoteLargMat");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteLargMat;
    
        [DataMember]
        public string GeraNumLoteCod
        {
            get { return _geraNumLoteCod; }
            set
            {
                if (_geraNumLoteCod != value)
                {
                    _geraNumLoteCod = value;
                    OnPropertyChanged("GeraNumLoteCod");
                }
            }
        }
        private string _geraNumLoteCod;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteQtdSaldoCalc
        {
            get { return _ctrlLoteQtdSaldoCalc; }
            set
            {
                if (_ctrlLoteQtdSaldoCalc != value)
                {
                    _ctrlLoteQtdSaldoCalc = value;
                    OnPropertyChanged("CtrlLoteQtdSaldoCalc");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteQtdSaldoCalc;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteCompBruto
        {
            get { return _ctrlLoteCompBruto; }
            set
            {
                if (_ctrlLoteCompBruto != value)
                {
                    _ctrlLoteCompBruto = value;
                    OnPropertyChanged("CtrlLoteCompBruto");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteCompBruto;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteCompLiq
        {
            get { return _ctrlLoteCompLiq; }
            set
            {
                if (_ctrlLoteCompLiq != value)
                {
                    _ctrlLoteCompLiq = value;
                    OnPropertyChanged("CtrlLoteCompLiq");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteCompLiq;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteLargBruta
        {
            get { return _ctrlLoteLargBruta; }
            set
            {
                if (_ctrlLoteLargBruta != value)
                {
                    _ctrlLoteLargBruta = value;
                    OnPropertyChanged("CtrlLoteLargBruta");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteLargBruta;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteLargLiq
        {
            get { return _ctrlLoteLargLiq; }
            set
            {
                if (_ctrlLoteLargLiq != value)
                {
                    _ctrlLoteLargLiq = value;
                    OnPropertyChanged("CtrlLoteLargLiq");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteLargLiq;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteAltBruta
        {
            get { return _ctrlLoteAltBruta; }
            set
            {
                if (_ctrlLoteAltBruta != value)
                {
                    _ctrlLoteAltBruta = value;
                    OnPropertyChanged("CtrlLoteAltBruta");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteAltBruta;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteAltLiq
        {
            get { return _ctrlLoteAltLiq; }
            set
            {
                if (_ctrlLoteAltLiq != value)
                {
                    _ctrlLoteAltLiq = value;
                    OnPropertyChanged("CtrlLoteAltLiq");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteAltLiq;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteQtdBruta
        {
            get { return _ctrlLoteQtdBruta; }
            set
            {
                if (_ctrlLoteQtdBruta != value)
                {
                    _ctrlLoteQtdBruta = value;
                    OnPropertyChanged("CtrlLoteQtdBruta");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteQtdBruta;
    
        [DataMember]
        public Nullable<decimal> CtrlLotePotConcentr
        {
            get { return _ctrlLotePotConcentr; }
            set
            {
                if (_ctrlLotePotConcentr != value)
                {
                    _ctrlLotePotConcentr = value;
                    OnPropertyChanged("CtrlLotePotConcentr");
                }
            }
        }
        private Nullable<decimal> _ctrlLotePotConcentr;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteQtdPesagem
        {
            get { return _ctrlLoteQtdPesagem; }
            set
            {
                if (_ctrlLoteQtdPesagem != value)
                {
                    _ctrlLoteQtdPesagem = value;
                    OnPropertyChanged("CtrlLoteQtdPesagem");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteQtdPesagem;
    
        [DataMember]
        public string CtrlLoteClasCertificFSC
        {
            get { return _ctrlLoteClasCertificFSC; }
            set
            {
                if (_ctrlLoteClasCertificFSC != value)
                {
                    _ctrlLoteClasCertificFSC = value;
                    OnPropertyChanged("CtrlLoteClasCertificFSC");
                }
            }
        }
        private string _ctrlLoteClasCertificFSC;
    
        [DataMember]
        public Nullable<decimal> CtrlLotePeso
        {
            get { return _ctrlLotePeso; }
            set
            {
                if (_ctrlLotePeso != value)
                {
                    _ctrlLotePeso = value;
                    OnPropertyChanged("CtrlLotePeso");
                }
            }
        }
        private Nullable<decimal> _ctrlLotePeso;
    
        [DataMember]
        public string USERGranjaNucleoFLIP
        {
            get { return _uSERGranjaNucleoFLIP; }
            set
            {
                if (_uSERGranjaNucleoFLIP != value)
                {
                    _uSERGranjaNucleoFLIP = value;
                    OnPropertyChanged("USERGranjaNucleoFLIP");
                }
            }
        }
        private string _uSERGranjaNucleoFLIP;
    
        [DataMember]
        public Nullable<short> USERIdateLoteFLIP
        {
            get { return _uSERIdateLoteFLIP; }
            set
            {
                if (_uSERIdateLoteFLIP != value)
                {
                    _uSERIdateLoteFLIP = value;
                    OnPropertyChanged("USERIdateLoteFLIP");
                }
            }
        }
        private Nullable<short> _uSERIdateLoteFLIP;
    
        [DataMember]
        public Nullable<decimal> USERPercMediaIncUlt4SemFLIP
        {
            get { return _uSERPercMediaIncUlt4SemFLIP; }
            set
            {
                if (_uSERPercMediaIncUlt4SemFLIP != value)
                {
                    _uSERPercMediaIncUlt4SemFLIP = value;
                    OnPropertyChanged("USERPercMediaIncUlt4SemFLIP");
                }
            }
        }
        private Nullable<decimal> _uSERPercMediaIncUlt4SemFLIP;
    
        [DataMember]
        public Nullable<short> USERQtdeIncNaoImportApolo
        {
            get { return _uSERQtdeIncNaoImportApolo; }
            set
            {
                if (_uSERQtdeIncNaoImportApolo != value)
                {
                    _uSERQtdeIncNaoImportApolo = value;
                    OnPropertyChanged("USERQtdeIncNaoImportApolo");
                }
            }
        }
        private Nullable<short> _uSERQtdeIncNaoImportApolo;
    
        [DataMember]
        public Nullable<decimal> CtrlLotePesoLiq
        {
            get { return _ctrlLotePesoLiq; }
            set
            {
                if (_ctrlLotePesoLiq != value)
                {
                    _ctrlLotePesoLiq = value;
                    OnPropertyChanged("CtrlLotePesoLiq");
                }
            }
        }
        private Nullable<decimal> _ctrlLotePesoLiq;
    
        [DataMember]
        public Nullable<decimal> CtrlLotePesoBruto
        {
            get { return _ctrlLotePesoBruto; }
            set
            {
                if (_ctrlLotePesoBruto != value)
                {
                    _ctrlLotePesoBruto = value;
                    OnPropertyChanged("CtrlLotePesoBruto");
                }
            }
        }
        private Nullable<decimal> _ctrlLotePesoBruto;
    
        [DataMember]
        public string CtrlLoteCodSSCC
        {
            get { return _ctrlLoteCodSSCC; }
            set
            {
                if (_ctrlLoteCodSSCC != value)
                {
                    _ctrlLoteCodSSCC = value;
                    OnPropertyChanged("CtrlLoteCodSSCC");
                }
            }
        }
        private string _ctrlLoteCodSSCC;
    
        [DataMember]
        public string CtrlLoteSituacao
        {
            get { return _ctrlLoteSituacao; }
            set
            {
                if (_ctrlLoteSituacao != value)
                {
                    _ctrlLoteSituacao = value;
                    OnPropertyChanged("CtrlLoteSituacao");
                }
            }
        }
        private string _ctrlLoteSituacao;
    
        [DataMember]
        public string CtrlLoteAparencia
        {
            get { return _ctrlLoteAparencia; }
            set
            {
                if (_ctrlLoteAparencia != value)
                {
                    _ctrlLoteAparencia = value;
                    OnPropertyChanged("CtrlLoteAparencia");
                }
            }
        }
        private string _ctrlLoteAparencia;
    
        [DataMember]
        public string CtrlLoteObsAparencia
        {
            get { return _ctrlLoteObsAparencia; }
            set
            {
                if (_ctrlLoteObsAparencia != value)
                {
                    _ctrlLoteObsAparencia = value;
                    OnPropertyChanged("CtrlLoteObsAparencia");
                }
            }
        }
        private string _ctrlLoteObsAparencia;
    
        [DataMember]
        public string CtrlLoteObs
        {
            get { return _ctrlLoteObs; }
            set
            {
                if (_ctrlLoteObs != value)
                {
                    _ctrlLoteObs = value;
                    OnPropertyChanged("CtrlLoteObs");
                }
            }
        }
        private string _ctrlLoteObs;
    
        [DataMember]
        public string CtrlLoteCondicaoBauTransp
        {
            get { return _ctrlLoteCondicaoBauTransp; }
            set
            {
                if (_ctrlLoteCondicaoBauTransp != value)
                {
                    _ctrlLoteCondicaoBauTransp = value;
                    OnPropertyChanged("CtrlLoteCondicaoBauTransp");
                }
            }
        }
        private string _ctrlLoteCondicaoBauTransp;
    
        [DataMember]
        public string CtrlLoteTermoking
        {
            get { return _ctrlLoteTermoking; }
            set
            {
                if (_ctrlLoteTermoking != value)
                {
                    _ctrlLoteTermoking = value;
                    OnPropertyChanged("CtrlLoteTermoking");
                }
            }
        }
        private string _ctrlLoteTermoking;
    
        [DataMember]
        public Nullable<decimal> CtrlLoteTemperBauTransp
        {
            get { return _ctrlLoteTemperBauTransp; }
            set
            {
                if (_ctrlLoteTemperBauTransp != value)
                {
                    _ctrlLoteTemperBauTransp = value;
                    OnPropertyChanged("CtrlLoteTemperBauTransp");
                }
            }
        }
        private Nullable<decimal> _ctrlLoteTemperBauTransp;
    
        [DataMember]
        public string CtrlLoteNumLoteOrig
        {
            get { return _ctrlLoteNumLoteOrig; }
            set
            {
                if (_ctrlLoteNumLoteOrig != value)
                {
                    _ctrlLoteNumLoteOrig = value;
                    OnPropertyChanged("CtrlLoteNumLoteOrig");
                }
            }
        }
        private string _ctrlLoteNumLoteOrig;
    
        [DataMember]
        public string CtrlLoteCompLote1
        {
            get { return _ctrlLoteCompLote1; }
            set
            {
                if (_ctrlLoteCompLote1 != value)
                {
                    _ctrlLoteCompLote1 = value;
                    OnPropertyChanged("CtrlLoteCompLote1");
                }
            }
        }
        private string _ctrlLoteCompLote1;
    
        [DataMember]
        public string CtrlLoteCompLote2
        {
            get { return _ctrlLoteCompLote2; }
            set
            {
                if (_ctrlLoteCompLote2 != value)
                {
                    _ctrlLoteCompLote2 = value;
                    OnPropertyChanged("CtrlLoteCompLote2");
                }
            }
        }
        private string _ctrlLoteCompLote2;
    
        [DataMember]
        public string CtrlLoteCompLote3
        {
            get { return _ctrlLoteCompLote3; }
            set
            {
                if (_ctrlLoteCompLote3 != value)
                {
                    _ctrlLoteCompLote3 = value;
                    OnPropertyChanged("CtrlLoteCompLote3");
                }
            }
        }
        private string _ctrlLoteCompLote3;
    
        [DataMember]
        public string CtrlLoteCompLote4
        {
            get { return _ctrlLoteCompLote4; }
            set
            {
                if (_ctrlLoteCompLote4 != value)
                {
                    _ctrlLoteCompLote4 = value;
                    OnPropertyChanged("CtrlLoteCompLote4");
                }
            }
        }
        private string _ctrlLoteCompLote4;
    
        [DataMember]
        public string CtrlLoteCompLote5
        {
            get { return _ctrlLoteCompLote5; }
            set
            {
                if (_ctrlLoteCompLote5 != value)
                {
                    _ctrlLoteCompLote5 = value;
                    OnPropertyChanged("CtrlLoteCompLote5");
                }
            }
        }
        private string _ctrlLoteCompLote5;
    
        [DataMember]
        public string CtrlLoteEntCod
        {
            get { return _ctrlLoteEntCod; }
            set
            {
                if (_ctrlLoteEntCod != value)
                {
                    ChangeTracker.RecordOriginalValue("CtrlLoteEntCod", _ctrlLoteEntCod);
                    if (!IsDeserializing)
                    {
                        if (ENTIDADE1 != null && ENTIDADE1.EntCod != value)
                        {
                            ENTIDADE1 = null;
                        }
                    }
                    _ctrlLoteEntCod = value;
                    OnPropertyChanged("CtrlLoteEntCod");
                }
            }
        }
        private string _ctrlLoteEntCod;

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
    
        [DataMember]
        public ENTIDADE1 ENTIDADE1
        {
            get { return _eNTIDADE1; }
            set
            {
                if (!ReferenceEquals(_eNTIDADE1, value))
                {
                    var previousValue = _eNTIDADE1;
                    _eNTIDADE1 = value;
                    FixupENTIDADE1(previousValue);
                    OnNavigationPropertyChanged("ENTIDADE1");
                }
            }
        }
        private ENTIDADE1 _eNTIDADE1;

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
            ENTIDADE1 = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupEMPRESA_FILIAL(EMPRESA_FILIAL previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CTRL_LOTE.Contains(this))
            {
                previousValue.CTRL_LOTE.Remove(this);
            }
    
            if (EMPRESA_FILIAL != null)
            {
                if (!EMPRESA_FILIAL.CTRL_LOTE.Contains(this))
                {
                    EMPRESA_FILIAL.CTRL_LOTE.Add(this);
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
    
            if (previousValue != null && previousValue.CTRL_LOTE.Contains(this))
            {
                previousValue.CTRL_LOTE.Remove(this);
            }
    
            if (PRODUTO1 != null)
            {
                if (!PRODUTO1.CTRL_LOTE.Contains(this))
                {
                    PRODUTO1.CTRL_LOTE.Add(this);
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
    
        private void FixupENTIDADE1(ENTIDADE1 previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CTRL_LOTE.Contains(this))
            {
                previousValue.CTRL_LOTE.Remove(this);
            }
    
            if (ENTIDADE1 != null)
            {
                if (!ENTIDADE1.CTRL_LOTE.Contains(this))
                {
                    ENTIDADE1.CTRL_LOTE.Add(this);
                }
    
                CtrlLoteEntCod = ENTIDADE1.EntCod;
            }
            else if (!skipKeys)
            {
                CtrlLoteEntCod = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("ENTIDADE1")
                    && (ChangeTracker.OriginalValues["ENTIDADE1"] == ENTIDADE1))
                {
                    ChangeTracker.OriginalValues.Remove("ENTIDADE1");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("ENTIDADE1", previousValue);
                }
                if (ENTIDADE1 != null && !ENTIDADE1.ChangeTracker.ChangeTrackingEnabled)
                {
                    ENTIDADE1.StartTracking();
                }
            }
        }

        #endregion

    }
}
