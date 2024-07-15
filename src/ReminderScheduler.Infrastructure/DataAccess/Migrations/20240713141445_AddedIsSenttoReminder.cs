using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderScheduler.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsSenttoReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "Reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "Reminders");
        }
    }
}
