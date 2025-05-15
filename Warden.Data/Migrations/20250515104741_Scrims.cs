using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warden.Data.Migrations
{
    /// <inheritdoc />
    public partial class Scrims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scrims_Teams_Team2Id",
                table: "Scrims");

            migrationBuilder.AlterColumn<int>(
                name: "Team2Id",
                table: "Scrims",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Scrims_Teams_Team2Id",
                table: "Scrims",
                column: "Team2Id",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scrims_Teams_Team2Id",
                table: "Scrims");

            migrationBuilder.AlterColumn<int>(
                name: "Team2Id",
                table: "Scrims",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Scrims_Teams_Team2Id",
                table: "Scrims",
                column: "Team2Id",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
