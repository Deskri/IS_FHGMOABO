using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class MakeuniqueNumberinRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Houses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Houses_Number",
                table: "Houses",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Houses_Number",
                table: "Houses");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Houses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
