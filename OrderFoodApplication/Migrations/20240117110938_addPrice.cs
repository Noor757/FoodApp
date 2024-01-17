using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFoodApplication.Migrations
{
    public partial class addPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Carts");
        }
    }
}
