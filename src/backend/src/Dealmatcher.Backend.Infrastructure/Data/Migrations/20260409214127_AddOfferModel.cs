using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dealmatcher.Backend.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddOfferModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PropertyRelatedEnums",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PropertyRelatedEnums", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Offers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                SellerId = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                Availability = table.Column<int>(type: "int", nullable: false),
                CategoryId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Offers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Offers_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Offers_Users_SellerId",
                    column: x => x.SellerId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "PropertyDefinitions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                CategoryId = table.Column<int>(type: "int", nullable: false),
                DefinitionType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                PropertyRelatedEnumId = table.Column<int>(type: "int", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PropertyDefinitions", x => x.Id);
                table.ForeignKey(
                    name: "FK_PropertyDefinitions_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_PropertyDefinitions_PropertyRelatedEnums_PropertyRelatedEnumId",
                    column: x => x.PropertyRelatedEnumId,
                    principalTable: "PropertyRelatedEnums",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "PropertyRelatedEnumValues",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PropertyRelatedEnumId = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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

        migrationBuilder.CreateTable(
            name: "Properties",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PropertyDefinitionId = table.Column<int>(type: "int", nullable: false),
                OfferId = table.Column<int>(type: "int", nullable: false),
                PropertyType = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                BooleanValue = table.Column<bool>(type: "bit", nullable: true),
                NumericValue = table.Column<double>(type: "float", nullable: true),
                SelectValue = table.Column<int>(type: "int", nullable: true),
                TextValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Properties", x => x.Id);
                table.ForeignKey(
                    name: "FK_Properties_Offers_OfferId",
                    column: x => x.OfferId,
                    principalTable: "Offers",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Properties_PropertyDefinitions_PropertyDefinitionId",
                    column: x => x.PropertyDefinitionId,
                    principalTable: "PropertyDefinitions",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Offers_CategoryId",
            table: "Offers",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Offers_SellerId",
            table: "Offers",
            column: "SellerId");

        migrationBuilder.CreateIndex(
            name: "IX_Properties_OfferId",
            table: "Properties",
            column: "OfferId");

        migrationBuilder.CreateIndex(
            name: "IX_Properties_PropertyDefinitionId",
            table: "Properties",
            column: "PropertyDefinitionId");

        migrationBuilder.CreateIndex(
            name: "IX_PropertyDefinitions_CategoryId",
            table: "PropertyDefinitions",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_PropertyDefinitions_PropertyRelatedEnumId",
            table: "PropertyDefinitions",
            column: "PropertyRelatedEnumId");

        migrationBuilder.CreateIndex(
            name: "IX_PropertyRelatedEnumValues_PropertyRelatedEnumId",
            table: "PropertyRelatedEnumValues",
            column: "PropertyRelatedEnumId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Properties");

        migrationBuilder.DropTable(
            name: "PropertyRelatedEnumValues");

        migrationBuilder.DropTable(
            name: "Offers");

        migrationBuilder.DropTable(
            name: "PropertyDefinitions");

        migrationBuilder.DropTable(
            name: "Categories");

        migrationBuilder.DropTable(
            name: "PropertyRelatedEnums");
    }
}
