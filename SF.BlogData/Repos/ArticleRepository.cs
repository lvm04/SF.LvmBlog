using Microsoft.EntityFrameworkCore;
using SF.BlogData;
using SF.BlogData.Models;
using System.Text;

namespace SF.BlogData.Repository;

public class ArticleRepository : Repository<Article>
{
    public ArticleRepository(ApplicationDbContext db) : base(db)
    {

    }

    Random rand = new Random(DateTime.Now.Millisecond);

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

    public async Task<IEnumerable<Article>> GetByAuthor(int id)
    {
        return await Set.Include(a => a.Author)
                        .Include(a => a.Tags)
                        .Include(a => a.Comments)
                        .AsNoTracking()
                        .Where(a => a.AuthorId == id)
                        .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetByText(string text)
    {
        return await Set.Include(a => a.Author)
                        .Include(a => a.Tags)
                        .Include(a => a.Comments)
                        .AsNoTracking()
                        .Where(a => EF.Functions.Like(a.Text, $"%{text}%") ||
                                    EF.Functions.Like(a.Title, $"%{text}%")
                        //.Where(a => EF.Functions.Like(a.Text.ToLower(), $"%{text.ToLower()}%") ||
                        //            EF.Functions.Like(a.Title.ToLower(), $"%{text.ToLower()}%")
                        )
                        .ToListAsync();

        // На SQLite функция lower() по умолчанию не работает с кириллицей. Необходима компиляция SQLite с опцией SQLITE_ENABLE_ICU 
    }

    public async Task<IEnumerable<Article>> GetByTag(string tagName)
    {
        return await Set.Include(a => a.Author)
                        .Include(a => a.Tags)
                        .Include(a => a.Comments)
                        .AsNoTracking()
                        .Where(a => a.Tags.Select(t => t.Name).Contains(tagName))
                        .ToListAsync();
    }

    /// <summary>
    /// Генерация случайных статей
    /// </summary>
    public async Task Generate(int amount)
    {
        var users = _db.Set<User>().Where(u => u.Id != 1).ToArray();
        var tags = _db.Set<Tag>().ToList();

        var sb = new StringBuilder(10000);
        var _articles = await Set.Where(a => a.Id < 7).AsNoTracking().ToListAsync();  
        _articles.ForEach(a => sb.Append(a.Text).Append(' '));
        string[] words = sb.ToString().Split(' ');
       

        for (int i = 0; i < amount; i++)
        {
            tags.Shuffle();
            var article = new Article
            {
                Author = users[rand.Next(0, users.Length - 1)],
                Title = GetRandomText(words, 2, 6),      //$"Заголовок {rand.Next(1, 1000000)}",
                Tags = tags.Take(rand.Next(1, 3)).ToList(),
                Text = GetRandomText(words, 100, 300),
                TimeStamp = DateTime.Now.AddDays(-rand.Next(1, (DateTime.Now - DateTime.Now.AddYears(-5)).Days)),
                NumberViews = rand.Next(1, 1000)
            };
            await Set.AddAsync(article);
        }
        await _db.SaveChangesAsync();
    }

    private string GetRandomText(string[] source, int minValue, int maxValue)
    {
        int length = rand.Next(minValue, maxValue);
        var sb = new StringBuilder(10000);

        for (int i = 0; i < length; i++)
        {
            sb.Append(source[rand.Next(0, source.Length)]).Append(' ');
        }
        return sb.ToString();
    }

    
}
