using System.Collections.Generic;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class Wear
    {

        public IEnumerable<PriceHistory> PriceHistories(bool allowGuess)
        {
            return allowGuess
                ? this.WearClass.PriceHistories
                : this.WearClass.PriceHistories.Where(p => p.GuessDate == null);
        }

        public PriceHistory? LastPrice(bool allowGuess)
        {
            return this.PriceHistories(allowGuess)
                       .OrderByDescending(p => p.PriceDate)
                       .FirstOrDefault();
        }

        public double? CurrentPrice => this.LastPrice(false)?.PriceValue;

        public async Task<byte[]?[]> GetTexture()
        {
            int nbTextures = this.Skin.UvType != 3 ? 1 : 2;
            byte[]?[] textures = new byte[nbTextures][];

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("https://3d.cs.money")
            };

            for (int i = 1; i <= nbTextures; i++)
            {
                string url = "/images/texture/s2/" + this.Uuid + $"_component{i}_texture1.png";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    textures[i - 1] = await response.Content.ReadAsByteArrayAsync();
                else textures[i - 1] = null;
            }
            return textures;
        }
    }
}