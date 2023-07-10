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
    [KnownType(typeof(Config_Clientes_AniPlan))]
    public partial class Config_Clientes_AniPlan_Item: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
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
        public int IDConfigClientesAniPlan
        {
            get { return _iDConfigClientesAniPlan; }
            set
            {
                if (_iDConfigClientesAniPlan != value)
                {
                    ChangeTracker.RecordOriginalValue("IDConfigClientesAniPlan", _iDConfigClientesAniPlan);
                    if (!IsDeserializing)
                    {
                        if (Config_Clientes_AniPlan != null && Config_Clientes_AniPlan.ID != value)
                        {
                            Config_Clientes_AniPlan = null;
                        }
                    }
                    _iDConfigClientesAniPlan = value;
                    OnPropertyChanged("IDConfigClientesAniPlan");
                }
            }
        }
        private int _iDConfigClientesAniPlan;
    
        [DataMember]
        public string Empresa
        {
            get { return _empresa; }
            set
            {
                if (_empresa != value)
                {
                    _empresa = value;
                    OnPropertyChanged("Empresa");
                }
            }
        }
        private string _empresa;
    
        [DataMember]
        public string Tipo
        {
            get { return _tipo; }
            set
            {
                if (_tipo != value)
                {
                    _tipo = value;
                    OnPropertyChanged("Tipo");
                }
            }
        }
        private string _tipo;
    
        [DataMember]
        public string Codigo
        {
            get { return _codigo; }
            set
            {
                if (_codigo != value)
                {
                    _codigo = value;
                    OnPropertyChanged("Codigo");
                }
            }
        }
        private string _codigo;
    
        [DataMember]
        public decimal PrecoUnitario
        {
            get { return _precoUnitario; }
            set
            {
                if (_precoUnitario != value)
                {
                    _precoUnitario = value;
                    OnPropertyChanged("PrecoUnitario");
                }
            }
        }
        private decimal _precoUnitario;
    
        [DataMember]
        public string SelecionadoPedido
        {
            get { return _selecionadoPedido; }
            set
            {
                if (_selecionadoPedido != value)
                {
                    _selecionadoPedido = value;
                    OnPropertyChanged("SelecionadoPedido");
                }
            }
        }
        private string _selecionadoPedido;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Config_Clientes_AniPlan Config_Clientes_AniPlan
        {
            get { return _config_Clientes_AniPlan; }
            set
            {
                if (!ReferenceEquals(_config_Clientes_AniPlan, value))
                {
                    var previousValue = _config_Clientes_AniPlan;
                    _config_Clientes_AniPlan = value;
                    FixupConfig_Clientes_AniPlan(previousValue);
                    OnNavigationPropertyChanged("Config_Clientes_AniPlan");
                }
            }
        }
        private Config_Clientes_AniPlan _config_Clientes_AniPlan;

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
            Config_Clientes_AniPlan = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupConfig_Clientes_AniPlan(Config_Clientes_AniPlan previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Config_Clientes_AniPlan_Item.Contains(this))
            {
                previousValue.Config_Clientes_AniPlan_Item.Remove(this);
            }
    
            if (Config_Clientes_AniPlan != null)
            {
                if (!Config_Clientes_AniPlan.Config_Clientes_AniPlan_Item.Contains(this))
                {
                    Config_Clientes_AniPlan.Config_Clientes_AniPlan_Item.Add(this);
                }
    
                IDConfigClientesAniPlan = Config_Clientes_AniPlan.ID;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Config_Clientes_AniPlan")
                    && (ChangeTracker.OriginalValues["Config_Clientes_AniPlan"] == Config_Clientes_AniPlan))
                {
                    ChangeTracker.OriginalValues.Remove("Config_Clientes_AniPlan");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Config_Clientes_AniPlan", previousValue);
                }
                if (Config_Clientes_AniPlan != null && !Config_Clientes_AniPlan.ChangeTracker.ChangeTrackingEnabled)
                {
                    Config_Clientes_AniPlan.StartTracking();
                }
            }
        }

        #endregion

    }
}
