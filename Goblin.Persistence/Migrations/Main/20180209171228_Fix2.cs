using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Persistence.Migrations.Main
{
    public partial class Fix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "Users");

            migrationBuilder.AlterColumn<bool>(
                name: "Weather",
                table: "Users",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Schedule",
                table: "Users",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<int>(
                name: "CityNumber",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "Group",
                table: "Users",
                nullable: false,
                defaultValue: (short) 0);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Reminds",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Users");

            migrationBuilder.AlterColumn<bool>(
                name: "Weather",
                table: "Users",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Schedule",
                table: "Users",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CityID",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "GroupID",
                table: "Users",
                nullable: false,
                defaultValue: (short) 0);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Reminds",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}