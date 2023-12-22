using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Films.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
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
                    blob_created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blobs", x => x.blob_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "movie",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_nickname = table.Column<string>(type: "text", nullable: false),
                    user_fullname = table.Column<string>(type: "text", nullable: false),
                    user_password = table.Column<string>(type: "text", nullable: false),
                    user_mail = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
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
                    movie_viewed = table.Column<bool>(type: "boolean", nullable: false),
                    movie_year = table.Column<int>(type: "integer", nullable: false),
                    movie_type = table.Column<int>(type: "integer", nullable: false),
                    movie_genre = table.Column<int[]>(type: "integer[]", nullable: false),
                    trailer_href = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "viewed",
                schema: "movie",
                columns: table => new
                {
                    viewed_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    viewed_movie_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_viewed", x => new { x.viewed_user_id, x.viewed_movie_id });
                    table.ForeignKey(
                        name: "FK_viewed_movies_viewed_movie_id",
                        column: x => x.viewed_movie_id,
                        principalSchema: "movie",
                        principalTable: "movies",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_viewed_users_viewed_user_id",
                        column: x => x.viewed_user_id,
                        principalSchema: "movie",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_users_user_mail",
                schema: "movie",
                table: "users",
                column: "user_mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_user_nickname",
                schema: "movie",
                table: "users",
                column: "user_nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_user_nickname_user_mail",
                schema: "movie",
                table: "users",
                columns: new[] { "user_nickname", "user_mail" });

            migrationBuilder.CreateIndex(
                name: "IX_viewed_viewed_movie_id",
                schema: "movie",
                table: "viewed",
                column: "viewed_movie_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "viewed",
                schema: "movie");

            migrationBuilder.DropTable(
                name: "movies",
                schema: "movie");

            migrationBuilder.DropTable(
                name: "users",
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
