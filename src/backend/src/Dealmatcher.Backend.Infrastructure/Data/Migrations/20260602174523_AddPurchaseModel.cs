using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddPurchaseModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Activities",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                OfferId = table.Column<int>(type: "int", nullable: true),
                Action = table.Column<int>(type: "int", nullable: false),
                IPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Activities", x => x.Id);
                table.ForeignKey(
                    name: "FK_Activities_Offers_OfferId",
                    column: x => x.OfferId,
                    principalTable: "Offers",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Activities_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Purchases",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BuyerId = table.Column<int>(type: "int", nullable: false),
                OfferId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                DeliveryProviderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                PaymentProviderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                PaymentSessionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Purchases", x => x.Id);
                table.ForeignKey(
                    name: "FK_Purchases_Offers_OfferId",
                    column: x => x.OfferId,
                    principalTable: "Offers",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Purchases_Users_BuyerId",
                    column: x => x.BuyerId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Activities_OfferId",
            table: "Activities",
            column: "OfferId");

        migrationBuilder.CreateIndex(
            name: "IX_Activities_UserId",
            table: "Activities",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Purchases_BuyerId",
            table: "Purchases",
            column: "BuyerId");

        migrationBuilder.CreateIndex(
            name: "IX_Purchases_OfferId",
            table: "Purchases",
            column: "OfferId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Activities");

        migrationBuilder.DropTable(
            name: "Purchases");
    }
}
