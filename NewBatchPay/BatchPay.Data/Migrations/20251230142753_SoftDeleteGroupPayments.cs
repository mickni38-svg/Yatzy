using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteGroupPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAtUtc",
                schema: "dbo",
                table: "GroupPayments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "GroupPayments",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAtUtc",
                schema: "dbo",
                table: "GroupPaymentMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "GroupPaymentMembers",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivatedAtUtc",
                schema: "dbo",
                table: "GroupPayments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "GroupPayments");

            migrationBuilder.DropColumn(
                name: "DeactivatedAtUtc",
                schema: "dbo",
                table: "GroupPaymentMembers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "GroupPaymentMembers");
        }
    }
}
