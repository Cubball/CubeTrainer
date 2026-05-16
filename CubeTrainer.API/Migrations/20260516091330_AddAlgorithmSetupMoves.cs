using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CubeTrainer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgorithmSetupMoves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetupMoves",
                table: "Algorithms",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetupMoves",
                table: "Algorithms");
        }
    }
}