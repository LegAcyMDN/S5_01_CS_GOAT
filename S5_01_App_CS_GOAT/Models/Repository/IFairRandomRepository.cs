using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.Collections.Generic;

namespace S5_01_App_CS_GOAT.Models.Repository;

/// <summary>
/// Manages fair and provably fair random number generation for gaming operations
/// </summary>
/// <remarks>
/// Provably fair gaming allows users to verify that results were predetermined
/// without server manipulation. Uses server seeds, client seeds, and nonces for verification.
/// </remarks>
public interface IFairRandomRepository : IDataRepository<FairRandom, int>
{
    /// <summary>
    /// Initializes a new fair random session for the specified user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="requestUnresolved">If true, creates a new session even if one is already active</param>
    /// <returns>A new FairRandom session with generated server seed and hash</returns>
    Task<FairRandom> Init(int userId, bool requestUnresolved = false);

    /// <summary>
    /// Initializes a new fair random session for the specified user
    /// </summary>
    /// <param name="user">The user to initialize the session for</param>
    /// <param name="requestUnresolved">If true, creates a new session even if one is already active</param>
    /// <param name="init">Optional existing FairRandom to reuse seeds and hash from</param>
    /// <returns>A new FairRandom session with generated or provided server seed and hash</returns>
    Task<FairRandom> Init(User user, bool requestUnresolved = false, FairRandom? init = null);

    /// <summary>
    /// Resolves a fair random session by combining server seed, client seed, and nonce
    /// </summary>
    /// <param name="user">The user who initiated the session</param>
    /// <param name="random">The fair random session to resolve (or null to create new one)</param>
    /// <param name="requestUnresolved">If true, creates a new session if provided one is already resolved</param>
    /// <returns>The resolved FairRandom session with computed final seed</returns>
    Task<FairRandom> Resolve(User user, FairRandom? random, bool requestUnresolved = true);
}