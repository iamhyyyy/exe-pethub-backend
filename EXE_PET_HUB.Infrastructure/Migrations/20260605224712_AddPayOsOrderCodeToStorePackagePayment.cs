using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE_PET_HUB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayOsOrderCodeToStorePackagePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PremiumExpiredAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "StorePackagePayment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "PayOsOrderCode",
                table: "StorePackagePayment",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremiumExpiredAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "StorePackagePayment");

            migrationBuilder.DropColumn(
                name: "PayOsOrderCode",
                table: "StorePackagePayment");
        }
    }
}
