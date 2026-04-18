using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddIsDeleted : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Users",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "PropertyDefinitions",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Properties",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Offers",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Categories",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "PropertyDefinitions");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Properties");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Offers");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Categories");
    }
}
