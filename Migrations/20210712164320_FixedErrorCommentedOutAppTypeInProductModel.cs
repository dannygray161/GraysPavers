using Microsoft.EntityFrameworkCore.Migrations;

namespace GraysPavers.Migrations
{
    public partial class FixedErrorCommentedOutAppTypeInProductModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Product_AppId",
                table: "Product",
                column: "AppId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AppType_AppId",
                table: "Product",
                column: "AppId",
                principalTable: "AppType",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AppType_AppId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_AppId",
                table: "Product");
        }
    }
}
