using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Goblin.Persistence.Migrations.Main
{
    public partial class Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    City = table.Column<string>(nullable: true),
                    CityID = table.Column<int>(nullable: false),
                    GroupID = table.Column<short>(nullable: false),
                    Schedule = table.Column<bool>(nullable: false),
                    Vk = table.Column<int>(nullable: false),
                    Weather = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.ID); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    VkID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    City = table.Column<string>(nullable: true),
                    CityID = table.Column<int>(nullable: false),
                    GroupID = table.Column<short>(nullable: false),
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Schedule = table.Column<bool>(nullable: false),
                    Weather = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Persons", x => x.VkID); });
        }
    }
}