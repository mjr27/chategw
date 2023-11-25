using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class MaxTocLeveladded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "toc_depth",
                table: "publication_placement",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "toc_depth",
                table: "publication_placement");
        }
    }
}
