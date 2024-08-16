using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEgypt.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddImageForCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Categories");
        }
    }
}
