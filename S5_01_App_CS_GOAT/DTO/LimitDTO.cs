namespace S5_01_App_CS_GOAT.DTO
{
    public class LimitDTO
    {
        public double LimitAmount { get; set; } = 0; // amount of money the user don't want to spent

        public string LimitTypeName { get; set; } = null!; // name of why the user want to limit is spending
    }
}
