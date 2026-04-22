using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class ImagesAndTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Images",
            table: "Offers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "[]");

        migrationBuilder.AddColumn<string>(
            name: "Tags",
            table: "Offers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "[]");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Images",
            table: "Offers");

        migrationBuilder.DropColumn(
            name: "Tags",
            table: "Offers");
    }
}
