using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Films.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGenreToArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "movie_genre",
                schema: "movie",
                table: "movies",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<bool>(
                name: "movie_viewed",
                schema: "movie",
                table: "movies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "blob_created_at",
                schema: "movie",
                table: "blobs",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users",
                schema: "movie");

            migrationBuilder.DropColumn(
                name: "movie_genre",
                schema: "movie",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "movie_viewed",
                schema: "movie",
                table: "movies");

            migrationBuilder.AlterColumn<DateTime>(
                name: "blob_created_at",
                schema: "movie",
                table: "blobs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
