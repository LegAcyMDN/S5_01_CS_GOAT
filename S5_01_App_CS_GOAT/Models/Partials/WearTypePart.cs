using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class WearType : IType
    {
        public int TypeId => WearTypeId;
        public string TypeName => WearTypeName;
    }
}