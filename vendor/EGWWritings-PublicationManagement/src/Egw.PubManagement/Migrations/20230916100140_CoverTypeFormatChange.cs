using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class CoverTypeFormatChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "proportions",
                table: "cover_types");

            migrationBuilder.AlterColumn<int>(
                name: "min_width",
                table: "cover_types",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "min_height",
                table: "cover_types",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "min_height",
                table: "cover_types");

            migrationBuilder.AlterColumn<int>(
                name: "min_width",
                table: "cover_types",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "proportions",
                table: "cover_types",
                type: "numeric(8,5)",
                precision: 8,
                scale: 5,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
