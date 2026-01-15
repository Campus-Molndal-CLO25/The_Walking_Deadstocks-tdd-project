using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyZombieProject.App.Migrations
{
    /// <inheritdoc />
    public partial class MakeShelterIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShelterId",
                table: "Supplies",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShelterId",
                table: "Supplies");
        }
    }
}
