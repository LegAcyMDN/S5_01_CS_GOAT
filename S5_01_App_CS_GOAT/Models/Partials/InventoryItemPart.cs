using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class InventoryItem : IUserDependant
    {
        public int? DependantUserId { get => this.UserId; }

        public IEnumerable<PriceHistory> PriceHistories(bool allowGuess = false)
        {
            return this.Wear.PriceHistories(allowGuess);
        }

        public PriceHistory? LastPrice(bool allowGuess = false)
        {
            return this.Wear.LastPrice(allowGuess);
        }
    }
}