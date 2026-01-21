using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    /// <summary>
    /// Partial class providing user dependency implementation for InventoryItem entities
    /// </summary>
    public partial class InventoryItem : IUserDependant
    {
        /// <summary>
        /// Gets the user ID that owns this inventory item
        /// </summary>
        public int? DependantUserId => UserId;
    }
}