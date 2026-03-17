using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupThreeTrailerParkProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSitePhotoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SitePhoto_Site_SiteId",
                table: "SitePhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SitePhoto",
                table: "SitePhoto");

            migrationBuilder.RenameTable(
                name: "SitePhoto",
                newName: "SitePhotos");

            migrationBuilder.RenameIndex(
                name: "IX_SitePhoto_SiteId",
                table: "SitePhotos",
                newName: "IX_SitePhotos_SiteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SitePhotos",
                table: "SitePhotos",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_SitePhotos_Site_SiteId",
                table: "SitePhotos",
                column: "SiteId",
                principalTable: "Site",
                principalColumn: "SiteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SitePhotos_Site_SiteId",
                table: "SitePhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SitePhotos",
                table: "SitePhotos");

            migrationBuilder.RenameTable(
                name: "SitePhotos",
                newName: "SitePhoto");

            migrationBuilder.RenameIndex(
                name: "IX_SitePhotos_SiteId",
                table: "SitePhoto",
                newName: "IX_SitePhoto_SiteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SitePhoto",
                table: "SitePhoto",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_SitePhoto_Site_SiteId",
                table: "SitePhoto",
                column: "SiteId",
                principalTable: "Site",
                principalColumn: "SiteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
