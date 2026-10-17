using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoboForge.Modules.Sales.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateSalesOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Price = table.Column<string>(type: "jsonb", nullable: false),
                    PlacedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrackingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CompletedStages = table.Column<string>(type: "jsonb", nullable: false),
                    Notes = table.Column<string>(type: "jsonb", nullable: false),
                    Parts = table.Column<string>(type: "jsonb", nullable: false),
                    LeadTime_WorkingDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ConfigurationId",
                schema: "sales",
                table: "Orders",
                column: "ConfigurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PlacedOnUtc",
                schema: "sales",
                table: "Orders",
                column: "PlacedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders",
                schema: "sales");
        }
    }
}
