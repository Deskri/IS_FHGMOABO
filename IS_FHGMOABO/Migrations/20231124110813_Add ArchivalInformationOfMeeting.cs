using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class AddArchivalInformationOfMeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivalInformationOfMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    TotalAreaHouse = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ResidentialAreaInOwnership = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ResidentialAreaInNonOwnership = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonresidentialArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OwnersParticipated = table.Column<int>(type: "int", nullable: false),
                    ParticipatingArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivalInformationOfMeetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivalInformationOfMeetings_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivalInformationOfMeetings_MeetingId",
                table: "ArchivalInformationOfMeetings",
                column: "MeetingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivalInformationOfMeetings");
        }
    }
}
