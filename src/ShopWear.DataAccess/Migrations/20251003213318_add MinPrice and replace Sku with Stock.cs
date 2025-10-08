using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWear.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addMinPriceandreplaceSkuwithStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "ProductVariant",
                newName: "Stock");

            migrationBuilder.AddColumn<decimal>(
                name: "MinPrice",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "ProductVariant",
                newName: "Sku");
        }
    }
}
