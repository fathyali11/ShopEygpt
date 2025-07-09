using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEgypt.Web.Migrations
{
    /// <inheritdoc />
    public partial class updateColumnNameOfCategoryCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Categories",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Categories",
                newName: "CreatedDate");
        }
    }
}
