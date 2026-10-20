using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WatchShop.GeneratedModels;

public partial class ScaffoldedDbContext : DbContext
{
    public ScaffoldedDbContext()
    {
    }

    public ScaffoldedDbContext(DbContextOptions<ScaffoldedDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwWatchesFull> VwWatchesFulls { get; set; }

    public virtual DbSet<Watch> Watches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=WatchShopDB;Integrated Security=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC070A25F629");

            entity.HasIndex(e => e.Name, "UQ__Categori__737584F660BD0988").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NameKey).HasMaxLength(50);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Manufact__3214EC07406E6C69");

            entity.HasIndex(e => e.Name, "UQ__Manufact__737584F61454DF86").IsUnique();

            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07715803EE");

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Новый");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC07C9A11113");

            entity.ToTable(tb => tb.HasTrigger("trg_OrderItems_UpdateStock"));

            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItems_Order");

            entity.HasOne(d => d.Watch).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.WatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Watch");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07BA40BBCE");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E427935D11").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Client");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<VwWatchesFull>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_WatchesFull");

            entity.Property(e => e.CategoryKey).HasMaxLength(50);
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.ImagePath).HasMaxLength(500);
            entity.Property(e => e.ManufacturerName).HasMaxLength(100);
            entity.Property(e => e.Material).HasMaxLength(50);
            entity.Property(e => e.MovementType).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ShortName).HasMaxLength(100);
            entity.Property(e => e.Size).HasMaxLength(20);
            entity.Property(e => e.WaterResistance).HasMaxLength(20);
        });

        modelBuilder.Entity<Watch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Watches__3214EC077CC37335");

            entity.ToTable(tb => tb.HasTrigger("trg_Watches_UpdateInStock"));

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.ImagePath).HasMaxLength(500);
            entity.Property(e => e.InStock).HasDefaultValue(true);
            entity.Property(e => e.Material).HasMaxLength(50);
            entity.Property(e => e.MovementType).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ShortName).HasMaxLength(100);
            entity.Property(e => e.Size).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.WaterResistance).HasMaxLength(20);

            entity.HasOne(d => d.Category).WithMany(p => p.Watches)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Watches_Category");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Watches)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_Watches_Manufacturer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
