using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class SyncAfterMerge : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EstimatedDays",
            table: "DeliveryMethods");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "EstimatedDays",
            table: "DeliveryMethods",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
