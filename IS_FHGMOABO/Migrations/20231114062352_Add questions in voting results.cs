using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class Addquestionsinvotingresults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "VotingResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VotingResults_QuestionId",
                table: "VotingResults",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_VotingResults_Questions_QuestionId",
                table: "VotingResults",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotingResults_Questions_QuestionId",
                table: "VotingResults");

            migrationBuilder.DropIndex(
                name: "IX_VotingResults_QuestionId",
                table: "VotingResults");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "VotingResults");
        }
    }
}
