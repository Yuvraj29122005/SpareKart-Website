using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpareKart_Website.Migrations
{
    /// <inheritdoc />
    public partial class AddStockQtyToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQty",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQty",
                table: "Products");
        }
    }
}
