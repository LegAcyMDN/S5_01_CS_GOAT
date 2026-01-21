using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

public class TypeRepository<TEntity> :
    ReadRepository<TEntity, int>,
    ITypeRepository<TEntity>
    where TEntity : class, IType
{
    private new readonly CSGOATDbContext _context;

    public TypeRepository(CSGOATDbContext context) : base(context)
    {
        _context = context;
    }

    public TEntity? GetTypeByName(string typeName)
    {
        return _context.Set<TEntity>().ToList()
            .Where(t => t.TypeName == typeName)
            .FirstOrDefault();
    }
}