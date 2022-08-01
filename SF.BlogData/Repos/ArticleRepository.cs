using Microsoft.EntityFrameworkCore;
using SF.BlogData.Models;


namespace SF.BlogData.Repository;

public class ArticleRepository : Repository<Article>
{
    public ArticleRepository(ApplicationDbContext db) : base(db)
    {

    }

    public async Task<Article> GetById(int id)
    {
        return await Set.Include(a => a.Author)
                        .Include(a => a.Tags)
                        .Include(a => a.Comments)
                        .ThenInclude(c => c.Author)
                        .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Article>> GetAllWithTags()
    {
        return await Set.Include(a => a.Author)
                        .Include(a => a.Tags)
                        .Include(a => a.Comments)
                        .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetAllByAuthor(int id)
    {
        return await Set.Include(a => a.Author)
                        .Include(a => a.Tags)
                        .Include(a => a.Comments)
                        .AsNoTracking()
                        .Where(a => a.AuthorId == id)
                        .ToListAsync();
    }

}
