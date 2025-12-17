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

        private static bool IsHexChar(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        public int? DependantUserId { get => this.UserId ?? this.GetRandomTransaction()?.DependantUserId; }
    }
}