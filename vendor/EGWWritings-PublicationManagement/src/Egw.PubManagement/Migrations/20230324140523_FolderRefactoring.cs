using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class FolderRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "global_order",
                table: "folders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "materialized_path",
                table: "folders",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "global_order",
                table: "folders");

            migrationBuilder.DropColumn(
                name: "materialized_path",
                table: "folders");
        }
    }
}
