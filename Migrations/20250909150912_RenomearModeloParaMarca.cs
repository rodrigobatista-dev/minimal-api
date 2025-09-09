using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class RenomearModeloParaMarca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "Modelo",
        table: "Veiculos",
        newName: "Marca");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "Marca",
        table: "Veiculos",
        newName: "Modelo");
}

    }
}
