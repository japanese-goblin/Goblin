using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class ChangeCronTimeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hours",
                table: "CronJobs");

            migrationBuilder.DropColumn(
                name: "Minutes",
                table: "CronJobs");

            migrationBuilder.AddColumn<string>(
                name: "Time_DayOfMonth",
                table: "CronJobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time_DayOfWeek",
                table: "CronJobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time_Hour",
                table: "CronJobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time_Minute",
                table: "CronJobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time_Month",
                table: "CronJobs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time_DayOfMonth",
                table: "CronJobs");

            migrationBuilder.DropColumn(
                name: "Time_DayOfWeek",
                table: "CronJobs");

            migrationBuilder.DropColumn(
                name: "Time_Hour",
                table: "CronJobs");

            migrationBuilder.DropColumn(
                name: "Time_Minute",
                table: "CronJobs");

            migrationBuilder.DropColumn(
                name: "Time_Month",
                table: "CronJobs");

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "CronJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Minutes",
                table: "CronJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
