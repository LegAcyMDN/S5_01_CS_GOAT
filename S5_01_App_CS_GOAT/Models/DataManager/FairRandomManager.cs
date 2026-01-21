using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Manages provably fair random number generation for gaming operations
    /// </summary>
    /// <remarks>
    /// Implements the provably fair system by combining server seeds, client seeds, and nonces
    /// to generate verifiable random outcomes that players can independently verify.
    /// </remarks>
    public class FairRandomManager : CrudRepository<FairRandom, int>, IFairRandomRepository
    {
        protected new readonly CSGOATDbContext _context;
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

            return user == null ? throw new ArgumentException("User not found", nameof(userId)) : await Init(user, requestUnresolved);
        }

        public async Task<FairRandom> Init(User user, bool requestUnresolved = false, FairRandom? init = null)
        {
            FairRandom? existing = user.FairRandom;
            if (existing != null)
            {
                if (!requestUnresolved || !existing.IsResolved)
                {
                    return existing;
                }

                if (existing.GetRandomTransaction() == null)
                {
                    _ = _context.Set<FairRandom>().Remove(existing);
                }
                else
                {
                    existing.UserId = null;
                    _ = _context.Set<FairRandom>().Update(existing);
                }
            }
            string seed = init != null ? init.ServerSeed :
                SecurityService.GenerateSeed(16);
            string hash = init != null ? init.ServerHash :
                SecurityService.HashString(seed);
            var newFairRandom = new FairRandom()
            {
                UserId = user.UserId,
                ServerSeed = seed,
                ServerHash = hash
            };
            _ = _context.Set<FairRandom>().Add(newFairRandom);
            _ = await _context.SaveChangesAsync();
            return newFairRandom;
        }

        public async Task<FairRandom> Resolve(User user, FairRandom? oldRandom, bool requestUnresolved = true)
        {
            FairRandom? newRandom;
            if (
                oldRandom == null
                || (oldRandom.IsResolved
                && requestUnresolved)
            )
            {
                if (oldRandom != null && !requestUnresolved && oldRandom.IsResolved)
                {
                    return oldRandom;
                }
                else
                {
                    newRandom = await Init(user, requestUnresolved, oldRandom);
                }
            }
            else
            {
                newRandom = oldRandom;
            }

            if (!requestUnresolved && newRandom.IsResolved)
            {
                return newRandom;
            }

            newRandom.UserSeed = user.Seed;
            newRandom.UserNonce = user.Nonce;
            newRandom.Compute();
            newRandom.UserId = null;
            user.Nonce += 1;
            _ = await _context.SaveChangesAsync();
            _ = _context.Set<FairRandom>().Update(newRandom);
            _ = _context.Set<User>().Update(user);
            _ = await _context.SaveChangesAsync();
            return newRandom;
        }
    }
}
