using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCrm.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteBajaLogica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Eliminado",
                table: "Clientes",
                column: "Eliminado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_Eliminado",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Clientes");
        }
    }
}
