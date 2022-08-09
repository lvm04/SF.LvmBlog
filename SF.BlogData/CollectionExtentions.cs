using SF.BlogData.Models;
using SF.BlogData.Repository;

namespace SF.BlogData;

public static class CollectionExtentions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var rand = new Random(DateTime.Now.Millisecond);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
