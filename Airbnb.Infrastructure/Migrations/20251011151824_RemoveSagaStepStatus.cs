using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSagaStepStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookingSagaJournals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BookingSagaJournals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
