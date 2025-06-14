using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projetoTeste.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Housing_median_age = table.Column<float>(type: "real", nullable: false),
                    Total_rooms = table.Column<float>(type: "real", nullable: false),
                    Total_bedrooms = table.Column<float>(type: "real", nullable: false),
                    Population = table.Column<float>(type: "real", nullable: false),
                    Households = table.Column<float>(type: "real", nullable: false),
                    Median_income = table.Column<float>(type: "real", nullable: false),
                    Median_house_value = table.Column<float>(type: "real", nullable: false),
                    Ocean_proximity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelInputs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelOutputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Score = table.Column<float>(type: "real", nullable: false),
                    ModelInputId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelOutputs_ModelInputs_ModelInputId",
                        column: x => x.ModelInputId,
                        principalTable: "ModelInputs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelOutputs_ModelInputId",
                table: "ModelOutputs",
                column: "ModelInputId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelOutputs");

            migrationBuilder.DropTable(
                name: "ModelInputs");
        }
    }
}
