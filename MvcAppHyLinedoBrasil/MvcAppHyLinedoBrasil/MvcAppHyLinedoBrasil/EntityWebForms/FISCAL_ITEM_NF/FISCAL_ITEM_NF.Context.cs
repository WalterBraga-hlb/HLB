﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace MvcAppHyLinedoBrasil.EntityWebForms.FISCAL_ITEM_NF
{
    public partial class Apolo10Entities : ObjectContext
    {
        public const string ConnectionString = "name=Apolo10Entities";
        public const string ContainerName = "Apolo10Entities";
    
        #region Constructors
    
        public Apolo10Entities()
            : base(ConnectionString, ContainerName)
        {
            Initialize();
        }
    
        public Apolo10Entities(string connectionString)
            : base(connectionString, ContainerName)
        {
            Initialize();
        }
    
        public Apolo10Entities(EntityConnection connection)
            : base(connection, ContainerName)
        {
            Initialize();
        }
    
        private void Initialize()
        {
            // Creating proxies requires the use of the ProxyDataContractResolver and
            // may allow lazy loading which can expand the loaded graph during serialization.
            ContextOptions.ProxyCreationEnabled = false;
            ObjectMaterialized += new ObjectMaterializedEventHandler(HandleObjectMaterialized);
        }
    
        private void HandleObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity as IObjectWithChangeTracker;
            if (entity != null)
            {
                bool changeTrackingEnabled = entity.ChangeTracker.ChangeTrackingEnabled;
                try
                {
                    entity.MarkAsUnchanged();
                }
                finally
                {
                    entity.ChangeTracker.ChangeTrackingEnabled = changeTrackingEnabled;
                }
                this.StoreReferenceKeyValues(entity);
            }
        }
    
        #endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<FISCAL_ITEM_NF> FISCAL_ITEM_NF
        {
            get { return _fISCAL_ITEM_NF  ?? (_fISCAL_ITEM_NF = CreateObjectSet<FISCAL_ITEM_NF>("FISCAL_ITEM_NF")); }
        }
        private ObjectSet<FISCAL_ITEM_NF> _fISCAL_ITEM_NF;
    
        public ObjectSet<FISCAL_NF> FISCAL_NF
        {
            get { return _fISCAL_NF  ?? (_fISCAL_NF = CreateObjectSet<FISCAL_NF>("FISCAL_NF")); }
        }
        private ObjectSet<FISCAL_NF> _fISCAL_NF;

        #endregion

    }
}
