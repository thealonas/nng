using Redis.OM.Searching;

namespace nng.Extensions;

public static class RedisCollectionExtensions
{
    public static bool TryGetById<T>(this IRedisCollection<T> collection, string id, out T data)
    {
        try
        {
            data = collection.FindById(id) ?? throw new InvalidOperationException();
            return true;
        }
        catch (Exception)
        {
            data = default!;
            return false;
        }
    }
}
