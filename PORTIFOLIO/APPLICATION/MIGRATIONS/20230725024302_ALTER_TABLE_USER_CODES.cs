using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPLICATION.Migrations
{
    /// <inheritdoc />
    public partial class ALTER_TABLE_USER_CODES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConfirmationCodes",
                table: "UserConfirmationCodes");

            migrationBuilder.RenameTable(
                name: "UserConfirmationCodes",
                newName: "AspNetUserCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserCodes",
                table: "AspNetUserCodes",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserCodes",
                table: "AspNetUserCodes");

            migrationBuilder.RenameTable(
                name: "AspNetUserCodes",
                newName: "UserConfirmationCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConfirmationCodes",
                table: "UserConfirmationCodes",
                column: "Id");
        }
    }
}
