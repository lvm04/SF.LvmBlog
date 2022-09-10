using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.BlogApi.Contracts
{
    public class CommentView
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AuthorId { get; set; }
        public int ArticleId { get; set; }
        public string Text { get; set; }
    }
}
