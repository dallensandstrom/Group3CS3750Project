using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupThreeTrailerParkProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkReservationToSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SiteNumber",
                table: "Reservations",
                newName: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SiteId",
                table: "Reservations",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Site_SiteId",
                table: "Reservations",
                column: "SiteId",
                principalTable: "Site",
                principalColumn: "SiteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Site_SiteId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_SiteId",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "SiteId",
                table: "Reservations",
                newName: "SiteNumber");
        }
    }
}
