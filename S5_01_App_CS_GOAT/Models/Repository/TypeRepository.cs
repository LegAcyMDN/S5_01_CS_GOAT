using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

public class TypeRepository<TEntity> : ITypeRepository<TEntity>
    where TEntity : class, IType
{
    private readonly CSGOATDbContext _context;

    public TypeRepository(CSGOATDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetTypeByNameAsync(string typeName)
    {
        return await _context.Set<TEntity>()
            .Where(t => t.TypeName == typeName)
            .FirstOrDefaultAsync();
    }
}