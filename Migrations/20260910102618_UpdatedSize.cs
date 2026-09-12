using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Products",
                newName: "Sizes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sizes",
                table: "Products",
                newName: "Size");
        }
    }
}
