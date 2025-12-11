using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class FairRandom : IUserDependant
    {
        public RandomTransaction? GetRandomTransaction()
        {
            return this.RandomTransaction ?? this.UpgradeResult?.RandomTransaction;
        }

        public bool IsResolved => this.UserId == null;

        public double Compute()
        {
            if (this.Fraction != null)
                return this.Fraction.Value;
            if (this.UserSeed == null || this.UserNonce == null)
                throw new InvalidOperationException("Cannot compute FairRandom without UserSeed and UserNonce.");

            string combined = SecurityService.HashString(this.ServerSeed + this.UserSeed + this.UserNonce.ToString());
            this.CombinedHash = combined;

            string first8 = combined.Substring(0, 8);
            uint intValue = Convert.ToUInt32(first8, 16);
            this.Fraction = intValue / (double)uint.MaxValue;
            return this.Fraction.Value;
        }

        public int? DependantUserId { get => this.UserId ?? this.GetRandomTransaction()?.DependantUserId; }
    }
}