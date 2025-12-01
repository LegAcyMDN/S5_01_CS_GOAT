using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class NotificationType : IType
    {
        public int TypeId => NotificationTypeId;
        public string TypeName => NotificationTypeName;
    }
}