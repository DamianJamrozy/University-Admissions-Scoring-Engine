using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Admissions_Scoring_Engine.Migrations
{
    /// <inheritdoc />
    public partial class AddRequirementsAndMinimums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CzyWymagany",
                table: "AlgorytmLicz",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimalnePunkty",
                table: "AlgorytmLicz",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimalnePunkty",
                table: "AlgorytmGrupa",
                type: "decimal(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CzyWymagany",
                table: "AlgorytmLicz");

            migrationBuilder.DropColumn(
                name: "MinimalnePunkty",
                table: "AlgorytmLicz");

            migrationBuilder.DropColumn(
                name: "MinimalnePunkty",
                table: "AlgorytmGrupa");
        }
    }
}
