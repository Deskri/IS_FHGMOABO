using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class Creatingpropeties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LegalPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalPersons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NaturalPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patronymic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturalPersons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    LeagalPersonId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Share = table.Column<decimal>(type: "decimal(10,9)", nullable: false),
                    DateOfTaking = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeOfStateRegistration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ByWhomIssued = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_LegalPersons_LeagalPersonId",
                        column: x => x.LeagalPersonId,
                        principalTable: "LegalPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Properties_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NaturalPersonProperty",
                columns: table => new
                {
                    NaturalPersonsId = table.Column<int>(type: "int", nullable: false),
                    PropertiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturalPersonProperty", x => new { x.NaturalPersonsId, x.PropertiesId });
                    table.ForeignKey(
                        name: "FK_NaturalPersonProperty_NaturalPersons_NaturalPersonsId",
                        column: x => x.NaturalPersonsId,
                        principalTable: "NaturalPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NaturalPersonProperty_Properties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NaturalPersonProperty_PropertiesId",
                table: "NaturalPersonProperty",
                column: "PropertiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_LeagalPersonId",
                table: "Properties",
                column: "LeagalPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_RoomId",
                table: "Properties",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NaturalPersonProperty");

            migrationBuilder.DropTable(
                name: "NaturalPersons");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "LegalPersons");
        }
    }
}
