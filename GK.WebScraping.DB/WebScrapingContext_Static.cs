using System;
using GK.WebScraping.Utilities;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GK.WebScraping.DB

{
    public partial class WebScrapingContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {

            EncryptionUtility encryptionUtility = new EncryptionUtility();

            if (!builder.IsConfigured)
            {
                if (Environment.MachineName == "GK-WS1")
                    builder.UseSqlServer(
                        String.Format("Server={0};Database={1};User Id={2};Password={3}",
                            Configuration.Instance.SqlServer.Host,
                            Configuration.Instance.SqlServer.Database,
                            Configuration.Instance.SqlServer.Username,
                            encryptionUtility.Decrypt(Configuration.Instance.SqlServer.PasswordEncrypted)
                        ));
                else
                    builder.UseSqlServer("Server=localhost;Database=WebScraping;Trusted_Connection=True");

            }

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Page>(entity =>
            //{

            //    entity
            //        .Property(e => e.PageId).ValueGeneratedOnAdd();

            //    entity.HasAlternateKey(x => x.Url);
            //});
        }
    }
}
