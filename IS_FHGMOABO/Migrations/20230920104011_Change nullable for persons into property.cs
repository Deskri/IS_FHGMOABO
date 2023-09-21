using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class Changenullableforpersonsintoproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_LegalPersons_LeagalPersonId",
                table: "Properties");

            migrationBuilder.AlterColumn<int>(
                name: "LeagalPersonId",
                table: "Properties",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_LegalPersons_LeagalPersonId",
                table: "Properties",
                column: "LeagalPersonId",
                principalTable: "LegalPersons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_LegalPersons_LeagalPersonId",
                table: "Properties");

            migrationBuilder.AlterColumn<int>(
                name: "LeagalPersonId",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_LegalPersons_LeagalPersonId",
                table: "Properties",
                column: "LeagalPersonId",
                principalTable: "LegalPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
