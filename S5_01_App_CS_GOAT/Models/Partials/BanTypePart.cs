using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class BanType : IType
    {
        public int TypeId => BanTypeId;
        public string TypeName => BanTypeName;
    }
}