using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yatzy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerHasLeft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasLeft",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasLeft",
                table: "Players");
        }
    }
}
