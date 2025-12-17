namespace S5_01_Blazor_CS_GOAT.Utils
{
    public static class StringHelper
    {
        /// <summary>
        /// Calcule la distance de Levenshtein entre deux chaînes
        /// </summary>
        public static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            int sourceLength = source.Length;
            int targetLength = target.Length;
            int[,] distance = new int[sourceLength + 1, targetLength + 1];

            // Initialisation de la première colonne et ligne
            for (int i = 0; i <= sourceLength; i++)
            {
                distance[i, 0] = i;
            }

            for (int j = 0; j <= targetLength; j++)
            {
                distance[0, j] = j;
            }

            // Calcul de la distance
            for (int i = 1; i <= sourceLength; i++)
            {
                for (int j = 1; j <= targetLength; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceLength, targetLength];
        }
    }
}
