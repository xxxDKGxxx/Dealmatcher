using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddDeliveryServiceProviders : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "StringId",
            table: "DeliveryMethods",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(250)",
            oldMaxLength: 250);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DeliveryMethods",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100);

        migrationBuilder.AddColumn<string>(
            name: "ProviderName",
            table: "DeliveryMethods",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ProviderName",
            table: "DeliveryMethods");

        migrationBuilder.AlterColumn<string>(
            name: "StringId",
            table: "DeliveryMethods",
            type: "nvarchar(250)",
            maxLength: 250,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DeliveryMethods",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);
    }
}
