using Microsoft.EntityFrameworkCore;
using ParkG.Domain.Entities;
using ParkG.Infrastructure.Security;

namespace ParkG.Infrastructure.Data;

public class ParkGDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ParkGDbContext(DbContextOptions<ParkGDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Operador> Operadores { get; set; }
    public DbSet<EspacioParking> EspaciosParking { get; set; }
    public DbSet<TarifaTenant> TarifasTenant { get; set; }
    public DbSet<Estadia> Estadias { get; set; }
    public DbSet<Comprobante> Comprobantes { get; set; }
    public DbSet<RolCatalogo> Roles { get; set; }
    public DbSet<MetodoPago> MetodosPago { get; set; }
    public DbSet<TipoVehiculo> TiposVehiculo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>().HasKey(t => t.Id);
        modelBuilder.Entity<Tenant>().HasIndex(t => t.Ruc).IsUnique();

        modelBuilder.Entity<Operador>().HasKey(o => o.Id);
        modelBuilder.Entity<Operador>().HasIndex(o => new { o.TenantId, o.Username }).IsUnique();
        modelBuilder.Entity<Operador>().HasQueryFilter(o => _tenantContext.TenantId == null || o.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<EspacioParking>().HasKey(e => e.Id);
        modelBuilder.Entity<EspacioParking>().HasIndex(e => new { e.TenantId, e.Codigo }).IsUnique();
        modelBuilder.Entity<EspacioParking>().HasQueryFilter(e => _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<TarifaTenant>().HasKey(t => t.Id);
        modelBuilder.Entity<TarifaTenant>().HasIndex(t => new { t.TenantId, t.TipoVehiculo, t.VigenteDesde }).IsUnique();
        modelBuilder.Entity<TarifaTenant>().HasQueryFilter(t => _tenantContext.TenantId == null || t.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<Estadia>().HasKey(e => e.Id);
        modelBuilder.Entity<Estadia>().HasIndex(e => new { e.TenantId, e.Placa, e.Estado });
        modelBuilder.Entity<Estadia>().HasQueryFilter(e => _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<Comprobante>().HasKey(c => c.Id);
        modelBuilder.Entity<Comprobante>().HasIndex(c => c.EstadiaId).IsUnique();
        modelBuilder.Entity<Comprobante>().HasQueryFilter(c => _tenantContext.TenantId == null || c.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<MetodoPago>().HasKey(m => m.Id);
        modelBuilder.Entity<TipoVehiculo>().HasKey(t => t.Id);
        modelBuilder.Entity<RolCatalogo>().HasKey(r => r.Id);

        modelBuilder.Entity<Operador>()
            .HasOne(o => o.Tenant)
            .WithMany()
            .HasForeignKey(o => o.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EspacioParking>()
            .HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TarifaTenant>()
            .HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Estadia>()
            .HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Estadia>()
            .HasOne<EspacioParking>()
            .WithMany()
            .HasForeignKey(e => e.EspacioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Estadia>()
            .HasOne<Operador>()
            .WithMany()
            .HasForeignKey(e => e.OperadorIngresoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Estadia>()
            .HasOne<Operador>()
            .WithMany()
            .HasForeignKey(e => e.OperadorSalidaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comprobante>()
            .HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comprobante>()
            .HasOne(c => c.Estadia)
            .WithMany()
            .HasForeignKey(c => c.EstadiaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TarifaTenant>().Property(t => t.VigenteDesde).HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Estadia>().Property(e => e.FechaIngreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Comprobante>().Property(c => c.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<TarifaTenant>().Property(t => t.TarifaHora).HasPrecision(12, 2);
        modelBuilder.Entity<TarifaTenant>().Property(t => t.TarifaDia).HasPrecision(12, 2);
        modelBuilder.Entity<Comprobante>().Property(c => c.SubtotalNeto).HasPrecision(12, 2);
        modelBuilder.Entity<Comprobante>().Property(c => c.Igv).HasPrecision(12, 2);
        modelBuilder.Entity<Comprobante>().Property(c => c.Total).HasPrecision(12, 2);

        modelBuilder.Entity<Operador>().Property(o => o.Rol).HasMaxLength(30);
        modelBuilder.Entity<EspacioParking>().Property(e => e.TipoVehiculoPermitido).HasMaxLength(20);
        modelBuilder.Entity<EspacioParking>().Property(e => e.Estado).HasMaxLength(20);
        modelBuilder.Entity<TarifaTenant>().Property(t => t.TipoVehiculo).HasMaxLength(20);
        modelBuilder.Entity<Estadia>().Property(e => e.TipoVehiculo).HasMaxLength(20);
        modelBuilder.Entity<Estadia>().Property(e => e.Estado).HasMaxLength(20);
        modelBuilder.Entity<Tenant>().Property(t => t.Ruc).HasMaxLength(11);
        modelBuilder.Entity<Tenant>().Property(t => t.NombreComercial).HasMaxLength(200);
        modelBuilder.Entity<Comprobante>().Property(c => c.Total).HasPrecision(12, 2);

        modelBuilder.Entity<RolCatalogo>().HasData(
            new RolCatalogo { Id = 1, Codigo = "owner", Descripcion = "Propietario del tenant" },
            new RolCatalogo { Id = 2, Codigo = "admin", Descripcion = "Administrador del tenant" },
            new RolCatalogo { Id = 3, Codigo = "operador", Descripcion = "Operador de parking" });

        modelBuilder.Entity<TipoVehiculo>().HasData(
            new TipoVehiculo { Id = 1, Codigo = "carro" },
            new TipoVehiculo { Id = 2, Codigo = "camion" },
            new TipoVehiculo { Id = 3, Codigo = "trailer" });

        modelBuilder.Entity<MetodoPago>().HasData(
            new MetodoPago { Id = 1, Codigo = "efectivo" },
            new MetodoPago { Id = 2, Codigo = "tarjeta" },
            new MetodoPago { Id = 3, Codigo = "yape" });

        var tenantCentroId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var tenantNorteId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var operadorCentroId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var operadorNorteId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var espacioCentro1Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var espacioCentro2Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var espacioNorte1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var espacioNorte2Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var tarifaCentroId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var tarifaNorteId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var adminPasswordHash = "AQIAAACghgEAEAAAAAABAgMEBQYHCAkKCwwNDg97z2IZyhqC3YaJEn8frGtw7fMb52Ky5VJvyBDCaUXU9w==";
        var baseDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Tenant>().HasData(
            new Tenant
            {
                Id = tenantCentroId,
                Ruc = "20111111111",
                NombreComercial = "Garaje Centro",
                Estado = "activo",
                CreadoEn = baseDate
            },
            new Tenant
            {
                Id = tenantNorteId,
                Ruc = "20222222222",
                NombreComercial = "Garaje Norte",
                Estado = "activo",
                CreadoEn = baseDate.AddMinutes(1)
            });

        modelBuilder.Entity<Operador>().HasData(
            new Operador
            {
                Id = operadorCentroId,
                TenantId = tenantCentroId,
                Username = "admin",
                PasswordHash = adminPasswordHash,
                Rol = "admin",
                Activo = true
            },
            new Operador
            {
                Id = operadorNorteId,
                TenantId = tenantNorteId,
                Username = "admin",
                PasswordHash = adminPasswordHash,
                Rol = "admin",
                Activo = true
            });

        modelBuilder.Entity<EspacioParking>().HasData(
            new EspacioParking
            {
                Id = espacioCentro1Id,
                TenantId = tenantCentroId,
                Codigo = "CC-001",
                TipoVehiculoPermitido = "carro",
                Estado = "libre"
            },
            new EspacioParking
            {
                Id = espacioCentro2Id,
                TenantId = tenantCentroId,
                Codigo = "CC-002",
                TipoVehiculoPermitido = "carro",
                Estado = "libre"
            },
            new EspacioParking
            {
                Id = espacioNorte1Id,
                TenantId = tenantNorteId,
                Codigo = "CN-001",
                TipoVehiculoPermitido = "carro",
                Estado = "libre"
            },
            new EspacioParking
            {
                Id = espacioNorte2Id,
                TenantId = tenantNorteId,
                Codigo = "CN-002",
                TipoVehiculoPermitido = "carro",
                Estado = "libre"
            });

        modelBuilder.Entity<TarifaTenant>().HasData(
            new TarifaTenant
            {
                Id = tarifaCentroId,
                TenantId = tenantCentroId,
                TipoVehiculo = "carro",
                TarifaHora = 5.00m,
                TarifaDia = 30.00m,
                VigenteDesde = baseDate
            },
            new TarifaTenant
            {
                Id = tarifaNorteId,
                TenantId = tenantNorteId,
                TipoVehiculo = "carro",
                TarifaHora = 5.00m,
                TarifaDia = 30.00m,
                VigenteDesde = baseDate
            });
    }
}