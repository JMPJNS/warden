using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warden.Data.Migrations
{
    /// <inheritdoc />
    public partial class RingerMsg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RingerMsgId",
                table: "Scrims",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ScrimMsgId",
                table: "Scrims",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RingerChannelId",
                table: "GuildConfigs",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RingerMsgId",
                table: "Scrims");

            migrationBuilder.DropColumn(
                name: "ScrimMsgId",
                table: "Scrims");

            migrationBuilder.DropColumn(
                name: "RingerChannelId",
                table: "GuildConfigs");
        }
    }
}
