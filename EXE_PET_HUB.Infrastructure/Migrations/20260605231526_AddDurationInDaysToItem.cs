using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE_PET_HUB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationInDaysToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "Item",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "Item");
        }
    }
}
