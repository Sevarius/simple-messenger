using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UserChatReadStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_chats_creator_id",
                table: "chats",
                newName: "ix_chats_creator_id");

            migrationBuilder.AddColumn<long>(
                name: "row_version",
                table: "messages",
                type: "bigint",
                rowVersion: true,
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "chats",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_message_timestamp",
                table: "chats",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "row_version",
                table: "chats",
                type: "bigint",
                rowVersion: true,
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "user_chat_read_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_chat_read_status_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    chat_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    last_read_message_timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    row_version = table.Column<long>(type: "bigint", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_chat_read_statuses", x => x.user_chat_read_status_id);
                    table.ForeignKey(
                        name: "fk_user_chat_read_statuses_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "chat_id");
                    table.ForeignKey(
                        name: "fk_user_chat_read_statuses_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_chat_read_statuses_chat_id_user_id",
                table: "user_chat_read_statuses",
                columns: new[] { "chat_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_chat_read_statuses_user_id",
                table: "user_chat_read_statuses",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_chat_read_statuses");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "last_message_timestamp",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "chats");

            migrationBuilder.RenameIndex(
                name: "ix_chats_creator_id",
                table: "chats",
                newName: "IX_chats_creator_id");
        }
    }
}
