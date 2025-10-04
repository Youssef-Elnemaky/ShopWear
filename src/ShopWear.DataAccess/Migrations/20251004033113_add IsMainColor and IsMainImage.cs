using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWear.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addIsMainColorandIsMainImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMainImage",
                table: "ProductImage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMainColor",
                table: "ProductColor",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMainImage",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "IsMainColor",
                table: "ProductColor");
        }
    }
}
