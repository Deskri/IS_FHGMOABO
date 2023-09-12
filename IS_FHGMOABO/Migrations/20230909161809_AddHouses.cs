using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS_FHGMOABO.Migrations
{
    /// <inheritdoc />
    public partial class AddHouses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Houses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InhabitedLocality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseCadastralNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlotCadastralNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HousesPassportedFloorArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlotPassportedFloorArea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ResidentialPremisesPassportedArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonResidentialPremisesPassportedArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Houses");
        }
    }
}
