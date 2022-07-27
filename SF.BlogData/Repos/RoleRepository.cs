using Microsoft.EntityFrameworkCore;
using SF.BlogData.Models;


namespace SF.BlogData.Repository;

public class RoleRepository : Repository<Role>
{
    public RoleRepository(ApplicationDbContext db) : base(db)
    {

    }

    public async Task<Role> GetByName(string name)
    {
        return await Set.FirstOrDefaultAsync(u => u.Name == name);
    }

}
