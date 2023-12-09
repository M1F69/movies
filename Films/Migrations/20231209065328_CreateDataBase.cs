using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Films.Migrations
{
    /// <inheritdoc />
    public partial class CreateDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "movie");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:lo", ",,");

            migrationBuilder.CreateTable(
                name: "authors",
                schema: "movie",
                columns: table => new
                {
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_name = table.Column<string>(type: "text", nullable: false),
                    author_year = table.Column<string>(type: "text", nullable: false),
                    author_description = table.Column<string>(type: "text", nullable: false),
                    author_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.author_id);
                });

            migrationBuilder.CreateTable(
                name: "blobs",
                schema: "movie",
                columns: table => new
                {
                    blob_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lo_id = table.Column<uint>(type: "lo", nullable: false),
                    blob_name = table.Column<string>(type: "text", nullable: false),
                    blob_mime_type = table.Column<string>(type: "text", nullable: false),
                    blob_size = table.Column<long>(type: "bigint", nullable: false),
                    blob_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blobs", x => x.blob_id);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                schema: "movie",
                columns: table => new
                {
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    movie_author_id = table.Column<Guid>(type: "uuid", nullable: true),
                    movie_image_id = table.Column<Guid>(type: "uuid", nullable: true),
                    movie_name = table.Column<string>(type: "text", nullable: false),
                    movie_description = table.Column<string>(type: "text", nullable: false),
                    movie_year = table.Column<int>(type: "integer", nullable: false),
                    movie_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.movie_id);
                    table.ForeignKey(
                        name: "FK_movies_authors_movie_author_id",
                        column: x => x.movie_author_id,
                        principalSchema: "movie",
                        principalTable: "authors",
                        principalColumn: "author_id");
                    table.ForeignKey(
                        name: "FK_movies_blobs_movie_image_id",
                        column: x => x.movie_image_id,
                        principalSchema: "movie",
                        principalTable: "blobs",
                        principalColumn: "blob_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_movies_movie_author_id",
                schema: "movie",
                table: "movies",
                column: "movie_author_id");

            migrationBuilder.CreateIndex(
                name: "IX_movies_movie_image_id",
                schema: "movie",
                table: "movies",
                column: "movie_image_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movies",
                schema: "movie");

            migrationBuilder.DropTable(
                name: "authors",
                schema: "movie");

            migrationBuilder.DropTable(
                name: "blobs",
                schema: "movie");
        }
    }
}
