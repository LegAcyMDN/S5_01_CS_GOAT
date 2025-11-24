namespace S5_01_App_CS_GOAT.Services
{
    public interface IUserDependant
    {
        public int? DependantUserId { get; }

        public IEnumerable<IUserDependant> FilterByUser(IEnumerable<IUserDependant> collection, int userId)
        {
            return collection.Where(item => item.DependantUserId == userId);
        }
    }
}
