using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNIC.Migrations
{
    public partial class productStoreImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreImage",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "StoreImageUrl",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreImageUrl",
                table: "Products");

            migrationBuilder.AddColumn<byte[]>(
                name: "StoreImage",
                table: "Products",
                type: "bytea",
                nullable: true);
        }
    }
}
