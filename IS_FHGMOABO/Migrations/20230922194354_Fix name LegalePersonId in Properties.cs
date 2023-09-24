using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class FixnameLegalePersonIdinProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_LegalPersons_LeagalPersonId",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "LeagalPersonId",
                table: "Properties",
                newName: "LegalPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_LeagalPersonId",
                table: "Properties",
                newName: "IX_Properties_LegalPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_LegalPersons_LegalPersonId",
                table: "Properties",
                column: "LegalPersonId",
                principalTable: "LegalPersons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_LegalPersons_LegalPersonId",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "LegalPersonId",
                table: "Properties",
                newName: "LeagalPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_LegalPersonId",
                table: "Properties",
                newName: "IX_Properties_LeagalPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_LegalPersons_LeagalPersonId",
                table: "Properties",
                column: "LeagalPersonId",
                principalTable: "LegalPersons",
                principalColumn: "Id");
        }
    }
}
