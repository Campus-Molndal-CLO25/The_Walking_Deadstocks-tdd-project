using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyZombieProject.App.Migrations
{
    /// <inheritdoc />
    public partial class CapacityOnStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Shelters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Shelters");
        }
    }
}
