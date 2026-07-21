using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ParkG.Migrations
{
    /// <inheritdoc />
    public partial class InitialParkGSaaS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetodosPago",
                columns: table => new
                {
                    Id = table.Column<short>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<short>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Ruc = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    NombreComercial = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposVehiculo",
                columns: table => new
                {
                    Id = table.Column<short>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposVehiculo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EspaciosParking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    TipoVehiculoPermitido = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspaciosParking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EspaciosParking_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Operadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operadores_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TarifasTenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TipoVehiculo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TarifaHora = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    TarifaDia = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    VigenteDesde = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    VigenteHasta = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarifasTenant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TarifasTenant_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estadias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Placa = table.Column<string>(type: "TEXT", nullable: false),
                    TipoVehiculo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DniCliente = table.Column<string>(type: "TEXT", nullable: false),
                    EspacioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperadorIngresoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OperadorSalidaId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaSalida = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estadias_EspaciosParking_EspacioId",
                        column: x => x.EspacioId,
                        principalTable: "EspaciosParking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estadias_Operadores_OperadorIngresoId",
                        column: x => x.OperadorIngresoId,
                        principalTable: "Operadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estadias_Operadores_OperadorSalidaId",
                        column: x => x.OperadorSalidaId,
                        principalTable: "Operadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estadias_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comprobantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EstadiaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MinutosTotales = table.Column<int>(type: "INTEGER", nullable: false),
                    SubtotalNeto = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    Igv = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprobantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comprobantes_Estadias_EstadiaId",
                        column: x => x.EstadiaId,
                        principalTable: "Estadias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comprobantes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MetodosPago",
                columns: new[] { "Id", "Codigo" },
                values: new object[,]
                {
                    { (short)1, "efectivo" },
                    { (short)2, "tarjeta" },
                    { (short)3, "yape" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Codigo", "Descripcion" },
                values: new object[,]
                {
                    { (short)1, "owner", "Propietario del tenant" },
                    { (short)2, "admin", "Administrador del tenant" },
                    { (short)3, "operador", "Operador de parking" }
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "CreadoEn", "Estado", "NombreComercial", "Ruc" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "activo", "Garaje Centro", "20111111111" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 7, 20, 0, 1, 0, 0, DateTimeKind.Utc), "activo", "Garaje Norte", "20222222222" }
                });

            migrationBuilder.InsertData(
                table: "TiposVehiculo",
                columns: new[] { "Id", "Codigo" },
                values: new object[,]
                {
                    { (short)1, "carro" },
                    { (short)2, "camion" },
                    { (short)3, "trailer" }
                });

            migrationBuilder.InsertData(
                table: "EspaciosParking",
                columns: new[] { "Id", "Codigo", "Estado", "TenantId", "TipoVehiculoPermitido" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), "CC-001", "libre", new Guid("11111111-1111-1111-1111-111111111111"), "carro" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "CC-002", "libre", new Guid("11111111-1111-1111-1111-111111111111"), "carro" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "CN-001", "libre", new Guid("22222222-2222-2222-2222-222222222222"), "carro" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "CN-002", "libre", new Guid("22222222-2222-2222-2222-222222222222"), "carro" }
                });

            migrationBuilder.InsertData(
                table: "Operadores",
                columns: new[] { "Id", "Activo", "PasswordHash", "Rol", "TenantId", "Username" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), true, "AQIAAACghgEAEAAAAAABAgMEBQYHCAkKCwwNDg97z2IZyhqC3YaJEn8frGtw7fMb52Ky5VJvyBDCaUXU9w==", "admin", new Guid("11111111-1111-1111-1111-111111111111"), "admin" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), true, "AQIAAACghgEAEAAAAAABAgMEBQYHCAkKCwwNDg97z2IZyhqC3YaJEn8frGtw7fMb52Ky5VJvyBDCaUXU9w==", "admin", new Guid("22222222-2222-2222-2222-222222222222"), "admin" }
                });

            migrationBuilder.InsertData(
                table: "TarifasTenant",
                columns: new[] { "Id", "TarifaDia", "TarifaHora", "TenantId", "TipoVehiculo", "VigenteDesde", "VigenteHasta" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), 30.00m, 5.00m, new Guid("11111111-1111-1111-1111-111111111111"), "carro", new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 30.00m, 5.00m, new Guid("22222222-2222-2222-2222-222222222222"), "carro", new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_EstadiaId",
                table: "Comprobantes",
                column: "EstadiaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_TenantId",
                table: "Comprobantes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EspaciosParking_TenantId_Codigo",
                table: "EspaciosParking",
                columns: new[] { "TenantId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estadias_EspacioId",
                table: "Estadias",
                column: "EspacioId");

            migrationBuilder.CreateIndex(
                name: "IX_Estadias_OperadorIngresoId",
                table: "Estadias",
                column: "OperadorIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estadias_OperadorSalidaId",
                table: "Estadias",
                column: "OperadorSalidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Estadias_TenantId_Placa_Estado",
                table: "Estadias",
                columns: new[] { "TenantId", "Placa", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Operadores_TenantId_Username",
                table: "Operadores",
                columns: new[] { "TenantId", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarifasTenant_TenantId_TipoVehiculo_VigenteDesde",
                table: "TarifasTenant",
                columns: new[] { "TenantId", "TipoVehiculo", "VigenteDesde" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Ruc",
                table: "Tenants",
                column: "Ruc",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comprobantes");

            migrationBuilder.DropTable(
                name: "MetodosPago");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "TarifasTenant");

            migrationBuilder.DropTable(
                name: "TiposVehiculo");

            migrationBuilder.DropTable(
                name: "Estadias");

            migrationBuilder.DropTable(
                name: "EspaciosParking");

            migrationBuilder.DropTable(
                name: "Operadores");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
