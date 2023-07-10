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
    [KnownType(typeof(OCORRENCIA))]
    [KnownType(typeof(COND_PAG))]
    public partial class MOTIVO_OCOR: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public string MotOcorCodEstr
        {
            get { return _motOcorCodEstr; }
            set
            {
                if (_motOcorCodEstr != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'MotOcorCodEstr' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _motOcorCodEstr = value;
                    OnPropertyChanged("MotOcorCodEstr");
                }
            }
        }
        private string _motOcorCodEstr;
    
        [DataMember]
        public string MotOcorDescr
        {
            get { return _motOcorDescr; }
            set
            {
                if (_motOcorDescr != value)
                {
                    _motOcorDescr = value;
                    OnPropertyChanged("MotOcorDescr");
                }
            }
        }
        private string _motOcorDescr;
    
        [DataMember]
        public Nullable<short> TipoOcorCod
        {
            get { return _tipoOcorCod; }
            set
            {
                if (_tipoOcorCod != value)
                {
                    _tipoOcorCod = value;
                    OnPropertyChanged("TipoOcorCod");
                }
            }
        }
        private Nullable<short> _tipoOcorCod;
    
        [DataMember]
        public string GrpUsuCod
        {
            get { return _grpUsuCod; }
            set
            {
                if (_grpUsuCod != value)
                {
                    _grpUsuCod = value;
                    OnPropertyChanged("GrpUsuCod");
                }
            }
        }
        private string _grpUsuCod;
    
        [DataMember]
        public string MotOcorResp1
        {
            get { return _motOcorResp1; }
            set
            {
                if (_motOcorResp1 != value)
                {
                    _motOcorResp1 = value;
                    OnPropertyChanged("MotOcorResp1");
                }
            }
        }
        private string _motOcorResp1;
    
        [DataMember]
        public string MotOcorResp2
        {
            get { return _motOcorResp2; }
            set
            {
                if (_motOcorResp2 != value)
                {
                    _motOcorResp2 = value;
                    OnPropertyChanged("MotOcorResp2");
                }
            }
        }
        private string _motOcorResp2;
    
        [DataMember]
        public string MotOcorResp3
        {
            get { return _motOcorResp3; }
            set
            {
                if (_motOcorResp3 != value)
                {
                    _motOcorResp3 = value;
                    OnPropertyChanged("MotOcorResp3");
                }
            }
        }
        private string _motOcorResp3;
    
        [DataMember]
        public Nullable<short> MotOcorQtdPrazo
        {
            get { return _motOcorQtdPrazo; }
            set
            {
                if (_motOcorQtdPrazo != value)
                {
                    _motOcorQtdPrazo = value;
                    OnPropertyChanged("MotOcorQtdPrazo");
                }
            }
        }
        private Nullable<short> _motOcorQtdPrazo;
    
        [DataMember]
        public string MotOcorTipoPrazo
        {
            get { return _motOcorTipoPrazo; }
            set
            {
                if (_motOcorTipoPrazo != value)
                {
                    _motOcorTipoPrazo = value;
                    OnPropertyChanged("MotOcorTipoPrazo");
                }
            }
        }
        private string _motOcorTipoPrazo;
    
        [DataMember]
        public string MotOcorAltDataPrev
        {
            get { return _motOcorAltDataPrev; }
            set
            {
                if (_motOcorAltDataPrev != value)
                {
                    _motOcorAltDataPrev = value;
                    OnPropertyChanged("MotOcorAltDataPrev");
                }
            }
        }
        private string _motOcorAltDataPrev;
    
        [DataMember]
        public string MotOcorTexto
        {
            get { return _motOcorTexto; }
            set
            {
                if (_motOcorTexto != value)
                {
                    _motOcorTexto = value;
                    OnPropertyChanged("MotOcorTexto");
                }
            }
        }
        private string _motOcorTexto;
    
        [DataMember]
        public string MotOcorSolicitaProd
        {
            get { return _motOcorSolicitaProd; }
            set
            {
                if (_motOcorSolicitaProd != value)
                {
                    _motOcorSolicitaProd = value;
                    OnPropertyChanged("MotOcorSolicitaProd");
                }
            }
        }
        private string _motOcorSolicitaProd;
    
        [DataMember]
        public Nullable<short> MotOcorPrior
        {
            get { return _motOcorPrior; }
            set
            {
                if (_motOcorPrior != value)
                {
                    _motOcorPrior = value;
                    OnPropertyChanged("MotOcorPrior");
                }
            }
        }
        private Nullable<short> _motOcorPrior;
    
        [DataMember]
        public string MotOcorAltPrior
        {
            get { return _motOcorAltPrior; }
            set
            {
                if (_motOcorAltPrior != value)
                {
                    _motOcorAltPrior = value;
                    OnPropertyChanged("MotOcorAltPrior");
                }
            }
        }
        private string _motOcorAltPrior;
    
        [DataMember]
        public string TipoSolOcorCod
        {
            get { return _tipoSolOcorCod; }
            set
            {
                if (_tipoSolOcorCod != value)
                {
                    _tipoSolOcorCod = value;
                    OnPropertyChanged("TipoSolOcorCod");
                }
            }
        }
        private string _tipoSolOcorCod;
    
        [DataMember]
        public string RotConvCod
        {
            get { return _rotConvCod; }
            set
            {
                if (_rotConvCod != value)
                {
                    _rotConvCod = value;
                    OnPropertyChanged("RotConvCod");
                }
            }
        }
        private string _rotConvCod;
    
        [DataMember]
        public string MotOcorCodEstrNiv
        {
            get { return _motOcorCodEstrNiv; }
            set
            {
                if (_motOcorCodEstrNiv != value)
                {
                    _motOcorCodEstrNiv = value;
                    OnPropertyChanged("MotOcorCodEstrNiv");
                }
            }
        }
        private string _motOcorCodEstrNiv;
    
        [DataMember]
        public string MotOcorGrupo
        {
            get { return _motOcorGrupo; }
            set
            {
                if (_motOcorGrupo != value)
                {
                    _motOcorGrupo = value;
                    OnPropertyChanged("MotOcorGrupo");
                }
            }
        }
        private string _motOcorGrupo;
    
        [DataMember]
        public string MotOcorAlarmeParaResp
        {
            get { return _motOcorAlarmeParaResp; }
            set
            {
                if (_motOcorAlarmeParaResp != value)
                {
                    _motOcorAlarmeParaResp = value;
                    OnPropertyChanged("MotOcorAlarmeParaResp");
                }
            }
        }
        private string _motOcorAlarmeParaResp;
    
        [DataMember]
        public string MotOcorAlarmeParaAtend
        {
            get { return _motOcorAlarmeParaAtend; }
            set
            {
                if (_motOcorAlarmeParaAtend != value)
                {
                    _motOcorAlarmeParaAtend = value;
                    OnPropertyChanged("MotOcorAlarmeParaAtend");
                }
            }
        }
        private string _motOcorAlarmeParaAtend;
    
        [DataMember]
        public string MotOcorEntraPesqInternet
        {
            get { return _motOcorEntraPesqInternet; }
            set
            {
                if (_motOcorEntraPesqInternet != value)
                {
                    _motOcorEntraPesqInternet = value;
                    OnPropertyChanged("MotOcorEntraPesqInternet");
                }
            }
        }
        private string _motOcorEntraPesqInternet;
    
        [DataMember]
        public string MotOcorAltDataOcor
        {
            get { return _motOcorAltDataOcor; }
            set
            {
                if (_motOcorAltDataOcor != value)
                {
                    _motOcorAltDataOcor = value;
                    OnPropertyChanged("MotOcorAltDataOcor");
                }
            }
        }
        private string _motOcorAltDataOcor;
    
        [DataMember]
        public string MotOcorPermAltGrpResp
        {
            get { return _motOcorPermAltGrpResp; }
            set
            {
                if (_motOcorPermAltGrpResp != value)
                {
                    _motOcorPermAltGrpResp = value;
                    OnPropertyChanged("MotOcorPermAltGrpResp");
                }
            }
        }
        private string _motOcorPermAltGrpResp;
    
        [DataMember]
        public Nullable<System.DateTime> MotOcorDataValidIni
        {
            get { return _motOcorDataValidIni; }
            set
            {
                if (_motOcorDataValidIni != value)
                {
                    _motOcorDataValidIni = value;
                    OnPropertyChanged("MotOcorDataValidIni");
                }
            }
        }
        private Nullable<System.DateTime> _motOcorDataValidIni;
    
        [DataMember]
        public Nullable<System.DateTime> MotOcorDataValidFim
        {
            get { return _motOcorDataValidFim; }
            set
            {
                if (_motOcorDataValidFim != value)
                {
                    _motOcorDataValidFim = value;
                    OnPropertyChanged("MotOcorDataValidFim");
                }
            }
        }
        private Nullable<System.DateTime> _motOcorDataValidFim;
    
        [DataMember]
        public string MotOcorCtrlMortalidade
        {
            get { return _motOcorCtrlMortalidade; }
            set
            {
                if (_motOcorCtrlMortalidade != value)
                {
                    _motOcorCtrlMortalidade = value;
                    OnPropertyChanged("MotOcorCtrlMortalidade");
                }
            }
        }
        private string _motOcorCtrlMortalidade;
    
        [DataMember]
        public string MotOcorCancCartCred
        {
            get { return _motOcorCancCartCred; }
            set
            {
                if (_motOcorCancCartCred != value)
                {
                    _motOcorCancCartCred = value;
                    OnPropertyChanged("MotOcorCancCartCred");
                }
            }
        }
        private string _motOcorCancCartCred;
    
        [DataMember]
        public string MotOcorPermRelacVisita
        {
            get { return _motOcorPermRelacVisita; }
            set
            {
                if (_motOcorPermRelacVisita != value)
                {
                    _motOcorPermRelacVisita = value;
                    OnPropertyChanged("MotOcorPermRelacVisita");
                }
            }
        }
        private string _motOcorPermRelacVisita;
    
        [DataMember]
        public string USERCodigoTPManut
        {
            get { return _uSERCodigoTPManut; }
            set
            {
                if (_uSERCodigoTPManut != value)
                {
                    _uSERCodigoTPManut = value;
                    OnPropertyChanged("USERCodigoTPManut");
                }
            }
        }
        private string _uSERCodigoTPManut;
    
        [DataMember]
        public string MOTOCORPASTA
        {
            get { return _mOTOCORPASTA; }
            set
            {
                if (_mOTOCORPASTA != value)
                {
                    _mOTOCORPASTA = value;
                    OnPropertyChanged("MOTOCORPASTA");
                }
            }
        }
        private string _mOTOCORPASTA;
    
        [DataMember]
        public string MotOcorHabDtSolHistOcor
        {
            get { return _motOcorHabDtSolHistOcor; }
            set
            {
                if (_motOcorHabDtSolHistOcor != value)
                {
                    _motOcorHabDtSolHistOcor = value;
                    OnPropertyChanged("MotOcorHabDtSolHistOcor");
                }
            }
        }
        private string _motOcorHabDtSolHistOcor;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<OCORRENCIA> OCORRENCIA
        {
            get
            {
                if (_oCORRENCIA == null)
                {
                    _oCORRENCIA = new TrackableCollection<OCORRENCIA>();
                    _oCORRENCIA.CollectionChanged += FixupOCORRENCIA;
                }
                return _oCORRENCIA;
            }
            set
            {
                if (!ReferenceEquals(_oCORRENCIA, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_oCORRENCIA != null)
                    {
                        _oCORRENCIA.CollectionChanged -= FixupOCORRENCIA;
                    }
                    _oCORRENCIA = value;
                    if (_oCORRENCIA != null)
                    {
                        _oCORRENCIA.CollectionChanged += FixupOCORRENCIA;
                    }
                    OnNavigationPropertyChanged("OCORRENCIA");
                }
            }
        }
        private TrackableCollection<OCORRENCIA> _oCORRENCIA;
    
        [DataMember]
        public TrackableCollection<COND_PAG> COND_PAG
        {
            get
            {
                if (_cOND_PAG == null)
                {
                    _cOND_PAG = new TrackableCollection<COND_PAG>();
                    _cOND_PAG.CollectionChanged += FixupCOND_PAG;
                }
                return _cOND_PAG;
            }
            set
            {
                if (!ReferenceEquals(_cOND_PAG, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_cOND_PAG != null)
                    {
                        _cOND_PAG.CollectionChanged -= FixupCOND_PAG;
                    }
                    _cOND_PAG = value;
                    if (_cOND_PAG != null)
                    {
                        _cOND_PAG.CollectionChanged += FixupCOND_PAG;
                    }
                    OnNavigationPropertyChanged("COND_PAG");
                }
            }
        }
        private TrackableCollection<COND_PAG> _cOND_PAG;

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
    
        protected virtual void ClearNavigationProperties()
        {
            OCORRENCIA.Clear();
            COND_PAG.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupOCORRENCIA(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (OCORRENCIA item in e.NewItems)
                {
                    item.MOTIVO_OCOR = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("OCORRENCIA", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (OCORRENCIA item in e.OldItems)
                {
                    if (ReferenceEquals(item.MOTIVO_OCOR, this))
                    {
                        item.MOTIVO_OCOR = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("OCORRENCIA", item);
                    }
                }
            }
        }
    
        private void FixupCOND_PAG(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (COND_PAG item in e.NewItems)
                {
                    item.MOTIVO_OCOR = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("COND_PAG", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (COND_PAG item in e.OldItems)
                {
                    if (ReferenceEquals(item.MOTIVO_OCOR, this))
                    {
                        item.MOTIVO_OCOR = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("COND_PAG", item);
                    }
                }
            }
        }

        #endregion

    }
}