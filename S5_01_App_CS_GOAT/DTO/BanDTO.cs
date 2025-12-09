namespace S5_01_App_CS_GOAT.DTO;

public class BanDTO
{
    public int BanId { get; set; }

    public string BanReason { get; set; } = null!;

    public DateTime BanDate { get; set; } // start day of the ban

    public int BanDuration { get; set; } //  day duration of the ban

    public string BanTypeName { get; set; } = null!;

    public string BanTypeDescription { get; set; } = null!;
}
