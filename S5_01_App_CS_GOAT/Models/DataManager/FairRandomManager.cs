using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class FairRandomManager: CrudRepository<FairRandom, int>, IFairRandomRepository
    {
        protected readonly CSGOATDbContext _context;
        public FairRandomManager(CSGOATDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<FairRandom> Init(int userId, bool requestUnresolved = false)
        {
            User? user = await _context.Set<User>()
                .Include(u => u.FairRandom)
                    .ThenInclude(fr => fr.RandomTransaction)
                .Include(u => u.FairRandom)
                    .ThenInclude(fr => fr.UpgradeResult)
                        .ThenInclude(ur => ur.RandomTransaction)
                .FirstOrDefaultAsync(u => u.UserId == userId);
                
            if (user == null) throw new ArgumentException("User not found", nameof(userId));
            return await this.Init(user, requestUnresolved);
        }

        public async Task<FairRandom> Init(User user, bool requestUnresolved = false, FairRandom? init = null)
        {
            FairRandom? existing = user.FairRandom;
            if (existing != null)
            {
                if (!requestUnresolved || !existing.IsResolved) return existing;
                if (existing.GetRandomTransaction() == null)
                {
                    _context.Set<FairRandom>().Remove(existing);
                }
                else
                {
                    existing.UserId = null;
                    _context.Set<FairRandom>().Update(existing);
                }
            }
            string seed = init != null ? init.ServerSeed :
                SecurityService.GenerateSeed(16);
            string hash = init != null ? init.ServerHash :
                SecurityService.HashString(seed);
            FairRandom newFairRandom = new FairRandom()
            {
                UserId = user.UserId,
                ServerSeed = seed,
                ServerHash = hash
            };
            _context.Set<FairRandom>().Add(newFairRandom);
            await _context.SaveChangesAsync();
            return newFairRandom;
        }

        public async Task<FairRandom> Resolve(User user, FairRandom? random, bool requestUnresolved = true)
        {
            if (
                random == null
                || (random.IsResolved
                && requestUnresolved)
            ) random = await this.Init(user, requestUnresolved, random);
            if (!requestUnresolved && random.IsResolved) return random;
            random.UserSeed = user.Seed;
            random.UserNonce = user.Nonce;
            random.Compute();
            random.UserId = null;
            user.Nonce += 1;
            _context.Set<FairRandom>().Update(random);
            _context.Set<User>().Update(user);
            await _context.SaveChangesAsync();
            return random;
        }
    }
}
