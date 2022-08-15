using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmptyAPI.Migrations
{
    public partial class mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExcelDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Segment = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Product = table.Column<string>(nullable: true),
                    DiscountBand = table.Column<string>(nullable: true),
                    UnitsSold = table.Column<double>(nullable: false),
                    ManufacturingPrice = table.Column<double>(nullable: false),
                    SellPrice = table.Column<double>(nullable: false),
                    GrossSales = table.Column<double>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    Sale = table.Column<double>(nullable: false),
                    COGS = table.Column<double>(nullable: false),
                    Profit = table.Column<double>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelDatas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcelDatas");
        }
    }
}
