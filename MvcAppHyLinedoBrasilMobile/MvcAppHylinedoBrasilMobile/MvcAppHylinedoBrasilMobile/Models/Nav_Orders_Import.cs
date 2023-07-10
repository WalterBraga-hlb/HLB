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
    public partial class Nav_Orders_Import: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public Nullable<System.DateTime> hatch_date
        {
            get { return _hatch_date; }
            set
            {
                if (_hatch_date != value)
                {
                    _hatch_date = value;
                    OnPropertyChanged("hatch_date");
                }
            }
        }
        private Nullable<System.DateTime> _hatch_date;
    
        [DataMember]
        public Nullable<System.DateTime> del_date
        {
            get { return _del_date; }
            set
            {
                if (_del_date != value)
                {
                    _del_date = value;
                    OnPropertyChanged("del_date");
                }
            }
        }
        private Nullable<System.DateTime> _del_date;
    
        [DataMember]
        public string delivery
        {
            get { return _delivery; }
            set
            {
                if (_delivery != value)
                {
                    _delivery = value;
                    OnPropertyChanged("delivery");
                }
            }
        }
        private string _delivery;
    
        [DataMember]
        public string farm_loc
        {
            get { return _farm_loc; }
            set
            {
                if (_farm_loc != value)
                {
                    _farm_loc = value;
                    OnPropertyChanged("farm_loc");
                }
            }
        }
        private string _farm_loc;
    
        [DataMember]
        public Nullable<int> book_id
        {
            get { return _book_id; }
            set
            {
                if (_book_id != value)
                {
                    _book_id = value;
                    OnPropertyChanged("book_id");
                }
            }
        }
        private Nullable<int> _book_id;
    
        [DataMember]
        public Nullable<System.DateTime> cal_date
        {
            get { return _cal_date; }
            set
            {
                if (_cal_date != value)
                {
                    _cal_date = value;
                    OnPropertyChanged("cal_date");
                }
            }
        }
        private Nullable<System.DateTime> _cal_date;
    
        [DataMember]
        public string customer
        {
            get { return _customer; }
            set
            {
                if (_customer != value)
                {
                    _customer = value;
                    OnPropertyChanged("customer");
                }
            }
        }
        private string _customer;
    
        [DataMember]
        public Nullable<int> item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged("item");
                }
            }
        }
        private Nullable<int> _item;
    
        [DataMember]
        public Nullable<int> quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged("quantity");
                }
            }
        }
        private Nullable<int> _quantity;
    
        [DataMember]
        public Nullable<decimal> price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged("price");
                }
            }
        }
        private Nullable<decimal> _price;
    
        [DataMember]
        public Nullable<int> orderno
        {
            get { return _orderno; }
            set
            {
                if (_orderno != value)
                {
                    _orderno = value;
                    OnPropertyChanged("orderno");
                }
            }
        }
        private Nullable<int> _orderno;
    
        [DataMember]
        public string order_type
        {
            get { return _order_type; }
            set
            {
                if (_order_type != value)
                {
                    _order_type = value;
                    OnPropertyChanged("order_type");
                }
            }
        }
        private string _order_type;
    
        [DataMember]
        public string comment_1
        {
            get { return _comment_1; }
            set
            {
                if (_comment_1 != value)
                {
                    _comment_1 = value;
                    OnPropertyChanged("comment_1");
                }
            }
        }
        private string _comment_1;
    
        [DataMember]
        public string comment_2
        {
            get { return _comment_2; }
            set
            {
                if (_comment_2 != value)
                {
                    _comment_2 = value;
                    OnPropertyChanged("comment_2");
                }
            }
        }
        private string _comment_2;
    
        [DataMember]
        public string comment_3
        {
            get { return _comment_3; }
            set
            {
                if (_comment_3 != value)
                {
                    _comment_3 = value;
                    OnPropertyChanged("comment_3");
                }
            }
        }
        private string _comment_3;
    
        [DataMember]
        public string location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged("location");
                }
            }
        }
        private string _location;
    
        [DataMember]
        public string accountno
        {
            get { return _accountno; }
            set
            {
                if (_accountno != value)
                {
                    _accountno = value;
                    OnPropertyChanged("accountno");
                }
            }
        }
        private string _accountno;
    
        [DataMember]
        public string alt_desc
        {
            get { return _alt_desc; }
            set
            {
                if (_alt_desc != value)
                {
                    _alt_desc = value;
                    OnPropertyChanged("alt_desc");
                }
            }
        }
        private string _alt_desc;
    
        [DataMember]
        public string item_ord
        {
            get { return _item_ord; }
            set
            {
                if (_item_ord != value)
                {
                    _item_ord = value;
                    OnPropertyChanged("item_ord");
                }
            }
        }
        private string _item_ord;
    
        [DataMember]
        public string creatdby
        {
            get { return _creatdby; }
            set
            {
                if (_creatdby != value)
                {
                    _creatdby = value;
                    OnPropertyChanged("creatdby");
                }
            }
        }
        private string _creatdby;
    
        [DataMember]
        public Nullable<System.DateTime> datecrtd
        {
            get { return _datecrtd; }
            set
            {
                if (_datecrtd != value)
                {
                    _datecrtd = value;
                    OnPropertyChanged("datecrtd");
                }
            }
        }
        private Nullable<System.DateTime> _datecrtd;
    
        [DataMember]
        public string modifdby
        {
            get { return _modifdby; }
            set
            {
                if (_modifdby != value)
                {
                    _modifdby = value;
                    OnPropertyChanged("modifdby");
                }
            }
        }
        private string _modifdby;
    
        [DataMember]
        public Nullable<System.DateTime> datemodi
        {
            get { return _datemodi; }
            set
            {
                if (_datemodi != value)
                {
                    _datemodi = value;
                    OnPropertyChanged("datemodi");
                }
            }
        }
        private Nullable<System.DateTime> _datemodi;
    
        [DataMember]
        public string itm_ddate
        {
            get { return _itm_ddate; }
            set
            {
                if (_itm_ddate != value)
                {
                    _itm_ddate = value;
                    OnPropertyChanged("itm_ddate");
                }
            }
        }
        private string _itm_ddate;
    
        [DataMember]
        public Nullable<int> vat
        {
            get { return _vat; }
            set
            {
                if (_vat != value)
                {
                    _vat = value;
                    OnPropertyChanged("vat");
                }
            }
        }
        private Nullable<int> _vat;
    
        [DataMember]
        public string salesrep
        {
            get { return _salesrep; }
            set
            {
                if (_salesrep != value)
                {
                    _salesrep = value;
                    OnPropertyChanged("salesrep");
                }
            }
        }
        private string _salesrep;
    
        [DataMember]
        public Nullable<int> bookkey
        {
            get { return _bookkey; }
            set
            {
                if (_bookkey != value)
                {
                    _bookkey = value;
                    OnPropertyChanged("bookkey");
                }
            }
        }
        private Nullable<int> _bookkey;
    
        [DataMember]
        public string item_desc
        {
            get { return _item_desc; }
            set
            {
                if (_item_desc != value)
                {
                    _item_desc = value;
                    OnPropertyChanged("item_desc");
                }
            }
        }
        private string _item_desc;
    
        [DataMember]
        public string variety
        {
            get { return _variety; }
            set
            {
                if (_variety != value)
                {
                    _variety = value;
                    OnPropertyChanged("variety");
                }
            }
        }
        private string _variety;
    
        [DataMember]
        public string form
        {
            get { return _form; }
            set
            {
                if (_form != value)
                {
                    _form = value;
                    OnPropertyChanged("form");
                }
            }
        }
        private string _form;
    
        [DataMember]
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("name");
                }
            }
        }
        private string _name;
    
        [DataMember]
        public Nullable<int> contact_no
        {
            get { return _contact_no; }
            set
            {
                if (_contact_no != value)
                {
                    _contact_no = value;
                    OnPropertyChanged("contact_no");
                }
            }
        }
        private Nullable<int> _contact_no;
    
        [DataMember]
        public string shpname
        {
            get { return _shpname; }
            set
            {
                if (_shpname != value)
                {
                    _shpname = value;
                    OnPropertyChanged("shpname");
                }
            }
        }
        private string _shpname;
    
        [DataMember]
        public string salesman
        {
            get { return _salesman; }
            set
            {
                if (_salesman != value)
                {
                    _salesman = value;
                    OnPropertyChanged("salesman");
                }
            }
        }
        private string _salesman;
    
        [DataMember]
        public string account_no
        {
            get { return _account_no; }
            set
            {
                if (_account_no != value)
                {
                    _account_no = value;
                    OnPropertyChanged("account_no");
                }
            }
        }
        private string _account_no;
    
        [DataMember]
        public string sl_code
        {
            get { return _sl_code; }
            set
            {
                if (_sl_code != value)
                {
                    _sl_code = value;
                    OnPropertyChanged("sl_code");
                }
            }
        }
        private string _sl_code;
    
        [DataMember]
        public string street_1
        {
            get { return _street_1; }
            set
            {
                if (_street_1 != value)
                {
                    _street_1 = value;
                    OnPropertyChanged("street_1");
                }
            }
        }
        private string _street_1;
    
        [DataMember]
        public string street_2
        {
            get { return _street_2; }
            set
            {
                if (_street_2 != value)
                {
                    _street_2 = value;
                    OnPropertyChanged("street_2");
                }
            }
        }
        private string _street_2;
    
        [DataMember]
        public string city
        {
            get { return _city; }
            set
            {
                if (_city != value)
                {
                    _city = value;
                    OnPropertyChanged("city");
                }
            }
        }
        private string _city;
    
        [DataMember]
        public string state
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged("state");
                }
            }
        }
        private string _state;
    
        [DataMember]
        public string zip
        {
            get { return _zip; }
            set
            {
                if (_zip != value)
                {
                    _zip = value;
                    OnPropertyChanged("zip");
                }
            }
        }
        private string _zip;
    
        [DataMember]
        public string country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    OnPropertyChanged("country");
                }
            }
        }
        private string _country;
    
        [DataMember]
        public string usuario
        {
            get { return _usuario; }
            set
            {
                if (_usuario != value)
                {
                    _usuario = value;
                    OnPropertyChanged("usuario");
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
