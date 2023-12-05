using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiConcesionario.Models;

public partial class MiConcesionarioContext : DbContext
{
    public MiConcesionarioContext()
    {
    }

    public MiConcesionarioContext(DbContextOptions<MiConcesionarioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Coch> Coches { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-U12GTB1;Initial Catalog=MiConcesionario;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Clientes__D5946642C3395653");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Coch>(entity =>
        {
            entity.HasKey(e => e.Matricula).HasName("PK__Coches__0FB9FB4E57BD815C");

            entity.Property(e => e.Matricula).HasMaxLength(20);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__Usuarios__A9D10535127F1606");

            entity.Property(e => e.Email).HasMaxLength(255);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Ventas__BC1240BDFE665714");

            entity.Property(e => e.Matricula).HasMaxLength(20);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Venta)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Ventas__ClienteI__3C69FB99");

            entity.HasOne(d => d.MatriculaNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.Matricula)
                .HasConstraintName("FK__Ventas__Matricul__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
