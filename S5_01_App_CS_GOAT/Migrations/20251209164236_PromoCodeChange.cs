using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S5_01_App_CS_GOAT.Migrations
{
    /// <inheritdoc />
    public partial class PromoCodeChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_e_pricehistory_prh_t_e_wear_wer_WearId",
                table: "t_e_pricehistory_prh");

            migrationBuilder.DropIndex(
                name: "IX_t_e_pricehistory_prh_WearId",
                table: "t_e_pricehistory_prh");

            migrationBuilder.DropColumn(
                name: "WearId",
                table: "t_e_pricehistory_prh");

            migrationBuilder.AlterColumn<DateTime>(
                name: "prc_expirydate",
                table: "t_e_promocode_prc",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "prc_refreshdelay",
                table: "t_e_promocode_prc",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prc_remaininguses",
                table: "t_e_promocode_prc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "prc_validitystart",
                table: "t_e_promocode_prc",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "prc_refreshdelay",
                table: "t_e_promocode_prc");

            migrationBuilder.DropColumn(
                name: "prc_remaininguses",
                table: "t_e_promocode_prc");

            migrationBuilder.DropColumn(
                name: "prc_validitystart",
                table: "t_e_promocode_prc");

            migrationBuilder.AlterColumn<DateTime>(
                name: "prc_expirydate",
                table: "t_e_promocode_prc",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WearId",
                table: "t_e_pricehistory_prh",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_WearId",
                table: "t_e_pricehistory_prh",
                column: "WearId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_e_pricehistory_prh_t_e_wear_wer_WearId",
                table: "t_e_pricehistory_prh",
                column: "WearId",
                principalTable: "t_e_wear_wer",
                principalColumn: "wer_id");
        }
    }
}
