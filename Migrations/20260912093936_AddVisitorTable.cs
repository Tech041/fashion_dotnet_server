using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceServer.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    VisitorId = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_CreatedAt",
                table: "Visitors",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_VisitorId_Date",
                table: "Visitors",
                columns: new[] { "VisitorId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visitors");
        }
    }
}
