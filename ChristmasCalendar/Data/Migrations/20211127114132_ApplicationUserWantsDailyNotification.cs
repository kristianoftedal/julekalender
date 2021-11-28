using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChristmasCalendar.Data.Migrations
{
    public partial class ApplicationUserWantsDailyNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WantsDailyNotification",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WantsDailyNotification",
                table: "AspNetUsers");
        }
    }
}
