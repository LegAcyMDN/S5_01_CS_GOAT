using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_tokentype_tkt")]
    [Index(nameof(TokenTypeName), IsUnique = true)]
    public class TokenType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("tkt_id")]
        public int TokenTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("tkt_tokentypename")]
        public string TokenTypeName { get; set; } = null!;

        [InverseProperty(nameof(Token.TokenType))]
        public virtual ICollection<Token> Tokens { get; set; } = null!;
    }
}