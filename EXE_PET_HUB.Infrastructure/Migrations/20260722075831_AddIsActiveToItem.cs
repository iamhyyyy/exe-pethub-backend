using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE_PET_HUB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Item",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Đặt IsActive = true cho tất cả item hiện tại (item mới tạo trước khi có cột này)
            migrationBuilder.Sql("UPDATE \"Item\" SET \"IsActive\" = true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Item");
        }
    }
}
