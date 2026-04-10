using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class FixNonNullOfferDescription : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Offers",
            type: "nvarchar(400)",
            maxLength: 400,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(400)",
            oldMaxLength: 400,
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Offers",
            type: "nvarchar(400)",
            maxLength: 400,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(400)",
            oldMaxLength: 400);
    }
}
