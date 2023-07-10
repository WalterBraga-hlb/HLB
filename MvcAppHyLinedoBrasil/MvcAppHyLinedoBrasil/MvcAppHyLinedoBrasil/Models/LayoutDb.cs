using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;

namespace MvcAppHyLinedoBrasil.Models
{
    public class LayoutDb : DbContext
    {
        public DbSet<LayoutDDASegmentoG> LinhasSegmentoG { get; set; }
        public DbSet<ArquivosLidos> ArquivosLidosDDA { get; set; }
        public DbSet<LayoutOrdemProducao> OrdemProducao { get; set; }
        public DbSet<LayoutPedidoPlanilha> PedidoPlanilha { get; set; }

        public LayoutDb()
        {
            //this is telling EF to not create the DB
            //Database.SetInitializer<LayoutDb>(true);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            modelBuilder.Entity<LayoutPedidoPlanilha>().Property(o => o.ValorUnitario).HasPrecision(14, 4);
            base.OnModelCreating(modelBuilder);
        }
    }
}