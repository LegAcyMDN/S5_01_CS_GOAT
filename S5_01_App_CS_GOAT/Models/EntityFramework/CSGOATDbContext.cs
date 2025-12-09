using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class CSGOATDbContext : DbContext
    {
        // Core Entity Sets
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Skin> Skins { get; set; }
        public DbSet<Wear> Wears { get; set; }
        public DbSet<WearType> WearTypes { get; set; }
        public DbSet<Rarity> Rarities { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseContent> CaseContents { get; set; }

        // Inventory & Transaction Sets
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MoneyTransaction> MoneyTransactions { get; set; }
        public DbSet<ItemTransaction> ItemTransactions { get; set; }
        public DbSet<RandomTransaction> RandomTransactions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        // Upgrade & Random Sets
        public DbSet<UpgradeResult> UpgradeResults { get; set; }
        public DbSet<FairRandom> FairRandoms { get; set; }

        // Notification Sets
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<GlobalNotification> GlobalNotifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<NotificationSetting> NotificationSettings { get; set; }

        // Security & Access Sets
        public DbSet<Token> Tokens { get; set; }
        public DbSet<TokenType> TokenTypes { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<BanType> BanTypes { get; set; }

        // Other Sets
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<Limit> Limits { get; set; }
        public DbSet<LimitType> LimitTypes { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        public CSGOATDbContext()
        {
        }

        public CSGOATDbContext(DbContextOptions<CSGOATDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(p => p.UserId);
            });

            // Configure Item Type hierarchy
            modelBuilder.Entity<ItemType>(e =>
            {
                e.HasKey(p => p.ItemTypeId);
                e.HasMany(p => p.SubItemTypes)
                    .WithOne(m => m.ParentItemType)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Configure Item
            modelBuilder.Entity<Item>(e =>
            {
                e.HasKey(p => p.ItemId);
                e.HasMany(p => p.Skins)
                    .WithOne(m => m.Item)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_skin_item");
            });

            // Configure Rarity
            modelBuilder.Entity<Rarity>(e =>
            {
                e.HasKey(p => p.RarityId);
                e.HasMany(p => p.Skins)
                    .WithOne(m => m.Rarity)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_skin_rarity");
            });

            // Configure Skin
            modelBuilder.Entity<Skin>(e =>
            {
                e.HasKey(p => p.SkinId);
                e.HasMany(p => p.Wears)
                    .WithOne(m => m.Skin)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_wear_skin");
                e.HasMany(p => p.CaseContents)
                    .WithOne(m => m.Skin)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_casecontent_skin");
            });

            // Configure Wear
            modelBuilder.Entity<Wear>(e =>
            {
                e.HasKey(p => p.WearId);
                e.HasMany(p => p.InventoryItems)
                    .WithOne(m => m.Wear)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_inventoryitem_wear");
                e.HasOne(p => p.WearType)
                    .WithMany(m => m.Wears)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_wear_weartype");
            });

            // Configure Case
            modelBuilder.Entity<Case>(e =>
            {
                e.HasKey(p => p.CaseId);
                e.HasMany(p => p.CaseContents)
                    .WithOne(m => m.Case)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_casecontent_case");
                e.HasMany(p => p.RandomTransactions)
                    .WithOne(m => m.Case)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_randomtransaction_case");
                e.HasMany(p => p.Favorites)
                    .WithOne(m => m.Case)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_favorite_case");
            });

            // Configure CaseContent
            modelBuilder.Entity<CaseContent>(e =>
            {
                e.HasKey(p => new { p.CaseId, p.SkinId });
            });

            // Configure PaymentMethod
            modelBuilder.Entity<PaymentMethod>(e =>
            {
                e.HasKey(p => p.PaymentMethodId);
                e.HasMany(p => p.MoneyTransactions)
                    .WithOne(m => m.PaymentMethod)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_moneytransaction_paymentmethod");
            });

            // Configure Transaction
            modelBuilder.Entity<Transaction>(e =>
            {
                e.HasKey(p => p.TransactionId);
                e.HasOne(p => p.User)
                    .WithMany(m => m.Transactions)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_transaction_user");
            });

            modelBuilder.Entity<MoneyTransaction>(e =>
            {
                e.HasOne(p => p.PaymentMethod)
                    .WithMany(m => m.MoneyTransactions)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_moneytransaction_paymentmethod");
            });

            modelBuilder.Entity<ItemTransaction>(e =>
            {
                e.HasOne(p => p.InventoryItem)
                    .WithMany(m => m.ItemTransactions)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_itemtransaction_inventoryitem");
            });

            modelBuilder.Entity<RandomTransaction>(e =>
            {
                e.HasMany(p => p.UpgradeResults)
                    .WithOne(m => m.RandomTransaction)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_upgraderesult_randomtransaction");
                e.HasOne(p => p.Case)
                    .WithMany(m => m.RandomTransactions)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_randomtransaction_case");
            });

            // Configure Notification
            modelBuilder.Entity<Notification>(e =>
            {
                e.HasKey(p => p.NotificationId);
                e.HasOne(p => p.NotificationType)
                    .WithMany(m => m.Notifications)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_notification_notificationtype");
            });

            modelBuilder.Entity<GlobalNotification>(e =>
            {
                e.ToTable("t_e_globalnotification_gnf");
            });

            modelBuilder.Entity<UserNotification>(e =>
            {
                e.ToTable("t_e_usernotification_unf");
                e.HasOne(p => p.User)
                    .WithMany(m => m.UserNotifications)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_usernotification_user");
            });

            // Configure NotificationType
            modelBuilder.Entity<NotificationType>(e =>
            {
                e.HasKey(p => p.NotificationTypeId);
                e.HasMany(p => p.Notifications)
                    .WithOne(m => m.NotificationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_notification_notificationtype");
                e.HasMany(p => p.NotificationSettings)
                    .WithOne(m => m.NotificationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_notificationsetting_notificationtype");
            });

            // Configure NotificationSetting
            modelBuilder.Entity<NotificationSetting>(e =>
            {
                e.HasKey(p => new { p.UserId, p.NotificationTypeId });
            });

            // Configure Token
            modelBuilder.Entity<Token>(e =>
            {
                e.HasKey(p => p.TokenId);
                e.HasOne(p => p.User)
                    .WithMany(m => m.Tokens)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_token_user");
                e.HasOne(p => p.TokenType)
                    .WithMany(m => m.Tokens)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_token_tokentype");
            });

            // Configure TokenType
            modelBuilder.Entity<TokenType>(e =>
            {
                e.HasKey(p => p.TokenTypeId);
                e.HasMany(p => p.Tokens)
                    .WithOne(m => m.TokenType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_token_tokentype");
            });

            // Configure Ban
            modelBuilder.Entity<Ban>(e =>
            {
                e.HasKey(p => new { p.UserId, p.BanTypeId });
                e.HasOne(p => p.User)
                    .WithMany(m => m.Bans)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ban_user");
                e.HasOne(p => p.BanType)
                    .WithMany(m => m.Bans)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ban_bantype");
            });

            // Configure BanType
            modelBuilder.Entity<BanType>(e =>
            {
                e.HasKey(p => p.BanTypeId);
                e.HasMany(p => p.Bans)
                    .WithOne(m => m.BanType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ban_bantype");
                e.HasOne(p => p.ParentBanType)
                    .WithMany(m => m.SubBanTypes)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Configure Favorite
            modelBuilder.Entity<Favorite>(e =>
            {
                e.HasKey(p => new { p.UserId, p.CaseId });
                e.HasOne(p => p.User)
                    .WithMany(m => m.Favorites)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_favorite_user");
                e.HasOne(p => p.Case)
                    .WithMany(m => m.Favorites)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_favorite_case");
            });

            // Configure InventoryItem
            modelBuilder.Entity<InventoryItem>(e =>
            {
                e.HasKey(p => p.InventoryItemId);
                e.HasOne(p => p.User)
                    .WithMany(m => m.InventoryItems)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_inventoryitem_user");
                e.HasOne(p => p.Wear)
                    .WithMany(m => m.InventoryItems)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_inventoryitem_wear");
                e.HasMany(p => p.UpgradeResults)
                    .WithOne(m => m.InventoryItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_upgraderesult_inventoryitem");
                e.HasMany(p => p.ItemTransactions)
                    .WithOne(m => m.InventoryItem)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_itemtransaction_inventoryitem");
            });

            // Configure UpgradeResult
            modelBuilder.Entity<UpgradeResult>(e =>
            {
                e.HasKey(p => new { p.InventoryItemId, p.TransactionId });
                e.HasOne(p => p.InventoryItem)
                    .WithMany(m => m.UpgradeResults)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_upgraderesult_inventoryitem");
                e.HasOne(p => p.RandomTransaction)
                    .WithMany(m => m.UpgradeResults)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_upgraderesult_randomtransaction");
                e.HasOne(p => p.FairRandom)
                    .WithOne(m => m.UpgradeResult)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_fairrandom_upgraderesult");
            });

            // Configure FairRandom
            modelBuilder.Entity<FairRandom>(e =>
            {
                e.HasKey(p => p.FairRandomId);
                e.HasOne(p => p.RandomTransaction)
                    .WithOne(m => m.FairRandom)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_fairrandom_randomtransaction");
                e.HasOne(p => p.UpgradeResult)
                    .WithOne(m => m.FairRandom)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_fairrandom_upgraderesult");
            });

            // Configure PriceHistory
            modelBuilder.Entity<PriceHistory>(e =>
            {
                e.HasKey(p => p.PriceHistoryId);
                e.HasOne(p => p.WearType)
                    .WithMany(m => m.PriceHistories)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_pricehistory_weartype");
                e.HasOne(p => p.Skin)
                    .WithMany(m => m.PriceHistories)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_pricehistory_skin");
            });

            // Configure Limit
            modelBuilder.Entity<Limit>(e =>
            {
                e.HasKey(p => new { p.UserId, p.LimitTypeId });
                e.HasOne(p => p.User)
                    .WithMany(m => m.Limits)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_limit_user");
                e.HasOne(p => p.LimitType)
                    .WithMany(m => m.Limits)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_limit_limittype");
            });

            // Configure LimitType
            modelBuilder.Entity<LimitType>(e =>
            {
                e.HasKey(p => p.LimitTypeId);
                e.HasMany(p => p.Limits)
                    .WithOne(m => m.LimitType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_limit_limittype");
            });

            // Configure PromoCode
            modelBuilder.Entity<PromoCode>(e =>
            {
                e.HasKey(p => p.PromoCodeId);
                e.HasOne(p => p.User)
                    .WithMany(m => m.PromoCodes)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_promocode_user");
                e.HasOne(p => p.Case)
                    .WithMany(m => m.PromoCodes)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_promocode_case");
            });

            // Configure WearType
            modelBuilder.Entity<WearType>(e =>
            {
                e.HasKey(p => p.WearTypeId);
                e.HasMany(p => p.Wears)
                    .WithOne(m => m.WearType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_wear_weartype");
                e.HasMany(p => p.PriceHistories)
                    .WithOne(m => m.WearType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_pricehistory_weartype");
            });

            // Seeding static data

            modelBuilder.Entity<ItemType>().HasData(
                new ItemType { ItemTypeId = 1, ItemTypeName = "Pistol", ParentItemTypeId = null },
                new ItemType { ItemTypeId = 2, ItemTypeName = "Rifle", ParentItemTypeId = null },
                new ItemType { ItemTypeId = 3, ItemTypeName = "Sniper Rifle", ParentItemTypeId = null },
                new ItemType { ItemTypeId = 4, ItemTypeName = "Machinegun", ParentItemTypeId = null },
                new ItemType { ItemTypeId = 5, ItemTypeName = "SMG", ParentItemTypeId = null },
                new ItemType { ItemTypeId = 6, ItemTypeName = "Shotgun", ParentItemTypeId = null },
                new ItemType { ItemTypeId = 7, ItemTypeName = "Knife", ParentItemTypeId = null },
                new ItemType { ItemTypeId = 8, ItemTypeName = "Gloves", ParentItemTypeId = null }
            );

            modelBuilder.Entity<WearType>().HasData(
                new WearType { WearTypeId = 1, WearTypeName = "Factory New" },
                new WearType { WearTypeId = 2, WearTypeName = "Minimal Wear" },
                new WearType { WearTypeId = 3, WearTypeName = "Field-Tested" },
                new WearType { WearTypeId = 4, WearTypeName = "Well-Worn" },
                new WearType { WearTypeId = 5, WearTypeName = "Battle-Scarred" }
            );

            modelBuilder.Entity<Rarity>().HasData(
                new Rarity { RarityId = 1, RarityName = "Consumer", RarityColor = "#afafaf" },
                new Rarity { RarityId = 2, RarityName = "Industrial", RarityColor = "#6496e1" },
                new Rarity { RarityId = 3, RarityName = "Mil-Spec", RarityColor = "#4b69cd" },
                new Rarity { RarityId = 4, RarityName = "Restricted", RarityColor = "#8847ff" },
                new Rarity { RarityId = 5, RarityName = "Classified", RarityColor = "#d32ce6" },
                new Rarity { RarityId = 6, RarityName = "Covert", RarityColor = "#eb4b4b" },
                new Rarity { RarityId = 7, RarityName = "Contraband", RarityColor = "#f29b1d" }
            );

            modelBuilder.Entity<BanType>().HasData(
                new BanType { BanTypeId = 1, BanTypeName = "Total", BanTypeDescription = "Perte d'accès à tous les fonctions du site, connexion incluse." },
                new BanType { BanTypeId = 2, BanTypeName = "Transactionnel", BanTypeDescription = "Compte en lecture seule.", ParentBanTypeId = 1 },
                new BanType { BanTypeId = 3, BanTypeName = "Inventaire", BanTypeDescription = "Impossibilité de modifier l'inventaire.", ParentBanTypeId = 2 },
                new BanType { BanTypeId = 4, BanTypeName = "Ouverture", BanTypeDescription = "Interdiction d'ouvrir des caisses.", ParentBanTypeId = 3 },
                new BanType { BanTypeId = 5, BanTypeName = "Vente", BanTypeDescription = "Restrictions sur les ventes d'objets.", ParentBanTypeId = 3 },
                new BanType { BanTypeId = 6, BanTypeName = "Amélioration", BanTypeDescription = "Les objets dans l'inventaire ne peuvent pas être améliorés.", ParentBanTypeId = 3 },
                new BanType { BanTypeId = 7, BanTypeName = "Monétaire", BanTypeDescription = "Ne peut pas effectuer de dépôt ou de retrait.", ParentBanTypeId = 2 },
                new BanType { BanTypeId = 8, BanTypeName = "Crédit", BanTypeDescription = "Le compte ne peut pas être crédité depuis l'extérieur.", ParentBanTypeId = 7 },
                new BanType { BanTypeId = 9, BanTypeName = "Débit", BanTypeDescription = "Le solde ne peut pas être exporté vers d'autres plateformes.", ParentBanTypeId = 7 }
            );

            modelBuilder.Entity<TokenType>().HasData(
                new TokenType { TokenTypeId = 1, TokenTypeName = "Remember Cookie" },
                new TokenType { TokenTypeId = 2, TokenTypeName = "Password Reset" },
                new TokenType { TokenTypeId = 3, TokenTypeName = "Email Verification" },
                new TokenType { TokenTypeId = 4, TokenTypeName = "Phone Verification" },
                new TokenType { TokenTypeId = 5, TokenTypeName = "2FA" }
            );

            modelBuilder.Entity<NotificationType>().HasData(
                new NotificationType { NotificationTypeId = 1, NotificationTypeName = "Annonce" },
                new NotificationType { NotificationTypeId = 2, NotificationTypeName = "Sécurité & Confidentialité" },
                new NotificationType { NotificationTypeId = 3, NotificationTypeName = "Offres Spéciales" },
                new NotificationType { NotificationTypeId = 4, NotificationTypeName = "Mise à jour" },
                new NotificationType { NotificationTypeId = 5, NotificationTypeName = "Évènement" }
            );

            modelBuilder.Entity<LimitType>().HasData(
                new LimitType { LimitTypeId = 1, LimitTypeName = "Dépôt Horaire", Duration = 1 },
                new LimitType { LimitTypeId = 2, LimitTypeName = "Dépenses Horaire", Duration = 1 },
                new LimitType { LimitTypeId = 3, LimitTypeName = "Ouvertures Horaire", Duration = 1 },
                new LimitType { LimitTypeId = 4, LimitTypeName = "Améliorations Horaire", Duration = 1 },
                new LimitType { LimitTypeId = 5, LimitTypeName = "Dépôt Quotidien", Duration = 24 },
                new LimitType { LimitTypeId = 6, LimitTypeName = "Dépenses Quotidien", Duration = 24 },
                new LimitType { LimitTypeId = 7, LimitTypeName = "Ouvertures Quotidien", Duration = 24 },
                new LimitType { LimitTypeId = 8, LimitTypeName = "Améliorations Quotidien", Duration = 24 },
                new LimitType { LimitTypeId = 9, LimitTypeName = "Dépôt Hebdomadaire", Duration = 168 },
                new LimitType { LimitTypeId = 10, LimitTypeName = "Dépenses Hebdomadaire", Duration = 168 },
                new LimitType { LimitTypeId = 11, LimitTypeName = "Ouvertures Hebdomadaire", Duration = 168 },
                new LimitType { LimitTypeId = 12, LimitTypeName = "Améliorations Hebdomadaire", Duration = 168 },
                new LimitType { LimitTypeId = 13, LimitTypeName = "Dépôt Mensuel", Duration = 720 },
                new LimitType { LimitTypeId = 14, LimitTypeName = "Dépenses Mensuel", Duration = 720 },
                new LimitType { LimitTypeId = 15, LimitTypeName = "Ouvertures Mensuel", Duration = 720 },
                new LimitType { LimitTypeId = 16, LimitTypeName = "Améliorations Mensuel", Duration = 720 }
            );

            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { PaymentMethodId = 1, PaymentMethodName = "Carte de crédit", FromWallet = false, ToWallet = true },
                new PaymentMethod { PaymentMethodId = 2, PaymentMethodName = "RIB", FromWallet = true, ToWallet = false },
                new PaymentMethod { PaymentMethodId = 3, PaymentMethodName = "PayPal", FromWallet = true, ToWallet = true }
            );

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}