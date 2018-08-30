using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Goblin.Migrations.Main
{
    public partial class Editusergroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Group",
                table: "Users",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(short),
                oldDefaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Group",
                table: "Users",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(int),
                oldDefaultValue: 0);
        }
    }
}
