namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class Skin
    {
        public IEnumerable<PriceHistory> Prices(bool allowGuess = false)
        {
            IEnumerable<PriceHistory> histories = new List<PriceHistory>();
            foreach (Wear wear in this.Wears)
            {
                PriceHistory? lastPrice = wear.LastPrice(allowGuess);
                if (lastPrice != null) histories.Append(lastPrice);
            }
            return histories;
        }


        public Wear GetClosestWear(float floatValue)
        {
            Wear? closestWear = null;
            float closestDifference = float.MaxValue;

            foreach (Wear wear in Wears)
            {
                float difference = Math.Abs(wear.WearFloat - floatValue);
                if (difference < closestDifference)
                {
                    closestDifference = difference;
                    closestWear = wear;
                }
            }
            if (closestWear == null)
                throw new Exception("No wears available for this skin.");
            return closestWear;
        }
    }
}