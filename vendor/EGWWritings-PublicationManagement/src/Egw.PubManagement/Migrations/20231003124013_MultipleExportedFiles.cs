using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class MultipleExportedFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "filename",
                table: "publication_exports");

            migrationBuilder.AddColumn<bool>(
                name: "is_main",
                table: "publication_exports",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "size",
                table: "publication_exports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "uri",
                table: "publication_exports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_publication_exports_is_main_publication_id_type",
                table: "publication_exports",
                columns: new[] { "is_main", "publication_id", "type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_publication_exports_is_main_publication_id_type",
                table: "publication_exports");

            migrationBuilder.DropColumn(
                name: "is_main",
                table: "publication_exports");

            migrationBuilder.DropColumn(
                name: "size",
                table: "publication_exports");

            migrationBuilder.DropColumn(
                name: "uri",
                table: "publication_exports");

            migrationBuilder.AddColumn<string>(
                name: "filename",
                table: "publication_exports",
                type: "character varying(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");
        }
    }
}
