using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO;

public class FairRandomDTO : IQueryableDTO
{
    public static string? DefaultSortKey => "UserNonce";

    public static SortingType? DefaultSortType => SortingType.Ascending;

    public static int DefaultPageSize => 25;

    public static bool CanSearch => true;

    public string? SearchTerm => ServerHash;


    public string ServerSeed { get; set; } = null!;

    public string ServerHash { get; set; } = null!;

    public string UserSeed { get; set; } = null!;

    public int UserNonce { get; set; } // number of time the user open case and/or upgrade his item

    public string CombinedHash { get; set; } = null!; // serverseed + userseed + usernonce

    public double Fraction1 { get; set; } // combinedhash transform between 0 and 1

    public double Fraction2 { get; set; } // combinedhash transform between 0 and 1

    public int TransactionId { get; set; }
}
