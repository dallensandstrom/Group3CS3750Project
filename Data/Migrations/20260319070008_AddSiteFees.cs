using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupThreeTrailerParkProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteFees",
                columns: table => new
                {
                    SiteFeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeID = table.Column<int>(type: "int", nullable: false),
                    SiteNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteFees", x => x.SiteFeeID);
                    table.ForeignKey(
                        name: "FK_SiteFees_Fees_FeeID",
                        column: x => x.FeeID,
                        principalTable: "Fees",
                        principalColumn: "FeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiteFees_Site_SiteNumber",
                        column: x => x.SiteNumber,
                        principalTable: "Site",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteFees_FeeID",
                table: "SiteFees",
                column: "FeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteFees_SiteNumber",
                table: "SiteFees",
                column: "SiteNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteFees");
        }
    }
}
