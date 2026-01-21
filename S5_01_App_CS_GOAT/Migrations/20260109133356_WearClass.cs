using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S5_01_App_CS_GOAT.Migrations
{
    /// <inheritdoc />
    public partial class WearClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_wear_wer_skn_id",
                table: "t_e_wear_wer");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_pricehistory_prh_skn_id",
                table: "t_e_pricehistory_prh");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_t_j_ban_ban",
                table: "t_j_ban_ban");

            _ = migrationBuilder.RenameTable(
                name: "t_j_ban_ban",
                newName: "t_e_ban_ban");

            _ = migrationBuilder.RenameIndex(
                name: "IX_t_j_ban_ban_bnt_id",
                table: "t_e_ban_ban",
                newName: "IX_t_e_ban_ban_bnt_id");

            _ = migrationBuilder.RenameIndex(
                name: "IX_t_j_ban_ban_ban_bandate",
                table: "t_e_ban_ban",
                newName: "IX_t_e_ban_ban_ban_bandate");

            _ = migrationBuilder.AlterColumn<int>(
                name: "prc_discountpercentage",
                table: "t_e_promocode_prc",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            _ = migrationBuilder.AlterColumn<double>(
                name: "prc_discountamount",
                table: "t_e_promocode_prc",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            _ = migrationBuilder.AddColumn<int>(
                name: "WearId",
                table: "t_e_pricehistory_prh",
                type: "integer",
                nullable: true);

            _ = migrationBuilder.AlterColumn<int>(
                name: "inv_id",
                table: "t_e_itemtransaction_itr",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_t_e_ban_ban",
                table: "t_e_ban_ban",
                columns: new[] { "usr_id", "bnt_id" });

            _ = migrationBuilder.CreateTable(
                name: "t_j_wearclass_wrc",
                columns: table => new
                {
                    skn_id = table.Column<int>(type: "integer", nullable: false),
                    wrt_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_j_wearclass_wrc", x => new { x.skn_id, x.wrt_id });
                    _ = table.ForeignKey(
                        name: "FK_wearclass_skin",
                        column: x => x.skn_id,
                        principalTable: "t_e_skin_skn",
                        principalColumn: "skn_id");
                    _ = table.ForeignKey(
                        name: "FK_wearclass_weartype",
                        column: x => x.wrt_id,
                        principalTable: "t_e_weartype_wrt",
                        principalColumn: "wrt_id");
                });

            // Populate wearclass table with cross join of all skins and wear types
            _ = migrationBuilder.Sql(@"
                INSERT INTO t_j_wearclass_wrc (skn_id, wrt_id)
                SELECT s.skn_id, w.wrt_id
                FROM t_e_skin_skn s
                CROSS JOIN t_e_weartype_wrt w
            ");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_inventoryitem_inv_inv_float",
                table: "t_j_inventoryitem_inv",
                column: "inv_float");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_wear_wer_skn_id_wrt_id",
                table: "t_e_wear_wer",
                columns: new[] { "skn_id", "wrt_id" });

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_wear_wer_wer_wearfloat",
                table: "t_e_wear_wer",
                column: "wer_wearfloat");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_usernotification_unf_unf_isread",
                table: "t_e_usernotification_unf",
                column: "unf_isread");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_skn_id_wrt_id",
                table: "t_e_pricehistory_prh",
                columns: new[] { "skn_id", "wrt_id" });

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_WearId",
                table: "t_e_pricehistory_prh",
                column: "WearId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_fairrandom_frn_frn_usernonce",
                table: "t_e_fairrandom_frn",
                column: "frn_usernonce");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_wearclass_wrc_wrt_id",
                table: "t_j_wearclass_wrc",
                column: "wrt_id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_pricehistory_wearclass",
                table: "t_e_pricehistory_prh",
                columns: new[] { "skn_id", "wrt_id" },
                principalTable: "t_j_wearclass_wrc",
                principalColumns: new[] { "skn_id", "wrt_id" });

            _ = migrationBuilder.AddForeignKey(
                name: "FK_t_e_pricehistory_prh_t_e_wear_wer_WearId",
                table: "t_e_pricehistory_prh",
                column: "WearId",
                principalTable: "t_e_wear_wer",
                principalColumn: "wer_id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_wear_wearclass",
                table: "t_e_wear_wer",
                columns: new[] { "skn_id", "wrt_id" },
                principalTable: "t_j_wearclass_wrc",
                principalColumns: new[] { "skn_id", "wrt_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_pricehistory_wearclass",
                table: "t_e_pricehistory_prh");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_t_e_pricehistory_prh_t_e_wear_wer_WearId",
                table: "t_e_pricehistory_prh");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_wear_wearclass",
                table: "t_e_wear_wer");

            _ = migrationBuilder.DropTable(
                name: "t_j_wearclass_wrc");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_j_inventoryitem_inv_inv_float",
                table: "t_j_inventoryitem_inv");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_wear_wer_skn_id_wrt_id",
                table: "t_e_wear_wer");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_wear_wer_wer_wearfloat",
                table: "t_e_wear_wer");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_usernotification_unf_unf_isread",
                table: "t_e_usernotification_unf");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_pricehistory_prh_skn_id_wrt_id",
                table: "t_e_pricehistory_prh");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_pricehistory_prh_WearId",
                table: "t_e_pricehistory_prh");

            _ = migrationBuilder.DropIndex(
                name: "IX_t_e_fairrandom_frn_frn_usernonce",
                table: "t_e_fairrandom_frn");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_t_e_ban_ban",
                table: "t_e_ban_ban");

            _ = migrationBuilder.DropColumn(
                name: "WearId",
                table: "t_e_pricehistory_prh");

            _ = migrationBuilder.RenameTable(
                name: "t_e_ban_ban",
                newName: "t_j_ban_ban");

            _ = migrationBuilder.RenameIndex(
                name: "IX_t_e_ban_ban_bnt_id",
                table: "t_j_ban_ban",
                newName: "IX_t_j_ban_ban_bnt_id");

            _ = migrationBuilder.RenameIndex(
                name: "IX_t_e_ban_ban_ban_bandate",
                table: "t_j_ban_ban",
                newName: "IX_t_j_ban_ban_ban_bandate");

            _ = migrationBuilder.AlterColumn<int>(
                name: "prc_discountpercentage",
                table: "t_e_promocode_prc",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<double>(
                name: "prc_discountamount",
                table: "t_e_promocode_prc",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<int>(
                name: "inv_id",
                table: "t_e_itemtransaction_itr",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_t_j_ban_ban",
                table: "t_j_ban_ban",
                columns: new[] { "usr_id", "bnt_id" });

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_wear_wer_skn_id",
                table: "t_e_wear_wer",
                column: "skn_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_skn_id",
                table: "t_e_pricehistory_prh",
                column: "skn_id");
        }
    }
}
