namespace S5_01_App_CS_GOAT.Services
{
    /// <summary>
    /// Marks an entity as belonging to a specific user for access control purposes
    /// </summary>
    public interface IUserDependant
    {
        public int? DependantUserId { get; }
    }
}
