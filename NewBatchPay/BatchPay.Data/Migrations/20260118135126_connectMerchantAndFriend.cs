using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    /// <inheritdoc />
    public partial class connectMerchantAndFriend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_Users_ReceiverUserId",
                schema: "dbo",
                table: "FriendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_Users_RequesterUserId",
                schema: "dbo",
                table: "FriendRequest");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequest_ReceiverUserId",
                schema: "dbo",
                table: "FriendRequest");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "dbo",
                table: "FriendRequest");

            migrationBuilder.AddColumn<int>(
                name: "ReceiverType",
                schema: "dbo",
                table: "FriendRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequesterType",
                schema: "dbo",
                table: "FriendRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverType",
                schema: "dbo",
                table: "FriendRequest");

            migrationBuilder.DropColumn(
                name: "RequesterType",
                schema: "dbo",
                table: "FriendRequest");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "dbo",
                table: "FriendRequest",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequest_ReceiverUserId",
                schema: "dbo",
                table: "FriendRequest",
                column: "ReceiverUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_Users_ReceiverUserId",
                schema: "dbo",
                table: "FriendRequest",
                column: "ReceiverUserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_Users_RequesterUserId",
                schema: "dbo",
                table: "FriendRequest",
                column: "RequesterUserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
