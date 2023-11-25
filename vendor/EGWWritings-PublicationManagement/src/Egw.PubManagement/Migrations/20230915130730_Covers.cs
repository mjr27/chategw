using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class Covers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cover_types",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    proportions = table.Column<decimal>(type: "numeric(8,5)", precision: 8, scale: 5, nullable: false),
                    min_width = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cover_types", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "covers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_main = table.Column<bool>(type: "boolean", nullable: false),
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    format = table.Column<string>(type: "text", nullable: false),
                    uri = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_covers", x => x.id);
                    table.ForeignKey(
                        name: "fk_covers_cover_types_type_temp_id",
                        column: x => x.type_id,
                        principalTable: "cover_types",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_covers_publications_publication_id",
                        column: x => x.publication_id,
                        principalTable: "publications",
                        principalColumn: "publication_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_covers_publication_id",
                table: "covers",
                column: "publication_id");

            migrationBuilder.CreateIndex(
                name: "ix_covers_type_id",
                table: "covers",
                column: "type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "covers");

            migrationBuilder.DropTable(
                name: "cover_types");
        }
    }
}
