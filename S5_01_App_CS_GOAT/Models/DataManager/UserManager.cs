using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager;

public class UserManager : CrudRepository<User>, IDataRepository<User, int>
{
    public UserManager(CSGOATDbContext context) : base(context)
    { }

    

    // Returns users that are not soft-deleted. Applies basic filters and sorts if provided.
    public async Task<IEnumerable<User>> GetAllForAdminAsync(FilterOptions? filters = null, SortOptions? sorts = null)
    {
        IQueryable<User> query = _context.Set<User>().AsQueryable();
        query = query.Where(u => u.DeleteOn == null);

        // TODO: REMOVE
        throw new NotImplementedException();
        return await query.ToListAsync();
    }

    // Soft-delete: set DeleteOn instead of removing from DB
    public async Task SoftDeleteAsync(int id)
    {
        var user = await _context.Set<User>().FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.DeleteOn = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task PatchSeedAsync(int id, string seed)
    {
        var user = await _context.Set<User>().FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.Seed = seed;
        await _context.SaveChangesAsync();
    }

    public async Task PatchPasswordAsync(int id, string? salt, string hash)
    {
        var user = await _context.Set<User>().FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!string.IsNullOrEmpty(salt))
            user.SaltPassword = salt;

        user.HashPassword = hash;
        await _context.SaveChangesAsync();
    }

    // Simple verification example: if code matches seed, mark email verified. Real logic should be more robust.
    public async Task<bool> VerifyAsync(int id, string code)
    {
        throw new NotImplementedException();

        /*var user = await _context.Set<User>().FindAsync(id);
        if (user == null)
            return false;

        if (code == user.Seed)
        {
            user.EmailVerifiedOn = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;*/
    }

    public async Task<User> GetByKeyAsync(string login)
    {
        throw new NotImplementedException();
    }
}
