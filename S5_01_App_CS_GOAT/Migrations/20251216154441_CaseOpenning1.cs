using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S5_01_App_CS_GOAT.Migrations
{
    /// <inheritdoc />
    public partial class CaseOpenning1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.RenameColumn(
                name: "frn_fraction",
                table: "t_e_fairrandom_frn",
                newName: "frn_fraction2");

            _ = migrationBuilder.AddColumn<double>(
                name: "frn_fraction1",
                table: "t_e_fairrandom_frn",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "frn_fraction1",
                table: "t_e_fairrandom_frn");

            _ = migrationBuilder.RenameColumn(
                name: "frn_fraction2",
                table: "t_e_fairrandom_frn",
                newName: "frn_fraction");
        }
    }
}
