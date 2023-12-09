using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Films.Migrations
{
    /// <inheritdoc />
    public partial class Blob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS tr_blobs_lo_manage ON movie.blobs;
                CREATE TRIGGER tr_blobs_lo_manage BEFORE UPDATE OR DELETE ON movie.blobs
                    FOR EACH ROW EXECUTE PROCEDURE lo_manage(lo_id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS tr_blobs_lo_manage ON movie.blobs;");
        }
    }
}
