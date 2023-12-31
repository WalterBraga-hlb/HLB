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
    [KnownType(typeof(CIDADE))]
    [KnownType(typeof(NOTA_FISCAL))]
    public partial class ENDER_ENT: IObjectWithChangeTracker, INotifyPropertyChanged
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
                    _entCod = value;
                    OnPropertyChanged("EntCod");
                }
            }
        }
        private string _entCod;
    
        [DataMember]
        public short EnderEntSeq
        {
            get { return _enderEntSeq; }
            set
            {
                if (_enderEntSeq != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'EnderEntSeq' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _enderEntSeq = value;
                    OnPropertyChanged("EnderEntSeq");
                }
            }
        }
        private short _enderEntSeq;
    
        [DataMember]
        public string EnderEntEntrega
        {
            get { return _enderEntEntrega; }
            set
            {
                if (_enderEntEntrega != value)
                {
                    _enderEntEntrega = value;
                    OnPropertyChanged("EnderEntEntrega");
                }
            }
        }
        private string _enderEntEntrega;
    
        [DataMember]
        public string EnderEntCobranca
        {
            get { return _enderEntCobranca; }
            set
            {
                if (_enderEntCobranca != value)
                {
                    _enderEntCobranca = value;
                    OnPropertyChanged("EnderEntCobranca");
                }
            }
        }
        private string _enderEntCobranca;
    
        [DataMember]
        public string EnderEntNome
        {
            get { return _enderEntNome; }
            set
            {
                if (_enderEntNome != value)
                {
                    _enderEntNome = value;
                    OnPropertyChanged("EnderEntNome");
                }
            }
        }
        private string _enderEntNome;
    
        [DataMember]
        public string EnderEnt
        {
            get { return _enderEnt; }
            set
            {
                if (_enderEnt != value)
                {
                    _enderEnt = value;
                    OnPropertyChanged("EnderEnt");
                }
            }
        }
        private string _enderEnt;
    
        [DataMember]
        public string TipoLogradAbrev
        {
            get { return _tipoLogradAbrev; }
            set
            {
                if (_tipoLogradAbrev != value)
                {
                    _tipoLogradAbrev = value;
                    OnPropertyChanged("TipoLogradAbrev");
                }
            }
        }
        private string _tipoLogradAbrev;
    
        [DataMember]
        public string EnderEntNo
        {
            get { return _enderEntNo; }
            set
            {
                if (_enderEntNo != value)
                {
                    _enderEntNo = value;
                    OnPropertyChanged("EnderEntNo");
                }
            }
        }
        private string _enderEntNo;
    
        [DataMember]
        public string EnderEntNoPI
        {
            get { return _enderEntNoPI; }
            set
            {
                if (_enderEntNoPI != value)
                {
                    _enderEntNoPI = value;
                    OnPropertyChanged("EnderEntNoPI");
                }
            }
        }
        private string _enderEntNoPI;
    
        [DataMember]
        public string EnderEntComp
        {
            get { return _enderEntComp; }
            set
            {
                if (_enderEntComp != value)
                {
                    _enderEntComp = value;
                    OnPropertyChanged("EnderEntComp");
                }
            }
        }
        private string _enderEntComp;
    
        [DataMember]
        public string EnderEntBair
        {
            get { return _enderEntBair; }
            set
            {
                if (_enderEntBair != value)
                {
                    _enderEntBair = value;
                    OnPropertyChanged("EnderEntBair");
                }
            }
        }
        private string _enderEntBair;
    
        [DataMember]
        public string CidCod
        {
            get { return _cidCod; }
            set
            {
                if (_cidCod != value)
                {
                    ChangeTracker.RecordOriginalValue("CidCod", _cidCod);
                    if (!IsDeserializing)
                    {
                        if (CIDADE != null && CIDADE.CidCod != value)
                        {
                            CIDADE = null;
                        }
                    }
                    _cidCod = value;
                    OnPropertyChanged("CidCod");
                }
            }
        }
        private string _cidCod;
    
        [DataMember]
        public string EnderEntCep
        {
            get { return _enderEntCep; }
            set
            {
                if (_enderEntCep != value)
                {
                    _enderEntCep = value;
                    OnPropertyChanged("EnderEntCep");
                }
            }
        }
        private string _enderEntCep;
    
        [DataMember]
        public string EnderEntTipoFJ
        {
            get { return _enderEntTipoFJ; }
            set
            {
                if (_enderEntTipoFJ != value)
                {
                    _enderEntTipoFJ = value;
                    OnPropertyChanged("EnderEntTipoFJ");
                }
            }
        }
        private string _enderEntTipoFJ;
    
        [DataMember]
        public string EnderEntCpfCgc
        {
            get { return _enderEntCpfCgc; }
            set
            {
                if (_enderEntCpfCgc != value)
                {
                    _enderEntCpfCgc = value;
                    OnPropertyChanged("EnderEntCpfCgc");
                }
            }
        }
        private string _enderEntCpfCgc;
    
        [DataMember]
        public string EnderEntRgIe
        {
            get { return _enderEntRgIe; }
            set
            {
                if (_enderEntRgIe != value)
                {
                    _enderEntRgIe = value;
                    OnPropertyChanged("EnderEntRgIe");
                }
            }
        }
        private string _enderEntRgIe;
    
        [DataMember]
        public string EnderEntRgOrgExped
        {
            get { return _enderEntRgOrgExped; }
            set
            {
                if (_enderEntRgOrgExped != value)
                {
                    _enderEntRgOrgExped = value;
                    OnPropertyChanged("EnderEntRgOrgExped");
                }
            }
        }
        private string _enderEntRgOrgExped;
    
        [DataMember]
        public string EnderEntCxaPost
        {
            get { return _enderEntCxaPost; }
            set
            {
                if (_enderEntCxaPost != value)
                {
                    _enderEntCxaPost = value;
                    OnPropertyChanged("EnderEntCxaPost");
                }
            }
        }
        private string _enderEntCxaPost;
    
        [DataMember]
        public string EnderEntEMail
        {
            get { return _enderEntEMail; }
            set
            {
                if (_enderEntEMail != value)
                {
                    _enderEntEMail = value;
                    OnPropertyChanged("EnderEntEMail");
                }
            }
        }
        private string _enderEntEMail;
    
        [DataMember]
        public string EnderEntWWW
        {
            get { return _enderEntWWW; }
            set
            {
                if (_enderEntWWW != value)
                {
                    _enderEntWWW = value;
                    OnPropertyChanged("EnderEntWWW");
                }
            }
        }
        private string _enderEntWWW;
    
        [DataMember]
        public string EnderEntContato
        {
            get { return _enderEntContato; }
            set
            {
                if (_enderEntContato != value)
                {
                    _enderEntContato = value;
                    OnPropertyChanged("EnderEntContato");
                }
            }
        }
        private string _enderEntContato;
    
        [DataMember]
        public string EnderEntTexto
        {
            get { return _enderEntTexto; }
            set
            {
                if (_enderEntTexto != value)
                {
                    _enderEntTexto = value;
                    OnPropertyChanged("EnderEntTexto");
                }
            }
        }
        private string _enderEntTexto;
    
        [DataMember]
        public string EnderEntCod
        {
            get { return _enderEntCod; }
            set
            {
                if (_enderEntCod != value)
                {
                    _enderEntCod = value;
                    OnPropertyChanged("EnderEntCod");
                }
            }
        }
        private string _enderEntCod;
    
        [DataMember]
        public string EnderEntFaturam
        {
            get { return _enderEntFaturam; }
            set
            {
                if (_enderEntFaturam != value)
                {
                    _enderEntFaturam = value;
                    OnPropertyChanged("EnderEntFaturam");
                }
            }
        }
        private string _enderEntFaturam;
    
        [DataMember]
        public string EnderEntColeta
        {
            get { return _enderEntColeta; }
            set
            {
                if (_enderEntColeta != value)
                {
                    _enderEntColeta = value;
                    OnPropertyChanged("EnderEntColeta");
                }
            }
        }
        private string _enderEntColeta;
    
        [DataMember]
        public Nullable<System.DateTime> EnderEntDataValInic
        {
            get { return _enderEntDataValInic; }
            set
            {
                if (_enderEntDataValInic != value)
                {
                    _enderEntDataValInic = value;
                    OnPropertyChanged("EnderEntDataValInic");
                }
            }
        }
        private Nullable<System.DateTime> _enderEntDataValInic;
    
        [DataMember]
        public Nullable<System.DateTime> EnderEntDataValFinal
        {
            get { return _enderEntDataValFinal; }
            set
            {
                if (_enderEntDataValFinal != value)
                {
                    _enderEntDataValFinal = value;
                    OnPropertyChanged("EnderEntDataValFinal");
                }
            }
        }
        private Nullable<System.DateTime> _enderEntDataValFinal;
    
        [DataMember]
        public string EnderEntCertificado
        {
            get { return _enderEntCertificado; }
            set
            {
                if (_enderEntCertificado != value)
                {
                    _enderEntCertificado = value;
                    OnPropertyChanged("EnderEntCertificado");
                }
            }
        }
        private string _enderEntCertificado;
    
        [DataMember]
        public Nullable<int> EnderSeq
        {
            get { return _enderSeq; }
            set
            {
                if (_enderSeq != value)
                {
                    _enderSeq = value;
                    OnPropertyChanged("EnderSeq");
                }
            }
        }
        private Nullable<int> _enderSeq;
    
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
        public CIDADE CIDADE
        {
            get { return _cIDADE; }
            set
            {
                if (!ReferenceEquals(_cIDADE, value))
                {
                    var previousValue = _cIDADE;
                    _cIDADE = value;
                    FixupCIDADE(previousValue);
                    OnNavigationPropertyChanged("CIDADE");
                }
            }
        }
        private CIDADE _cIDADE;
    
        [DataMember]
        public TrackableCollection<NOTA_FISCAL> NOTA_FISCAL
        {
            get
            {
                if (_nOTA_FISCAL == null)
                {
                    _nOTA_FISCAL = new TrackableCollection<NOTA_FISCAL>();
                    _nOTA_FISCAL.CollectionChanged += FixupNOTA_FISCAL;
                }
                return _nOTA_FISCAL;
            }
            set
            {
                if (!ReferenceEquals(_nOTA_FISCAL, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_nOTA_FISCAL != null)
                    {
                        _nOTA_FISCAL.CollectionChanged -= FixupNOTA_FISCAL;
                    }
                    _nOTA_FISCAL = value;
                    if (_nOTA_FISCAL != null)
                    {
                        _nOTA_FISCAL.CollectionChanged += FixupNOTA_FISCAL;
                    }
                    OnNavigationPropertyChanged("NOTA_FISCAL");
                }
            }
        }
        private TrackableCollection<NOTA_FISCAL> _nOTA_FISCAL;
    
        [DataMember]
        public TrackableCollection<NOTA_FISCAL> NOTA_FISCAL1
        {
            get
            {
                if (_nOTA_FISCAL1 == null)
                {
                    _nOTA_FISCAL1 = new TrackableCollection<NOTA_FISCAL>();
                    _nOTA_FISCAL1.CollectionChanged += FixupNOTA_FISCAL1;
                }
                return _nOTA_FISCAL1;
            }
            set
            {
                if (!ReferenceEquals(_nOTA_FISCAL1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_nOTA_FISCAL1 != null)
                    {
                        _nOTA_FISCAL1.CollectionChanged -= FixupNOTA_FISCAL1;
                    }
                    _nOTA_FISCAL1 = value;
                    if (_nOTA_FISCAL1 != null)
                    {
                        _nOTA_FISCAL1.CollectionChanged += FixupNOTA_FISCAL1;
                    }
                    OnNavigationPropertyChanged("NOTA_FISCAL1");
                }
            }
        }
        private TrackableCollection<NOTA_FISCAL> _nOTA_FISCAL1;
    
        [DataMember]
        public TrackableCollection<NOTA_FISCAL> NOTA_FISCAL2
        {
            get
            {
                if (_nOTA_FISCAL2 == null)
                {
                    _nOTA_FISCAL2 = new TrackableCollection<NOTA_FISCAL>();
                    _nOTA_FISCAL2.CollectionChanged += FixupNOTA_FISCAL2;
                }
                return _nOTA_FISCAL2;
            }
            set
            {
                if (!ReferenceEquals(_nOTA_FISCAL2, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_nOTA_FISCAL2 != null)
                    {
                        _nOTA_FISCAL2.CollectionChanged -= FixupNOTA_FISCAL2;
                    }
                    _nOTA_FISCAL2 = value;
                    if (_nOTA_FISCAL2 != null)
                    {
                        _nOTA_FISCAL2.CollectionChanged += FixupNOTA_FISCAL2;
                    }
                    OnNavigationPropertyChanged("NOTA_FISCAL2");
                }
            }
        }
        private TrackableCollection<NOTA_FISCAL> _nOTA_FISCAL2;

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
            CIDADE = null;
            NOTA_FISCAL.Clear();
            NOTA_FISCAL1.Clear();
            NOTA_FISCAL2.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupCIDADE(CIDADE previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ENDER_ENT.Contains(this))
            {
                previousValue.ENDER_ENT.Remove(this);
            }
    
            if (CIDADE != null)
            {
                if (!CIDADE.ENDER_ENT.Contains(this))
                {
                    CIDADE.ENDER_ENT.Add(this);
                }
    
                CidCod = CIDADE.CidCod;
            }
            else if (!skipKeys)
            {
                CidCod = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("CIDADE")
                    && (ChangeTracker.OriginalValues["CIDADE"] == CIDADE))
                {
                    ChangeTracker.OriginalValues.Remove("CIDADE");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("CIDADE", previousValue);
                }
                if (CIDADE != null && !CIDADE.ChangeTracker.ChangeTrackingEnabled)
                {
                    CIDADE.StartTracking();
                }
            }
        }
    
        private void FixupNOTA_FISCAL(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (NOTA_FISCAL item in e.NewItems)
                {
                    item.ENDER_ENT = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("NOTA_FISCAL", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (NOTA_FISCAL item in e.OldItems)
                {
                    if (ReferenceEquals(item.ENDER_ENT, this))
                    {
                        item.ENDER_ENT = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("NOTA_FISCAL", item);
                    }
                }
            }
        }
    
        private void FixupNOTA_FISCAL1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (NOTA_FISCAL item in e.NewItems)
                {
                    item.ENDER_ENT1 = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("NOTA_FISCAL1", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (NOTA_FISCAL item in e.OldItems)
                {
                    if (ReferenceEquals(item.ENDER_ENT1, this))
                    {
                        item.ENDER_ENT1 = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("NOTA_FISCAL1", item);
                    }
                }
            }
        }
    
        private void FixupNOTA_FISCAL2(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (NOTA_FISCAL item in e.NewItems)
                {
                    item.ENDER_ENT2 = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("NOTA_FISCAL2", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (NOTA_FISCAL item in e.OldItems)
                {
                    if (ReferenceEquals(item.ENDER_ENT2, this))
                    {
                        item.ENDER_ENT2 = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("NOTA_FISCAL2", item);
                    }
                }
            }
        }

        #endregion

    }
}
