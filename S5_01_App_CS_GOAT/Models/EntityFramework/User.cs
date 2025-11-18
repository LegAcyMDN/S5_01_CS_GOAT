using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_user_usr")]
    [Index(nameof(Login), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(SteamId), IsUnique = true)]
    [Index(nameof(LastLogin))]
    [Index(nameof(IsAdmin))]
    [Index(nameof(DeleteOn))]
    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("usr_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("usr_login")]
        public string Login { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Column("usr_displayname")]
        public string DisplayName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        [Column("usr_email")]
        public string Email { get; set; } = null!;

        [StringLength(20)]
        [Phone]
        [Column("usr_phone")]
        public string? Phone { get; set; }

        [Column("usr_phoneverifiedon")]
        public DateTime? PhoneVerifiedOn { get; set; }

        [Column("usr_emailverifiedon")]
        public DateTime? EmailVerifiedOn { get; set; }

        [Required]
        [Column("usr_twofaisphone")]
        public bool TwoFaIsPhone { get; set; } = false;

        [Required]
        [Column("usr_twofaisemail")]
        public bool TwoFaIsEmail { get; set; } = false;

        [Required]
        [Column("usr_isadmin")]
        public bool IsAdmin { get; set; } = false;

        [Required]
        [Column("usr_creationdate")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [Column("usr_lastlogin")]
        public DateTime LastLogin { get; set; } = DateTime.Now;

        [StringLength(255)]
        [Column("usr_saltpassword")]
        public string? SaltPassword { get; set; }

        [StringLength(255)]
        [Column("usr_hashpassword")]
        public string? HashPassword { get; set; }

        [StringLength(50)]
        [Column("usr_steamid")]
        public string? SteamId { get; set; }

        [Required]
        [StringLength(16)]
        [Column("usr_seed")]
        public string Seed { get; set; } = GenerateRandomSeed();

        [Required]
        [Column("usr_nonce")]
        [Range(0, int.MaxValue)]
        public int Nonce { get; set; } = 0;

        [Required]
        [Column("usr_wallet")]
        [Range(0.0, double.MaxValue)]
        public double Wallet { get; set; } = 0.0;

        [Column("usr_deletedon")]
        public DateTime? DeleteOn { get; set; }

        [InverseProperty(nameof(Limit.User))]
        public virtual ICollection<Limit> Limits { get; set; } = null!;

        [InverseProperty(nameof(Token.User))]
        public virtual ICollection<Token> Tokens { get; set; } = null!;

        [InverseProperty(nameof(UserNotification.User))]
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = null!;

        [InverseProperty(nameof(NotificationSetting.User))]
        public virtual ICollection<NotificationSetting> NotificationSettings { get; set; } = null!;

        [InverseProperty(nameof(Transaction.User))]
        public virtual ICollection<Transaction> Transactions { get; set; } = null!;

        [InverseProperty(nameof(Ban.User))]
        public virtual ICollection<Ban> Bans { get; set; } = null!;

        [InverseProperty(nameof(Favorite.User))]
        public virtual ICollection<Favorite> Favorites { get; set; } = null!;

        [InverseProperty(nameof(InventoryItem.User))]
        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = null!;
    }
}
