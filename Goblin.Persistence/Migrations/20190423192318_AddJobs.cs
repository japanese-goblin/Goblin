using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Goblin.Persistence.Migrations
{
    public partial class AddJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<int>(
                name: "Group",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    JobName = table.Column<string>(nullable: true),
                    Conversation = table.Column<int>(nullable: false),
                    NarfuGroup = table.Column<int>(nullable: false),
                    WeatherCity = table.Column<string>(nullable: true),
                    Hours = table.Column<int>(nullable: false),
                    Minutes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

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

            migrationBuilder.AlterColumn<int>(
                name: "Group",
                table: "Users",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int));
        }
    }
}
