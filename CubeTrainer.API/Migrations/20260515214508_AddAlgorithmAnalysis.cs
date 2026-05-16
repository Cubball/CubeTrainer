using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CubeTrainer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgorithmAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Analysis",
                table: "Algorithms",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Analysis",
                table: "Algorithms");
        }
    }
}
