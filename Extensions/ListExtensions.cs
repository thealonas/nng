namespace nng.Extensions;

public static class ListExtensions
{
    public static List<List<T>> TakeBy<T>(this IEnumerable<T> source, int number)
    {
        return source.Select((x, i) => new {i, x})
            .GroupBy(x => x.i / number).Select(x => x.Select(y => y.x).ToList()).ToList();
    }
}
