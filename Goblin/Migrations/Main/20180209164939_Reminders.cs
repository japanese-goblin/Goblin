using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Migrations.Main
{
    public partial class Reminders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Reminds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Text = table.Column<string>(nullable: false),
                    VkID = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Reminds", x => x.ID); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Reminds");
        }
    }
}