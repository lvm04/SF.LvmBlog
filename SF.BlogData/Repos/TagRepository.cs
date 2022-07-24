using Microsoft.EntityFrameworkCore;
using SF.BlogData.Models;


namespace SF.BlogData.Repository;

public class TagRepository : Repository<Tag>
{
    public TagRepository(ApplicationDbContext db) : base(db)
    {

    }

    public async Task<Tag> GetByName(string name)
    {
        return await Set.FirstOrDefaultAsync(u => u.Name == name);
    }

}
