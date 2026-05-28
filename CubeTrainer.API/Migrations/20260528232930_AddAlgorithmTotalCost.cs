using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CubeTrainer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgorithmTotalCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalCost",
                table: "Algorithms",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Algorithms");
        }
    }
}
