using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Marketplace.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "MasterOrderId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MasterOrderId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MasterOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyerId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterOrders_Addresses_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterOrders_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MasterOrderId",
                table: "Payments",
                column: "MasterOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MasterOrderId",
                table: "Orders",
                column: "MasterOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterOrders_BuyerId",
                table: "MasterOrders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterOrders_ShippingAddressId",
                table: "MasterOrders",
                column: "ShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MasterOrders_MasterOrderId",
                table: "Orders",
                column: "MasterOrderId",
                principalTable: "MasterOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_MasterOrders_MasterOrderId",
                table: "Payments",
                column: "MasterOrderId",
                principalTable: "MasterOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MasterOrders_MasterOrderId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_MasterOrders_MasterOrderId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "MasterOrders");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MasterOrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Orders_MasterOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MasterOrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MasterOrderId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
