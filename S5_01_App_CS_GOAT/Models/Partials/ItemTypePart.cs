using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class ItemType : IType
    {
        public int TypeId => ItemTypeId;
        public string TypeName => ItemTypeName;
    }
}