using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class PublicationArchiveIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_publication_archive_deleted_at_publication_id_archived_at",
                table: "publication_archive",
                columns: new[] { "deleted_at", "publication_id", "archived_at" });

            migrationBuilder.CreateIndex(
                name: "ix_publication_archive_hash",
                table: "publication_archive",
                column: "hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_publication_archive_deleted_at_publication_id_archived_at",
                table: "publication_archive");

            migrationBuilder.DropIndex(
                name: "ix_publication_archive_hash",
                table: "publication_archive");
        }
    }
}
