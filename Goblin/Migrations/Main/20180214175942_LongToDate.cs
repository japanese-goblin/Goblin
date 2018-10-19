using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Goblin.Migrations.Main
{
    public partial class LongToDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reminds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    VkID = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Reminds", x => x.ID); });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    City = table.Column<string>(nullable: true),
                    CityNumber = table.Column<int>(nullable: false, defaultValue: 0),
                    Group = table.Column<short>(nullable: false, defaultValue: (short) 0),
                    Schedule = table.Column<bool>(nullable: false, defaultValue: false),
                    Vk = table.Column<int>(nullable: false),
                    Weather = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.ID); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminds");

            migrationBuilder.DropTable(
                name: "Users");

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
    }
}