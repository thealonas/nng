using System.Globalization;
using nng_api.Exceptions;
using Redis.OM;

namespace nng.Services;

public class IndexerCreationService<T>
{
    private readonly string _indexName;
    private readonly RedisConnectionProvider _provider;

    public IndexerCreationService(RedisConnectionProvider provider, string indexName)
    {
        _indexName = indexName;
        _provider = provider;
    }

    public async Task CreateIndexIfNeeded()
    {
        if ((await GetIndexes()).Any(x => x == _indexName))
            throw new IndexHasBeenAlreadyCreated();
        await _provider.Connection.CreateIndexAsync(typeof(T));
    }

    private async Task<IEnumerable<string>> GetIndexes()
    {
        return (await _provider.Connection.ExecuteAsync("FT._LIST"))
            .ToArray().Select(x => x.ToString(CultureInfo.CurrentCulture));
    }
}
