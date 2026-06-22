using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE_PET_HUB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPlaformPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatformPlan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DurationInDays = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformPlan", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformPlan");
        }
    }
}
