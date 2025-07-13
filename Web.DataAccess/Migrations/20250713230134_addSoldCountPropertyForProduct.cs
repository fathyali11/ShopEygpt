using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEgypt.Web.Migrations
{
    /// <inheritdoc />
    public partial class addSoldCountPropertyForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoldCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldCount",
                table: "Products");
        }
    }
}
