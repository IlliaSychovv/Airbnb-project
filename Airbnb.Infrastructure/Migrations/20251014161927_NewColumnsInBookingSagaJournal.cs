using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnsInBookingSagaJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "BookingSagaJournals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "BookingSagaJournals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "BookingSagaJournals");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "BookingSagaJournals");
        }
    }
}
