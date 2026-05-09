using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EXE_PET_HUB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "Age", "Name", "Price", "Species" },
                values: new object[,]
                {
                    { 1, 2, "Mochi", 5000000.0, "Chó" },
                    { 2, 1, "Kitty", 3000000.0, "Mèo" },
                    { 3, 3, "Buddy", 7000000.0, "Chó" },
                    { 4, 2, "Coco", 4000000.0, "Mèo" },
                    { 5, 1, "Hamchi", 500000.0, "Hamster" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
