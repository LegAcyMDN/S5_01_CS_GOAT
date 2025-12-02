using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_token_tkn")]
    [Index(nameof(TokenCreationDate))]
    [Index(nameof(TokenExpiry))]
    [Index(nameof(TokenValue))]
    public partial class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("tkn_id")]
        public int TokenId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("tkn_token")]
        public string TokenValue { get; set; } = SecurityService.GenerateToken(255);

        [Required]
        [Column("tkn_tokencreationdate")]
        public DateTime TokenCreationDate { get; set; } = DateTime.Now;

        [Required]
        [Column("tkn_tokenexpiry")]
        public DateTime TokenExpiry { get; set; }

        [Required]
        [Column("usr_id")]
        public int UserId { get; set; }

        [Required]
        [Column("tkt_id")]
        public int TokenTypeId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Tokens))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(TokenTypeId))]
        [InverseProperty(nameof(TokenType.Tokens))]
        public virtual TokenType TokenType { get; set; } = null!;
    }
}