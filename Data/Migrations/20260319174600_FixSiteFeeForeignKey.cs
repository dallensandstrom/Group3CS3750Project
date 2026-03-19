using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupThreeTrailerParkProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSiteFeeForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteFees_Site_SiteNumber",
                table: "SiteFees");

            migrationBuilder.RenameColumn(
                name: "SiteNumber",
                table: "SiteFees",
                newName: "SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_SiteFees_SiteNumber",
                table: "SiteFees",
                newName: "IX_SiteFees_SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteFees_Site_SiteId",
                table: "SiteFees",
                column: "SiteId",
                principalTable: "Site",
                principalColumn: "SiteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteFees_Site_SiteId",
                table: "SiteFees");

            migrationBuilder.RenameColumn(
                name: "SiteId",
                table: "SiteFees",
                newName: "SiteNumber");

            migrationBuilder.RenameIndex(
                name: "IX_SiteFees_SiteId",
                table: "SiteFees",
                newName: "IX_SiteFees_SiteNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteFees_Site_SiteNumber",
                table: "SiteFees",
                column: "SiteNumber",
                principalTable: "Site",
                principalColumn: "SiteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
