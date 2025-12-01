using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class LimitType : IType
    {
        public int TypeId => LimitTypeId;
        public string TypeName => LimitTypeName;
    }
}