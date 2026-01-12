using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service dédié au mapping des résultats de cases
    /// Respecte le principe de responsabilité unique (SRP)
    /// </summary>
    public class CaseResultMapperService
    {
        /// <summary>
        /// Convertit les résultats multiples de cases en listes de skins pour l'animation
        /// </summary>
        public List<List<SkinDTO>> ConvertMultipleCaseResultsToSkinLists(MultipleCaseResultDTO multipleCaseResults)
        {
            List<List<SkinDTO>> casesWithSkins = new();
            int caseNumber = 0;

            foreach (var oneCase in multipleCaseResults.Results)
            {
                casesWithSkins.Add(new List<SkinDTO>());

                // Ajouter les 72 premiers skins
                for (int i = 0; i <= 71; i++)
                {
                    SkinDTO skinOfThisIteration = multipleCaseResults.Skins[oneCase.Roller[i]];
                    casesWithSkins[caseNumber].Add(skinOfThisIteration);
                }

                // Ajouter le skin gagné au milieu (position 72)
                InventoryItemDetailDTO wonSkinDetail = oneCase.Reward;
                casesWithSkins[caseNumber].Add(new SkinDTO
                {
                    AnyUuid = wonSkinDetail.Uuid,
                    ItemName = wonSkinDetail.ItemName,
                    RarityColor = wonSkinDetail.RarityColor,
                    RarityName = wonSkinDetail.RarityName,
                    SkinName = wonSkinDetail.SkinName
                });

                // Ajouter les 10 derniers skins (73-82)
                for (int i = 72; i < 82; i++)
                {
                    SkinDTO skinOfThisIteration = multipleCaseResults.Skins[oneCase.Roller[i]];
                    casesWithSkins[caseNumber].Add(skinOfThisIteration);
                }

                caseNumber++;
            }

            return casesWithSkins;
        }

        /// <summary>
        /// Extrait les skins gagnés des résultats de cases
        /// </summary>
        public List<InventoryItemDetailDTO> ExtractWonSkins(MultipleCaseResultDTO caseResults)
        {
            return caseResults.Results
                .Select(r => r.Reward)
                .ToList();
        }
    }
}
