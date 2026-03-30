using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace otrs_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaHoursToPriorities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SlaHours",
                table: "Priorities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 48);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlaHours",
                table: "Priorities");
        }
    }
}
