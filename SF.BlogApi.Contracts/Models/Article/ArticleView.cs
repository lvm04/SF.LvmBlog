using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.BlogApi.Contracts
{
    public class ArticleView
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string[] Tags { get; set; }
        public string[] Comments { get; set; }
    }
}
