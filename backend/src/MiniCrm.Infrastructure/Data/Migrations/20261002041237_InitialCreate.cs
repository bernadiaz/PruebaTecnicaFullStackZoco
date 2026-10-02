using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCrm.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asesores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asesores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Cuit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CuitNormalizado = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    AsesorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProximoContacto = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Asesores_AsesorId",
                        column: x => x.AsesorId,
                        principalTable: "Asesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gestiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoContacto = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Comentario = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    EstadoResultante = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    FechaGestion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProximoContacto = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gestiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gestiones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_AsesorId",
                table: "Clientes",
                column: "AsesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CuitNormalizado",
                table: "Clientes",
                column: "CuitNormalizado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_ClienteId_FechaGestion",
                table: "Gestiones",
                columns: new[] { "ClienteId", "FechaGestion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gestiones");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Asesores");
        }
    }
}
