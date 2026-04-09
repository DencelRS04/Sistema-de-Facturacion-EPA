using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Models;

namespace Sistema_de_Facturacion_EPA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CategoriaProducto> CategoriaProductos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraDetalle> CompraDetalles { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoriaProducto>().HasKey(x => x.IdCategoria);
            modelBuilder.Entity<Proveedor>().HasKey(x => x.IdProveedor);
            modelBuilder.Entity<Cliente>().HasKey(x => x.IdCliente);
            modelBuilder.Entity<Producto>().HasKey(x => x.IdProducto);
            modelBuilder.Entity<Inventario>().HasKey(x => x.IdInventario);
            modelBuilder.Entity<Compra>().HasKey(x => x.IdCompra);
            modelBuilder.Entity<CompraDetalle>().HasKey(x => x.IdCompraDetalle);
            modelBuilder.Entity<Factura>().HasKey(x => x.IdFactura);
            modelBuilder.Entity<FacturaDetalle>().HasKey(x => x.IdFacturaDetalle);

            modelBuilder.Entity<Inventario>()
                .HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Proveedor)
                .WithMany()
                .HasForeignKey(c => c.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CompraDetalle>()
                .HasOne(cd => cd.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(cd => cd.IdCompra)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompraDetalle>()
                .HasOne(cd => cd.Producto)
                .WithMany()
                .HasForeignKey(cd => cd.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente)
                .WithMany()
                .HasForeignKey(f => f.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacturaDetalle>()
                .HasOne(fd => fd.Factura)
                .WithMany(f => f.Detalles)
                .HasForeignKey(fd => fd.IdFactura)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FacturaDetalle>()
                .HasOne(fd => fd.Producto)
                .WithMany()
                .HasForeignKey(fd => fd.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}