namespace S5_01_App_CS_GOAT.DTO;

public class PriceHistoryDTO
{
    public DateTime PriceDate { get; set; }

    public double PriceValue { get; set; }

    public bool IsGuess { get; set; } // prediction for the AI to predict the future price
}