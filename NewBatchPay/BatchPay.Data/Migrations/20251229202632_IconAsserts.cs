using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    /// <inheritdoc />
    public partial class IconAsserts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconKey",
                schema: "dbo",
                table: "GroupPayments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconKey",
                schema: "dbo",
                table: "GroupPayments");
        }
    }
}
