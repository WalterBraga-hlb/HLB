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
    [KnownType(typeof(PRODUTO1))]
    public partial class ITEM_OCOR: IObjectWithChangeTracker, INotifyPropertyChanged
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
                        if (OCORRENCIA != null && OCORRENCIA.EmpCod != value)
                        {
                            OCORRENCIA = null;
                        }
                    }
                    _empCod = value;
                    OnPropertyChanged("EmpCod");
                }
            }
        }
        private string _empCod;
    
        [DataMember]
        public int OcorCod
        {
            get { return _ocorCod; }
            set
            {
                if (_ocorCod != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'OcorCod' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (OCORRENCIA != null && OCORRENCIA.OcorCod != value)
                        {
                            OCORRENCIA = null;
                        }
                    }
                    _ocorCod = value;
                    OnPropertyChanged("OcorCod");
                }
            }
        }
        private int _ocorCod;
    
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
        public Nullable<decimal> ItOcorQtd
        {
            get { return _itOcorQtd; }
            set
            {
                if (_itOcorQtd != value)
                {
                    _itOcorQtd = value;
                    OnPropertyChanged("ItOcorQtd");
                }
            }
        }
        private Nullable<decimal> _itOcorQtd;
    
        [DataMember]
        public string ItOcorOrdServNum
        {
            get { return _itOcorOrdServNum; }
            set
            {
                if (_itOcorOrdServNum != value)
                {
                    _itOcorOrdServNum = value;
                    OnPropertyChanged("ItOcorOrdServNum");
                }
            }
        }
        private string _itOcorOrdServNum;
    
        [DataMember]
        public string ItOcorTexto
        {
            get { return _itOcorTexto; }
            set
            {
                if (_itOcorTexto != value)
                {
                    _itOcorTexto = value;
                    OnPropertyChanged("ItOcorTexto");
                }
            }
        }
        private string _itOcorTexto;
    
        [DataMember]
        public Nullable<decimal> ItOrcQtdMortalidade
        {
            get { return _itOrcQtdMortalidade; }
            set
            {
                if (_itOrcQtdMortalidade != value)
                {
                    _itOrcQtdMortalidade = value;
                    OnPropertyChanged("ItOrcQtdMortalidade");
                }
            }
        }
        private Nullable<decimal> _itOrcQtdMortalidade;
    
        [DataMember]
        public Nullable<decimal> ItOrcValItem
        {
            get { return _itOrcValItem; }
            set
            {
                if (_itOrcValItem != value)
                {
                    _itOrcValItem = value;
                    OnPropertyChanged("ItOrcValItem");
                }
            }
        }
        private Nullable<decimal> _itOrcValItem;
    
        [DataMember]
        public string ProdUnidMedCod
        {
            get { return _prodUnidMedCod; }
            set
            {
                if (_prodUnidMedCod != value)
                {
                    _prodUnidMedCod = value;
                    OnPropertyChanged("ProdUnidMedCod");
                }
            }
        }
        private string _prodUnidMedCod;
    
        [DataMember]
        public Nullable<short> ProdUnidMedPos
        {
            get { return _prodUnidMedPos; }
            set
            {
                if (_prodUnidMedPos != value)
                {
                    _prodUnidMedPos = value;
                    OnPropertyChanged("ProdUnidMedPos");
                }
            }
        }
        private Nullable<short> _prodUnidMedPos;
    
        [DataMember]
        public string USERHoraInicial
        {
            get { return _uSERHoraInicial; }
            set
            {
                if (_uSERHoraInicial != value)
                {
                    _uSERHoraInicial = value;
                    OnPropertyChanged("USERHoraInicial");
                }
            }
        }
        private string _uSERHoraInicial;
    
        [DataMember]
        public string USERHoraFinal
        {
            get { return _uSERHoraFinal; }
            set
            {
                if (_uSERHoraFinal != value)
                {
                    _uSERHoraFinal = value;
                    OnPropertyChanged("USERHoraFinal");
                }
            }
        }
        private string _uSERHoraFinal;
    
        [DataMember]
        public Nullable<System.DateTime> USERDataApontamento
        {
            get { return _uSERDataApontamento; }
            set
            {
                if (_uSERDataApontamento != value)
                {
                    _uSERDataApontamento = value;
                    OnPropertyChanged("USERDataApontamento");
                }
            }
        }
        private Nullable<System.DateTime> _uSERDataApontamento;
    
        [DataMember]
        public string USERResponsavel
        {
            get { return _uSERResponsavel; }
            set
            {
                if (_uSERResponsavel != value)
                {
                    _uSERResponsavel = value;
                    OnPropertyChanged("USERResponsavel");
                }
            }
        }
        private string _uSERResponsavel;
    
        [DataMember]
        public string USERMinutoInicial
        {
            get { return _uSERMinutoInicial; }
            set
            {
                if (_uSERMinutoInicial != value)
                {
                    _uSERMinutoInicial = value;
                    OnPropertyChanged("USERMinutoInicial");
                }
            }
        }
        private string _uSERMinutoInicial;
    
        [DataMember]
        public string USERMinutoFinal
        {
            get { return _uSERMinutoFinal; }
            set
            {
                if (_uSERMinutoFinal != value)
                {
                    _uSERMinutoFinal = value;
                    OnPropertyChanged("USERMinutoFinal");
                }
            }
        }
        private string _uSERMinutoFinal;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public OCORRENCIA OCORRENCIA
        {
            get { return _oCORRENCIA; }
            set
            {
                if (!ReferenceEquals(_oCORRENCIA, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (EmpCod != value.EmpCod || OcorCod != value.OcorCod)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _oCORRENCIA;
                    _oCORRENCIA = value;
                    FixupOCORRENCIA(previousValue);
                    OnNavigationPropertyChanged("OCORRENCIA");
                }
            }
        }
        private OCORRENCIA _oCORRENCIA;
    
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
            OCORRENCIA = null;
            PRODUTO1 = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupOCORRENCIA(OCORRENCIA previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ITEM_OCOR.Contains(this))
            {
                previousValue.ITEM_OCOR.Remove(this);
            }
    
            if (OCORRENCIA != null)
            {
                if (!OCORRENCIA.ITEM_OCOR.Contains(this))
                {
                    OCORRENCIA.ITEM_OCOR.Add(this);
                }
    
                EmpCod = OCORRENCIA.EmpCod;
                OcorCod = OCORRENCIA.OcorCod;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("OCORRENCIA")
                    && (ChangeTracker.OriginalValues["OCORRENCIA"] == OCORRENCIA))
                {
                    ChangeTracker.OriginalValues.Remove("OCORRENCIA");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("OCORRENCIA", previousValue);
                }
                if (OCORRENCIA != null && !OCORRENCIA.ChangeTracker.ChangeTrackingEnabled)
                {
                    OCORRENCIA.StartTracking();
                }
            }
        }
    
        private void FixupPRODUTO1(PRODUTO1 previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ITEM_OCOR.Contains(this))
            {
                previousValue.ITEM_OCOR.Remove(this);
            }
    
            if (PRODUTO1 != null)
            {
                if (!PRODUTO1.ITEM_OCOR.Contains(this))
                {
                    PRODUTO1.ITEM_OCOR.Add(this);
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
