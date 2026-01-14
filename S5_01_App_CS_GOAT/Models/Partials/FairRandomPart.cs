using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    /// <summary>
    /// Partial class providing fair random computation and resolution methods
    /// </summary>
    public partial class FairRandom : IUserDependant
    {
        /// <summary>
        /// Gets the associated random transaction, either directly or through upgrade result
        /// </summary>
        /// <returns>The RandomTransaction if available; otherwise null</returns>
        public RandomTransaction? GetRandomTransaction()
        {
            return this.RandomTransaction ?? this.UpgradeResult?.RandomTransaction;
        }

        /// <summary>
        /// Gets a value indicating whether this fair random session has been resolved
        /// </summary>
        /// <remarks>
        /// A session is considered resolved when the user ID is null, indicating
        /// the server seed has been revealed and computation is complete.
        /// </remarks>
        public bool IsResolved => this.UserId == null;

        /// <summary>
        /// Computes the final random fractions by combining server seed, user seed, and nonce
        /// </summary>
        /// <remarks>
        /// This implements the provably fair algorithm:
        /// 1. Combines server seed + user seed + nonce
        /// 2. Hashes the combination with SHA256
        /// 3. Extracts two 32-bit unsigned integers from the hash
        /// 4. Converts them to fractions (0.0 to 1.0) for random selection
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if UserSeed or UserNonce are not set</exception>
        public void Compute()
        {
            if (this.Fraction1 != null && this.Fraction2 != null) return;
            if (this.UserSeed == null || this.UserNonce == null)
                throw new InvalidOperationException("Cannot compute FairRandom without UserSeed and UserNonce.");

            string combined = SecurityService.HashString(this.ServerSeed + this.UserSeed + this.UserNonce.ToString());
            this.CombinedHash = combined;
            byte[] hashBytes = Convert.FromBase64String(combined);

            uint intValue1 = BitConverter.ToUInt32(hashBytes, 0);
            this.Fraction1 = intValue1 / (double)uint.MaxValue;

            uint intValue2 = BitConverter.ToUInt32(hashBytes, 4);
            this.Fraction2 = intValue2 / (double)uint.MaxValue;
        }

        public int? DependantUserId { get => this.UserId ?? this.GetRandomTransaction()?.DependantUserId; }
    }
}