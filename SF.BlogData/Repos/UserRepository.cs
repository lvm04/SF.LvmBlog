using Microsoft.EntityFrameworkCore;
using SF.BlogData.Models;
using System.Globalization;

namespace SF.BlogData.Repository;

public class UserRepository : Repository<User>
{
    public UserRepository(ApplicationDbContext db) : base(db)
    {

    }

    public async Task<User> GetById(int id)
    {
        return await Set.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetByLogin(string login)
    {
        return await Set.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<IEnumerable<User>> GetAllWithRoles()
    {
        return await Set.Include(u => u.Roles).AsNoTracking().ToListAsync();
    }

    public async Task<User> CheckCredentials(string login, string password)
    {
        return await Set.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
    }

    public async Task<IEnumerable<User>> GetByText(string text)
    {
        var result = await Set.Include(u => u.Roles).AsNoTracking().ToListAsync();
        return result.Where(u => u.Login.ToLower().IndexOf(text.ToLower()) != -1 || u.Name.ToLower().IndexOf(text.ToLower()) != -1);
    }

}
