using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.Collections.Generic;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IFairRandomRepository : IDataRepository<FairRandom, int>
{
    Task<FairRandom> Init(int userId, bool requestUnresolved = false, bool save = true);

    Task<FairRandom> Init(User user, bool requestUnresolved = false, bool save = true);

    Task<FairRandom> Resolve(User user, FairRandom? random, bool requestUnresolved = true, bool save = false);

    Task<IEnumerable<FairRandom>> Chain(User user, FairRandom? init, int lenght, bool save = false);
}