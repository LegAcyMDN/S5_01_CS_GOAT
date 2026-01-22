using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable


namespace S5_01_App_CS_GOAT.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration_09122025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "t_e_bantype_bnt",
                columns: table => new
                {
                    bnt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bnt_bantypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    bnt_bantypedescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    bnt_parentid = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_bantype_bnt", x => x.bnt_id);
                    _ = table.ForeignKey(
                        name: "FK_t_e_bantype_bnt_t_e_bantype_bnt_bnt_parentid",
                        column: x => x.bnt_parentid,
                        principalTable: "t_e_bantype_bnt",
                        principalColumn: "bnt_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_case_cas",
                columns: table => new
                {
                    cas_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cas_casename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cas_caseimage = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cas_caseprice = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_case_cas", x => x.cas_id));

            _ = migrationBuilder.CreateTable(
                name: "t_e_fairrandom_frn",
                columns: table => new
                {
                    frn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    frn_serverseed = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    frn_serverhash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    frn_usernonce = table.Column<int>(type: "integer", nullable: false),
                    frn_combinedhash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    frn_fraction = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_fairrandom_frn", x => x.frn_id));

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_t_e_itemtype_itt", x => x.itt_id);
                    _ = table.ForeignKey(
                        name: "FK_t_e_itemtype_itt_t_e_itemtype_itt_itt_parentid",
                        column: x => x.itt_parentid,
                        principalTable: "t_e_itemtype_itt",
                        principalColumn: "itt_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_limittype_lmt",
                columns: table => new
                {
                    lmt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lmt_limittypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    lmt_duration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_limittype_lmt", x => x.lmt_id));

            _ = migrationBuilder.CreateTable(
                name: "t_e_notificationtype_ntt",
                columns: table => new
                {
                    ntt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ntt_notificationtypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_notificationtype_ntt", x => x.ntt_id));

            _ = migrationBuilder.CreateTable(
                name: "t_e_paymentmethod_pmt",
                columns: table => new
                {
                    pmt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pmt_paymentmethodname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pmt_fromwallet = table.Column<bool>(type: "boolean", nullable: false),
                    pmt_towallet = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_paymentmethod_pmt", x => x.pmt_id));

            _ = migrationBuilder.CreateTable(
                name: "t_e_rarity_rar",
                columns: table => new
                {
                    rar_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rar_rarityname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rar_raritycolor = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_rarity_rar", x => x.rar_id));

            _ = migrationBuilder.CreateTable(
                name: "t_e_tokentype_tkt",
                columns: table => new
                {
                    tkt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tkt_tokentypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_tokentype_tkt", x => x.tkt_id));

            _ = migrationBuilder.CreateTable(
                name: "t_e_user_usr",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usr_login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    usr_displayname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    usr_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                constraints: table => table.PrimaryKey("PK_t_e_user_usr", x => x.usr_id));

            _ = migrationBuilder.CreateTable(
                name: "t_e_weartype_wrt",
                columns: table => new
                {
                    wrt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wrt_weartypename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_t_e_weartype_wrt", x => x.wrt_id));

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_t_e_item_itm", x => x.itm_id);
                    _ = table.ForeignKey(
                        name: "FK_t_e_item_itm_t_e_itemtype_itt_itt_id",
                        column: x => x.itt_id,
                        principalTable: "t_e_itemtype_itt",
                        principalColumn: "itt_id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_t_e_notification_ntf", x => x.ntf_id);
                    _ = table.ForeignKey(
                        name: "FK_notification_notificationtype",
                        column: x => x.ntt_id,
                        principalTable: "t_e_notificationtype_ntt",
                        principalColumn: "ntt_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_promocode_prc",
                columns: table => new
                {
                    prc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prc_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prc_discountpercentage = table.Column<int>(type: "integer", nullable: false),
                    prc_discountamount = table.Column<double>(type: "double precision", nullable: false),
                    prc_expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cas_id = table.Column<int>(type: "integer", nullable: true),
                    usr_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_promocode_prc", x => x.prc_id);
                    _ = table.ForeignKey(
                        name: "FK_promocode_case",
                        column: x => x.cas_id,
                        principalTable: "t_e_case_cas",
                        principalColumn: "cas_id");
                    _ = table.ForeignKey(
                        name: "FK_promocode_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_t_e_token_tkn", x => x.tkn_id);
                    _ = table.ForeignKey(
                        name: "FK_token_tokentype",
                        column: x => x.tkt_id,
                        principalTable: "t_e_tokentype_tkt",
                        principalColumn: "tkt_id");
                    _ = table.ForeignKey(
                        name: "FK_token_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_j_ban_ban",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    bnt_id = table.Column<int>(type: "integer", nullable: false),
                    ban_id = table.Column<int>(type: "integer", nullable: false),
                    ban_reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ban_bandate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ban_banduration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_j_ban_ban", x => new { x.usr_id, x.bnt_id });
                    _ = table.ForeignKey(
                        name: "FK_ban_bantype",
                        column: x => x.bnt_id,
                        principalTable: "t_e_bantype_bnt",
                        principalColumn: "bnt_id");
                    _ = table.ForeignKey(
                        name: "FK_ban_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_j_favorite_fav",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    cas_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_j_favorite_fav", x => new { x.usr_id, x.cas_id });
                    _ = table.ForeignKey(
                        name: "FK_favorite_case",
                        column: x => x.cas_id,
                        principalTable: "t_e_case_cas",
                        principalColumn: "cas_id");
                    _ = table.ForeignKey(
                        name: "FK_favorite_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_j_limit_lim",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    lmt_id = table.Column<int>(type: "integer", nullable: false),
                    lim_limitamount = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_j_limit_lim", x => new { x.usr_id, x.lmt_id });
                    _ = table.ForeignKey(
                        name: "FK_limit_limittype",
                        column: x => x.lmt_id,
                        principalTable: "t_e_limittype_lmt",
                        principalColumn: "lmt_id");
                    _ = table.ForeignKey(
                        name: "FK_limit_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_t_j_notificationsetting_nts", x => new { x.usr_id, x.ntt_id });
                    _ = table.ForeignKey(
                        name: "FK_notificationsetting_notificationtype",
                        column: x => x.ntt_id,
                        principalTable: "t_e_notificationtype_ntt",
                        principalColumn: "ntt_id");
                    _ = table.ForeignKey(
                        name: "FK_t_j_notificationsetting_nts_t_e_user_usr_usr_id",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_skin_skn",
                columns: table => new
                {
                    skn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    skn_skinname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    skn_paintindex = table.Column<int>(type: "integer", nullable: false),
                    skn_uvtype = table.Column<int>(type: "integer", nullable: false),
                    itm_id = table.Column<int>(type: "integer", nullable: false),
                    rar_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_skin_skn", x => x.skn_id);
                    _ = table.ForeignKey(
                        name: "FK_skin_item",
                        column: x => x.itm_id,
                        principalTable: "t_e_item_itm",
                        principalColumn: "itm_id");
                    _ = table.ForeignKey(
                        name: "FK_skin_rarity",
                        column: x => x.rar_id,
                        principalTable: "t_e_rarity_rar",
                        principalColumn: "rar_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_globalnotification_gnf",
                columns: table => new
                {
                    ntf_id = table.Column<int>(type: "integer", nullable: false),
                    gnf_includevisitors = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_globalnotification_gnf", x => x.ntf_id);
                    _ = table.ForeignKey(
                        name: "FK_t_e_globalnotification_gnf_t_e_notification_ntf_ntf_id",
                        column: x => x.ntf_id,
                        principalTable: "t_e_notification_ntf",
                        principalColumn: "ntf_id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("PK_t_e_transaction_txn", x => x.txn_id);
                    _ = table.ForeignKey(
                        name: "FK_t_e_transaction_txn_t_e_notification_ntf_ntf_id",
                        column: x => x.ntf_id,
                        principalTable: "t_e_notification_ntf",
                        principalColumn: "ntf_id");
                    _ = table.ForeignKey(
                        name: "FK_transaction_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_usernotification_unf",
                columns: table => new
                {
                    ntf_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    unf_isread = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_usernotification_unf", x => x.ntf_id);
                    _ = table.ForeignKey(
                        name: "FK_t_e_usernotification_unf_t_e_notification_ntf_ntf_id",
                        column: x => x.ntf_id,
                        principalTable: "t_e_notification_ntf",
                        principalColumn: "ntf_id",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_usernotification_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_wear_wer",
                columns: table => new
                {
                    wer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wrt_id = table.Column<int>(type: "integer", nullable: false),
                    wer_wearfloat = table.Column<float>(type: "real", nullable: false),
                    wer_uuid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    skn_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_wear_wer", x => x.wer_id);
                    _ = table.ForeignKey(
                        name: "FK_wear_skin",
                        column: x => x.skn_id,
                        principalTable: "t_e_skin_skn",
                        principalColumn: "skn_id");
                    _ = table.ForeignKey(
                        name: "FK_wear_weartype",
                        column: x => x.wrt_id,
                        principalTable: "t_e_weartype_wrt",
                        principalColumn: "wrt_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_j_casecontent_cct",
                columns: table => new
                {
                    cas_id = table.Column<int>(type: "integer", nullable: false),
                    skn_id = table.Column<int>(type: "integer", nullable: false),
                    cct_weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_j_casecontent_cct", x => new { x.cas_id, x.skn_id });
                    _ = table.ForeignKey(
                        name: "FK_casecontent_case",
                        column: x => x.cas_id,
                        principalTable: "t_e_case_cas",
                        principalColumn: "cas_id");
                    _ = table.ForeignKey(
                        name: "FK_casecontent_skin",
                        column: x => x.skn_id,
                        principalTable: "t_e_skin_skn",
                        principalColumn: "skn_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_moneytransaction_mtr",
                columns: table => new
                {
                    txn_id = table.Column<int>(type: "integer", nullable: false),
                    pmt_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_moneytransaction_mtr", x => x.txn_id);
                    _ = table.ForeignKey(
                        name: "FK_moneytransaction_paymentmethod",
                        column: x => x.pmt_id,
                        principalTable: "t_e_paymentmethod_pmt",
                        principalColumn: "pmt_id");
                    _ = table.ForeignKey(
                        name: "FK_t_e_moneytransaction_mtr_t_e_transaction_txn_txn_id",
                        column: x => x.txn_id,
                        principalTable: "t_e_transaction_txn",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_randomtransaction_rtr",
                columns: table => new
                {
                    txn_id = table.Column<int>(type: "integer", nullable: false),
                    rtr_userseed = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cas_id = table.Column<int>(type: "integer", nullable: true),
                    frn_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_randomtransaction_rtr", x => x.txn_id);
                    _ = table.ForeignKey(
                        name: "FK_fairrandom_randomtransaction",
                        column: x => x.frn_id,
                        principalTable: "t_e_fairrandom_frn",
                        principalColumn: "frn_id",
                        onDelete: ReferentialAction.SetNull);
                    _ = table.ForeignKey(
                        name: "FK_randomtransaction_case",
                        column: x => x.cas_id,
                        principalTable: "t_e_case_cas",
                        principalColumn: "cas_id",
                        onDelete: ReferentialAction.SetNull);
                    _ = table.ForeignKey(
                        name: "FK_t_e_randomtransaction_rtr_t_e_transaction_txn_txn_id",
                        column: x => x.txn_id,
                        principalTable: "t_e_transaction_txn",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_pricehistory_prh",
                columns: table => new
                {
                    prh_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prh_pricedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    prh_pricevalue = table.Column<double>(type: "double precision", nullable: false),
                    prh_guessdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    wrt_id = table.Column<int>(type: "integer", nullable: false),
                    skn_id = table.Column<int>(type: "integer", nullable: false),
                    WearId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_pricehistory_prh", x => x.prh_id);
                    _ = table.ForeignKey(
                        name: "FK_pricehistory_skin",
                        column: x => x.skn_id,
                        principalTable: "t_e_skin_skn",
                        principalColumn: "skn_id");
                    _ = table.ForeignKey(
                        name: "FK_pricehistory_weartype",
                        column: x => x.wrt_id,
                        principalTable: "t_e_weartype_wrt",
                        principalColumn: "wrt_id");
                    _ = table.ForeignKey(
                        name: "FK_t_e_pricehistory_prh_t_e_wear_wer_WearId",
                        column: x => x.WearId,
                        principalTable: "t_e_wear_wer",
                        principalColumn: "wer_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_j_inventoryitem_inv",
                columns: table => new
                {
                    inv_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    wer_id = table.Column<int>(type: "integer", nullable: false),
                    inv_float = table.Column<float>(type: "real", nullable: false),
                    inv_isfavorite = table.Column<bool>(type: "boolean", nullable: false),
                    inv_acquiredon = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inv_removedon = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_j_inventoryitem_inv", x => x.inv_id);
                    _ = table.ForeignKey(
                        name: "FK_inventoryitem_user",
                        column: x => x.usr_id,
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id");
                    _ = table.ForeignKey(
                        name: "FK_inventoryitem_wear",
                        column: x => x.wer_id,
                        principalTable: "t_e_wear_wer",
                        principalColumn: "wer_id");
                });

            _ = migrationBuilder.CreateTable(
                name: "t_e_itemtransaction_itr",
                columns: table => new
                {
                    txn_id = table.Column<int>(type: "integer", nullable: false),
                    inv_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_e_itemtransaction_itr", x => x.txn_id);
                    _ = table.ForeignKey(
                        name: "FK_itemtransaction_inventoryitem",
                        column: x => x.inv_id,
                        principalTable: "t_j_inventoryitem_inv",
                        principalColumn: "inv_id",
                        onDelete: ReferentialAction.SetNull);
                    _ = table.ForeignKey(
                        name: "FK_t_e_itemtransaction_itr_t_e_transaction_txn_txn_id",
                        column: x => x.txn_id,
                        principalTable: "t_e_transaction_txn",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "t_j_upgraderesult_upg",
                columns: table => new
                {
                    inv_id = table.Column<int>(type: "integer", nullable: false),
                    tra_id = table.Column<int>(type: "integer", nullable: false),
                    upg_floatstart = table.Column<float>(type: "real", nullable: false),
                    upg_floatend = table.Column<float>(type: "real", nullable: false),
                    upg_probintact = table.Column<double>(type: "double precision", nullable: false),
                    upg_probdegrade = table.Column<double>(type: "double precision", nullable: false),
                    upg_propdestroy = table.Column<double>(type: "double precision", nullable: false),
                    upg_degradefuction = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    frn_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_t_j_upgraderesult_upg", x => new { x.inv_id, x.tra_id });
                    _ = table.ForeignKey(
                        name: "FK_fairrandom_upgraderesult",
                        column: x => x.frn_id,
                        principalTable: "t_e_fairrandom_frn",
                        principalColumn: "frn_id",
                        onDelete: ReferentialAction.SetNull);
                    _ = table.ForeignKey(
                        name: "FK_upgraderesult_inventoryitem",
                        column: x => x.inv_id,
                        principalTable: "t_j_inventoryitem_inv",
                        principalColumn: "inv_id");
                    _ = table.ForeignKey(
                        name: "FK_upgraderesult_randomtransaction",
                        column: x => x.tra_id,
                        principalTable: "t_e_randomtransaction_rtr",
                        principalColumn: "txn_id",
                        onDelete: ReferentialAction.SetNull);
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_bantype_bnt",
                columns: new[] { "bnt_id", "bnt_bantypedescription", "bnt_bantypename", "bnt_parentid" },
                values: new object[] { 1, "Perte d'accès à tous les fonctions du site, connexion incluse.", "Total", null });

            _ = migrationBuilder.InsertData(
                table: "t_e_itemtype_itt",
                columns: new[] { "itt_id", "itt_itemtypename", "itt_parentid" },
                values: new object[,]
                {
                    { 1, "Pistol", null },
                    { 2, "Rifle", null },
                    { 3, "Sniper Rifle", null },
                    { 4, "Machinegun", null },
                    { 5, "SMG", null },
                    { 6, "Shotgun", null },
                    { 7, "Knife", null },
                    { 8, "Gloves", null }
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_limittype_lmt",
                columns: new[] { "lmt_id", "lmt_duration", "lmt_limittypename" },
                values: new object[,]
                {
                    { 1, 1, "Dépôt Horaire" },
                    { 2, 1, "Dépenses Horaire" },
                    { 3, 1, "Ouvertures Horaire" },
                    { 4, 1, "Améliorations Horaire" },
                    { 5, 24, "Dépôt Quotidien" },
                    { 6, 24, "Dépenses Quotidien" },
                    { 7, 24, "Ouvertures Quotidien" },
                    { 8, 24, "Améliorations Quotidien" },
                    { 9, 168, "Dépôt Hebdomadaire" },
                    { 10, 168, "Dépenses Hebdomadaire" },
                    { 11, 168, "Ouvertures Hebdomadaire" },
                    { 12, 168, "Améliorations Hebdomadaire" },
                    { 13, 720, "Dépôt Mensuel" },
                    { 14, 720, "Dépenses Mensuel" },
                    { 15, 720, "Ouvertures Mensuel" },
                    { 16, 720, "Améliorations Mensuel" }
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_notificationtype_ntt",
                columns: new[] { "ntt_id", "ntt_notificationtypename" },
                values: new object[,]
                {
                    { 1, "Annonce" },
                    { 2, "Sécurité & Confidentialité" },
                    { 3, "Offres Spéciales" },
                    { 4, "Mise à jour" },
                    { 5, "Évènement" }
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_paymentmethod_pmt",
                columns: new[] { "pmt_id", "pmt_fromwallet", "pmt_paymentmethodname", "pmt_towallet" },
                values: new object[,]
                {
                    { 1, false, "Carte de crédit", true },
                    { 2, true, "RIB", false },
                    { 3, true, "PayPal", true }
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_rarity_rar",
                columns: new[] { "rar_id", "rar_raritycolor", "rar_rarityname" },
                values: new object[,]
                {
                    { 1, "#afafaf", "Consumer" },
                    { 2, "#6496e1", "Industrial" },
                    { 3, "#4b69cd", "Mil-Spec" },
                    { 4, "#8847ff", "Restricted" },
                    { 5, "#d32ce6", "Classified" },
                    { 6, "#eb4b4b", "Covert" },
                    { 7, "#f29b1d", "Contraband" }
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_tokentype_tkt",
                columns: new[] { "tkt_id", "tkt_tokentypename" },
                values: new object[,]
                {
                    { 1, "Remember Cookie" },
                    { 2, "Password Reset" },
                    { 3, "Email Verification" },
                    { 4, "Phone Verification" },
                    { 5, "2FA" }
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_weartype_wrt",
                columns: new[] { "wrt_id", "wrt_weartypename" },
                values: new object[,]
                {
                    { 1, "Factory New" },
                    { 2, "Minimal Wear" },
                    { 3, "Field-Tested" },
                    { 4, "Well-Worn" },
                    { 5, "Battle-Scarred" }
                });

            _ = migrationBuilder.InsertData(
                table: "t_e_bantype_bnt",
                columns: new[] { "bnt_id", "bnt_bantypedescription", "bnt_bantypename", "bnt_parentid" },
                values: new object[,]
                {
                    { 2, "Compte en lecture seule.", "Transactionnel", 1 },
                    { 3, "Impossibilité de modifier l'inventaire.", "Inventaire", 2 },
                    { 7, "Ne peut pas effectuer de dépôt ou de retrait.", "Monétaire", 2 },
                    { 4, "Interdiction d'ouvrir des caisses.", "Ouverture", 3 },
                    { 5, "Restrictions sur les ventes d'objets.", "Vente", 3 },
                    { 6, "Les objets dans l'inventaire ne peuvent pas être améliorés.", "Amélioration", 3 },
                    { 8, "Le compte ne peut pas être crédité depuis l'extérieur.", "Crédit", 7 },
                    { 9, "Le solde ne peut pas être exporté vers d'autres plateformes.", "Débit", 7 }
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_bantype_bnt_bnt_bantypename",
                table: "t_e_bantype_bnt",
                column: "bnt_bantypename",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_bantype_bnt_bnt_parentid",
                table: "t_e_bantype_bnt",
                column: "bnt_parentid");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_case_cas_cas_casename",
                table: "t_e_case_cas",
                column: "cas_casename",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_case_cas_cas_caseprice",
                table: "t_e_case_cas",
                column: "cas_caseprice");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_globalnotification_gnf_gnf_includevisitors",
                table: "t_e_globalnotification_gnf",
                column: "gnf_includevisitors");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_item_itm_itm_defindex",
                table: "t_e_item_itm",
                column: "itm_defindex");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_item_itm_itm_itemname",
                table: "t_e_item_itm",
                column: "itm_itemname");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_item_itm_itt_id",
                table: "t_e_item_itm",
                column: "itt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_itemtransaction_itr_inv_id",
                table: "t_e_itemtransaction_itr",
                column: "inv_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_itemtype_itt_itt_itemtypename",
                table: "t_e_itemtype_itt",
                column: "itt_itemtypename",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_itemtype_itt_itt_parentid",
                table: "t_e_itemtype_itt",
                column: "itt_parentid");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_limittype_lmt_lmt_limittypename",
                table: "t_e_limittype_lmt",
                column: "lmt_limittypename",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_moneytransaction_mtr_pmt_id",
                table: "t_e_moneytransaction_mtr",
                column: "pmt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_ntf_ntf_notificationdate",
                table: "t_e_notification_ntf",
                column: "ntf_notificationdate");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_ntf_ntt_id",
                table: "t_e_notification_ntf",
                column: "ntt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_notificationtype_ntt_ntt_notificationtypename",
                table: "t_e_notificationtype_ntt",
                column: "ntt_notificationtypename",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_paymentmethod_pmt_pmt_paymentmethodname",
                table: "t_e_paymentmethod_pmt",
                column: "pmt_paymentmethodname",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_prh_pricedate",
                table: "t_e_pricehistory_prh",
                column: "prh_pricedate");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_skn_id",
                table: "t_e_pricehistory_prh",
                column: "skn_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_WearId",
                table: "t_e_pricehistory_prh",
                column: "WearId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_pricehistory_prh_wrt_id",
                table: "t_e_pricehistory_prh",
                column: "wrt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_promocode_prc_cas_id",
                table: "t_e_promocode_prc",
                column: "cas_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_promocode_prc_prc_code",
                table: "t_e_promocode_prc",
                column: "prc_code");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_promocode_prc_prc_expirydate",
                table: "t_e_promocode_prc",
                column: "prc_expirydate");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_promocode_prc_usr_id",
                table: "t_e_promocode_prc",
                column: "usr_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_randomtransaction_rtr_cas_id",
                table: "t_e_randomtransaction_rtr",
                column: "cas_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_randomtransaction_rtr_frn_id",
                table: "t_e_randomtransaction_rtr",
                column: "frn_id",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_rarity_rar_rar_rarityname",
                table: "t_e_rarity_rar",
                column: "rar_rarityname",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_skin_skn_itm_id",
                table: "t_e_skin_skn",
                column: "itm_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_skin_skn_rar_id",
                table: "t_e_skin_skn",
                column: "rar_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_skin_skn_skn_paintindex",
                table: "t_e_skin_skn",
                column: "skn_paintindex");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_skin_skn_skn_skinname",
                table: "t_e_skin_skn",
                column: "skn_skinname");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_token_tkn_tkn_token",
                table: "t_e_token_tkn",
                column: "tkn_token");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_token_tkn_tkn_tokencreationdate",
                table: "t_e_token_tkn",
                column: "tkn_tokencreationdate");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_token_tkn_tkn_tokenexpiry",
                table: "t_e_token_tkn",
                column: "tkn_tokenexpiry");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_token_tkn_tkt_id",
                table: "t_e_token_tkn",
                column: "tkt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_token_tkn_usr_id",
                table: "t_e_token_tkn",
                column: "usr_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_tokentype_tkt_tkt_tokentypename",
                table: "t_e_tokentype_tkt",
                column: "tkt_tokentypename",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_transaction_txn_ntf_id",
                table: "t_e_transaction_txn",
                column: "ntf_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_transaction_txn_txn_cancelledon",
                table: "t_e_transaction_txn",
                column: "txn_cancelledon");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_transaction_txn_txn_transactiondate",
                table: "t_e_transaction_txn",
                column: "txn_transactiondate");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_transaction_txn_usr_id",
                table: "t_e_transaction_txn",
                column: "usr_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_usr_deletedon",
                table: "t_e_user_usr",
                column: "usr_deletedon");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_usr_email",
                table: "t_e_user_usr",
                column: "usr_email",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_usr_isadmin",
                table: "t_e_user_usr",
                column: "usr_isadmin");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_usr_lastlogin",
                table: "t_e_user_usr",
                column: "usr_lastlogin");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_usr_login",
                table: "t_e_user_usr",
                column: "usr_login",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_usr_phone",
                table: "t_e_user_usr",
                column: "usr_phone",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_usr_steamid",
                table: "t_e_user_usr",
                column: "usr_steamid",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_usernotification_unf_usr_id",
                table: "t_e_usernotification_unf",
                column: "usr_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_wear_wer_skn_id",
                table: "t_e_wear_wer",
                column: "skn_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_wear_wer_wer_uuid",
                table: "t_e_wear_wer",
                column: "wer_uuid");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_wear_wer_wrt_id",
                table: "t_e_wear_wer",
                column: "wrt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_e_weartype_wrt_wrt_weartypename",
                table: "t_e_weartype_wrt",
                column: "wrt_weartypename",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_ban_ban_ban_bandate",
                table: "t_j_ban_ban",
                column: "ban_bandate");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_ban_ban_bnt_id",
                table: "t_j_ban_ban",
                column: "bnt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_casecontent_cct_cct_weight",
                table: "t_j_casecontent_cct",
                column: "cct_weight");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_casecontent_cct_skn_id",
                table: "t_j_casecontent_cct",
                column: "skn_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_favorite_fav_cas_id",
                table: "t_j_favorite_fav",
                column: "cas_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_inventoryitem_inv_inv_acquiredon",
                table: "t_j_inventoryitem_inv",
                column: "inv_acquiredon");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_inventoryitem_inv_inv_isfavorite",
                table: "t_j_inventoryitem_inv",
                column: "inv_isfavorite");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_inventoryitem_inv_inv_removedon",
                table: "t_j_inventoryitem_inv",
                column: "inv_removedon");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_inventoryitem_inv_usr_id",
                table: "t_j_inventoryitem_inv",
                column: "usr_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_inventoryitem_inv_wer_id",
                table: "t_j_inventoryitem_inv",
                column: "wer_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_limit_lim_lmt_id",
                table: "t_j_limit_lim",
                column: "lmt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_notificationsetting_nts_ntt_id",
                table: "t_j_notificationsetting_nts",
                column: "ntt_id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_upgraderesult_upg_frn_id",
                table: "t_j_upgraderesult_upg",
                column: "frn_id",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_t_j_upgraderesult_upg_tra_id",
                table: "t_j_upgraderesult_upg",
                column: "tra_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "t_e_globalnotification_gnf");

            _ = migrationBuilder.DropTable(
                name: "t_e_itemtransaction_itr");

            _ = migrationBuilder.DropTable(
                name: "t_e_moneytransaction_mtr");

            _ = migrationBuilder.DropTable(
                name: "t_e_pricehistory_prh");

            _ = migrationBuilder.DropTable(
                name: "t_e_promocode_prc");

            _ = migrationBuilder.DropTable(
                name: "t_e_token_tkn");

            _ = migrationBuilder.DropTable(
                name: "t_e_usernotification_unf");

            _ = migrationBuilder.DropTable(
                name: "t_j_ban_ban");

            _ = migrationBuilder.DropTable(
                name: "t_j_casecontent_cct");

            _ = migrationBuilder.DropTable(
                name: "t_j_favorite_fav");

            _ = migrationBuilder.DropTable(
                name: "t_j_limit_lim");

            _ = migrationBuilder.DropTable(
                name: "t_j_notificationsetting_nts");

            _ = migrationBuilder.DropTable(
                name: "t_j_upgraderesult_upg");

            _ = migrationBuilder.DropTable(
                name: "t_e_paymentmethod_pmt");

            _ = migrationBuilder.DropTable(
                name: "t_e_tokentype_tkt");

            _ = migrationBuilder.DropTable(
                name: "t_e_bantype_bnt");

            _ = migrationBuilder.DropTable(
                name: "t_e_limittype_lmt");

            _ = migrationBuilder.DropTable(
                name: "t_j_inventoryitem_inv");

            _ = migrationBuilder.DropTable(
                name: "t_e_randomtransaction_rtr");

            _ = migrationBuilder.DropTable(
                name: "t_e_wear_wer");

            _ = migrationBuilder.DropTable(
                name: "t_e_fairrandom_frn");

            _ = migrationBuilder.DropTable(
                name: "t_e_case_cas");

            _ = migrationBuilder.DropTable(
                name: "t_e_transaction_txn");

            _ = migrationBuilder.DropTable(
                name: "t_e_skin_skn");

            _ = migrationBuilder.DropTable(
                name: "t_e_weartype_wrt");

            _ = migrationBuilder.DropTable(
                name: "t_e_notification_ntf");

            _ = migrationBuilder.DropTable(
                name: "t_e_user_usr");

            _ = migrationBuilder.DropTable(
                name: "t_e_item_itm");

            _ = migrationBuilder.DropTable(
                name: "t_e_rarity_rar");

            _ = migrationBuilder.DropTable(
                name: "t_e_notificationtype_ntt");

            _ = migrationBuilder.DropTable(
                name: "t_e_itemtype_itt");
        }
    }
}
