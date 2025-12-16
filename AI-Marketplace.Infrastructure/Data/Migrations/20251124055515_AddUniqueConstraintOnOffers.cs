using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Marketplace.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintOnOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offers_CustomRequestId",
                table: "Offers");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CustomRequestId_StoreId",
                table: "Offers",
                columns: new[] { "CustomRequestId", "StoreId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offers_CustomRequestId_StoreId",
                table: "Offers");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CustomRequestId",
                table: "Offers",
                column: "CustomRequestId");
        }
    }
}
