using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberTypeToGroupPaymentMembers2 : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            // Drop the existing index that references RequesterType before dropping the column.
            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_Requester_Receiver",
                table: "FriendRequest" );

            // Now it's safe to drop the RequesterType column
            migrationBuilder.DropColumn(
                name: "RequesterType",
                table: "FriendRequest" );

            // Add the new MemberType column on GroupPaymentMembers
            migrationBuilder.AddColumn<byte>(
                name: "MemberType",
                table: "GroupPaymentMembers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0 );

            // Recreate the index according to the current model (adjust columns if your model differs).
            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_Requester_Receiver",
                table: "FriendRequest",
                columns: new[] { "RequesterUserId", "ReceiverUserId" },
                unique: true );
        }


        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            // Reverse: drop the adjusted index, drop MemberType, add back RequesterType and recreate original index
            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_Requester_Receiver",
                table: "FriendRequest" );

            migrationBuilder.DropColumn(
                name: "MemberType",
                table: "GroupPaymentMembers" );

            migrationBuilder.AddColumn<byte>(
                name: "RequesterType",
                table: "FriendRequest",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0 );

            // Recreate the original index that included RequesterType.
            // Adjust the column order to exactly match the original DB index if needed.
            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_Requester_Receiver",
                table: "FriendRequest",
                columns: new[] { "RequesterUserId", "RequesterType", "ReceiverUserId" },
                unique: true );
        }
    }
}
