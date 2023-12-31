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
    public partial class GEP_Acao: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int SemanaDoAno
        {
            get { return _semanaDoAno; }
            set
            {
                if (_semanaDoAno != value)
                {
                    _semanaDoAno = value;
                    OnPropertyChanged("SemanaDoAno");
                }
            }
        }
        private int _semanaDoAno;
    
        [DataMember]
        public int Ano
        {
            get { return _ano; }
            set
            {
                if (_ano != value)
                {
                    _ano = value;
                    OnPropertyChanged("Ano");
                }
            }
        }
        private int _ano;
    
        [DataMember]
        public string Acao
        {
            get { return _acao; }
            set
            {
                if (_acao != value)
                {
                    _acao = value;
                    OnPropertyChanged("Acao");
                }
            }
        }
        private string _acao;
    
        [DataMember]
        public string Pilar
        {
            get { return _pilar; }
            set
            {
                if (_pilar != value)
                {
                    _pilar = value;
                    OnPropertyChanged("Pilar");
                }
            }
        }
        private string _pilar;
    
        [DataMember]
        public string Objetivo
        {
            get { return _objetivo; }
            set
            {
                if (_objetivo != value)
                {
                    _objetivo = value;
                    OnPropertyChanged("Objetivo");
                }
            }
        }
        private string _objetivo;
    
        [DataMember]
        public string Comentarios
        {
            get { return _comentarios; }
            set
            {
                if (_comentarios != value)
                {
                    _comentarios = value;
                    OnPropertyChanged("Comentarios");
                }
            }
        }
        private string _comentarios;
    
        [DataMember]
        public System.DateTime Prazo
        {
            get { return _prazo; }
            set
            {
                if (_prazo != value)
                {
                    _prazo = value;
                    OnPropertyChanged("Prazo");
                }
            }
        }
        private System.DateTime _prazo;
    
        [DataMember]
        public int Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        private int _status;
    
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
    
        [DataMember]
        public System.DateTime DataHoraCadastro
        {
            get { return _dataHoraCadastro; }
            set
            {
                if (_dataHoraCadastro != value)
                {
                    _dataHoraCadastro = value;
                    OnPropertyChanged("DataHoraCadastro");
                }
            }
        }
        private System.DateTime _dataHoraCadastro;
    
        [DataMember]
        public int SemanaOriginal
        {
            get { return _semanaOriginal; }
            set
            {
                if (_semanaOriginal != value)
                {
                    _semanaOriginal = value;
                    OnPropertyChanged("SemanaOriginal");
                }
            }
        }
        private int _semanaOriginal;
    
        [DataMember]
        public int AnoOriginal
        {
            get { return _anoOriginal; }
            set
            {
                if (_anoOriginal != value)
                {
                    _anoOriginal = value;
                    OnPropertyChanged("AnoOriginal");
                }
            }
        }
        private int _anoOriginal;

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
