using System.Collections.Generic;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class Wear
    {
        public IEnumerable<PriceHistory> PriceHistories(bool allowGuess = false)
        {
            IEnumerable<PriceHistory>? histories = this.WearType?.PriceHistories?.Where(p => p.SkinId == this.SkinId);
            if (histories == null) return Enumerable.Empty<PriceHistory>();
            if (!allowGuess) histories = histories.Where(p => p.GuessDate == null);
            return histories;
        }

        public PriceHistory? LastPrice(bool allowGuess = false)
        {
            return this.PriceHistories(allowGuess)
                       .OrderByDescending(p => p.PriceDate)
                       .FirstOrDefault();
        }

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