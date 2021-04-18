using System;
using GK.WebScraping.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GK.WebScraping.DB

{
    public partial class WebScrapingContext : DbContext
    {
        public WebScrapingContext()
        {
        }

        public WebScrapingContext(DbContextOptions<WebScrapingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<Map> Maps { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductDatum> ProductData { get; set; }
        public virtual DbSet<Store> Stores { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Turkish_CI_AS");

            modelBuilder.Entity<Field>(entity =>
            {
                entity.ToTable("Field");

                entity.Property(e => e.FieldId)
                    .ValueGeneratedNever()
                    .HasColumnName("fieldID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate");

                entity.Property(e => e.DataType).HasColumnName("dataType");

                entity.Property(e => e.FieldType).HasColumnName("fieldType");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Map>(entity =>
            {
                entity.HasKey(e => new { e.FieldId, e.PageId });

                entity.ToTable("Map");

                entity.Property(e => e.FieldId).HasColumnName("fieldID");

                entity.Property(e => e.PageId).HasColumnName("pageID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate");

                entity.Property(e => e.Map1)
                    .IsRequired()
                    .HasColumnName("map");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.Maps)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Map_Field");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.Maps)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Map_Link");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Page");

                entity.Property(e => e.PageId)
                    .ValueGeneratedNever()
                    .HasColumnName("pageID");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate");

                entity.Property(e => e.DeleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("deleteDate");

                entity.Property(e => e.LastReadDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastReadDate");

                entity.Property(e => e.MapStatus).HasColumnName("mapStatus");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("storeID");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Pages)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Link_Store");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId)
                    .ValueGeneratedNever()
                    .HasColumnName("productID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ProductDatum>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.FieldId });

                entity.Property(e => e.ProductId).HasColumnName("productID");

                entity.Property(e => e.FieldId).HasColumnName("fieldID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate");

                entity.Property(e => e.DateTimeData)
                    .HasColumnType("datetime")
                    .HasColumnName("dateTimeData");

                entity.Property(e => e.IntData).HasColumnName("intData");

                entity.Property(e => e.StringData).HasColumnName("stringData");

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.ProductData)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductData_Field");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductData)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductData_Product");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.Property(e => e.StoreId)
                    .ValueGeneratedNever()
                    .HasColumnName("storeID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate");

                entity.Property(e => e.DeleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("deleteDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.RootUrl)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("rootUrl");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
