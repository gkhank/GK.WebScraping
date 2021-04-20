using System;
using GK.WebScraping.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GK.WebScraping.DB

{
    public partial class WebScrapingContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {

            if (!builder.IsConfigured)
            {
                if (Environment.MachineName == "GK-WS1")
                    builder.UseSqlServer("Server=ec2-13-48-31-67.eu-north-1.compute.amazonaws.com;Database=WebScraping;User Id=SQL_Application;Password=*lZ[}0mB]*)00(o;");
                else
                    builder.UseSqlServer("Server=localhost;Database=WebScraping;Trusted_Connection=True");

            }

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Page>()
                    .HasIndex(u => u.Url)
                    .IsUnique();
        }
    }
}
