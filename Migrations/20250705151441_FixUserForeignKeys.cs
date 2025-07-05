using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImoSphere.Migrations
{
    /// <inheritdoc />
    public partial class FixUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatConversations_AspNetUsers_ComercialId",
                table: "ChatConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatConversations_AspNetUsers_UserId",
                table: "ChatConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_CreatedByUserId",
                table: "Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatConversations_AspNetUsers_ComercialId",
                table: "ChatConversations",
                column: "ComercialId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatConversations_AspNetUsers_UserId",
                table: "ChatConversations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_CreatedByUserId",
                table: "Properties",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatConversations_AspNetUsers_ComercialId",
                table: "ChatConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatConversations_AspNetUsers_UserId",
                table: "ChatConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_CreatedByUserId",
                table: "Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatConversations_AspNetUsers_ComercialId",
                table: "ChatConversations",
                column: "ComercialId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatConversations_AspNetUsers_UserId",
                table: "ChatConversations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_CreatedByUserId",
                table: "Properties",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
