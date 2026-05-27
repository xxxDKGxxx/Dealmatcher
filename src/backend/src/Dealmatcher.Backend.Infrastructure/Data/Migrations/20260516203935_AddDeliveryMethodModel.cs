using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddDeliveryMethodModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DeliveryMethods",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                StringId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                EstimatedDays = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeliveryMethods", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DeliveryMethods");
    }
}
