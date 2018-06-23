using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Migrations.Main
{
    public partial class RemindTimeToLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Timestamp",
                table: "Reminds",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Timestamp",
                table: "Reminds",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}