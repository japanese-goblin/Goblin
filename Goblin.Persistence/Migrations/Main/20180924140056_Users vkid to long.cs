using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Persistence.Migrations.Main
{
    public partial class Usersvkidtolong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Vk",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "VkID",
                table: "Reminds",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Vk",
                table: "Users",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "VkID",
                table: "Reminds",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}