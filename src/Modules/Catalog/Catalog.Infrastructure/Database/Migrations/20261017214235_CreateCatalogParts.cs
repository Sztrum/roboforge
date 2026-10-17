using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoboForge.Modules.Catalog.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateCatalogParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "Parts",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDiscontinued = table.Column<bool>(type: "boolean", nullable: false),
                    LeadTime_WorkingDays = table.Column<int>(type: "integer", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Price_Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TechSpec_MaxLoadGrams = table.Column<int>(type: "integer", nullable: true),
                    TechSpec_MountType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TechSpec_VoltageVolts = table.Column<int>(type: "integer", nullable: true),
                    TechSpec_WeightGrams = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parts_Category",
                schema: "catalog",
                table: "Parts",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parts",
                schema: "catalog");
        }
    }
}
