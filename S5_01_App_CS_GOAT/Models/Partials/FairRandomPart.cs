using S5_01_App_CS_GOAT.Models.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class FairRandom : IUserDependant
    {
        public int? DependantUserId 
        { 
            get
            {
                if (this.UpgradeResult != null)
                {
                    return this.UpgradeResult.DependantUserId;
                }
                else if (this.RandomTransaction != null)
                {
                    return this.RandomTransaction.DependantUserId;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}