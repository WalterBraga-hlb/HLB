using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace MvcAppHylinedoBrasilMobile.Models
{
    public class LayoutDb : DbContext
    {
        public DbSet<LayoutDiarioExpedicao> DiarioExpedicao { get; set; }
        public DbSet<LayoutDiarioExpedicaoPai> DiarioExpedicaoPai { get; set; }

        public LayoutDb()
        {
            //this is telling EF to not create the DB
            Database.SetInitializer<LayoutDb>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
        }
    }
}