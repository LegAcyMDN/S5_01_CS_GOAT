namespace S5_01_App_CS_GOAT.Services
{
    /// <summary>
    /// Defines a type/lookup entity with an ID and name
    /// </summary>
    public interface IType
    {
        int TypeId { get; }

        string TypeName { get; }
    }
}
