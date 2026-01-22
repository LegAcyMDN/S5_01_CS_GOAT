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
        protected readonly IUserRepository _userRepository;

        public FairRandomManager(
            CSGOATDbContext context,
            IUserRepository userRepository
            ) : base(context)
        {
            _context = context;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Initializes or retrieves the fair random session for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="requestUnresolved">If true, returns unresolved sessions; if false, returns already resolved ones</param>
        /// <returns>The initialized or existing FairRandom session</returns>
        /// <remarks>
        /// If the user already has a fair random session and requestUnresolved is false, returns it as-is.
        /// Otherwise creates a new session with a new server seed and hash.
        /// </remarks>
        public async Task<FairRandom> Init(int userId, bool requestUnresolved = false)
        {
            QueryOptions<User> options = new QueryOptions<User>()
                .Before(u => u.FairRandom.RandomTransaction)
                .Before(u => u.FairRandom.UpgradeResult.RandomTransaction);
            User? user = await _userRepository.GetByIdAsync(userId, options);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }
            return await Init(user, requestUnresolved);
        }

        /// <summary>
        /// Initializes or reuses a fair random session for a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="requestUnresolved">Whether to request an unresolved session</param>
        /// <param name="init">Optional FairRandom to use as template for seeds/hashes</param>
        /// <returns>A new or existing FairRandom session</returns>
        /// <remarks>
        /// Handles cleanup of old sessions and creation of new ones as needed.
        /// </remarks>
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

        /// <summary>
        /// Resolves a fair random session by combining server seed, user seed, and nonce to compute final random value
        /// </summary>
        /// <param name="user">The user whose seeds will be used</param>
        /// <param name="oldRandom">The existing fair random session to resolve</param>
        /// <param name="requestUnresolved">Whether to create new unresolved sessions if needed</param>
        /// <returns>The resolved FairRandom with computed random values</returns>
        /// <remarks>
        /// This implements the provably fair algorithm by computing a hash of server seed + user seed + nonce.
        /// The result can be independently verified by the user.
        /// </remarks>
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
