using Microsoft.EntityFrameworkCore;
using SF.BlogData.Models;


namespace SF.BlogData.Repository;

public class CommentRepository : Repository<Comment>
{
    public CommentRepository(ApplicationDbContext db) : base(db)
    {

    }

    public async Task<Comment> GetById(int id)
    {
        return await Set.Include(c => c.Author).Include(c => c.Article).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Comment>> GetByArticleId(int id)
    {
        return await Set.Include(c => c.Author).Include(c => c.Article)
                        .AsNoTracking().Where(c => c.ArticleId == id).ToListAsync();
    }

}
