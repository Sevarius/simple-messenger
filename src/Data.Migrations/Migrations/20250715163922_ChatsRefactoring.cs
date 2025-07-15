using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ChatsRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chats_users_creator_id",
                table: "chats");

            migrationBuilder.RenameIndex(
                name: "ix_chats_creator_id",
                table: "chats",
                newName: "IX_chats_creator_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "creator_id",
                table: "chats",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "idempotency_key",
                table: "chats",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_chats_idempotency_key",
                table: "chats",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_chats_users_creator_id",
                table: "chats",
                column: "creator_id",
                principalTable: "users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chats_users_creator_id",
                table: "chats");

            migrationBuilder.DropIndex(
                name: "IX_chats_idempotency_key",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "idempotency_key",
                table: "chats");

            migrationBuilder.RenameIndex(
                name: "IX_chats_creator_id",
                table: "chats",
                newName: "ix_chats_creator_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "creator_id",
                table: "chats",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "fk_chats_users_creator_id",
                table: "chats",
                column: "creator_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
