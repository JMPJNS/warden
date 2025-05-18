using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warden.Data.Migrations
{
    /// <inheritdoc />
    public partial class timezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeZone",
                table: "Players",
                newName: "Timezone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timezone",
                table: "Players",
                newName: "TimeZone");
        }
    }
}
