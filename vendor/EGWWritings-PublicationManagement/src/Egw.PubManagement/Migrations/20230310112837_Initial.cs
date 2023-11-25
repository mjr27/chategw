using System;
using Egw.PubManagement.Persistence.Entities.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Egw.PubManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    last_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    biography = table.Column<string>(type: "text", nullable: true),
                    birth_year = table.Column<int>(type: "integer", nullable: true),
                    death_year = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_authors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "folder_types",
                columns: table => new
                {
                    folder_type_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    allowed_types = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_folder_types", x => x.folder_type_id);
                });

            migrationBuilder.CreateTable(
                name: "publication_links",
                columns: table => new
                {
                    para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    original_para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    element_id = table.Column<int>(type: "integer", nullable: false),
                    original_publication_id = table.Column<int>(type: "integer", nullable: false),
                    original_element_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publication_links", x => new { x.para_id, x.original_para_id });
                });

            migrationBuilder.CreateTable(
                name: "publication_mp3files",
                columns: table => new
                {
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    filename = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    voice_type = table.Column<int>(type: "integer", nullable: false),
                    is_generated = table.Column<bool>(type: "boolean", nullable: false),
                    element_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publication_mp3files", x => new { x.publication_id, x.filename });
                });

            migrationBuilder.CreateTable(
                name: "publication_series",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    publications = table.Column<int[]>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publication_series", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "folders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    order = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_folders", x => x.id);
                    table.ForeignKey(
                        name: "fk_folders_folder_types_type_id",
                        column: x => x.type_id,
                        principalTable: "folder_types",
                        principalColumn: "folder_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_folders_folders_parent_id",
                        column: x => x.parent_id,
                        principalTable: "folders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    egw_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    bcp47code = table.Column<string>(type: "text", nullable: false),
                    is_right_to_left = table.Column<bool>(type: "boolean", nullable: false),
                    root_folder_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.code);
                    table.ForeignKey(
                        name: "fk_languages_folders_root_folder_id",
                        column: x => x.root_folder_id,
                        principalTable: "folders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "publications",
                columns: table => new
                {
                    publication_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    language_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<int>(type: "integer", nullable: true),
                    original_publication_id = table.Column<int>(type: "integer", nullable: true),
                    publication_year = table.Column<int>(type: "integer", nullable: true),
                    publisher = table.Column<string>(type: "text", nullable: false),
                    isbn = table.Column<string>(type: "text", nullable: true),
                    purchase_link = table.Column<string>(type: "text", nullable: true),
                    page_count = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publications", x => x.publication_id);
                    table.ForeignKey(
                        name: "fk_publications_authors_author_id",
                        column: x => x.author_id,
                        principalTable: "authors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_publications_languages_language_temp_id",
                        column: x => x.language_code,
                        principalTable: "languages",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_publications_publications_original_publication_id",
                        column: x => x.original_publication_id,
                        principalTable: "publications",
                        principalColumn: "publication_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "paragraphs",
                columns: table => new
                {
                    para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    paragraph_id = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    is_referenced = table.Column<bool>(type: "boolean", nullable: false),
                    heading_level = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_paragraphs", x => x.para_id);
                    table.ForeignKey(
                        name: "fk_paragraphs_publications_publication_id",
                        column: x => x.publication_id,
                        principalTable: "publications",
                        principalColumn: "publication_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publication_exports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    filename = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publication_exports", x => x.id);
                    table.ForeignKey(
                        name: "fk_publication_exports_publications_publication_id",
                        column: x => x.publication_id,
                        principalTable: "publications",
                        principalColumn: "publication_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publication_placement",
                columns: table => new
                {
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    folder_id = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    permission = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publication_placement", x => x.publication_id);
                    table.ForeignKey(
                        name: "fk_publication_placement_folders_folder_id",
                        column: x => x.folder_id,
                        principalTable: "folders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_publication_placement_publications_publication_id",
                        column: x => x.publication_id,
                        principalTable: "publications",
                        principalColumn: "publication_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "paragraph_metadata",
                columns: table => new
                {
                    para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    pagination = table.Column<PaginationMetaData>(type: "jsonb", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    bible_metadata = table.Column<BibleMetadata>(type: "jsonb", nullable: true),
                    lt_ms_metadata = table.Column<LtMsMetadata>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_paragraph_metadata", x => x.para_id);
                    table.ForeignKey(
                        name: "fk_paragraph_metadata_paragraphs_paragraph_id",
                        column: x => x.para_id,
                        principalTable: "paragraphs",
                        principalColumn: "para_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publication_chapters",
                columns: table => new
                {
                    para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    publication_id = table.Column<int>(type: "integer", nullable: false),
                    end_para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    content_end_para_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    chapter_id = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    end_order = table.Column<int>(type: "integer", nullable: false),
                    content_end_order = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publication_chapters", x => x.para_id);
                    table.ForeignKey(
                        name: "fk_publication_chapters_paragraphs_content_end_paragraph_paragrap",
                        column: x => x.content_end_para_id,
                        principalTable: "paragraphs",
                        principalColumn: "para_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_publication_chapters_paragraphs_end_paragraph_paragraph_id",
                        column: x => x.end_para_id,
                        principalTable: "paragraphs",
                        principalColumn: "para_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_publication_chapters_paragraphs_paragraph_id",
                        column: x => x.para_id,
                        principalTable: "paragraphs",
                        principalColumn: "para_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "folder_types",
                columns: new[] { "folder_type_id", "allowed_types", "created_at", "title", "updated_at" },
                values: new object[,]
                {
                    { "adventist-beliefs", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adventist beliefs", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "apl", "book periodical/page-break periodical/no-page-break", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adventist Pioneer Library", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "bible", "bible", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bible", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "bible-commentaries", "bible-commentary", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bible Commentaries", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "bible-concordances", "dictionary topical-index", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bible Concordances", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "bible-dictionaries", "dictionary", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bible Dictionaries", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "bible-sdasi", "scripture-index", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "EGW Scripture Indices", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "bible-versions", "bible", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bible Versions", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "biography", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Biography", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "books", "book bible-commentary", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Books", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "childrens-stories", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Children stories", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "devotionals", "devotional", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Devotionals", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "dictionary", "dictionary", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dictionaries", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "egwwritings", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "EGW Writings", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "letters-manuscripts", "manuscript-volume", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Letters & manuscripts", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "manuscript-releases", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Manuscript releases", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "misc-collections", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Misc Collections", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "misc-titles", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Misc Titles", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "modern-english", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Modern English", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "pamphlets", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pamphlets", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "periodicals", "periodical/page-break periodical/no-page-break", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Periodicals", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "recent-authors", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Recent Authors", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "reference", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Reference", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "reference-works", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Reference Works", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "research-documents", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "EGW Research Documents", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "root", "", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Root", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "study-guides", "book", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Study guides", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "topical-index", "topical-index", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Topical indexes", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "publication_series",
                columns: new[] { "code", "created_at", "publications", "title", "type", "updated_at" },
                values: new object[,]
                {
                    { "bc", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 90, 91, 92, 93, 94, 95, 96, 97 }, "EGW SDA Bible Commentary", "series", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "christ_s_life_and_ministry", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 15, 108, 130, 144, 150 }, "Christ's Life and Ministry", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "christian_lifestyle", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 13, 14, 25, 27, 31, 77, 78, 87, 108, 137, 138, 146, 148, 386, 11974 }, "Christian Lifestyle", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "church_history", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 11, 41, 104, 127, 132, 140, 141, 145, 665 }, "Church History", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "church_life_and_ministry", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 19, 21, 22, 83, 98, 99, 100, 121, 123, 1445 }, "Church Life and Ministry", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "conflict_of_the_ages_series", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 84, 88, 127, 130, 132 }, "Conflict of the Ages Series", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "devotional_readings", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 9, 17, 33, 38, 44, 74, 79, 80, 81, 89, 102, 126, 131, 147, 149, 151, 153, 12210, 12862 }, "Devotional Readings", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "education", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 10, 23, 29, 32, 103, 1976 }, "Education", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "egw_biography", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 11, 41, 105, 665, 667, 668, 669, 670, 671, 672 }, "EGW Biography", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "evangelism_and_witnessing", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 1, 11, 13, 16, 30, 34, 35, 45, 152, 389, 489, 492, 496, 806, 12399 }, "Evangelism and Witnessing", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "health_and_wellness", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 20, 75, 77, 78, 110, 125, 135, 384, 387, 388 }, "Health and Wellness", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "history_of_redemption", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 84, 88, 104, 105, 106, 127, 130, 132, 140, 141, 142, 143, 145 }, "History of Redemption", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "last_day_events", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 3, 26, 28, 36, 39, 100, 132, 1445 }, "Last Day Events", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "leadership", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 12, 35, 84, 88, 127, 14222 }, "Leadership", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "lessons_from_the_bible", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 15, 18, 84, 88, 90, 91, 92, 93, 94, 95, 96, 101, 127, 130, 12119 }, "Lessons from the Bible", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "parenting", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 2, 6, 7, 8, 86, 128 }, "Parenting", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "publishing", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 16, 24, 390 }, "Publishing", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "relationships_and_marriage", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 2, 4, 19, 40, 76, 77, 78, 122, 128 }, "Relationships and Marriage", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "sdabc", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 12511, 12513, 12514, 12515, 12516, 12517, 12518 }, "SDA Bible Commentary", "series", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "testimonies_for_the_church", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 112, 113, 114, 115, 116, 117, 118, 119, 120, 2003, 2004, 2005 }, "Testimonies for the Church", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "the_life_of_faith_collection", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 15, 29, 108, 135, 150 }, "The Life of Faith Collection", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "youth_and_modern_english", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new[] { 7, 76, 144, 1974, 1976, 1977, 1978, 1980, 2017, 2715, 2723, 12269 }, "Youth and Modern English", "topic", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_folders_parent_id_order",
                table: "folders",
                columns: new[] { "parent_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_folders_type_id",
                table: "folders",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_languages_bcp47code",
                table: "languages",
                column: "bcp47code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_languages_root_folder_id",
                table: "languages",
                column: "root_folder_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_paragraph_metadata_publication_id",
                table: "paragraph_metadata",
                column: "publication_id");

            migrationBuilder.CreateIndex(
                name: "ix_paragraph_metadata_publication_id_date",
                table: "paragraph_metadata",
                columns: new[] { "publication_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_paragraphs_publication_id_heading_level",
                table: "paragraphs",
                columns: new[] { "publication_id", "heading_level" });

            migrationBuilder.CreateIndex(
                name: "ix_paragraphs_publication_id_order",
                table: "paragraphs",
                columns: new[] { "publication_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_paragraphs_publication_id_para_id",
                table: "paragraphs",
                columns: new[] { "publication_id", "para_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_publication_chapters_content_end_para_id",
                table: "publication_chapters",
                column: "content_end_para_id");

            migrationBuilder.CreateIndex(
                name: "ix_publication_chapters_end_para_id",
                table: "publication_chapters",
                column: "end_para_id");

            migrationBuilder.CreateIndex(
                name: "ix_publication_chapters_publication_id",
                table: "publication_chapters",
                column: "publication_id");

            migrationBuilder.CreateIndex(
                name: "ix_publication_exports_publication_id",
                table: "publication_exports",
                column: "publication_id");

            migrationBuilder.CreateIndex(
                name: "ix_publication_links_original_para_id",
                table: "publication_links",
                column: "original_para_id");

            migrationBuilder.CreateIndex(
                name: "ix_publication_links_original_publication_id_original_element_",
                table: "publication_links",
                columns: new[] { "original_publication_id", "original_element_id" });

            migrationBuilder.CreateIndex(
                name: "ix_publication_links_publication_id_element_id",
                table: "publication_links",
                columns: new[] { "publication_id", "element_id" });

            migrationBuilder.CreateIndex(
                name: "ix_publication_links_publication_id_original_publication_id",
                table: "publication_links",
                columns: new[] { "publication_id", "original_publication_id" });

            migrationBuilder.CreateIndex(
                name: "ix_publication_mp3files_para_id_voice_type",
                table: "publication_mp3files",
                columns: new[] { "para_id", "voice_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_publication_placement_folder_id_order",
                table: "publication_placement",
                columns: new[] { "folder_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_publications_author_id",
                table: "publications",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_publications_language_code",
                table: "publications",
                column: "language_code");

            migrationBuilder.CreateIndex(
                name: "ix_publications_original_publication_id",
                table: "publications",
                column: "original_publication_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paragraph_metadata");

            migrationBuilder.DropTable(
                name: "publication_chapters");

            migrationBuilder.DropTable(
                name: "publication_exports");

            migrationBuilder.DropTable(
                name: "publication_links");

            migrationBuilder.DropTable(
                name: "publication_mp3files");

            migrationBuilder.DropTable(
                name: "publication_placement");

            migrationBuilder.DropTable(
                name: "publication_series");

            migrationBuilder.DropTable(
                name: "paragraphs");

            migrationBuilder.DropTable(
                name: "publications");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "languages");

            migrationBuilder.DropTable(
                name: "folders");

            migrationBuilder.DropTable(
                name: "folder_types");
        }
    }
}
