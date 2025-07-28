using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEgypt.Web.Migrations
{
    /// <inheritdoc />
    public partial class removeSalesFromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSale",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SaleEndDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalePercentage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SaleStartDate",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSale",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SaleEndDate",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalePercentage",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SaleStartDate",
                table: "Products",
                type: "datetime2",
                nullable: true);
        }
    }
}
