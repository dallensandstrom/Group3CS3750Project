using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupThreeTrailerParkProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPriceRangeForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceRanges_Site_SiteNumber",
                table: "PriceRanges");

            migrationBuilder.RenameColumn(
                name: "SiteNumber",
                table: "PriceRanges",
                newName: "SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_PriceRanges_SiteNumber",
                table: "PriceRanges",
                newName: "IX_PriceRanges_SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceRanges_Site_SiteId",
                table: "PriceRanges",
                column: "SiteId",
                principalTable: "Site",
                principalColumn: "SiteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceRanges_Site_SiteId",
                table: "PriceRanges");

            migrationBuilder.RenameColumn(
                name: "SiteId",
                table: "PriceRanges",
                newName: "SiteNumber");

            migrationBuilder.RenameIndex(
                name: "IX_PriceRanges_SiteId",
                table: "PriceRanges",
                newName: "IX_PriceRanges_SiteNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceRanges_Site_SiteNumber",
                table: "PriceRanges",
                column: "SiteNumber",
                principalTable: "Site",
                principalColumn: "SiteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
