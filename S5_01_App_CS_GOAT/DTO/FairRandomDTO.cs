namespace S5_01_App_CS_GOAT.DTO;

public class FairRandomDTO
{
    public string ServerSeed { get; set; } = null!;

    public string ServerHash { get; set; } = null!;

    public int UserNonce { get; set; } // number of time the user open case and/or upgrade his item
  
    public string CombinedHash { get; set; } = null!; // serverseed + userseed + usernonce

    public double Fraction { get; set; } // combinedhash transform between 0 and 1

    public int TransactionId { get; set; }
}
