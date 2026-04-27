using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    /// <inheritdoc />
    public partial class UnifyUserAndMerchantModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivatedAtUtc",
                schema: "dbo",
                table: "GroupPayments");

            migrationBuilder.RenameTable(
                name: "MerchantIntegrations",
                schema: "dbo",
                newName: "MerchantIntegrations");

            migrationBuilder.RenameTable(
                name: "GroupPayments",
                schema: "dbo",
                newName: "GroupPayments");

            migrationBuilder.RenameTable(
                name: "GroupPaymentMembers",
                schema: "dbo",
                newName: "GroupPaymentMembers");

            migrationBuilder.AlterColumn<string>(
                name: "WebhookUrl",
                table: "MerchantIntegrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "SigningSecretHash",
                table: "MerchantIntegrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "MerchantIntegrations",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultReturnUrl",
                table: "MerchantIntegrations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApiKeyHash",
                table: "MerchantIntegrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "AllowedOrigin",
                table: "MerchantIntegrations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "GroupPayments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "MerchantIntegrations",
                newName: "MerchantIntegrations",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "GroupPayments",
                newName: "GroupPayments",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "GroupPaymentMembers",
                newName: "GroupPaymentMembers",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "WebhookUrl",
                schema: "dbo",
                table: "MerchantIntegrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SigningSecretHash",
                schema: "dbo",
                table: "MerchantIntegrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                schema: "dbo",
                table: "MerchantIntegrations",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultReturnUrl",
                schema: "dbo",
                table: "MerchantIntegrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApiKeyHash",
                schema: "dbo",
                table: "MerchantIntegrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AllowedOrigin",
                schema: "dbo",
                table: "MerchantIntegrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "GroupPayments",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAtUtc",
                schema: "dbo",
                table: "GroupPayments",
                type: "datetime2",
                nullable: true);
        }
    }
}
