namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    /// <summary>
    /// Partial class providing price and wear retrieval methods for Skin entities
    /// </summary>
    public partial class Skin
    {
        /// <summary>
        /// Gets all price histories for this skin across all its wear classes
        /// </summary>
        /// <param name="allowGuess">If true, includes guessed future prices; otherwise only actual historical prices</param>
        /// <returns>Collection of PriceHistory entries for this skin</returns>
        public IEnumerable<PriceHistory> Prices(bool allowGuess = false)
        {
            IEnumerable<PriceHistory> histories = [];
            foreach (Wear wear in Wears)
            {
                PriceHistory? lastPrice = wear.LastPrice(allowGuess);
                if (lastPrice != null)
                {
                    _ = histories.Append(lastPrice);
                }
            }
            return histories;
        }


        /// <summary>
        /// Finds the wear class of this skin closest to the specified float value
        /// </summary>
        /// <remarks>
        /// Float values in CSGO represent skin wear from 0.0 (Factory New) to 1.0 (Battle-Scarred).
        /// This method finds the wear class with the closest float value to the provided value.
        /// </remarks>
        /// <param name="floatValue">The float value to match (0.0 to 1.0)</param>
        /// <returns>The wear class with the closest float value</returns>
        /// <exception cref="Exception">Thrown if no wear classes are available for this skin</exception>
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
            return closestWear == null ? throw new Exception("No wears available for this skin.") : closestWear;
        }
    }
}