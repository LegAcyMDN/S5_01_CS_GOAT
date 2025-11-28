namespace S5_01_App_CS_GOAT.DTO
{
    public class UpgradeResultDTO
    {
        public float FloatStart { get; set; } // actual float of the wear before the upgrade

        public float FloatEnd { get; set; } // float of the wear after the upgrade

        public double ProbIntact { get; set; } // probability of the wear to be intact during the upgrade

        public double ProbDegrade { get; set; }  // probability of the wear to be degraded during the upgrade

        public double PropDestroy { get; set; } // probability of the wear to be destroyed during the upgrade

        public string DegradeFunction { get; set; } = null!; // name of the function that was used
    }
}
