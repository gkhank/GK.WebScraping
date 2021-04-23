using System;
using System.Collections.Generic;
using GK.WebScraping.Utilities;
using Microsoft.EntityFrameworkCore;

namespace GK.WebScraping.DB

{
    public partial class WebScrapingContext : DbContext
    {

        public virtual ICollection<tvp_IntTable> Tvp_IntTables { get; set; }
        public virtual ICollection<tvp_StringTable> Tvp_StringTables { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {


            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(
                    String.Format("Server={0};Database={1};User Id={2};Password={3};",
                        Configuration.Instance.SqlServer.Host,
                        Configuration.Instance.SqlServer.Database,
                        Configuration.Instance.SqlServer.Username,
                        Configuration.Instance.SqlServer.PasswordEncrypted
                    ));

            }

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<tvp_IntTable>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("tvp_IntTable");
                entity.Property(x => x.Value).HasColumnName("[value]");
            })

            .Entity<tvp_StringTable>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("tvp_StringTable");
                entity.Property(x => x.Value).HasColumnName("[value]");
            });
        }
    }
}
