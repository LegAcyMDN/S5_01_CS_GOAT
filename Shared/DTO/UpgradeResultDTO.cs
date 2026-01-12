using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO
{
    public class UpgradeResultDTO : IQueryableDTO
    {
        public static string? DefaultSortKey => null;

        public static SortingType? DefaultSortType => null;

        public static int DefaultPageSize => 25;

        public static bool CanSearch => false;

        public string? SearchTerm => null;


        public float FloatStart { get; set; } // actual float of the wear before the upgrade

        public float FloatEnd { get; set; } // float of the wear after the upgrade

        public double ProbIntact { get; set; } // probability of the wear to be intact during the upgrade

        public double ProbDegrade { get; set; }  // probability of the wear to be degraded during the upgrade

        public double PropDestroy { get; set; } // probability of the wear to be destroyed during the upgrade

        public string DegradeFunction { get; set; } = null!; // name of the function that was used
    }
}
