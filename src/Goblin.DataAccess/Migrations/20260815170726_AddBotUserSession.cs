using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Goblin.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBotUserSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotUserSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    BotUserId = table.Column<long>(type: "bigint", nullable: false),
                    FlowType = table.Column<int>(type: "integer", nullable: false),
                    FlowStepType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUserSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotUsers_Id",
                table: "BotUsers",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BotUsers_BotUserSessions_Id",
                table: "BotUsers",
                column: "Id",
                principalTable: "BotUserSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotUsers_BotUserSessions_Id",
                table: "BotUsers");

            migrationBuilder.DropTable(
                name: "BotUserSessions");

            migrationBuilder.DropIndex(
                name: "IX_BotUsers_Id",
                table: "BotUsers");
        }
    }
}
