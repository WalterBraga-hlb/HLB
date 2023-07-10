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

namespace MvcAppHyLinedoBrasil.Models.HLBAPP
{
    [DataContract(IsReference = true)]
    public partial class Prog_Diaria_Transp_Veiculos: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public Nullable<System.DateTime> DataProgramacao
        {
            get { return _dataProgramacao; }
            set
            {
                if (_dataProgramacao != value)
                {
                    _dataProgramacao = value;
                    OnPropertyChanged("DataProgramacao");
                }
            }
        }
        private Nullable<System.DateTime> _dataProgramacao;
    
        [DataMember]
        public Nullable<int> NumVeiculo
        {
            get { return _numVeiculo; }
            set
            {
                if (_numVeiculo != value)
                {
                    _numVeiculo = value;
                    OnPropertyChanged("NumVeiculo");
                }
            }
        }
        private Nullable<int> _numVeiculo;
    
        [DataMember]
        public string Placa
        {
            get { return _placa; }
            set
            {
                if (_placa != value)
                {
                    _placa = value;
                    OnPropertyChanged("Placa");
                }
            }
        }
        private string _placa;
    
        [DataMember]
        public string Motorista01
        {
            get { return _motorista01; }
            set
            {
                if (_motorista01 != value)
                {
                    _motorista01 = value;
                    OnPropertyChanged("Motorista01");
                }
            }
        }
        private string _motorista01;
    
        [DataMember]
        public string Motorista02
        {
            get { return _motorista02; }
            set
            {
                if (_motorista02 != value)
                {
                    _motorista02 = value;
                    OnPropertyChanged("Motorista02");
                }
            }
        }
        private string _motorista02;
    
        [DataMember]
        public Nullable<int> QuantidadeTotal
        {
            get { return _quantidadeTotal; }
            set
            {
                if (_quantidadeTotal != value)
                {
                    _quantidadeTotal = value;
                    OnPropertyChanged("QuantidadeTotal");
                }
            }
        }
        private Nullable<int> _quantidadeTotal;
    
        [DataMember]
        public Nullable<int> QuantidadePorCaixa
        {
            get { return _quantidadePorCaixa; }
            set
            {
                if (_quantidadePorCaixa != value)
                {
                    _quantidadePorCaixa = value;
                    OnPropertyChanged("QuantidadePorCaixa");
                }
            }
        }
        private Nullable<int> _quantidadePorCaixa;
    
        [DataMember]
        public Nullable<int> QunatidadeCaixa
        {
            get { return _qunatidadeCaixa; }
            set
            {
                if (_qunatidadeCaixa != value)
                {
                    _qunatidadeCaixa = value;
                    OnPropertyChanged("QunatidadeCaixa");
                }
            }
        }
        private Nullable<int> _qunatidadeCaixa;
    
        [DataMember]
        public Nullable<decimal> ValorTotal
        {
            get { return _valorTotal; }
            set
            {
                if (_valorTotal != value)
                {
                    _valorTotal = value;
                    OnPropertyChanged("ValorTotal");
                }
            }
        }
        private Nullable<decimal> _valorTotal;
    
        [DataMember]
        public string EmpresaTranportador
        {
            get { return _empresaTranportador; }
            set
            {
                if (_empresaTranportador != value)
                {
                    _empresaTranportador = value;
                    OnPropertyChanged("EmpresaTranportador");
                }
            }
        }
        private string _empresaTranportador;
    
        [DataMember]
        public string InicioCarregamentoEsperado
        {
            get { return _inicioCarregamentoEsperado; }
            set
            {
                if (_inicioCarregamentoEsperado != value)
                {
                    _inicioCarregamentoEsperado = value;
                    OnPropertyChanged("InicioCarregamentoEsperado");
                }
            }
        }
        private string _inicioCarregamentoEsperado;
    
        [DataMember]
        public string HorarioEntregaNF
        {
            get { return _horarioEntregaNF; }
            set
            {
                if (_horarioEntregaNF != value)
                {
                    _horarioEntregaNF = value;
                    OnPropertyChanged("HorarioEntregaNF");
                }
            }
        }
        private string _horarioEntregaNF;
    
        [DataMember]
        public string Tranportadora
        {
            get { return _tranportadora; }
            set
            {
                if (_tranportadora != value)
                {
                    _tranportadora = value;
                    OnPropertyChanged("Tranportadora");
                }
            }
        }
        private string _tranportadora;
    
        [DataMember]
        public Nullable<decimal> ValorKM
        {
            get { return _valorKM; }
            set
            {
                if (_valorKM != value)
                {
                    _valorKM = value;
                    OnPropertyChanged("ValorKM");
                }
            }
        }
        private Nullable<decimal> _valorKM;
    
        [DataMember]
        public string UnidadeBaseEmbarcador
        {
            get { return _unidadeBaseEmbarcador; }
            set
            {
                if (_unidadeBaseEmbarcador != value)
                {
                    _unidadeBaseEmbarcador = value;
                    OnPropertyChanged("UnidadeBaseEmbarcador");
                }
            }
        }
        private string _unidadeBaseEmbarcador;
    
        [DataMember]
        public Nullable<int> CargaLiberada
        {
            get { return _cargaLiberada; }
            set
            {
                if (_cargaLiberada != value)
                {
                    _cargaLiberada = value;
                    OnPropertyChanged("CargaLiberada");
                }
            }
        }
        private Nullable<int> _cargaLiberada;
    
        [DataMember]
        public Nullable<System.DateTime> DataEmbarque
        {
            get { return _dataEmbarque; }
            set
            {
                if (_dataEmbarque != value)
                {
                    _dataEmbarque = value;
                    OnPropertyChanged("DataEmbarque");
                }
            }
        }
        private Nullable<System.DateTime> _dataEmbarque;
    
        [DataMember]
        public string AeroportoOrigem
        {
            get { return _aeroportoOrigem; }
            set
            {
                if (_aeroportoOrigem != value)
                {
                    _aeroportoOrigem = value;
                    OnPropertyChanged("AeroportoOrigem");
                }
            }
        }
        private string _aeroportoOrigem;
    
        [DataMember]
        public string HorarioChegadaAeroporto
        {
            get { return _horarioChegadaAeroporto; }
            set
            {
                if (_horarioChegadaAeroporto != value)
                {
                    _horarioChegadaAeroporto = value;
                    OnPropertyChanged("HorarioChegadaAeroporto");
                }
            }
        }
        private string _horarioChegadaAeroporto;
    
        [DataMember]
        public string Despachante
        {
            get { return _despachante; }
            set
            {
                if (_despachante != value)
                {
                    _despachante = value;
                    OnPropertyChanged("Despachante");
                }
            }
        }
        private string _despachante;
    
        [DataMember]
        public Nullable<System.DateTime> DataInicioVazio
        {
            get { return _dataInicioVazio; }
            set
            {
                if (_dataInicioVazio != value)
                {
                    _dataInicioVazio = value;
                    OnPropertyChanged("DataInicioVazio");
                }
            }
        }
        private Nullable<System.DateTime> _dataInicioVazio;
    
        [DataMember]
        public Nullable<int> IDCargaEmbarcador
        {
            get { return _iDCargaEmbarcador; }
            set
            {
                if (_iDCargaEmbarcador != value)
                {
                    _iDCargaEmbarcador = value;
                    OnPropertyChanged("IDCargaEmbarcador");
                }
            }
        }
        private Nullable<int> _iDCargaEmbarcador;
    
        [DataMember]
        public string InicioCarregamentoReal
        {
            get { return _inicioCarregamentoReal; }
            set
            {
                if (_inicioCarregamentoReal != value)
                {
                    _inicioCarregamentoReal = value;
                    OnPropertyChanged("InicioCarregamentoReal");
                }
            }
        }
        private string _inicioCarregamentoReal;
    
        [DataMember]
        public string TerminoCarregamentoReal
        {
            get { return _terminoCarregamentoReal; }
            set
            {
                if (_terminoCarregamentoReal != value)
                {
                    _terminoCarregamentoReal = value;
                    OnPropertyChanged("TerminoCarregamentoReal");
                }
            }
        }
        private string _terminoCarregamentoReal;
    
        [DataMember]
        public Nullable<decimal> OdometroVeiculoDataEmbarque
        {
            get { return _odometroVeiculoDataEmbarque; }
            set
            {
                if (_odometroVeiculoDataEmbarque != value)
                {
                    _odometroVeiculoDataEmbarque = value;
                    OnPropertyChanged("OdometroVeiculoDataEmbarque");
                }
            }
        }
        private Nullable<decimal> _odometroVeiculoDataEmbarque;
    
        [DataMember]
        public string EntCod
        {
            get { return _entCod; }
            set
            {
                if (_entCod != value)
                {
                    _entCod = value;
                    OnPropertyChanged("EntCod");
                }
            }
        }
        private string _entCod;
    
        [DataMember]
        public Nullable<int> IdRoteiroTarget
        {
            get { return _idRoteiroTarget; }
            set
            {
                if (_idRoteiroTarget != value)
                {
                    _idRoteiroTarget = value;
                    OnPropertyChanged("IdRoteiroTarget");
                }
            }
        }
        private Nullable<int> _idRoteiroTarget;
    
        [DataMember]
        public Nullable<int> IdOperacaoTransporte
        {
            get { return _idOperacaoTransporte; }
            set
            {
                if (_idOperacaoTransporte != value)
                {
                    _idOperacaoTransporte = value;
                    OnPropertyChanged("IdOperacaoTransporte");
                }
            }
        }
        private Nullable<int> _idOperacaoTransporte;
    
        [DataMember]
        public string EntCodMotorista01
        {
            get { return _entCodMotorista01; }
            set
            {
                if (_entCodMotorista01 != value)
                {
                    _entCodMotorista01 = value;
                    OnPropertyChanged("EntCodMotorista01");
                }
            }
        }
        private string _entCodMotorista01;
    
        [DataMember]
        public string EntCodMotorista02
        {
            get { return _entCodMotorista02; }
            set
            {
                if (_entCodMotorista02 != value)
                {
                    _entCodMotorista02 = value;
                    OnPropertyChanged("EntCodMotorista02");
                }
            }
        }
        private string _entCodMotorista02;
    
        [DataMember]
        public string EquipCodEstrVeiculo
        {
            get { return _equipCodEstrVeiculo; }
            set
            {
                if (_equipCodEstrVeiculo != value)
                {
                    _equipCodEstrVeiculo = value;
                    OnPropertyChanged("EquipCodEstrVeiculo");
                }
            }
        }
        private string _equipCodEstrVeiculo;

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
