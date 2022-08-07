using Microsoft.EntityFrameworkCore.Migrations;

namespace DNIC.Migrations
{
    public partial class productRealId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RealId",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealId",
                table: "Products");
        }
    }
}
