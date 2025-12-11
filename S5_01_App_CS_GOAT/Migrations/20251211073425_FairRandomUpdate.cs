using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S5_01_App_CS_GOAT.Migrations
{
    /// <inheritdoc />
    public partial class FairRandomUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_e_randomtransaction_rtr_t_e_transaction_txn_txn_id",
                table: "t_e_randomtransaction_rtr");

            migrationBuilder.DropColumn(
                name: "rtr_userseed",
                table: "t_e_randomtransaction_rtr");

            migrationBuilder.AlterColumn<int>(
                name: "frn_usernonce",
                table: "t_e_fairrandom_frn",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "frn_fraction",
                table: "t_e_fairrandom_frn",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "frn_combinedhash",
                table: "t_e_fairrandom_frn",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "usr_id",
                table: "t_e_fairrandom_frn",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "usr_seed",
                table: "t_e_fairrandom_frn",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_fairrandom_frn_usr_id",
                table: "t_e_fairrandom_frn",
                column: "usr_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_fairrandom_user",
                table: "t_e_fairrandom_frn",
                column: "usr_id",
                principalTable: "t_e_user_usr",
                principalColumn: "usr_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_t_e_randomtransaction_rtr_t_e_itemtransaction_itr_txn_id",
                table: "t_e_randomtransaction_rtr",
                column: "txn_id",
                principalTable: "t_e_itemtransaction_itr",
                principalColumn: "txn_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fairrandom_user",
                table: "t_e_fairrandom_frn");

            migrationBuilder.DropForeignKey(
                name: "FK_t_e_randomtransaction_rtr_t_e_itemtransaction_itr_txn_id",
                table: "t_e_randomtransaction_rtr");

            migrationBuilder.DropIndex(
                name: "IX_t_e_fairrandom_frn_usr_id",
                table: "t_e_fairrandom_frn");

            migrationBuilder.DropColumn(
                name: "usr_id",
                table: "t_e_fairrandom_frn");

            migrationBuilder.DropColumn(
                name: "usr_seed",
                table: "t_e_fairrandom_frn");

            migrationBuilder.AddColumn<string>(
                name: "rtr_userseed",
                table: "t_e_randomtransaction_rtr",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "frn_usernonce",
                table: "t_e_fairrandom_frn",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "frn_fraction",
                table: "t_e_fairrandom_frn",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "frn_combinedhash",
                table: "t_e_fairrandom_frn",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_t_e_randomtransaction_rtr_t_e_transaction_txn_txn_id",
                table: "t_e_randomtransaction_rtr",
                column: "txn_id",
                principalTable: "t_e_transaction_txn",
                principalColumn: "txn_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
