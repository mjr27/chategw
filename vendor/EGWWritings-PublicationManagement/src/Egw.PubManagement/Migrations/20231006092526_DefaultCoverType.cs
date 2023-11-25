using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class DefaultCoverType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "cover_types",
                columns: new[] { "code", "description", "min_height", "min_width" },
                values: new object[] { "web", "Web export", 801, 546 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "cover_types",
                keyColumn: "code",
                keyValue: "web");
        }
    }
}
