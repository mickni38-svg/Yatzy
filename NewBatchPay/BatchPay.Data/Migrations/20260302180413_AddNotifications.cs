using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MerchantId",
                table: "GroupPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GroupPaymentId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_DirectoryEntries_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "DirectoryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupPayments_MerchantId",
                table: "GroupPayments",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ToUserId_CreatedAtUtc",
                table: "Notifications",
                columns: new[] { "ToUserId", "CreatedAtUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPayments_DirectoryEntries_MerchantId",
                table: "GroupPayments",
                column: "MerchantId",
                principalTable: "DirectoryEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPayments_DirectoryEntries_MerchantId",
                table: "GroupPayments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_GroupPayments_MerchantId",
                table: "GroupPayments");

            migrationBuilder.DropColumn(
                name: "MerchantId",
                table: "GroupPayments");
        }
    }
}
