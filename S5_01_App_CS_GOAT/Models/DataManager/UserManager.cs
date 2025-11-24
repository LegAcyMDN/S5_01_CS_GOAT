using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager;

public class UserManager : CrudRepository<User>, IVerifiableRepository<User>
{
    public UserManager(CSGOATDbContext context) : base(context)
    { }

    public override async Task<User?> GetByKeyAsync(string key)
    {
        // Search by login or email
        return await _context.Set<User>().FirstOrDefaultAsync(u => u.Login == key || u.Email == key);
    }

    // Returns users that are not soft-deleted. Applies basic filters and sorts if provided.
    public async Task<IEnumerable<User>> GetAllForAdminAsync(FilterOptions? filters = null, SortOptions? sorts = null)
    {
        IQueryable<User> query = _context.Set<User>().AsQueryable();
        query = query.Where(u => u.DeleteOn == null);

        if (filters?.Filters != null)
        {
            foreach (var f in filters.Filters)
            {
                if (f.Key.Equals("login", StringComparison.OrdinalIgnoreCase) && f.Value is string login)
                    query = query.Where(u => u.Login.Contains(login));
                if (f.Key.Equals("email", StringComparison.OrdinalIgnoreCase) && f.Value is string email)
                    query = query.Where(u => u.Email.Contains(email));
                if (f.Key.Equals("displayname", StringComparison.OrdinalIgnoreCase) && f.Value is string dn)
                    query = query.Where(u => u.DisplayName.Contains(dn));
            }

            if (filters.Page.HasValue && filters.PageSize.HasValue && filters.PageSize > 0)
            {
                int skip = (filters.Page.Value - 1) * filters.PageSize.Value;
                query = query.Skip(skip).Take(filters.PageSize.Value);
            }
        }

        if (sorts?.Sorts != null)
        {
            // Support a small set of sortable fields
            IOrderedQueryable<User>? ordered = null;
            foreach (var s in sorts.Sorts)
            {
                bool desc = s.Value?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                if (s.Key.Equals("creationdate", StringComparison.OrdinalIgnoreCase))
                    ordered = ordered == null ? (desc ? query.OrderByDescending(u => u.CreationDate) : query.OrderBy(u => u.CreationDate)) : (desc ? ordered.ThenByDescending(u => u.CreationDate) : ordered.ThenBy(u => u.CreationDate));
                if (s.Key.Equals("login", StringComparison.OrdinalIgnoreCase))
                    ordered = ordered == null ? (desc ? query.OrderByDescending(u => u.Login) : query.OrderBy(u => u.Login)) : (desc ? ordered.ThenByDescending(u => u.Login) : ordered.ThenBy(u => u.Login));
                if (s.Key.Equals("displayname", StringComparison.OrdinalIgnoreCase))
                    ordered = ordered == null ? (desc ? query.OrderByDescending(u => u.DisplayName) : query.OrderBy(u => u.DisplayName)) : (desc ? ordered.ThenByDescending(u => u.DisplayName) : ordered.ThenBy(u => u.DisplayName));
            }

            if (ordered != null)
                query = ordered;
        }

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
        var user = await _context.Set<User>().FindAsync(id);
        if (user == null)
            return false;

        if (code == user.Seed)
        {
            user.EmailVerifiedOn = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
