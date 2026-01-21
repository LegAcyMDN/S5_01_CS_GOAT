namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    /// <summary>
    /// Partial class providing price and texture retrieval methods for Wear/Skin entities
    /// </summary>
    public partial class Wear
    {

        /// <summary>
        /// Gets the price history entries for this wear class
        /// </summary>
        /// <param name="allowGuess">If true, includes predicted/guessed prices; otherwise only historical prices</param>
        /// <returns>Collection of PriceHistory entries</returns>
        public IEnumerable<PriceHistory> PriceHistories(bool allowGuess)
        {
            return (allowGuess
                ? WearClass?.PriceHistories
                : WearClass?.PriceHistories.Where(p => p.GuessDate == null))
                ?? Enumerable.Empty<PriceHistory>();
        }

        /// <summary>
        /// Gets the most recent price history entry for this wear class
        /// </summary>
        /// <param name="allowGuess">If true, includes predicted prices; otherwise only historical prices</param>
        /// <returns>The most recent PriceHistory entry; null if no history exists</returns>
        public PriceHistory? LastPrice(bool allowGuess)
        {
            return PriceHistories(allowGuess)
                       .OrderByDescending(p => p.PriceDate)
                       .FirstOrDefault();
        }

        public double? CurrentPrice => LastPrice(false)?.PriceValue;

        public async Task<byte[]?[]> GetTexture()
        {
            int nbTextures = Skin.UvType != 3 ? 1 : 2;
            byte[]?[] textures = new byte[nbTextures][];

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("https://3d.cs.money")
            };

            for (int i = 1; i <= nbTextures; i++)
            {
                string url = "/images/texture/s2/" + Uuid + $"_component{i}_texture1.png";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                textures[i - 1] = response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
            }
            return textures;
        }
    }
}