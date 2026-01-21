using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyZombieProject.App.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthToSurvivors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Health",
                table: "Survivors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnMission",
                table: "Survivors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Health",
                table: "Survivors");

            migrationBuilder.DropColumn(
                name: "IsOnMission",
                table: "Survivors");
        }
    }
}
