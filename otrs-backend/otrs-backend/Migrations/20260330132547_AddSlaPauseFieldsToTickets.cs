using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace otrs_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaPauseFieldsToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PausedAtUtc",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalPausedMinutes",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PausedAtUtc",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TotalPausedMinutes",
                table: "Tickets");
        }
    }
}
