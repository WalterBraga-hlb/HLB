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

namespace MvcAppHylinedoBrasilMobile.Models
{
    [DataContract(IsReference = true)]
    public partial class CHIC_Programacoes: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public Nullable<int> COD_CLI
        {
            get { return _cOD_CLI; }
            set
            {
                if (_cOD_CLI != value)
                {
                    _cOD_CLI = value;
                    OnPropertyChanged("COD_CLI");
                }
            }
        }
        private Nullable<int> _cOD_CLI;
    
        [DataMember]
        public string CODAPOLO
        {
            get { return _cODAPOLO; }
            set
            {
                if (_cODAPOLO != value)
                {
                    _cODAPOLO = value;
                    OnPropertyChanged("CODAPOLO");
                }
            }
        }
        private string _cODAPOLO;
    
        [DataMember]
        public string NOME_RAZ
        {
            get { return _nOME_RAZ; }
            set
            {
                if (_nOME_RAZ != value)
                {
                    _nOME_RAZ = value;
                    OnPropertyChanged("NOME_RAZ");
                }
            }
        }
        private string _nOME_RAZ;
    
        [DataMember]
        public string TP_END
        {
            get { return _tP_END; }
            set
            {
                if (_tP_END != value)
                {
                    _tP_END = value;
                    OnPropertyChanged("TP_END");
                }
            }
        }
        private string _tP_END;
    
        [DataMember]
        public Nullable<int> COD_END
        {
            get { return _cOD_END; }
            set
            {
                if (_cOD_END != value)
                {
                    _cOD_END = value;
                    OnPropertyChanged("COD_END");
                }
            }
        }
        private Nullable<int> _cOD_END;
    
        [DataMember]
        public string EE
        {
            get { return _eE; }
            set
            {
                if (_eE != value)
                {
                    _eE = value;
                    OnPropertyChanged("EE");
                }
            }
        }
        private string _eE;
    
        [DataMember]
        public Nullable<System.DateTime> DATA
        {
            get { return _dATA; }
            set
            {
                if (_dATA != value)
                {
                    _dATA = value;
                    OnPropertyChanged("DATA");
                }
            }
        }
        private Nullable<System.DateTime> _dATA;
    
        [DataMember]
        public Nullable<int> VOLUME
        {
            get { return _vOLUME; }
            set
            {
                if (_vOLUME != value)
                {
                    _vOLUME = value;
                    OnPropertyChanged("VOLUME");
                }
            }
        }
        private Nullable<int> _vOLUME;
    
        [DataMember]
        public Nullable<decimal> PRECO
        {
            get { return _pRECO; }
            set
            {
                if (_pRECO != value)
                {
                    _pRECO = value;
                    OnPropertyChanged("PRECO");
                }
            }
        }
        private Nullable<decimal> _pRECO;
    
        [DataMember]
        public string COND_PGT
        {
            get { return _cOND_PGT; }
            set
            {
                if (_cOND_PGT != value)
                {
                    _cOND_PGT = value;
                    OnPropertyChanged("COND_PGT");
                }
            }
        }
        private string _cOND_PGT;
    
        [DataMember]
        public string COND_TXT
        {
            get { return _cOND_TXT; }
            set
            {
                if (_cOND_TXT != value)
                {
                    _cOND_TXT = value;
                    OnPropertyChanged("COND_TXT");
                }
            }
        }
        private string _cOND_TXT;
    
        [DataMember]
        public Nullable<int> COD_REP
        {
            get { return _cOD_REP; }
            set
            {
                if (_cOD_REP != value)
                {
                    _cOD_REP = value;
                    OnPropertyChanged("COD_REP");
                }
            }
        }
        private Nullable<int> _cOD_REP;
    
        [DataMember]
        public string COD_REP_APOLO
        {
            get { return _cOD_REP_APOLO; }
            set
            {
                if (_cOD_REP_APOLO != value)
                {
                    _cOD_REP_APOLO = value;
                    OnPropertyChanged("COD_REP_APOLO");
                }
            }
        }
        private string _cOD_REP_APOLO;
    
        [DataMember]
        public Nullable<int> PRODUTO
        {
            get { return _pRODUTO; }
            set
            {
                if (_pRODUTO != value)
                {
                    _pRODUTO = value;
                    OnPropertyChanged("PRODUTO");
                }
            }
        }
        private Nullable<int> _pRODUTO;
    
        [DataMember]
        public string DESCRICAO_PROD
        {
            get { return _dESCRICAO_PROD; }
            set
            {
                if (_dESCRICAO_PROD != value)
                {
                    _dESCRICAO_PROD = value;
                    OnPropertyChanged("DESCRICAO_PROD");
                }
            }
        }
        private string _dESCRICAO_PROD;
    
        [DataMember]
        public string COD_EMB
        {
            get { return _cOD_EMB; }
            set
            {
                if (_cOD_EMB != value)
                {
                    _cOD_EMB = value;
                    OnPropertyChanged("COD_EMB");
                }
            }
        }
        private string _cOD_EMB;
    
        [DataMember]
        public Nullable<int> QT_EMB
        {
            get { return _qT_EMB; }
            set
            {
                if (_qT_EMB != value)
                {
                    _qT_EMB = value;
                    OnPropertyChanged("QT_EMB");
                }
            }
        }
        private Nullable<int> _qT_EMB;
    
        [DataMember]
        public Nullable<int> CDVCNAV
        {
            get { return _cDVCNAV; }
            set
            {
                if (_cDVCNAV != value)
                {
                    _cDVCNAV = value;
                    OnPropertyChanged("CDVCNAV");
                }
            }
        }
        private Nullable<int> _cDVCNAV;
    
        [DataMember]
        public string NMVCNAV
        {
            get { return _nMVCNAV; }
            set
            {
                if (_nMVCNAV != value)
                {
                    _nMVCNAV = value;
                    OnPropertyChanged("NMVCNAV");
                }
            }
        }
        private string _nMVCNAV;
    
        [DataMember]
        public Nullable<int> CDVCNAV1
        {
            get { return _cDVCNAV1; }
            set
            {
                if (_cDVCNAV1 != value)
                {
                    _cDVCNAV1 = value;
                    OnPropertyChanged("CDVCNAV1");
                }
            }
        }
        private Nullable<int> _cDVCNAV1;
    
        [DataMember]
        public string NMVCNAV1
        {
            get { return _nMVCNAV1; }
            set
            {
                if (_nMVCNAV1 != value)
                {
                    _nMVCNAV1 = value;
                    OnPropertyChanged("NMVCNAV1");
                }
            }
        }
        private string _nMVCNAV1;
    
        [DataMember]
        public Nullable<int> CDVCNAV2
        {
            get { return _cDVCNAV2; }
            set
            {
                if (_cDVCNAV2 != value)
                {
                    _cDVCNAV2 = value;
                    OnPropertyChanged("CDVCNAV2");
                }
            }
        }
        private Nullable<int> _cDVCNAV2;
    
        [DataMember]
        public string NMVCNAV2
        {
            get { return _nMVCNAV2; }
            set
            {
                if (_nMVCNAV2 != value)
                {
                    _nMVCNAV2 = value;
                    OnPropertyChanged("NMVCNAV2");
                }
            }
        }
        private string _nMVCNAV2;
    
        [DataMember]
        public string CDVCNAV3
        {
            get { return _cDVCNAV3; }
            set
            {
                if (_cDVCNAV3 != value)
                {
                    _cDVCNAV3 = value;
                    OnPropertyChanged("CDVCNAV3");
                }
            }
        }
        private string _cDVCNAV3;
    
        [DataMember]
        public string NMVCNAV3
        {
            get { return _nMVCNAV3; }
            set
            {
                if (_nMVCNAV3 != value)
                {
                    _nMVCNAV3 = value;
                    OnPropertyChanged("NMVCNAV3");
                }
            }
        }
        private string _nMVCNAV3;
    
        [DataMember]
        public int ID
        {
            get { return _iD; }
            set
            {
                if (_iD != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ID' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _iD = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        private int _iD;
    
        [DataMember]
        public Nullable<System.DateTime> Importacao
        {
            get { return _importacao; }
            set
            {
                if (_importacao != value)
                {
                    _importacao = value;
                    OnPropertyChanged("Importacao");
                }
            }
        }
        private Nullable<System.DateTime> _importacao;
    
        [DataMember]
        public Nullable<decimal> DSCTO
        {
            get { return _dSCTO; }
            set
            {
                if (_dSCTO != value)
                {
                    _dSCTO = value;
                    OnPropertyChanged("DSCTO");
                }
            }
        }
        private Nullable<decimal> _dSCTO;

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
        }

        #endregion

    }
}
