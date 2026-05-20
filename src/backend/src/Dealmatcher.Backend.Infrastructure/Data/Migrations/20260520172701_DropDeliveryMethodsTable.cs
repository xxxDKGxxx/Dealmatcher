using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class DropDeliveryMethodsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DeliveryMethods");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DeliveryMethods",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                ProviderName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                StringId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeliveryMethods", x => x.Id);
            });
    }
}
