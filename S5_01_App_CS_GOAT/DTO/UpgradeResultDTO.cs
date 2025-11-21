namespace S5_01_App_CS_GOAT.DTO
{
    public class UpgradeResultDTO
    {
        public float FloatStart { get; set; }


        public float FloatEnd { get; set; }


        public double ProbIntact { get; set; }


        public double ProbDegrade { get; set; }


        public double PropDestroy { get; set; }


        public string DegradeFunction { get; set; } = null!;

        public int FairRandomId { get; set; }

        public int? UserIdUpgrade { get; set; }

        public int? WearIdUpgrade { get; set; }

        public int TransactionId { get; set; }


    }
}
