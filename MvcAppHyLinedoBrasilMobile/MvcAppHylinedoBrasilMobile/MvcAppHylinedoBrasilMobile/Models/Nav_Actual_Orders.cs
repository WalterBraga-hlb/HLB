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
    public partial class Nav_Actual_Orders: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public string Week
        {
            get { return _week; }
            set
            {
                if (_week != value)
                {
                    _week = value;
                    OnPropertyChanged("Week");
                }
            }
        }
        private string _week;
    
        [DataMember]
        public Nullable<System.DateTime> HatchDate
        {
            get { return _hatchDate; }
            set
            {
                if (_hatchDate != value)
                {
                    _hatchDate = value;
                    OnPropertyChanged("HatchDate");
                }
            }
        }
        private Nullable<System.DateTime> _hatchDate;
    
        [DataMember]
        public Nullable<System.DateTime> DelDate
        {
            get { return _delDate; }
            set
            {
                if (_delDate != value)
                {
                    _delDate = value;
                    OnPropertyChanged("DelDate");
                }
            }
        }
        private Nullable<System.DateTime> _delDate;
    
        [DataMember]
        public string LocationCode
        {
            get { return _locationCode; }
            set
            {
                if (_locationCode != value)
                {
                    _locationCode = value;
                    OnPropertyChanged("LocationCode");
                }
            }
        }
        private string _locationCode;
    
        [DataMember]
        public string OrderNo
        {
            get { return _orderNo; }
            set
            {
                if (_orderNo != value)
                {
                    _orderNo = value;
                    OnPropertyChanged("OrderNo");
                }
            }
        }
        private string _orderNo;
    
        [DataMember]
        public string Country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    OnPropertyChanged("Country");
                }
            }
        }
        private string _country;
    
        [DataMember]
        public string Customer
        {
            get { return _customer; }
            set
            {
                if (_customer != value)
                {
                    _customer = value;
                    OnPropertyChanged("Customer");
                }
            }
        }
        private string _customer;
    
        [DataMember]
        public string Client
        {
            get { return _client; }
            set
            {
                if (_client != value)
                {
                    _client = value;
                    OnPropertyChanged("Client");
                }
            }
        }
        private string _client;
    
        [DataMember]
        public string Breed
        {
            get { return _breed; }
            set
            {
                if (_breed != value)
                {
                    _breed = value;
                    OnPropertyChanged("Breed");
                }
            }
        }
        private string _breed;
    
        [DataMember]
        public Nullable<decimal> UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged("UnitPrice");
                }
            }
        }
        private Nullable<decimal> _unitPrice;
    
        [DataMember]
        public Nullable<int> Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }
        private Nullable<int> _quantity;
    
        [DataMember]
        public string QuoteNo
        {
            get { return _quoteNo; }
            set
            {
                if (_quoteNo != value)
                {
                    _quoteNo = value;
                    OnPropertyChanged("QuoteNo");
                }
            }
        }
        private string _quoteNo;
    
        [DataMember]
        public string Gender
        {
            get { return _gender; }
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged("Gender");
                }
            }
        }
        private string _gender;
    
        [DataMember]
        public string Usuario
        {
            get { return _usuario; }
            set
            {
                if (_usuario != value)
                {
                    _usuario = value;
                    OnPropertyChanged("Usuario");
                }
            }
        }
        private string _usuario;

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