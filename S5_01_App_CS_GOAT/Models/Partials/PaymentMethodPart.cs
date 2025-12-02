using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class PaymentMethod : IType
    {
        public int TypeId => PaymentMethodId;
        public string TypeName => PaymentMethodName;
    }
}