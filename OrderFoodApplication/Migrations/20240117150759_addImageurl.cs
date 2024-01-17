using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFoodApplication.Migrations
{
    public partial class addImageurl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image_url",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image_url",
                table: "Orders");
        }
    }
}
