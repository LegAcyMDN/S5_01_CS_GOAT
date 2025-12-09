using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_bantype_bnt")]
    [Index(nameof(BanTypeName), IsUnique = true)]
    public partial class BanType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("bnt_id")]
        public int BanTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("bnt_bantypename")]
        public string BanTypeName { get; set; } = null!;

        [Required]
        [Column("bnt_bantypedescription")]
        [StringLength(255)]
        public string BanTypeDescription { get; set; } = null!;

        [Column("bnt_parentid")]
        public int? ParentBanTypeId { get; set; }

        [InverseProperty(nameof(Ban.BanType))]
        public virtual ICollection<Ban> Bans { get; set; } = null!;

        [ForeignKey(nameof(ParentBanTypeId))]
        [InverseProperty(nameof(SubBanTypes))]
        public virtual BanType? ParentBanType { get; set; }

        [InverseProperty(nameof(ParentBanType))]
        public virtual ICollection<BanType> SubBanTypes { get; set; } = null!;
    }
}