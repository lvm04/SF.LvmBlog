using Microsoft.EntityFrameworkCore;
using SF.BlogData.Models;


namespace SF.BlogData.Repository;

public class UserRepository : Repository<User>
{
    public UserRepository(ApplicationDbContext db) : base(db)
    {

    }

    public async Task<User> GetByLogin(string login)
    {
        return await Set.FirstOrDefaultAsync(u => u.Login == login);
    }


}
