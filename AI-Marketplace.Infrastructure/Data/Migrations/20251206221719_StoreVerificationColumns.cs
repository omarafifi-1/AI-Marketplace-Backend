using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Marketplace.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class StoreVerificationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "VerifiedAt",
                table: "Stores",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "Stores");
        }
    }
}
