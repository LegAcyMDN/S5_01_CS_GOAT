using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace S5_01_App_CS_GOAT.Migrations
{
    /// <inheritdoc />
    public partial class CreationCSGOATAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_e_bantype_bnt",
                columns: table => new
                {
                    bnt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bnt_bantypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_bantype_bnt", x => x.bnt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_case_cas",
                columns: table => new
                {
                    cas_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cas_casename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cas_caseimage = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cas_caseprice = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_case_cas", x => x.cas_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_itemtype_itt",
                columns: table => new
                {
                    itt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    itt_itemtypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    itt_parentid = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_itemtype_itt", x => x.itt_id);
                    table.ForeignKey(
                        name: "FK_t_e_itemtype_itt_t_e_itemtype_itt_itt_parentid",
                        column: x => x.itt_parentid,
                        principalTable: "t_e_itemtype_itt",
                        principalColumn: "itt_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_limittype_lmt",
                columns: table => new
                {
                    lmt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lmt_limittypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    lmt_duration = table.Column<int>(type: "integer", nullable: false),
                    lmt_durationname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_limittype_lmt", x => x.lmt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notificationtype_ntt",
                columns: table => new
                {
                    ntt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ntt_notificationtypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notificationtype_ntt", x => x.ntt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_paymentmethod_pmt",
                columns: table => new
                {
                    pmt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pmt_paymentmethodname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pmt_fromwallet = table.Column<bool>(type: "boolean", nullable: false),
                    pmt_towallet = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_paymentmethod_pmt", x => x.pmt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_rarity_rar",
                columns: table => new
                {
                    rar_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rar_rarityname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_rarity_rar", x => x.rar_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_tokentype_tkt",
                columns: table => new
                {
                    tkt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tkt_tokentypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_tokentype_tkt", x => x.tkt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_user_usr",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usr_login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    usr_displayname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usr_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    usr_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    usr_phoneverifiedon = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usr_emailverifiedon = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usr_twofaisphone = table.Column<bool>(type: "boolean", nullable: false),
                    usr_twofaisemail = table.Column<bool>(type: "boolean", nullable: false),
                    usr_isadmin = table.Column<bool>(type: "boolean", nullable: false),
                    usr_creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usr_lastlogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usr_saltpassword = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    usr_hashpassword = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    usr_steamid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    usr_seed = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    usr_nonce = table.Column<int>(type: "integer", nullable: false),
                    usr_wallet = table.Column<double>(type: "double precision", nullable: false),
                    usr_deletedon = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_user_usr", x => x.usr_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_item_itm",
                columns: table => new
                {
                    itm_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    itm_itemname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    itm_itemmodel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    itm_defindex = table.Column<int>(type: "integer", nullable: true),
                    itt_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_item_itm", x => x.itm_id);
                    table.ForeignKey(
                        name: "FK_t_e_item_itm_t_e_itemtype_itt_itt_id",
                        column: x => x.itt_id,
                        principalTable: "t_e_itemtype_itt",
                        principalColumn: "itt_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_ntf",
                columns: table => new
                {
                    ntf_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ntf_notificationsummary = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ntf_notificationcontent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ntf_notificationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ntt_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_notification_ntf", x => x.ntf_id);
                    table.ForeignKey(
                        name: "FK_notification_notificationtype",
                        column: x => x.ntt_id,
                        principalTable: "t_e_notificationtype_ntt",
                        principalColumn: "ntt_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_token_tkn",
                columns: table => new
                {
                    tkn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tkn_token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tkn_tokencreationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tkn_tokenexpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    tkt_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_token_tkn", x => x.tkn_id);
                    table.ForeignKey(
                        name: "FK_token_tokentype",
                        column: x => x.tkt_id,
                        principalTable: "t_e_tokentype_tkt",
                        principalColumn: "tkt_id");
                    table.ForeignKey(
                        name: "FK_token_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_ban_ban",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    bnt_id = table.Column<int>(type: "integer", nullable: false),
                    ban_bandate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ban_banduration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_ban_ban", x => new { x.usr_id, x.bnt_id });
                    table.ForeignKey(
                        name: "FK_ban_bantype",
                        column: x => x.bnt_id,
                        principalTable: "t_e_bantype_bnt",
                        principalColumn: "bnt_id");
                    table.ForeignKey(
                        name: "FK_ban_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_favorite_fav",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    cas_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_favorite_fav", x => new { x.usr_id, x.cas_id });
                    table.ForeignKey(
                        name: "FK_favorite_case",
                        column: x => x.cas_id,
                        principalTable: "t_e_case_cas",
                        principalColumn: "cas_id");
                    table.ForeignKey(
                        name: "FK_favorite_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_limit_lim",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    lmt_id = table.Column<int>(type: "integer", nullable: false),
                    lim_limitamount = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_limit_lim", x => new { x.usr_id, x.lmt_id });
                    table.ForeignKey(
                        name: "FK_limit_limittype",
                        column: x => x.lmt_id,
                        principalTable: "t_e_limittype_lmt",
                        principalColumn: "lmt_id");
                    table.ForeignKey(
                        name: "FK_limit_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_notificationsetting_nts",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    ntt_id = table.Column<int>(type: "integer", nullable: false),
                    nts_onsite = table.Column<bool>(type: "boolean", nullable: false),
                    nts_byemail = table.Column<bool>(type: "boolean", nullable: false),
                    nts_byphone = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_notificationsetting_nts", x => new { x.usr_id, x.ntt_id });
                    table.ForeignKey(
                        name: "FK_notificationsetting_notificationtype",
                        column: x => x.ntt_id,
                        principalTable: "t_e_notificationtype_ntt",
                        principalColumn: "ntt_id");
                    table.ForeignKey(
                        name: "FK_t_j_notificationsetting_nts_t_e_user_usr_usr_id",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_skin_skn",
                columns: table => new
                {
                    skn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    skn_skinname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    skn_paintindex = table.Column<int>(type: "integer", nullable: false),
                    itm_id = table.Column<int>(type: "integer", nullable: false),
                    rar_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_skin_skn", x => x.skn_id);
                    table.ForeignKey(
                        name: "FK_skin_item",
                        column: x => x.itm_id,
                        principalTable: "t_e_item_itm",
                        principalColumn: "itm_id");
                    table.ForeignKey(
                        name: "FK_skin_rarity",
                        column: x => x.rar_id,
                        principalTable: "t_e_rarity_rar",
                        principalColumn: "rar_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_globalnotification_gnf",
                columns: table => new
                {
                    ntf_id = table.Column<int>(type: "integer", nullable: false),
                    gnf_includevisitors = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_globalnotification_gnf", x => x.ntf_id);
                    table.ForeignKey(
                        name: "FK_t_e_globalnotification_gnf_t_e_notification_ntf_ntf_id",
                        column: x => x.ntf_id,
                        principalTable: "t_e_notification_ntf",
                        principalColumn: "ntf_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_transaction_txn",
                columns: table => new
                {
                    txn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    txn_transactiondate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    txn_walletvalue = table.Column<double>(type: "double precision", nullable: false),
                    txn_cancelledon = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    ntf_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_transaction_txn", x => x.txn_id);
                    table.ForeignKey(
                        name: "FK_t_e_transaction_txn_t_e_notification_ntf_ntf_id",
                        column: x => x.ntf_id,
                        principalTable: "t_e_notification_ntf",
                        principalColumn: "ntf_id");
                    table.ForeignKey(
                        name: "FK_transaction_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_usernotification_unf",
                columns: table => new
                {
                    ntf_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_usernotification_unf", x => x.ntf_id);
                    table.ForeignKey(
                        name: "FK_t_e_usernotification_unf_t_e_notification_ntf_ntf_id",
                        column: x => x.ntf_id,
                        principalTable: "t_e_notification_ntf",
                        principalColumn: "ntf_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usernotification_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_wear_wer",
                columns: table => new
                {
                    wer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wer_wearid = table.Column<int>(type: "integer", nullable: false),
                    wer_floatlow = table.Column<float>(type: "real", nullable: false),
                    wer_floathigh = table.Column<float>(type: "real", nullable: false),
                    wer_uuid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    skn_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_wear_wer", x => x.wer_id);
                    table.ForeignKey(
                        name: "FK_wear_skin",
                        column: x => x.skn_id,
                        principalTable: "t_e_skin_skn",
                        principalColumn: "skn_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_casecontent_cct",
                columns: table => new
                {
                    cas_id = table.Column<int>(type: "integer", nullable: false),
                    skn_id = table.Column<int>(type: "integer", nullable: false),
                    cct_weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_casecontent_cct", x => new { x.cas_id, x.skn_id });
                    table.ForeignKey(
                        name: "FK_casecontent_case",
                        column: x => x.cas_id,
                        principalTable: "t_e_case_cas",
                        principalColumn: "cas_id");
                    table.ForeignKey(
                        name: "FK_casecontent_skin",
                        column: x => x.skn_id,
                        principalTable: "t_e_skin_skn",
                        principalColumn: "skn_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_moneytransaction_mtr",
                columns: table => new
                {
                    txn_id = table.Column<int>(type: "integer", nullable: false),
                    pmt_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_moneytransaction_mtr", x => x.txn_id);
                    table.ForeignKey(
                        name: "FK_moneytransaction_paymentmethod",
                        column: x => x.pmt_id,
                        principalTable: "t_e_paymentmethod_pmt",
                        principalColumn: "pmt_id");
                    table.ForeignKey(
                        name: "FK_t_e_moneytransaction_mtr_t_e_transaction_txn_txn_id",
                        column: x => x.txn_id,
                        principalTable: "t_e_transaction_txn",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_pricehistory_prc",
                columns: table => new
                {
                    prc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prc_pricedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    prc_pricevalue = table.Column<double>(type: "double precision", nullable: false),
                    prc_guessdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    wer_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_pricehistory_prc", x => x.prc_id);
                    table.ForeignKey(
                        name: "FK_pricehistory_wear",
                        column: x => x.wer_id,
                        principalTable: "t_e_wear_wer",
                        principalColumn: "wer_id");
                });

            migrationBuilder.CreateTable(
                name: "t_j_inventoryitem_inv",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    wer_id = table.Column<int>(type: "integer", nullable: false),
                    inv_float = table.Column<float>(type: "real", nullable: false),
                    inv_isfavorite = table.Column<bool>(type: "boolean", nullable: false),
                    inv_acquiredon = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inv_removedon = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_inventoryitem_inv", x => new { x.usr_id, x.wer_id });
                    table.ForeignKey(
                        name: "FK_inventoryitem_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                    table.ForeignKey(
                        name: "FK_inventoryitem_wear",
                        column: x => x.wer_id,
                        principalTable: "t_e_wear_wer",
                        principalColumn: "wer_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_itemtransaction_itr",
                columns: table => new
                {
                    txn_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id_item = table.Column<int>(type: "integer", nullable: true),
                    wer_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_itemtransaction_itr", x => x.txn_id);
                    table.ForeignKey(
                        name: "FK_itemtransaction_inventoryitem",
                        columns: x => new { x.usr_id_item, x.wer_id },
                        principalTable: "t_j_inventoryitem_inv",
                        principalColumns: new[] { "usr_id", "wer_id" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_t_e_itemtransaction_itr_t_e_transaction_txn_txn_id",
                        column: x => x.txn_id,
                        principalTable: "t_e_transaction_txn",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_upgraderesult_upg",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    wer_id = table.Column<int>(type: "integer", nullable: false),
                    upg_floatstart = table.Column<float>(type: "real", nullable: false),
                    upg_floatend = table.Column<float>(type: "real", nullable: false),
                    upg_probintact = table.Column<double>(type: "double precision", nullable: false),
                    upg_probdegrade = table.Column<double>(type: "double precision", nullable: false),
                    upg_propdestroy = table.Column<double>(type: "double precision", nullable: false),
                    upg_degradefuction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_j_upgraderesult_upg", x => new { x.usr_id, x.wer_id });
                    table.ForeignKey(
                        name: "FK_upgraderesult_inventoryitem",
                        columns: x => new { x.usr_id, x.wer_id },
                        principalTable: "t_j_inventoryitem_inv",
                        principalColumns: new[] { "usr_id", "wer_id" });
                });

            migrationBuilder.CreateTable(
                name: "t_e_randomtransaction_rtr",
                columns: table => new
                {
                    txn_id = table.Column<int>(type: "integer", nullable: false),
                    rtr_userseed = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    usr_id_upgrade = table.Column<int>(type: "integer", nullable: true),
                    wer_id_upgrade = table.Column<int>(type: "integer", nullable: true),
                    cas_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_randomtransaction_rtr", x => x.txn_id);
                    table.ForeignKey(
                        name: "FK_randomtransaction_case",
                        column: x => x.cas_id,
                        principalTable: "t_e_case_cas",
                        principalColumn: "cas_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_randomtransaction_upgraderesult",
                        columns: x => new { x.usr_id_upgrade, x.wer_id_upgrade },
                        principalTable: "t_j_upgraderesult_upg",
                        principalColumns: new[] { "usr_id", "wer_id" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_t_e_randomtransaction_rtr_t_e_transaction_txn_txn_id",
                        column: x => x.txn_id,
                        principalTable: "t_e_transaction_txn",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_fairrandom_frn",
                columns: table => new
                {
                    frn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    frn_serverseed = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    frn_serverhash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    frn_usernonce = table.Column<int>(type: "integer", nullable: false),
                    frn_combinedhash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    frn_fraction = table.Column<double>(type: "double precision", nullable: false),
                    rtr_id = table.Column<int>(type: "integer", nullable: true),
                    usr_id_upgrade = table.Column<int>(type: "integer", nullable: true),
                    wer_id_upgrade = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_fairrandom_frn", x => x.frn_id);
                    table.ForeignKey(
                        name: "FK_fairrandom_randomtransaction",
                        column: x => x.rtr_id,
                        principalTable: "t_e_randomtransaction_rtr",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_fairrandom_upgraderesult",
                        columns: x => new { x.usr_id_upgrade, x.wer_id_upgrade },
                        principalTable: "t_j_upgraderesult_upg",
                        principalColumns: new[] { "usr_id", "wer_id" },
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_fairrandom_frn_rtr_id",
                table: "t_e_fairrandom_frn",
                column: "rtr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_fairrandom_frn_usr_id_upgrade_wer_id_upgrade",
                table: "t_e_fairrandom_frn",
                columns: new[] { "usr_id_upgrade", "wer_id_upgrade" });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_item_itm_itt_id",
                table: "t_e_item_itm",
                column: "itt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_itemtransaction_itr_usr_id_item_wer_id",
                table: "t_e_itemtransaction_itr",
                columns: new[] { "usr_id_item", "wer_id" });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_itemtype_itt_itt_parentid",
                table: "t_e_itemtype_itt",
                column: "itt_parentid");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_moneytransaction_mtr_pmt_id",
                table: "t_e_moneytransaction_mtr",
                column: "pmt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_ntf_ntt_id",
                table: "t_e_notification_ntf",
                column: "ntt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prc_wer_id",
                table: "t_e_pricehistory_prc",
                column: "wer_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_randomtransaction_rtr_cas_id",
                table: "t_e_randomtransaction_rtr",
                column: "cas_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_randomtransaction_rtr_usr_id_upgrade_wer_id_upgrade",
                table: "t_e_randomtransaction_rtr",
                columns: new[] { "usr_id_upgrade", "wer_id_upgrade" });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_skin_skn_itm_id",
                table: "t_e_skin_skn",
                column: "itm_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_skin_skn_rar_id",
                table: "t_e_skin_skn",
                column: "rar_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_token_tkn_tkt_id",
                table: "t_e_token_tkn",
                column: "tkt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_token_tkn_usr_id",
                table: "t_e_token_tkn",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_transaction_txn_ntf_id",
                table: "t_e_transaction_txn",
                column: "ntf_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_transaction_txn_usr_id",
                table: "t_e_transaction_txn",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_usernotification_unf_usr_id",
                table: "t_e_usernotification_unf",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_wear_wer_skn_id",
                table: "t_e_wear_wer",
                column: "skn_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_ban_ban_bnt_id",
                table: "t_j_ban_ban",
                column: "bnt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_casecontent_cct_skn_id",
                table: "t_j_casecontent_cct",
                column: "skn_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_favorite_fav_cas_id",
                table: "t_j_favorite_fav",
                column: "cas_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_inventoryitem_inv_wer_id",
                table: "t_j_inventoryitem_inv",
                column: "wer_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_limit_lim_lmt_id",
                table: "t_j_limit_lim",
                column: "lmt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_notificationsetting_nts_ntt_id",
                table: "t_j_notificationsetting_nts",
                column: "ntt_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_fairrandom_frn");

            migrationBuilder.DropTable(
                name: "t_e_globalnotification_gnf");

            migrationBuilder.DropTable(
                name: "t_e_itemtransaction_itr");

            migrationBuilder.DropTable(
                name: "t_e_moneytransaction_mtr");

            migrationBuilder.DropTable(
                name: "t_e_pricehistory_prc");

            migrationBuilder.DropTable(
                name: "t_e_token_tkn");

            migrationBuilder.DropTable(
                name: "t_e_usernotification_unf");

            migrationBuilder.DropTable(
                name: "t_j_ban_ban");

            migrationBuilder.DropTable(
                name: "t_j_casecontent_cct");

            migrationBuilder.DropTable(
                name: "t_j_favorite_fav");

            migrationBuilder.DropTable(
                name: "t_j_limit_lim");

            migrationBuilder.DropTable(
                name: "t_j_notificationsetting_nts");

            migrationBuilder.DropTable(
                name: "t_e_randomtransaction_rtr");

            migrationBuilder.DropTable(
                name: "t_e_paymentmethod_pmt");

            migrationBuilder.DropTable(
                name: "t_e_tokentype_tkt");

            migrationBuilder.DropTable(
                name: "t_e_bantype_bnt");

            migrationBuilder.DropTable(
                name: "t_e_limittype_lmt");

            migrationBuilder.DropTable(
                name: "t_e_case_cas");

            migrationBuilder.DropTable(
                name: "t_j_upgraderesult_upg");

            migrationBuilder.DropTable(
                name: "t_e_transaction_txn");

            migrationBuilder.DropTable(
                name: "t_j_inventoryitem_inv");

            migrationBuilder.DropTable(
                name: "t_e_notification_ntf");

            migrationBuilder.DropTable(
                name: "t_e_user_usr");

            migrationBuilder.DropTable(
                name: "t_e_wear_wer");

            migrationBuilder.DropTable(
                name: "t_e_notificationtype_ntt");

            migrationBuilder.DropTable(
                name: "t_e_skin_skn");

            migrationBuilder.DropTable(
                name: "t_e_item_itm");

            migrationBuilder.DropTable(
                name: "t_e_rarity_rar");

            migrationBuilder.DropTable(
                name: "t_e_itemtype_itt");
        }
    }
}
