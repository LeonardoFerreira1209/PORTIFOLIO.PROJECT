using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPLICATION.Migrations
{
    /// <inheritdoc />
    public partial class FEATUREFLAGS_ENTITY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "FeatureFlags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FeatureFlags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "FeatureFlags",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "FeatureFlags");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FeatureFlags");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "FeatureFlags");
        }
    }
}
