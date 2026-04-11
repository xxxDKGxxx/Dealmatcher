using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class RemovePropertyRelatedEnums : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_PropertyDefinitions_PropertyRelatedEnums_PropertyRelatedEnumId",
            table: "PropertyDefinitions");

        migrationBuilder.DropTable(
            name: "PropertyRelatedEnumValues");

        migrationBuilder.DropTable(
            name: "PropertyRelatedEnums");

        migrationBuilder.DropIndex(
            name: "IX_PropertyDefinitions_PropertyRelatedEnumId",
            table: "PropertyDefinitions");

        migrationBuilder.DropColumn(
            name: "PropertyRelatedEnumId",
            table: "PropertyDefinitions");

        migrationBuilder.AddColumn<string>(
            name: "Values",
            table: "PropertyDefinitions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "SelectValue",
            table: "Properties",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Categories",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Values",
            table: "PropertyDefinitions");

        migrationBuilder.DropColumn(
            name: "Description",
            table: "Categories");

        migrationBuilder.AddColumn<int>(
            name: "PropertyRelatedEnumId",
            table: "PropertyDefinitions",
            type: "int",
            nullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "SelectValue",
            table: "Properties",
            type: "int",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "PropertyRelatedEnums",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PropertyRelatedEnums", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PropertyRelatedEnumValues",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PropertyRelatedEnumId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Value = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PropertyRelatedEnumValues", x => x.Id);
                table.ForeignKey(
                    name: "FK_PropertyRelatedEnumValues_PropertyRelatedEnums_PropertyRelatedEnumId",
                    column: x => x.PropertyRelatedEnumId,
                    principalTable: "PropertyRelatedEnums",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_PropertyDefinitions_PropertyRelatedEnumId",
            table: "PropertyDefinitions",
            column: "PropertyRelatedEnumId");

        migrationBuilder.CreateIndex(
            name: "IX_PropertyRelatedEnumValues_PropertyRelatedEnumId",
            table: "PropertyRelatedEnumValues",
            column: "PropertyRelatedEnumId");

        migrationBuilder.AddForeignKey(
            name: "FK_PropertyDefinitions_PropertyRelatedEnums_PropertyRelatedEnumId",
            table: "PropertyDefinitions",
            column: "PropertyRelatedEnumId",
            principalTable: "PropertyRelatedEnums",
            principalColumn: "Id");
    }
}
