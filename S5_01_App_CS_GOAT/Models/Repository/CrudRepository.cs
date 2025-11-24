namespace S5_01_App_CS_GOAT.Models.Repository
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using S5_01_App_CS_GOAT.Models.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class CrudRepository<TEntity> : IDataRepository<TEntity, int, string> where TEntity : class
    {
        protected readonly CSGOATDbContext _context;

        public CrudRepository(CSGOATDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity?>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public abstract Task<TEntity> GetByKeyAsync(string str);

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
        {
            _context.Set<TEntity>().Attach(entityToUpdate);
            _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        public async Task PatchAsync(TEntity entityToUpdate, IDictionary<string, object> updates)
        {
            _context.Set<TEntity>().Attach(entityToUpdate);
            EntityEntry<TEntity> entry = _context.Entry(entityToUpdate);

            foreach (KeyValuePair<string, object> update in updates)
            {
                PropertyEntry property = entry.Property(update.Key);
                if (property != null)
                {
                    property.CurrentValue = update.Value;
                    property.IsModified = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}