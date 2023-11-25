using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class UnitTestingRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_folders_parent_id_order",
                table: "folders");

            migrationBuilder.AlterColumn<string>(
                name: "bcp47code",
                table: "languages",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_languages_egw_code",
                table: "languages",
                column: "egw_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_folders_parent_id_order",
                table: "folders",
                columns: new[] { "parent_id", "order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_languages_egw_code",
                table: "languages");

            migrationBuilder.DropIndex(
                name: "ix_folders_parent_id_order",
                table: "folders");

            migrationBuilder.AlterColumn<string>(
                name: "bcp47code",
                table: "languages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.CreateIndex(
                name: "ix_folders_parent_id_order",
                table: "folders",
                columns: new[] { "parent_id", "order" },
                unique: true);
        }
    }
}
