using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class Deleteuniqueofnumberofrooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_Number",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Number",
                table: "Rooms",
                column: "Number",
                unique: true);
        }
    }
}
