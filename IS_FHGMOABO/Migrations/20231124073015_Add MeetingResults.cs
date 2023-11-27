using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeetingResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AreaFor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AreaAgainst = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AreaAbstained = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingResults_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingResults_QuestionId",
                table: "MeetingResults",
                column: "QuestionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingResults");
        }
    }
}
