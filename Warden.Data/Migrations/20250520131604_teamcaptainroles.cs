using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warden.Data.Migrations
{
    /// <inheritdoc />
    public partial class teamcaptainroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamCaptainRoleId",
                table: "GuildConfigs");

            migrationBuilder.AddColumn<decimal[]>(
                name: "TeamCaptainRoleIds",
                table: "GuildConfigs",
                type: "numeric(20,0)[]",
                nullable: false,
                defaultValue: new decimal[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamCaptainRoleIds",
                table: "GuildConfigs");

            migrationBuilder.AddColumn<decimal>(
                name: "TeamCaptainRoleId",
                table: "GuildConfigs",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
