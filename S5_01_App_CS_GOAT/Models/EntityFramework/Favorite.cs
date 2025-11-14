using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_favorite_fav")]
    public class Favorite
    {
        [Key]
        [Column("usr_id", Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column("cas_id", Order = 2)]
        public int CaseId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Favorites))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(CaseId))]
        [InverseProperty(nameof(Case.Favorites))]
        public virtual Case Case { get; set; } = null!;
    }
}