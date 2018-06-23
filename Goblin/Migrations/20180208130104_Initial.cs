using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    CityID = table.Column<int>(nullable: false),
                    GroupID = table.Column<short>(nullable: false),
                    Schedule = table.Column<bool>(nullable: false),
                    VkID = table.Column<int>(nullable: false, defaultValueSql: "0"),
                    Weather = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Persons", x => x.ID); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}