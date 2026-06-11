using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE_PET_HUB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayOsOrderCodeToInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PayOsOrderCode",
                table: "Invoice",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayOsOrderCode",
                table: "Invoice");
        }
    }
}
